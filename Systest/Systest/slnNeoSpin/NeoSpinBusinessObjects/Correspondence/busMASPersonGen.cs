#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using NeoSpin.CustomDataObjects;

#endregion

namespace NeoSpin.BusinessObjects
{
    /// <summary>
    /// Class NeoSpin.BusinessObjects.busMasPersonGen:
    /// Inherited from busBase, used to create new business object for main table cdoMasPerson and its children table. 
    /// </summary>
	[Serializable]
	public class busMasPersonGen : busExtendBase
    {
        /// <summary>
        /// Constructor for NeoSpin.BusinessObjects.busMasPersonGen
        /// </summary>
		public busMasPersonGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busMasPersonGen.
        /// </summary>
		public cdoMasPerson icdoMasPerson { get; set; }


        /// <summary>
        /// Gets or sets the collection object of type busMasDefCompProvider. 
        /// </summary>
		public Collection<busMasDefCompProvider> iclbMasDefCompProvider { get; set; }

        /// <summary>
        /// Gets or sets the collection object of type busMasFlexConversion. 
        /// </summary>
		public Collection<busMasFlexConversion> iclbMasFlexConversion { get; set; }

        /// <summary>
        /// Gets or sets the collection object of type busMasFlexOption. 
        /// </summary>
		public Collection<busMasFlexOption> iclbMasFlexOption { get; set; }

        /// <summary>
        /// Gets or sets the collection object of type busMasLifeOption. 
        /// </summary>
		public Collection<busMasLifeOption> iclbMasLifeOption { get; set; }

        /// <summary>
        /// Gets or sets the collection object of type busMasPayeeAccount. 
        /// </summary>
		public Collection<busMasPayeeAccount> iclbMasPayeeAccount { get; set; }

        /// <summary>
        /// Gets or sets the collection object of type busMasPersonCalculation. 
        /// </summary>
		public Collection<busMasPersonCalculation> iclbMasPersonCalculation { get; set; }

        /// <summary>
        /// Gets or sets the collection object of type busMasPersonPlan. 
        /// </summary>
		public Collection<busMASPersonPlan> iclbMasPersonPlan { get; set; }

        /// <summary>
        /// Gets or sets the collection object of type busMasPersonPlanBeneficiaryDependent. 
        /// </summary>
		public Collection<busMasPersonPlanBeneficiaryDependent> iclbMasPersonPlanBeneficiaryDependent { get; set; }



        /// <summary>
        /// NeoSpin.busMasPersonGen.FindMasPerson():
        /// Finds a particular record from cdoMasPerson with its primary key. 
        /// </summary>
        /// <param name="aintmaspersonid">A primary key value of type int of cdoMasPerson on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindMasPerson(int aintmaspersonid)
		{
			bool lblnResult = false;
			if (icdoMasPerson == null)
			{
				icdoMasPerson = new cdoMasPerson();
			}
			if (icdoMasPerson.SelectRow(new object[1] { aintmaspersonid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

        /// <summary>
        /// NeoSpin.busMasPersonGen.LoadMasDefCompProviders():
        /// Loads Collection object iclbMasDefCompProvider of type busMasDefCompProvider.
        /// </summary>
		public virtual void LoadMasDefCompProviders()
		{
			DataTable ldtbList = Select<cdoMasDefCompProvider>(
				new string[1] { "MAS_PERSON_ID" },
				new object[1] { icdoMasPerson.mas_person_id }, null, null);
			iclbMasDefCompProvider = GetCollection<busMasDefCompProvider>(ldtbList, "icdoMasDefCompProvider");
		}

        /// <summary>
        /// NeoSpin.busMasPersonGen.LoadMasFlexConversions():
        /// Loads Collection object iclbMasFlexConversion of type busMasFlexConversion.
        /// </summary>
		public virtual void LoadMasFlexConversions()
		{
			DataTable ldtbList = Select<cdoMasFlexConversion>(
                new string[1] { "MAS_PERSON_ID" },
				new object[1] { icdoMasPerson.mas_person_id }, null, null);
			iclbMasFlexConversion = GetCollection<busMasFlexConversion>(ldtbList, "icdoMasFlexConversion");
		}

        /// <summary>
        /// NeoSpin.busMasPersonGen.LoadMasFlexOptions():
        /// Loads Collection object iclbMasFlexOption of type busMasFlexOption.
        /// </summary>
		public virtual void LoadMasFlexOptions()
		{
			DataTable ldtbList = Select<cdoMasFlexOption>(
				new string[1] { "MAS_PERSON_ID" },
				new object[1] { icdoMasPerson.mas_person_id }, null, null);
			iclbMasFlexOption = GetCollection<busMasFlexOption>(ldtbList, "icdoMasFlexOption");
		}

        /// <summary>
        /// NeoSpin.busMasPersonGen.LoadMasLifeOptions():
        /// Loads Collection object iclbMasLifeOption of type busMasLifeOption.
        /// </summary>
		public virtual void LoadMasLifeOptions()
		{
			DataTable ldtbList = Select<cdoMasLifeOption>(
                new string[1] { "MAS_PERSON_ID" },
				new object[1] { icdoMasPerson.mas_person_id }, null, null);
			iclbMasLifeOption = GetCollection<busMasLifeOption>(ldtbList, "icdoMasLifeOption");
		}

        /// <summary>
        /// NeoSpin.busMasPersonGen.LoadMasPayeeAccounts():
        /// Loads Collection object iclbMasPayeeAccount of type busMasPayeeAccount.
        /// </summary>
		public virtual void LoadMasPayeeAccounts()
		{
			DataTable ldtbList = Select<cdoMasPayeeAccount>(
                new string[1] { "MAS_PERSON_ID" },
				new object[1] { icdoMasPerson.mas_person_id }, null, null);
			iclbMasPayeeAccount = GetCollection<busMasPayeeAccount>(ldtbList, "icdoMasPayeeAccount");
		}

        /// <summary>
        /// NeoSpin.busMasPersonGen.LoadMasPersonCalculations():
        /// Loads Collection object iclbMasPersonCalculation of type busMasPersonCalculation.
        /// </summary>
		public virtual void LoadMasPersonCalculations()
		{
			DataTable ldtbList = Select<cdoMasPersonCalculation>(
                new string[1] { "MAS_PERSON_ID" },
				new object[1] { icdoMasPerson.mas_person_id }, null, null);
			iclbMasPersonCalculation = GetCollection<busMasPersonCalculation>(ldtbList, "icdoMasPersonCalculation");
		}

        /// <summary>
        /// NeoSpin.busMasPersonGen.LoadMasPersonPlans():
        /// Loads Collection object iclbMasPersonPlan of type busMasPersonPlan.
        /// </summary>
		public virtual void LoadMasPersonPlans()
		{
			DataTable ldtbList = Select<cdoMasPersonPlan>(
                new string[1] { "MAS_PERSON_ID" },
				new object[1] { icdoMasPerson.mas_person_id }, null, null);
			iclbMasPersonPlan = GetCollection<busMASPersonPlan>(ldtbList, "icdoMasPersonPlan");
		}

        /// <summary>
        /// NeoSpin.busMasPersonGen.LoadMasPersonPlanBeneficiaryDependents():
        /// Loads Collection object iclbMasPersonPlanBeneficiaryDependent of type busMasPersonPlanBeneficiaryDependent.
        /// </summary>
		public virtual void LoadMasPersonPlanBeneficiaryDependents()
		{
			DataTable ldtbList = Select<cdoMasPersonPlanBeneficiaryDependent>(
				new string[1] { "MAS_PERSON_ID" },
				new object[1] { icdoMasPerson.mas_person_id }, null, null);
			iclbMasPersonPlanBeneficiaryDependent = GetCollection<busMasPersonPlanBeneficiaryDependent>(ldtbList, "icdoMasPersonPlanBeneficiaryDependent");
		}

	}
}
