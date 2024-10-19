using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playable_Piano.UI
{
    internal class FreePlayUI : IClickableMenu
    {
        public override void draw(SpriteBatch b)
        {
            UIUtil.drawExitInstructions(b);
        }
    }
}
