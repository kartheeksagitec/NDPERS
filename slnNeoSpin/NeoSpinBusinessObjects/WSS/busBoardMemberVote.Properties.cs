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
    /// partial Class NeoSpin.busBoardMemberVote:
    /// </summary>	
	public partial class busBoardMemberVote 
	{

        /// <summary>
        /// Gets or sets the main-table object contained in busBoardMemberVote.
        /// </summary>
        #region One-To-One Properties
        public doBoardMemberVote icdoBoardMemberVote { get; set; }
        public string istrBoardMemberStaticText { get; set; }

        public string istrBalletStaticText { get; set; }

        public string istrIsWriteInCandidateSelected { get; set; }
        public string istrWriteInCandidateName { get; set; }
        public busBoardMemberElection ibusBoardMemberElection { get; set; }
        public DataTable idtbBoardMemberElectionCandidates {get; set; }
        #endregion

        #region One-To-Many Properties
        public Collection<busBoardMemberElectionCandidate> iclcBoardMemberElectionCandidates { get; set; }
        public utlCollection<doBoardMemberElectionCandidate> iclcBoardMemberElectedCandidates { get; set; }
        #endregion
    }
}
