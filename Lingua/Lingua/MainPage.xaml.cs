using System;
using Xamarin.Forms;

namespace Lingua
{
	public partial class MainPage : ContentPage
	{
		public string convoID;
		public MainPage(string _convoID)
		{
			convoID = _convoID;
			InitializeComponent();
			NavigationPage.SetHasNavigationBar(this, false);
		}
		private async void STVButtonClicked(object sender, EventArgs e)
		{
			await Navigation.PushAsync(new STTPage(true, convoID));
		}

		private async void STTButtonClicked(object sender, EventArgs e)
		{
			await Navigation.PushAsync(new STTPage(false, convoID));
		}

		private async void VTSButtonClicked(object sender, EventArgs e)
		{
			await Navigation.PushAsync(new VTSPage(convoID));
		}

		private async void STutButtonClicked(object sender, EventArgs e)
		{
			await Navigation.PushAsync(new STutPage());
		}
	}
}
