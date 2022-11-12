using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimplexNoise.Tests
{
    [TestClass]
    public class BarycenterTests
    {
        [TestMethod]
        public void CloseBarycentersSameValue()
        {
            NoiseGrid? noiseGrid = new(0, 0, 0, 1.1255);

            WeightedBarycenter<double> barryA = noiseGrid.WeightedBarycenterAtPoint(1, 1);
            WeightedBarycenter<double> barryB = noiseGrid.WeightedBarycenterAtPoint(1.01, 1.01);

            Assert.AreNotEqual(barryA.A, barryB.A, 0.0000001);
            Assert.AreNotEqual(barryA.B, barryB.B, 0.0000001);
            Assert.AreNotEqual(barryA.C, barryB.C, 0.0000001);

            Assert.AreEqual(barryA.U, barryB.U, 0.0000001);
            Assert.AreEqual(barryA.V, barryB.V, 0.0000001);
            Assert.AreEqual(barryA.W, barryB.W, 0.0000001);
        }

        [TestMethod]
        public void EdgeCaseBarycenter()
        {
            NoiseGrid? noiseGrid = new(0, 0, 0, 1.1255);

            noiseGrid.WeightedBarycenterAtPoint(0, 0.84);
        }

        [TestMethod]
        public void EdgeCaseBarycenter2()
        {
            NoiseGrid? noiseGrid = new(0, 0, 0, 1.1255);

            noiseGrid.WeightedBarycenterAtPoint(0, 6.1200000000000045);
        }



        [TestMethod]
        public void IntegerGridBarycenter()
        {
            NoiseGrid? noiseGrid = new(0, 0, 0, 1.1255);

            noiseGrid.WeightedBarycenterAtPoint(0, 0);
        }

        [TestMethod]
        public void BatchBarycenters()
        {
            NoiseGrid? noiseGrid = new(0, 0, 0, 1.1255);

            int count = 0;

            for (double x = -200; x < 200; x+=0.1527)
            {
                for (double y = -200; y < 200; y += 0.1527)
                {
                    noiseGrid.WeightedBarycenterAtPoint(x, y);
                    count++;
                }
            }

            Console.WriteLine($"Ran {count} iterations");
        }

        [TestMethod]
        public void BatchBarycentersLargeNumbers()
        {
            NoiseGrid? noiseGrid = new(0, 0, 0, 1.1255);

            int count = 0;

            for (double x = -400; x < 400; x += 4.1527)
            {
                for (double y = -400; y < 400; y += 4.1527)
                {
                    noiseGrid.WeightedBarycenterAtPoint(x, y);
                    count++;
                }
            }

            Console.WriteLine($"Ran {count} iterations");
        }

        [TestMethod]
        public void BatchIntegerBarycenters()
        {
            NoiseGrid? noiseGrid = new(0, 0, 0, 1.1255);

            int count = 0;

            var outBarcenters = new List<WeightedBarycenter<double>>();

            for (double x = -400; x < 400; x += 4.1527)
            {
                for (double y = -400; y < 400; y += 4.1527)
                {
                    outBarcenters.Add(noiseGrid.WeightedBarycenterAtPoint(x, y));
                    count++;
                }
            }

            Console.WriteLine($"Ran {count} iterations");

            var outInts = new List<WeightedBarycenter<Int2>>();

            for (double x = -400; x < 400; x += 4.1527)
            {
                for (double y = -400; y < 400; y += 4.1527)
                {
                    outInts.Add(noiseGrid.IndexedBarycenterAtPoint(x, y));
                }
            }

            outBarcenters.Zip(outInts).ToList().ForEach(x =>
            {
                var u = noiseGrid.WeightAtGrid(x.Second.U);
                Assert.AreEqual(x.First.U, u, 0.00000001);
                var v = noiseGrid.WeightAtGrid(x.Second.V);
                Assert.AreEqual(x.First.V, v, 0.00000001);
                var w = noiseGrid.WeightAtGrid(x.Second.W);
                Assert.AreEqual(x.First.W, w, 0.00000001);
            });

        }
    }
}