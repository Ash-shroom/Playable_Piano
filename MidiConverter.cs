using MidiParser;
using StardewValley.Menus;
using System.Runtime.ConstrainedExecution;
namespace Playable_Piano
{
    public class MidiConverter
    {
        private MidiFile midiFile;
        private int mainTrackNumber;
        private int TicksPerQuarterNote;


        public MidiConverter(MidiFile midiFile, int mainTrackNumber )
        {
            this.midiFile = midiFile;
            this.mainTrackNumber = mainTrackNumber;
            TicksPerQuarterNote = midiFile.TicksPerQuarterNote;
        }

        public List<Note> convertToNotes()
        {
            List<Note> notes = new List<Note>();
            // extract tempo data
            // (Tick, BPM) Tuples
            List<(int, int)> BPMIntervals = new List<(int, int)>();
            foreach (MidiTrack track in midiFile.Tracks)
            {
                foreach (MidiEvent midiEvent in track.MidiEvents)
                {
                    if (midiEvent.MetaEventType == MetaEventType.Tempo)
                    {
                        BPMIntervals.Add((midiEvent.Time, calculateTickRatio(midiEvent.Arg2)));
                    }
                }
            }
            // in case Tempo changes are spread out between tracks
            BPMIntervals.Sort((x,y) => {if (x.Item1 < y.Item1) {return -1;} else if (x.Item1 > y.Item1) {return 1;} else {return 0;}});
            int currentBPMInterval = 0;
            int midiTicksPerGameTick;
            if (BPMIntervals.Count > 0)
            {
                midiTicksPerGameTick = BPMIntervals[0].Item2;
            } 
            else
            {
                midiTicksPerGameTick = calculateTickRatio(120);
            }

            // extract note data
            if (mainTrackNumber == -1) // all tracks
            {
                foreach (MidiTrack track in midiFile.Tracks)
                {
                    foreach (MidiEvent midiEvent in track.MidiEvents) 
                    {
                        if (midiEvent.MidiEventType == MidiEventType.NoteOn)
                        {
                            // if current Note in new BPM Interval         AND note is played after next Interval starts
                            if (currentBPMInterval + 1 < BPMIntervals.Count && BPMIntervals[currentBPMInterval+1].Item1 < midiEvent.Time)
                            {
                                currentBPMInterval++;
                                midiTicksPerGameTick = BPMIntervals[currentBPMInterval].Item2;
                            }
                            notes.Add(new Note(midiEvent.Arg2, midiEvent.Time / midiTicksPerGameTick));
                            /*
                            Console.WriteLine("MidiTime: " + midiEvent.Time);
                            Console.WriteLine("Ratio: " + midiTicksPerGameTick);
                            Console.WriteLine("Converted Time: " + (midiEvent.Time / midiTicksPerGameTick).ToString());
                            Console.WriteLine("");
                            */
                        }
                    }
                }
            }
            else
            {
                // one specified track
                foreach (MidiEvent midiEvent in midiFile.Tracks[mainTrackNumber].MidiEvents)
                {
                    if (midiEvent.MidiEventType == MidiEventType.NoteOn)
                    {
                        // if current Note in new BPM Interval         AND note is played after next Interval starts
                        if (currentBPMInterval + 1 < BPMIntervals.Count && BPMIntervals[currentBPMInterval+1].Item1 < midiEvent.Time)
                        {
                            currentBPMInterval++;
                            midiTicksPerGameTick = BPMIntervals[currentBPMInterval].Item2;
                        }
                        notes.Add(new Note(midiEvent.Arg2, midiEvent.Time / midiTicksPerGameTick));
                        /*
                        Console.WriteLine("MidiTime: " + midiEvent.Time);
                        Console.WriteLine("Ratio: " + midiTicksPerGameTick);
                        Console.WriteLine("Converted Time: " + (midiEvent.Time / midiTicksPerGameTick).ToString());
                        Console.WriteLine("");
                        */
                    }
                }
            }
            notes.Sort((x, y) => { if (x.gameTick < y.gameTick) { return -1; } else if (x.gameTick > y.gameTick) { return 1; } else { return 0; } });

            // negative Note Pitch marks end of song,
            // during Playback for each played Note the Index gets incremented, and checked if it should be played (chords)
            // this causes an IndexOutOfBound on the last note, when only one End Note is existant
            notes.Add(new Note(-200,notes.Last().gameTick + 10));
            notes.Add(new Note(-200, notes.Last().gameTick + 10));
            return notes;
        }

        private int calculateTickRatio(int BPM)
        {
            int ratio = BPM * TicksPerQuarterNote / 3600;
            return ratio;
        }

    }

}