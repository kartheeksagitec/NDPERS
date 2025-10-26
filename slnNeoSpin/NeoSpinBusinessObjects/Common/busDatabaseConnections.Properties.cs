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
namespace Neospin.BusinessObjects
{
    /// <summary>
    /// partial Class NeoSpin.busDatabaseConnections:
    /// </summary>	
	public partial class busDatabaseConnections 
	{
        
        /// <summary>
        /// Gets or sets the main-table object contained in busDatabaseConnections.
        /// </summary>
		public doDatabaseConnections icdoDatabaseConnections { get; set; }
	}
}
