using AegisFlow.Application;
using AegisFlow.Domain;
using AegisFlow.Event;
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
        private readonly SimulationModelAppService m_SimulationModelAppService;
        private readonly SimulationAppService m_SimulationAppService;
        private readonly DomainEventBus m_EventBus;

        public UICommandBinder(
            UICommandDispatcher dispatcher,
            EntityDomainService entityDomainService,
            SimulationModelAppService simulationModelAppService,
            SimulationAppService simulationAppService,
            DomainEventBus eventBus)
        {
            m_Dispatcher = dispatcher;
            m_EntityDomainService = entityDomainService;
            m_SimulationModelAppService = simulationModelAppService;
            m_SimulationAppService = simulationAppService;
            m_EventBus = eventBus;
        }

        public void Bind()
        {
            m_Dispatcher.Register<CreateEntityCommand>(OnCreateEntityCommand);
            m_Dispatcher.Register<SaveModelCommand>(OnSaveModelCommand);
            m_Dispatcher.Register<LoadModelCommand>(OnLoadModelCommand);
            m_Dispatcher.Register<RetrySimulationEventCommand>(OnRetrySimulationEventCommand);
            m_Dispatcher.Register<SkipSimulationEventCommand>(OnSkipSimulationEventCommand);
            m_Dispatcher.Register<TerminateSimulationRuntimeCommand>(OnTerminateSimulationRuntimeCommand);
        }

        public void Unbind()
        {
            m_Dispatcher.Unregister<CreateEntityCommand>(OnCreateEntityCommand);
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
            FireCommandResult(command.CommandId, isSuccess, isSuccess ? "创建实体成功。" : "创建实体失败。");
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

        private void FireCommandResult(string commandId, bool isSuccess, string message)
        {
            m_EventBus.Fire(new UICommandExecutedEvent(Time.time, commandId, isSuccess, message));
        }
    }
}
