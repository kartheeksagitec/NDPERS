 #region Using directives

using System;
using System.Data;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using NeoSpin.CustomDataObjects;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.DBUtility;

#endregion

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busBenefitDroApplicationLookup : busBenefitDroApplicationLookupGen
    {
        public override ArrayList ValidateNew(Hashtable ahstParam)
        {
            ArrayList larrErrors = new ArrayList();

            if (ahstParam["plan_id"].ToString() == "")
            {
                utlError lobjError = null;
                lobjError = AddError(1057, "");
                larrErrors.Add(lobjError);
            }
            if (((ahstParam["dro_model_value"].ToString()) == "") || (ahstParam["dro_model_value"].ToString()) == "ALL")
            {
                utlError lobjError = null;
                lobjError = AddError(7632, "");
                larrErrors.Add(lobjError);
            }
            if (ahstParam["member_perslink_id"].ToString() == "")
            {
                utlError lobjError = null;
                lobjError = AddError(1902, "");
                larrErrors.Add(lobjError);
            }
            if ((ahstParam["alternate_payee_id"].ToString() == ""))
            {
                utlError lobjError = null;
                lobjError = AddError(7601, "");
                larrErrors.Add(lobjError);
            }
            if (larrErrors.Count == 0)
            {               
                if ((ahstParam["alternate_payee_id"].ToString()!="") )
                {
                    busPerson lobjPerson = new busPerson();
                    if (!lobjPerson.FindPerson(Convert.ToInt32(ahstParam["alternate_payee_id"])))
                    {
                        utlError lobjError = null;
                        lobjError = AddError(7627, "");
                        larrErrors.Add(lobjError);
                    }
                }
                if (ahstParam["member_perslink_id"].ToString() != "")
                {
                    busPerson lobjPerson = new busPerson();
                    lobjPerson.FindPerson(Convert.ToInt32(ahstParam["member_perslink_id"]));

                    if (lobjPerson.iclbDROApplication == null)
                        lobjPerson.LoadDROApplications();
                    foreach (busBenefitDroApplication lobjdroApplication in lobjPerson.iclbDROApplication)
                    {
                        if ((lobjdroApplication.icdoBenefitDroApplication.member_perslink_id.ToString() == ahstParam["member_perslink_id"].ToString()) &&
                            (lobjdroApplication.icdoBenefitDroApplication.dro_model_value == ahstParam["dro_model_value"].ToString()) &&
                            (lobjdroApplication.icdoBenefitDroApplication.plan_id.ToString() == ahstParam["plan_id"].ToString()) &&
                            (lobjdroApplication.icdoBenefitDroApplication.alternate_payee_perslink_id.ToString() == ahstParam["alternate_payee_id"].ToString()) &&
                            (lobjdroApplication.IsDROApplicationApprovedOrReceivedOrQualified()))
                        {
                            utlError lobjError = null;
                            lobjError = AddError(7606, "");
                            larrErrors.Add(lobjError);
                        }
                    }
                }
                if ((ahstParam["dro_model_value"].ToString() != busConstant.DROApplicationModelDeferredCompModel) &&
                    (Convert.ToInt32(ahstParam["plan_id"]) == busConstant.PlanIdDeferredCompensation))
                {
                    utlError lobjError = null;
                    lobjError = AddError(7628, "");
                    larrErrors.Add(lobjError);
                }
                if ((ahstParam["dro_model_value"].ToString() != busConstant.DROApplicationModelDCModel) &&
                    (Convert.ToInt32(ahstParam["plan_id"]) == busConstant.PlanIdDC ||
                    Convert.ToInt32(ahstParam["plan_id"]) == busConstant.PlanIdDC2025 || //PIR 25920
                    Convert.ToInt32(ahstParam["plan_id"]) == busConstant.PlanIdDC2020)) //PIR 20232
                {
                    utlError lobjError = null;
                    lobjError = AddError(7628, "");
                    larrErrors.Add(lobjError);
                }                
                else if ((ahstParam["dro_model_value"].ToString() == busConstant.DROApplicationModelDeferredCompModel) &&
                    (Convert.ToInt32(ahstParam["plan_id"]) != busConstant.PlanIdDeferredCompensation))
                {
                    utlError lobjError = null;
                    lobjError = AddError(7624, "");
                    larrErrors.Add(lobjError);
                }
                else if ((ahstParam["dro_model_value"].ToString() == busConstant.DROApplicationModelDCModel) &&
                         (Convert.ToInt32(ahstParam["plan_id"]) != busConstant.PlanIdDC &&
                         Convert.ToInt32(ahstParam["plan_id"]) != busConstant.PlanIdDC2025 && //PIR 2590
                        Convert.ToInt32(ahstParam["plan_id"]) != busConstant.PlanIdDC2020)) //PIR 20232
                {
                    utlError lobjError = null;
                    lobjError = AddError(7631, "");
                    larrErrors.Add(lobjError);
                }
                else if ((ahstParam["dro_model_value"].ToString() == busConstant.DROApplicationModelActiveDBModel) ||
                        (ahstParam["dro_model_value"].ToString() == busConstant.DROApplicationModelRetireeDBModel))
                {
                    if (ahstParam["plan_id"].ToString() == busConstant.Plan_ID_Job_Service)
                    {
                        utlError lobjError = null;
                        lobjError = AddError(7611, "");
                        larrErrors.Add(lobjError);
                    }
                }
                else if ((ahstParam["dro_model_value"].ToString() == busConstant.DROApplicationModelActiveDBModel) ||
                   (ahstParam["dro_model_value"].ToString() == busConstant.DROApplicationModelRetireeDBModel))
                {
                    busPlan lobjPlan = new busPlan();
                    lobjPlan.FindPlan(Convert.ToInt32(ahstParam["plan_id"]));
                    if (!lobjPlan.IsDBRetirementPlan())
                    {
                        utlError lobjError = null;
                        lobjError = AddError(7613, "");
                        larrErrors.Add(lobjError);
                    }
                }
                else if ((ahstParam["dro_model_value"].ToString() == busConstant.DROApplicationModelActiveJobServiceModel) ||
                      (ahstParam["dro_model_value"].ToString() == busConstant.DROApplicationModelRetiredJobServiceModel))
                {
                    if ((ahstParam["plan_id"].ToString() != busConstant.Plan_ID_Job_Service))
                    {
                        utlError lobjError = null;
                        lobjError = AddError(7612, "");
                        larrErrors.Add(lobjError);
                    }
                }
                //check whether member is enrolled in the selected plan with status Enrolled Or Suspended
                if ((ahstParam["member_perslink_id"].ToString() != "") && (ahstParam["plan_id"].ToString() != ""))
                {
                    int lintPersonId = Convert.ToInt32(ahstParam["member_perslink_id"].ToString());
                    int lintPlanId = Convert.ToInt32(ahstParam["plan_id"].ToString());

                    if ((ahstParam["dro_model_value"].ToString() != busConstant.DROApplicationModelRetiredJobServiceModel) &&
                      (ahstParam["dro_model_value"].ToString() != busConstant.DROApplicationModelRetireeDBModel))
                    {
                        DataTable ldtbGetCountOfPersonAccount = busBase.Select("cdoBenefitDroApplication.LoadEnrolledOrSuspendedPerson", new object[2] { lintPersonId, lintPlanId });
                        if (ldtbGetCountOfPersonAccount.Rows.Count == 0)
                        {
                            //checks if the person has enrolled in the plan
                            utlError lobjError = null;
                            lobjError = AddError(1914, "");
                            larrErrors.Add(lobjError);
                        }
                    }
                    else
                    {
                        DataTable ldtbGetCountOfPersonAccount = busBase.Select("cdoBenefitDroApplication.LoadMemberReceivingPayeeAccount", 
                                                                new object[2] { lintPersonId, lintPlanId });
                        if (ldtbGetCountOfPersonAccount.Rows.Count == 0)
                        {
                            //checks if the person has enrolled in the plan
                            utlError lobjError = null;
                            lobjError = AddError(7634, "");
                            larrErrors.Add(lobjError);
                        }
                    }
                }
            }
            return larrErrors;
        }
    }
}