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
	public class busBenefitApplicationDbTffrTransferGen : busExtendBase
    {
		public busBenefitApplicationDbTffrTransferGen()
		{

		}

        private cdoBenefitApplicationDbTffrTransfer _icdoBenefitApplicationDbTffrTransfer;
        public cdoBenefitApplicationDbTffrTransfer icdoBenefitApplicationDbTffrTransfer
        {
            get
            {
                return _icdoBenefitApplicationDbTffrTransfer;
            }
            set
            {
                _icdoBenefitApplicationDbTffrTransfer = value;
            }
        }

        public bool FindBenefitApplicationDbTffrTransfer(int Aintbenefitapplicationdbtffrtransferid)
        {
            bool lblnResult = false;
            if (_icdoBenefitApplicationDbTffrTransfer == null)
            {
                _icdoBenefitApplicationDbTffrTransfer = new cdoBenefitApplicationDbTffrTransfer();
            }
            if (_icdoBenefitApplicationDbTffrTransfer.SelectRow(new object[1] { Aintbenefitapplicationdbtffrtransferid }))
            {
                lblnResult = true;
            }
            return lblnResult;
        }
        private busBenefitApplication _ibusBenefitApplication;

        public busBenefitApplication ibusBenefitApplication
        {
            get { return _ibusBenefitApplication; }
            set { _ibusBenefitApplication = value; }
        }
        public void LoadbenefitApplication()
        {
            if (_ibusBenefitApplication == null)
                _ibusBenefitApplication = new busBenefitApplication();
            _ibusBenefitApplication.FindBenefitApplication(icdoBenefitApplicationDbTffrTransfer.benefit_application_id);
        }

	}
}
