#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using Sagitec.BusinessTier;
using Sagitec.BusinessObjects;
using NeoSpin.Interface;
using Sagitec.Common;
using System.Collections;
using NeoSpin.BusinessObjects;
using System.Collections.ObjectModel;
using System.Data;
using NeoSpin.CustomDataObjects;
using Sagitec.ExceptionPub;
using System.IO;
using System.Web;
using System.Linq;
using Sagitec.CorBuilder;
using Sagitec.Bpm;
#endregion

namespace NeoSpin.BusinessTier
{
    /// <summary>
    /// Summary description for srvPension.
    /// </summary>
    public abstract class srvNeoSpin : srvMainDBAccess, INeoSpinBusinessTier
    {
        public srvNeoSpin()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        static srvNeoSpin()
        {
            busMainBase.GetObjectStore = delegate () { return new utlSolObjectStore(); };
            HelperFunction.GetUserDefinedFormat = srvNeoSpin.GetUserDefinedFormat;
        }
        static string GetUserDefinedFormat(object aobjValue, string astrDataFormat, string astrResult)
        {
            if (utlPassInfo.iobjPassInfo.istrFormName == "wfmOrgPlanMaintenance" && !string.IsNullOrEmpty(astrDataFormat) && astrDataFormat.ToLower().StartsWith("{0:n") && Convert.ToString(aobjValue) == "0")
                return "0";
            return null;
        }
        //public override object GetBusinessTierDetails()
        //{
        //    if (busNeoSpinBase.iutlServerDetail == null)
        //    {
        //        busNeoSpinBase lobjN = new busNeoSpinBase();
        //    }

        //    utlServerDetail lutlServerDetail = new utlServerDetail();
        //    lutlServerDetail.istrIPAddress = busNeoSpinBase.iutlServerDetail.istrIPAddress;
        //    lutlServerDetail.istrReleaseDate = busNeoSpinBase.iutlServerDetail.istrReleaseDate;

        //    return lutlServerDetail;
        //}


        //public override utlCorresPondenceInfo SetCorrespondence(string astrTemplateName, ArrayList aarrResult, Hashtable ahtbQueryBkmarks)
        //{
        //    utlCorresPondenceInfo lobjCorrespondenceInfo = busNeoSpinBase.SetCorrespondence(astrTemplateName, aarrResult, ahtbQueryBkmarks);

        //    //During the ondemand generation of the correspondence - suppress the auto print of the letter just in case
        //    lobjCorrespondenceInfo.istrAutoPrintFlag = "N";
        //    return lobjCorrespondenceInfo;
        //}

        public Collection<busSolBpmProcessInstanceAttachments> LoadImageDataByProcessInstance(int aintProcessInstanceID)
        {
            Collection<busSolBpmProcessInstanceAttachments> lclbWfImageDataResult = new Collection<busSolBpmProcessInstanceAttachments>();
            busBpmProcessInstance lobjProcessInstance = ClassMapper.GetObject<busBpmProcessInstance>();
            if (lobjProcessInstance.FindByPrimaryKey(aintProcessInstanceID))
            {
                lobjProcessInstance.LoadBpmCaseInstance();
                Collection<busSolBpmProcessInstanceAttachments> lclbWfImageData = new Collection<busSolBpmProcessInstanceAttachments>();

                DataTable ldtbWfImageData = busBase.Select<doBpmPrcsInstAttachments>(new string[1] { "bpm_process_instance_id" },
                      new object[1] { aintProcessInstanceID }, null, null);

                lclbWfImageData = new busBase().GetCollection<busSolBpmProcessInstanceAttachments>(ldtbWfImageData, "icdoBpmProcessInstanceAttachments");

                //Select the Distinct Document Code To Avoid Duplicate showing on Grid                
                var lenuDistinctList = lclbWfImageData.GroupBy(i => i.icdoBpmProcessInstanceAttachments.doc_type).Select(i => i.First());

                //Loading the System Indexing Data for each Document in order to view the images
                foreach (busSolBpmProcessInstanceAttachments lobjWfImageData in lenuDistinctList)
                {
                    Collection<busSolBpmProcessInstanceAttachments> lclbTempWfImageData =
                        busFileNetHelper.LoadFileNetImages(
                            lobjWfImageData.icdoBpmProcessInstanceAttachments.additional_info,
                            lobjWfImageData.icdoBpmProcessInstanceAttachments.doc_type,
                            lobjWfImageData.icdoBpmProcessInstanceAttachments.created_date,
                            lobjProcessInstance.ibusBpmCaseInstance.icdoBpmCaseInstance.person_id,
                            busGlobalFunctions.GetOrgCodeFromOrgId(lobjProcessInstance.ibusBpmCaseInstance.icdoBpmCaseInstance.org_id));
                    foreach (busSolBpmProcessInstanceAttachments lobjTempWfImageData in lclbTempWfImageData)
                    {
                        lobjTempWfImageData.icdoBpmProcessInstanceAttachments = lobjWfImageData.icdoBpmProcessInstanceAttachments;
                        lclbWfImageDataResult.Add(lobjTempWfImageData);
                    }
                }
            }
            return lclbWfImageDataResult;
        }
        public Collection<busSolBpmProcessInstanceAttachments> LoadImageDataByAttachmentId(int aintProcessInstanceAttachmentId)
        {
            busSolBpmProcessInstanceAttachments lbusSolBpmProcessInstanceAttachments = new busSolBpmProcessInstanceAttachments();
            lbusSolBpmProcessInstanceAttachments.FindByPrimaryKey(aintProcessInstanceAttachmentId);
            Collection<busSolBpmProcessInstanceAttachments> lclbWfImageDataResult = new Collection<busSolBpmProcessInstanceAttachments>();
            busBpmProcessInstance lobjProcessInstance = ClassMapper.GetObject<busBpmProcessInstance>();
            if (lobjProcessInstance.FindByPrimaryKey(lbusSolBpmProcessInstanceAttachments.icdoBpmProcessInstanceAttachments.bpm_process_instance_id))
            {
                lobjProcessInstance.LoadBpmCaseInstance();
                Collection<busSolBpmProcessInstanceAttachments> lclbWfImageData = new Collection<busSolBpmProcessInstanceAttachments>();
                lclbWfImageData.Add(lbusSolBpmProcessInstanceAttachments);



                //Loading the System Indexing Data for each Document in order to view the images
                foreach (busSolBpmProcessInstanceAttachments lobjWfImageData in lclbWfImageData)
                {
                    Collection<busSolBpmProcessInstanceAttachments> lclbTempWfImageData =
                    busFileNetHelper.LoadFileNetImages(
                    lobjWfImageData.icdoBpmProcessInstanceAttachments.additional_info,
                    lobjWfImageData.icdoBpmProcessInstanceAttachments.doc_type,
                    lobjWfImageData.icdoBpmProcessInstanceAttachments.created_date,
                    lobjProcessInstance.ibusBpmCaseInstance.icdoBpmCaseInstance.person_id,
                    busGlobalFunctions.GetOrgCodeFromOrgId(lobjProcessInstance.ibusBpmCaseInstance.icdoBpmCaseInstance.org_id));
                    foreach (busSolBpmProcessInstanceAttachments lobjTempWfImageData in lclbTempWfImageData)
                    {
                        lobjTempWfImageData.icdoBpmProcessInstanceAttachments = lobjWfImageData.icdoBpmProcessInstanceAttachments;
                        lclbWfImageDataResult.Add(lobjTempWfImageData);
                    }
                }
            }
            return lclbWfImageDataResult;
        }
        public ArrayList InitializeFileNetUploadService(string astrFileName, string astrUserID, int aintUserSerialID)
        {
            ArrayList larrList = new ArrayList();
            utlError lobjError;
            try
            {
                int lintTrackingID = Convert.ToInt32(astrFileName.Substring(astrFileName.LastIndexOf("-") + 1, 10));

                //Validation
                cdoCorTracking lcdoCorTracking = new cdoCorTracking();
                if (lcdoCorTracking.SelectRow(new object[1] { lintTrackingID }))
                {
                    if (lcdoCorTracking.cor_status_value == busConstant.CorrespondenceStatus_Imaged)
                    {
                        lobjError = new utlError();
                        lobjError.istrErrorMessage = "Document is already Imaged!";
                        larrList.Add(lobjError);
                        return larrList;
                    }

                    busCorTemplates lobjCorTemplates = new busCorTemplates();
                    if (lobjCorTemplates.FindCorTemplates(lcdoCorTracking.template_id))
                    {
                        if ((String.IsNullOrEmpty(lobjCorTemplates.icdoCorTemplates.filenet_document_type_value) ||
                            String.IsNullOrEmpty(lobjCorTemplates.icdoCorTemplates.image_doc_category_value) ||
                            String.IsNullOrEmpty(lobjCorTemplates.icdoCorTemplates.document_code)))
                        {
                            lobjError = new utlError();
                            lobjError.istrErrorID = "609";
                            lobjError.istrErrorMessage = "FileNet Related Information must be set in Templates before Imaging. (Document Type, Image Doc Category, Document Code)"; //PIR 16111, 16097 - Cor Editor Tool expects this error message
                            larrList.Add(lobjError);
                            return larrList;
                        }
                    }
                    else
                    {
                        lobjError = new utlError();
                        lobjError.istrErrorMessage = "Invalid Template";
                        larrList.Add(lobjError);
                        return larrList;
                    }
                }
                else
                {
                    lobjError = new utlError();
                    lobjError.istrErrorMessage = "Invalid Tracking ID";
                    larrList.Add(lobjError);
                    return larrList;
                }

                Dictionary<string, object> ldicParams = new Dictionary<string, object>();
                ldicParams[utlConstants.istrConstUserID] = astrUserID;
                ldicParams[utlConstants.istrConstUserSerialID] = aintUserSerialID;
                // Added log code to check PROD image button issue
                if (!UpdateCorrespondenceTrackingStatus(lintTrackingID, busConstant.CorrespondenceStatus_Ready_For_Imaging, ldicParams))
                {
                    lobjError = new utlError();
                    lobjError.istrErrorMessage = "Correspondence is not imaged.";
                    larrList.Add(lobjError);
                    return larrList;
                }
            }
            catch (Exception _exc)
            {
                ExceptionManager.Publish(_exc);
                lobjError = new utlError();
                lobjError.istrErrorMessage = "Invalid Tracking ID";
                larrList.Add(lobjError);
            }
            return larrList;
        }


        /// <summary>
        /// Methed overloded for correspondence Templete
        /// </summary>
        /// <param name="astrGeneratedDocumentName"></param>
        /// <param name="astrTemplateName"></param>
        /// <param name="astrTrackingId"></param>
        /// <returns></returns>
        protected override ArrayList GetGeneratedCorrespondence(string astrGeneratedDocumentName = null, string astrTemplateName = null, string astrTrackingId = null)
        {
            ArrayList larrResult = null;

            try
            {
                if (iobjPassInfo.istrFormName == "wfmCorTemplatesLookup")
                {
                    string FilePath = GetBaseDirectory();
                    astrGeneratedDocumentName = FilePath + "Correspondence\\Templates\\" + astrGeneratedDocumentName + ".docx";
                }
                if (astrGeneratedDocumentName != null)
                {
                    larrResult = EditCorrOnLocalTool(astrGeneratedDocumentName, iobjPassInfo.idictParams);
                }
                else
                {
                    string lstrFileName = null;
                    astrTrackingId = astrTrackingId.PadLeft(10, '0');
                    lstrFileName = astrTemplateName + "-" + astrTrackingId + ".docx";
                    string lstrCorrespondencePath = iobjPassInfo.isrvDBCache.GetPathInfo("CorrGenr");
                    larrResult = EditCorrOnLocalTool(lstrCorrespondencePath + lstrFileName, iobjPassInfo.idictParams);
                }
                return larrResult;
            }
            catch (Exception exc)
            {
                throw HandleGlobalError(exc);
            }
            finally
            {
                DisposeFrameworkConnection();
            }
        }

        public override bool UpdateCorrespondenceTrackingStatus(int aintTrackingID, string astrStatus, Dictionary<string, object> adictParams)
        {
            try
            {
                if (iobjPassInfo.istrFormName != "wfmCorTemplatesLookup")
                {
                    iobjPassInfo.idictParams = adictParams;
                    BeginTransaction();

                    cdoCorTracking lcdoCorTracking = new cdoCorTracking();
                    if (lcdoCorTracking.SelectRow(new object[1] { aintTrackingID }))
                    {
                        if (astrStatus != "" && astrStatus != "SAVE")
                        {
                            if (astrStatus == "PRINT")
                                astrStatus = busConstant.CorrespondenceStatus_Printed;

                            lcdoCorTracking.cor_status_value = astrStatus;
                            if (astrStatus == busConstant.CorrespondenceStatus_Printed)
                                lcdoCorTracking.printed_date = DateTime.Now;
                            //Dont Set the Imaged Date here.. Because Imaging is happening through Service 
                            //and the service will update the imaged_date.
                        }
                        lcdoCorTracking.Update();
                    }
                    Commit();
                }
            }
            catch (Exception _exc)
            {
                Rollback();
                ExceptionManager.Publish(_exc);
                return false;
            }
            finally
            {
                if ((iobjPassInfo.iconFramework != null) &&
                    (iobjPassInfo.iconFramework.State == ConnectionState.Open))
                {
                    iobjPassInfo.iconFramework.Close();
                    iobjPassInfo.iconFramework.Dispose();
                }
            }
            return true;
        }

        public string SagitecEncrypt(string astrKey, string astrValue)
        {
            return HelperFunction.SagitecEncrypt(astrKey, astrValue);
        }

        public string SagitecDecrypt(string astrKey, string astrValue)
        {
            return HelperFunction.SagitecDecrypt(astrKey, astrValue);
        }

        public ArrayList NewCalculate_Click(int aintBenefitApplicationID)
        {
            return new busRetirementDisabilityApplication().NewCalculate_Click(aintBenefitApplicationID);
        }

        public byte[] CreateMemberAnnualStatement(int aintSelectionID, string astrReportName)
        {
            busMASSelection lobjSelection = new busMASSelection { icdoMasSelection = new cdoMasSelection() };
            lobjSelection.FindMASSelection(aintSelectionID);

            if (lobjSelection.ibusBatchRequest == null)
                lobjSelection.LoadBatchRequest();
            if (lobjSelection.ibusBatchRequest.icdoMasBatchRequest.group_type_value == busConstant.GroupTypeNonRetired)
            {
                return lobjSelection.CreateMASReport(astrReportName, busConstant.ReportMASPath);
            }
            else
            {
                return lobjSelection.CreateAnnualRetireeStatement(astrReportName, busConstant.ReportMASPath);
            }
        }

        public byte[] CreateRemittanceReport(int aintEmployerPayrollHeaderId)
        {
            busEmployerPayrollHeader lbusEmployerPayrollHeader = new busEmployerPayrollHeader
            {
                icdoEmployerPayrollHeader = new cdoEmployerPayrollHeader()
            };
            lbusEmployerPayrollHeader.FindEmployerPayrollHeader(aintEmployerPayrollHeaderId);
            return lbusEmployerPayrollHeader.btnGenerateRemittanceReport_Click();
        }

        //FW Upgrade :: PIR 11480 - To open pdf in new tab in IE browser, get a file path only 
        public string CreateRemittanceReportPath(int aintEmployerPayrollHeaderId)
        {
            busEmployerPayrollHeader lbusEmployerPayrollHeader = new busEmployerPayrollHeader
            {
                icdoEmployerPayrollHeader = new cdoEmployerPayrollHeader()
            };
            lbusEmployerPayrollHeader.FindEmployerPayrollHeader(aintEmployerPayrollHeaderId);
            return lbusEmployerPayrollHeader.btnGenerateRemittanceReport_ClickPath();
        }

        public ArrayList PublishToWSSAndSendMail(string astrFileName, string astrUserID, int aintUserSerialID)
        {
            ArrayList larrList = new ArrayList();
            busWSSHome lobjWSSHome = new busWSSHome();
            iobjPassInfo.istrUserID = astrUserID;
            larrList = lobjWSSHome.PublishToWSSAndEmail(iobjPassInfo.isrvDBCache.GetPathInfo("CorrGenr") + "\\" + astrFileName);
            return larrList;
        }

        //UCS 24 benefit calculation menu display
        public bool IsPersonEnrolledOrSuspendedInRetPlan(int aintPersonId)
        {
            busMSSHome lobjHome = new busMSSHome();
            lobjHome.LoadPerson(aintPersonId);
            return (lobjHome.IsMemberEnrolledOrSuspendedInDBPlan() || lobjHome.IsMemberEnrolledOrSuspendedInDCPlan());
        }

        //UCS 24 benefit calculation menu display
        public bool IsActiveMember(int aintPersonId)
        {
            busMSSHome lobjHome = new busMSSHome();
            lobjHome.LoadPerson(aintPersonId);
            lobjHome.ibusPerson.LoadPersonEmployment();
            if (lobjHome.ibusPerson.icolPersonEmployment.Where(iobj => iobj.icdoPersonEmployment.end_date == DateTime.MinValue).Any() || // PIR 9709
                lobjHome.IsMemberEnrolledOrSuspendedInDBPlan())
                return true;
            return false;
        }

        //For MSS Layout change
        public bool IsRetiree(int aintPersonId)
        {
            busPayeeAccount ibusPayeeAccount = new busPayeeAccount();
            if (ibusPayeeAccount.IsRetiree(aintPersonId))
                return true;
            return false;
        }

        //pir 6887
        //UCS 24 pension payment details menu display
        public bool IsPersonRetiredOrWithdrawnPlan(int aintPersonId)
        {
            busMSSHome lobjHome = new busMSSHome();
            lobjHome.LoadPerson(aintPersonId);
            return (lobjHome.IsMemberHaveRetiredOrWithdrawnAccount());
        }

        //pir 6887
        //UCS 24 view service purchase menu display
        public bool IsPersonHavePurchase(int aintPersonId)
        {
            busMSSHome lobjHome = new busMSSHome();
            lobjHome.LoadPerson(aintPersonId);
            return (lobjHome.IsMemberHavePurchase());
        }

        // PROD PIR 8861 
        public bool IsInsurancePlanRetirees(int aintPersonId)
        {
            busMSSHome lobjHome = new busMSSHome();
            lobjHome.LoadPerson(aintPersonId);
            return lobjHome.IsInsurancePlanRetirees();
        }

        //PIR-13473
        public bool IsEnrolledInCOBRA(int aintPersonId)
        {
            busMSSHome lobjHome = new busMSSHome();
            lobjHome.LoadPerson(aintPersonId);
            return lobjHome.IsEnrolledInCOBRA();
        }

        public byte[] View1099RReport_Click(int aint1099rID)
        {
            string lstrForm1099RName = string.Empty;
            busNeoSpinBase lobjNeospinBase = new busNeoSpinBase();
            busPayment1099r lobj1099R = new busPayment1099r();
            DataSet ldt = new DataSet();
            ldt = lobj1099R.GetDataSetToCreateReport(aint1099rID);
            //PIR-16715 created report file with respective year.
            if (ldt.Tables[0].Rows.Count > 0)
                lstrForm1099RName = ldt.Tables[0].Rows[0]["RUN_YEAR"].ToString();

            return lobjNeospinBase.CreateDynamicReport("rptForm1099R_" + lstrForm1099RName + ".rpt", ldt, string.Empty);
        }

        public ArrayList GenerateMailingLabelWordReport(int aintMailingBatchID)
        {
            busNeoSpinBase lobjNeospinBase = new busNeoSpinBase();
            busMailingLabel lbusMailingLabel = new busMailingLabel();
            ArrayList larlstResult = new ArrayList();
            larlstResult.Add("rptMailingLabelReport_"+DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss_tt") + ".docx");
            DataSet ldtResult = new DataSet();
            ldtResult = lbusMailingLabel.GenerateMailingLabelDataSet(aintMailingBatchID);
            larlstResult.Add(lobjNeospinBase.CreateDynamicWordReport("rptMailingLabelAvery.rpt", ldtResult, string.Empty));
            larlstResult.Add("application/vnd.openxmlformats-officedocument.wordprocessingml.document");
            return larlstResult;
        }

        public bool UploadPIRAttachements(byte[] uploadedFileContent, string astrFolderPath, string astrFileName)
        {
            try
            {
                if (!System.IO.Directory.Exists(astrFolderPath))
                {
                    System.IO.Directory.CreateDirectory(astrFolderPath);
                }
                if (System.IO.Directory.Exists(astrFolderPath))
                {
                    File.WriteAllBytes(astrFolderPath + "\\" + astrFileName, uploadedFileContent);
                }
                return true;
            }
            catch (Exception _exc)
            {
                ExceptionManager.Publish(_exc);
                return false;
            }
        }

        public List<string> LoadPIRAttachments(string astrFolderPath)
        {
            List<string> llstResult = new List<string>();

            if (System.IO.Directory.Exists(astrFolderPath))
            {
                System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(astrFolderPath);
                System.IO.FileInfo[] files = dir.GetFiles();
                int i = 0;
                foreach (System.IO.FileInfo file in files)
                {
                    llstResult.Add(System.IO.Path.GetFileName(file.Name));
                    i++;
                    if (i == 25)
                    {
                        break;
                    }
                }
            }

            return llstResult;
        }
        public Collection<busPIRAttachment> GetPIRAttachmentsCollection(string astrFolderPath)
        {
            Collection<busPIRAttachment> iclbPIRAttchment = new Collection<busPIRAttachment>();
            if (System.IO.Directory.Exists(astrFolderPath))
            {
                System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(astrFolderPath);
                System.IO.FileInfo[] files = dir.GetFiles();
                int i = 0;
                foreach (System.IO.FileInfo file in files)
                {
                    busPIRAttachment lobjPIRAttachment = new busPIRAttachment();
                    lobjPIRAttachment.istrAttchmentName = System.IO.Path.GetFileName(file.Name);
                    lobjPIRAttachment.idtmAttchmentLastModifiedDate = file.LastWriteTime;
                    lobjPIRAttachment.idblAttchmentSize = Math.Round(Convert.ToDouble(file.Length)/1000);
                    lobjPIRAttachment.istrAttachmentType = busNeoSpinBase.GetMIMEType(file.Name);
                    iclbPIRAttchment.Add(lobjPIRAttachment);
                    i++;
                    if (i == 25)
                    {
                        break;
                    }
                }
                iclbPIRAttchment.OrderByDescending(o=>o.idtmAttchmentLastModifiedDate);
            }

            return iclbPIRAttchment;
        }   

        public byte[] DownloadPIRAttachment(string astrFilePath)
        {
            return File.ReadAllBytes(astrFilePath); ;
        }

        public bool DeletePIRAttachment(string astrFilePath)
        {
            astrFilePath = iobjPassInfo.isrvDBCache.GetPathInfo("PIRA") + "\\" + astrFilePath;
            if (System.IO.File.Exists(astrFilePath))
            {
                try
                {
                    System.IO.File.Delete(astrFilePath);
                    return true;
                }
                catch (Exception _exc)
                {
                    ExceptionManager.Publish(_exc);
                    return false;
                }
            }
            return false;
        }

        //public busActivityInstance ReloadActivityInstance(busActivityInstance abusActivityInstance)
        //{
        //    abusActivityInstance.icdoActivityInstance.Select();
        //    return abusActivityInstance;
        //}


        //RA Verify
        //public new ArrayList GetActivityRedirectInformation(string astrButtonID, ArrayList aarrResult, bool ablnCheckOut, bool ablnResume, Dictionary<string, object> adictParams)
        //{
        //    ArrayList larrResult = new ArrayList();

        //    //Validate if the Selected Activity Instance is already checked out by some other user..
        //    if ((aarrResult != null) && (aarrResult.Count > 0))
        //    {
        //        if (aarrResult[0] is busActivityInstance)
        //        {
        //            busActivityInstance lbusActivityInstance = (busActivityInstance)aarrResult[0];
        //            cdoActivityInstance lcdoActivityInstance = new cdoActivityInstance();
        //            lcdoActivityInstance.SelectRow((new object[1] { lbusActivityInstance.icdoActivityInstance.activity_instance_id }));
        //            if (lbusActivityInstance.icdoActivityInstance.update_seq < lcdoActivityInstance.update_seq)
        //            {
        //                utlError lutlError = new utlError();
        //                lutlError.istrErrorMessage = "This Activity is already checked out by Some other User! Please Reload the My Basket.";
        //                larrResult.Add(lutlError);
        //                return larrResult;
        //            }
        //        }
        //    }
        //    return base.GetActivityRedirectInformation(astrButtonID, aarrResult, ablnCheckOut, ablnResume, adictParams) as ArrayList;
        //}

        /// <summary>
        /// Generate correspondence, create the tracking record for the generated letter
        /// </summary>
        /// <param name="aintTemplateID"></param>
        /// <param name="aintPersonID"></param>
        /// <param name="astrUserId"></param>
        /// <param name="aarrResult"></param>
        /// <returns></returns>
        ///
        //Commented for 6.0 Upgrade 
        //public override string CreateCorrespondence(string astrTemplateName, ArrayList aarrResult, Hashtable ahtbQueryBkmarks, Dictionary<string, object> adictParams)
        public override string CreateCorrespondence(string astrTemplateName, object aobjBase, Hashtable ahtbQueryBkmarks, Dictionary<string, object> adictParams)
        {
            iobjPassInfo.idictParams = adictParams;
            string lstrUserID = "";
            if (adictParams.ContainsKey(utlConstants.istrConstUserID.ToString()))
            {
                lstrUserID = adictParams[utlConstants.istrConstUserID.ToString()].ToString();
            }

            try
            {
                BeginTransaction();

                CorBuilderXML lobjCorBuilder = null;
                try
                {
                    // Commented for 6.0 upgrade
                    //utlCorresPondenceInfo lobjCorresPondenceInfo = SetCorrespondence(astrTemplateName, aarrResult, ahtbQueryBkmarks);

                    utlCorresPondenceInfo lobjCorresPondenceInfo = busNeoSpinBase.SetCorrespondence(astrTemplateName, aobjBase as busBase, ahtbQueryBkmarks);
                    lobjCorresPondenceInfo.istrAutoPrintFlag = "N";

                    if (lobjCorresPondenceInfo == null)
                    {
                        throw new Exception("Unable to create correspondence, SetCorrespondence method not found in " +
                            " business solutions base object");
                    }

                    lobjCorBuilder = new CorBuilderXML();
                    lobjCorBuilder.InstantiateWord();
                    string lstrFileName = lobjCorBuilder.CreateCorrespondenceFromTemplate(astrTemplateName, lobjCorresPondenceInfo, lstrUserID);
                    lobjCorBuilder.CloseWord();
                    Commit();
                    return lstrFileName;
                }

                catch (Exception e)
                {
                    //modified by deepak and rachit on 04-22-2009 with FPPA Muralidharan 
                    //FPPA pir 101162, if the network drive where the correspondence is getting generated goes down then the 
                    //word instance does not get closed. below code will ensure that word instance gets closed.
                    if (lobjCorBuilder != null)
                    {
                        try
                        {
                            lobjCorBuilder.CloseWord();
                        }
                        catch
                        {
                        }
                    }
                    Rollback();
                    string lstrMessage = "Failed to CreateCorrespondence, following error occured : " + e.Message;
                    if (e.InnerException != null)
                    {
                        lstrMessage += " Inner Exception : " + e.InnerException.Message;
                    }
                    throw new Exception(lstrMessage);
                }
            }
            finally
            {
                if ((iobjPassInfo.iconFramework != null) &&
                    (iobjPassInfo.iconFramework.State == ConnectionState.Open))
                {
                    iobjPassInfo.iconFramework.Close();
                    iobjPassInfo.iconFramework.Dispose();
                }

            }
        }

        // PIR 9649
        public string GetMSSAccessValue(int aintPersonId)
        {
            busMSSHome lobjHome = new busMSSHome();
            lobjHome.LoadPerson(aintPersonId);
            lobjHome.ibusPerson.LoadPersonEmployment(); // PIR 10059
            if (lobjHome.ibusPerson.icolPersonEmployment.Where(o => o.ibusOrganization.icdoOrganization.mss_access_value == busConstant.OrganizationFullAccess).Any())
                return busConstant.OrganizationFullAccess;
            else
            {
                lobjHome.ibusPerson.LoadCurrentEmployer();
                lobjHome.ibusPerson.LoadCurrentEmployerDetails();
                lobjHome.LoadAllPersonEmploymentDetails();
                if (lobjHome.iclbPersonEmploymentDetail.Count > 0)
                {
                    lobjHome.iintCurrentEmploymentDetailID = lobjHome.iclbPersonEmploymentDetail[0].icdoPersonEmploymentDetail.person_employment_dtl_id;
                    busPersonEmploymentDetail lobjEmpDtl = new busPersonEmploymentDetail { icdoPersonEmploymentDetail = new cdoPersonEmploymentDetail() };
                    lobjEmpDtl = lobjHome.iclbPersonEmploymentDetail[0];
                    lobjEmpDtl.LoadPersonEmployment();
                    lobjEmpDtl.ibusPersonEmployment.LoadOrganization();
                    if (lobjEmpDtl.ibusPersonEmployment.ibusOrganization.icdoOrganization.mss_access_value == busConstant.OrganizationLimitedAccess)
                        lobjHome.istrIsLimitedAccessRetiree = busConstant.Flag_Yes;
                    return lobjEmpDtl.ibusPersonEmployment.ibusOrganization.icdoOrganization.mss_access_value;
                }
            }
            return string.Empty;
        }

        // PIR 9773
        public string GetESSAccessValue(int aintOrgId)
        {
            busOrganization lobjOrg = new busOrganization { icdoOrganization = new cdoOrganization() };
            lobjOrg.FindOrganization(aintOrgId);
            return lobjOrg.icdoOrganization.mss_access_value;
        }
        public bool DoesFileExists(string astrFileName)
        {
            return File.Exists(astrFileName);
        }
        public byte[] btnGenerateAgencyExcelReport_Click(int aintOrgId, string astrBenefitType, string astrFromDate, string astrToDate)
        {
            busNeoSpinBase lobjNeospinBase = new busNeoSpinBase();
            busEmployerPayrollMonthlyStatement ibusEmployerPayrollMonthlyStatement = new busEmployerPayrollMonthlyStatement();
            busOrganization lobjOrganization = new busOrganization();
            Hashtable lhstParam = new Hashtable();
            DataSet ldt = new DataSet();

            ldt = ibusEmployerPayrollMonthlyStatement.btnGenerateAgencyExcelReport_Click(aintOrgId, astrBenefitType, astrFromDate, astrToDate);
            lobjOrganization.FindOrganization(aintOrgId);
            lhstParam.Add("OrgName", lobjOrganization.icdoOrganization.org_name);

            switch (astrBenefitType)
            {
                case "Retirement":
                    return lobjNeospinBase.CreateDynamicExcelReport("rptAgencyStatementForRetirement.rpt", ldt, string.Empty, hstParam: lhstParam);
                case "Insurance":
                    return lobjNeospinBase.CreateDynamicExcelReport("rptAgencyStatementForInsurance.rpt", ldt, string.Empty, hstParam: lhstParam);
                case "DefComp":
                    return lobjNeospinBase.CreateDynamicExcelReport("rptAgencyStatementForDeferredComp.rpt", ldt, string.Empty, hstParam: lhstParam);
            }
            return null;
        }

        /// <summary>
        /// Overidden method to perform action on Custom Button click of Correspondence Editor Tool.
        /// </summary>
        /// <param name="adictCorrInfo"></param>
        /// <param name="adictParams"></param>
        /// <returns></returns>
        public override ArrayList CorrToolCustomButton(Dictionary<string, object> adictCorrInfo, Dictionary<string, object> adictParams)
        {
            ArrayList larrErrorList = new ArrayList();
            utlError lobjError;
            string lstrUserID, lstrFileName = string.Empty;
            int lintUserSerialID = 0;
            if (adictCorrInfo.IsNotNull() && adictCorrInfo.Keys.Count > 0)
            {
                lstrUserID = Convert.ToString(adictCorrInfo["UserID"]);
                int.TryParse(Convert.ToString(adictCorrInfo["UserSerialID"]), out lintUserSerialID);
                lstrFileName = Convert.ToString(adictCorrInfo["FileName"]);
            }
            else
            {
                lobjError = new utlError();
                lobjError.istrErrorMessage = "There is a problem with the server, please try after sometime.";
                larrErrorList.Add(lobjError);
                return larrErrorList;
            }
            if (string.IsNullOrEmpty(lstrUserID) || string.IsNullOrEmpty(lstrFileName) || lintUserSerialID == 0)
            {
                lobjError = new utlError();
                lobjError.istrErrorMessage = "There is a problem with the server, please try after sometime.";
                larrErrorList.Add(lobjError);
                return larrErrorList;
            }
            return InitializeFileNetUploadService(lstrFileName, lstrUserID, lintUserSerialID);
        }
        //PIR - 7941 - We want to avoid having to go into each detail individually to ignore them or to add comments when there are many records to ignore.
        public List<int> IgnoreSelectedPayrollDetail(List<int> aEmpPayDetailIds, string astrComment, bool ablnIsSetIgnorStatus)
        {
            busEmployerPayrollDetail lbusEmployerPayrollDetail = new busEmployerPayrollDetail();
            return lbusEmployerPayrollDetail.IgnoreSelectedPayrollDetail(aEmpPayDetailIds, astrComment, ablnIsSetIgnorStatus);

        }
        #region PIR-18492 
        //Conditions to check if person is varified for not allowed to navigate anywhere else within MSS
        public bool IsPersonVertify(int aintPersonId)
        {
            busMSSHome ibusMSSHome = new busMSSHome();        
            ibusMSSHome.LoadPerson(aintPersonId);
            int lintMonths = Convert.ToInt32(busGlobalFunctions.GetData1ByCodeValue(7010, "MNTH", iobjPassInfo));

            //Allowing to navigate to other links: 1. Person is login into MSS within 9 months and verified the Activation_code or if email_waiver is selected.
            if (ibusMSSHome.ibusPerson.icdoPerson.certify_date != DateTime.MinValue && ((ibusMSSHome.ibusPerson.icdoPerson.certify_date).AddMonths(lintMonths) >= DateTime.Today) &&
               (ibusMSSHome.ibusPerson.icdoPerson.activation_code_flag == busConstant.Flag_Yes || ibusMSSHome.ibusPerson.icdoPerson.email_waiver_flag == busConstant.Flag_Yes))
                return true;
            //Not Allowing to navigate to other links: 1.  When person is login to MSS for first time - certify_date is null. 
            //Not Allowing to navigate to other links: 2. When person login into MSS after 9 montths and Activation code is not verified (activation_code_flag = N)
            if (ibusMSSHome.ibusPerson.icdoPerson.certify_date == DateTime.MinValue ||
               (ibusMSSHome.ibusPerson.icdoPerson.certify_date != DateTime.MinValue && ((ibusMSSHome.ibusPerson.icdoPerson.certify_date).AddMonths(lintMonths) < DateTime.Today) &&
               (ibusMSSHome.ibusPerson.icdoPerson.activation_code_flag.IsNullOrEmpty() || ibusMSSHome.ibusPerson.icdoPerson.activation_code_flag == busConstant.Flag_No)))
            {
                return false;
            }
            //Not Allowing to navigate to other links: 3. Person is login into MSS within 9 months and not verified a Activation_code
            if (ibusMSSHome.ibusPerson.icdoPerson.certify_date != DateTime.MinValue && ((ibusMSSHome.ibusPerson.icdoPerson.certify_date).AddMonths(lintMonths) >= DateTime.Today) &&
             (ibusMSSHome.ibusPerson.icdoPerson.activation_code_flag.IsNullOrEmpty() || ibusMSSHome.ibusPerson.icdoPerson.activation_code_flag == busConstant.Flag_No))
            {
                return false;
            }   
            //Not Allowing to navigate to other links:  4: (Point 5): If email is not changed in last 9 months, then user has to waived the email. 
            if (ibusMSSHome.ibusPerson.icdoPerson.email_waiver_date != DateTime.MinValue && (ibusMSSHome.ibusPerson.icdoPerson.email_waiver_date).AddMonths(lintMonths) < DateTime.Today)
                return false;
            //login after 9 months
            if (ibusMSSHome.ibusPerson.icdoPerson.certify_date != DateTime.MinValue && ((ibusMSSHome.ibusPerson.icdoPerson.certify_date).AddMonths(lintMonths) < DateTime.Today))
                return false;
            return true;
        }
        #endregion PIR-18492

        public override DataTable GetCustomSecurity()
        {
            DataTable ldtbList = null;
            if (iobjPassInfo.IsNotNull() && iobjPassInfo.idictParams.Count > 0 && (iobjPassInfo.idictParams.ContainsKey("OrgID")) && (iobjPassInfo.idictParams.ContainsKey("ContactID")) && (iobjPassInfo.idictParams.ContainsKey("IsFromWhichPortal") && iobjPassInfo.idictParams["IsFromWhichPortal"].Equals("ESS")))
            {
                int lintOrgID = (int)iobjPassInfo.idictParams["OrgID"];
                int lintContactID = (int)iobjPassInfo.idictParams["ContactID"];
                ldtbList = busNeoSpinBase.Select("entContact.GetSecurityBasedOnContactIDOrgID", new object[2] { lintContactID, lintOrgID });
                if (ldtbList.Rows.Count > 0)
                {
                    DataRow row = ldtbList.NewRow();
                    row["CONTACT_ID"] = lintContactID;
                    row["RESOURCE_ID"] = 5000;
                    row["RESOURCE_DESCRIPTION"] = "Create Payroll Report";
                    row["SECURITY_LEVEL"] = (iobjPassInfo.idictParams.ContainsKey("CentralPayroll") &&
                                            (string)iobjPassInfo.idictParams["CentralPayroll"] ==
                                            busConstant.Flag_Yes_Value) ? 0 : 5;
                    ldtbList.Rows.Add(row);
                }
            }
            else if (iobjPassInfo.IsNotNull() && iobjPassInfo.idictParams.Count > 0 &&
                (iobjPassInfo.idictParams.ContainsKey("PersonID")) && (iobjPassInfo.idictParams.ContainsKey("IsFromWhichPortal") && iobjPassInfo.idictParams["IsFromWhichPortal"].Equals("MSS")))
            {
                ldtbList = busNeoSpinBase.Select("cdoPerson.GetSecurityBasedOnPersonID", new object[1] { iobjPassInfo.idictParams["PersonID"] });
            }
            if (ldtbList == null || ldtbList.Rows.Count == 0)
                return null;
            return ldtbList;
        }
        public string GetBaseDirectory() => Convert.ToString(iobjPassInfo.isrvDBCache.GetSystemManagement()?.Rows[0]["BASE_DIRECTORY"]);
        public ArrayList UploadPIRAttachment(string astrPIRID, string astrFileName, byte[] afleByteArray)
        {
            ArrayList larrResult = new ArrayList();
            try
            {
                string lstrFolderPath = iobjPassInfo.isrvDBCache.GetPathInfo("PIRA") + "\\" + astrPIRID;
                if (!System.IO.Directory.Exists(lstrFolderPath))
                {
                    System.IO.Directory.CreateDirectory(lstrFolderPath);
                }
                if (System.IO.Directory.Exists(lstrFolderPath))
                {
                    System.IO.File.WriteAllBytes(lstrFolderPath + "\\" + astrFileName, afleByteArray);
                }
                return larrResult;
            }
            catch(Exception ex)
            {
                larrResult.Add(new utlError() { istrErrorID = "0", istrErrorMessage = ex.Message });
                return larrResult;
            }
        }

        protected override void ModifyActivityRedirectInformation(utlWorkflowActivityInfo aobjWorkflowActivityInfo, utlProcessMaintainance.utlActivity aobjActivity, string astrButtonID, object aobjBase)
        {
            busBpmActivityInstance lobAct = aobjWorkflowActivityInfo.ibusBaseActivityInstance as busBpmActivityInstance;
            if (lobAct == null)
                return;
            if(lobAct.icdoBpmActivityInstance.status_value == BpmActivityInstanceStatus.Resumed && string.IsNullOrEmpty(lobAct.icdoBpmActivityInstance.checked_out_user))
            {
                if (lobAct.ibusBpmProcessInstance.ibusBpmCaseInstance.ibusBpmCase.icdoBpmCase.name.Equals("sbpProcessAutoRefund"))
                {
                    if (lobAct.ibusBpmActivity.icdoBpmActivity.name.Equals("Enter and Calculate Auto Refund"))
                    {
                        if (lobAct.ibusBpmProcessInstance.ibusBpmCaseInstance.ibusBpmRequest.icdoBpmRequest.source_value.Equals(BpmRequestSource.Batch))
                        {
                            lobAct.CheckoutActivity(iobjPassInfo.istrUserID, false);
                        }
                    }
                }
                else if (lobAct.ibusBpmProcessInstance.ibusBpmCaseInstance.ibusBpmCase.icdoBpmCase.name.Equals("sbpACHPullForIBSInsurance"))
                {
                    if (lobAct.ibusBpmActivity.icdoBpmActivity.name.Equals("Run ACH Pull Automation – IBS batch"))
                    {
                        if (lobAct.ibusBpmProcessInstance.ibusBpmCaseInstance.ibusBpmRequest.icdoBpmRequest.source_value.Equals(BpmRequestSource.Batch))
                        {
                            lobAct.CheckoutActivity(iobjPassInfo.istrUserID, false);
                        }
                    }
                }
            }
            int lintReferenceId = lobAct.GetBpmParameterValueByType<int>("reference_id");
            utlBPMElement lutlBPMElement = lobAct.ibusBpmActivity.GetBpmActivityDetails();
            utlBPMTask lutlBPMTask = lutlBPMElement as utlBPMTask;
          
            int lintPersonId = lobAct.GetBpmParameterValueByType<int>("PersonId");
            int lintOrgId = lobAct.GetBpmParameterValueByType<int>("OrgId");

            if (aobjWorkflowActivityInfo.istrURL == "wfmBpmActivityInstanceMaintenance")
            {
                aobjWorkflowActivityInfo.ihstLaunchParameters["aintactivityinstanceid"] = ((busBpmActivityInstance)aobjWorkflowActivityInfo.ibusBaseActivityInstance).icdoBpmActivityInstance.activity_instance_id;
            }
            if (busWorkflowHelper.GetWorkflowProcessIdByBpmProcessName(lobAct.ibusBpmActivity.ibusBpmProcess.icdoBpmProcess.name) == busConstant.Map_Recalculate_Pension_and_RHIC_Benefit && busWorkflowHelper.GetWorkflowActivityIdByBpmActivityName(lobAct.ibusBpmActivity.ibusBpmProcess.icdoBpmProcess.name, lobAct.ibusBpmActivity.icdoBpmActivity.name) == 171)
            {
                busPayeeAccount lobjPayeeAccount = new busPayeeAccount();
                if (lobjPayeeAccount.FindPayeeAccount(lintReferenceId))
                {
                    if (lobjPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypePreRetirementDeath)
                    {
                        aobjWorkflowActivityInfo.ihstLaunchParameters = new Hashtable();
                        aobjWorkflowActivityInfo.istrURL = "wfmPreRetirementDeathFinalCalculationMaintenance";
                        aobjWorkflowActivityInfo.ihstLaunchParameters.Add("reference_id", lintReferenceId);
                    }
                    else if (lobjPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypePostRetirementDeath)
                    {
                        aobjWorkflowActivityInfo.ihstLaunchParameters = new Hashtable();
                        aobjWorkflowActivityInfo.istrURL = "wfmPostRetirementDeathFinalCalculationMaintenance";
                        aobjWorkflowActivityInfo.ihstLaunchParameters.Add("reference_id", lintReferenceId);
                    }
                }
            }
            if (busWorkflowHelper.GetWorkflowProcessIdByBpmProcessName(lobAct.ibusBpmActivity.ibusBpmProcess.icdoBpmProcess.name) == busConstant.MAP_Process_DC_Enrollment && 
               busWorkflowHelper.GetWorkflowActivityIdByBpmActivityName(lobAct.ibusBpmActivity.ibusBpmProcess.icdoBpmProcess.name, lobAct.ibusBpmActivity.icdoBpmActivity.name) == 357)
            {
                busPerson lobjPerson = new busPerson { icdoPerson = new cdoPerson() };
                lobjPerson.icdoPerson.person_id = lintPersonId;
                lobjPerson.FindPerson(lintPersonId);
                lobjPerson.LoadDC2025PersonAccount();

                if (lobjPerson.IsNotNull() && lobjPerson.ibusDC2025PersonAccount.icdoPersonAccount.person_account_id > 0)
                {
                    aobjWorkflowActivityInfo.ihstLaunchParameters = new Hashtable();
                    aobjWorkflowActivityInfo.istrURL = "wfmPensionPlanMaintenance";
                    aobjWorkflowActivityInfo.ihstLaunchParameters.Add("AintPersonAccountID", lobjPerson.ibusDC2025PersonAccount.icdoPersonAccount.person_account_id);
                }
            }
            else if (busWorkflowHelper.GetWorkflowProcessIdByBpmProcessName(lobAct.ibusBpmActivity.ibusBpmProcess.icdoBpmProcess.name) == busConstant.Map_Initialize_Process_Death_Notification_Workflow
                       && busWorkflowHelper.GetWorkflowActivityIdByBpmActivityName(lobAct.ibusBpmActivity.ibusBpmProcess.icdoBpmProcess.name, lobAct.ibusBpmActivity.icdoBpmActivity.name) == 57)
            {
                if (lintReferenceId == 0 && lutlBPMTask != null)
                {
                    lintReferenceId = lobAct.GetBpmParameterValueByType<int>(lutlBPMTask.istrSfwReferenceID);
                }
                if (lintReferenceId == 0)
                {
                    int lintContactTicketId = lobAct.GetBpmParameterValueByType<int>("contact_ticket_id");
                    if (lintContactTicketId > 0)
                    {
                        busDeathNotice lbusDeathNotice = new busDeathNotice();
                        lbusDeathNotice.FindDeathNoticeByContactTicket(lintContactTicketId);
                        aobjWorkflowActivityInfo.ihstLaunchParameters = new Hashtable();
                        aobjWorkflowActivityInfo.ihstLaunchParameters.Add("person_id", lintPersonId);
                        aobjWorkflowActivityInfo.ihstLaunchParameters.Add("dateof_death", lbusDeathNotice.icdoDeathNotice.death_date);
                    }
                }
            }
            else if (busWorkflowHelper.GetWorkflowProcessIdByBpmProcessName(lobAct.ibusBpmActivity.ibusBpmProcess.icdoBpmProcess.name) == busConstant.Map_Update_Dues_Rate_Table
                        && (busWorkflowHelper.GetWorkflowActivityIdByBpmActivityName(lobAct.ibusBpmActivity.ibusBpmProcess.icdoBpmProcess.name, lobAct.ibusBpmActivity.icdoBpmActivity.name) == 189 || busWorkflowHelper.GetWorkflowActivityIdByBpmActivityName(lobAct.ibusBpmActivity.ibusBpmProcess.icdoBpmProcess.name, lobAct.ibusBpmActivity.icdoBpmActivity.name) == 190) && lintReferenceId == 0)
            {
                if (lobAct.ibusBpmProcessInstance.ibusBpmCaseInstance.ibusOrganization == null)
                    lobAct.ibusBpmProcessInstance.ibusBpmCaseInstance.LoadOrganization();

                aobjWorkflowActivityInfo.ihstLaunchParameters = new Hashtable();
                aobjWorkflowActivityInfo.ihstLaunchParameters.Add("org_code", ((busOrganization)lobAct.ibusBpmProcessInstance.ibusBpmCaseInstance.ibusOrganization).icdoOrganization.org_code);
            }
            else if (busWorkflowHelper.GetWorkflowProcessIdByBpmProcessName(lobAct.ibusBpmActivity.ibusBpmProcess.icdoBpmProcess.name) == busConstant.MapWSSEnrollNewHireInPensionAndInsurancePlans ||
                busWorkflowHelper.GetWorkflowProcessIdByBpmProcessName(lobAct.ibusBpmActivity.ibusBpmProcess.icdoBpmProcess.name) == busConstant.Map_Enroll_Retiree_Insurance_Plans ||
                busWorkflowHelper.GetWorkflowProcessIdByBpmProcessName(lobAct.ibusBpmActivity.ibusBpmProcess.icdoBpmProcess.name) == busConstant.Map_MSS_Enroll_COBRA_Insurance_Plans) //PIR 18493
            {
                busWssPersonAccountEnrollmentRequest lobjEnrollmentRequest = new busWssPersonAccountEnrollmentRequest();
                if (lobjEnrollmentRequest.FindWssPersonAccountEnrollmentRequest(lintReferenceId))
                {
                    if (lobjEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.enrollment_type_value == busConstant.EnrollmentTypeDBRetirement)
                    {
                        aobjWorkflowActivityInfo.ihstLaunchParameters = new Hashtable();
                        aobjWorkflowActivityInfo.istrURL = "wfmViewRequestPensionPlanRetirementEnrollmentMaintenanceLOB";
                        aobjWorkflowActivityInfo.ihstLaunchParameters.Add("aintRequestId", lintReferenceId);
                    }
                    else if (lobjEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.enrollment_type_value == busConstant.EnrollmentTypeDBOptional)
                    {
                        aobjWorkflowActivityInfo.ihstLaunchParameters = new Hashtable();
                        aobjWorkflowActivityInfo.istrURL = "wfmViewRequestPensionPlanMainRetirementOptionalEnrollmentMaintenanceLOB";
                        aobjWorkflowActivityInfo.ihstLaunchParameters.Add("aintRequestId", lintReferenceId);
                    }
                    else if (lobjEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.enrollment_type_value == busConstant.EnrollmentTypeDBElectedOfficial)
                    {
                        aobjWorkflowActivityInfo.ihstLaunchParameters = new Hashtable();
                        aobjWorkflowActivityInfo.istrURL = "wfmViewRequestDBElectedOfficialMaintenanceLOB";
                        aobjWorkflowActivityInfo.ihstLaunchParameters.Add("aintRequestId", lintReferenceId);
                    }
                    else if (lobjEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.enrollment_type_value == busConstant.EnrollmentTypeDCOptional)
                    {
                        aobjWorkflowActivityInfo.ihstLaunchParameters = new Hashtable();
                        aobjWorkflowActivityInfo.istrURL = "wfmViewRequestPensionPlanDCRetirementEnrollmentMaintenanceLOB";
                        aobjWorkflowActivityInfo.ihstLaunchParameters.Add("aintRequestId", lintReferenceId);
                    }
                    else if (lobjEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.enrollment_type_value == busConstant.EnrollmentTypeDCRetirement)
                    {
                        aobjWorkflowActivityInfo.ihstLaunchParameters = new Hashtable();
                        aobjWorkflowActivityInfo.istrURL = "wfmViewRquestPensionPlanDCRetirementEnrollmentMaintenance";
                        aobjWorkflowActivityInfo.ihstLaunchParameters.Add("aint_request_id", lintReferenceId);
                    }
                    else if (lobjEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.enrollment_type_value == busConstant.EnrollmentTypeGHDV)
                    {
                        aobjWorkflowActivityInfo.ihstLaunchParameters = new Hashtable();
                        aobjWorkflowActivityInfo.istrURL = "wfmViewRequestGHDVEnrollmentMaintenanceLOB";
                        aobjWorkflowActivityInfo.ihstLaunchParameters.Add("aintRequestId", lintReferenceId);
                    }
                    else if (lobjEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.enrollment_type_value == busConstant.EnrollmentTypeLife)
                    {
                        aobjWorkflowActivityInfo.ihstLaunchParameters = new Hashtable();
                        aobjWorkflowActivityInfo.istrURL = "wfmViewRequestLifeEnrollmentMaintenanceLOB";
                        aobjWorkflowActivityInfo.ihstLaunchParameters.Add("aintRequestId", lintReferenceId);
                    }
                    else if (lobjEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.enrollment_type_value == busConstant.EnrollmentTypeFlexComp)
                    {
                        aobjWorkflowActivityInfo.ihstLaunchParameters = new Hashtable();
                        aobjWorkflowActivityInfo.istrURL = "wfmViewRequestFlexCompEnrollmentMaintenanceLOB";
                        aobjWorkflowActivityInfo.ihstLaunchParameters.Add("aintRequestId", lintReferenceId);
                    }
                }
            }
            else if (busWorkflowHelper.GetWorkflowProcessIdByBpmProcessName(lobAct.ibusBpmActivity.ibusBpmProcess.icdoBpmProcess.name) == busConstant.Map_ACH_Pull_For_IBS_Insurance ||
                busWorkflowHelper.GetWorkflowProcessIdByBpmProcessName(lobAct.ibusBpmActivity.ibusBpmProcess.icdoBpmProcess.name) == busConstant.Map_ACH_Pull_For_Insurance ||
                busWorkflowHelper.GetWorkflowProcessIdByBpmProcessName(lobAct.ibusBpmActivity.ibusBpmProcess.icdoBpmProcess.name) == busConstant.Map_ACH_Pull_For_Retirement ||
                busWorkflowHelper.GetWorkflowProcessIdByBpmProcessName(lobAct.ibusBpmActivity.ibusBpmProcess.icdoBpmProcess.name) == busConstant.Map_ACH_Pull_For_DeferredCompensation)
            {
                if (aobjWorkflowActivityInfo.istrURL == "wfmDepositTapeMaintenance")
                {
                    aobjWorkflowActivityInfo.ihstLaunchParameters.Add("aint_activity_instance_id", lobAct.icdoBpmActivityInstance.activity_instance_id);
                }
            }
            else if (busWorkflowHelper.GetWorkflowProcessIdByBpmProcessName(lobAct.ibusBpmActivity.ibusBpmProcess.icdoBpmProcess.name) == busConstant.Map_Initialize_Process_Beneficiary_Auto_Refund_Workflow)
            {
                //process instance is not loaded thats y loaded here again. for PIR - 1724

                string lstradditional_parameter1 = lobAct.GetBpmParameterValueByType<string>("additional_parameter1");

                int lintBeneficiaryPersonId = 0;
                if (!String.IsNullOrEmpty(lstradditional_parameter1))
                    lintBeneficiaryPersonId = Convert.ToInt32(lstradditional_parameter1);

                aobjWorkflowActivityInfo.ihstLaunchParameters = new Hashtable();
                aobjWorkflowActivityInfo.ihstLaunchParameters.Add("member_person_id", lintPersonId);
                aobjWorkflowActivityInfo.ihstLaunchParameters.Add("recipient_person_id", lintBeneficiaryPersonId);
                aobjWorkflowActivityInfo.ihstLaunchParameters.Add("benefit_application_id", lintReferenceId);
            }  //PIR-10697 Start Sets the parameter required for the Find method in the Launching screen
            else if (busWorkflowHelper.GetWorkflowProcessIdByBpmProcessName(lobAct.ibusBpmActivity.ibusBpmProcess.icdoBpmProcess.name) == busConstant.MapMSSHDVAnnualEnrollment)
            {
                busWssPersonAccountEnrollmentRequest lobjEnrollmentRequest = new busWssPersonAccountEnrollmentRequest();
                if (lobjEnrollmentRequest.FindWssPersonAccountEnrollmentRequest(lintReferenceId))
                {
                    if (lobjEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.enrollment_type_value == busConstant.EnrollmentTypeGHDV)
                    {
                        aobjWorkflowActivityInfo.ihstLaunchParameters = new Hashtable();
                        aobjWorkflowActivityInfo.istrURL = "wfmViewRequestGHDVEnrollmentMaintenanceLOB";
                        aobjWorkflowActivityInfo.ihstLaunchParameters.Add("aintRequestId", lintReferenceId);
                    }
                }
            }
            else if (busWorkflowHelper.GetWorkflowProcessIdByBpmProcessName(lobAct.ibusBpmActivity.ibusBpmProcess.icdoBpmProcess.name) == busConstant.MSSLifeInsuranceAnnualEnrollment)
            {
                busWssPersonAccountEnrollmentRequest lobjEnrollmentRequest = new busWssPersonAccountEnrollmentRequest();
                if (lobjEnrollmentRequest.FindWssPersonAccountEnrollmentRequest(lintReferenceId))
                {
                    if (lobjEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.enrollment_type_value == busConstant.EnrollmentTypeLife)
                    {
                        aobjWorkflowActivityInfo.ihstLaunchParameters = new Hashtable();
                        aobjWorkflowActivityInfo.istrURL = "wfmViewRequestLifeEnrollmentMaintenanceLOB";
                        aobjWorkflowActivityInfo.ihstLaunchParameters.Add("aintRequestId", lintReferenceId);
                    }
                }
            }
            else if (busWorkflowHelper.GetWorkflowProcessIdByBpmProcessName(lobAct.ibusBpmActivity.ibusBpmProcess.icdoBpmProcess.name) == busConstant.MSSFlexcompAnnualEnrollment)
            {
                busWssPersonAccountEnrollmentRequest lobjEnrollmentRequest = new busWssPersonAccountEnrollmentRequest();
                if (lobjEnrollmentRequest.FindWssPersonAccountEnrollmentRequest(lintReferenceId))
                {
                    if (lobjEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.enrollment_type_value == busConstant.EnrollmentTypeFlexComp)
                    {
                        aobjWorkflowActivityInfo.ihstLaunchParameters = new Hashtable();
                        aobjWorkflowActivityInfo.istrURL = "wfmViewRequestFlexCompEnrollmentMaintenanceLOB";
                        aobjWorkflowActivityInfo.ihstLaunchParameters.Add("aintRequestId", lintReferenceId);
                    }
                }
            }//PIR-10697 End  
            else if (busWorkflowHelper.GetWorkflowProcessIdByBpmProcessName(lobAct.ibusBpmActivity.ibusBpmProcess.icdoBpmProcess.name) == busConstant.WSSProcessUpdateFlexCompPlan)//PIR-10820 Start
            {
                busWssPersonAccountEnrollmentRequest lobjEnrollmentRequest = new busWssPersonAccountEnrollmentRequest();
                if (lobjEnrollmentRequest.FindWssPersonAccountEnrollmentRequest(lintReferenceId))
                {
                    if (lobjEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.enrollment_type_value == busConstant.EnrollmentTypeFlexComp)
                    {
                        aobjWorkflowActivityInfo.ihstLaunchParameters = new Hashtable();
                        aobjWorkflowActivityInfo.istrURL = "wfmViewRequestFlexCompEnrollmentMaintenanceLOB";
                        aobjWorkflowActivityInfo.ihstLaunchParameters.Add("aintRequestId", lintReferenceId);
                    }
                }
            }
            else if (busWorkflowHelper.GetWorkflowProcessIdByBpmProcessName(lobAct.ibusBpmActivity.ibusBpmProcess.icdoBpmProcess.name) == busConstant.Map_Process_Uncashed_Benefit_Checks &&
                (busWorkflowHelper.GetWorkflowActivityIdByBpmActivityName(lobAct.ibusBpmActivity.ibusBpmProcess.icdoBpmProcess.name, lobAct.ibusBpmActivity.icdoBpmActivity.name) == 168 ||
                busWorkflowHelper.GetWorkflowActivityIdByBpmActivityName(lobAct.ibusBpmActivity.ibusBpmProcess.icdoBpmProcess.name, lobAct.ibusBpmActivity.icdoBpmActivity.name) == 283) &&
                aobjWorkflowActivityInfo.istrURL == "wfmPaymentHistoryLookup")
            {
                if(lintOrgId > 0)
                {
                    string lstrOrgCode = busGlobalFunctions.GetOrgCodeFromOrgId(lintOrgId);
                    aobjWorkflowActivityInfo.ihstLaunchParameters = new Hashtable();
                    aobjWorkflowActivityInfo.ihstLaunchParameters.Add("org_code", lstrOrgCode);
                }
            }
            else if (busWorkflowHelper.GetWorkflowProcessIdByBpmProcessName(lobAct.ibusBpmActivity.ibusBpmProcess.icdoBpmProcess.name) == busConstant.Map_PSTD_First_Beneficiary_Application &&
                aobjWorkflowActivityInfo.istrURL == "wfmPayeeAccountLookup")
            {
                aobjWorkflowActivityInfo.ihstLaunchParameters = new Hashtable();
                string lstrPayeePerslinkID = null;
                string lstrOrgCode = null;
                if (lintOrgId > 0)
                {
                    lstrOrgCode = busGlobalFunctions.GetOrgCodeFromOrgId(lintOrgId);
                }
                else if (lintPersonId > 0)
                {
                    lstrPayeePerslinkID = Convert.ToString(lintPersonId);
                }
                aobjWorkflowActivityInfo.ihstLaunchParameters.Add("payeeorgcode", lstrOrgCode);
                aobjWorkflowActivityInfo.ihstLaunchParameters.Add("payee_perslink_id", lstrPayeePerslinkID);
            }
            //F/W Upgrade launch parameters were getting passed as null when the user does not do what he is expected to do as part of the activity.
            utlProcessMaintainance.utlForm lobjForm = null;
            foreach (var item in aobjActivity.iarrItems)
            {
                lobjForm = item as utlProcessMaintainance.utlForm;
                if (lobjForm.IsNotNull() && lobjForm.ienmPageMode == aobjWorkflowActivityInfo.ienmPageMode)
                    break;
                else
                    lobjForm = null;
            }
            if (lobjForm.IsNotNull())
            {
                string lstrParameterValue = string.Empty;
                foreach (utlProcessMaintainance.utlParameter lutlParameter in lobjForm.icolParameters)
                {
                    if (lutlParameter.istrParameterValueSource == "Parameter")
                    {
                        lstrParameterValue = Convert.ToString(aobjWorkflowActivityInfo.ihstLaunchParameters[lutlParameter.istrParameterName]);
                        if (string.IsNullOrEmpty(lstrParameterValue))
                            lstrParameterValue = Convert.ToString(aobjWorkflowActivityInfo.ihstLaunchParameters[lutlParameter.istrFieldName]);
                        if (string.IsNullOrEmpty(lstrParameterValue))
                        {
                            switch (lutlParameter.istrDataType)
                            {
                                case "int":
                                    lstrParameterValue = "0";
                                    break;
                                case "string":
                                    lstrParameterValue = string.Empty;
                                    break;
                            }
                            aobjWorkflowActivityInfo.ihstLaunchParameters[lutlParameter.istrParameterName] = lstrParameterValue;
                        }
                    }
                }
            }
            //PIR-10820 End
        }

        ////venkat - find references
        //protected override busMainBase GetWorkflowActivityInstanceById(int aintActivityInstanceId)
        //{
        //    busActivityInstance lbusActivityInstance = new busActivityInstance() { icdoActivityInstance = new cdoActivityInstance() };
        //    if (lbusActivityInstance.FindActivityInstance(aintActivityInstanceId))
        //    {
        //        lbusActivityInstance.LoadActivity();
        //        lbusActivityInstance.LoadProcessInstance();
        //        lbusActivityInstance.ibusProcessInstance.LoadProcess();
        //        lbusActivityInstance.ibusProcessInstance.LoadPerson();
        //        lbusActivityInstance.ibusProcessInstance.LoadOrganization();
        //        lbusActivityInstance.icdoActivityInstance.istrActivityName = lbusActivityInstance.ibusActivity.icdoActivity.name;
        //        lbusActivityInstance.icdoActivityInstance.istrProcessName = lbusActivityInstance.ibusProcessInstance.ibusProcess.icdoProcess.name;
        //    }
        //    return lbusActivityInstance;
        //}

        public ArrayList GenerateCSVFromCollection(int Aintemployerpayrollheaderid)
        {
            busEmployerPayrollHeader lobjEmployerPayrollHeader = new busEmployerPayrollHeader();
            ArrayList larlstResult = new ArrayList();
            if (lobjEmployerPayrollHeader.FindEmployerPayrollHeader(Aintemployerpayrollheaderid))
            {
                larlstResult.Add(iobjPassInfo.istrFormName + ".csv");
                larlstResult.Add(lobjEmployerPayrollHeader.GenerateCSVFromCollection());
                larlstResult.Add(System.Net.Mime.MediaTypeNames.Application.Octet);
            }
            return larlstResult;
        }
        public ArrayList GenerateElectionResultReport(int aintElectionId) => new busBoardMemberElection().ViewElectionResultsReport(aintElectionId);
        public bool InactiveSelectedOrgContact(string aintOrgIDs, int aintContactId)
        {
            try
            {
                busContact lbusContact = new busContact();
                foreach (string astrOrgId in aintOrgIDs.Split(','))
                {
                    if (!string.IsNullOrEmpty(astrOrgId))
                        lbusContact.btnRemoveOrg_Click(astrOrgId, aintContactId);
                }
                return true;
            }
            catch (Exception _exc)
            {
                ExceptionManager.Publish(_exc);
                return false;
            }
        }
    }
}
