using MidiParser;
using StardewValley.Menus;
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
                    if (midiEvent.Type == (int)MetaEventType.Tempo)
                    {
                        BPMIntervals.Add((midiEvent.Time, calculateTickRatio(midiEvent.Arg2)));
                    }
                }
            }
            // in case Tempo changes are spread out between tracks
            BPMIntervals.Sort((x,y) => {if (x.Item1 < y.Item1) {return -1;} else if (x.Item1 > y.Item1) {return -1;} else {return 0;}});
            int currentBPMInterval = 0;
            int midiTicksPerGameTick;
            if (BPMIntervals.Count > 0)
            {
                midiTicksPerGameTick = calculateTickRatio(BPMIntervals[0].Item2);
            } 
            else
            {
                midiTicksPerGameTick = calculateTickRatio(120);
            }

            // extract note data
            foreach (MidiEvent midiEvent in midiFile.Tracks[mainTrackNumber].MidiEvents)
            {
                MidiEventType type = (MidiEventType) midiEvent.Type;
                if (type == MidiEventType.NoteOn)
                {
                    // if current Note in new BPM Interval         AND note is played after next Interval starts
                    if (currentBPMInterval + 1 < BPMIntervals.Count && BPMIntervals[currentBPMInterval+1].Item1 < midiEvent.Time)
                    {
                        currentBPMInterval++;
                        midiTicksPerGameTick = calculateTickRatio(BPMIntervals[currentBPMInterval].Item2);
                    }
                    notes.Add(new Note(midiEvent.Arg2, midiEvent.Time * midiTicksPerGameTick));
                }
            }

            notes.Add(new Note(-200,notes.Last().gameTick + 1));
            return notes;
        }

        private int calculateTickRatio(int BPM)
        {
            return BPM * TicksPerQuarterNote / 3600;
        }

    }

}