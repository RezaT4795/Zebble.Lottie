namespace Zebble
{
    using System.ComponentModel;
    using System.Threading.Tasks;
    using PlatformView = Windows.UI.Xaml.FrameworkElement;
    using SkiaSharp.Views.UWP;
    using System;

    [EditorBrowsable(EditorBrowsableState.Never)]
    class LottieRenderer : INativeRenderer
    {
        LottiePlayer Player;
        LottieView View;

        public Task<PlatformView> Render(Renderer renderer)
        {
            View = (LottieView)renderer.View;
            Player = new LottiePlayer(View.Data, OnFinished);

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

        class LottiePlayer : SKXamlCanvas
        {
            LottieAnimationController Controller;

            public LottiePlayer(string data, Action onFinished)
                => Controller = new(data, Invalidate, onFinished);

            public void Play() => Controller.Play();

            public void Pause() => Controller.Pause();

            public void Resume() => Controller.Resume();

            public void Stop() => Controller.Stop();

            protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
            {
                base.OnPaintSurface(e);
                Controller.Render(e.Surface.Canvas, e.Info.Rect);
            }

            public void Dispose()
            {
                Controller?.Dispose();
                Controller = null;
            }
        }
    }
}