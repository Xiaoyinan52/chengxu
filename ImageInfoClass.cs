using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WireTestProgram
{
  public  class ImageInfoClass
    {
        public int  ImageID { get; set; }
        public string  ImageName { get; set; }
        public string ImageShortName { get; set; }
        public int ImageWidth { get; set; }
        public int ImageHeight { get; set; }
        public int ImageLeft { get; set; }
        public int ImageTop { get; set; }
        public bool ImageUse { get; set; }
        public int ImageUseColumn { get; set; }
    }
}
