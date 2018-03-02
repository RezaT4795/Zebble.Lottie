namespace Zebble
{
    using System;
    using System.Threading.Tasks;
    using Windows.UI.Xaml;

    class LottieRenderer : INativeRenderer
    {
        public Task<FrameworkElement> Render(Renderer renderer)
        {
            Device.Log.Error("Lottie plugin not works on UWP");
            throw new Exception("Lottie plugin not works on UWP");
        }

        public void Dispose()
        {
        }
    }
}
