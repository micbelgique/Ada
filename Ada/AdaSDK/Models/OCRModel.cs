using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AdaSDK.Models
{
    public class OCRModel
    {
        public string Language { get; set; }
        public List<Regions> Regions { get; set; }

    }
}