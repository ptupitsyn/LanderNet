using System.Linq;
using LanderNet.Game;
using LanderNet.Game.Components;
using LanderNet.Game.GameObject;
using LanderNet.UI.Components;
using LanderNet.UI.GameObjects;
using LanderNet.UI.Options;
using LanderNet.UI.Util;
using TpsGraphNet;

namespace LanderNet.UI.MainView
{
    /// <summary>
    /// Presentation logic for LanderGame: Tracks game events and objects, adds visuals and sounds accordingly; performs rendering.
    /// </summary>
    internal class LanderGamePresenter
    {
        public LanderGamePresenter(LanderGame landerGame, OptionsViewModel options)
        {
            _landerGame = landerGame;
            _options = options;
            landerGame.GameObjectAdded += LanderGame_GameObjectAdded;
            landerGame.GameObjectDestroyed += LanderGame_GameObjectDestroyed;
        }

        public void Render(Sprite renderTarget)
        {
            RenderBackground(renderTarget);
            RenderGameObjects(renderTarget);
        }

        public void ResetBackground()
        {
            _stars.Reset();
        }

        private void RenderBackground(Sprite renderTarget)
        {
            var shipSpeed =
                _landerGame.GameObjects.OfType<Spaceship>()
                    .Select(x => x.GetComponent<LinearMovementComponent>())
                    .FirstOrDefault();

            if (shipSpeed == null)
                return;

            if (!_landerGame.IsPaused)
            {
                _stars.Move(shipSpeed.XSpeed, shipSpeed.YSpeed, renderTarget.Width, renderTarget.Height, _landerGame.SecondsPassed);
            }

            _stars.Render(renderTarget);
        }

        private void RenderGameObjects(Sprite renderTarget)
        {
            var ssPos = _landerGame.GameObjects.OfType<Spaceship>().Single().GetComponent<PositionComponent>();

            // Order: Z-Index, then newer objects on top (thus Reverse)
            foreach (var gameObject in _landerGame.GameObjects.Reverse().OrderBy(x => x.GetZIndex()))
            {
                var pos = gameObject.GetComponent<PositionComponent>();
                if (pos == null) continue;
                var size = gameObject.GetComponent<SizeComponent>() ?? new SizeComponent();

                var relativeX = pos.X - ssPos.X;
                if (relativeX < -renderTarget.Width || relativeX > renderTarget.Width) continue;

                var relativeY = pos.Y - ssPos.Y;
                if (relativeY < -renderTarget.Height || relativeY > renderTarget.Height) continue;

                foreach (var sprite in gameObject.GetComponents<AnimatedSpriteComponent>().Where(x => x.IsVisible))
                {
                    // Ship is always centered horizontally and 15px from the bottom border
                    var renderX = (double)renderTarget.Width / 2 + relativeX - 50 + sprite.OffsetX;
                    var renderY = (double)renderTarget.Height - 15 - relativeY + sprite.OffsetY - size.Height;

                    renderTarget.PutSprite((uint)renderX, (uint)renderY, sprite.GetCurrentSprite(), sprite.BlendMode);
                }
            }
        }

        private static void AddSprites(IGameObject obj)
        {
            var objectType = obj.GetType();

            if (objectType == typeof(Spaceship))
            {
                // Fire
                var movementComponent = obj.GetComponent<KeyboardControlledMovementComponent>();
                obj.AddComponent(new AnimatedSpriteComponent("F", 9, 50, 10)
                {
                    OffsetX = 35,
                    OffsetY = 60,
                    VisibilityFunc = () => movementComponent != null && movementComponent.IsAccelerating,
                    BlendMode = TpsGraphWrapper.BlendMode.Additive
                });

                // Ship
                obj.SetZIndex(100).AddComponent(new AnimatedSpriteComponent("S", 49, 50, 2, 2));
            }

            if (objectType == typeof(Asteroid))
            {
                var asteroidPrefix = new[] { "Z1", "Z2", "Z3", "Z4", "Z5", "Z6" }.GetRandomItem();
                var size = obj.GetComponent<SizeComponent>();
                obj.AddComponent(new AnimatedSpriteComponent(asteroidPrefix, 20, RandomHelper.Instance.Next(15, 40), 5,
                    decodePixelWidth: (int)size.Width,
                    decodePixelHeight: (int)size.Height));
            }

            if (objectType == typeof(Crate))
            {
                obj.SetZIndex(1)
                    .AddComponent(new AnimatedSpriteComponent("C", 17, RandomHelper.Instance.Next(15, 40), 6,
                        decodePixelWidth: 50, decodePixelHeight: 50));
            }

            if (objectType == typeof(Rocket))
            {
                obj.SetZIndex(2).AddComponent(new AnimatedSpriteComponent("R", 19, 20, 5));
            }

            if (objectType == typeof(Bullet))
            {
                obj.SetZIndex(3).AddComponent(new AnimatedSpriteComponent("B", 1));
            }
        }

        private void LanderGame_GameObjectAdded(IGameObject obj)
        {
            AddSprites(obj);
            PlaySound(obj);
        }

        private void LanderGame_GameObjectDestroyed(IGameObject gameObject)
        {
            if (gameObject.GetType() == typeof(Asteroid) || gameObject.GetType() == typeof(Crate))
            {
                var explosion = new Explosion(gameObject).SetZIndex(200);
                _landerGame.AddGameObject(explosion);
                PlaySound(explosion);

                // Add debris
                if (gameObject.GetType() == typeof(Asteroid))
                {
                    Enumerable.Range(0, RandomHelper.Instance.Next(_options.DebrisLimit / 3, _options.DebrisLimit))
                        .Select(x => Debris.Create(gameObject))
                        .ToList()
                        .ForEach(_landerGame.AddGameObject);
                }
            }

            if (gameObject.GetType() == typeof(Bullet))
            {
                _landerGame.AddGameObject(new Explosion(gameObject, 0.2, true).SetZIndex(200));
            }
        }

        private void PlaySound(IGameObject gameObject)
        {
            if (!_options.EnableSound) return;

            if (gameObject is Rocket)
            {
                _sound.RocketLaunch();
            }
            else if (gameObject is Bullet)
            {
                _sound.MachineGun();
            }
            else if (gameObject is Explosion)
            {
                _sound.Explosion(gameObject, _landerGame.GameObjects.OfType<Spaceship>().First());
            }
        }


        private readonly LanderGame _landerGame;
        private readonly OptionsViewModel _options;
        private readonly SoundHelper _sound = new SoundHelper();
        private readonly StarBackground _stars = new StarBackground();
    }
}
