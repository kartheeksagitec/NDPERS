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
using System.Linq;
#endregion

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busPersonEmploymentDetailGen : busExtendBase
    {
        public busPersonEmploymentDetailGen()
        {

        }

        public string istrIsDBPlan
        {
            get { return busConstant.Flag_Yes; }
        }
        public string istrIsDCPlan
        {
            get { return busConstant.Flag_Yes; }
        }

        public string istrIs457
        {
            get { return busConstant.Flag_Yes; }
        }
        #region Boolean Properties for Plan
        private bool _iblnIsPlanDC;

        public bool iblnIsPlanDC
        {
            get { return _iblnIsPlanDC; }
            set { _iblnIsPlanDC = value; }
        }
        private bool _iblnIsPlanDB;

        public bool iblnIsPlanDB
        {
            get { return _iblnIsPlanDB; }
            set { _iblnIsPlanDB = value; }
        }
		//PIR 25920 New DC Plan
        private bool _iblnIsPlanHB;
        public bool iblnIsPlanHB
        {
            get { return _iblnIsPlanHB; }
            set { _iblnIsPlanHB = value; }
        }
        private bool _iblnIsPlanTFFR;

        public bool iblnIsPlanTFFR
        {
            get { return _iblnIsPlanTFFR; }
            set { _iblnIsPlanTFFR = value; }
        }
        private bool _iblnIsPlanTIAA;

        public bool iblnIsPlanTIAA
        {
            get { return _iblnIsPlanTIAA; }
            set { _iblnIsPlanTIAA = value; }
        }
        private bool _iblnIsPlanDeferredComp;

        public bool iblnIsPlanDeferredComp
        {
            get { return _iblnIsPlanDeferredComp; }
            set { _iblnIsPlanDeferredComp = value; }
        }
        private bool _iblnIsPlanLTC;

        public bool iblnIsPlanLTC
        {
            get { return _iblnIsPlanLTC; }
            set { _iblnIsPlanLTC = value; }
        }
        private bool _iblnIsPlanGrouypHealth;

        public bool iblnIsPlanGrouypHealth
        {
            get { return _iblnIsPlanGrouypHealth; }
            set { _iblnIsPlanGrouypHealth = value; }
        }
        private bool _iblnIsPlanGroupDental;

        public bool iblnIsPlanGroupDental
        {
            get { return _iblnIsPlanGroupDental; }
            set { _iblnIsPlanGroupDental = value; }
        }
        private bool _iblnIsPlanGroupVision;
        public bool iblnIsPlanGroupVision
        {
            get { return _iblnIsPlanGroupVision; }
            set { _iblnIsPlanGroupVision = value; }
        }
        private bool _iblnIsPlanOther457;
        public bool iblnIsPlanOther457
        {
            get { return _iblnIsPlanOther457; }
            set { _iblnIsPlanOther457 = value; }
        }
        private bool _iblnIsPlanHMO;
        public bool iblnIsPlanHMO
        {
            get { return _iblnIsPlanHMO; }
            set { _iblnIsPlanHMO = value; }
        }
        private bool _iblnIsPlanEAP;
        public bool iblnIsPlanEAP
        {
            get { return _iblnIsPlanEAP; }
            set { _iblnIsPlanEAP = value; }
        }
        private bool _iblnIsPlanMedicare;
        public bool iblnIsPlanMedicare
        {
            get { return _iblnIsPlanMedicare; }
            set { _iblnIsPlanMedicare = value; }
        }
        private bool _iblnIsPlanGroupLife;
        public bool iblnIsPlanGroupLife
        {
            get { return _iblnIsPlanGroupLife; }
            set { _iblnIsPlanGroupLife = value; }
        }
        private bool _iblnIsPlanFlexComp;
        public bool iblnIsPlanFlexComp
        {
            get { return _iblnIsPlanFlexComp; }
            set { _iblnIsPlanFlexComp = value; }
        }

        private bool _iblnIsPlanDC2020;

        public bool iblnIsPlanDC2020
        {
            get { return _iblnIsPlanDC2020; }
            set { _iblnIsPlanDC2020 = value; }
        }
        #endregion
        public int iintPlanIDDental
        {
            get { return busConstant.PlanIdDental; }
        }

        public int iintPlanIdVision
        {
            get { return busConstant.PlanIdVision; }
        }
        public int iintPlanIdGroupHealth
        {
            get { return busConstant.PlanIdGroupHealth; }
        }
        public int iintPlanIdHMO
        {
            get { return busConstant.PlanIdHMO; }
        }

        public int iintPlanidMedicare
        {
            get { return busConstant.PlanIdMedicarePartD; }
        }
        //this is used in SFN - 54366 17627
        //to get the employer name
        public int iintPlanIDMain
        {
            get { return busConstant.PlanIdMain; }
        }

        //PIR 20232 ?code
        public int iintPlanIDMain2020
        {
            get { return busConstant.PlanIdMain2020; }
        }
        public int iintPlanIDDC
        {
            get { return busConstant.PlanIdDC; }
        }
        //PIR 20232 ?code
        public int iintPlanIDDC2020
        {
            get { return busConstant.PlanIdDC2020; }
        }
      
        private cdoPersonEmploymentDetail _icdoPersonEmploymentDetail;
        public cdoPersonEmploymentDetail icdoPersonEmploymentDetail
        {
            get
            {
                return _icdoPersonEmploymentDetail;
            }
            set
            {
                _icdoPersonEmploymentDetail = value;
            }
        }
        private busPersonEmployment _ibusPersonEmployment;
        public busPersonEmployment ibusPersonEmployment
        {
            get
            {
                return _ibusPersonEmployment;
            }
            set
            {
                _ibusPersonEmployment = value;
            }
        }
        private Collection<busPersonAccount> _icolPersonAccount;
        public Collection<busPersonAccount> icolPersonAccount
        {
            get
            {
                return _icolPersonAccount;
            }

            set
            {
                _icolPersonAccount = value;
            }
        }       

        public bool FindPersonEmploymentDetail(int AintPersonEmploymentDetailID)
        {
            bool lblnResult = false;
            if (_icdoPersonEmploymentDetail == null)
            {
                _icdoPersonEmploymentDetail = new cdoPersonEmploymentDetail();
            }
            if (_icdoPersonEmploymentDetail.SelectRow(new object[1] { AintPersonEmploymentDetailID }))
            {
                lblnResult = true;
            }
            return lblnResult;
        }
        public void LoadPlansOffered()
        {
            if (ibusPersonEmployment == null)
                LoadPersonEmployment();
            if (ibusPersonEmployment.ibusOrganization == null)
                ibusPersonEmployment.LoadOrganization();
            // 1) Load all plans as of current date
            // 2) PROD PIR 4526 -- Load all plans as of employment detail start date
            // 3) PROD PIR 6136 -- Load all plans the plans irrespective of dates
            //PIR 21118 
            if (ibusPersonEmployment.ibusOrganization.iclbOrganizationOfferedPlans == null)
                ibusPersonEmployment.ibusOrganization.LoadOrganizationOfferedPlans(DateTime.MaxValue, LoadEarliestContributingEmpDetailStartDate(), ibusPersonEmployment.icdoPersonEmployment.person_id);
        }
        //PIR 21118 - Load earliest Employment_Detail_Start_Date for that Person_Employment_ID where the Status_Value = CONT 
        public DateTime LoadEarliestContributingEmpDetailStartDate()
        {
            if (ibusPersonEmployment.icolPersonEmploymentDetail.IsNull())
                ibusPersonEmployment.LoadPersonEmploymentDetail(false);
            busPersonEmploymentDetail lbusPersonEmploymentDetail = ibusPersonEmployment.icolPersonEmploymentDetail.LastOrDefault(detail => detail.icdoPersonEmploymentDetail.status_value == busConstant.EmploymentStatusContributing);
            return lbusPersonEmploymentDetail.IsNotNull() ? lbusPersonEmploymentDetail.icdoPersonEmploymentDetail.start_date : this.icdoPersonEmploymentDetail.start_date;
        }
        public void LoadPersonEmployment()
        {
            if (_ibusPersonEmployment == null)
            {
                _ibusPersonEmployment = new busPersonEmployment { icdoPersonEmployment = new cdoPersonEmployment() };
            }
            else if (_ibusPersonEmployment.icdoPersonEmployment == null)
            {
                _ibusPersonEmployment.icdoPersonEmployment = new cdoPersonEmployment();
            }
            _ibusPersonEmployment.FindPersonEmployment(_icdoPersonEmploymentDetail.person_employment_id);
        }
        public void LoadPlansByEmployer()
        {
            _icolPersonAccount = new Collection<busPersonAccount>();
            DataTable ldtbPersonAccount = Select<cdoPersonAccount>(
              new string[1] { "person_id" },
              new object[1] { ibusPersonEmployment.icdoPersonEmployment.person_id }, null, null);
            Collection<busPersonAccount> lclbPersonAccount = GetCollection<busPersonAccount>(ldtbPersonAccount, "icdoPersonAccount");
            foreach (busPersonAccount lobjPersonAccount in lclbPersonAccount)
            {
                bool lblnAddToPersonAccount = false;
                DataTable ldtbPersonAccountEmpDtl =
                    Select<cdoPersonAccountEmploymentDetail>(
                        new string[3] { "person_employment_dtl_id", "plan_id", "person_account_id" },
                        new object[3]
                            {
                                icdoPersonEmploymentDetail.person_employment_dtl_id,
                                lobjPersonAccount.icdoPersonAccount.plan_id,
                                lobjPersonAccount.icdoPersonAccount.person_account_id
                            }, null, null);

                if (ldtbPersonAccountEmpDtl.Rows.Count > 0)
                {
                    lblnAddToPersonAccount = true;
                }
                if (lblnAddToPersonAccount)
                {
                    lobjPersonAccount.LoadPlan();
                    _icolPersonAccount.Add(lobjPersonAccount);
                }
            }
        }
    }
}
