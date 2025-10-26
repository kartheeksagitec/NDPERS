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
    /// Class NeoSpin.BusinessObjects.busIbsPersonSummary:
    /// Inherited from busIbsPersonSummaryGen, the class is used to customize the business object busIbsPersonSummaryGen.
    /// </summary>
    [Serializable]
    public class busIbsPersonSummary : busIbsPersonSummaryGen
    {
        public bool FindIbsPersonSummaryByIbsHeaderAndPerson(int aintIbsHeaderId, int aintPersonID)
        {
            if (icdoIbsPersonSummary == null)
            {
                icdoIbsPersonSummary = new cdoIbsPersonSummary();
            }
            DataTable ldtbList = Select<cdoIbsPersonSummary>(new string[2] { "ibs_header_id", "person_id" },
                 new object[2] { aintIbsHeaderId, aintPersonID }, null, null);
            if (ldtbList.Rows.Count > 0)
            {
                icdoIbsPersonSummary.LoadData(ldtbList.Rows[0]);
                return true;
            }
            return false;
        }
    }
}
