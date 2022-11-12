namespace SimplexNoise
{
    public readonly struct WeightedVector2
    {
        public WeightedVector2(double x, double y, float w)
        {
            XY = new Vector2(x, y);
            W = w;
        }

        public double X => XY.X;
        public double Y => XY.Y;
        public Vector2 XY { get; }
        public float W { get; }

        public override string ToString()
        {
            return $"{X:0.000}, {Y:0.000}, {W:0.000}";
        }
    }
}
