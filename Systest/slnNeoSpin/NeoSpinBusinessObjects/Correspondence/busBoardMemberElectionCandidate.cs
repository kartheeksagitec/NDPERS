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
using System.Linq;
#endregion
namespace NeoSpin.BusinessObjects
{
    /// <summary>
    /// partial Class NeoSpin.busBoardMemberElectionCandidate:
    /// </summary>
	[Serializable]
	public partial class busBoardMemberElectionCandidate : busExtendBase
    {
		/// <summary>
        	/// Constructor for NeoSpin.busBoardMemberElectionCandidate
        	/// </summary>
		public busBoardMemberElectionCandidate()
		{
            idictHttpPostedFiles = new System.Collections.Generic.Dictionary<string, Collection<utlPostedFile>>();
		}
        public override void BeforePersistChanges()
        {
            if(idictHttpPostedFiles.Count > 0 && idictHttpPostedFiles.ContainsKey("fluCandidatePicture"))
            {
                utlPostedFile lUtlPostedFile = idictHttpPostedFiles["fluCandidatePicture"].FirstOrDefault();
                this.icdoBoardMemberElectionCandidate.candidate_picture = lUtlPostedFile.iarrFileBytes;
            }
            base.BeforePersistChanges();
        }
    }
}
