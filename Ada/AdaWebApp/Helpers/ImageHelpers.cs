using System.Drawing;
using System.Drawing.Imaging;
using Microsoft.ProjectOxford.Face.Contract;

namespace AdaWebApp.Helpers
{
    public static class ImageHelpers
    {
        public static void CreateImageWithRectangleFace(string sourcePath, string outputPath, FaceRectangle faceRectangle)
        {
            using (Bitmap image = new Bitmap(sourcePath))
            {
                using (Graphics g = Graphics.FromImage(image))
                {
                    Pen p = new Pen(new SolidBrush(Color.Green), 3); 
                    g.DrawRectangle(p, faceRectangle.Left, faceRectangle.Top, faceRectangle.Width, faceRectangle.Height); 
                }

                image.Save(outputPath, ImageFormat.Jpeg); 
            }
        }
    }
}
