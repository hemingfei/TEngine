using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Doozy.Engine.UI;
using Doozy.Engine.UI.Animation;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace GameLogic
{
    public abstract class UIAnimWindow : UIWindow
    {
        private UIAnimView[] _doozyViews;
        private UIAnimView _rootDoozyView;

        internal override void BindAnimProperty()
        {
            _rootDoozyView = gameObject.GetComponent<UIAnimView>();
            _doozyViews = gameObject.GetComponentsInChildren<UIAnimView>(true);
            if (_doozyViews != null && _doozyViews.Length > 0)
            {
                float[] uiCloseAnimDuraion = new float[_doozyViews.Length];
                for (int i = 0; i < _doozyViews.Length; i++)
                {
                    _doozyViews[i].InstantHide();
                    uiCloseAnimDuraion[i] = _doozyViews[i].UIAnimationOnHide.TotalDuration;
                }
                UICloseAnimDuraion = Mathf.Max(uiCloseAnimDuraion);
            }
            if (_rootDoozyView == null)
            {
                UICloseAnimDuraion = 0;
            }
            else
            {
                if (!_rootDoozyView.NeedHideAnim)
                {
                    UICloseAnimDuraion = 0;
                }
            }
        }

        internal override void PlayOpenAnim()
        {
            if (_rootDoozyView != null && _rootDoozyView.NeedShowAnim && _doozyViews != null)
            {
                for (int i = 0; i < _doozyViews.Length; i++)
                {
                    _doozyViews[i].Show();
                }
            }
        }
        internal override void PlayCloseAnim()
        {
            if (_rootDoozyView != null && _rootDoozyView.NeedHideAnim && _doozyViews != null)
            {
                for (int i = 0; i < _doozyViews.Length; i++)
                {
                    _doozyViews[i].Hide();
                }
            }
        }
    }
}