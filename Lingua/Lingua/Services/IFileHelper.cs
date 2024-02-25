using System.Threading.Tasks;

namespace Lingua.Services
{
	public interface IFileHelper
	{
		Task<string> SavePhotoToInternalStorage(byte[] photoData);
	}
}
