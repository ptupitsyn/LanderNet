using System;
using System.Collections.Generic;

namespace LanderNet.UI.Util
{
    public static class RandomHelper
    {
        public static T GetRandomItem<T>(this IList<T> items)
        {
            return items[Instance.Next(items.Count)];
        }

        public static bool YesOrNo()
        {
            return Instance.Next(2) > 0;
        }

        public static readonly Random Instance = new Random();
    }
}