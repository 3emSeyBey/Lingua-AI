using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Lingua.Droid.Services;
using Lingua.Services;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(ImageManipulation))]

namespace Lingua.Droid
{
	[Activity(Label = "Lingua", Icon = "@mipmap/icon", Theme = "@style/MainTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
	{
		//allowing the device to change the screen orientation based on the rotation 

		private const int RECORD_AUDIO = 1;
		private IMicrophoneService micService;
		internal static MainActivity Instance { get; private set; }

		protected override void OnCreate(Bundle savedInstanceState)
		{
			Xamarin.Essentials.Platform.Init(this, savedInstanceState);
			ZXing.Net.Mobile.Forms.Android.Platform.Init();
			Instance = this;
			TabLayoutResource = Resource.Layout.Tabbar;
			ToolbarResource = Resource.Layout.Toolbar;

			base.OnCreate(savedInstanceState);
			Xamarin.Essentials.Platform.Init(this, savedInstanceState);
			global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
			LoadApplication(new App());
			Xamarin.Forms.DependencyService.Register<IMicrophoneService, MicrophoneService>();
			Xamarin.Forms.DependencyService.Register<IImageManipulation, ImageManipulation>();
			micService = Xamarin.Forms.DependencyService.Get<IMicrophoneService>();
			MessagingCenter.Subscribe<STTPage>(this, "AllowLandscape", sender =>
			{
				RequestedOrientation = ScreenOrientation.ReverseLandscape;
			});
			//during page close setting back to portrait
			MessagingCenter.Subscribe<STTPage>(this, "PreventLandscape", sender =>
			{
				RequestedOrientation = ScreenOrientation.Portrait;
			});
		}
		public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
		{
			Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

			base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

			switch (requestCode)
			{
				case RECORD_AUDIO:
					{
						if (grantResults[0] == Permission.Granted)
						{
							micService.OnRequestPermissionsResult(true);
						}
						else
						{
							micService.OnRequestPermissionsResult(false);
						}
					}
					break;
			}
		}
	}
}