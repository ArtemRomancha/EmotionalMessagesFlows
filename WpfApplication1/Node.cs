using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication1
{
    public class Node
    {
        public List<Node> Edges { get; private set; } //Узлы
        public List<int> EdgeWeight { get; private set; } //Веса узлов
        public List<TimeSpan> TimeMessage { get; private set; } //Время сообщения 
        public List<Node> TimeEdges { get; private set; } //Время узла
        public int No { get; private set; } //Номер

        public Node(int no)
        {
            No = no;
            Edges = new List<Node>();
            EdgeWeight = new List<int>();
            TimeMessage = new List<TimeSpan>();
            TimeEdges = new List<Node>();
        }
        public void SendMessage(Node recipient)
        {
            recipient.Receive(this);
            bool found = false;
            for (int i = 0; i < Edges.Count; i++)
            {
                if (Edges[i].Equals(recipient))
                {
                    found = true;
                    EdgeWeight[i] += 1;
                }
            }

            if (!found)
            {
                Edges.Add(recipient);
                EdgeWeight.Add(1);
            }
        }
        protected void Receive(Node sender)
        {
            bool found = false;
            for (int i = 0; i < Edges.Count; i++)
            {
                if (Edges[i].Equals(sender))
                {
                    found = true;
                    EdgeWeight[i] += 1;
                }
            }

            if(!found)
            {
                Edges.Add(sender);
                EdgeWeight.Add(1);
            }
        }
        protected void ReceiveInformaion(Node sender, TimeSpan time)
        {
            TimeEdges.Add(sender);
            TimeMessage.Add(time);
        }
        public void SendInformalMessage(Node recipient, TimeSpan time)
        {
            TimeEdges.Add(recipient);
            TimeMessage.Add(time);
            this.SendMessage(recipient);
            recipient.ReceiveInformaion(this, time);
        }
        public int Degree()
        {
            return this.Edges.Count;
        }
        public void Utilize(int limit, TimeSpan Time, out string log)
        {
            log = "";
            for (int i = 0; i < TimeMessage.Count; i++) 
            {
                if (Time.Subtract(TimeMessage[i]).TotalSeconds > limit)
                {
                    for (int f = 0; f < Edges.Count; f++) 
                    {
                        if(TimeEdges[i].Equals(Edges[f]))
                        {
                            if (EdgeWeight[f] > 1)
                            {
                                log += "Edge #" + No + " & Edge # " + Edges[f].No + " get low\n";
                                EdgeWeight[f] -= 1;
                                TimeEdges.RemoveAt(i);
                                TimeMessage.RemoveAt(i);                                
                                break;
                            }
                            else
                            {
                                log += "Edge #" + No + " & Edge # " + Edges[f].No + " delete\n";
                                Edges.RemoveAt(f);
                                EdgeWeight.RemoveAt(f);
                                TimeEdges.RemoveAt(i);
                                TimeMessage.RemoveAt(i);                                
                                break;
                            }
                        }
                    }
                    --i;
                }
            }
        }
    }
}