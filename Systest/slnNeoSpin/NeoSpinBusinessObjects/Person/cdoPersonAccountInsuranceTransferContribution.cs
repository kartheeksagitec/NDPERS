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
	/// Class NeoSpin.CustomDataObjects.cdoPersonAccountInsuranceTransferContribution:
	/// Inherited from doPersonAccountInsuranceTransferContribution, the class is used to customize the database object doPersonAccountInsuranceTransferContribution.
	/// </summary>
    [Serializable]
	public class cdoPersonAccountInsuranceTransferContribution : doPersonAccountInsuranceTransferContribution
	{
		public cdoPersonAccountInsuranceTransferContribution() : base()
		{
		}
    } 
} 
