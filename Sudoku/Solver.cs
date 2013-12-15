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
            string sudokuNumber = "03";
            bool ac3Solvable = true;

            Grid grid = new Grid(3);

            #region File Read
            string filename = "";
            if (ac3Solvable)
            {
                filename += @"../../sudokus/ac3-solvable/" + sudokuNumber + ".txt";
            }
            else
            {
                filename += @"../../sudokus/non-ac3-solvable/" + sudokuNumber + ".txt";
            }
            grid.ReadValuesFromFile(filename);
            #endregion

            TheoreticalGrid theoreticalGrid = new TheoreticalGrid(grid);
            theoreticalGrid.AC3();

            #region File Write
            string outputFilename = "";
            if (ac3Solvable)
            {
                outputFilename += @"../../sudokus/ac3-solvable/" + sudokuNumber + "-output.txt";
            }
            else
            {
                outputFilename += @"../../sudokus/non-ac3-solvable/" + sudokuNumber + "-output.txt";
            }
            theoreticalGrid.WriteValuesToFile(outputFilename);
            #endregion
        }
    }
}
