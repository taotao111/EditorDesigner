using UnityEngine;
using System.Collections.Generic;
using Designer.Runtime;
using UnityEditor;
using System;

namespace Designer.Editor
{
    public class NodeDesigner
    {
        public static void DrawNode(NodeData node, bool selected, bool enable)
        {
            //描绘四个顶点
            foreach (var it in node.nodeCollection)
            {
                if (it.enable)
                {
                    GUI.Label(it.Rect, string.Empty, DesignerUtility.GetBackgroundGUIStyle(node.nodeColor));
                }
            }

            //选中效果
            if (node.selected)
            {
                GUI.Label(new Rect(node.Rect.x - 1, node.Rect.y - 1, node.Rect.width + 2, node.Rect.height + 2), string.Empty, DesignerUtility.GetNodeSelectGUIStyle(node.nodeColor));
            }

            //背景框
            GUI.Label(node.Rect, string.Empty, DesignerUtility.GetBackgroundGUIStyle(node.nodeColor));

            Vector2 size = DesignerUtility.NodeTitleGUIStyle.CalcSize(new GUIContent(node.name));

            GUI.Label(new Rect(node.Position.x - size.x * 0.5f, node.Position.y - size.y * 0.5f, node.Rect.width, size.y), node.name, DesignerUtility.NodeTitleGUIStyle);
            //描绘背景框
            DrawTexture(node.Rect, null);

            //GUI.BeginGroup(node.Rect);
            ////GUILayout.Label(node.name);
            //GUI.Label(new Rect(node.Position.x,node.Position.y,100,16), node.name, EditorStyles.label);
            //GUI.EndGroup();
        }
        public static void DrawTexture(Rect rect, Texture2D iconBorderTexture)
        {
            GUI.DrawTexture(rect, iconBorderTexture);
        }
        public static void DrawConnection(NodeConnection connection)
        {
            Color color = new Color(0.6f, 0.6f, 0.6f, 1);

            if (connection.mouse_on)
            {
                color = Color.red;
            }

            Vector3 startPos = connection.beginPoint.Position;
            Vector3 endPos = connection.endPoint.Position;
            float mnog = Vector3.Distance(startPos, endPos);
            //Vector3 startTangent = startPos + Vector3.down * (mnog / 3f);
            //Vector3 endTangent = endPos + Vector3.up * (mnog / 3f);
            Vector3 startTangent = startPos;
            Vector3 endTangent = endPos;

            switch (connection.beginPoint.nodePosition)
            {
                case NodePosition.Top:
                    {
                        startTangent += Vector3.down * (mnog / 2f);
                        break;
                    }
                case NodePosition.Bottom:
                    {
                        startTangent += Vector3.up * (mnog / 2f);
                        break;
                    }
                case NodePosition.Left:
                    {
                        startTangent += Vector3.left * (mnog / 2f);
                        break;
                    }
                case NodePosition.Right:
                    {
                        startTangent += Vector3.right * (mnog / 2f);
                        break;
                    }
            }
            switch (connection.endPoint.nodePosition)
            {
                case NodePosition.Top:
                    {
                        endTangent += Vector3.down * (mnog / 2f);
                        break;
                    }
                case NodePosition.Bottom:
                    {
                        endTangent += Vector3.up * (mnog / 2f);
                        break;
                    }
                case NodePosition.Left:
                    {
                        endTangent += Vector3.left * (mnog / 2f);
                        break;
                    }
                case NodePosition.Right:
                    {
                        endTangent += Vector3.right * (mnog / 2f);
                        break;
                    }
            }
            Handles.BeginGUI();
            Handles.DrawBezier(startPos, endPos, startTangent, endTangent, color, null, 4f);
            Handles.EndGUI();

            //画点，起始点
            Color color2 = GUI.color;
            GUI.color = color;

            DrawTexture(connection.beginPoint.Rect, DesignerUtility.GetNodeConnectionTextures(connection.Begin.nodeColor));
            //DrawTexture(connection.endPoint.Rect, DesignerUtility.GetNodeConnectionTextures(connection.End.nodeColor));

            GUI.color = color2;
            //终点箭头
            switch (connection.endPoint.nodePosition)
            {
                case NodePosition.Top:
                    {
                        //DrawArrowHead(connection.endPoint.Rect, DesignerUtility.GetNodeConnectionEndTopTextures(connection.End.nodeColor), connection.endPoint.Position, color, 1.0f);
                        DrawArrowHead(connection.endPoint.Rect, DesignerUtility.GetNodeConnectionEndTopTextures(NodeColor.Def), connection.endPoint.Position, color, 1.0f);

                        break;
                    }
                case NodePosition.Bottom:
                    {
                        //DrawArrowHead(connection.endPoint.Rect, DesignerUtility.GetNodeConnectionEndBottomTextures(connection.End.nodeColor), connection.endPoint.Position, color, 1.0f);
                        DrawArrowHead(connection.endPoint.Rect, DesignerUtility.GetNodeConnectionEndBottomTextures(NodeColor.Def), connection.endPoint.Position, color, 1.0f);

                        break;
                    }
            }
        }
        public static void DrawRect(Rect rect, GUIStyle guiStyle)
        {
            GUI.Label(rect, string.Empty, guiStyle);
        }
        public static void DrawArrowHead(Texture leftArrow, Vector2 pos, Color color, bool flipTexture, float scale)
        {
            Color color2 = GUI.color;
            GUI.color = color;
            if (!flipTexture)
            {
                GUI.DrawTexture(new Rect(pos.x, pos.y - (float)leftArrow.height * 0.5f, (float)leftArrow.width * scale, (float)leftArrow.height), leftArrow);
            }
            else
            {
                Matrix4x4 matrix = GUI.matrix;
                GUIUtility.ScaleAroundPivot(new Vector2(-1f, 1f), pos);
                GUI.DrawTexture(new Rect(pos.x, pos.y - (float)leftArrow.height * 0.5f, (float)leftArrow.width * scale, (float)leftArrow.height), leftArrow);
                GUI.matrix = matrix;
            }
            GUI.color = color2;
        }
        public static void DrawArrowHead(Rect rect,Texture leftArrow, Vector2 pos, Color color, float scale)
        {
            Color color2 = GUI.color;
            GUI.color = color;

            Matrix4x4 matrix = GUI.matrix;
            GUIUtility.ScaleAroundPivot(new Vector2(-1f, 1f), pos);
            //GUI.DrawTexture(new Rect(pos.x, pos.y, (float)leftArrow.width, (float)leftArrow.height), leftArrow);
            GUI.DrawTexture(new Rect(pos.x - rect.width * scale * 0.5f, pos.y - rect.height * scale * 0.5f, rect.width * scale,rect.height * scale), leftArrow);
            GUI.matrix = matrix;

            GUI.color = color2;
        }

    }

}