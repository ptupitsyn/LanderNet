using LanderNet.Game.Components;
using LanderNet.Game.GameObject;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LanderNet.Game.Tests
{
    [TestClass]
    public class CollisionTests
    {
        #region public methods

        [TestMethod]
        public void TestCollisionDetection()
        {
            var bullet = new Bullet().SetPosition(0, 0);
            var asteroid = new Asteroid().SetPosition(0, 0);
            Assert.IsTrue(CollisionComponent.AreCollided(bullet, asteroid));

            for (var i = 0; i < 100; i++)
            {
                bullet.SetPosition(i, i);
                Assert.IsTrue(CollisionComponent.AreCollided(bullet, asteroid));
                Assert.IsTrue(CollisionComponent.AreCollided(asteroid, bullet));
            }
        }

        #endregion
    }
}