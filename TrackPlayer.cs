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
        public string title;

        private List<Note> notation;
        private int currentNote;
        private int remainingDuration = 0;


        public TrackPlayer(string title, List<Note> notes)
        {
            this.title = title;
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
            if (remainingDuration == 0)
            {
                remainingDuration = this.notation[currentNote].duration;
                Note nextNote = this.notation[currentNote++];
                return nextNote;
            }
            else
            {
                remainingDuration--;
                return null;
            }
        }
    }
}
