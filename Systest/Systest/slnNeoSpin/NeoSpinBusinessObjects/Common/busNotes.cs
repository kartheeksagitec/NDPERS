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
	[Serializable]
	public class busNotes : busNotesGen
	{
        public override void AfterPersistChanges()
        {
            if (icdoNotes.person_id > 0)
                icdoNotes.person_name = busGlobalFunctions.GetPersonNameByPersonID(icdoNotes.person_id);
            if (icdoNotes.org_id > 0)
                icdoNotes.org_name = busGlobalFunctions.GetOrgNameByOrgID(icdoNotes.org_id);
            base.AfterPersistChanges();
        }
	}
}
