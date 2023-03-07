using System.Collections.Generic;
using System.Threading.Tasks;


namespace Modules.UniChat.External.DataObjects.Interfaces.New
{
	public interface IVectorDatabaseApi
	{
		Task<string> Query(IReadOnlyList<double> vector, bool logging = false);
		Task<string> Upsert(string message);
	}
}