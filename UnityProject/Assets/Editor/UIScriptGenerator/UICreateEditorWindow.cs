//
//  UICreateEditorWindow.cs 
//
//  Author: HGT 
//
//  Copyright (c) 2021 hegametech.com 
//

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Doozy.Engine.UI;
using GameLogic;
using TEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.UI;

namespace TEngine.Editor.UI
{
    public class UICreateEditorWindow : EditorWindow
    {
        public const string S_UIPrefabFolderPath = "AssetRaw/UI/";
        public const string S_UIAssemblyName = "GameLogic";
        public const string S_AppNamespace = "GameLogic";
        public const string S_UiWindowClassPath = "GameScripts/HotFix/GameLogic/UI/";


        [MenuItem("Tools/Create UI \t\t\t 创建UI", false, 20)]
        public static void ShowWindow()
        {
            GetWindow(typeof(UICreateEditorWindow));
        }

        private void OnGUI()
        {
            titleContent.text = "UI编辑器";
            EditorGUILayout.BeginVertical();

            CreateUIGUI();

            EditorGUILayout.Space(10);

            EditorGUILayout.EndVertical();
        }

        #region createUI

        private string m_UIname = "";
        private string m_description = "";
        private UILayer m_UIType = UILayer.UI;

        private void CreateUIGUI()
        {
            if (DrawHeader("创建 UI", "CreateUI", true, false))
            {
                EditorGUI.indentLevel = 1;
                EditorGUILayout.LabelField("提示： 脚本和 UI 名称会自动添加Form后缀");
                m_UIname = EditorGUILayout.TextField("UI Name:", m_UIname);
                m_UIType = (UILayer)EditorGUILayout.EnumPopup("UI Type:", m_UIType);

                if (m_UIname != "")
                {
                    var l_nameTmp = m_UIname + "Form";
                    var assem = S_AppNamespace + "." + l_nameTmp + "," +
                                S_UIAssemblyName +
                                ", Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"; // typeof(HGT.AppComponent.AppBoot).Assembly.GetName();
                    var l_typeTmp = Type.GetType(assem, false);

                    if (l_typeTmp != null)
                    {
                        if (l_typeTmp.BaseType.Equals(typeof(UIWindow)) || l_typeTmp.BaseType.IsSubclassOf(typeof(UIWindow)))
                        {
                            if (GUILayout.Button("已存在脚本，点击创建Prefab"))
                            {
                                CreateUIPrefab(l_nameTmp, m_UIType);
                                m_UIname = "";
                            }
                        }
                        else
                        {
                            EditorGUILayout.LabelField("该类没有继承UIWindow");
                        }
                    }
                    else
                    {
                        m_description = EditorGUILayout.TextField("UI 描述:", m_description);

                        if (GUILayout.Button("创建 UI 脚本和 Prefab"))
                        {
                            EditorPrefs.SetBool("Create_UI", true);
                            EditorPrefs.SetString("Create_UI_Name", m_UIname);
                            EditorPrefs.SetInt("Create_UI_TypePrefab", (int)m_UIType);
                            CreatUIScript(l_nameTmp, m_description, m_UIType);
                            m_UIname = "";
                        }
                    }
                }
            }
        }

        [DidReloadScripts]
        private static void OnUiScriptCreated()
        {
            #region 创建 Game UI
            if (EditorPrefs.GetBool("Create_UI", false))
            {
                EditorPrefs.SetBool("Create_UI", false);
                var m_UIname = EditorPrefs.GetString("Create_UI_Name");
                var l_nameTmp = m_UIname + "Form";
                var assem = S_AppNamespace + "." + l_nameTmp + "," +
                            S_UIAssemblyName +
                            ", Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"; // typeof(HGT.AppComponent.AppBoot).Assembly.GetName();
                var l_typeTmp = Type.GetType(assem, false);
                if (l_typeTmp != null)
                {
                    if (l_typeTmp.BaseType == typeof(UIWindow) || l_typeTmp.BaseType.IsSubclassOf(typeof(UIWindow)))
                    {
                        var m_UIType = (UILayer)EditorPrefs.GetInt("Create_UI_TypePrefab", 2);
                        CreateUIPrefab(l_nameTmp, m_UIType);
                    }
                    else
                    {
                        Debug.LogError("l_typeTmp is not UIWindow");
                    }
                }
            }
            #endregion
        }
        #endregion

        #region 创建ui预制体
        /// <summary>
        ///     创建ui预制体
        /// </summary>
        public static void CreateUIPrefab(string UIWindowName, UILayer UIType)
        {
            var uiGo = new GameObject(UIWindowName);
            var uicanvas = GameObject.Find("UIRoot/UICanvas");
            if (uicanvas)
            {
                uiGo.transform.SetParent(uicanvas.transform);
                uiGo.transform.localPosition = Vector3.zero;
                uiGo.transform.localScale = Vector3.one;
            }
            uiGo.layer = LayerMask.NameToLayer("UI");
            uiGo.AddComponent<GraphicRaycaster>();
            uiGo.AddComponent<CanvasGroup>();
            uiGo.AddComponent<UIAnimView>();
            var ui = uiGo.GetComponent<RectTransform>();
            ui.sizeDelta = Vector2.zero;
            ui.anchorMin = Vector2.zero;
            ui.anchorMax = Vector2.one;
            var BgGo = new GameObject("bg");
            BgGo.layer = LayerMask.NameToLayer("UI");
            var Bg = BgGo.AddComponent<RectTransform>();
            Bg.SetParent(ui);
            Bg.sizeDelta = Vector2.zero;
            Bg.anchorMin = Vector2.zero;
            Bg.anchorMax = Vector2.one;
            var rootGo = new GameObject("root");
            rootGo.layer = LayerMask.NameToLayer("UI");
            var root = rootGo.AddComponent<RectTransform>();
            root.SetParent(ui);
            root.sizeDelta = Vector2.zero;
            root.anchorMin = Vector2.zero;
            root.anchorMax = Vector2.one;
            var Path = S_UIPrefabFolderPath + UIWindowName + ".prefab";
            CreatFilePath(Application.dataPath + "/" + Path);
            PrefabUtility.SaveAsPrefabAssetAndConnect(uiGo, "Assets/" + Path, InteractionMode.UserAction);
            {
                // 选中
                Selection.activeObject = uiGo;
                EditorGUIUtility.PingObject(uiGo);
                GUIUtility.keyboardControl = 0;
                GUIUtility.hotControl = 0;
            }
            ProjectWindowUtil.ShowCreatedAsset(uiGo);
        }
        #endregion

        #region 创建UI脚本
        /// <summary>
        ///     按照模板创建UI窗口脚本
        /// </summary>
        /// <param name="UIWindowName"></param>
        public static void CreatUIScript(string UIWindowName, string description, UILayer UIType)
        {
            var SavePath = Application.dataPath + "/" + S_UiWindowClassPath + "/" + UIWindowName +
                           "/" + UIWindowName + ".cs";
            StringBuilder strFile = new StringBuilder();

#if ENABLE_TEXTMESHPRO
            strFile.Append("using TMPro;\n");
#endif
            strFile.Append("using Cysharp.Threading.Tasks;\n");
            strFile.Append("using UnityEngine;\n");
            strFile.Append("using UnityEngine.UI;\n");
            strFile.Append("using TEngine;\n\n");
            strFile.Append($"namespace {ScriptGeneratorSetting.GetUINameSpace()}\n");
            strFile.Append("{\n");
            strFile.Append("\t/// </summary>\n");
            strFile.Append($"\t/// {description}\n");
            strFile.Append("\t/// </summary>\n");
            strFile.Append($"\t[Window(UILayer.{UIType.ToString()})]\n");
            strFile.Append("\tpublic partial class " + UIWindowName + " : UIAnimWindow\n");
            strFile.Append("\t{\n");

            strFile.Append("\t\t/// </summary>\n");
            strFile.Append("\t\t/// 绑定UI成员元素\n");
            strFile.Append("\t\t/// </summary>\n");
            strFile.Append("\t\tprotected override void BindMemberProperty()\n");
            strFile.Append("\t\t{\n");
            strFile.Append("\t\t}\n\n");

            strFile.Append("\t\t/// </summary>\n");
            strFile.Append("\t\t/// 注册事件\n");
            strFile.Append("\t\t/// </summary>\n");
            strFile.Append("\t\tprotected override void RegisterEvent()\n");
            strFile.Append("\t\t{\n");
            strFile.Append("\t\t}\n\n");

            strFile.Append("\t\t/// </summary>\n");
            strFile.Append("\t\t/// 窗口打开\n");
            strFile.Append("\t\t/// </summary>\n");
            strFile.Append("\t\tprotected override void OnCreate()\n");
            strFile.Append("\t\t{\n");
            strFile.Append("\t\t}\n\n");

            strFile.Append("\t\t/// </summary>\n");
            strFile.Append("\t\t/// 窗口关闭\n");
            strFile.Append("\t\t/// </summary>\n");
            strFile.Append("\t\tprotected override void OnDestroy()\n");
            strFile.Append("\t\t{\n");
            strFile.Append("\t\t}\n");
            strFile.Append("\t}\n");
            strFile.Append("}\n");
            WriteStringByFile(SavePath, strFile.ToString());
            AssetDatabase.Refresh();
        }
        #endregion

        #region draw
        public static void DrawUILine(Color color, int thickness = 2, int padding = 10)
        {
            var r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
            r.height = thickness;
            r.y += padding / 2;
            r.x -= 2;
            r.width += 6;
            EditorGUI.DrawRect(r, color);
        }

        /// <summary>
        ///     Draw a distinctly different looking header label
        /// </summary>
        public static bool DrawHeader(string text, string key, bool forceOn, bool minimalistic)
        {
            var state = EditorPrefs.GetBool(key, true);
            if (!minimalistic) GUILayout.Space(3f);
            if (!forceOn && !state) GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f);
            GUILayout.BeginHorizontal();
            GUI.changed = false;
            if (minimalistic)
            {
                if (state)
                    text = "\u25BC" + (char)0x200a + text;
                else
                    text = "\u25BA" + (char)0x200a + text;
                GUILayout.BeginHorizontal();
                GUI.contentColor = EditorGUIUtility.isProSkin
                    ? new Color(1f, 1f, 1f, 0.7f)
                    : new Color(0f, 0f, 0f, 0.7f);
                if (!GUILayout.Toggle(true, text, "PreToolbar2", GUILayout.MinWidth(20f))) state = !state;
                GUI.contentColor = Color.white;
                GUILayout.EndHorizontal();
            }
            else
            {
                text = "<b><size=11>" + text + "</size></b>";
                if (state)
                    text = "\u25BC " + text;
                else
                    text = "\u25BA " + text;
                if (!GUILayout.Toggle(true, text, "dragtab", GUILayout.MinWidth(20f))) state = !state;
            }
            if (GUI.changed) EditorPrefs.SetBool(key, state);
            if (!minimalistic) GUILayout.Space(2f);
            GUILayout.EndHorizontal();
            GUI.backgroundColor = Color.white;
            if (!forceOn && !state) GUILayout.Space(3f);
            return state;
        }
        #endregion

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