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
    /// partial Class NeoSpin.busCorRetirement:
    /// </summary>
	[Serializable]
	public partial class busCorRetirement : busNeoSpinBase
	{
		/// <summary>
        	/// Constructor for NeoSpin.busCorRetirement
        	/// </summary>
		public busCorRetirement()
		{
		}
        public busCorRetirement LoadCorRetirement(int aintDeathNotificationID, int aintPersonAccountID)
        {
            busCorRetirement lbusCorRetirement = new busCorRetirement() { icdoCorRetirement = new doCorRetirement() };
            DataTable ldtbCorRetirement = SelectWithOperator<doCorRetirement>(
                    new string[2] { enmCorRetirement.death_notification_id.ToString(), enmCorRetirement.person_account_id.ToString()},
                    new string[2] { "=", "=" },
                    new object[2] { aintDeathNotificationID, aintPersonAccountID }, enmCorRetirement.cor_retirement_id.ToString() + " asc");
            if (ldtbCorRetirement.Rows.Count > 0)
                lbusCorRetirement.icdoCorRetirement.LoadData(ldtbCorRetirement.Rows[0]);
            return lbusCorRetirement;
        }
    }
}
