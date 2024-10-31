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
        protected abstract PlayablePiano mainMod {
            get; set;
        }
        public abstract void handleButton(SButton button);
    }
}
