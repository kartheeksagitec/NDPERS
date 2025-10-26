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
    /// Class NeoSpin.BusinessObjects.busIbsCheckEntryHeaderGen:
    /// Inherited from busBase, used to create new business object for main table cdoIbsCheckEntryHeader and its children table. 
    /// </summary>
    [Serializable]
    public class busIbsCheckEntryHeaderGen : busExtendBase
    {
        /// <summary>
        /// Constructor for NeoSpin.BusinessObjects.busIbsCheckEntryHeaderGen
        /// </summary>
        public busIbsCheckEntryHeaderGen()
        {

        }

        /// <summary>
        /// Gets or sets the main-table object contained in busIbsCheckEntryHeaderGen.
        /// </summary>
        public cdoIbsCheckEntryHeader icdoIbsCheckEntryHeader { get; set; }




        /// <summary>
        /// NeoSpin.busIbsCheckEntryHeaderGen.FindIbsCheckEntryHeader():
        /// Finds a particular record from cdoIbsCheckEntryHeader with its primary key. 
        /// </summary>
        /// <param name="aintibscheckentryheaderid">A primary key value of type int of cdoIbsCheckEntryHeader on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
        public virtual bool FindIbsCheckEntryHeader(int aintibscheckentryheaderid)
        {
            bool lblnResult = false;
            if (icdoIbsCheckEntryHeader == null)
            {
                icdoIbsCheckEntryHeader = new cdoIbsCheckEntryHeader();
            }
            if (icdoIbsCheckEntryHeader.SelectRow(new object[1] { aintibscheckentryheaderid }))
            {
                lblnResult = true;
            }
            return lblnResult;
        }
        public Collection<busIbsCheckEntryDetail> iclbIbsCheckEntryDetail { get; set; }

        public void LoadIbsCheckEntryDetail()
        {
            DataTable ldtbCheckentriDetail = Select<cdoIbsCheckEntryDetail>(new string[1] { "ibs_check_entry_header_id" },
                new object[1] { icdoIbsCheckEntryHeader.ibs_check_entry_header_id }, null, null);
            iclbIbsCheckEntryDetail = GetCollection<busIbsCheckEntryDetail>(ldtbCheckentriDetail, "icdoIbsCheckEntryDetail");
            foreach (busIbsCheckEntryDetail lbusIbsCheckEntryDetail in iclbIbsCheckEntryDetail)
            {
                lbusIbsCheckEntryDetail.icdoIbsCheckEntryDetail.due_amount = lbusIbsCheckEntryDetail.GetDueAmount();
                lbusIbsCheckEntryDetail.LoadPerson();
            }
        }

        //public busPerson ibusPerson { get; set; }
        
        //public void LoadPerson()
        //{
        //    if (ibusPerson == null)
        //        ibusPerson = new busPerson();
        //    ibusPerson.FindPerson(icdoIbsCheckEntryHeader.
        //}
    }
}