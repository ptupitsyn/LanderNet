using System;
using System.Threading;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using LanderNet.Game;
using LanderNet.UI.Options;
using LanderNet.UI.Util;
using TpsGraphNet;

namespace LanderNet.UI.MainView
{
    internal class GameViewModel : NotifierBase
    {
        public GameViewModel()
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
            _landerGame = new LanderGame();
            _presenter = new LanderGamePresenter(_landerGame, Options);
            _devInfo = new DevInfoViewModel(_landerGame);

            RenderWidth = 800;
            RenderHeight = 600;

            _landerGame.ResetGame();
            IsPaused = true;  // Show menu initially

            // Terminate game loop properly
            Application.Current.Exit += (sender, args) =>
            {
                _runGameLoop = false;
                _renderGate.Set();

                // Wait for render thread to finish.
                _renderThreadEndEvent.Wait();
            };
        }

        public LanderGame LanderGame => _landerGame;

        public OptionsViewModel Options { get; } = new OptionsViewModel
        {
            AsteroidsCount = 30,
            DebrisLimit = 30,
            EnableSound = true
        };

        public DevInfoViewModel DevInfo => _devInfo;

        public bool IsPaused
        {
            get { return _isPaused; }
            set
            {
                if (_isPaused == value)
                    return;
                _isPaused = value;
                if (_isPaused)
                {
                    _landerGame.Pause();
                }
                else
                {
                    _landerGame.Resume();
                }

                OnPropertyChanged(nameof(IsPaused));
            }
        }

        public uint RenderHeight
        {
            get { return _renderHeight; }
            set
            {
                if (_renderHeight == value) return;
                _renderHeight = value;
                OnResize();
            }
        }

        public uint RenderWidth
        {
            get { return _renderWidth; }
            set
            {
                if (value == _renderWidth) return;
                _renderWidth = value;
                OnResize();
            }
        }

        public BitmapSource ScreenBufferBitmap
        {
            get { return _screenBufferBitmap; }
            private set
            {
                _screenBufferBitmap = value;
                OnPropertyChanged(nameof(ScreenBufferBitmap));
            }
        }

        public DelegateCommand ShowHideMenuCommand
        {
            get
            {
                return _showHideMenuCommand ??
                       (_showHideMenuCommand = new DelegateCommand(() => { IsPaused = !IsPaused; }));
            }
        }

        public void RunGameLoop()
        {
            var gameLoopThread = new Thread(unused => GameLoop());
            gameLoopThread.SetApartmentState(ApartmentState.STA);
            gameLoopThread.Start();

            CompositionTarget.Rendering += (sender, args) =>
            {
                var bmp = ((InteropBitmap) ScreenBufferBitmap);
                if (bmp != null)
                {
                    bmp.Invalidate();
                    _renderGate.Set(); // Start next frame
                }
            };
        }

        private void GameLoop()
        {
            // Gamer loop thread
            while (_runGameLoop)
            {
                // Game logic
                _devInfo.MeasureGameLoopTime(_landerGame.GameLoopImpl);
                _devInfo.UpdateFps();
                
                // Back buffer composition
                Render();
                
                // Draw back buffer on the main screen
                // Double buffering?
                _renderGate.WaitOne();
            }

            _renderThreadEndEvent.Set();
        }

        private void OnResize()
        {
            _renderSizeChanged = _screenBackBuf != null;
        }

        private void Render()
        {
            VerifyScreenBuffers();
            _devInfo.MeasureCompositionTime(() => _presenter.Render(_screenBackBuf));
            UpdateBitmapSource();
        }

        private void UpdateBitmapSource()
        {
            ScreenBufferBitmap = _screenBackBuf.GetOrUpdateBitmapSource();
        }

        private void VerifyScreenBuffers()
        {
            if (_screenBackBuf == null || _renderSizeChanged)
            {
                _screenBackBuf?.Dispose();
                _screenBackBuf = new Sprite(RenderWidth, RenderHeight);
                _dispatcher.Invoke((Action) (UpdateBitmapSource));
                OnPropertyChanged(nameof(ScreenBufferBitmap));
                _renderSizeChanged = false;
                _presenter.ResetBackground();
            }
        }

        private readonly DevInfoViewModel _devInfo;
        private readonly Dispatcher _dispatcher;
        private readonly LanderGame _landerGame;
        private readonly LanderGamePresenter _presenter;
        private readonly AutoResetEvent _renderGate = new AutoResetEvent(true);
        private readonly ManualResetEventSlim _renderThreadEndEvent = new ManualResetEventSlim(false);
 
        private bool _isPaused;
        private uint _renderHeight;
        private bool _renderSizeChanged;
        private uint _renderWidth;
        private bool _runGameLoop = true;
        private Sprite _screenBackBuf;
        private BitmapSource _screenBufferBitmap;

        private DelegateCommand _showHideMenuCommand;
    }
}