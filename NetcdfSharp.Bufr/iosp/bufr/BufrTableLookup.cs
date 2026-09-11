// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System.Collections.Generic;

/*
 * Copyright (c) 1998-2018 University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */

namespace NetcdfSharp.ucar.nc2.iosp.bufr
{
	using NetcdfSharp.ucar.nc2.iosp.bufr.tables;
	using CommonCodeTable = NetcdfSharp.ucar.nc2.wmo.CommonCodeTable;

	/// <summary>
	/// Look up info in BUFR tables.
	/// Allows local center overrides for BUFR tables
	/// 
	/// @author caron
	/// @since 8/22/13
	/// </summary>
	public class BufrTableLookup
	{

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public static BufrTableLookup factory(Message m) throws NetcdfSharp.java.io.IOException
	  public static BufrTableLookup factory(Message m)
	  {
		return new BufrTableLookup(m.@is.BufrEdition, m.ids.CenterId, m.ids.SubCenterId, m.ids.MasterTableId, m.ids.MasterTableVersion, m.ids.LocalTableVersion, m.ids.Category, m.ids.SubCategory, m.ids.LocalSubCategory);
	  }

	  /*
	   * static public BufrTableLookup factory(int bufrEdition, int center, int subCenter, int masterId, int masterVersion,
	   * int localVersion,
	   * int category, int subCategory, int localSubCategory) {
	   * return new BufrTableLookup(bufrEdition, center, subCenter, masterId, masterVersion, localVersion, category,
	   * subCategory, localSubCategory);
	   * }
	   */

	  //////////////////////////////////////////////////////////

	  private int center, subCenter, masterId, masterVersion, localVersion, bufrEdition, category, subCategory, localSubCategory;

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: private BufrTableLookup(int bufrEdition, int center, int subCenter, int masterId, int masterVersion, int localVersion, int category, int subCategory, int localSubCategory) throws NetcdfSharp.java.io.IOException
	  private BufrTableLookup(int bufrEdition, int center, int subCenter, int masterId, int masterVersion, int localVersion, int category, int subCategory, int localSubCategory)
	  {
		this.bufrEdition = bufrEdition;
		this.center = center;
		this.subCenter = subCenter;
		this.masterId = masterId;
		this.masterVersion = masterVersion;
		this.localVersion = localVersion;
		this.category = category;
		this.subCategory = subCategory;
		this.localSubCategory = localSubCategory;

		tlookup = new TableLookup(center, subCenter, masterVersion, localVersion, category);
	  }

	  public virtual int BufrEdition
	  {
		  get
		  {
			return bufrEdition;
		  }
	  }

	  public virtual int Center
	  {
		  get
		  {
			return center;
		  }
	  }

	  public virtual int SubCenter
	  {
		  get
		  {
			return subCenter;
		  }
	  }

	  public virtual int MasterTableId
	  {
		  get
		  {
			return masterId;
		  }
	  }

	  public virtual int MasterTableVersion
	  {
		  get
		  {
			return masterVersion;
		  }
	  }

	  public virtual int LocalTableVersion
	  {
		  get
		  {
			return localVersion;
		  }
	  }

	  public virtual int Category
	  {
		  get
		  {
			return category;
		  }
	  }

	  public virtual int SubCategory
	  {
		  get
		  {
			return subCategory;
		  }
	  }

	  public virtual int LocalSubCategory
	  {
		  get
		  {
			return localSubCategory;
		  }
	  }

	  public virtual string CenterName
	  {
		  get
		  {
			string name = CommonCodeTable.getCenterNameBufr(Center, BufrEdition);
			string subname = CommonCodeTable.getSubCenterName(Center, SubCenter);
			if (!string.ReferenceEquals(subname, null))
			{
			  name = name + " / " + subname;
			}
			return Center + "." + SubCenter + " (" + name + ")";
		  }
	  }

	  public virtual string CenterNo
	  {
		  get
		  {
			return Center + "." + SubCenter;
		  }
	  }

	  public virtual string TableName
	  {
		  get
		  {
			return MasterTableId + "." + MasterTableVersion + "." + LocalTableVersion;
		  }
	  }

	  public virtual string CategoryFullName
	  {
		  get
		  { // throws IOException {
			string catName = CategoryName;
			string subcatName = SubCategoryName;

			if (!string.ReferenceEquals(subcatName, null))
			{
			  return CategoryNo + "=" + catName + " / " + subcatName;
			}
			else
			{
			  return CategoryNo + "=" + catName;
			}
		  }
	  }

	  public virtual string SubCategoryName
	  {
		  get
		  { // throws IOException {
			string subcatName = null;
			if (center == 7)
			{
			  subcatName = NcepTable.getDataSubcategory(Category, SubCategory);
			}
			if (string.ReferenceEquals(subcatName, null))
			{
			  subcatName = CommonCodeTable.getDataSubcategoy(Category, SubCategory);
			}
			return subcatName;
		  }
	  }

	  public virtual string CategoryName
	  {
		  get
		  {
			return TableA.getDataCategory(Category);
		  }
	  }

	  public virtual string getCategoryName(int cat)
	  {
		return TableA.getDataCategoryName(cat);
	  }

	  public virtual string CategoryNo
	  {
		  get
		  {
			string result = Category + "." + SubCategory;
			if (LocalSubCategory >= 0)
			{
			  result += "." + LocalSubCategory;
			}
			return result;
		  }
	  }

	  ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	  private TableLookup tlookup;

	  private void init()
	  {
	  }

	  public virtual TableLookup TableLookup
	  {
		  set
		  {
			this.tlookup = value;
		  }
	  }

	  public virtual TableB.Descriptor getDescriptorTableB(short fxy)
	  {
		return tlookup.getDescriptorTableB(fxy);
	  }

	  public virtual TableD.Descriptor getDescriptorTableD(short fxy)
	  {
		return tlookup.getDescriptorTableD(fxy);
	  }

	  public virtual string WmoTableBName
	  {
		  get
		  {
			return tlookup.WmoTableBName;
		  }
	  }

	  public virtual string LocalTableBName
	  {
		  get
		  {
			return tlookup.LocalTableBName;
		  }
	  }

	  public virtual string LocalTableDName
	  {
		  get
		  {
			return tlookup.LocalTableDName;
		  }
	  }

	  public virtual string WmoTableDName
	  {
		  get
		  {
			return tlookup.WmoTableDName;
		  }
	  }

	  public virtual BufrTables.Mode Mode
	  {
		  get
		  {
			return tlookup.Mode;
		  }
	  }

	  public virtual void showMissingFields(IList<short> ddsList, Formatter @out)
	  {
		foreach (short fxy in ddsList)
		{
		  int f = (fxy & 0xC000) >> 14;
		  if (f == 3)
		  {
			IList<short> sublist = getDescriptorListTableD(fxy);
			if (sublist == null)
			{
			  @out.format("%s, ", Descriptor.makeString(fxy));
			}
			else
			{
			  showMissingFields(sublist, @out);
			}

		  }
		  else if (f == 0)
		  { // skip the 2- operators for now
			TableB.Descriptor b = getDescriptorTableB(fxy);
			if (b == null)
			{
			  @out.format("%s, ", Descriptor.makeString(fxy));
			}
		  }
		}
	  }

	  public virtual IList<string> getDescriptorListTableD(string fxy)
	  {
		short id = Descriptor.getFxy(fxy);
		IList<short> seq = getDescriptorListTableD(id);
		if (seq == null)
		{
		  return null;
		}
		IList<string> result = new List<string>(seq.Count);
		foreach (short? s in seq)
		{
		  result.Add(Descriptor.makeString(s.Value));
		}
		return result;
	  }

	  public virtual IList<short> getDescriptorListTableD(short id)
	  {
		TableD.Descriptor d = getDescriptorTableD(id);
		if (d != null)
		{
		  return d.Sequence;
		}
		return null;
	  }
	}

}
