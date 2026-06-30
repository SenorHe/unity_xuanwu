using UnityEngine;

namespace AegisFlow.Procedure
{
    /// <summary>
    /// 预加载流程。负责触发资源、配置、基础数据预加载。
    /// </summary>
    public sealed class ProcedurePreload : ProcedureBase
    {
        private readonly ProcedureDependencies m_Dependencies;

        public ProcedurePreload(ProcedureController controller, ProcedureDependencies dependencies) : base(controller)
        {
            m_Dependencies = dependencies;
        }

        public override void OnEnter()
        {
            Debug.Log("[AegisFlow] Enter ProcedurePreload.");

            // [扩展点] 接入配置表、资源清单、热更版本检查。
            m_Controller.ChangeProcedure(new ProcedureHub(m_Controller, m_Dependencies));
        }
    }
}
