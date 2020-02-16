using System;

namespace Devdog.Rucksack
{
    public sealed class SimpleShape2D : IShape2D
    {
        public int convexX { get; }
        public int convexY { get; }

        public SimpleShape2D()
            : this(1, 1)
        { }
        
        public SimpleShape2D(int xSize, int ySize)
        {
            convexX = Math.Max(1, xSize);
            convexY = Math.Max(1, ySize);
        }

        public bool IsBlocking(int x, int y)
        {
            return x <= convexX - 1 && x >= 0 &&
                   y <= convexY - 1 && y >= 0;
        }
    }
}