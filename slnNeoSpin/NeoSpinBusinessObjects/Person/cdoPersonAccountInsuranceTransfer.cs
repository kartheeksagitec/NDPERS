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
	/// Class NeoSpin.CustomDataObjects.cdoPersonAccountInsuranceTransfer:
	/// Inherited from doPersonAccountInsuranceTransfer, the class is used to customize the database object doPersonAccountInsuranceTransfer.
	/// </summary>
    [Serializable]
	public class cdoPersonAccountInsuranceTransfer : doPersonAccountInsuranceTransfer
	{
		public cdoPersonAccountInsuranceTransfer() : base()
		{
		}
        //used to get Person id from screen and to get ToPersonAccountId.
        public int iintTransferToPersonID { get; set; }
        //used for display purpose
        public decimal idecReceivedPremiumAmount { get; set; }
        // for display purpose only
        public string istrFromPersonName { get; set; }
        public string istrToPersonName { get; set; }
    } 
} 
