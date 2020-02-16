using System;
using System.Linq;
using UnityEngine;

namespace Devdog.Rucksack.Items
{
    [Serializable]
    public sealed class UnityShape2D : IShape2D, IEquatable<UnityShape2D>
    {
        [System.Serializable]
        private class Rows
        {
            public bool[] cols = new bool[0];
        }
        
        public int convexX
        {
            get { return _shape.Length; }
        }

        public int convexY
        {
            get { return _shape.Max(o => o.cols.Length); }
        }

        /// <summary>
        /// Defines the layoutShape of the object. If the bool is set to true it blocks, false hides.
        /// The array is also used to define the width and height of the item.
        /// </summary>
        [SerializeField]
        private Rows[] _shape;
        public bool[,] shape
        {
            get
            {
                var s = new bool[convexX, convexY];
                for (int x = 0; x < convexX; x++)
                {
                    for (int y = 0; y < convexY; y++)
                    {
                        s[x, y] = _shape[x].cols[y];
                    }
                }

                return s;
            }
        }

//        public UnityShape2D()
//            : this(1, 1)
//        { }

//        public UnityShape2D(int sizeX, int sizeY)
//        {
//            
//            for (int x = 0; x < sizeX; x++)
//            {
//                for (int y = 0; y < sizeY; y++)
//                {
//                    
//                }
//            }
//        }
        
        public UnityShape2D(bool[,] shape)
        {
            _shape = new Rows[shape.GetLength(0)];
            for (var i = 0; i < shape.GetLength(0); i++)
            {
                _shape[i] = new Rows
                {
                    cols = new bool[shape.GetLength(1)]
                };
            }
            
            for (int x = 0; x < shape.GetLength(0); x++)
            {
                for (int y = 0; y < shape.GetLength(1); y++)
                {
                    _shape[x].cols[y] = shape[x, y];
                }
            }
        }
        
        public static bool operator ==(UnityShape2D left, UnityShape2D right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(UnityShape2D left, UnityShape2D right)
        {
            return !Equals(left, right);
        }
        
        
        public bool IsBlocking(int x, int y)
        {
            return x <= convexX - 1 && x >= 0 &&
                   y <= convexY - 1 && y >= 0 &&
                   _shape[x].cols[y];
        }


        public bool Equals(UnityShape2D other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            if (other._shape.Length != _shape.Length) return false;
            
            // Compare contents of arrays
            for (int x = 0; x < _shape.Length; x++)
            {
                if (other._shape[x].cols.Length != _shape[x].cols.Length) return false;
                
                for (int y = 0; y < _shape[x].cols.Length; y++)
                {
                    if (other._shape[x].cols[y] != _shape[x].cols[y])
                    {
                        return false;
                    }
                }
            }

            return true;
//            return Equals(_shape, other._shape);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is UnityShape2D && Equals((UnityShape2D) obj);
        }

        public override int GetHashCode()
        {
            return (_shape != null ? _shape.GetHashCode() : 0);
        }
    }
}