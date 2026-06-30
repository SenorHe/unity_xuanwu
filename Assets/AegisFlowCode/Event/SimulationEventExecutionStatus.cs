namespace AegisFlow.Event
{
    public enum SimulationEventExecutionStatus
    {
        Succeeded,
        InvalidEvent,
        HandlerNotFound,
        HandlerReturnedFailure,
        HandlerException
    }
}
