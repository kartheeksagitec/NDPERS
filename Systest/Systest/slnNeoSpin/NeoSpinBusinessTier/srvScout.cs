#region Using directives

using System;
using System.Data;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Collections.ObjectModel;
using NeoSpin.BusinessObjects;
using NeoSpin.CustomDataObjects;
using Sagitec.Common;
using Sagitec.DBUtility;
using System.Globalization;


#endregion

namespace NeoSpin.BusinessTier
{
    public class srvScout : srvNeoSpin
	{
		public srvScout()
		{
			//
			// TODO: Add constructor logic here
			//
		}
        protected override IDbConnection GetDBConnection()
        {
            return DBFunction.GetDBConnection("scout");
        }
		public busUsecase FindUsecase(int aintUsecaseId)
		{
			busUsecase lobjUsecase = new busUsecase();
			if (lobjUsecase.FindUsecase(aintUsecaseId))
			{
				lobjUsecase.LoadUsecaseFlow();
			}
			return lobjUsecase;
		}

		public busUsecaseLookup LoadUsecases(DataTable adtbSearchResult)
		{
			busUsecaseLookup lobjUsecaseLookup = new busUsecaseLookup();
			lobjUsecaseLookup.LoadUsecases(adtbSearchResult);
			return lobjUsecaseLookup;
		}

        public busUseCaseFlowLookup LoadUsecaseFlow(DataTable adtbSearchResult)
        {
            busUseCaseFlowLookup lobjUsecaseFlowLookup = new busUseCaseFlowLookup();
            lobjUsecaseFlowLookup.LoadUseCaseFlow(adtbSearchResult);
            return lobjUsecaseFlowLookup;
        }


		public busUsecaseFlow FindUsecaseFlow(int aintFlowId)
		{
			busUsecaseFlow lobjUsecaseFlow = new busUsecaseFlow();
			if (lobjUsecaseFlow.FindUsecaseFlow(aintFlowId))
			{
				lobjUsecaseFlow.LoadUsecase();
			}
			return lobjUsecaseFlow;
		}
        public busRequirementHistory FindRequirementHistory(int aintRequirementHistoryId)
        {
            busRequirementHistory lobjRequirementHistory = new busRequirementHistory();
            if (lobjRequirementHistory.FindRequirementHistory(aintRequirementHistoryId))
            {
                lobjRequirementHistory.LoadRequirementKey();                           
            }
            
            return lobjRequirementHistory;
        }
		public busUsecaseFlow NewUsecaseFlow(int aintUsecaseId)
		{
			busUsecaseFlow lobjUsecaseFlow = new busUsecaseFlow();
			lobjUsecaseFlow.icdoUsecaseFlow = new cdoUsecaseFlow();
			lobjUsecaseFlow.icdoUsecaseFlow.usecase_id = aintUsecaseId;
			lobjUsecaseFlow.LoadUsecase();
			return lobjUsecaseFlow;
		}

		public busRequirement FindRequirement(int aintRequirementId)
		{
			busRequirement lobjRequirement = new busRequirement();
			if (lobjRequirement.FindRequirement(aintRequirementId))
			{                
                lobjRequirement.LoadRequirementHistory();
                lobjRequirement.LoadParentRequirements();
                lobjRequirement.LoadChildRequirements();
                lobjRequirement.LoadUsecaseFlow();
                lobjRequirement.LoadRequirementKey();
			}
			return lobjRequirement;
		}

		public busRequirement NewRequirement()
		{
			busRequirement lobjRequirement = new busRequirement();
			lobjRequirement.icdoRequirement = new cdoRequirement();
            return lobjRequirement;
		}

		public busRequirementLookup LoadRequirement(DataTable adtbSearchResult)
		{
			busRequirementLookup lobjRequirementLookup = new busRequirementLookup();
			lobjRequirementLookup.LoadRequirements(adtbSearchResult);
			return lobjRequirementLookup;
		}

        //public busTraceabilityLookup LoadTraceability(DataTable adtbSearchResult)
        //{
        //    busTraceabilityLookup lobjTraceabilityLookup = new busTraceabilityLookup();
        //    lobjTraceabilityLookup.LoadTraceability(adtbSearchResult);
        //    return lobjTraceabilityLookup;
        //}

		public busTestcase FindTestcase(int aintTestcaseId)
		{
			busTestcase lobjTestcase = new busTestcase();
			if (lobjTestcase.FindTestcase(aintTestcaseId))
			{
				lobjTestcase.LoadTestcaseDetail();
			}
			return lobjTestcase;
		}

		public busTestcase NewTestcase()
		{
			busTestcase lobjTestcase = new busTestcase();
			lobjTestcase.icdoTestcase = new cdoTestcase();
			//Assign default values;
			lobjTestcase.icdoTestcase.status_value = "PROG";  //In progress status
			return lobjTestcase;
		}

		public busTestcaseDetail FindTestcaseDetail(int aintTestcaseDtlId)
		{
			busTestcaseDetail lobjTestcaseDetail = new busTestcaseDetail();
			if (lobjTestcaseDetail.FindTestcaseDetail(aintTestcaseDtlId))
			{
				//lobjTestcaseDetail.LoadTestcaseDetail();
			}
			return lobjTestcaseDetail;
		}

		public busTestcaseDetail NewTestcaseDetail(int aintTestcaseId)
		{
			busTestcaseDetail lobjTestcaseDetail = new busTestcaseDetail();
			lobjTestcaseDetail.icdoTestcaseDetail = new cdoTestcaseDetail();
			//Assign default values;
			lobjTestcaseDetail.icdoTestcaseDetail.testcase_id = aintTestcaseId;
			return lobjTestcaseDetail;
		}

		public busTestcaseLookup LoadTestcases(DataTable adtbSearchResult)
		{
			busTestcaseLookup lobjTestcaseLookup = new busTestcaseLookup();
			lobjTestcaseLookup.LoadTestcases(adtbSearchResult);
			return lobjTestcaseLookup;
		}

		public busPir FindPir(int aintPirId)
		{
			busPir lobjPir = new busPir();
			if (lobjPir.FindPir(aintPirId))
			{
                if (lobjPir.icdoPir.priority_date != DateTime.MinValue)
                {
                    lobjPir.icdoPir.istrPriorityDate =
                        lobjPir.icdoPir.priority_date.ToString("MM/yyyy", CultureInfo.CreateSpecificCulture("en-US"));
                }
				lobjPir.LoadPirHistory();
                lobjPir.LoadAssignedTo();
                lobjPir.LoadReportedBy();
                lobjPir.LoadCreatedByUser();
                //List<string> llstResult = LoadPIRAttachments(iobjPassInfo.isrvDBCache.GetPathInfo("PIRA") + "\\" + aintPirId);
                //lobjPir.LoadPIRAttachment(llstResult);
                lobjPir.iclbPIRAttchment = GetPIRAttachmentsCollection(iobjPassInfo.isrvDBCache.GetPathInfo("PIRA") + "\\" + aintPirId);

            }
			return lobjPir;
		}        

        public busPir NewPir()
		{
			busPir lobjPir = new busPir();
			lobjPir.icdoPir = new cdoPir();
			//Assign default values;            
            lobjPir.icdoPir.severity_value = busConstant.PirSeverityImportant;
            lobjPir.icdoPir.priority_value = busConstant.PirPriorityMedium;

            lobjPir.icdoPir.status_value = busConstant.PirStatusLogged;
            lobjPir.icdoPir.assigned_to_id = busConstant.USER_SERIAL_ID_AssignedToIT;
            lobjPir.icdoPir.reported_by_id = Convert.ToInt32(iobjPassInfo.iintUserSerialID);
            lobjPir.LoadLoggedUser();
            if(lobjPir.ibusLoggedUser.IsNotNull())
                lobjPir.istrCreateByUserFullName = Convert.ToString(lobjPir.ibusLoggedUser.icdoUser.User_full_name);
            lobjPir.icdoPir.status_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(40, lobjPir.icdoPir.status_value);
            return lobjPir;
		}

		public busPirLookup LoadPirs(DataTable adtbSearchResult)
		{
			busPirLookup lobjPirLookup = new busPirLookup();
			lobjPirLookup.LoadPirs(adtbSearchResult);
			return lobjPirLookup;
		}


        //FW Upgrade : Bttton btnReadyforSystest, btnReleaseToUAT, btnReleasetoProd was not working
        public busPirLookup btnReleaseToSystest(ArrayList arrSelectedObjects)
        {
            busPirLookup lobjPirLookup = new busPirLookup();
            lobjPirLookup.UpdatePirs(arrSelectedObjects, busConstant.PIRStatusReadyForSystest); 
            return lobjPirLookup;
        }

        public busPirLookup btnReleaseToUAT(ArrayList arrSelectedObjects)
        {
            busPirLookup lobjPirLookup = new busPirLookup();
            lobjPirLookup.UpdatePirs(arrSelectedObjects, busConstant.PIRStatusReadyForUAT);
            return lobjPirLookup;
        }

        public busPirLookup btnReleaseToPROD(ArrayList arrSelectedObjects)
        {
            busPirLookup lobjPirLookup = new busPirLookup();
            lobjPirLookup.UpdatePirs(arrSelectedObjects, busConstant.PIRStatusReadyForPROD);
            return lobjPirLookup;
		}
		
		
		
		public Collection<busScreen> GetScreens() => new busPir().GetScreens();

		public Collection<busScreen> GetScreensInModule(string astrModule)
		{
			Collection<busScreen> lclbScreens = new Collection<busScreen>();
			Collection<utlFunctionalModule> lcolFunctionalModule =
				iobjPassInfo.isrvMetaDataCache.GetScreens();
			busScreen lobjScreen = null;
			foreach (utlFunctionalModule lobjFunctionalModule in lcolFunctionalModule)
			{
				if (lobjFunctionalModule.istrFunctionalModule == astrModule)
				{
					foreach (string lstrScreen in lobjFunctionalModule.icolScreens)
					{
						lobjScreen = new busScreen();
						lobjScreen.istrScreenName = lstrScreen;
						lclbScreens.Add(lobjScreen);
					}
				}
			}
			return lclbScreens;
		}

		public Collection<busScreen> GetFunctionalModule()
		{
			Collection<busScreen> lclbScreens = new Collection<busScreen>();
			Collection<utlFunctionalModule> lcolFunctionalModule =
				iobjPassInfo.isrvMetaDataCache.GetScreens();
			busScreen lobjScreen = null;
			foreach (utlFunctionalModule lobjFunctionalModule in lcolFunctionalModule)
			{
				lobjScreen = new busScreen();
				lobjScreen.istrScreenName = lobjFunctionalModule.istrFunctionalModule;
				lclbScreens.Add(lobjScreen);
			}
			return lclbScreens;
		}

        public busRequirementUsecaseFlowCrossref FindRequirementUsecaseFlowCrossref(int AintRequirementFlowCrossrefId)
		{
			busRequirementUsecaseFlowCrossref lobjRequirementUsecaseFlowCrossref = new busRequirementUsecaseFlowCrossref();
            if (lobjRequirementUsecaseFlowCrossref.FindRequirementUsecaseFlowCrossref(AintRequirementFlowCrossrefId))
			{
                lobjRequirementUsecaseFlowCrossref.LoadRequirement();
                lobjRequirementUsecaseFlowCrossref.LoadUsecaseFlow();
			}
			return lobjRequirementUsecaseFlowCrossref;
		}

        public busRequirementUsecaseFlowCrossref NewRequirementUsecaseFlowCrossref(int AintRequirementId)
        {
            busRequirementUsecaseFlowCrossref lobjRequirementUsecaseFlowCrossref = new busRequirementUsecaseFlowCrossref();
            lobjRequirementUsecaseFlowCrossref.icdoRequirementUsecaseFlowCrossref = new cdoRequirementUsecaseFlowCrossref();
            lobjRequirementUsecaseFlowCrossref.icdoRequirementUsecaseFlowCrossref.requirement_id = AintRequirementId;
            lobjRequirementUsecaseFlowCrossref.LoadRequirement();
            lobjRequirementUsecaseFlowCrossref.LoadUsecaseFlow();
            return lobjRequirementUsecaseFlowCrossref;
        }

        public ArrayList DownloadPIRAttachmentSrvScout(int aintPirId,string astrFilePath)
        {
            ArrayList larlstResult = new ArrayList();
            larlstResult.Add(astrFilePath);
            astrFilePath = iobjPassInfo.isrvDBCache.GetPathInfo("PIRA") + "\\" + aintPirId +"\\"+ astrFilePath;
            larlstResult.Add(DownloadPIRAttachment(astrFilePath));
            larlstResult.Add(System.Net.Mime.MediaTypeNames.Application.Octet);
            return larlstResult;
        }

    }
}
