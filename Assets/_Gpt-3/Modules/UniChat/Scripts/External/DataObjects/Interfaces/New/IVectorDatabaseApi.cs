using System.Collections.Generic;
using System.Threading.Tasks;
using System;


namespace Modules.UniChat.External.DataObjects.Interfaces.New
{
	public interface IVectorDatabaseApi
	{
		Task<List<Guid>> Query(IReadOnlyList<double> vector, bool logging = false);
		Task<string> Upsert(string message);
	}
}