using System.Collections.Generic;
using System.Threading.Tasks;
using System;


namespace Modules.UniChat.External.DataObjects.Interfaces
{
	public interface IVectorDatabaseApi
	{
		Task DeleteAllVectorsInNamespace(string nameSpace, bool logging = false);
		Task<string> DescribeIndexStats(bool logging = false);
		Task<List<Guid>> Query(string nameSpace, IEnumerable<double> vector, float minScore, int numNeighbours, bool logging = false);
		Task<Guid> Upsert(string botName, IEnumerable<double> vector, bool logging = false);
	}
}