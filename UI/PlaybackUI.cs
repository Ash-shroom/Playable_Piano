using Microsoft.Xna.Framework.Graphics;
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
        protected override PlayablePiano mainMod { get; set; }

        public PlaybackUI(PlayablePiano mod, string fileName, int trackNumber)
        {
            this.mainMod = mod;
            this.sound = mainMod.sound;
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
                    Game1.currentLocation.playSound(sound, Game1.player.Tile, playedNote.pitch);
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
