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
	/// Class NeoSpin.CustomDataObjects.cdoProviderReportDataInsuranceSplit:
	/// Inherited from doProviderReportDataInsuranceSplit, the class is used to customize the database object doProviderReportDataInsuranceSplit.
	/// </summary>
    [Serializable]
	public class cdoProviderReportDataInsuranceSplit : doProviderReportDataInsuranceSplit
	{
		public cdoProviderReportDataInsuranceSplit() : base()
		{
		}
    } 
} 
