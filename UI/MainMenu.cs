using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using System.Runtime.CompilerServices;
using System.Reflection.Emit;
using Force.DeepCloner;
using StardewModdingAPI;

namespace Playable_Piano.UI
{

    internal sealed class MainMenu : IClickableMenu, IBaseUI
    {

        const int BUTTONWIDTH = 200;
        const int BUTTONHEIGHT = 50;
        const int BUTTONMARGIN = 10;
        ClickableComponent freePlayButton = new ClickableComponent(new Rectangle(Game1.viewport.Width/2, Game1.viewport.Height/2, BUTTONWIDTH, BUTTONHEIGHT), "FreeplayButton", "Freeplay");
        ClickableComponent? trackPlayButton = new ClickableComponent(new Rectangle(Game1.viewport.Width/2, Game1.viewport.Height/2 + 2 * BUTTONHEIGHT, BUTTONWIDTH, BUTTONHEIGHT), "TrackSelectionButton", "Play Track");
        private string sound;
        private PlayablePiano mainMod;

        public MainMenu(PlayablePiano mod, string sound)
        {
            this.mainMod = mod;
            this.sound = sound;
        }

        //128 384 Sprite Pos Music note

        public override void draw(SpriteBatch b)
        {
            int xPos = Game1.viewport.Width / 2 - BUTTONWIDTH / 2;
            int yPos = Game1.viewport.Height / 2 - 2 * BUTTONHEIGHT;
            drawButtons(b, xPos, yPos);

            UIUtil.drawExitInstructions(b, "main");
            //ClickableComponent freePlayButton = new ClickableComponent(new Rectangle(xPos + 10, yPos + 10, 100, 50), "freeplayButton", "Button");
            drawMouse(b);
        }
        


        private void drawButtons(SpriteBatch b, int xPos, int yPos)
        {
            ClickableComponent freePlayButton = new ClickableComponent(new Rectangle(xPos, yPos, BUTTONWIDTH, BUTTONHEIGHT), "FreeplayButton", "Freeplay");
            ClickableComponent trackPlayButton = new ClickableComponent(new Rectangle(xPos, yPos + 2 * BUTTONHEIGHT, BUTTONWIDTH, BUTTONHEIGHT), "TrackSelectionButton", "Play Track");
            
            // Button Background
            Utility.DrawSquare(b, freePlayButton.bounds, 5, UIUtil.borderColor, UIUtil.backgroundColor);
            Utility.DrawSquare(b, trackPlayButton.bounds, 5, UIUtil.borderColor, UIUtil.backgroundColor);

            // Button Text
            Utility.drawTextWithShadow(b, freePlayButton.label, Game1.dialogueFont, new Vector2(freePlayButton.bounds.X + BUTTONMARGIN, freePlayButton.bounds.Y + BUTTONMARGIN), Game1.textColor);
            Utility.drawTextWithShadow(b, trackPlayButton.label, Game1.dialogueFont, new Vector2(trackPlayButton.bounds.X + BUTTONMARGIN, trackPlayButton.bounds.Y + BUTTONMARGIN), Game1.textColor);
        }
        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (freePlayButton.containsPoint(x,y))
            {
                exitThisMenu();
                FreePlayUI menu = new FreePlayUI(mainMod, sound);
                Game1.activeClickableMenu = ;
            }
            else if (trackPlayButton.containsPoint(x,y))
            {
            }
        }

        public void handleButton(SButton button)
        {
        }

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
            exitThisMenu();
            mainMod.setActiveMenu(null);
        }
    }
}

