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
	/// Class NeoSpin.CustomDataObjects.cdoMasLifeOptions:
	/// Inherited from doMasLifeOptions, the class is used to customize the database object doMasLifeOptions.
	/// </summary>
    [Serializable]
	public class cdoMasLifeOption : doMasLifeOption
	{
		public cdoMasLifeOption() : base()
		{
		}
    } 
} 
