using OpenCvSharp;
using System.ComponentModel;
using static TurkMite.Program;

namespace TurkmiteTest
{
    [TestClass]
    public class TurkmiteBaseTests
    {
        private TestTurkmiteBase turkmite = new TestTurkmiteBase(new Mat(10, 10, MatType.CV_8UC3));

        [TestMethod]
        public void GetNextColorAndUpdateDirection_IsCalled()
        {
            var t = new TestTurkmiteBase(new Mat(10, 10, MatType.CV_8UC1));
            t.Step();
            Assert.IsTrue(t.GetNextColorAndUpdateDirectionInvoked);
        }

        class TestTurkmiteBase : TurkmiteBase
        {
            public int X { get { return this.X; } set { this.X = value; } }
            public int Y { get { return this.Y; } set { this.Y = value; } }
            public int D { get { return this.D; } set { this.D = value; } }

            public new void PerformMove(int deltaDirection)
            {
                base.PerformMove(deltaDirection);
            }

            public bool GetNextColorAndUpdateDirectionInvoked = false;
            public readonly Vec3b ReturnedColor = new Vec3b(1, 1, 1);

            protected override (Vec3b newColor, int deltaDirection) GetNextColorAndUpdateDirection(Vec3b currentColor)
            {
                GetNextColorAndUpdateDirectionInvoked = true;
                return (ReturnedColor,1);
            }

            public TestTurkmiteBase(Mat img) : base(img) { }
            public override int PerferredIterationCount => 0;
        }

        [TestMethod]
        public void PerformMove_DirectionCorrect()
        {
            turkmite.X = 5;
            turkmite.Y = 5;
            turkmite.D = 0;
            turkmite.PerformMove(0);
            AssertTurkmiteState(5, 4, 0);
            turkmite.PerformMove(1);
            AssertTurkmiteState(6, 4, 1);
            turkmite.PerformMove(1);
            AssertTurkmiteState(6, 5, 2);
            turkmite.PerformMove(1);
            AssertTurkmiteState(5, 5, 3);
            turkmite.PerformMove(1);
            AssertTurkmiteState(5, 4, 0);
            turkmite.PerformMove(-1);
            AssertTurkmiteState(4, 4, 3);
        }
        private void AssertTurkmiteState(int x, int y, int d)
        {
            Assert.AreEqual(x, turkmite.X);
            Assert.AreEqual(y, turkmite.Y);
            Assert.AreEqual(d, turkmite.D);
        }

        [TestMethod]
        public void ImageBoundaryWorks()
        {
            AssertMove(5, 0, 0, 5, 0);
            AssertMove(5, 9, 2, 5, 9);
            AssertMove(9, 5, 1, 9, 5);
            AssertMove(0, 5, 3, 0, 5);
        }

        private void AssertMove(int startX, int startY, int direction, int finalX, int finalY)
        {
            turkmite.X = startX;
            turkmite.Y = startY;
            turkmite.D = direction;
            turkmite.PerformMove(0);
            AssertTurkmiteState(finalX, finalY, direction);
        }

        public void StepUpdatesCorrectly()
        {
            turkmite.X = 5;
            turkmite.Y = 5;
            turkmite.D = 0;
            turkmite.Step();
            var indexer = turkmite.Image.GetGenericIndexer<Vec3b>();
            var newColor = indexer[5, 5];
            Assert.AreEqual(turkmite.ReturnedColor, newColor);
            AssertTurkmiteState(6, 5, 1);
        }

        [TestClass]
        public class OriginalTurkmiteTests
        {
            TestOriginalTurkmite turkmite = new TestOriginalTurkmite(new Mat(10, 10, MatType.CV_8UC3, new Scalar(0, 0, 0)));

            [TestMethod]
            public void BlackField_Correct()
            {
                var result = turkmite.GetNextColorAndUpdateDirection(turkmite.Black);
                Assert.AreEqual(turkmite.White, result.newColor);
                Assert.AreEqual(1, result.deltaDirection);
            }

            [TestMethod]
            public void WhiteField_Correct()
            {
                var result = turkmite.GetNextColorAndUpdateDirection(turkmite.White);
                Assert.AreEqual(turkmite.Black, result.newColor);
                Assert.AreEqual(-1, result.deltaDirection);
            }

            private class TestOriginalTurkmite : OriginalTurkmite
            {
                public new (Vec3b newColor, int deltaDirection) GetNextColorAndUpdateDirection(Vec3b currentColor)
                {
                    return base.GetNextColorAndUpdateDirection(currentColor);
                }

                public Vec3b White { get { return White; } }
                public Vec3b Black { get { return Black; } }

                public TestOriginalTurkmite(Mat image) : base(image)
                {
                }
            }
        }
    }
}