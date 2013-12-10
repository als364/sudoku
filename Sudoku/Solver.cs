using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Sudoku
{
    class Solver
    {
        public static void Main()
        {
            Grid grid = new Grid(3);
            TheoreticalGrid theoreticalGrid = new TheoreticalGrid(grid);

            string filename = @"../../sudokus/01.txt";
            grid.ReadValuesFromFile(filename);
            Debug.WriteLine(grid.Valid());
        }
    }
}
