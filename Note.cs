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


        public Note(int midiNote, int Tick)
        {
            if (midiNote == 96)
            {
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
