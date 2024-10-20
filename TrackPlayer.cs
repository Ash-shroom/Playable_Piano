using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Playable_Piano
{
   
    public class TrackPlayer
    {
        private List<Note> notation;
        private int currentNote = 0;
        private int currentTick = 0;


        public TrackPlayer(List<Note> notes)
        {
            this.notation = notes;
        }

        /// <summary>
        /// Gets the nextNote from the currently playing track
        /// </summary>
        /// <returns> A Note Object when the next Note is supposed to be played, 
        /// or null if the last note still hasn't finished playing
        /// </returns>
        public Note? GetNextNote()
        {
            Note? note = null;
            if (currentTick == notation[currentNote].gameTick)
            {
                note = notation[currentNote];
                currentNote++;
            }
            currentTick++;
            return note;
        }
    }
}
