namespace AegisFlow.UI
{
    /// <summary>
    /// Toast 动画状态机。只管理状态流转，不直接操作 Unity UI。
    /// </summary>
    public sealed class ToastAnimationStateMachine
    {
        public ToastAnimationState State { get; private set; } = ToastAnimationState.Hidden;

        public bool IsVisible => State == ToastAnimationState.Showing || State == ToastAnimationState.Visible;

        public void BeginShow()
        {
            State = ToastAnimationState.Showing;
        }

        public void MarkVisible()
        {
            State = ToastAnimationState.Visible;
        }

        public void BeginHide()
        {
            State = ToastAnimationState.Hiding;
        }

        public void MarkHidden()
        {
            State = ToastAnimationState.Hidden;
        }
    }
}
