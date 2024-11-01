using StardewModdingAPI.Events;
using StardewModdingAPI;
using StardewValley;
using Playable_Piano.UI;
using Microsoft.Xna.Framework.Audio;
using StardewValley.Triggers;
using StardewValley.Delegates;

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
            TriggerActionManager.RegisterAction("Mushroomy.PlayablePiano_AddSound", this.addInstrument);
            TriggerActionManager.RegisterTrigger("Mushroomy.PlayablePiano_RegisterInstrument");
            if (this.instrumentSoundData == null)
            {
                this.Monitor.Log("Could not load Instrument Data, check whether the Mods config.json exists and file permissions. Using default config", LogLevel.Warn);
                this.instrumentSoundData = new Dictionary<string, string>{{"Dark Piano", "toyPiano"}, {"UprightPiano", "toyPiano"}};
            }
            loadInstrumentSounds();

            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
            helper.Events.GameLoop.SaveLoaded += this.CPIntegration;
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
                string audioPath = Path.Combine(Helper.DirectoryPath, "assets" ,"sounds", soundName + ".wav");
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
                var sound = entry.Value;
                if (!Game1.soundBank.Exists(sound))
                {
                    if (loadSoundData(sound))
                    {
                        Monitor.Log($"loaded sound: {sound}", LogLevel.Debug);
                    }
                    else
                    {
                        Monitor.Log($"Couldn't load {sound} for {entry.Key}. Skipping Entry", LogLevel.Warn);
                        continue;
                    }
                }
                if (!Game1.soundBank.Exists(sound + "Low"))
                {
                    if (loadSoundData(sound + "Low"))
                    {
                        Monitor.Log($"  loaded lower range for {sound}", LogLevel.Debug);
                    }
                }
                if (!Game1.soundBank.Exists(sound + "High"))
                {
                    if (loadSoundData(sound + "High"))
                    {
                        Monitor.Log($"  loaded upper range for {sound}", LogLevel.Debug);
                    }
                }
            }
        }
        public void CPIntegration(object? sender, SaveLoadedEventArgs e){
            Monitor.Log("Loading CP Instruments");
            TriggerActionManager.Raise("Mushroomy.PlayablePiano_RegisterInstrument");
        }

        public bool addInstrument(string[] args, TriggerActionContext context, out string? error)
        {
            string instrumentName = args[1];
            string soundName = args[2];
            if (Game1.soundBank.Exists(soundName))
            {
                this.instrumentSoundData.Add(instrumentName, soundName);
                Monitor.Log($"Added {instrumentName} with sound {soundName}");
                error = null;
                return true;
            }
            else
            {
                error = $"sound {soundName} doesn't exist";
                return false;
            }
        }
    }
}

