using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace  MultiIdeogram_CS
{
    class CytoSort 
        : IComparer<CytoGenetic>
    {
        int IComparer<CytoGenetic>.Compare(CytoGenetic x, CytoGenetic y)
        {
            if (y == null || x == null)
            {
                if (y == null)
                { return -1; }
                else if (x == null)
                { return 1; }
            }

            return x.StartOfRegion - y.StartOfRegion;
        }

    }
}
