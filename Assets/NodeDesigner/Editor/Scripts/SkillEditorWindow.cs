using UnityEngine;
using UnityEditor;
using Designer.Editor;
using Designer.Runtime;

public class SkillEditorWindow : DesignerWindow {
    protected Vector2 inspector_scroll = Vector2.zero;

    [MenuItem("Tools/DesignerWindow")]
    public static void Open()
    {
        GetWindow<SkillEditorWindow>();
    }

    public override void VirtualToolTips(int select_tooltip)
    {
        DrawInspector();
    }
    protected void DrawInspector()
    {
        GUILayout.BeginArea(new Rect(5, 21, 295, position.height));
        GUILayout.Space(5);
        if (selectionNodes.Count == 0)
        {
            GUILayout.Space(50);
            GUILayout.Label("                        请选择节点进行编辑!");
        }
        else if (selectionNodes.Count == 1)
        {
            if (selectionNodes[0].node == null)
            {
                selectionNodes[0].node = new NodeExtern();
            }
            inspector_scroll = GUILayout.BeginScrollView(inspector_scroll, false, false, null);
            DrawNodeInspector(selectionNodes[0]);
            GUILayout.EndScrollView();
        }
        else
        {
            GUILayout.Space(50);
            GUILayout.Label("                        只能编辑一个节点！");
        }
        GUILayout.EndArea();
    }
}
