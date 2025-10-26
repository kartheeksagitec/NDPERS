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
	/// Class NeoSpin.BusinessObjects.busPersonAccountDeferredCompTransferContribution:
	/// Inherited from busPersonAccountDeferredCompTransferContributionGen, the class is used to customize the business object busPersonAccountDeferredCompTransferContributionGen.
	/// </summary>
	[Serializable]
	public class busPersonAccountDeferredCompTransferContribution : busPersonAccountDeferredCompTransferContributionGen
	{
        public busPersonAccountDeferredCompContribution ibusPersonAccountDefCompContribution { get; set; }      
	}
}
