using System;
using System.Collections.Generic;
using System.Linq;
using LanderNet.Game.Components;
using LanderNet.Game.GameObject;
using LanderNet.Game.Stages;
using LanderNet.Game.Util;

namespace LanderNet.Game
{
    public sealed class LanderGame
    {
        public LanderGame()
        {
            _score = new ScoreCounter(this);
        }

        public event Action<IGameObject> AsteroidShot;
        public event Action<IGameObject> CrateCollected;
        public event Action<IGameObject> GameObjectAdded;
        public event Action<IGameObject> GameObjectDestroyed;

        public IEnumerable<IGameObject> GameObjects
        {
            get { return _gameObjects; }
        }

        public int Score
        {
            get { return _score.Score; }
        }

        public double SecondsPassed
        {
            get { return _currentSecond - _previousGameLoopIterationSecond; }
        }

        public IStage Stage
        {
            get { return _stage; }
        }

        public bool IsPaused { get; private set; }
        
        public bool IsGameOver { get; private set; }

        public void AddGameObject(IGameObject obj)
        {
            if (obj == null) throw new ArgumentNullException();
            _gameObjects.Add(obj);
            OnGameObjectAdded(obj);
        }

        /// <summary>
        /// Runs a game loop once.
        /// </summary>
        public void GameLoopImpl()
        {
            if (IsPaused)
                return;

            // http://gafferongames.com/game-physics/fix-your-timestep/
            _previousGameLoopIterationSecond = _currentSecond;
            _currentSecond = GetCurrentSecond() - _pauseCompensationSeconds;
            AddAsteroids();
            AddCrates();
            ProcessExpiredAndAwayObjects();
            ProcessTimedObjects();
        }

        public void Pause()
        {
            if (IsPaused)
                return;

            _pausedSecond = _currentSecond;
            IsPaused = true;
        }

        public void ResetGame()
        {
            IsGameOver = false;
            _gameObjects.Clear();
            AddSpaceShip();

            _stage = new SurvivalStage();
            _score.Reset();
        }

        public void Resume()
        {
            if (!IsPaused)
                return;

            if (IsGameOver)
            {
                ResetGame();
            }

            _pauseCompensationSeconds = GetCurrentSecond() - _pausedSecond;
            IsPaused = false;
        }

        /// <summary>
        /// Adds the incoming asteroids on the ship path.
        /// </summary>
        private void AddAsteroids()
        {
            var timePassed = _currentSecond - _previousAsteroidSecond;
            var asteroidsPerSecond = Stage.AsteroidsPerSecond;

            if (timePassed*asteroidsPerSecond > 1)
            {
                var shipPos = _spaceship.GetComponent<PositionComponent>();
                var shipSpeed = _spaceship.GetComponent<LinearMovementComponent>();
                const int distanceY = 1300;
                const int distanceX = 4000;

                var timeToReach = distanceY/shipSpeed.YSpeed;
                // this is time when we'll reach this asteroid in Y coordinate
                var speedOffsetX = shipSpeed.XSpeed*timeToReach*1.2;
                for (int i = 0; i < timePassed*asteroidsPerSecond; i++)
                {
                    var asteroid =
                        new Asteroid().SetPosition(shipPos.X + Random.Next(distanceX) - distanceX/2 + speedOffsetX,
                            shipPos.Y + distanceY).SetSpeed(Random.Next(200) - 100, -Random.Next(200));

                    // Set random size
                    var size = asteroid.GetComponent<SizeComponent>();
                    size.Width = size.Height = 80 + ((_currentSecond + i)%4)*10;

                    // Health is proportional to size
                    asteroid.GetComponent<HealthComponent>().Health = size.Width*size.Height/100;

                    AddGameObject(asteroid);
                }
                _previousAsteroidSecond = _currentSecond;
            }
        }

        private void AddCrates()
        {
            var cratesPerSecond = Stage.CratesPerSecond;
            var timePassed = _currentSecond - _previousCrateSecond;
            if (timePassed*cratesPerSecond > 1)
            {
                var shipPos = _spaceship.GetComponent<PositionComponent>();
                const int distanceY = 1300;
                const int distanceX = 4000;
                for (var i = 0; i < timePassed*cratesPerSecond; i++)
                {
                    var crate =
                        new Crate().SetPosition(shipPos.X + Random.Next(distanceX) - distanceX/2, shipPos.Y + distanceY)
                            .SetSpeed(Random.Next(200) - 100, -Random.Next(200));
                    AddGameObject(crate);
                }
                _previousCrateSecond = _currentSecond;
            }
        }

        private void AddSpaceShip()
        {
            _spaceship = new Spaceship(OnMachinegunFire, OnRocketFire);

            var collisionComponent = new CollisionComponent(_spaceship, this, typeof (Asteroid))
            {
                Threshold = 20
            };
            collisionComponent.Collided += (component, gameObject) => OnAsteroidCollision(gameObject);
            _spaceship.AddComponent(collisionComponent);

            var crateCollisionComponent = new CollisionComponent(_spaceship, this, typeof (Crate))
            {
                Threshold = 20
            };
            crateCollisionComponent.Collided += (component, gameObject) => OnCrateCollected(gameObject);
            _spaceship.AddComponent(crateCollisionComponent);

            AddGameObject(_spaceship);
        }

        private T CreateProjectile<T>() where T : IGameObject, new()
        {
            var projectile = new T();

            var collision = new CollisionComponent(projectile, this, typeof (Asteroid), typeof (Crate))
            {
                Threshold = 15
            };
            collision.Collided += (component, gameObject) => OnProjectileHit(gameObject, projectile);
            projectile.AddComponent(collision);

            AddGameObject(projectile);
            return projectile;
        }

        private void DestroyObject(IGameObject gameObject)
        {
            _gameObjects.Remove(gameObject);
            OnGameObjectDestroyed(gameObject);
        }

        private void GameOver()
        {
            IsGameOver = true;
            Pause();
        }

        private static double GetCurrentSecond()
        {
            // Use _minTick to avoid huge numbers
            var ticks = DateTime.Now.Ticks - MinTick;
            return ((double) ticks)/(10000*1000);
        }

        private static bool IsFlyByObject(IGameObject gameObject, PositionComponent shipPos)
        {
            var pos = gameObject.GetComponent<PositionComponent>();
            if (pos == null) return false;

            return pos.Y < shipPos.Y - 300 || pos.Y > shipPos.Y + 3000;
        }

        private void OnAsteroidCollision(IGameObject asteroid)
        {
            if (asteroid.GetHealth() > 0)
            {
                _spaceship.AddHealth(-asteroid.GetHealth()/2);

                if (_spaceship.GetHealth() < 0)
                {
                    GameOver();
                }
            }
            DestroyObject(asteroid);
        }

        private void OnAsteroidShot(IGameObject obj)
        {
            var handler = AsteroidShot;
            if (handler != null) handler(obj);
        }

        private void OnCrateCollected(IGameObject crate)
        {
            _spaceship.ProcessPowerup(crate);
            _gameObjects.Remove(crate);

            var handler = CrateCollected;
            if (handler != null) handler(crate);
        }

        private void OnGameObjectAdded(IGameObject obj)
        {
            var handler = GameObjectAdded;
            if (handler != null) handler(obj);
        }

        private void OnGameObjectDestroyed(IGameObject obj)
        {
            var handler = GameObjectDestroyed;
            if (handler != null) handler(obj);
        }

        private void OnMachinegunFire()
        {
            var level = _spaceship.MachinegunLevel;

            var shipPos = _spaceship.GetComponent<PositionComponent>();
            var shipSpeed = _spaceship.GetComponent<LinearMovementComponent>();

            switch (level)
            {
                case 1:
                    CreateProjectile<Bullet>()
                        .SetSpeed(shipSpeed.XSpeed + Random.Next(20) - 10, shipSpeed.YSpeed + 900)
                        .SetPosition(shipPos.X + 48, shipPos.Y + 55);
                    break;
                case 2:
                    CreateProjectile<Bullet>()
                        .SetSpeed(shipSpeed.XSpeed + Random.Next(20) - 100, shipSpeed.YSpeed + 700)
                        .SetPosition(shipPos.X + 38, shipPos.Y + 55);
                    CreateProjectile<Bullet>()
                        .SetSpeed(shipSpeed.XSpeed - Random.Next(20) + 100, shipSpeed.YSpeed + 700)
                        .SetPosition(shipPos.X + 58, shipPos.Y + 55);
                    break;
                default:
                    CreateProjectile<Bullet>()
                        .SetSpeed(shipSpeed.XSpeed + Random.Next(20) - 10, shipSpeed.YSpeed + 900)
                        .SetPosition(shipPos.X + 48, shipPos.Y + 55);
                    CreateProjectile<Bullet>()
                        .SetSpeed(shipSpeed.XSpeed + Random.Next(20) - 100, shipSpeed.YSpeed + 700)
                        .SetPosition(shipPos.X + 38, shipPos.Y + 55);
                    CreateProjectile<Bullet>()
                        .SetSpeed(shipSpeed.XSpeed - Random.Next(20) + 100, shipSpeed.YSpeed + 700)
                        .SetPosition(shipPos.X + 58, shipPos.Y + 55);
                    break;
            }
        }

        private void OnProjectileHit(IGameObject gameObject, IGameObject projectile)
        {
            gameObject.AddHealth(-projectile.GetHealth());

            if (gameObject.GetHealth() <= 0)
            {
                // Explode game object
                // TODO: Split asteroid into smaller when it is bigger than some threshold?
                DestroyObject(gameObject);
                OnAsteroidShot(gameObject);
            }
            else
            {
                // Slow down game object
                var healthRatio = projectile.GetHealth()/3000;
                var projectileSpeed = projectile.GetComponent<LinearMovementComponent>();
                var objSpeed = gameObject.GetComponent<LinearMovementComponent>();
                objSpeed.YSpeed += projectileSpeed.YSpeed*healthRatio;
                objSpeed.XSpeed += projectileSpeed.XSpeed*healthRatio;
            }

            projectile.SetSpeed(0, 0);
            DestroyObject(projectile);
        }

        private void OnRocketFire()
        {
            var level = _spaceship.RocketLevel;

            if (level < 1)
                return;

            var shipPos = _spaceship.GetComponent<PositionComponent>();
            var shipSpeed = _spaceship.GetComponent<LinearMovementComponent>();

            if (level == 1)
            {
                CreateProjectile<Rocket>()
                    .SetSpeed(shipSpeed.XSpeed, shipSpeed.YSpeed, yAcceleration: 1500)
                    .SetPosition(shipPos.X + 40, shipPos.Y + 43);
            }
            else
            {
                CreateProjectile<Rocket>()
                    .SetSpeed(shipSpeed.XSpeed - 30, shipSpeed.YSpeed, yAcceleration: 1500)
                    .SetPosition(shipPos.X + 30, shipPos.Y + 43);
                CreateProjectile<Rocket>()
                    .SetSpeed(shipSpeed.XSpeed + 30, shipSpeed.YSpeed, yAcceleration: 1500)
                    .SetPosition(shipPos.X + 50, shipPos.Y + 43);
            }
        }

        /// <summary>
        /// Remove expired objects and those that flew away.
        /// </summary>
        private void ProcessExpiredAndAwayObjects()
        {
            var shipPos = _spaceship.GetComponent<PositionComponent>();

            // AsParallel seem to be slower in most cases (except when there are A LOT of objects)
            var objs = GameObjects
                //.AsParallel()
                .Where(x => x.IsExpired() || IsFlyByObject(x, shipPos)).ToArray();
            foreach (var toRemove in objs)
            {
                _gameObjects.Remove(toRemove);
            }
        }

        private void ProcessTimedObjects()
        {
            // Calculate average frame duration for smoothness
            // Sometimes individual frames take longer than usual, which can lead to visual "jerks"
            // Save difference between actual dt and average dt in _leftoverSeconds so that simulation is still correct

            // TODO: Extract this into a class
            var actualSecondsPassed = _currentSecond - _previousGameLoopIterationSecond + _leftoverSeconds;
            var secondsPassed = _secondsPerFrameAverage.Add(actualSecondsPassed);
            _leftoverSeconds = actualSecondsPassed - secondsPassed;
            if (_leftoverSeconds < 0)
                _leftoverSeconds = 0;

            _stage.Update(secondsPassed);

            // TODO: Parallel processing of debris (IsBackgroundObject or something)
            foreach (var obj in GameObjects.ToArray())
            {
                obj.UpdateOnTime(secondsPassed);
            }
        }

        private static readonly long MinTick = DateTime.Now.Ticks;
        private static readonly Random Random = new Random();

        private readonly List<IGameObject> _gameObjects = new List<IGameObject>();
        private readonly ScoreCounter _score;
        private readonly MovingAverage _secondsPerFrameAverage = new MovingAverage(50);

        private double _currentSecond;
        private double _leftoverSeconds;
        private double _pauseCompensationSeconds;
        private double _pausedSecond;
        private double _previousAsteroidSecond;
        private double _previousCrateSecond;
        private double _previousGameLoopIterationSecond = double.MaxValue;

        private Spaceship _spaceship;
        private IStage _stage = new SurvivalStage();
    }
}