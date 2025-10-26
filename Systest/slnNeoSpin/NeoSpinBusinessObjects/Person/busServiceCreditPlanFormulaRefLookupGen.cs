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
	[Serializable]
	public class busServiceCreditPlanFormulaRefLookupGen : busMainBase
	{

		private Collection<busServiceCreditPlanFormulaRef> _iclbServiceCreditPlanFormulaRef;
		public Collection<busServiceCreditPlanFormulaRef> iclbServiceCreditPlanFormulaRef
		{
			get
			{
				return _iclbServiceCreditPlanFormulaRef;
			}
			set
			{
				_iclbServiceCreditPlanFormulaRef = value;
			}
		}

		public void LoadServiceCreditPlanFormulaRefs(DataTable adtbSearchResult)
		{
			_iclbServiceCreditPlanFormulaRef = GetCollection<busServiceCreditPlanFormulaRef>(adtbSearchResult, "icdoServiceCreditPlanFormulaRef");
		}
	}
}
