using ABC;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Playable_Piano
{
    internal class TrackPlayer
    {
        public readonly struct Note
        {
            public int Pitch { get; init; }
            public int Duration { get; init; }

            public Note(int pitch, int duration)
            {
                this.Pitch = pitch;
                this.Duration = duration;
            }
        }

        public string title;
        private List<Note> notation;
        private int currentNote;


        public TrackPlayer(Tune tune)
        {
            this.title = tune.title;
            List<Note> notes = new List<Note>();
            foreach (ABC.Item item in tune.voices[0].items)
            {
                if (item.GetType() == typeof(ABC.Note))
                {
                    ABC.Note NoteItem = (ABC.Note)item;
                    notes.Add(new Note((int)NoteItem.pitch + (int)NoteItem.accidental, (int)NoteItem.length));
                }
                else if (item.GetType() == typeof(ABC.Chord))
                {
                    ABC.Chord chordItem = (ABC.Chord)item;
                    foreach (ABC.Chord.Element chordNote in chordItem.notes.Take(chordItem.notes.Length - 1))
                    {
                        // duration == 0 thus notes get played at the same time, the last note determines the lenght of the chord
                        notes.Add(new Note((int)chordNote.pitch + (int)chordNote.accidental, 0));
                    }
                    ABC.Chord.Element lastChordNote = chordItem.notes[chordItem.notes.Length];
                    notes.Add(new Note((int)lastChordNote.pitch + (int)lastChordNote.accidental, (int)chordItem.length));
                }

                this.currentNote = 0;
                this.notation = notes;
            }

        }

        public Note GetNextNote()
        {
            return this.notation[currentNote++];
        }
    }
}
