using UnityEngine;
using System.Collections.Generic;
namespace Designer.Runtime
{
    public class NodeDataCollection : MonoBehaviour
    {
        public List<NodeData> collection = new List<NodeData>();
        public List<NodeConnection> connection = new List<NodeConnection>();
        [HideInInspector]
        public Vector2 offset;
        [HideInInspector]
        public Vector2 scrollPosition = new Vector2(-1, -1);
        [HideInInspector]
        public float scale;

        public void Init()
        {
            foreach (var it in collection)
            {
                it.ResetNodeCollection();
            }
            for (int i = 0; i < connection.Count; i++)
            {
                foreach (var it in collection)
                {
                    if (connection[i].Begin.id == it.id)
                    {
                        connection[i].Begin = it;
                        continue;
                    }
                    if (connection[i].End.id == it.id)
                    {
                        connection[i].End = it;
                        continue;
                    }
                }
                connection[i].AddConnectionToNode(connection[i].beginPoint.nodePosition, connection[i].endPoint.nodePosition);
            }
        }
        public void Remove(NodeData nodeData)
        {
            for (int i = connection.Count - 1; i >= 0; i--)
            {
                if (connection[i].Begin == nodeData || connection[i].End == nodeData)
                {
                    Remove(connection[i]);
                }
            }
            collection.Remove(nodeData);
            ReSortNode();
        }
        public void Remove(NodeConnection nodeConnection)
        {
            nodeConnection.Remove();
            connection.Remove(nodeConnection);
        }
        public void Add(NodeData nodeData)
        {
            if (!collection.Contains(nodeData))
            {
                collection.Add(nodeData);
                ReSortNode();
            }
        }
        public void ReSortNode()
        {
            for (int i = 0; i < collection.Count; i++)
            {
                collection[i].id = i;
            }
        }
        public void Add(NodeConnection nodeConnection)
        {
            if (!connection.Contains(nodeConnection))
            {
                connection.Add(nodeConnection);
            }
        }
    }
}