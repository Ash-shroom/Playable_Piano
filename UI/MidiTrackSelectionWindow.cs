using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using MidiParser;
using Microsoft.Xna.Framework.Media;
using StardewModdingAPI;


namespace Playable_Piano.UI
{
    internal class MidiTrackSelectionWindow : IClickableMenu
    {
        private const int BORDERWIDTH = 5;
        private const int BORDERMARGIN = 5;
        private const int ENTRYHEIGHT = 40;
        private const int WINDOWMARGINX = 50;
        private const int WINDOWMARGINY = 100;

        private PlayablePiano mainMod;
        private string trackName;
        private List<ClickableComponent> trackSelection = new List<ClickableComponent>();
        private List<int> tracksWithNotes;
        public MidiTrackSelectionWindow(PlayablePiano mainMod, string trackName, List<int> tracksWithNotes)
        {
            this.mainMod = mainMod;
            this.trackName = trackName;
            this.tracksWithNotes = tracksWithNotes;
        }


        public override void draw(SpriteBatch b)
        {
            UIUtil.drawExitInstructions(b);
            Utility.DrawSquare(b, new Rectangle(WINDOWMARGINX, WINDOWMARGINY, Game1.viewport.Width / 4, Game1.viewport.Height - WINDOWMARGINY * 2), BORDERWIDTH, UIUtil.borderColor, UIUtil.backgroundColor);
            string wrappedString = wrapString("This MIDI contains multiple Instrument Tracks, select one", (Game1.viewport.Width / 4) - 2* (BORDERWIDTH + BORDERMARGIN));
            Utility.drawBoldText(b, wrappedString, Game1.smallFont, new Vector2(WINDOWMARGINX + BORDERMARGIN + BORDERWIDTH, WINDOWMARGINY + BORDERMARGIN + BORDERWIDTH), Color.Black);
            trackSelection.Clear();
            foreach (int trackNumber in tracksWithNotes)
            {
                trackSelection.Add(new ClickableComponent(new Rectangle(WINDOWMARGINX + BORDERMARGIN + BORDERWIDTH, WINDOWMARGINY + BORDERMARGIN + BORDERWIDTH + (int) Game1.smallFont.MeasureString(wrappedString).Y + ENTRYHEIGHT * (trackNumber ), Game1.viewport.Width / 4 - 2 * (BORDERWIDTH + BORDERMARGIN), ENTRYHEIGHT),trackNumber.ToString(),$"Track {trackNumber.ToString()}"));
            }
            trackSelection.Add(new ClickableComponent(new Rectangle(WINDOWMARGINX + BORDERMARGIN + BORDERWIDTH, WINDOWMARGINY + BORDERMARGIN + BORDERWIDTH + (int) Game1.smallFont.MeasureString(wrappedString).Y + ENTRYHEIGHT * (trackSelection.Count ), Game1.viewport.Width / 4 - 2 * (BORDERWIDTH + BORDERMARGIN), ENTRYHEIGHT), "-1", "All Tracks"));
            foreach (ClickableComponent track in trackSelection)
            {
                ICursorPosition cursorpos = mainMod.Helper.Input.GetCursorPosition();
                Color textColor = Color.Black;
                if (track.containsPoint((int)cursorpos.GetScaledScreenPixels().X, (int)cursorpos.GetScaledScreenPixels().Y))
                {
                    Utility.DrawSquare(b, track.bounds, 0, UIUtil.selectionColor, UIUtil.selectionColor);
                    textColor = Color.Gray;
                }
                Utility.drawBoldText(b, track.label, Game1.smallFont, new Vector2(track.bounds.X, track.bounds.Y), textColor);
            }
            drawMouse(b);
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            foreach (ClickableComponent track in trackSelection)
            {
                if (track.containsPoint(x, y))
                {
                    mainMod.handleUIButtonPress("PerformButton", Int32.Parse(track.name), trackName);
                    exitThisMenu();
                    return;
                }
            }
        }

        private string wrapString(string text, int windowWidth)
        {
            string[] words = text.Split(" ");
            string wrappedString = "";
            foreach (string word in words)
            {
                if (Game1.smallFont.MeasureString(wrappedString.Split("\n").Last() + word).X > windowWidth)
                {
                    wrappedString = wrappedString + "\n" + word;
                }
                else
                {
                    wrappedString = wrappedString + " " + word;
                }
                mainMod.Monitor.LogOnce(Game1.smallFont.MeasureString(wrappedString).X.ToString());
            }
            return wrappedString.Remove(0, 1); // remove first whitespace;
        }


    }
}
