namespace SimplexNoise
{
    public readonly struct Barycenter
    {
        public Barycenter(double a, double b, double c)
        {
            A = a;
            B = b;
            C = c;
        }

        public double A { get; }
        public double B { get; }
        public double C { get; }

        public override string ToString()
        {
            return $"{A:0.000}, {B:0.000}, {C:0.000}";
        }
    }
}
