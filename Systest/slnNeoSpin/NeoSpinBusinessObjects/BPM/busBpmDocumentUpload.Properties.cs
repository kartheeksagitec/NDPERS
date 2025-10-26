#region Using directives
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using NeoBase.BPMDataObjects;
#endregion
namespace NeoBase.BPM
{
    /// <summary>
    /// partial Class NeoSpin.busBpmDocumentUpload:
    /// </summary>	
    /// 
   // [Serializable]
    public partial class busBpmDocumentUpload 
	{
        
        /// <summary>
        /// Gets or sets the main-table object contained in busBpmDocumentUpload.
        /// </summary>
		public Sagitec.Bpm.doBpmDocumentUpload icdoBpmDocumentUpload { get; set; }
	}
}
