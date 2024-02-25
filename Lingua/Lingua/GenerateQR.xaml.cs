using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Lingua
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class GenerateQR : ContentPage
	{
		public string convoID;
		public GenerateQR(string _convoID)
		{
			InitializeComponent();
			QRCodeView.BarcodeValue = _convoID;
			convoID = _convoID;
		}
		public async void proceed(object sender, EventArgs e)
		{
			await Navigation.PushAsync(new MainPage(convoID));
		}
	}
}