using System.Threading.Tasks;
using GoogleSearchIntegration.DTO;

namespace GoogleSearchIntegration.Interfaces
{
	public interface IGoogleSearchClient
	{
		Task<string> GetGoogleSearchResultsAsync(string searchString);
	}
}