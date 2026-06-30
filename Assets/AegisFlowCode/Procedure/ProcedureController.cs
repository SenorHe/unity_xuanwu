using UnityEngine;

namespace AegisFlow.Procedure
{
    /// <summary>
    /// 简化版 Procedure 控制器。正式接入 UGF 时可替换为 GameFramework ProcedureComponent。
    /// </summary>
    public sealed class ProcedureController
    {
        private ProcedureBase m_CurrentProcedure;

        public ProcedureBase CurrentProcedure => m_CurrentProcedure;

        public void ChangeProcedure(ProcedureBase nextProcedure)
        {
            if (nextProcedure == null)
            {
                Debug.LogError("[AegisFlow] 目标 Procedure 为空。");
                return;
            }

            m_CurrentProcedure?.OnLeave();
            m_CurrentProcedure = nextProcedure;
            m_CurrentProcedure.OnEnter();
        }

        public void Update(float elapseSeconds, float realElapseSeconds)
        {
            m_CurrentProcedure?.OnUpdate(elapseSeconds, realElapseSeconds);
        }

        public void Shutdown()
        {
            m_CurrentProcedure?.OnLeave();
            m_CurrentProcedure = null;
        }
    }
}
