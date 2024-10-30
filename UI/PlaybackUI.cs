using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using MidiParser;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playable_Piano.UI
{
    internal class PlaybackUI : BaseUI
    {
        TrackPlayer songPlayer;
        private string sound;
        private string? soundLow;
        private string? soundHigh;
        protected override PlayablePiano mainMod { get; set; }

        public PlaybackUI(PlayablePiano mod, string fileName, int trackNumber)
        {
            this.mainMod = mod;
            this.sound = mainMod.sound;
            if (Game1.soundBank.Exists(sound + "Low") && Game1.soundBank.Exists(sound + "High"))
            {
                soundLow = sound + "Low";
                soundHigh = sound + "High";
            }
            MidiFile midiFile = new MidiFile(Path.Combine(mainMod.Helper.DirectoryPath, "songs", fileName));
            List<Note> notes = new MidiConverter(midiFile, trackNumber).convertToNotes();
            songPlayer = new TrackPlayer(notes);
            mainMod.Helper.Events.GameLoop.UpdateTicking += playSong;
        }


        private void playSong(object? sender, UpdateTickingEventArgs e)
        {
            foreach (Note playedNote in songPlayer.GetNextNote())
            {
                if (playedNote.pitch >= 0)
                {
                    switch (playedNote.octave)
                    {
                        case (Octave.normal):
                            mainMod.Monitor.Log($"playing normal {playedNote.pitch}");
                            Game1.currentLocation.playSound(sound, Game1.player.Tile, playedNote.pitch);
                            break;
                            // custom sounds aren't affected by the pitch of playSound
                            // to still get multiple pitches out of one wav, the CueDefinitions pitch gets adjusted
                            // the result sound worse than the base pitch, but is still passable
                        case (Octave.low):
                            mainMod.Monitor.Log($"playing low {playedNote.pitch}");
                            Game1.soundBank.GetCueDefinition(soundLow).sounds.First<XactSoundBankSound>().pitch = (playedNote.pitch - 1200) / 1200f;
                            Game1.currentLocation.playSound(soundLow, Game1.player.Tile, playedNote.pitch);
                            break;
                        case (Octave.high):
                            mainMod.Monitor.Log($"playing high {playedNote.pitch}");
                            Game1.soundBank.GetCueDefinition(soundHigh).sounds.First<XactSoundBankSound>().pitch = (playedNote.pitch - 1200) / 1200f;
                            Game1.currentLocation.playSound(soundHigh, Game1.player.Tile, playedNote.pitch);
                            break;
                    }
                    
                }
                else // Song finish marked by two invalid -200 Pitch notes
                {
                    mainMod.Monitor.Log("finished");
                    mainMod.Helper.Events.GameLoop.UpdateTicking -= playSong;
                    MainMenu menu = new MainMenu(mainMod);
                    mainMod.setActiveMenu(menu);
                }
            }
        }
        
        public override void draw(SpriteBatch b)
        {
            UIUtil.drawExitInstructions(b);
        }

        public override void handleButton(SButton button)
        {
            string input = button.ToString();
            if (input == "Escape" || input == "MouseRight")
            {
                mainMod.Helper.Events.GameLoop.UpdateTicking -= playSong;
                mainMod.Helper.Input.Suppress(button);
                exitThisMenu();
                TrackSelection menu = new TrackSelection(mainMod);
                mainMod.setActiveMenu(menu);
            }
        }

    }
}
