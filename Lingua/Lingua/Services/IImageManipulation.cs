namespace Lingua.Services
{
	public interface IImageManipulation
	{
		//string _imageCrop(string source, int deviceX);
		string saveToFolder(byte[] source, string folder, string filename);
		byte[] _ImageNormalize(byte[] image, int desiredWidth, int deviceX);

		void miscCaller(string prompt);
	}
}
