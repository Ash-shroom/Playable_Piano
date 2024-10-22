using StardewModdingAPI;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playable_Piano.UI
{
    internal abstract class BaseUI : IClickableMenu
    {
        protected PlayablePiano mainMod 
        { 
            get
            {
                return mainMod;
            }
            set
            {
                mainMod = value;
            }
        }
        protected string sound 
        {
            get
            {
                return sound;
            }
            set
            {
                sound = value;
            }
        }
        public abstract void handleButton(SButton button);
    }
}
