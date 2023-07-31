namespace MiXAssessment
{
    internal class SearchArea
    {
        public double[] MinPoint { get; private set; }
        public double[] MaxPoint { get; private set; }

        public static SearchArea Infinite(int dimensions, double positiveInfinity, double negativeInfinity)
        {
            var rect = new SearchArea();

            rect.MinPoint = new double[dimensions];
            rect.MaxPoint = new double[dimensions];

            for (var dimension = 0; dimension < dimensions; dimension++)
            {
                rect.MinPoint[dimension] = negativeInfinity;
                rect.MaxPoint[dimension] = positiveInfinity;
            }

            return rect;
        }

        public SearchArea Clone()
        {
            var rect = new SearchArea();
            rect.MinPoint = this.MinPoint;
            rect.MaxPoint = this.MaxPoint;
            return rect;
        }

        public double[] GetClosestPoint(double[] toPoint)
        {
            var closest = new double[toPoint.Length];

            for (var dimension = 0; dimension < toPoint.Length; dimension++)
            {
                if (this.MinPoint[dimension].CompareTo(toPoint[dimension]) > 0)
                {
                    closest[dimension] = this.MinPoint[dimension];
                }
                else if (this.MaxPoint[dimension].CompareTo(toPoint[dimension]) < 0)
                {
                    closest[dimension] = this.MaxPoint[dimension];
                }
                else
                {
                    closest[dimension] = toPoint[dimension];
                }
            }

            return closest;
        }
    }
}
