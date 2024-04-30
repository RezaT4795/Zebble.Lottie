namespace Zebble
{
    using Olive;
    using SkiaSharp;
    using SkiaSharp.Resources;
    using System;
    using System.Text;
    using System.Threading.Tasks;
    using Zebble.Device;
    using SKAnimation = SkiaSharp.Skottie.Animation;

#if ANDROID
    [Android.Runtime.Preserve]
#endif
    public class LottieView : View, IRenderedBy<LottieRenderer>
    {
        internal readonly AsyncEvent OnPlay = new();
        internal readonly AsyncEvent OnPause = new();
        internal readonly AsyncEvent OnResume = new();
        internal readonly AsyncEvent OnStop = new();

        internal SKAnimation Animation;

        public string Data
        {
            set
            {
                var builder = SKAnimation.CreateBuilder()
                    .SetResourceProvider(new CachingResourceProvider(new DataUriResourceProvider()))
                    .SetFontManager(SKFontManager.Default);
                var data = SKData.CreateCopy(value.ToBytes(Encoding.ASCII));
                Animation = builder.Build(data);
            }
        }

        public override async Task OnInitializing()
        {
            await base.OnInitializing();

            App.Started += Play;
            App.WentIntoBackground += Pause;
        }

        public override void Dispose()
        {
            App.Started -= Play;
            App.WentIntoBackground -= Pause;

            Animation?.Dispose();
            Animation = null;

            base.Dispose();
        }

        public bool Loop { get; set; } = true;

        public void Play() => OnPlay.RaiseOn(Thread.UI);
        public void Pause() => OnPause.RaiseOn(Thread.UI);
        public void Resume() => OnResume.RaiseOn(Thread.UI);
        public void Stop() => OnStop.RaiseOn(Thread.UI);
    }

    public static class LottieViewExtensions
    {
        public static TView Data<TView>(this TView @this, string value) where TView : LottieView
            => @this.Set(x => x.Data = value);

        public static TView Loop<TView>(this TView @this, bool value = true) where TView : LottieView
            => @this.Set(x => x.Loop = value);
    }
}
