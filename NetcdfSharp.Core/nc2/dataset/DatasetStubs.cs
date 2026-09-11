// NetcdfSharp.ucar.nc2.dataset minimal shims.
// The dataset enhancement layer is not translated. These types exist so that the
// translated filter classes and CDMNode compile; the enhancement features are not activated.
using System;
using System.Collections.Generic;

namespace NetcdfSharp.ucar.nc2.dataset
{
    /// <summary>Stub for NetcdfSharp.ucar.nc2.dataset.NetcdfDataset. Enhancement enum + open() entry points not needed for core reading.
    /// Non-static so it can be used as a return type by the (unported) dataset-layer callers.</summary>
    public class NetcdfDataset
    {
        /// <summary>Enhancement flags used by the dataset enhancement layer.</summary>
        public enum Enhance
        {
            ApplyScaleOffset,
            ConvertUnsigned,
            ConvertEnums,
            ConvertMissing,
            CoordSystems,
            IncompleteCoordSystems,
            ConvertUnsignedShort,
            ConvertUnsignedByte,
            ConvertUnsignedInt,
            ScaleMissingDeferred,
            AlwaysScaleMissing,
            AlwaysMissing,
            CreateVlenData,
            EveryThing,
        }

        /// <summary>Default enhancement mode used by the (unported) enhancement layer.</summary>
        public static Enhance getDefaultEnhanceMode()
        {
            return Enhance.EveryThing;
        }
    }

    /// <summary>Stub for NetcdfSharp.ucar.nc2.dataset.NetcdfDatasets. Enhancement is not ported; enhance() returns null.</summary>
    public static class NetcdfDatasets
    {
        public static object enhance(NetcdfSharp.ucar.nc2.NetcdfFile ncfile, NetcdfDataset.Enhance enhanceMode, object something)
        {
            return null;
        }
    }

    /// <summary>Stub for NetcdfSharp.ucar.nc2.dataset.VariableDS. Not used by core reading.</summary>
    public class VariableDS : NetcdfSharp.ucar.nc2.Variable
    {
        private string unitsString;

        public VariableDS(NetcdfSharp.ucar.nc2.Group parentGroup, NetcdfSharp.ucar.nc2.Variable orgVar, bool isCaching)
            : base(orgVar)
        {
            setParentGroup(parentGroup);
        }

        public VariableDS(NetcdfSharp.ucar.nc2.Group parentGroup, NetcdfSharp.ucar.nc2.Variable orgVar)
            : this(parentGroup, orgVar, true)
        {
        }

        public override string UnitsString
        {
            get => unitsString;
        }

        public virtual void setUnitsString(string units)
        {
            unitsString = units;
        }

        public virtual NetcdfSharp.ucar.nc2.Variable OriginalVariable => null;

        public virtual NetcdfSharp.ucar.ma2.Signedness Signedness => DataType.getSignedness();
        public virtual NetcdfSharp.ucar.ma2.DataType OriginalDataType => DataType;
        public virtual NetcdfSharp.ucar.ma2.DataType UnsignedConversionType => DataType;

        public virtual object convertUnsigned(object value) => value;
        public virtual object convertUnsigned(object value, NetcdfSharp.ucar.ma2.DataType dataType) => value;

        public virtual bool fillValueIsMissing() => false;
        public virtual bool invalidDataIsMissing() => false;
        public virtual bool missingDataIsMissing() => false;
    }

    /// <summary>Stub for NetcdfSharp.ucar.nc2.dataset.StructureDS. Not used by core reading.</summary>
    public class StructureDS : NetcdfSharp.ucar.nc2.Structure
    {
        public StructureDS() : base((NetcdfSharp.ucar.nc2.NetcdfFile) null, null, null, "") { }
        public virtual NetcdfSharp.ucar.nc2.Variable OriginalVariable => null;
    }

    /// <summary>Stub for NetcdfSharp.ucar.nc2.dataset.CoordinateSystem / CoordinateAxis / CoordinateTransform (dataset layer, not translated).</summary>
    public class CoordinateSystem { }
    public class CoordinateAxis { }
    public class CoordinateTransform { }
}
