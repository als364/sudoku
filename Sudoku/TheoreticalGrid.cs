using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Sudoku
{
    class TheoreticalGrid
    {
        private HashSet<int>[,] grid;
        private int gridsize;
        private int gridsizeSquared;
        //private int backtracks;

        public TheoreticalGrid(int gridsize)
        {
            this.gridsize = gridsize;
            this.gridsizeSquared = gridsize * gridsize;
            grid = new HashSet<int>[gridsize * gridsize, gridsize * gridsize];
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    grid[i, j] = Utils.getAlphabet(gridsize);
                }
            }
        }

        public TheoreticalGrid(TheoreticalGrid givenTheoreticalGrid)
        {
            gridsize = givenTheoreticalGrid.gridsize;
            gridsizeSquared = givenTheoreticalGrid.gridsizeSquared;
            grid = new HashSet<int>[gridsizeSquared,gridsizeSquared];
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    HashSet<int> set = new HashSet<int>();
                    foreach (int value in givenTheoreticalGrid.Get(i, j))
                    {
                        set.Add(value);
                    }
                    grid[i, j] = set;
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
        public int GridsizeSquared
        {
            get
            {
                return gridsizeSquared;
            }
        }
        public HashSet<int>[,] Grid
        {
            get
            {
                return grid;
            }
        }
        #endregion

        public TheoreticalGrid Clone()
        {
            return new TheoreticalGrid(this);
        }

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

        public bool Complete(HashSet<int>[,] gridToCheck)
        {
            for (int i = 0; i < gridToCheck.GetLength(0); i++)
            {
                for (int j = 0; j < gridToCheck.GetLength(1); j++)
                {
                    if (gridToCheck[i, j].Count == 0)
                    {
                        return false;
                    }
                    if (gridToCheck[i, j].Count == 1)
                    {
                        #region Rows
                        for (int k = 0; k < gridToCheck.GetLength(1); k++)
                        {
                            if ((k != j) && (gridToCheck[i, j] == gridToCheck[i, k]))
                            {
                                return false;
                            }
                        }
                        #endregion
                        #region Columns
                        for (int k = 0; k < gridToCheck.GetLength(1); k++)
                        {
                            if ((k != i) && (gridToCheck[i, j] == gridToCheck[k, j]))
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
                                if ((x != i && y != j) && (gridToCheck[i, j] == gridToCheck[x, y]))
                                {
                                    return false;
                                }
                            }
                        }
                        #endregion
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public bool Complete()
        {
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    /*if (grid[i, j].Count > 1 || grid[i, j].Count < 0)
                    {
                        return false;
                    }*/
                    if (grid[i, j].Count != 1)
                    {
                        return false;
                    }
                }
            }
            return true;
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
                        int value = grid[i, j].ToArray()[0];
                        List<Point> peers = GetPeers(new Point(i, j));
                        foreach (Point peer in peers)
                        {
                            if (grid[peer.X, peer.Y].Count == 1)
                            {
                                int peerValue = grid[peer.X, peer.Y].ToArray()[0];
                                if (value == peerValue)
                                {
                                    return false;
                                }
                            }
                        }
                    }
                }
            }
            return true;
        }

        public void ReadValuesFromFile(string filename)
        {
            string[] lines = System.IO.File.ReadAllLines(filename);

            int row = 0;
            foreach (string line in lines)
            {
                int column = 0;
                foreach (char number in line)
                {
                    //chars are represented as ascii bytes, in order, 0 - 9.
                    //If I subtract the ascii for 0 from that value, I get the numerical value of that char.
                    if (number != '0')
                    {
                        Set((int)(number - '0'), row, column);
                    }
                    else
                    {
                        grid[row, column] = Utils.getAlphabet(gridsize);
                    }
                    column++;
                }
                row++;
            }
        }

        public void WriteValuesToFile(string filename, TimeSpan duration)
        {
            string values = this.ToString();
            if (this.Valid())
            {
                values += "valid and ";
            }
            else
            {
                values += "INVALID and ";
            }
            if (this.Complete())
            {
                values += "complete\n";
            }
            else
            {
                values += "INCOMPLETE\n";
            }
            values += duration;
            string[] lines = values.Split(new char[] { '\n' });
            System.IO.File.WriteAllLines(filename, lines);
        }
    }
}
