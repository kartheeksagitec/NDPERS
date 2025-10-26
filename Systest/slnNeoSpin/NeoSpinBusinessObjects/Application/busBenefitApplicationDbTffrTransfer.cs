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
    public class busBenefitApplicationDbTffrTransfer : busBenefitApplicationDbTffrTransferGen
    {
        public bool IsRecordExistsForSameDate()
        {
            if ((icdoBenefitApplicationDbTffrTransfer.start_date != DateTime.MinValue) &&
                (icdoBenefitApplicationDbTffrTransfer.end_date != DateTime.MinValue))
            {
                if (ibusBenefitApplication == null)
                    LoadbenefitApplication();
                if (ibusBenefitApplication.iclbBenefitApplicationDbTffrTransfer == null)
                    ibusBenefitApplication.LoadBenefitApplicationDbTffrTransfer();
                foreach (busBenefitApplicationDbTffrTransfer lobjBenefitApplicationDbTffrTransfer in ibusBenefitApplication.iclbBenefitApplicationDbTffrTransfer)
                {
                    if (icdoBenefitApplicationDbTffrTransfer.benefit_application_db_tffr_transfer_id != lobjBenefitApplicationDbTffrTransfer.icdoBenefitApplicationDbTffrTransfer.benefit_application_db_tffr_transfer_id)
                    {
                        if ((busGlobalFunctions.CheckDateOverlapping(icdoBenefitApplicationDbTffrTransfer.start_date,
                            lobjBenefitApplicationDbTffrTransfer.icdoBenefitApplicationDbTffrTransfer.start_date, lobjBenefitApplicationDbTffrTransfer.icdoBenefitApplicationDbTffrTransfer.end_date))
                            ||
                            (busGlobalFunctions.CheckDateOverlapping(icdoBenefitApplicationDbTffrTransfer.end_date,
                            lobjBenefitApplicationDbTffrTransfer.icdoBenefitApplicationDbTffrTransfer.start_date, lobjBenefitApplicationDbTffrTransfer.icdoBenefitApplicationDbTffrTransfer.end_date)))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        public bool IsStartDateLessthanPlanParticipationStartDate()
        {
            if (ibusBenefitApplication == null)
                LoadbenefitApplication();
            if(ibusBenefitApplication.ibusPersonAccount==null)
                ibusBenefitApplication.LoadPersonAccount();
            if (icdoBenefitApplicationDbTffrTransfer.start_date != DateTime.MinValue)
            {
                if (icdoBenefitApplicationDbTffrTransfer.start_date < ibusBenefitApplication.ibusPersonAccount.icdoPersonAccount.start_date)
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsEndDateNotLastDayOfTheMonth()
        {
            bool lblnResult = false;
            if (icdoBenefitApplicationDbTffrTransfer.end_date != DateTime.MinValue &&
                icdoBenefitApplicationDbTffrTransfer.end_date != icdoBenefitApplicationDbTffrTransfer.end_date.GetLastDayofMonth())
            {
                lblnResult = true;
            }
            return lblnResult;
        }
    }
}