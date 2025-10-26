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
	/// Class NeoSpin.CustomDataObjects.cdoWssAutoPostingCrossRef:
	/// Inherited from doWssAutoPostingCrossRef, the class is used to customize the database object doWssAutoPostingCrossRef.
	/// </summary>
    [Serializable]
	public class cdoWssAutoPostingCrossRef : doWssAutoPostingCrossRef
	{
		public cdoWssAutoPostingCrossRef() : base()
		{
		}
    } 
} 
