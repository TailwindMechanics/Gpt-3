
using Modules.ModulesSystem.External.DataObjects;
using Modules.UniChat.External.DataObjects;


namespace Modules.UniChat.Internal.Behaviours
{
    public class UniChat : BaseModule
    {
        #region IO Setup
        protected override string ModuleName => ThisName(this);
        protected override int OutputCount() => outputVo.outputs.Count;
        public OutputVo<IOpenAIOutput> outputVo;
        void Awake() => outputVo = new OutputVo<IOpenAIOutput>(this);
        #endregion IO Setup
    }
}