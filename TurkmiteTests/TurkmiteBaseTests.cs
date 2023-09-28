using Microsoft.VisualStudio.TestPlatform.TestHost;
using OpenCvSharp;
using TurkMite;

namespace TurkmiteTests 
{
    public class TurkmiteBaseTests
    {
        [Fact]
        public void GetNextColorAndUpdateDirection_IsCalled()
        {
            var t = new TestTurkmiteBase(new Mat(10,10,MatType.CV_8UC1));
            t.Step();
            Assert.True(t.GetNextColorAndUpdateDirectionInvoked);
        }

        class TestTurkmiteBase : TurkMite.Program.TurkmiteBase
        {
            public bool GetNextColorAndUpdateDirectionInvoked = false;

            protected override (Vec3b newColor, int deltaDirection) GetNextColorAndUpdateDirection(Vec3b newColor)
            {
                GetNextColorAndUpdateDirectionInvoked=true;
                return (new Vec3b(0, 0, 0), 0);
            }
            public TestTurkmiteBase(Mat img) : base(img) { }

            public override int PerferredIterationCount => 0;
        }
    } 
}