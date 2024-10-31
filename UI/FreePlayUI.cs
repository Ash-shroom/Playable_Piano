using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Sickhead.Engine.Util;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Audio;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Playable_Piano.UI
{
    internal class FreePlayUI : BaseUI
    {
        enum ButtonToPitches : int
        {
            // lower Octave
            Z = 0,
            S = 100,
            X = 200,
            D = 300,
            C = 400,
            V = 500,
            G = 600,
            B = 700,
            H = 800,
            N = 900,
            J = 1000,
            M = 1100,

            // uppper Octave
            Q = 1200,
            D2 = 1300,
            W = 1400,
            D3 = 1500,
            E = 1600,
            R = 1700,
            D5 = 1800,
            T = 1900,
            D6 = 2000,
            Y = 2100,
            D7 = 2200,
            U = 2300,

            I = 2400
        }

        private string sound;
        private string soundLow;
        private string soundHigh;
        private string selectedSoundCue;
        protected override PlayablePiano mainMod { get; set; }
        

        public FreePlayUI(PlayablePiano mod)
        {
            mainMod = mod;
            this.sound = mainMod.sound;
            this.soundLow = mainMod.soundLow;
            this.soundHigh = mainMod.soundHigh;
            this.selectedSoundCue = sound;

        }

        public override void draw(SpriteBatch b)
        {
            UIUtil.drawExitInstructions(b);
        }


        public override void  handleButton(SButton button)
        {
            mainMod.Helper.Input.Suppress(button);
            string input = button.ToString();
            ButtonToPitches playedNote;
            if (ButtonToPitches.TryParse(input, out playedNote))
            {
                int playedPitch = (int)playedNote;
                GameLocation location = Game1.currentLocation;
                Vector2 tileCords = Game1.player.Tile;
                if (!Game1.soundBank.GetCue(selectedSoundCue).IsPitchBeingControlledByRPC)
                {
                    Game1.soundBank.GetCueDefinition(selectedSoundCue).sounds.First().pitch = (playedPitch - 1200) / 1200f;
                }
                location.playSound(selectedSoundCue, tileCords, playedPitch);
                
            }
            else if (input == "LeftControl" && mainMod.lowerOctaves)
            {
                selectedSoundCue = soundLow;
            }
            else if (input == "LeftShift" && mainMod.upperOctaves)
            {
                selectedSoundCue = soundHigh;
            }
            else if (input == "Tab")
            {
                selectedSoundCue = sound;
            }
            else if (input == "MouseRight" || input == "Escape")
            {
                exitThisMenu();
                MainMenu menu = new MainMenu(mainMod);
                mainMod.setActiveMenu(menu);
            }
        }

    }
}
