using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiXAssessment
{
    internal class Node
    {
        public Node(double[] point, int NodeId)
        {
            Id = NodeId;
            Point = point;
        }
        public int Id { get; set; }
        public Node LeftNode { get; set; }
        public Node RightNode { get; set; }

        public double[] Point { get; set; }
    }
}
