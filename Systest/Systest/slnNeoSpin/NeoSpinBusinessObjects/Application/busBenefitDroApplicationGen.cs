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
using Sagitec.CustomDataObjects;
#endregion

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busBenefitDroApplicationGen : busPersonBase
    {
        public busBenefitDroApplicationGen()
        {

        }


        private bool _iblnApproveOrQualifyButtonClick;

        public bool iblnApproveOrQualifyButtonClick
        {
            get { return _iblnApproveOrQualifyButtonClick; }
            set { _iblnApproveOrQualifyButtonClick = value; }
        }

        private bool _iblnSaveClick;

        public bool iblnSaveClick
        {
            get { return _iblnSaveClick; }
            set { _iblnSaveClick = value; }
        }

        public bool iblnIntialSaveClick { get; set; }

        private string _Is18MonthsNotice;

        public string Is18MonthsNotice
        {
            get { return _Is18MonthsNotice; }
            set { _Is18MonthsNotice = value; }
        }
        public DateTime idtExpirationDate
        {
            get
            {
                if (icdoBenefitDroApplication.received_date != DateTime.MinValue)
                {
                    return icdoBenefitDroApplication.received_date.AddDays(548);
                }
                else
                {
                    return DateTime.MinValue;
                }
            }
        }
        public string istrExpirationDate
        {
            get
            {
                if (idtExpirationDate != DateTime.MinValue)
                    return idtExpirationDate.ToString(busConstant.DateFormatLongDate);
                else
                    return string.Empty;
            }
        }

        private cdoBenefitDroApplication _icdoBenefitDroApplication;
        public cdoBenefitDroApplication icdoBenefitDroApplication
        {
            get
            {
                return _icdoBenefitDroApplication;
            }
            set
            {
                _icdoBenefitDroApplication = value;
            }
        }

        public bool FindBenefitDroApplication(int Aintdroapplicationid)
        {
            bool lblnResult = false;
            if (_icdoBenefitDroApplication == null)
            {
                _icdoBenefitDroApplication = new cdoBenefitDroApplication();
            }
            if (_icdoBenefitDroApplication.SelectRow(new object[1] { Aintdroapplicationid }))
            {
                lblnResult = true;
            }
            return lblnResult;
        }
        public bool FindBenefitDroApplicationBypersonID(int Aintpersonid)
        {
            bool lblnResult = false;
            if (_icdoBenefitDroApplication == null)
            {
                _icdoBenefitDroApplication = new cdoBenefitDroApplication();
            }
            DataTable ldtbList = Select<cdoBenefitDroApplication>(
                                  new string[1] { "member_perslink_id" }, new object[1] { Aintpersonid }, null, null);
            if (ldtbList.Rows.Count == 1)
            {
                _icdoBenefitDroApplication.LoadData(ldtbList.Rows[0]);
                lblnResult = true;
            }
            else
            {
                lblnResult = false;
            }
            return lblnResult;
        }
        private busBenefitDroCalculation _ibusBenefitDroCalculation;
        public busBenefitDroCalculation ibusBenefitDroCalculation
        {
            get { return _ibusBenefitDroCalculation; }
            set { _ibusBenefitDroCalculation = value; }
        }

        private busPerson _ibusMember;
        public busPerson ibusMember
        {
            get { return _ibusMember; }
            set { _ibusMember = value; }
        }

        public void LoadMember()
        {
            if (ibusMember == null)
                ibusMember = new busPerson();
            ibusMember.FindPerson(icdoBenefitDroApplication.member_perslink_id);
        }

        private busPerson _ibusAlternatePayee;
        public busPerson ibusAlternatePayee
        {
            get { return _ibusAlternatePayee; }
            set { _ibusAlternatePayee = value; }
        }

        public void LoadAlternatePayee()
        {
            if (ibusAlternatePayee == null)
                ibusAlternatePayee = new busPerson();
            ibusAlternatePayee.FindPerson(icdoBenefitDroApplication.alternate_payee_perslink_id);
        }

        private busPlan _ibusPlan;
        public busPlan ibusPlan
        {
            get { return _ibusPlan; }
            set { _ibusPlan = value; }
        }

        public void LoadPlan()
        {
            if (ibusPlan == null)
                ibusPlan = new busPlan();
            ibusPlan.FindPlan(icdoBenefitDroApplication.plan_id);
        }

        private busPersonAccount _ibusPersonAccount;
        public busPersonAccount ibusPersonAccount
        {
            get
            {
                return _ibusPersonAccount;
            }
            set
            {
                _ibusPersonAccount = value;
            }
        }
        private Collection<cdoCodeValue> _iclcCodeValue;
        public Collection<cdoCodeValue> iclcCodeValue
        {
            get { return _iclcCodeValue; }
            set { _iclcCodeValue = value; }
        }

        public Collection<cdoCodeValue> iclcBenefiReceipt { get; set; }

        public void LoadPersonAccount()
        {
            if (_ibusPersonAccount == null)
            {
                _ibusPersonAccount = new busPersonAccount();
            }
            _ibusPersonAccount.FindPersonAccount(icdoBenefitDroApplication.person_account_id);
        }
        //prop to load retirement person account information
        public busPersonAccountRetirement ibusPersonAccountRetirement { get; set; }
        public void LoadRetirementPersonAccount()
        {
            if (ibusPersonAccountRetirement == null)
            {
                ibusPersonAccountRetirement = new busPersonAccountRetirement();
            }
            ibusPersonAccountRetirement.FindPersonAccountRetirement(icdoBenefitDroApplication.person_account_id);
        }

        private Collection<busBenefitRefundApplication> _iclbBenefitRefundApplication;
        public Collection<busBenefitRefundApplication> iclbBenefitRefundApplication
        {
            get { return _iclbBenefitRefundApplication; }
            set { _iclbBenefitRefundApplication = value; }
        }

        public void LoadRefundApplication()
        {
            DataTable ldtbList = Select<cdoBenefitApplication>(
                                    new string[2] { "member_person_id", "plan_id" },
                                    new object[2] { icdoBenefitDroApplication.member_perslink_id, icdoBenefitDroApplication.plan_id }, null, null);
            iclbBenefitRefundApplication = GetCollection<busBenefitRefundApplication>(ldtbList, "icdoBenefitApplication");
        }

        //this methos used in checkin whether refuind application exists or not
        public bool IsDROApplicationCancelledOrDenied()
        {
            if (icdoBenefitDroApplication.dro_status_value == busConstant.DROApplicationStatusDenied)
            {
                return true;
            }
            else if (icdoBenefitDroApplication.dro_status_value == busConstant.DROApplicationStatusCancelled)
            {
                return true;
            }
            return false;
        }

        //to set the visiblity for all controls
        public bool IsDROApplicationCancelledOrDeniedOrCancelledOrQualified()
        {
            if (IsDROApplicationCancelledOrDenied())
            {
                return true;
            }
            else if (icdoBenefitDroApplication.dro_status_value == busConstant.DROApplicationStatusNullified)
            {
                return true;
            }
            else if (icdoBenefitDroApplication.dro_status_value == busConstant.DROApplicationStatusQualified)
            {
                return true;
            }
            else if (icdoBenefitDroApplication.dro_status_value == busConstant.DROApplicationStatusPendingNullified)
                return true;
            return false;
        }


        public bool IsDROApplicationApprovedOrReceivedOrQualified()
        {
            if (icdoBenefitDroApplication.dro_status_value == busConstant.DROApplicationStatusRecieved)
            {
                return true;
            }
            else if (icdoBenefitDroApplication.dro_status_value == busConstant.DROApplicationStatusApproved)
            {
                return true;
            }
            else if (icdoBenefitDroApplication.dro_status_value == busConstant.DROApplicationStatusQualified)
            {
                return true;
            }
            return false;
        }
        //To set the Visibility for btnApproveCopy
        public bool IsStatusQualified()
        {
            if (icdoBenefitDroApplication.dro_status_value == busConstant.DROApplicationStatusQualified)
            {
                if (iobjPassInfo.istrUserID != icdoBenefitDroApplication.qualified_by_user)
                {
                    return true;
                }
            }
            return false;
        }
        //To set the visibility for monthly benefit tab
        public bool IsMonthlyBenefitTabVisible()
        {
            if (IsDROModelDBOriented())
            {
                return true;
            }
            return false;
        }
        public bool IsDROModelDBOriented()
        {
            if (icdoBenefitDroApplication.dro_model_value == busConstant.DROApplicationModelActiveDBFormerModel)
            {
                return true;
            }
            else if (icdoBenefitDroApplication.dro_model_value == busConstant.DROApplicationModelActiveDBModel)
            {
                return true;
            }
            else if (icdoBenefitDroApplication.dro_model_value == busConstant.DROApplicationModelRetireeDBModel)
            {
                return true;
            }
            else if (icdoBenefitDroApplication.dro_model_value == busConstant.DROApplicationModelActiveJobServiceModel)
            {
                return true;
            }
            else if (icdoBenefitDroApplication.dro_model_value == busConstant.DROApplicationModelRetiredJobServiceModel)
            {
                return true;
            }
            return false;
        }
        //To set the visibility for refund tab
        public bool IsRefundScheduleTabVisible()
        {
            if (IsDROModelRefundOriented())
            {
                return true;
            }
            return false;
        }
        public bool IsDROModelRefundOriented()
        {
            if (icdoBenefitDroApplication.dro_model_value == busConstant.DROApplicationModelActiveDBModel)
            {
                return true;
            }
            else if (icdoBenefitDroApplication.dro_model_value == busConstant.DROApplicationModelActiveDBFormerModel)
            {
                return true;
            }
            else if (icdoBenefitDroApplication.dro_model_value == busConstant.DROApplicationModelActiveJobServiceModel)
            {
                return true;
            }
            return false;
        }
        //To set the visibility for refund lumpsum tab
        public bool IsRefundScheduleLumpsumTabVisible()
        {
            if (IsDROModelRefundLumpSumOriented())
            {
                return true;
            }
            return false;
        }
        public bool IsDROModelRefundLumpSumOriented()
        {
            if (icdoBenefitDroApplication.dro_model_value == busConstant.DROApplicationModelDeferredCompModel)
            {
                return true;
            }

            else if (icdoBenefitDroApplication.dro_model_value == busConstant.DROApplicationModelDCModel)
            {
                return true;
            }

            return false;
        }

        //To set the visibility for death schedule tab
        public bool IsDeathScheduleTabVisible()
        {
            if (IsDROModelDeathOriented())
            {
                return true;
            }
            return false;
        }
        public bool IsDROModelDeathOriented()
        {
            if (icdoBenefitDroApplication.dro_model_value == busConstant.DROApplicationModelActiveDBModel)
            {
                return true;
            }
            else if (icdoBenefitDroApplication.dro_model_value == busConstant.DROApplicationModelActiveDBFormerModel)
            {
                return true;
            }
            //uat pir - 928,1313
            else if (icdoBenefitDroApplication.dro_model_value == busConstant.DROApplicationModelActiveJobServiceModel)
                return true;
            return false;
        }
        //Fill the Duration Of Benefit Option DropDownList Based on the DRO Model
        public Collection<cdoCodeValue> LoadTimeofBenefitReceipDROModel()
        {
            string lstrData1 = string.Empty;
            DataTable ldtbList = new DataTable();
            if (icdoBenefitDroApplication.dro_model_value == busConstant.DROApplicationModelActiveJobServiceModel)
            {
                lstrData1 = busConstant.Flag_Yes;
                ldtbList = iobjPassInfo.isrvDBCache.GetCodeValues(2403, lstrData1, null, null);
            }
            else
            {
                ldtbList = iobjPassInfo.isrvDBCache.GetCodeValues(2403,null,null,null);
            }
            iclcBenefiReceipt = new Collection<cdoCodeValue>();

            iclcBenefiReceipt = Sagitec.DataObjects.doBase.GetCollection<cdoCodeValue>(ldtbList);

            return iclcBenefiReceipt;
        }
        //Fill the Duration Of Benefit Option DropDownList Based on the DRO Model
        public Collection<cdoCodeValue> LoadDurationOfBenefitOptionByDROModel()
        {
            iclcCodeValue = new Collection<cdoCodeValue>();
            DataTable ldtbList = iobjPassInfo.isrvDBCache.GetCodeValues(2404, icdoBenefitDroApplication.dro_model_value, null, null);
            iclcCodeValue = Sagitec.DataObjects.doBase.GetCollection<cdoCodeValue>(ldtbList);
            return iclcCodeValue;
        }

        //Get Total VSC for that person 
        //bool property is passed in order to check if the plan is job service or not.
        //returened value is then rounded to decimal 4.
        public decimal GetRoundedTVSC()
        {
            if (ibusMember == null)
                LoadMember();
            bool lblnIsPlanJobService = false;
            if (icdoBenefitDroApplication.plan_id == busConstant.PlanIdJobService)
                lblnIsPlanJobService = true;
            // added received date as parameter done by Deepa
            // in order to get only the contribution those falls below this date
            return Math.Round(ibusMember.GetTotalVSCForPerson(lblnIsPlanJobService, icdoBenefitDroApplication.received_date), 4, MidpointRounding.AwayFromZero);
        }

        private DateTime _idtNormalRetirementDate;

        public DateTime idtNormalRetirementDate
        {
            get { return _idtNormalRetirementDate; }
            set { _idtNormalRetirementDate = value; }
        }
        private DateTime _idtRetirementDate;

        public DateTime idtRetirementDate
        {
            get { return _idtRetirementDate; }
            set { _idtRetirementDate = value; }
        }
        private DateTime _idtEarlyRetirementDate;

        public DateTime idtEarlyRetirementDate
        {
            get { return _idtEarlyRetirementDate; }
            set { _idtEarlyRetirementDate = value; }
        }

        # region UCS- 54
        public Collection<busPayeeAccount> iclbPayeeAccountByDROAppId { get; set; }

        public void LoadPayeeAccountByDROApplicationID()
        {
            DataTable ldtbResult = Select<cdoPayeeAccount>(
                                         new string[1] { "dro_application_id" },
                                         new object[1] { icdoBenefitDroApplication.dro_application_id }, null, null);
            iclbPayeeAccountByDROAppId = GetCollection<busPayeeAccount>(ldtbResult, "icdoPayeeAccount");
        }
        # endregion

    }
}