using UnityEngine;

namespace AegisFlow.Procedure
{
    /// <summary>
    /// 主入口流程。负责进入业务 Hub，不直接处理业务规则。
    /// </summary>
    public sealed class ProcedureHub : ProcedureBase
    {
        private readonly ProcedureRouteService m_RouteService;

        public ProcedureHub(ProcedureController controller, ProcedureDependencies dependencies) : base(controller)
        {
            m_RouteService = new ProcedureRouteService(controller, dependencies);
        }

        public override void OnEnter()
        {
            Debug.Log("[AegisFlow] Enter ProcedureHub.");

            // [扩展点] 打开主界面 UI，等待用户选择编辑、运行、战斗或聊天室流程。
        }

        public void EnterEditor(string modelId)
        {
            m_RouteService.ToEditor(modelId);
        }

        public void EnterRuntime(string modelId)
        {
            m_RouteService.ToRuntime(modelId);
        }
    }
}
