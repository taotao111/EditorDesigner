using UnityEngine;
using System;
namespace Designer.Runtime {
    /// <summary>
    /// 节点连接线
    /// </summary>
    [System.Serializable]
    public class NodeConnection
    {
        public NodeConnectionPoint beginPoint;
        public NodeConnectionPoint endPoint;
        public NodeData Begin;
        public NodeData End;
        [NonSerialized]
        public bool selected;
        public NodeConnection(NodeData begin, NodeData end)
        {
            Begin = begin;
            End = end;
            beginPoint = new NodeConnectionPoint();
            endPoint = new NodeConnectionPoint();
            AddConnectionToNode(NodePosition.Left, NodePosition.Right);
        }
        public void AddConnectionToNode(NodePosition beginPos, NodePosition endPos)
        {
            Begin.AddNodeConnection(beginPos, beginPoint);
            End.AddNodeConnection(endPos, endPoint);
        }
        public void UpdatePointPosition()
        {
            if (Begin.Position.y == End.Position.y)
            {
                if (beginPoint.Position.x < endPoint.Position.x)
                {
                    if (beginPoint.nodePosition != NodePosition.Right)
                    {
                        Begin.RemoveNodeConnection(beginPoint.nodePosition, beginPoint);
                        beginPoint.nodePosition = NodePosition.Right;
                        Begin.AddNodeConnection(beginPoint.nodePosition, beginPoint);
                    }

                    if (endPoint.nodePosition != NodePosition.Left)
                    {
                        End.RemoveNodeConnection(endPoint.nodePosition, endPoint);
                        endPoint.nodePosition = NodePosition.Left;
                        End.AddNodeConnection(endPoint.nodePosition, endPoint);
                    }
                }
            }
            else if (Begin.Position.y < End.Position.y)
            {
                if (beginPoint.nodePosition != NodePosition.Bottom)
                {
                    Begin.RemoveNodeConnection(beginPoint.nodePosition, beginPoint);
                    beginPoint.nodePosition = NodePosition.Bottom;
                    Begin.AddNodeConnection(beginPoint.nodePosition, beginPoint);
                }

                if (endPoint.nodePosition != NodePosition.Top)
                {
                    End.RemoveNodeConnection(endPoint.nodePosition, endPoint);
                    endPoint.nodePosition = NodePosition.Top;
                    End.AddNodeConnection(endPoint.nodePosition, endPoint);
                }
            }
            else if (Begin.Position.y > End.Position.y)
            {
                if (beginPoint.nodePosition != NodePosition.Top)
                {
                    Begin.RemoveNodeConnection(beginPoint.nodePosition, beginPoint);
                    beginPoint.nodePosition = NodePosition.Top;
                    Begin.AddNodeConnection(beginPoint.nodePosition, beginPoint);
                }

                if (endPoint.nodePosition != NodePosition.Bottom)
                {
                    End.RemoveNodeConnection(endPoint.nodePosition, endPoint);
                    endPoint.nodePosition = NodePosition.Bottom;
                    End.AddNodeConnection(endPoint.nodePosition, endPoint);
                }
            }
        }
        public void UpdatePosition()
        {
            Begin.UpdateNodeCollection();
            End.UpdateNodeCollection();
            UpdatePointPosition();
        }
        public void ReplacePoint(NodeData nd)
        {
            if (Begin == nd || End == nd)
            {
                UpdatePosition();
                return;
            }
            if (beginPoint.selected) 
            {
                Begin.RemoveNodeConnection(beginPoint);
                Begin = nd;
                Begin.AddNodeConnection(NodePosition.Top, beginPoint);
            }
            if (endPoint.selected)
            {
                End.RemoveNodeConnection(endPoint);
                End = nd;
                End.AddNodeConnection(NodePosition.Top, endPoint);
            }
            UpdatePosition();
        }
    }
    /// <summary>
    /// 节点连接线点
    /// </summary>
    [System.Serializable]
    public class NodeConnectionPoint
    {
        public int id = -1;
        private int cell = 10;
        [HideInInspector]
        [SerializeField]
        protected Vector2 m_position;
        [HideInInspector]
        [SerializeField]
        protected Rect m_rect;
        //[HideInInspector]
        //[SerializeField]
        //protected float m_Zoom = 1;
        [HideInInspector]
        public NodePosition nodePosition = NodePosition.Top;
        [NonSerialized]
        public bool selected = false;
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
        //public float Zoom
        //{
        //    set
        //    {
        //        m_Zoom = value;

        //        UpdateRect();
        //    }
        //}
        public Rect Rect
        {
            get
            {
                return m_rect;
            }
        }

        public void UpdateRect()
        {
            switch (nodePosition)
            {
                case NodePosition.Top:
                    {
                        m_position.y -= 0.75f * cell;
                        break;
                    }
                case NodePosition.Bottom:
                    {
                        m_position.y += 0.75f * cell;
                        break;
                    }
                case NodePosition.Left:
                    {
                        m_position.x -= 0.75f * cell;
                        break;
                    }
                case NodePosition.Right:
                    {
                        m_position.x += 0.75f * cell;
                        break;
                    }
            }

            m_rect.width = cell;
            m_rect.height = cell;
            m_rect.x = m_position.x - m_rect.width * 0.5f;
            m_rect.y = m_position.y - m_rect.height * 0.5f;
            //m_rect.x = m_position.x - cell * 0.5f;
            //m_rect.y = m_position.y - cell * 0.5f;
        }
    }
}