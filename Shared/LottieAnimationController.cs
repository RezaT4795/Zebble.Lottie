namespace Zebble
{
    using System.Timers;
    using System;
    using SKAnimation = SkiaSharp.Skottie.Animation;
    using SkiaSharp;

    class LottieAnimationController
    {
        static readonly TimeSpan Interval = Animation.OneFrame * 2;
        readonly object RenderLock = new();

        readonly SKAnimation SKAnimation;
        readonly Action OnInvalidate;
        readonly Action<bool> OnSeek;

        int CurrentFrame = -1;
        TimeSpan CurrentPosition => Interval * CurrentFrame;
        Timer Timer;

        public LottieAnimationController(SKAnimation animation, Action onInvalidate, Action<bool> onSeek)
        {
            SKAnimation = animation;
            OnInvalidate = onInvalidate;
            OnSeek = onSeek;

            Timer = new Timer(Interval.TotalMilliseconds);
            Timer.Elapsed += TimerOnElapsed;
        }

        public void Play()
        {
            CurrentFrame = -1;
            Timer.Start();
        }

        public void Pause() => Timer.Stop();

        public void Resume() => Timer.Start();

        public void Stop()
        {
            Timer.Stop();
            CurrentFrame = -1;
        }

        void TimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            lock (RenderLock)
            {
                CurrentFrame++;

                var isFinished = CurrentPosition >= SKAnimation.Duration;

                if (isFinished == false)
                {
                    SKAnimation.SeekFrame(CurrentFrame);
                    OnInvalidate();
                }

                OnSeek(isFinished);
            }
        }

        public void Render(SKCanvas canvas, SKRect rect)
        {
            canvas.Clear();
            SKAnimation.Render(canvas, rect);
        }

        public void Dispose()
        {
            Timer?.Dispose();
            Timer = null;
        }
    }
}
