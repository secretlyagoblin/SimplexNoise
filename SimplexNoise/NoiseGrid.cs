using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimplexNoise
{
    public class NoiseGrid
    {
        public int OffsetX { get; } = 0;
        public int OffsetY { get; } = 0;
        public int OffsetZ { get; } = 0;

        const float SCALE = 5643.1253656f;

        public double Scale { get; } = 1;
        public NoiseGrid(int offsetX, int offsetY, int offsetZ, double scale)
        {
            OffsetX = offsetX;
            OffsetY = offsetY;
            OffsetZ = offsetZ;
            Scale = scale;
        }

        private readonly Dictionary<Int2, WeightedVector2> _weightDict = new Dictionary<Int2, WeightedVector2>();

        public double WeightAtGrid(int x, int y) => WeightAtGrid(new Int2(x, y));

        public double WeightAtGrid(Int2 xy) => WeightAtGrid(xy, true);

        private float WeightAtGrid(Int2 xy, bool performLookup)
        {
            if (performLookup && _weightDict.TryGetValue(xy, out var wv)) return wv.W;

            if (performLookup) return JitteredPointAtGrid(xy, false).W;

            return Noise.Calc3D(xy.X + OffsetX, xy.Y + OffsetY, -2943 + OffsetZ, SCALE);
        }

        public WeightedVector2 JitteredPointAtGrid(int x, int y) => JitteredPointAtGrid(new Int2(x, y));

        public WeightedVector2 JitteredPointAtGrid(Int2 xy) => JitteredPointAtGrid(xy, true);

        public WeightedVector2 JitteredPointAtGrid(Int2 xy, bool performLookup)
        {
            if (performLookup && _weightDict.TryGetValue(xy, out var wv)) return wv;

            var floatX = Noise.Calc3D(xy.X + OffsetX, xy.Y + OffsetY, 7 + OffsetZ, SCALE) * 0.5;
            var floatY = Noise.Calc3D(xy.X + OffsetX,xy.Y + OffsetY, 1392 + OffsetZ, SCALE) * 0.5;
            var floatW = WeightAtGrid(xy,false);

            var wv2 = new WeightedVector2(xy.X + floatX, xy.Y + floatY, floatW);

            _weightDict[xy] = wv2;

            return wv2;
        }

        private static readonly byte[] TriangeOrder = { 
            0, 1, 4, 
            1, 2, 4, 
            2, 5, 4, 
            5, 8, 4, 
            8, 7, 4, 
            7, 6, 4, 
            6, 3, 4, 
            3, 0, 4 
        };

        public WeightedBarycenter<double> WeightedBarycenterAtPoint(double x, double y)
        {
            Vector2 test = new Vector2(x*Scale, y*Scale);

            int xIndex = (int) Math.Round(test.X);
            int yIndex = (int) Math.Round(test.Y);            

            WeightedVector2[] points = new WeightedVector2[9];

            int count = 0;

            for (int u = -1; u < 2; u++)
            {
                for (int v = -1; v < 2; v++)
                {
                    points[count++] = JitteredPointAtGrid(xIndex+u, yIndex+v);
                }
            }

            for (int t = 0; t < 24; t+=3)
            {
                var t0 = TriangeOrder[t];
                var t1 = TriangeOrder[t + 1];
                var t2 = TriangeOrder[t + 2];

                var barycenter = 
                    SlowCalculateBarycentricWeight(points[t0], points[t1], points[t2], test);


                if (barycenter.A <= 1 && barycenter.A >= 0 && 
                   barycenter.B <= 1 && barycenter.B >= 0 && 
                   barycenter.C <= 1 && barycenter.C >= 0)
                {
                    return new WeightedBarycenter<double>(barycenter, points[t0].W, points[t1].W, points[t2].W);
                }
            }

            throw new Exception($"Could not solve barycenter for point ({x}, {y})");
        }

        public WeightedBarycenter<Int2> IndexedBarycenterAtPoint(double x, double y)
        {
            Vector2 test = new Vector2(x * Scale, y * Scale);

            int xIndex = (int)Math.Round(test.X);
            int yIndex = (int)Math.Round(test.Y);

            WeightedVector2[] points = new WeightedVector2[9];
            Int2[] indices = new Int2[9];

            int count = 0;

            for (int u = -1; u < 2; u++)
            {
                for (int v = -1; v < 2; v++)
                {
                    indices[count] = new Int2(xIndex + u, yIndex + v);
                    points[count++] = JitteredPointAtGrid(xIndex + u, yIndex + v);
                }
            }

            for (int t = 0; t < 24; t += 3)
            {
                var t0 = TriangeOrder[t];
                var t1 = TriangeOrder[t + 1];
                var t2 = TriangeOrder[t + 2];

                var barycenter =
                    SlowCalculateBarycentricWeight(points[t0], points[t1], points[t2], test);


                if (barycenter.A <= 1 && barycenter.A >= 0 &&
                   barycenter.B <= 1 && barycenter.B >= 0 &&
                   barycenter.C <= 1 && barycenter.C >= 0)
                {
                    return new WeightedBarycenter<Int2>(barycenter, indices[t0], indices[t1], indices[t2]);
                }
            }

            throw new Exception($"Could not solve barycenter for point ({x}, {y})");
        }


        /// <summary>
        /// Calculates the barycentric weight of the inner triangles.
        /// </summary>
        /// <param name="vertA"></param>
        /// <param name="vertB"></param>
        /// <param name="vertC"></param>
        /// <param name="test"></param>
        /// <returns></returns>
        private static Barycenter FastCalculateBarycentricWeight(WeightedVector2 vertA, WeightedVector2 vertB, WeightedVector2 vertC, Vector2 test)
        {
            Vector2 v0 = vertB.XY - vertA.XY, v1 = vertC.XY - vertA.XY, v2 = test - vertA.XY;
            double d00 = Vector2.Dot(v0, v0);
            double d01 = Vector2.Dot(v0, v1);
            double d11 = Vector2.Dot(v1, v1);
            double d20 = Vector2.Dot(v2, v0);
            double d21 = Vector2.Dot(v2, v1);
            double denom = d00 * d11 - d01 * d01;
            double v = (d11 * d20 - d01 * d21) / denom;
            double w = (d00 * d21 - d01 * d20) / denom;
            double u = 1.0 - v - w;

            return new Barycenter(u, v, w);
        }

        private static Barycenter SlowCalculateBarycentricWeight(WeightedVector2 vertA, WeightedVector2 vertB, WeightedVector2 vertC, Vector2 test)
        {
            var AB = DistanceSquared(vertA.XY,vertB.XY);
            var BC = DistanceSquared(vertB.XY,vertC.XY);
            var CA = DistanceSquared(vertC.XY,vertA.XY);

            var CT = DistanceSquared(vertC.XY, test);

            var areaABC = AreaSquaredSquared(AB, BC, CA);
            var areaPBC = AreaSquaredSquared(
                BC,
                DistanceSquared(vertB.XY, test),
                CT);
            var areaPCA = AreaSquaredSquared(
                CA,
                CT,
                DistanceSquared(vertA.XY, test));

            var outA = areaPBC / areaABC; // alpha
            var outB = areaPCA / areaABC; // beta
            var outC = 1.0 - outA - outB; // gamma

            return new Barycenter(outA, outB, outC);

            double DistanceSquared(Vector2 a, Vector2 b)
            {
                Vector2 delta = a - b;

                return Math.Sqrt(Math.Pow(delta.X, 2) + Math.Pow(delta.Y, 2));
            }

            double AreaSquaredSquared(double a, double b, double c)
            {
                var s = (a + b + c) * 0.5;

                var sum = s * (s - a) * (s - b) * (s - c);

                return Math.Sqrt(sum); 
            }
        }

    }
}
