#region Using directives

using System;
using System.Data;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using NeoSpin.CustomDataObjects;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.DBUtility;

#endregion

namespace NeoSpin.BusinessObjects
{
	public partial class busOrgContactLookup
	{
        protected override void LoadOtherObjects(DataRow adtrRow, busBase aobjBus)
        {
            if (aobjBus is busOrgContact)
            {
                busOrgContact lobjOrgContact = (busOrgContact)aobjBus;
                lobjOrgContact.ibusContact = new busContact();
                lobjOrgContact.ibusContact.icdoContact = new cdoContact();
                lobjOrgContact.ibusContact.icdoContact.first_name = (adtrRow["first_name"]).ToString();
                lobjOrgContact.ibusContact.icdoContact.last_name = (adtrRow["last_name"]).ToString();
                lobjOrgContact.istrRoleValue =iobjPassInfo.isrvDBCache.GetCodeDescriptionString(515 ,(adtrRow["contact_role_value"]).ToString());
                lobjOrgContact.istrOrgCode = (adtrRow["org_code"]).ToString();
            }
            
        }
	}
}
