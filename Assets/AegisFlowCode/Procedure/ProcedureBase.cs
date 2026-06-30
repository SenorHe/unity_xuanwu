namespace AegisFlow.Procedure
{
    /// <summary>
    /// Procedure 岗位基类。只负责流程进入、退出和切换，不承载业务细节。
    /// </summary>
    public abstract class ProcedureBase
    {
        protected readonly ProcedureController m_Controller;

        protected ProcedureBase(ProcedureController controller)
        {
            m_Controller = controller;
        }

        public virtual void OnEnter()
        {
        }

        public virtual void OnLeave()
        {
        }

        public virtual void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
        }
    }
}
