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

            string filename = @"../../sudokus/01.txt";
            grid.ReadValuesFromFile(filename);

            TheoreticalGrid theoreticalGrid = new TheoreticalGrid(grid);

            Debug.WriteLine(theoreticalGrid);

            theoreticalGrid.AC3();

            Debug.WriteLine(theoreticalGrid);
        }
    }
}
