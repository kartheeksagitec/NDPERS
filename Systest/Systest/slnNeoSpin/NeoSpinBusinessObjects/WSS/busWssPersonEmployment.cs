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
	/// Class NeoSpin.BusinessObjects.busWssPersonEmployment:
	/// Inherited from busWssPersonEmploymentGen, the class is used to customize the business object busWssPersonEmploymentGen.
	/// </summary>
	[Serializable]
	public class busWssPersonEmployment : busWssPersonEmploymentGen
	{
        public busWssPersonEmploymentDetail ibusWSSEmploymentDetail { get; set; }

        public busOrganization ibusOrganization { get; set; }

        public void LoadOrganization()
        {
            ibusOrganization = new busOrganization();
            ibusOrganization.FindOrganization(icdoWssPersonEmployment.org_id);
        }
	}
}
