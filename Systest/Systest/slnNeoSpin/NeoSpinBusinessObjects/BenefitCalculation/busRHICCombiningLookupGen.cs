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
	public class busRHICCombiningLookupGen : busMainBase
	{
        private Collection<busBenefitRhicCombine> _iclbRHICCombining;
        public Collection<busBenefitRhicCombine> iclbRHICCombining
        {
            get { return _iclbRHICCombining; }
            set { _iclbRHICCombining = value; }
        }

        public void LoadBenefitRHICCombining(DataTable adtRHICCombining)
        {
            if (_iclbRHICCombining == null)
                _iclbRHICCombining = new Collection<busBenefitRhicCombine>();
            _iclbRHICCombining = GetCollection<busBenefitRhicCombine>(adtRHICCombining, "icdoBenefitRhicCombine");
        }       
	}
}
