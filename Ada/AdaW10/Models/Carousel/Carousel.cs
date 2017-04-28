using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaW10.Models.Carousel
{
    public class Carousel
    {
        public string Title { get; set; }
        public string Text { get; set; }
        public Image[] Images  { get; set; }

        public class Image
        {
            public Uri Url { get; set; }
        }
    }
}
