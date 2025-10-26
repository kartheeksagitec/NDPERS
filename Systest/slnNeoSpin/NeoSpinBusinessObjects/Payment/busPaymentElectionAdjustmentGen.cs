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
    /// Class NeoSpin.BusinessObjects.busPaymentElectionAdjustmentGen:
    /// Inherited from busBase, used to create new business object for main table cdoPaymentElectionAdjustment and its children table. 
    /// </summary>
	[Serializable]
	public class busPaymentElectionAdjustmentGen : busExtendBase
	{
        /// <summary>
        /// Constructor for NeoSpin.BusinessObjects.busPaymentElectionAdjustmentGen
        /// </summary>
		public busPaymentElectionAdjustmentGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busPaymentElectionAdjustmentGen.
        /// </summary>
		public cdoPaymentElectionAdjustment icdoPaymentElectionAdjustment { get; set; }




        /// <summary>
        /// NeoSpin.busPaymentElectionAdjustmentGen.FindPaymentElectionAdjustment():
        /// Finds a particular record from cdoPaymentElectionAdjustment with its primary key. 
        /// </summary>
        /// <param name="aintpaymentelectionadjustmentid">A primary key value of type int of cdoPaymentElectionAdjustment on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindPaymentElectionAdjustment(int aintpaymentelectionadjustmentid)
		{
			bool lblnResult = false;
			if (icdoPaymentElectionAdjustment == null)
			{
				icdoPaymentElectionAdjustment = new cdoPaymentElectionAdjustment();
			}
			if (icdoPaymentElectionAdjustment.SelectRow(new object[1] { aintpaymentelectionadjustmentid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}
        public Collection<busPaymentElectionAdjustment> LoadPaymentElectionAdjustmentByPersonAccountID(int aintPersonAccountId)
        {         
            if (icdoPaymentElectionAdjustment == null)
            {
                icdoPaymentElectionAdjustment = new cdoPaymentElectionAdjustment();
            }
            Collection<busPaymentElectionAdjustment> lclbPaymentElectionAdjustment = new Collection<busPaymentElectionAdjustment>();
            DataTable ldtbList = Select<cdoPaymentElectionAdjustment>(
            new string[1] { "PERSON_ACCOUNT_ID" },
            new object[1] { aintPersonAccountId }, null, "created_date desc");
            lclbPaymentElectionAdjustment = GetCollection<busPaymentElectionAdjustment>(ldtbList, "icdoPaymentElectionAdjustment");
            return lclbPaymentElectionAdjustment;
        }

	}
}
