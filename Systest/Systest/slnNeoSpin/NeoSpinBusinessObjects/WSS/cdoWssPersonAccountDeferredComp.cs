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
	/// Class NeoSpin.CustomDataObjects.cdoWssPersonAccountDeferredComp:
	/// Inherited from doWssPersonAccountDeferredComp, the class is used to customize the database object doWssPersonAccountDeferredComp.
	/// </summary>
    [Serializable]
	public class cdoWssPersonAccountDeferredComp : doWssPersonAccountDeferredComp
	{
		public cdoWssPersonAccountDeferredComp() : base()
		{
		}
    } 
} 
