using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;
using Microsoft.Xna.Framework;

namespace Playable_Piano.UI
{
    public static class UIUtil
    {
        public static Color backgroundColor = new Color(249, 186, 102, 255);
        public static Color borderColor = new Color(91, 43, 42, 255);
        public static Color selectionColor = new Color(251, 207, 149, 255);

        public static void drawExitInstructions(SpriteBatch b, string menu = "")
        {
            Utility.DrawSquare(b, new Rectangle(5, 5, Game1.viewport.Width - 10, 70), 5, borderColor, backgroundColor);
            if (menu == "main")
            {
                Utility.drawBoldText(b, "Press Escape or Right Mousebutton to close this menu", Game1.smallFont, new Vector2(20, 20), Color.Black);
            }
            else
            {
                Utility.drawBoldText(b, "Press Escape or Right Mousebutton to return to the Mode Selection", Game1.smallFont, new Vector2(20, 20), Color.Black);
            }
        }
    }
}
