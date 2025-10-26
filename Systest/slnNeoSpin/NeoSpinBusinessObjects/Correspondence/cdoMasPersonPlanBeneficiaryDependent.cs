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
	/// Class NeoSpin.CustomDataObjects.cdoMasPersonPlanBeneficiaryDependent:
	/// Inherited from doMasPersonPlanBeneficiaryDependent, the class is used to customize the database object doMasPersonPlanBeneficiaryDependent.
	/// </summary>
    [Serializable]
	public class cdoMasPersonPlanBeneficiaryDependent : doMasPersonPlanBeneficiaryDependent
	{
		public cdoMasPersonPlanBeneficiaryDependent() : base()
		{
		}
    } 
} 
