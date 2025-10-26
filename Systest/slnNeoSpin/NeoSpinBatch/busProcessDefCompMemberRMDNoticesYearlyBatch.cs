using System;
using System.Collections.ObjectModel;
using System.Data;
using NeoSpin.BusinessObjects;
using Sagitec.BusinessObjects;
using NeoSpin.CustomDataObjects;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NeoSpinBatch
{
    public class busProcessDefCompMemberRMDNoticesYearlyBatch : busNeoSpinBatch
    {
        public void ProcessDefCompMemberRMDNoticesYearly()
        {
            istrProcessName = "Process Def Comp Member RMD Notices - Yearly Batch";
            DateTime ldtBatchDate = iobjSystemManagement.icdoSystemManagement.batch_date;
            DateTime ldtBatchEndDate = new DateTime(ldtBatchDate.AddYears(-1).Year, 12, 31);
            // Records Fetched as per the rule: BR-093-11
            DataTable ldtbResult = busBase.Select("cdoBenefitApplication.DefCompRMDNoticeYearlyBatch", new object[1] { ldtBatchEndDate });
            if (ldtbResult.Rows.Count > 0)
            {
                idlgUpdateProcessLog("Processing Fetched Records", "INFO", istrProcessName);
                foreach (DataRow dr in ldtbResult.Rows)
                {
                    busPersonAccountDeferredComp lobjPADefComp = new busPersonAccountDeferredComp
                    {
                        icdoPersonAccountDeferredComp = new cdoPersonAccountDeferredComp(),
                        icdoPersonAccount = new cdoPersonAccount(),
                        ibusPerson = new busPerson { icdoPerson = new cdoPerson() }
                    };
                    lobjPADefComp.icdoPersonAccount.LoadData(dr);
                    lobjPADefComp.icdoPersonAccountDeferredComp.LoadData(dr);
                    lobjPADefComp.ibusPerson.icdoPerson.LoadData(dr);

                    idlgUpdateProcessLog("Processing Person id = " + lobjPADefComp.icdoPersonAccount.person_id, "INFO", istrProcessName);
                    lobjPADefComp.istrIsMonthlyBatch = busConstant.Flag_No;
                    lobjPADefComp.idteRMDEligibleDate = lobjPADefComp.ibusPerson.icdoPerson.date_of_birth.AddMonths(876);

                    if (lobjPADefComp.icolPersonAccountDeferredCompProvider == null)
                        lobjPADefComp.LoadPersonAccountProviders();

                    IEnumerable<busPersonAccountDeferredCompProvider> lenmDefCompProvider =
                        lobjPADefComp.icolPersonAccountDeferredCompProvider.OrderByDescending(o => o.icdoPersonAccountDeferredCompProvider.end_date_no_null);

                    lobjPADefComp.iclbPADefCompProviders = new Collection<busPersonAccountDeferredCompProvider>();
                    foreach (busPersonAccountDeferredCompProvider lobjDefCompProvider in lobjPADefComp.icolPersonAccountDeferredCompProvider)
                    {
                        lobjDefCompProvider.SetProviderContact();
                        if (string.IsNullOrEmpty(lobjDefCompProvider.icdoPersonAccountDeferredCompProvider.payment_status_value) &&
                            (string.IsNullOrEmpty(lobjDefCompProvider.icdoPersonAccountDeferredCompProvider.assets_with_provider_value) ||
                            lobjDefCompProvider.icdoPersonAccountDeferredCompProvider.assets_with_provider_value == busConstant.AssetsWithProviderMember) &&
                            !lobjPADefComp.iclbPADefCompProviders.Where(o => o.icdoPersonAccountDeferredCompProvider.provider_org_plan_id ==
                                lobjDefCompProvider.icdoPersonAccountDeferredCompProvider.provider_org_plan_id).Any())
                        {
                            lobjPADefComp.iclbPADefCompProviders.Add(lobjDefCompProvider);
                        }                        
                    }
                    //PIR 23294	- 457 Deferred Comp RMD Letters // ENR-5601 - When populating provider on template sort by Person Account Provider ID desc
                    lobjPADefComp.iclbPADefCompProviders = lobjPADefComp.iclbPADefCompProviders.OrderByDescending(o => o.icdoPersonAccountDeferredCompProvider.person_account_provider_id).ToList().ToCollection();
                    // Generate Correspondence
                    //ArrayList larrlist = new ArrayList();
                    //larrlist.Add(lobjPADefComp);
                    Hashtable lhstDummyTable = new Hashtable();
                    lhstDummyTable.Add("sfwCallingForm", "Batch");
                    CreateCorrespondence("ENR-5601", lobjPADefComp, lhstDummyTable);
                    lobjPADefComp.icdoPersonAccount.def_comp_yearly_letter_flag = busConstant.Flag_Yes;
                    lobjPADefComp.icdoPersonAccount.Update();
                }
            }
            else
                idlgUpdateProcessLog("No Records Fetched", "INFO", istrProcessName);
        }
    }
}
