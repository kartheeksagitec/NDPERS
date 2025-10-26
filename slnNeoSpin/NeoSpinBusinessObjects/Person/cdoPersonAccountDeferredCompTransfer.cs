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
	/// Class NeoSpin.CustomDataObjects.cdoPersonAccountDeferredCompTransfer:
	/// Inherited from doPersonAccountDeferredCompTransfer, the class is used to customize the database object doPersonAccountDeferredCompTransfer.
	/// </summary>
    [Serializable]
	public class cdoPersonAccountDeferredCompTransfer : doPersonAccountDeferredCompTransfer
	{
		public cdoPersonAccountDeferredCompTransfer() : base()
		{
		}

        public decimal idecPayPeriodContributionAmount { get; set; }

        public int iintTransferToPersonId { get; set; }
        // for display purpose only
        public string istrFromPersonName { get; set; }
        public string istrToPersonName { get; set; }
    } 
} 
