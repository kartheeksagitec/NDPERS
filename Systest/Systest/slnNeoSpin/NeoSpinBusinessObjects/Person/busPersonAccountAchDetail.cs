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
    public class busPersonAccountAchDetail : busPersonAccountAchDetailGen
    {
        //If Ach Start Date is less than  next benefit payment date or not first of month,then throw an error BR-23

        public bool IsStartdateValid()
        {
            if (icdoPersonAccountAchDetail.ach_start_date != DateTime.MinValue)
            {                
                if (icdoPersonAccountAchDetail.ach_start_date < busIBSHelper.GetLastPostedIBSBatchDate().AddMonths(1))
                {
                    return true;
                }
                else if (icdoPersonAccountAchDetail.ach_start_date >= busIBSHelper.GetLastPostedIBSBatchDate().AddMonths(1))
                {
                    if (icdoPersonAccountAchDetail.ach_start_date.Day != 1)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        //If Ach End Date is less than or equal to next benefit payment date or one day before next benefit payment date ,then throw an error BR-24

        public bool IsEnddateValid()
         {
            if ((icdoPersonAccountAchDetail.ach_start_date != DateTime.MinValue) && (icdoPersonAccountAchDetail.ach_end_date != DateTime.MinValue))
            {   
                if (icdoPersonAccountAchDetail.ach_end_date < busIBSHelper.GetLastPostedIBSBatchDate().AddMonths(1).AddDays(-1))
                {
                    return true;
                }               
            }
            return false;
        }
        public override void BeforeValidate(utlPageMode aenmPageMode)
        {
            if (icdoPersonAccountAchDetail.org_code != null)
            {
                LoadBankOrgByOrgCode();
                icdoPersonAccountAchDetail.bank_org_id = ibusBankOrg.icdoOrganization.org_id;
            }
            base.BeforeValidate(aenmPageMode);
        }
        public bool IsACHRecordExist()
        {
            if (ibusPersonAccount == null)            
                LoadPersonAccount();            
            if (ibusPersonAccount.iclbPersonAccountAchDetail == null)
                ibusPersonAccount.LoadPersonAccountAchDetail();
            if (icdoPersonAccountAchDetail.ach_start_date != DateTime.MinValue)
            {
                foreach (busPersonAccountAchDetail lobjAchDetail in ibusPersonAccount.iclbPersonAccountAchDetail)
                {
                    if (lobjAchDetail.icdoPersonAccountAchDetail.person_account_ach_detail_id != icdoPersonAccountAchDetail.person_account_ach_detail_id)
                    {
                        if (busGlobalFunctions.CheckDateOverlapping(icdoPersonAccountAchDetail.ach_start_date, lobjAchDetail.icdoPersonAccountAchDetail.ach_start_date,
                            lobjAchDetail.icdoPersonAccountAchDetail.ach_end_date))
                        {
                            return true;
                        }
                        else if (busGlobalFunctions.CheckDateOverlapping(lobjAchDetail.icdoPersonAccountAchDetail.ach_start_date, icdoPersonAccountAchDetail.ach_start_date, icdoPersonAccountAchDetail.ach_end_date))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}