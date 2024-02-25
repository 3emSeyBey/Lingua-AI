using Lingua.Droid.Services;
using Lingua.Services;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(FileHelper))]

namespace Lingua.Droid.Services
{

	public class FileHelper : IFileHelper
	{
		public async Task<string> SavePhotoToInternalStorage(byte[] photoData)
		{
			string dirPath = Android.App.Application.Context.GetExternalFilesDir(null).AbsolutePath;
			string photosDirPath = System.IO.Path.Combine(dirPath, "Photos");
			System.IO.Directory.CreateDirectory(photosDirPath);
			string fileName = "myphoto.jpg";
			string filePath = System.IO.Path.Combine(photosDirPath, fileName);

			using (FileStream stream = new FileStream(filePath, FileMode.Create))
			{
				await stream.WriteAsync(photoData, 0, photoData.Length);
			}

			return filePath;
		}
	}
}