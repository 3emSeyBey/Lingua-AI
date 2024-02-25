using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Lingua
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class NameElect : ContentPage
	{

		int rolenum;
		public NameElect(int role)
		{
			rolenum = role;
			InitializeComponent();
		}
		public async void onButtonClicked(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(entry_name.Text))
			{
				entryFrame.BorderColor = Color.Red;
				entry_name.PlaceholderColor = Color.Red;
				return;
			}
			else
			{
				//role = 1: Send, role = 2: receive
				if (rolenum == 1)
				{
					Random random = new Random();
					string hexCode = random.Next(0x10000000).ToString("X8");
					string convoID = entry_name.Text + "_" + hexCode;
					await Navigation.PushAsync(new GenerateQR(convoID));
				}
				else if (rolenum == 2)
				{
					await Navigation.PushAsync(new ScanQR());
				}
			}
		}

		public void entrytextchanged(object sender, EventArgs e)
		{
			entryFrame.BorderColor = Color.Transparent;
			entry_name.PlaceholderColor = Color.Green;
		}
	}
}