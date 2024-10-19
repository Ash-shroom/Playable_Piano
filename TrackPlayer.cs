using ABC;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Playable_Piano
{
    public class Note
    {
        public int Pitch;
        public int Duration;

        private Dictionary<ABC.Pitch, int> NoteToPitchMap = new Dictionary<ABC.Pitch, int>
        {
            {ABC.Pitch.A0, 900},
            {ABC.Pitch.B0, 1100},

            {ABC.Pitch.C1, 0},
            {ABC.Pitch.D1, 200},
            {ABC.Pitch.E1, 400},
            {ABC.Pitch.F1, 500},
            {ABC.Pitch.G1, 700},
            {ABC.Pitch.A1, 900},
            {ABC.Pitch.B1, 1100},

            {ABC.Pitch.C2, 0},
            {ABC.Pitch.D2, 200},
            {ABC.Pitch.E2, 400},
            {ABC.Pitch.F2, 500},
            {ABC.Pitch.G2, 700},
            {ABC.Pitch.A2, 900},
            {ABC.Pitch.B2, 1100},

            {ABC.Pitch.C3, 0},
            {ABC.Pitch.D3, 200},
            {ABC.Pitch.E3, 400},
            {ABC.Pitch.F3, 500},
            {ABC.Pitch.G3, 700},
            {ABC.Pitch.A3, 900},
            {ABC.Pitch.B3, 1100},

            {ABC.Pitch.C4, 0},
            {ABC.Pitch.D4, 200},
            {ABC.Pitch.E4, 400},
            {ABC.Pitch.F4, 500},
            {ABC.Pitch.G4, 700},
            {ABC.Pitch.A4, 900},
            {ABC.Pitch.B4, 1100},

            {ABC.Pitch.C5, 0},
            {ABC.Pitch.D5, 200},
            {ABC.Pitch.E5, 400},
            {ABC.Pitch.F5, 500},
            {ABC.Pitch.G5, 700},
            {ABC.Pitch.A5, 900},
            {ABC.Pitch.B5, 1100},

            {ABC.Pitch.C6, 1200},
            {ABC.Pitch.D6, 1400},
            {ABC.Pitch.E6, 1600},
            {ABC.Pitch.F6, 1700},
            {ABC.Pitch.G6, 1900},
            {ABC.Pitch.A6, 2100},
            {ABC.Pitch.B6, 2300},

            {ABC.Pitch.C7, 2400},
            {ABC.Pitch.D7, 1400},
            {ABC.Pitch.E7, 1600},
            {ABC.Pitch.F7, 1700},
            {ABC.Pitch.G7, 1900},
            {ABC.Pitch.A7, 2100},
            {ABC.Pitch.B7, 2300},

            {ABC.Pitch.C8, 2400}
        };
        private Dictionary<ABC.Accidental, int> AccidentalToPitchMap = new Dictionary<Accidental, int>
        {
            {ABC.Accidental.Unspecified, 0},
            {ABC.Accidental.Natural, 0},
            {ABC.Accidental.Sharp, 100},
            {ABC.Accidental.Flat, -100}
        };
        private Dictionary<ABC.Length, int> LenghtMap = new Dictionary<Length, int>
        {
            {ABC.Length.Unknown, 0},
            {ABC.Length.Sixteenth, 3},
            {ABC.Length.Eighth, 7},
            {ABC.Length.Quarter, 15},
            {ABC.Length.Half, 30},
            {ABC.Length.Whole, 60}
        };
        public Note(ABC.Note noteItem)
        {
            Pitch = NoteToPitchMap[noteItem.pitch] + AccidentalToPitchMap[noteItem.accidental];
            Duration = 2* LenghtMap[noteItem.length] + 2*(int) Math.Ceiling((float)(LenghtMap[noteItem.length] / 2 ) * noteItem.dotCount);
        }
        public Note(){
            //Invalid Note for marking the end
            Pitch = -100;
            Duration = -100;
        }
    }
    public class TrackPlayer
    {
        public string title;

        private List<Note> notation;
        private int currentNote;
        private int remainingDuration = 0;


        public TrackPlayer(Tune tune)
        {
            this.title = tune.title;
            List<Note> notes = new List<Note>();

            // convert ABC Notation into a List of Notes
            foreach (ABC.Item item in tune.voices[0].items)
            {
                if (item.GetType() == typeof(ABC.Note))
                {
                    ABC.Note NoteItem = (ABC.Note)item;
                    notes.Add(new Note(NoteItem));
                }
                else if (item.GetType() == typeof(ABC.Chord))
                {
                    ABC.Chord chordItem = (ABC.Chord)item;
                    foreach (ABC.Chord.Element chordNote in chordItem.notes.Take(chordItem.notes.Length - 1))
                    {
                        // duration == 0 thus notes get played at the same time, the last note determines the lenght of the chord
                        ABC.Note zeroLengthChordNote= new ABC.Note(chordNote.pitch, ABC.Length.Unknown, chordNote.accidental, 0);
                        notes.Add(new Note(zeroLengthChordNote));
                    }
                    ABC.Chord.Element lastChordElement = chordItem.notes[chordItem.notes.Length];
                    ABC.Note lastChordNote = new ABC.Note(lastChordElement.pitch,chordItem.length,lastChordElement.accidental,chordItem.dotCount);
                    notes.Add(new Note(lastChordNote));
                }
            }
            
            // Final terminating Note
            notes.Add(new Note());
            this.currentNote = 0;
            this.notation = notes;

        }

        /// <summary>
        /// Gets the nextNote from the currently playing track
        /// </summary>
        /// <returns> A Note Object when the next Note is supposed to be played, 
        /// or null if the last note still hasn't finished playing
        /// </returns>
        public Note? GetNextNote()
        {
            if (remainingDuration == 0)
            {
                remainingDuration = this.notation[currentNote].Duration;
                Note nextNote = this.notation[currentNote++];
                return nextNote;
            }
            else
            {
                remainingDuration--;
                return null;
            }
        }
    }
}
