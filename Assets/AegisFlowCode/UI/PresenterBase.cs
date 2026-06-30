namespace AegisFlow.UI
{
    /// <summary>
    /// Presenter 基类。负责将 DC / Repository 数据转为 ViewData。
    /// </summary>
    public abstract class PresenterBase<TViewData>
    {
        public abstract TViewData BuildViewData();
    }
}
