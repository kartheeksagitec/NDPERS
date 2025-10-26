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
using System.Linq;
using System.Linq.Expressions;

#endregion

namespace NeoSpin.BusinessObjects
{
    /// <summary>
    /// Class NeoSpin.BusinessObjects.busMASBatchRequest:
    /// Inherited from busMASBatchRequestGen, the class is used to customize the business object busMASBatchRequestGen.
    /// </summary>
    [Serializable]
    public class busMASBatchRequest : busMASBatchRequestGen
    {
        public busPerson ibusMember { get; set; }

        public void LoadMember()
        {
            if (ibusMember.IsNull()) ibusMember = new busPerson { icdoPerson = new cdoPerson() };
            ibusMember.FindPerson(icdoMasBatchRequest.person_id);
        }

        public busOrganization  ibusOrganization { get; set; }

        public void LoadOrganization()
        {
            if (ibusOrganization.IsNull()) ibusOrganization = new busOrganization { icdoOrganization = new cdoOrganization() };
            ibusOrganization.FindOrganization(icdoMasBatchRequest.org_id);
        }

        public utlCollection<cdoMasBatchRequestPlan> iclcPensionPlan { get; set; }
        public utlCollection<cdoMasBatchRequestPlan> iclcInsurancePlan { get; set; }

        public utlCollection<cdoMasBatchRequestPlan> iclcNRPensionPlan { get; set; }
        public utlCollection<cdoMasBatchRequestPlan> iclcNRInsurancePlan { get; set; }

        public void LoadPlan()
        {
            iclcPensionPlan = new utlCollection<cdoMasBatchRequestPlan>();
            iclcInsurancePlan = new utlCollection<cdoMasBatchRequestPlan>();
            iclcNRPensionPlan = new utlCollection<cdoMasBatchRequestPlan>();
            iclcNRInsurancePlan = new utlCollection<cdoMasBatchRequestPlan>();
            DataTable ldtbMasPlan = busBase.Select<cdoMasBatchRequestPlan>(new string[1] { "mas_batch_request_id" },
                                                          new object[1] { icdoMasBatchRequest.mas_batch_request_id }, null, null);
            foreach (DataRow ldtrPlan in ldtbMasPlan.Rows)
            {
                cdoMasBatchRequestPlan lcdoMasBatchRequestPlan = new cdoMasBatchRequestPlan();
                lcdoMasBatchRequestPlan.LoadData(ldtrPlan);
                busPlan lbusPlan = new busPlan { icdoPlan = new cdoPlan() };
                lbusPlan.FindPlan(Convert.ToInt32(ldtrPlan["plan_id"]));
                if (lbusPlan.icdoPlan.benefit_type_value == busConstant.PlanBenefitTypeRetirement)
                {
                    if (icdoMasBatchRequest.group_type_value == busConstant.GroupTypeRetired)
                        iclcPensionPlan.Add(lcdoMasBatchRequestPlan);
                    else
                        iclcNRPensionPlan.Add(lcdoMasBatchRequestPlan);
                }
                else if (lbusPlan.icdoPlan.benefit_type_value == busConstant.PlanBenefitTypeInsurance ||
                         lbusPlan.icdoPlan.benefit_type_value == busConstant.PlanBenefitTypeFlex)
                {
                    if (icdoMasBatchRequest.group_type_value == busConstant.GroupTypeRetired)
                        iclcInsurancePlan.Add(lcdoMasBatchRequestPlan);
                    else
                        iclcNRInsurancePlan.Add(lcdoMasBatchRequestPlan);
                }
            }
        }

        public override void SetParentKey(Sagitec.DataObjects.doBase aobjBase)
        {
            if (aobjBase is cdoMasBatchRequestPlan)
            {
                cdoMasBatchRequestPlan lcdoBatchRequestPlan = (cdoMasBatchRequestPlan)aobjBase;
                lcdoBatchRequestPlan.mas_batch_request_id = icdoMasBatchRequest.mas_batch_request_id;
            }
        }

        public bool IsMemberValid()
        {
            DataTable ldtbResult = Select("entMASBatchRequest.GetMemberCount", new object[3]{
                                    icdoMasBatchRequest.mas_batch_request_id,
                                    icdoMasBatchRequest.batch_request_type_value,
                                    icdoMasBatchRequest.person_id});
            if (ldtbResult.Rows.Count > 0)
            {
                int lintCount = Convert.ToInt32(ldtbResult.Rows[0]["TOTALCOUNT"]);
                if (lintCount > 0)
                    return true;
            }
            return false;
        }

        public ArrayList btnCount_Clicked()
        {
            ArrayList larrreturn = new ArrayList();
            DataTable ldtbResult = new DataTable();
            if (icdoMasBatchRequest.group_type_value == busConstant.GroupTypeNonRetired)
            {
                ldtbResult = Select("entMASBatchRequest.GetMemberCount", new object[3]{
                                    icdoMasBatchRequest.mas_batch_request_id,
                                    icdoMasBatchRequest.batch_request_type_value,
                                    icdoMasBatchRequest.person_id});
                if (ldtbResult.Rows.Count > 0)
                {
                    icdoMasBatchRequest.record_count = Convert.ToInt32(ldtbResult.Rows[0]["TOTALCOUNT"]);
                }
            }
            else
            {
                ldtbResult = Select("entMASBatchRequest.GetCountOfRetirees", new object[1] { icdoMasBatchRequest.mas_batch_request_id});
                if (ldtbResult.Rows.Count > 0)
                {
                    icdoMasBatchRequest.record_count = Convert.ToInt32(ldtbResult.Rows[0]["record_count"]);
                }
            }
            //icdoMasBatchRequest.Update();
            larrreturn.Add(this);
            return larrreturn;
        }

        public override void BeforeValidate(utlPageMode aenmPageMode)
        {
            AssignStatementEffectiveDate();
            if (icdoMasBatchRequest.group_type_value == busConstant.GroupTypeRetired)
            {
                if (icdoMasBatchRequest.retired_person_id > 0)
                    icdoMasBatchRequest.person_id = icdoMasBatchRequest.retired_person_id;
            }
            if (icdoMasBatchRequest.batch_request_type_value == busConstant.BatchRequestTypeIndividual && icdoMasBatchRequest.person_id > 0)
            {
                icdoMasBatchRequest.retired_person_id = icdoMasBatchRequest.person_id;
                // PIR ID 2273
                if (icdoMasBatchRequest.group_type_value == busConstant.GroupTypeNonRetired)
                    LoadMember();
            }
            base.BeforeValidate(aenmPageMode);
        }

        public override void BeforePersistChanges()
        {
            if (icdoMasBatchRequest.suppress_warnings_flag == busConstant.Flag_Yes)
                icdoMasBatchRequest.suppress_warnings_by = iobjPassInfo.istrUserID;

            if (icdoMasBatchRequest.ienuObjectState == ObjectState.Insert)
            {
                RemoveCheckListDeleteEntriesInCopyMode();
            }
            if (icdoMasBatchRequest.group_type_value == busConstant.GroupTypeNonRetired)
            {
                if (icdoMasBatchRequest.org_code_non_retired.IsNotNullOrEmpty())
                    icdoMasBatchRequest.org_id = busGlobalFunctions.GetOrgIdFromOrgCode(icdoMasBatchRequest.org_code_non_retired);
            }
            else
            {
                if(icdoMasBatchRequest.org_code_retired.IsNotNullOrEmpty())
                    icdoMasBatchRequest.org_id = busGlobalFunctions.GetOrgIdFromOrgCode(icdoMasBatchRequest.org_code_retired);
            }
            base.BeforePersistChanges();
        }

        private void RemoveCheckListDeleteEntriesInCopyMode()
        {
            if (icdoMasBatchRequest.group_type_value == busConstant.GroupTypeRetired)
            {
                if (iclcPensionPlan != null)
                {
                    RemoveCheckListDeleteEntries(iclcPensionPlan);
                }
                if (iclcInsurancePlan != null)
                {
                    RemoveCheckListDeleteEntries(iclcInsurancePlan);
                }
            }
            else
            {
                if (iclcPensionPlan != null)
                {
                    RemoveCheckListDeleteEntries(iclcNRPensionPlan);
                }
                if (iclcInsurancePlan != null)
                {
                    RemoveCheckListDeleteEntries(iclcNRInsurancePlan);
                }
            }
        }

        private void RemoveCheckListDeleteEntries(utlCollection<cdoMasBatchRequestPlan> aclcPlan)
        {
            for (int i = aclcPlan.Count - 1; i >= 0; i--)
            {
                cdoMasBatchRequestPlan lcdoMASPlan = aclcPlan[i];
                if (lcdoMASPlan.ienuObjectState == ObjectState.CheckListDelete)
                {
                    aclcPlan.Remove(lcdoMASPlan);
                    iarrChangeLog.Remove(lcdoMASPlan);
                }
                else if (lcdoMASPlan.ienuObjectState == ObjectState.None)
                {
                    lcdoMASPlan.ienuObjectState = ObjectState.CheckListInsert;
                    iarrChangeLog.Add(lcdoMASPlan);
                }
            }
        }

        public override void AfterPersistChanges()
        {
            base.AfterPersistChanges();
            if (icdoMasBatchRequest.batch_request_type_value == busConstant.BatchRequestTypeTargeted)
            {
                LoadPlan();
                LoadOrganizationCode();
            }
        }

        private void AssignStatementEffectiveDate()
        {
            if (icdoMasBatchRequest.group_type_value == busConstant.GroupTypeNonRetired)
            {
                switch (icdoMasBatchRequest.batch_request_type_value)
                {
                    case busConstant.BatchRequestTypeAnnual:
                        icdoMasBatchRequest.statement_effective_date = icdoMasBatchRequest.annual_statement_effective_date;
                        break;
                    case busConstant.BatchRequestTypeIndividual:
                        icdoMasBatchRequest.statement_effective_date = icdoMasBatchRequest.individual_statement_effective_date;
                        break;
                    case busConstant.BatchRequestTypeTargeted:
                        icdoMasBatchRequest.statement_effective_date = icdoMasBatchRequest.targeted_statement_effective_date;
                        break;
                    default:
                        break;
                }
            }
            else if (icdoMasBatchRequest.group_type_value == busConstant.GroupTypeRetired)
            {
                switch (icdoMasBatchRequest.batch_request_type_value)
                {
                    case busConstant.BatchRequestTypeAnnual:
                        icdoMasBatchRequest.statement_effective_date = icdoMasBatchRequest.retiree_annual_statement_effective_date;
                        break;
                    case busConstant.BatchRequestTypeIndividual:
                        icdoMasBatchRequest.statement_effective_date = icdoMasBatchRequest.retiree_individual_statement_effective_date;
                        break;
                    case busConstant.BatchRequestTypeTargeted:
                        icdoMasBatchRequest.statement_effective_date = icdoMasBatchRequest.retiree_targeted_statement_effective_date;
                        break;
                    default:
                        break;
                }
            }
        }

        public void LoadStatementEffectiveDate()
        {
            if (icdoMasBatchRequest.group_type_value == busConstant.GroupTypeNonRetired)
            {
                switch (icdoMasBatchRequest.batch_request_type_value)
                {

                    case busConstant.BatchRequestTypeAnnual:
                        icdoMasBatchRequest.annual_statement_effective_date = icdoMasBatchRequest.statement_effective_date;
                        break;
                    case busConstant.BatchRequestTypeIndividual:
                        icdoMasBatchRequest.individual_statement_effective_date = icdoMasBatchRequest.statement_effective_date;
                        break;
                    case busConstant.BatchRequestTypeTargeted:
                        icdoMasBatchRequest.targeted_statement_effective_date = icdoMasBatchRequest.statement_effective_date;
                        break;
                    default:
                        break;
                }
            }
            else if (icdoMasBatchRequest.group_type_value == busConstant.GroupTypeRetired)
            {
                switch (icdoMasBatchRequest.batch_request_type_value)
                {

                    case busConstant.BatchRequestTypeAnnual:
                        icdoMasBatchRequest.retiree_annual_statement_effective_date = icdoMasBatchRequest.statement_effective_date;
                        break;
                    case busConstant.BatchRequestTypeIndividual:
                        icdoMasBatchRequest.retiree_individual_statement_effective_date = icdoMasBatchRequest.statement_effective_date;
                        break;
                    case busConstant.BatchRequestTypeTargeted:
                        icdoMasBatchRequest.retiree_targeted_statement_effective_date = icdoMasBatchRequest.statement_effective_date;
                        break;
                    default:
                        break;
                }
            }
        }

        public void LoadOrganizationCode()
        {
            if (icdoMasBatchRequest.org_id != 0)
            {
                if (icdoMasBatchRequest.group_type_value == busConstant.GroupTypeNonRetired)
                    icdoMasBatchRequest.org_code_non_retired = busGlobalFunctions.GetOrgCodeFromOrgId(icdoMasBatchRequest.org_id);
                else
                    icdoMasBatchRequest.org_code_retired = busGlobalFunctions.GetOrgCodeFromOrgId(icdoMasBatchRequest.org_id);
            }
        }

        /// BR-094-54 - Statement Effective Date should be the last day of a month.
        public bool IsStatementEffectiveDateValid()
        {
            if (icdoMasBatchRequest.statement_effective_date != DateTime.MinValue)
            {
                if (DateTime.DaysInMonth(icdoMasBatchRequest.statement_effective_date.Year, icdoMasBatchRequest.statement_effective_date.Month) ==
                    icdoMasBatchRequest.statement_effective_date.Day)
                    return true;
                return false;
            }
            return true;
        }

        /// BR-094-04 - For Annual Type,  Statement Effective month should be the June.
        public bool IsAnnualStatementEffectiveDateValid()
        {
            if ((icdoMasBatchRequest.statement_effective_date != DateTime.MinValue) &&
                (icdoMasBatchRequest.batch_request_type_value == busConstant.BatchRequestTypeAnnual &&
                icdoMasBatchRequest.group_type_value == busConstant.GroupTypeNonRetired))
            {
                if (icdoMasBatchRequest.statement_effective_date.Month != 6)
                    return false;
            }
            if ((icdoMasBatchRequest.statement_effective_date != DateTime.MinValue) &&
               (icdoMasBatchRequest.batch_request_type_value == busConstant.BatchRequestTypeAnnual &&
               icdoMasBatchRequest.group_type_value == busConstant.GroupTypeRetired))
            {
                if (icdoMasBatchRequest.statement_effective_date.Month != 12)
                    return false;
            }
            return true;
        }

        /// BR-094-56
        public bool IsStatementEffectiveDateEarlierThanFY()
        {
            // SYS PIR ID 2392
            if (icdoMasBatchRequest.created_date != DateTime.MinValue)
            {
                DateTime ldtePrevAnnualRanDate = new DateTime(icdoMasBatchRequest.created_date.AddYears(-1).Year, 06, 30);
                if (icdoMasBatchRequest.statement_effective_date <= ldtePrevAnnualRanDate)
                    return true;
            }
            return false;
        }

        public bool IsPersonExists()
        {
            if (icdoMasBatchRequest.batch_request_type_value == busConstant.BatchRequestTypeIndividual)
            {
                busPerson lobjperson = new busPerson { icdoPerson = new cdoPerson() };
                if (!(lobjperson.FindPerson(icdoMasBatchRequest.person_id)))
                {
                    return false;
                }
            }
            return true;
        }

        /// BR-094-58 Need to Validate with the Selected Job Class, Type & Plan participation status for Targeted Group
        public bool IsJobClassTypeorStatusValid()
        {
            Collection<cdoMasBatchRequestPlan> lclcPlan = new Collection<cdoMasBatchRequestPlan>();
            foreach (cdoMasBatchRequestPlan lcdoInsurancePlan in iclcInsurancePlan)
                lclcPlan.Add(lcdoInsurancePlan);
            foreach (cdoMasBatchRequestPlan lcdoPensionPlan in iclcPensionPlan)
                lclcPlan.Add(lcdoPensionPlan);

            if (lclcPlan != null)
            {
                DataTable ldtbCrossRef = iobjPassInfo.isrvDBCache.GetCacheData("sgt_plan_job_class_crossref", null);

                // Case 1 : If Job Class is selected
                if (!string.IsNullOrEmpty(icdoMasBatchRequest.job_class_value))
                {
                    foreach (cdoMasBatchRequestPlan lcdoMASPlan in lclcPlan)
                    {
                        DataTable lintFilterResult = ldtbCrossRef.AsEnumerable().Where(o => o.Field<int>("plan_id") == lcdoMASPlan.plan_id &&
                            o.Field<string>("job_class_value") == icdoMasBatchRequest.job_class_value).AsDataTable();
                        if (lintFilterResult.Rows.Count == 0)
                            return false;
                    }
                }

                // Case 2 : If Job Class is selected
                if (!string.IsNullOrEmpty(icdoMasBatchRequest.employment_type_value))
                {
                    foreach (cdoMasBatchRequestPlan lcdoMASPlan in lclcPlan)
                    {
                        DataTable lintFilterResult = ldtbCrossRef.AsEnumerable().Where(o => o.Field<int>("plan_id") == lcdoMASPlan.plan_id &&
                            o.Field<string>("job_type_value") == icdoMasBatchRequest.employment_type_value).AsDataTable();
                        if (lintFilterResult.Rows.Count == 0)
                            return false;
                    }
                }

                // Case 3 : If Plan Participation Status is selected // TODO: Need to optimize
                if (!string.IsNullOrEmpty(icdoMasBatchRequest.plan_participation_status_value))
                {
                    Collection<cdoCodeValue> lclbCodeValue = GetCodeValue(337);

                    foreach (cdoMasBatchRequestPlan lcdoMASPlan in lclcPlan)
                    {
                        busPlan lobjPlan = new busPlan { icdoPlan = new cdoPlan() };
                        lobjPlan.FindPlan(lcdoMASPlan.plan_id);
                        DataTable ldbCodeValue = Select<cdoCodeValue>(new string[2] { "code_id", "data1" },
                                new object[2] { 337, lobjPlan.icdoPlan.benefit_type_value }, null, null);
                        bool lbnFlag = false;
                        foreach (DataRow ldtr in ldbCodeValue.Rows)
                        {

                            cdoCodeValue lcdoCodeValue = new cdoCodeValue();
                            lcdoCodeValue.LoadData(ldtr);
                            if (lcdoCodeValue.data2 == icdoMasBatchRequest.plan_participation_status_value)
                            {
                                lbnFlag = true;
                                break;
                            }
                        }
                        if (!lbnFlag)
                            return false;
                    }
                }
            }
            return true;
        }

        public int CreateMASReport()
        {
            int lintCounter = 0;
            if (ldtbAllMASSelection.IsNotNull())
            {
                foreach (DataRow ldtrSelection in ldtbAllMASSelection.Rows)
                {
                    busMASSelection lobjMASSelection = new busMASSelection { icdoMasSelection = new cdoMasSelection() };
                    lobjMASSelection.icdoMasSelection.LoadData(ldtrSelection);

                    //DataSet ldsAnnualStatement = FilterReportData(lobjMASSelection.icdoMasSelection.mas_selection_id);

                    if (icdoMasBatchRequest.batch_request_type_value == busConstant.BatchRequestTypeAnnual)                        
                    {     // Annual Statements - PIR 17506
                        string lstrSummaryStatement = null;
                        string[] lstrTemp = new string[] { };

                        if( icdoMasBatchRequest.mailing_generate_flag == busConstant.Flag_Yes)
                        {
                            DataSet ldsAnnualStatement = FilterReportData(lobjMASSelection.icdoMasSelection.mas_selection_id);
                            string lstrSummaryStmt = CreateReport("rptSummaryMemberStatement.rpt", ldsAnnualStatement,
                           lobjMASSelection.icdoMasSelection.person_id.ToString() + "_", busConstant.ReportMASPath);
                             lstrTemp = lstrSummaryStmt.Split('\\');
                             if (lstrTemp.Count() > 0)
                             {
                                 lstrSummaryStatement = lstrTemp[lstrTemp.Count() - 1];
                             }
                        }                        
                       
                            cdoMasStatementFile lcdoSummaryStatement = new cdoMasStatementFile
                            {
                                mas_selection_id = lobjMASSelection.icdoMasSelection.mas_selection_id,
                                statement_type_id = 3056,
                                statement_type_value = busConstant.SummaryStatementFile,
                                statement_name = lstrSummaryStatement
                            };
                            lcdoSummaryStatement.Insert();
                        

                        // SYS PIR ID 2598: For Annual, do not create Online Member statement. But create the statement file record.
                        ////cdoMasStatementFile lcdoAnnualStatement = new cdoMasStatementFile
                        ////{
                        ////    mas_selection_id = lobjMASSelection.icdoMasSelection.mas_selection_id,
                        ////    statement_type_id = 3056,
                        ////    statement_type_value = busConstant.OnlineStatementFile
                        ////};
                        //lcdoAnnualStatement.Insert();
                        lintCounter++;
                    }
                    //PIR 9790
                    //else
                    //{
                    //    string lstrOnlineStmt = CreateReport("rptOnlineMemberStatement.rpt", ldsAnnualStatement,
                    //        lobjMASSelection.icdoMasSelection.person_id.ToString() + "_", busConstant.ReportMASPath);
                    //    string[] lstrTemp2 = lstrOnlineStmt.Split('\\');
                    //    if (lstrTemp2.Count() > 0)
                    //    {
                    //        string lstrAnnualStatement = lstrTemp2[lstrTemp2.Count() - 1];
                    //        cdoMasStatementFile lcdoAnnualStatement = new cdoMasStatementFile
                    //        {
                    //            mas_selection_id = lobjMASSelection.icdoMasSelection.mas_selection_id,
                    //            statement_type_id = 3056,
                    //            statement_type_value = busConstant.OnlineStatementFile,
                    //            statement_name = lstrAnnualStatement
                    //        };
                    //        lcdoAnnualStatement.Insert();
                    //    }
                    //    lintCounter++;
                    //}
                    // Update the Statement name
                    lobjMASSelection.icdoMasSelection.is_report_created_flag = busConstant.Flag_Yes;
                    lobjMASSelection.icdoMasSelection.Update();
                }
            }
            return lintCounter;
        }

        public void LoadMASSelection(string astrDataPulledFlag, string astrReportGenerateFlag)
        {
            ldtbAllMASSelection = Select("entMASBatchRequest.LoadMASSelectionOrderbyAddress", new object[3] { icdoMasBatchRequest.mas_batch_request_id, 
                                                                                                                astrDataPulledFlag, astrReportGenerateFlag });
        }

        public Collection<cdoMasBatchRequestPlan> iclcPlan { get; set; }

        // This Collection is added mainly for Delete Functionality.
        public void LoadMASPlan()
        {
            if (iclcPlan.IsNull())
                iclcPlan = new Collection<cdoMasBatchRequestPlan>();
            DataTable ldbtList = Select<cdoMasBatchRequestPlan>(new string[1] { "MAS_BATCH_REQUEST_ID" }, new object[1] { icdoMasBatchRequest.mas_batch_request_id }, null, null);
            foreach (DataRow ldtr in ldbtList.Rows)
            {
                cdoMasBatchRequestPlan lcdoPlan = new cdoMasBatchRequestPlan();
                lcdoPlan.LoadData(ldtr);
                iclcPlan.Add(lcdoPlan);
            }
        }

        //override delete in order to delete errors associated with the application
        public override int Delete()
        {
            if (ValidateDelete())
            {
                //delete all errors
                DBFunction.DBNonQuery("entMASBatchRequest.DeleteError", new object[1] { icdoMasBatchRequest.mas_batch_request_id },
                                      iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                return base.Delete();
            }
            return 0;
        }

        public Collection<busMASSelection> iclbMasSelection { get; set; }

        public void LoadSelection()
        {
            if (iclbMasSelection.IsNull()) iclbMasSelection = new Collection<busMASSelection>();
            DataTable ldtbList = Select<cdoMasSelection>(
                new string[1] { "MAS_BATCH_REQUEST_ID" },
                new object[1] { icdoMasBatchRequest.mas_batch_request_id }, null, null);
            iclbMasSelection = GetCollection<busMASSelection>(ldtbList, "icdoMasSelection");
        }
    }
}
