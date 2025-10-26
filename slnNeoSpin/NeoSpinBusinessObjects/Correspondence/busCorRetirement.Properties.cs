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
    /// partial Class NeoSpin.busCorRetirement:
    /// </summary>	
	public partial class busCorRetirement 
	{
        
        /// <summary>
        /// Gets or sets the main-table object contained in busCorRetirement.
        /// </summary>
		public doCorRetirement icdoCorRetirement { get; set; }
	}
}
