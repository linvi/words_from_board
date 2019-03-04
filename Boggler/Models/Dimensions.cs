﻿namespace Boggler.Models
{
    public class Dimensions
    {
        public Dimensions(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; }
        public int Y { get; }

        public override string ToString()
        {
            return $"({X}, {Y})";
        }
    }
}
