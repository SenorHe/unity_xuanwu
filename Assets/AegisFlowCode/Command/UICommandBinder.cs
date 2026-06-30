using AegisFlow.Application;
using AegisFlow.Domain;
using AegisFlow.Event;
using AegisFlow.Data;
using UnityEngine;

namespace AegisFlow.Command
{
    /// <summary>
    /// UI 命令绑定器。负责把 UICommand 分发到对应领域服务。
    /// </summary>
    public sealed class UICommandBinder
    {
        private readonly UICommandDispatcher m_Dispatcher;
        private readonly EntityDomainService m_EntityDomainService;
        private readonly TwinDomainService m_TwinDomainService;
        private readonly SimulationModelAppService m_SimulationModelAppService;
        private readonly SimulationAppService m_SimulationAppService;
        private readonly TwinDC m_TwinDC;
        private readonly DomainEventBus m_EventBus;

        public UICommandBinder(
            UICommandDispatcher dispatcher,
            EntityDomainService entityDomainService,
            TwinDomainService twinDomainService,
            SimulationModelAppService simulationModelAppService,
            SimulationAppService simulationAppService,
            TwinDC twinDC,
            DomainEventBus eventBus)
        {
            m_Dispatcher = dispatcher;
            m_EntityDomainService = entityDomainService;
            m_TwinDomainService = twinDomainService;
            m_SimulationModelAppService = simulationModelAppService;
            m_SimulationAppService = simulationAppService;
            m_TwinDC = twinDC;
            m_EventBus = eventBus;
        }

        public void Bind()
        {
            m_Dispatcher.Register<CreateEntityCommand>(OnCreateEntityCommand);
            m_Dispatcher.Register<PlaceEntityCommand>(OnPlaceEntityCommand);
            m_Dispatcher.Register<MoveEntityCommand>(OnMoveEntityCommand);
            m_Dispatcher.Register<StartSimulationCommand>(OnStartSimulationCommand);
            m_Dispatcher.Register<StopSimulationCommand>(OnStopSimulationCommand);
            m_Dispatcher.Register<SaveModelCommand>(OnSaveModelCommand);
            m_Dispatcher.Register<LoadModelCommand>(OnLoadModelCommand);
            m_Dispatcher.Register<RetrySimulationEventCommand>(OnRetrySimulationEventCommand);
            m_Dispatcher.Register<SkipSimulationEventCommand>(OnSkipSimulationEventCommand);
            m_Dispatcher.Register<TerminateSimulationRuntimeCommand>(OnTerminateSimulationRuntimeCommand);
        }

        public void Unbind()
        {
            m_Dispatcher.Unregister<CreateEntityCommand>(OnCreateEntityCommand);
            m_Dispatcher.Unregister<PlaceEntityCommand>(OnPlaceEntityCommand);
            m_Dispatcher.Unregister<MoveEntityCommand>(OnMoveEntityCommand);
            m_Dispatcher.Unregister<StartSimulationCommand>(OnStartSimulationCommand);
            m_Dispatcher.Unregister<StopSimulationCommand>(OnStopSimulationCommand);
            m_Dispatcher.Unregister<SaveModelCommand>(OnSaveModelCommand);
            m_Dispatcher.Unregister<LoadModelCommand>(OnLoadModelCommand);
            m_Dispatcher.Unregister<RetrySimulationEventCommand>(OnRetrySimulationEventCommand);
            m_Dispatcher.Unregister<SkipSimulationEventCommand>(OnSkipSimulationEventCommand);
            m_Dispatcher.Unregister<TerminateSimulationRuntimeCommand>(OnTerminateSimulationRuntimeCommand);
        }

        private void OnCreateEntityCommand(CreateEntityCommand command)
        {
            if (command == null)
            {
                return;
            }

            bool isSuccess = m_EntityDomainService.CreateEntity(command.EntityData);
            if (isSuccess)
            {
                FireEntityStatusChanged(command.EntityData, "Created");
            }
            FireCommandResult(command.CommandId, isSuccess, isSuccess ? "创建实体成功。" : "创建实体失败。");
        }

        private void OnPlaceEntityCommand(PlaceEntityCommand command)
        {
            if (command == null)
            {
                return;
            }

            if (!m_TwinDomainService.CanPlace(command.EntityType, command.PosX, command.PosZ))
            {
                FireCommandResult(command.CommandId, false, "放置位置不合法。");
                return;
            }

            EntityData entityData = command.ToEntityData();
            bool isSuccess = m_EntityDomainService.CreateEntity(entityData);
            if (isSuccess)
            {
                m_TwinDC.UpdateTelemetry(
                    entityData.EntityId,
                    entityData.PosX, entityData.PosY, entityData.PosZ,
                    100f, "Placed", null, 0f);
                FireEntityStatusChanged(entityData, "Placed");
            }
            FireCommandResult(command.CommandId, isSuccess, isSuccess ? "放置实体成功。" : "放置实体失败，ID 已存在。");
        }

        private void OnMoveEntityCommand(MoveEntityCommand command)
        {
            if (command == null)
            {
                return;
            }

            bool isSuccess = m_TwinDomainService.MoveEntity(
                command.EntityId, command.PosX, command.PosY, command.PosZ, command.RotY);
            if (isSuccess)
            {
                m_TwinDC.UpdatePosition(command.EntityId, command.PosX, command.PosY, command.PosZ);
                EntityData entity = m_TwinDomainService.GetEntity(command.EntityId);
                if (entity != null)
                {
                    FireEntityStatusChanged(entity, "Moved");
                }
            }
            FireCommandResult(command.CommandId, isSuccess, isSuccess ? "移动实体成功。" : "移动实体失败。");
        }

        private void OnStartSimulationCommand(StartSimulationCommand command)
        {
            if (command == null)
            {
                return;
            }

            bool isSuccess = m_SimulationAppService.StartRuntime(command.ModelId);
            FireCommandResult(command.CommandId, isSuccess, isSuccess ? "仿真已启动。" : "仿真启动失败，请先保存模型。");
        }

        private void OnStopSimulationCommand(StopSimulationCommand command)
        {
            if (command == null)
            {
                return;
            }

            m_SimulationAppService.StopRuntime();
            m_TwinDC.Clear();
            FireCommandResult(command.CommandId, true, "仿真已停止。");
        }

        private void OnSaveModelCommand(SaveModelCommand command)
        {
            if (command == null)
            {
                return;
            }

            bool isSuccess = m_SimulationModelAppService.SaveCurrentModel();
            FireCommandResult(command.CommandId, isSuccess, isSuccess ? "保存模型成功。" : "保存模型失败。");
        }

        private void OnLoadModelCommand(LoadModelCommand command)
        {
            if (command == null)
            {
                return;
            }

            bool isSuccess = m_SimulationModelAppService.LoadModel(command.ModelId);
            FireCommandResult(command.CommandId, isSuccess, isSuccess ? "加载模型成功。" : "加载模型失败。");
        }

        private void OnRetrySimulationEventCommand(RetrySimulationEventCommand command)
        {
            if (command == null)
            {
                return;
            }

            bool isSuccess = m_SimulationAppService.RetryFailedEvent();
            FireCommandResult(command.CommandId, isSuccess, isSuccess ? "失败事件重试成功。" : "失败事件重试失败。");
        }

        private void OnSkipSimulationEventCommand(SkipSimulationEventCommand command)
        {
            if (command == null)
            {
                return;
            }

            bool isSuccess = m_SimulationAppService.SkipFailedEvent();
            FireCommandResult(command.CommandId, isSuccess, isSuccess ? "失败事件已跳过。" : "无法跳过失败事件。");
        }

        private void OnTerminateSimulationRuntimeCommand(TerminateSimulationRuntimeCommand command)
        {
            if (command == null)
            {
                return;
            }

            bool isSuccess = m_SimulationAppService.TerminateFailedRuntime();
            FireCommandResult(command.CommandId, isSuccess, isSuccess ? "失败运行态已终止。" : "无法终止失败运行态。");
        }

        private void FireEntityStatusChanged(EntityData entityData, string status)
        {
            m_EventBus.Fire(new EntityStatusChangedEvent(
                Time.time,
                entityData.EntityId,
                entityData.EntityType,
                status,
                entityData.PosX, entityData.PosY, entityData.PosZ));
        }

        private void FireCommandResult(string commandId, bool isSuccess, string message)
        {
            m_EventBus.Fire(new UICommandExecutedEvent(Time.time, commandId, isSuccess, message));
        }
    }
}
