using System;

namespace TetrisGame
{
    public class Board
    {
        public const int Width = 10;
        public const int Height = 20;

        private readonly int[,] grid;

        public Board()
        {
            grid = new int[Height, Width];
        }

        public int this[int row, int col]
        {
            get
            {
                if (row < 0 || row >= Height || col < 0 || col >= Width)
                    return 1;
                return grid[row, col];
            }
        }

        public bool CanPlace(Figure figure, int x, int y)
        {
            for (int row = 0; row < figure.Height; row++)
            {
                for (int col = 0; col < figure.Width; col++)
                {
                    if (figure.Shape[row, col] == 1)
                    {
                        int boardRow = y + row;
                        int boardCol = x + col;

                        if (boardRow < 0 || boardRow >= Height ||
                            boardCol < 0 || boardCol >= Width ||
                            grid[boardRow, boardCol] == 1)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        public void PlaceFigure(Figure figure, int x, int y)
        {
            for (int row = 0; row < figure.Height; row++)
            {
                for (int col = 0; col < figure.Width; col++)
                {
                    if (figure.Shape[row, col] == 1)
                    {
                        int boardRow = y + row;
                        int boardCol = x + col;
                        if (boardRow >= 0 && boardRow < Height && boardCol >= 0 && boardCol < Width)
                        {
                            grid[boardRow, boardCol] = 1;
                        }
                    }
                }
            }
        }

        public int ClearFullRows()
        {
            int cleared = 0;
            for (int row = Height - 1; row >= 0;)
            {
                if (IsRowFull(row))
                {
                    RemoveRow(row);
                    cleared++;
                }
                else
                {
                    row--;
                }
            }
            return cleared;
        }

        private bool IsRowFull(int row)
        {
            for (int col = 0; col < Width; col++)
            {
                if (grid[row, col] == 0)
                    return false;
            }
            return true;
        }

        private void RemoveRow(int row)
        {
            for (int r = row; r > 0; r--)
            {
                for (int c = 0; c < Width; c++)
                {
                    grid[r, c] = grid[r - 1, c];
                }
            }
            for (int c = 0; c < Width; c++)
            {
                grid[0, c] = 0;
            }
        }

        public bool IsGameOver()
        {
            for (int col = 0; col < Width; col++)
            {
                if (grid[0, col] == 1)
                    return true;
            }
            return false;
        }

        public void Clear()
        {
            for (int row = 0; row < Height; row++)
            {
                for (int col = 0; col < Width; col++)
                {
                    grid[row, col] = 0;
                }
            }
        }
    }
}