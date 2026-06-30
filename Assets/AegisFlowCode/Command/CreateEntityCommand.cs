using AegisFlow.Data;

namespace AegisFlow.Command
{
    /// <summary>
    /// 创建实体命令。由 UI / RTE 发起，由 ApplicationService 处理。
    /// </summary>
    public sealed class CreateEntityCommand : UICommand
    {
        public EntityData EntityData { get; private set; }

        public CreateEntityCommand(EntityData entityData) : base("CreateEntity")
        {
            EntityData = entityData;
        }
    }
}
