using MiXAssessment.Extentions;
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
        public double[][] InternalPointArray { get; private set; }

        public int[] InternalNodeArray { get; private set; }

        public KdBush(
            int dimensions,
            double minValue = double.NegativeInfinity,
            double maxValue = double.PositiveInfinity)
        {
            this.Dimensions = dimensions;
            MinValue = minValue;
            MaxValue = maxValue;

            InternalPointArray = Enumerable.Repeat(default(double[]), 0).ToArray();
            InternalNodeArray = Enumerable.Repeat(default(int), 0).ToArray();
        }

        public void LoadData(double[][] points,int[] nodes)
        {
            var elementCount = (int)Math.Pow(2, (int)(Math.Log(points.Length + InternalPointArray.Length) / Math.Log(2)) + 1);

            var temp = InternalNodeArray;
            InternalPointArray = Enumerable.Repeat(default(double[]), elementCount).ToArray();
            InternalNodeArray = Enumerable.Repeat(default(int), elementCount).ToArray();
            this.Count = points.Length;
            this.GenerateTree(0, 0, points, nodes);
        }

        private void GenerateTree(
            int index,
            int dim,
            IReadOnlyCollection<double[]> points,
            IEnumerable<int> nodes)
        {
            var zippedList = points.Zip(nodes, (p, n) => new { Point = p, Node = n });

            var sortedPoints = zippedList.OrderBy(z => z.Point[dim]).ToArray();

            var medianPoint = sortedPoints[points.Count / 2];
            var medianPointIdx = sortedPoints.Length / 2;

            this.InternalPointArray[index] = medianPoint.Point;
            this.InternalNodeArray[index] = medianPoint.Node;

            var leftPoints = new double[medianPointIdx][];
            var leftNodes = new int[medianPointIdx];

            Array.Copy(sortedPoints.Select(z => z.Point).ToArray(), leftPoints, leftPoints.Length);
            Array.Copy(sortedPoints.Select(z => z.Node).ToArray(), leftNodes, leftNodes.Length);

            var rightPoints = new double[sortedPoints.Length - (medianPointIdx + 1)][];
            var rightNodes = new int[sortedPoints.Length - (medianPointIdx + 1)];
            Array.Copy(
                sortedPoints.Select(z => z.Point).ToArray(),
                medianPointIdx + 1,
                rightPoints,
                0,
                rightPoints.Length);
            Array.Copy(sortedPoints.Select(z => z.Node).ToArray(), medianPointIdx + 1, rightNodes, 0, rightNodes.Length);

            var nextDim = (dim + 1) % Dimensions;

            if (leftPoints.Length <= 1)
            {
                if (leftPoints.Length == 1)
                {
                    this.InternalPointArray[LeftChildIndex(index)] = leftPoints[0];
                    this.InternalNodeArray[LeftChildIndex(index)] = leftNodes[0];
                }
            }
            else
            {
                this.GenerateTree(LeftChildIndex(index), nextDim, leftPoints, leftNodes);
            }

            if (rightPoints.Length <= 1)
            {
                if (rightPoints.Length == 1)
                {
                    this.InternalPointArray[RightChildIndex(index)] = rightPoints[0];
                    this.InternalNodeArray[RightChildIndex(index)] = rightNodes[0];
                }
            }
            else
            {
                this.GenerateTree(RightChildIndex(index), nextDim, rightPoints, rightNodes);
            }
        }

        public Tuple<double, int>[] NearestNeighbors(double[] point, int neighbors)
        {
            Tuple<double, int>[] result;

            var nearestNeighborList = new BoundedPriorityList<int, double>(neighbors, true);
            var rect = SearchArea.Infinite(2, MinValue, MaxValue);
            this.SearchForNearestNeighbors(0, point, rect, 0, nearestNeighborList, double.MaxValue);

            result = new Tuple<double, int>[nearestNeighborList.Count];

            for (int i = 0; i < nearestNeighborList.Count; ++i)
            {
                result[i] = new Tuple<double, int>(
                        nearestNeighborList.MaxPriority,
                        InternalNodeArray[nearestNeighborList[i]]);
            }
            return result;
        }

        public Tuple<double, int>[] NearestNeighbors2(double[] point, int neighbors)
        {
            Tuple<double, int>[] result;

            var nearestNeighborList = new BoundedPriorityList<Node, double>(neighbors, true);
            var rect = SearchArea.Infinite(2, MinValue, MaxValue);
            this.SearchForNearestNeighbors2(Root, point, rect, 0, nearestNeighborList, double.MaxValue);

            result = new Tuple<double, int>[nearestNeighborList.Count];

            for (int i = 0; i < nearestNeighborList.Count; ++i)
            {
                result[i] = new Tuple<double, int>(
                        nearestNeighborList.MaxPriority,
                        nearestNeighborList[i].Id);
            }
            return result;
        }

        public void Insert(double[][] points, int[] nodeIds)
        {
            int index = 0;
            foreach (var point in points)
            {
                Root = InsertRec(Root, point, 0, nodeIds[index]);
                index++;
            }
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int RightChildIndex(int index)
        {
            return (2 * index) + 2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int LeftChildIndex(int index)
        {
            return (2 * index) + 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ParentIndex(int index)
        {
            return (index - 1) / 2;
        }

        private void SearchForNearestNeighbors(
                int nodeIndex,
                double[] target,
                SearchArea rect,
                int dimension,
                BoundedPriorityList<int, double> nearestNeighbors,
                double maxSearchRadiusSquared)
        {
            if (this.InternalPointArray.Length <= nodeIndex || nodeIndex < 0
                || this.InternalPointArray[nodeIndex] == null)
            {
                return;
            }

            var dim = dimension % this.Dimensions;

            var leftRect = rect.Clone();
            leftRect.MaxPoint[dim] = this.InternalPointArray[nodeIndex][dim];

            var rightRect = rect.Clone();
            rightRect.MinPoint[dim] = this.InternalPointArray[nodeIndex][dim];

            var compare = target[dim].CompareTo(this.InternalPointArray[nodeIndex][dim]);

            var nearerRect = compare <= 0 ? leftRect : rightRect;
            var furtherRect = compare <= 0 ? rightRect : leftRect;

            var nearerNode = compare <= 0 ? LeftChildIndex(nodeIndex) : RightChildIndex(nodeIndex);
            var furtherNode = compare <= 0 ? RightChildIndex(nodeIndex) : LeftChildIndex(nodeIndex);

            this.SearchForNearestNeighbors(
                nearerNode,
                target,
                nearerRect,
                dimension + 1,
                nearestNeighbors,
                maxSearchRadiusSquared);

            var closestPointInFurtherRect = furtherRect.GetClosestPoint(target);
            var distanceSquaredToTarget = CalculateDistace(closestPointInFurtherRect, target);

            if (distanceSquaredToTarget.CompareTo(maxSearchRadiusSquared) <= 0)
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


            distanceSquaredToTarget = CalculateDistace(this.InternalPointArray[nodeIndex], target);
            if (distanceSquaredToTarget.CompareTo(maxSearchRadiusSquared) <= 0)
            {
                nearestNeighbors.Add(nodeIndex, distanceSquaredToTarget);
            }
        }

        private void SearchForNearestNeighbors2(
                Node node,
                double[] target,
                SearchArea rect,
                int dimension,
                BoundedPriorityList<Node, double> nearestNeighbors,
                double maxSearchRadiusSquared)
        {
            //if (this.InternalPointArray.Length <= nodeIndex || nodeIndex < 0
            //    || this.InternalPointArray[nodeIndex] == null)
            //{
            //    return;
            //}

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
                SearchForNearestNeighbors2(
                nearerNode,
                target,
                nearerRect,
                dimension + 1,
                nearestNeighbors,
                maxSearchRadiusSquared);

                var closestPointInFurtherRect = furtherRect.GetClosestPoint(target);
                var distanceSquaredToTarget = CalculateDistace(closestPointInFurtherRect, target);

                if (distanceSquaredToTarget.CompareTo(maxSearchRadiusSquared) <= 0 && furtherNode != null)
                {
                    if (nearestNeighbors.IsFull)
                    {
                        if (distanceSquaredToTarget.CompareTo(nearestNeighbors.MaxPriority) < 0)
                        {
                            this.SearchForNearestNeighbors2(
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
                        this.SearchForNearestNeighbors2(
                            furtherNode,
                            target,
                            furtherRect,
                            dimension + 1,
                            nearestNeighbors,
                            maxSearchRadiusSquared);
                    }
                }
            }


            var closestDistance = CalculateDistace(node.Point, target);
            if (closestDistance.CompareTo(maxSearchRadiusSquared) <= 0)
            {
                nearestNeighbors.Add(node, closestDistance);
            }
        }

        public double CalculateDistace(double[] point1, double[] point2)
        {
            double dist = 0f;
            for (int i = 0; i < point1.Length; i++)
            {
                dist += (point1[i] - point2[i]) * (point1[i] - point2[i]);
            }

            return dist;
        }
    }
}
