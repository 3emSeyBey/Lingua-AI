using Android.Widget;
using Lingua.Services;
using SkiaSharp;
using System.IO;

namespace Lingua.Droid.Services
{
	public class ImageManipulation : IImageManipulation
	{
		public void miscCaller(string prompt)
		{
			Toast.MakeText(Android.App.Application.Context, prompt, ToastLength.Long).Show();
		}

		public byte[] _imageCrop(byte[] image, int deviceX)
		{
			// Load the original image from a file
			using var originalBitmap = SKBitmap.Decode(image);

			// Compute the crop region based on the dimensions of the original image
			int cropY = (int)((originalBitmap.Height - deviceX) / 2 + (deviceX - ((deviceX * 0.9)))) - 20; // X-coordinate of the top-left corner of the cropped region
			int cropX = (int)(originalBitmap.Width / 2) - 110; // Y-coordinate of the top-left corner of the cropped region
			int cropWidth = 220; // Width of the cropped region
			int cropHeight = 220; // Height of the cropped region

			// Create a new bitmap for the cropped image
			using var croppedBitmap = new SKBitmap(cropWidth, cropHeight);

			// Create a canvas for the cropped image
			using var canvas = new SKCanvas(croppedBitmap);

			// Draw the cropped region of the original image onto the canvas
			canvas.DrawBitmap(originalBitmap, new SKRect(cropX, cropY, cropX + cropWidth, cropY + cropHeight), new SKRect(0, 0, cropWidth, cropHeight));

			// Create a new bitmap for the rotated image
			using var rotatedBitmap = new SKBitmap(cropHeight, cropWidth);

			// Create a canvas for the rotated image
			using var rotatedCanvas = new SKCanvas(rotatedBitmap);

			// Rotate the canvas by -90 degrees (i.e., 270 degrees counterclockwise)
			rotatedCanvas.RotateDegrees(-90);

			// Draw the cropped image onto the rotated canvas
			rotatedCanvas.DrawBitmap(croppedBitmap, -cropHeight, 0);

			// Save the rotated bitmap to the original file path
			using (var image_ = SKImage.FromBitmap(rotatedBitmap))
			using (var data = image_.Encode(SKEncodedImageFormat.Jpeg, 100))
			{
				return data.ToArray();
			}

		}

		public byte[] _imageShrink(byte[] image, int desiredWidth)
		{
			using (var originalImage = SKBitmap.Decode(image))
			{
				// Calculate the desired height while maintaining the aspect ratio
				int desiredHeight = originalImage.Height * desiredWidth / originalImage.Width;

				// Resize the image using the desired width and height
				using (var resizedImage = originalImage.Resize(new SKImageInfo(desiredWidth, desiredHeight), SKFilterQuality.Medium))
				{

					// Create a new file stream for the shrunken image
					using (var memoryStream = new MemoryStream())
					{
						// Encode the resized image and save it to the file stream
						resizedImage.Encode(SKEncodedImageFormat.Jpeg, 80).SaveTo(memoryStream);

						return memoryStream.ToArray();
					}
					// Return the file path of the shrunken image
					// SetExifOrientation(newFilePath);
				}
			}
		}
		string IImageManipulation.saveToFolder(byte[] source, string folder, string filename)
		{
			if (folder == "train")
			{
				string downloadsPath = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads).AbsolutePath;
				string filePath = System.IO.Path.Combine(downloadsPath, filename + ".jpg");
				File.WriteAllBytes(filePath, source);
				return filePath;
			}
			else
			{
				string downloadsPath = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads).AbsolutePath + "/LinguaTraining/" + folder;
				if (!Directory.Exists(downloadsPath))
				{
					Directory.CreateDirectory(downloadsPath);
				}
				string filePath = System.IO.Path.Combine(downloadsPath, filename + ".jpg");
				File.WriteAllBytes(filePath, source);
				return downloadsPath;
			}
		}

		byte[] IImageManipulation._ImageNormalize(byte[] image, int desiredWidth, int deviceX)
		{
			return _imageCrop(_imageShrink(image, desiredWidth), deviceX);
		}
	}
}