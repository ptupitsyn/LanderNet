using System;

namespace LanderNet.Game.Components
{
    public static class PositionComponentExtensions
    {
        public static double GetDistance(this PositionComponent x, PositionComponent y)
        {
            return Math.Sqrt(Math.Pow(x.X - y.X, 2) + Math.Pow((x.Y - y.Y), 2));
        }
    }
}