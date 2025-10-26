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
using System.Linq;

#endregion

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busCaseLookup : busCaseLookupGen
    {
        protected override void LoadOtherObjects(DataRow adtrRow, busBase aobjBus)
        {
            busCase lobjCase = (busCase)aobjBus;
            lobjCase.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
            lobjCase.ibusPerson.icdoPerson.LoadData(adtrRow);

            lobjCase.ibusUser = new busUser { icdoUser = new cdoUser() };
            lobjCase.ibusUser.icdoUser.first_name = adtrRow["UserFirstName"].ToString();
            lobjCase.ibusUser.icdoUser.last_name = adtrRow["UserLastName"].ToString();

            base.LoadOtherObjects(adtrRow, aobjBus);
        }

        public override ArrayList ValidateNew(Hashtable ahstParam)
        {
            ArrayList larrErrors = new ArrayList();
            utlError lobjError = null;

            //person id not entered
            if (ahstParam["aint_person_id"].ToString() == "")
            {
                lobjError = AddError(176, "");
                larrErrors.Add(lobjError);
            }
            else
            {
                busPerson lobjPerson = new busPerson { icdoPerson = new cdoPerson() };
                if (!lobjPerson.FindPerson(Convert.ToInt32(ahstParam["aint_person_id"])))
                {
                    lobjError = AddError(176, "");
                    larrErrors.Add(lobjError);
                }
            }
            //case type not selected
            if (((ahstParam["case_type"].ToString()) == "") || ((ahstParam["case_type"].ToString()) == "ALL"))
            {
                lobjError = AddError(2076, "");
                larrErrors.Add(lobjError);
            }
            else
            {
                //if case type is beenfit type then check if appeal type is selected
                if ((ahstParam["case_type"].ToString()) == busConstant.CaseTypeBenefitAppeal)
                {
                    if (ahstParam["appeal_type"].ToString() == "")
                    {
                        lobjError = AddError(2077, "");
                        larrErrors.Add(lobjError);
                    }
                }
            }
            if (larrErrors.Count == 0)
            {
                if ((ahstParam["case_type"].ToString()) == busConstant.CaseTypeFinancialHardship)
                {
                    //check if the person is enrolled in the Deferred Comp with status as Suspended or enrolled
                    DataTable ldtbGetSuspDefCompPersonAccountCount = busBase.Select<cdoPersonAccount>(new string[2] { "person_id", "plan_id" },
                        new object[2] { Convert.ToInt32(ahstParam["aint_person_id"]), busConstant.PlanIdDeferredCompensation }, null, null);

                    if (((from o in ldtbGetSuspDefCompPersonAccountCount.AsEnumerable()
                          where ((o["plan_participation_status_value"].ToString() == busConstant.PlanParticipationStatusDefCompSuspended)
                          || (o["plan_participation_status_value"].ToString() == busConstant.PlanParticipationStatusDefCompEnrolled))
                          select o).Count()) == 0)
                    {
                        //checks if the person has enrolled in Def Comp plan                     
                        lobjError = AddError(2078, "");
                        larrErrors.Add(lobjError);
                    }
                }
                //check whether any payee account exists for the person with status other than completed or cancelled
                //if not then raise error
                if (((ahstParam["case_type"].ToString()) == busConstant.CaseTypeDisabilityRecertification)
                    || ((ahstParam["case_type"].ToString()) == busConstant.CaseTypePre1991DisabilityRecertification))
                {
                    DataTable ldtbDisabilityPayeeAccountCount = busBase.Select("cdoPayeeAccount.GetPayeeAccountForDisability",
                                                                    new object[1] { Convert.ToInt32(ahstParam["aint_person_id"]) });

                    var ldtPayeeAccount = (from o in ldtbDisabilityPayeeAccountCount.AsEnumerable()
                                           where ((o["payment_status_value"].ToString() != busConstant.PayeeAccountStatusDisabilityPaymentCompleted)
                                           && (o["payment_status_value"].ToString() != busConstant.PayeeAccountStatusDisabilityCancelled))
                                           select o).Take(1);
                    if (ldtPayeeAccount.Count() == 0)
                    {
                        lobjError = AddError(2079, "");
                        larrErrors.Add(lobjError);
                    }
                    else
                    {
                        //check if the payee account benefit begin date is greater than july 1991
                        //if yes then throw error only disabillity is allowed for the payee account
                        busPayeeAccount lobjPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
                        lobjPayeeAccount.icdoPayeeAccount.LoadData(ldtPayeeAccount.FirstOrDefault());

                        if ((ahstParam["case_type"].ToString()) == busConstant.CaseTypePre1991DisabilityRecertification)
                        {
                            DateTime ldtDateToCompare = new DateTime(1991, 7, 1);
                            if (lobjPayeeAccount.icdoPayeeAccount.benefit_begin_date >= ldtDateToCompare)
                            {
                                lobjError = AddError(2081, "");
                                larrErrors.Add(lobjError);
                            }
                        }
                        //check if the recertification date is null
                        if (lobjPayeeAccount.icdoPayeeAccount.recertification_date == DateTime.MinValue)
                        {
                            lobjError = AddError(2082, "");
                            larrErrors.Add(lobjError);
                        }
                    }
                }
            }

            return larrErrors;
        }
    }
}
