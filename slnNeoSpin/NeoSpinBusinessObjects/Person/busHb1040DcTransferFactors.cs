#region Using directives
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using NeoSpin.DataObjects;
#endregion
namespace NeoSpin.BusinessObjects
{
    /// <summary>
    /// partial Class NeoSpin.busHb1040DcTransferFactors:
    /// </summary>
	[Serializable]
	public partial class busHb1040DcTransferFactors : busExtendBase
    {
		/// <summary>
        	/// Constructor for NeoSpin.busHb1040DcTransferFactors
        	/// </summary>
		public busHb1040DcTransferFactors()
		{
		}
        public doHb1040DcTransferFactors icdoHb1040DcTransferFactors { get; set; }

        public bool FindDcTransferFactors(decimal adecAge, decimal adecRetirementAge)
        {
            bool lblnResult = false;
            if (icdoHb1040DcTransferFactors == null)
            {
                icdoHb1040DcTransferFactors = new doHb1040DcTransferFactors();
            }
            DataTable ldtbList = Select<doHb1040DcTransferFactors>(
                                    new string[2] { enmHb1040DcTransferFactors.age.ToString(), enmHb1040DcTransferFactors.retirement_age.ToString() },
                                    new object[2] { adecAge, adecRetirementAge }, null, null);
            if (ldtbList.Rows.Count > 0)
            {
                icdoHb1040DcTransferFactors.LoadData(ldtbList.Rows[0]);
                lblnResult = true;
            }
            return lblnResult;
        }
    }
}
