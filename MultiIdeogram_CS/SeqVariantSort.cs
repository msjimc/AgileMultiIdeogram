using System;
using System.Collections.Generic;
using System.Text;

namespace  MultiIdeogram_CS
    {
    class SeqVariantSort: IComparer<SeqVariant>
        {
        int IComparer<SeqVariant>.Compare(SeqVariant x, SeqVariant y)
            {
            if (y == null && x == null) { return 0; }
            else if (x == null) { return -1; }
            else if (y == null) { return 1; }

            if (x.Chromosome == y.Chromosome)
            { return x.Position - y.Position; }
            else
            { return x.Chromosome - y.Chromosome; }

            }
        }
    }
