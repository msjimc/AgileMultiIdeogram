using System;
using System.Collections.Generic;
using System.Text;

namespace  MultiIdeogram_CS
{
    public class SeqSNPSorter : IComparer<SeqSNP>
    {

        int IComparer<SeqSNP>.Compare(SeqSNP x, SeqSNP y)
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


            if (x.Position == y.Position)
            {
                string[] s = { x.Name, y.Name };
                Array.Sort(s);
                if (s[0] == x.Name)
                {
                    return 1;
                }
                else
                {
                    return -1;
                }
            }

            if (x.Position > 500)
            {
                return Convert.ToInt32(x.Position - y.Position);
            }
            else
            {
                return Convert.ToInt32((x.Position - y.Position) * 1000000);
            }

        }

    }

}
