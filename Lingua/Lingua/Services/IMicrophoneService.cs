using System.Threading.Tasks;

namespace Lingua
{
	public interface IMicrophoneService
	{
		Task<bool> GetPermissionsAsync();
		void OnRequestPermissionsResult(bool isGranted);
	}
}
