namespace Zebble
{
#if ANDROID
    [Android.Runtime.Preserve]
#endif
    public class LottieView : View, IRenderedBy<LottieRenderer>
    {
        internal AsyncEvent OnPlay = new();
        internal AsyncEvent OnPause = new();
        internal AsyncEvent OnResume = new();
        internal AsyncEvent OnPropertyChanged = new();

        public LottieView() { }

        public string AnimationJsonFile { get; set; }
        public string AnimationJsonString { get; set; }

        bool loop = true;
        public bool Loop { get => loop; set => SetLoop(value); }

        async void SetLoop(bool value)
        {
            if (value == loop) return;
            loop = value;
            await OnPropertyChanged.RaiseOn(Thread.UI);
        }

        float from = 0;
        public float From { get => from; set => SetFrom(value); }

        async void SetFrom(float value)
        {
            if (value == from) return;
            if (value > 1.0) value = 1;
            if (value < 0.0) value = 0;
            if (value > to) value = to;
            from = value;
            await OnPropertyChanged.RaiseOn(Thread.UI);
        }

        float to = 1;
        public float To { get => to; set => SetTo(value); }

        async void SetTo(float value)
        {
            if (value == to) return;
            if (value > 1.0) value = 1;
            if (value < 0.0) value = 0;
            if (value < from) value = from;
            to = value;
            await OnPropertyChanged.RaiseOn(Thread.UI);
        }

        float playBackRate = 1;
        public float PlayBackRate { get => playBackRate; set => SetPlayBackRate(value); }

        async void SetPlayBackRate(float value)
        {
            if (value == playBackRate) return;
            playBackRate = value;
            await OnPropertyChanged.RaiseOn(Thread.UI);
        }
        public void Pause() => OnPause.RaiseOn(Thread.UI);
        public void Play() => OnPlay.RaiseOn(Thread.UI);
        public void Resume() => OnResume.RaiseOn(Thread.UI);
    }
}
