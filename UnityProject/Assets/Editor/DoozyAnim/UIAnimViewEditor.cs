using System;
using System.Collections;
using System.Collections.Generic;
using DG.DOTweenEditor;
using Doozy.Editor.UI.Animation;
using Doozy.Engine.UI;
using Doozy.Engine.UI.Animation;
using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;
using System.Text;
using System.IO;

namespace Doozy.Editor.UI
{
    [CustomEditor(typeof(UIAnimView))]
    public class UIAnimViewEditor : UnityEditor.Editor
    {
        private UIAnimView m_UIView;
        private SerializedProperty m_StartBehavior;

        private bool _needShowAnim;
        private bool _needHideAnim;

        #region OnShow

        private UIAnimationData databaseOnShow;
        private bool isMoveEnabledOnShow, isRotateEnabledOnShow, isScaleEnabledOnShow, isFadeEnabledOnShow;
        private bool isEditorNewOnShowAnimation;
        private int selectIndexOnShowPresetCategory;
        private int selectIndexOnShowPresetName;

        private SerializedProperty m_OnShowPresetCategory,
            m_OnShowPresetName,
            m_MoveOnShow,
            m_RotateOnShow,
            m_ScaleOnShow,
            m_FadeOnShow;

        #endregion
        #region OnHide

        private UIAnimationData databaseOnHide;
        private bool isMoveEnabledOnHide, isRotateEnabledOnHide, isScaleEnabledOnHide, isFadeEnabledOnHide;
        private bool isEditorNewOnHideAnimation;
        private int selectIndexOnHidePresetCategory;
        private int selectIndexOnHidePresetName;

        private SerializedProperty m_OnHidePresetCategory,
            m_OnHidePresetName,
            m_MoveOnHide,
            m_RotateOnHide,
            m_ScaleOnHide,
            m_FadeOnHide;

        #endregion
        private void OnEnable()
        {
            m_UIView = target as UIAnimView;
            isEditorNewOnShowAnimation = false;
            isEditorNewOnHideAnimation = false;
            _needShowAnim = m_UIView.NeedShowAnim;
            _needHideAnim = m_UIView.NeedHideAnim;
            m_StartBehavior = serializedObject.FindProperty("StartBehavior");
            m_OnShowPresetCategory = serializedObject.FindProperty("OnShowPresetCategory");
            m_OnShowPresetName = serializedObject.FindProperty("OnShowPresetName");
            m_MoveOnShow = serializedObject.FindProperty("MoveOnShow");
            m_RotateOnShow = serializedObject.FindProperty("RotateOnShow");
            m_ScaleOnShow = serializedObject.FindProperty("ScaleOnShow");
            m_FadeOnShow = serializedObject.FindProperty("FadeOnShow");

            m_OnHidePresetCategory = serializedObject.FindProperty("OnHidePresetCategory");
            m_OnHidePresetName = serializedObject.FindProperty("OnHidePresetName");
            m_MoveOnHide = serializedObject.FindProperty("MoveOnHide");
            m_RotateOnHide = serializedObject.FindProperty("RotateOnHide");
            m_ScaleOnHide = serializedObject.FindProperty("ScaleOnHide");
            m_FadeOnHide = serializedObject.FindProperty("FadeOnHide");

            try
            {
                if (string.IsNullOrEmpty(m_UIView.OnShowPresetCategory))
                {
                    selectIndexOnShowPresetCategory = 0;
                }
                else
                {
                    selectIndexOnShowPresetCategory =
                        UIAnimations.Instance.Show.DatabaseNames.FindIndex(t => t == m_UIView.OnShowPresetCategory);
                }
            }
            catch
            {
                selectIndexOnShowPresetCategory = 0;
            }

            try
            {
                if (string.IsNullOrEmpty(m_UIView.OnShowPresetName))
                {
                    selectIndexOnShowPresetName = 0;
                }
                else
                {
                    selectIndexOnShowPresetName = UIAnimations.Instance.Show.Databases[selectIndexOnShowPresetCategory]
                        .AnimationNames.FindIndex(t => t == m_UIView.OnShowPresetName);
                }
            }
            catch
            {
                selectIndexOnShowPresetName = 0;
            }

            try
            {
                if (string.IsNullOrEmpty(m_UIView.OnHidePresetCategory))
                {
                    selectIndexOnHidePresetCategory = 0;
                }
                else
                {
                    selectIndexOnHidePresetCategory =
                        UIAnimations.Instance.Hide.DatabaseNames.FindIndex(t => t == m_UIView.OnHidePresetCategory);
                }
            }
            catch
            {
                selectIndexOnHidePresetCategory = 0;
            }

            try
            {
                if (string.IsNullOrEmpty(m_UIView.OnHidePresetName))
                {
                    selectIndexOnHidePresetName = 0;
                }
                else
                {
                    selectIndexOnHidePresetName = UIAnimations.Instance.Hide.Databases[selectIndexOnHidePresetCategory]
                        .AnimationNames.FindIndex(t => t == m_UIView.OnHidePresetName);
                }
            }
            catch
            {
                selectIndexOnHidePresetName = 0;
            }

            try
            {
                if (selectIndexOnShowPresetCategory == 0 && selectIndexOnShowPresetName == 0)
                {
                    LoadOnShowPreset();
                }
                else
                {
                    isMoveEnabledOnShow = m_UIView.MoveOnShow.Enabled;
                    isRotateEnabledOnShow = m_UIView.RotateOnShow.Enabled;
                    isScaleEnabledOnShow = m_UIView.ScaleOnShow.Enabled;
                    isFadeEnabledOnShow = m_UIView.FadeOnShow.Enabled;
                }
            }
            catch (Exception e)
            {
                selectIndexOnShowPresetCategory = 0;
                selectIndexOnShowPresetName = 0;
                LoadOnShowPreset();
                Debug.LogError(e);
            }
            try
            {
                if (selectIndexOnHidePresetCategory == 0 && selectIndexOnHidePresetName == 0)
                {
                    LoadOnHidePreset();
                }
                else
                {
                    isMoveEnabledOnHide = m_UIView.MoveOnHide.Enabled;
                    isRotateEnabledOnHide = m_UIView.RotateOnHide.Enabled;
                    isScaleEnabledOnHide = m_UIView.ScaleOnHide.Enabled;
                    isFadeEnabledOnHide = m_UIView.FadeOnHide.Enabled;
                }
            }
            catch (Exception e)
            {
                selectIndexOnHidePresetCategory = 0;
                selectIndexOnHidePresetName = 0;
                LoadOnHidePreset();
                Debug.LogError(e);
            }
        }

        public override void OnInspectorGUI()
        {
            GUILayout.Space(15);
            if (GUILayout.Button("生成代码"))
            {
                string content = TEngine.Editor.UI.ScriptGenerator.Generate(false);
                string windowName = m_UIView.gameObject.name;
                string path = Application.dataPath + "/GameScripts/HotFix/GameLogic/UI/" + windowName + "/" + windowName + ".Generate.cs";
                WriteStringByFile(path, content);
                AssetDatabase.Refresh();
            }
            GUILayout.Space(15);
            if (GUILayout.Button("默认动画"))
            {
                selectIndexOnShowPresetCategory = 2;
                selectIndexOnShowPresetName = 3;
                LoadOnShowPreset();
                EditorUtility.SetDirty(m_UIView);
                AssetDatabase.SaveAssets();

                selectIndexOnHidePresetCategory = 2;
                selectIndexOnHidePresetName = 4;
                LoadOnHidePreset();
                EditorUtility.SetDirty(m_UIView);
                AssetDatabase.SaveAssets();
            }
            GUILayout.Space(15);
            GUILayout.Label("初始状态");
            EditorGUILayout.PropertyField(m_StartBehavior);

            GUILayout.Space(15);
            _needShowAnim = EditorGUILayout.Toggle("是否需要打开动画", _needShowAnim);
            GUILayout.Space(15);
            _needHideAnim = EditorGUILayout.Toggle("是否需要关闭动画", _needHideAnim);
            if (m_UIView.NeedShowAnim != _needShowAnim || m_UIView.NeedHideAnim != _needHideAnim)
            {
                m_UIView.NeedShowAnim = _needShowAnim;
                m_UIView.NeedHideAnim = _needHideAnim;
                EditorUtility.SetDirty(m_UIView);
                AssetDatabase.SaveAssets();
                return;
            }

            if (_needShowAnim)
            {
                GUILayout.Space(15);
                GUILayout.Label("打开动画");
                if (GUILayoutHelper.DrawHeader("OnShow", "UIView", true, false))
                {
                    try
                    {
                        DrawOnShow();
                    }
                    catch
                    {
                        selectIndexOnShowPresetCategory = 0;
                        selectIndexOnShowPresetName = 0;
                        LoadOnShowPreset();
                    }
                }
            }

            if (_needHideAnim)
            {
                GUILayout.Space(30);
                GUILayout.Label("关闭动画");
                if (GUILayoutHelper.DrawHeader("OnHide", ""))
                {
                    try
                    {
                        DrawOnHide();
                    }
                    catch
                    {
                        selectIndexOnHidePresetCategory = 0;
                        selectIndexOnHidePresetName = 0;
                        LoadOnHidePreset();
                    }
                }
            }

        }

        private void DrawOnShow()
        {

            GUILayout.BeginVertical();
            {
                int selectIndex = selectIndexOnShowPresetCategory;
                selectIndexOnShowPresetCategory = EditorGUILayout.Popup("Preset Category", selectIndex,
                    UIAnimations.Instance.Show.DatabaseNames.ToArray(),
                    GUILayout.ExpandWidth(true));
                if (selectIndex != selectIndexOnShowPresetCategory)
                {
                    m_UIView.OnShowPresetCategory =
                        UIAnimations.Instance.Show.DatabaseNames[selectIndexOnShowPresetCategory];
                }

                if (!isEditorNewOnShowAnimation)
                {
                    int selectIndex2 = selectIndexOnShowPresetName;
                    if (selectIndex2 >= UIAnimations.Instance.Show.Databases[selectIndexOnShowPresetCategory]
                            .AnimationNames.Count)
                    {
                        selectIndex2 = 0;
                    }

                    selectIndexOnShowPresetName = EditorGUILayout.Popup("Preset Name", selectIndex2,
                        UIAnimations.Instance.Show.Databases[selectIndexOnShowPresetCategory].AnimationNames.ToArray());
                }

                string selectOnShowPresetName = UIAnimations.Instance.Show
                    .Databases[selectIndexOnShowPresetCategory]
                    .AnimationNames[selectIndexOnShowPresetName];
                if (m_UIView.OnShowPresetName != null && m_UIView.OnShowPresetName != selectOnShowPresetName)
                {
                    if (!isEditorNewOnShowAnimation)
                    {
                        m_UIView.OnShowPresetName = selectOnShowPresetName;
                    }
                }

                if (isEditorNewOnShowAnimation)
                {
                    EditorGUILayout.PropertyField(m_OnShowPresetName, new GUIContent("Preset Name"));
                }

                GUILayout.BeginHorizontal();
                {
                    isMoveEnabledOnShow = GUILayout.Toggle(isMoveEnabledOnShow, "Move In");
                    isRotateEnabledOnShow = GUILayout.Toggle(isRotateEnabledOnShow, "Rotate In");
                    isScaleEnabledOnShow = GUILayout.Toggle(isScaleEnabledOnShow, "Scale In");
                    isFadeEnabledOnShow = GUILayout.Toggle(isFadeEnabledOnShow, "Fade In");
                }
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                {
                    if (!isEditorNewOnShowAnimation)
                    {
                        if (GUILayout.Button("Load Preset"))
                        {
                            LoadOnShowPreset();
                            EditorUtility.SetDirty(m_UIView);
                            AssetDatabase.SaveAssets();
                        }

                        // if (GUILayout.Button("Delete Preset"))
                        // {
                        //     EditorGUIUtility.PingObject(UIAnimations.Instance.Show
                        //         .Databases[selectIndexOnShowPresetCategory]);
                        //     UIAnimations.Instance.Show.Databases[selectIndexOnShowPresetCategory].Delete(m_UIView.OnShowPresetName,true);
                        //     selectIndexOnShowPresetName = 0;
                        //     if (UIAnimations.Instance.Show.Databases[selectIndexOnShowPresetCategory].Database.Count ==
                        //         0)
                        //     {
                        //         selectIndexOnShowPresetCategory = 0;
                        //     }
                        // }
                        if (GUILayout.Button("Save New"))
                        {
                            isEditorNewOnShowAnimation = true;
                        }
                    }
                    else
                    {
                        if (GUILayout.Button("Save"))
                        {
                            // bool isSucess=UIAnimations.Instance.Show.Databases[selectIndexOnShowPresetCategory]
                            //     .CreatePreset(m_UIView.OnShowPresetName,new UIAnimation(AnimationType.Show, m_UIView.MoveOnShow, 
                            //         m_UIView.RotateOnShow, m_UIView.ScaleOnShow, m_UIView.FadeOnShow));
                            if (UIAnimations.Instance.Show.Get("Custom") == null)
                            {
                                UIAnimations.Instance.Show.AddTheCustomUIAnimationDatabase();
                            }
                            bool isSucess = UIAnimations.Instance.Show.Get("Custom")
                                .CreatePreset(m_UIView.OnShowPresetName, new UIAnimation(AnimationType.Show, m_UIView.MoveOnShow,
                                m_UIView.RotateOnShow, m_UIView.ScaleOnShow, m_UIView.FadeOnShow));
                            if (isSucess)
                            {
                                selectIndexOnShowPresetCategory = UIAnimations.Instance.Show.GetIndex("Custom");
                                m_UIView.OnShowPresetCategory =
                                    UIAnimations.Instance.Show.DatabaseNames[selectIndexOnShowPresetCategory];
                                selectIndexOnShowPresetName = UIAnimations.Instance.Show.Databases[selectIndexOnShowPresetCategory].GetIndex(m_UIView.OnShowPresetName);
                            }
                            else
                            {
                                Debug.LogError("命名重复：" + m_UIView.OnShowPresetName);
                            }
                            AssetDatabase.Refresh();
                            EditorUtility.SetDirty(m_UIView);
                            isEditorNewOnShowAnimation = false;
                        }

                        if (GUILayout.Button("Cannel"))
                        {
                            isEditorNewOnShowAnimation = false;
                            m_UIView.OnShowPresetName = selectOnShowPresetName;
                        }
                    }

                }
                GUILayout.EndHorizontal();
                if (GUILayout.Button("预览"))
                {
                    UIAnimatorUtils.PreviewViewAnimation(m_UIView, new UIAnimation(AnimationType.Show,
                        m_UIView.MoveOnShow, m_UIView.RotateOnShow, m_UIView.ScaleOnShow, m_UIView.FadeOnShow));
                }
                OnShowAnimationData();
                // 保存序列化数据，否则会出现设置数据丢失情况
                serializedObject.ApplyModifiedProperties();
            }
            GUILayout.EndVertical();
        }

        private void DrawOnHide()
        {

            GUILayout.BeginVertical();
            {
                int selectIndex = selectIndexOnHidePresetCategory;
                selectIndexOnHidePresetCategory = EditorGUILayout.Popup("Preset Category", selectIndex,
                    UIAnimations.Instance.Hide.DatabaseNames.ToArray(),
                    GUILayout.ExpandWidth(true));
                if (selectIndex != selectIndexOnHidePresetCategory)
                {
                    m_UIView.OnHidePresetCategory =
                        UIAnimations.Instance.Hide.DatabaseNames[selectIndexOnHidePresetCategory];
                }

                if (!isEditorNewOnHideAnimation)
                {
                    int selectIndex2 = selectIndexOnHidePresetName;
                    if (selectIndex2 >= UIAnimations.Instance.Hide.Databases[selectIndexOnHidePresetCategory]
                            .AnimationNames.Count)
                    {
                        selectIndex2 = 0;
                    }

                    selectIndexOnHidePresetName = EditorGUILayout.Popup("Preset Name", selectIndex2,
                        UIAnimations.Instance.Hide.Databases[selectIndexOnHidePresetCategory].AnimationNames.ToArray());
                }

                string selectOnHidePresetName = UIAnimations.Instance.Hide
                    .Databases[selectIndexOnHidePresetCategory]
                    .AnimationNames[selectIndexOnHidePresetName];
                if (m_UIView.OnHidePresetName != null && m_UIView.OnHidePresetName != selectOnHidePresetName)
                {
                    if (!isEditorNewOnHideAnimation)
                    {
                        m_UIView.OnHidePresetName = selectOnHidePresetName;
                    }
                }

                if (isEditorNewOnHideAnimation)
                {
                    EditorGUILayout.PropertyField(m_OnHidePresetName, new GUIContent("Preset Name"));
                }

                GUILayout.BeginHorizontal();
                {
                    isMoveEnabledOnHide = GUILayout.Toggle(isMoveEnabledOnHide, "Move Out");
                    isRotateEnabledOnHide = GUILayout.Toggle(isRotateEnabledOnHide, "Rotate Out");
                    isScaleEnabledOnHide = GUILayout.Toggle(isScaleEnabledOnHide, "Scale Out");
                    isFadeEnabledOnHide = GUILayout.Toggle(isFadeEnabledOnHide, "Fade Out");
                }
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                {
                    if (!isEditorNewOnHideAnimation)
                    {
                        if (GUILayout.Button("Load Preset"))
                        {
                            LoadOnHidePreset();
                            EditorUtility.SetDirty(m_UIView);
                        }

                        // if (GUILayout.Button("Delete Preset"))
                        // {
                        //     EditorGUIUtility.PingObject(UIAnimations.Instance.Hide
                        //         .Databases[selectIndexOnHidePresetCategory]);
                        //     UIAnimations.Instance.Hide.Databases[selectIndexOnHidePresetCategory].Delete(m_UIView.OnHidePresetName,true);
                        //     selectIndexOnHidePresetName = 0;
                        //     if (UIAnimations.Instance.Hide.Databases[selectIndexOnHidePresetCategory].Database.Count ==
                        //         0)
                        //     {
                        //         selectIndexOnHidePresetCategory = 0;
                        //     }
                        // }
                        if (GUILayout.Button("Save New"))
                        {
                            isEditorNewOnHideAnimation = true;
                        }
                    }
                    else
                    {
                        if (GUILayout.Button("Save"))
                        {
                            // bool isSucess=UIAnimations.Instance.Hide.Databases[selectIndexOnHidePresetCategory]
                            //     .CreatePreset(m_UIView.OnHidePresetName,new UIAnimation(AnimationType.Hide, m_UIView.MoveOnHide, 
                            //         m_UIView.RotateOnHide, m_UIView.ScaleOnHide, m_UIView.FadeOnHide));
                            if (UIAnimations.Instance.Hide.Get("Custom") == null)
                            {
                                UIAnimations.Instance.Hide.AddTheCustomUIAnimationDatabase();
                            }
                            bool isSucess = UIAnimations.Instance.Hide.Get("Custom")
                                .CreatePreset(m_UIView.OnHidePresetName, new UIAnimation(AnimationType.Hide, m_UIView.MoveOnHide,
                                m_UIView.RotateOnHide, m_UIView.ScaleOnHide, m_UIView.FadeOnHide));
                            if (isSucess)
                            {
                                selectIndexOnHidePresetCategory = UIAnimations.Instance.Hide.GetIndex("Custom");
                                m_UIView.OnHidePresetCategory =
                                    UIAnimations.Instance.Hide.DatabaseNames[selectIndexOnHidePresetCategory];
                                selectIndexOnHidePresetName = UIAnimations.Instance.Hide.Databases[selectIndexOnHidePresetCategory].GetIndex(m_UIView.OnHidePresetName);
                            }
                            else
                            {
                                Debug.LogError("命名重复：" + m_UIView.OnHidePresetName);
                            }
                            AssetDatabase.Refresh();
                            EditorUtility.SetDirty(m_UIView);
                            isEditorNewOnHideAnimation = false;
                        }

                        if (GUILayout.Button("Cannel"))
                        {
                            isEditorNewOnHideAnimation = false;
                            m_UIView.OnHidePresetName = selectOnHidePresetName;
                        }
                    }

                }
                GUILayout.EndHorizontal();
                if (GUILayout.Button("预览"))
                {
                    UIAnimatorUtils.PreviewViewAnimation(m_UIView, new UIAnimation(AnimationType.Hide,
                        m_UIView.MoveOnHide, m_UIView.RotateOnHide, m_UIView.ScaleOnHide, m_UIView.FadeOnHide));
                }
                OnHideAnimationData();
                // 保存序列化数据，否则会出现设置数据丢失情况
                serializedObject.ApplyModifiedProperties();
            }
            GUILayout.EndVertical();
        }
        private void LoadOnShowPreset()
        {
            databaseOnShow = UIAnimations.Instance.Show.Databases[selectIndexOnShowPresetCategory]
                .Database[selectIndexOnShowPresetName];
            isMoveEnabledOnShow = databaseOnShow.Animation.Move.Enabled;
            isRotateEnabledOnShow = databaseOnShow.Animation.Rotate.Enabled;
            isScaleEnabledOnShow = databaseOnShow.Animation.Scale.Enabled;
            isFadeEnabledOnShow = databaseOnShow.Animation.Fade.Enabled;
            m_UIView.MoveOnShow = databaseOnShow.Animation.Move.Copy();
            m_UIView.RotateOnShow = databaseOnShow.Animation.Rotate.Copy();
            m_UIView.ScaleOnShow = databaseOnShow.Animation.Scale.Copy();
            m_UIView.FadeOnShow = databaseOnShow.Animation.Fade.Copy();
            m_UIView.OnShowPresetCategory = UIAnimations.Instance.Show.DatabaseNames[selectIndexOnShowPresetCategory];
            m_UIView.OnShowPresetName = UIAnimations.Instance.Show.Databases[selectIndexOnShowPresetCategory].AnimationNames[selectIndexOnShowPresetName];
        }

        private void LoadOnHidePreset()
        {
            databaseOnHide = UIAnimations.Instance.Hide.Databases[selectIndexOnHidePresetCategory]
                .Database[selectIndexOnHidePresetName];
            isMoveEnabledOnHide = databaseOnHide.Animation.Move.Enabled;
            isRotateEnabledOnHide = databaseOnHide.Animation.Rotate.Enabled;
            isScaleEnabledOnHide = databaseOnHide.Animation.Scale.Enabled;
            isFadeEnabledOnHide = databaseOnHide.Animation.Fade.Enabled;
            m_UIView.MoveOnHide = databaseOnHide.Animation.Move.Copy();
            m_UIView.RotateOnHide = databaseOnHide.Animation.Rotate.Copy();
            m_UIView.ScaleOnHide = databaseOnHide.Animation.Scale.Copy();
            m_UIView.FadeOnHide = databaseOnHide.Animation.Fade.Copy();
            m_UIView.OnHidePresetCategory = UIAnimations.Instance.Hide.DatabaseNames[selectIndexOnHidePresetCategory];
            m_UIView.OnHidePresetName = UIAnimations.Instance.Hide.Databases[selectIndexOnHidePresetCategory].AnimationNames[selectIndexOnHidePresetName];

        }
        private void OnShowAnimationData()
        {
            GUILayout.BeginVertical();
            {
                if (isMoveEnabledOnShow)
                {
                    GUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.PropertyField(m_MoveOnShow);
                    }
                    GUILayout.EndHorizontal();
                }
                if (isRotateEnabledOnShow)
                {
                    GUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.PropertyField(m_RotateOnShow);
                    }
                    GUILayout.EndHorizontal();
                }
                if (isScaleEnabledOnShow)
                {
                    GUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.PropertyField(m_ScaleOnShow);
                    }
                    GUILayout.EndHorizontal();
                }
                if (isFadeEnabledOnShow)
                {
                    GUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.PropertyField(m_FadeOnShow);
                    }
                    GUILayout.EndHorizontal();
                }
            }
            GUILayout.EndVertical();
        }

        private void OnHideAnimationData()
        {
            GUILayout.BeginVertical();
            {
                if (isMoveEnabledOnHide)
                {
                    GUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.PropertyField(m_MoveOnHide);
                    }
                    GUILayout.EndHorizontal();
                }
                if (isRotateEnabledOnHide)
                {
                    GUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.PropertyField(m_RotateOnHide);
                    }
                    GUILayout.EndHorizontal();
                }
                if (isScaleEnabledOnHide)
                {
                    GUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.PropertyField(m_ScaleOnHide);
                    }
                    GUILayout.EndHorizontal();
                }
                if (isFadeEnabledOnHide)
                {
                    GUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.PropertyField(m_FadeOnHide);
                    }
                    GUILayout.EndHorizontal();
                }
            }
            GUILayout.EndVertical();
        }

        #region read write string file
        public static void WriteStringByFile(string path, string content)
        {
            var dataByte = Encoding.GetEncoding("UTF-8").GetBytes(content);
            CreateFile(path, dataByte);
        }
        public static string ReadStringByFile(string path)
        {
            var line = new StringBuilder();
            try
            {
                if (!File.Exists(path))
                {
                    Debug.Log("path dont exists ! : " + path);
                    return "";
                }
                var sr = File.OpenText(path);
                line.Append(sr.ReadToEnd());
                sr.Close();
                sr.Dispose();
            }
            catch (Exception e)
            {
                Debug.Log("Load text fail ! message:" + e.Message);
            }

            return line.ToString();
        }
        public static void DeleteFile(string path)
        {
            if (File.Exists(path))
                File.Delete(path);
            else
                Debug.Log("File:[" + path + "] dont exists");
        }
        public static void CreateFile(string path, byte[] byt)
        {
            try
            {
                CreatFilePath(path);
                File.WriteAllBytes(path, byt);
            }
            catch (Exception e)
            {
                Debug.LogError("File Create Fail! \n" + e.Message);
            }
        }
        public static void CreatFilePath(string filepath)
        {
            var newPathDir = Path.GetDirectoryName(filepath);
            CreatPath(newPathDir);
        }
        public static void CreatPath(string path)
        {
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        }
        #endregion
    }
}