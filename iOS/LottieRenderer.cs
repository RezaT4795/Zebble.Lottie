namespace Zebble
{
    using System.ComponentModel;
    using System.Threading.Tasks;
    using UIKit;
    using Zebble.Device;
    
    [EditorBrowsable(EditorBrowsableState.Never)]
    partial class LottieRenderer : INativeRenderer
    {
        Airbnb.Lottie.LOTAnimationView Result;
        Lottie View;

        public Task<UIView> Render(Renderer renderer)
        {
            if (Result == null)
            {
                View = (Lottie)renderer.View;
                var info = IO.File(View.AnimationJsonFile);

                Result = Airbnb.Lottie.LOTAnimationView.AnimationWithFilePath(info.FullName);
            }

            View.OnPlay.Handle(() => Result.Play());
            View.OnPause.Handle(() => Result.Pause());
            View.OnLoopChanged.Handle((loop) => Result.LoopAnimation = loop);

            return Task.FromResult<UIView>(Result);
        }

        public void Dispose() => Result.Dispose();
    }
}