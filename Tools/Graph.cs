using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools
{
    public class Graph<T>
    {
        public List<Node<T>> Nodes { get; set; }

        public Graph()
        {
            Nodes = new List<Node<T>>();
        }
        public void SetEdgeWasFalse()
        {
            for (int i = 0; i < Nodes.Count; i++)
            {
                Nodes[i].SetEdgeWasFalse();
            }
        }//очистка
        public void SetNodeClear()
        {
            for (int i = 0; i < Nodes.Count; i++)
            {
                Nodes[i].Was = false;
                Nodes[i].last = null;
                Nodes[i].dist = -1;
            }
        }//очистка
        List<Node<T>> GetOddNodes()
        {
            List<Node<T>> odd = new List<Node<T>>();
            for (int i = 0; i < Nodes.Count; i++)
                if (Nodes[i].Edges.Count % 2 == 1)
                    odd.Add(Nodes[i]);
            return odd;
        } //нечётные вершины 
        List<Edge<T>> GetEdges(Node<T> start, Node<T> end, out int dist)
        {
            SetNodeClear();
            start.dist = 0;
            for (int i = 0; i < Nodes.Count; i++)
            {
                Node<T> min = null;
                for (int j = 0; j < Nodes.Count; j++)
                    if (!Nodes[j].Was && Nodes[j].dist >= 0 && (min == null || Nodes[j].dist < min.dist))
                        min = Nodes[j];
                if (min == end)
                    break;
                for (int j = 0; j < min.Edges.Count; j++)
                    if (min.Edges[j].Second.dist == -1 || min.Edges[j].Second.dist > min.dist + min.Edges[j].Dist)
                    {
                        min.Edges[j].Second.dist = min.dist + min.Edges[j].Dist;
                        min.Edges[j].Second.last = min;
                    }
                min.Was = true;
            }
            dist = end.dist;
            List<Edge<T>> WayEdges = new List<Edge<T>>();
            Node<T> Go = end;
            do
            {
                WayEdges.Add(new Edge<T>(Go, Go.last, -1));
                Go = Go.last;
            } while (Go != start);
            return WayEdges;
        } //путь
        public List<Node<T>> CPPTask() //основная задача
        {
            if (Nodes.Count != 0 && Connected())
            {
                AddEdges(ToDouble(GetOddNodes()));
                List<Node<T>> Cycle = GetEilerСycle();
                ClearEdges();
                return Cycle;
            }
            else
                throw new InvalidOperationException();
        }
        public bool Connected()
        {
            SetNodeClear();
            Queue<Node<T>> queue = new Queue<Node<T>>();
            queue.Enqueue(Nodes[0]);
            int Count = 1;
            while (queue.Count != 0)
            {
                Node<T> node = queue.Dequeue();
                node.Was = true;
                
                for (int i = 0; i < node.Edges.Count; i++)
                    if (!node.Edges[i].Second.Was)
                    {
                        queue.Enqueue(node.Edges[i].Second);
                        Count++;
                        node.Edges[i].Second.Was = true;
                    }
            }
            return Count == Nodes.Count;
        }//проверка связанности
        void AddEdges(List<Edge<T>> edges)
        {
            for (int i = 0; i < edges.Count; i++)
            {
                int t = 0;
                int j = 0;
                while (t != 2)
                {
                    if (Nodes[j] == edges[i].First)
                    {
                        Nodes[j].Edges.Add(edges[i]);
                        t++;
                    }
                    if (Nodes[j] == edges[i].Second)
                    {
                        Nodes[j].Edges.Add(new Edge<T>(edges[i].Second, edges[i].First, -1));
                        t++;
                    }
                    j++;
                }
            }
        }//добавление пар граней
        void ClearEdges()
        {
            for (int i = 0; i < Nodes.Count; i++)
                for (int j = 0; j < Nodes[i].Edges.Count;)
                {
                    if (Nodes[i].Edges[j].Dist == -1)
                        Nodes[i].Edges.RemoveAt(j);
                    else
                        j++;
                }
        } //удаление ненужных граней

        //нахождение путей между нечётными
        List<Edge<T>> ToDouble(List<Node<T>> nodes)
        {
            dist = new int[nodes.Count, nodes.Count];
            List<Edge<T>>[,] edges = new List<Edge<T>>[nodes.Count, nodes.Count];
            for (int i = 0; i < nodes.Count - 1; i++)
            {
                for (int j = i + 1; j < nodes.Count; j++)
                {
                    if (i != j)
                    {
                        edges[i, j] = GetEdges(nodes[i], nodes[j], out dist[i, j]);
                        edges[j, i] = edges[i, j];
                        dist[j, i] = dist[i, j];
                    }
                }
            }
            getMin(nodes.Count);
            List<Edge<T>> DoubleEdges = new List<Edge<T>>();
            for (int i = 0; i < nodes.Count/2; i++)
            {
                DoubleEdges.InsertRange(DoubleEdges.Count, edges[min[2 * i], min[2 * i + 1]]);
            }
            return DoubleEdges;
        } 
        int[,] dist;
        List<int> min;
        int minDist;
        List<int> getMin(int n, List<int> was = null)
        {
            if (was == null)
            {
                was = new List<int>();
                min = null;
                minDist = 0;
            }
            if(was.Count == n)
            {
                int T = 0;
                for (int i = 0; i < n/2; i++)
                    T += dist[was[2 * i],was[ 2 * i + 1]];
                if(min == null || minDist > T)
                {
                    min = new List<int>(was);
                    minDist = T;
                }
                return min;
            }
            int first = 0;
            while (was.Contains(first))
                first++;
            was.Add(first);
            for (int i = first+1; i < n; i++)
            {
                if(!was.Contains(i))
                {
                    was.Add(i);
                    getMin(n, was);
                    was.Remove(was.Count - 1);
                }
            }
            was.Remove(was.Count - 1);
            return min;
        }


        List<Node<T>> GetEilerСycle()
        {
            List<Node<T>> Cycle = new List<Node<T>>();
            Stack<Node<T>> stack = new Stack<Node<T>>();
            stack.Push(Nodes[0]);
            SetEdgeWasFalse();
            while (stack.Count!=0)
            {
                Node<T> node = stack.Peek();
                bool t = true;
                for (int i = 0; i < node.Edges.Count; i++)
                    if(!node.Edges[i].Was)
                    {
                        t = false;
                        stack.Push(node.Edges[i].Second);
                        node.Edges[i].Was = true;
                        break;
                    }
                if (t)
                    Cycle.Add(stack.Pop());
            }
            return Cycle;
        }
    }
}
