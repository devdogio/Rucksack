using System;

namespace Devdog.Rucksack
{
    public sealed class ComplexShape2D : IShape2D
    {
        public int convexX
        {
            get { return _shape.GetLength(0); }
        }

        public int convexY
        {
            get { return _shape.GetLength(1); }
        }

        /// <summary>
        /// Defines the shape of the object. If the bool is set to true it blocks, false hides.
        /// The array is also used to define the width and height of the item.
        /// </summary>
        private readonly bool[,] _shape;

        public ComplexShape2D()
            : this(new bool[1,1]{ {true} })
        { }
        
        public ComplexShape2D(bool[,] shape)
        {
            _shape = shape;

            if (convexX <= 0 || convexY <= 0)
            {
                throw new ArgumentException("Jagged array has to be at least 1x1!");
            }
        }

        public bool IsBlocking(int x, int y)
        {
            return x <= convexX - 1 && x >= 0 &&
                   y <= convexY - 1 && y >= 0 &&
                   _shape[x, y];
        }
    }
}