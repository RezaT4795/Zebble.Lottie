using Foundation;
using Olive;

namespace Zebble
{
    using System.ComponentModel;
    using System.Threading.Tasks;
    using UIKit;
    using Zebble.Device;
    using Airbnb.Lottie;

    [EditorBrowsable(EditorBrowsableState.Never)]
    class LottieRenderer : INativeRenderer
    {
        LOTAnimationView Player;
        LottieView View;

        public Task<UIView> Render(Renderer renderer)
        {
            if (Player == null)
            {
                View = (LottieView)renderer.View;

                if (View.AnimationJsonString.HasValue())
                {
                    var data = NSData.FromString(View.AnimationJsonString);
                    var dictionary = (NSDictionary)NSJsonSerialization.Deserialize(data, 0, out _);
                    Player = LOTAnimationView.AnimationFromJSON(dictionary);
                }
                else Player = LOTAnimationView.AnimationWithFilePath(IO.File(View.AnimationJsonFile).FullName);

                Player.ContentMode = UIViewContentMode.ScaleAspectFit;
            }

            View.OnPlay.Handle(() => Player.Play());
            View.OnPause.Handle(() => Player.Pause());
            View.OnResume.Handle(() => Player.PlayWithCompletion(AnimationCompletionBlock));
            View.OnPropertyChanged.Handle(() =>
            {
                Player.AnimationSpeed = View.PlayBackRate;
                Player.PlayFromProgress(View.From, View.To, AnimationCompletionBlock);
            });

            if (View.Loop)
                Player.PlayWithCompletion(AnimationCompletionBlock);
            else
                Player.Play();

            return Task.FromResult<UIView>(Player);
        }

        void AnimationCompletionBlock(bool animationFinished)
        {
            if (animationFinished)
            {
                if (View.Loop)
                    Player.PlayWithCompletion(AnimationCompletionBlock);
                else
                    Player.Stop();
            }
        }

        public void Dispose() => Player.Dispose();
    }
}