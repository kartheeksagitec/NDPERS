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
    /// partial Class NeoSpin.busCorPayeeAccount:
    /// </summary>	
	public partial class busCorPayeeAccount 
	{
        
        /// <summary>
        /// Gets or sets the main-table object contained in busCorPayeeAccount.
        /// </summary>
		public doCorPayeeAccount icdoCorPayeeAccount { get; set; }
	}
}
