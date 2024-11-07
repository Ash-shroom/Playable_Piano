using StardewModdingAPI.Events;
using StardewModdingAPI;
using StardewValley;
using Playable_Piano.UI;
using Microsoft.Xna.Framework.Audio;
using StardewValley.Triggers;
using StardewValley.Delegates;
using System.Linq;

namespace Playable_Piano
{
    internal sealed class PlayablePiano : Mod
    {
        private Dictionary<string, string> instrumentSoundData = new Dictionary<string, string>();
        private BaseUI? activeMenu;
        public string sound = "Mushroomy.PlayablePiano_Piano"; 
        public string soundLow = "Mushroomy.PlayablePiano_PianoLow";
        public string soundHigh = "Mushroomy.PlayablePiano_PianoHigh";
        public bool lowerOctaves = false;
        public bool upperOctaves = false;



        public override void Entry(IModHelper helper)
        {            
            this.instrumentSoundData = helper.ReadConfig<ModConfig>().InstrumentData;
            TriggerActionManager.RegisterAction("Mushroomy.PlayablePiano_AddSound", this.addInstrument);
            TriggerActionManager.RegisterTrigger("Mushroomy.PlayablePiano_SaveLoaded");
            if (this.instrumentSoundData == null)
            {
                this.Monitor.Log("Could not load Instrument Data, check whether the Mods config.json exists and file permissions. Using default config", LogLevel.Warn);
                this.instrumentSoundData = new Dictionary<string, string>{{"Dark Piano", "Mushroomy.PlayablePiano_Piano"}, {"UprightPiano", "Mushroomy.PlayablePiano_Piano"}};
            }
            loadSounds();
            StardewValley.Object exmplItem = new StardewValley.Object();
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
            helper.Events.GameLoop.SaveLoaded += this.CPIntegration;
        }


        private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady)
            {
                return;
            }
            else if (activeMenu is not null && Game1.activeClickableMenu is not null) //activeMenu has handleButton Method, and is thus used instead of activeClickableMenu
            {
                activeMenu.handleButton(e.Button);
                return;
            }
            else if (Game1.activeClickableMenu is null && Game1.player.ActiveItem is not null && !Game1.player.ActiveItem.isPlaceable() && (e.Button.ToString() == "MouseLeft"))
            {
                string instrument = Game1.player.ActiveItem.Name;
                if (instrumentSoundData.ContainsKey(Game1.player.ActiveItem.Name))
                {
                    Helper.Input.Suppress(e.Button);
                    openInstrumentMenu(instrumentSoundData[instrument]);
                    return;
                }
            }
            else if (Game1.activeClickableMenu is null && Game1.player.IsSitting())
            {
                string input = e.Button.ToString();
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
                    openInstrumentMenu(instrumentSoundData[tile_name]);
                }
                else
                {
                    this.Monitor.LogOnce($"No Instrument data found for '{tile_name}'. If it's supposed to have sound check the mod's config file", LogLevel.Debug);
                    return;
                }
            }
            else
            {
                // if somehow Game1.activeClickableMenu got nulled, without activeMenu getting nulled, they get synced here
                activeMenu = null;
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

       
        public void CPIntegration(object? sender, SaveLoadedEventArgs e)
        {
            foreach (var action in TriggerActionManager.GetActionsForTrigger("Mushroomy.PlayablePiano_SaveLoaded"))
            {
                if (Game1.player.triggerActionsRun.Contains(action.Data.Id))
                {
                    Monitor.Log($"{action.Data.Id} has Marked its AddSound Action as applied, please notify the Mod's author to set 'MarkActionApplied' as false.", LogLevel.Debug);
                    Game1.player.triggerActionsRun.Remove(action.Data.Id);
                }
            }
            Monitor.Log("adding CP Instruments");
            TriggerActionManager.Raise("Mushroomy.PlayablePiano_SaveLoaded");
        }

        public bool addInstrument(string[] args, TriggerActionContext context, out string? error)
        {
            string instrumentName = args[1];
            string soundName = args[2];
            if (Game1.soundBank.Exists(soundName))
            {
                // if it can't be added, then the sound assignment has already happened in the config
                if (this.instrumentSoundData.TryAdd(instrumentName, soundName))
                {
                    Monitor.Log($"Added {instrumentName} with sound {soundName}");
                }
                error = null;
                return true;
            }
            else
            {
                error = $"sound {soundName} doesn't exist";
                return false;
            }
        }

        private void openInstrumentMenu(string soundName)
        {
            sound = soundName;
            soundLow = soundName + "Low";
            soundHigh = soundName + "High";
            lowerOctaves = Game1.soundBank.Exists(soundLow);
            upperOctaves = Game1.soundBank.Exists(soundHigh);

            // open main Piano Menu
            MainMenu pianoMenu = new MainMenu(this);
            activeMenu = pianoMenu;
            Game1.activeClickableMenu = pianoMenu;
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

        private void loadSounds()
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

        

        
    }
}

