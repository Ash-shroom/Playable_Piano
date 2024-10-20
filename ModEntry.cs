using StardewModdingAPI.Events;
using StardewModdingAPI;
using StardewValley;
using xTile.Tiles;
using System.ComponentModel;
using xTile;
using xTile.Layers;
using xTile.Dimensions;
using System.Collections;
using StardewValley.Menus;
using Microsoft.Xna.Framework.Graphics;
using System.Drawing;
using Microsoft.Xna.Framework;
using xTile.Display;
using Playable_Piano.UI;
using System.Linq.Expressions;

namespace Playable_Piano
{
    enum ButtonToPitches : int
    {
        // lower Octave
        Z = 0,
        S = 100,
        X = 200,
        D = 300,
        C = 400,
        V = 500,
        G = 600,
        B = 700,
        H = 800,
        N = 900,
        J = 1000,
        M = 1100,

        // uppper Octave
        Q = 1200,
        D2 = 1300,
        W = 1400,
        D3 = 1500,
        E = 1600,
        R = 1700,
        D5 = 1800,
        T = 1900,
        D6 = 2000,
        Y = 2100,
        D7 = 2200,
        U = 2300,
        I = 2400
    }

   

    enum State
    {
        None,
        Menu,
        Freeplay,
        TrackSelection,
        Performance
    }

    internal sealed class PlayablePiano : Mod
    {
        private Dictionary<string, string>? instrumentSoundData;
        private State currentState = State.None;
        private string sound = "toyPiano";
        private PianoMenu? mainMenu;
        private TrackPlayer? trackPlayer;



        public override void Entry(IModHelper helper)
        {
            this.instrumentSoundData = helper.ReadConfig<ModConfig>().InstrumentData;
            if (this.instrumentSoundData == null)
            {
                this.Monitor.Log("Could not load Instrument Data, check whether the config file exists.", LogLevel.Error);
                return;
            }
            this.Monitor.Log($"Loaded Instruments:\n{string.Join("\n", this.instrumentSoundData.Select(pair => $"\t{pair.Key} : {pair.Value}"))}", LogLevel.Debug);
            mainMenu = new PianoMenu(this);
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
        }


        private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;
            if (Game1.player.IsSitting())
            {
                GameLocation location = Game1.currentLocation;
                Farmer player = Game1.player;
                string input = e.Button.ToString();
                switch (currentState)
                {
                    case (State.None):
                        // sat down at Piano
                        if (input == "MouseRight" || input == "Escape")
                        {
                            return;
                        }
                        string? tile_name;
                        try
                        {
                            // getObjectAtTile returns null when called in the middle of sitting down/standing up
                            tile_name = location.getObjectAtTile((int)player.Tile.X, (int)player.Tile.Y, true).Name;
                        }
                        catch (NullReferenceException)
                        {
                            return;
                        }



                        // check if Sound data exists for the instrument
                        // On ModEntry instrumentSoundData gets checked for a null value
                        // if it is, the Mod doesn't load, the Null Dereference warning can thus be ignored
                        if (instrumentSoundData.ContainsKey(tile_name))
                        {
                            sound = instrumentSoundData[tile_name];
                            Game1.activeClickableMenu = mainMenu;
                            currentState = State.Menu;
                            break;
                        }
                        else
                        {
                            this.Monitor.Log($"No Sounddata found for '{tile_name}'. If it's supposed to have sound check the mod's config file", LogLevel.Debug);
                            return;
                        }
                    case (State.TrackSelection):
                        if (input == "MouseRight" || input == "Escape")
                        {
                            this.Helper.Input.Suppress(e.Button);
                            Game1.activeClickableMenu = mainMenu;
                            currentState = State.Menu;
                        }
                        break;

                    case (State.Freeplay):
                        ButtonToPitches played_note;
                        this.Helper.Input.Suppress(e.Button);
                        if (ButtonToPitches.TryParse(input, out played_note))  
                        {
                            int pitch = (int)played_note;
                            location.playSound(sound, player.Tile, pitch);
                        }
                        else if (input == "MouseRight" || input == "Escape")
                        {
                            Game1.activeClickableMenu = mainMenu;
                            currentState = State.Menu;
                        }
                        break;
                        
                    case (State.Performance):
                        
                        this.Helper.Input.Suppress(e.Button);
                        if (input == "MouseRight" | input == "Escape")
                        {
                            this.Helper.Events.GameLoop.UpdateTicking -= PlaySong;
                            Game1.activeClickableMenu = mainMenu;
                            currentState = State.Menu;
                            break;
                        }       
                        break;

                    case (State.Menu):
                        // while in Menu do nothing
                        break;
                }
            }
            else
            {
                // when the player stands up, always return to default state None
                currentState = State.None;
            }

        }

        /// <summary>
        /// Plays the song until the last note is received. Last note is marked by a negative Pitch
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlaySong(object? sender, UpdateTickingEventArgs e)
        {
            foreach (Note playedNote in trackPlayer.GetNextNote())
            {
                this.Monitor.Log($"{playedNote.pitch} on Tick {playedNote.gameTick}");
                //Normal Note
                if (playedNote.pitch >= 0)
                {
                    Game1.currentLocation.playSound(sound, Game1.player.Tile, playedNote.pitch);
                }
                else
                {
                    // Song finished
                    this.Monitor.Log("finished");
                    this.Helper.Events.GameLoop.UpdateTicking -= this.PlaySong;
                    Game1.activeClickableMenu = mainMenu;
                    currentState = State.Menu;
                }
            }
        }


        internal void handleUIButtonPress(string buttonName, string trackName="", int trackNumber)
        {
            switch (buttonName)
            {
                case ("FreeplayButton"):
                    currentState = State.Freeplay;
                    Game1.activeClickableMenu = new FreePlayUI();
                    break;
                case ("TrackSelectionButton"):
                    currentState = State.TrackSelection;
                    Game1.activeClickableMenu = new TrackSelection(this);
                    break;
                case ("PerformButton"):
                    MidiParser.MidiFile midiFile = new MidiParser.MidiFile(Path.Combine(Helper.DirectoryPath, "songs", trackName));
                    if (midiFile.TracksCount > 1)
                    {
                        // TODO: TrackSelection Window
                        trackNumber = 1;
                    }
                    MidiConverter converter = new MidiConverter(midiFile, trackNumber);
                    List<Note> notes = converter.convertToNotes();
                    trackPlayer = new TrackPlayer(notes);
                    this.Helper.Events.GameLoop.UpdateTicking += PlaySong;
                    currentState = State.Performance;
                    Game1.activeClickableMenu = new PlaybackUI();
                    break;
                case ("MenuClose"):
                    currentState = State.None;
                    break;
            }
        }
    }
}

