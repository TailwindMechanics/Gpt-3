using System.Threading.Tasks;


namespace Modules.UniChat.External.DataObjects.Interfaces
{
	public interface IChat
	{
		Task<string> GetResponse(string message);
	}
}