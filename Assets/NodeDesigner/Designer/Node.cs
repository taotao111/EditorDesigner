using UnityEngine;
using System.Collections;
namespace Designer.Runtime
{
    public abstract class Node
    {
        [NodeTooltip("Name")]
        public string Name;
        public int id;
        public NodePosition position = NodePosition.Bottom;
        public Vector3 v3;
    }

    public class NodeExtern : Node
    {
        public Rect rect;
    }
}