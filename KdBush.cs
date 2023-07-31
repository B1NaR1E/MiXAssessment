using MiXAssessment.Extentions;
using System;
using System.Runtime.CompilerServices;

namespace MiXAssessment
{
    internal class KdBush
    {
        public Node Root { get; set; }

        public int Count { get; private set; }

        private int Dimensions { get; set; }

        private double MaxValue { get; }

        private double MinValue { get; }

        public KdBush(
            int dimensions,
            double minValue = double.NegativeInfinity,
            double maxValue = double.PositiveInfinity)
        {
            this.Dimensions = dimensions;
            MinValue = minValue;
            MaxValue = maxValue;
        }

        public Tuple<double, int>[] NearestNeighbors(double[] point, int neighbors)
        {
            Tuple<double, int>[] result;

            var nearestNeighborList = new BoundedPriorityList<Node, double>(neighbors, true);
            var rect = SearchArea.Infinite(2, MinValue, MaxValue);
            this.SearchForNearestNeighbors(Root, point, rect, 0, nearestNeighborList, double.MaxValue);

            result = new Tuple<double, int>[nearestNeighborList.Count];

            for (int i = 0; i < nearestNeighborList.Count; ++i)
            {
                result[i] = new Tuple<double, int>(
                        nearestNeighborList.MaxPriority,
                        nearestNeighborList[i].Id);
            }
            return result;
        }

        public void InsertRecord(double[] point, int nodeId)
        {
            Root = InsertRec(Root, point, 0, nodeId);
        }

        private Node InsertRec(Node nood, double[] point, int depth, int nodeId)
        {
            if (nood == null)
            {
                ++Count;
                return new Node(point, nodeId);
            }

            int cd = depth % Dimensions;

            if (point[cd] < nood.Point[cd])
            {
                nood.LeftNode = InsertRec(nood.LeftNode, point, depth + 1, nodeId);
            }
            else
            {
                nood.RightNode = InsertRec(nood.RightNode, point, depth + 1, nodeId);
            }

            return nood;
        }

        private void SearchForNearestNeighbors(
                Node node,
                double[] target,
                SearchArea rect,
                int dimension,
                BoundedPriorityList<Node, double> nearestNeighbors,
                double maxSearchRadiusSquared)
        {

            if(node == null)
                return;

            var dim = dimension % this.Dimensions;

            var leftRect = rect.Clone();
            leftRect.MaxPoint[dim] = node.Point[dim];

            var rightRect = rect.Clone();
            rightRect.MinPoint[dim] = node.Point[dim];

            var compare = target[dim].CompareTo(node.Point[dim]);

            var nearerRect = compare <= 0 ? leftRect : rightRect;
            var furtherRect = compare <= 0 ? rightRect : leftRect;

            var nearerNode = compare <= 0 ? node.LeftNode : node.RightNode;
            var furtherNode = compare <= 0 ? node.RightNode : node.LeftNode;

            if (nearerNode != null)
            {
                SearchForNearestNeighbors(
                nearerNode,
                target,
                nearerRect,
                dimension + 1,
                nearestNeighbors,
                maxSearchRadiusSquared);

                var closestPointInFurtherRect = furtherRect.GetClosestPoint(target);
                var distanceSquaredToTarget = CalculateDistance(closestPointInFurtherRect, target);

                if (distanceSquaredToTarget.CompareTo(maxSearchRadiusSquared) <= 0 && furtherNode != null)
                {
                    if (nearestNeighbors.IsFull)
                    {
                        if (distanceSquaredToTarget.CompareTo(nearestNeighbors.MaxPriority) < 0)
                        {
                            this.SearchForNearestNeighbors(
                                furtherNode,
                                target,
                                furtherRect,
                                dimension + 1,
                                nearestNeighbors,
                                maxSearchRadiusSquared);
                        }
                    }
                    else
                    {
                        this.SearchForNearestNeighbors(
                            furtherNode,
                            target,
                            furtherRect,
                            dimension + 1,
                            nearestNeighbors,
                            maxSearchRadiusSquared);
                    }
                }
            }


            var closestDistance = Math.Round(CalculateDistance(node.Point, target), 2);
            if (closestDistance.CompareTo(maxSearchRadiusSquared) <= 0)
            {
                nearestNeighbors.Add(node, closestDistance);
            }
        }

        private double CalculateDistance(double[] point1, double[] point2, char unit = ' ')
        {
            if ((point1[1] == point2[1]) && (point1[0] == point2[0]))
            {
                return 0;
            }
            else
            {
                double theta = point1[0] - point2[0];
                double dist = Math.Sin(point1[1].ToRadians()) * Math.Sin(point2[1].ToRadians()) + Math.Cos(point1[1].ToRadians()) * Math.Cos(point2[1].ToRadians()) * Math.Cos(theta.ToRadians());
                dist = Math.Acos(dist);
                dist = dist.ToDegrees();
                dist = dist * 60 * 1.1515;
                if (unit == 'K')
                {
                    dist = dist * 1.609344;
                }
                else if (unit == 'N')
                {
                    dist = dist * 0.8684;
                }
                return (dist);
            }
        }
    }
}
