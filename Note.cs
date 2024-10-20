using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playable_Piano
{
    public class Note
    {
        public int pitch;
        public int duration;
        public Note(int pitch = -200, int duration = -200) 
        {
            this.pitch = pitch;
            this.duration = duration;
        }
    }
}
