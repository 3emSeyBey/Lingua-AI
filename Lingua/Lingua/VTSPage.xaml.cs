using Firebase.Database;
using Firebase.Database.Query;
using Microsoft.CognitiveServices.Speech;
using System;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Lingua
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class VTSPage : ContentPage
	{
		FirebaseClient firebaseClient = new FirebaseClient("https://linguamessaging-default-rtdb.asia-southeast1.firebasedatabase.app/");

		private SpeechRecognizer recognizer;
		string convoID;
		public VTSPage(string _convoId)
		{
			InitializeComponent();
			convoID = _convoId;
			NavigationPage.SetHasNavigationBar(this, false);
		}
		public async void sendToDatabase(string msg)
		{
			var mesClass = new MessagingRecord();
			mesClass.MessagingProperty += "\n" + RecognitionText.Text;
			await firebaseClient.Child(convoID).PostAsync(new MessagingRecord
			{
				MessagingProperty = msg
			});
		}
		public async Task StartListening()
		{
			StringBuilder sb = new StringBuilder();
			Image_btn_recognize.IsAnimationPlaying = true;
			try
			{
				var config = SpeechConfig.FromSubscription("090c76d1092f4dbcb28108916ea3c548", "eastus");
				config.SpeechRecognitionLanguage = "en-US";
				recognizer = new SpeechRecognizer(config);
				recognizer.Recognized += (s, result) =>
				{
					sb.AppendLine(result.Result.Text);
					UpdateUI(result.Result.Text);
					sendToDatabase(result.Result.Text);
				};
				await recognizer.StartContinuousRecognitionAsync().ConfigureAwait(false);

				// Listen for changes in the network connectivity
				Connectivity.ConnectivityChanged += ConnectivityChanged;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}
		}

		private void ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
		{
			if (e.NetworkAccess == NetworkAccess.Internet)
			{
				// Reconnect to the speech recognizer
				_ = StartListening();
			}
			else
			{
				// Disconnect from the speech recognizer
				StopListening();
			}
		}

		public async void StopListening()
		{
			if (Image_btn_recognize.IsAnimationPlaying)
			{
				Image_btn_recognize.IsAnimationPlaying = false;
				await recognizer.StopContinuousRecognitionAsync().ConfigureAwait(false);
				await Task.Delay(500).ConfigureAwait(false); // give some time for the last recognition to finish
				recognizer.Dispose();
			}
		}

		private async void OnRecognitionButtonClicked(object sender, EventArgs e)
		{
			bool micAccessGranted = await DependencyService.Get<IMicrophoneService>().GetPermissionsAsync();
			if (!micAccessGranted)
			{
				UpdateUI("Please give access to microphone");
				return;
			}
			if (!Image_btn_recognize.IsAnimationPlaying)
			{
				_ = StartListening();
			}
			else
			{
				StopListening();
			}
		}
		private void UpdateUI(String message)
		{
			Device.BeginInvokeOnMainThread(() =>
			{
				RecognitionText.Text = message;
			});
		}
		private async void OnDisconnectClick(object sender, EventArgs e)
		{
			StopListening();
			await firebaseClient.Child(convoID).DeleteAsync();
			await Navigation.PopAsync();
		}
		protected override void OnDisappearing()
		{
			base.OnDisappearing();
			StopListening();
		}
	}
}