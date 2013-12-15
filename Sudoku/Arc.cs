using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sudoku
{
    class Arc
    {
        private Point p1;
        private Point p2;

        public Arc(Point p1, Point p2)
        {
            this.p1 = p1;
            this.p2 = p2;
        }

        public Point P1
        {
            get { return p1; }
            set { p1 = value; }
        }

        public Point P2
        {
            get { return p2; }
            set { p2 = value; }
        }
    }
}
