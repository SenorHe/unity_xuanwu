using System.Collections.Generic;
using AegisFlow.Data;
using UnityEngine;

namespace AegisFlowDigitalTwin.Simulation
{
    /// <summary>
    /// AGV 任务序列生成器。为每个 AGV 生成完整的搬运任务序列：
    /// Move→Rack → Pickup → Move→WorkStation → Drop → Move→Charger → Charge
    /// </summary>
    public static class AgvTaskSequencer
    {
        public static List<AgvTask> GenerateTasks(
            IReadOnlyList<EntityData> entities,
            int agvCount)
        {
            List<AgvTask> tasks = new List<AgvTask>();

            List<EntityData> agvs = FilterByType(entities, "AGV");
            List<EntityData> racks = FilterByType(entities, "RACK");
            List<EntityData> workStations = FilterByType(entities, "WORKSTATION");
            List<EntityData> chargers = FilterByType(entities, "CHARGER");

            int rackIndex = 0;
            int wsIndex = 0;
            int chargerIndex = 0;

            for (int i = 0; i < agvs.Count; i++)
            {
                EntityData agv = agvs[i];

                if (racks.Count > 0)
                {
                    EntityData rack = racks[rackIndex % racks.Count];
                    tasks.Add(new AgvTask(agv.EntityId, "MoveToRack", rack.EntityId, new Vector3(rack.PosX, 0, rack.PosZ)));
                    tasks.Add(new AgvTask(agv.EntityId, "Pickup", rack.EntityId, Vector3.zero));
                    rackIndex++;
                }

                if (workStations.Count > 0)
                {
                    EntityData ws = workStations[wsIndex % workStations.Count];
                    tasks.Add(new AgvTask(agv.EntityId, "MoveToWorkStation", ws.EntityId, new Vector3(ws.PosX, 0, ws.PosZ)));
                    tasks.Add(new AgvTask(agv.EntityId, "Drop", ws.EntityId, Vector3.zero));
                    wsIndex++;
                }

                if (chargers.Count > 0)
                {
                    EntityData charger = chargers[chargerIndex % chargers.Count];
                    tasks.Add(new AgvTask(agv.EntityId, "MoveToCharger", charger.EntityId, new Vector3(charger.PosX, 0, charger.PosZ)));
                    tasks.Add(new AgvTask(agv.EntityId, "Charge", charger.EntityId, Vector3.zero));
                    chargerIndex++;
                }
            }

            return tasks;
        }

        private static List<EntityData> FilterByType(IReadOnlyList<EntityData> entities, string type)
        {
            List<EntityData> result = new List<EntityData>();
            for (int i = 0; i < entities.Count; i++)
            {
                if (entities[i].EntityType == type)
                {
                    result.Add(entities[i]);
                }
            }

            return result;
        }
    }

    /// <summary>
    /// 单个 AGV 任务。
    /// </summary>
    public struct AgvTask
    {
        public string AgvId;
        public string TaskType;
        public string TargetEntityId;
        public Vector3 TargetPosition;

        public AgvTask(string agvId, string taskType, string targetEntityId, Vector3 targetPosition)
        {
            AgvId = agvId;
            TaskType = taskType;
            TargetEntityId = targetEntityId;
            TargetPosition = targetPosition;
        }
    }
}
