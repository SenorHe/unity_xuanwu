using UnityEngine;

namespace AegisFlow.Procedure
{
    /// <summary>
    /// 启动流程。这里只做启动阶段调度，不处理业务细节。
    /// </summary>
    public sealed class ProcedureLaunch : ProcedureBase
    {
        private readonly ProcedureDependencies m_Dependencies;

        public ProcedureLaunch(ProcedureController controller, ProcedureDependencies dependencies) : base(controller)
        {
            m_Dependencies = dependencies;
        }

        public override void OnEnter()
        {
            Debug.Log("[AegisFlow] Enter ProcedureLaunch.");
            m_Controller.ChangeProcedure(new ProcedurePreload(m_Controller, m_Dependencies));
        }
    }
}
