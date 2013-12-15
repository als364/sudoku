using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sudoku
{
    static class Utils
    {
        public static HashSet<int> getAlphabet(int gridsize)
        {
            HashSet<int> set = new HashSet<int>();
            for (int i = 1; i <= gridsize * gridsize; i++)
            {
                set.Add(i);
            }
            return set;
        }
    }
}