#define LOADTEXTUREFOLDER
using UnityEngine;
using UnityEditor;
using Designer.Runtime;
using System.IO;
using System.Reflection;
using System;
using System.Collections.Generic;

namespace Designer.Editor
{
    public class DesignerUtility
    {
        private readonly static string texture_folder_path = "Assets/NodeDesigner/Editor/Textures/";
        private static GUIStyle graphBackgroundGUIStyle = null;
        private static GUIStyle selectionGUIStyle = null;
        private static Texture2D nodeConnectTexture = null;
        private static Texture2D[] nodeTopTextures = new Texture2D[9];
        private static Texture2D[] nodeBottomTextures = new Texture2D[9];
        private static Texture2D[] nodeLeftTextures = new Texture2D[9];
        private static Texture2D[] nodeRightTextures = new Texture2D[9];
        private static Texture2D[] nodeConnectionTextures = new Texture2D[9];
        private static Texture2D[] nodeConnectionEndBottomTextures = new Texture2D[9];
        private static Texture2D[] nodeConnectionEndTopTextures = new Texture2D[9];
        private static GUIStyle[] nodeBackgroundGUIStyle = new GUIStyle[9];
        private static GUIStyle[] nodeSelectGUIStyle = new GUIStyle[9];
        private static GUIStyle nodeTitleGUIStyle = null;

        public static GUIStyle GraphBackgroundGUIStyle
        {
            get
            {
                if (DesignerUtility.graphBackgroundGUIStyle == null)
                {
                    DesignerUtility.InitGraphBackgroundGUIStyle();
                }
                return DesignerUtility.graphBackgroundGUIStyle;
            }
        }
        public static GUIStyle SelectionGUIStyle
        {
            get
            {
                if (selectionGUIStyle == null)
                {
                    InitSelectionGUIStyle();
                }

                return selectionGUIStyle;
            }

        }
        public static GUIStyle NodeTitleGUIStyle
        {
            get
            {
                if (nodeTitleGUIStyle == null)
                {
                    InitNodeTitleGUIStyle();
                }
                return nodeTitleGUIStyle;
            }
        }
        public static Texture2D NodeConnectTexture
        {
            get
            {
                if (nodeConnectTexture == null)
                {

                }
                return nodeConnectTexture;
            }
        }
        /// <summary>
        /// 背景Top区域图
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Texture2D GetTopTextures(NodeColor color)
        {
            if (DesignerUtility.nodeTopTextures[(int)color] == null)
            {
                InitNodeTopTextures( color);
            }
            return DesignerUtility.nodeTopTextures[(int)color];
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Texture2D GetBottomTextures(NodeColor color)
        {
            if (DesignerUtility.nodeBottomTextures[(int)color] == null)
            {
                InitNodeBottomTextures(color);
            }
            return DesignerUtility.nodeBottomTextures[(int)color];
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Texture2D GetLeftTextures(NodeColor color)
        {
            if (DesignerUtility.nodeLeftTextures[(int)color] == null)
            {
                InitNodeLeftTextures(color);
            }
            return DesignerUtility.nodeLeftTextures[(int)color];
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Texture2D GetRightTextures(NodeColor color)
        {
            if (DesignerUtility.nodeRightTextures[(int)color] == null)
            {
                InitNodeRightTextures(color);
            }
            return DesignerUtility.nodeRightTextures[(int)color];
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Texture2D GetNodeConnectionTextures(NodeColor color)
        {
            if (DesignerUtility.nodeConnectionTextures[(int)color] == null)
            {
                InitNodeConnectionTextures(color);
            }
            return DesignerUtility.nodeConnectionTextures[(int)color];
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Texture2D GetNodeConnectionEndBottomTextures(NodeColor color)
        {
            if (DesignerUtility.nodeConnectionEndBottomTextures[(int)color] == null)
            {
                InitNodeConnectionEndBottomTextures(color);
            }
            return DesignerUtility.nodeConnectionEndBottomTextures[(int)color];
        }



        public static Texture2D GetNodeConnectionEndTopTextures(NodeColor color)
        {
            if (DesignerUtility.nodeConnectionEndTopTextures[(int)color] == null)
            {
                InitNodeConnectionEndTopTextures(color);
            }
            return DesignerUtility.nodeConnectionEndTopTextures[(int)color];
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static GUIStyle GetBackgroundGUIStyle(NodeColor color)
        {
            if (DesignerUtility.nodeBackgroundGUIStyle[(int)color] == null)
            {
                InitNodeBackgroundGUIStyle(color);
            }
            return DesignerUtility.nodeBackgroundGUIStyle[(int)color];
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static GUIStyle GetNodeSelectGUIStyle(NodeColor color)
        {
            if (DesignerUtility.nodeSelectGUIStyle[(int)color] == null)
            {
                InitNodeSelectGUIStyle(color);
            }
            return DesignerUtility.nodeSelectGUIStyle[(int)color];
        }
        /// <summary>
        /// 初始化背景GUI
        /// </summary>
        private static void InitGraphBackgroundGUIStyle()
        {
            Texture2D texture2D = new Texture2D(1, 1, TextureFormat.RGBA32, false, true);
            if (EditorGUIUtility.isProSkin)
            {
                texture2D.SetPixel(1, 1, new Color(0.1647f, 0.1647f, 0.1647f));
            }
            else
            {
                texture2D.SetPixel(1, 1, new Color(0.3647f, 0.3647f, 0.3647f));
            }
            texture2D.hideFlags = HideFlags.HideAndDontSave;
            texture2D.Apply();
            DesignerUtility.graphBackgroundGUIStyle = new GUIStyle(GUI.skin.box);
            DesignerUtility.graphBackgroundGUIStyle.normal.background = texture2D;
            DesignerUtility.graphBackgroundGUIStyle.active.background = texture2D;
            DesignerUtility.graphBackgroundGUIStyle.hover.background = texture2D;
            DesignerUtility.graphBackgroundGUIStyle.focused.background = texture2D;
            DesignerUtility.graphBackgroundGUIStyle.normal.textColor = Color.white;
            DesignerUtility.graphBackgroundGUIStyle.active.textColor = Color.white;
            DesignerUtility.graphBackgroundGUIStyle.hover.textColor = Color.white;
            DesignerUtility.graphBackgroundGUIStyle.focused.textColor = Color.white;
        }
        private static void InitSelectionGUIStyle()
        {
            Texture2D texture2D = new Texture2D(1, 1, TextureFormat.RGBA32, false, true);
            Color color = (!EditorGUIUtility.isProSkin) ? new Color(0.243f, 0.5686f, 0.839f, 0.5f) : new Color(0.188f, 0.4588f, 0.6862f, 0.5f);
            texture2D.SetPixel(1, 1, color);
            texture2D.hideFlags = HideFlags.HideAndDontSave;
            texture2D.Apply();
            DesignerUtility.selectionGUIStyle = new GUIStyle(GUI.skin.box);
            DesignerUtility.selectionGUIStyle.normal.background = texture2D;
            DesignerUtility.selectionGUIStyle.active.background = texture2D;
            DesignerUtility.selectionGUIStyle.hover.background = texture2D;
            DesignerUtility.selectionGUIStyle.focused.background = texture2D;
            DesignerUtility.selectionGUIStyle.normal.textColor = Color.white;
            DesignerUtility.selectionGUIStyle.active.textColor = Color.white;
            DesignerUtility.selectionGUIStyle.hover.textColor = Color.white;
            DesignerUtility.selectionGUIStyle.focused.textColor = Color.white;
        }
        private static void InitNodeTitleGUIStyle()
        {
            DesignerUtility.nodeTitleGUIStyle = new GUIStyle(GUI.skin.label);
            DesignerUtility.nodeTitleGUIStyle.alignment = TextAnchor.UpperCenter;
            DesignerUtility.nodeTitleGUIStyle.fontSize = 12;
            DesignerUtility.nodeTitleGUIStyle.fontStyle = FontStyle.Normal;
        }
        private static GUIStyle InitNodeGUIStyle(Texture2D texture, RectOffset overflow)
        {
            return new GUIStyle(GUI.skin.box)
            {
                border = new RectOffset(10, 10, 10, 10),
                overflow = overflow,
                normal =
                {
                    background = texture,
                    textColor = Color.white
                },
                active =
                {
                    background = texture,
                    textColor = Color.white
                },
                hover =
                {
                    background = texture,
                    textColor = Color.white
                },
                focused =
                {
                    background = texture,
                    textColor = Color.white
                },

                stretchHeight = true,
                stretchWidth = true
            };
        }


        private static void InitNodeTopTextures(NodeColor color)
        {
            DesignerUtility.nodeTopTextures[(int)color] = DesignerUtility.LoadTexture("TaskConnectionTop" + ((color == NodeColor.Def) ? "" : color.ToString()) + ".png", true, null);
        }
        private static void InitNodeBottomTextures(NodeColor color)
        {
            DesignerUtility.nodeBottomTextures[(int)color] = DesignerUtility.LoadTexture("TaskConnectionBottom" + ((color == NodeColor.Def) ? "" : color.ToString()) + ".png", true, null);
        }
        private static void InitNodeLeftTextures(NodeColor color)
        {
            DesignerUtility.nodeLeftTextures[(int)color] = DesignerUtility.LoadTexture("TaskConnectionLeft" + ((color == NodeColor.Def) ? "" : color.ToString()) + ".png", true, null);
        }
        private static void InitNodeRightTextures(NodeColor color)
        {
            DesignerUtility.nodeRightTextures[(int)color] = DesignerUtility.LoadTexture("TaskConnectionRight" + ((color == NodeColor.Def) ? "" : color.ToString()) + ".png", true, null);
        }
        private static void InitNodeConnectionTextures(NodeColor color)
        {
            DesignerUtility.nodeConnectionTextures[(int)color] = DesignerUtility.LoadTexture("ConnectionStartNode.png", true, null);
            //DesignerUtility.nodeConnectionTextures[(int)color] = DesignerUtility.LoadTexture("ColorSelector" + ((color == NodeColor.Def) ? "" : color.ToString()) + ".png", true, null);
            //DesignerUtility.nodeConnectionTextures[(int)color] = DesignerUtility.LoadTexture("ConnectionNode" + ((color == NodeColor.Def) ? "" : color.ToString()) + ".png", true, null);
        }
        private static void InitNodeConnectionEndBottomTextures(NodeColor color)
        {

            DesignerUtility.nodeConnectionEndBottomTextures[(int)color] = DesignerUtility.LoadTexture("ConnectionEndNodeBottom" + ((color == NodeColor.Def) ? "" : color.ToString()) + ".png", true, null);
            //DesignerUtility.nodeConnectionTextures[(int)color] = DesignerUtility.LoadTexture("ConnectionNode" + ((color == NodeColor.Def) ? "" : color.ToString()) + ".png", true, null);
        }
        private static void InitNodeConnectionEndTopTextures(NodeColor color)
        {
            DesignerUtility.nodeConnectionEndTopTextures[(int)color] = DesignerUtility.LoadTexture("ConnectionEndNodeTop.png", true, null);

            //DesignerUtility.nodeConnectionEndTopTextures[(int)color] = DesignerUtility.LoadTexture("ConnectionEndNodeTop" + ((color == NodeColor.Def) ? "" : color.ToString()) + ".png", true, null);
        }
        private static void InitNodeBackgroundGUIStyle(NodeColor color)
        {
            nodeBackgroundGUIStyle[(int)color] = InitNodeGUIStyle(LoadTexture("Task" + ((color == NodeColor.Def) ? "" : color.ToString()) + ".png", true, null), new RectOffset(5, 3, 3, 5));
        }
        private static void InitNodeSelectGUIStyle(NodeColor color)
        {
            nodeSelectGUIStyle[(int)color] = InitNodeGUIStyle(LoadTexture("TaskSelected" + ((color == NodeColor.Def) ? "" : color.ToString()) + ".png", true, null), new RectOffset(5, 3, 3, 5));
        }
        private static Texture2D LoadTexture(string imageName, bool useSkinColor = true, ScriptableObject obj = null)
        {
            Texture2D texture2D = null;
            string name = string.Format("{0}{1}", (!useSkinColor) ? string.Empty : ((!EditorGUIUtility.isProSkin) ? "Light" : "Dark"), imageName);
            Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(name);
            if (manifestResourceStream == null)
            {
                name = string.Format("DesignerEditor.Resources.{0}{1}", (!useSkinColor) ? string.Empty : ((!EditorGUIUtility.isProSkin) ? "Light" : "Dark"), imageName);
                manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(name);
            }
            if (manifestResourceStream != null)
            {
                texture2D = new Texture2D(0, 0, TextureFormat.RGBA32, false, true);
                texture2D.LoadImage(DesignerUtility.ReadToEnd(manifestResourceStream));
                manifestResourceStream.Close();
            }
#if LOADTEXTUREFOLDER
            name = string.Format("{0}{1}", (!useSkinColor) ? string.Empty : ((!EditorGUIUtility.isProSkin) ? "Light" : "Dark"), imageName);
            texture2D = AssetDatabase.LoadAssetAtPath(texture_folder_path + name,typeof(Texture2D)) as Texture2D;
#endif
            if (texture2D == null)
            {
                Debug.Log(string.Format("{0}/Images/Task Backgrounds/{1}{2}", DesignerUtility.GetEditorBaseDirectory(obj), (!useSkinColor) ? string.Empty : ((!EditorGUIUtility.isProSkin) ? "Light" : "Dark"), imageName));
            }
            texture2D.hideFlags = HideFlags.HideAndDontSave;

            return texture2D;
        }
        public static string GetEditorBaseDirectory(UnityEngine.Object obj = null)
        {
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            string text = Uri.UnescapeDataString(new UriBuilder(codeBase).Path);
            return Path.GetDirectoryName(text.Substring(Application.dataPath.Length - 6));
        }
        private static byte[] ReadToEnd(Stream stream)
        {
            byte[] array = new byte[16384];
            byte[] result;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                int count;
                while ((count = stream.Read(array, 0, array.Length)) > 0)
                {
                    memoryStream.Write(array, 0, count);
                }
                result = memoryStream.ToArray();
            }
            return result;
        }
        public static List<Type> GetBaseClasses(Type t)
        {
            List<Type> list = new List<Type>();
            while (t != null && !t.Equals(typeof(System.Object)))
            {
                list.Add(t);
                t = t.BaseType;
            }
            return list;
        }
        public static string LoadName(Type type)
        {
            string txt = type.ToString();
            txt = txt.Substring(txt.LastIndexOf('.') + 1);
            return txt;
        }
    }
}