#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using   NeoSpin.CustomDataObjects;
using NeoSpin.DataObjects;

#endregion

namespace   NeoSpin.BusinessObjects
{
    /// <summary>
    /// Class   NeoSpin.BusinessObjects.busBenefitRhicCombineGen:
    /// Inherited from busBase, used to create new business object for main table cdoBenefitRhicCombine and its children table. 
    /// </summary>
	[Serializable]
	public class busBenefitRhicCombineGen : busExtendBase
	{
        /// <summary>
        /// Constructor for   NeoSpin.BusinessObjects.busBenefitRhicCombineGen
        /// </summary>
		public busBenefitRhicCombineGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busBenefitRhicCombineGen.
        /// </summary>
		public cdoBenefitRhicCombine icdoBenefitRhicCombine { get; set; }

        /// <summary>
        /// Gets or sets the non-collection object of type busPerson.
        /// </summary>
		public busPerson ibusPerson { get; set; }


        /// <summary>
        /// Gets or sets the collection object of type busBenefitRhicEstimateCombineDetail. 
        /// </summary>
		public Collection<busBenefitRhicEstimateCombineDetail> iclbBenefitRhicEstimateCombineDetail { get; set; }

        /// <summary>
        /// Gets or sets the collection object of type busBenefitRhicCombineDetail. 
        /// </summary>
		public Collection<busBenefitRhicCombineDetail> iclbBenefitRhicCombineDetail { get; set; }

        /// <summary>
        /// Gets or sets the collection object of type busBenefitRhicCombineHealthSplit. 
        /// </summary>
		public Collection<busBenefitRhicCombineHealthSplit> iclbBenefitRhicCombineHealthSplit { get; set; }



        /// <summary>
        ///   NeoSpin.busBenefitRhicCombineGen.FindBenefitRhicCombine():
        /// Finds a particular record from cdoBenefitRhicCombine with its primary key. 
        /// </summary>
        /// <param name="aintbenefitrhiccombineid">A primary key value of type int of cdoBenefitRhicCombine on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindBenefitRhicCombine(int aintbenefitrhiccombineid)
		{
			bool lblnResult = false;
			if (icdoBenefitRhicCombine == null)
			{
				icdoBenefitRhicCombine = new cdoBenefitRhicCombine();
			}
			if (icdoBenefitRhicCombine.SelectRow(new object[1] { aintbenefitrhiccombineid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

        /// <summary>
        ///   NeoSpin.busBenefitRhicCombineGen.LoadPerson():
        /// Loads non-collection object ibusPerson of type busPerson.
        /// </summary>
		public virtual void LoadPerson()
		{
			if (ibusPerson == null)
			{
				ibusPerson = new busPerson();
			}
			ibusPerson.FindPerson(icdoBenefitRhicCombine.person_id);
		}

        /// <summary>
        ///   NeoSpin.busBenefitRhicCombineGen.LoadBenefitRhicEstimateCombineDetails():
        /// Loads Collection object iclbBenefitRhicEstimateCombineDetail of type busBenefitRhicEstimateCombineDetail.
        /// </summary>
		public virtual void LoadBenefitRhicEstimateCombineDetails()
		{
			DataTable ldtbList = Select<cdoBenefitRhicEstimateCombineDetail>(
				new string[1] { enmBenefitRhicEstimateCombineDetail.benefit_rhic_combine_id.ToString() },
				new object[1] { icdoBenefitRhicCombine.benefit_rhic_combine_id }, null, null);
			iclbBenefitRhicEstimateCombineDetail = GetCollection<busBenefitRhicEstimateCombineDetail>(ldtbList, "icdoBenefitRhicEstimateCombineDetail");
		}

        /// <summary>
        ///   NeoSpin.busBenefitRhicCombineGen.LoadBenefitRhicCombineDetails():
        /// Loads Collection object iclbBenefitRhicCombineDetail of type busBenefitRhicCombineDetail.
        /// </summary>
		public virtual void LoadBenefitRhicCombineDetails()
		{
			DataTable ldtbList = Select<cdoBenefitRhicCombineDetail>(
				new string[1] { enmBenefitRhicCombineDetail.benefit_rhic_combine_id.ToString() },
				new object[1] { icdoBenefitRhicCombine.benefit_rhic_combine_id }, null, null);
			iclbBenefitRhicCombineDetail = GetCollection<busBenefitRhicCombineDetail>(ldtbList, "icdoBenefitRhicCombineDetail");
		}

        /// <summary>
        ///   NeoSpin.busBenefitRhicCombineGen.LoadBenefitRhicCombineHealthSplits():
        /// Loads Collection object iclbBenefitRhicCombineHealthSplit of type busBenefitRhicCombineHealthSplit.
        /// </summary>
		public virtual void LoadBenefitRhicCombineHealthSplits()
		{
			DataTable ldtbList = Select<cdoBenefitRhicCombineHealthSplit>(
				new string[1] { enmBenefitRhicCombineHealthSplit.benefit_rhic_combine_id.ToString() },
				new object[1] { icdoBenefitRhicCombine.benefit_rhic_combine_id }, null, enmBenefitRhicCombineHealthSplit.benefit_rhic_combine_id.ToString());
			iclbBenefitRhicCombineHealthSplit = GetCollection<busBenefitRhicCombineHealthSplit>(ldtbList, "icdoBenefitRhicCombineHealthSplit");
		}

	}
}
