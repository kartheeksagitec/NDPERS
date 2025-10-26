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
    public class busPersonAccountLifeOption : busPersonAccount
    {
        public bool iblnOverlapHistoryFound { get; set; }
        private cdoPersonAccountLifeOption _icdoPersonAccountLifeOption;
        public cdoPersonAccountLifeOption icdoPersonAccountLifeOption
        {
            get
            {
                return _icdoPersonAccountLifeOption;
            }
            set
            {
                _icdoPersonAccountLifeOption = value;
            }
        }

        public bool FindPersonAccountLifeOption(int Aintaccountlifeid)
        {
            bool lblnResult = false;
            if (_icdoPersonAccountLifeOption == null)
            {
                _icdoPersonAccountLifeOption = new cdoPersonAccountLifeOption();
            }
            if (_icdoPersonAccountLifeOption.SelectRow(new object[1] { Aintaccountlifeid }))
            {
                lblnResult = true;
            }
            return lblnResult;
        }

        public Collection<cdoPersonAccountLifeOption> LoadCoverageAmountByLevelofCoverage()
        {
            DataTable ldtbList = Select("cdoPersonAccountLife.LoadCoverageAmount", new object[1] { icdoPersonAccountLifeOption.level_of_coverage_value });
            Collection<cdoPersonAccountLifeOption> lclcCoverageAmount = Sagitec.DataObjects.doBase.GetCollection<cdoPersonAccountLifeOption>(ldtbList);
            return lclcCoverageAmount;
        }
        //PIR 25798
        public decimal iintCoverageAmountForCorr { get; set; }
    }
}