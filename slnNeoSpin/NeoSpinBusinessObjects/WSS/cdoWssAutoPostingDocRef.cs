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
	/// Class NeoSpin.CustomDataObjects.cdoWssAutoPostingDocRef:
	/// Inherited from doWssAutoPostingDocRef, the class is used to customize the database object doWssAutoPostingDocRef.
	/// </summary>
    [Serializable]
	public class cdoWssAutoPostingDocRef : doWssAutoPostingDocRef
	{
		public cdoWssAutoPostingDocRef() : base()
		{
		}
    } 
} 
