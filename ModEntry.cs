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
        private Dictionary<string, string> instrumentSoundData = new Dictionary<string, string>();
        private bool atPiano = false;
        private IBaseUI? activeMenu;



        public override void Entry(IModHelper helper)
        {
            this.instrumentSoundData = helper.ReadConfig<ModConfig>().InstrumentData;
            if (this.instrumentSoundData == null)
            {
                this.Monitor.Log("Could not load Instrument Data, check whether the Mods config.json exists.", LogLevel.Error);
                return;
            }
            this.Monitor.Log($"Loaded Instruments:\n{string.Join("\n", this.instrumentSoundData.Select(pair => $"\t{pair.Key} : {pair.Value}"))}", LogLevel.Debug);
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
        }


        private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;
            if (Game1.player.IsSitting())
            {
                string input = e.Button.ToString();
                if (atPiano && activeMenu is not null)
                {
                    this.Helper.Input.Suppress(e.Button);
                    activeMenu.handleButton(e.Button);
                }
                else
                {
                    // Leaving Piano/Furniture without opening Menu
                    if (input == "MouseRight" || input == "Escape")
                    {
                        return;
                    }

                    GameLocation location = Game1.currentLocation;
                    Farmer player = Game1.player;

                    string tile_name;
                    // sat down at Piano
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
                    if (instrumentSoundData.ContainsKey(tile_name))
                    {
                        string sound = instrumentSoundData[tile_name];
                        MainMenu pianoMenu = new MainMenu(this, sound);
                        activeMenu = pianoMenu;
                        Game1.activeClickableMenu = pianoMenu;
                        atPiano = true;
                    }
                    else
                    {
                        this.Monitor.LogOnce($"No Sounddata found for '{tile_name}'. If it's supposed to have sound check the mod's config file", LogLevel.Debug);
                        return;
                    }
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
                        activeMenu.handleButton(e.Button);
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


        internal void handleUIButtonPress(string buttonName, int trackNumber = 0, string trackName = "")
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

        public void setActiveMenu(IBaseUI? newMenu)
        {
            this.activeMenu = newMenu;
        }
    }
}

