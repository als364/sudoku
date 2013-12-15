using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sudoku
{
    class TheoreticalGrid
    {
        private bool[, ,] grid;
        private int gridsize;

        public TheoreticalGrid(int gridsize)
        {
            this.gridsize = gridsize;
            grid = new bool[gridsize * gridsize, gridsize * gridsize, gridsize * gridsize];
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    for (int k = 0; k < grid.GetLength(2); k++)
                    {
                        grid[i, j, k] = true;
                    }
                }
            }
        }

        public TheoreticalGrid(Grid givenGrid)
        {
            gridsize = givenGrid.Gridsize;
            grid = new bool[givenGrid.Gridsize * givenGrid.Gridsize, givenGrid.Gridsize * givenGrid.Gridsize, givenGrid.Gridsize * givenGrid.Gridsize];
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    if (givenGrid.Get(i, j) != 0)
                    {
                        for (int k = 0; k < grid.GetLength(2); k++)
                        {
                            if (k == givenGrid.Get(i, j))
                            {
                                grid[i, j, k] = true;
                            }
                            else
                            {
                                grid[i, j, k] = false;
                            }
                        }
                    }
                    else
                    {
                        for (int k = 0; k < grid.GetLength(2); k++)
                        {
                            grid[i, j, k] = true;
                        }
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

        public void Set(bool value, int row, int column, int index)
        {
            grid[row, column, index] = value;
        }

        public bool Get(int row, int column, int index)
        {
            return grid[row, column, index];
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

            while (worklist.Count > 0)
            {
                Arc currentArc = worklist.Dequeue();
                if (ArcReduce(currentArc))
                {
                    bool hasValues = false;
                    for (int i = 0; i < grid.GetLength(2); i++)
                    {
                        if (grid[currentArc.P1.X, currentArc.P2.X, i])
                        {
                            hasValues = true;
                        }
                        if (!hasValues)
                        {
                            return false;
                        }
                        else
                        {
                            for (int x = 0; x < grid.GetLength(0); x++)
                            {
                                for (int y = 0; y < grid.GetLength(1); y++)
                                {
                                    Point currentPoint = new Point(x, y);
                                    List<Point> peers = GetPeers(currentPoint);
                                    foreach (Point peer in peers)
                                    {
                                        if (peer != currentArc.P2)
                                        {
                                            worklist.Enqueue(new Arc(peer, currentPoint));
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return true;
        }

        public bool ArcReduce(Arc arc)
        {
            bool change = false;
            List<Point> peers = GetPeers(arc.P1);
            foreach (Point peer in peers)
            {
                if(peer.Equals(arc.P2))
                {
                    return false;
                }
            }
            for (int i = 0; i < grid.GetLength(2); i++)
            {
                bool existsSatisfyingAssignment = false;
                bool pCanBeX = grid[arc.P1.X, arc.P1.Y, i];
                if (pCanBeX)
                {
                    for (int j = 0; j < grid.GetLength(2); j++)
                    {
                        bool pCanBeY = grid[arc.P2.X, arc.P2.Y, j];
                        if (pCanBeY)
                        {
                            if (i != j)
                            {
                                existsSatisfyingAssignment = true;
                            }
                        }
                    }
                }
                if (!existsSatisfyingAssignment)
                {
                    grid[arc.P1.X, arc.P1.Y, i] = false;
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
                    string toAdd = "";
                    for (int z = 0; z < grid.GetLength(2); z++)
                    {
                        if (grid[x, y, z])
                        {
                            toAdd += z.ToString();
                        }
                    }
                    if (toAdd.Length == 1)
                    {
                        gridString += toAdd;
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
    }
}
