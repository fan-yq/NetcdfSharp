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

namespace NetcdfSharp.ucar.nc2.iosp.bufr
{
	using NetcdfSharp.ucar.ma2;
	using NetcdfSharp.ucar.nc2;
	using RandomAccessFile = NetcdfSharp.ucar.unidata.io.RandomAccessFile;
	using TableA = NetcdfSharp.ucar.nc2.iosp.bufr.tables.TableA;
	using TableB = NetcdfSharp.ucar.nc2.iosp.bufr.tables.TableB;
	using TableD = NetcdfSharp.ucar.nc2.iosp.bufr.tables.TableD;
	using Util = NetcdfSharp.ucar.nc2.wmo.Util;
	using WmoXmlReader = NetcdfSharp.ucar.nc2.iosp.bufr.tables.WmoXmlReader;

	/// <summary>
	/// BUFR allows you to encode a BUFR table in BUFR.
	/// if table is embedded, all entries must be from it
	/// LOOK: may be NCEP specific ?
	/// 
	/// @author John
	/// @since 8/11/11
	/// </summary>
	public class EmbeddedTable
	{
		private const bool showB = false;
		private const bool showD = false;

		private readonly RandomAccessFile raf;
		private readonly BufrIdentificationSection ids;

		private IList<Message> messages = new List<Message>();
		private bool tableRead;
		private TableA a;
		private TableB b;
		private TableD d;
		private Structure seq1, seq2, seq3, seq4;
		private TableLookup tlookup;

		internal EmbeddedTable(Message m, RandomAccessFile raf)
		{
			this.raf = raf;
			this.ids = m.ids;
			a = new TableA("embed", raf.Location);
			b = new TableB("embed", raf.Location);
			d = new TableD("embed", raf.Location);
		}

		public virtual void addTable(Message m)
		{
			messages.Add(m);
		}

		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: private void read2() throws NetcdfSharp.java.io.IOException
		private void read2()
		{
			Message proto = messages[0];

			// make root sub key data descriptors name as null, so the following construct
			// will have seq2 and seq3 variables
			DataDescriptor root = proto.RootDataDescriptor;
			foreach (DataDescriptor ds in root.subKeys)
			{
				ds.name = null;
			}

			BufrConfig config = BufrConfig.openFromMessage(raf, proto, null);
			Construct2 construct = new Construct2(proto, config, new NetcdfFileSubclass());

			Sequence obs = construct.ObsStructure;
			seq1 = (Structure)obs.findVariable("seq1");
			seq2 = (Structure)obs.findVariable("seq2");
			seq3 = (Structure)obs.findVariable("seq3");
			seq4 = (Structure)seq3.findVariable("seq4");

			// read all the messages
			ArrayStructure data;
			foreach (Message m in messages)
			{
				if (!m.dds.Compressed)
				{
					MessageUncompressedDataReader reader = new MessageUncompressedDataReader();
					data = reader.readEntireMessage(obs, proto, m, raf, null);
				}
				else
				{
					MessageCompressedDataReader reader = new MessageCompressedDataReader();
					data = reader.readEntireMessage(obs, proto, m, raf, null);
				}
				while (data.hasNext())
				{
					StructureData sdata = (StructureData)data.next();
					add(sdata);
				}
			}
		}

		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: private void add(StructureData data) throws NetcdfSharp.java.io.IOException
		private void add(StructureData data)
		{
			foreach (StructureMembers.Member m in data.Members)
			{
				if (showB)
				{
					// JAVA TO C# CONVERTER TASK: The following line has a Java format specifier which cannot be directly translated to .NET:
					Console.Write("{0}%n", m);
				}
				if (m.DataType == DataType.SEQUENCE)
				{
					if (m.Name.Equals("seq1"))
					{
						ArraySequence seq = data.getArraySequence(m);
						StructureDataIterator iter = seq.StructureDataIterator;
						while (iter.hasNext())
						{
							addTableEntryA(iter.next());
						}
					}
					else if (m.Name.Equals("seq2"))
					{
						ArraySequence seq = data.getArraySequence(m);
						StructureDataIterator iter = seq.StructureDataIterator;
						while (iter.hasNext())
						{
							addTableEntryB(iter.next());
						}
					}
					else if (m.Name.Equals("seq3"))
					{
						ArraySequence seq = data.getArraySequence(m);
						StructureDataIterator iter = seq.StructureDataIterator;
						while (iter.hasNext())
						{
							addTableEntryD(iter.next());
						}
					}
				}
			}
		}

		private void addTableEntryA(StructureData sdata)
		{
			int scale = 0, refVal = 0, width = 0;
			string entry = "", line1 = "", line2 = "";
			IList<StructureMembers.Member> members = sdata.Members;
			IList<Variable> vars = seq1.Variables;
			for (int i = 0; i < vars.Count; i++)
			{
				Variable v = vars[i];
				StructureMembers.Member m = members[i];
				string data = sdata.getScalarString(m);
				NcAttribute att = v.attributes().findAttribute(BufrIosp2.fxyAttName);
				switch (att.StringValue)
				{
					case "0-0-1":
						entry = sdata.getScalarString(m);
						break;
					case "0-0-2":
						line1 = sdata.getScalarString(m);
						break;
					case "0-0-3":
						line2 = sdata.getScalarString(m);
						break;
				}
			}

			int code = int.Parse(entry);

			// split name and description from appended line 1 and 2
			string desc = (line1 + line2).Trim();
			string name = "";
			int pos = desc.IndexOf(' ');
			if (pos > 0)
			{
				name = desc.Substring(0, pos);
			}

			TableA.Descriptor d = a.addDescriptor(code, desc);
			d.Name = name;
		}

		private void addTableEntryB(StructureData sdata)
		{
			string name = "", units = "", signScale = null, signRef = null;
			int scale = 0, refVal = 0, width = 0;
			short x1 = 0, y1 = 0;
			IList<StructureMembers.Member> members = sdata.Members;
			IList<Variable> vars = seq2.Variables;
			for (int i = 0; i < vars.Count; i++)
			{
				Variable v = vars[i];
				StructureMembers.Member m = members[i];
				string data = sdata.getScalarString(m);
				if (showB)
				{
					// JAVA TO C# CONVERTER TASK: The following line has a Java format specifier which cannot be directly translated to .NET:
					Console.Write("{0} == {1}%n", v, data);
				}

				NcAttribute att = v.attributes().findAttribute(BufrIosp2.fxyAttName);
				switch (att.StringValue)
				{
					case "0-0-10":
						sdata.getScalarString(m);
						break;
					case "0-0-11":
						string x = sdata.getScalarString(m);
						x1 = short.Parse(x.Trim());
						break;
					case "0-0-12":
						string y = sdata.getScalarString(m);
						y1 = short.Parse(y.Trim());
						break;
					case "0-0-13":
						name = sdata.getScalarString(m);
						break;
					case "0-0-14":
						name += sdata.getScalarString(m); // append both lines

						break;
					case "0-0-15":
						units = sdata.getScalarString(m);
						units = WmoXmlReader.cleanUnit(units.Trim());
						break;
					case "0-0-16":
						signScale = sdata.getScalarString(m).Trim();
						break;
					case "0-0-17":
						string scaleS = sdata.getScalarString(m);
						scale = int.Parse(scaleS.Trim());
						break;
					case "0-0-18":
						signRef = sdata.getScalarString(m).Trim();
						break;
					case "0-0-19":
						string refS = sdata.getScalarString(m);
						refVal = int.Parse(refS.Trim());
						break;
					case "0-0-20":
						string widthS = sdata.getScalarString(m);
						width = int.Parse(widthS.Trim());
						break;
				}
			}
			if (showB)
			{
				Console.Write("%n");
			}

			// split name and description from appended line 1 and 2
			string desc = null;
			name = name.Trim();
			int pos = name.IndexOf(' ');
			if (pos > 0)
			{
				desc = Util.cleanName(name.Substring(pos + 1));
				name = name.Substring(0, pos);
				name = Util.cleanName(name);
			}

			if ("-".Equals(signScale))
			{
				scale = -1 * scale;
			}
			if ("-".Equals(signRef))
			{
				refVal = -1 * refVal;
			}

			b.addDescriptor(x1, y1, scale, refVal, width, name, units, desc);
		}

		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: private void addTableEntryD(StructureData sdata) throws NetcdfSharp.java.io.IOException
		private void addTableEntryD(StructureData sdata)
		{
			string name = null;
			short x1 = 0, y1 = 0;
			IList<short> dds = null;

			IList<StructureMembers.Member> members = sdata.Members;
			IList<Variable> vars = seq3.Variables;
			for (int i = 0; i < vars.Count; i++)
			{
				Variable v = vars[i];
				StructureMembers.Member m = members[i];
				if (m.Name.Equals("seq4"))
				{
					dds = getDescriptors(sdata.getArraySequence(m));
					continue;
				}

				NcAttribute att = v.attributes().findAttribute(BufrIosp2.fxyAttName);
				if (att != null)
				{
					if (showD)
					{
						// JAVA TO C# CONVERTER TASK: The following line has a Java format specifier which cannot be directly translated to .NET:
						Console.Write("{0} == {1}%n", v, sdata.getScalarString(m));
					}
					switch (att.StringValue)
					{
						case "0-0-10":
							sdata.getScalarString(m);
							break;
						case "0-0-11":
							string x = sdata.getScalarString(m);
							x1 = short.Parse(x.Trim());
							break;
						case "0-0-12":
							string y = sdata.getScalarString(m);
							y1 = short.Parse(y.Trim());
							break;
						case "2-5-64":
							name = sdata.getScalarString(m);
							break;
					}
				}
			}
			if (showD)
			{
				Console.Write("%n");
			}

			name = Util.cleanName(name);

			d.addDescriptor(x1, y1, name, dds);
		}

		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: private NetcdfSharp.java.util.List<short> getDescriptors(ArraySequence seqdata) throws NetcdfSharp.java.io.IOException
		private IList<short> getDescriptors(ArraySequence seqdata)
		{
			IList<short> list = new List<short>();
			string fxyS = null;
			IList<Variable> vars = seq4.Variables;

			StructureDataIterator iter = seqdata.StructureDataIterator;
			while (iter.hasNext())
			{
				StructureData sdata = iter.next();

				IList<StructureMembers.Member> members = sdata.Members;
				for (int i = 0; i < vars.Count; i++)
				{
					Variable v = vars[i];
					StructureMembers.Member m = members[i];
					string data = sdata.getScalarString(m);
					if (showD)
					{
						// JAVA TO C# CONVERTER TASK: The following line has a Java format specifier which cannot be directly translated to .NET:
						Console.Write("{0} == {1}%n", v, data);
					}

					NcAttribute att = v.attributes().findAttribute(BufrIosp2.fxyAttName);
					if (att != null && att.StringValue.Equals("0-0-30"))
					{
						fxyS = sdata.getScalarString(m);
					}
				}
				if (showD)
				{
					Console.Write("%n");
				}

				if (!string.ReferenceEquals(fxyS, null))
				{
					short id = Descriptor.getFxy2(fxyS);
					list.Add(id);
				}
			}
			return list;
		}

		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: TableLookup getTableLookup() throws NetcdfSharp.java.io.IOException
		internal virtual TableLookup TableLookup
		{
			get
			{
				if (!tableRead)
				{
					read2();
					tableRead = true;
					tlookup = new TableLookup(ids, a, b, d);
				}
				return tlookup;
			}
		}

	}

}
