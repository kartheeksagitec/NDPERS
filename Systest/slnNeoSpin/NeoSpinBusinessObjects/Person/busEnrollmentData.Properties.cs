#region Using directives
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using NeoSpin.DataObjects;
#endregion
namespace NeoSpin.BusinessObjects
{
    /// <summary>
    /// partial Class NeoSpin.busEnrollmentData:
    /// </summary>	
	public partial class busEnrollmentData 
	{
        
        /// <summary>
        /// Gets or sets the main-table object contained in busEnrollmentData.
        /// </summary>
		public doEnrollmentData icdoEnrollmentData { get; set; }
	}
}
