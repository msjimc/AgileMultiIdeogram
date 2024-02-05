using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;


namespace  MultiIdeogram_CS
{
    public class DNARegionSort : IComparer
    {

        public int Compare(object x, object y)
        {
            if (x == null || y == null)
            {
                if ((y != null))
                {
                    return -1;
                }
                else if ((x != null))
                {
                    return 1;
                }
                return 0;
            }

            DNARegion a = default(DNARegion);
            DNARegion b = default(DNARegion);
            a = (DNARegion)x;
            b = (DNARegion)y;

            if (a.Chromosome == b.Chromosome)
            {
                return a.StartPoint - b.StartPoint;
            }
            else { return a.Chromosome - b.Chromosome;}
            
        }
    }
}

