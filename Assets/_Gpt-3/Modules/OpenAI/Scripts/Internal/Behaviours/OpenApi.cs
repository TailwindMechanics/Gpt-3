using Modules.ModulesSystem.External.DataObjects;
using Modules.OpenAI.External.DataObjects;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Modules.OpenAI.Internal.Behaviours
{
    public class OpenApi : BaseModule
    {
        #region Fields
        [FoldoutGroup("References"), GUIColor("$GetModuleColor"), SerializeField]
        OpenApiCredentialsSo openApiCredentialsSo;
        #endregion Fields


        #region IO Setup
        public override int OutputCount() => outputVo.outputs.Count;
        public OutputVo<IOpenAIOutput> outputVo;
        void Awake() => outputVo = new OutputVo<IOpenAIOutput>(this);
        #endregion IO Setup
    }
}