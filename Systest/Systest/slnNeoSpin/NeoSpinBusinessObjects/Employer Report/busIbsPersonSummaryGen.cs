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
    /// Class NeoSpin.BusinessObjects.busIbsPersonSummaryGen:
    /// Inherited from busBase, used to create new business object for main table cdoIbsPersonSummary and its children table. 
    /// </summary>
	[Serializable]
	public class busIbsPersonSummaryGen : busExtendBase
    {
        /// <summary>
        /// Constructor for NeoSpin.BusinessObjects.busIbsPersonSummaryGen
        /// </summary>
		public busIbsPersonSummaryGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busIbsPersonSummaryGen.
        /// </summary>
		public cdoIbsPersonSummary icdoIbsPersonSummary { get; set; }

        /// <summary>
        /// Gets or sets the non-collection object of type busIbsHeader.
        /// </summary>
		public busIbsHeader ibusIbsHeader { get; set; }

        /// <summary>
        /// Gets or sets the non-collection object of type busPerson.
        /// </summary>
		public busPerson ibusPerson { get; set; }




        /// <summary>
        /// NeoSpin.busIbsPersonSummaryGen.FindIbsPersonSummary():
        /// Finds a particular record from cdoIbsPersonSummary with its primary key. 
        /// </summary>
        /// <param name="aintibspersonsummaryid">A primary key value of type int of cdoIbsPersonSummary on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindIbsPersonSummary(int aintibspersonsummaryid)
		{
			bool lblnResult = false;
			if (icdoIbsPersonSummary == null)
			{
				icdoIbsPersonSummary = new cdoIbsPersonSummary();
			}
			if (icdoIbsPersonSummary.SelectRow(new object[1] { aintibspersonsummaryid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

        /// <summary>
        /// NeoSpin.busIbsPersonSummaryGen.LoadIbsHeader():
        /// Loads non-collection object ibusIbsHeader of type busIbsHeader.
        /// </summary>
		public virtual void LoadIbsHeader()
		{
			if (ibusIbsHeader == null)
			{
				ibusIbsHeader = new busIbsHeader();
			}
			ibusIbsHeader.FindIbsHeader(icdoIbsPersonSummary.ibs_header_id);
		}

        /// <summary>
        /// NeoSpin.busIbsPersonSummaryGen.LoadPerson():
        /// Loads non-collection object ibusPerson of type busPerson.
        /// </summary>
		public virtual void LoadPerson()
		{
			if (ibusPerson == null)
			{
				ibusPerson = new busPerson();
			}
			ibusPerson.FindPerson(icdoIbsPersonSummary.person_id);
		}

	}
}
