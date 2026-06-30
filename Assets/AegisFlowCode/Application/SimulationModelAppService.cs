using AegisFlow.Data;
using AegisFlow.Save;

namespace AegisFlow.Application
{
    /// <summary>
    /// 仿真模型应用服务。负责编辑态模型保存与加载编排。
    /// </summary>
    public sealed class SimulationModelAppService
    {
        private readonly ModelRepository m_ModelRepository;
        private readonly EntityRepository m_EntityRepository;
        private readonly SimulationDC m_SimulationDC;
        private readonly ISaveJsonAdapter m_SaveJsonAdapter;

        public SimulationModelAppService(
            ModelRepository modelRepository,
            EntityRepository entityRepository,
            SimulationDC simulationDC,
            ISaveJsonAdapter saveJsonAdapter)
        {
            m_ModelRepository = modelRepository;
            m_EntityRepository = entityRepository;
            m_SimulationDC = simulationDC;
            m_SaveJsonAdapter = saveJsonAdapter;
        }

        public bool SaveCurrentModel()
        {
            if (string.IsNullOrEmpty(m_SimulationDC.CurrentModelId))
            {
                return false;
            }

            string modelId = m_SimulationDC.CurrentModelId;
            SimulationModelSaveData saveData = new SimulationModelSaveData(modelId, false, m_EntityRepository.GetAll());

            if (!SaveAppService.Save(BuildSaveKey(modelId), m_SaveJsonAdapter.ToJson(saveData)))
            {
                return false;
            }

            m_ModelRepository.Add(modelId);
            m_SimulationDC.MarkSaved();
            return true;
        }

        public bool LoadModel(string modelId)
        {
            if (string.IsNullOrEmpty(modelId))
            {
                return false;
            }

            string saveKey = BuildSaveKey(modelId);

            if (!TryLoadSaveData(saveKey, modelId, out SimulationModelSaveData saveData))
            {
                return false;
            }

            m_EntityRepository.Clear();

            for (int i = 0; i < saveData.Entities.Count; i++)
            {
                m_EntityRepository.Add(saveData.Entities[i]);
            }

            m_ModelRepository.Add(saveData.ModelId);
            m_SimulationDC.SwitchModel(saveData.ModelId);
            return true;
        }

        private bool TryLoadSaveData(string saveKey, string expectedModelId, out SimulationModelSaveData saveData)
        {
            saveData = null;

            if (SaveAppService.TryLoad(saveKey, out string json)
                && TryParseMatchingModel(json, expectedModelId, out saveData))
            {
                return true;
            }

            return SaveAppService.TryLoadBackup(saveKey, out string backupJson)
                && TryParseMatchingModel(backupJson, expectedModelId, out saveData);
        }

        private bool TryParseMatchingModel(string json, string expectedModelId, out SimulationModelSaveData saveData)
        {
            saveData = null;

            return !string.IsNullOrEmpty(json)
                && m_SaveJsonAdapter.TryFromJson(json, out saveData)
                && saveData.ModelId == expectedModelId;
        }

        private string BuildSaveKey(string modelId)
        {
            return $"simulation_{modelId}";
        }
    }
}
