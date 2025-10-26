#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using NeoSpin.DataObjects;

#endregion

namespace NeoSpin.CustomDataObjects
{
	/// <summary>
	/// Class NeoSpin.CustomDataObjects.cdoPersonAccountPaymentElectionHistory:
	/// Inherited from doPersonAccountPaymentElectionHistory, the class is used to customize the database object doPersonAccountPaymentElectionHistory.
	/// </summary>
    [Serializable]
	public class cdoPersonAccountPaymentElectionHistory : doPersonAccountPaymentElectionHistory
	{
		public cdoPersonAccountPaymentElectionHistory() : base()
		{

		}

        private string _Billing_Organization;
        public string Billing_Organization
        {
            get { return _Billing_Organization; }
            set { _Billing_Organization = value; }
        }

        private string _Supplemental_Billing_Organization;
        public string Supplemental_Billing_Organization
        {
            get { return _Supplemental_Billing_Organization; }
            set { _Supplemental_Billing_Organization = value; }
        }
    } 
} 
