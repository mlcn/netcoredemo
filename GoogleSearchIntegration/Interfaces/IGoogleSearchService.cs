using System.Threading.Tasks;
using GoogleSearchIntegration.DTO;

namespace GoogleSearchIntegration.Interfaces
{
	public interface IGoogleSearchService
	{
		Task<string[]> GetSearchResults(GoogleSearchRequest request);
	}
}