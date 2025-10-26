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
	public class busUsecase : busExtendBase
    {
		public busUsecase()
		{
		}

		private cdoUsecase _icdoUsecase;
		public cdoUsecase icdoUsecase
		{
			get
			{
				return _icdoUsecase;
			}

			set
			{
				_icdoUsecase = value;
			}
		}

		private Collection<busUsecaseFlow> _iclbUsecaseFlow;
		public Collection<busUsecaseFlow> iclbUsecaseFlow
		{
			get
			{
				return _iclbUsecaseFlow;
			}

			set
			{
				_iclbUsecaseFlow = value;
			}
		}

		
		public bool FindUsecase(int aintUsecaseId)
		{
			bool lblnResult = false;
			if (_icdoUsecase == null)
			{
				_icdoUsecase = new cdoUsecase();
			}
			if (_icdoUsecase.SelectRow(new object[1] { aintUsecaseId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

		public void LoadUsecaseFlow()
		{
			DataTable ldtbList = Select<cdoUsecaseFlow>(
				new string[1] { "usecase_id" },
				new object[1] { icdoUsecase.usecase_id }, null, null);
			_iclbUsecaseFlow = GetCollection<busUsecaseFlow>(ldtbList, "icdoUsecaseFlow");
		}

		protected override void LoadOtherObjects(DataRow adtrRow, busBase aobjBus)
		{
			if (aobjBus is busUsecaseFlow)
			{
			}
		}


	}
}
