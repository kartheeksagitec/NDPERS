#region Using directives

using System;
using System.Data;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using NeoSpin.BusinessObjects;
using NeoSpin.CustomDataObjects;
using Sagitec.Common;

#endregion

namespace NeoSpin.BusinessTier
{
    public class srvCorrespondence : srvNeoSpin
    {
        public srvCorrespondence()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        protected override ArrayList ValidateNew(string astrFormName, Hashtable ahstParam)
        {
            ArrayList larrErrors = null;
           // iobjPassInfo.iconFramework.Open();
            try
            {
                if (astrFormName == "wfmMASBatchRequestLookup")
                {
                    busMASBatchRequestLookup lobjMASBatchRequest = new busMASBatchRequestLookup();
                    larrErrors = lobjMASBatchRequest.ValidateNew(ahstParam);
                }
            }
            finally
            {
              //  iobjPassInfo.iconFramework.Close();
            }
            return larrErrors;
        }

        public busCorTemplates FindCorrTemplate(int ainttemplateid)
        {
            busCorTemplates lobjCorTemplates = new busCorTemplates();
            if (lobjCorTemplates.FindCorTemplates(ainttemplateid))
            {
            }

            return lobjCorTemplates;
        }


        public busCorTemplatesLookup LoadCorrTemplates(DataTable adtbSearchResult)
        {
            busCorTemplatesLookup lobjCorTemplatesLookup = new busCorTemplatesLookup();
            lobjCorTemplatesLookup.LoadCorTemplates(adtbSearchResult);
            return lobjCorTemplatesLookup;
        }

        public busCorTrackingLookup LoadCorTracking(DataTable adtbSearchResult)
        {
            busCorTrackingLookup lobjCorTrackingLookup = new busCorTrackingLookup();
            lobjCorTrackingLookup.LoadCorTracking(adtbSearchResult);
            return lobjCorTrackingLookup;
        }

        public busMailingLabel NewMailingLabel()
        {
            busMailingLabel lobjMailingLabel = new busMailingLabel();
            lobjMailingLabel.icdoMailingLabel = new cdoMailingLabel();
            lobjMailingLabel.iclcPersonType = new utlCollection<cdoMailingLabelPersonType>();
            lobjMailingLabel.iclcPlan = new utlCollection<cdoMailingLabelPlan>();
            lobjMailingLabel.istrReportTemplateName = "rptMailingLabelAvery";
            return lobjMailingLabel;
        }

        public busMailingLabel FindMailingLabel(int Aintmailinglabelid)
        {
            busMailingLabel lobjMailingLabel = new busMailingLabel();
            if (lobjMailingLabel.FindMailingLabel(Aintmailinglabelid))
            {
                lobjMailingLabel.LoadPersonType();
                lobjMailingLabel.LoadPlan();
            }
            else
            {
                lobjMailingLabel.iclcPersonType = new utlCollection<cdoMailingLabelPersonType>();
                lobjMailingLabel.iclcPlan = new utlCollection<cdoMailingLabelPlan>();
            }
            lobjMailingLabel.istrReportTemplateName = "rptMailingLabelAvery";
            return lobjMailingLabel;
        }

        public busMailingLabelLookup LoadMailingLabels(DataTable adtbSearchResult)
        {
            busMailingLabelLookup lobjMailingLabelLookup = new busMailingLabelLookup();
            lobjMailingLabelLookup.LoadMailingLabel(adtbSearchResult);
            return lobjMailingLabelLookup;
        }

        public ArrayList ViewReport_Click(int aintMailingBatchID)
        {
            return new busMailingLabel().ViewReport_Click(aintMailingBatchID);
        }

        public busMASBatchRequestLookup LoadMASBatchRequests(DataTable adtbSearchResult)
        {
            busMASBatchRequestLookup lobjMASBatchRequestLookup = new busMASBatchRequestLookup();
            lobjMASBatchRequestLookup.LoadMASBatchRequests(adtbSearchResult);
            return lobjMASBatchRequestLookup;
        }

        public busMASBatchRequest NewMASBatchRequest(int aintMASBatchRequestID, string astrBatchRequestType, string astrGroupTypeValue)
        {
            busMASBatchRequest lobjMASBatchRequest = new busMASBatchRequest { icdoMasBatchRequest = new cdoMasBatchRequest() };
            lobjMASBatchRequest.iclcInsurancePlan = new utlCollection<cdoMasBatchRequestPlan>();
            lobjMASBatchRequest.iclcPensionPlan = new utlCollection<cdoMasBatchRequestPlan>();
            lobjMASBatchRequest.iclcNRInsurancePlan = new utlCollection<cdoMasBatchRequestPlan>();
            lobjMASBatchRequest.iclcNRPensionPlan = new utlCollection<cdoMasBatchRequestPlan>();
            if (aintMASBatchRequestID == 0)
            {
                // New MAS Batch Request
                lobjMASBatchRequest.icdoMasBatchRequest.batch_request_type_value = astrBatchRequestType;
                lobjMASBatchRequest.icdoMasBatchRequest.batch_request_type_description =
                            iobjPassInfo.isrvDBCache.GetCodeDescriptionString(3051, astrBatchRequestType);
                lobjMASBatchRequest.icdoMasBatchRequest.group_type_value = astrGroupTypeValue;
                lobjMASBatchRequest.icdoMasBatchRequest.group_type_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(3050, astrGroupTypeValue);
            }
            else
            {
                // Copy MAS Batch Request
                lobjMASBatchRequest = new busMASBatchRequest { icdoMasBatchRequest = new cdoMasBatchRequest() };
                lobjMASBatchRequest.FindMASBatchRequest(aintMASBatchRequestID);
                lobjMASBatchRequest.LoadPlan();
                lobjMASBatchRequest.LoadStatementEffectiveDate();
                lobjMASBatchRequest.icdoMasBatchRequest.mas_batch_request_id = 0;
                lobjMASBatchRequest.icdoMasBatchRequest.bulk_insert_mas_data_flag = busConstant.Flag_No;
                lobjMASBatchRequest.icdoMasBatchRequest.mailing_generate_flag = busConstant.Flag_No;  // Annual Statements - PIR 17506
                lobjMASBatchRequest.icdoMasBatchRequest.ienuObjectState = ObjectState.Insert;
                lobjMASBatchRequest.iarrChangeLog.Add(lobjMASBatchRequest.icdoMasBatchRequest);
                foreach (cdoMasBatchRequestPlan lcdoMASPlan in lobjMASBatchRequest.iclcPensionPlan)
                {
                    lcdoMASPlan.mas_batch_request_id = 0;
                    lcdoMASPlan.mas_batch_request_plan_id = 0;
                    lcdoMASPlan.ienuObjectState = ObjectState.None;
                }
                foreach (cdoMasBatchRequestPlan lcdoMASPlan in lobjMASBatchRequest.iclcInsurancePlan)
                {
                    lcdoMASPlan.mas_batch_request_id = 0;
                    lcdoMASPlan.mas_batch_request_plan_id = 0;
                    lcdoMASPlan.ienuObjectState = ObjectState.None;
                }
                lobjMASBatchRequest.icdoMasBatchRequest.created_date = DateTime.Today;
                if (lobjMASBatchRequest.icdoMasBatchRequest.org_id > 0)
                {
                    lobjMASBatchRequest.LoadOrganization();
                    lobjMASBatchRequest.LoadOrganizationCode();
                }
            }
            lobjMASBatchRequest.icdoMasBatchRequest.action_status_value = busConstant.StatusPending;
            lobjMASBatchRequest.icdoMasBatchRequest.ienuObjectState = ObjectState.Insert;
            lobjMASBatchRequest.EvaluateInitialLoadRules();
            return lobjMASBatchRequest;
        }

        public busMASBatchRequest FindMASBatchRequest(int aintMASBatchRequestID)
        {
            busMASBatchRequest lobjMASBatchRequest = new busMASBatchRequest();
            if (lobjMASBatchRequest.FindMASBatchRequest(aintMASBatchRequestID))
            {
                //if(lobjMASBatchRequest.icdoMasBatchRequest.batch_request_type_value==busConstant.BatchRequestTypeTargeted)
                lobjMASBatchRequest.LoadPlan();
                lobjMASBatchRequest.LoadStatementEffectiveDate();
                lobjMASBatchRequest.LoadOrganizationCode();
                lobjMASBatchRequest.LoadErrors();
                if (lobjMASBatchRequest.icdoMasBatchRequest.group_type_value == busConstant.GroupTypeRetired &&
                    lobjMASBatchRequest.icdoMasBatchRequest.batch_request_type_value == busConstant.BatchRequestTypeIndividual)
                    lobjMASBatchRequest.icdoMasBatchRequest.retired_person_id = lobjMASBatchRequest.icdoMasBatchRequest.person_id;
                if (lobjMASBatchRequest.icdoMasBatchRequest.org_id > 0)
                {
                    lobjMASBatchRequest.LoadOrganization();
                    lobjMASBatchRequest.LoadOrganizationCode();
                }
            }

            return lobjMASBatchRequest;
        }
        public busCustomCorrespondence NewCustomCorrespondence(int aintDeathNotificationID, int aintPersonId)
        {
            busCustomCorrespondence lbusCustomCorrespondence = new busCustomCorrespondence()
            {
                icdoCustomCorrespondence = new DataObjects.doCustomCorrespondence(),
                ibusDeathNotification = new busDeathNotification() { icdoDeathNotification = new cdoDeathNotification() }
            };
            if (lbusCustomCorrespondence.ibusDeathNotification.FindDeathNotification(aintDeathNotificationID))
            {
                if (lbusCustomCorrespondence.ibusDeathNotification.icdoDeathNotification.person_id != 0)
                {
                    lbusCustomCorrespondence.ibusDeathNotification.LoadPerson();
                    lbusCustomCorrespondence.ibusDeathNotification.LoadCorPersonBeneficiary();
                    lbusCustomCorrespondence.LoadDistinctPersonBeneficiary();
                    lbusCustomCorrespondence.LoadCorPersonAccountDependent();
                    lbusCustomCorrespondence.LoadCorPersonAccountBeneficiary();
                }
            }
            return lbusCustomCorrespondence;
        }
    }
}
