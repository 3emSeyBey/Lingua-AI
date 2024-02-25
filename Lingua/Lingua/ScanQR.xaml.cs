using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Lingua
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ScanQR : ContentPage
	{
		public ScanQR()
		{
			InitializeComponent();
			zxing.OnScanResult += (result) => Device.BeginInvokeOnMainThread(() =>
			{
				Proceed(result.Text);
			});
		}
		protected override void OnAppearing()
		{
			base.OnAppearing();
			zxing.IsScanning = true;
		}

		private async void Proceed(string convoID)
		{
			Navigation.PopAsync();
			await Navigation.PushAsync(new ReceiverPage(convoID));
		}

		protected override void OnDisappearing()
		{
			zxing.IsScanning = false;
			base.OnDisappearing();
		}
	}
}