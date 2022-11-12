using System;
namespace SimplexNoise

{
    public readonly struct Int2 : IEquatable<Int2>
    {
        public Int2(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get;}
        public int Y { get;}

        public override bool Equals(object obj)
        {
            return obj is Int2 @int &&
                   X == @int.X &&
                   Y == @int.Y;
        }

        public bool Equals(Int2 other)
        {            
                return X == other.X &&
                       Y == other.Y;
        }

        public override int GetHashCode()
        {
            int hashCode = 1861411795;
            hashCode = hashCode * -1521134295 + X.GetHashCode();
            hashCode = hashCode * -1521134295 + Y.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(Int2 left, Int2 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Int2 left, Int2 right)
        {
            return !(left == right);
        }
    }
}