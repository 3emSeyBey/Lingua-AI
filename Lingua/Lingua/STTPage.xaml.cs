using Firebase.Database;
using Firebase.Database.Query;
using Lingua.Services;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Lingua
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class STTPage : ContentPage
	{
		FirebaseClient firebaseClient = new FirebaseClient("https://linguamessaging-default-rtdb.asia-southeast1.firebasedatabase.app/");
		bool isOnLoop = false;
		bool isToVoice;
		string convoID;
		public STTPage(bool _isToVoice, string _convoID)
		{
			InitializeComponent();
			isToVoice = _isToVoice;
			NavigationPage.SetHasNavigationBar(this, false);
			convoID = _convoID;
		}
		public async void Button_Azure_Clicked(System.IO.Stream image)
		{
			var imageStream = image;
			if (imageStream == null)
				return;
			var client = new CustomVisionPredictionClient(new Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction.ApiKeyServiceClientCredentials(KeysAndUrls.CustomVisionPredictionApiKey))
			{
				Endpoint = KeysAndUrls.PredictionUrl
			};
			TagLabel.Text = "Loading...";
			var result = await client.ClassifyImageAsync(KeysAndUrls.ProjectId, KeysAndUrls.IterationName, imageStream);
			var bestResult = result.Predictions.OrderByDescending(p => p.Probability).FirstOrDefault();

			if (bestResult == null)
				return;

			TagLabel.Text = "Best Prediction: " + bestResult.TagName + " (" + Math.Round(bestResult.Probability * 100, 2).ToString() + ")";
			if (bestResult.TagName == "BACKSPACE")
			{
				subPredictText.Text = subPredictText.Text.Substring(0, subPredictText.Text.Length - 1);
			}
			else if (bestResult.TagName == "SPACE")
			{
				subPredictText.Text += " ";
			}
			else if (char.IsUpper(bestResult.TagName[0]))
			{
				subPredictText.Text += bestResult.TagName;
			}
		}
		private async void CloseAndBackAsync(object sender, EventArgs e)
		{
			isOnLoop = false;
			await firebaseClient.Child(convoID).DeleteAsync();
			await Navigation.PopAsync();
		}
		private void suggestionSelected(object sender, SelectedItemChangedEventArgs e)
		{
			if (e.SelectedItem == null)
			{
				return;
			}
			string originalString = subPredictText.Text;
			string newLastWord = e.SelectedItem.ToString();

			// Split the string into words
			if (originalString.Contains(" "))
			{
				string[] words = originalString.Split(' ');
				// Replace the last word with the new word
				if (words.Length > 0)
				{
					words[words.Length - 1] = newLastWord;
				}
				// Join the words back into a string
				subPredictText.Text = string.Join(" ", words);
			}
			else
			{
				subPredictText.Text = newLastWord;
			}
		}
		public async void sendToDatabase(object sender, EventArgs e)
		{
			var mesClass = new MessagingRecord();
			if (!isToVoice)
			{
				mesClass.MessagingProperty += "\n" + subPredictText.Text;
				await firebaseClient.Child(convoID).PostAsync(new MessagingRecord
				{
					MessagingProperty = subPredictText.Text
				});
			}
			else
			{
				mesClass.MessagingProperty += "\n" + "~" + subPredictText.Text;
				await firebaseClient.Child(convoID).PostAsync(new MessagingRecord
				{
					MessagingProperty = "~" + subPredictText.Text
				});
			}
		}
		private void StartContinousCapture(object sender, EventArgs e)
		{
			isOnLoop = true;
			cameraView.Shutter();
		}
		private void PauseContinousCapture(object sender, EventArgs e)
		{
			isOnLoop = false;
		}
		private async void CaptureTextChanged(object sender, EventArgs e)
		{
			if (isOnLoop)
			{
				getSuggestion();
				await Task.Delay(1500);
				TagLabel.Text = "Capturing Image...";
				await Task.Delay(300);
				cameraView.Shutter();
			}
		}
		public async void getSuggestion()
		{
			var client = new HttpClient();
			var request = new HttpRequestMessage
			{
				Method = HttpMethod.Post,
				RequestUri = new Uri("https://typewise-ai.p.rapidapi.com/completion/complete"),
				Headers =
				{
					{ "X-RapidAPI-Key", "3e7b41810dmsh36643b0730bb46fp1ab16bjsnf465416451bc" },
					{ "X-RapidAPI-Host", "typewise-ai.p.rapidapi.com" },
				},
				Content = new StringContent("{\r\"text\": \"" + subPredictText.Text + "\",\r\"correctTypoInPartialWord\": false,\r\"language\": \"en\"\r}")
				{
					Headers =
						{
							ContentType = new MediaTypeHeaderValue("application/json")
						}
				}
			};
			var response = await client.SendAsync(request);

			if (response.StatusCode == System.Net.HttpStatusCode.OK)
			{
				var jsonResponse = await response.Content.ReadAsStringAsync();
				var jsonDoc = JsonDocument.Parse(jsonResponse);
				var predictionsArray = jsonDoc.RootElement.GetProperty("predictions");

				List<string> predictionsList = new List<string>();
				foreach (var prediction in predictionsArray.EnumerateArray())
				{
					var predictionText = prediction.GetProperty("text").GetString();
					predictionsList.Add(predictionText);
				}
				listView.ItemsSource = predictionsList;
			}
			else
			{
				listView.ItemsSource = new List<string>() { "error" };
			}
		}
		private void OnMediaCaptured(object sender, MediaCapturedEventArgs e)
		{
			byte[] normalizedImage = DependencyService.Get<IImageManipulation>()._ImageNormalize(e.ImageData, (int)Application.Current.MainPage.Height, (int)Application.Current.MainPage.Width);
			Button_Azure_Clicked(new MemoryStream(normalizedImage));
		}
	}

}