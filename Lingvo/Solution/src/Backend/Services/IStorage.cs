using System.IO;
using System.Threading.Tasks;

namespace Lingvo.Backend.Services
{
	public interface IStorage
	{
		Task SaveAsync(string identifier, Stream content);

		Task DeleteAsync(string identifier);

		Task<string> GetAccessUrlAsync(string identifier);
	}
}
