using System.IO;
using System.Threading.Tasks;

namespace Lingua.Services
{
	public interface IPlatformPredictionService
	{
		Task<ClassificationResult> Classify(Stream imageStream);
	}
}
