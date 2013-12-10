using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sudoku
{
    class TheoreticalGrid
    {
        private bool[,,] grid;
        private int gridsize;

        public TheoreticalGrid(int gridsize)
        {
            this.gridsize = gridsize;
            grid = new bool[gridsize * gridsize, gridsize * gridsize, gridsize * gridsize];
            for(int i = 0; i < grid.GetLength(0); i++)
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

        public void AC3()
        {
        }

        public bool ArcReduce(Point p1, Point p2)
        {
            bool change = false;
            List<Point> peers = GetPeers(p1);
            if (!peers.Contains(p2))
            {
                return change;
            }
            for (int i = 0; i < grid.GetLength(3); i++)
            {
                bool existsSatisfyingAssignment = false;
                bool pCanBeX = grid[p1.X, p1.Y, i];
                if (pCanBeX)
                {
                    for (int j = 0; j < grid.GetLength(3); j++)
                    {
                        bool pCanBeY = grid[p2.X, p2.Y, j];
                        if(pCanBeY)
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
                    grid[p1.X, p1.Y, i] = false;
                    change = true;
                }
            }
            return change;
        }
    }

    class Point
    {
        private int x;
        private int y;

        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public int X
        {
            get {return x;}
            set {x = value;}
        }

        public int Y
        {
            get {return x;}
            set {x = value;}
        }
    }
}
