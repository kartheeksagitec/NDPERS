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
    /// partial Class NeoSpin.busBoardMemberElection:
    /// </summary>	
	public partial class busBoardMemberElection 
	{
        #region Properties
        /// <summary>
        /// Gets or sets the main-table object contained in busBoardMemberElection.
        /// </summary>
        public doBoardMemberElection icdoBoardMemberElection { get; set; }

        public Collection<busBoardMemberElectionCandidate> iclbbusBoardMemberElectionCandidates { get; set; }

        public int iintNoOfVotesCast { get; set; }
        #endregion

    }
}
