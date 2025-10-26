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
using Sagitec.DataObjects;
using System.Linq;
using System.Collections.Generic;
using NeoSpin.DataObjects;


#endregion

namespace NeoSpin.BusinessObjects
{
    public partial class busOrgContact : busExtendBase
    {
        private busOrgContactAddress _ibusOrgAndContactPrimaryAddress;
        public busOrgContactAddress ibusOrgAndContactPrimaryAddress
        {
            get
            {
                return _ibusOrgAndContactPrimaryAddress;
            }

            set
            {
                _ibusOrgAndContactPrimaryAddress = value;
            }
        }
        //Prop used to fix PIR 1172
        public cdoOrgContactRole icdoContactRole { get; set; }



        

        private busPlan _ibusPlan;
        public busPlan ibusPlan
        {
            get
            {
                return _ibusPlan;
            }

            set
            {
                _ibusPlan = value;
            }
        }

        //PIR 271
        private Collection<cdoPlan> _iclcPlan;
        public Collection<cdoPlan> iclcPlan
        {
            get
            {
                return _iclcPlan;
            }

            set
            {
                _iclcPlan = value;
            }
        }

        //this property is used to display role value in lookup screen
        private string _istrRoleValue;
        public string istrRoleValue
        {
            get { return _istrRoleValue; }
            set { _istrRoleValue = value; }
        }

        private string _istrOrgCode;
        public string istrOrgCode
        {
            get { return _istrOrgCode; }
            set { _istrOrgCode = value; }
        }

        private Collection<busSeminarSchedule> _icolSeminarAttendance;
        public Collection<busSeminarSchedule> icolSeminarAttendance
        {
            get { return _icolSeminarAttendance; }
            set { _icolSeminarAttendance = value; }
        }
        public bool iblnIsOrgProvideRetirementPlan { get; set; }
        public bool iblnIsOrgProvideInsurancePlan { get; set; }
        public bool iblnIsOrgProvideFlexPlan { get; set; }
        public bool iblnIsOrgProvideDefCompPlan { get; set; }
        private Collection<busOrgContactPlanAndRole> _iclbOrgContactPlanAndRole;
        public Collection<busOrgContactPlanAndRole> iclbOrgContactPlanAndRole
        {
            get { return _iclbOrgContactPlanAndRole; }
            set { _iclbOrgContactPlanAndRole = value; }
        }
        public bool iblnIsContactAffiliatedWithNoOrg { get; set; }
        public bool iblnIsRetriementPersist { get; set; }
        public bool iblnIsInsurancePersist { get; set; }
        public bool iblnIsDefCompPersist { get; set; }
        public bool iblnIsFlexPersist { get; set; }
        public bool iblnIsNoPlanPersist { get; set; }
        public bool iblnIsNewlyInserted { get; set; }
        public Collection<busScreenNotes> iclbScreenNotes { get; set; }
        public string istrPlans { get; set; }
        public string istrContactRole { get; set; }
        public string istrStatusValue { get; set; }

        public string istrConsolidatSetStatusValue { get; set; }
        public string istrSetStatusValueWhichWasOnLoad { get; set; }
        private Collection<busOrgContact> _iclbOtherOrgContacts;
        public Collection<busOrgContact> iclbOtherOrgContacts
        {
            get { return _iclbOtherOrgContacts; }
            set { _iclbOtherOrgContacts = value; }
        }
        private Collection<busOrgContact> _iclbOrgContactsAndPlan;
        public Collection<busOrgContact> iclbOrgContactsAndPlan
        {
            get { return _iclbOrgContactsAndPlan; }
            set { _iclbOrgContactsAndPlan = value; }
        }
        private Collection<busOrgContactAddress> _iclbOrgAndContactAddresses;
        public Collection<busOrgContactAddress> iclbOrgAndContactAddresses
        {
            get { return _iclbOrgAndContactAddresses; }
            set { _iclbOrgAndContactAddresses = value; }
        }
        private bool _iblnIsUpdateAddressClicked;
        public bool iblnIsUpdateAddressClicked
        {
            get
            {
                return _iblnIsUpdateAddressClicked;
            }
            set
            {
                _iblnIsUpdateAddressClicked = value;
            }
        }
        private string _istrSuppressWarning;

        public string istrSuppressWarning
        {
            get { return _istrSuppressWarning; }
            set { _istrSuppressWarning = value; }
        }
        private string _address_validate_flag;
        public string address_validate_flag
        {
            get
            {
                return _address_validate_flag;
            }

            set
            {
                _address_validate_flag = value;
            }
        }      
        public void LoadSeminarAttendance()
        {
            DataTable ldtbList = busNeoSpinBase.Select("cdoOrganization.LOAD_SEMINAR_ATTENDANCE", new object[1] { _icdoOrgContact.org_id });
            _icolSeminarAttendance = GetCollection<busSeminarSchedule>(ldtbList, "icdoSeminarSchedule");
        }
        private utlCollection<cdoOrgContactRole> _iclbOrgContactRole;

        public utlCollection<cdoOrgContactRole> iclbOrgContactRole
        {
            get { return _iclbOrgContactRole; }
            set { _iclbOrgContactRole = value; }
        }
        private utlCollection<cdoOrgContactRole> _iclbOrgContactGroupByRole;

        public utlCollection<cdoOrgContactRole> iclbOrgContactGroupByRole
        {
            get { return _iclbOrgContactGroupByRole; }
            set { _iclbOrgContactGroupByRole = value; }
        }
        public override void SetParentKey(Sagitec.DataObjects.doBase aobjBase)
        {
            cdoOrgContactRole lobjContactRole = (cdoOrgContactRole)aobjBase;
            lobjContactRole.org_contact_id = _icdoOrgContact.org_contact_id;
            //base.SetParentKey(aobjBase);
        }

        public void LoadContactTypes()
        {
            _iclbOrgContactRole = GetCollection<cdoOrgContactRole>(
                new string[1] { "org_contact_id" }, new object[1] { this.icdoOrgContact.org_contact_id }, null, null);
        }

        public void LoadContactPlanRolesGroupByRole()
        {
            _iclbOrgContactGroupByRole = new utlCollection<cdoOrgContactRole>();            
            DataTable ldtbList = busNeoSpinBase.Select("cdoOrgContactRole.GetContactPlanRolesGroupByRoles", new object[2] { this.icdoOrgContact.contact_id, this.icdoOrgContact.org_id });            
            foreach (DataRow adrRow in ldtbList.Rows)
            {
                icdoContactRole = new cdoOrgContactRole();
                icdoContactRole.LoadData(adrRow);
                icdoContactRole.contact_role_description = Convert.ToString(adrRow["contact_role_description"]);
                icdoContactRole.contact_role_value = Convert.ToString(adrRow["contact_role_value"]);
                _iclbOrgContactGroupByRole.Add(icdoContactRole);
            }
        }
        public void LoadContactPlanRoles()
        {
            _iclbOrgContactRole = new utlCollection<cdoOrgContactRole>();
            DataTable ldtbList = busNeoSpinBase.Select("cdoOrgContactRole.GetContactPlanRoles", new object[1] { this.icdoOrgContact.contact_id });
            foreach (DataRow adrRow in ldtbList.Rows)
            {
                icdoContactRole = new cdoOrgContactRole();
                icdoContactRole.LoadData(adrRow);
                _iclbOrgContactRole.Add(icdoContactRole);
            }
        }

        //To Check whether the selected Address is Active before setting Primary Address.
        private busOrgContactAddress _ibusOrgcontactAddress;
        public busOrgContactAddress ibusOrgContactAddress
        {
            get { return _ibusOrgcontactAddress; }
            set { _ibusOrgcontactAddress = value; }
        }
        public ArrayList SetPrimaryAddress(int aintAddressId)
        {
            ArrayList larrError = new ArrayList();
            utlError lobjError = new utlError();
            if (_ibusOrgcontactAddress == null)
            {
                _ibusOrgcontactAddress = new busOrgContactAddress();
            }
            _ibusOrgcontactAddress.FindOrgContactAddress(aintAddressId);
            if (_ibusOrgcontactAddress.icdoOrgContactAddress.status_value == busConstant.StatusActive)
            {
                _icdoOrgContact.primary_address_id = aintAddressId;
                _icdoOrgContact.modified_by = iobjPassInfo.istrUserID;
                _icdoOrgContact.Update();
                larrError.Add(this);
            }
            else
            {
                lobjError = AddError(busConstant.MessageIdPrimaryAddress, string.Empty);
                larrError.Add(lobjError);
            }
            return larrError;
        }

        public void LoadOtherOrgContacts()
        {
            if (_iclbOtherOrgContacts == null)
            {
                _iclbOtherOrgContacts = new Collection<busOrgContact>();
            }

            DataTable ldtbOtherOrgContacts = busNeoSpinBase.Select("cdoOrgContact.LoadOtherOrgCotacts", new object[2] { _icdoOrgContact.org_id, _icdoOrgContact.org_contact_id });
            _iclbOtherOrgContacts = GetCollection<busOrgContact>(ldtbOtherOrgContacts, "icdoOrgContact");
            foreach (busOrgContact lobjOrgcontact in _iclbOtherOrgContacts)
            {
                lobjOrgcontact.LoadContact();
                lobjOrgcontact.LoadOrgAndContactPrimaryAddress();
            }
        }
        public void LoadCodeValuePlanAndRole()
        {
            //set visiblity for plan benefit types
            //if (iclcPlan.IsNullOrEmpty()) LoadPlanCollection();
            //ibusOrgContactPlanAndRole = new busOrgContactPlanAndRole();
            if (ibusOrganization.IsNull()) LoadOrganization();
            ibusOrganization.LoadOrgPlan();
            ibusOrganization.iclbOrgPlan.Where(plan => plan.icdoOrgPlan.end_date_no_null >= DateTime.Now) //PIR 26078 - load the grid with future dated plans and only restrict end dated plans from showing
                        .ForEach(objPlan =>
                        {
                            if (objPlan.ibusPlan?.icdoPlan?.benefit_type_value == "INSR")
                                iblnIsOrgProvideInsurancePlan = true;
                            if (objPlan.ibusPlan?.icdoPlan?.benefit_type_value == "RETR")
                                iblnIsOrgProvideRetirementPlan = true;
                            if (objPlan.ibusPlan?.icdoPlan?.benefit_type_value == "DEFF")
                                iblnIsOrgProvideDefCompPlan = true;
                            if (objPlan.ibusPlan?.icdoPlan?.benefit_type_value == "FLEX")
                                iblnIsOrgProvideFlexPlan = true;
                        });

            
            iclbOrgContactPlanAndRole = new Collection<busOrgContactPlanAndRole>();
            
            DataTable ldtbPlanAndRole = busNeoSpinBase.Select("cdoOrgContact.LoadPlanAndRole", new object[2] { _icdoOrgContact.contact_id, _icdoOrgContact.org_id });

            List<utlCodeValue> llstCodeValue = iobjPassInfo.isrvDBCache.GetCodeValuesFromDict(515);
            foreach (utlCodeValue lobjCodeValue in llstCodeValue)
            {
                busOrgContactPlanAndRole lobjCodeValuePlanAndRole = new busOrgContactPlanAndRole();
                lobjCodeValuePlanAndRole.istrCodeValueRole = lobjCodeValue.code_value;
                lobjCodeValuePlanAndRole.istrDescriptionRole = lobjCodeValue.description;
                lobjCodeValuePlanAndRole.istrCheckDefComp = busConstant.Flag_No;
                lobjCodeValuePlanAndRole.istrCheckFlex = busConstant.Flag_No;
                lobjCodeValuePlanAndRole.istrCheckInsurance = busConstant.Flag_No;
                lobjCodeValuePlanAndRole.istrCheckRetirement = busConstant.Flag_No;
                lobjCodeValuePlanAndRole.istrCheckNoPlan = busConstant.Flag_No;

                foreach (DataRow dr in ldtbPlanAndRole.Rows)
                {
                    if (lobjCodeValue.code_value == dr["CONTACT_ROLE_VALUE"].ToString())
                    {
                        if (dr["BENEFIT_TYPE_VALUE"].ToString() == "INSR")
                            lobjCodeValuePlanAndRole.istrCheckInsurance = busConstant.Flag_Yes;
                        if (dr["BENEFIT_TYPE_VALUE"].ToString() == "RETR")
                            lobjCodeValuePlanAndRole.istrCheckRetirement = busConstant.Flag_Yes;
                        if (dr["BENEFIT_TYPE_VALUE"].ToString() == "DEFF")
                            lobjCodeValuePlanAndRole.istrCheckDefComp = busConstant.Flag_Yes;
                        if (dr["BENEFIT_TYPE_VALUE"].ToString() == "FLEX")
                            lobjCodeValuePlanAndRole.istrCheckFlex = busConstant.Flag_Yes;
                        if (Convert.IsDBNull(dr["BENEFIT_TYPE_VALUE"]))
                            lobjCodeValuePlanAndRole.istrCheckNoPlan = busConstant.Flag_Yes;
                        if (lobjCodeValuePlanAndRole.istrCheckInsurance == busConstant.Flag_Yes || lobjCodeValuePlanAndRole.istrCheckRetirement == busConstant.Flag_Yes ||
                            lobjCodeValuePlanAndRole.istrCheckDefComp == busConstant.Flag_Yes || lobjCodeValuePlanAndRole.istrCheckFlex == busConstant.Flag_Yes)
                            lobjCodeValuePlanAndRole.istrCheckNoPlan = busConstant.Flag_Yes;
                    }
                }

                iclbOrgContactPlanAndRole.Add(lobjCodeValuePlanAndRole);
            }            
        }
        public void LoadPlan()
        {
            if (_ibusPlan == null)
            {
                _ibusPlan = new busPlan();
            }
            _ibusPlan.FindPlan(_icdoOrgContact.plan_id);
        }

        //PIR 271
        //To load plan in which org participated 
        public Collection<cdoPlan> LoadPlanCollection()
        {
            DataTable ldtbPlanList = Select("cdoOrgPlan.LoadPlanByOrg", new object[1] { this.icdoOrgContact.org_id });
            _iclcPlan = cdoPlan.GetCollection<cdoPlan>(ldtbPlanList);
            return _iclcPlan;
        }
        public void LoadOrgAndContactPrimaryAddress()
        {
            if (_ibusOrgAndContactPrimaryAddress == null)
            {
                _ibusOrgAndContactPrimaryAddress = new busOrgContactAddress();
            }
            _ibusOrgAndContactPrimaryAddress.FindOrgContactAddress(_icdoOrgContact.primary_address_id);
        }
        public void LoadOrgAndContactAddresses()
        {
            if (_iclbOrgAndContactAddresses == null)
            {
                _iclbOrgAndContactAddresses = new Collection<busOrgContactAddress>();
            }
            DataTable ldtbOrgAndContactAddresses = busNeoSpinBase.Select("cdoOrgContact.GetOrgAndContactAddresses",
                                                                new object[2] { _icdoOrgContact.org_id, _icdoOrgContact.contact_id });
            _iclbOrgAndContactAddresses = GetCollection<busOrgContactAddress>(ldtbOrgAndContactAddresses, "icdoOrgContactAddress");
            //PIR - 334
            foreach (busOrgContactAddress lobjOrgContactAddress in _iclbOrgAndContactAddresses)
            {
                lobjOrgContactAddress.LoadOrganization();
                lobjOrgContactAddress.LoadContact();
                if (lobjOrgContactAddress.icdoOrgContactAddress.contact_org_address_id == _icdoOrgContact.primary_address_id)
                {
                    lobjOrgContactAddress.primary_address_flag = busConstant.PrimaryAddressYes;
                }
                else
                {
                    lobjOrgContactAddress.primary_address_flag = busConstant.PrimaryAddressNo;
                }
            }
        }
        public void setConsolidatStatusValue()
        {
            bool lblnStatus = false;
            DataTable ldtChekcContactStatus = Select<cdoOrgContact>(new string[3] { "contact_id", "org_id", "status_value" }, new object[3] { _icdoOrgContact.contact_id, _icdoOrgContact.org_id, busConstant.OrgContactStatusActive }, null, null);
            if(ldtChekcContactStatus.IsNotNull() && ldtChekcContactStatus.Rows.Count > 0)
                istrConsolidatSetStatusValue = busConstant.OrgContactStatusActive;
            else
                istrConsolidatSetStatusValue = busConstant.OrgContactStatusInActive;
            istrSetStatusValueWhichWasOnLoad = istrConsolidatSetStatusValue;
        }
        public override int PersistChanges()
        {
            if (iarrChangeLog.Count > 0 && iarrChangeLog.Any(dobj => (dobj is cdoOrgContact)))
            {
                List<doBase> llstCovDetails = iarrChangeLog.Where(dobj => (dobj is cdoOrgContact)).ToList();
                if (llstCovDetails.Count > 0)
                {
                    foreach (doBase item in llstCovDetails)
                    {
                        iarrChangeLog.Remove(item);
                    }
                }
            }
            int lintRetunPersistChangesValue = base.PersistChanges();
            
            
            //taking collection for looping benfit type by plan 
            if (iclcPlan.IsNull()) LoadPlanCollection();
            //Add blank plan for No Plan checkbox
            iclcPlan.Add(new cdoPlan { plan_id = 0,benefit_type_value = string.Empty });
            //DataTable ldtOrgContactRole = Select<cdoOrgContactRole>(new string[1] { "org_contact_id" }, new object[2] {  }, null, null);
            Collection <cdoOrgContactRole> lclbOrgContactRole = new Collection<cdoOrgContactRole>();
            bool lblnIsStausUpdatedToInactive = false;
            if (ienmPageMode == utlPageMode.Update && icdoOrgContact.ihstOldValues.Count > 0)
            {
                if(istrConsolidatSetStatusValue == busConstant.OrgContactStatusInActive && istrSetStatusValueWhichWasOnLoad != istrConsolidatSetStatusValue)
                {
                    lblnIsStausUpdatedToInactive = true;
                }
            }
            if (iclbOrgContactPlanAndRole == null)
            {
                iclbOrgContactPlanAndRole = new Collection<busOrgContactPlanAndRole>();
            }
            if (lblnIsStausUpdatedToInactive)
                DBFunction.DBNonQuery("entContact.UpdateOrgContactStatusToInactive", new object[2] { _icdoOrgContact.org_id, _icdoOrgContact.contact_id }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            else
            {
                //every first time save set the variable to false
                iblnIsRetriementPersist = false;
                iblnIsInsurancePersist = false;
                iblnIsDefCompPersist = false;
                iblnIsInsurancePersist = false;
                iblnIsNoPlanPersist = false;
                //loop plan and role grid with ckeck values according to plan benefit type, loop is iterating all the values from code value group 515
                foreach (busOrgContactPlanAndRole lbusOrgContactPlanAndRole in iclbOrgContactPlanAndRole)
                {
                    iblnIsNewlyInserted = false;
                    CheckBenefitPlanAndInsert(lbusOrgContactPlanAndRole.istrCodeValueRole, lbusOrgContactPlanAndRole.istrCheckNoPlan, string.Empty);
                    if (iblnIsOrgProvideRetirementPlan == true)
                        CheckBenefitPlanAndInsert(lbusOrgContactPlanAndRole.istrCodeValueRole, lbusOrgContactPlanAndRole.istrCheckRetirement, busConstant.PlanBenefitTypeRetirement);
                    if (iblnIsOrgProvideInsurancePlan == true)
                        CheckBenefitPlanAndInsert(lbusOrgContactPlanAndRole.istrCodeValueRole, lbusOrgContactPlanAndRole.istrCheckInsurance, busConstant.PlanBenefitTypeInsurance);
                    if (iblnIsOrgProvideDefCompPlan == true)
                        CheckBenefitPlanAndInsert(lbusOrgContactPlanAndRole.istrCodeValueRole, lbusOrgContactPlanAndRole.istrCheckDefComp, busConstant.PlanBenefitTypeDeferredComp);
                    if (iblnIsOrgProvideFlexPlan == true)
                        CheckBenefitPlanAndInsert(lbusOrgContactPlanAndRole.istrCodeValueRole, lbusOrgContactPlanAndRole.istrCheckFlex, busConstant.PlanBenefitTypeFlex);                    
                }
                //----------------End
            }
            iclcPlan.Clear();
            LoadPlanCollection();
            FindOrgContact(icdoOrgContact.org_contact_id);
            setConsolidatStatusValue();
            return lintRetunPersistChangesValue;
        }

        public override void AfterPersistChanges()
        {
            //ucs - 032
            if (iintContactId > 0)
            {
                ibusContact.icdoContact.primary_address_id = ibusContact.ibusContactPrimaryAddress.icdoOrgContactAddress.contact_org_address_id;
                ibusContact.icdoContact.Update();
            }
            //FindOrgContact(icdoOrgContact.org_contact_id);
            LoadContact();
            LoadOrgAndContactAddresses();
            base.AfterPersistChanges();
            LoadContactTypes();
            LoadContactPlanRoles();
            LoadCodeValuePlanAndRole();
            LoadOrgContactGroupByRoles();
            IsContactAffiliatedWithNoOrg();
        }

        public void CheckBenefitPlanAndInsert(string astrCODE_VALUE_Roles, string astrIsCheckPlan,string astrPlanBenefitType)
        {
            int lintORG_CONTACT_ID = 0;
            busOrgContact lbusOrgContact = new busOrgContact { icdoOrgContact = new cdoOrgContact() };
            lbusOrgContact.icdoOrgContact.org_id = icdoOrgContact.org_id;
            lbusOrgContact.icdoOrgContact.status_value = istrConsolidatSetStatusValue;
            lbusOrgContact.icdoOrgContact.contact_id = icdoOrgContact.contact_id;
            //lbusOrgContactPlanAndRole.checkRetirement == busConstant.Flag_Yes 

            
            //loop throup plan benefit type to each plan in group
            foreach (cdoPlan lcdoPlan in iclcPlan.Where(Retr => Retr.benefit_type_value == astrPlanBenefitType))
            {
                lbusOrgContact.icdoOrgContact.plan_id = lcdoPlan.plan_id;
                //taking collection for checking record already exist and need to update 
                DataTable ldtOrgContactsAndPlan = Select<cdoOrgContact>(new string[2] { "contact_id", "org_id" }, new object[2] { _icdoOrgContact.contact_id, _icdoOrgContact.org_id }, null, null);
                Collection<busOrgContact> lclbOrgContactsAndPlan = GetCollection<busOrgContact>(ldtOrgContactsAndPlan, "icdoOrgContact");

                //checking plan is alredy exist , then update status value else insert
                //checking bool value status updated or not, only status value need to update or not 
                if (lclbOrgContactsAndPlan.Any(plan => plan.icdoOrgContact.plan_id == lcdoPlan.plan_id))
                {
                    //lintORG_CONTACT_ID = lclbOrgContactsAndPlan.FirstOrDefault(plan => plan.icdoOrgContact.plan_id == lcdoPlan.plan_id).icdoOrgContact.org_contact_id;//.FirstOrDefault(p=>p.icdoOrgContact.org_contact_id);
                    busOrgContact lbusExistingOrgContact = lclbOrgContactsAndPlan.FirstOrDefault(plan => plan.icdoOrgContact.plan_id == lcdoPlan.plan_id);
                    lintORG_CONTACT_ID = lbusExistingOrgContact.icdoOrgContact.org_contact_id;
                    //bool lblnCheckBoxChecked = iclbOrgContactPlanAndRole.Where(lobjOrgContact=>lobjOrgContact.istrCheckInsurance );
                    if (astrIsCheckPlan == busConstant.Flag_Yes)
                    {
                        if (lbusExistingOrgContact.icdoOrgContact.status_value != busConstant.OrgContactStatusActive)
                        {
                            lbusExistingOrgContact.icdoOrgContact.status_value = busConstant.OrgContactStatusActive;
                            lbusExistingOrgContact.icdoOrgContact.ienuObjectState = ObjectState.Update;
                            lbusExistingOrgContact.icdoOrgContact.Update();
                        }
                        //set variale to true so for next iterration it could not be set inactive 
                        switch (astrPlanBenefitType)
                        {
                            case busConstant.PlanBenefitTypeRetirement: iblnIsRetriementPersist = true; break;
                            case busConstant.PlanBenefitTypeInsurance: iblnIsInsurancePersist = true;   break;
                            case busConstant.PlanBenefitTypeDeferredComp: iblnIsDefCompPersist = true; break;
                            case busConstant.PlanBenefitTypeFlex: iblnIsFlexPersist = true; break;
                            case "" : iblnIsNoPlanPersist = true; break;
                        }
                    }
                    else if (!iblnIsRetriementPersist && !iblnIsInsurancePersist && !iblnIsDefCompPersist && !iblnIsFlexPersist && !iblnIsNoPlanPersist
                        && astrIsCheckPlan == busConstant.Flag_No && lbusExistingOrgContact.icdoOrgContact.status_value != busConstant.OrgContactStatusInActive
                        && !iblnIsNewlyInserted)
                    {
                        lbusExistingOrgContact.icdoOrgContact.status_value = busConstant.OrgContactStatusInActive;
                        lbusExistingOrgContact.icdoOrgContact.ienuObjectState = ObjectState.Update;
                        lbusExistingOrgContact.icdoOrgContact.Update();
                    }
                }
                else
                {
                    //if check box checked then 
                    if (astrIsCheckPlan == busConstant.Flag_Yes)
                    {
                        lbusOrgContact.icdoOrgContact.ienuObjectState = ObjectState.Insert;
                        lbusOrgContact.icdoOrgContact.Insert();
                        lintORG_CONTACT_ID = lbusOrgContact.icdoOrgContact.org_contact_id;
                        iblnIsNewlyInserted = true;

                        //set variale to true so for next iterration it could not be set inactive 
                        switch (astrPlanBenefitType)
                        {
                            case busConstant.PlanBenefitTypeRetirement: iblnIsRetriementPersist = true; break;
                            case busConstant.PlanBenefitTypeInsurance: iblnIsInsurancePersist = true; break;
                            case busConstant.PlanBenefitTypeDeferredComp: iblnIsDefCompPersist = true; break;
                            case busConstant.PlanBenefitTypeFlex: iblnIsFlexPersist = true; break;
                            case "": iblnIsNoPlanPersist = true; break;
                        }
                    }
                    //else
                    //{

                    //}
                }
                //taking org_contact_id for inserting role values 
                if (lintORG_CONTACT_ID > 0)
                {
                    cdoOrgContactRole lcdoOrgContactRole = new cdoOrgContactRole();
                    //checking role value is alredy exist , then update value else insert
                    //checking role value is alredy exist, and uncheck the check box then deleted from table 
                    DataTable ldtOrgContactRole = Select<cdoOrgContactRole>(new string[2] { "org_contact_id", "contact_role_value" }, new object[2] { lintORG_CONTACT_ID, astrCODE_VALUE_Roles }, null, null);
                    if (ldtOrgContactRole.Rows.Count > 0)
                    {
                        if (ienmPageMode == utlPageMode.Update && astrIsCheckPlan == busConstant.Flag_No)
                        {
                            lcdoOrgContactRole.LoadData(ldtOrgContactRole.Rows[0]);
                            lcdoOrgContactRole.ienuObjectState = ObjectState.Delete;
                            lcdoOrgContactRole.Delete();
                        }
                        //lcdoOrgContactRole.org_contact_role_id = lclbOrgContactRole.FirstOrDefault(plan => plan.org_contact_id == lintORG_CONTACT_ID).org_contact_role_id;
                        //lcdoOrgContactRole.ienuObjectState = ObjectState.Update;
                        //lcdoOrgContactRole.Update();
                    }
                    else
                    {
                        if (astrIsCheckPlan == busConstant.Flag_Yes)
                        {
                            lcdoOrgContactRole.org_contact_id = lintORG_CONTACT_ID;
                            lcdoOrgContactRole.contact_role_value = astrCODE_VALUE_Roles;
                            lcdoOrgContactRole.ienuObjectState = ObjectState.Insert;
                            lcdoOrgContactRole.Insert();
                        }
                    }
                } // if end org contact role 
            }//loop end retirement plan 
                
        }
        public void IsContactAffiliatedWithNoOrg()
        {
            if (ibusContact.IsNull()) LoadContact();
            ibusContact.LoadOrgContact();
            if (ibusContact.icdoContact.status_value == busConstant.OrgContactStatusActive 
                && !ibusContact.iclbOrgContact.Any(lobjOrgContact => lobjOrgContact.icdoOrgContact.status_value == busConstant.OrgContactStatusActive))
                iblnIsContactAffiliatedWithNoOrg = true;
        }
        public ArrayList btnInactiveContact(int aintContactId)
        {
            ArrayList larrList = new ArrayList();
            if (ibusContact.IsNull()) LoadContact();
            if (ibusContact.icdoContact.contact_id == aintContactId)
            {
                ibusContact.icdoContact.status_value = busConstant.OrgContactStatusInActive;
                ibusContact.icdoContact.Update();
            }

            //EvaluateInitialLoadRules(utlPageMode.All);
            larrList.Add(this);
            return larrList;
        }
        //To Check Primary Authorized Agent exists
        public bool CheckPrimaryAuthorizedAgentSelectedAndNotExists()
        {
            bool _lblnResult = false;
            bool _lblIsPrimaryAuthorizedAgentSelected = CheckPrimaryAuthAgentSelected();

            // Check for Org Type Employer
            if (_ibusOrganization.icdoOrganization.org_type_value == busConstant.OrgTypeEmployer) //Put it in busConstant
            {
                int lintCount = 0;
                //Check for New Mode and Execute query to check contact exists with Role Primary Authorized Agent
                if (icdoOrgContact.ienuObjectState == ObjectState.Insert)
                {
                    lintCount = (int)DBFunction.DBExecuteScalar("cdoOrgContact.CheckPrimaryAuthorizedAgentExists", new object[1] { _icdoOrgContact.org_id }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                }
                //Execute query in Update Mode to check contact exists with Role Primary Authorized Agent excluding current record
                else
                {
                    lintCount = (int)DBFunction.DBExecuteScalar("cdoOrgContact.CheckPrimaryAuthorizedAgentExistsUpdate", new object[2] { _icdoOrgContact.org_id, _icdoOrgContact.org_contact_id }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                }

                if (lintCount == 0)
                {
                    if (_lblIsPrimaryAuthorizedAgentSelected)
                    {
                        _lblnResult = true;
                    }
                    else
                    {
                        _lblnResult = false;
                    }
                }
                else
                {
                    _lblnResult = true;
                }
            }
            else
            {
                //Other Org Type like Provider / Vendor etc..
                _lblnResult = true;
            }

            return _lblnResult;
        }

        public bool CheckMultipleActivePrimaryAuthAgentExists()
        {
            bool _lblnResult = true;
            bool _lblIsPrimaryAuthorizedAgentSelected= false;// = CheckPrimaryAuthAgentSelected();
            int count = 0;
            foreach (busOrgContactPlanAndRole lbusRole in iclbOrgContactPlanAndRole)
            {
                if (lbusRole.istrCodeValueRole == busConstant.OrgContactRolePrimaryAuthorizedAgent)
                {
                    if (lbusRole.istrCheckRetirement == busConstant.Flag_Yes)
                        count++;
                    if (lbusRole.istrCheckInsurance == busConstant.Flag_Yes)
                        count++;
                    if (lbusRole.istrCheckDefComp == busConstant.Flag_Yes)
                        count++;
                    if (lbusRole.istrCheckFlex == busConstant.Flag_Yes)
                        count++;
                    if (lbusRole.istrCheckNoPlan == busConstant.Flag_Yes)
                        count++;
                }
            }
            if (count > 0)
                _lblIsPrimaryAuthorizedAgentSelected = true;

            if ((!_lblIsPrimaryAuthorizedAgentSelected))
            {
                _lblnResult = false;
                return _lblnResult;
            }

            // Check for Org Type Employer
            // if (_ibusOrganization.icdoOrganization.org_type_value == busConstant.OrgTypeEmployer) //Put it in busConstant
            // {
            int lintCount = 0;
            //Check for New Mode and Execute query to check contact exists with Role Primary Authorized Agent
            if (icdoOrgContact.ienuObjectState == ObjectState.Insert)
            {
                lintCount = (int)DBFunction.DBExecuteScalar("cdoOrgContact.CheckPrimaryAuthorizedAgentExists", new object[1] { _icdoOrgContact.org_id }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            }
            //Execute query in Update Mode to check contact exists with Role Primary Authorized Agent excluding current record
            else
            {
                lintCount = (int)DBFunction.DBExecuteScalar("cdoOrgContact.CheckPrimaryAuthorizedAgentExistsUpdate", new object[2] { _icdoOrgContact.org_id, _icdoOrgContact.contact_id }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            }

            if (lintCount <= 0)
            {
                _lblnResult = false;
            }
            //}
            //else
            //{
            //Other Org Type like Provider / Vendor etc..
            //    _lblnResult = false;
            // }
            return _lblnResult;
        }
        public bool CheckRolePlanSelected()
        {
            //Role is required – at least one role must be selected
            if (istrConsolidatSetStatusValue == busConstant.OrgContactStatusActive)
                return iclbOrgContactPlanAndRole.Where(objOrgContactRole => objOrgContactRole.istrCheckNoPlan == busConstant.Flag_Yes ||
                                                 objOrgContactRole.istrCheckRetirement == busConstant.Flag_Yes || objOrgContactRole.istrCheckInsurance == busConstant.Flag_Yes ||
                                                 objOrgContactRole.istrCheckDefComp == busConstant.Flag_Yes || objOrgContactRole.istrCheckFlex == busConstant.Flag_Yes
                                               ).Count() >= 1;
            else return true;
        }

        public bool CheckNoRoleIsSelected()
        {
            //Role is required
            if (istrConsolidatSetStatusValue == busConstant.OrgContactStatusActive)
                return iclbOrgContactPlanAndRole.Where(objOrgContactRole => objOrgContactRole.istrCheckNoPlan == busConstant.Flag_No &&
                                                 (objOrgContactRole.istrCheckRetirement == busConstant.Flag_Yes || objOrgContactRole.istrCheckInsurance == busConstant.Flag_Yes ||
                                                 objOrgContactRole.istrCheckDefComp == busConstant.Flag_Yes || objOrgContactRole.istrCheckFlex == busConstant.Flag_Yes)
                                               ).Count() >= 1;
            else return false;
        }

        public bool CheckAuthorizedAgentAndPlanSelected()
        {
            // Plan is required 
            if (istrConsolidatSetStatusValue == busConstant.OrgContactStatusActive)
                return iclbOrgContactPlanAndRole.Where(objOrgContactRole => objOrgContactRole.istrCodeValueRole == busConstant.OrgContactRoleAuthorizedAgent && objOrgContactRole.istrCheckNoPlan == busConstant.Flag_Yes &&
                                                 (objOrgContactRole.istrCheckRetirement == busConstant.Flag_No && objOrgContactRole.istrCheckInsurance == busConstant.Flag_No &&
                                                 objOrgContactRole.istrCheckDefComp == busConstant.Flag_No && objOrgContactRole.istrCheckFlex == busConstant.Flag_No)
                                               ).Count() >= 1;
            else return false;
        }
        public bool CheckPrimaryAuthAgentSelected()
        {
            bool _lblIsPrimaryAuthorizedAgentSelected = false;
            Collection<cdoOrgContactRole> lclbTempOrgContactRole = (Collection<cdoOrgContactRole>)_iclbOrgContactRole;

            //Get the Roles from Collection
            foreach (cdoOrgContactRole _lcdoOrgContactRole in lclbTempOrgContactRole)
            {
                //Check Role is Primary Authorized Agent
                if (_lcdoOrgContactRole.contact_role_value == busConstant.OrgContactRolePrimaryAuthorizedAgent)                    
                {
                    if (_lcdoOrgContactRole.ienuObjectState == ObjectState.CheckListInsert || _lcdoOrgContactRole.ienuObjectState == ObjectState.Select)
                    {
                        _lblIsPrimaryAuthorizedAgentSelected = true;
                        break;
                    }
                    else if (_lcdoOrgContactRole.ienuObjectState == ObjectState.CheckListDelete) // PROD PIR ID 4247
                    {
                        _lblIsPrimaryAuthorizedAgentSelected = false;
                        break;
                    }
                }
            }
            return _lblIsPrimaryAuthorizedAgentSelected;
        }

        public bool ValidatePlansForContactRole()
        {
            if (_ibusOrganization.icdoOrganization.org_type_value == busConstant.OrgTypeProvider)
            {
                return iclbOrgContactPlanAndRole.Where(objOrgContactRole => objOrgContactRole.istrCheckDefComp == busConstant.Flag_Yes && 
                                    !(objOrgContactRole.istrCodeValueRole == busConstant.OrgContactRoleAgent || objOrgContactRole.istrCodeValueRole == busConstant.OrgContactRoleOther
                                    || objOrgContactRole.istrCodeValueRole == busConstant.OrgContactRolePrimaryAuthorizedAgent)
                       ).Count() >= 1;
            }
            return false;
        }
        public bool CheckRoleEligiblityforEmployer()
        {
            if (_ibusOrganization.icdoOrganization.org_type_value != busConstant.OrganizationTypeEmployer)
            {
                return iclbOrgContactPlanAndRole.Where(objOrgContactRole => objOrgContactRole.istrCodeValueRole == busConstant.OrgContactRoleFinance &&
                                                (objOrgContactRole.istrCheckRetirement == busConstant.Flag_Yes || objOrgContactRole.istrCheckInsurance == busConstant.Flag_Yes ||
                                                objOrgContactRole.istrCheckDefComp == busConstant.Flag_Yes || objOrgContactRole.istrCheckFlex == busConstant.Flag_Yes)
                                              ).Count() >= 1;
            }
            return false;
        }
        public bool CheckIfOrgContactRoleIsAgent()
        {
            if (_ibusOrganization.icdoOrganization.org_type_value == busConstant.OrganizationTypeProvider)
                return iclbOrgContactPlanAndRole.Where(objOrgContactRole => objOrgContactRole.istrCodeValueRole == busConstant.OrgContactRoleAgent &&
                                            (objOrgContactRole.istrCheckRetirement == busConstant.Flag_Yes || objOrgContactRole.istrCheckInsurance == busConstant.Flag_Yes ||
                                            objOrgContactRole.istrCheckFlex == busConstant.Flag_Yes)
                                          ).Count() >= 1;
            return false;            
        }
        public bool AllowPrimaryAuthAgentDelete()
        {
            bool lblnResult = true;
            if (_ibusOrganization.icdoOrganization.org_type_value == busConstant.OrgTypeEmployer)
            {
                if (_icdoOrgContact.status_value == "ACTV")
                {
                    int lintCount = (int)DBFunction.DBExecuteScalar("cdoOrgContact.GetCountOfOrgContact", new object[1] { _icdoOrgContact.org_id }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                    if (lintCount > 1)
                    {
                        DataTable ldtbCTlist = Select<cdoOrgContactRole>(new string[1] { "org_contact_id" }, new object[1] { _icdoOrgContact.org_contact_id }, null, null);
                        if (ldtbCTlist.Rows.Count > 0)
                        {
                            if ((Convert.ToString(ldtbCTlist.Rows[0]["contact_role_value"])) == busConstant.OrgContactRolePrimaryAuthorizedAgent)
                            {
                                lblnResult = false;
                                return lblnResult;
                            }
                        }
                    }
                }
            }
            else
            {
                return lblnResult;
            }
            return lblnResult;
        }

        // PIR -341

        public ArrayList ApplyToAllPlans_Click()
        {
            ArrayList larrError = new ArrayList();
            utlError lobjError = new utlError();
            DataTable ldtbList = Select("cdoOrgContact.SelectApplicablePlans", new object[1] { _icdoOrgContact.org_id });
            _iclbOrgContactsAndPlan = GetCollection<busOrgContact>(ldtbList, "icdoOrgContact");
            if (_iclbOrgContactsAndPlan.Count > 0)
            {
                foreach (busOrgContact lobjOrgContact in _iclbOrgContactsAndPlan)
                {
                    lobjOrgContact.icdoOrgContact.org_id = _icdoOrgContact.org_id;
                    lobjOrgContact.icdoOrgContact.contact_id = _icdoOrgContact.contact_id;
                    lobjOrgContact._icdoOrgContact.status_value = _icdoOrgContact.status_value;
                    lobjOrgContact.icdoOrgContact.Insert();
                }
                larrError.Add(this);
            }
            else
            {
                lobjError = AddError(4138, String.Empty);
                larrError.Add(lobjError);
            }

            return larrError;
        }

        public override void BeforePersistChanges()
        {
            iblnIsUpdateAddressClicked = false;
            if (iintContactId > 0)
            {
                if (ibusContact.ibusContactPrimaryAddress.icdoOrgContactAddress.ienuObjectState == ObjectState.Insert ||
                    ibusContact.ibusContactPrimaryAddress.icdoOrgContactAddress.ienuObjectState == ObjectState.Update)
                {
                    ibusContact.ibusContactPrimaryAddress.icdoOrgContactAddress.contact_id = icdoOrgContact.contact_id;
                    iarrChangeLog.Add(ibusContact.ibusContactPrimaryAddress.icdoOrgContactAddress);
                }
            }
            base.BeforePersistChanges();
        }

        #region UCS - 032

        public bool iblnPAAG { get; set; }

        public int iintPAAG { get; set; }

        //this property is set from ESS only, some functionality is depenedent on it
        public int iintContactId { get; set; }

        public bool IsSaveButtonVisible()
        {
            bool lblnResult = true;
            if (!iblnPAAG && icdoOrgContact.contact_id != iintContactId)
                lblnResult = false;
            return lblnResult;
        }

        #endregion

        public override busBase GetCorOrganization()
        {
            if (ibusOrganization.IsNull()) LoadOrganization();
            ibusOrganization.ibusOrgContact = this;

            // UAT PIR ID 1630
            if (ibusContact.IsNull()) LoadContact();
            if (ibusOrganization.ibusContact.IsNull()) ibusOrganization.ibusContact = new busContact { icdoContact = new cdoContact() };
            ibusOrganization.ibusContact = ibusContact;

            return ibusOrganization;
        }

        //this is used in COR ORG- 1503 & 1002
        public string istrContactRoles
        {
            get
            {
                return iclbOrgContactRole.IsNull() ? String.Empty : iclbOrgContactRole.Count == 0 ? string.Empty : iclbOrgContactRole[0].contact_role_description;
            }
        }
        public ArrayList UpdateAddressClick()
        {
            ArrayList larrList = new ArrayList();
            iblnIsUpdateAddressClicked = true;
            istrSuppressWarning = "N";
            this.EvaluateInitialLoadRules();
            larrList.Add(this);
            return larrList;
        }
        public void VerifyAddressUsingUSPS()
        {
            ArrayList larrErrors = new ArrayList();

            //If Suppress Warning Flag is Checked, we can skip the Web Service Validation            
            if (istrSuppressWarning == busConstant.Flag_Yes)
            {                
                ibusContact.ibusContactPrimaryAddress.icdoOrgContactAddress.address_validate_flag = "Y";
                return;
            }                       
            cdoWebServiceAddress _lobjcdoWebServiceAddress = new cdoWebServiceAddress();
            _lobjcdoWebServiceAddress.addr_line_1 = ibusContact.ibusContactPrimaryAddress.icdoOrgContactAddress.addr_line_1;
            _lobjcdoWebServiceAddress.addr_line_2 = ibusContact.ibusContactPrimaryAddress.icdoOrgContactAddress.addr_line_2;
            _lobjcdoWebServiceAddress.addr_city = ibusContact.ibusContactPrimaryAddress.icdoOrgContactAddress.city;
            _lobjcdoWebServiceAddress.addr_state_value = ibusContact.ibusContactPrimaryAddress.icdoOrgContactAddress.state_value;
            _lobjcdoWebServiceAddress.addr_zip_code = ibusContact.ibusContactPrimaryAddress.icdoOrgContactAddress.zip_code;
            _lobjcdoWebServiceAddress.addr_zip_4_code = ibusContact.ibusContactPrimaryAddress.icdoOrgContactAddress.zip_4_code;

            cdoWebServiceAddress _lobjcdoWebServiceAddressResult = busGlobalFunctions.ValidateWebServiceAddress(_lobjcdoWebServiceAddress);
            if (_lobjcdoWebServiceAddressResult.address_validate_flag != busConstant.Flag_No)
            {
                //ASSSIGN 
                ibusContact.ibusContactPrimaryAddress.icdoOrgContactAddress.addr_line_1 = _lobjcdoWebServiceAddressResult.addr_line_1;
                ibusContact.ibusContactPrimaryAddress.icdoOrgContactAddress.addr_line_2 = _lobjcdoWebServiceAddressResult.addr_line_2;
                ibusContact.ibusContactPrimaryAddress.icdoOrgContactAddress.city = _lobjcdoWebServiceAddressResult.addr_city;
                ibusContact.ibusContactPrimaryAddress.icdoOrgContactAddress.state_value = _lobjcdoWebServiceAddressResult.addr_state_value;
                ibusContact.ibusContactPrimaryAddress.icdoOrgContactAddress.zip_code = _lobjcdoWebServiceAddressResult.addr_zip_code;
                ibusContact.ibusContactPrimaryAddress.icdoOrgContactAddress.zip_4_code = _lobjcdoWebServiceAddressResult.addr_zip_4_code;
            }            
            ibusContact.ibusContactPrimaryAddress.icdoOrgContactAddress.address_validate_flag = _lobjcdoWebServiceAddressResult.address_validate_flag;
            ibusContact.ibusContactPrimaryAddress.icdoOrgContactAddress.address_validate_error = _lobjcdoWebServiceAddressResult.address_validate_error;            
        }
        public bool iblnIsContactActive { get; set; } // PIR 26305
        public override void BeforeValidate(utlPageMode aenmPageMode)
        {
            if (this.iblnPAAG)
            {
                VerifyAddressUsingUSPS();
            }
            // PIR 26305
            if (ibusContact.IsNull()) LoadContact();
            LoadContact();
            if (ibusContact.icdoContact.status_value == busConstant.OrgContactStatusInActive)
                iblnIsContactActive = true;
            base.BeforeValidate(aenmPageMode);
        }

        public override void ValidateHardErrors(utlPageMode aenmPageMode)
        {
            iarrErrors = new ArrayList();
            base.ValidateHardErrors(aenmPageMode);
            if (iblnIsFromESS)
            {
                foreach (utlError lobjErr in iarrErrors)
                {
                    lobjErr.istrErrorID = string.Empty;
                }
            }
        }

        public bool iblnIsFromESS { get; set; }


        public bool VerifyContactPhoneNumber()
        {
            //busContact.icdoContact.phone_no;
            bool lblnPhoneValid = false;

            //Validation only for ESS link only
            if (iblnIsFromESS)
            {
                lblnPhoneValid = false;
                if (ibusContact != null)
                {
                    if (ibusContact.icdoContact.phone_no != null)
                    {
                        if (ibusContact.icdoContact.phone_no.Length == 10)
                        {
                            lblnPhoneValid = true;
                        }
                    }
                }
            }
            return lblnPhoneValid;

        }


        public bool VerifyContactFaxNumber()
        {
            //busContact.icdoContact.phone_no;
            bool lblnFaxValid = false;

            //Validation only for ESS link only
            if (iblnIsFromESS)
            {
                lblnFaxValid = false;
                if (ibusContact != null)
                {
                    if (ibusContact.icdoContact.fax_no != null)
                    {
                        if (ibusContact.icdoContact.fax_no.Length == 10)
                        {
                            lblnFaxValid = true;
                        }
                    }
                }
            }
            return lblnFaxValid;
        }
		//PIR-18492
        public bool ValidateEmailPattern()  
        {
            if (ibusContact != null)
            {
                if (!String.IsNullOrEmpty(ibusContact.icdoContact.email_address))
                {
                     return busGlobalFunctions.IsEmailValid(ibusContact.icdoContact.email_address);
                }
            }
            return true;;
        }

        public void LoadScreenNotes(string astrScreenValue, int aintScreenPrimaryId)
        {
            DataTable ldtbScreenNotesList = Select<doScreenNotes>(
                                        new string[2] { enmScreenNotes.screen_value.ToString(), enmScreenNotes.screen_primary_id.ToString() },
                                        new object[2] { astrScreenValue, aintScreenPrimaryId }, null, "screen_notes_id desc");
            iclbScreenNotes = GetCollection<busScreenNotes>(ldtbScreenNotesList, "icdoScreenNotes");

        }
        public void LoadOrgContactGroupByRoles()
        {
            iclbOrgContactGroupByRole = new utlCollection<cdoOrgContactRole>();
            DataTable ldtbList = busNeoSpinBase.Select("cdoOrgContactRole.GetContactPlanRolesGroupByRoles", new object[2] { this.icdoOrgContact.contact_id, this.icdoOrgContact.org_id });
            foreach (DataRow ldrRow in ldtbList.Rows)
            {
                icdoContactRole = new cdoOrgContactRole();
                icdoContactRole.LoadData(ldrRow);
                icdoContactRole.contact_role_description = Convert.ToString(ldrRow["contact_role_description"]);
                icdoContactRole.contact_role_value = Convert.ToString(ldrRow["contact_role_value"]);
                iclbOrgContactGroupByRole.Add(icdoContactRole);
            }
        }
        public Collection<cdoOrgContactRole> iclbContactRolesDescription { get; set; }
        public void GetDisctinctContactRolesFromContactID()
        {
            iclbContactRolesDescription = new Collection<cdoOrgContactRole>();
            DataTable ldtbList = busNeoSpinBase.Select("entOrgContact.GetDisctinctContactRolesForContact", new object[2] { this.icdoOrgContact.contact_id, this.icdoOrgContact.org_id });
            foreach(DataRow ldrRow in ldtbList.Rows)
            {
                icdoContactRole = new cdoOrgContactRole();
                icdoContactRole.LoadData(ldrRow);
                icdoContactRole.contact_role_description = Convert.ToString(ldrRow["description"]);
                if(icdoContactRole.contact_role_description.IsNotNullOrEmpty())
                    iclbContactRolesDescription.Add(icdoContactRole);
            }
        }
    }
}
