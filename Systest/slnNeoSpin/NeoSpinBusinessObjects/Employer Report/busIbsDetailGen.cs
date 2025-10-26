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
	public partial class busIbsDetail : busExtendBase
	{
		public busIbsDetail()
		{

		} 

		private cdoIbsDetail _icdoIbsDetail;
		public cdoIbsDetail icdoIbsDetail
		{
			get
			{
				return _icdoIbsDetail;
			}

			set
			{
				_icdoIbsDetail = value;
			}
		}

		private Collection<busIbsRemittanceAllocation> _icolIbsRemittanceAllocation;
		public Collection<busIbsRemittanceAllocation> icolIbsRemittanceAllocation
		{
			get
			{
				return _icolIbsRemittanceAllocation;
			}

			set
			{
				_icolIbsRemittanceAllocation = value;
			}
		}

		public bool FindIbsDetail(int Aintibsdetailid)
		{
			bool lblnResult = false;
			if (_icdoIbsDetail == null)
			{
				_icdoIbsDetail = new cdoIbsDetail();
			}
			if (_icdoIbsDetail.SelectRow(new object[1] { Aintibsdetailid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}
        private busPersonAccountGhdv _ibusPersonAccountGHDV;

        public busPersonAccountGhdv ibusPersonAccountGHDV
        {
            get { return _ibusPersonAccountGHDV; }
            set { _ibusPersonAccountGHDV = value; }
        }
        private busPersonAccountLife _ibusPersonAccountLife;

        public busPersonAccountLife ibusPersonAccountLife
        {
            get { return _ibusPersonAccountLife; }
            set { _ibusPersonAccountLife = value; }
        }
        private busPersonAccountLtc _ibusPersonAccountLtc;

        public busPersonAccountLtc ibusPersonAccountLtc
        {
            get { return _ibusPersonAccountLtc; }
            set { _ibusPersonAccountLtc = value; }
        }

		public void LoadIbsRemittanceAllocations()
		{
			DataTable ldtbList = Select<cdoIbsRemittanceAllocation>(
				new string[1] { "ibs_detail_id" },
				new object[1] { icdoIbsDetail.ibs_detail_id }, null, null);
			_icolIbsRemittanceAllocation = GetCollection<busIbsRemittanceAllocation>(ldtbList, "icdoIbsRemittanceAllocation");
		}

       
	}
}
