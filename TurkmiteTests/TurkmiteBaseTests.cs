using Microsoft.VisualStudio.TestPlatform.TestHost;
using OpenCvSharp;
using TurkMite;

namespace TurkmiteTests 
{
    public class TurkmiteBaseTests
    {
        [Fact]
        public void Instantiation()
        {
            var t = new TurkMite.Program.OriginalTurkmite(new Mat(10, 10, MatType.CV_8UC3));
        }
    } 
}