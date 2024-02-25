using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Lingua
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class RoleSelectionPage : ContentPage
	{
		public RoleSelectionPage()
		{
			InitializeComponent();
		}

		public async void btnSendClicked(object sender, EventArgs e)
		{
			await Navigation.PushAsync(new NameElect(1));
		}

		public async void btnReceiveClicked(object sender, EventArgs e)
		{
			await Navigation.PushAsync(new NameElect(2));
		}
	}
}