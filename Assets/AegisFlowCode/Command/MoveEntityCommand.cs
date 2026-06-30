namespace AegisFlow.Command
{
    /// <summary>
    /// 移动实体位置命令。
    /// </summary>
    public sealed class MoveEntityCommand : UICommand
    {
        public string EntityId { get; private set; }
        public float PosX { get; private set; }
        public float PosY { get; private set; }
        public float PosZ { get; private set; }
        public float RotY { get; private set; }

        public MoveEntityCommand(
            string entityId,
            float posX, float posY, float posZ,
            float rotY = 0f) : base("MoveEntity")
        {
            EntityId = entityId;
            PosX = posX;
            PosY = posY;
            PosZ = posZ;
            RotY = rotY;
        }
    }
}
