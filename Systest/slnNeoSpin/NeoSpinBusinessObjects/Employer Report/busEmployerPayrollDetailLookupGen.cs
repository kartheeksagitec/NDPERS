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
	public partial class busEmployerPayrollDetailLookup  : busMainBase
	{
        private Collection<busEmployerPayrollDetail> _iclbPayrollDetail;
        public Collection<busEmployerPayrollDetail> iclbPayrollDetail
        {
            get
            {
                return _iclbPayrollDetail;
            }

            set
            {
                _iclbPayrollDetail = value;
            }
        }

        public void LoadPayrollDetail(DataTable adtbSearchResult)
        {
            _iclbPayrollDetail = GetCollection<busEmployerPayrollDetail>(adtbSearchResult, "icdoEmployerPayrollDetail");
        }
	}
}
