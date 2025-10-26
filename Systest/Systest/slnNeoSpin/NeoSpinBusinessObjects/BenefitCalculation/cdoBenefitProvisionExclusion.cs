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
	/// Class NeoSpin.CustomDataObjects.cdoBenefitProvisionExclusion:
	/// Inherited from doBenefitProvisionExclusion, the class is used to customize the database object doBenefitProvisionExclusion.
	/// </summary>
    [Serializable]
	public class cdoBenefitProvisionExclusion : doBenefitProvisionExclusion
	{
		public cdoBenefitProvisionExclusion() : base()
		{
		}
    } 
} 
