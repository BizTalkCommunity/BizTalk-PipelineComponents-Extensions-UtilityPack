using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BizTalk.PipelineComponents.PDF2Xml
{
    public class TextObject
    {

        public TextObject(double x, double y, string value)
        {
            this.X = x;
            this.Y = y;
            this.Value = value;
        }

        public TextObject()
        {
        }

        public double Y { set; get; }
        public double X { set; get; }
        public string Value{get;set;}
    }
}