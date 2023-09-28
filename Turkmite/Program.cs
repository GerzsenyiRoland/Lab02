using OpenCvSharp;
using System;

namespace TurkMite
{
    public class Program
    {
        static void Main(string[] args)
        {
            Mat img1 = new Mat(200, 200, MatType.CV_8UC3, new Scalar(0, 0, 0));
            Mat img2 = new Mat(200, 200, MatType.CV_8UC3, new Scalar(0, 0, 0));
            var turkmite = new Program.OriginalTurkmite(img1);
            for (int i = 0; i < turkmite.PerferredIterationCount; i++)
            {
                turkmite.Step();
            }
            var turkmit = new ThreeColorTurkmite(img2);
            for (int i = 0; i<turkmit.PerferredIterationCount; i++)
            {
                turkmit.Step();
            }

            Cv2.ImShow("TurkMite1", img1);
            Cv2.ImShow("TurkMite2", img2);
            Cv2.WaitKey();

        }

         public class OriginalTurkmite : TurkmiteBase
        {
            readonly Vec3b black = new Vec3b(0, 0, 0);
            readonly Vec3b white = new Vec3b(255, 255, 255);

            public OriginalTurkmite(Mat image) : base(image) { }

            public override int PerferredIterationCount => 13000;

            protected override (Vec3b newColor, int deltaDirection) GetNextColorAndUpdateDirection(Vec3b currentColor)
            {
                return (currentColor == black) ? (white,1) : (black,-1);
            }
        }

        class ThreeColorTurkmite : TurkmiteBase
        {
            readonly private Vec3b black = new Vec3b(0,0,0);
            readonly private Vec3b white = new Vec3b(255,255,255);
            readonly private Vec3b red = new Vec3b(0,0,255);

            public ThreeColorTurkmite(Mat image):base(image) { }

            public override int PerferredIterationCount => 13000;

            protected override (Vec3b newColor, int deltaDirection) GetNextColorAndUpdateDirection(Vec3b currentColor)
            {
                if (currentColor == black)
                    return (white, 1);
                else if (currentColor == white)
                    return (red, -1);
                else return (black, -1);
            }
        }

        public abstract class TurkmiteBase
        {
            public Mat Image { get; }
            private Mat.Indexer<Vec3b> indexer;
            private int x;
            private int y;
            protected int direction;
            protected TurkmiteBase(Mat image)
            {
                Image = image;
                x = image.Cols / 2;
                y = image.Rows / 2;
                direction = 0;
                indexer = image.GetGenericIndexer<Vec3b>();
            }
             
            readonly private (int x,int y)[] delta = new (int x, int y)[] { (0, -1), (1, 0), (0, 1), (-1, 0) };

            public void Step()
            {
                int deltaDirection;
                (indexer[y, x], deltaDirection) = 
                    GetNextColorAndUpdateDirection(indexer[y, x]);
                PerformMove(deltaDirection);
            }

            public abstract int PerferredIterationCount { get; }
            protected abstract (Vec3b newColor, int deltaDirection) GetNextColorAndUpdateDirection(Vec3b currentColor);
            protected void PerformMove(int deltaDirection)
            {
                direction += deltaDirection;
                direction = (direction + 4) % 4;
                x += delta[direction].x;
                y += delta[direction].y;
                x = Math.Max(0, Math.Min(Image.Cols, x));
                y = Math.Max(0, Math.Min(Image.Rows, y));
            }
        }
    }
}
