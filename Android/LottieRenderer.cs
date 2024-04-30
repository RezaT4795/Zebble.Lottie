namespace Zebble
{
    using System.ComponentModel;
    using System.Threading.Tasks;
    using PlatformView = Android.Views.View;
    using SkiaSharp.Views.Android;
    using SKAnimation = SkiaSharp.Skottie.Animation;
    using System;

    [EditorBrowsable(EditorBrowsableState.Never)]
    class LottieRenderer : INativeRenderer
    {
        readonly SKAnimation Animation;

        LottiePlayer Player;
        LottieView View;

        public Task<PlatformView> Render(Renderer renderer)
        {
            View = (LottieView)renderer.View;
            Player = new LottiePlayer(View.Animation, OnFinished);

            View.OnPlay.Handle(Player.Play);
            View.OnPause.Handle(Player.Pause);
            View.OnResume.Handle(Player.Resume);
            View.OnStop.Handle(Player.Stop);

            Player.Play();

            return Task.FromResult<PlatformView>(Player);
        }
        
        void OnFinished()
        {
            if (View.Loop) Player.Play();
        }

        public void Dispose()
        {
            Player?.Dispose();
            Player = null;
        }

        class LottiePlayer : SKCanvasView
        {
            LottieAnimationController Controller;

            public LottiePlayer(SKAnimation animation, Action onFinished) : base(UIRuntime.CurrentActivity)
                => Controller = new(animation, Invalidate, onFinished);

            public void Play() => Controller.Play();

            public void Pause() => Controller.Pause();

            public void Resume() => Controller.Resume();

            public void Stop() => Controller.Stop();

            protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
            {
                base.OnPaintSurface(e);
                Controller.Render(e.Surface.Canvas, e.Info.Rect);
            }

            protected override void Dispose(bool disposing)
            {
                base.Dispose(disposing);
                Controller?.Dispose();
                Controller = null;
            }
        }
    }
}