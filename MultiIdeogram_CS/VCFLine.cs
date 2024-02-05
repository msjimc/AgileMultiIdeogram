using System;
using System.Collections.Generic;
using System.Text;

namespace  MultiIdeogram_CS
    {
    class VCFLine
        {
        string[] items=null;

        public VCFLine(string dataLine)
            {
             items = dataLine.Split('\t');
            }

        public string value(int index)
            {
            return items[index];
            }

        }
    }
