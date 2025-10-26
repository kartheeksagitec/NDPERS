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
	/// Class NeoSpin.DataObjects.doBoardMemberVote:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doBoardMemberVote : doBase
    {
         public doBoardMemberVote() : base()
         {
         }
         public int board_member_vote_id { get; set; }
         public int election_id { get; set; }
        public int person_id { get; set; }
        public int candidate_id { get; set; }
         public DateTime vote_cast_time { get; set; }
         public string candidate_name { get; set; }
    }
    [Serializable]
    public enum enmBoardMemberVote
    {
         board_member_vote_id ,
         election_id ,
         person_id ,
         candidate_id ,
         vote_cast_time ,
         candidate_name ,
    }
}
