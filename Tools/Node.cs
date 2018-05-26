using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools
{
    public class Node<T>
    {
        public T Value { get; set; }
        public List<Edge<T>> Edges { get; set; }
        public void AddEdgeTo(Node<T> second, int value)
        {
            if (second == this)
                return;
            for (int i = 0; i < Edges.Count; i++)
                if (Edges[i].Second == second)
                    return;
            Edges.Add(new Edge<T>(this, second,value));
            second.Edges.Add(new Edge<T>(second, this, value));
        }
        public bool Was { get; set; }
        public void SetEdgeWasFalse()
        {
            for (int i = 0; i < Edges.Count; i++)
            {
                Edges[i].Was = false;
            }
        }
        public void RemoveEdgeTo(Node<T> second)
        {
            for (int i = 0; i < Edges.Count; i++)
            {
                if (Edges[i].Second == second)
                {
                    Edges.RemoveAt(i);
                    break;
                }
            }
            for (int i = 0; i < second.Edges.Count; i++)
            {
                if(second.Edges[i].Second == this)
                {
                    second.Edges.RemoveAt(i);
                    break;
                }
            }
        }
        public Node<T> last;
        public int dist = -1;

        public Node(T value)
        {
            Value = value;
            Edges = new List<Edge<T>>();
            Was = false;
            last = null;
        }
        public void RemoveEdges()
        {
            for (int i = 0; i < Edges.Count;)
            {
                Edges[i].Second.RemoveEdgeTo(this);
            }
        }
    }
}
