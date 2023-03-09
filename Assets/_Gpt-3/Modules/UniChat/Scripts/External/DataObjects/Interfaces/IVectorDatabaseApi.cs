using System.Collections.Generic;
using System.Threading.Tasks;
using System;


namespace Modules.UniChat.External.DataObjects.Interfaces
{
	public interface IVectorDatabaseApi
	{
		Task<List<Guid>> Query(IEnumerable<double> vector, bool logging = false);
		Task<Guid> Upsert(IEnumerable<double> vector, bool logging = false);
	}
}