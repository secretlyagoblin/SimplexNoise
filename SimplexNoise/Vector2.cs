using System;

namespace SimplexNoise
{
    public readonly struct Vector2 : IEquatable<Vector2>
    {
        public double X { get; }
        public double Y { get; }
        public Vector2(double x, double y)
        {
            X = x;
            Y = y;
        }  
        public bool Equals(Vector2 obj)
        {
            return obj is Vector2 vector &&
                   X == vector.X &&
                   Y == vector.Y;
        }
        public override bool Equals(object obj)
        {
            if (obj is Vector2 vector) return this.Equals(vector);
            else return false;
        }
        public override int GetHashCode()
        {
            int hashCode = 1861411795;
            hashCode = hashCode * -1521134295 + X.GetHashCode();
            hashCode = hashCode * -1521134295 + Y.GetHashCode();
            return hashCode;
        }
        public static bool operator ==(Vector2 left, Vector2 right)
        {
            return left.Equals(right);
        }
        public static bool operator !=(Vector2 left, Vector2 right)
        {
            return !(left == right);
        }
        public static Vector2 operator +(Vector2 left, Vector2 right)
        {
            return new Vector2(left.X + right.X, left.Y + right.Y);
        }
        public static Vector2 operator -(Vector2 left, Vector2 right)
        {
            return new Vector2(left.X - right.X, left.Y - right.Y);
        }
        public static double Dot(Vector2 left, Vector2 right)
        {
            return left.X * right.X +
                   left.Y * right.Y;
        }
        public override string ToString()
        {
            return $"{X:0.000}, {Y:0.000}";
        }
    }
}
