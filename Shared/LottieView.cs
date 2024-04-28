namespace Zebble
{
    using Zebble.Device;
    using SKAnimation = SkiaSharp.Skottie.Animation;

#if ANDROID
    [Android.Runtime.Preserve]
#endif
    public class LottieView : View, IRenderedBy<LottieRenderer>
    {
        internal AsyncEvent OnPlay = new();
        internal AsyncEvent OnPause = new();
        internal AsyncEvent OnResume = new();
        internal SKAnimation Animation;

        public string AnimationJsonFile { set => Animation = SKAnimation.Create(IO.File(value).FullName); }

        public string AnimationJsonString { set => Animation = SKAnimation.Parse(value); }

        public bool Loop { get; set; } = true;

        public void Pause() => OnPause.RaiseOn(Thread.UI);
        public void Play() => OnPlay.RaiseOn(Thread.UI);
        public void Resume() => OnResume.RaiseOn(Thread.UI);
    }
}
