using System;
using System.Diagnostics;
using System.Linq;
using LanderNet.Game;
using LanderNet.UI.Util;

namespace LanderNet.UI.MainView
{
    /// <summary>
    /// ViewModel for FPS and other developer information.
    /// </summary>
    internal class DevInfoViewModel : NotifierBase
    {
        public DevInfoViewModel(LanderGame landerGame)
        {
            _landerGame = landerGame;
        }

        public double CompositionTime
        {
            get { return _compositionTime; }
            set
            {
                _compositionTime = value;
                OnPropertyChanged("CompositionTime");
            }
        }

        public double FPS
        {
            get { return _fps; }
            private set
            {
                _fps = value;
                OnPropertyChanged("FPS");
            }
        }

        public double GameLoopTime
        {
            get { return _gameLoopTime; }
            set
            {
                _gameLoopTime = value;
                OnPropertyChanged("GameLoopTime");
            }
        }

        public double ObjectCount
        {
            get { return _landerGame.GameObjects.Count(); }
        }

        public void MeasureCompositionTime(Action composeAction)
        {
            var sw = Stopwatch.StartNew();
            composeAction();
            _composeRenderCounter += sw.ElapsedTicks;
        }

        public void MeasureGameLoopTime(Action gameLoopAction)
        {
            var sw = Stopwatch.StartNew();
            gameLoopAction();
            _gameLoopCounter += sw.ElapsedTicks;
        }

        public void UpdateFps()
        {
            _frameCounter++;
            if (_frameCounter == FrameBatchSize)
            {
                FPS = FrameBatchSize / (DateTime.Now - _lastFpsTime).TotalSeconds;
                _frameCounter = 0;

                GameLoopTime = (double)_gameLoopCounter / FrameBatchSize / TimeSpan.TicksPerMillisecond;
                _gameLoopCounter = 0;

                CompositionTime = (double)_composeRenderCounter / FrameBatchSize / TimeSpan.TicksPerMillisecond;
                _composeRenderCounter = 0;

                _lastFpsTime = DateTime.Now;
                OnPropertyChanged("ObjectCount");
            }
        }

        private const int FrameBatchSize = 30;
        private readonly LanderGame _landerGame;

        private long _composeRenderCounter;
        private double _compositionTime;
        private double _fps;
        private uint _frameCounter;
        private long _gameLoopCounter;
        private double _gameLoopTime;
        private DateTime _lastFpsTime = DateTime.Now;
    }
}
