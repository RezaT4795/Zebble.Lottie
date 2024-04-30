namespace Zebble
{
    using System.Timers;
    using System;
    using SKAnimation = SkiaSharp.Skottie.Animation;
    using SkiaSharp;
    using Olive;

    class LottieAnimationController
    {
        static readonly TimeSpan Interval = Animation.OneFrame * 2;
        readonly object RenderLock = new();

        readonly SKAnimation SKAnimation;
        readonly int MaxFrames;
        readonly Action OnInvalidate;
        readonly Action OnFinished;

        int CurrentFrame = 0;
        Timer Timer;

        public LottieAnimationController(SKAnimation animation, Action onInvalidate, Action onFinished)
        {
            SKAnimation = animation;
            MaxFrames = (int)(animation.Duration / Interval).LimitMin(0);
            OnInvalidate = onInvalidate;
            OnFinished = onFinished;

            SKAnimation.SeekFrame(0);

            Timer = new Timer(Interval.TotalMilliseconds);
            Timer.Elapsed += TimerOnElapsed;
        }

        public void Play()
        {
            CurrentFrame = 0;
            Timer.Start();
        }

        public void Pause() => Timer.Stop();

        public void Resume() => Timer.Start();

        public void Stop()
        {
            Timer.Stop();
            CurrentFrame = 0;
        }

        void TimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            lock (RenderLock)
            {
                var isFinished = CurrentFrame > MaxFrames;

                if (isFinished)
                {
                    Stop();
                    OnFinished();
                    return;
                }

                    SKAnimation.SeekFrame(CurrentFrame);
                Thread.UI.RunAction(OnInvalidate);

                CurrentFrame++;
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
