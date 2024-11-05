using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using MidiParser;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Audio;
using StardewValley.GameData;
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
        private string soundLow;
        private string soundHigh;
        protected override PlayablePiano mainMod { get; set; }

        public PlaybackUI(PlayablePiano mod, string fileName, int trackNumber)
        {
            this.mainMod = mod;
            this.sound = mainMod.sound;
            this.soundLow = mainMod.soundLow;
            this.soundHigh = mainMod.soundHigh;
            MidiFile midiFile = new MidiFile(Path.Combine(mainMod.Helper.DirectoryPath, "assets", "songs", fileName));
            List<Note> notes = new MidiConverter(midiFile, trackNumber).convertToNotes();

            // if lower or upper Octaves don't exist, convert ranges to base range
            foreach (Note note in notes)
            {
                note.octave = ((note.octave == Octave.low && !mainMod.lowerOctaves) || (note.octave == Octave.high && !mainMod.upperOctaves)) ? Octave.normal : note.octave;
            }
            songPlayer = new TrackPlayer(notes);
            mainMod.Helper.Events.GameLoop.UpdateTicking += playSong;
            Game1.stopMusicTrack(MusicContext.Default);
        }


        private void playSong(object? sender, UpdateTickingEventArgs e)
        {
            foreach (Note playedNote in songPlayer.GetNextNote())
            {
                if (playedNote.pitch >= 0)
                {
                    string playedSoundCue = sound;
                    switch (playedNote.octave)
                    {
                        case (Octave.normal):
                            break;
                        case (Octave.low):
                            playedSoundCue = soundLow;
                            break;
                        case (Octave.high):
                            playedSoundCue = soundHigh;
                            break;
                    }
                    // RPC controlled sounds have auto pitch, non controlled have to be set manually
                    // TODO: check with new audio engine in custom audio affected by pitch
                    if (!Game1.soundBank.GetCue(playedSoundCue).IsPitchBeingControlledByRPC)
                    {
                        Game1.soundBank.GetCueDefinition(playedSoundCue).sounds.First<XactSoundBankSound>().pitch = (playedNote.pitch - 1200) / 1200f;
                    }
                    Game1.currentLocation.playSound(playedSoundCue, Game1.player.Tile, playedNote.pitch);


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
