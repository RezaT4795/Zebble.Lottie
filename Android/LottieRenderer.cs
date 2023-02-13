namespace Zebble
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Android.Animation;
    using Android.Runtime;
    using Com.Airbnb.Lottie;
    using Olive;

    [Preserve]
    [EditorBrowsable(EditorBrowsableState.Never)]
    class LottieRenderer : INativeRenderer
    {
        LottieAnimationView Player;
        LottieView View;

        public async Task<Android.Views.View> Render(Renderer renderer)
        {
            View = (LottieView)renderer.View;

            Player = new(UIRuntime.CurrentActivity);
            Player.SetAnimationFromJson(await Device.IO.File(View.AnimationJsonFile).ReadAllTextAsync(), View.AnimationJsonFile);
            if (View.Loop) Player.RepeatCount = ValueAnimator.Infinite;

            View.OnPlay.Handle(() => Player?.PlayAnimation());
            View.OnPause.Handle(() => Player?.PauseAnimation());
            View.OnResume.Handle(() => Player?.ResumeAnimation());
            View.OnPropertyChanged.Handle(OnPropertyChanged);
            
            try { Player.PlayAnimation(); }
            catch (Exception ex) { Debug.WriteLine(ex.Message); }

            return Player;
        }

        void OnPropertyChanged()
        {
            try
            {
                if (Player == null) return;
                Player.Speed = View.PlayBackRate;
                Player.SetMinProgress(View.From);
                Player.SetMaxProgress(View.To);
                Player.RepeatCount = View.Loop ? ValueAnimator.Infinite : 0;
                Player.PlayAnimation();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public void Dispose()
        {
            Player?.Dispose();
            Player = null;
        }
    }
}