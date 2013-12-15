using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sudoku
{
    class TheoreticalGrid
    {
        private HashSet<int>[,] grid;
        private int gridsize;

        public TheoreticalGrid(int gridsize)
        {
            this.gridsize = gridsize;
            grid = new HashSet<int>[gridsize * gridsize, gridsize * gridsize];
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    grid[i, j] = Utils.getAlphabet(gridsize);
                }
            }
        }

        public TheoreticalGrid(Grid givenGrid)
        {
            gridsize = givenGrid.Gridsize;
            grid = new HashSet<int>[givenGrid.Gridsize * givenGrid.Gridsize, givenGrid.Gridsize * givenGrid.Gridsize];
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    if (givenGrid.Get(i, j) != 0)
                    {
                        HashSet<int> set = new HashSet<int>();
                        set.Add(givenGrid.Get(i, j));
                        grid[i, j] = set;
                    }
                    else
                    {
                        grid[i, j] = Utils.getAlphabet(gridsize);
                    }
                }
            }
        }

        #region Getters and Setters
        public int Gridsize
        {
            get
            {
                return gridsize;
            }
        }
        #endregion

        public void Set(int value, int row, int column)
        {
            grid[row, column].Clear();
            grid[row, column].Add(value);
        }

        public HashSet<int> Get(int row, int column)
        {
            return grid[row, column];
        }

        public void Remove(int value, int row, int column)
        {
            grid[row, column].Remove(value);
        }

        public List<Point> GetPeers(Point p)
        {
            List<Point> peers = new List<Point>();

            for (int x = 0; x < grid.GetLength(0); x++)
            {
                if (x != p.X)
                {
                    peers.Add(new Point(x, p.Y));
                }
            }
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                if (y != p.Y)
                {
                    peers.Add(new Point(p.X, y));
                }
            }
            int xregion = (int)(p.X / gridsize);
            int yregion = (int)(p.Y / gridsize);
            for (int x = (xregion * gridsize); x < ((xregion * gridsize) + gridsize); x++)
            {
                for (int y = (yregion * gridsize); y < ((yregion * gridsize) + gridsize); y++)
                {
                    if (x != p.X && y != p.Y)
                    {
                        peers.Add(new Point(x, y));
                    }
                }
            }
            return peers;
        }

        public bool AC3()
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
            for (int x = 0; x < grid.GetLength(0); x++)
            {
                for (int y = 0; y < grid.GetLength(1); y++)
                {
                    Point currentPoint = new Point(x, y);
                    List<Point> peers = GetPeers(currentPoint);
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
                if (ArcReduce(currentArc))
                {
                    List<int> domainX = grid[currentArc.P1.X, currentArc.P1.Y].ToList();
                    if (domainX.Count == 0)
                    {
                        return false;
                    }
                    else
                    {
                        List<Point> peers = GetPeers(currentArc.P1);
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

        public bool ArcReduce(Arc arc)
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
            List<Point> peers = GetPeers(arc.P1);
            foreach (Point peer in peers)
            {
                if (peer.Equals(arc.P2))
                {
                    return false;
                }
            }
            List<int> domainX = grid[arc.P1.X, arc.P1.Y].ToList();
            for (int i = 0; i < domainX.Count; i++)
            {
                bool existsSatisfyingAssignment = false;
                int vx = domainX[i];

                List<int> domainY = grid[arc.P2.X, arc.P2.Y].ToList();
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
                    grid[arc.P1.X, arc.P1.Y].Remove(vx);
                    change = true;
                }
            }
            return change;
        }

        public override string ToString()
        {
            string gridString = "";
            for (int x = 0; x < grid.GetLength(0); x++)
            {
                for (int y = 0; y < grid.GetLength(1); y++)
                {
                    if (grid[x, y].Count == 1)
                    {
                        int[] gridArray = grid[x, y].ToArray();
                        gridString += grid[x, y].ToArray()[0];
                        //Extensibility? What's extensibility?
                        /*if (gridsize > 3 && grid[x, y].ToArray()[0] < 10)
                        {
                            toAdd += " ";
                        }*/
                    }
                    else
                    {
                        gridString += "0";
                    }
                }
                gridString += "\n";
            }
            return gridString;
        }

        public bool Valid()
        {
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    if (grid[i, j].Count == 0)
                    {
                        return false;
                    }
                    if (grid[i, j].Count == 1)
                    {
                        #region Rows
                        for (int k = 0; k < grid.GetLength(1); k++)
                        {
                            if ((k != j) && (grid[i, j] == grid[i, k]))
                            {
                                return false;
                            }
                        }
                        #endregion
                        #region Columns
                        for (int k = 0; k < grid.GetLength(1); k++)
                        {
                            if ((k != i) && (grid[i, j] == grid[k, j]))
                            {
                                return false;
                            }
                        }
                        #endregion
                        #region Regions
                        //not generalized
                        int xregion = (int)(i / gridsize);
                        int yregion = (int)(j / gridsize);
                        for (int x = (xregion * gridsize); x < ((xregion * gridsize) + gridsize); x++)
                        {
                            for (int y = (yregion * gridsize); y < ((yregion * gridsize) + gridsize); y++)
                            {
                                if ((x != i && y != j) && (grid[i, j] == grid[x, y]))
                                {
                                    return false;
                                }
                            }
                        }
                        #endregion
                    }
                }
            }
            return true;
        }
    }
}
