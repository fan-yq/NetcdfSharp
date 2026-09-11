// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System.Collections.Generic;

namespace NetcdfSharp.ucar.nc2.filter
{
	using NetcdfSharp.ucar.ma2;
	using NetcdfSharp.ucar.nc2;
	using CDM = NetcdfSharp.ucar.nc2.constants.CDM;
	using Enhance = NetcdfSharp.ucar.nc2.dataset.NetcdfDataset.Enhance;
	using IndexIterator = NetcdfSharp.ucar.ma2.IndexIterator;
	using Misc = NetcdfSharp.ucar.nc2.util.Misc;
	using NcArray = NetcdfSharp.ucar.ma2.NcArray;
	using NcAttribute = NetcdfSharp.ucar.nc2.NcAttribute;
	using VariableDS = NetcdfSharp.ucar.nc2.dataset.VariableDS;


	public class Classifier : Enhancement
	{

		private string[] AttCat;
		private IList<int[]> rules = new List<int[]>();

		private const string ATTRIBUTE_NAME = "classify";

		public Classifier()
		{
			this.AttCat = new string[0];
			this.rules = new List<int[]>();
		}

		// Constructor with attributes
		public Classifier(string[] AttCat)
		{
			this.AttCat = AttCat;
			this.rules = loadClassificationRules();
		}

		// Factory method to create a Classifier from a Variable
		public static Classifier createFromVariable(VariableDS var)
		{
			IList<NcAttribute> attributes = var.attributes().Attributes;

			foreach (NcAttribute attribute in attributes)
			{
				// check like this, or something else?
				if (attribute == var.attributes().findAttribute(ATTRIBUTE_NAME))
				{
					string[] sets = attribute.StringValue.Split(';');
					for (int i = 0; i < sets.Length; i++)
					{
						// trim and clean so it's ready
						sets[i] = sets[i].Trim();
					}
					return new Classifier(sets);
				}
			}

			return new Classifier();
		}

		public virtual int[] classifyWithAttributes(NcArray arr)
		{
			int[] classifiedArray = new int[(int)arr.Size];
			IndexIterator iterArr = arr.IndexIterator;
			int i = 0;
			while (iterArr.hasNext())
			{
				object value = iterArr.ObjectNext;
				if (!double.IsNaN(Convert.ToDouble(value)))
				{
					classifiedArray[i] = classifyArrayAttribute(Convert.ToDouble(value));
				}
				else
				{
					classifiedArray[i] = int.MinValue;
				}
				i++;
			}
			return classifiedArray;
		}

		public virtual int classifyArrayAttribute(double val)
		{
			foreach (int[] rule in rules)
			{
				if (val > rule[0] && val <= rule[1] + Misc.defaultMaxRelativeDiffFloat)
				{
					return rule[2];
				}
			}
			// Return min possible int if no rule matches
			return int.MinValue;
		}

		// Method to load classification rules from the attributes
		private IList<int[]> loadClassificationRules()
		{
			foreach (string rules in this.AttCat)
			{
				int[] rule = stringToIntArray(rules);
				this.rules.Add(rule);
			}
			return rules;
		}

		public virtual double convert(double val)
		{
			return classifyArrayAttribute(val);
		}

		public static int[] stringToIntArray(string str)
		{
			string[] stringArray = str.Split(' '); // Split the string by spaces
			int[] intArray = new int[stringArray.Length]; // Create an array to hold the parsed integers

			for (int i = 0; i < stringArray.Length; i++)
			{

				double value = double.Parse(stringArray[i]); // Parse each string to a double

				if (double.IsNaN(value))
				{
					// Check if the entry is NaN and assign Integer.MIN_VALUE or Integer.MAX_VALUE based on the index
					if (i == 0)
					{
						intArray[i] = int.MinValue;
					}
					else if (i == 1)
					{
						intArray[i] = int.MaxValue;
					}
					else
					{
						intArray[i] = -99999; // Default value for other indices if needed
					}
				}
				else
				{
					intArray[i] = (int)value; // Convert the value to int if it is not NaN
				}

			}

			return intArray;
		}

		public class Provider : EnhancementProvider
		{

			public virtual string AttributeName
			{
				get
				{
					return ATTRIBUTE_NAME;
				}
			}

			public virtual bool appliesTo(ISet<Enhance> enhance, DataType dt)
			{
				return dt.isNumeric();
			}

			public virtual Enhancement create(VariableDS var)
			{
				return createFromVariable(var);
			}
		}

	}



}
