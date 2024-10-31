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
using Microsoft.Xna.Framework.Audio;
using System.Diagnostics.Metrics;
using StardewValley.Objects;

namespace Playable_Piano
{
    internal sealed class PlayablePiano : Mod
    {
        private Dictionary<string, string> instrumentSoundData = new Dictionary<string, string>();
        private bool atPiano = false;
        private BaseUI? activeMenu;
        public string sound = "toyPiano"; // Fallback Option
        public string soundLow = "toyPianoLow";
        public string soundHigh = "toyPianoHigh";
        public bool lowerOctaves = false;
        public bool upperOctaves = false;



        public override void Entry(IModHelper helper)
        {
            this.instrumentSoundData = helper.ReadConfig<ModConfig>().InstrumentData;

            if (this.instrumentSoundData == null)
            {
                this.Monitor.Log("Could not load Instrument Data, check whether the Mods config.json exists and file permissions. Using default config", LogLevel.Warn);
                this.instrumentSoundData = new Dictionary<string, string>{{"Dark Piano", "toyPiano"}, {"UprightPiano", "toyPiano"}};
            }
            loadInstrumentSounds();

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
                        sound = instrumentSoundData[tile_name];
                        soundLow = sound + "Low";
                        soundHigh = sound + "High";

                        lowerOctaves = Game1.soundBank.Exists(soundLow);
                        upperOctaves = Game1.soundBank.Exists(soundHigh);
                            
                        // open main Piano Menu
                        MainMenu pianoMenu = new MainMenu(this);
                        activeMenu = pianoMenu;
                        Game1.activeClickableMenu = pianoMenu;
                        atPiano = true;
                    }
                    else
                    {
                        this.Monitor.LogOnce($"No Instrument data found for '{tile_name}'. If it's supposed to have sound check the mod's config file", LogLevel.Debug);
                        return;
                    }
                }
            }
            else
            {
                atPiano = false;
            }

        }

        public void setActiveMenu(BaseUI? newMenu)
        {
            this.activeMenu = newMenu;
            if (newMenu is not null)
            {
                Game1.activeClickableMenu = newMenu;
            }
        }


        private bool loadSoundData(string soundName)
        {
            try
            {
                SoundEffect audio;
                string audioPath = Path.Combine(Helper.DirectoryPath, "sounds", soundName + ".wav");
                using (var stream = new FileStream(audioPath, FileMode.Open))
                {
                    audio = SoundEffect.FromStream(stream);
                }
                CueDefinition cueDef = new CueDefinition(soundName, audio, Game1.audioEngine.GetCategoryIndex("Sound"));
                Game1.soundBank.AddCue(cueDef);
                return true;
            }
            catch
            {
                Monitor.Log($"Couldn't load {soundName}.wav", LogLevel.Trace);
                return false;
            }
        }

        private void loadInstrumentSounds()
        {
            foreach (var entry in instrumentSoundData)
            {
                var instrument = entry.Value;
                if (!Game1.soundBank.Exists(instrument))
                {
                    if (loadSoundData(instrument))
                    {
                        Monitor.Log($"loaded sound: {instrument}", LogLevel.Debug);
                    }
                    else
                    {
                        Monitor.Log($"Couldn't load {instrument} for {entry.Key}. Skipping Entry", LogLevel.Warn);
                        continue;
                    }
                }
                if (!Game1.soundBank.Exists(instrument + "Low"))
                {
                    if (loadSoundData(instrument + "Low"))
                    {
                        Monitor.Log($"  loaded lower range for {instrument}", LogLevel.Debug);
                    }
                }
                if (!Game1.soundBank.Exists(instrument + "High"))
                {
                    if (loadSoundData(instrument + "High"))
                    {
                        Monitor.Log($"  loaded upper range for {instrument}", LogLevel.Debug);
                    }
                }
            }
        }
    }
}

