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
	/// Class NeoSpin.CustomDataObjects.cdoPersonAccountDeferredCompTransferContribution:
	/// Inherited from doPersonAccountDeferredCompTransferContribution, the class is used to customize the database object doPersonAccountDeferredCompTransferContribution.
	/// </summary>
    [Serializable]
	public class cdoPersonAccountDeferredCompTransferContribution : doPersonAccountDeferredCompTransferContribution
	{
		public cdoPersonAccountDeferredCompTransferContribution() : base()
		{
		}
    } 
} 
