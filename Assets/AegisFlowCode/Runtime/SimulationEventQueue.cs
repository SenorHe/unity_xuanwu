using System.Collections.Generic;

namespace AegisFlow.Runtime
{
    /// <summary>
    /// 仿真事件队列。按 time_step 存取事件，Scheduler 只负责调度。
    /// </summary>
    public sealed class SimulationEventQueue
    {
        private readonly Dictionary<int, List<SimulationEvent>> m_EventDic = new Dictionary<int, List<SimulationEvent>>();

        public void Enqueue(SimulationEvent simulationEvent)
        {
            if (simulationEvent == null)
            {
                return;
            }

            if (!m_EventDic.TryGetValue(simulationEvent.TimeStep, out List<SimulationEvent> events))
            {
                events = new List<SimulationEvent>();
                m_EventDic.Add(simulationEvent.TimeStep, events);
            }

            events.Add(simulationEvent);
        }

        public List<SimulationEvent> Dequeue(int timeStep)
        {
            if (!m_EventDic.TryGetValue(timeStep, out List<SimulationEvent> events))
            {
                return null;
            }

            m_EventDic.Remove(timeStep);
            return events;
        }

        public void Clear()
        {
            m_EventDic.Clear();
        }
    }
}
