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
	[Serializable]
    public partial class busBoardMemberVote : busExtendBase
    {
        /// <summary>
        /// Constructor for NeoSpin.busBoardMemberVote
        /// </summary>
        public busBoardMemberVote()
        {
        }
        #region LoadMethods
        public void LoadStaticTexts()
        {
            busBoardMemberElection lbusBoardMemberElection = new busBoardMemberElection() { icdoBoardMemberElection = new doBoardMemberElection() };
            if (lbusBoardMemberElection.FindByPrimaryKey(icdoBoardMemberVote.election_id))
            {
                DataTable ldtbBoardMemberStaticText = Select("cdoWssAcknowledgement.SelectAck", new object[2] { busGlobalFunctions.GetSysManagementBatchDate(), lbusBoardMemberElection.icdoBoardMemberElection.audience_value == busConstant.GroupTypeRetired ? busConstant.BoardMemberStaticTextRetiree : busConstant.BoardMemberStaticTextActive });
                if (ldtbBoardMemberStaticText.Rows.Count > 0)
                {
                    busWssAcknowledgement lobjWssAcknowledgement = new busWssAcknowledgement();
                    lobjWssAcknowledgement.icdoWssAcknowledgement = new CustomDataObjects.cdoWssAcknowledgement();
                    if (ldtbBoardMemberStaticText.Rows.Count > 0 && ldtbBoardMemberStaticText.Rows[0]["acknowledgement_text"] != DBNull.Value)
                    {
                        lobjWssAcknowledgement.icdoWssAcknowledgement.acknowledgement_text = Convert.ToString(ldtbBoardMemberStaticText.Rows[0]["acknowledgement_text"]);
                        if (icdoBoardMemberVote.election_id > 0)
                        {
                            istrBoardMemberStaticText = string.Format(lobjWssAcknowledgement.icdoWssAcknowledgement.acknowledgement_text,
                                                                        lbusBoardMemberElection.icdoBoardMemberElection.end_date.Year,
                                                                        lbusBoardMemberElection.icdoBoardMemberElection.term_start_date.ToString("MMMM dd, yyyy"),
                                                                        lbusBoardMemberElection.icdoBoardMemberElection.term_end_date.ToString("MMMM dd, yyyy"),
                                                                        busGlobalFunctions.CentsPart(Convert.ToString(lbusBoardMemberElection.icdoBoardMemberElection.allowed_votes_per_person)),
                                                                        lbusBoardMemberElection.icdoBoardMemberElection.end_date.ToLongDateString());
                        }
                    }
                }
                DataTable ldtbBalletStaticText = Select("cdoWssAcknowledgement.SelectAck", new object[2] { busGlobalFunctions.GetSysManagementBatchDate(), lbusBoardMemberElection.icdoBoardMemberElection.audience_value == busConstant.GroupTypeRetired ? busConstant.BallotStaticTextRetiree : busConstant.BallotStaticTextActive });
                if (ldtbBalletStaticText.Rows.Count > 0)
                {
                    busWssAcknowledgement lobjWssAcknowledgement = new busWssAcknowledgement();
                    lobjWssAcknowledgement.icdoWssAcknowledgement = new CustomDataObjects.cdoWssAcknowledgement();
                    if (ldtbBalletStaticText.Rows.Count > 0 && ldtbBalletStaticText.Rows[0]["acknowledgement_text"] != DBNull.Value)
                    {
                        lobjWssAcknowledgement.icdoWssAcknowledgement.acknowledgement_text = Convert.ToString(ldtbBalletStaticText.Rows[0]["acknowledgement_text"]);
                        if (icdoBoardMemberVote.election_id > 0)
                        {
                                istrBalletStaticText = string.Format(lobjWssAcknowledgement.icdoWssAcknowledgement.acknowledgement_text,
                                                                            busGlobalFunctions.CentsPart(Convert.ToString(lbusBoardMemberElection.icdoBoardMemberElection.allowed_votes_per_person)),
                                                                            lbusBoardMemberElection.icdoBoardMemberElection.audience_description == "Active Member" ? "Active" : lbusBoardMemberElection.icdoBoardMemberElection.audience_description,
                                                                            lbusBoardMemberElection.icdoBoardMemberElection.end_date.ToLongDateString());
                        }
                    }
                }
            }

        }

        public void LoadBoardMemberElectionCandidatesInRandomOrder()
        {
            idtbBoardMemberElectionCandidates = Select("entBoardMemberElection.LoadBoardMemberElectionCandidatesInRandomOrder", new object[1] { icdoBoardMemberVote.election_id });
            iclcBoardMemberElectionCandidates = GetCollection<busBoardMemberElectionCandidate>(idtbBoardMemberElectionCandidates, "icdoBoardMemberElectionCandidate");
        }
        public utlCollection<doBoardMemberElectionCandidate> LoadBoardMemberElectionCandidatesRandomOrder()
        {
            return Sagitec.DataObjects.doBase.GetCollection<doBoardMemberElectionCandidate>(idtbBoardMemberElectionCandidates);
        }
        public void LoadBoardMemberElection()
        {
            if (ibusBoardMemberElection.IsNull())
                ibusBoardMemberElection = new busBoardMemberElection() { icdoBoardMemberElection = new doBoardMemberElection() };
            ibusBoardMemberElection.FindByPrimaryKey(this.icdoBoardMemberVote.election_id);
        }
        #endregion LoadMethods

        #region OverridenMethods

        public override void BeforeValidate(utlPageMode aenmPageMode)
        {
            base.BeforeValidate(aenmPageMode);
        }
        public override void ValidateHardErrors(utlPageMode aenmPageMode)
        {
            base.ValidateHardErrors(aenmPageMode);
        }
        public override void BeforePersistChanges()
        {
            base.BeforePersistChanges();
        }
        public override int PersistChanges()
        {
            if (istrIsWriteInCandidateSelected == busConstant.Flag_Yes && !string.IsNullOrEmpty(istrWriteInCandidateName))
            {
                doBoardMemberVote ldoBoardMemberVote = new doBoardMemberVote();
                ldoBoardMemberVote.person_id = icdoBoardMemberVote.person_id;
                ldoBoardMemberVote.election_id = icdoBoardMemberVote.election_id;
                ldoBoardMemberVote.candidate_name = istrWriteInCandidateName;
                ldoBoardMemberVote.vote_cast_time = DateTime.Now;
                ldoBoardMemberVote.Insert();
            }
            if (iclcBoardMemberElectedCandidates.Count > 0)
            {
                foreach (doBoardMemberElectionCandidate ldoBoardMemberElectionCandidate in iclcBoardMemberElectedCandidates)
                {

                    doBoardMemberVote ldoBoardMemberVote = new doBoardMemberVote();
                    ldoBoardMemberVote.person_id = icdoBoardMemberVote.person_id;
                    ldoBoardMemberVote.election_id = icdoBoardMemberVote.election_id;
                    ldoBoardMemberVote.candidate_id = ldoBoardMemberElectionCandidate.candidate_id;
                    ldoBoardMemberVote.vote_cast_time = DateTime.Now;
                    ldoBoardMemberVote.Insert();
                }
            }
            return 1;
        }
        #endregion
    }
}
