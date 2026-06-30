using AegisFlow.Runtime;
using UnityEngine;

namespace AegisFlow.Procedure
{
    /// <summary>
    /// 仿真运行流程。负责进入运行态，实际运行由 RuntimeController 承担。
    /// </summary>
    public sealed class ProcedureSimulationRuntime : ProcedureBase
    {
        private readonly SimulationRuntimeController m_RuntimeController;

        public ProcedureSimulationRuntime(ProcedureController controller, SimulationRuntimeController runtimeController) : base(controller)
        {
            m_RuntimeController = runtimeController;
        }

        public override void OnEnter()
        {
            Debug.Log("[AegisFlow] Enter ProcedureSimulationRuntime.");

            // [扩展点] 打开运行态 UI，显示播放、暂停、速度控制。
        }

        public override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            m_RuntimeController?.Tick(elapseSeconds);
        }

        public override void OnLeave()
        {
            m_RuntimeController?.Stop();
            // [扩展点] 释放运行态实体。
        }
    }
}
