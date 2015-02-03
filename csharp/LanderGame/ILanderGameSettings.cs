using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace LanderNet.Game
{
    /// <summary>
    /// Describes game mechanics settings (speeds, health, etc).
    /// </summary>
    public interface ILanderGameSettings
    {
        double BulletSpeed { get; }
        double RocketAcceleration { get; }
    }
}
