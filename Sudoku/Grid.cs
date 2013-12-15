using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Sudoku
{
    class Grid
    {
        private int[,] grid;
        private int gridsize;

        //non-generalized
        public Grid(int gridsize)
        {
            this.gridsize = gridsize;
            grid = new int[gridsize * gridsize, gridsize * gridsize];
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    grid[i, j] = 0;
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

        public bool Valid()
        {
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    if (grid[i, j] != 0)
                    {
                        #region Rows
                        for (int k = 0; k < grid.GetLength(1); k++)
                        {
                            if ((k != j) && (grid[i, k] != 0) && (grid[i, j] == grid[i, k]))
                            {
                                return false;
                            }
                        }
                        #endregion
                        #region Columns
                        for (int k = 0; k < grid.GetLength(1); k++)
                        {
                            if ((k != i) && (grid[k, j] != 0) && (grid[i, j] == grid[k, j]))
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
                                if ((x != i && y != j) && (grid[x, y] != 0) && (grid[i, j] == grid[x, y]))
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

        public override string ToString()
        {
            string gridString = "";
            for (int x = 0; x < grid.GetLength(0); x++)
            {
                for (int y = 0; y < grid.GetLength(1); y++)
                {
                    gridString += grid[x, y];
                }
                gridString += "\n";
            }
            return gridString;
        }

        public class Point
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
                get { return x; }
                set { x = value; }
            }

            public int Y
            {
                get { return y; }
                set { y = value; }
            }
        }

        public void Set(int value, int row, int column)
        {
            grid[row, column] = value;
        }

        public int Get(int row, int column)
        {
            return grid[row, column];
        }

        public void ReadValuesFromFile(string filename)
        {
            string[] lines = File.ReadAllLines(filename);
            int row = 0;
            foreach (string line in lines)
            {
                int column = 0;
                foreach (char number in line)
                {
                    //chars are represented as ascii bytes, in order, 0 - 9.
                    //If I subtract the ascii for 0 from that value, I get the numerical value of that char.
                    Set((int)(number - '0'), row, column);
                    column++;
                }
                row++;
            }
        }
    }
}