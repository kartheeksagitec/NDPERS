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
using System.Web;
using System.Collections.Generic;
using System.Text.RegularExpressions;

#endregion
namespace NeoSpin.BusinessObjects
{
    /// <summary>
    /// partial Class NeoSpin.busBoardMemberElectionCandidate:
    /// </summary>	
	public partial class busBoardMemberElectionCandidate 
	{
        
        /// <summary>
        /// Gets or sets the main-table object contained in busBoardMemberElectionCandidate.
        /// </summary>
		public doBoardMemberElectionCandidate icdoBoardMemberElectionCandidate { get; set; }
        public Dictionary<string, Collection<utlPostedFile>> idictHttpPostedFiles { get; set; }
        public string istrCandidateBioFormatted {
            get {
                Regex regex = new Regex(@"\n");
                return (regex.Replace(icdoBoardMemberElectionCandidate.candidate_bio, "</br>"));
            }
        }
    }
}
