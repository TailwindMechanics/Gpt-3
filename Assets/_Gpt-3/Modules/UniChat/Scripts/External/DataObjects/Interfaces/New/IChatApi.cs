using System.Threading.Tasks;
using OpenAI.Models;


namespace Modules.UniChat.External.DataObjects.Interfaces.New
{
	public interface IChatApi
	{
		Task<string> GetReply(string message, Model model);
	}
}