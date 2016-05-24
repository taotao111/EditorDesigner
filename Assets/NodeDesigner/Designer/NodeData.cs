using UnityEngine;
using System.Collections.Generic;
using System;

namespace Designer.Runtime
{
    public enum NodeStatus
    {
        Inactive,
        Success,
        Failed,
    }
    public enum NodeColor
    {
        Def = 0,
        Red,
        Pink,
        Brown,
        RedOrange,
        Turquoise,
        Cyan,
        Blue,
        Purple,
    }
    public enum NodePosition
    {
        Top,
        Bottom,
        Left,
        Right,
    }
    [System.Serializable]
    public class NodeData
    {
        [HideInInspector]
        public int id = -1;
        public bool isEntry = false;
        public bool selected = false;

        public NodeStatus nodeStatus = NodeStatus.Inactive;
        public NodeColor nodeColor = NodeColor.Def;

        public Node node = null;

        //[HideInInspector]
        public List<NodeCollection> nodeCollection = new List<NodeCollection>();
        [HideInInspector]
        [SerializeField]
        private Vector2 m_Position;

        private Vector2 m_Scale = new Vector2(100,100);
        private Rect m_Rect = new Rect(0, 0, 0, 0);
        private float m_Zoom = 1;
        
        public Vector2 Position
        {
            get
            {
                return m_Position;
            }
            set
            {
                m_Position = value;
                UpdateRect();
            }
        }
        public Vector2 Scale
        {
            get
            {
                return m_Scale;
            }
            set
            {
                m_Scale = value;
                UpdateRect();
            }
        }
        public Rect Rect
        {
            get
            {
                return m_Rect;
            }
        }
        [System.NonSerialized]
        public List<NodeData> Fronts = new List<NodeData>();
        [System.NonSerialized]
        public List<NodeData> Nexts = new List<NodeData>();

        public NodeData()
        {
            AddNodeConnection(NodePosition.Top);
            AddNodeConnection(NodePosition.Bottom);
            AddNodeConnection(NodePosition.Left);
            AddNodeConnection(NodePosition.Right);
            //NodeCollection left = AddNodeConnection(NodePosition.Left);
            //left.enable = false;
            //NodeCollection right = AddNodeConnection(NodePosition.Right);
            //right.enable = false;
        }

        public void UpdateRect()
        {
            m_Rect.x = (Position.x - Scale.x * 0.5f * m_Zoom);
            m_Rect.y = (Position.y - Scale.y * 0.5f * m_Zoom);
            m_Rect.width = Scale.x * m_Zoom;
            m_Rect.height = Scale.y * m_Zoom;

            //
            UpdateNodeCollection();
        }
        public void UpdateRect(float zoom)
        {
            m_Zoom = zoom;
            UpdateRect();
        }
        public void UpdateRect(Vector2 offset)
        {
            Position += offset;
        }

        #region COLLECTION
        public void UpdateNodeCollection()
        {
            foreach (var it in nodeCollection)
            {
                switch (it.nodePosition)
                {
                    case NodePosition.Top:
                        {
                            it.Position = this.Rect.Top();
                            break;
                        }
                    case NodePosition.Bottom:
                        {
                            it.Position = this.Rect.Bottom();
                            break;
                        }
                    case NodePosition.Left:
                        {
                            it.Position = this.Rect.CenterLeft();
                            break;
                        }
                    case NodePosition.Right:
                        {
                            it.Position = this.Rect.CenterRight();
                            break;
                        }
                }
                it.Zoom = m_Zoom;
            }
        }
        private NodeCollection AddNodeConnection(NodePosition nodePosition)
        {
            NodeCollection nodeCon = null;
            if (GetNodeConnection(nodePosition,out nodeCon))
            {
                return nodeCon;
            }
            switch (nodePosition)
            {
                case NodePosition.Top:
                    {
                        nodeCon = new NodeCollection(NodePosition.Top,NodeCollection.Axis.Horizontal);
                        nodeCon.Position = this.Rect.Top();
                        break;
                    }
                case NodePosition.Bottom:
                    {
                        nodeCon = new NodeCollection(NodePosition.Bottom,NodeCollection.Axis.Horizontal);
                        nodeCon.Position = this.Rect.Bottom();
                        break;
                    }
                case NodePosition.Left:
                    {
                        nodeCon = new NodeCollection(NodePosition.Left,NodeCollection.Axis.Vertical);
                        nodeCon.Position = this.Rect.CenterLeft();
                        break;
                    }
                case NodePosition.Right:
                    {
                        nodeCon = new NodeCollection(NodePosition.Right,NodeCollection.Axis.Vertical);
                        nodeCon.Position = this.Rect.CenterRight();
                        break;
                    }
            }

            nodeCollection.Add(nodeCon);

            return nodeCon;
        }
        public bool GetNodeConnection(NodePosition nodePosition,out NodeCollection outNodeCon)
        {
            for (int i = 0; i < nodeCollection.Count; i++)
            {
                if (nodeCollection[i].nodePosition == nodePosition)
                {
                    outNodeCon = nodeCollection[i];
                    return true;
                }
            }
            outNodeCon = null;
            return false;
        }
        public bool IsExist(NodePosition nodePosition)
        {
            for (int i = 0; i < nodeCollection.Count; i++)
            {
                if (nodeCollection[i].nodePosition == nodePosition)
                {
                    return true;
                }
            }
            return false;
        }
        public bool AddNodeConnection(NodePosition nodePosition,NodeConnectionPoint point)
        {
            NodeCollection nodeCon;
            if (!GetNodeConnection(nodePosition, out nodeCon))
            {
                nodeCon = AddNodeConnection(nodePosition);
            }
            point.nodePosition = nodePosition;
            return nodeCon.AddPoint(point);
        }
        public bool RemoveNodeConnection(NodePosition nodePosition, NodeConnectionPoint point)
        {
            NodeCollection nodeCon;
            if (!GetNodeConnection(nodePosition, out nodeCon))
            {
                nodeCon = AddNodeConnection(nodePosition);
                return false;
            }

            return nodeCon.RemovePoint(point);
        }
        public bool RemoveNodeConnection(NodeConnectionPoint point)
        {
            NodeCollection nodeCon;
            if (!GetNodeConnection(point.nodePosition, out nodeCon))
            {
                nodeCon = AddNodeConnection(point.nodePosition);
                return false;
            }

            return nodeCon.RemovePoint(point);
        }
        public void ResetNodeCollection()
        {
            foreach (var it in nodeCollection)
            {
                it.points.Clear();
            }
        }
        #endregion


    }
    [System.Serializable]
    public class NodeCollection
    {
        public enum Axis
        {
            Horizontal,
            Vertical,
        }
        public NodePosition nodePosition = NodePosition.Top;
        private readonly int base_size = 20;
        private readonly int cell_size = 10;
        public bool enable = true;
        private Rect m_Rect = new Rect();
        private Axis axis = Axis.Horizontal;
        //[HideInInspector]
        [SerializeField]
        private Vector2 m_position;
        [HideInInspector]
        [SerializeField]
        private float m_Zoom = 1;
        [SerializeField]
        public List<NodeConnectionPoint> points = new List<NodeConnectionPoint>();

        public int Count
        {
            get
            {
                return points.Count;
            }
        }
        public Vector2 Position
        {
            get
            {
                return m_position;
            }
            set
            {
                m_position = value;

                UpdateRect();
            }
        }
        public float Zoom
        {
            get
            {
                return m_Zoom;
            }
            set
            {
                m_Zoom = value;

                UpdateRect();
            }
        }
        public Rect Rect
        {
            get
            {
                return m_Rect;
            }
        }

        public NodeCollection(NodePosition nodePosition,Axis axis)
        {
            this.nodePosition = nodePosition;
            this.axis = axis;
        }

        private void UpdateRect()
        {
            m_Rect.width = ((axis == Axis.Horizontal) ? 25 + 10 * Count : 25) * m_Zoom;
            m_Rect.height = ((axis == Axis.Vertical) ? 25 + 10 * Count : 25) * m_Zoom;

            m_Rect.x = Position.x - m_Rect.width * 0.5f;
            m_Rect.y = Position.y - m_Rect.height * 0.5f;

            UpdatePointsPosition();
        }

        public bool AddPoint(NodeConnectionPoint point)
        {
            if (!points.Contains(point))
            {
                points.Add(point);
                //points.Sort(Sort);
                UpdateId();
                UpdateRect();
                return true;
            }
            return false;
        }

        public bool RemovePoint(NodeConnectionPoint point)
        {
            if (points.Remove(point))
            {
                //points.Sort(Sort);
                UpdateId();
                UpdateRect();
                return true;
            }
            return false;
        }
        public void UpdateId()
        {
            for (int i = 0; i < points.Count; i++)
            {
                points[i].id = i;
            }
        }
        public int Sort(NodeConnectionPoint a,NodeConnectionPoint b)
        {
            switch(nodePosition)
            {
                case NodePosition.Top:
                case NodePosition.Bottom:
                    {
                        return b.Position.x.CompareTo(a.Position.x);
                    }
                case NodePosition.Left:
                case NodePosition.Right:
                    {
                        return b.Position.y.CompareTo(a.Position.y);
                    }
            }
            return 0;
        }
        public void UpdatePointsPosition()
        {
            for (int i = 0; i < points.Count; i++)
            {
                switch (axis)
                {
                    case Axis.Horizontal:
                        {
                            points[i].Position = new Vector2(Position.x + (points.Count - 1) * -0.5f * cell_size + i * cell_size, Position.y);

                            break;
                        }
                    case Axis.Vertical:
                        {
                            points[i].Position = new Vector2(Position.x, Position.y + (points.Count - 1) * -0.5f * cell_size + i * cell_size);
                            break;
                        }
                }
                //points[i].Zoom = Zoom;
            }
        }
    }
}