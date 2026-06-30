namespace AegisFlow.UI
{
    /// <summary>
    /// Toast 动画状态。当前只提供状态骨架，具体动画由 UI 层实现。
    /// </summary>
    public enum ToastAnimationState
    {
        Hidden,
        Showing,
        Visible,
        Hiding
    }
}
