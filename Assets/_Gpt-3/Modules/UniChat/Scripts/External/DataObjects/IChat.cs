using System.Threading.Tasks;

namespace Modules.UniChat.External.DataObjects
{
	public interface IChat
	{
		Task<string> GetResponse(string message);
	}
}