using System;

namespace AegisFlow.Tests
{
    /// <summary>
    /// 架构规则测试骨架。正式项目可接入 NUnit / EditMode Test Runner。
    /// </summary>
    public sealed class ArchitectureRuleTests
    {
        public bool UIShouldNotDependOnRepository(string sourceCode)
        {
            return NotContains(sourceCode, "AegisFlow.Data.EntityRepository")
                && NotContains(sourceCode, "AegisFlow.Data.ModelRepository");
        }

        public bool ProcedureShouldNotWriteRepositoryDirectly(string sourceCode)
        {
            return NotContains(sourceCode, ".Add(new EntityData")
                && NotContains(sourceCode, ".Remove(");
        }

        public bool RuntimeShouldUseEventQueueBuilder(string sourceCode)
        {
            return Contains(sourceCode, "RuntimeEventQueueBuilder")
                && NotContains(sourceCode, "new SimulationEvent(\"Creating\"");
        }

        private bool Contains(string sourceCode, string value)
        {
            return !string.IsNullOrEmpty(sourceCode) && sourceCode.IndexOf(value, StringComparison.Ordinal) >= 0;
        }

        private bool NotContains(string sourceCode, string value)
        {
            return string.IsNullOrEmpty(sourceCode) || sourceCode.IndexOf(value, StringComparison.Ordinal) < 0;
        }
    }
}
