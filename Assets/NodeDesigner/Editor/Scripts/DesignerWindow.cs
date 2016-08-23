using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using Designer.Runtime;
using System.Reflection;
using System.Threading;

namespace Designer.Editor
{
    public enum MouseOperation
    {
        None,
        SelectionNode,
        MoveNode,
        MoveConnection,
        AddConnection,
    }
    public class DesignerWindow : EditorWindow
    {
        protected string path = "Assets/NodeDesigner/Editor/Prefabs/Designer.prefab";
        protected string prefab_name = "";
        #region Inspector 功能
        protected string[] tooltipsNames = new string[] { "main", "Inspector" };
        protected int tooltipsselection = 0;
        #endregion
        #region 背景框区域功能
        /// <summary>
        /// 线条材质球
        /// </summary>
        protected Material mGridMaterial;
        /// <summary>
        /// 当前鼠标位置
        /// </summary>
        protected Vector2 mCurrentMousePosition = Vector2.zero;
        /// <summary>
        /// 网格界面滚动位置
        /// </summary>
        protected Vector2 mGraphScrollPosition = new Vector2(-1f, -1f);
        /// <summary>
        /// 格子区域比例
        /// </summary>
        protected Vector2 mGraphOffset = Vector2.zero;
        /// <summary>
        /// 格子区域缩放比例
        /// </summary>
        protected float mGraphZoom = 1f;
        /// <summary>
        /// 格子棋盘区域
        /// </summary>
        protected Rect mGraphRect;
        /// <summary>
        /// 网格区域大小
        /// </summary>
        protected Vector2 mGraphScrollSize = new Vector2(20000, 20000);
        protected Rect mDragMouse = new Rect(0, 0, 0, 0);
        protected MouseOperation mMouseStatus = MouseOperation.None;

        protected List<NodeData> selectionNodes = new List<NodeData>();
        protected NodeConnection selectionNodeConnection = null;
        protected List<NodeData> designerNodes
        {
            get
            {
                if (mNodeCollection == null)
                {
                    LoadCollection();
                }

                return mNodeCollection.collection;
            }
        }
        protected List<NodeConnection> designerConnection
        {
            get
            {
                if (mNodeCollection == null)
                {
                    LoadCollection();
                }
                return mNodeCollection.connection;
            }
        }
        /// <summary>
        /// 编辑器数据集合
        /// </summary>
        protected NodeDataCollection mNodeCollection;
        protected NodeConnection mMouseStay;
        #endregion

        void OnEnable()
        {
            //创建材质球用来渲染线条
            if (this.mGridMaterial == null)
            {
                this.mGridMaterial = new Material(Shader.Find("Hidden/Designer/Grid"));
                this.mGridMaterial.hideFlags = HideFlags.HideAndDontSave;
                this.mGridMaterial.shader.hideFlags = HideFlags.HideAndDontSave;
            }

            //窗口能否移动
            wantsMouseMove = true;
            minSize = new Vector2(800, 800);
            prefab_name = path.Substring(path.LastIndexOf('/')).Replace(".prefab","");
            //
            VirtualOnEnable();
        }
        void OnDisable()
        {
            VirtualOnDisable();
        }
        void OnGUI()
        {
            //网格背景区域大小，和Screen2Scroll有关，如果修改此处记得修改和Screen2Scroll有关函数
            mGraphRect = new Rect(300, 21, position.width - 15 - 300, position.height - 21 - 15);

            //默认网格位于中心位置
            //if (mGraphScrollPosition == new Vector2(-1, -1))
            //{
            //    this.mGraphScrollPosition = (this.mGraphScrollSize - new Vector2(this.mGraphRect.width, this.mGraphRect.height)) / 2f - 2f * new Vector2(15f, 15f);
            //}
            
            VirtualOnGUI();
            DrawToolTips();
            HandleEvents();
            DrawGraphArea();
        }

        /// <summary>
        /// 处理各种响应事件
        /// </summary>
        protected void HandleEvents()
        {
            VirtualHandleEvent(Event.current);
            switch (Event.current.type)
            {
                case EventType.MouseDown:
                    {
                        if (Event.current.button == 0)
                        {
                            Vector2 mousePosition;
                            if (this.GetMousePositionInGraph(out mousePosition))
                            {
                                if (mMouseStatus == MouseOperation.AddConnection)
                                {
                                    NodeData nd = IsInNode(mCurrentMousePosition);
                                    if (nd != null)
                                    {
                                        selectionNodeConnection.ReplacePoint(nd);
                                    }
                                    else
                                    {
                                        selectionNodeConnection.UpdatePosition();
                                    }

                                    selectionNodeConnection.selected = false;
                                    selectionNodeConnection.beginPoint.selected = false;
                                    selectionNodeConnection.endPoint.selected = false;
                                    selectionNodeConnection = null;
                                    mMouseStatus = MouseOperation.None;
                                    //Event.current.Use();
                                }

                                if (IsSelectConnection(mCurrentMousePosition))
                                {
                                    //
                                }

                                if (mMouseStatus == MouseOperation.None)
                                {
                                    //判断是否点击节点
                                    if (!IsSelectNode(mCurrentMousePosition))
                                    {
                                        mDragMouse.x = Screen2Scroll(mCurrentMousePosition).x;
                                        mDragMouse.y = Screen2Scroll(mCurrentMousePosition).y;
                                        mDragMouse.width = 1;
                                        mDragMouse.height = 1;

                                        mMouseStatus = MouseOperation.SelectionNode;
                                        SelectNode(mDragMouse);
                                    }
                                }
                            }
                            //Event.current.Use();
                        }
                        else if (Event.current.button == 1)
                        {
                            AddRightButtonMenu();
                            //Event.current.Use();
                        }
                        break;
                    }
                case EventType.MouseUp:
                    {
                        if (Event.current.button == 0)
                        {
                            switch (mMouseStatus)
                            {
                                case MouseOperation.SelectionNode:
                                    {
                                        mDragMouse.x = 0;
                                        mDragMouse.y = 0;
                                        mDragMouse.width = 0;
                                        mDragMouse.height = 0;
                                        mMouseStatus = MouseOperation.None;
                                        break;
                                    }
                                case MouseOperation.MoveNode:
                                    {
                                        mDragMouse.x = 0;
                                        mDragMouse.y = 0;
                                        mDragMouse.width = 0;
                                        mDragMouse.height = 0;
                                        mMouseStatus = MouseOperation.None;
                                        break;
                                    }
                                case MouseOperation.MoveConnection:
                                    {
                                        NodeData nd = IsInNode(mCurrentMousePosition);
                                        if (nd != null)
                                        {
                                            selectionNodeConnection.ReplacePoint(nd);
                                        }
                                        else
                                        {
                                            selectionNodeConnection.UpdatePosition();
                                        }

                                        selectionNodeConnection.selected = false;
                                        selectionNodeConnection.beginPoint.selected = false;
                                        selectionNodeConnection.endPoint.selected = false;
                                        selectionNodeConnection = null;
                                        mMouseStatus = MouseOperation.None;
                                        break;
                                    }
                            }

                            //Event.current.Use();
                        }
                        break;
                    }
                case EventType.MouseDrag:
                    {
                        if (Event.current.button == 2 && this.MousePan())
                        {
                            Event.current.Use();
                        }
                        else if (Event.current.button == 0)
                        {
                            switch (mMouseStatus)
                            {
                                case MouseOperation.MoveNode://移动节点
                                    {
                                        for (int i = 0; i < selectionNodes.Count; i++)
                                        {
                                            selectionNodes[i].UpdateRect(Event.current.delta);
                                            //Event.current.Use();
                                        }

                                        SetNodeDirty();
                                        break;
                                    }
                                case MouseOperation.SelectionNode://选择节点，描绘选择框
                                    {
                                        mDragMouse.width = Screen2Scroll(mCurrentMousePosition).x - mDragMouse.x;
                                        mDragMouse.height = Screen2Scroll(mCurrentMousePosition).y - mDragMouse.y;
                                        SelectNode(mDragMouse);
                                        //Event.current.Use();
                                        break;
                                    }
                                case MouseOperation.MoveConnection:
                                    {
                                        if (selectionNodeConnection.beginPoint.selected)
                                        {
                                            selectionNodeConnection.beginPoint.Position = Screen2Scroll(mCurrentMousePosition);
                                        }
                                        else if (selectionNodeConnection.endPoint.selected)
                                        {
                                            selectionNodeConnection.endPoint.Position = Screen2Scroll(mCurrentMousePosition);
                                        }
                                        //Event.current.Use();
                                        break;
                                    }
                            }
                        }
                        this.Repaint();
                        break;
                    }
                case EventType.MouseMove:
                    {
                        MouseStayOn(mCurrentMousePosition);
                        switch (mMouseStatus)
                        {
                            case MouseOperation.None:
                                {
                                    Event.current.Use();
                                    break;
                                }
                            case MouseOperation.AddConnection:
                                {
                                    if (selectionNodeConnection.beginPoint.selected)
                                    {
                                        selectionNodeConnection.beginPoint.Position = Screen2Scroll(mCurrentMousePosition);
                                    }
                                    else if (selectionNodeConnection.endPoint.selected)
                                    {
                                        selectionNodeConnection.endPoint.Position = Screen2Scroll(mCurrentMousePosition);
                                    }
                                    Event.current.Use();
                                    break;
                                }
                        }
                        break;
                    }
                case EventType.scrollWheel:
                    {
                        if (MouseZoom())
                        {
                            Event.current.Use();
                        }
                        break;
                    }
                case EventType.KeyDown:
                    {
                        if (Event.current.keyCode == KeyCode.Delete)
                        {
                            //if (mMouseStatus == MouseOperation.Move)
                            {
                                for (int i = selectionNodes.Count - 1; i >= 0; i--)
                                {
                                    mNodeCollection.Remove(selectionNodes[i]);
                                }
                                selectionNodes.Clear();
                            }
                            Event.current.Use();
                        }
                        break;
                    }

            }
            this.Repaint();
        }
        /// <summary>
        /// 描绘左边Inspector选择Toolbar
        /// </summary>
        public void DrawToolTips()
        {
            GUILayout.BeginArea(new Rect(0,0,300,21));
            tooltipsselection = GUILayout.Toolbar(tooltipsselection, tooltipsNames, EditorStyles.toolbarButton, new GUILayoutOption[0]);
            GUILayout.EndArea();

            VirtualToolTips(tooltipsselection);
        }
        /// <summary>
        /// 加载prefab资源
        /// </summary>
        protected void LoadCollection()
        {
            string absolutePath = Application.dataPath + (path.StartsWith("Assets/") ? path.Substring(6) : path);

            if (!System.IO.File.Exists(absolutePath))
            {
                GameObject go = new GameObject(prefab_name);
                mNodeCollection = go.AddComponent<NodeDataCollection>();

                PrefabUtility.CreatePrefab(path, go);
            }
            else
            {
                GameObject go = PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath(path, typeof(GameObject))) as GameObject;
                mNodeCollection = go.GetComponent<NodeDataCollection>();
                if (mNodeCollection == null)
                {
                    mNodeCollection = go.AddComponent<NodeDataCollection>();
                }
            }

            mGraphOffset = mNodeCollection.offset;
            if (mNodeCollection.scale <= 0.4f)
            {
                mNodeCollection.scale = 0.4f;
            }
            mGraphZoom = mNodeCollection.scale;
            mGraphScrollPosition = mNodeCollection.scrollPosition;
            mNodeCollection.Init();
        }
        /// <summary>
        /// 保存prefab资源
        /// </summary>
        protected void SaveCollection()
        {
            mNodeCollection.offset = mGraphOffset;
            mNodeCollection.scale = mGraphZoom;
            mNodeCollection.scrollPosition = mGraphScrollPosition;
            PrefabUtility.ReplacePrefab(mNodeCollection.gameObject, AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)));
        }
        /// <summary>
        /// 描绘节点详细属性
        /// </summary>
        /// <param name="node"></param>
        public void DrawNodeInspector(NodeData nodedata)
        {
            //name
            GUILayout.BeginHorizontal();
            if (string.IsNullOrEmpty(nodedata.name))
            {
                nodedata.name = DesignerUtility.LoadName(nodedata.node.GetType());
            }
            
            GUILayout.Label("Name", EditorStyles.label, GUILayout.Width(80));
            nodedata.name = GUILayout.TextField(nodedata.name, new GUILayoutOption[2] { GUILayout.Width(190), GUILayout.Height(16) });
            //GUI.DrawTexture();
            GUILayout.EndHorizontal();
            //type
            if (!nodedata.name.Equals(DesignerUtility.LoadName(nodedata.node.GetType())))
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Type", EditorStyles.label, GUILayout.Width(80));
                GUILayout.Label(DesignerUtility.LoadName(nodedata.node.GetType()), new GUILayoutOption[2] { GUILayout.Width(190), GUILayout.Height(16) });
                GUILayout.EndHorizontal();
            }
            //comment
            GUILayout.Label("Comment", EditorStyles.label, GUILayout.Width(80));
            nodedata.comment = GUILayout.TextArea(nodedata.comment, new GUILayoutOption[2] { GUILayout.Width(275), GUILayout.Height(54) });

            Node node = nodedata.node;
            foreach (var it in DesignerUtility.GetBaseClasses(node.GetType()))
            {
                BindingFlags bindingAttr = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
                FieldInfo[] fields = it.GetFields(bindingAttr);
                GUILayout.BeginVertical();

                GUILayoutOption[] options = new GUILayoutOption[2];
                options[0] = GUILayout.Width(190);
                options[1] = GUILayout.Height(16);
                for (int i = fields.Length - 1; i >= 0; i--)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(fields[i].Name), EditorStyles.label, GUILayout.Width(80));
                    if (fields[i].FieldType == typeof(int))
                    {
                        fields[i].SetValue(node, EditorGUILayout.IntField((int)fields[i].GetValue(node), options));
                    }
                    else if (fields[i].FieldType == typeof(string))
                    {
                        fields[i].SetValue(node, EditorGUILayout.TextField((string)fields[i].GetValue(node), options));
                    }
                    else if (fields[i].FieldType == typeof(bool))
                    {
                        fields[i].SetValue(node, EditorGUILayout.Toggle((bool)fields[i].GetValue(node), options));
                    }
                    else if (fields[i].FieldType == typeof(float))
                    {
                        fields[i].SetValue(node, EditorGUILayout.FloatField((float)fields[i].GetValue(node), options));
                    }
                    else if (fields[i].FieldType == typeof(Vector2))
                    {
                        fields[i].SetValue(node, EditorGUILayout.Vector2Field("", (Vector2)fields[i].GetValue(node), options));
                    }
                    else if (fields[i].FieldType == typeof(Vector3))
                    {
                        fields[i].SetValue(node, EditorGUILayout.Vector3Field("", (Vector3)fields[i].GetValue(node), options));
                    }
                    else if (fields[i].FieldType == typeof(Rect))
                    {
                        options[1] = GUILayout.Height(32);
                        fields[i].SetValue(node, EditorGUILayout.RectField("", (Rect)fields[i].GetValue(node), options));
                    }
                    else if (fields[i].FieldType.IsEnum)
                    {
                        fields[i].SetValue(node, EditorGUILayout.EnumPopup((System.Enum)System.Enum.Parse(fields[i].FieldType, fields[i].GetValue(node).ToString()), options));
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.Space(3);
                }
                GUILayout.EndVertical();
                if (GUI.changed)
                {
                    Debug.LogError("Here is save data!!!");
                }
            }
            //BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            //FieldInfo[] fields = node.GetType().GetFields(bindingAttr);
            //GUILayout.BeginVertical();

            //GUILayoutOption[] options = new GUILayoutOption[2];
            //options[0] = GUILayout.Width(190);
            //options[1] = GUILayout.Height(16);
            //for (int i = fields.Length - 1; i >= 0; i--)
            //{
            //    GUILayout.BeginHorizontal();
            //    GUILayout.Label(Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(fields[i].Name), EditorStyles.label, GUILayout.Width(80));
            //    if (fields[i].FieldType == typeof(int))
            //    {
            //        fields[i].SetValue(node, EditorGUILayout.IntField((int)fields[i].GetValue(node), options));
            //    }
            //    else if (fields[i].FieldType == typeof(string))
            //    {
            //        fields[i].SetValue(node, EditorGUILayout.TextField((string)fields[i].GetValue(node), options));
            //    }
            //    else if (fields[i].FieldType == typeof(bool))
            //    {
            //        fields[i].SetValue(node, EditorGUILayout.Toggle((bool)fields[i].GetValue(node), options));
            //    }
            //    else if (fields[i].FieldType == typeof(float))
            //    {
            //        fields[i].SetValue(node, EditorGUILayout.FloatField((float)fields[i].GetValue(node), options));
            //    }
            //    else if (fields[i].FieldType == typeof(Vector2))
            //    {
            //        fields[i].SetValue(node, EditorGUILayout.Vector2Field("", (Vector2)fields[i].GetValue(node), options));
            //    }
            //    else if (fields[i].FieldType == typeof(Vector3))
            //    {
            //        fields[i].SetValue(node, EditorGUILayout.Vector3Field("", (Vector3)fields[i].GetValue(node), options));
            //    }
            //    else if (fields[i].FieldType == typeof(Rect))
            //    {
            //        fields[i].SetValue(node, EditorGUILayout.RectField("", (Rect)fields[i].GetValue(node), options));
            //    }
            //    else if (fields[i].FieldType.IsEnum)
            //    {
            //        fields[i].SetValue(node, EditorGUILayout.EnumPopup((System.Enum)System.Enum.Parse(fields[i].FieldType, fields[i].GetValue(node).ToString()), options));
            //    }
            //    GUILayout.EndHorizontal();
            //    GUILayout.Space(3);
            //}
            //GUILayout.EndVertical();
            //if (GUI.changed)
            //{
            //    Debug.LogError("Here is save data!!!");
            //}
        }

        /// <summary>
        /// 转换当前鼠标位置到桂东条中的位置，如果Inspector界面区域改变，修改此函数
        /// </summary>
        /// <param name="mousePosition"></param>
        /// <returns></returns>
        public Vector2 Screen2Scroll(Vector2 mousePosition)
        {
            return new Vector2(mCurrentMousePosition.x + mGraphScrollPosition.x - 300, mCurrentMousePosition.y + mGraphScrollPosition.y - 21);
        }
        #region 背景功能
        /// <summary>
        /// 描绘格子棋盘
        /// </summary>
        /// <returns></returns>
        protected bool DrawGraphArea()
        {
            GUI.Box(this.mGraphRect, string.Empty, DesignerUtility.GraphBackgroundGUIStyle);
            this.DrawGrid();
            EditorZoomArea.Begin(this.mGraphRect, this.mGraphZoom);
            Vector2 mousePosition;
            if (!this.GetMousePositionInGraph(out mousePosition))
            {
                mousePosition = new Vector2(-1f, -1f);
            }
            EditorZoomArea.End();
            if (Event.current.type != EventType.ScrollWheel)
            {
                Vector2 vector = GUI.BeginScrollView(new Rect(this.mGraphRect.x, this.mGraphRect.y, this.mGraphRect.width + 15f, this.mGraphRect.height + 15f), this.mGraphScrollPosition, new Rect(0f, 0f, this.mGraphScrollSize.x, this.mGraphScrollSize.y), true, true);
                if (vector != this.mGraphScrollPosition && Event.current.type != EventType.DragUpdated && Event.current.type != EventType.Ignore)
                {
                    this.mGraphOffset -= (vector - this.mGraphScrollPosition) / this.mGraphZoom;
                    this.mGraphScrollPosition = vector;
                }

                DrawNode();

                if (mMouseStatus == MouseOperation.SelectionNode)
                {
                    NodeDesigner.DrawRect(mDragMouse, DesignerUtility.SelectionGUIStyle);
                }
                GUI.EndScrollView();
            }



            return true;
        }
        /// <summary>
        /// 画背景格子棋盘
        /// </summary>
        protected void DrawGrid()
        {
            this.mGridMaterial.SetPass((!EditorGUIUtility.isProSkin) ? 1 : 0);
            GL.PushMatrix();
            GL.Begin(1);
            this.DrawGridLines(10f * this.mGraphZoom, new Vector2(this.mGraphOffset.x % 10f * this.mGraphZoom, this.mGraphOffset.y % 10f * this.mGraphZoom));
            GL.End();
            GL.PopMatrix();
            this.mGridMaterial.SetPass((!EditorGUIUtility.isProSkin) ? 3 : 2);
            GL.PushMatrix();
            GL.Begin(1);
            this.DrawGridLines(50f * this.mGraphZoom, new Vector2(this.mGraphOffset.x % 50f * this.mGraphZoom, this.mGraphOffset.y % 50f * this.mGraphZoom));
            GL.End();
            GL.PopMatrix();
        }
        /// <summary>
        /// 画格子棋盘线条
        /// </summary>
        /// <param name="gridSize"></param>
        /// <param name="offset"></param>
        protected void DrawGridLines(float gridSize, Vector2 offset)
        {
            float num = this.mGraphRect.x + offset.x;
            if (offset.x < 0f)
            {
                num += gridSize;
            }
            for (float num2 = num; num2 < this.mGraphRect.x + this.mGraphRect.width; num2 += gridSize)
            {
                this.DrawLine(new Vector2(num2, this.mGraphRect.y), new Vector2(num2, this.mGraphRect.y + this.mGraphRect.height));
            }
            float num3 = this.mGraphRect.y + offset.y;
            if (offset.y < 0f)
            {
                num3 += gridSize;
            }
            for (float num4 = num3; num4 < this.mGraphRect.y + this.mGraphRect.height; num4 += gridSize)
            {
                this.DrawLine(new Vector2(this.mGraphRect.x, num4), new Vector2(this.mGraphRect.x + this.mGraphRect.width, num4));
            }
        }
        /// <summary>
        /// 画线
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        protected void DrawLine(Vector2 p1, Vector2 p2)
        {
            GL.Vertex(p1);
            GL.Vertex(p2);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mousePosition"></param>
        /// <returns></returns>
        protected bool GetMousePositionInGraph(out Vector2 mousePosition)
        {
            mousePosition = this.mCurrentMousePosition;
            if (!this.mGraphRect.Contains(mousePosition))
            {
                return false;
            }

            mousePosition -= new Vector2(this.mGraphRect.xMin, this.mGraphRect.yMin);
            mousePosition /= this.mGraphZoom;
            return true;
        }
        /// <summary>
        /// 控制界面的移动
        /// </summary>
        /// <returns></returns>
        protected bool MousePan()
        {
            Vector2 vector;
            if (!this.GetMousePositionInGraph(out vector))
            {
                return false;
            }
            Vector2 vector2 = Event.current.delta;
            if (Event.current.type == EventType.ScrollWheel)
            {
                vector2 *= -1.5f;
                if (Event.current.modifiers == EventModifiers.Control)
                {
                    vector2.x = vector2.y;
                    vector2.y = 0f;
                }
            }
            this.ScrollGraph(vector2);
            return true;
        }
        protected void ScrollGraph(Vector2 amount)
        {
            this.mGraphOffset += amount / this.mGraphZoom;
            this.mGraphScrollPosition -= amount;
            SetNodeDirty();
            base.Repaint();
        }
        /// <summary>
        /// 控制界面的缩放
        /// </summary>
        /// <returns></returns>
        protected bool MouseZoom()
        {
            Vector2 b;
            if (!this.GetMousePositionInGraph(out b))
            {
                return false;
            }
            float num = -Event.current.delta.y / 150f;
            float origin = mGraphZoom;
            this.mGraphZoom += num;
            this.mGraphZoom = Mathf.Clamp(this.mGraphZoom, 0.6f, 1.3f);
            Vector2 a;
            this.GetMousePositionInGraph(out a);
            this.mGraphOffset += a - b;
            //this.mGraphScrollPosition += a - b;
            //mGraphScrollPosition += a - b;
            //mGraphScrollPosition += a - b;
            //SetNodeDirty();
            SetNodeDirty(origin);
            return true;
        }
        #endregion
        #region 描绘节点
        /// <summary>
        /// 描绘节点和连接线
        /// </summary>
        protected void DrawNode()
        {
            for (int i = 0; i < designerNodes.Count; i++)
            {
                NodeDesigner.DrawNode(designerNodes[i], true, true);
            }
            for (int i = 0; i < designerConnection.Count; i++)
            {
                NodeDesigner.DrawConnection(designerConnection[i]);
            }
        }
        /// <summary>
        /// 刷新节点位置
        /// </summary>
        protected void SetNodeDirty()
        {
            for (int i = 0; i < designerNodes.Count; i++)
            {
                designerNodes[i].UpdateRect(mGraphZoom);
            }
            for (int i = 0; i < designerConnection.Count; i++)
            {
                designerConnection[i].UpdatePointPosition();
            }
        }
        /// <summary>
        /// 刷新节点位置
        /// </summary>
        /// <param name="origin"></param>
        protected void SetNodeDirty(float origin)
        {
            for (int i = 0; i < designerNodes.Count; i++)
            {
                designerNodes[i].UpdateRect(mGraphZoom);
                designerNodes[i].Position = Screen2Scroll(mCurrentMousePosition) + (designerNodes[i].Position - Screen2Scroll(mCurrentMousePosition)) / origin * mGraphZoom;
            }
        }
        #endregion
        #region MOUSE BUTTON 0 FUNTION
        /// <summary>
        /// 选择节点
        /// </summary>
        /// <param name="rect"></param>
        public void SelectNode(Rect rect)
        {
            selectionNodes.Clear();
            for (int i = 0; i < designerNodes.Count; i++)
            {
                if (rect.Contains(designerNodes[i].Position))
                {
                    selectionNodes.Add(designerNodes[i]);
                    designerNodes[i].selected = true;
                }
                else
                {
                    designerNodes[i].selected = false;
                }
            }
            if (selectionNodes.Count > 0)
            {
                mMouseStatus = MouseOperation.SelectionNode;
            }
        }
        /// <summary>
        /// 判断鼠标位置是否再节点内
        /// </summary>
        /// <param name="mousePosition"></param>
        /// <returns></returns>
        public bool IsSelectNode(Vector2 mousePosition)
        {
            mousePosition = Screen2Scroll(mousePosition);
            for (int i = 0; i < designerNodes.Count; i++)
            {
                if (designerNodes[i].Rect.Contains(mousePosition))
                {
                    designerNodes[i].selected = true;
                    if (selectionNodes.Contains(designerNodes[i]))
                    {
                        mMouseStatus = MouseOperation.MoveNode;
                    }
                    else
                    {
                        foreach (var it in selectionNodes)
                        {
                            it.selected = false;
                        }
                        selectionNodes.Clear();
                        selectionNodes.Add(designerNodes[i]);
                        mMouseStatus = MouseOperation.MoveNode;
                    }
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mousePosition"></param>
        /// <returns></returns>
        public NodeData IsInNode(Vector2 mousePosition)
        {
            mousePosition = Screen2Scroll(mousePosition);
            for (int i = 0; i < designerNodes.Count; i++)
            {
                if (designerNodes[i].Rect.Contains(mousePosition))
                {
                    return designerNodes[i];
                }
            }
            return null;
        }
        /// <summary>
        /// 选择连接线
        /// </summary>
        /// <param name="mousePosition"></param>
        /// <returns></returns>
        public bool IsSelectConnection(Vector2 mousePosition)
        {
            mousePosition = Screen2Scroll(mousePosition);
            for (int i = 0; i < designerConnection.Count; i++)
            {
                if (designerConnection[i].beginPoint.Rect.Contains(mousePosition))
                {
                    selectionNodeConnection = designerConnection[i];
                    selectionNodeConnection.selected = true;
                    selectionNodeConnection.beginPoint.selected = true;
                    mMouseStatus = MouseOperation.MoveConnection;

                    return true;
                }

                if (designerConnection[i].endPoint.Rect.Contains(mousePosition))
                {
                    selectionNodeConnection = designerConnection[i];
                    selectionNodeConnection.selected = true;
                    selectionNodeConnection.endPoint.selected = true;
                    mMouseStatus = MouseOperation.MoveConnection;

                    return true;
                }
            }
            return false;
        }
        public void MouseStayOn(Vector2 mousePosition)
        {
            if (mMouseStay != null)
            {
                mMouseStay.mouse_on = false;
            }
            mousePosition = Screen2Scroll(mousePosition);
            for (int i = 0; i < designerConnection.Count; i++)
            {
                if (designerConnection[i].beginPoint.Rect.Contains(mousePosition) || designerConnection[i].endPoint.Rect.Contains(mousePosition))
                {
                    mMouseStay = designerConnection[i];
                    designerConnection[i].mouse_on = true;
                }
            }
        }
        public NodePosition CalculateNodePosition(NodeConnection nodeCon)
        {

            return NodePosition.Bottom;
        }
        #endregion
        #region MOUSE BUTTON 1 FUNTION
        protected GenericMenu genericMenu = new GenericMenu();
        public void AddRightButtonMenu()
        {
            Vector2 mouse_position;
            if (!GetMousePositionInGraph(out mouse_position)) { return; }
            genericMenu = new GenericMenu();

            if (IsSelectConnection(mCurrentMousePosition))
            {
                //未选中节点时，添加新的节点功能
                genericMenu.AddItem(new GUIContent("Delete Connection"), false, DeleteNodeConnection);
                genericMenu.ShowAsContext();
                
                return;
            }

            if (IsSelectNode(mCurrentMousePosition))
            {
                if (selectionNodes.Count == 0)
                {
                    //未选中节点时，添加新的节点功能
                    genericMenu.AddItem(new GUIContent("Add/Node"), false, AddNode);
                    genericMenu.ShowAsContext();
                }
                else if (selectionNodes.Count == 1)
                {
                    if (designerNodes.Count > 1)
                    {
                        genericMenu.AddItem(new GUIContent("Add/Connection"), false, AddConnection);
                        genericMenu.AddSeparator("");
                    }

                    genericMenu.AddItem(new GUIContent("Delete Node"), false, DeleteNode);
                    genericMenu.ShowAsContext();
                }
                else if(selectionNodes.Count > 1)
                {
                    genericMenu.AddItem(new GUIContent("Delete Nodes"), false, DeleteNode);
                    genericMenu.ShowAsContext();
                }
            }
            else
            {
                //未选中节点时，添加新的节点功能
                genericMenu.AddItem(new GUIContent("Add Node/Node"), false, AddNode);
                genericMenu.AddSeparator("");
                genericMenu.ShowAsContext();
            }
        }
        /// <summary>
        /// 删除节点
        /// </summary>
        private void DeleteNode()
        {
            for (int i = selectionNodes.Count - 1; i >= 0; i--)
            {
                mNodeCollection.Remove(selectionNodes[i]);
            }
            selectionNodes.Clear();
        }
        private void DeleteNodeConnection()
        {
            mNodeCollection.Remove(selectionNodeConnection);
            selectionNodeConnection = null;
            mMouseStatus = MouseOperation.None;
        }
        public void AddNode()
        {
            mNodeCollection.Add(new NodeData() { nodeColor = NodeColor.Purple, Position = Screen2Scroll(mCurrentMousePosition) });
            if (designerNodes.Count >= 2)
            {
                mNodeCollection.Add(new NodeConnection(designerNodes[designerNodes.Count - 1], designerNodes[designerNodes.Count - 2]));
            }
            SetNodeDirty();
        }
        private void AddConnection()
        {
            NodeConnection conn = new NodeConnection(selectionNodes[0], (designerNodes[0] == selectionNodes[0]) ? designerNodes[1] : designerNodes[0]);
            mNodeCollection.Add(conn);
            selectionNodeConnection = conn;
            selectionNodeConnection.selected = true;
            selectionNodeConnection.endPoint.selected = true;
            selectionNodeConnection.endPoint.Position = Screen2Scroll(mCurrentMousePosition);
            mMouseStatus = MouseOperation.AddConnection;
            conn.UpdatePointPosition();
        }
        #endregion
        #region override function
        /// <summary>
        /// 
        /// </summary>
        public virtual void VirtualOnEnable()
        {
            LoadCollection();
            SetNodeDirty();
        }
        /// <summary>
        /// 
        /// </summary>
        public virtual void VirtualOnDisable()
        {
            SaveCollection();
            DestroyImmediate(mNodeCollection.gameObject);
        }
        /// <summary>
        /// 
        /// </summary>
        public virtual void VirtualOnGUI()
        {
            //刷新鼠标位置
            mCurrentMousePosition = Event.current.mousePosition;
        }
        /// <summary>
        /// 左界面Inspector界面toolbar功能
        /// </summary>
        /// <param name="select_tooltip"></param>
        public virtual void VirtualToolTips(int select_tooltip)
        {
        }
        /// <summary>
        /// 响应事件处理
        /// </summary>
        /// <param name="currect"></param>
        public virtual void VirtualHandleEvent(Event currect)
        {

        }
        protected void VirtualButtonRight()
        {
            if (IsInNode(mCurrentMousePosition) == null)
            {

            }
        }
        #endregion
    }
}