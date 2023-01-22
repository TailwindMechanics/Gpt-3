using System.Threading.Tasks;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;

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
        [FoldoutGroup("Tooling"), TextArea, SerializeField]
        string input;
        [FoldoutGroup("Tooling"), Button(ButtonSizes.Medium)]
        void DoCall () => Call();
        [FoldoutGroup("Tooling"), PropertyOrder(1), TextArea, SerializeField]
        string output;

        async Task Call ()
        {
            LogMessage("Ask", input);
            var api = new OpenAI_API.OpenAIAPI(openApiCredentialsSo.ApiKey);
            var temp = Random.Range(0.1f, 1.0f);
            var result = await api.Completions.CreateCompletionAsync(input, temperature: temp);
            output = result.ToString().Trim();
            LogMessage("Answer", output, 1);
        }
        #endregion Fields


        #region IO Setup
        protected override string ModuleName => ThisName(this);
        protected override int OutputCount() => outputVo.outputs.Count;
        public OutputVo<IOpenAIOutput> outputVo;
        void Awake() => outputVo = new OutputVo<IOpenAIOutput>(this);
        #endregion IO Setup
    }
}