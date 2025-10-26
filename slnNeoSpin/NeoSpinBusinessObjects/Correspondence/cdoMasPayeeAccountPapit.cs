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
	/// Class NeoSpin.CustomDataObjects.cdoMasPayeeAccountPapit:
	/// Inherited from doMasPayeeAccountPapit, the class is used to customize the database object doMasPayeeAccountPapit.
	/// </summary>
    [Serializable]
	public class cdoMasPayeeAccountPapit : doMasPayeeAccountPapit
	{
		public cdoMasPayeeAccountPapit() : base()
		{
		}
    } 
} 
