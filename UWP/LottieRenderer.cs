namespace Zebble
{
    using System;
    using System.Threading.Tasks;
    using Windows.UI.Xaml;
    using Microsoft.Toolkit.Uwp.UI.Lottie;
    using Microsoft.UI.Xaml.Controls;
    using Windows.Storage.Streams;
    using Olive;

    public class LottieRenderer : INativeRenderer
    {
        AnimatedVisualPlayer Player;
        LottieView View;
        LottieVisualSource Source;

        public async Task<FrameworkElement> Render(Renderer renderer)
        {
            Player = new();
            View = (LottieView)renderer.View;

            try
            {
                Source = new();

                if (View.AnimationJsonString.HasValue()) Source.SetSourceAsync(await CreateJsonStream(View.AnimationJsonString));
                else Source.SetSourceAsync($@"ms-appx:///Resources/{View.AnimationJsonFile}".AsUri());
                
                Player.Source = Source;
            }
            catch (Exception ex)
            {
                await Dialogs.Current.Toast("Failed: " + ex.Message);
            }

            View.OnPlay.Handle(async () => await Player.PlayAsync(View.From, View.To, View.Loop));
            View.OnPause.Handle(() => Player.Pause());
            View.OnResume.Handle(() => Player.Resume());

            View.OnPropertyChanged.Handle(async () =>
            {
                Player.PlaybackRate = View.PlayBackRate;
                await Player.PlayAsync(View.From, View.To, View.Loop);
            });

            return await Task.FromResult(Player);
        }

        static async Task<IInputStream> CreateJsonStream(string jsonString)
        {
            using var stream = new InMemoryRandomAccessStream();

            using (var dataWriter = new DataWriter(stream))
            {
                dataWriter.UnicodeEncoding = UnicodeEncoding.Utf8;
                dataWriter.ByteOrder = ByteOrder.LittleEndian;

                dataWriter.WriteString(jsonString);

                await dataWriter.StoreAsync();
                await dataWriter.FlushAsync();

                dataWriter.DetachStream();
            }

            return stream.GetInputStreamAt(0);
        }

        public void Dispose()
        {
            Player = null;
            View = null;
        }
    }
}
