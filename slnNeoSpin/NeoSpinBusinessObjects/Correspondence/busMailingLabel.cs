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
using System.Text.RegularExpressions;

#endregion

namespace NeoSpin.BusinessObjects
{
    public partial class busMailingLabel : busExtendBase
    {

        private string _lstrRecordCount;
        public string lstrRecordCount
        {
            get { return _lstrRecordCount; }
            set { _lstrRecordCount = value; }
        }

        private bool _lblnViewReportFlag;
        public bool lblnViewReportFlag
        {
            get { return _lblnViewReportFlag; }
            set { _lblnViewReportFlag = value; }
        }

        private utlCollection<cdoMailingLabelPersonType> _iclcPersonType;
        public utlCollection<cdoMailingLabelPersonType> iclcPersonType
        {
            get { return _iclcPersonType; }
            set { _iclcPersonType = value; }
        }

        private utlCollection<cdoMailingLabelPlan> _iclcPlan;
        public utlCollection<cdoMailingLabelPlan> iclcPlan
        {
            get { return _iclcPlan; }
            set { _iclcPlan = value; }
        }

        //FW Upgrade :: Code Conversion for "View Report" method
        public string istrReportTemplateName { get; set; }
        public override void SetParentKey(Sagitec.DataObjects.doBase aobjBase)
        {
            if (aobjBase is cdoMailingLabelPersonType)
            {
                cdoMailingLabelPersonType lcdoMalingLabelPersonType = (cdoMailingLabelPersonType)aobjBase;
                lcdoMalingLabelPersonType.mailing_label_batch_id = _icdoMailingLabel.mailing_label_batch_id;
            }
            if (aobjBase is cdoMailingLabelPlan)
            {
                cdoMailingLabelPlan lcdoMailingLabelPlan = (cdoMailingLabelPlan)aobjBase;
                lcdoMailingLabelPlan.mailing_label_batch_id = _icdoMailingLabel.mailing_label_batch_id;
            }
        }

        public override int PersistChanges()
        {
            _icdoMailingLabel.user_id = busGlobalFunctions.ToTitleCase(iobjPassInfo.istrUserID);
            _icdoMailingLabel.Insert();
            foreach (cdoMailingLabelPersonType lobjPersonType in _iclcPersonType)
            {
                if (lobjPersonType.ienuObjectState != ObjectState.CheckListDelete)
                {
                    lobjPersonType.mailing_label_batch_id = _icdoMailingLabel.mailing_label_batch_id;
                    lobjPersonType.Insert();
                }
            }
            foreach (cdoMailingLabelPlan lobjPlan in _iclcPlan)
            {
                if (lobjPlan.ienuObjectState != ObjectState.CheckListDelete)
                {
                    lobjPlan.mailing_label_batch_id = _icdoMailingLabel.mailing_label_batch_id;
                    lobjPlan.Insert();
                }
            }
            _lblnViewReportFlag = true;
            return 1;
        }

        public override void BeforeValidate(utlPageMode aenmPageMode)
        {
            switch (iobjPassInfo.istrPostBackControlID)
            {
                case busConstant.MailingLabelCSVButton:
                    _icdoMailingLabel.output_format_value = busConstant.MailingLabelOutputFormatCSV;
                    _icdoMailingLabel.run_date = DateTime.MinValue;
                    break;
                case busConstant.MailingLabelAveryButton:
                    _icdoMailingLabel.output_format_value = busConstant.MailingLabelOutputFormatAvery;
                    _icdoMailingLabel.run_date = DateTime.Now;
                    break;
            }
            base.BeforeValidate(aenmPageMode);
        }

        public override void AfterPersistChanges()
        {
            base.AfterPersistChanges();
            LoadPersonType();
            LoadPlan();
            LoadRecordCount();
        }

        public void LoadPersonType()
        {
            _iclcPersonType = GetCollection<cdoMailingLabelPersonType>(
                new string[1] { "mailing_label_batch_id" }, new object[1] { _icdoMailingLabel.mailing_label_batch_id }, null, null);
        }

        public void LoadPlan()
        {
            _iclcPlan = GetCollection<cdoMailingLabelPlan>(
                new string[1] { "mailing_label_batch_id" }, new object[1] { _icdoMailingLabel.mailing_label_batch_id }, null, null);
        }

        public void LoadRecordCount()
        {
            Collection<busMailingAddress> iclbMailingAddress = GetSearchResult(_icdoMailingLabel.mailing_label_batch_id);
            _lstrRecordCount = iclbMailingAddress.Count.ToString();
        }

        public ArrayList ViewReport_Click(int aintMailingBatchID)
        {
            ArrayList larrResult = new ArrayList();
            larrResult.Add("Successfully Added");
            larrResult.Add(GenerateMailingLabelDataSet(aintMailingBatchID));
            return larrResult;
        }

        public DataSet GenerateMailingLabelDataSet(int aintMailingBatchID)
        {
            DataSet lds = new DataSet("MLC");

            Collection<busMailingAddress> iclbMailingAddress = new Collection<busMailingAddress>();
            iclbMailingAddress = GetSearchResult(aintMailingBatchID);

            DataTable ldtMailingAddressess = new DataTable();
            ldtMailingAddressess.Columns.Add("ID");
            ldtMailingAddressess.Columns.Add("FullName");
            ldtMailingAddressess.Columns.Add("ContactName");
            ldtMailingAddressess.Columns.Add("AddressLine1");
            ldtMailingAddressess.Columns.Add("AddressLine2");
            ldtMailingAddressess.Columns.Add("City");
            ldtMailingAddressess.Columns.Add("State");
            ldtMailingAddressess.Columns.Add("ZipCode");
            ldtMailingAddressess.Columns.Add("Country");
            foreach (busMailingAddress lobjMailingAddress in iclbMailingAddress)
            {
                DataRow dr = ldtMailingAddressess.NewRow();
                dr["ID"] = lobjMailingAddress.icdoMailingAddress.lintID;
                dr["FullName"] = lobjMailingAddress.icdoMailingAddress.lstrFullName;
                dr["ContactName"] = lobjMailingAddress.icdoMailingAddress.lstrContactName;
                dr["AddressLine1"] = lobjMailingAddress.icdoMailingAddress.lstrAddressLine1;
                dr["AddressLine2"] = lobjMailingAddress.icdoMailingAddress.lstrAddressLine2;
                dr["City"] = lobjMailingAddress.icdoMailingAddress.lstrCity;
                dr["State"] = lobjMailingAddress.icdoMailingAddress.lstrState;
                dr["ZipCode"] = lobjMailingAddress.icdoMailingAddress.lstrZipCode;
                dr["Country"] = lobjMailingAddress.icdoMailingAddress.lstrCountry;
                ldtMailingAddressess.Rows.Add(dr);
            }

            ldtMailingAddressess.TableName = busConstant.ReportTableName;
            ldtMailingAddressess.ExtendedProperties.Add("sgrReportName", "rptMailingLabelAvery.rpt");
            lds.Tables.Add(ldtMailingAddressess);
            return lds;
        }
        public DataSet GenerateMailingLabelDataSetMVVM(int aintMailingBatchID)
        {
            DataSet lds = new DataSet("MLC");

            Collection<busMailingAddress> iclbMailingAddress = new Collection<busMailingAddress>();
            iclbMailingAddress = GetSearchResult(aintMailingBatchID);

            DataTable ldtMailingAddressess = new DataTable();
            ldtMailingAddressess.Columns.Add("ID");
            ldtMailingAddressess.Columns.Add("FullName");
            ldtMailingAddressess.Columns.Add("ContactName");
            ldtMailingAddressess.Columns.Add("AddressLine1");
            ldtMailingAddressess.Columns.Add("AddressLine2");
            ldtMailingAddressess.Columns.Add("City");
            ldtMailingAddressess.Columns.Add("State");
            ldtMailingAddressess.Columns.Add("ZipCode");
            ldtMailingAddressess.Columns.Add("Country");
            foreach (busMailingAddress lobjMailingAddress in iclbMailingAddress)
            {
                DataRow dr = ldtMailingAddressess.NewRow();
                dr["ID"] = lobjMailingAddress.icdoMailingAddress.lintID;
                dr["FullName"] = lobjMailingAddress.icdoMailingAddress.lstrFullName;
                dr["ContactName"] = lobjMailingAddress.icdoMailingAddress.lstrContactName;
                dr["AddressLine1"] = lobjMailingAddress.icdoMailingAddress.lstrAddressLine1;
                dr["AddressLine2"] = lobjMailingAddress.icdoMailingAddress.lstrAddressLine2;
                dr["City"] = lobjMailingAddress.icdoMailingAddress.lstrCity;
                dr["State"] = lobjMailingAddress.icdoMailingAddress.lstrState;
                dr["ZipCode"] = lobjMailingAddress.icdoMailingAddress.lstrZipCode;
                dr["Country"] = lobjMailingAddress.icdoMailingAddress.lstrCountry;
                ldtMailingAddressess.Rows.Add(dr);
            }

            ldtMailingAddressess.TableName = busConstant.ReportTableName;
            //ldtMailingAddressess.ExtendedProperties.Add("sgrReportName", "rptMailingLabelAvery.rpt");
            lds.Tables.Add(ldtMailingAddressess);
            return lds;
        }
        public bool lblnIsSearchForPerson
        {
            get
            {
                if ((_icdoMailingLabel.org_type_value != null) || (_icdoMailingLabel.employer_type_value != null) || (_icdoMailingLabel.org_contact_role_value != null))
                    return false;
                else
                    return true;
            }
        }

        public Collection<busMailingAddress> GetSearchResult(int AintMailingLabelBatchID)
        {
            Collection<busMailingAddress> iclbMailingAddress = new Collection<busMailingAddress>();
            if (AintMailingLabelBatchID != 0)
            {
                FindMailingLabel(AintMailingLabelBatchID);
                LoadPersonType();
                LoadPlan();
                if (lblnIsSearchForPerson)
                {
                    Collection<busPerson> _iclbPerson = new Collection<busPerson>();
                    _iclbPerson = GetLookedPersonIDs();
                    iclbMailingAddress = GetPersonAddressess(_iclbPerson);
                }
                else
                {
                    Collection<busOrganization> _iclbOrganization = new Collection<busOrganization>();
                    _iclbOrganization = GetLookedOrgIDs();
                    iclbMailingAddress = GetOrganizationAddress(_iclbOrganization);
                }

                /// Sorting by Zipcode
                iclbMailingAddress = busGlobalFunctions.Sort<busMailingAddress>("icdoMailingAddress.lstrZipCode", iclbMailingAddress);
            }
            return iclbMailingAddress;
        }

        public Collection<busMailingAddress> GetPersonAddressess(Collection<busPerson> _iclbPerson)
        {
            Collection<busMailingAddress> iclbMailingAddress = new Collection<busMailingAddress>();
            foreach (busPerson lobjPerson in _iclbPerson)
            {
                if (lobjPerson.ibusPersonCurrentAddress != null)
                {
                    busMailingAddress lobjMailingAddress = new busMailingAddress();
                    lobjMailingAddress.icdoMailingAddress = new cdoMailingAddress();
                    lobjMailingAddress.icdoMailingAddress.lintID = lobjPerson.icdoPerson.person_id;
                    lobjMailingAddress.icdoMailingAddress.lstrFullName = lobjPerson.icdoPerson.FullName;
                    if (lobjPerson.icdoPerson.first_name != null)
                        lobjMailingAddress.icdoMailingAddress.lstrFirstName = lobjPerson.icdoPerson.first_name.Trim().ToUpper();
                    if (lobjPerson.icdoPerson.last_name != null)
                        lobjMailingAddress.icdoMailingAddress.lstrLastName = lobjPerson.icdoPerson.last_name.Trim().ToUpper();
                    lobjMailingAddress.icdoMailingAddress.lstrAddressLine1 = lobjPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_line_1;
                    lobjMailingAddress.icdoMailingAddress.lstrAddressLine2 = lobjPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_line_2;
                    lobjMailingAddress.icdoMailingAddress.lstrCity = lobjPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_city;
                    lobjMailingAddress.icdoMailingAddress.lstrState = lobjPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_state_value;
                    lobjMailingAddress.icdoMailingAddress.lstrZipCode = lobjPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_zip_code;
                    if (lobjPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_zip_4_code != null)
                        lobjMailingAddress.icdoMailingAddress.lstrZipCode += "-" + lobjPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_zip_4_code;
                    // PIR ID 500
                    if (lobjMailingAddress.icdoMailingAddress.lstrAddressLine1 != null &&
                        lobjMailingAddress.icdoMailingAddress.lstrCity != null &&
                        lobjMailingAddress.icdoMailingAddress.lstrState != null &&
                        lobjMailingAddress.icdoMailingAddress.lstrZipCode != null)
                        iclbMailingAddress.Add(lobjMailingAddress);
                }
            }
            return iclbMailingAddress;
        }

        public Collection<busMailingAddress> GetOrganizationAddress(Collection<busOrganization> _iclbOrg)
        {
            Collection<busMailingAddress> iclbMailingAddress = new Collection<busMailingAddress>();
            foreach (busOrganization lobjOrg in _iclbOrg)
            {
                if (lobjOrg.ibusOrgContactPrimaryAddress != null)
                {
                    busMailingAddress lobjMailingAddress = new busMailingAddress();
                    lobjMailingAddress.icdoMailingAddress = new cdoMailingAddress();
                    lobjMailingAddress.icdoMailingAddress.lintID = lobjOrg.icdoOrganization.org_id;
                    lobjMailingAddress.icdoMailingAddress.lstrFullName = lobjOrg.icdoOrganization.org_name;
                    if (lobjOrg.ibusContact.full_name != string.Empty)
                        lobjMailingAddress.icdoMailingAddress.lstrContactName = lobjOrg.ibusContact.full_name;
                    if (lobjOrg.ibusContact.icdoContact.first_name != null)
                        lobjMailingAddress.icdoMailingAddress.lstrFirstName = lobjOrg.ibusContact.icdoContact.first_name.Trim().ToUpper();
                    if (lobjOrg.ibusContact.icdoContact.last_name != null)
                        lobjMailingAddress.icdoMailingAddress.lstrLastName = lobjOrg.ibusContact.icdoContact.last_name.Trim().ToUpper();
                    lobjMailingAddress.icdoMailingAddress.lstrAddressLine1 = lobjOrg.ibusOrgContactPrimaryAddress.icdoOrgContactAddress.addr_line_1;
                    lobjMailingAddress.icdoMailingAddress.lstrAddressLine2 = lobjOrg.ibusOrgContactPrimaryAddress.icdoOrgContactAddress.addr_line_2;
                    lobjMailingAddress.icdoMailingAddress.lstrCity = lobjOrg.ibusOrgContactPrimaryAddress.icdoOrgContactAddress.city;
                    lobjMailingAddress.icdoMailingAddress.lstrState = lobjOrg.ibusOrgContactPrimaryAddress.icdoOrgContactAddress.state_value;
                    lobjMailingAddress.icdoMailingAddress.lstrZipCode = lobjOrg.ibusOrgContactPrimaryAddress.icdoOrgContactAddress.zip_code;
                    if (lobjOrg.ibusOrgContactPrimaryAddress.icdoOrgContactAddress.zip_4_code != null)
                        lobjMailingAddress.icdoMailingAddress.lstrZipCode += "-" + lobjOrg.ibusOrgContactPrimaryAddress.icdoOrgContactAddress.zip_4_code;
                    // PIR ID 500
                    if (lobjMailingAddress.icdoMailingAddress.lstrAddressLine1 != null &&
                        lobjMailingAddress.icdoMailingAddress.lstrCity != null &&
                        lobjMailingAddress.icdoMailingAddress.lstrState != null &&
                        lobjMailingAddress.icdoMailingAddress.lstrZipCode != null)
                        iclbMailingAddress.Add(lobjMailingAddress);
                }
            }
            return iclbMailingAddress;
        }

        public Collection<busPerson> GetLookedPersonIDs()
        {
            bool iblnIsTrnManual = false;
            if (iobjPassInfo.itrnFramework.IsNull())
            {
                iobjPassInfo.BeginTransaction();
                iblnIsTrnManual = true;
            }

            //Truncate the Temp Table
            DBFunction.DBNonQuery("Truncate Table SGT_MAILING_LABEL_FILTER_RESULT", iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

            string lstrQuery = "";
            utlMethodInfo lobjMethodInfo = null;

            //Search Criteria if Person Type is Selected.
            if (_iclcPersonType.Count > 0)
            {
                foreach (cdoMailingLabelPersonType lobjPersonType in _iclcPersonType)
                {
                    switch (lobjPersonType.person_type_value)
                    {
                        case busConstant.PersonTypeBeneficiary:
                            if (_iclcPlan.Count > 0)
                            {
                                lobjMethodInfo = iobjPassInfo.isrvMetaDataCache.GetMethodInfo("cdoMailingLabel.GetBeneficiaryPersonIDByPlan");
                                if (lstrQuery != "")
                                    lstrQuery += " union ";
                                lstrQuery += lobjMethodInfo.istrCommand + " WHERE SPA.PLAN_ID in ( ";

                                bool lblnaddComma = false;
                                foreach (cdoMailingLabelPlan lobjPlan in _iclcPlan)
                                {
                                    if (lblnaddComma)
                                        lstrQuery += ",";
                                    lstrQuery += lobjPlan.plan_id.ToString();
                                    lblnaddComma = true;
                                }
                                lstrQuery += " ) ";
                            }
                            else
                            {
                                lobjMethodInfo = iobjPassInfo.isrvMetaDataCache.GetMethodInfo("cdoMailingLabel.GetBeneficiaryPersonID");
                                if (lstrQuery != "")
                                    lstrQuery += " union ";
                                lstrQuery += lobjMethodInfo.istrCommand;
                            }
                            break;

                        case busConstant.PersonTypeMember:
                            if (_iclcPlan.Count > 0)
                            {
                                lobjMethodInfo = iobjPassInfo.isrvMetaDataCache.GetMethodInfo("cdoMailingLabel.GetMemberIDByPlan");
                                if (lstrQuery != "")
                                    lstrQuery += " union ";
                                lstrQuery += lobjMethodInfo.istrCommand + " WHERE PLAN_ID in ( ";

                                bool lblnaddComma = false;
                                foreach (cdoMailingLabelPlan lobjPlan in _iclcPlan)
                                {
                                    if (lblnaddComma)
                                        lstrQuery += ",";
                                    lstrQuery += lobjPlan.plan_id.ToString();
                                    lblnaddComma = true;
                                }
                                lstrQuery += " ) ";
                            }
                            else
                            {
                                lobjMethodInfo = iobjPassInfo.isrvMetaDataCache.GetMethodInfo("cdoMailingLabel.GetMemberID");
                                if (lstrQuery != "")
                                    lstrQuery += " union ";
                                lstrQuery += lobjMethodInfo.istrCommand;
                            }
                            break;

                        case busConstant.PersonTypeDependent:
                            if (_iclcPlan.Count > 0)
                            {
                                lobjMethodInfo = iobjPassInfo.isrvMetaDataCache.GetMethodInfo("cdoMailingLabel.GetDependentPersonIDByPlan");
                                if (lstrQuery != "")
                                    lstrQuery += " union ";
                                lstrQuery += lobjMethodInfo.istrCommand + " WHERE PLAN_ID in ( ";

                                bool lblnaddComma = false;
                                foreach (cdoMailingLabelPlan lobjPlan in _iclcPlan)
                                {
                                    if (lblnaddComma)
                                        lstrQuery += ",";
                                    lstrQuery += lobjPlan.plan_id.ToString();
                                    lblnaddComma = true;
                                }
                                lstrQuery += " ) ";
                            }
                            else
                            {
                                lobjMethodInfo = iobjPassInfo.isrvMetaDataCache.GetMethodInfo("cdoMailingLabel.GetDependentPersonID");
                                if (lstrQuery != "")
                                    lstrQuery += " union ";
                                lstrQuery += lobjMethodInfo.istrCommand;
                            }
                            break;

                        case busConstant.PersonTypeSpouse:
                            lobjMethodInfo = iobjPassInfo.isrvMetaDataCache.GetMethodInfo("cdoMailingLabel.GetSpouse");
                            if (lstrQuery != "")
                                lstrQuery += " union ";
                            lstrQuery += lobjMethodInfo.istrCommand;
                            break;

                        case busConstant.PersonTypeAlternatePayee:
                            if (_iclcPlan.Count > 0)
                            {
                                lobjMethodInfo = iobjPassInfo.isrvMetaDataCache.GetMethodInfo("cdoMailingLabel.GetAlternatePayeeByPlanID");
                                if (lstrQuery != "")
                                    lstrQuery += " union ";
                                lstrQuery += lobjMethodInfo.istrCommand + " WHERE PLAN_ID in ( ";

                                bool lblnaddComma = false;
                                foreach (cdoMailingLabelPlan lobjPlan in _iclcPlan)
                                {
                                    if (lblnaddComma)
                                        lstrQuery += ",";
                                    lstrQuery += lobjPlan.plan_id.ToString();
                                    lblnaddComma = true;
                                }
                                lstrQuery += " ) ";
                            }
                            else
                            {
                                lobjMethodInfo = iobjPassInfo.isrvMetaDataCache.GetMethodInfo("cdoMailingLabel.GetAlternatePayee");
                                if (lstrQuery != "")
                                    lstrQuery += " union ";
                                lstrQuery += lobjMethodInfo.istrCommand;
                            }
                            break;
                        case busConstant.PersonTypePayee:
                            if (_iclcPlan.Count > 0)
                            {
                                lobjMethodInfo = iobjPassInfo.isrvMetaDataCache.GetMethodInfo("cdoMailingLabel.GetPayeeByPlanId");
                                if (lstrQuery != "")
                                    lstrQuery += " union ";
                                lstrQuery += lobjMethodInfo.istrCommand + " WHERE PLAN_ID in ( ";

                                bool lblnaddComma = false;
                                foreach (cdoMailingLabelPlan lobjPlan in _iclcPlan)
                                {
                                    if (lblnaddComma)
                                        lstrQuery += ",";
                                    lstrQuery += lobjPlan.plan_id.ToString();
                                    lblnaddComma = true;
                                }
                                lstrQuery += " ) ";
                            }
                            else
                            {
                                lobjMethodInfo = iobjPassInfo.isrvMetaDataCache.GetMethodInfo("cdoMailingLabel.GetPayee");
                                if (lstrQuery != "")
                                    lstrQuery += " union ";
                                lstrQuery += lobjMethodInfo.istrCommand;
                            }
                            break;
                        case busConstant.PersonTypeRetiree:
                            if (_iclcPlan.Count > 0)
                            {
                                lobjMethodInfo = iobjPassInfo.isrvMetaDataCache.GetMethodInfo("cdoMailingLabel.GetRetireeByPlan");
                                if (lstrQuery != "")
                                    lstrQuery += " union ";
                                lstrQuery += lobjMethodInfo.istrCommand + " WHERE PLAN_ID in ( ";

                                bool lblnaddComma = false;
                                foreach (cdoMailingLabelPlan lobjPlan in _iclcPlan)
                                {
                                    if (lblnaddComma)
                                        lstrQuery += ",";
                                    lstrQuery += lobjPlan.plan_id.ToString();
                                    lblnaddComma = true;
                                }
                                lstrQuery += " ) ";
                            }
                            else
                            {
                                lobjMethodInfo = iobjPassInfo.isrvMetaDataCache.GetMethodInfo("cdoMailingLabel.GetRetiree");
                                if (lstrQuery != "")
                                    lstrQuery += " union ";
                                lstrQuery += lobjMethodInfo.istrCommand;
                            }
                            break;
                        default:
                            break;
                    }
                }
            }

            // Search Criteria For Person Employment Status.
            if (_icdoMailingLabel.employment_status_value != null)
            {
                lobjMethodInfo = iobjPassInfo.isrvMetaDataCache.GetMethodInfo("cdoMailingLabel.GetPersonByEmploymentStatus");
                if (lstrQuery != "")
                    lstrQuery += " union ";
                lstrQuery += lobjMethodInfo.istrCommand + " WHERE DTL.STATUS_VALUE = '" + _icdoMailingLabel.employment_status_value + "'";
            }

            // Search Criteria For Plan Participation Status.    
            if (_icdoMailingLabel.plan_participation_status_value != null)
            {
                if (_iclcPlan.Count > 0)
                {
                    string _lstrSelectedPlans = string.Empty;
                    int lintLoopCount = 0;
                    foreach (cdoMailingLabelPlan lobjPlan in _iclcPlan)
                    {
                        lintLoopCount++;
                        _lstrSelectedPlans += Convert.ToString(lobjPlan.plan_id);
                        if (_iclcPlan.Count - 1 > lintLoopCount)
                            _lstrSelectedPlans += ",";
                    }
                    lobjMethodInfo = iobjPassInfo.isrvMetaDataCache.GetMethodInfo("cdoMailingLabel.GetPlanParticipationStatus");
                    if (lstrQuery != "")
                        lstrQuery += " union ";
                    lstrQuery += lobjMethodInfo.istrCommand + " WHERE SPA.PLAN_ID IN (" + _lstrSelectedPlans + " ) AND CV.DATA2 = '" +
                                                _icdoMailingLabel.plan_participation_status_value + "'";
                }
            }

            if (lstrQuery != "")
            {
                lstrQuery = "INSERT INTO SGT_MAILING_LABEL_FILTER_RESULT " + lstrQuery;
                DBFunction.DBNonQuery(lstrQuery, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            }

            if (!String.IsNullOrEmpty(_icdoMailingLabel.city))
            {
                lstrQuery = @"DELETE FROM SGT_MAILING_LABEL_FILTER_RESULT WHERE 
                                NOT EXISTS(SELECT 1 FROM SGT_PERSON_ADDRESS B WHERE SGT_MAILING_LABEL_FILTER_RESULT.PERSON_ID = B.PERSON_ID AND B.ADDR_CITY = '" + _icdoMailingLabel.city + "')";

                DBFunction.DBNonQuery(lstrQuery, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            }

            if (!String.IsNullOrEmpty(_icdoMailingLabel.zip_code))
            {
                lstrQuery = @"DELETE FROM SGT_MAILING_LABEL_FILTER_RESULT WHERE 
                                NOT EXISTS(SELECT 1 FROM SGT_PERSON_ADDRESS B WHERE SGT_MAILING_LABEL_FILTER_RESULT.PERSON_ID = B.PERSON_ID 
                                AND B.ADDR_ZIP_CODE IN ('" + _icdoMailingLabel.zip_code.Replace(",", "','") + "'))";

                DBFunction.DBNonQuery(lstrQuery, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            }

            iobjPassInfo.Commit();
            if (!iblnIsTrnManual)
                iobjPassInfo.BeginTransaction();

            DataTable ldtFilteredPerson =
                DBFunction.DBSelect(
                    "SELECT DISTINCT A.* FROM SGT_PERSON A (NOLOCK) INNER JOIN SGT_MAILING_LABEL_FILTER_RESULT B (NOLOCK) ON A.PERSON_ID = B.PERSON_ID",  iobjPassInfo.iconFramework,  iobjPassInfo.itrnFramework);

            DataTable ldtAddress =
                DBFunction.DBSelect(
                    "SELECT DISTINCT A.*,C.County FROM SGT_PERSON_ADDRESS A INNER JOIN SGT_MAILING_LABEL_FILTER_RESULT B  ON A.PERSON_ID = B.PERSON_ID INNER JOIN SGT_COUNTY_REF C ON C.CITY = A.ADDR_CITY",
                    new Collection<utlWhereClause>(), iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

            Collection<busPerson> iclbFilteredPerson = new Collection<busPerson>();
            foreach (DataRow ldrRow in ldtFilteredPerson.Rows)
            {
                busPerson lobjPerson = new busPerson();
                lobjPerson.icdoPerson = new cdoPerson();
                lobjPerson.icdoPerson.LoadData(ldrRow);

                bool lblnAddressFound = GetPersonCurrentAddressByType(lobjPerson, ldtAddress, busConstant.AddressTypeTemporary);
                if (!lblnAddressFound)
                {
                    lblnAddressFound = GetPersonCurrentAddressByType(lobjPerson, ldtAddress, busConstant.AddressTypePermanent);
                }

                //lobjPerson.LoadPersonCurrentAddress();
                bool lblnAddPerson = true;
                if (lobjPerson.ibusPersonCurrentAddress != null)
                {
                    if (!String.IsNullOrEmpty(_icdoMailingLabel.county))
                    {
                        if (lobjPerson.ibusPersonCurrentAddress.icdoPersonAddress.county.ToLower() != _icdoMailingLabel.county.ToLower())
                            lblnAddPerson = false;
                    }

                    if (!String.IsNullOrEmpty(_icdoMailingLabel.zip_code))
                    {
                        bool lblnZipCodeMatch = false;
                        string[] lstrZipCodes = _icdoMailingLabel.zip_code.Split(new char[] { ',' });
                        for (int i = 0; i <= lstrZipCodes.Length - 1; i++)
                        {
                            if (lobjPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_zip_code == lstrZipCodes[i])
                            {
                                lblnZipCodeMatch = true;
                                break;
                            }
                        }

                        if (!lblnZipCodeMatch)
                            lblnAddPerson = false;
                    }

                    if (!String.IsNullOrEmpty(_icdoMailingLabel.city))
                    {
                        if (lobjPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_city.Trim().ToLower() != _icdoMailingLabel.city.Trim().ToLower())
                            lblnAddPerson = false;
                    }
                }

                // Search Criteria if Exclude Email Prefrence Flag is selected.
                if (_icdoMailingLabel.exclude_email_preference_flag == busConstant.Flag_Yes)
                {
                    if (lobjPerson.icdoPerson.communication_preference_value == busConstant.PersonCommPrefMail)
                    {
                        lblnAddPerson = false;
                    }
                }

                if (lblnAddPerson)
                    iclbFilteredPerson.Add(lobjPerson);
            }

            return iclbFilteredPerson;
        }

        private bool GetPersonCurrentAddressByType(busPerson aobjPerson, DataTable adtAddress, string astrAddressType)
        {
            //Filter By Person ID Address Type
            DataRow[] ldtrFilterRow =
                adtAddress.Select("person_id = " + aobjPerson.icdoPerson.person_id.ToString() +
                                  " and address_type_value = '" + astrAddressType + "'");

            Collection<busPersonAddress> _iclbTempPersonAddress = GetCollection<busPersonAddress>(ldtrFilterRow, "icdoPersonAddress");
            foreach (busPersonAddress _lbusPersonAddress in _iclbTempPersonAddress)
            {
                if (busGlobalFunctions.CheckDateOverlapping(DateTime.Today,
                                                            _lbusPersonAddress.icdoPersonAddress.start_date,
                                                            _lbusPersonAddress.icdoPersonAddress.end_date))
                {
                    aobjPerson.ibusPersonCurrentAddress = _lbusPersonAddress;
                    aobjPerson.ibusPersonCurrentAddress.icdoPersonCurrentAddress = _lbusPersonAddress.icdoPersonAddress;
                    return true;
                }
            }
            return false;
        }

        public Collection<busOrganization> GetLookedOrgIDs()
        {
            Collection<busOrganization> _iclbOrg = new Collection<busOrganization>();
            Collection<busOrganization> _iclbFilteredOrg = new Collection<busOrganization>();

            // Passing Empty strings if the value is null.
            DataTable ldtbOrg = busBase.Select("cdoMailingLabel.GetActiveOrgsByOrgAndEmpType",
                new object[2] { (_icdoMailingLabel.org_type_value==null ? string.Empty : _icdoMailingLabel.org_type_value),
                            ( _icdoMailingLabel.employer_type_value==null ? string.Empty : _icdoMailingLabel.employer_type_value)});
            _iclbOrg = GetCollection<busOrganization>(ldtbOrg, "icdoOrganization");

            // Load Addressess by Contact Role.
            if (_icdoMailingLabel.org_contact_role_value != null)
            {
                _iclbFilteredOrg = new Collection<busOrganization>();
                foreach (busOrganization lobjOrg in _iclbOrg)
                {
                    //Get the Primary Address of Given Org Contact Role
                    lobjOrg.LoadOrgContactPrimaryAddressByRole(_icdoMailingLabel.org_contact_role_value);
                    if (lobjOrg.ibusOrgContactPrimaryAddress.icdoOrgContactAddress.contact_org_address_id != 0)
                        _iclbFilteredOrg.Add(lobjOrg);
                }
                _iclbOrg = _iclbFilteredOrg;
            }
            else
            {
                foreach (busOrganization lobjOrg in _iclbOrg)
                {
                    //By Default,Load the Org Contact Primary Address for the Role "Primary Authroized Agent"
                    lobjOrg.LoadOrgContactPrimaryAddressByRole(busConstant.OrgContactRolePrimaryAuthorizedAgent);

                    //If no primary authorized agent defined, get the earliest org contact (Ref : Maik Mail)
                    if (lobjOrg.ibusOrgContactPrimaryAddress.icdoOrgContactAddress.contact_org_address_id == 0)
                    {
                        DataTable ldtOrgContact = Select("cdoMailingLabel.GetOrgContact", new object[1] { lobjOrg.icdoOrganization.org_id });
                        if (ldtOrgContact.Rows.Count > 0)
                        {
                            lobjOrg.LoadOrgContactPrimaryAddressByContact(Convert.ToInt32(ldtOrgContact.Rows[0]["CONTACT_ID"]));
                        }
                        else
                        {
                            //If there is no org contact Available, Create a Empty Instance of Contact Object
                            lobjOrg.ibusContact = new busContact();
                            lobjOrg.ibusContact.icdoContact = new cdoContact();

                            //Load the Org Primary Address
                            lobjOrg.LoadOrgPrimaryAddress();
                            lobjOrg.ibusOrgContactPrimaryAddress = lobjOrg.ibusOrgPrimaryAddress;
                        }
                    }
                }
            }


            // Search Criteria for the Plans Checked.
            if (_iclcPlan.Count > 0)
            {
                _iclbFilteredOrg = new Collection<busOrganization>();
                foreach (busOrganization lobjOrg in _iclbOrg)
                {
                    foreach (cdoMailingLabelPlan lobjPlan in _iclcPlan)
                    {
                        int lintCount = Convert.ToInt32(DBFunction.DBExecuteScalar("cdoMailingLabel.IsOrgExistsInPlan", new object[2] { lobjPlan.plan_id, lobjOrg.icdoOrganization.org_id },
                                        iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework));
                        if (lintCount > 0)
                            if (!_iclbFilteredOrg.Contains(lobjOrg))
                                _iclbFilteredOrg.Add(lobjOrg);
                    }
                }
                _iclbOrg = _iclbFilteredOrg;
            }

            // Search Criteria if County is Selected.
            if (_icdoMailingLabel.county != null)
            {
                _iclbFilteredOrg = new Collection<busOrganization>();
                DataTable ldtbCities = busBase.Select("cdoMailingLabel.GetCitiesByCounty", new object[1] { _icdoMailingLabel.county });
                foreach (busOrganization lobjOrg in _iclbOrg)
                {
                    foreach (DataRow dr in ldtbCities.Rows)
                    {
                        if (lobjOrg.ibusOrgContactPrimaryAddress.icdoOrgContactAddress.city != null)
                            if (lobjOrg.ibusOrgContactPrimaryAddress.icdoOrgContactAddress.city.Trim().ToLower() == Convert.ToString(dr["CITY"]).Trim().ToLower())
                                _iclbFilteredOrg.Add(lobjOrg);
                    }
                }
                _iclbOrg = _iclbFilteredOrg;
            }

            // Search Criteria if City is Entered.
            if (_icdoMailingLabel.city != null)
            {
                _iclbFilteredOrg = new Collection<busOrganization>();
                foreach (busOrganization lobjOrg in _iclbOrg)
                {
                    if (lobjOrg.ibusOrgContactPrimaryAddress.icdoOrgContactAddress.city != null)
                        if (lobjOrg.ibusOrgContactPrimaryAddress.icdoOrgContactAddress.city.Trim().ToLower() == _icdoMailingLabel.city.Trim().ToLower())
                            _iclbFilteredOrg.Add(lobjOrg);
                }
                _iclbOrg = _iclbFilteredOrg;
            }

            // Search Criteria if ZipCode is Entered.
            if (_icdoMailingLabel.zip_code != null)
            {
                _iclbFilteredOrg = new Collection<busOrganization>();
                string[] lstrZipCodes = _icdoMailingLabel.zip_code.Split(new char[] { ',' });
                foreach (busOrganization lobjOrg in _iclbOrg)
                {
                    for (int i = 0; i <= lstrZipCodes.Length - 1; i++)
                    {
                        if (lobjOrg.ibusOrgContactPrimaryAddress.icdoOrgContactAddress.zip_code == lstrZipCodes[i])
                            _iclbFilteredOrg.Add(lobjOrg);
                    }
                }
                _iclbOrg = _iclbFilteredOrg;
            }

            return _iclbOrg;
        }

        public bool IsPersonAndOrgEntered()
        {
            int lintCount = _iclcPersonType.Count;
            foreach (cdoMailingLabelPersonType lobjPersonType in _iclcPersonType)
            {
                if (lobjPersonType.ienuObjectState == ObjectState.CheckListDelete)
                {
                    lintCount -= 1;
                }
            }
            if (((lintCount > 0) || (_icdoMailingLabel.exclude_email_preference_flag == busConstant.Flag_Yes) || (_icdoMailingLabel.employment_status_value != null)) &&
                ((_icdoMailingLabel.org_contact_role_value != null) || (_icdoMailingLabel.org_type_value != null) || (_icdoMailingLabel.employer_type_value != null)))
            {
                return true;
            }
            return false;
        }

        public bool IsValidZipCode()
        {
            if (_icdoMailingLabel.zip_code != null)
            {
                Regex lobjRexp = new Regex("^[0-9]{5}$");
                string[] lstrZipCode = _icdoMailingLabel.zip_code.Split(busConstant.ZipCodeSeparator);
                foreach (string Zip in lstrZipCode)
                {
                    if (Zip != string.Empty)
                    {
                        if (!(lobjRexp.IsMatch(Zip)))
                            return false;
                    }
                }
            }
            return true;
        }

        public bool IsValidFileName()
        {
            if (_icdoMailingLabel.file_name != null)
            {
                Regex lobjexp = new Regex("[^A-Za-z0-9_.`~!@#$%^&\\-+=;',\\s]");
                if (lobjexp.IsMatch(_icdoMailingLabel.file_name))
                    return false;
            }
            return true;
        }

        public bool IsCityValidForCounty()
        {
            if (_icdoMailingLabel.city != null)
            {
                String lstrCounty = Convert.ToString(DBFunction.DBExecuteScalar("cdoCountyRef.LOOKUP",
                    new object[1] { _icdoMailingLabel.city }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework));
                if (lstrCounty == string.Empty)
                    return false;
                if (_icdoMailingLabel.county != null)
                {
                    if (_icdoMailingLabel.county.ToLower() == lstrCounty.ToLower())
                        return true;
                    else
                        return false;
                }
            }
            return true;
        }

        public bool IsNoCriteriaEntered()
        {
            if ((_iclcPersonType.Count == 0) && (_iclcPlan.Count == 0) && (_icdoMailingLabel.plan_participation_status_value == null) &&
                (_icdoMailingLabel.employment_status_value == null) && (_icdoMailingLabel.org_type_value == null) &&
                (_icdoMailingLabel.employer_type_value == null) && (_icdoMailingLabel.org_contact_role_value == null) && (_icdoMailingLabel.city == null) &&
                (_icdoMailingLabel.county == null) && (_icdoMailingLabel.zip_code == null))
            {
                return true;
            }
            return false;
        }

        public bool IsValidCriteria()
        {
            if (((_iclcPlan.Count > 0) || (_icdoMailingLabel.county != null) || (_icdoMailingLabel.city != null) || (_icdoMailingLabel.zip_code != null))
                && ((_iclcPersonType.Count == 0) && (_icdoMailingLabel.employment_status_value == null) && (_icdoMailingLabel.org_type_value == null)
                && (_icdoMailingLabel.employer_type_value == null) && (_icdoMailingLabel.org_contact_role_value == null)))
            {
                return false;
            }
            return true;
        }
    }
}
