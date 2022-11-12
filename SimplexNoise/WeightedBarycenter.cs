namespace SimplexNoise
{
    public readonly struct WeightedBarycenter<T>
    {
        public WeightedBarycenter(Barycenter abc, T u, T v, T w)
        {
            ABC = abc;
            U = u;
            V = v;
            W = w;
        }

        public Barycenter ABC { get; }
        public double A => ABC.A; 
        public double B => ABC.B;
        public double C => ABC.C;
        public T U { get; }
        public T V { get; }
        public T W { get; }

        public override string ToString()
        {
            return $"W: ({A:0.000}, {B:0.000}, {C:0.000}), Vals: ({U}, {V}, {W})";
        }
    }
}
