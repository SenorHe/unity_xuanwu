# AegisFlowCode 玄盾流架构代码骨架

> 这是一套可迁入 Unity 项目的玄盾流架构 C# 代码骨架。  
> 目标不是替换旧项目，而是为新功能建立清晰边界。

---

## 1. 代码骨架范围

```text
AegisFlowCode
├── Bootstrap       启动入口
├── Procedure       流程岗位
├── Application     用例编排
├── Domain          领域防腐
├── Data            DC / Repository 数据容器
├── Event           强类型领域事件
├── Command         UI / 领域命令
├── Network         协议处理器注册表
├── Factory         实体创建、分组、组件绑定
├── UI              UIForm / Presenter 基类
├── Runtime         运行态 Snapshot / Scheduler
├── Resource        资源统一入口
├── Save            保存统一入口
└── Legacy          旧系统防腐适配器
```

---

## 2. 推荐接入顺序

### 第一步：接入 Bootstrap

将以下文件迁入 Unity：

```text
Bootstrap/GameEntry.cs
Bootstrap/AppBootstrap.cs
Procedure/ProcedureBase.cs
Procedure/ProcedureController.cs
Procedure/ProcedureLaunch.cs
Procedure/ProcedurePreload.cs
Procedure/ProcedureHub.cs
```

在启动场景中创建一个 GameObject，挂载 `AegisFlow.Bootstrap.GameEntry`。

---

### 第二步：接入资源入口

迁入：

```text
Resource/IAssetLoader.cs
Resource/AssetService.cs
Resource/ResourcesLegacyAdapter.cs
```

旧项目仍可先走 `ResourcesLegacyAdapter`，后续再替换为 UGF Resource / Addressables Adapter。

---

### 第三步：接入数据与领域层

迁入：

```text
Data/EntityData.cs
Data/EntityRepository.cs
Data/ModelRepository.cs
Domain/EntityDomainService.cs
Domain/ModelDomainService.cs
Application/RuntimeSnapshotService.cs
```

注意：

```text
Repository 只做数据容器。
业务规则写在 DomainService。
UI 不允许直接写 Repository。
```

---

### 第四步：接入 UI 基类

迁入：

```text
UI/UIFormBase.cs
UI/PresenterBase.cs
```

新 UI 必须遵守：

```text
UI 只展示 ViewData。
UI 只发 Command。
UI 不直接写 DC / Repository / PlayerState。
```

---

### 第五步：接入运行态快照

迁入：

```text
Data/RuntimeSnapshot.cs
Runtime/SimulationRuntimeController.cs
Runtime/SimulationStepScheduler.cs
Application/SimulationAppService.cs
```

运行态必须遵守：

```text
运行态不直接读编辑态 Repository。
运行态使用 RuntimeSnapshot。
StepScheduler 禁止 async void 重入。
```

---

### 第六步：接入保存系统

迁入：

```text
Save/ISaveRepository.cs
Save/SaveAppService.cs
Save/LocalSaveRepository.cs
```

真实项目中建议替换为已有高质量实现：

```text
DataSaver.SaveDataToJsonSafe
```

---

### 第七步：接入旧系统防腐层

迁入：

```text
Legacy/LegacyGameAdapter.cs
```

后续按需扩展：

```text
LegacyPlayerStateAdapter
LegacyMrCSaveAdapter
LegacyEntityRepositoryAdapter
LegacyResourceAdapter
```

---

## 3. 当前骨架重点解决的问题

| 问题 | 对应代码 |
|---|---|
| GameEntry MonoBehaviour new 风险 | Bootstrap/GameEntry.cs |
| Procedure 职责膨胀 | Procedure/ProcedureBase.cs |
| EntityRepository 上帝对象 | Domain/EntityDomainService.cs + Data/EntityRepository.cs |
| UI 直写业务数据 | UI/UIFormBase.cs + UI/PresenterBase.cs |
| 资源入口分散 | Resource/AssetService.cs |
| 运行态污染编辑态 | Data/RuntimeSnapshot.cs |
| async void 重入 | Runtime/SimulationStepScheduler.cs |
| 保存入口分散 | Save/SaveAppService.cs |
| 旧系统强耦合 | Legacy/LegacyGameAdapter.cs |

---

## 4. 第二批已补齐骨架

第二批已补齐：

```text
1. Event/DomainEvent.cs
2. Event/DomainEventBus.cs
3. Command/UICommand.cs
4. Command/CreateEntityCommand.cs
5. Network/IDataProcessor.cs
6. Network/DataProcessorRegistry.cs
7. Procedure/ProcedureSimulationEditor.cs
8. Procedure/ProcedureSimulationRuntime.cs
9. Factory/EntityFactory.cs
10. Factory/EntityGroupResolver.cs
11. Factory/EntityComponentBinder.cs
12. Factory/EntityRestoreService.cs
13. Resource/UGFResourceAdapter.cs
14. Resource/AddressablesAdapter.cs
```

## 5. 第三批已补齐骨架

第三批已补齐：

```text
1. Data/DataCenterBase.cs
2. Data/AccountDC.cs
3. Data/PlayerDC.cs
4. Data/SimulationDC.cs
5. Data/RuntimeDC.cs
6. Domain/PlayerDomainService.cs
7. Domain/BattlePrepareDomainService.cs
8. Domain/BattleInput.cs
9. Network/NetworkClient.cs
10. Network/ReqServiceBase.cs
11. Network/LoginReqService.cs
12. Runtime/SimulationEvent.cs
13. Runtime/ISimulationEventHandler.cs
14. Runtime/SimulationEventProcessor.cs
15. Runtime/CreateEntityEventHandler.cs
16. Command/UICommandDispatcher.cs
```

## 6. 第四批已补齐骨架

第四批已补齐：

```text
1. Bootstrap/AegisFlowContext.cs
2. Bootstrap/AppBootstrap.cs 装配 AegisFlowContext
3. Runtime/SimulationRuntimeController.cs 接入 RuntimeDC
4. Runtime/SimulationStepScheduler.cs 接入 SimulationEventProcessor
```

## 7. 第五批已补齐骨架

第五批已补齐：

```text
1. Network/LoginDataProcessor.cs
2. Network/PlayerDataProcessor.cs
3. Command/UICommandBinder.cs
4. Bootstrap/AegisFlowContext.cs 注册协议处理器与 UICommandBinder
5. Procedure/ProcedureSimulationRuntime.cs 离场 Stop 运行态
```

## 8. 第六批已补齐骨架

第六批已补齐：

```text
1. Event/LoginSucceededEvent.cs
2. Event/PlayerInfoUpdatedEvent.cs
3. Event/RuntimeStepChangedEvent.cs
4. Network/LoginDataProcessor.cs 写 DC 后 Fire 登录事件
5. Network/PlayerDataProcessor.cs 写 DC 后 Fire 玩家事件
6. Runtime/SimulationStepScheduler.cs step 变化后 Fire 运行态事件
7. UI/PlayerViewData.cs
8. UI/PlayerPresenter.cs
9. UI/RuntimeViewData.cs
10. UI/RuntimePresenter.cs
11. Legacy/LegacyPlayerStateAdapter.cs
12. Legacy/LegacyMrCSaveAdapter.cs
13. Legacy/LegacyEntityRepositoryAdapter.cs
14. Bootstrap/AegisFlowContext.cs 注册 EventBus / Presenter / LegacyAdapter
```

## 9. 第七批已补齐骨架

第七批已补齐：

```text
1. UI/PlayerInfoForm.cs
2. UI/RuntimeStatusForm.cs
3. Procedure/ProcedureLaunch.cs 传递 AegisFlowContext
4. Procedure/ProcedurePreload.cs 传递 AegisFlowContext
5. Procedure/ProcedureHub.cs 通过 Context 进入运行态
6. Application/SimulationAppService.cs StartRuntime 返回 bool
7. Legacy/LegacySaveMigrationService.cs
8. Bootstrap/AegisFlowContext.cs 注册 LegacySaveMigrationService
```

## 10. 第八批已补齐骨架

第八批已补齐：

```text
1. Runtime/SimulationEventQueue.cs
2. Runtime/SimulationStepScheduler.cs 按 time_step 调度事件队列
3. Runtime/SimulationRuntimeController.cs 加载/停止时维护事件队列
4. Bootstrap/AegisFlowContext.cs 注册 SimulationEventQueue
5. Save/ISaveRepository.cs 增加 TryLoad
6. Save/SaveAppService.cs 增加 TryLoad
7. Save/LocalSaveRepository.cs 增加本地读取实现
8. UI/SUIRuntimeStatus.cs
9. UI/RuntimeStatusForm.cs 接入 varSUIRuntimeStatus
10. Procedure/ProcedureSimulationEditor.cs 增加 SaveCurrentModel
```

## 11. 第九批已补齐骨架

第九批已补齐：

```text
1. Procedure/ProcedureHub.cs 新增 EnterEditor
2. Application/SimulationModelAppService.cs
3. Procedure/ProcedureSimulationEditor.cs 通过 SimulationModelAppService 保存/加载模型
4. Bootstrap/AegisFlowContext.cs 注册 SimulationModelAppService
5. Command/SaveModelCommand.cs
6. Command/UICommandBinder.cs 绑定 SaveModelCommand
7. Runtime/RuntimeEventQueueBuilder.cs
8. Runtime/SimulationRuntimeController.cs 使用 RuntimeEventQueueBuilder 构建事件队列
9. Bootstrap/AegisFlowContext.cs 注册 RuntimeEventQueueBuilder
```

## 12. 第十批已补齐骨架

第十批已补齐：

```text
1. Command/LoadModelCommand.cs
2. Command/UICommandBinder.cs 绑定 LoadModelCommand
3. Procedure/ProcedureSimulationEditor.cs 从 AegisFlowContext 注入
4. Procedure/ProcedureSimulationEditor.cs 新增 BackToHub / EnterRuntime
5. Procedure/ProcedureHub.cs 简化进入编辑态构造
6. Runtime/ActivateEntityEventHandler.cs
7. Runtime/CompleteEntityEventHandler.cs
8. Runtime/RuntimeEventQueueBuilder.cs 生成 Creating / Activating / Completed 多类型事件
9. Bootstrap/AegisFlowContext.cs 注册新增事件 Handler
10. Save/SimulationModelSaveData.cs
11. Application/SimulationModelAppService.cs 使用结构化存档数据
```

## 13. 第十一批已补齐骨架

第十一批已补齐：

```text
1. Save/SimulationModelSaveData.cs 支持 Entity 列表结构化 JSON
2. Application/SimulationModelAppService.cs 保存模型时写入 EntityRepository.GetAll()
3. Runtime/IRuntimeEventStrategy.cs
4. Runtime/DefaultRuntimeEventStrategy.cs
5. Runtime/AgvRuntimeEventStrategy.cs
6. Runtime/RuntimeEventQueueBuilder.cs 按策略生成运行态事件
7. Event/UICommandExecutedEvent.cs
8. Command/UICommandBinder.cs Fire UICommandExecutedEvent
9. Procedure/ProcedureRouteService.cs
10. Procedure/ProcedureHub.cs 使用 ProcedureRouteService
11. Procedure/ProcedureSimulationEditor.cs 使用 ProcedureRouteService 返回 Hub / Runtime
12. Bootstrap/AegisFlowContext.cs 更新 UICommandBinder 与 RuntimeEventQueueBuilder 装配
```

## 14. 第十二批已补齐骨架

第十二批已补齐：

```text
1. UI/CommandToastForm.cs
2. Data/EntityRepository.cs 增加 Clear
3. Save/SimulationModelSaveData.cs 增加 TryParse / Entities
4. Application/SimulationModelAppService.cs LoadModel 恢复 EntityRepository
5. Runtime/RuntimeEventStrategyConfig.cs
6. Runtime/ConfigRuntimeEventStrategy.cs
7. Bootstrap/AegisFlowContext.cs 以配置注册 AGV / RACK 事件序列
8. Procedure/ProcedureRouteService.cs 增加空 modelId 路由守卫
```

## 15. 第十三批已补齐骨架

第十三批已补齐：

```text
1. UI/UIFormLifecycleBinder.cs
2. Bootstrap/AegisFlowContext.cs 注册 UIFormLifecycleBinder
3. Runtime/RuntimeEventStrategyConfigLoader.cs
4. Bootstrap/AegisFlowContext.cs 通过 RuntimeEventStrategyConfigLoader.LoadDefault 加载策略配置
5. Save/SaveJsonUtility.cs
6. Save/SimulationModelSaveData.cs 使用 SaveJsonUtility.Escape
7. Procedure/ProcedureRouteService.cs 增加 HasPlayer 权限守卫
8. Procedure/ProcedureRouteService.cs 增加 SimulationDC.IsDirty 脏数据守卫
```

## 16. 第十四批已补齐骨架

第十四批已补齐：

```text
1. Runtime/RuntimeEventStrategyConfigLoader.cs 增加 LoadFromText
2. Save/ISaveJsonAdapter.cs
3. Save/LightweightSaveJsonAdapter.cs
4. Application/SimulationModelAppService.cs 通过 ISaveJsonAdapter 读写 JSON
5. Bootstrap/AegisFlowContext.cs 注册 LightweightSaveJsonAdapter
6. Event/ProcedureRouteFailedEvent.cs
7. Procedure/ProcedureRouteService.cs 路由失败时 Fire ProcedureRouteFailedEvent
8. UI/CommandToastForm.cs 订阅 ProcedureRouteFailedEvent
9. Tests/ArchitectureRuleTests.cs
```

## 17. 第十五批已补齐骨架

第十五批已补齐：

```text
1. UI/ToastViewData.cs
2. UI/ToastPresenter.cs
3. UI/CommandToastForm.cs 统一使用 ToastPresenter / ToastViewData
4. UI/UIFormLifecycleBinder.cs 注入 ToastPresenter
5. Bootstrap/AegisFlowContext.cs 注册 ToastPresenter
```

## 18. 第十六批已补齐骨架

第十六批已补齐：

```text
1. UI/ToastPolicy.cs
2. UI/ToastQueue.cs
3. UI/CommandToastForm.cs 接入 ToastQueue
4. UI/CommandToastForm.cs 增加 Update 自动隐藏与展示下一条
5. UI/UIFormLifecycleBinder.cs 注入 ToastQueue / ToastPolicy
6. Bootstrap/AegisFlowContext.cs 注册 ToastPolicy / ToastQueue
7. Bootstrap/AegisFlowContext.cs Dispose 清理 ToastQueue
```

## 19. 第十七批工程约束已补齐

第十七批已补齐：

```text
1. Data / Domain / Runtime 等模块接入 asmdef 编译边界
2. Domain / Runtime、Bootstrap / Procedure、Save / Legacy 循环依赖清理
3. ProcedureDependencies 替代 Procedure 反向依赖 AegisFlowContext
4. ArchitectureRuleTests 接入 NUnit / EditMode Runner
5. SaveJsonAdapter 版本化、原子保存与备份恢复
```

## 20. 第十八批运行时可靠性已补齐

第十八批已补齐：

```text
1. SimulationEventExecutionResult 结构化执行结果
2. SimulationEventExecutionPolicy 有限重试与失败动作
3. SimulationEventProcessor 捕获 Handler 异常并统计耗时
4. SimulationStepScheduler 失败暂停与订阅者异常隔离
5. SimulationEventExecutedEvent 运行事件观测通知
6. 默认不自动重试，仅幂等 Handler 可显式开启
```

## 21. 第十九批失败恢复已补齐

第十九批已补齐：

```text
1. Scheduler 保存失败事件与同一步剩余事件上下文
2. Retry 仅重试失败项，不重复推进 time step
3. Skip 跳过失败项后按顺序执行剩余事件
4. Terminate 清理失败上下文并停止运行态
5. Retry / Skip / Terminate UI Command
6. SimulationRecoveryEvent 恢复审计事件
```

下一步建议接入 Unity BatchMode CI，并增加失败上下文持久化与操作权限控制。

---

## 22. SenorHe 落地原则

```text
Procedure 是岗位，不是功能堆砌场。
DC 是单一事实来源，UI 只读不写。
Event 解耦，不是 Event 滥用。
能用防腐隔离解决的，不推倒重来。
能用组合解决的，不用继承强拆。
```
