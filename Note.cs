using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Playable_Piano
{
    public enum Octave
    {
        low,
        normal,
        high
    }
    public class Note
    {
        public int pitch;
        public int gameTick;
        public Octave octave;


        // C4 = 60; C5 = 72; C6 = 84; C7 = 96
        public Note(int midiNote, int Tick)
        {
            if (midiNote % 24 == 0 && midiNote != 72) // overlap between octaves, e.g. C7 should be 2400, but would be 0 in next calculation, thus filter it out 
            {
                this.pitch = 2400;
            }
            else
            {
                this.pitch = midiNote < 0 ? -200 : (midiNote % 24) * 100;
            }
            Console.WriteLine($"converted {midiNote} to {pitch}");
            this.gameTick = Tick;

            if (midiNote < 72)
            {
                this.octave = Octave.low;
            }
            else if (midiNote > 96)
            {
                this.octave = Octave.high;
            }
            else
            {
                this.octave = Octave.normal;
            }
        }

    }
}
