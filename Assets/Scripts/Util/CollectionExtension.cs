using System;
using System.Collections.Generic;

namespace Util
{
    public static class CollectionExtension
    {
        private static Random Rand = new Random();

        public static T RandomElement<T>(this T[] items)
        {
            return items[Rand.Next(0, items.Length)];
        }

        public static T RandomElement<T>(this List<T> items)
        {
            return items[Rand.Next(0, items.Count)];
        }
    }
}