// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Collections.Generic;

/*
 * Copyright (c) 1998-2018 University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */

namespace NetcdfSharp.ucar.nc2.ft.point.bufr
{
	using BufrIosp2 = NetcdfSharp.ucar.nc2.iosp.bufr.BufrIosp2;
	using CalendarDate = NetcdfSharp.ucar.nc2.time.CalendarDate;
	using DataDescriptor = NetcdfSharp.ucar.nc2.iosp.bufr.DataDescriptor;
	using DataType = NetcdfSharp.ucar.ma2.DataType;
	using Message = NetcdfSharp.ucar.nc2.iosp.bufr.Message;
	using NcAttribute = NetcdfSharp.ucar.nc2.NcAttribute;
	using Structure = NetcdfSharp.ucar.nc2.Structure;
	using StructureData = NetcdfSharp.ucar.ma2.StructureData;
	using StructureMembers = NetcdfSharp.ucar.ma2.StructureMembers;
	using Variable = NetcdfSharp.ucar.nc2.Variable;

	/// <summary>
	/// Extract standard fields from BUFR
	/// 
	/// @author caron
	/// @since 8/7/13
	/// </summary>
	public class StandardFields
	{
		private static int nflds = 50;
		private static IDictionary<BufrCdmIndexProto.FldType, IList<string>> type2Flds = new Dictionary<BufrCdmIndexProto.FldType, IList<string>>(2 * nflds);
		private static IDictionary<string, TypeAndOrder> fld2type = new Dictionary<string, TypeAndOrder>(2 * nflds);
		private static IDictionary<int, IDictionary<string, BufrCdmIndexProto.FldType>> locals = new Dictionary<int, IDictionary<string, BufrCdmIndexProto.FldType>>(10);

		static StandardFields()
		{
			// first choice
			addField("0-1-1", BufrCdmIndexProto.FldType.wmoBlock);
			addField("0-1-2", BufrCdmIndexProto.FldType.wmoId);
			addField("0-1-18", BufrCdmIndexProto.FldType.stationId);
			addField("0-4-1", BufrCdmIndexProto.FldType.year);
			addField("0-4-2", BufrCdmIndexProto.FldType.month);
			addField("0-4-3", BufrCdmIndexProto.FldType.day);
			addField("0-4-4", BufrCdmIndexProto.FldType.hour);
			addField("0-4-5", BufrCdmIndexProto.FldType.minute);
			addField("0-4-6", BufrCdmIndexProto.FldType.sec);
			addField("0-5-1", BufrCdmIndexProto.FldType.lat);
			addField("0-6-1", BufrCdmIndexProto.FldType.lon);
			addField("0-7-30", BufrCdmIndexProto.FldType.heightOfStation);

			// second choice
			addField("0-1-15", BufrCdmIndexProto.FldType.stationId);
			addField("0-1-19", BufrCdmIndexProto.FldType.stationId);
			addField("0-4-7", BufrCdmIndexProto.FldType.sec);
			addField("0-4-43", BufrCdmIndexProto.FldType.doy);
			addField("0-5-2", BufrCdmIndexProto.FldType.lat);
			addField("0-6-2", BufrCdmIndexProto.FldType.lon);
			addField("0-7-1", BufrCdmIndexProto.FldType.heightOfStation);

			// third choice
			addField("0-1-62", BufrCdmIndexProto.FldType.stationId);
			addField("0-1-63", BufrCdmIndexProto.FldType.stationId);
			addField("0-7-2", BufrCdmIndexProto.FldType.height);
			addField("0-7-10", BufrCdmIndexProto.FldType.height);
			addField("0-7-7", BufrCdmIndexProto.FldType.height);

			// 4th choice LOOK
			addField("0-1-5", BufrCdmIndexProto.FldType.stationId);
			addField("0-1-6", BufrCdmIndexProto.FldType.stationId);
			// addField("0-1-7", BufrCdmIndexProto.FldType.stationId); satellite id
			addField("0-1-8", BufrCdmIndexProto.FldType.stationId);
			addField("0-1-10", BufrCdmIndexProto.FldType.stationId);
			addField("0-1-11", BufrCdmIndexProto.FldType.stationId);
			addField("0-7-6", BufrCdmIndexProto.FldType.heightAboveStation);
			addField("0-7-7", BufrCdmIndexProto.FldType.heightAboveStation);

			// locals
			/*
			 * Map<String, BufrCdmIndexProto.FldType> ncep = new HashMap<String, BufrCdmIndexProto.FldType>(10);
			 * ncep.put("0-1-198", BufrCdmIndexProto.FldType.stationId);
			 * locals.put(7, ncep);
			 */

			IDictionary<string, BufrCdmIndexProto.FldType> uu = new Dictionary<string, BufrCdmIndexProto.FldType>(10);
			uu["0-1-194"] = BufrCdmIndexProto.FldType.stationId;
			locals[59] = uu;
		}

		internal class TypeAndOrder
		{
			internal BufrCdmIndexProto.FldType type;
			internal int order;

			internal TypeAndOrder(BufrCdmIndexProto.FldType type, int order)
			{
				this.type = type;
				this.order = order;
			}
		}

		private static void addField(string fld, BufrCdmIndexProto.FldType type)
		{
			if (!type2Flds.TryGetValue(type, out IList<string> list))
			{
				list = new List<string>();
				type2Flds[type] = list;
			}
			list.Add(fld); // keep in order

			TypeAndOrder tao = new TypeAndOrder(type, list.Count - 1);
			fld2type[fld] = tao;
		}

		//////////////////////////////////////////////////////

		private static TypeAndOrder findTao(int center, string key)
		{
			locals.TryGetValue(center, out IDictionary<string, BufrCdmIndexProto.FldType> local);
			if (local != null && local.TryGetValue(key, out BufrCdmIndexProto.FldType result))
			{
				return new TypeAndOrder(result, -1);
			}

			return fld2type.TryGetValue(key, out TypeAndOrder tao) ? tao : null;
		}

		public static BufrCdmIndexProto.FldType? findField(int center, string key)
		{
			locals.TryGetValue(center, out IDictionary<string, BufrCdmIndexProto.FldType> local);
			if (local != null && local.TryGetValue(key, out BufrCdmIndexProto.FldType result))
			{
				return result;
			}

			return findStandardField(key);
		}

		public static BufrCdmIndexProto.FldType? findStandardField(string key)
		{
			if (fld2type.TryGetValue(key, out TypeAndOrder tao))
			{
				return tao.type;
			}
			return null;
		}

		/////////////////////////////////////////////////////////////////////////////////////////

		public static StandardFieldsFromMessage extract(Message m)
		{
			StandardFieldsFromMessage result = new StandardFieldsFromMessage();
			extract(m.ids.CenterId, m.RootDataDescriptor, result);
			return result;
		}

		private static void extract(int center, DataDescriptor dds, StandardFieldsFromMessage extract)
		{
			foreach (DataDescriptor subdds in dds.SubKeys)
			{
				extract.match(center, subdds);

				if (subdds.SubKeys != null)
				{
					StandardFields.extract(center, subdds, extract);
				}
			}
		}

		public class StandardFieldsFromMessage
		{

			internal IDictionary<BufrCdmIndexProto.FldType, IList<DataDescriptor>> typeMap = new SortedDictionary<BufrCdmIndexProto.FldType, IList<DataDescriptor>>();

			internal virtual void match(int center, DataDescriptor dds)
			{
				string name = dds.FxyName;
				BufrCdmIndexProto.FldType? type = findField(center, name);
				if (type == null)
				{
					return;
				}

				// got a match
				BufrCdmIndexProto.FldType t = type.Value;
				if (!typeMap.TryGetValue(t, out IList<DataDescriptor> list))
				{
					list = new List<DataDescriptor>(3);
					typeMap[t] = list;
				}
				list.Add(dds);
			}

			public virtual bool hasStation()
			{
				if (typeMap[BufrCdmIndexProto.FldType.lat] == null)
				{
					return false;
				}
				if (typeMap[BufrCdmIndexProto.FldType.lon] == null)
				{
					return false;
				}
				if (typeMap[BufrCdmIndexProto.FldType.stationId] != null)
				{
					return true;
				}
				return typeMap[BufrCdmIndexProto.FldType.wmoId] != null;
			}

			public virtual bool hasTime()
			{
				if (typeMap[BufrCdmIndexProto.FldType.year] == null)
				{
					return false;
				}
				if (typeMap[BufrCdmIndexProto.FldType.month] == null)
				{
					return false;
				}
				return typeMap[BufrCdmIndexProto.FldType.day] != null || typeMap[BufrCdmIndexProto.FldType.doy] != null;
				// if (typeMap.get(BufrCdmIndexProto.FldType.hour) == null) return false; // LOOK could assume 0:0 ??
				// if (typeMap.get(BufrCdmIndexProto.FldType.minute) == null) return false;
			}

			public override string ToString()
			{
				using (Formatter f = new Formatter())
				{
					foreach (BufrCdmIndexProto.FldType type in typeMap.Keys)
					{
						f.format(" %20s: ", type);
						IList<DataDescriptor> list = typeMap[type];
						foreach (DataDescriptor dds in list)
						{
							f.format(" %s", dds.Name);
							if (!string.ReferenceEquals(dds.Desc, null))
							{
								f.format("=%s", dds.Desc);
							}
							f.format(",");
						}
						f.format(" %n");
					}
					return f.ToString();
				}
			}
		}

		public class StandardFieldsFromStructure
		{

			internal class Field
			{
				internal TypeAndOrder tao;
				internal string memberName;
				internal string valueS;
				internal int value = -1;
				internal double valueD = double.NaN;
				internal double scale = 1.0;
				internal double offset;
				internal bool hasScale;

				internal Field(TypeAndOrder tao, Variable v)
				{
					this.tao = tao;
					this.memberName = v.ShortName;
					NcAttribute att = v.attributes().findAttribute("scale_factor");
					if (att != null && !att.String)
					{
						scale = Convert.ToDouble(att.NumericValue);
						hasScale = true;
					}
					att = v.attributes().findAttribute("add_offset");
					if (att != null && !att.String)
					{
						offset = Convert.ToDouble(att.NumericValue);
						hasScale = true;
					}
				}
			}

			internal IDictionary<BufrCdmIndexProto.FldType, Field> map = new Dictionary<BufrCdmIndexProto.FldType, Field>();

			public StandardFieldsFromStructure(int center, Structure obs)
			{
				// run through all available fields - LOOK we are not recursing into sub sequences
				foreach (Variable v in obs.Variables)
				{
					NcAttribute att = v.attributes().findAttribute(BufrIosp2.fxyAttName);
					if (att == null)
					{
						continue;
					}
					string key = att.StringValue;
					TypeAndOrder tao = findTao(center, key);
					if (tao == null)
					{
						continue;
					}

					Field oldFld = map[tao.type];
					if (oldFld == null)
					{
						Field fld = new Field(tao, v);
						map[tao.type] = fld;
					}
					else
					{
						if (oldFld.tao.order < tao.order)
						{ // replace old one
							Field fld = new Field(tao, v);
							map[tao.type] = fld;
						}
					}
				}
			}

			// extract standard fields values from specific StructureData
			public virtual void extract(StructureData sdata)
			{
				StructureMembers sm = sdata.StructureMembers;
				foreach (Field fld in map.Values)
				{
					StructureMembers.Member m = sm.findMember(fld.memberName);
					DataType dtype = m.DataType;
					if (dtype.isString())
					{
						fld.valueS = sdata.getScalarString(m).Trim();
					}
					else if (dtype.isIntegral())
					{
						fld.value = sdata.convertScalarInt(m);
						fld.valueD = fld.value;
					}
					else if (dtype.isNumeric())
					{
						fld.valueD = sdata.convertScalarDouble(m);
					}
				}
			}

			public virtual bool hasField(BufrCdmIndexProto.FldType type)
			{
				return null != map[type];
			}

			public virtual string getFieldName(BufrCdmIndexProto.FldType type)
			{
				Field fld = map[type];
				return (fld == null) ? null : fld.memberName;
			}

			public virtual string getFieldValueS(BufrCdmIndexProto.FldType type)
			{
				Field fld = map[type];
				if (fld == null)
				{
					return null;
				}
				if (!string.ReferenceEquals(fld.valueS, null))
				{
					return fld.valueS;
				}
				if (fld.value != -1)
				{
					return Convert.ToString(fld.value);
				}
				if (!double.IsNaN(fld.valueD))
				{
					return Convert.ToString(fld.valueD);
				}
				return null;
			}

			public virtual int getFieldValue(BufrCdmIndexProto.FldType type)
			{
				Field fld = map[type];
				return (fld == null) ? -1 : fld.value;
			}

			public virtual double getFieldValueD(BufrCdmIndexProto.FldType type)
			{
				Field fld = map[type];
				if (fld == null)
				{
					return double.NaN;
				}
				if (fld.hasScale)
				{
					return fld.valueD * fld.scale + fld.offset;
				}
				return fld.valueD;
			}

			public virtual string StationId
			{
				get
				{
					if (hasField(BufrCdmIndexProto.FldType.stationId))
					{
						return getFieldValueS(BufrCdmIndexProto.FldType.stationId);
					}
					if (hasField(BufrCdmIndexProto.FldType.wmoBlock) && hasField(BufrCdmIndexProto.FldType.wmoId))
					{
						return getFieldValue(BufrCdmIndexProto.FldType.wmoBlock) + "/" + getFieldValue(BufrCdmIndexProto.FldType.wmoId);
					}
					if (hasField(BufrCdmIndexProto.FldType.wmoId))
					{
						return Convert.ToString(getFieldValue(BufrCdmIndexProto.FldType.wmoId));
					}
					return null;
				}
			}

			public virtual CalendarDate makeCalendarDate()
			{
				if (!hasField(BufrCdmIndexProto.FldType.year))
				{
					return null;
				}
				int year = getFieldValue(BufrCdmIndexProto.FldType.year);

				int hour = !hasField(BufrCdmIndexProto.FldType.hour) ? 0 : getFieldValue(BufrCdmIndexProto.FldType.hour);
				int minute = !hasField(BufrCdmIndexProto.FldType.minute) ? 0 : getFieldValue(BufrCdmIndexProto.FldType.minute);
				int sec = !hasField(BufrCdmIndexProto.FldType.sec) ? 0 : getFieldValue(BufrCdmIndexProto.FldType.sec);
				if (sec < 0)
				{
					sec = 0;
				}
				else if (sec > 0)
				{
					Field fld = map[BufrCdmIndexProto.FldType.sec];
					if (fld.scale != 0)
					{
						sec = (int)(sec * fld.scale); // throw away msecs
					}
					if (sec < 0 || sec > 59)
					{
						sec = 0;
					}
				}

				if (hasField(BufrCdmIndexProto.FldType.month) && hasField(BufrCdmIndexProto.FldType.day))
				{
					int month = getFieldValue(BufrCdmIndexProto.FldType.month);
					int day = getFieldValue(BufrCdmIndexProto.FldType.day);
					return CalendarDate.of(null, year, month, day, hour, minute, sec);

				}
				else if (hasField(BufrCdmIndexProto.FldType.doy))
				{
					int doy = getFieldValue(BufrCdmIndexProto.FldType.doy);
					return CalendarDate.withDoy(null, year, doy, hour, minute, sec);
				}

				return null;
			}

		}

	}

}
