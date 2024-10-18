using System;

namespace ABC
{
    public class Slur : Grouping, IEquatable<Slur>, IComparable<Slur> {
        

        public Slur(int startId, int endId) :base(startId, endId)
        {

        }

        public bool Equals(Slur other) {
            if (other == null)
            {
                return false;
            }

            return startId == other.startId && endId == other.endId;
        }

        public int CompareTo(Slur other)
        {
            if (ReferenceEquals(other, null))
            {
                return 1;
            } 

            return startId.CompareTo(other.startId);
        }
    }
}