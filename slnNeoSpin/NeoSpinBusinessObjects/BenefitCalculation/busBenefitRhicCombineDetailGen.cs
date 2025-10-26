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
    public class busBenefitRhicCombineDetailGen : busExtendBase
    {
        public busBenefitRhicCombineDetailGen()
        {

        }

        private cdoBenefitRhicCombineDetail _icdoBenefitRhicCombineDetail;
        public cdoBenefitRhicCombineDetail icdoBenefitRhicCombineDetail
        {
            get
            {
                return _icdoBenefitRhicCombineDetail;
            }
            set
            {
                _icdoBenefitRhicCombineDetail = value;
            }
        }

        public bool FindBenefitRhicCombineDetail(int Aintbenefitrhiccombinedetailid)
        {
            bool lblnResult = false;
            if (_icdoBenefitRhicCombineDetail == null)
            {
                _icdoBenefitRhicCombineDetail = new cdoBenefitRhicCombineDetail();
            }
            if (_icdoBenefitRhicCombineDetail.SelectRow(new object[1] { Aintbenefitrhiccombinedetailid }))
            {
                lblnResult = true;
            }
            return lblnResult;
        }
    }
}
