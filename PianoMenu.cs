using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Playable_Piano
{
    internal sealed class PianoMenu : IClickableMenu
    {
        int menuWidth = Game1.viewport.Width / 2 ;
        int menuHeight = Game1.viewport.Height/ 2 ;
        PlayablePiano mainMod;

        public PianoMenu(PlayablePiano mainMod)
        {
            this.mainMod = mainMod;
        }

        ClickableComponent? freePlayButton;
        public override void draw(SpriteBatch b)
        {
            int xPos = Game1.viewport.Width / 4 ;
            int yPos = Game1.viewport.Height / 4 ;
            
            Game1.drawDialogueBox(xPos, yPos, menuWidth, menuHeight, false, true);
            Game1.drawDialogueBox(xPos + 30, yPos + 30, 250, 200, false, true);

            //ClickableComponent freePlayButton = new ClickableComponent(new Rectangle(xPos + 10, yPos + 10, 100, 50), "freeplayButton", "Button");
            freePlayButton = new ClickableComponent(new Rectangle(xPos + 30, yPos + 30, 250, 200), "freeplayButton", "Freeplay");
            this.drawMouse(b);
            //freeplayButton.draw(b);
            Utility.drawTextWithShadow(b, "Freeplay", Game1.smallFont, new Vector2(xPos + 70 , yPos + 150), Color.Black);
        }
        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (freePlayButton.containsPoint(x, y)) 
            {
                
            }
            Game1.player.StopSitting();
        }
    }
}

