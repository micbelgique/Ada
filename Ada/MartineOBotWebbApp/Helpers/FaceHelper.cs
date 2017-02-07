using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.ProjectOxford.Face.Contract;

namespace MartineOBotWebApp.Helpers
{
    public static class FaceHelper
    {
        public static void FixFaceRectangle(ref FaceRectangle faceRectangle)
        {
            int outline = (int)Math.Ceiling(faceRectangle.Width * 0.1);
        }
    }
}