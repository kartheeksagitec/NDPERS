#region Using directives
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Sagitec.Common;
using Sagitec.DataObjects;
#endregion
namespace NeoSpin.DataObjects
{
	/// <summary>
	/// Class NeoSpin.DataObjects.doBoardMemberElection:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doBoardMemberElection : doBase
    {
        public doBoardMemberElection() : base()
        {
        }
        public int election_id { get; set; }
        public DateTime start_date { get; set; }
        public DateTime end_date { get; set; }
        public DateTime term_start_date { get; set; }
        public DateTime term_end_date { get; set; }
        public int audience_id { get; set; }
        public string audience_description { get; set; }
        public string audience_value { get; set; }
        public byte allowed_votes_per_person { get; set; }

        public int iintNoOfVotesCast { get; set; }
    }
    [Serializable]
    public enum enmBoardMemberElection
    {
        election_id,
        start_date,
        end_date,
        audience_id,
        audience_description,
        audience_value,
        allowed_votes_per_person,
        term_start_date,
        term_end_date
    }
}
