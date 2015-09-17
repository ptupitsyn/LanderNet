using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using LanderNet.Game.Components;
using LanderNet.Game.GameObject;
using SlimDX.Multimedia;
using SlimDX.XAudio2;

namespace LanderNet.UI.Util
{
    public class SoundHelper
    {
        public void Explosion(IGameObject gameObject, IGameObject spaceShip)
        {
            // Calculate balance and volume based on distance from spaceship
            var balance = 0.0;
            var volume = 0.5;

            var objPos = gameObject.GetComponent<PositionComponent>();

            if (objPos != null)
            {
                var shipPos = spaceShip.GetComponent<PositionComponent>();
                volume = (1200 - shipPos.GetDistance(objPos)) / 1200;
                if (volume < 0) volume = 0;
                if (volume > 1) volume = 1;

                var xDist = objPos.X - shipPos.X;
                if (xDist > 500) xDist = 500;
                if (xDist < -500) xDist = -500;
                balance = (500 - Math.Abs(xDist)) / 1000 * Math.Sign(xDist);
            }

            Explosion(balance, volume);
        }

        public void MachineGun()
        {
            Play("MachnGun.wav", volume:0.05);
        }

        public void RocketLaunch()
        {
            Play("rocket.wav", volume:0.1);
        }

        private void Explosion(double balance = 0, double volume = 0.5)
        {
            Play("explos.wav", balance, volume);
        }

        private static MemoryStream GetSoundStream(string soundName)
        {
            byte[] data;
            if (!SoundCache.TryGetValue(soundName, out data))
            {
                var moduleName = Assembly.GetExecutingAssembly().GetName().Name;
                var resourceLocation = string.Format("pack://application:,,,/{0};component/Resources/Sounds/{1}", moduleName, soundName);
                var stream = Application.GetResourceStream(new Uri(resourceLocation));

                if (stream == null)
                    return null;

                data = new byte[stream.Stream.Length];
                stream.Stream.Read(data, 0, data.Length);

                SoundCache[soundName] = data;
            }
            return new MemoryStream(data);
        }

        private void Init()
        {
            if (_isInitialized)
                return;

            _isInitialized = true;

            // Start a background thread for sounds
            var playerThread = new Thread(RunPlayerThread) {IsBackground = true, Priority = ThreadPriority.BelowNormal};
            playerThread.Start();
        }

        private void Play(string soundName, double balance = 0, double volume = 0.5)
        {
            Init();

            // Queue on the other thread via Dispatcher
            if (_playerThreadDispatcher != null)
            {
                _playerThreadDispatcher.BeginInvoke((Action) (() => PlayImpl(soundName, balance, volume)));
            }
        }

        private void PlayImpl(string soundName, double balance, double volume)
        {
            var soundStream = GetSoundStream(soundName);
            if (soundStream == null)
                return;

            var stream = new WaveStream(soundStream);
            var voice = new SourceVoice(_audioDevice, stream.Format) {Volume = (float) volume};
            var buf = new AudioBuffer {AudioData = stream, AudioBytes = (int) stream.Length};

            voice.SubmitSourceBuffer(buf);
            voice.Start();

            // This does not work, need matrix http://xboxforums.create.msdn.com/forums/t/75836.aspx
            //voice.SetChannelVolumes(2, new[] {(float) (balance <= 0 ? 1 : balance), (float) (balance >= 0 ? 1 : -balance)});

            voice.BufferEnd += (sender, args) => _playerThreadDispatcher.BeginInvoke((Action) (() =>
            {
                voice.Stop();
                buf.Dispose();
                voice.Dispose();
                stream.Dispose();
            }));
        }

        private void RunPlayerThread()
        {
            try
            {
                _audioDevice = new XAudio2();

                // Master voice is never used, but must be created
                _masterVoice = new MasteringVoice(_audioDevice) { Volume = 1 };

                _playerThreadDispatcher = Dispatcher.CurrentDispatcher;
                Dispatcher.Run();
            }
            catch (Exception)
            {
                // Audio initialization failed.
            }
        }

        private static readonly Dictionary<string, byte[]> SoundCache = new Dictionary<string, byte[]>();
        
        private XAudio2 _audioDevice;
        private bool _isInitialized;
        private MasteringVoice _masterVoice;
        private Dispatcher _playerThreadDispatcher;
    }
}