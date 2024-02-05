using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace  MultiIdeogram_CS
{
    class CytoGenetic
    {
        private int StartRegion, EndRegion, Chromosome;
        private string RegionName, ChromosomeName;
        private Brush stainColour;

        public CytoGenetic(string[] items)
        {
            RegionName = items[3];
            StartRegion = Convert.ToInt32(items[1]);
            EndRegion = Convert.ToInt32(items[2]);

            ChromosomeName = items[0];
            switch (ChromosomeName.ToLower())
            {
                case "x":
                    Chromosome = 23;
                    break;
                case "y":
                    Chromosome = 24;
                    break;
                case "m":
                    Chromosome = 25;
                    break;
                default:
                    Chromosome = Convert.ToInt32(ChromosomeName);
                    break;
            }

            switch (items[4].ToLower())
            {
                case "gpos25":
                    stainColour = Brushes.LightGray;
                    break;
                case "gpos50":
                case "stalk":
                    stainColour = Brushes.Gray;
                    break;
                case "gpos75":
                    stainColour = Brushes.DarkGray;
                    break;
                case "gpos100":
                    stainColour = Brushes.Black;
                    break;
                case "gvar":
                case "gneg":
                    stainColour = Brushes.WhiteSmoke;
                    break;
                default:
                    stainColour = Brushes.Orange;
                    break;
            }
        }

        public void Draw(Graphics g, float scale, Point Offset, bool Odd, int multiple)
        {
            Rectangle r;
            int W = (int)((EndRegion - StartRegion) / scale);
            int X = (20 * multiple)  + (int)((StartRegion - Offset.X) / scale);
            if (W == 0) { W = 1; }

            r = new Rectangle(X, Offset.Y, W, 10 * multiple);

            g.FillRectangle(stainColour, r);
            if (multiple > 1)
            {
                Pen b = new Pen(Color.Black, 2f);
                g.DrawRectangle(b, r);
            }
            else
            { g.DrawRectangle(Pens.Black, r); }

        }

        public Brush GetBrush { get { return stainColour; } }

        public string Name { get { return RegionName; } }

        public int StartOfRegion { get { return StartRegion; } }

        public int EndOfRegion { get { return EndRegion; } }

        public int ChromosomeOfRegion { get { return Chromosome; } }

    }
}
