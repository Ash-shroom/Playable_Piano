using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playable_Piano
{
    internal class FreePlayUI : IClickableMenu
    {
        public override void draw(SpriteBatch b)
        {
            Utility.DrawSquare(b, new Rectangle(5, 5, Game1.viewport.Width - 10, 70), 5, new Color(91, 43, 42, 255), new Color(249, 186, 102, 255));
            Utility.drawBoldText(b, "Press Escape or Right Mousebutton to exit Freeplay Mode", Game1.smallFont, new Vector2(20, 20), Microsoft.Xna.Framework.Color.Black);
        }
    }
}
