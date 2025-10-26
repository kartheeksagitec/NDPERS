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
	/// Class NeoSpin.CustomDataObjects.cdoWssPersonAccountDeferredCompProvider:
	/// Inherited from doWssPersonAccountDeferredCompProvider, the class is used to customize the database object doWssPersonAccountDeferredCompProvider.
	/// </summary>
    [Serializable]
	public class cdoWssPersonAccountDeferredCompProvider : doWssPersonAccountDeferredCompProvider
	{
		public cdoWssPersonAccountDeferredCompProvider() : base()
		{
		}
    } 
} 
