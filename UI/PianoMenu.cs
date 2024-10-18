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

namespace Playable_Piano.UI
{
    internal sealed class PianoMenu : IClickableMenu
    {
        int ButtonWidth = 200;
        int ButtonHeight = 50;
        PlayablePiano mainMod;
        private List<ClickableComponent> Buttons;
        bool currentlyOpen = false;

        public PianoMenu(PlayablePiano mainMod)
        {
            this.mainMod = mainMod;
            Buttons = new List<ClickableComponent>();
        }

        //128 384 Sprite Pos Music note

        public override void draw(SpriteBatch b)
        {
            int xPos = Game1.viewport.Width / 2 - ButtonWidth / 2;
            int yPos = Game1.viewport.Height / 2 - 2 * ButtonHeight;
            setupUI(xPos, yPos);
            //Game1.drawDialogueBox(xPos, yPos, menuWidth, menuHeight, false, true);
            drawButtons(b);

            Utility.DrawSquare(b, new Rectangle(5, 5, Game1.viewport.Width - 10, 70), 5, new Color(91, 43, 42, 255), new Color(249, 186, 102, 255));
            Utility.drawBoldText(b, "Press Right Mousebutton to close this menu", Game1.smallFont, new Vector2(20, 20), Color.Black);
            //ClickableComponent freePlayButton = new ClickableComponent(new Rectangle(xPos + 10, yPos + 10, 100, 50), "freeplayButton", "Button");
            drawMouse(b);
            currentlyOpen = true;
        }
        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            foreach (ClickableComponent button in Buttons)
            {
                if (button.containsPoint(x, y))
                {
                    mainMod.handleUIButtonPress(button.name);
                    currentlyOpen = false;
                    exitThisMenu();
                }
            }
        }
        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
            if (currentlyOpen)
            {
                mainMod.handleUIButtonPress("MenuClose");
                currentlyOpen = false;
                exitThisMenu();
            }
        }

        private void setupUI(int xPos, int yPos)
        {
            Buttons.Clear();
            Buttons.Add(new ClickableComponent(new Rectangle(xPos, yPos, ButtonWidth, ButtonHeight), "FreeplayButton", "Freeplay"));
            Buttons.Add(new ClickableComponent(new Rectangle(xPos, yPos + 2 * ButtonHeight, ButtonWidth, ButtonHeight), "TrackplayButton", "Play Track"));
        }

        private void drawButtons(SpriteBatch b)
        {
            foreach (ClickableComponent button in Buttons)
            {
                Rectangle buttonBackgroundBounds = button.bounds.ShallowClone();
                buttonBackgroundBounds.X -= 10;
                buttonBackgroundBounds.Y -= 10;
                buttonBackgroundBounds.Height += 20;
                buttonBackgroundBounds.Width += 20;
                Utility.DrawSquare(b, buttonBackgroundBounds, 5, new Color(91, 43, 42, 255), new Color(249, 186, 102, 255));
                Utility.drawTextWithShadow(b, button.label, Game1.dialogueFont, new Vector2(button.bounds.X, button.bounds.Y), Game1.textColor);
            }
        }
    }
}

