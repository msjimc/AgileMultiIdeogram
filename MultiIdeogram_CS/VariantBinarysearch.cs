using System;
using System.Collections.Generic;
using System.Text;

namespace  MultiIdeogram_CS
    {
    class VariantBinarysearch: System.Collections.IComparer
        {
        int System.Collections.IComparer.Compare(object x, object y)
            {
            if (y == null && x == null) { return 0; }
            else if (x == null) { return -1; }
            else if (y == null) { return 1; }
            Variant a = (Variant) x;
            VCFPharser b = (VCFPharser) y;

            if (a.Chromosome == b.ChromosomeNumber)
            {return a.Position - b.Position; }
            else
            { return a.Chromosome - b.ChromosomeNumber; }

            }

        }
    }
