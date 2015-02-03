using LanderNet.UI.Util;
using TpsGraphNet;

namespace LanderNet.UI.MainView
{
    internal class StarBackground
    {
        public void Move(double xSpeed, double ySpeed, uint renderWidth, uint renderHeight, double secondsPassed)
        {
            InitStars(renderWidth, renderHeight);

            const double starSpeedMultiplier = 0.8;
            foreach (var star in _stars)
            {
                var depth = byte.MaxValue/star[2];
                var x = star[0] - xSpeed/depth*secondsPassed*starSpeedMultiplier;
                var y = star[1] + ySpeed/depth*secondsPassed*starSpeedMultiplier;

                if (x < 0) x = renderWidth;
                if (x > renderWidth) x = 0;
                if (y < 0) y = renderHeight;
                if (y > renderHeight) y = 0;

                star[0] = x;
                star[1] = y;
            }
        }

        public void Render(Sprite renderTarget)
        {
            renderTarget.Fill(40); // Bluish background
            InitStars(renderTarget.Width, renderTarget.Height);

            foreach (var star in _stars)
            {
                var colorByte = (uint)(star[2]);
                renderTarget.SetPixel((uint)star[0], (uint)star[1], colorByte | colorByte << 8 | colorByte << 16);
            }
        }

        public void Reset()
        {
            _stars = null;
        }

        private void InitStars(uint renderWidth, uint renderHeight)
        {
            if (_stars != null)
                return;

            var rnd = RandomHelper.Instance;
            _stars = new double[200][];
            for (int i = 0; i < _stars.Length; i++)
            {
                var x = rnd.Next((int) renderWidth);
                var y = rnd.Next((int) renderHeight);
                var depth = rnd.Next(byte.MaxValue/4, byte.MaxValue);
                _stars[i] = new double[] {x, y, depth};
            }
        }

        private double[][] _stars;
    }
}
