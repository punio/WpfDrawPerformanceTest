using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.Direct2D1;

namespace WpfDrawPerformanceTest
{
    class D2dDrawControl : D2dControl.D2dControl
    {
        public D2dDrawControl(CustomDrawControl.Particle[] particles, Random random)
        {
            _particles = particles;
            _random = random;
        }

        private readonly CustomDrawControl.Particle[] _particles;
        private readonly Random _random;
        private uint _counter = 0;

        public override void Render(RenderTarget target)
        {
            target.Clear(new SharpDX.Mathematics.Interop.RawColor4(0, 0, 0, 0));

            #region Create Palette
            resCache.Clear();

            for (var i = 0; i < 8; i++)
            {
                var c = CustomDrawControl.GetColor((_counter + i * 8) % 360);
                resCache.Add("Color" + i, t => new SolidColorBrush(t, new SharpDX.Mathematics.Interop.RawColor4(c.R / 255.0f, c.G / 255.0f, c.B / 255.0f, c.A / 255.0f)));
            }
            #endregion

            var counter = 0;
            foreach (var p in _particles)
            {
                if (p.Age > CustomDrawControl.Particle.MaxAge) p.Init(_random, this.ActualWidth, this.ActualHeight);
                p.X2 = p.X1 + (_random.NextDouble() - .5) * 2;
                p.Y2 = p.Y1 + (_random.NextDouble() - .5) * 2;
                p.Pallete = (counter++) % 8;
                p.Age++;
            }
            foreach (var g in _particles.GroupBy(p => p.Pallete))
            {
                var brush = resCache["Color" + g.Key] as Brush;
                foreach (var p in g)
                {
                    target.DrawLine(new SharpDX.Mathematics.Interop.RawVector2((float)p.X1, (float)p.Y1), new SharpDX.Mathematics.Interop.RawVector2((float)p.X2, (float)p.Y2), brush, 2);
                    p.X1 = p.X2;
                    p.Y1 = p.Y2;
                }
            }
        }
    }
}
