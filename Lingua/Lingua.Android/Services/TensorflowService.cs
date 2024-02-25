using Lingua.Services;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xam.Plugins.OnDeviceCustomVision;

namespace Lingua.Droid.Services
{
	public class TensorflowService : IPlatformPredictionService
	{
		public async Task<ClassificationResult> Classify(Stream imageStream)
		{
			var tags = await CrossImageClassifier.Current.ClassifyImage(imageStream);
			var bestResult = tags.OrderByDescending(t => t.Probability).First();

			return new ClassificationResult(bestResult.Tag, bestResult.Probability);
		}
	}
}