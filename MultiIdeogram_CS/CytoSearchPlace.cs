using System;
using System.Collections;
using System.Linq;
using System.Text;

namespace  MultiIdeogram_CS
{
    class CytoSearchPlace: IComparer
    {
        public int Compare(object x, object y)
        {
            if (y == null || x == null)
            {
                if (y == null)
                { return -1; }
                else if (x == null)
                { return 1; }
            }

            CytoGenetic a = (CytoGenetic)x;
            int b = (int)y;

            return a.StartOfRegion - b;
        }
    }
}
