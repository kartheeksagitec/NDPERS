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
	/// Class NeoSpin.CustomDataObjects.cdoPersonAccountDependentBillingLink:
	/// Inherited from doPersonAccountDependentBillingLink, the class is used to customize the database object doPersonAccountDependentBillingLink.
	/// </summary>
    [Serializable]
	public class cdoPersonAccountDependentBillingLink : doPersonAccountDependentBillingLink
	{
		public cdoPersonAccountDependentBillingLink() : base()
		{
		}
    } 
} 
