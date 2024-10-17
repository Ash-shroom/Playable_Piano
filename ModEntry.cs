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

namespace Playable_Piano
{
    enum Notes : int
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
        Performance
    }

    internal sealed class PlayablePiano : Mod
    {
        private Dictionary<string, string>? instrumentSoundData;
        internal State currentState = State.None;
        

        public override void Entry(IModHelper helper)
        {
            this.instrumentSoundData = helper.ReadConfig<ModConfig>().InstrumentData;
            if (this.instrumentSoundData == null)
            {
                this.Monitor.Log("Could not load Instrument Data, check whether the config file exists.", LogLevel.Error);
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
                GameLocation location = Game1.currentLocation;
                Farmer player = Game1.player;
                string? tile_name;

                // getObjectAtTile returns null when called in the middle of sitting down/standing up
                tile_name = location.getObjectAtTile((int)player.Tile.X, (int)player.Tile.Y, true).Name;

                if (tile_name == null)
                {
                    return;
                }

                string sound;

                // get Instrument Sound, implicitly check if player is sitting at a Piano or other instrument
                try
                {
                    // On ModEntry instrumentSoundData gets checked for a null value
                    // if it is, the Mod doesn't load, the Null Dereference warning can thus be ignored
                    sound = this.instrumentSoundData[tile_name];
                }
                catch (KeyNotFoundException)
                {
                    this.Monitor.Log($"No Sounddata found for '{tile_name}' check the mod's config file", LogLevel.Debug);
                    return;
                }

                // Player is sitting at a piano and Sound Data has been found

                PianoMenu Menu = new PianoMenu(this);

                Game1.activeClickableMenu = Menu;

                Notes played_note;
                if (Notes.TryParse(e.Button.ToString(), out played_note))
                {
                    this.Helper.Input.Suppress(e.Button);
                    int pitch = (int)played_note;
                    location.playSound(sound, player.Tile, pitch);
                }

            }

        }
    }
}

