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
	/// Class NeoSpin.BusinessObjects.busPersonAccountInsuranceTransferContribution:
	/// Inherited from busPersonAccountInsuranceTransferContributionGen, the class is used to customize the business object busPersonAccountInsuranceTransferContributionGen.
	/// </summary>
	[Serializable]
	public class busPersonAccountInsuranceTransferContribution : busPersonAccountInsuranceTransferContributionGen
	{
        //to load the contribution details in the grid
        public busPersonAccountInsuranceContribution ibusPersonAccountInsuranceContribution { get; set; }
    }
}
