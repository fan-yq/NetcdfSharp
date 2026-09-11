// NetcdfSharp.ucar.nc2.@internal.dataset.CoordSystemBuilder minimal shim.
// Coordinate-system building is dataset-layer functionality; only the small static
// helpers used by HdfEos are provided.
using System;
using System.Collections.Generic;
using NetcdfSharp.ucar.ma2;

namespace NetcdfSharp.ucar.nc2.@internal.dataset
{
    /// <summary>Coordinate system builder. Minimal static helpers.</summary>
    public static class CoordSystemBuilder
    {
        /// <summary>Count the number of distinct dimensions in the given axes.</summary>
        public static int countDomainSize(params NetcdfSharp.ucar.nc2.Variable.Builder[] axes)
        {
            var domain = new HashSet<NetcdfSharp.ucar.nc2.Dimension>();
            if (axes == null) return 0;
            foreach (var axis in axes)
            {
                if (axis == null) continue;
                var dims = axis.getDimensions();
                if (dims != null)
                {
                    foreach (var d in dims) domain.Add(d);
                }
            }
            return domain.Count;
        }

        /// <summary>Does this variable look like a coordinate variable (1D with same-named dimension).</summary>
        public static bool isCoordinateVariable(NetcdfSharp.ucar.nc2.Variable.Builder vb)
        {
            if (vb == null) return false;
            if (vb.dataType == DataType.STRUCTURE || vb.getParentStructureBuilder() != null)
                return false;
            int rank = vb.getRank();
            if (rank == 1)
            {
                string firstd = vb.getFirstDimensionName();
                if (vb.shortName != null && vb.shortName.Equals(firstd, StringComparison.Ordinal))
                    return true;
            }
            return false;
        }
    }
}
