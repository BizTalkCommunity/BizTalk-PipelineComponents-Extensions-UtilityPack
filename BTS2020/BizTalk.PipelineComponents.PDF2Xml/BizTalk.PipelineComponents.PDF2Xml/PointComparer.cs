using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BizTalk.PipelineComponents.PDF2Xml
{
    class TextObjectComparer : IComparer<TextObject>
    {
        public int Compare(TextObject first, TextObject second)
        {
            /*Note that the IComparer.Compare method requires a tertiary comparison. 1, 0, or -1 is returned depending on whether one value is greater than, equal to, or less than the other. 
             The sort order (ascending or descending) can be changed by switching the logical operators in this method.
            */
            if (first.Y == second.Y)
            {
                //I want the lowest X to come first
                return (int)( first.X -second.X);
            }
            else
            {
                //I want the highest Y to come first
                return (int)(second.Y - first.Y);
            }
        } 
    } 
}