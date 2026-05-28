using System;
using System.Collections.Generic;

namespace TetrisGame
{
    public class Figure
    {
        public int[,] Shape { get; private set; }

        public int Width => Shape.GetLength(1);
        public int Height => Shape.GetLength(0);

        public Figure(int[,] shape)
        {
            Shape = shape;
        }

        public Figure Clone()
        {
            int[,] clonedShape = (int[,])Shape.Clone();
            return new Figure(clonedShape);
        }

        public Figure Rotate()
        {
            int h = Height;
            int w = Width;
            int[,] rotated = new int[w, h];

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    rotated[x, h - 1 - y] = Shape[y, x];
                }
            }
            return new Figure(rotated);
        }

        // I
        public static Figure CreateI() => new Figure(new int[,]
        {
            { 0,0,0,0 },
            { 0,0,0,0 },
            { 1,1,1,1 },
            { 0,0,0,0 }
        });

        // O
        public static Figure CreateO() => new Figure(new int[,]
        {
            { 0,0,0,0 },
            { 0,1,1,0 },
            { 0,1,1,0 },
            { 0,0,0,0 }
        });

        // T
        public static Figure CreateT() => new Figure(new int[,]
        {
            { 0,0,0,0 },
            { 0,1,0,0 },
            { 1,1,1,0 },
            { 0,0,0,0 }
        });

        // S
        public static Figure CreateS() => new Figure(new int[,]
        {
            { 0,0,0,0 },
            { 0,1,1,0 },
            { 1,1,0,0 },
            { 0,0,0,0 }
        });

        // Z
        public static Figure CreateZ() => new Figure(new int[,]
        {
            { 0,0,0,0 },
            { 1,1,0,0 },
            { 0,1,1,0 },
            { 0,0,0,0 }
        });

        // L
        public static Figure CreateL() => new Figure(new int[,]
        {
            { 0,0,0,0 },
            { 1,0,0,0 },
            { 1,1,1,0 },
            { 0,0,0,0 }
        });

        // J
        public static Figure CreateJ() => new Figure(new int[,]
        {
            { 0,0,0,0 },
            { 0,0,1,0 },
            { 1,1,1,0 },
            { 0,0,0,0 }
        });

        private static readonly Random random = new Random();
        public static Figure GetRandomFigure()
        {
            int type = random.Next(7);
            return type switch
            {
                0 => CreateI(),
                1 => CreateO(),
                2 => CreateT(),
                3 => CreateS(),
                4 => CreateZ(),
                5 => CreateL(),
                6 => CreateJ(),
                _ => CreateI()
            };
        }
    }
}