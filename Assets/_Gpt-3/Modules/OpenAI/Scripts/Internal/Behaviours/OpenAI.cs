using System.Threading.Tasks;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;
using OpenAI_API;
using TMPro;

using Modules.ModulesSystem.External.DataObjects;
using Modules.OpenAI.External.DataObjects;


namespace Modules.OpenAI.Internal.Behaviours
{
    public class OpenAI : BaseModule
    {
        #region Fields
        [UsedImplicitly]
        const bool disabled = true;
        [FoldoutGroup("References"), GUIColor("$GetModuleColor"), SerializeField]
        OpenApiCredentialsSo openApiCredentialsSo;
        [FoldoutGroup("Tooling"), SerializeField]
        TMP_InputField inputBox;
        [FoldoutGroup("Tooling"), SerializeField]
        Transform repliesParent;
        [FoldoutGroup("Tooling"), SerializeField]
        SpawnableChatReply humanReply;
        [FoldoutGroup("Tooling"), SerializeField]
        SpawnableChatReply aiReply;
        #endregion Fields


        #region IO Setup
        protected override string ModuleName => ThisName(this);
        protected override int OutputCount() => outputVo.outputs.Count;
        public OutputVo<IOpenAIOutput> outputVo;
        void Awake() => outputVo = new OutputVo<IOpenAIOutput>(this);
        #endregion IO Setup

        void OnEnable()
            => inputBox.onSubmit.AddListener(body => OnSend("", body));
        void OnDisable()
        {
            inputBox.onSubmit.RemoveAllListeners();
            var children = repliesParent.GetComponentsInChildren<SpawnableChatReply>();
            foreach (var child in children)
            {
                Destroy(child);
            }
        }

        async Task Call (string input)
        {
            var api = new OpenAIAPI(openApiCredentialsSo.ApiKey);
            var request = new CompletionRequest
            (
                input,
                Model.DavinciText,
                200,
                0.5,
                presencePenalty: 0.1,
                frequencyPenalty: 0.1
            );

            var reply = Instantiate(aiReply, repliesParent);
            await foreach (var token in api.Completions.StreamCompletionEnumerableAsync(request))
            {
                reply.SetText("", token.ToString());
            }

            ToggleInputBox(true);
        }

        void OnSend (string title, string body)
        {
            var input = inputBox.text;
            inputBox.text = "";
            ToggleInputBox(false);
            var reply = Instantiate(humanReply, repliesParent);
            reply.SetText(title, body);
            Call(input);
        }

        void ToggleInputBox (bool isInteractable)
            => inputBox.interactable = isInteractable;
    }
}