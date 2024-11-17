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
        private BaseUI? activeMenu;
        private ModConfig config;
        public string sound = "Mushroomy.PlayablePiano_Piano"; 
        public string soundLow = "Mushroomy.PlayablePiano_PianoLow";
        public string soundHigh = "Mushroomy.PlayablePiano_PianoHigh";
        public bool lowerOctaves = false;
        public bool upperOctaves = false;
        private OnlinePlayer? onlinePlayer;



        #region public Methods
        public override void Entry(IModHelper helper)
        {
            config = helper.ReadConfig<ModConfig>();
            TriggerActionManager.RegisterAction("Mushroomy.PlayablePiano_AddSound", this.addInstrument);
            TriggerActionManager.RegisterTrigger("Mushroomy.PlayablePiano_SaveLoaded");
            if (config == null)
            {
                this.Monitor.Log("Could not load Instrument Data, check whether the Mods config.json exists and file permissions. Using default config", LogLevel.Warn);
                config = new ModConfig();
                config.InstrumentData = new Dictionary<string, string>{{"Dark Piano", "Mushroomy.PlayablePiano_Piano"}, {"UprightPiano", "Mushroomy.PlayablePiano_Piano"}};
                helper.WriteConfig<ModConfig>(config);
            }
            loadDefaultSounds();
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
            helper.Events.GameLoop.SaveLoaded += this.CPIntegration;
            helper.Events.Multiplayer.ModMessageReceived += this.receiveMessage;
        }



        /// <summary>
        /// sets activeMenu to the specified value. 
        /// </summary>
        /// <param name="newMenu"></param>
        public void setActiveMenu(BaseUI? newMenu)
        {
            this.activeMenu = newMenu;
            Monitor.Log($"activeMenu changed to {newMenu}");
            if (newMenu is not null)
            {
                Game1.activeClickableMenu = newMenu;
            }
        }

       
       /// <summary>
       /// Raises the SaveLoaded Trigger to Add Content Patcher instruments to the config.
       /// </summary>
       /// <param name="sender"></param>
       /// <param name="e"></param>
        public void CPIntegration(object? sender, SaveLoadedEventArgs e)
        {
            // check the config and attempt to load sounds not loaded yet, mostly a user's custom sounds
            // happens after the save loaded as all previously added instruments should have their sounds loaded by now
            checkConfigSounds();
            Monitor.Log("adding CP Instruments");
            TriggerActionManager.Raise("Mushroomy.PlayablePiano_SaveLoaded");
        }

        /// <summary>
        /// Adds a Content Packs instrument to the Config, but only if the sound exists.
        /// </summary>
        /// <param name="args"> Argument 1 is the instrument Name; Argument 2 is the soundName.</param>
        /// <param name="context"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public bool addInstrument(string[] args, TriggerActionContext context, out string? error)
        {
            string instrumentName = args[1];
            string soundName = args[2];
            if (Game1.soundBank.Exists(soundName))
            {
                if (config.InstrumentData.TryAdd(instrumentName, soundName))
                {
                    Monitor.Log($"Added {instrumentName} with sound {soundName}");
                    Helper.WriteConfig<ModConfig>(config);
                }
                error = null;
                return true;
            }
            else
            {
                error = $"sound {soundName} for {instrumentName} doesn't exist contact the Mod's author.";
                return false;
            }
        }

        public void receiveMessage(object? sender, ModMessageReceivedEventArgs e)
        {
            if (e.FromModID == this.ModManifest.UniqueID)
            {
                if (e.Type == "startPlayback")
                {
                    startPlayback message = e.ReadAs<startPlayback>();
                    onlinePlayer = new OnlinePlayer(this, message.performerTilePos, message.notation, message.sound);
                } 
                else if (e.Type == "stopPlayback" && onlinePlayer is not null)
                {
                    onlinePlayer.stopSong();
                    onlinePlayer = null;
                }
                else if (e.Type == "playNote")
                {
                    playNote message = e.ReadAs<playNote>();
                    string receivedSound = !Game1.soundBank.Exists(message.sound) ? message.sound : "toyPiano";
                    Game1.soundBank.GetCueDefinition(message.sound).sounds.First().pitch = (message.pitch - 1200) / 1200f;
                    Game1.currentLocation.localSound(message.sound, message.performerTile, message.pitch);
                }
            }
        }

        #endregion

        #region private Methods
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
                if (config.InstrumentData.ContainsKey(instrument))
                {
                    Helper.Input.Suppress(e.Button);
                    Monitor.Log($"started Playing on Instrument {instrument}");
                    openInstrumentMenu(config.InstrumentData[instrument]);
                    return;
                }
            }
            // no else if, due to the previous else if being entered, when sitting down and having an Active Item
            if (Game1.activeClickableMenu is null && Game1.player.IsSitting())
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
                if (config.InstrumentData.ContainsKey(tile_name))
                {
                    Monitor.Log($"Sat down at Instrument {tile_name}");
                    openInstrumentMenu(config.InstrumentData[tile_name]);
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

        /// <summary>
        /// creates and opens the Mod's main Menu, additionally sets the currently used sound to the specified value
        /// </summary>
        /// <param name="soundName">the Sound of the instrument</param>
        private void openInstrumentMenu(string soundName)
        {
            sound = soundName;
            soundLow = soundName + "Low";
            soundHigh = soundName + "High";
            lowerOctaves = Game1.soundBank.Exists(soundLow);
            upperOctaves = Game1.soundBank.Exists(soundHigh);
            Monitor.Log($"using sound {sound}");
            Monitor.Log($"extended Pitches lower: {lowerOctaves}, higher: {upperOctaves}");

            // open main Piano Menu
            MainMenu pianoMenu = new MainMenu(this);
            activeMenu = pianoMenu;
            Game1.activeClickableMenu = pianoMenu;
        }

        /// <summary>
        /// Loads a sound from the sounds folder, then creates and adds a SoundCue for it  
        /// </summary>
        /// <param name="soundName">The Name of the Cue and of the sound file</param>
        /// <returns></returns>
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
                Monitor.Log($"Couldn't load {soundName}.wav", LogLevel.Debug);
                return false;
            }
        }

        /// <summary>
        /// loads the default Sounds, which come bundled in with the mod. 
        /// </summary>
        private void loadDefaultSounds()
        {
            string[] defaultSounds = {"toyPianoLow", "toyPianoHigh", "fluteLow", "fluteHigh", "Mushroomy.PlayablePiano_Piano", "Mushroomy.PlayablePiano_PianoLow", "Mushroomy.PlayablePiano_PianoHigh"};
            foreach (string sound in defaultSounds)
            {
                loadSoundData(sound);
                Monitor.Log($"sound {sound} loaded", LogLevel.Debug);
            }
        }

        /// <summary>
        /// checks which sounds from the config are loaded, and attempts to load missing ones from assets/sounds.
        /// This usually only consist of Custom Sounds added by the user to override an existing sound.
        /// </summary>
        private void checkConfigSounds()
        {
            foreach (var entry in config.InstrumentData)
            {
                var sound = entry.Value;
                if (!Game1.soundBank.Exists(sound))
                {
                    if (!loadSoundData(sound))
                    {
                        Monitor.Log($"Couldn't load {sound} for {entry.Key}. Skipping Entry", LogLevel.Warn);
                        continue;
                    }
                    Monitor.Log($"sound {sound} loaded", LogLevel.Debug);
                }
                if (!Game1.soundBank.Exists(sound + "Low"))
                {
                    if (loadSoundData(sound + "Low"))
                    {
                        Monitor.Log($"  lower range for {sound} loaded", LogLevel.Debug);
                    }
                }
                else
                {
                    Monitor.Log($"  lower range for {sound} loaded", LogLevel.Debug);
                }
                if (!Game1.soundBank.Exists(sound + "High"))
                {
                    if (loadSoundData(sound + "High"))
                    {
                        Monitor.Log($"  upper range for {sound} loaded", LogLevel.Debug);
                    }
                }
                else
                {
                    Monitor.Log($"  upper range for {sound} loaded", LogLevel.Debug);
                }
            }
        }
        #endregion
    }
}

