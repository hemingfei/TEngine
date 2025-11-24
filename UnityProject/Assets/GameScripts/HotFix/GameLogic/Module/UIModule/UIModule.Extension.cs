using Cysharp.Threading.Tasks;
using System;

namespace GameLogic
{
    public sealed partial class UIModule
    {
        /// <summary>
        /// 检查是否打开了窗口
        /// </summary>
        public bool HasUIForm<T>() where T : UIWindow
        {
            return HasWindow(typeof(T));
        }

        /// <summary>
        /// 获取打开的窗口
        /// </summary>
        public UIWindow GetUIForm<T>() where T : UIWindow
        {
            string windowName = typeof(T).FullName;
            return GetWindow(windowName);
        }

        /// <summary>
        /// 异步打开窗口。
        /// </summary>
        public async UniTask OpenUIForm<T>(params System.Object[] userDatas) where T : UIWindow, new()
        {
            await ShowUIAsyncAwait<T>(userDatas);
        }

        /// <summary>
        /// 同步打开窗口。
        /// </summary>
        public void OpenUIFormSync<T>(params System.Object[] userDatas) where T : UIWindow, new()
        {
            ShowUI<T>(userDatas);
        }

        /// <summary>
        /// 关闭窗口，如果有关闭动画，则等待动画播放完毕再关闭。
        /// </summary>
        public void CloseUIForm<T>() where T : UIWindow
        {
            CloseUIForm(typeof(T));
        }

        /// <summary>
        /// 关闭窗口，如果有关闭动画，则等待动画播放完毕再关闭。
        /// </summary>
        private void CloseUIForm(Type type)
        {
            string windowName = type.FullName;
            UIWindow window = GetWindow(windowName);
            CloseUIForm(window);
        }

        /// <summary>
        /// 关闭窗口，如果有关闭动画，则等待动画播放完毕再关闭。
        /// </summary>
        private void CloseUIForm(UIWindow window)
        {
            if (window == null)
                return;
            window.PlayCloseAnim();
            if (window.UICloseAnimDuraion > 0)
            {
                window.HideTimerId = GameModule.Timer.AddTimer((arg) =>
                {
                    CloseUI(window);
                }, window.UICloseAnimDuraion);
            }
            else
            {
                CloseUI(window);
            }
        }

        /// <summary>
        /// 关闭所有窗口。
        /// </summary>
        public void CloseAllUIForm()
        {
            for (int i = _uiStack.Count - 1; i >= 0; i--)
            {
                UIWindow window = _uiStack[i];
                CloseUIForm(window);
            }
        }

        /// <summary>
        /// 关闭所有窗口除了。
        /// </summary>
        public void CloseAllUIFormWithOut<T>() where T : UIWindow
        {
            for (int i = _uiStack.Count - 1; i >= 0; i--)
            {
                UIWindow window = _uiStack[i];
                if (window.GetType() == typeof(T))
                {
                    continue;
                }
                CloseUIForm(window);
            }
        }

        private void CloseUI(UIWindow window)
        {
            if (window == null)
                return;
            window.InternalDestroy();
            Pop(window);
            OnSortWindowDepth(window.WindowLayer);
            OnSetWindowVisible();
        }
    }
}
