namespace Zebble
{
    using System;
    using System.Threading.Tasks;
    using Zebble.Device;

#if ANDROID
    [Android.Runtime.Preserve]
#endif
    public class LottieView : View, IRenderedBy<LottieRenderer>
    {
        internal readonly AsyncEvent OnPlay = new();
        internal readonly AsyncEvent OnPause = new();
        internal readonly AsyncEvent OnResume = new();
        internal readonly AsyncEvent OnStop = new();

        public string Data { get; set; }

        public override async Task OnInitializing()
        {
            await base.OnInitializing();

            App.Started += Play;
            App.WentIntoBackground += Pause;
            App.Stopping += Stop;
        }

        public override void Dispose()
        {
            App.Started -= Play;
            App.WentIntoBackground -= Pause;
            App.Stopping -= Stop;

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
