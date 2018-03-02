namespace Zebble
{
    public partial class Lottie : View, IRenderedBy<LottieRenderer>
    {
        internal AsyncEvent OnPlay = new AsyncEvent();
        internal AsyncEvent OnPause = new AsyncEvent();
        internal AsyncEvent<bool> OnLoopChanged = new AsyncEvent<bool>();
        internal string AnimationJsonFile { get; private set; }

        bool loop;
        public bool Loop
        {
            get => loop;
            set
            {
                if (value == loop) return;
                loop = value;
                OnLoopChanged.RaiseOn(Thread.UI, loop);
            }
        }

        public void Pause() => OnPlay.RaiseOn(Thread.UI);

        public void Play() => OnPlay.RaiseOn(Thread.UI);

        public Lottie(string animationJsonFile)
        {
#if ANDROID
            AnimationJsonFile="Android.Resources." + animationJsonFile;
#else
            AnimationJsonFile = animationJsonFile;
#endif

            Render();
        }
    }
}
