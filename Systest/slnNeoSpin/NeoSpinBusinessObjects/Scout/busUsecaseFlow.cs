#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using NeoSpin.CustomDataObjects;

#endregion

namespace NeoSpin.BusinessObjects
{
	[Serializable]
	public class busUsecaseFlow : busExtendBase
    {
		public busUsecaseFlow()
		{
		}

		private cdoUsecaseFlow _icdoUsecaseFlow;
		public cdoUsecaseFlow icdoUsecaseFlow
		{
			get
			{
				return _icdoUsecaseFlow;
			}

			set
			{
				_icdoUsecaseFlow = value;
			}
		}

		private busUsecase _ibusUsecase;
		public busUsecase ibusUsecase
		{
			get
			{
				return _ibusUsecase;
			}
			set
			{
				_ibusUsecase = value;
			}
		}

		public bool FindUsecaseFlow(int aintFlowId)
		{
			bool lblnResult = false;
			if (_icdoUsecaseFlow == null)
			{
				_icdoUsecaseFlow = new cdoUsecaseFlow();
			}
			if (_icdoUsecaseFlow.SelectRow(new object[1] { aintFlowId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

		public void LoadUsecase()
		{
			if (_ibusUsecase == null) 
			{
				_ibusUsecase = new busUsecase();
			}
			_ibusUsecase.FindUsecase(_icdoUsecaseFlow.usecase_id);
		}
	}
}
