using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Playable_Piano
{
    public class Note
    {
        public int pitch;
        public int gameTick;

        /*
        public Note(int midiNote, int Tick) 
        {
            if (midiNote < 72)
            {
                // C0--B4
                // gets flattend to C5-B5
                this.pitch = (midiNote % 12) * 100;
            }
            else if (midiNote == 96)
            {
                // C7
                this.pitch = 2400;
            } 
            else if (midiNote > 96)
            {
                // D7--G9
                // gets flattend to C6-B6
                this.pitch = (midiNote % 12 + 12) * 100;
            }
            else 
            {
                // C5--B6
                this.pitch = (midiNote % 24) * 100;
            }
            this.gameTick = Tick;
        }
        */

        public Note(int midiNote, int Tick)
        {
            if (midiNote == 96)
            {
                Console.WriteLine(midiNote.ToString());
                this.pitch = 2400;
            }
            else
            {
                this.pitch = midiNote < 0 ? -200 : (midiNote % 24) * 100;
            }
            this.gameTick = Tick;
        }

    }
}
