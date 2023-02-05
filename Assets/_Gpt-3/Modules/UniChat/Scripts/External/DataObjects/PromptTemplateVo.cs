using UnityEngine;
using System;


namespace Modules.UniChat.External.DataObjects
{
	[Serializable]
	public class PromptTemplateVo
	{
		public string Direction => direction;
		public string Memory => memory;


		[TextArea(10, 10), SerializeField] string memory;
		[TextArea(10, 10), SerializeField] string direction;


		/*

		 Yes, you could add several things to the prompt template to improve the dialogue and better retain information over time.

Relevance to current conversation: You could include information about the current conversation topic and goal, so that the AI can better understand the context and provide more relevant responses.

Keywords and entities: You could extract and include important keywords and entities from the conversation history, so that the AI can prioritize and focus on retaining information related to them.

User preferences: You could include information about the user's preferences and interests, so that the AI can tailor its responses and prioritize information accordingly.

Previous questions and answers: You could include the previous questions and answers in the conversation, so that the AI can better retain context and build upon previous discussions.

Feedback: You could include feedback mechanisms in the prompt, such as asking the AI to reflect on its previous responses and evaluate its performance, so that it can continually improve its memory retention and dialogue capabilities.

These are just a few examples, and the exact details of what you include in the prompt will depend on your specific goals and needs for the project.



		 */
	}
}