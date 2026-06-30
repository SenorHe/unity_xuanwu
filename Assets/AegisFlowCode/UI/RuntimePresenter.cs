using AegisFlow.Data;

namespace AegisFlow.UI
{
    /// <summary>
    /// 运行态 Presenter。只读 RuntimeDC，不直接驱动运行态逻辑。
    /// </summary>
    public sealed class RuntimePresenter : PresenterBase<RuntimeViewData>
    {
        private readonly RuntimeDC m_RuntimeDC;

        public RuntimePresenter(RuntimeDC runtimeDC)
        {
            m_RuntimeDC = runtimeDC;
        }

        public override RuntimeViewData BuildViewData()
        {
            return new RuntimeViewData(
                m_RuntimeDC.RunningModelId,
                m_RuntimeDC.CurrentStep,
                m_RuntimeDC.IsPlaying);
        }
    }
}
