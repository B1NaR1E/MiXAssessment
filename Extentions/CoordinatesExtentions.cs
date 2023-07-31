using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiXAssessment.Extentions
{
    internal static class CoordinatesExtentions
    {
        public static double ToRadians(this double val)
        {
            return (val * Math.PI / 180);
        }

        public static double ToDegrees(this double val)
        {
            return (val / Math.PI * 180);
        }
    }
}
