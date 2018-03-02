namespace Zebble
{
    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Threading.Tasks;

    [EditorBrowsable(EditorBrowsableState.Never)]
    partial class LottieRenderer : INativeRenderer
    {
        Com.Airbnb.Lottie.LottieAnimationView Result;
        Lottie View;

        public void Dispose()
        {
            Result.Dispose();
            Result = null;
        }

        public async Task<Android.Views.View> Render(Renderer renderer)
        {
            View = (Lottie)renderer.View;
            var assembly = UIRuntime.CurrentActivity.GetType().GetAssembly();
            var stream = new StreamReader(assembly.GetManifestResourceStream(View.AnimationJsonFile));

            Result = new Com.Airbnb.Lottie.LottieAnimationView(UIRuntime.CurrentActivity);
            Result.SetAnimationFromJson(stream.ReadToEnd());

            View.OnPlay.Handle(() => Result.PlayAnimation());
            View.OnPause.Handle(() => Result.PauseAnimation());
            View.OnLoopChanged.Handle((loop) => Result.Loop(loop));

            return Result;
        }
    }
}