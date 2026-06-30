using AegisFlow.Application;
using AegisFlow.Data;
using UnityEngine;

namespace AegisFlow.Procedure
{
    /// <summary>
    /// 仿真编辑流程。负责进入编辑态，不直接操作 Repository 字典。
    /// </summary>
    public sealed class ProcedureSimulationEditor : ProcedureBase
    {
        private readonly ProcedureRouteService m_RouteService;
        private readonly SimulationModelAppService m_SimulationModelAppService;
        private readonly SimulationDC m_SimulationDC;

        public ProcedureSimulationEditor(ProcedureController controller, ProcedureDependencies dependencies) : base(controller)
        {
            m_RouteService = new ProcedureRouteService(controller, dependencies);
            m_SimulationModelAppService = dependencies.SimulationModelAppService;
            m_SimulationDC = dependencies.SimulationDC;
        }

        public override void OnEnter()
        {
            Debug.Log("[AegisFlow] Enter ProcedureSimulationEditor.");

            // [扩展点] 打开编辑态 UI / RTE，绑定 Presenter。
        }

        public bool SaveCurrentModel()
        {
            return m_SimulationModelAppService.SaveCurrentModel();
        }

        public bool LoadModel(string modelId)
        {
            return m_SimulationModelAppService.LoadModel(modelId);
        }

        public void BackToHub()
        {
            m_RouteService.ToHub();
        }

        public void EnterRuntime()
        {
            if (string.IsNullOrEmpty(m_SimulationDC.CurrentModelId))
            {
                return;
            }

            m_RouteService.ToRuntime(m_SimulationDC.CurrentModelId);
        }

        public override void OnLeave()
        {
            // [扩展点] 关闭编辑态 UI，解除 RTE 监听。
        }
    }
}
