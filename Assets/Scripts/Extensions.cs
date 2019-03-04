using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Extensions
{
    public static class ArrayExtension
    {
        private static Random r = new Random();

        public static T GetOne<T>(this T[] _array)
        {
            return _array[r.Next(0, _array.Length)];
        }
    }
}
