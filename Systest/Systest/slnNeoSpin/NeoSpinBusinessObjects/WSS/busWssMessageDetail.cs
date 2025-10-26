#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using NeoSpin.CustomDataObjects;

#endregion

namespace NeoSpin.BusinessObjects
{
	/// <summary>
	/// Class NeoSpin.BusinessObjects.busWssMessageDetail:
	/// Inherited from busWssMessageDetailGen, the class is used to customize the business object busWssMessageDetailGen.
	/// </summary>
	[Serializable]
	public class busWssMessageDetail : busWssMessageDetailGen
	{
        public busWssMessageHeader ibusWSSMessageHeader { get; set; }
        public void LoadWSSMessageHeader()
        {
            ibusWSSMessageHeader = new busWssMessageHeader();
            ibusWSSMessageHeader.FindWssMessageHeader(icdoWssMessageDetail.wss_message_id);
        }

        public busOrganization ibusOrganization { get; set; }

        public void LoadOrganization()
        {
            ibusOrganization = new busOrganization();
            ibusOrganization.FindOrganization(icdoWssMessageDetail.org_id);
        }

        public busPerson ibusPerson { get; set; }
        public bool IsRquestForNewEmployee() => ibusWSSMessageHeader?.icdoWssMessageHeader?.istrRequestType == busConstant.MemberRecordRequestWizard;
        public void LoadPerson()
        {
            ibusPerson = new busPerson();
            ibusPerson.FindPerson(icdoWssMessageDetail.person_id);
        }
	}
}
