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
	public class busOrgDeductionType : busOrgDeductionTypeGen
	{
        public busOrganization ibusOrganization { get; set; }
        public void LoadOrganization()
        {
            if (ibusOrganization == null)
                ibusOrganization=new busOrganization();
            ibusOrganization.FindOrganization(icdoOrgDeductionType.org_id);
        }
	}
}
