using UnityEngine;
using UnityEngine.UI;

namespace AegisFlow.UI
{
    /// <summary>
    /// 运行态子组件示例。var 前缀控件应由工具生成，业务代码只使用不重复声明。
    /// </summary>
    public sealed class SUIRuntimeStatus : MonoBehaviour
    {
        [SerializeField] private Text varTxtModelId;
        [SerializeField] private Text varTxtStep;
        [SerializeField] private Text varTxtPlaying;

        public void Refresh(RuntimeViewData viewData)
        {
            if (viewData == null)
            {
                return;
            }

            if (varTxtModelId != null)
            {
                varTxtModelId.text = viewData.RunningModelId;
            }

            if (varTxtStep != null)
            {
                varTxtStep.text = viewData.CurrentStep.ToString();
            }

            if (varTxtPlaying != null)
            {
                varTxtPlaying.text = viewData.IsPlaying ? "Playing" : "Stopped";
            }
        }
    }
}
