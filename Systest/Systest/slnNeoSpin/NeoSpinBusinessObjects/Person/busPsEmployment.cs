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
using NeoSpin.DataObjects;
using System.Linq;
using System.Collections.Generic;
using Sagitec.ExceptionPub;


#endregion

namespace NeoSpin.BusinessObjects
{
    /// <summary>
    /// Class NeoSpin.BusinessObjects.busPsEmployment:
    /// Inherited from busPsEmploymentGen, the class is used to customize the business object busPsEmploymentGen.
    /// </summary>
    [Serializable]
    public class busPsEmployment : busPsEmploymentGen
    {
        public Collection<busPsEmployment> iclbPsPersonEmployment { get; set; }
        public Collection<busPsEmployment> iclbUnprocessedPsEmployment { get; set; }
        public Collection<busPsEmployment> iclbProcessedPsEmployment { get; set; }
        //public Collection<busPsEmployment> iclbPersonEmploymentInformativeErrors { get; set; }
        public busPersonEmployment ibusPersonEmployment { get; set; }
        public busPerson ibusPerson { get; set; }
        public ArrayList iarrEmploymentErrorList { get; set; }
        public Dictionary<int, ArrayList> idictPsEmploymentError = new Dictionary<int, ArrayList>();
        //public ArrayList iarrErrors = new ArrayList();
        public int iintOrgId { get; set; }
        public int iintPeoplesoftId { get; set; }
        static string istrUserId = "PERSLinkBatch";
        public busPsPerson ibusPsPerson { get; set; }
        public busPsAddress ibusPsAddress { get; set; }

        //PIR 12578
        public bool iblnEmploymentChangeRequestChangeTypeClassification { get; set; }
        public bool iblnEmploymentChangeRequestChangeTypeEmployment { get; set; }
        public bool iblnEmploymentChangeRequestChangeTypeLOA { get; set; }
        public bool iblnEmploymentChangeRequestChangeTypeLOAM { get; set; }
        public bool iblnEmploymentChangeRequestChangeTypeFMLA { get; set; }
        public busPsEmployment()
        {
        }
        public void LoadUnprocessedPsEmployment()
        {
            //DataTable ldtPsEmployment = Select<cdoPsEmployment>(new string[1] { enmPsEmployment.processed_flag.ToString() },
                                                                       //new object[1] { busConstant.Flag_No }, null, null);
            DataTable ldtPsEmployment = Select("cdoPsEmployment.GetUnprocessedPsEmployment", new object[] { });  ///Testcode
            iclbPsPersonEmployment = GetCollection<busPsEmployment>(ldtPsEmployment, "icdoPsEmployment");

        }
        public void InsertPersonEmployment(Collection<busPsPerson> aclbProcessedPsPerson, Collection<busPsAddress> aclbProcessedPsAddress, ref Collection<busPsEmployment> iclbPersonEmploymentInformativeErrors)
        {


            busWssEmploymentChangeRequest lbusWssEmploymentChangeRequest = null;
            
            iclbUnprocessedPsEmployment = new Collection<busPsEmployment>();
            iclbProcessedPsEmployment = new Collection<busPsEmployment>();
            //iclbPersonEmploymentInformativeErrors = new Collection<busPsEmployment>();
            foreach (busPsEmployment lbusPsEmployment in iclbPsPersonEmployment)
            {
                if (lbusPsEmployment.icdoPsEmployment.empl_start_date <= DateTime.Today.AddDays(30) &&
                   (lbusPsEmployment.icdoPsEmployment.empl_end_date == DateTime.MinValue || lbusPsEmployment.icdoPsEmployment.empl_end_date <= DateTime.Today.AddDays(30)))
 
                {
                    busPsEmployment lobjPSEmp = iclbPsPersonEmployment.Where(i => i.icdoPsEmployment.ssn == lbusPsEmployment.icdoPsEmployment.ssn).OrderBy(i => i.icdoPsEmployment.ps_employment_id).FirstOrDefault();
                    if (lobjPSEmp?.icdoPsEmployment?.ps_employment_id  == lbusPsEmployment?.icdoPsEmployment?.ps_employment_id)
                    {
                        busWssMemberRecordRequest lbusWssMemberRecordRequest = new busWssMemberRecordRequest { icdoWssMemberRecordRequest = new cdoWssMemberRecordRequest() };
                        lbusPsEmployment.idictPsEmploymentError = new Dictionary<int, ArrayList>();
                        lbusPsEmployment.iarrErrors = new ArrayList();
                        if (aclbProcessedPsPerson.Where(o => o.icdoPsPerson.ssn == lbusPsEmployment.icdoPsEmployment.ssn).Count() > 0
                            || aclbProcessedPsAddress.Where(o => o.icdoPsAddress.ssn == lbusPsEmployment.icdoPsEmployment.ssn).Count() > 0)
                        {
                            iclbProcessedPsEmployment.Add(lbusPsEmployment);
                        }
                        else
                        {
                            if (!lbusPsEmployment.icdoPsEmployment.ssn.IsNumeric())
                            {
                                utlError lobjError = null;
                                lobjError = AddError(1037, "SSN must be numeric.");
                                lbusPsEmployment.iarrErrors.Add(lobjError);
                                iclbUnprocessedPsEmployment.Add(lbusPsEmployment);
                                if (!lbusPsEmployment.idictPsEmploymentError.ContainsKey(lbusPsEmployment.icdoPsEmployment.ps_employment_id))
                                    lbusPsEmployment.idictPsEmploymentError.Add(lbusPsEmployment.icdoPsEmployment.ps_employment_id, lbusPsEmployment.iarrErrors);
                                continue;
                            }

                            int lintOrgId = 0;
                            iintPeoplesoftId = Convert.ToInt32(lbusPsEmployment.icdoPsEmployment.peoplesoft_id);

                            lbusPsEmployment.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
                            lbusPsEmployment.ibusPerson = lbusPsEmployment.ibusPerson.LoadPersonBySsn(lbusPsEmployment.icdoPsEmployment.ssn);
                            if (lbusPsEmployment.ibusPerson == null)
                            {
                                utlError lobjError = null;
                                lobjError = AddError(4571, "Person ID does not exist. Person has to be enroll in PERSLink System.");
                                lbusPsEmployment.iarrErrors.Add(lobjError);
                                iclbUnprocessedPsEmployment.Add(lbusPsEmployment);
                                if (!lbusPsEmployment.idictPsEmploymentError.ContainsKey(lbusPsEmployment.icdoPsEmployment.ps_employment_id))
                                    lbusPsEmployment.idictPsEmploymentError.Add(lbusPsEmployment.icdoPsEmployment.ps_employment_id, lbusPsEmployment.iarrErrors);
                                continue;
                            }
                            lbusPsEmployment.ibusPsPerson = LoadPSPersonBySSN(lbusPsEmployment.icdoPsEmployment.ssn);
                            if (lbusPsEmployment.ibusPsPerson != null)
                                lbusPsEmployment.ibusPsAddress = lbusPsEmployment.ibusPsPerson.LoadPSPersonAddressBySSN(lbusPsEmployment.icdoPsEmployment.ssn);
                            DataTable ldtbPersonEmployment = Select("cdoPersonEmployment.GetEmploymentDetail", new object[1] { lbusPsEmployment.icdoPsEmployment.ssn });
                            Collection<busPersonEmployment> lclbPersonEmployment = GetCollection<busPersonEmployment>(ldtbPersonEmployment, "icdoPersonEmployment");

                            DataTable ldtbOrganization = Select("cdoOrganization.GetOrganizationFromOrgCode", new object[1] { lbusPsEmployment.icdoPsEmployment.org_code });
                            if (ldtbOrganization != null)
                                lintOrgId = Convert.ToInt32(ldtbOrganization.Rows[0][enmOrganization.org_id.ToString()]);
                            busOrganization lobjOrganization = new busOrganization();
                            lobjOrganization.FindOrganization(lintOrgId);


                            if (lbusPsEmployment.icdoPsEmployment.empl_end_date == DateTime.MinValue &&
                                (lbusPsEmployment.icdoPsEmployment.job_class_value.IsNull() || lbusPsEmployment.icdoPsEmployment.job_type_value.IsNull()
                                || lbusPsEmployment.icdoPsEmployment.job_class_value.Trim().IsEmpty() || lbusPsEmployment.icdoPsEmployment.job_type_value.Trim().IsEmpty()))
                            {
                                utlError lobjError = null;
                                lobjError = AddError(10221, "Job class/Job type is required.");
                                lbusPsEmployment.iarrErrors.Add(lobjError);
                                iclbUnprocessedPsEmployment.Add(lbusPsEmployment);
                                if (!lbusPsEmployment.idictPsEmploymentError.ContainsKey(lbusPsEmployment.icdoPsEmployment.ps_employment_id))
                                    lbusPsEmployment.idictPsEmploymentError.Add(lbusPsEmployment.icdoPsEmployment.ps_employment_id, lbusPsEmployment.iarrErrors);
                                continue;
                            }

                            //As per mail from Maik dated 03/20/2014, Issue#4
                            if ((lbusPsEmployment.ibusPerson != null) && (lbusPsEmployment.icdoPsEmployment.empl_end_date != DateTime.MinValue))
                            {
                                lbusPsEmployment.ibusPerson.LoadPersonEmployment();
                                busPersonEmployment lbusPersonEmployment = lbusPsEmployment.ibusPerson.icolPersonEmployment.Where(o => o.icdoPersonEmployment.org_id == lintOrgId)
                                                                            .OrderByDescending(o => o.icdoPersonEmployment.start_date).FirstOrDefault();
                                if (lbusPersonEmployment.IsNull())
                                {
                                    utlError lobjError = null;
                                    lobjError = AddError(4720, "Employment does not exist.");
                                    lbusPsEmployment.iarrErrors.Add(lobjError);
                                    iclbUnprocessedPsEmployment.Add(lbusPsEmployment);
                                    if (!lbusPsEmployment.idictPsEmploymentError.ContainsKey(lbusPsEmployment.icdoPsEmployment.ps_employment_id))
                                        lbusPsEmployment.idictPsEmploymentError.Add(lbusPsEmployment.icdoPsEmployment.ps_employment_id, lbusPsEmployment.iarrErrors);
                                    continue;
                                }
                            }

                            #region Dual Case
                            if (lclbPersonEmployment.Count() > 1) //Dual Employment Case: 
                            {
                                //Dual Employment Scenario : 2 Sep
                                busPersonEmployment lbusPersomEmp = lclbPersonEmployment.Where(lobj => lobj.icdoPersonEmployment.org_id == lintOrgId).FirstOrDefault();
                                lbusPersomEmp.LoadLatestPersonEmploymentDetail();
                                if (lbusPersomEmp != null)
                                {
                                    //Same Record Number Same Org     
                                    if (lbusPsEmployment.icdoPsEmployment.ps_empl_record_number == (lbusPersomEmp.icdoPersonEmployment.ps_empl_record_number == null ? "0" : lbusPersomEmp.icdoPersonEmployment.ps_empl_record_number))
                                    {
                                        if (lbusPsEmployment.icdoPsEmployment.loa_date_of_return != DateTime.MinValue
                                            && !(lbusPersomEmp.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.status_value == busConstant.EmploymentStatusLOA
                                            || lbusPersomEmp.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.status_value == busConstant.EmploymentStatusLOAM
                                            || lbusPersomEmp.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.status_value == busConstant.EmploymentStatusFMLA))
                                        {
                                            iclbUnprocessedPsEmployment.Add(lbusPsEmployment);
                                            if (lbusPsEmployment.GetEmploymentChangeRequestInReviewLOA())
                                            {
                                                utlError lobjError = null;
                                                lobjError = AddError(10222, "LOA employment doesn't exist.");
                                                lbusPsEmployment.iarrErrors.Add(lobjError);
                                                iclbPersonEmploymentInformativeErrors.Add(lbusPsEmployment);
                                                if (!lbusPsEmployment.idictPsEmploymentError.ContainsKey(lbusPsEmployment.icdoPsEmployment.ps_employment_id))
                                                    lbusPsEmployment.idictPsEmploymentError.Add(lbusPsEmployment.icdoPsEmployment.ps_employment_id, lbusPsEmployment.iarrErrors);
                                            }
                                            continue;
                                        }
                                        lbusWssEmploymentChangeRequest = new busWssEmploymentChangeRequest { icdoWssEmploymentChangeRequest = new cdoWssEmploymentChangeRequest() };
                                        CreateEmploymentChangeRequest(lbusWssEmploymentChangeRequest, lbusPsEmployment, lbusPersomEmp);
                                        if (lbusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.change_type_value.IsNull())
                                        {
                                            if (lbusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.last_date_of_service == DateTime.MinValue)
                                            {
                                                //utlError lobjError = null;
                                                //lobjError = AddError(10210, "Employment Record already exists.");
                                                if (!iclbProcessedPsEmployment.Contains(lbusPsEmployment))
                                                    iclbProcessedPsEmployment.Add(lbusPsEmployment);
                                                continue;
                                            }
                                        }
                                        ValidatePSRecords(lbusWssEmploymentChangeRequest, lbusPsEmployment, utlPageMode.New);
                                    }
                                    else //Same Organization Different Record number
                                    {
                                        //If Perlink does not have same record number / if it is Null and the employment record is sent for Employment termination then in that case display error in Error Report 
                                        if (lbusPersomEmp.icdoPersonEmployment.ps_empl_record_number != lbusPsEmployment.icdoPsEmployment.ps_empl_record_number && lbusPsEmployment.icdoPsEmployment.empl_end_date != DateTime.MinValue)
                                        {
                                            utlError lobjError = null;
                                            lobjError = AddError(10215, "Empl record number does not exists in Perslink.");
                                            lbusPsEmployment.iarrErrors.Add(lobjError);
                                            iclbUnprocessedPsEmployment.Add(lbusPsEmployment);
                                            iclbPersonEmploymentInformativeErrors.Add(lbusPsEmployment);
                                            if (!lbusPsEmployment.idictPsEmploymentError.ContainsKey(lbusPsEmployment.icdoPsEmployment.ps_employment_id))
                                                lbusPsEmployment.idictPsEmploymentError.Add(lbusPsEmployment.icdoPsEmployment.ps_employment_id, lbusPsEmployment.iarrErrors);
                                            continue;
                                        }
                                        if (lbusPsEmployment.ibusPsPerson != null && lbusPsEmployment.ibusPsAddress != null)
                                        {
                                            CreateMemberRecordRequest(lbusWssMemberRecordRequest, lbusPsEmployment.ibusPsPerson);
                                            lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.org_id = lintOrgId;
                                            ////2 October Changes Start
                                            if (lbusPsEmployment.ibusPsAddress == null)
                                            {
                                                lbusPsEmployment.SetWssPersonAddressInfo(lbusWssMemberRecordRequest);
                                            }
                                            else
                                            {
                                                lbusPsEmployment.ibusPsAddress.SetWssPersonAddressInfo(lbusWssMemberRecordRequest);
                                                if (lbusPsEmployment.ibusPsAddress.iblnIsAddressNotValidated)
                                                {
                                                    utlError lobjError = null;
                                                    lobjError = AddError(130, "Person Address is invalid.");
                                                    lbusPsEmployment.iarrErrors.Add(lobjError);
                                                    iclbUnprocessedPsEmployment.Add(lbusPsEmployment);
                                                    iclbPersonEmploymentInformativeErrors.Add(lbusPsEmployment);
                                                    if (!lbusPsEmployment.idictPsEmploymentError.ContainsKey(lbusPsEmployment.icdoPsEmployment.ps_employment_id))
                                                        lbusPsEmployment.idictPsEmploymentError.Add(lbusPsEmployment.icdoPsEmployment.ps_employment_id, lbusPsEmployment.iarrErrors);
                                                    continue;
                                                }
                                            }
                                            //2 October Changes End
                                            lbusPsEmployment.SetWssPersonEmploymentInfo(lbusWssMemberRecordRequest, lobjOrganization);
												//25926	FILLER: Remove PS Error Report Error of "Employment exists, but Empl Record Number is Different"
                                            //if (lbusPersomEmp.icdoPersonEmployment.ps_empl_record_number == null)
                                            //{
                                            //    if (lbusPsEmployment.icdoPsEmployment.empl_start_date == lbusPersomEmp.icdoPersonEmployment.start_date)
                                            //    {
                                            //        if (lbusPsEmployment.icdoPsEmployment.job_class_value == lbusPersomEmp.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.job_class_value
                                            //        && lbusPsEmployment.icdoPsEmployment.job_type_value == lbusPersomEmp.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.type_value
                                            //        && lbusPsEmployment.icdoPsEmployment.empl_status_value == lbusPersomEmp.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.status_value
                                            //        )
                                            //        {
                                            //            utlError lobjError = null;
                                            //            lobjError = AddError(10214, "Employment exists, but Empl Record Number is different.");
                                            //            lbusPsEmployment.iarrErrors.Add(lobjError);
                                            //            iclbUnprocessedPsEmployment.Add(lbusPsEmployment);
                                            //            iclbPersonEmploymentInformativeErrors.Add(lbusPsEmployment);
                                            //            if (!lbusPsEmployment.idictPsEmploymentError.ContainsKey(lbusPsEmployment.icdoPsEmployment.ps_employment_id))
                                            //                lbusPsEmployment.idictPsEmploymentError.Add(lbusPsEmployment.icdoPsEmployment.ps_employment_id, lbusPsEmployment.iarrErrors);
                                            //            continue;
                                            //        }
                                            //    }
                                            //}
                                            ValidatePSRecords(lbusWssMemberRecordRequest, lbusPsEmployment, utlPageMode.New, iclbPersonEmploymentInformativeErrors);
                                        }
                                        else if (lbusPsEmployment.ibusPerson != null)
                                        {
                                            CreateMemberRecordRequest(lbusWssMemberRecordRequest, lbusPsEmployment.ibusPerson);
                                            lbusPsEmployment.ibusPerson.LoadPersonCurrentAddress();
                                            if (lbusPsEmployment.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.person_address_id > 0)
                                            {
                                                lbusPsEmployment.SetWssPersonAddressInfo(lbusWssMemberRecordRequest);
                                                lbusPsEmployment.SetWssPersonEmploymentInfo(lbusWssMemberRecordRequest, lobjOrganization);
													//25926	FILLER: Remove PS Error Report Error of "Employment exists, but Empl Record Number is Different"
                                                //if (lbusPersomEmp.icdoPersonEmployment.ps_empl_record_number == null)
                                                //{
                                                //    if (lbusPsEmployment.icdoPsEmployment.empl_start_date == lbusPersomEmp.icdoPersonEmployment.start_date)
                                                //    {
                                                //        if (lbusPsEmployment.icdoPsEmployment.job_class_value == lbusPersomEmp.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.job_class_value
                                                //        && lbusPsEmployment.icdoPsEmployment.job_type_value == lbusPersomEmp.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.type_value
                                                //        && lbusPsEmployment.icdoPsEmployment.empl_status_value == lbusPersomEmp.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.status_value
                                                //        )
                                                //        {
                                                //            utlError lobjError = null;
                                                //            lobjError = AddError(10214, "Employment exists, but Empl Record Number is different.");
                                                //            lbusPsEmployment.iarrErrors.Add(lobjError);
                                                //            iclbUnprocessedPsEmployment.Add(lbusPsEmployment);
                                                //            iclbPersonEmploymentInformativeErrors.Add(lbusPsEmployment);
                                                //            if (!lbusPsEmployment.idictPsEmploymentError.ContainsKey(lbusPsEmployment.icdoPsEmployment.ps_employment_id))
                                                //                lbusPsEmployment.idictPsEmploymentError.Add(lbusPsEmployment.icdoPsEmployment.ps_employment_id, lbusPsEmployment.iarrErrors);
                                                //            continue;
                                                //        }
                                                //    }
                                                //}
                                                ValidatePSRecords(lbusWssMemberRecordRequest, lbusPsEmployment, utlPageMode.New, iclbPersonEmploymentInformativeErrors);
                                            }
                                            else
                                            {
                                                utlError lobjError = null;
                                                lobjError = AddError(10212, "Person does not have active address.");
                                                lbusPsEmployment.iarrErrors.Add(lobjError);
                                                iclbUnprocessedPsEmployment.Add(lbusPsEmployment);
                                                //iclbPersonEmploymentInformativeErrors.Add(lbusPsEmployment);
                                                if (!lbusPsEmployment.idictPsEmploymentError.ContainsKey(lbusPsEmployment.icdoPsEmployment.ps_employment_id))
                                                    lbusPsEmployment.idictPsEmploymentError.Add(lbusPsEmployment.icdoPsEmployment.ps_employment_id, lbusPsEmployment.iarrErrors);
                                                continue;
                                            }
                                        }
                                        else
                                        {
                                            if (lbusPsEmployment.icdoPsEmployment.loa_date_of_return != DateTime.MinValue
                                                && !(lbusPersomEmp.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.status_value == busConstant.EmploymentStatusLOA
                                                || lbusPersomEmp.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.status_value == busConstant.EmploymentStatusLOAM
                                                || lbusPersomEmp.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.status_value == busConstant.EmploymentStatusFMLA))
                                            {
                                                iclbUnprocessedPsEmployment.Add(lbusPsEmployment);
                                                if (lbusPsEmployment.GetEmploymentChangeRequestInReviewLOA())
                                                {
                                                    utlError lobjError = null;
                                                    lobjError = AddError(10222, "LOA employment doesn't exist.");
                                                    lbusPsEmployment.iarrErrors.Add(lobjError);
                                                    iclbPersonEmploymentInformativeErrors.Add(lbusPsEmployment);
                                                    if (!lbusPsEmployment.idictPsEmploymentError.ContainsKey(lbusPsEmployment.icdoPsEmployment.ps_employment_id))
                                                        lbusPsEmployment.idictPsEmploymentError.Add(lbusPsEmployment.icdoPsEmployment.ps_employment_id, lbusPsEmployment.iarrErrors);
                                                }
                                                continue;
                                            }
                                            lbusWssEmploymentChangeRequest = new busWssEmploymentChangeRequest { icdoWssEmploymentChangeRequest = new cdoWssEmploymentChangeRequest() };
                                            CreateEmploymentChangeRequest(lbusWssEmploymentChangeRequest, lbusPsEmployment, lbusPersomEmp);
												//25926	FILLER: Remove PS Error Report Error of "Employment exists, but Empl Record Number is Different"
                                            //if (lbusPersomEmp.icdoPersonEmployment.ps_empl_record_number == null)
                                            //{
                                            //    if (lbusPsEmployment.icdoPsEmployment.empl_start_date == lbusPersomEmp.icdoPersonEmployment.start_date)
                                            //    {
                                            //        if (lbusPsEmployment.icdoPsEmployment.job_class_value == lbusPersomEmp.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.job_class_value
                                            //        && lbusPsEmployment.icdoPsEmployment.job_type_value == lbusPersomEmp.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.type_value
                                            //        && lbusPsEmployment.icdoPsEmployment.empl_status_value == lbusPersomEmp.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.status_value
                                            //        )
                                            //        {
                                            //            utlError lobjError = null;
                                            //            lobjError = AddError(10214, "Employment exists, but Empl Record Number is different.");
                                            //            lbusPsEmployment.iarrErrors.Add(lobjError);
                                            //            iclbUnprocessedPsEmployment.Add(lbusPsEmployment);
                                            //            iclbPersonEmploymentInformativeErrors.Add(lbusPsEmployment);
                                            //            if (!lbusPsEmployment.idictPsEmploymentError.ContainsKey(lbusPsEmployment.icdoPsEmployment.ps_employment_id))
                                            //                lbusPsEmployment.idictPsEmploymentError.Add(lbusPsEmployment.icdoPsEmployment.ps_employment_id, lbusPsEmployment.iarrErrors);
                                            //            continue;
                                            //        }
                                            //    }
                                            //}
                                            ValidatePSRecords(lbusWssEmploymentChangeRequest, lbusPsEmployment, utlPageMode.New);
                                        }
                                    }
                                }
                                else
                                {
                                    lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.org_id = lintOrgId;
                                    if (lbusPsEmployment.ibusPsPerson != null)
                                    {
                                        CreateMemberRecordRequest(lbusWssMemberRecordRequest, lbusPsEmployment.ibusPsPerson);
                                        SetWssPersonAddressInfo(lbusWssMemberRecordRequest); //Added on 11/27/2013
                                    }
                                    else if (lbusPsEmployment.ibusPerson != null)
                                    {
                                        CreateMemberRecordRequest(lbusWssMemberRecordRequest, lbusPsEmployment.ibusPerson);
                                    }
                                    else
                                    {
                                        utlError lobjError = null;
                                        lobjError = AddError(10208, "PS person Record does not exist.");
                                        lbusPsEmployment.iarrErrors.Add(lobjError);
                                        iclbProcessedPsEmployment.Add(lbusPsEmployment);
                                        if (!lbusPsEmployment.idictPsEmploymentError.ContainsKey(lbusPsEmployment.icdoPsEmployment.ps_employment_id))
                                            lbusPsEmployment.idictPsEmploymentError.Add(lbusPsEmployment.icdoPsEmployment.ps_employment_id, lbusPsEmployment.iarrErrors);
                                        continue;
                                    }
                                    //lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.org_id = lintOrgId; //Commented on 11/27/2013
                                    //CreateMemberRecordRequest(lbusWssMemberRecordRequest, lbusPsEmployment.ibusPsPerson);
                                    lbusPsEmployment.SetWssPersonEmploymentInfo(lbusWssMemberRecordRequest, lobjOrganization);
                                    ValidatePSRecords(lbusWssMemberRecordRequest, lbusPsEmployment, utlPageMode.New, iclbPersonEmploymentInformativeErrors);
                                }
                            }
                            #endregion Dual Case
                            else if (lclbPersonEmployment.Count() == 1)
                            {
                                foreach (busPersonEmployment lobjPersonEmployment in lclbPersonEmployment)
                                {
                                    lobjPersonEmployment.LoadLatestPersonEmploymentDetail();
                                    ///Same Record number Different ORG
                                    if (lintOrgId != lobjPersonEmployment.icdoPersonEmployment.org_id)
                                    {
                                        lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.org_id = lintOrgId;
                                        if (lbusPsEmployment.ibusPsPerson != null)
                                        {
                                            CreateMemberRecordRequest(lbusWssMemberRecordRequest, lbusPsEmployment.ibusPsPerson);
                                        }
                                        else if (lbusPsEmployment.ibusPerson != null)
                                        {
                                            CreateMemberRecordRequest(lbusWssMemberRecordRequest, lbusPsEmployment.ibusPerson);
                                        }
                                        else
                                        {
                                            utlError lobjError = null;
                                            lobjError = AddError(10208, "PS person Record does not exist.");
                                            lbusPsEmployment.iarrErrors.Add(lobjError);
                                            iclbProcessedPsEmployment.Add(lbusPsEmployment);
                                            if (!lbusPsEmployment.idictPsEmploymentError.ContainsKey(lbusPsEmployment.icdoPsEmployment.ps_employment_id))
                                                lbusPsEmployment.idictPsEmploymentError.Add(lbusPsEmployment.icdoPsEmployment.ps_employment_id, lbusPsEmployment.iarrErrors);
                                            continue;
                                        }
                                        lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.org_id = lintOrgId;
                                        //2 October Changes Start
                                        if (lbusPsEmployment.ibusPsAddress == null)
                                        {
                                            lbusPsEmployment.SetWssPersonAddressInfo(lbusWssMemberRecordRequest);
                                        }
                                        else
                                        {
                                            lbusPsEmployment.ibusPsAddress.SetWssPersonAddressInfo(lbusWssMemberRecordRequest);
                                            if (lbusPsEmployment.ibusPsAddress.iblnIsAddressNotValidated)
                                            {
                                                utlError lobjError = null;
                                                lobjError = AddError(130, "Person Address is invalid.");
                                                lbusPsEmployment.iarrErrors.Add(lobjError);
                                                iclbUnprocessedPsEmployment.Add(lbusPsEmployment);
                                                iclbPersonEmploymentInformativeErrors.Add(lbusPsEmployment);
                                                if (!lbusPsEmployment.idictPsEmploymentError.ContainsKey(lbusPsEmployment.icdoPsEmployment.ps_employment_id))
                                                    lbusPsEmployment.idictPsEmploymentError.Add(lbusPsEmployment.icdoPsEmployment.ps_employment_id, lbusPsEmployment.iarrErrors);
                                                continue;
                                            }
                                        }
                                        //2 October Changes End
                                        lbusPsEmployment.SetWssPersonEmploymentInfo(lbusWssMemberRecordRequest, lobjOrganization);
                                        ValidatePSRecords(lbusWssMemberRecordRequest, lbusPsEmployment, utlPageMode.New, iclbPersonEmploymentInformativeErrors);
                                    }
                                    else
                                    {   //Create Employment Change request  //Same Record Number Same Org  
                                        if (lbusPsEmployment.icdoPsEmployment.ps_empl_record_number == (lobjPersonEmployment.icdoPersonEmployment.ps_empl_record_number == null ? "0" : lobjPersonEmployment.icdoPersonEmployment.ps_empl_record_number))
                                        {
                                            if (lbusPsEmployment.icdoPsEmployment.loa_date_of_return != DateTime.MinValue
                                                && !(lobjPersonEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.status_value == busConstant.EmploymentStatusLOA
                                                || lobjPersonEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.status_value == busConstant.EmploymentStatusLOAM
                                                || lobjPersonEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.status_value == busConstant.EmploymentStatusFMLA))
                                            {
                                                iclbUnprocessedPsEmployment.Add(lbusPsEmployment);
                                                if (lbusPsEmployment.GetEmploymentChangeRequestInReviewLOA())
                                                {
                                                    utlError lobjError = null;
                                                    lobjError = AddError(10222, "LOA employment doesn't exist.");
                                                    lbusPsEmployment.iarrErrors.Add(lobjError);
                                                    iclbPersonEmploymentInformativeErrors.Add(lbusPsEmployment);
                                                    if (!lbusPsEmployment.idictPsEmploymentError.ContainsKey(lbusPsEmployment.icdoPsEmployment.ps_employment_id))
                                                        lbusPsEmployment.idictPsEmploymentError.Add(lbusPsEmployment.icdoPsEmployment.ps_employment_id, lbusPsEmployment.iarrErrors);
                                                }
                                                continue;
                                            }
                                            lbusWssEmploymentChangeRequest = new busWssEmploymentChangeRequest { icdoWssEmploymentChangeRequest = new cdoWssEmploymentChangeRequest() };
                                            CreateEmploymentChangeRequest(lbusWssEmploymentChangeRequest, lbusPsEmployment, lobjPersonEmployment);
                                            if (lbusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.change_type_value.IsNull())
                                            {
                                                if (lbusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.last_date_of_service == DateTime.MinValue)
                                                {
                                                    //utlError lobjError = null;
                                                    //lobjError = AddError(10210, "Employment Record already exists.");
                                                    if (!iclbProcessedPsEmployment.Contains(lbusPsEmployment))
                                                        iclbProcessedPsEmployment.Add(lbusPsEmployment);
                                                    continue;
                                                }
                                            }
                                            ValidatePSRecords(lbusWssEmploymentChangeRequest, lbusPsEmployment, utlPageMode.New);
                                        }
                                        else
                                        {
                                            //If Perlink does not have same record number / if it is Null and the employment record is sent for Employment termination then in that case display error in Error Report
                                            if (lobjPersonEmployment.icdoPersonEmployment.ps_empl_record_number != lbusPsEmployment.icdoPsEmployment.ps_empl_record_number && lbusPsEmployment.icdoPsEmployment.empl_end_date != DateTime.MinValue)
                                            {
                                                utlError lobjError = null;
                                                lobjError = AddError(10215, "Empl record number does not exists in Perslink.");
                                                lbusPsEmployment.iarrErrors.Add(lobjError);
                                                iclbUnprocessedPsEmployment.Add(lbusPsEmployment);
                                                iclbPersonEmploymentInformativeErrors.Add(lbusPsEmployment);
                                                if (!lbusPsEmployment.idictPsEmploymentError.ContainsKey(lbusPsEmployment.icdoPsEmployment.ps_employment_id))
                                                    lbusPsEmployment.idictPsEmploymentError.Add(lbusPsEmployment.icdoPsEmployment.ps_employment_id, lbusPsEmployment.iarrErrors);
                                                continue;
                                            }
                                            if (lbusPsEmployment.ibusPsPerson != null && lbusPsEmployment.ibusPsAddress != null)
                                            {
                                                //Same Organization Different Record Number 
                                                lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.org_id = lintOrgId;
                                                CreateMemberRecordRequest(lbusWssMemberRecordRequest, lbusPsEmployment.ibusPsPerson);
                                                //2 October Changes Start
                                                if (lbusPsEmployment.ibusPsAddress == null)
                                                {
                                                    lbusPsEmployment.SetWssPersonAddressInfo(lbusWssMemberRecordRequest);
                                                }
                                                else
                                                {
                                                    lbusPsEmployment.ibusPsAddress.SetWssPersonAddressInfo(lbusWssMemberRecordRequest);
                                                    if (lbusPsEmployment.ibusPsAddress.iblnIsAddressNotValidated)
                                                    {
                                                        utlError lobjError = null;
                                                        lobjError = AddError(130, "Person Address is invalid.");
                                                        lbusPsEmployment.iarrErrors.Add(lobjError);
                                                        iclbUnprocessedPsEmployment.Add(lbusPsEmployment);
                                                        iclbPersonEmploymentInformativeErrors.Add(lbusPsEmployment);
                                                        if (!lbusPsEmployment.idictPsEmploymentError.ContainsKey(lbusPsEmployment.icdoPsEmployment.ps_employment_id))
                                                            lbusPsEmployment.idictPsEmploymentError.Add(lbusPsEmployment.icdoPsEmployment.ps_employment_id, lbusPsEmployment.iarrErrors);
                                                        continue;
                                                    }
                                                }
                                                //2 October Changes End
                                                lbusPsEmployment.SetWssPersonEmploymentInfo(lbusWssMemberRecordRequest, lobjOrganization);
													//25926	FILLER: Remove PS Error Report Error of "Employment exists, but Empl Record Number is Different"
                                                //if (lobjPersonEmployment.icdoPersonEmployment.ps_empl_record_number == null)
                                                //{
                                                //    if (lbusPsEmployment.icdoPsEmployment.empl_start_date == lobjPersonEmployment.icdoPersonEmployment.start_date)
                                                //    {
                                                //        if (lbusPsEmployment.icdoPsEmployment.job_class_value == lobjPersonEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.job_class_value
                                                //        && lbusPsEmployment.icdoPsEmployment.job_type_value == lobjPersonEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.type_value
                                                //        && lbusPsEmployment.icdoPsEmployment.empl_status_value == lobjPersonEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.status_value
                                                //        )
                                                //        {
                                                //            utlError lobjError = null;
                                                //            lobjError = AddError(10214, "Employment exists, but Empl Record Number is different.");
                                                //            lbusPsEmployment.iarrErrors.Add(lobjError);
                                                //            iclbUnprocessedPsEmployment.Add(lbusPsEmployment);
                                                //            iclbPersonEmploymentInformativeErrors.Add(lbusPsEmployment);
                                                //            if (!lbusPsEmployment.idictPsEmploymentError.ContainsKey(lbusPsEmployment.icdoPsEmployment.ps_employment_id))
                                                //                lbusPsEmployment.idictPsEmploymentError.Add(lbusPsEmployment.icdoPsEmployment.ps_employment_id, lbusPsEmployment.iarrErrors);
                                                //            continue;
                                                //        }
                                                //    }
                                                //}
                                                ValidatePSRecords(lbusWssMemberRecordRequest, lbusPsEmployment, utlPageMode.New, iclbPersonEmploymentInformativeErrors);
                                            }
                                            else if (lbusPsEmployment.ibusPerson != null)
                                            {
                                                CreateMemberRecordRequest(lbusWssMemberRecordRequest, lbusPsEmployment.ibusPerson);
                                                lbusPsEmployment.ibusPerson.LoadPersonCurrentAddress();
                                                if (lbusPsEmployment.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.person_address_id > 0)
                                                {
                                                    lbusPsEmployment.SetWssPersonAddressInfo(lbusWssMemberRecordRequest);
                                                    lbusPsEmployment.SetWssPersonEmploymentInfo(lbusWssMemberRecordRequest, lobjOrganization);
														//25926	FILLER: Remove PS Error Report Error of "Employment exists, but Empl Record Number is Different"
                                                    //if (lobjPersonEmployment.icdoPersonEmployment.ps_empl_record_number == null)
                                                    //{
                                                    //    if (lbusPsEmployment.icdoPsEmployment.empl_start_date == lobjPersonEmployment.icdoPersonEmployment.start_date)
                                                    //    {
                                                    //        if (lbusPsEmployment.icdoPsEmployment.job_class_value == lobjPersonEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.job_class_value
                                                    //        && lbusPsEmployment.icdoPsEmployment.job_type_value == lobjPersonEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.type_value
                                                    //        && lbusPsEmployment.icdoPsEmployment.empl_status_value == lobjPersonEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.status_value
                                                    //        )
                                                    //        {
                                                    //            utlError lobjError = null;
                                                    //            lobjError = AddError(10214, "Employment exists, but Empl Record Number is different.");
                                                    //            lbusPsEmployment.iarrErrors.Add(lobjError);
                                                    //            iclbUnprocessedPsEmployment.Add(lbusPsEmployment);
                                                    //            iclbPersonEmploymentInformativeErrors.Add(lbusPsEmployment);
                                                    //            if (!lbusPsEmployment.idictPsEmploymentError.ContainsKey(lbusPsEmployment.icdoPsEmployment.ps_employment_id))
                                                    //                lbusPsEmployment.idictPsEmploymentError.Add(lbusPsEmployment.icdoPsEmployment.ps_employment_id, lbusPsEmployment.iarrErrors);
                                                    //            continue;
                                                    //        }
                                                    //    }
                                                    //}
                                                    ValidatePSRecords(lbusWssMemberRecordRequest, lbusPsEmployment, utlPageMode.New, iclbPersonEmploymentInformativeErrors);
                                                }
                                                else
                                                {
                                                    utlError lobjError = null;
                                                    lobjError = AddError(10212, "Person does not have active address.");
                                                    lbusPsEmployment.iarrErrors.Add(lobjError);
                                                    iclbUnprocessedPsEmployment.Add(lbusPsEmployment);
                                                    //iclbPersonEmploymentInformativeErrors.Add(lbusPsEmployment);
                                                    if (!lbusPsEmployment.idictPsEmploymentError.ContainsKey(lbusPsEmployment.icdoPsEmployment.ps_employment_id))
                                                        lbusPsEmployment.idictPsEmploymentError.Add(lbusPsEmployment.icdoPsEmployment.ps_employment_id, lbusPsEmployment.iarrErrors);
                                                    continue;
                                                }
                                            }
                                            else //Need to Verify On 7th October 
                                            {
                                                if (lbusPsEmployment.icdoPsEmployment.loa_date_of_return != DateTime.MinValue
                                                    && !(lobjPersonEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.status_value == busConstant.EmploymentStatusLOA
                                                    || lobjPersonEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.status_value == busConstant.EmploymentStatusLOAM
                                                    || lobjPersonEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.status_value == busConstant.EmploymentStatusFMLA))
                                                {
                                                    iclbUnprocessedPsEmployment.Add(lbusPsEmployment);
                                                    if (lbusPsEmployment.GetEmploymentChangeRequestInReviewLOA())
                                                    {
                                                        utlError lobjError = null;
                                                        lobjError = AddError(10222, "LOA employment doesn't exist.");
                                                        lbusPsEmployment.iarrErrors.Add(lobjError);
                                                        iclbPersonEmploymentInformativeErrors.Add(lbusPsEmployment);
                                                        if (!lbusPsEmployment.idictPsEmploymentError.ContainsKey(lbusPsEmployment.icdoPsEmployment.ps_employment_id))
                                                            lbusPsEmployment.idictPsEmploymentError.Add(lbusPsEmployment.icdoPsEmployment.ps_employment_id, lbusPsEmployment.iarrErrors);
                                                    }
                                                    continue;
                                                }
                                                lbusWssEmploymentChangeRequest = new busWssEmploymentChangeRequest { icdoWssEmploymentChangeRequest = new cdoWssEmploymentChangeRequest() };
                                                CreateEmploymentChangeRequest(lbusWssEmploymentChangeRequest, lbusPsEmployment, lobjPersonEmployment);
													//25926	FILLER: Remove PS Error Report Error of "Employment exists, but Empl Record Number is Different"
                                                //if (lobjPersonEmployment.icdoPersonEmployment.ps_empl_record_number == null)
                                                //{
                                                //    if (lbusPsEmployment.icdoPsEmployment.empl_start_date == lobjPersonEmployment.icdoPersonEmployment.start_date)
                                                //    {
                                                //        if (lbusPsEmployment.icdoPsEmployment.job_class_value == lobjPersonEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.job_class_value
                                                //        && lbusPsEmployment.icdoPsEmployment.job_type_value == lobjPersonEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.type_value
                                                //        && lbusPsEmployment.icdoPsEmployment.empl_status_value == lobjPersonEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.status_value
                                                //        )
                                                //        {
                                                //            utlError lobjError = null;
                                                //            lobjError = AddError(10214, "Employment exists, but Empl Record Number is different.");
                                                //            lbusPsEmployment.iarrErrors.Add(lobjError);
                                                //            iclbUnprocessedPsEmployment.Add(lbusPsEmployment);
                                                //            iclbPersonEmploymentInformativeErrors.Add(lbusPsEmployment);
                                                //            if (!lbusPsEmployment.idictPsEmploymentError.ContainsKey(lbusPsEmployment.icdoPsEmployment.ps_employment_id))
                                                //                lbusPsEmployment.idictPsEmploymentError.Add(lbusPsEmployment.icdoPsEmployment.ps_employment_id, lbusPsEmployment.iarrErrors);
                                                //            continue;
                                                //        }
                                                //    }
                                                //}
                                                ValidatePSRecords(lbusWssEmploymentChangeRequest, lbusPsEmployment, utlPageMode.New);
                                            }
                                        }
                                    }
                                }
                            }
                            else if (lclbPersonEmployment.Count() == 0)
                            {
                                // As per mail from Maik on 03/07/2014
                                // 1. If PS_EMPLOYMENT record is sent without person or address record and 2. Person already exists in PERSLink because they are a dependent/beneficiary to someone
                                // and 3. No Address exists in PERSLink yet, then error should be reported in error report
                                if (lbusPsEmployment.ibusPsPerson.IsNull() && lbusPsEmployment.ibusPsAddress.IsNull())
                                {
                                    if (lbusPsEmployment.ibusPerson != null)
                                    {
                                        lbusPsEmployment.ibusPerson.LoadPersonCurrentAddress();
                                        if (lbusPsEmployment.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.person_address_id == 0)
                                        {
                                            utlError lobjError = null;
                                            lobjError = AddError(10212, "Person does not have active address.");
                                            lbusPsEmployment.iarrErrors.Add(lobjError);
                                            iclbUnprocessedPsEmployment.Add(lbusPsEmployment);
                                            if (!lbusPsEmployment.idictPsEmploymentError.ContainsKey(lbusPsEmployment.icdoPsEmployment.ps_employment_id))
                                                lbusPsEmployment.idictPsEmploymentError.Add(lbusPsEmployment.icdoPsEmployment.ps_employment_id, lbusPsEmployment.iarrErrors);
                                            continue;
                                        }
                                    }
                                }


                                if (lbusPsEmployment.icdoPsEmployment.empl_end_date != DateTime.MinValue)
                                {
                                    utlError lobjError = null;
                                    lobjError = AddError(10211, "Terminated Employment already exists.");
                                    lbusPsEmployment.iarrErrors.Add(lobjError);
                                    iclbUnprocessedPsEmployment.Add(lbusPsEmployment);
                                    iclbPersonEmploymentInformativeErrors.Add(lbusPsEmployment);
                                    if (!lbusPsEmployment.idictPsEmploymentError.ContainsKey(lbusPsEmployment.icdoPsEmployment.ps_employment_id))
                                        lbusPsEmployment.idictPsEmploymentError.Add(lbusPsEmployment.icdoPsEmployment.ps_employment_id, lbusPsEmployment.iarrErrors);
                                    continue;
                                }

                                lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.org_id = lintOrgId;
                                if (lbusPsEmployment.ibusPsPerson != null)
                                {
                                    CreateMemberRecordRequest(lbusWssMemberRecordRequest, lbusPsEmployment.ibusPsPerson);
                                }
                                else if (lbusPsEmployment.ibusPerson != null)
                                {
                                    CreateMemberRecordRequest(lbusWssMemberRecordRequest, lbusPsEmployment.ibusPerson);
                                }
                                else
                                {
                                    utlError lobjError = null;
                                    lobjError = AddError(10208, "PS person Record does not exist.");
                                    lbusPsEmployment.iarrErrors.Add(lobjError);
                                    iclbProcessedPsEmployment.Add(lbusPsEmployment);
                                    if (!lbusPsEmployment.idictPsEmploymentError.ContainsKey(lbusPsEmployment.icdoPsEmployment.ps_employment_id))
                                        lbusPsEmployment.idictPsEmploymentError.Add(lbusPsEmployment.icdoPsEmployment.ps_employment_id, lbusPsEmployment.iarrErrors);
                                    continue;
                                }
                                lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.org_id = lintOrgId;
                                lbusPsEmployment.SetWssPersonEmploymentInfo(lbusWssMemberRecordRequest, lobjOrganization);
                                ValidatePSRecords(lbusWssMemberRecordRequest, lbusPsEmployment, utlPageMode.New, iclbPersonEmploymentInformativeErrors);
                            }
                        }
                    }
                }

                //}
                //catch (Exception e)
                //{
                //    DBFunction.StoreProcessLog(100, "An error occured while updating person employment Id: " +
                //                                            lbusPsEmployment.icdoPsEmployment.ps_employment_id + " Index : " + iclbPsPersonEmployment.IndexOf(lbusPsEmployment) + " Error Message : " + e,
                //                                            "ERR", "Error in finalize file", istrUserId, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                //}

            }
        }
        //2 October Changes Start
        private void SetWssPersonAddressInfo(busWssMemberRecordRequest lbusWssMemberRecordRequest)
        {
            ibusPerson.LoadPersonCurrentAddress();
            if (ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.person_address_id > 0)
            {
                lbusWssMemberRecordRequest.icdoWssPersonAddress = new cdoWssPersonAddress();
                lbusWssMemberRecordRequest.icdoWssPersonAddress.addr_line_1 = ibusPerson.ibusPersonCurrentAddress.icdoPersonCurrentAddress.addr_line_1;
                lbusWssMemberRecordRequest.icdoWssPersonAddress.addr_line_2 = ibusPerson.ibusPersonCurrentAddress.icdoPersonCurrentAddress.addr_line_2;
                lbusWssMemberRecordRequest.icdoWssPersonAddress.addr_city = ibusPerson.ibusPersonCurrentAddress.icdoPersonCurrentAddress.addr_city;
                lbusWssMemberRecordRequest.icdoWssPersonAddress.addr_state_value = ibusPerson.ibusPersonCurrentAddress.icdoPersonCurrentAddress.addr_state_value;
                lbusWssMemberRecordRequest.icdoWssPersonAddress.addr_country_value = ibusPerson.ibusPersonCurrentAddress.icdoPersonCurrentAddress.addr_country_value;
                lbusWssMemberRecordRequest.icdoWssPersonAddress.addr_zip_code = ibusPerson.ibusPersonCurrentAddress.icdoPersonCurrentAddress.addr_zip_code;
                lbusWssMemberRecordRequest.icdoWssPersonAddress.addr_zip_4_code = ibusPerson.ibusPersonCurrentAddress.icdoPersonCurrentAddress.addr_zip_4_code;
                lbusWssMemberRecordRequest.icdoWssPersonAddress.addr_effective_date = ibusPerson.ibusPersonCurrentAddress.icdoPersonCurrentAddress.start_date;
                //if (iblnAddressChanged) // Check if Address Updated in this batch 
                //lbusWssMemberRecordRequest.icdoWssPersonAddress.address_updated_in_ps_batch = busConstant.Flag_Yes;
            }
            
        }
        //2 October Changes Start

        



        private busPerson LoadPersonBySSN(string astrssn)
        {
            Collection<busPerson> lclbPerson = new Collection<busPerson>();
            DataTable ldtbPerson = DBFunction.DBSelect("cdoPerson.GetPersonBySSN", new string[1] { astrssn },
                iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            if (ldtbPerson.Rows.Count > 0)
            {
                lclbPerson = GetCollection<busPerson>(ldtbPerson, "icdoPerson");
                return lclbPerson[0];
            }
            else
                return null;
        }



        private void CreateMemberRecordRequest(busWssMemberRecordRequest lbusWssMemberRecordRequest, busPsPerson lbusPsPerson)
        {
            lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.name_prefix_value = lbusPsPerson.icdoPsPerson.name_prefix_value;
            lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.first_name = lbusPsPerson.icdoPsPerson.first_name;
            lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.middle_name = lbusPsPerson.icdoPsPerson.middle_name;
            lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.last_name = lbusPsPerson.icdoPsPerson.last_name;
            lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.name_suffix_value = lbusPsPerson.icdoPsPerson.name_suffix_value;
            lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.date_of_birth = lbusPsPerson.icdoPsPerson.date_of_birth;
            lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.gender_value = lbusPsPerson.icdoPsPerson.gender_value;
            lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.ssn = lbusPsPerson.icdoPsPerson.ssn;
            lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.marital_status_value = lbusPsPerson.icdoPsPerson.marital_status_value;
            lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.marital_status_date = lbusPsPerson.icdoPsPerson.ms_change_date; //Need to refresh cdo 
            lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.work_phone_no = lbusPsPerson.icdoPsPerson.work_phone_no;
            lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.work_phone_ext = lbusPsPerson.icdoPsPerson.work_phone_ext;//Need to  refresh cdo
            lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.cell_phone_no = lbusPsPerson.icdoPsPerson.cell_phone_no;
            lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.home_phone_no = lbusPsPerson.icdoPsPerson.home_phone_no;
            lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.peoplesoft_id = Convert.ToInt32(lbusPsPerson.icdoPsPerson.peoplesoft_id);
            lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.created_by = "PERSLinkBatch";
            lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.created_date = DateTime.Now;
            lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.modified_by = "PERSLinkBatch";
            lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.modified_date = DateTime.Now;
            lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.ps_initiated_flag = busConstant.Flag_Yes;
            lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.status_value = busConstant.EmploymentChangeRequestStatusPendingAutoPosting;
        }

        private void CreateEmploymentChangeRequest(busWssEmploymentChangeRequest abusWssEmploymentChangeRequest, busPsEmployment lbusPsEmployment, busPersonEmployment lobjPersonEmployment)
        {
            lobjPersonEmployment.LoadLatestPersonEmploymentDetail();
            abusWssEmploymentChangeRequest.ibusPersonEmployment = lobjPersonEmployment;
            abusWssEmploymentChangeRequest.ibusPersonEmploymentDetail = lobjPersonEmployment.ibusLatestEmploymentDetail;
            abusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest = new cdoWssEmploymentChangeRequest();
            abusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.ienuObjectState = ObjectState.Insert;
            abusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.ps_empl_record_number = lbusPsEmployment.icdoPsEmployment.ps_empl_record_number;
            abusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.peoplesoft_id = lbusPsEmployment.icdoPsEmployment.peoplesoft_id;
            //Need to confirm to which filed we have to bind in sgt_wss_employment_change_request 
            if(lbusPsEmployment.icdoPsEmployment.job_class_value == busConstant.PersonJobClassStateElectedOfficial)
                abusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.term_begin_date = lbusPsEmployment.icdoPsEmployment.term_begin_date;
            abusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.person_id = lbusPsEmployment.ibusPerson.icdoPerson.person_id;
            abusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.org_id = lobjPersonEmployment.icdoPersonEmployment.org_id;

            //*****************Need to confirm with Maik on what basis we shall assign the change_type_value 
            //*****************also need the confirmation to add job_class_change_effective_date and employment_type_change_effective_date

            if (lbusPsEmployment.icdoPsEmployment.empl_end_date == DateTime.MinValue)
            {
                //if (lbusPsEmployment.icdoPsEmployment.job_class_value.IsNotNull() && lbusPsEmployment.icdoPsEmployment.term_begin_date != DateTime.MinValue && lbusPsEmployment.icdoPsEmployment.job_class_change_effective_date != DateTime.MinValue)
                if (lbusPsEmployment.icdoPsEmployment.job_class_value != null && lbusPsEmployment.icdoPsEmployment.job_class_value.Trim().IsNotNullOrEmpty() && lbusPsEmployment.icdoPsEmployment.job_class_value != lobjPersonEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.job_class_value
                    //&& lbusPsEmployment.icdoPsEmployment.term_begin_date != DateTime.MinValue 
                    && lbusPsEmployment.icdoPsEmployment.empl_start_date != DateTime.MinValue)
                {
                    abusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.change_type_value = busConstant.EmploymentChangeRequestChangeTypeClassification;
                    abusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.job_class_change_effective_date = lbusPsEmployment.icdoPsEmployment.empl_start_date;
                    abusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.employment_type_change_effective_date = lbusPsEmployment.icdoPsEmployment.empl_start_date;
                    iblnEmploymentChangeRequestChangeTypeClassification = true;

                }
                //else if (lbusPsEmployment.icdoPsEmployment.job_type_value.IsNotNull() && lbusPsEmployment.icdoPsEmployment.empl_status_value.IsNotNull() && lbusPsEmployment.icdoPsEmployment.employment_type_change_effective_date != DateTime.MinValue)
                if (((lbusPsEmployment.icdoPsEmployment.job_type_value != null && lbusPsEmployment.icdoPsEmployment.job_type_value.Trim().IsNotNullOrEmpty() && lbusPsEmployment.icdoPsEmployment.job_type_value != lobjPersonEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.type_value)
                    && (lbusPsEmployment.icdoPsEmployment.empl_status_value != null && lbusPsEmployment.icdoPsEmployment.empl_status_value.Trim().IsNotNullOrEmpty() && lbusPsEmployment.icdoPsEmployment.empl_start_date != DateTime.MinValue))
                    || (lobjPersonEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.hourly_value != lbusPsEmployment.icdoPsEmployment.hourly_value
                    || lobjPersonEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.status_value != lbusPsEmployment.icdoPsEmployment.empl_status_value))
                {
                    abusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.change_type_value = busConstant.EmploymentChangeRequestChangeTypeEmployment;
                    abusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.employment_type_change_effective_date = lbusPsEmployment.icdoPsEmployment.empl_start_date;
                    iblnEmploymentChangeRequestChangeTypeEmployment = true;
                }
                if (lbusPsEmployment.icdoPsEmployment.loa_start_date != DateTime.MinValue && lbusPsEmployment.icdoPsEmployment.empl_status_value == busConstant.EmploymentStatusLOAM
                    || (lbusPsEmployment.icdoPsEmployment.loa_date_of_return != DateTime.MinValue && lobjPersonEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.status_value == busConstant.EmploymentStatusLOAM))
                {
                    abusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.change_type_value = busConstant.EmploymentChangeRequestChangeTypeLOAM;
                    iblnEmploymentChangeRequestChangeTypeLOAM = true;
                }
                if ((lbusPsEmployment.icdoPsEmployment.loa_start_date != DateTime.MinValue && lbusPsEmployment.icdoPsEmployment.empl_status_value == busConstant.EmploymentStatusLOA)
                    || (lbusPsEmployment.icdoPsEmployment.loa_date_of_return != DateTime.MinValue && lobjPersonEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.status_value == busConstant.EmploymentStatusLOA))
                {
                    abusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.change_type_value = busConstant.EmploymentChangeRequestChangeTypeLOA;
                    iblnEmploymentChangeRequestChangeTypeLOA = true;
                }
                //FMLA
                if ((lbusPsEmployment.icdoPsEmployment.loa_start_date != DateTime.MinValue && lbusPsEmployment.icdoPsEmployment.empl_status_value == busConstant.EmploymentStatusFMLA)
                   || (lbusPsEmployment.icdoPsEmployment.loa_date_of_return != DateTime.MinValue && lobjPersonEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.status_value == busConstant.EmploymentStatusFMLA))
                {
                    abusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.change_type_value = busConstant.EmploymentStatusFMLA;
                    iblnEmploymentChangeRequestChangeTypeFMLA = true;
                }
            }

            abusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.hourly_value = lbusPsEmployment.icdoPsEmployment.hourly_value; // PIR 12578
            abusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.type_value = lbusPsEmployment.icdoPsEmployment.job_type_value;
            abusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.job_class_value = lbusPsEmployment.icdoPsEmployment.job_class_value;
            abusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.official_list_value = lbusPsEmployment.icdoPsEmployment.official_list_value;
            abusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.employment_end_date = lbusPsEmployment.icdoPsEmployment.empl_end_date;
            abusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.last_date_of_service = lbusPsEmployment.icdoPsEmployment.empl_end_date;
            abusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.employment_status_value = lbusPsEmployment.icdoPsEmployment.empl_status_value;
            abusWssEmploymentChangeRequest.istrIsTermsAndConditionsAgreed = busConstant.Flag_Yes;

            //*********************************************************************************************************************************************************************//

            abusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.loa_start_date = lbusPsEmployment.icdoPsEmployment.loa_start_date;
            abusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.date_of_return = lbusPsEmployment.icdoPsEmployment.loa_date_of_return;
            abusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.date_of_last_regular_paycheck = lbusPsEmployment.icdoPsEmployment.date_of_last_regular_paycheck;
            abusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.last_month_on_employer_billing = lbusPsEmployment.icdoPsEmployment.last_month_on_employer_billing;
            abusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.last_retirement_transmittal_of_deduction = lbusPsEmployment.icdoPsEmployment.last_retirement_transmittal_of_deduction;
            if (lbusPsEmployment.icdoPsEmployment.job_class_value == busConstant.PersonJobClassStateElectedOfficial)
                abusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.term_begin_date = lbusPsEmployment.icdoPsEmployment.term_begin_date; //As Discussed with Karthik.
            abusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.created_by = "PERSLinkBatch";
            abusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.created_date = DateTime.Now;
            abusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.modified_by = "PERSLinkBatch";
            abusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.modified_date = DateTime.Now;
            abusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.status_value = busConstant.StatusReview;
        }

        private void CreateMemberRecordRequest(busWssMemberRecordRequest lbusWssMemberRecordRequest, busPerson lbusPerson)
        {
            lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.name_prefix_value = lbusPerson.icdoPerson.name_prefix_value;
            lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.first_name = lbusPerson.icdoPerson.first_name;
            lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.middle_name = lbusPerson.icdoPerson.middle_name;
            lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.last_name = lbusPerson.icdoPerson.last_name;
            lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.name_suffix_value = lbusPerson.icdoPerson.name_suffix_value;
            lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.date_of_birth = lbusPerson.icdoPerson.date_of_birth;
            lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.gender_value = lbusPerson.icdoPerson.gender_value;
            lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.ssn = lbusPerson.icdoPerson.ssn;
            lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.marital_status_value = lbusPerson.icdoPerson.marital_status_value;
            lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.marital_status_date = lbusPerson.icdoPerson.ms_change_date; //Need to refresh cdo 
            lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.work_phone_no = lbusPerson.icdoPerson.work_phone_no;
            lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.work_phone_ext = lbusPerson.icdoPerson.work_phone_ext;//Need to  refresh cdo
            lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.cell_phone_no = lbusPerson.icdoPerson.cell_phone_no;
            lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.home_phone_no = lbusPerson.icdoPerson.home_phone_no;
            lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.peoplesoft_id = iintPeoplesoftId;
            lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.ps_initiated_flag = busConstant.Flag_Yes;
            lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.status_value = busConstant.EmploymentChangeRequestStatusPendingAutoPosting;
            lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.created_by = "PERSLinkBatch";
            lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.created_date = DateTime.Now;
            lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.modified_by = "PERSLinkBatch";
            lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.modified_date = DateTime.Now;
            lbusPerson.LoadAddresses();
            if (lbusPerson.icolPersonAddress.Count > 0)
            {
                busPersonAddress lbusPersonAddress = lbusPerson.icolPersonAddress.OrderByDescending(obj => obj.icdoPersonAddress.start_date).FirstOrDefault(); //Added on 11/27/2013
                
                if (lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.member_record_request_id == 0)
                {
                    lbusWssMemberRecordRequest.icdoWssPersonAddress = new cdoWssPersonAddress();
                }
                lbusWssMemberRecordRequest.icdoWssPersonAddress.addr_line_1 = lbusPersonAddress.icdoPersonAddress.addr_line_1;
                lbusWssMemberRecordRequest.icdoWssPersonAddress.addr_line_2 = lbusPersonAddress.icdoPersonAddress.addr_line_2;
                lbusWssMemberRecordRequest.icdoWssPersonAddress.addr_city = lbusPersonAddress.icdoPersonAddress.addr_city;
                lbusWssMemberRecordRequest.icdoWssPersonAddress.addr_state_value = lbusPersonAddress.icdoPersonAddress.addr_state_value;
                lbusWssMemberRecordRequest.icdoWssPersonAddress.addr_country_value = lbusPersonAddress.icdoPersonAddress.addr_country_value;
                lbusWssMemberRecordRequest.icdoWssPersonAddress.addr_zip_code = lbusPersonAddress.icdoPersonAddress.addr_zip_code;
                lbusWssMemberRecordRequest.icdoWssPersonAddress.addr_zip_4_code = lbusPersonAddress.icdoPersonAddress.addr_zip_4_code;
                lbusWssMemberRecordRequest.icdoWssPersonAddress.addr_effective_date = DateTime.Now;
                //lbusWssMemberRecordRequest.icdoWssPersonAddress.address_updated_in_ps_batch = busConstant.Flag_Yes; //Added on 11/28/2013
            }

        }

        private busPsPerson LoadPSPersonBySSN(string astrssn)
        {
            Collection<busPsPerson> lclbPsPerson = new Collection<busPsPerson>();
            DataTable ldtbPsPerson = DBFunction.DBSelect("cdoPsPerson.GetPsPersonBySSN", new string[1] { astrssn },
                iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            if (ldtbPsPerson.Rows.Count > 0)
            {
                lclbPsPerson = GetCollection<busPsPerson>(ldtbPsPerson, "icdoPsPerson");
                return lclbPsPerson[0];
            }
            else
                return null;
        }
        /// <summary>
        /// Validate the Member Record Request for the Ps records 
        /// </summary>
        /// <param name="aobjWssMemberRecordRequest"></param>
        /// <param name="aobjPsEmployment"></param>
        /// <param name="mode"></param>
        private void ValidatePSRecords(busWssMemberRecordRequest aobjWssMemberRecordRequest, busPsEmployment aobjPsEmployment, utlPageMode mode,Collection <busPsEmployment> iclbPersonEmploymentInformativeErrors)
        {
            aobjWssMemberRecordRequest.iblnIsFromPS = true;
            aobjWssMemberRecordRequest.BeforeValidate(mode);
            aobjWssMemberRecordRequest.ValidateHardErrors(mode);
            if (aobjWssMemberRecordRequest.iarrErrors.Count == 1 && aobjWssMemberRecordRequest.iblnIsMemberContributing)
            {
                if (!aobjPsEmployment.idictPsEmploymentError.ContainsKey(aobjPsEmployment.icdoPsEmployment.ps_employment_id))
                    aobjPsEmployment.idictPsEmploymentError.Add(aobjPsEmployment.icdoPsEmployment.ps_employment_id, aobjWssMemberRecordRequest.iarrErrors);
                iclbUnprocessedPsEmployment.Add(aobjPsEmployment);
                iclbPersonEmploymentInformativeErrors.Add(aobjPsEmployment);
            }
            else
            {
                if (aobjWssMemberRecordRequest.iarrErrors.Count > 0)
                {
                    if (!iclbUnprocessedPsEmployment.Contains(aobjPsEmployment))
                    {
                        iclbUnprocessedPsEmployment.Add(aobjPsEmployment);
                        if (!aobjPsEmployment.idictPsEmploymentError.ContainsKey(aobjPsEmployment.icdoPsEmployment.ps_employment_id))
                            aobjPsEmployment.idictPsEmploymentError.Add(aobjPsEmployment.icdoPsEmployment.ps_employment_id, aobjWssMemberRecordRequest.iarrErrors);
                    }
                }
                else
                {
                    DataTable ltbMemberRecordRequest = Select("cdoPsEmployment.GetMemberRecordRequestBySSN", new object[1] { aobjPsEmployment.icdoPsEmployment.ssn });
                    if (ltbMemberRecordRequest.Rows.Count == 0)
                    {
                        aobjPsEmployment.icdoPsEmployment.processed_flag = busConstant.Flag_Yes;
                        aobjWssMemberRecordRequest.BeforePersistChanges();
                        aobjWssMemberRecordRequest.PersistChanges();
                        aobjWssMemberRecordRequest.ValidateSoftErrors();
                        aobjWssMemberRecordRequest.AfterPersistChanges();
                        if (!iclbProcessedPsEmployment.Contains(aobjPsEmployment))
                        {
                            iclbProcessedPsEmployment.Add(aobjPsEmployment);
                        }
                        //iclbProcessedPsPerson.ReplaceIfExists(lbusPsPerson);
                    }
                }
            }


        }
        /// <summary>
        /// Validate the Employment Change Request for the Ps records 
        /// </summary>
        /// <param name="aobjWssEmploymentChangeRequest"></param>
        /// <param name="aobjPsEmployment"></param>
        /// <param name="mode"></param>
        private void ValidatePSRecords(busWssEmploymentChangeRequest aobjWssEmploymentChangeRequest, busPsEmployment aobjPsEmployment, utlPageMode mode)
        {
            if (mode == utlPageMode.New)
                aobjWssEmploymentChangeRequest.iblnIsNewMode = true;
            aobjWssEmploymentChangeRequest.iblnisFromPs = true;
            aobjWssEmploymentChangeRequest.BeforeValidate(mode);
            //aobjWssEmploymentChangeRequest.ValidateHardErrors(mode);
            // This code code will evaluate the group rules according to the change type value as they are triggered in the wizard for respective wizard steps
            //PIR 12578 - Boolean values checked as there can be more than one change which need to be reflected.
            if (iblnEmploymentChangeRequestChangeTypeClassification)
            {
                aobjWssEmploymentChangeRequest.ValidateGroupRules("ClassificationChange", mode);
                if (aobjWssEmploymentChangeRequest.iarrErrors.Count == 0)
                {
                    aobjWssEmploymentChangeRequest.ValidateGroupRules("EmpTypeChange", mode);
                }
            }
            if (iblnEmploymentChangeRequestChangeTypeEmployment)
            {
                aobjWssEmploymentChangeRequest.ValidateGroupRules("EmpTypeChange", mode);
            }
            if (iblnEmploymentChangeRequestChangeTypeLOA || iblnEmploymentChangeRequestChangeTypeLOAM || iblnEmploymentChangeRequestChangeTypeFMLA)
            {
                aobjWssEmploymentChangeRequest.ValidateGroupRules("LOA", mode);
            }
            
            if (aobjWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.last_date_of_service != DateTime.MinValue )
            {
                aobjWssEmploymentChangeRequest.ValidateGroupRules("TerminateEmployment", mode);
            }

            iblnEmploymentChangeRequestChangeTypeClassification = false;
            iblnEmploymentChangeRequestChangeTypeEmployment = false;
            iblnEmploymentChangeRequestChangeTypeLOA = false;
            iblnEmploymentChangeRequestChangeTypeLOAM = false;
            iblnEmploymentChangeRequestChangeTypeFMLA = false;


            if (aobjWssEmploymentChangeRequest.iarrErrors != null && aobjWssEmploymentChangeRequest.iarrErrors.Count > 0)
            {
                if (!iclbUnprocessedPsEmployment.Contains(aobjPsEmployment))
                {
                    iclbUnprocessedPsEmployment.Add(aobjPsEmployment);
                    if (!aobjPsEmployment.idictPsEmploymentError.ContainsKey(aobjPsEmployment.icdoPsEmployment.ps_employment_id))
                        aobjPsEmployment.idictPsEmploymentError.Add(aobjPsEmployment.icdoPsEmployment.ps_employment_id, aobjWssEmploymentChangeRequest.iarrErrors);
                }
            }
            else
            {
                DataTable ltbEmploymentChangeRequest = Select("cdoPsEmployment.GetEmploymentChangeRequest", new object[1] { busGlobalFunctions.GetPersonIDBySSN(aobjPsEmployment.icdoPsEmployment.ssn) });
                if (ltbEmploymentChangeRequest.Rows.Count == 0)
                {
                    aobjPsEmployment.icdoPsEmployment.processed_flag = busConstant.Flag_Yes;
                    aobjWssEmploymentChangeRequest.BeforePersistChanges();
                    aobjWssEmploymentChangeRequest.PersistChanges();
                    aobjWssEmploymentChangeRequest.ValidateSoftErrors();
                    aobjWssEmploymentChangeRequest.AfterPersistChanges();
                    if (!iclbProcessedPsEmployment.Contains(aobjPsEmployment))
                    {
                        iclbProcessedPsEmployment.Add(aobjPsEmployment);
                    }
                    //iclbProcessedPsPerson.ReplaceIfExists(lbusPsPerson);
                }
            }
        }
        public void SetWssPersonEmploymentInfo(busWssMemberRecordRequest abusWssMemberRecordRequest, busOrganization aobjOrganization)
        {
            if (abusWssMemberRecordRequest.icdoWssMemberRecordRequest.member_record_request_id > 0)
            {
                abusWssMemberRecordRequest.icdoWssPersonEmployment.start_date = icdoPsEmployment.empl_start_date;
                abusWssMemberRecordRequest.icdoWssPersonEmployment.ps_empl_record_number = icdoPsEmployment.ps_empl_record_number;
                abusWssMemberRecordRequest.ibusOrganization = aobjOrganization;
                abusWssMemberRecordRequest.icdoWssPersonEmploymentDetail.type_value = icdoPsEmployment.job_type_value;
                abusWssMemberRecordRequest.icdoWssPersonEmploymentDetail.job_class_value = icdoPsEmployment.job_class_value;
                abusWssMemberRecordRequest.icdoWssPersonEmploymentDetail.employment_status_value = icdoPsEmployment.empl_status_value;
                abusWssMemberRecordRequest.icdoWssMemberRecordRequest.first_month_for_retirement_contribution = icdoPsEmployment.empl_status_value == busConstant.EmploymentStatusContributing ? new DateTime(icdoPsEmployment.empl_start_date.Year, icdoPsEmployment.empl_start_date.Month, 01) : DateTime.MinValue;
                if (icdoPsEmployment.hourly_value == busConstant.Flag_Yes)
                    abusWssMemberRecordRequest.icdoWssPersonEmploymentDetail.hourly_value = busConstant.Flag_Yes_Value;
                else
                    abusWssMemberRecordRequest.icdoWssPersonEmploymentDetail.hourly_value = busConstant.Flag_No_Value;
                abusWssMemberRecordRequest.icdoWssPersonEmploymentDetail.official_list_value = icdoPsEmployment.official_list_value;
                if(abusWssMemberRecordRequest.icdoWssPersonEmploymentDetail.job_class_value == busConstant.PersonJobClassStateElectedOfficial)
                    abusWssMemberRecordRequest.icdoWssPersonEmploymentDetail.term_begin_date = icdoPsEmployment.term_begin_date;
                if (abusWssMemberRecordRequest.icdoWssPersonContact.wss_person_contact_id > 0)
                {
                    abusWssMemberRecordRequest.icdoWssPersonContact.contact_org_code = aobjOrganization.icdoOrganization.org_code;
                    abusWssMemberRecordRequest.icdoWssPersonContact.contact_org_id = iintOrgId;
                    abusWssMemberRecordRequest.SetContactAddress();
                }
            }
            else
            {
                abusWssMemberRecordRequest.icdoWssPersonEmployment = new cdoWssPersonEmployment();
                abusWssMemberRecordRequest.icdoWssPersonEmployment.start_date = icdoPsEmployment.empl_start_date;
                abusWssMemberRecordRequest.icdoWssPersonEmployment.ps_empl_record_number = icdoPsEmployment.ps_empl_record_number;
                abusWssMemberRecordRequest.icdoWssPersonContact = new cdoWssPersonContact();
                abusWssMemberRecordRequest.icdoWssPersonContact.contact_org_code = aobjOrganization.icdoOrganization.org_code;
                abusWssMemberRecordRequest.icdoWssPersonContact.contact_org_id = iintOrgId;
                abusWssMemberRecordRequest.SetContactAddress();
                abusWssMemberRecordRequest.ibusOrganization = aobjOrganization;
                abusWssMemberRecordRequest.icdoWssPersonEmploymentDetail = new cdoWssPersonEmploymentDetail();
                abusWssMemberRecordRequest.icdoWssPersonEmploymentDetail.type_value = icdoPsEmployment.job_type_value;
                abusWssMemberRecordRequest.icdoWssPersonEmploymentDetail.job_class_value = icdoPsEmployment.job_class_value;
                abusWssMemberRecordRequest.icdoWssPersonEmploymentDetail.employment_status_value = icdoPsEmployment.empl_status_value;
                abusWssMemberRecordRequest.icdoWssPersonEmploymentDetail.hourly_value = icdoPsEmployment.hourly_value;
                abusWssMemberRecordRequest.icdoWssPersonEmploymentDetail.official_list_value = icdoPsEmployment.official_list_value;
                abusWssMemberRecordRequest.icdoWssMemberRecordRequest.first_month_for_retirement_contribution = icdoPsEmployment.empl_status_value == busConstant.EmploymentStatusContributing ? new DateTime(icdoPsEmployment.empl_start_date.Year, icdoPsEmployment.empl_start_date.Month, 01) : DateTime.MinValue;
                if (abusWssMemberRecordRequest.icdoWssPersonEmploymentDetail.job_class_value == busConstant.PersonJobClassStateElectedOfficial)
                    abusWssMemberRecordRequest.icdoWssPersonEmploymentDetail.term_begin_date = icdoPsEmployment.term_begin_date;
            }
        }

        //6th September Start
        public bool CreatePersonEmploymentDetails(busPersonEmployment aobjPersonEmployment,busPsEmployment aobjPsEmployment,ref busPersonEmploymentDetail aobjEmpdetails)
        {
            

            aobjEmpdetails.icdoPersonEmploymentDetail.ienuObjectState = ObjectState.Insert;
            aobjEmpdetails.IsNewMode = true;
            aobjEmpdetails.icdoPersonEmploymentDetail.person_employment_id = aobjPersonEmployment.icdoPersonEmployment.person_employment_id;
            aobjEmpdetails.icdoPersonEmploymentDetail.start_date = aobjPersonEmployment.icdoPersonEmployment.start_date;
            aobjEmpdetails.icdoPersonEmploymentDetail.type_value = aobjPsEmployment.icdoPsEmployment.job_type_value;
            aobjEmpdetails.icdoPersonEmploymentDetail.job_class_value = aobjPsEmployment.icdoPsEmployment.job_class_value;
            if (aobjEmpdetails.icdoPersonEmploymentDetail.job_class_value == busConstant.PersonJobClassStateElectedOfficial)
                aobjEmpdetails.icdoPersonEmploymentDetail.term_begin_date = aobjPsEmployment.icdoPsEmployment.term_begin_date;
            aobjEmpdetails.icdoPersonEmploymentDetail.official_list_value = aobjPsEmployment.icdoPsEmployment.official_list_value;
            aobjEmpdetails.icdoPersonEmploymentDetail.hourly_value = aobjPsEmployment.icdoPsEmployment.hourly_value;
            //if ((aobjPersonEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.status_value == busConstant.EmploymentStatusContributing &&
            //    aobjPersonEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.type_value == busConstant.PersonJobTypePermanent) &&
            //    (aobjPsEmployment.icdoPsEmployment.job_type_value == busConstant.PersonJobTypeTemporary && aobjPsEmployment.icdoPsEmployment.empl_status_value == busConstant.EmploymentStatusContributing))
            //    aobjEmpdetails.icdoPersonEmploymentDetail.status_value = busConstant.EmploymentStatusNonContributing;
            //else
            aobjEmpdetails.icdoPersonEmploymentDetail.status_value = aobjPsEmployment.icdoPsEmployment.empl_status_value;//busConstant.EmploymentStatusContributing;
            //lobjEmpDetail.BeforeValidate(utlPageMode.New);
            aobjEmpdetails.icdoPersonEmploymentDetail.person_employment_id = aobjPersonEmployment.icdoPersonEmployment.person_employment_id;
            aobjEmpdetails.LoadPersonEmployment();
            aobjEmpdetails.ibusPersonEmployment.LoadPerson();
            aobjEmpdetails.ibusPersonEmployment.LoadOrganization();
            aobjEmpdetails.LoadPlansOffered();
            aobjEmpdetails.ValidateHardErrors(utlPageMode.New);
            if (aobjEmpdetails.iarrErrors.Count > 0)
            {
                return false;
            }
            else
            {

                aobjEmpdetails.BeforePersistChanges();
                aobjEmpdetails.PersistChanges();
                aobjEmpdetails.ValidateSoftErrors();
                aobjEmpdetails.AfterPersistChanges();
                return true;
                
           }
            //// PIR 9802
            //if (lobjEmpDetail.ibusPersonEmployment.IsNull()) lobjEmpDetail.LoadPersonEmployment();
            //if (lobjEmpDetail.ibusPersonEmployment.ibusOrganization.IsNull()) lobjEmpDetail.ibusPersonEmployment.LoadOrganization();
            //lobjEmpDetail.InsertEmployerOfferedPlans();
        }
        //6th September End

        public bool GetEmploymentChangeRequestInReviewLOA()
        {
            bool lblnResult = true;
            int iintCount = 0;

            object lobjCount = DBFunction.DBExecuteScalar("cdoPsEmployment.GetEmploymentChangeRequestLOA", new object[2]{
                                        icdoPsEmployment.ssn, icdoPsEmployment.org_code}, iobjPassInfo.iconFramework,
                                        iobjPassInfo.itrnFramework);
            if (lobjCount.IsNotNull())
            {
                iintCount = Convert.ToInt32(lobjCount);
            }
            if (iintCount > 0)
                lblnResult = false;

            return lblnResult;
        }

    }

}



//public static class CollectionExtensions
//{
//    public static bool ReplaceIfExists<T>(this Collection<T> collectionObj, T obj) where T : busBase
//    {
//        int index = -1;
//        //collectionObj.ForEach<T>(i =>
//        //    {
//        //        if (i.GetHashCode() == obj.GetHashCode())
//        //        {
//        //            index = collectionObj.IndexOf(i);
//        //        }
//        //    });            
//        index = collectionObj.IndexOf(obj);
//        if (index >= 0)
//        {
//            collectionObj.RemoveAt(index);
//            collectionObj.Insert(index, obj);
//        }
//        else
//        {
//            collectionObj.Add(obj);
//        }
//        return true;
//    }
//}

