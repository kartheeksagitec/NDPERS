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
	public partial class busIbsHeader : busExtendBase
    {
		public busIbsHeader()
		{

		} 

		private cdoIbsHeader _icdoIbsHeader;
		public cdoIbsHeader icdoIbsHeader
		{
			get
			{
				return _icdoIbsHeader;
			}

			set
			{
				_icdoIbsHeader = value;
			}
		}

		private Collection<busIbsDetail> _icolIbsDetail;
		public Collection<busIbsDetail> icolIbsDetail
		{
			get
			{
				return _icolIbsDetail;
			}

			set
			{
				_icolIbsDetail = value;
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

		private Collection<busJsRhicRemittanceAllocation> _icolJsRhicRemittanceAllocation;
		public Collection<busJsRhicRemittanceAllocation> icolJsRhicRemittanceAllocation
		{
			get
			{
				return _icolJsRhicRemittanceAllocation;
			}

			set
			{
				_icolJsRhicRemittanceAllocation = value;
			}
		}

		public bool FindIbsHeader(int Aintibsheaderid)
		{
			bool lblnResult = false;
			if (_icdoIbsHeader == null)
			{
				_icdoIbsHeader = new cdoIbsHeader();
			}
			if (_icdoIbsHeader.SelectRow(new object[1] { Aintibsheaderid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

		

		public void LoadIbsRemittanceAllocations()
		{
			DataTable ldtbList = Select<cdoIbsRemittanceAllocation>(
				new string[1] { "ibs_header_id" },
				new object[1] { icdoIbsHeader.ibs_header_id }, null, null);
			_icolIbsRemittanceAllocation = GetCollection<busIbsRemittanceAllocation>(ldtbList, "icdoIbsRemittanceAllocation");
		}

		public void LoadJsRhicRemittanceAllocations()
		{
			DataTable ldtbList = Select<cdoJsRhicRemittanceAllocation>(
				new string[1] { "ibs_header_id" },
				new object[1] { icdoIbsHeader.ibs_header_id }, null, null);
			_icolJsRhicRemittanceAllocation = GetCollection<busJsRhicRemittanceAllocation>(ldtbList, "icdoJsRhicRemittanceAllocation");
		}
	}
}
