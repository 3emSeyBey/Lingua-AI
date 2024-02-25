using Firebase.Database;
using Microsoft.CognitiveServices.Speech;
using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Lingua
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ReceiverPage : ContentPage
	{
		static string speechKey = Environment.GetEnvironmentVariable("SPEECH_KEY");
		static string speechRegion = Environment.GetEnvironmentVariable("SPEECH_REGION");

		FirebaseClient firebaseClient = new FirebaseClient("https://linguamessaging-default-rtdb.asia-southeast1.firebasedatabase.app/");
		public ObservableCollection<MessagingRecord> MessageItems { get; set; } = new ObservableCollection<MessagingRecord>();
		public string convoID;
		private string incomingMessage;
		public ReceiverPage(string _convoID)
		{
			convoID = _convoID;
			InitializeComponent();
			//usernamelabel.Text = convoID.Split("_")[0];
			NavigationPage.SetHasNavigationBar(this, false);
			BindingContext = this;
			var collection = firebaseClient.Child(convoID).AsObservable<MessagingRecord>().Subscribe((dbevent) =>
			{
				if (dbevent.Object != null)
				{
					incomingMessage = dbevent.Object.MessagingProperty;
					if (autoplay.IsChecked)
					{
						if (incomingMessage[0] == '~')
						{
							submitTextToVoice(incomingMessage.Substring(1));
						}
					}
					MessageItems.Add(dbevent.Object);
				}
			});
		}
		public async void CloseReceive(object sender, EventArgs e)
		{
			await firebaseClient.Child(convoID).DeleteAsync();
			await Navigation.PopAsync();
		}
		public async void submitTextToVoice(string message)
		{
			var config = SpeechConfig.FromSubscription("090c76d1092f4dbcb28108916ea3c548", "eastus");
			config.SpeechSynthesisVoiceName = "en-US-JaneNeural";
			// use the default speaker as audio output.
			using (var synthesizer = new SpeechSynthesizer(config))
			{
				using (var result = await synthesizer.SpeakTextAsync(message))
				{
					if (result.Reason == ResultReason.SynthesizingAudioCompleted)
					{
					}
					else if (result.Reason == ResultReason.Canceled)
					{
						var cancellation = SpeechSynthesisCancellationDetails.FromResult(result);
					}
				}
			}

		}

		private void Button_Clicked(object sender, EventArgs e)
		{

		}
	}
}