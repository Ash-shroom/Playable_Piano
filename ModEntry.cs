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
        private BaseUI? activeMenu;
        public string sound = "toyPiano"; // Fallback Option



        public override void Entry(IModHelper helper)
        {
            this.instrumentSoundData = helper.ReadConfig<ModConfig>().InstrumentData;
            SoundEffect lowPianoAudio, highPianoAudio;
            string lowPianoPath = Path.Combine(helper.DirectoryPath, "toyPianoLow.wav");
            string highPianoPath = Path.Combine(helper.DirectoryPath, "toyPianoHigh.wav");
            using (var stream = new System.IO.FileStream(lowPianoPath, System.IO.FileMode.Open))
            {
                lowPianoAudio = SoundEffect.FromStream(stream);
            }
            using (var stream = new System.IO.FileStream(highPianoPath, System.IO.FileMode.Open))
            {
                highPianoAudio = SoundEffect.FromStream(stream);
            }
            CueDefinition lowPianoCueDef = new CueDefinition("toyPianoLow", lowPianoAudio, Game1.audioEngine.GetCategoryIndex("Sound"));
            CueDefinition highPianoCueDef = new CueDefinition("toyPianoHigh", highPianoAudio, Game1.audioEngine.GetCategoryIndex("Sound"));
            Game1.soundBank.AddCue(lowPianoCueDef);
            Game1.soundBank.AddCue(highPianoCueDef);


            if (this.instrumentSoundData == null)
            {
                this.Monitor.Log("Could not load Instrument Data, check whether the Mods config.json exists and file permissions.Using fallback Option", LogLevel.Warn);
                this.instrumentSoundData = new Dictionary<string, string>{{"Dark Piano", "toyPiano"}, {"UprightPiano", "toyPiano"}};
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
                        // open main Piano Menu
                        sound = instrumentSoundData[tile_name];
                        MainMenu pianoMenu = new MainMenu(this);
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
    }
}

