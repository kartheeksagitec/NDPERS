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
	/// Class NeoSpin.CustomDataObjects.cdoMasPersonCalculation:
	/// Inherited from doMasPersonCalculation, the class is used to customize the database object doMasPersonCalculation.
	/// </summary>
    [Serializable]
	public class cdoMasPersonCalculation : doMasPersonCalculation
	{
		public cdoMasPersonCalculation() : base()
		{
		}
    } 
} 
