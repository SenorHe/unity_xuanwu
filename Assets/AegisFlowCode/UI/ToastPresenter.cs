using AegisFlow.Event;

namespace AegisFlow.UI
{
    /// <summary>
    /// Toast Presenter。负责把不同事件转换成统一 ToastViewData。
    /// </summary>
    public sealed class ToastPresenter
    {
        public ToastViewData Build(UICommandExecutedEvent eventData)
        {
            if (eventData == null)
            {
                return null;
            }

            return new ToastViewData(eventData.CommandId, eventData.Message, eventData.IsSuccess, eventData.IsSuccess ? 0 : 50);
        }

        public ToastViewData Build(ProcedureRouteFailedEvent eventData)
        {
            if (eventData == null)
            {
                return null;
            }

            return new ToastViewData(eventData.RouteName, eventData.Message, false, 100);
        }
    }
}
