#region Using directives
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Sagitec.Common;
using Sagitec.DataObjects;
using System.Web;
#endregion
namespace NeoSpin.DataObjects
{
    /// <summary>
    /// Class NeoSpin.DataObjects.doBoardMemberElectionCandidate:
    /// Inherited from doBase, the class is used to create a wrapper of database table object.
    /// Each property of an instance of this class represents a column of database table object.  
    /// </summary>
    [Serializable]
    public class doBoardMemberElectionCandidate : doBase
    {
        public doBoardMemberElectionCandidate() : base()
        {
        }
        public int candidate_id { get; set; }
        public int election_id { get; set; }
        public string candidate_name { get; set; }
        public string candidate_bio { get; set; }
        public Byte[] candidate_picture { get; set; }
    }
    [Serializable]
    public enum enmBoardMemberElectionCandidate
    {
        candidate_id,
        election_id,
        candidate_name,
        candidate_bio,
        candidate_picture,
    }
}
