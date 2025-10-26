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
	[Serializable]
	public partial class busBoardMemberElection : busExtendBase
	{
		/// <summary>
        	/// Constructor for NeoSpin.busBoardMemberElection
        	/// </summary>
		public busBoardMemberElection()
		{
		}
        #region LoadMethods
        public void LoadBoardMemberElectionCandidates()
        {
            DataTable ldtbBoardMemberElectionCandidates = Select<doBoardMemberElectionCandidate>(new string[1] { Convert.ToString(NeoSpin.DataObjects.enmBoardMemberElectionCandidate.election_id) }, new object[1] { icdoBoardMemberElection.election_id }, null, null);
            iclbbusBoardMemberElectionCandidates = GetCollection<busBoardMemberElectionCandidate>(ldtbBoardMemberElectionCandidates, "icdoBoardMemberElectionCandidate");
        }
        #endregion

        public ArrayList ViewElectionResultsReport(int aintElectionId)
        {
            ArrayList larrlstReport = new ArrayList();
            string lstrReportName = "rptElectionResultsReport.rpt";
            larrlstReport.Add(lstrReportName + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".pdf");
            DataTable ldtbReportHeader = busNeoSpinBase.Select("entBoardMemberElection.BoardMemberElectionReportHeader", new object[1] { aintElectionId });
            DataTable ldtbIndividualVotesCast = busNeoSpinBase.Select("entBoardMemberElection.BoardMemberElectionIndividualVotesCast", new object[1] { aintElectionId });
            DataTable ldtbSummaryGridResults = busNeoSpinBase.Select("entBoardMemberElection.BoardMemberElectionOrderedSummaryGrid", new object[1] { aintElectionId });
            DataSet ldsElectionResults = new DataSet();
            ldtbIndividualVotesCast.TableName = busConstant.ReportTableName;
            ldsElectionResults.Tables.Add(ldtbIndividualVotesCast.Copy());
            ldtbSummaryGridResults.TableName = busConstant.ReportTableName02;
            ldsElectionResults.Tables.Add(ldtbSummaryGridResults.Copy());
            ldtbReportHeader.TableName = busConstant.ReportTableName03;
            ldsElectionResults.Tables.Add(ldtbReportHeader.Copy());
            busNeoSpinBase lobjNeospinBase = new busNeoSpinBase();
            larrlstReport.Add(lobjNeospinBase.CreateDynamicReport(lstrReportName, ldsElectionResults, string.Empty));
            larrlstReport.Add("application/pdf");
            return larrlstReport;
        }
    }
}
