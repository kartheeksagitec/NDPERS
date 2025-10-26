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
    /// Class NeoSpin.BusinessObjects.busIbsCheckEntryDetailGen:
    /// Inherited from busBase, used to create new business object for main table cdoIbsCheckEntryDetail and its children table. 
    /// </summary>
    [Serializable]
    public class busIbsCheckEntryDetailGen : busExtendBase
    {
        /// <summary>
        /// Constructor for NeoSpin.BusinessObjects.busIbsCheckEntryDetailGen
        /// </summary>
        public busIbsCheckEntryDetailGen()
        {

        }

        /// <summary>
        /// Gets or sets the main-table object contained in busIbsCheckEntryDetailGen.
        /// </summary>
        public cdoIbsCheckEntryDetail icdoIbsCheckEntryDetail { get; set; }

        /// <summary>
        /// Gets or sets the non-collection object of type busIbsCheckEntryHeader.
        /// </summary>
        public busIbsCheckEntryHeader ibusIbsCheckEntryHeader { get; set; }


        /// <summary>
        /// NeoSpin.busIbsCheckEntryDetailGen.FindIbsCheckEntryDetail():
        /// Finds a particular record from cdoIbsCheckEntryDetail with its primary key. 
        /// </summary>
        /// <param name="aintibscheckentrydetailid">A primary key value of type int of cdoIbsCheckEntryDetail on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
        public virtual bool FindIbsCheckEntryDetail(int aintibscheckentrydetailid)
        {
            bool lblnResult = false;
            if (icdoIbsCheckEntryDetail == null)
            {
                icdoIbsCheckEntryDetail = new cdoIbsCheckEntryDetail();
            }
            if (icdoIbsCheckEntryDetail.SelectRow(new object[1] { aintibscheckentrydetailid }))
            {
                lblnResult = true;
            }
            return lblnResult;
        }

        /// <summary>
        /// NeoSpin.busIbsCheckEntryDetailGen.LoadIbsCheckEntryHeader():
        /// Loads non-collection object ibusIbsCheckEntryHeader of type busIbsCheckEntryHeader.
        /// </summary>
        public virtual void LoadIbsCheckEntryHeader()
        {
            if (ibusIbsCheckEntryHeader == null)
            {
                ibusIbsCheckEntryHeader = new busIbsCheckEntryHeader();
            }
            ibusIbsCheckEntryHeader.FindIbsCheckEntryHeader(icdoIbsCheckEntryDetail.ibs_check_entry_header_id);
        }

        public busPerson ibusPerson { get; set; }
        public void LoadPerson()
        {
            if (ibusPerson == null)
                ibusPerson = new busPerson();
            ibusPerson.FindPerson(icdoIbsCheckEntryDetail.person_id);
        }
    }
}
