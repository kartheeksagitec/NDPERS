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
	/// Class NeoSpin.CustomDataObjects.cdoWssPersonAccountFlexComp:
	/// Inherited from doWssPersonAccountFlexComp, the class is used to customize the database object doWssPersonAccountFlexComp.
	/// </summary>
    [Serializable]
	public class cdoWssPersonAccountFlexComp : doWssPersonAccountFlexComp
	{
		public cdoWssPersonAccountFlexComp() : base()
		{
		}
    } 
} 
