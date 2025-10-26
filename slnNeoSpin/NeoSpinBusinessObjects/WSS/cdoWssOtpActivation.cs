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
	/// Class NeoSpin.CustomDataObjects.cdoWssOtpActivation:
	/// Inherited from doWssOtpActivation, the class is used to customize the database object doWssOtpActivation.
	/// </summary>
    [Serializable]
	public class cdoWssOtpActivation : doWssOtpActivation
	{
		public cdoWssOtpActivation() : base()
		{
		}
    } 
} 
