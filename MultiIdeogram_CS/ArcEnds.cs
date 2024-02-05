using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace  MultiIdeogram_CS
{
    class ArcEnds
    {
        Point startPoint = new Point();
        Point endPoint = new Point();

        public ArcEnds(Point theStart, Point theEnd)
        {
            startPoint = theStart;
            endPoint = theEnd;
        }

        public Point P1 { get { return startPoint; } }

        public Point P2 { get { return endPoint; } }

    }
}
