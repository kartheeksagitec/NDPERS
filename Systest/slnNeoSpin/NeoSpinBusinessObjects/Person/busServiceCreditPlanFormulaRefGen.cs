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
	public class busServiceCreditPlanFormulaRefGen : busExtendBase
    {
		public busServiceCreditPlanFormulaRefGen()
		{

		} 

		private cdoServiceCreditPlanFormulaRef _icdoServiceCreditPlanFormulaRef;
		public cdoServiceCreditPlanFormulaRef icdoServiceCreditPlanFormulaRef
		{
			get
			{
				return _icdoServiceCreditPlanFormulaRef;
			}

			set
			{
				_icdoServiceCreditPlanFormulaRef = value;
			}
		}

		public bool FindServiceCreditPlanFormulaRef(int Aintservicecreditplanformularefid)
		{
			bool lblnResult = false;
			if (_icdoServiceCreditPlanFormulaRef == null)
			{
				_icdoServiceCreditPlanFormulaRef = new cdoServiceCreditPlanFormulaRef();
			}
			if (_icdoServiceCreditPlanFormulaRef.SelectRow(new object[1] { Aintservicecreditplanformularefid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}
	}
}
