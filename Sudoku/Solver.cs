using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Sudoku
{
    class Solver
    {
        static int backtracks = 0;

        static bool FUC = false;
        static bool MRV = false;

        static bool LO = false;

        public static void Main()
        {
            string sudokuNumber = "07";

            bool forwardCheck = false;
            bool strictAC3 = false;
            
            bool backtracking = true;

            TimeSpan duration;

            TheoreticalGrid theoreticalGrid = new TheoreticalGrid(3);

            #region File Read
            string filename = @"../../sudokus/" + sudokuNumber + ".txt";
            theoreticalGrid.ReadValuesFromFile(filename);
            #endregion

            DateTime now = DateTime.Now; 

            if (forwardCheck)
            {
                ForwardCheck(theoreticalGrid);
            }
            if (strictAC3)
            {
                AC3(theoreticalGrid);
            }
            if (backtracking)
            {
                while (!theoreticalGrid.Complete())
                {
                    TheoreticalGrid backtracked = BacktrackingSearch(theoreticalGrid);
                    if (backtracked == null)
                    {
                        break;
                    }
                    else
                    {
                        
                        theoreticalGrid = backtracked;
                    }
                }
            }

            duration = DateTime.Now - now;
            #region File Write
            string outputFilename = @"../../sudokus/" + sudokuNumber + "-output.txt";
            theoreticalGrid.WriteValuesToFile(outputFilename, duration);
            #endregion
        }

        public static void ForwardCheck(TheoreticalGrid grid)
        {
            for (int x = 0; x < grid.Grid.GetLength(0); x++)
            {
                for (int y = 0; y < grid.Grid.GetLength(1); y++)
                {
                    if (grid.Grid[x, y].Count == 1)
                    {
                        int value = grid.Grid[x, y].ToArray()[0];
                        List<Point> peers = grid.GetPeers(new Point(x, y));
                        foreach (Point peer in peers)
                        {
                            grid.Grid[peer.X, peer.Y].Remove(value);
                        }
                    }
                }
            }
        }

        public static bool AC3(TheoreticalGrid grid)
        {
            #region Pseudocode
            // Initial domains are made consistent with unary constraints.
            /** 
             *  for each x in X
             *      D(x) := { x in D(x) | R1(x) }
             *  // 'worklist' contains all arcs we wish to prove consistent or not.
             *  worklist := { (x, y) | there exists a relation R2(x, y) or a relation R2(y, x) }
             */
            #endregion

            Queue<Arc> worklist = new Queue<Arc>();
            for (int x = 0; x < grid.Grid.GetLength(0); x++)
            {
                for (int y = 0; y < grid.Grid.GetLength(1); y++)
                {
                    Point currentPoint = new Point(x, y);
                    List<Point> peers = grid.GetPeers(currentPoint);
                    foreach (Point peer in peers)
                    {
                        worklist.Enqueue(new Arc(currentPoint, peer));
                    }
                }
            }

            #region Pseudocode
            /**
             *  while worklist not empty
             *      select any arc (x, y) from worklist
             *      worklist := worklist - (x, y)
             *      if arc-reduce (x, y) 
             *          if D(x) is empty
             *              return failure
             *          else
             *              worklist := worklist + { (z, x) | z != y and there exists a relation R2(x, z) or a relation R2(z, x) }
             */
            #endregion

            while (worklist.Count > 0)
            {
                Arc currentArc = worklist.Dequeue();
                if (ArcReduce(grid, currentArc))
                {
                    List<int> domainX = grid.Grid[currentArc.P1.X, currentArc.P1.Y].ToList();
                    if (domainX.Count == 0)
                    {
                        return false;
                    }
                    else
                    {
                        List<Point> peers = grid.GetPeers(currentArc.P1);
                        foreach (Point peer in peers)
                        {
                            if (!peer.Equals(currentArc.P1))
                            {
                                worklist.Enqueue(new Arc(peer, currentArc.P1));
                            }
                        }
                    }
                }
            }
            return true;
        }

        private static bool ArcReduce(TheoreticalGrid grid, Arc arc)
        {
            #region Pseudocode
            /**
             *  function arc-reduce (x, y)
             *      bool change = false
             *      for each vx in D(x)
             *          find a value vy in D(y) such that vx and vy satisfy the constraint R2(x, y)
             *          if there is no such vy 
             *          {
             *              D(x) := D(x) - vx
             *              change := true
             *          }
             *      return change
             */
            #endregion

            bool change = false;
            List<Point> peers = grid.GetPeers(arc.P1);
            foreach (Point peer in peers)
            {
                if (peer.Equals(arc.P2))
                {
                    return false;
                }
            }
            List<int> domainX = grid.Grid[arc.P1.X, arc.P1.Y].ToList();
            for (int i = 0; i < domainX.Count; i++)
            {
                bool existsSatisfyingAssignment = false;
                int vx = domainX[i];

                List<int> domainY = grid.Grid[arc.P2.X, arc.P2.Y].ToList();
                for (int j = 0; j < domainY.Count; j++)
                {
                    int vy = domainY[j];
                    if (vx != vy)
                    {
                        existsSatisfyingAssignment = true;
                        break;
                    }
                }
                if (!existsSatisfyingAssignment)
                {
                    grid.Grid[arc.P1.X, arc.P1.Y].Remove(vx);
                    change = true;
                }
            }
            return change;
        }

        public static TheoreticalGrid BacktrackingSearch(TheoreticalGrid branch)
        {
            TheoreticalGrid ret;

            if (!AC3(branch))
            {
                return null;
            }
            if (branch.Complete())
            {
                return branch;
            }
            else
            {
                Point point = TrackToTry(branch);

                while (branch.Grid[point.X, point.Y].Count > 1)
                {
                    int value = ValueToTry(branch, point);

                    TheoreticalGrid clone = (TheoreticalGrid)branch.Clone();
                    clone.Set(value, point.X, point.Y);

                    ret = BacktrackingSearch(clone);

                    if (ret != null)
                    {
                        return ret;
                    }

                    backtracks++;
                    branch.Remove(value, point.X, point.Y);

                    if (!AC3(branch))
                    {
                        return null;
                    }
                }
                return null;
            }
        }

        private static Point TrackToTry(TheoreticalGrid branch)
        {
            if (FUC)
            {
                return FirstUnassignedCell(branch);
            }
            else if (MRV)
            {
                return MinimumRemainingValues(branch);
            }
            else
            {
                return MaxDegree(branch, MinimumRemainingValuesList(branch));
            }
        }

        private static int ValueToTry(TheoreticalGrid branch, Point point)
        {
            if (LO)
            {
                return LexicographicalOrder(branch, point);
            }
            else
            {
                return LeastConstraintValue(branch, point);
            }
        }

        #region Cell Heuristics
        private static List<Point> MinimumRemainingValuesList(TheoreticalGrid branch)
        {
            int min = branch.GridsizeSquared + 1;
            List<Point> mrvs = new List<Point>();
            for (int i = 0; i < branch.GridsizeSquared; i++)
                for (int j = 0; j < branch.GridsizeSquared; j++)
                {
                    if ((branch.Get(i, j).Count != 1) && (branch.Get(i, j).Count == min))
                    {
                        mrvs.Add(new Point(i, j));
                        continue;
                    }
                    if ((branch.Get(i, j).Count != 1) && (branch.Get(i, j).Count < min))
                    {
                        mrvs.Clear();
                        min = branch.Get(i, j).Count;
                        mrvs.Add(new Point(i, j));
                    }

                }
            return mrvs;
        }

        private static Point MaxDegree(TheoreticalGrid branch, List<Point> mrvs)
        {
            int degree = -1;
            Point point = null;
            foreach (Point mrvPoint in mrvs)
            {
                int count = 0;
                List<Point> peers = branch.GetPeers(mrvPoint);
                foreach (Point peer in peers)
                {
                    if (branch.Get(peer.X, peer.Y).Count != 1)
                    {
                        count++;
                    }
                }
                if (count > degree)
                {
                    degree = count;
                    point = mrvPoint;
                }
            }
            return point;
        }

        private static Point MinimumRemainingValues(TheoreticalGrid branch)
        {
            int min = branch.GridsizeSquared + 1;
            Point point = new Point(-1, -1);

            for (int i = 0; i < branch.Grid.GetLength(0); i++)
            {
                for (int j = 1; j < branch.Grid.GetLength(1); j++)
                {
                    if ((branch.Get(i, j).Count != 1) && (branch.Get(i, j).Count < min))
                    {
                        point.X = i;
                        point.Y = j;
                        min = branch.Get(i, j).Count;
                    }
                }
            }
            return point;
        }

        private static Point FirstUnassignedCell(TheoreticalGrid branch)
        {
            for (int i = 0; i < branch.Grid.GetLength(0); i++)
            {
                for (int j = 0; j < branch.Grid.GetLength(1); j++)
                {
                    if (branch.Grid[i, j].Count > 1)
                    {
                        return new Point(i, j);
                    }
                }
            }
            return null;
        }
        #endregion

        #region Variable Heuristics
        private static int LexicographicalOrder(TheoreticalGrid branch, Point point)
        {
            return branch.Grid[point.X, point.Y].ToArray()[0];
        }

        private static int LeastConstraintValue(TheoreticalGrid branch, Point point)
        {
            int[] values = branch.Get(point.X, point.Y).ToArray();
            int[] constraints = new int[branch.Get(point.X, point.Y).Count];

            for (int i = 0; i < constraints.Length; i++)
            {
                constraints[i] = 0;
            }

            for (int i = 0; i < branch.Get(point.X, point.Y).Count; i++)
            {
                List<Point> peers = branch.GetPeers(point);
                foreach (Point peer in peers)
                {
                    if (branch.Get(peer.X, peer.Y).Contains(values[i]))
                    {
                        constraints[i]++;
                    }
                }
            }
            return values[GetMinIndex(constraints)];
        }

        private static int GetMinIndex(int[] constraints)
        {
            int min = constraints[0];
            int index = 0;
            for (int i = 1; i < constraints.Length; i++)
            {
                if (constraints[i] < min)
                {
                    min = constraints[i];
                    index = i;
                }
            }
            return index;
        }
        #endregion
    }
}
