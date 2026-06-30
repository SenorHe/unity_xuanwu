using AegisFlow.Event;
using UnityEngine;

namespace AegisFlow.Procedure
{
    /// <summary>
    /// Procedure 路由服务。集中管理流程跳转，避免各岗位散落 new Procedure。
    /// </summary>
    public sealed class ProcedureRouteService
    {
        private readonly ProcedureController m_Controller;
        private readonly ProcedureDependencies m_Dependencies;

        public ProcedureRouteService(ProcedureController controller, ProcedureDependencies dependencies)
        {
            m_Controller = controller;
            m_Dependencies = dependencies;
        }

        public void ToHub()
        {
            if (m_Dependencies.SimulationDC.IsDirty)
            {
                Fail("ToHub", "当前模型未保存，无法返回 Hub。");
                return;
            }

            m_Controller.ChangeProcedure(new ProcedureHub(m_Controller, m_Dependencies));
        }

        public void ToEditor(string modelId)
        {
            if (!m_Dependencies.PlayerDomainService.HasPlayer())
            {
                Fail("ToEditor", "尚未加载玩家信息，无法进入编辑态。");
                return;
            }

            if (string.IsNullOrEmpty(modelId))
            {
                Fail("ToEditor", "无法进入编辑态：modelId 为空。");
                return;
            }

            m_Dependencies.SimulationDC.SwitchModel(modelId);
            m_Dependencies.ModelDomainService.Register(modelId);
            m_Controller.ChangeProcedure(new ProcedureSimulationEditor(m_Controller, m_Dependencies));
        }

        public void ToRuntime(string modelId)
        {
            if (m_Dependencies.SimulationDC.IsDirty)
            {
                Fail("ToRuntime", "当前模型未保存，无法进入运行态。");
                return;
            }

            if (string.IsNullOrEmpty(modelId))
            {
                Fail("ToRuntime", "无法进入运行态：modelId 为空。");
                return;
            }

            if (!m_Dependencies.SimulationAppService.StartRuntime(modelId))
            {
                Fail("ToRuntime", $"无法进入运行态：{modelId}");
                return;
            }

            m_Controller.ChangeProcedure(new ProcedureSimulationRuntime(m_Controller, m_Dependencies.SimulationRuntimeController));
        }

        private void Fail(string routeName, string message)
        {
            Debug.LogWarning($"[AegisFlow] {message}");
            m_Dependencies.DomainEventBus.Fire(new ProcedureRouteFailedEvent(Time.time, routeName, message));
        }
    }
}
