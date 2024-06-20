namespace Zebble
{
    using System.Timers;
    using System;
    using SKAnimation = SkiaSharp.Skottie.Animation;
    using SkiaSharp;
    using Olive;
    using SkiaSharp.Resources;
    using System.Text;

    class LottieAnimationController
    {
        static readonly TimeSpan Interval = Animation.OneFrame * 2;
        readonly object RenderLock = new();

        readonly int MaxFrames;
        readonly Action OnInvalidate;
        readonly Action OnFinished;

        int CurrentFrame = 0;
        SKAnimation SKAnimation;
        Timer Timer;
        bool IsDisposed;

        public LottieAnimationController(string data, Action onInvalidate, Action onFinished)
        {
            SKAnimation = CreateSKAnimation(data);
            MaxFrames = (int)(SKAnimation.Duration / Interval).LimitMin(0);
            OnInvalidate = onInvalidate;
            OnFinished = onFinished;

            SKAnimation.SeekFrame(0);

            Timer = new Timer(Interval.TotalMilliseconds);
            Timer.Elapsed += TimerOnElapsed;
        }

        static SKAnimation CreateSKAnimation(string data)
        {
            return SKAnimation.CreateBuilder()
                .SetResourceProvider(new CachingResourceProvider(new DataUriResourceProvider()))
                .SetFontManager(SKFontManager.Default)
                .Build(SKData.CreateCopy(data.ToBytes(Encoding.ASCII)));
        }

        public void Play()
        {
            CurrentFrame = 0;
            Timer?.Start();
        }

        public void Pause() => Timer?.Stop();

        public void Resume() => Timer?.Start();

        public void Stop()
        {
            Timer?.Stop();
            CurrentFrame = 0;
        }

        void TimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            if (IsDisposed) return;

            lock (RenderLock)
            {
                if (IsDisposed) return;

                var isFinished = CurrentFrame > MaxFrames;

                if (isFinished)
                {
                    Stop();
                    OnFinished();
                    return;
                }

                try { SKAnimation?.SeekFrame(CurrentFrame); }
                catch { }

                if (IsDisposed) return;

                Thread.UI.RunAction(() =>
                {
                    try { OnInvalidate(); }
                    catch (ObjectDisposedException ex) { }
                });

                CurrentFrame++;
            }
        }

        public void Render(SKCanvas canvas, SKRect rect)
        {
            canvas.Clear();
            SKAnimation?.Render(canvas, rect);
        }

        public void Dispose()
        {
            IsDisposed = true;

            Timer?.Dispose();
            Timer = null;

            SKAnimation?.Dispose();
            SKAnimation = null;
        }
    }
}
