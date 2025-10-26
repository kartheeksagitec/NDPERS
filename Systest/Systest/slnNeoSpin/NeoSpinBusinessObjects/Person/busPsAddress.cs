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
#endregion

namespace NeoSpin.BusinessObjects
{
    /// <summary>
    /// Class NeoSpin.BusinessObjects.busPsAddress:
    /// Inherited from busPsAddressGen, the class is used to customize the business object busPsAddressGen.
    /// </summary>
    [Serializable]
    public class busPsAddress : busPsAddressGen
    {
        public Collection<busPsAddress> iclbPsPersonAddress { get; set; }
        public Collection<busPsAddress> iclbProcessedPsPersonAddress { get; set; }
        public Collection<busPsAddress> iclbUnProcessedPsPersonAddress { get; set; }
        //public Collection<busPsAddress> iclbPersonAddressInformativeErrors { get; set; }
        public ArrayList iarrAddressErrorList { get; set; }
        public busPerson ibusPerson { get; set; }
        public busPsPerson ibusPsPerson { get; set; }
        // 5 September
        public bool iblnAddressChanged { get; set; }
        public bool iblnNewAndFirstAddress { get; set; }

        //PIR 12578
        public bool iblnEmploymentChangeRequestChangeTypeClassification { get; set; }
        public bool iblnEmploymentChangeRequestChangeTypeEmployment { get; set; }
        public bool iblnEmploymentChangeRequestChangeTypeLOA { get; set; }
        public bool iblnEmploymentChangeRequestChangeTypeLOAM { get; set; }
        public bool iblnEmploymentChangeRequestChangeTypeFMLA { get; set; }

        public bool iblnIsAddressNotValidated { get; set; }

        //public ArrayList iarrErrors = new ArrayList();
        public Dictionary<int, ArrayList> idictPsAddressError = new Dictionary<int, ArrayList>();
        /// <summary>
        /// 
        /// </summary>
        public void LoadUnprocessedPsAddress()
        {
            //DataTable ldtPsAddress = Select<cdoPsAddress>(new string[1] { enmPsAddress.processed_flag.ToString() },
                                                                       //new object[1] { busConstant.Flag_No }, null, null);
            DataTable ldtPsAddress = Select("cdoPsAddress.GetUnprocessedPsAddress", new object[] { });
            iclbPsPersonAddress = GetCollection<busPsAddress>(ldtPsAddress, "icdoPsAddress");

        }

        /// <summary>
        /// 
        /// </summary>
        public void InsertPersonAddress(Collection<busPsPerson> aclbProcesssedPsRecord, ref Collection<busPsAddress> iclbPersonAddressInformativeErrors)
        {
            //busWssPersonAddress lbusWssPersonAddress = null;
            iclbProcessedPsPersonAddress = new Collection<busPsAddress>();
            iclbUnProcessedPsPersonAddress = new Collection<busPsAddress>();
            //iclbPersonAddressInformativeErrors = new Collection<busPsAddress>();
            foreach (busPsAddress lbusPsAddress in iclbPsPersonAddress)
            {
                lbusPsAddress.idictPsAddressError = new Dictionary<int, ArrayList>();
                lbusPsAddress.iarrErrors = new ArrayList();
                lbusPsAddress.ibusPsPerson = LoadPSPersonBySSN(lbusPsAddress.icdoPsAddress.ssn);

                if (aclbProcesssedPsRecord.Where(o => o.icdoPsPerson.ssn == lbusPsAddress.icdoPsAddress.ssn).Count() > 0)
                    iclbProcessedPsPersonAddress.Add(lbusPsAddress);
                else
                {
                    if (!lbusPsAddress.icdoPsAddress.ssn.IsNumeric())
                    {
                        utlError lobjError = null;
                        lobjError = AddError(1037, "SSN must be numeric.");
                        lbusPsAddress.iarrErrors.Add(lobjError);
                        iclbUnProcessedPsPersonAddress.Add(lbusPsAddress);
                        if (!lbusPsAddress.idictPsAddressError.ContainsKey(lbusPsAddress.icdoPsAddress.ps_address_id))
                            lbusPsAddress.idictPsAddressError.Add(lbusPsAddress.icdoPsAddress.ps_address_id, lbusPsAddress.iarrErrors);
                        continue;
                    }
                    
                    busWssMemberRecordRequest lbusWssMemberRecordRequest = new busWssMemberRecordRequest { icdoWssMemberRecordRequest = new cdoWssMemberRecordRequest() };
                    // Assign the matching properties for the person address based on the logic and given scenarios
                    lbusPsAddress.ibusPerson = new busPerson();
                    lbusPsAddress.ibusPerson = lbusPsAddress.ibusPerson.LoadPersonBySsn(lbusPsAddress.icdoPsAddress.ssn);
                    //busWssMemberRecordRequest lobjPendingWssMemberRecordRequest = lbusWssMemberRecordRequest.LoadPendingMemberRecordRequestbySSN(lbusPsAddress.icdoPsAddress.ssn);
                    //if (lobjPendingWssMemberRecordRequest == null)
                    //{
                    if (lbusPsAddress.ibusPsPerson != null)
                    {
                        CreateMemberRecordRequest(lbusWssMemberRecordRequest, lbusPsAddress.ibusPsPerson, null);
                        lbusPsAddress.SetWssPersonAddressInfo(lbusWssMemberRecordRequest);
                        if (lbusPsAddress.iblnIsAddressNotValidated)
                        {
                            utlError lobjError = null;
                            lobjError = AddError(130, "Person Address is invalid.");
                            lbusPsAddress.iarrErrors.Add(lobjError);
                            iclbUnProcessedPsPersonAddress.Add(lbusPsAddress);
                            iclbPersonAddressInformativeErrors.Add(lbusPsAddress);
                            if (!lbusPsAddress.idictPsAddressError.ContainsKey(lbusPsAddress.icdoPsAddress.ps_address_id))
                                lbusPsAddress.idictPsAddressError.Add(lbusPsAddress.icdoPsAddress.ps_address_id, lbusPsAddress.iarrErrors);
                            continue;
                        }
                    }
                    else
                    {

                        busPerson lobjPerson = new busPerson { icdoPerson = new cdoPerson() };
                        lobjPerson = lobjPerson.LoadPersonBySsn(lbusPsAddress.icdoPsAddress.ssn);
                        if (lobjPerson.icdoPerson.person_id > 0)
                        {
                            lbusWssMemberRecordRequest.icdoWssMemberRecordRequest = new cdoWssMemberRecordRequest();
                            CreateMemberRecordRequest(lbusWssMemberRecordRequest, null, lobjPerson);
                            lbusWssMemberRecordRequest.icdoWssPersonAddress = new cdoWssPersonAddress();
                            lbusPsAddress.SetWssPersonAddressInfo(lbusWssMemberRecordRequest);
                            if (lbusPsAddress.iblnIsAddressNotValidated)
                            {
                                utlError lobjError = null;
                                lobjError = AddError(130, "Person Address is invalid.");
                                lbusPsAddress.iarrErrors.Add(lobjError);
                                iclbUnProcessedPsPersonAddress.Add(lbusPsAddress);
                                iclbPersonAddressInformativeErrors.Add(lbusPsAddress);
                                if (!lbusPsAddress.idictPsAddressError.ContainsKey(lbusPsAddress.icdoPsAddress.ps_address_id))
                                    lbusPsAddress.idictPsAddressError.Add(lbusPsAddress.icdoPsAddress.ps_address_id, lbusPsAddress.iarrErrors);
                                continue;
                            }

                        }
                        else
                        {
                            utlError lobjError = null;
                            lobjError = AddError(10208, "PS person Record does not exist.");
                            lbusPsAddress.iarrErrors.Add(lobjError);
                            iclbUnProcessedPsPersonAddress.Add(lbusPsAddress);
                            if (!lbusPsAddress.idictPsAddressError.ContainsKey(lbusPsAddress.icdoPsAddress.ps_address_id))
                                lbusPsAddress.idictPsAddressError.Add(lbusPsAddress.icdoPsAddress.ps_address_id, lbusPsAddress.iarrErrors);
                            continue;
                        }
                    }

                    #region Ps Employment
                    Collection<busPsEmployment> lclbPsEmployment = LoadPsEmploymentBySSN(lbusPsAddress.icdoPsAddress.ssn);
                    if (lclbPsEmployment.Count == 0)
                    {
                        utlError lobjError = null;
                        lobjError = AddError(10206, "PS Employment Record does not exist.");
                        lbusPsAddress.iarrErrors.Add(lobjError);
                        iclbUnProcessedPsPersonAddress.Add(lbusPsAddress);
                        if (!lbusPsAddress.idictPsAddressError.ContainsKey(lbusPsAddress.icdoPsAddress.ps_address_id))
                            lbusPsAddress.idictPsAddressError.Add(lbusPsAddress.icdoPsAddress.ps_address_id, lbusPsAddress.iarrErrors);
                        continue;
                    }
                    else
                    {
                        foreach (busPsEmployment lbusPsEmployment in lclbPsEmployment)
                        {
                            int lintOrgId = 0;
                            busWssEmploymentChangeRequest lbusWssEmploymentChangeRequest = new busWssEmploymentChangeRequest { icdoWssEmploymentChangeRequest = new cdoWssEmploymentChangeRequest() };
                            DataTable ldtbOrganization = Select("cdoOrganization.GetOrganizationFromOrgCode", new object[1] { lbusPsEmployment.icdoPsEmployment.org_code });
                            if (ldtbOrganization != null)
                                lbusPsEmployment.iintOrgId = Convert.ToInt32(ldtbOrganization.Rows[0][enmOrganization.org_id.ToString()]);
                            busOrganization lobjOrganization = new busOrganization();
                            lobjOrganization.FindOrganization(lbusPsEmployment.iintOrgId);
                            lbusPsEmployment.ibusPerson = new busPerson();
                            lbusPsEmployment.ibusPerson = lbusPsEmployment.ibusPerson.LoadPersonBySsn(lbusPsEmployment.icdoPsEmployment.ssn);
                            DataTable ldtbPersonEmployment = Select("cdoPersonEmployment.GetEmploymentDetail", new object[1] { lbusPsEmployment.icdoPsEmployment.ssn });
                            Collection<busPersonEmployment> lclbPersonEmployment = GetCollection<busPersonEmployment>(ldtbPersonEmployment, "icdoPersonEmployment");


                            if (lbusPsEmployment.icdoPsEmployment.empl_end_date == DateTime.MinValue &&
                                (lbusPsEmployment.icdoPsEmployment.job_class_value.IsNull() || lbusPsEmployment.icdoPsEmployment.job_type_value.IsNull()
                                || lbusPsEmployment.icdoPsEmployment.job_class_value.Trim().IsEmpty() || lbusPsEmployment.icdoPsEmployment.job_type_value.Trim().IsEmpty()))
                            {
                                utlError lobjError = null;
                                lobjError = AddError(10221, "Job class/Job type is required.");
                                lbusPsAddress.iarrErrors.Add(lobjError);
                                iclbUnProcessedPsPersonAddress.Add(lbusPsAddress);
                                iclbPersonAddressInformativeErrors.Add(lbusPsAddress);
                                if (!lbusPsAddress.idictPsAddressError.ContainsKey(lbusPsAddress.icdoPsAddress.ps_address_id))
                                    lbusPsAddress.idictPsAddressError.Add(lbusPsAddress.icdoPsAddress.ps_address_id, lbusPsAddress.iarrErrors);
                                continue;
                            }
                            
                            // As per mail from Maik dated 03/20/2014 - Issue#4
                            if ((lbusPsEmployment.ibusPerson != null) && (lbusPsEmployment.icdoPsEmployment.empl_end_date != DateTime.MinValue))
                            {
                                lbusPsEmployment.ibusPerson.LoadPersonEmployment();
                                busPersonEmployment lbusPersonEmployment = lbusPsEmployment.ibusPerson.icolPersonEmployment.Where(o => o.icdoPersonEmployment.org_id == lbusPsEmployment.iintOrgId)
                                                                            .OrderByDescending(o => o.icdoPersonEmployment.start_date).FirstOrDefault();
                                if (lbusPersonEmployment.IsNull())
                                {
                                    utlError lobjError = null;
                                    lobjError = AddError(4720, "Employment does not exist.");
                                    lbusPsAddress.iarrErrors.Add(lobjError);
                                    iclbUnProcessedPsPersonAddress.Add(lbusPsAddress);
                                    iclbPersonAddressInformativeErrors.Add(lbusPsAddress);
                                    if (!lbusPsAddress.idictPsAddressError.ContainsKey(lbusPsAddress.icdoPsAddress.ps_address_id))
                                        lbusPsAddress.idictPsAddressError.Add(lbusPsAddress.icdoPsAddress.ps_address_id, lbusPsAddress.iarrErrors);
                                    continue;
                                }
                            }

                            #region Dual Employment
                            if (lclbPersonEmployment.Count > 1)
                            {
                                //Dual Employment Scenario : 2 Sep
                                busPersonEmployment lbusPersomEmp = lclbPersonEmployment.Where(lobj => lobj.icdoPersonEmployment.org_id == lbusPsEmployment.iintOrgId).FirstOrDefault();
                                if (lbusPersomEmp != null)
                                {
                                    lbusPersomEmp.LoadLatestPersonEmploymentDetail();
                                    if (lbusPsEmployment.icdoPsEmployment.ps_empl_record_number == (lbusPersomEmp.icdoPersonEmployment.ps_empl_record_number == null ? "0" : lbusPersomEmp.icdoPersonEmployment.ps_empl_record_number))
                                    {
                                        if (lbusPsEmployment.icdoPsEmployment.loa_date_of_return != DateTime.MinValue
                                            && !(lbusPersomEmp.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.status_value == busConstant.EmploymentStatusLOA
                                            || lbusPersomEmp.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.status_value == busConstant.EmploymentStatusLOAM
                                            || lbusPersomEmp.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.status_value == busConstant.EmploymentStatusFMLA))
                                        {
                                            iclbUnProcessedPsPersonAddress.Add(lbusPsAddress);
                                            if (lbusPsEmployment.GetEmploymentChangeRequestInReviewLOA())
                                            {
                                                utlError lobjError = null;
                                                lobjError = AddError(10222, "LOA employment doesn't exist.");
                                                lbusPsAddress.iarrErrors.Add(lobjError);
                                                
                                                iclbPersonAddressInformativeErrors.Add(lbusPsAddress);
                                                if (!lbusPsAddress.idictPsAddressError.ContainsKey(lbusPsAddress.icdoPsAddress.ps_address_id))
                                                    lbusPsAddress.idictPsAddressError.Add(lbusPsAddress.icdoPsAddress.ps_address_id, lbusPsAddress.iarrErrors);
                                            }
                                            continue;
                                        }
                                        CreateEmploymentChangeRequest(lbusWssEmploymentChangeRequest, lbusPsEmployment, lbusPersomEmp);
                                        if (lbusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.change_type_value.IsNull())
                                        {
                                            if (lbusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.last_date_of_service == DateTime.MinValue)
                                            {
                                                //utlError lobjError = null;
                                                //lobjError = AddError(10210, "Employment Record already exists.");
                                                if (!iclbProcessedPsPersonAddress.Contains(lbusPsAddress))
                                                    iclbProcessedPsPersonAddress.Add(lbusPsAddress);
                                                continue;
                                            }
                                        }
                                        ValidatePSRecords(lbusWssEmploymentChangeRequest, lbusPsAddress, utlPageMode.New);
                                    }
                                    else //Same Organization Different Record number
                                    {
                                        if (lbusPsAddress.ibusPsPerson != null) //lbusPsEmployment.ibusPsPerson != null && lbusPsEmployment.ibusPsAddress != null
                                        {
                                            lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.org_id = lbusPsEmployment.iintOrgId;
                                            lbusPsEmployment.SetWssPersonEmploymentInfo(lbusWssMemberRecordRequest, lobjOrganization);
                                            if (lbusPersomEmp.icdoPersonEmployment.ps_empl_record_number == null)
                                            {
												//25926	FILLER: Remove PS Error Report Error of "Employment exists, but Empl Record Number is Different"
                                                //if (lbusPsEmployment.icdoPsEmployment.empl_start_date == lbusPersomEmp.icdoPersonEmployment.start_date)
                                                //{
                                                //    if (lbusPsEmployment.icdoPsEmployment.job_class_value == lbusPersomEmp.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.job_class_value
                                                //    && lbusPsEmployment.icdoPsEmployment.job_type_value == lbusPersomEmp.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.type_value
                                                //    && lbusPsEmployment.icdoPsEmployment.empl_status_value == lbusPersomEmp.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.status_value
                                                //    )
                                                //    {
                                                //        utlError lobjError = null;
                                                //        lobjError = AddError(10214, "Employment exists, but Empl Record Number is different.");
                                                //        lbusPsAddress.iarrErrors.Add(lobjError);
                                                //        iclbUnProcessedPsPersonAddress.Add(lbusPsAddress);
                                                //        iclbPersonAddressInformativeErrors.Add(lbusPsAddress);
                                                //        if (!lbusPsAddress.idictPsAddressError.ContainsKey(lbusPsAddress.icdoPsAddress.ps_address_id))
                                                //            lbusPsAddress.idictPsAddressError.Add(lbusPsAddress.icdoPsAddress.ps_address_id, lbusPsAddress.iarrErrors);
                                                //        continue;
                                                //    }
                                                //}
                                                //else
                                                //{
                                                //     if (lbusPsEmployment.icdoPsEmployment.job_class_value == lbusPersomEmp.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.job_class_value
                                                //    && lbusPsEmployment.icdoPsEmployment.job_type_value == lbusPersomEmp.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.type_value
                                                //    && lbusPsEmployment.icdoPsEmployment.empl_status_value == lbusPersomEmp.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.status_value
                                                //    )
                                                //    {
                                                //        utlError lobjError = null;
                                                //        lobjError = AddError(10213, "Employment Record already exists with different date.");
                                                //        lbusPsAddress.iarrErrors.Add(lobjError);
                                                //        iclbUnProcessedPsPersonAddress.Add(lbusPsAddress);
                                                //        iclbPersonAddressInformativeErrors.Add(lbusPsAddress);
                                                //        if (!lbusPsAddress.idictPsAddressError.ContainsKey(lbusPsAddress.icdoPsAddress.ps_address_id))
                                                //            lbusPsAddress.idictPsAddressError.Add(lbusPsAddress.icdoPsAddress.ps_address_id, lbusPsAddress.iarrErrors);
                                                //        continue;
                                                //    }
                                                //}
                                            }
                                            ValidatePSRecords(lbusWssMemberRecordRequest, lbusPsAddress, utlPageMode.New, iclbPersonAddressInformativeErrors);
                                        }
                                        else if (lbusPsAddress.ibusPerson != null)
                                        {
                                            lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.org_id = lbusPsEmployment.iintOrgId;
                                            lbusPsEmployment.SetWssPersonEmploymentInfo(lbusWssMemberRecordRequest, lobjOrganization);
                                            if (lbusPersomEmp.icdoPersonEmployment.ps_empl_record_number == null)
                                            {
                                                if (lbusPsEmployment.icdoPsEmployment.empl_start_date == lbusPersomEmp.icdoPersonEmployment.start_date)
                                                {
                                                    if (lbusPsEmployment.icdoPsEmployment.job_class_value == lbusPersomEmp.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.job_class_value
                                                    && lbusPsEmployment.icdoPsEmployment.job_type_value == lbusPersomEmp.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.type_value
                                                    && lbusPsEmployment.icdoPsEmployment.empl_status_value == lbusPersomEmp.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.status_value
                                                    )
                                                    {
                                                        //utlError lobjError = null;
                                                        //lobjError = AddError(10210, "Employment Record already exists.");
                                                        if (!iclbProcessedPsPersonAddress.Contains(lbusPsAddress))
                                                            iclbProcessedPsPersonAddress.Add(lbusPsAddress);
                                                        continue;
                                                    }
                                                }
                                                //else
                                                //{
                                                //    if (lbusPsEmployment.icdoPsEmployment.job_class_value == lbusPersomEmp.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.job_class_value
                                                //    && lbusPsEmployment.icdoPsEmployment.job_type_value == lbusPersomEmp.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.type_value
                                                //    && lbusPsEmployment.icdoPsEmployment.empl_status_value == lbusPersomEmp.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.status_value
                                                //    )
                                                //    {
                                                //        utlError lobjError = null;
                                                //        lobjError = AddError(10213, "Employment Record already exists with different date.");
                                                //        lbusPsAddress.iarrErrors.Add(lobjError);
                                                //        iclbUnProcessedPsPersonAddress.Add(lbusPsAddress);
                                                //        iclbPersonAddressInformativeErrors.Add(lbusPsAddress);
                                                //        if (!lbusPsAddress.idictPsAddressError.ContainsKey(lbusPsAddress.icdoPsAddress.ps_address_id))
                                                //            lbusPsAddress.idictPsAddressError.Add(lbusPsAddress.icdoPsAddress.ps_address_id, lbusPsAddress.iarrErrors);
                                                //        continue;
                                                //    }
                                                //}
                                            }
                                            ValidatePSRecords(lbusWssMemberRecordRequest, lbusPsAddress, utlPageMode.New, iclbPersonAddressInformativeErrors);
                                        }
                                        else
                                        {
                                            if (lbusPsEmployment.icdoPsEmployment.loa_date_of_return != DateTime.MinValue
                                                && !(lbusPersomEmp.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.status_value == busConstant.EmploymentStatusLOA
                                                || lbusPersomEmp.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.status_value == busConstant.EmploymentStatusLOAM
                                                || lbusPersomEmp.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.status_value == busConstant.EmploymentStatusFMLA))
                                            {
                                                iclbUnProcessedPsPersonAddress.Add(lbusPsAddress);
                                                if (lbusPsEmployment.GetEmploymentChangeRequestInReviewLOA())
                                                {
                                                    utlError lobjError = null;
                                                    lobjError = AddError(10222, "LOA employment doesn't exist.");
                                                    lbusPsAddress.iarrErrors.Add(lobjError);

                                                    iclbPersonAddressInformativeErrors.Add(lbusPsAddress);
                                                    if (!lbusPsAddress.idictPsAddressError.ContainsKey(lbusPsAddress.icdoPsAddress.ps_address_id))
                                                        lbusPsAddress.idictPsAddressError.Add(lbusPsAddress.icdoPsAddress.ps_address_id, lbusPsAddress.iarrErrors);
                                                }
                                                continue;
                                            }
                                            CreateEmploymentChangeRequest(lbusWssEmploymentChangeRequest, lbusPsEmployment, lbusPersomEmp);
                                            if (lbusPersomEmp.icdoPersonEmployment.ps_empl_record_number == null)
                                            {
												//25926	FILLER: Remove PS Error Report Error of "Employment exists, but Empl Record Number is Different"
                                                //if (lbusPsEmployment.icdoPsEmployment.empl_start_date == lbusPersomEmp.icdoPersonEmployment.start_date)
                                                //{
                                                //    if (lbusPsEmployment.icdoPsEmployment.job_class_value == lbusPersomEmp.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.job_class_value
                                                //    && lbusPsEmployment.icdoPsEmployment.job_type_value == lbusPersomEmp.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.type_value
                                                //    && lbusPsEmployment.icdoPsEmployment.empl_status_value == lbusPersomEmp.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.status_value
                                                //    )
                                                //    {
                                                //        utlError lobjError = null;
                                                //        lobjError = AddError(10214, "Employment exists, but Empl Record Number is different.");
                                                //        lbusPsAddress.iarrErrors.Add(lobjError);
                                                //        iclbUnProcessedPsPersonAddress.Add(lbusPsAddress);
                                                //        iclbPersonAddressInformativeErrors.Add(lbusPsAddress);
                                                //        if (!lbusPsAddress.idictPsAddressError.ContainsKey(lbusPsAddress.icdoPsAddress.ps_address_id))
                                                //            lbusPsAddress.idictPsAddressError.Add(lbusPsAddress.icdoPsAddress.ps_address_id, lbusPsAddress.iarrErrors);
                                                //        continue;
                                                //    }
                                                //}
                                                //else
                                                //{
                                                //    if (lbusPsEmployment.icdoPsEmployment.job_class_value == lbusPersomEmp.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.job_class_value
                                                //    && lbusPsEmployment.icdoPsEmployment.job_type_value == lbusPersomEmp.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.type_value
                                                //    && lbusPsEmployment.icdoPsEmployment.empl_status_value == lbusPersomEmp.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.status_value
                                                //    )
                                                //    {
                                                //        utlError lobjError = null;
                                                //        lobjError = AddError(10213, "Employment Record already exists with different date.");
                                                //        lbusPsAddress.iarrErrors.Add(lobjError);
                                                //        iclbUnProcessedPsPersonAddress.Add(lbusPsAddress);
                                                //        iclbPersonAddressInformativeErrors.Add(lbusPsAddress);
                                                //        if (!lbusPsAddress.idictPsAddressError.ContainsKey(lbusPsAddress.icdoPsAddress.ps_address_id))
                                                //            lbusPsAddress.idictPsAddressError.Add(lbusPsAddress.icdoPsAddress.ps_address_id, lbusPsAddress.iarrErrors);
                                                //        continue;
                                                //    }
                                                //}
                                            }
                                            ValidatePSRecords(lbusWssEmploymentChangeRequest, lbusPsAddress, utlPageMode.New);
                                        }
                                    }
                                }
                                else
                                {
                                    lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.org_id = lbusPsEmployment.iintOrgId;
                                    lbusPsEmployment.SetWssPersonEmploymentInfo(lbusWssMemberRecordRequest, lobjOrganization);
                                    ValidatePSRecords(lbusWssMemberRecordRequest, lbusPsAddress, utlPageMode.New, iclbPersonAddressInformativeErrors);
                                }
                            }
                            #endregion Dual Employment
                            else if (lclbPersonEmployment.Count == 1)
                            {
                                foreach (busPersonEmployment lobjPersonEmployment in lclbPersonEmployment)
                                {
                                    lobjPersonEmployment.LoadLatestPersonEmploymentDetail();
                                    if (lbusPsEmployment.iintOrgId != lobjPersonEmployment.icdoPersonEmployment.org_id)
                                    {
                                        lbusPsEmployment.SetWssPersonEmploymentInfo(lbusWssMemberRecordRequest, lobjOrganization);
                                        ValidatePSRecords(lbusWssMemberRecordRequest, lbusPsAddress, utlPageMode.New, iclbPersonAddressInformativeErrors);
                                    }
                                    else
                                    {
                                        //Same Organization Same Record Number
                                        if (lbusPsEmployment.icdoPsEmployment.ps_empl_record_number == (lobjPersonEmployment.icdoPersonEmployment.ps_empl_record_number == null ? "0" : lobjPersonEmployment.icdoPersonEmployment.ps_empl_record_number))
                                        {
                                            if (lbusPsEmployment.icdoPsEmployment.loa_date_of_return != DateTime.MinValue
                                                && !(lobjPersonEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.status_value == busConstant.EmploymentStatusLOA
                                                || lobjPersonEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.status_value == busConstant.EmploymentStatusLOAM
                                                || lobjPersonEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.status_value == busConstant.EmploymentStatusFMLA))
                                            {
                                                iclbUnProcessedPsPersonAddress.Add(lbusPsAddress);
                                                if (lbusPsEmployment.GetEmploymentChangeRequestInReviewLOA())
                                                {
                                                    utlError lobjError = null;
                                                    lobjError = AddError(10222, "LOA employment doesn't exist.");
                                                    lbusPsAddress.iarrErrors.Add(lobjError);

                                                    iclbPersonAddressInformativeErrors.Add(lbusPsAddress);
                                                    if (!lbusPsAddress.idictPsAddressError.ContainsKey(lbusPsAddress.icdoPsAddress.ps_address_id))
                                                        lbusPsAddress.idictPsAddressError.Add(lbusPsAddress.icdoPsAddress.ps_address_id, lbusPsAddress.iarrErrors);
                                                }
                                                continue;
                                            }
                                            CreateEmploymentChangeRequest(lbusWssEmploymentChangeRequest, lbusPsEmployment, lobjPersonEmployment);
                                            if (lbusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.change_type_value.IsNull())
                                            {
                                                if (lbusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.last_date_of_service == DateTime.MinValue)
                                                {
                                                    //utlError lobjError = null;
                                                    //lobjError = AddError(10210, "Employment Record already exists.");
                                                    if (!iclbProcessedPsPersonAddress.Contains(lbusPsAddress))
                                                        iclbProcessedPsPersonAddress.Add(lbusPsAddress);
                                                    continue;
                                                }
                                            }
                                            ValidatePSRecords(lbusWssEmploymentChangeRequest, lbusPsAddress, utlPageMode.New);
                                        }
                                        else //Same Organization Different Record Number 
                                        {
                                            if (lbusPsAddress.ibusPsPerson != null) //lbusPsEmployment.ibusPsPerson != null && lbusPsEmployment.ibusPsAddress != null
                                            {
                                                lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.org_id = lbusPsEmployment.iintOrgId;
                                                lbusPsEmployment.SetWssPersonEmploymentInfo(lbusWssMemberRecordRequest, lobjOrganization);
                                                if (lobjPersonEmployment.icdoPersonEmployment.ps_empl_record_number == null)
                                                {
													//25926	FILLER: Remove PS Error Report Error of "Employment exists, but Empl Record Number is Different"
                                                    //if (lbusPsEmployment.icdoPsEmployment.empl_start_date == lobjPersonEmployment.icdoPersonEmployment.start_date)
                                                    //{

                                                    //    if (lbusPsEmployment.icdoPsEmployment.job_class_value == lobjPersonEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.job_class_value
                                                    //    && lbusPsEmployment.icdoPsEmployment.job_type_value == lobjPersonEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.type_value
                                                    //    && lbusPsEmployment.icdoPsEmployment.empl_status_value == lobjPersonEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.status_value
                                                    //    )
                                                    //    {
                                                    //        utlError lobjError = null;
                                                    //        lobjError = AddError(10214, "Employment exists, but Empl Record Number is different.");
                                                    //        lbusPsAddress.iarrErrors.Add(lobjError);
                                                    //        iclbUnProcessedPsPersonAddress.Add(lbusPsAddress);
                                                    //        iclbPersonAddressInformativeErrors.Add(lbusPsAddress);
                                                    //        if (!lbusPsAddress.idictPsAddressError.ContainsKey(lbusPsAddress.icdoPsAddress.ps_address_id))
                                                    //            lbusPsAddress.idictPsAddressError.Add(lbusPsAddress.icdoPsAddress.ps_address_id, lbusPsAddress.iarrErrors);
                                                    //        continue;
                                                    //    }
                                                    //}
                                                    //else
                                                    //{
                                                    //    if (lbusPsEmployment.icdoPsEmployment.job_class_value == lobjPersonEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.job_class_value
                                                    //    && lbusPsEmployment.icdoPsEmployment.job_type_value == lobjPersonEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.type_value
                                                    //    && lbusPsEmployment.icdoPsEmployment.empl_status_value == lobjPersonEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.status_value
                                                    //    )
                                                    //    {
                                                    //        utlError lobjError = null;
                                                    //        lobjError = AddError(10213, "Employment Record already exists with different date.");
                                                    //        lbusPsAddress.iarrErrors.Add(lobjError);
                                                    //        iclbUnProcessedPsPersonAddress.Add(lbusPsAddress);
                                                    //        iclbPersonAddressInformativeErrors.Add(lbusPsAddress);
                                                    //        if (!lbusPsAddress.idictPsAddressError.ContainsKey(lbusPsAddress.icdoPsAddress.ps_address_id))
                                                    //            lbusPsAddress.idictPsAddressError.Add(lbusPsAddress.icdoPsAddress.ps_address_id, lbusPsAddress.iarrErrors);
                                                    //        continue;
                                                    //    }
                                                    //}
                                                }
                                                ValidatePSRecords(lbusWssMemberRecordRequest, lbusPsAddress, utlPageMode.New, iclbPersonAddressInformativeErrors);
                                            }
                                            else
                                            {
                                                if (lbusPsEmployment.icdoPsEmployment.loa_date_of_return != DateTime.MinValue
                                                    && !(lobjPersonEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.status_value == busConstant.EmploymentStatusLOA
                                                    || lobjPersonEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.status_value == busConstant.EmploymentStatusLOAM
                                                    || lobjPersonEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.status_value == busConstant.EmploymentStatusFMLA))
                                                {
                                                    iclbUnProcessedPsPersonAddress.Add(lbusPsAddress);
                                                    if (lbusPsEmployment.GetEmploymentChangeRequestInReviewLOA())
                                                    {
                                                        utlError lobjError = null;
                                                        lobjError = AddError(10222, "LOA employment doesn't exist.");
                                                        lbusPsAddress.iarrErrors.Add(lobjError);

                                                        iclbPersonAddressInformativeErrors.Add(lbusPsAddress);
                                                        if (!lbusPsAddress.idictPsAddressError.ContainsKey(lbusPsAddress.icdoPsAddress.ps_address_id))
                                                            lbusPsAddress.idictPsAddressError.Add(lbusPsAddress.icdoPsAddress.ps_address_id, lbusPsAddress.iarrErrors);
                                                    }
                                                    continue;
                                                }
                                                CreateEmploymentChangeRequest(lbusWssEmploymentChangeRequest, lbusPsEmployment, lobjPersonEmployment);
                                                if (lobjPersonEmployment.icdoPersonEmployment.ps_empl_record_number == null)
                                                {
													//25926	FILLER: Remove PS Error Report Error of "Employment exists, but Empl Record Number is Different"
                                                    //if (lbusPsEmployment.icdoPsEmployment.empl_start_date == lobjPersonEmployment.icdoPersonEmployment.start_date)
                                                    //{
                                                    //    if (lbusPsEmployment.icdoPsEmployment.job_class_value == lobjPersonEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.job_class_value
                                                    //    && lbusPsEmployment.icdoPsEmployment.job_type_value == lobjPersonEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.type_value
                                                    //    && lbusPsEmployment.icdoPsEmployment.empl_status_value == lobjPersonEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.status_value
                                                    //    )
                                                    //    {
                                                    //        utlError lobjError = null;
                                                    //        lobjError = AddError(10214, "Employment exists, but Empl Record Number is different.");
                                                    //        lbusPsAddress.iarrErrors.Add(lobjError);
                                                    //        iclbUnProcessedPsPersonAddress.Add(lbusPsAddress);
                                                    //        iclbPersonAddressInformativeErrors.Add(lbusPsAddress);
                                                    //        if (!lbusPsAddress.idictPsAddressError.ContainsKey(lbusPsAddress.icdoPsAddress.ps_address_id))
                                                    //            lbusPsAddress.idictPsAddressError.Add(lbusPsAddress.icdoPsAddress.ps_address_id, lbusPsAddress.iarrErrors);
                                                    //        continue;
                                                    //    }
                                                    //}
                                                    //else
                                                    //{
                                                    //    if (lbusPsEmployment.icdoPsEmployment.job_class_value == lobjPersonEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.job_class_value
                                                    //   && lbusPsEmployment.icdoPsEmployment.job_type_value == lobjPersonEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.type_value
                                                    //   && lbusPsEmployment.icdoPsEmployment.empl_status_value == lobjPersonEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.status_value
                                                    //   )
                                                    //    {
                                                    //        utlError lobjError = null;
                                                    //        lobjError = AddError(10213, "Employment Record already exists with different date.");
                                                    //        lbusPsAddress.iarrErrors.Add(lobjError);
                                                    //        iclbUnProcessedPsPersonAddress.Add(lbusPsAddress);
                                                    //        iclbPersonAddressInformativeErrors.Add(lbusPsAddress);
                                                    //        if (!lbusPsAddress.idictPsAddressError.ContainsKey(lbusPsAddress.icdoPsAddress.ps_address_id))
                                                    //            lbusPsAddress.idictPsAddressError.Add(lbusPsAddress.icdoPsAddress.ps_address_id, lbusPsAddress.iarrErrors);
                                                    //        continue;
                                                    //    }
                                                    //}
                                                }
                                                ValidatePSRecords(lbusWssEmploymentChangeRequest, lbusPsAddress, utlPageMode.New);
                                            }
                                        }
                                    }
                                }

                            }
                            else
                            {
                                busPsPerson lbusPsPerson = LoadPSPersonBySSN(lbusPsEmployment.icdoPsEmployment.ssn);
                                if (lbusPsPerson != null)
                                {
                                    CreateMemberRecordRequest(lbusWssMemberRecordRequest, lbusPsPerson);
                                    busPsAddress lobjPsAddress = LoadPSPersonAddressBySSN(lbusPsAddress.icdoPsAddress.ssn);
                                    if (lbusPsAddress != null && lbusPsAddress.icdoPsAddress.processed_flag == busConstant.Flag_No)
                                    {
                                        lbusPsAddress.SetWssPersonAddressInfo(lbusWssMemberRecordRequest);
                                        if (lbusPsAddress.iblnIsAddressNotValidated)
                                        {
                                            utlError lobjError = null;
                                            lobjError = AddError(130, "Person Address is invalid.");
                                            lbusPsAddress.iarrErrors.Add(lobjError);
                                            iclbUnProcessedPsPersonAddress.Add(lbusPsAddress);
                                            iclbPersonAddressInformativeErrors.Add(lbusPsAddress);
                                            if (!lbusPsAddress.idictPsAddressError.ContainsKey(lbusPsAddress.icdoPsAddress.ps_address_id))
                                                lbusPsAddress.idictPsAddressError.Add(lbusPsAddress.icdoPsAddress.ps_address_id, lbusPsAddress.iarrErrors);
                                            continue;
                                        }
                                        # region Person Employment


                                        foreach (busPsEmployment lobjPsEmployment in lclbPsEmployment)
                                        {
                                            DataTable ldtbOrg = Select("cdoOrganization.GetOrganizationFromOrgCode", new object[1] { lbusPsEmployment.icdoPsEmployment.org_code });
                                            if (ldtbOrg != null)
                                                lbusPsEmployment.iintOrgId = Convert.ToInt32(ldtbOrg.Rows[0][enmOrganization.org_id.ToString()]);
                                            //Load person 
                                            lbusPsEmployment.ibusPerson = new busPerson();
                                            lbusPsEmployment.ibusPerson = lbusPsEmployment.ibusPerson.LoadPersonBySsn(lbusPsEmployment.icdoPsEmployment.ssn);
                                            lbusWssMemberRecordRequest.ibusOrganization = new busOrganization { icdoOrganization = new cdoOrganization() };
                                            lbusWssMemberRecordRequest.ibusOrganization = lobjOrganization;

                                            DataTable ldtbPersonEmp = Select("cdoPersonEmployment.GetEmploymentDetail", new object[1] { lbusPsEmployment.icdoPsEmployment.ssn });
                                            Collection<busPersonEmployment> lclbPersonEmpm = GetCollection<busPersonEmployment>(ldtbPersonEmp, "icdoPersonEmployment");
                                            if (lclbPersonEmpm.Count > 1)
                                            {
                                                //Dual Employment Scenario : 2 Sep
                                                busPersonEmployment lbusPersomEmp = lclbPersonEmployment.Where(lobj => lobj.icdoPersonEmployment.org_id == lbusPsEmployment.iintOrgId).FirstOrDefault();
                                                if (lbusPersomEmp != null)
                                                {
                                                    if (lbusPsEmployment.icdoPsEmployment.ps_empl_record_number == (lbusPersomEmp.icdoPersonEmployment.ps_empl_record_number == null ? "0" : lbusPersomEmp.icdoPersonEmployment.ps_empl_record_number))
                                                    {
                                                        if (lbusPsEmployment.icdoPsEmployment.loa_date_of_return != DateTime.MinValue
                                                            && !(lbusPersomEmp.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.status_value == busConstant.EmploymentStatusLOA
                                                            || lbusPersomEmp.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.status_value == busConstant.EmploymentStatusLOAM
                                                            || lbusPersomEmp.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.status_value == busConstant.EmploymentStatusFMLA))
                                                        {
                                                            iclbUnProcessedPsPersonAddress.Add(lbusPsAddress);
                                                            if (lbusPsEmployment.GetEmploymentChangeRequestInReviewLOA())
                                                            {
                                                                utlError lobjError = null;
                                                                lobjError = AddError(10222, "LOA employment doesn't exist.");
                                                                lbusPsAddress.iarrErrors.Add(lobjError);

                                                                iclbPersonAddressInformativeErrors.Add(lbusPsAddress);
                                                                if (!lbusPsAddress.idictPsAddressError.ContainsKey(lbusPsAddress.icdoPsAddress.ps_address_id))
                                                                    lbusPsAddress.idictPsAddressError.Add(lbusPsAddress.icdoPsAddress.ps_address_id, lbusPsAddress.iarrErrors);
                                                            }
                                                            continue;
                                                        }
                                                        CreateEmploymentChangeRequest(lbusWssEmploymentChangeRequest, lbusPsEmployment, lbusPersomEmp);
                                                        if (lbusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.change_type_value.IsNull())
                                                        {
                                                            if (lbusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.last_date_of_service == DateTime.MinValue)
                                                            {
                                                                //utlError lobjError = null;
                                                                //lobjError = AddError(10210, "Employment Record already exists.");
                                                                if (!iclbProcessedPsPersonAddress.Contains(lbusPsAddress))
                                                                    iclbProcessedPsPersonAddress.Add(lbusPsAddress);
                                                                continue;
                                                            }
                                                        }
                                                        ValidatePSRecords(lbusWssEmploymentChangeRequest, lbusPsAddress, utlPageMode.New);
                                                    }
                                                    else //Same Organization Different Record number
                                                    {
                                                        if (lbusPsAddress.ibusPsPerson != null)//lbusPsEmployment.ibusPsPerson != null && lbusPsEmployment.ibusPsAddress != null
                                                        {
                                                            lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.org_id = lbusPsEmployment.iintOrgId;
                                                            lbusPsEmployment.SetWssPersonEmploymentInfo(lbusWssMemberRecordRequest, lobjOrganization);
                                                            if (lbusPersomEmp.icdoPersonEmployment.ps_empl_record_number == null)
                                                            {
																//25926	FILLER: Remove PS Error Report Error of "Employment exists, but Empl Record Number is Different"
                                                                //if (lbusPsEmployment.icdoPsEmployment.empl_start_date == lbusPersomEmp.icdoPersonEmployment.start_date)
                                                                //{
                                                                //    if (lbusPsEmployment.icdoPsEmployment.job_class_value == lbusPersomEmp.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.job_class_value
                                                                //    && lbusPsEmployment.icdoPsEmployment.job_type_value == lbusPersomEmp.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.type_value
                                                                //    && lbusPsEmployment.icdoPsEmployment.empl_status_value == lbusPersomEmp.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.status_value
                                                                //    )
                                                                //    {
                                                                //        utlError lobjError = null;
                                                                //        lobjError = AddError(10214, "Employment exists, but Empl Record Number is different.");
                                                                //        lbusPsAddress.iarrErrors.Add(lobjError);
                                                                //        iclbUnProcessedPsPersonAddress.Add(lbusPsAddress);
                                                                //        iclbPersonAddressInformativeErrors.Add(lbusPsAddress);
                                                                //        if (!lbusPsAddress.idictPsAddressError.ContainsKey(lbusPsAddress.icdoPsAddress.ps_address_id))
                                                                //            lbusPsAddress.idictPsAddressError.Add(lbusPsAddress.icdoPsAddress.ps_address_id, lbusPsAddress.iarrErrors);
                                                                //        continue;
                                                                //    }
                                                                //}
                                                                //else
                                                                //{
                                                                //    if (lbusPsEmployment.icdoPsEmployment.job_class_value == lbusPersomEmp.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.job_class_value
                                                                //    && lbusPsEmployment.icdoPsEmployment.job_type_value == lbusPersomEmp.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.type_value
                                                                //    && lbusPsEmployment.icdoPsEmployment.empl_status_value == lbusPersomEmp.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.status_value
                                                                //    )
                                                                //    {
                                                                //        utlError lobjError = null;
                                                                //        lobjError = AddError(10210, "Employment Record already exists with different date.");
                                                                //        lbusPsAddress.iarrErrors.Add(lobjError);
                                                                //        iclbUnProcessedPsPersonAddress.Add(lbusPsAddress);
                                                                //        iclbPersonAddressInformativeErrors.Add(lbusPsAddress);
                                                                //        if (!lbusPsAddress.idictPsAddressError.ContainsKey(lbusPsAddress.icdoPsAddress.ps_address_id))
                                                                //            lbusPsAddress.idictPsAddressError.Add(lbusPsAddress.icdoPsAddress.ps_address_id, lbusPsAddress.iarrErrors);
                                                                //        continue;
                                                                //    }
                                                                //}
                                                            }
                                                            ValidatePSRecords(lbusWssMemberRecordRequest, lbusPsAddress, utlPageMode.New, iclbPersonAddressInformativeErrors);
                                                        }
                                                        else
                                                        {
                                                            if (lbusPsEmployment.icdoPsEmployment.loa_date_of_return != DateTime.MinValue
                                                                && !(lbusPersomEmp.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.status_value == busConstant.EmploymentStatusLOA
                                                                || lbusPersomEmp.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.status_value == busConstant.EmploymentStatusLOAM
                                                                || lbusPersomEmp.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.status_value == busConstant.EmploymentStatusFMLA))
                                                            {
                                                                iclbUnProcessedPsPersonAddress.Add(lbusPsAddress);
                                                                if (lbusPsEmployment.GetEmploymentChangeRequestInReviewLOA())
                                                                {
                                                                    utlError lobjError = null;
                                                                    lobjError = AddError(10222, "LOA employment doesn't exist.");
                                                                    lbusPsAddress.iarrErrors.Add(lobjError);

                                                                    iclbPersonAddressInformativeErrors.Add(lbusPsAddress);
                                                                    if (!lbusPsAddress.idictPsAddressError.ContainsKey(lbusPsAddress.icdoPsAddress.ps_address_id))
                                                                        lbusPsAddress.idictPsAddressError.Add(lbusPsAddress.icdoPsAddress.ps_address_id, lbusPsAddress.iarrErrors);
                                                                }
                                                                continue;
                                                            }
                                                            CreateEmploymentChangeRequest(lbusWssEmploymentChangeRequest, lbusPsEmployment, lbusPersomEmp);
                                                            if (lbusPersomEmp.icdoPersonEmployment.ps_empl_record_number == null)
                                                            {
																//25926	FILLER: Remove PS Error Report Error of "Employment exists, but Empl Record Number is Different"
                                                                //if (lbusPsEmployment.icdoPsEmployment.empl_start_date == lbusPersomEmp.icdoPersonEmployment.start_date)
                                                                //{
                                                                //    if (lbusPsEmployment.icdoPsEmployment.job_class_value == lbusPersomEmp.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.job_class_value
                                                                //    && lbusPsEmployment.icdoPsEmployment.job_type_value == lbusPersomEmp.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.type_value
                                                                //    && lbusPsEmployment.icdoPsEmployment.empl_status_value == lbusPersomEmp.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.status_value
                                                                //    )
                                                                //    {
                                                                //        utlError lobjError = null;
                                                                //        lobjError = AddError(10214, "Employment exists, but Empl Record Number is different.");
                                                                //        lbusPsAddress.iarrErrors.Add(lobjError);
                                                                //        iclbUnProcessedPsPersonAddress.Add(lbusPsAddress);
                                                                //        iclbPersonAddressInformativeErrors.Add(lbusPsAddress);
                                                                //        if (!lbusPsAddress.idictPsAddressError.ContainsKey(lbusPsAddress.icdoPsAddress.ps_address_id))
                                                                //            lbusPsAddress.idictPsAddressError.Add(lbusPsAddress.icdoPsAddress.ps_address_id, lbusPsAddress.iarrErrors);
                                                                //        continue;
                                                                //    }
                                                                //}
                                                                //else
                                                                //{
                                                                //    if (lbusPsEmployment.icdoPsEmployment.job_class_value == lbusPersomEmp.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.job_class_value
                                                                //    && lbusPsEmployment.icdoPsEmployment.job_type_value == lbusPersomEmp.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.type_value
                                                                //    && lbusPsEmployment.icdoPsEmployment.empl_status_value == lbusPersomEmp.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.status_value
                                                                //    )
                                                                //    {
                                                                //        utlError lobjError = null;
                                                                //        lobjError = AddError(10213, "Employment Record already exists with different date.");
                                                                //        lbusPsAddress.iarrErrors.Add(lobjError);
                                                                //        iclbUnProcessedPsPersonAddress.Add(lbusPsAddress);
                                                                //        iclbPersonAddressInformativeErrors.Add(lbusPsAddress);
                                                                //        if (!lbusPsAddress.idictPsAddressError.ContainsKey(lbusPsAddress.icdoPsAddress.ps_address_id))
                                                                //            lbusPsAddress.idictPsAddressError.Add(lbusPsAddress.icdoPsAddress.ps_address_id, lbusPsAddress.iarrErrors);
                                                                //        continue;
                                                                //    }
                                                                //}
                                                            }
                                                            ValidatePSRecords(lbusWssEmploymentChangeRequest, lbusPsAddress, utlPageMode.New);
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.org_id = lbusPsEmployment.iintOrgId;
                                                    lbusPsEmployment.SetWssPersonEmploymentInfo(lbusWssMemberRecordRequest, lobjOrganization);
                                                    ValidatePSRecords(lbusWssMemberRecordRequest, lbusPsAddress, utlPageMode.New, iclbPersonAddressInformativeErrors);
                                                }
                                            }
                                            else if (lclbPersonEmployment.Count == 1)
                                            {
                                                foreach (busPersonEmployment lobjPersonEmployment in lclbPersonEmployment)
                                                {
                                                    lobjPersonEmployment.LoadLatestPersonEmploymentDetail(); //Added 15 october
                                                    if (lbusPsEmployment.iintOrgId != lobjPersonEmployment.icdoPersonEmployment.org_id)
                                                    {
                                                        lbusPsEmployment.SetWssPersonEmploymentInfo(lbusWssMemberRecordRequest, lobjOrganization);
                                                        ValidatePSRecords(lbusWssMemberRecordRequest, lbusPsAddress, utlPageMode.New, iclbPersonAddressInformativeErrors);
                                                    }
                                                    else
                                                    {
                                                        //Same Organization Same Record Number
                                                        if (lbusPsEmployment.icdoPsEmployment.ps_empl_record_number == (lobjPersonEmployment.icdoPersonEmployment.ps_empl_record_number == null ? "0" : lobjPersonEmployment.icdoPersonEmployment.ps_empl_record_number))
                                                        {
                                                            if (lbusPsEmployment.icdoPsEmployment.loa_date_of_return != DateTime.MinValue
                                                                && !(lobjPersonEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.status_value == busConstant.EmploymentStatusLOA
                                                                || lobjPersonEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.status_value == busConstant.EmploymentStatusLOAM
                                                                || lobjPersonEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.status_value == busConstant.EmploymentStatusFMLA))
                                                            {
                                                                iclbUnProcessedPsPersonAddress.Add(lbusPsAddress);
                                                                if (lbusPsEmployment.GetEmploymentChangeRequestInReviewLOA())
                                                                {
                                                                    utlError lobjError = null;
                                                                    lobjError = AddError(10222, "LOA employment doesn't exist.");
                                                                    lbusPsAddress.iarrErrors.Add(lobjError);

                                                                    iclbPersonAddressInformativeErrors.Add(lbusPsAddress);
                                                                    if (!lbusPsAddress.idictPsAddressError.ContainsKey(lbusPsAddress.icdoPsAddress.ps_address_id))
                                                                        lbusPsAddress.idictPsAddressError.Add(lbusPsAddress.icdoPsAddress.ps_address_id, lbusPsAddress.iarrErrors);
                                                                }
                                                                continue;
                                                            }
                                                            CreateEmploymentChangeRequest(lbusWssEmploymentChangeRequest, lbusPsEmployment, lobjPersonEmployment);
                                                            if (lbusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.change_type_value.IsNull())
                                                            {
                                                                if (lbusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.last_date_of_service == DateTime.MinValue)
                                                                {
                                                                    //utlError lobjError = null;
                                                                    //lobjError = AddError(10210, "Employment Record already exists.");
                                                                    if (!iclbProcessedPsPersonAddress.Contains(lbusPsAddress))
                                                                        iclbProcessedPsPersonAddress.Add(lbusPsAddress);
                                                                    continue;
                                                                }
                                                            }
                                                            ValidatePSRecords(lbusWssEmploymentChangeRequest, lbusPsAddress, utlPageMode.New);
                                                        }
                                                        else //Same Organization Different Record Number 
                                                        {
                                                            if (lbusPsAddress.ibusPsPerson != null) // lbusPsEmployment.ibusPsPerson != null && lbusPsEmployment.ibusPsAddress != null
                                                            {
                                                                lbusPsEmployment.SetWssPersonEmploymentInfo(lbusWssMemberRecordRequest, lobjOrganization);
                                                                if (lobjPersonEmployment.icdoPersonEmployment.ps_empl_record_number == null)
                                                                {
																	//25926	FILLER: Remove PS Error Report Error of "Employment exists, but Empl Record Number is Different"
                                                                    //if (lbusPsEmployment.icdoPsEmployment.empl_start_date == lobjPersonEmployment.icdoPersonEmployment.start_date)
                                                                    //{
                                                                    //    if (lbusPsEmployment.icdoPsEmployment.job_class_value == lobjPersonEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.job_class_value
                                                                    //    && lbusPsEmployment.icdoPsEmployment.job_type_value == lobjPersonEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.type_value
                                                                    //    && lbusPsEmployment.icdoPsEmployment.empl_status_value == lobjPersonEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.status_value
                                                                    //    )
                                                                    //    {
                                                                    //        utlError lobjError = null;
                                                                    //        lobjError = AddError(10214, "Employment exists, but Empl Record Number is different.");
                                                                    //        lbusPsAddress.iarrErrors.Add(lobjError);
                                                                    //        iclbUnProcessedPsPersonAddress.Add(lbusPsAddress);
                                                                    //        iclbPersonAddressInformativeErrors.Add(lbusPsAddress);
                                                                    //        if (!lbusPsAddress.idictPsAddressError.ContainsKey(lbusPsAddress.icdoPsAddress.ps_address_id))
                                                                    //            lbusPsAddress.idictPsAddressError.Add(lbusPsAddress.icdoPsAddress.ps_address_id, lbusPsAddress.iarrErrors);
                                                                    //        continue;
                                                                    //    }
                                                                    //}
                                                                    //else
                                                                    //{
                                                                    //    if (lbusPsEmployment.icdoPsEmployment.job_class_value == lobjPersonEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.job_class_value
                                                                    //    && lbusPsEmployment.icdoPsEmployment.job_type_value == lobjPersonEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.type_value
                                                                    //    && lbusPsEmployment.icdoPsEmployment.empl_status_value == lobjPersonEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.status_value
                                                                    //    )
                                                                    //    {
                                                                    //        utlError lobjError = null;
                                                                    //        lobjError = AddError(10210, "Employment Record already exists.");
                                                                    //        lbusPsAddress.iarrErrors.Add(lobjError);
                                                                    //        iclbUnProcessedPsPersonAddress.Add(lbusPsAddress);
                                                                    //        iclbPersonAddressInformativeErrors.Add(lbusPsAddress);
                                                                    //        if (!lbusPsAddress.idictPsAddressError.ContainsKey(lbusPsAddress.icdoPsAddress.ps_address_id))
                                                                    //            lbusPsAddress.idictPsAddressError.Add(lbusPsAddress.icdoPsAddress.ps_address_id, lbusPsAddress.iarrErrors);
                                                                    //        continue;
                                                                    //    }
                                                                    //}
                                                                }
                                                                ValidatePSRecords(lbusWssMemberRecordRequest, lbusPsAddress, utlPageMode.New, iclbPersonAddressInformativeErrors);
                                                            }
                                                            else
                                                            {
                                                                if (lbusPsEmployment.icdoPsEmployment.loa_date_of_return != DateTime.MinValue
                                                                    && !(lobjPersonEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.status_value == busConstant.EmploymentStatusLOA
                                                                    || lobjPersonEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.status_value == busConstant.EmploymentStatusLOAM
                                                                    || lobjPersonEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.status_value == busConstant.EmploymentStatusFMLA))
                                                                {
                                                                    iclbUnProcessedPsPersonAddress.Add(lbusPsAddress);
                                                                    if (lbusPsEmployment.GetEmploymentChangeRequestInReviewLOA())
                                                                    {
                                                                        utlError lobjError = null;
                                                                        lobjError = AddError(10222, "LOA employment doesn't exist.");
                                                                        lbusPsAddress.iarrErrors.Add(lobjError);

                                                                        iclbPersonAddressInformativeErrors.Add(lbusPsAddress);
                                                                        if (!lbusPsAddress.idictPsAddressError.ContainsKey(lbusPsAddress.icdoPsAddress.ps_address_id))
                                                                            lbusPsAddress.idictPsAddressError.Add(lbusPsAddress.icdoPsAddress.ps_address_id, lbusPsAddress.iarrErrors);
                                                                    }
                                                                    continue;
                                                                }
                                                                CreateEmploymentChangeRequest(lbusWssEmploymentChangeRequest, lbusPsEmployment, lobjPersonEmployment);
                                                                if (lobjPersonEmployment.icdoPersonEmployment.ps_empl_record_number == null)
                                                                {
																	//25926	FILLER: Remove PS Error Report Error of "Employment exists, but Empl Record Number is Different"
                                                                    //if (lbusPsEmployment.icdoPsEmployment.empl_start_date == lobjPersonEmployment.icdoPersonEmployment.start_date)
                                                                    //{
                                                                    //    if (lbusPsEmployment.icdoPsEmployment.job_class_value == lobjPersonEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.job_class_value
                                                                    //    && lbusPsEmployment.icdoPsEmployment.job_type_value == lobjPersonEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.type_value
                                                                    //    && lbusPsEmployment.icdoPsEmployment.empl_status_value == lobjPersonEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.status_value
                                                                    //    )
                                                                    //    {
                                                                    //        utlError lobjError = null;
                                                                    //        lobjError = AddError(10214, "Employment exists, but Empl Record Number is different.");
                                                                    //        lbusPsAddress.iarrErrors.Add(lobjError);
                                                                    //        iclbUnProcessedPsPersonAddress.Add(lbusPsAddress);
                                                                    //        iclbPersonAddressInformativeErrors.Add(lbusPsAddress);
                                                                    //        if (!lbusPsAddress.idictPsAddressError.ContainsKey(lbusPsAddress.icdoPsAddress.ps_address_id))
                                                                    //            lbusPsAddress.idictPsAddressError.Add(lbusPsAddress.icdoPsAddress.ps_address_id, lbusPsAddress.iarrErrors);
                                                                    //        continue;
                                                                    //    }
                                                                    //}
                                                                    //else
                                                                    //{
                                                                    //    if (lbusPsEmployment.icdoPsEmployment.job_class_value == lobjPersonEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.job_class_value
                                                                    //    && lbusPsEmployment.icdoPsEmployment.job_type_value == lobjPersonEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.type_value
                                                                    //    && lbusPsEmployment.icdoPsEmployment.empl_status_value == lobjPersonEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.status_value
                                                                    //    )
                                                                    //    {
                                                                    //        utlError lobjError = null;
                                                                    //        lobjError = AddError(10210, "Employment Record already exists.");
                                                                    //        lbusPsAddress.iarrErrors.Add(lobjError);
                                                                    //        iclbUnProcessedPsPersonAddress.Add(lbusPsAddress);
                                                                    //        iclbPersonAddressInformativeErrors.Add(lbusPsAddress);
                                                                    //        if (!lbusPsAddress.idictPsAddressError.ContainsKey(lbusPsAddress.icdoPsAddress.ps_address_id))
                                                                    //            lbusPsAddress.idictPsAddressError.Add(lbusPsAddress.icdoPsAddress.ps_address_id, lbusPsAddress.iarrErrors);
                                                                    //        continue;
                                                                    //    }
                                                                    //}
                                                                }
                                                                ValidatePSRecords(lbusWssEmploymentChangeRequest, lbusPsAddress, utlPageMode.New);
                                                            }
                                                        }
                                                    }

                                                }
                                            }
                                            else
                                            {
                                                if (lbusPsEmployment.icdoPsEmployment.empl_end_date != DateTime.MinValue)
                                                {
                                                    utlError lobjError = null;
                                                    lobjError = AddError(10211, "Terminated Employment already exists.");
                                                    lbusPsAddress.iarrErrors.Add(lobjError);
                                                    iclbUnProcessedPsPersonAddress.Add(lbusPsAddress);
                                                    iclbPersonAddressInformativeErrors.Add(lbusPsAddress);
                                                    if (!lbusPsEmployment.idictPsEmploymentError.ContainsKey(lbusPsAddress.icdoPsAddress.ps_address_id))
                                                        lbusPsEmployment.idictPsEmploymentError.Add(lbusPsAddress.icdoPsAddress.ps_address_id, lbusPsAddress.iarrErrors);
                                                    continue;
                                                }

                                                lbusPsEmployment.SetWssPersonEmploymentInfo(lbusWssMemberRecordRequest, lobjOrganization);
                                                ValidatePSRecords(lbusWssMemberRecordRequest, lbusPsAddress, utlPageMode.New, iclbPersonAddressInformativeErrors);
                                            }

                                        }
                                        # endregion
                                    }
                                    else
                                    {
                                        utlError lobjError = null;
                                        lobjError = AddError(10207, "PS Address Record does not exist.");
                                        lbusPsAddress.iarrErrors.Add(lobjError);
                                        iclbUnProcessedPsPersonAddress.Add(lbusPsAddress);
                                        if (!lbusPsAddress.idictPsAddressError.ContainsKey(lbusPsAddress.icdoPsAddress.ps_address_id))
                                            lbusPsAddress.idictPsAddressError.Add(lbusPsAddress.icdoPsAddress.ps_address_id, lbusPsAddress.iarrErrors);
                                        continue;
                                    }
                                }
                                else
                                {
                                    utlError lobjError = null;
                                    lobjError = AddError(10206, "PS Employment Record does not exist.");
                                    lbusPsAddress.iarrErrors.Add(lobjError);
                                    iclbUnProcessedPsPersonAddress.Add(lbusPsAddress);
                                    if (!lbusPsAddress.idictPsAddressError.ContainsKey(lbusPsAddress.icdoPsAddress.ps_address_id))
                                        lbusPsAddress.idictPsAddressError.Add(lbusPsAddress.icdoPsAddress.ps_address_id, lbusPsAddress.iarrErrors);
                                    continue;
                                }
                            }
                        }
                    }
                    #endregion
                }
            }
        }

        private void CreateEmploymentChangeRequest(busWssEmploymentChangeRequest abusWssEmploymentChangeRequest, busPsEmployment lbusPsEmployment, busPersonEmployment lobjPersonEmployment)
        {

            lobjPersonEmployment.LoadLatestPersonEmploymentDetail();
            abusWssEmploymentChangeRequest.ibusPersonEmployment = lobjPersonEmployment;
            abusWssEmploymentChangeRequest.ibusPersonEmploymentDetail = lobjPersonEmployment.ibusLatestEmploymentDetail;
            abusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.ienuObjectState = ObjectState.Insert;
            abusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.ps_empl_record_number = lbusPsEmployment.icdoPsEmployment.ps_empl_record_number;
            abusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.peoplesoft_id = lbusPsEmployment.icdoPsEmployment.peoplesoft_id;
            //Need to confirm to which filed we have to bind in sgt_wss_employment_change_request 
            if (lbusPsEmployment.icdoPsEmployment.job_class_value == busConstant.PersonJobClassStateElectedOfficial)
                abusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.term_begin_date = lbusPsEmployment.icdoPsEmployment.term_begin_date;
            abusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.person_id = lbusPsEmployment.ibusPerson.icdoPerson.person_id;
            abusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.org_id = lobjPersonEmployment.icdoPersonEmployment.org_id;

            //*****************Need to confirm with Maik on what basis we shall assign the change_type_value 
            //*****************also need the confirmation to add job_class_change_effective_date and employment_type_change_effective_date

            if (lbusPsEmployment.icdoPsEmployment.empl_end_date == DateTime.MinValue)
            {
                if (lbusPsEmployment.icdoPsEmployment.job_class_value != null && lbusPsEmployment.icdoPsEmployment.job_class_value.Trim().IsNotNullOrEmpty() && lbusPsEmployment.icdoPsEmployment.job_class_value != lobjPersonEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.job_class_value
                    //&& lbusPsEmployment.icdoPsEmployment.term_begin_date != DateTime.MinValue 
                   && lbusPsEmployment.icdoPsEmployment.empl_start_date != DateTime.MinValue)
                {
                    abusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.change_type_value = busConstant.EmploymentChangeRequestChangeTypeClassification;
                    abusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.job_class_change_effective_date = lbusPsEmployment.icdoPsEmployment.empl_start_date;
                    abusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.employment_type_change_effective_date = lbusPsEmployment.icdoPsEmployment.empl_start_date;
                    iblnEmploymentChangeRequestChangeTypeClassification = true;
                }
                if (((lbusPsEmployment.icdoPsEmployment.job_type_value != null && lbusPsEmployment.icdoPsEmployment.job_type_value.Trim().IsNotNullOrEmpty() && lbusPsEmployment.icdoPsEmployment.job_type_value != lobjPersonEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.type_value)
                    && (lbusPsEmployment.icdoPsEmployment.empl_status_value != null && lbusPsEmployment.icdoPsEmployment.empl_status_value.Trim().IsNotNullOrEmpty() && lbusPsEmployment.icdoPsEmployment.empl_start_date != DateTime.MinValue))
                    || (lobjPersonEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.hourly_value != lbusPsEmployment.icdoPsEmployment.hourly_value
                    || lobjPersonEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.status_value != lbusPsEmployment.icdoPsEmployment.empl_status_value))
                {
                    abusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.change_type_value = busConstant.EmploymentChangeRequestChangeTypeEmployment;
                    abusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.employment_type_change_effective_date = lbusPsEmployment.icdoPsEmployment.empl_start_date;
                    iblnEmploymentChangeRequestChangeTypeEmployment = true;
                }
                if ((lbusPsEmployment.icdoPsEmployment.loa_start_date != DateTime.MinValue && lbusPsEmployment.icdoPsEmployment.empl_status_value == busConstant.EmploymentStatusLOAM)
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
                if ((lbusPsEmployment.icdoPsEmployment.loa_start_date != DateTime.MinValue && lbusPsEmployment.icdoPsEmployment.empl_status_value == busConstant.EmploymentStatusFMLA)
                    || (lbusPsEmployment.icdoPsEmployment.loa_date_of_return != DateTime.MinValue && lobjPersonEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.status_value == busConstant.EmploymentStatusFMLA))
                {
                    abusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.change_type_value = busConstant.EmploymentStatusFMLA;
                    iblnEmploymentChangeRequestChangeTypeFMLA = true;
                }
            }

           

            abusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.hourly_value = lbusPsEmployment.icdoPsEmployment.hourly_value;
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

        private void CreateMemberRecordRequest(busWssMemberRecordRequest lbusWssMemberRecordRequest, busPsPerson abusPsPerson = null,busPerson abusperson= null)
        {
            if (abusPsPerson != null)
            {
                lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.name_prefix_value = abusPsPerson.icdoPsPerson.name_prefix_value;
                lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.first_name = abusPsPerson.icdoPsPerson.first_name;
                lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.middle_name = abusPsPerson.icdoPsPerson.middle_name;
                lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.last_name = abusPsPerson.icdoPsPerson.last_name;
                lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.name_suffix_value = abusPsPerson.icdoPsPerson.name_suffix_value;
                lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.date_of_birth = abusPsPerson.icdoPsPerson.date_of_birth;
                lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.gender_value = abusPsPerson.icdoPsPerson.gender_value;
                lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.ssn = abusPsPerson.icdoPsPerson.ssn;
                lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.marital_status_value = abusPsPerson.icdoPsPerson.marital_status_value;
                lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.marital_status_date = abusPsPerson.icdoPsPerson.ms_change_date; //Need to refresh cdo 
                lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.work_phone_no = abusPsPerson.icdoPsPerson.work_phone_no;
                lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.work_phone_ext = abusPsPerson.icdoPsPerson.work_phone_ext;//Need to  refresh cdo
                lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.cell_phone_no = abusPsPerson.icdoPsPerson.cell_phone_no;
                lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.home_phone_no = abusPsPerson.icdoPsPerson.home_phone_no;
                lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.peoplesoft_id = Convert.ToInt32(abusPsPerson.icdoPsPerson.peoplesoft_id);
                lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.created_by = "PERSLinkBatch";
                lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.created_date = DateTime.Now;
                lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.modified_by = "PERSLinkBatch";
                lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.modified_date = DateTime.Now;
                lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.ps_initiated_flag = busConstant.Flag_Yes;
                lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.status_value = busConstant.EmploymentChangeRequestStatusPendingAutoPosting;
            }
            else
            {
                lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.person_id = abusperson.icdoPerson.person_id;
                lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.first_name = abusperson.icdoPerson.first_name;
                lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.last_name = abusperson.icdoPerson.last_name;
                lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.middle_name = abusperson.icdoPerson.middle_name;
                lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.name_prefix_value = abusperson.icdoPerson.name_prefix_value;
                lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.name_suffix_value = abusperson.icdoPerson.name_suffix_value;
                lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.date_of_birth = abusperson.icdoPerson.date_of_birth;
                lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.ssn = abusperson.icdoPerson.ssn;
                lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.gender_value = abusperson.icdoPerson.gender_value;
                lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.marital_status_value = abusperson.icdoPerson.marital_status_value;
                lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.home_phone_no = abusperson.icdoPerson.home_phone_no;
                lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.work_phone_no = abusperson.icdoPerson.work_phone_no;
                lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.cell_phone_no = abusperson.icdoPerson.cell_phone_no;
                lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.peoplesoft_id = Convert.ToInt32(abusperson.icdoPerson.peoplesoft_id);
                lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.ps_initiated_flag = busConstant.Flag_Yes;
                lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.status_value = busConstant.EmploymentChangeRequestStatusPendingAutoPosting;
                lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.created_by = "PERSLinkBatch";
                lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.created_date = DateTime.Now;
                lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.modified_by = "PERSLinkBatch";
                lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.modified_date = DateTime.Now;
                
            }
        }

        public busPsPerson LoadPSPersonBySSN(string astrssn)
        {
            Collection<busPsPerson> lclbPsPerson = new Collection<busPsPerson>();
            DataTable ldtbPsPerson = DBFunction.DBSelect("cdoPsPerson.GetPsPersonBySSN", new string[1] { astrssn },
                iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            if (ldtbPsPerson.Rows.Count > 0)
            {
                lclbPsPerson = GetCollection<busPsPerson>(ldtbPsPerson, "icdoPsPerson");
                return lclbPsPerson[0];
            }
            return null;
        }

        public Collection<busPsEmployment> LoadPsEmploymentBySSN(string astrssn)
        {
            Collection<busPsEmployment> lclbPsEmployment = new Collection<busPsEmployment>();
            DataTable ldtbPsEmployment = DBFunction.DBSelect("cdoPsEmployment.GetPsEmploymentBySSN", new string[1] { astrssn },
                iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            if (ldtbPsEmployment.Rows.Count > 0)            
                lclbPsEmployment = GetCollection<busPsEmployment>(ldtbPsEmployment, "icdoPsEmployment");
            return lclbPsEmployment;
        }

        private busPsAddress LoadPSPersonAddressBySSN(string astrssn)
        {
            Collection<busPsAddress> lclbPsPerson = new Collection<busPsAddress>();
            DataTable ldtbPsAddress = DBFunction.DBSelect("cdoPsAddress.GetPsAddressBySSN", new string[1] { astrssn },
                iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            if (ldtbPsAddress.Rows.Count > 0)
            {
                lclbPsPerson = GetCollection<busPsAddress>(ldtbPsAddress, "icdoPsAddress");
                return lclbPsPerson[0];
            }
            return null;

        }

        private void ValidatePSRecords(busWssMemberRecordRequest aobjWssMemberRecordRequest, busPsAddress aobjPsAddress, utlPageMode mode, Collection<busPsAddress> iclbPersonAddressInformativeErrors)
        {
            aobjWssMemberRecordRequest.iblnIsFromPS = true;
            aobjWssMemberRecordRequest.BeforeValidate(utlPageMode.Update);
            aobjWssMemberRecordRequest.ValidateHardErrors(utlPageMode.Update);
            if (aobjWssMemberRecordRequest.iarrErrors.Count == 1 && aobjWssMemberRecordRequest.iblnIsMemberContributing)
            {
               
                if (!aobjPsAddress.idictPsAddressError.ContainsKey(aobjPsAddress.icdoPsAddress.ps_address_id))
                    aobjPsAddress.idictPsAddressError.Add(aobjPsAddress.icdoPsAddress.ps_address_id, aobjWssMemberRecordRequest.iarrErrors);
                iclbUnProcessedPsPersonAddress.Add(aobjPsAddress);
                iclbPersonAddressInformativeErrors.Add(aobjPsAddress);
            }
            else
            {
                if (aobjWssMemberRecordRequest.iarrErrors.Count > 0)
                {
                    if (!iclbUnProcessedPsPersonAddress.Contains(aobjPsAddress))
                    {
                        iclbUnProcessedPsPersonAddress.Add(aobjPsAddress);
                        if (!aobjPsAddress.idictPsAddressError.ContainsKey(aobjPsAddress.icdoPsAddress.ps_address_id))
                            aobjPsAddress.idictPsAddressError.Add(aobjPsAddress.icdoPsAddress.ps_address_id, aobjWssMemberRecordRequest.iarrErrors);
                    }
                }
                else
                {
                    DataTable ltbMemberRecordRequest = Select("cdoPsEmployment.GetMemberRecordRequestBySSN", new object[1] { aobjPsAddress.icdoPsAddress.ssn });
                    if (ltbMemberRecordRequest.Rows.Count == 0)
                    {
                        aobjPsAddress.icdoPsAddress.processed_flag = busConstant.Flag_Yes;
                        aobjWssMemberRecordRequest.BeforePersistChanges();
                        aobjWssMemberRecordRequest.PersistChanges();
                        aobjWssMemberRecordRequest.ValidateSoftErrors();
                        aobjWssMemberRecordRequest.AfterPersistChanges();
                        if (!iclbProcessedPsPersonAddress.Contains(aobjPsAddress))
                        {
                            iclbProcessedPsPersonAddress.Add(aobjPsAddress);
                        }
                    }
                }
            }
        }

        private void ValidatePSRecords(busWssEmploymentChangeRequest aobjWssEmploymentChangeRequest, busPsAddress aobjPsAddress, utlPageMode mode)
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

            if (aobjWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.last_date_of_service != DateTime.MinValue)
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
              
                if (!iclbUnProcessedPsPersonAddress.Contains(aobjPsAddress))
                {
                    iclbUnProcessedPsPersonAddress.Add(aobjPsAddress);
                    if (!aobjPsAddress.idictPsAddressError.ContainsKey(aobjPsAddress.icdoPsAddress.ps_address_id))
                        aobjPsAddress.idictPsAddressError.Add(aobjPsAddress.icdoPsAddress.ps_address_id, aobjWssEmploymentChangeRequest.iarrErrors);
                }
            }
            else
            {
                DataTable ltbEmploymentChangeRequest = Select("cdoPsEmployment.GetEmploymentChangeRequest", new object[1] { 
                                                            busGlobalFunctions.GetPersonIDBySSN(aobjPsAddress.icdoPsAddress.ssn) });
                if (ltbEmploymentChangeRequest.Rows.Count == 0)
                {
                    aobjPsAddress.icdoPsAddress.processed_flag = busConstant.Flag_Yes;
                    aobjWssEmploymentChangeRequest.BeforePersistChanges();
                    aobjWssEmploymentChangeRequest.PersistChanges();
                    aobjWssEmploymentChangeRequest.ValidateSoftErrors();
                    aobjWssEmploymentChangeRequest.AfterPersistChanges();
                    if (!iclbProcessedPsPersonAddress.Contains(aobjPsAddress))
                    {
                        iclbProcessedPsPersonAddress.Add(aobjPsAddress);
                    }
                }
            }
        }
        public void SetWssPersonAddressInfo(busWssMemberRecordRequest lbusWssMemberRecordRequest)
        {
            
            //int lintupdateSeq = 0;
            if (lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.member_record_request_id > 0)
            {
                lbusWssMemberRecordRequest.icdoWssPersonAddress.addr_line_1 = icdoPsAddress.addr_line_1;
                lbusWssMemberRecordRequest.icdoWssPersonAddress.addr_line_2 = icdoPsAddress.addr_line_2;
                lbusWssMemberRecordRequest.icdoWssPersonAddress.addr_city = icdoPsAddress.addr_city;
                lbusWssMemberRecordRequest.icdoWssPersonAddress.addr_state_value = icdoPsAddress.addr_state_value;
                lbusWssMemberRecordRequest.icdoWssPersonAddress.addr_country_value = busGlobalFunctions.GetCodeValueDetailsfromData1(151, icdoPsAddress.addr_country_value, iobjPassInfo);
                lbusWssMemberRecordRequest.icdoWssPersonAddress.addr_zip_code = icdoPsAddress.addr_zip_code;
                lbusWssMemberRecordRequest.icdoWssPersonAddress.addr_zip_4_code = icdoPsAddress.addr_zip_4_code;
                if (icdoPsAddress.addr_start_date <= DateTime.Now)
                    lbusWssMemberRecordRequest.icdoWssPersonAddress.addr_effective_date = DateTime.Now;
                else
                    lbusWssMemberRecordRequest.icdoWssPersonAddress.addr_effective_date = icdoPsAddress.addr_start_date;
                //if(iblnAddressChanged) // Check if Address Updated in this batch 
                    //lbusWssMemberRecordRequest.icdoWssPersonAddress.address_updated_in_ps_batch = busConstant.Flag_Yes;
                
            }
            else
            {
                lbusWssMemberRecordRequest.icdoWssPersonAddress = new cdoWssPersonAddress();
                lbusWssMemberRecordRequest.icdoWssPersonAddress.addr_line_1 = icdoPsAddress.addr_line_1;
                lbusWssMemberRecordRequest.icdoWssPersonAddress.addr_line_2 = icdoPsAddress.addr_line_2;
                lbusWssMemberRecordRequest.icdoWssPersonAddress.addr_city = icdoPsAddress.addr_city;
                lbusWssMemberRecordRequest.icdoWssPersonAddress.addr_state_value = icdoPsAddress.addr_state_value;
                lbusWssMemberRecordRequest.icdoWssPersonAddress.addr_country_value = busGlobalFunctions.GetCodeValueDetailsfromData1(151, icdoPsAddress.addr_country_value, iobjPassInfo);
                lbusWssMemberRecordRequest.icdoWssPersonAddress.addr_zip_code = icdoPsAddress.addr_zip_code;
                lbusWssMemberRecordRequest.icdoWssPersonAddress.addr_zip_4_code = icdoPsAddress.addr_zip_4_code;
                if (icdoPsAddress.addr_start_date <= DateTime.Now)
                    lbusWssMemberRecordRequest.icdoWssPersonAddress.addr_effective_date = DateTime.Now;
                else
                    lbusWssMemberRecordRequest.icdoWssPersonAddress.addr_effective_date = icdoPsAddress.addr_start_date;
                //if (iblnAddressChanged) // Check if Address Updated in this batch 
                    //lbusWssMemberRecordRequest.icdoWssPersonAddress.address_updated_in_ps_batch = busConstant.Flag_Yes;
            }
            VerifyAddressUsingUSPS(lbusWssMemberRecordRequest);

        }


		// Validate address - As per mail from Maik dated 03/20/2014, Issue#1
        public void VerifyAddressUsingUSPS(busWssMemberRecordRequest abusWssMemberRecordRequest)
        {
            cdoWebServiceAddress _lobjcdoWebServiceAddress = new cdoWebServiceAddress();
            _lobjcdoWebServiceAddress.addr_line_1 = abusWssMemberRecordRequest.icdoWssPersonAddress.addr_line_1;
            _lobjcdoWebServiceAddress.addr_line_2 = abusWssMemberRecordRequest.icdoWssPersonAddress.addr_line_2;
            _lobjcdoWebServiceAddress.addr_city = abusWssMemberRecordRequest.icdoWssPersonAddress.addr_city;
            _lobjcdoWebServiceAddress.addr_state_value = abusWssMemberRecordRequest.icdoWssPersonAddress.addr_state_value;
            _lobjcdoWebServiceAddress.addr_zip_code = abusWssMemberRecordRequest.icdoWssPersonAddress.addr_zip_code;
            _lobjcdoWebServiceAddress.addr_zip_4_code = abusWssMemberRecordRequest.icdoWssPersonAddress.addr_zip_4_code;
            cdoWebServiceAddress _lobjcdoWebServiceAddressResult = busGlobalFunctions.ValidateWebServiceAddress(_lobjcdoWebServiceAddress);

            iblnIsAddressNotValidated = false;

            if (_lobjcdoWebServiceAddressResult.address_validate_flag != busConstant.Flag_No)
            {
                abusWssMemberRecordRequest.icdoWssPersonAddress.addr_line_1 = _lobjcdoWebServiceAddressResult.addr_line_1;
                abusWssMemberRecordRequest.icdoWssPersonAddress.addr_line_2 = _lobjcdoWebServiceAddressResult.addr_line_2;
                abusWssMemberRecordRequest.icdoWssPersonAddress.addr_city = _lobjcdoWebServiceAddressResult.addr_city;
                abusWssMemberRecordRequest.icdoWssPersonAddress.addr_state_value = _lobjcdoWebServiceAddressResult.addr_state_value;
                abusWssMemberRecordRequest.icdoWssPersonAddress.addr_zip_code = _lobjcdoWebServiceAddressResult.addr_zip_code;
                abusWssMemberRecordRequest.icdoWssPersonAddress.addr_zip_4_code = _lobjcdoWebServiceAddressResult.addr_zip_4_code;
            }
            else if (abusWssMemberRecordRequest.icdoWssPersonAddress.addr_line_1.IsNullOrEmpty() || abusWssMemberRecordRequest.icdoWssPersonAddress.addr_city.IsNullOrEmpty()
                    || abusWssMemberRecordRequest.icdoWssPersonAddress.addr_state_value.IsNullOrEmpty() || abusWssMemberRecordRequest.icdoWssPersonAddress.addr_zip_code.IsNullOrEmpty())
            {
                iblnIsAddressNotValidated = true;
            }
            abusWssMemberRecordRequest.icdoWssPersonAddress.address_validate_error = _lobjcdoWebServiceAddressResult.address_validate_error;
            abusWssMemberRecordRequest.icdoWssPersonAddress.address_validate_flag = _lobjcdoWebServiceAddressResult.address_validate_flag;
        }
    }

}
