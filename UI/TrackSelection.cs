using StardewValley;
using StardewModdingAPI;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using MidiParser;

namespace Playable_Piano.UI
{
    internal class TrackSelection : IClickableMenu
    {
        private const int BORDERWIDTH = 5;
        private const int BORDERMARGIN = 5;
        private const int ENTRYHEIGHT = 40;
        private const int WINDOWMARGINX = 50;
        private const int WINDOWMARGINY = 100;
        private const int BUTTONSIZE = 32;


        PlayablePiano mainMod;
        private List<string> songList = new List<string>();
        private List<ClickableComponent> songSelection = new List<ClickableComponent>();
        private ClickableTextureComponent? prevButton;
        private ClickableTextureComponent? nextButton;
        private int pageNumber = 0;
        private int maxPageNumber = 0;




        public TrackSelection(PlayablePiano mainMod)
        {
            this.mainMod = mainMod;
            try
            {
                string songFolder = Path.Combine(mainMod.Helper.DirectoryPath, "songs");
                foreach (string song in Directory.GetFiles(songFolder, "*.mid"))
                {
                    this.songList.Add(Path.GetFileName(song));
                }
                this.songList.Sort();
            }
            catch
            {
                mainMod.Monitor.Log("Couldn't open songs folder", LogLevel.Error);
            }

        }

        public override void draw(SpriteBatch b)
        {   // 50 px Margin from Window
            int menuHeight = Game1.viewport.Height - WINDOWMARGINY - WINDOWMARGINX;
            int menuWidth = Game1.viewport.Width - 2*WINDOWMARGINX;

            UIUtil.drawExitInstructions(b);
            Utility.DrawSquare(b, new Rectangle(WINDOWMARGINX, WINDOWMARGINY, menuWidth, menuHeight), BORDERWIDTH, UIUtil.borderColor, UIUtil.backgroundColor);
            
            createSongSelection(menuWidth, menuHeight);
            drawSongSelection(b);

            drawNavigationButtons(b, menuWidth, menuHeight);

            drawMouse(b);
        }

        private void createSongSelection(int menuWidth, int menuHeight)
        {
            songSelection.Clear();
            int songCountThatFits = (menuHeight - (BUTTONSIZE + 2*BORDERWIDTH + 2*BORDERMARGIN)) / ENTRYHEIGHT;
            int entryWidth = menuWidth - 2*BORDERMARGIN - 2*BORDERWIDTH;
            maxPageNumber = songList.Count / songCountThatFits;
            for (int songNr = 0; songNr < songCountThatFits; songNr++)
            {
                if (songNr < songList.Count)
                {
                    string songName = songList[pageNumber * songCountThatFits + songNr];
                    ClickableComponent entry = new ClickableComponent(new Rectangle(WINDOWMARGINX + BORDERWIDTH + BORDERMARGIN, WINDOWMARGINY + BORDERWIDTH + BORDERMARGIN + songNr * ENTRYHEIGHT, entryWidth, ENTRYHEIGHT), songName, truncateString(songName, entryWidth));
                    songSelection.Add(entry);
                }
                else
                {
                    break;
                }
            }
        }


        private void drawSongSelection(SpriteBatch b)
        {
            foreach (ClickableComponent song in songSelection)
            {
                Color textColor = Color.Black;
                ICursorPosition cursorpos = mainMod.Helper.Input.GetCursorPosition();
                if (song.containsPoint((int) cursorpos.GetScaledScreenPixels().X, (int) cursorpos.GetScaledScreenPixels().Y))
                {
                    textColor = Color.Gray;
                    Utility.DrawSquare(b, song.bounds, 0, UIUtil.selectionColor, UIUtil.selectionColor);
                }
                Utility.drawBoldText(b, song.label, Game1.smallFont, new Vector2(song.bounds.X, song.bounds.Y), textColor);
            }
        }


        private void drawNavigationButtons(SpriteBatch b, int menuWidth, int menuHeight)
        {
            if (maxPageNumber != 0)
            {
                int centerX = WINDOWMARGINX + (menuWidth / 2);
                int centerY = WINDOWMARGINY + menuHeight - BUTTONSIZE - BORDERWIDTH - BORDERMARGIN;
                if (pageNumber != 0)
                {
                    Rectangle backButtonPosition = new Rectangle(centerX - BUTTONSIZE - (BUTTONSIZE / 2), centerY, BUTTONSIZE, BUTTONSIZE);
                    prevButton = new ClickableTextureComponent(backButtonPosition, Game1.content.Load<Texture2D>("LooseSprites\\Cursors"), new Rectangle(472, 96, 32, 32), 1);
                    prevButton.draw(b);
                }
                mainMod.Monitor.LogOnce("Page " + pageNumber.ToString());
                mainMod.Monitor.LogOnce("Max " + maxPageNumber.ToString());
                if (pageNumber != maxPageNumber)
                {
                    Rectangle nextButtonPosition = new Rectangle(centerX + BUTTONSIZE + (BUTTONSIZE / 2), centerY, BUTTONSIZE, BUTTONSIZE);
                    nextButton = new ClickableTextureComponent( nextButtonPosition, Game1.content.Load<Texture2D>("LooseSprites\\Cursors"), new Rectangle(448, 96, 32, 32), 1);
                    nextButton.draw(b);
                }
            }
        }


        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (prevButton is not null && prevButton.containsPoint(x,y)) 
            {
                pageNumber--;
            }
            else if (nextButton is not null && nextButton.containsPoint(x,y))
            {
                pageNumber++;
            }
            else
            {
                foreach (ClickableComponent song in songSelection)
                {
                    if (song.containsPoint(x, y))
                    {
                        MidiParser.MidiFile midiFile = new MidiParser.MidiFile(Path.Combine(mainMod.Helper.DirectoryPath, "songs", song.name));
                        List<int> tracksWithNotes = new List<int>();
                        foreach (MidiTrack midiTrack in midiFile.Tracks)
                        {
                            foreach (MidiEvent mEvent in midiTrack.MidiEvents)
                            {
                                if (mEvent.MidiEventType == MidiEventType.NoteOn)
                                {
                                    tracksWithNotes.Add(midiTrack.Index);
                                }
                                else
                                {
                                    continue;
                                }
                                break;
                            }
                        }
                        if (tracksWithNotes.Count > 1)
                        {
                            exitThisMenu();
                            Game1.activeClickableMenu = new MidiTrackSelectionWindow(mainMod, song.name, tracksWithNotes);
                        }
                        else
                        {
                            mainMod.handleUIButtonPress("PerformButton", tracksWithNotes[0], song.name);
                            exitThisMenu();
                            return;
                        }
                    }
                }
            }
        }



        /// <summary>
        /// checks if inputString fits into maxWidth, if not returns a truncated Version ending in "..."
        /// </summary>
        /// <param name="inputString"></param>
        /// <param name="maxWidth"></param>
        /// <returns></returns>
        private string truncateString(string inputString, int maxWidth)
        {
            if (Game1.smallFont.MeasureString(inputString).X <= maxWidth)
            {
                return inputString;
            }
            else
            {
                while (Game1.smallFont.MeasureString(inputString + "...").X > maxWidth)
                {
                    inputString = inputString.Substring(0, inputString.Length - 1);
                }
                return inputString + "...";
            }
        }
    }
}
