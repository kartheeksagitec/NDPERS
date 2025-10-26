var nsUserFunctions = nsUserFunctions || {};
var dots = 0;
var dotmax = 10;
var intervalId;
nsUserFunctions = {
    iblnMenuItemClicked: false,
    idteLastRequestCompletedTime: new Date(),
    istrLookupActivityName: "",
    istrLookupProcessName: "",
    istrLookupActNavParams: "",
    OpenDocument: function (e) {
        var lobjBtnInfo = nsCommon.GetEventInfo(e);
        var ActiveDivID = lobjBtnInfo.ActiveDivID;
        var lbtnSelf = lobjBtnInfo.lbtnSelf;
        var lintSelectedIndex = lobjBtnInfo.lintSelectedIndex;
        var ldictControlAttr = MVVMGlobal.GetControlAttribute(lbtnSelf, "GetAllAttr", ActiveDivID, true);
        ldictControlAttr = ldictControlAttr != null ? ldictControlAttr : {};
        var RelatedGridID = "";
        RelatedGridID = $(lbtnSelf).attr(nsConstants.SFW_RELATED_CONTROL) || ldictControlAttr[nsConstants.SFW_RELATED_CONTROL];
        var lobjGridWidget;
        var lobjSchedulerWidget;
        var lblnIsListView = false;
        var ldictParams = [];
        if (RelatedGridID != null) {
            var lobjRelatedControl = nsCommon.CheckGridOrListView(ActiveDivID, RelatedGridID);
            if (lobjRelatedControl.NotFound)
                return false;
            lblnIsListView = lobjRelatedControl.blnIsListView;
            lobjGridWidget = nsCommon.GetWidgetByActiveDivIdAndControlId(ActiveDivID, lobjRelatedControl.RelatedControlId);
            if (lobjGridWidget == undefined || lobjGridWidget.jsObject == undefined) {
                return false;
            }
            ldictParams = nsCommon.GetSelectedRows(lbtnSelf, e, lblnIsListView);
        }
        if (ldictParams.larrRows && ldictParams.larrRows.length == 1) {
            LaunchFileNetImageURL(ldictParams.larrRows[0].PrimaryKey);
        }
        return false;
    },
    //Framework Supported JS
    ShowWait: function (e) {
        intervalId = setInterval('StartShowGenerating()', 50);
        return true;
    },

    RefreshCenterLeft: function (e) {
        ns.BuildLeftForm(nsConstants.BPM_WORKFLOW_CENTERLEFT_MAINTENANCE, undefined);
        return false;
    },

    // Framwork versoin 6.0.2.1 Provide support for dynamic title for charts
    GetChartTitle: function (e) {
        e.data // DomainModel Object
        e.chartId // Id of the chart
        return "Dyanamic Title";
    },

    MainframeReport: function (e) {
        var lActiveDivId = e.context.activeDivID;
        var ldomDiv = $("#" + lActiveDivId);
        var PersonID = ldomDiv.find("#lblPersonId").text();
        if (PersonID != "") {
            window.open("https://app.powerbigov.us/groups/98996f9e-1afe-4222-9e09-56f6b4ed6de6/reports/d2da255f-7b62-4dbc-9ad0-64949ab6d258/ReportSection5f6725f1487c8a076dba?filter=PersXref%2FPersLinkID%20eq%20" + PersonID, "_blank");
        }
    },

    PrintCenterMiddle: function (e) {        
        nsEvents.btnPrintPage_Click(this);
        return false;
    },
    // Framework version 6.0.0.31.0 - Added function to change the language in case needed.
    SetLanguage: function (e) {
        //add solution side logic to get language.  
        var lstrLanguage = "en-US";
        //ns.iblnUseStoreDefaultsForLookup = false; //Set this to false only if we dont want to use Store Search functionality
        return lstrLanguage;
    },
    BeforeMenuNavigate: function (e) {
        if (ns.viewModel.currentModel.indexOf("wfmPaymentHistoryLookup") == 0) {
            nsEvents.OnDeleteFormClick(ns.viewModel.currentModel);
        }
        if (nsUserFunctions.iblnMenuItemClicked) {
            return false;
        }
        else {
            nsUserFunctions.iblnMenuItemClicked = ns.blnMenuClicked;
            return true;
        }
    },
    ChangeDisplayError: function (e) {
        nsUserFunctions.iblnMenuItemClicked = false;
    },
    HideActivityInfo: function (e) {
        $("#masterPageActivityInstanceDetails").empty();
        $("divBpmActivityInstanceDetails").empty();
        return true;
    },
    //Fix to stop displaying knowtion unsaved form on every screen.
    SetUnSavedFormIcon: function (e) {
        if (

            (e.context.activeDivID.indexOf("wfmBPMWorkflowCenterLeftMaintenance") == 0)

        ) {
            return false;
        }
        return true;
    },
   
    NewBenefitCalculationHardErrorShow: function (e) {
        var lActiveDivId = e.context.activeDivID;
        var ldomDiv = $("#" + lActiveDivId);
        if (lActiveDivId.indexOf("wfmRetirementBenefitCalculationLookup") >= 0) {
            if (ldomDiv.find("#ddlBenefitAccountTypeValue").length > 0) {
                var valueOfType = ldomDiv.find("#ddlBenefitAccountTypeValue").val();
                if (valueOfType == "") {
                    nsCommon.DispalyError("1901 Benefit Type is required.", lActiveDivId);
                    return false;
                }
                else if(valueOfType == "PSTD" || valueOfType == "RFND") {
                    nsCommon.DispalyError("1968 Estimate cannot be done for Benefit Type of 'Post Retirement Death' and 'Refund' ", lActiveDivId);
                    return false;
                }
                else {
                return true;
            }    
            }
        }
        return true;
    },


    //FW Upgrade PIR ID :	15595

    ReportHardErrorShow: function (e) {
        var lActiveDivId = e.context.activeDivID;
        var ldomDiv = $("#" + lActiveDivId);
        if (lActiveDivId.indexOf("wfmReportClient") >= 0) {
            if (ldomDiv.find("#ddlReports").length > 0) {
                var valueOfType = ldomDiv.find("#ddlReports").val();
                if (valueOfType == "") {
                    nsCommon.DispalyError("No report selected. Please select the report to be generated.", lActiveDivId);
                    return false;
                }
                else {
                    if ($("#" + ns.viewModel.currentModel).find('#wfmReportClient').val() != "") {
                        $('#ReportFrame').css('height', '1200px');
                        //$('#ReportFrame').css('width', '1800px'); -- commented because of some reports increase in width at runtime depends on data then this with limitation hides part of report
                    }
                    return true;
                }
            }
        }
        return true;
    },

    ClearReassignTreeNode: function (e) {
        //ns.tabsTreeView.remove("wfmBpmReassignWorkMaintenance0");
        var dataitem = ns.tabsTreeView.getNodeDataByDivID("wfmBpmReassignWorkMaintenance0");
        if (dataitem != undefined)
            MVVMGlobal.RemoveForm([], dataitem);
        return true;
    },

    SavePlanMemberTypeHardErrorShow: function (e) {
        var lActiveDivId = e.context.activeDivID;
        var ldomDiv = $("#" + e.context.activeDivID);
        if (lActiveDivId.indexOf("wfmPlanMemberTypeMaintenance") >= 0) {
            if (ldomDiv.find("#ddlJobClassValue").length > 0 && ldomDiv.find("#ddlEmploymentTypeValue").length > 0 && ldomDiv.find("#ddlMemberTypeValue").length > 0) {
                var valueOfJobClass = ldomDiv.find("#ddlJobClassValue").val();
                var valueOfEmploymentType = ldomDiv.find("#ddlEmploymentTypeValue").val();
                var valueOfMemberType = ldomDiv.find("#ddlMemberTypeValue").val();

                if (valueOfJobClass == "" && valueOfEmploymentType == "" && valueOfMemberType == "") {
                    nsCommon.DispalyError(" Job Class is required! <br/> Employment Type is required! <br/> Member Type is required! ", lActiveDivId);
                    return false;
                }

                else if (valueOfJobClass == "" && valueOfEmploymentType == "" ) {
                    nsCommon.DispalyError(" Job Class is required! <br/> Employment Type is required! ", lActiveDivId);
                    return false;
                }

               else if (valueOfEmploymentType == "" && valueOfMemberType == "") {
                    nsCommon.DispalyError(" Employment Type is required! <br/> Member Type is required! ", lActiveDivId);
                    return false;
                }

               else if (valueOfJobClass == ""  && valueOfMemberType == "") {
                    nsCommon.DispalyError(" Job Class is required! <br/> Member Type is required! ", lActiveDivId);
                    return false;
                }

               else if (valueOfJobClass == "" ) {
                    nsCommon.DispalyError(" Job Class is required!  ", lActiveDivId);
                    return false;
                }

                else if ( valueOfEmploymentType == "" ) {
                    nsCommon.DispalyError(" Employment Type is required!  ", lActiveDivId);
                    return false;
                }

                else if ( valueOfMemberType == "") {
                    nsCommon.DispalyError(" Member Type is required! ", lActiveDivId);
                    return false;
                }

                return true;
            }
        }
    },

    UpdateHistoryChangeDateHardError: function (e) {
        var lActiveDivId = e.context.activeDivID;
        var ldomDiv = $("#" + lActiveDivId);
        if (lActiveDivId.indexOf("wfmUpdateHistoryMaintenance") >= 0) {
            if (ldomDiv.find("#ddlUHPlanID").length > 0) {
                var valueOfPersonId = ldomDiv.find("#txtPersonID").val();
                var valueOfPlanId = ldomDiv.find("#ddlUHPlanID").val();
                var valueOfOldDate = ldomDiv.find("#txtOldDate").val();
                var valueOfNewDate = ldomDiv.find("#txtNewDate").val();
                if (valueOfPersonId == "" && valueOfPlanId == "0" && valueOfOldDate == "" && valueOfNewDate == "") {
                    nsCommon.DispalyError("Person Id is required. <br/> Plan Id is required. <br/> Old Date is required. <br/> New Date is required.", lActiveDivId);
                    return false;
                }
                else if (valueOfPersonId == "" && valueOfPlanId == "0" && valueOfOldDate == "") {
                    nsCommon.DispalyError("Person Id is required. <br/> Plan Id is required. <br/> Old Date is required.", lActiveDivId);
                    return false;
                }
                else if (valueOfPersonId == "" && valueOfPlanId == "0" && valueOfNewDate == "") {
                    nsCommon.DispalyError("Person Id is required. <br/> Plan Id is required. <br/> New Date is required.", lActiveDivId);
                    return false;
                }
                else if (valueOfPersonId == "" && valueOfOldDate == "" && valueOfNewDate == "") {
                    nsCommon.DispalyError("Person Id is required. <br/> Old Date is required. <br/> New Date is required.", lActiveDivId);
                    return false;
                }
                else if (valueOfPlanId == "0" && valueOfOldDate == "" && valueOfNewDate == "") {
                    nsCommon.DispalyError("Plan Id is required. <br/> Old Date is required. <br/> New Date is required.", lActiveDivId);
                    return false;
                }
                else if (valueOfOldDate == "" && valueOfNewDate == "") {
                    nsCommon.DispalyError("Old Date is required. <br/> New Date is required.", lActiveDivId);
                    return false;
                }
                else if (valueOfPersonId == "" && valueOfPlanId == "0") {
                    nsCommon.DispalyError("Person Id is required. <br/> Plan Id is required.", lActiveDivId);
                    return false;
                }
                else if (valueOfPersonId == "" && valueOfOldDate == "") {
                    nsCommon.DispalyError("Person Id is required. <br/> Old Date is required.", lActiveDivId);
                    return false;
                }
                else if (valueOfPersonId == "" && valueOfNewDate == "") {
                    nsCommon.DispalyError("Person Id is required. <br/> New Date is required.", lActiveDivId);
                    return false;
                }
                else if (valueOfPlanId == "0" && valueOfOldDate == "") {
                    nsCommon.DispalyError("Plan Id is required. <br/> Old Date is required.", lActiveDivId);
                    return false;
                }
                else if (valueOfPlanId == "0" && valueOfNewDate == "") {
                    nsCommon.DispalyError("Plan Id is required. <br/> New Date is required.", lActiveDivId);
                    return false;
                }
                else if (valueOfPersonId == "") {
                    nsCommon.DispalyError("Person Id is required", lActiveDivId);
                    return false;
                }
                else if (valueOfPlanId == "0") {
                    nsCommon.DispalyError("Plan Id is required.", lActiveDivId);
                    return false;
                }
                else if (valueOfOldDate == "") {
                    nsCommon.DispalyError("Old Date is required.", lActiveDivId);
                    return false;
                }
                else if (valueOfNewDate == "") {
                    nsCommon.DispalyError("New Date is required.", lActiveDivId);
                    return false;
                }
                else {
                return true;
            }    
            }
        }
        return true;
    },    

    UploadDocument: function (e) {
        
        var lstrFormName = nsCommon.GetFormNameFromDivID(e.context.activeDivID);
        ns.DirtyData[e.context.activeDivID] = { HeaderData: {}, istrFormName: lstrFormName, KeysData: {} };
        ns.DirtyData[e.context.activeDivID].HeaderData = { MaintenanceData: {} };
        ns.DirtyData[e.context.activeDivID].HeaderData.MaintenanceData = ns.viewModel[e.context.activeDivID].HeaderData.MaintenanceData;
        ns.DirtyData[e.context.activeDivID].KeysData = ns.viewModel[e.context.activeDivID].KeysData;
        switch (lstrFormName) {
            case "wfmPirMaintenance":
                {
                    ns.DirtyData[e.context.activeDivID].HeaderData.MaintenanceData["PIRId"] = $("#" + e.context.activeDivID).find("#lblPirId").text();
                }
                break;
            case "wfmInUploadFileMaintenance":
                {
                    ns.DirtyData[e.context.activeDivID].HeaderData.MaintenanceData["ddlFileType"] = $("select#ddlFileType option:selected").val();
                }
                break;
            
        }       
        return true;
    },   

   
    FileUploadSuccess: function (e) {
        var ActiveDivID = e.context.activeDivID;
        if (ActiveDivID.indexOf("wfmPirMaintenance") == 0) {
            var executeButton = $("#" + ActiveDivID).find("#btnCancel");
            executeButton.trigger("click");
        }
        setTimeout(function () {
            var ldomDiv = $('#' + ns.viewModel.currentModel);
            if (ns.viewModel.currentModel.indexOf("wfmInUploadFileMaintenance") == 0) {
                if (ldomDiv.find("ui[class ='s-fileupload-list'] li[class='s-fileupload-item s-fileupload-item-success']").length > 0)
                    ldomDiv.find("ui[class ='s-fileupload-list'] li[class='s-fileupload-item s-fileupload-item-success']").hide();
            }
        }, 10);

    },
    // START - FS001 Related JS
    // We have used to cleare the text box value on wfmUserChangePasswordMaintenance
    AfterApplyingUI: function (e) {
        if (ns.viewModel.currentModel.indexOf("wfmUserChangePasswordMaintenance") === 0) {
            if ($("#" + ns.viewModel.currentModel).find("#txtOldPassword").val() !== "" && $("#" + ns.viewModel.currentModel).find("#txtIstrNewPassword").val() == "" && $("#" + ns.viewModel.currentModel).find("#txtConfirmPassword").val() == "") {
                $("#" + ns.viewModel.currentModel).find("#txtOldPassword").val("").trigger("change");
            }
        }
            
    },
    AfterBindFormData: function (e) {
        var ldomDiv = e.context.idomActiveDiv != undefined ? e.context.idomActiveDiv : $("#" + e.context.activeDivID);
        if ((e.context.activeDivID.indexOf("wfmPensionContributionDetailMaintenance") === 0 || e.context.activeDivID.indexOf("wfmDeferredCompContributionDetailMaintenance") === 0
            || e.context.activeDivID.indexOf("wfmPremiumDetailLTDMaintenance") === 0) && ns.SenderID == "") {
            if (ldomDiv.find("#rblGroupBy_0").is(":checked") == false && ldomDiv.find("#rblGroupBy_1").is(":checked") == false &&
                ldomDiv.find("#rblGroupBy_2").is(":checked") == false && ldomDiv.find("#rblGroupBy_3").is(":checked") == false) {
                ldomDiv.find("#rblGroupBy_0").attr("checked", true).trigger("change")
            }
        }
        if (ns.viewModel.currentModel.indexOf("wfmEmployerPayrollDetailMaintenance") == 0) {
            if (ldomDiv.find('#lblStatus1').text() == 'Review') {
                ldomDiv.find('#capComments').attr('class', 'CommentInReview');
            }
            else {
                ldomDiv.find('#capComments').removeAttr('class');
            }
        }
        if (ns.viewModel.currentModel.indexOf("wfmEmployerPayrollDetailMaintenance") == 0) {
            if (ldomDiv.find('#lblStatus1').text() == 'Review') {
                ldomDiv.find('#txtComments').attr('class', 'CommentBoxInReview');
            }
            else {
                ldomDiv.find('#txtComments').removeAttr('class');
            }
        }
        if (ns.viewModel.currentModel.indexOf("wfmMedicarePartDDetailMaintenance") == 0) {
            var ReviewStatus = ldomDiv.find('#lblLabel16');
            if (ReviewStatus.text() == 'Review') {
                ReviewStatus.attr('class', 'ShowStatus');
            }
            else {
                ReviewStatus.removeAttr('class');
            }
        }
        if ((ns.viewModel.currentModel.indexOf("wfmRHICCombiningMaintenance") == 0)|| (ns.viewModel.currentModel.indexOf("wfmDeathNotificationMaintenance") == 0)) {
            var ReviewStatus = ldomDiv.find('#lblStatusValue');
            if (ReviewStatus.text() == 'Review') {
                ReviewStatus.attr('class', 'ShowStatus');
            }
            else {
                ReviewStatus.removeAttr('class');
            }
        }
        if ((ns.viewModel.currentModel.indexOf("wfmRefundApplicationMaintenance") == 0) || (ns.viewModel.currentModel.indexOf("wfmIBSCheckEntryMaintenance") == 0) || (ns.viewModel.currentModel.indexOf("wfmEmployerPayrollDetailMaintenance") == 0)
            || (ns.viewModel.currentModel.indexOf("wfmEAPMaintenance") == 0) || (ns.viewModel.currentModel.indexOf("wfmDeferredCompPlanMaintenance") == 0) || (ns.viewModel.currentModel.indexOf("wfmMergeEmployerHeaderMaintenance") == 0) || (ns.viewModel.currentModel.indexOf("wfmPayeeAccountMaintenance") == 0)
            || (ns.viewModel.currentModel.indexOf("wfmPaymentScheduleMaintenance") == 0) || (ns.viewModel.currentModel.indexOf("wfmRetirementBenefitCalculationEstimateMaintenance") == 0) || (ns.viewModel.currentModel.indexOf("wfmLTCMaintenance") == 0) || (ns.viewModel.currentModel.indexOf("wfmPensionPlanMaintenance") == 0)
            || (ns.viewModel.currentModel.indexOf("wfmRetirementBenefitCalculationFinalMaintenance") == 0) || (ns.viewModel.currentModel.indexOf("wfmPreRetirementDeathEstimateCalculationMaintenance") == 0) || (ns.viewModel.currentModel.indexOf("wfmPreRetirementDeathFinalCalculationMaintenance") == 0)
            || (ns.viewModel.currentModel.indexOf("wfmPostRetirementDeathApplicationMaintenance") == 0) || (ns.viewModel.currentModel.indexOf("wfmFlexCompMaintenance") == 0) || (ns.viewModel.currentModel.indexOf("wfmHealthDentalVisonMaintenance") == 0) || (ns.viewModel.currentModel.indexOf("wfmGroupLifeMaintenance") == 0)
            || (ns.viewModel.currentModel.indexOf("wfmPostRetirementDeathFinalCalculationMaintenance") == 0) || (ns.viewModel.currentModel.indexOf("wfmRefundBenefitCalculationMaintenance") == 0) || (ns.viewModel.currentModel.indexOf("wfmRemittanceMaintenance") == 0) || (ns.viewModel.currentModel.indexOf("wfmDROApplicationMaintenance") == 0)
            || (ns.viewModel.currentModel.indexOf("wfmFileHdrMaintenance") == 0) || (ns.viewModel.currentModel.indexOf("wfmDBDBTransferMaintenance") == 0) || (ns.viewModel.currentModel.indexOf("wfmPayeeAccountMaintenance") == 0)) {
            var ReviewStatus = ldomDiv.find('#lblStatusDescription');
            var EmployerPayrollDetailMaintenance = ldomDiv.find('#lblStatus1');
            if (ReviewStatus.text() == 'Review') {
                ReviewStatus.attr('class', 'ShowStatus');
            }
            else {
                ReviewStatus.removeAttr('class');
            }
            if (EmployerPayrollDetailMaintenance.text() == 'Review') {
                EmployerPayrollDetailMaintenance.attr('class', 'ShowStatus');
            }
        }
        if ((ns.viewModel.currentModel.indexOf("wfmServicePurchaseHeaderMaintenance") == 0) || (ns.viewModel.currentModel.indexOf("wfmServicePurchaseDetailConsolidatedMaintenance") == 0)){
                var ReviewStatus = ldomDiv.find('#lblServicePurchaseStatusDescription');
                if (ReviewStatus.text() == 'Review') {
                    ReviewStatus.attr('class', 'ShowStatus');
                }
                else {
                    ReviewStatus.removeAttr('class');
                }
            }
           
        if ((ns.viewModel.currentModel.indexOf("wfmRetirementApplicationMaintenance") == 0) || (ns.viewModel.currentModel.indexOf("wfmPreRetirementDeathApplicationMaintenance") == 0)) {
            var ReviewStatus = ldomDiv.find('#lblApplicationStatusValue');
            if (ReviewStatus.text() == 'Review') {
                ReviewStatus.attr('class', 'ShowStatus');
            }
            else {
                ReviewStatus.removeAttr('class');
            }
        }
        if (ns.viewModel.currentModel.indexOf("wfmEmployerPayrollHeaderMaintenance") == 0) {
            var ReviewStatus = ldomDiv.find('#btnStatusDescription');
            if (ReviewStatus.text() == 'Review') {
                ReviewStatus.attr('class', 'ShowStatus');
            }
            else {
                ReviewStatus.removeAttr('class');
            }
        }
        //PIR 25920 DC 2025 changes show 0 as selected value if DB has the same 
        if (ns.viewModel.currentModel.indexOf("wfmPensionPlanMaintenance") == 0) {
            if (ldomDiv.find("#lblAddlContributionPercentageZero").text() === "0")
            {
                ldomDiv.find("#ddlAddlContributionPercentage").val(-1);
            }
        }
        if (e.context.activeDivID.indexOf("wfmContactMaintenance") >= 0 || e.context.activeDivID.indexOf("wfmOrgContactMaintenance") >= 0) {
            if(e.context.activeDivID.indexOf("wfmContactMaintenance") >= 0)
            {
                if ((ldomDiv.find("#lblIsContactAffiliatedWithNoOrg").text() === "true") && (ldomDiv.find("#lblContactId").text() != "") &&
	                (ldomDiv.find("#ddlStatusValue").val() != "INCV")) {
	                CustomPopupToInactiveContact("This Contact is no longer associated with any ORGs, should their status be updated to reflect Inactive?");
	            }
	        }
	        if(e.context.activeDivID.indexOf("wfmOrgContactMaintenance") >= 0)
	        {
	            if (ldomDiv.find("#lblIsContactAffiliatedWithNoOrg").text() === "true") {
	                    CustomPopupToInactiveContact("This Contact is no longer associated with any ORGs, should their status be updated to reflect Inactive?");
	              }
	        }
        }

        //FW Upgrade :: PIR - 16116 - Graph details are cropped..
        if (e.context.activeDivID.indexOf("wfmExceptionDashboardMaintenance") == 0) {
            setTimeout(function () {
                $("#" + ns.viewModel.currentModel).find("#chrErrorsInLastFiveDays").css("width", "750px");
            }, 100);
        }
	if (e.context.activeDivID.indexOf("wfmWorkflowMaintenance") == 0) {
        $("#masterPageActivityInstanceDetails").empty();
        $("divBpmActivityInstanceDetails").empty();
        }
        if (e.context.activeDivID.indexOf("wfmWorkflowMaintenance") == 0) {
            ldomDiv.find("#divBpmActivityInstanceDetails").hide();
        }

        if (e.context.activeDivID.indexOf("wfmBpmActivityInstanceMaintenance") == 0) {
            if (ns.viewModel[e.context.activeDivID].OtherData["RefreshCenterLeft"] != undefined && ns.viewModel[e.context.activeDivID].OtherData["RefreshCenterLeft"] == true) {
                ns.BuildLeftForm(nsConstants.BPM_WORKFLOW_CENTERLEFT_MAINTENANCE, undefined);
            }
        }
    },
    BeforeShowDiv: function (e) {
        var ldomDiv = $("#" + ns.viewModel.currentModel);
        if ((ns.viewModel.currentModel.indexOf("wfmPayeeAccountRolloverDetailMaintenance") === 0) && (ldomDiv.find("#GlobalMessageDiv").val() == "")) {
            if (ns.viewModel.srcElement && ns.viewModel.srcElement.id == "btnOpenRolloverDetail") {
                nsCommon.DispalyMessage("[ Record displayed. Please make changes and press SAVE. ]", ns.viewModel.currentModel);
            }
        }
        nsUserFunctions.iblnMenuItemClicked = false;
    },
    ChangeSearchCriteriaOnOpenLookupFromBPM: function (e) {
       // return true;
        if (e.SearchCriteria.length > 0) {
            var formDiv = $('#' + e.formId);
            var ldctActInfo = e.SearchCriteria[0];
            var lstrdivBpmActivityInstanceDetails = '#divBpmActivityInstanceDetails';
            var ldivBpmActivityInstanceDetails = formDiv.find(lstrdivBpmActivityInstanceDetails);
            if (ldivBpmActivityInstanceDetails.length == 0) {
                formDiv.prepend('<div id="divBpmActivityInstanceDetails"></div>');
                ldivBpmActivityInstanceDetails = formDiv.find(lstrdivBpmActivityInstanceDetails);
            }
            formDiv.find('#spanBpmActivityInstanceDetails').remove();
            //var lstrdivBpmActivityInstanceDetails = '#masterPageActivityInstanceDetails';
            //var ldivBpmActivityInstanceDetails = $(lstrdivBpmActivityInstanceDetails);
            ldivBpmActivityInstanceDetails.find('#spanBpmActivityInstanceDetails').remove();
            if (ldctActInfo.ProcessName != undefined && ldctActInfo.ActivityName != undefined && ldctActInfo.ActivityDetailsNavParams != undefined) {
                //ldivBpmActivityInstanceDetails = $(ldivBpmActivityInstanceDetails);
                ldivBpmActivityInstanceDetails.html(
                    [
                        '<span id="spanBpmActivityInstanceDetails" >',
                        '<strong>', Sagitec.DefaultText.BPM_PROCESS, '</strong> ',
                        ldctActInfo.ProcessName,
                        ' <strong>' + Sagitec.DefaultText.BPM_ACTIVITY + '</strong> ',
                        '<a class="fakelink menuItem" id="lnkActivityInstance" title="Activity Instance Details" name="lnkActivityInstance" formid="wfmBpmActivityInstanceMaintenance" sfwMageMode="Update" sfwNavParams="',
                        ldctActInfo.ActivityDetailsNavParams,
                        '">',
                        ldctActInfo.ActivityName,
                        '</a > </span>'].join(''));
                
                if (ldctActInfo && ldctActInfo.ProcessInstanceId && ldctActInfo.ProcessInstanceId != undefined && ldctActInfo.ProcessInstanceId != null
                    && ns.viewModel.srcElement && ns.viewModel.srcElement.id && (ns.viewModel.srcElement.id == "btnInProcess2" || ns.viewModel.srcElement.id == "btnCheckout"))
                {
                    LaunchFileNetImageURLs(ldctActInfo.ProcessInstanceId);
                }
            }
            e.SearchCriteria.splice(0, 1);
        }
    },
    showDivCallBack: function (e) {
        $('#ToolTipDiv').hide();
        if (e.context.astrDivID != undefined && e.context.astrDivID.indexOf("Lookup") > 0) {
            var ldivmasterspanBpmActivityInstanceDetailhtml = $('#spanBpmActivityInstanceDetails');
            if (ldivmasterspanBpmActivityInstanceDetailhtml && ldivmasterspanBpmActivityInstanceDetailhtml.length > 0) {
       
                setTimeout(function () {
                    var ldivToolbarContainer = $('.s-freezed-crumtoolbar-Container');
                    if (ldivToolbarContainer.hasClass('hideByFreeze')) {
                        ldivToolbarContainer.removeClass('hideByFreeze');
                    }
                }, 50, e.context.idomActiveDiv);
            }
        }
        else {
            var ldomDiv = e.context.idomActiveDiv != undefined ? e.context.idomActiveDiv : $("#" + e.context.activeDivID);
           ldomDiv.find('#spanBpmActivityInstanceDetails').remove();
            var data = ns.viewModel[ns.viewModel.currentModel];
            setTimeout(function (data, formDiv) {
                if (data && data != undefined && data.OtherData != undefined && data.OtherData["ShowActivityInstanceDetails"] != undefined && data.OtherData["ShowActivityInstanceDetails"]) {
                    if (data.OtherData["ActivityDetailsNavParams"] != undefined) {
                        var lstrdivBpmActivityInstanceDetails = '#divBpmActivityInstanceDetails';
                        var ldivBpmActivityInstanceDetails = formDiv.find(lstrdivBpmActivityInstanceDetails);
                        formDiv.find('#spanBpmActivityInstanceDetails').remove();
                        ldivBpmActivityInstanceDetails = $(ldivBpmActivityInstanceDetails);
                        ldivBpmActivityInstanceDetails.html(
                            [
                                '<span id="spanBpmActivityInstanceDetails" >',
                                '<strong>', Sagitec.DefaultText.BPM_PROCESS, '</strong> ',
                                data.OtherData["ProcessName"],
                                ' <strong>' + Sagitec.DefaultText.BPM_ACTIVITY + '</strong> ',
                                '<a class="fakelink menuItem" id="lnkActivityInstance" title="Activity Instance Details" name="lnkActivityInstance" formid="wfmBpmActivityInstanceMaintenance" sfwMageMode="Update" sfwNavParams="',
                                data.OtherData["ActivityDetailsNavParams"],
                                '">',
                                data.OtherData["ActivityName"],
                                '</a > </span>'].join(''));
                        ldivBpmActivityInstanceDetails.appendTo($('#crumDiv')); //positioning the process & activity as discussed with Maik
                        ldivBpmActivityInstanceDetails.parent(".crumDiv").css("width", "100%");
                        if (data.OtherData["ProcessInstanceId"] && data.OtherData["ProcessInstanceId"] != undefined
                            && ns.viewModel.srcElement && ns.viewModel.srcElement.id && (ns.viewModel.srcElement.id == "btnInProcess2" || ns.viewModel.srcElement.id == "btnCheckout")) {
                            LaunchFileNetImageURLs(data.OtherData["ProcessInstanceId"]);
                        }
                    }
                }
            }, 50, data, e.context.idomActiveDiv);
        }
       
        if (e.context.activeDivID.indexOf("wfmDepositMaintenance0") == 0 && ns.viewModel && nsCommon.sessionGet("ResetTriggredDepositMaintenance") == undefined
            && ns.DirtyData["wfmDepositMaintenance0"] != undefined && ns.DirtyData["wfmDepositMaintenance0"] != null && ns.viewModel.srcElement != undefined && ns.viewModel.srcElement.id != null && ns.viewModel.srcElement.id == "btnNewDeposit") {
            var fnRefreshData = function () {
                var ldomDiv = $("#" + ns.viewModel.currentModel); 
                nsCommon.sessionSet("ResetTriggredDepositMaintenance", "true");                    
                ldomDiv.find("#btnCancel").trigger("click"); 
            }
            setTimeout(fnRefreshData, 50);
        }
        else if (e.context.activeDivID.indexOf("wfmRemittanceMaintenance0") == 0 && ns.viewModel && nsCommon.sessionGet("ResetTriggredRemittanceMaintenance") == undefined
            && ns.DirtyData["wfmRemittanceMaintenance0"] != undefined && ns.DirtyData["wfmRemittanceMaintenance0"] != null && ns.viewModel.srcElement != undefined && ns.viewModel.srcElement.id != null && ns.viewModel.srcElement.id == "btnNewRemittance") {
            var fnRefreshData = function () {
                var ldomDiv = $("#" + ns.viewModel.currentModel); 
                nsCommon.sessionSet("ResetTriggredRemittanceMaintenance", "true"); 
                ldomDiv.find("#btnCancel").trigger("click");  
            }
            setTimeout(fnRefreshData, 50);
        }
        else {
            nsCommon.sessionRemove("ResetTriggredDepositMaintenance");
            nsCommon.sessionRemove("ResetTriggredRemittanceMaintenance");
        }

        if (e.context.activeDivID.indexOf("wfmServicePurchaseHeaderMaintenance") === 0) {
            setTimeout(function () {
                ldomDiv.find("#btnOpenServicePurchaseDetailUserra").show();
                ldomDiv.find("#btnDeleteServicePurchaseUserra").show();
                ldomDiv.find("#btnDeleteServicePurchaseDetailConsolidatedNew").show();
                ldomDiv.find("#btnDeleteServicePurchaseDetailConsolidated").show();
                if (ldomDiv.find("#lblServicePurchaseHeaderId").text() == "") {
                    ldomDiv.find("#btnOpenServicePurchaseDetailUserra").hide();
                    ldomDiv.find("#btnDeleteServicePurchaseUserra").hide();
                    ldomDiv.find("#btnDeleteServicePurchaseDetailConsolidatedNew").hide();
                    ldomDiv.find("#btnDeleteServicePurchaseDetailConsolidated").hide();
                }
            }, 10);
            
        }
        if (e.context.activeDivID != undefined && e.context.activeDivID.indexOf("wfmReportClientMVVM_RptDiv") === 0) {
            ldomDiv.keypress(function (event) {
                var keycode = (event.keyCode ? event.keyCode : event.which);
                if (keycode == "13") {
                    $("#btnGenerateReport").trigger("click");
                }
                else {
                    event.stopPropagation();
                }
            });
        }

        //FW Upgrade PIR NO 17575 : Text is cached from previous result.
        ClearCommentsOnDetailLookup(); 
        if (ns.viewModel.currentModel.indexOf("wfmBacklogSummaryReportMaintenance") == 0) {
            setTimeout(function () { ldomDiv.find('#btnDownload').trigger('click').hide(); }, 200);
        }
        if (e.context.activeDivID.indexOf("wfmInUploadFile") == 0) {
            var fnRefreshData = function () {
                if (ns.viewModel.currentModel.indexOf("wfmInUploadFile") == 0 && nsCommon.sessionGet("RefreshUploadGrid") == undefined) {
                    nsCommon.sessionSet("RefreshUploadGrid", "true");
                    $("#" + ns.viewModel.currentModel).find("#btnCancel").trigger("click");
                }
            }
            setTimeout(fnRefreshData, 300);
        }
        else {
            var fnClearRefreshData = function () {
                nsCommon.sessionRemove("RefreshUploadGrid");
            }
            if ((nsCommon.sessionGet("RefreshUploadGrid") != undefined || nsCommon.sessionGet("RefreshUploadGrid") != null)
            ) {
                setTimeout(fnClearRefreshData, 300);
            }
        }
        if (ns.viewModel.currentModel.indexOf("wfmReviewPCDMaintenanceMaintenance") == 0) {
            setTimeout(function () {
                var btnDownload = ldomDiv.find('#btnDownload');
                if (btnDownload != null && btnDownload != undefined && btnDownload.length > 0 && btnDownload.attr('gridimagebutton')) {
                    btnDownload.removeAttr('gridimagebutton');
                    btnDownload.attr('class', 'button btnColumnsToExport_Click_button');
                    btnDownload.attr('style', 'width:155px');
                    btnDownload.attr('value', 'Review Payroll Details');
                }
            }, 100);
        }
        
        if ((ns.viewModel.currentModel && ns.viewModel.currentModel.indexOf("wfmMedicarePartDDetailMaintenance") >= 0) ||
            (ns.viewModel.currentModel && ns.viewModel.currentModel.indexOf("wfmHealthDentalVisonMaintenance") >= 0) || (ns.viewModel.currentModel && ns.viewModel.currentModel.indexOf("wfmGroupLifeMaintenance") >= 0) ||
            (ns.viewModel.currentModel && ns.viewModel.currentModel.indexOf("wfmEAPMaintenance") >= 0) || (ns.viewModel.currentModel && ns.viewModel.currentModel.indexOf("wfmFlexCompMaintenance") >= 0) ||
            (ns.viewModel.currentModel && ns.viewModel.currentModel.indexOf("wfmDeferredCompPlanMaintenance") >= 0)) {
            setTimeout(function () {
                ldomDiv.find("#txtHistoryChangeDate,#txtTextBox3").trigger("focus");
            }, 300);
        }
        if (e.context.activeDivID.indexOf("wfmDeathNotificationMaintenance") === 0) {
            setTimeout(function () {
                var ldomDiv = $("#" + ns.viewModel.currentModel);
                var lblCountOfOpenEmployment = ldomDiv.find("#lblCountOfOpenEmployment").text();
                if (lblCountOfOpenEmployment != 0) {
                    ldomDiv.find("#tshEmployment_Header").trigger("click");
                }
                else if (lblCountOfOpenEmployment == 0) {
                    ldomDiv.find("#tshValidationInfo_Header").trigger("click");
                }
            }, 10);
        }
        if (e.context.activeDivID.indexOf("wfmContactTicketMaintenance") === 0) {
            setTimeout(function () {
                if (ldomDiv.find('#ddlDeceasedName option:selected').text() == "Other") {
                    $("#" + ns.viewModel.currentModel).find("#lblName").removeClass("hideControl");
                    $("#" + ns.viewModel.currentModel).find("#txtDeceasedName").removeClass("hideControl");
                }
                else {
                    $("#" + ns.viewModel.currentModel).find("#lblName").addClass("hideControl");
                    $("#" + ns.viewModel.currentModel).find("#txtDeceasedName").addClass("hideControl");
                    $("#" + ns.viewModel.currentModel).find("#txtDeceasedName").text('').trigger("change");
                }
            }, 10);
        }
        var ldomTargetForm = $("#" + ns.viewModel.currentModel);
        if (ldomTargetForm.length > 0) {
            var retrievalLookups = ldomTargetForm.find("input[type='image'][imagebutton='true'][src*='Lookup.jpg']:hidden");
            if (retrievalLookups.length > 0) {
                retrievalLookups.each(function () {
                    $(this).show();
                });
            }
        }
        //PIR 26579
        if (ns.viewModel.currentModel.indexOf("wfmFlexCompMaintenance") == 0) {
            setTimeout(function () {
                var grvPreTaxGrid = ldomDiv.find('#Table_GridTable_grvLstPreTaxEnrollmentStatusInPreviousYears');
                if (grvPreTaxGrid != null && grvPreTaxGrid != undefined && grvPreTaxGrid.length > 0) {
                    //Update column headers to be dynamic Years
                    var CY = Number(ldomDiv.find('#lblLatestCY')[0].innerText);
                    grvPreTaxGrid.find("th").each(function (index) {
                        if (index == 2) {
                            $(this)[0].innerText = CY;
                        }
                        if (index == 3) {
                            $(this)[0].innerText = CY-1;
                        }
                        if (index == 4) {
                            $(this)[0].innerText = CY-2;
                        }
                        if (index == 5) {
                            $(this)[0].innerText = CY-3;
                        }
                    });
                    //Disable links for Not Enrolled plans
                    grvPreTaxGrid.find("td").each(function (index) {
                        //1 , 7 , 13 are the index for the Status td
                        if ([1, 7, 13].includes(index) && $(this)[0].innerHTML == "Not Enrolled") {
                            $(grvPreTaxGrid.find("td")[index - 1]).find("a")[0].setAttribute("style", "color: black; pointer-events: none;");
                        }
                    });
                    //Disable all checkboxes
                    grvPreTaxGrid.find("td input[type='checkbox']").each(function (index) {
                        $(this)[0].setAttribute("style", "pointer-events: none;");
                    });
                }
            }, 100);
        }
    },
    logoutSesssion: function () {
        var lstrLogoutUrl = ["../", ns.SiteName, "/account/logout"].join('');
        window.location.href = lstrLogoutUrl
    },
    logoutApp: function () {
        sessionStorage.clear();
        var lstrLogoutUrl = ["../", ns.SiteName, "/account/logout"].join('');
        window.location.href = lstrLogoutUrl;
    },

    //PrintCurrentPage: function (e) {
    //    $("#" + ns.viewModel.currentModel).jqprint();
    //},
    DeletePayHisLookup: function (e) {
        try {
            var delPayHisLookup = $("a.delete-link[deletenodeid='li_wfmPaymentHistoryLookup']");
            if (delPayHisLookup.length > 0)
                delPayHisLookup.trigger("click");
        }
        catch (ex) { }
        return true;
    },
    //END FS001 Related JS
    
    //CancelClickHandler: function (e) {
    //    var grid = $(e.currentTarget).closest("table");
    //    //Show confirm popup if atleast one record is selected
    //    //if (grid.find("input:checkbox").length > 1 && grid.find("input:checkbox:checked").length > 0 && grid.find("input:checkbox:checked").closest("tr").find("td:nth-child(4)").text() == 'Queued')
    //    if (grid.find("input:checkbox").length > 1 && grid.find("input:checkbox:checked").length > 0) {
    //        return confirm(e.context.idictParam.param0 ? e.context.idictParam.param0 : "Type your message here.");
    //    }
    //    else {
    //        return true;
    //    }
    //},    

    //FW Upgrade :: Code Conversion for "btnPopupReportEnvelope_Click" method
    ReportClient: function (e) {
        var activeDivID = e.context.activeDivID;
        var rptName = $(["#", activeDivID, " #lblTemplateName"].join('')).text();
        var lifrmRpt = $(["#", activeDivID, " #ReportFrame1"].join(''))
        if (lifrmRpt.length == 0) {
            var lblReport = $(["#", activeDivID, " #lblReport"].join(''));
            lifrmRpt = lblReport.parent().append('<iframe id="ReportFrame1" height="650px" width="1000px" style="display: none"></iframe>');
            //lifrmRpt.hide();
            lblReport.remove();
        }
        nsRpt.CurrentRpt.RptForm = rptName;
        nsRpt.CurrentRpt.iblnCustRept = true;
        nsRpt.CurrentRpt.CustomReportObj.ActiveDivId = activeDivID;

        var NewFormID = nsCommon.GetProperFormId(rptName);
        if (ns.Templates[NewFormID] == undefined) {
            var data = nsRequest.getTemplate(rptName, true);
            ns.Templates[NewFormID].HeaderData = MVVM.ServiceLoad.GetObservable(data.DomainModel.HeaderData);
        }
        return true;
    },
    GetCustomReportParams: function (e) {
        var RprtParams = {};
        if (ns.viewModel.currentModel && ns.viewModel.currentModel.indexOf("wfmMailingLabelMaintenance") >= 0) {
            RprtParams["aintMailingBatchID"] = $("#" + ns.viewModel.currentModel).find("#lblMailingLabelBatchId").text();
        }
        if (ns.viewModel.currentModel && ns.viewModel.currentModel.indexOf("wfmonezeronineninerMaintenance") >= 0) {
            RprtParams["aint1009rID"] = $("#" + ns.viewModel.currentModel).find("#lblPayment1099rId").text();
        }
        //else
        //    RprtParams["PERSONID"] = $("#" + ns.viewModel.currentModel).find("#lblPersonId").text();
        nsRpt.CurrentRpt.CustomReportObj.Params = RprtParams;
        return RprtParams;
    },
    CustomAfterReportGenerated: function (e) {
        var ReportUrl = [ns.ReportPagePath, "/AspxPages/wfmReportClient.aspx?ddlReports=", nsRpt.CurrentRpt.RptForm].join('');

        for (var RptParams in nsRpt.CurrentRpt.CustomReportObj.Params) {
            if (nsRpt.CurrentRpt.CustomReportObj.Params[RptParams].value !== undefined) {
                ReportUrl = [ReportUrl, "&", RptParams, "=", nsRpt.CurrentRpt.CustomReportObj.Params[RptParams].value].join('');
            } else {
                ReportUrl = [ReportUrl, "&", RptParams, "=", nsRpt.CurrentRpt.CustomReportObj.Params[RptParams]].join('');
            }
        }

        $("#ReportFrame1").attr("src", ReportUrl);
        $("#ReportFrame1").show();
        $("#ReportFrame1").contents().find("body").append("<h1 class='reportloader'>Loading...</h1>");
        setTimeout(function () {
            $("#lblReportGeneratedMessage").html("<b>[ Report Successfully Generated ]</b>").removeAttr("style").attr("style", "display: block !important");
            if (ns.viewModel.currentModel && ns.viewModel.currentModel.indexOf("wfmMailingLabelMaintenance") >= 0) {
                $("#lblReportGeneratedMessage").hide();
            }
            if (ns.viewModel.currentModel && ns.viewModel.currentModel.indexOf("wfmonezeronineninerMaintenance") >= 0) {
                $("#lblReportGeneratedMessage").hide();
            }
        }, 10);
    },

    

    //START COMMON JS 

    BuildLeftFormById: function (astrFormName, cssId, binddata) {
        var currentFormName = ns.viewModel.currentModel;
        ns.viewModel.currentModel = astrFormName;
        var data = ns.getTemplate(astrFormName, true);

        var lstrFormId = data.ExtraInfoFields["FormId"];
        ns.viewModel[data.ExtraInfoFields["FormId"]] = {
            HeaderData: kendo.observable(data.DomainModel.HeaderData),
            DetailsData: data.DomainModel.DetailsData
        };

        var sidebarForm = "<div id='" + astrFormName + "'><div id='" + astrFormName + "ErrorDiv' class='ErrorDiv'></div>" + data.Template + "</div>";

        var parentItem;
        if ($("#" + astrFormName).length > 0) {
            parentItem = $("#" + astrFormName).parent();
        }

        if (parentItem === undefined) {
            $(cssId).append(sidebarForm);
        } else {
            ns.blnFromDeleteTreeNode = true;
            ns.destroyAll(astrFormName);
            ns.blnFromDeleteTreeNode = false;
            $(cssId).append(sidebarForm);
        }
        if (currentFormName !== undefined || currentFormName !== null)
            ns.viewModel.currentModel = currentFormName;

        ns.applyKendoUI(cssId, astrFormName, astrFormName);
        ns.Templates[astrFormName].HeaderData = kendo.observable(ns.Templates[astrFormName].HeaderData);
        kendo.bind($(cssId + " #" + astrFormName)[0], ns.Templates[astrFormName].HeaderData);
    },

    // END COMMON JS

    InitilizeUserDefinedEvents: function () {

        //FW Upgrade After harderror page should scroll to top
        $("#CenterSplitter").scroll(function () {

            var ActiveDivID = "";
            if (ns.viewModel.currentForm.indexOf("Lookup") > 0)
                ActiveDivID = ns.viewModel.currentForm;
            else
                ActiveDivID = ns.viewModel.currentModel;
            ns.SessionStorePageState(ActiveDivID, "scroll", "scroll", $("#MainSplitter").scrollTop());
        }); 
        var OldPersonId = 0;
        $(document).on('change', "#txtPersonId", function (e) {
            if (ns.viewModel.currentModel.indexOf("wfmIBSCheckEntryDetailMaintenance") === 0) {
                var ldomDiv = $("#" + ns.viewModel.currentModel);
                var PersonId = ldomDiv.find("#txtPersonId").val();
                if (PersonId != undefined && PersonId != null && OldPersonId != PersonId) {
                    OldPersonId = PersonId;
                    ldomDiv.find("#btnServerSideRefresh").trigger("click");
                }
            }
        });
        //FW Upgrade PIR 15706 Enter button search functionality on Person Quick-Search
        $(document).off("keyup.CenterLeftSearch").on("keyup.CenterLeftSearch", "body", function (e) {
            if (e.keyCode == 13) {
                if ($("#wfmCenterLeftPersonLookup").is(":visible")) {

                    var activeControl = document.activeElement["id"];
                    if (activeControl == "txtCL_person_id" || activeControl == "txtCLFirstName" || activeControl == "txtCLLastName") {

                        $("#txtCL_person_id,#txtCLFirstName,#txtCLLastName").blur();
                        $("#btnCLSearch").trigger('click');
                    }
                }
                if ($("#wfmCenterLeftPersonLookup").is(":visible")) {

                    var activeControl = document.activeElement["id"];
                    if (activeControl == "txtCLOrgOrgCode" || activeControl == "txtCLOrgOrgName" || activeControl == "txtCLOrgOrgTypeValue") {

                        $("#txtCLOrgOrgCode,#txtCLOrgOrgName").blur();
                        $("#txtCLOrgOrgTypeValue").trigger("change");
                        $("#btnCLOrgSearch").trigger('click');
                    }
                }
            }
        });
        $(document).on("change", "#ddlReports", function (e) {
            if (ns.viewModel && ns.viewModel.currentModel && ns.viewModel.currentModel.indexOf("wfmReportClient") == 0) {
                $('#LookupFormTitle').text('Reports');
                $('#lblRequiredError').addClass('hideControl');
                $('#lblRequiredErrorText').text('');
                var rptparam = $('#RptParmsDiv #Main tr').find('td[class="captionTd"]');
                rptparam.css('width', '1%');
                rptparam.css('padding-left', '5px');
                var globalmessagediv = $("div[id='GlobalMessageDiv'][class='GlobalMessage']");
                if (globalmessagediv.length > 0) {
                    globalmessagediv[0].innerHTML = "";
                    $("div[id='GlobalMessageDiv']").removeClass("GlobalMessage");
                }
                $("#btnGenerateReport").trigger("focus");
                var ldomDiv = $("#" + ns.viewModel.currentModel);
                var lddlreports = ldomDiv.find("#ddlReports");
                if (lddlreports.length > 0) {
                    var valueOfType = lddlreports.val();
                    if (valueOfType != "") {
                        nsCommon.DispalyMessage("", ns.viewModel.currentModel);
                    }
                }
            }
        });
        $(document).on("click", "#files", function (e) {
            var ldomDiv = $('#' + ns.viewModel.currentModel);
            if (ns.viewModel.currentModel.indexOf("wfmInUploadFileMaintenance") == 0) {
                if (ldomDiv.find("ui[class ='s-fileupload-list'] li[class='s-fileupload-item s-fileupload-item-error']").length > 0) {
                    ldomDiv.find("span[class ='s-fileupload-clear']").trigger("click");
                }
                if (ldomDiv.find("ui[class ='s-fileupload-list'] li[class='s-fileupload-item']").length > 0) {
                    ldomDiv.find("span[class ='s-fileupload-clear']").trigger("click");
                }
            }
        });
        
        $(document).on("change", "#txtHistoryChangeDate", function (e) {
            var ldomDiv = $('#' + ns.viewModel.currentModel);
            if (ns.viewModel.currentModel.indexOf("wfmPensionPlanMaintenance") == 0) {
                var astrHistoryChangeDate = ldomDiv.find("#txtHistoryChangeDate").val();
                var ddlAddlContributionPercentage = ldomDiv.find("#ddlAddlContributionPercentage");
                var intPlanId = ldomDiv.find("#lblPlanID").text();
                
                if (intPlanId == 30) //busConstant.PlanIdDC2025
                {
                    var month = 0;
                    var year = 0;
                    var day = 0;
                    //var intAdditionalEEPermContribution = ldomDiv.find("#lblAdditionalEEPermContribution").text();
                    //var intAdditionalEETempContribution = ldomDiv.find("#lblAdditionalEETempContribution").text();
                    
                    //if (intAdditionalEEPermContribution.length > 1)
                    //    intAdditionalEEPermContribution = intAdditionalEEPermContribution.substring(0, intAdditionalEEPermContribution.lastIndexOf("."));
                    //if (intAdditionalEETempContribution.length > 1)
                    //    intAdditionalEETempContribution = intAdditionalEETempContribution.substring(0, intAdditionalEETempContribution.lastIndexOf("."));
                    //if (intAdditionalEEPermContribution == 0) intAdditionalEEPermContribution = -1;
                    //if (intAdditionalEETempContribution == 0) intAdditionalEETempContribution = -1;
                    
                    if (astrHistoryChangeDate != null && astrHistoryChangeDate != undefined && astrHistoryChangeDate != "") {
                        var spitdate = astrHistoryChangeDate.split('/');
                        if (spitdate.length > 0) {
                            month = parseInt(spitdate[0]);
                            day = parseInt(spitdate[1]);
                            year = parseInt(spitdate[2]);
                        }
                    }
                    
                    if (month > 0 && year > 0 && day > 0 && year.toString().length == 4) {
                        if (month > 0 && month <= 12 && year >= 1753 && year <= 9999 && day >= 1 && day <= 31) {

                            if ((nsCommon.sessionGet("HistoryChangeDate") == null && nsCommon.sessionGet("HistoryChangeDate") == undefined)
                                || nsCommon.sessionGet("HistoryChangeDate") != astrHistoryChangeDate) {
                                    ldomDiv.find("#btnGetADECValues").trigger("click");
                                    nsCommon.sessionSet("HistoryChangeDate", astrHistoryChangeDate);
                            }
                            //ldomDiv.find("#btnGetADECValues").trigger("click");
                            //var Parameters = {};
                            //Parameters["astrHistoryChangeDate"] = astrHistoryChangeDate;
                            //Parameters["aintPersonEmploymentDtlId"] = ldomDiv.find("#lblPersonEmploymentDtlId").text();
                            //Parameters["aintPersonAccountId"] = ldomDiv.find("#lblPersonAccountId").text();
                            //data = nsCommon.SyncPost("GetADECAmountValuesByEffectiveDate", Parameters, "POST");

                            //if (data != undefined) {
                            //    var jsonObject = JSON.parse(data);
                            //    ddlAddlContributionPercentage.empty();
                            //    if (jsonObject.length > 0) {
                            //        ddlAddlContributionPercentage.append('<option value=""></option>');
                            //        $.each(jsonObject, function (index, value) {
                            //            ddlAddlContributionPercentage.append('<option value="' + value.description + '">' + value.code_value + '</option>');
                            //        });
                                    
                            //        if (ldomDiv.find("#ddlAddlContributionPercentage option").length > 5)
                            //            ldomDiv.find("#ddlAddlContributionPercentage").val(intAdditionalEETempContribution);
                            //        else
                            //            ldomDiv.find("#ddlAddlContributionPercentage").val(intAdditionalEEPermContribution);
                                    
                            //        ddlAddlContributionPercentage.show();
                            //        ddlAddlContributionPercentage.focus();
                            //        //ldomDiv.find("#btnGetADECValues").trigger("click");
                            //    }
                            //}
                        }
                        else {
                            alert("Invalid Date");
                            //astrHistoryChangeDate.val('');
                            //astrHistoryChangeDate.trigger("focus");
                            //astrHistoryChangeDate.focus();
                        }
                    }
                    else {
                    }
                }
            }
        });

    },

    

    //Knowtion center left section starts here

    KnowtionSearchByFormId: function (astrFormId) {
        try {
            //$("#btnReset").val("Reset");
            var lstrFormId = nsCommon.sessionGet("KnowtionFormId");
            var blnSearch = false;
            if (lstrFormId === undefined) {
                blnSearch = true;
            }
            else {
                blnSearch = astrFormId != lstrFormId;
            }
            if (astrFormId === undefined)
                astrFormId = lstrFormId;
            nsCommon.sessionSet("KnowtionFormId", astrFormId);
            
        }
        catch (e) {
            console.log(e.message);
        }
    },

    ////Method to open the Knowtion Site url from left panel(Enterprise Knowledge)
    ////Method to open the Knowtion Site url from left panel(Enterprise Knowledge)

    //KnowtionHelpCenterLeftTitleClick: function (e) {
    //    try {

    //        var lstrFormName = nsCommon.GetFormNameFromDivID(e.context.activeDivID);

    //        var lstrHelpUrl = "";
    //        var lobjBtnInfo = nsCommon.GetEventInfo(e);
    //        var FormContainerID = lobjBtnInfo.FormContainerID;
    //        var ActiveDivID = lobjBtnInfo.ActiveDivID;
    //        var lbtnSelf = lobjBtnInfo.lbtnSelf;
    //        var lintSelectedIndex = lobjBtnInfo.lintSelectedIndex;
    //        var RelatedGridID = MVVMGlobal.GetControlAttribute(lbtnSelf, nsConstants.SFW_RELATED_CONTROL, ActiveDivID);
    //        if (RelatedGridID === null) {
    //            alert(DefaultMessages.GridNotFound);
    //            return false;
    //        }
    //        var lobjRelatedControl = nsCommon.CheckGridOrListView(ActiveDivID, RelatedGridID);
    //        if (lobjRelatedControl.NotFound)
    //            return false;
    //        var lblnIsListView = lobjRelatedControl.blnIsListView;
    //        var lobjGridWidget = nsCommon.GetWidgetByActiveDivIdAndControlId(ActiveDivID, lobjRelatedControl.RelatedControlId);
    //        if (lobjGridWidget === undefined || lobjGridWidget.jsObject === undefined) {
    //            return false;
    //        }
    //        //getting navigation params
    //        var ldictParams = nsCommon.GetNavigationParams(lbtnSelf, e);

    //        if (ldictParams.larrRows.length === 0 && RelatedGridID !== undefined) {
    //            nsCommon.DispalyError("No record selected. Please select record(s) and try again.", ActiveDivID);
    //            return false;
    //        }

    //        var larrRows = ldictParams.larrRows;
    //        lstrHelpUrl = ldictParams.larrRows[0]["astrHelpUrl"];

    //        if (lstrHelpUrl !== "") {
    //            var RedirectURL = ns.ReportPagePath + "AspxPages/wfmAuthenticateKnowtion.aspx?doSSO=true&Url=" + lstrHelpUrl;
    //            window.open(RedirectURL);
    //        }
    //        else {
    //            nsCommon.DispalyError("Failed to open help.", ActiveDivID);
    //        }
    //        return false;
    //    }
    //    catch (e) {
    //        console.log(e.message);
    //    }
    //},

    //OpenKnowtion: function () {
    //    try {

    //        var KnowtionSiteUrl = $("#OpenKnowtionUrl").val();
    //        var UserSerialId = $("#UserSerialIdForKnowtion").val();
    //        var RedirectToSite = ns.ReportPagePath + "AspxPages/wfmAuthenticateKnowtion.aspx?doSSO=true&Url=" + KnowtionSiteUrl + "&SId=" + UserSerialId;
    //        window.open(RedirectToSite);
    //        return false;

    //    } catch (e) {
    //        console.log(e.message);
    //    }
    //},

    KnowtionMoreResults: function (e) {
        try {
            var MoreResultURL = $("#SearchUrl").val();
            //getting navigation params
            var lstrSearchKeyword = "";
            lstrSearchKeyword = $("#lbllstrSearchKeyword").text();

            window.open(MoreResultURL + lstrSearchKeyword);
            return false;
        }
        catch (e) {
            console.log(e.message);
        }
    },

    ValidateBeforeSelectRecord: function (e) {
        var btnId = $(e)[0].target.id;
        var relatedGrid = MVVMGlobal.GetControlAttribute($($(e)[0].target), 'sfwRelatedControl', e.context.activeDivID, false); // templateAttr
        if (relatedGrid != null) {
            var lobjGridOrListView = nsCommon.GetWidgetByActiveDivIdAndControlId(e.context.activeDivID, relatedGrid);
            var dataRows = lobjGridOrListView.getSelectedRows();
            if (dataRows.length == 0) {
                nsCommon.DispalyError("[ No records selected. Please select a record. ]", e.context.activeDivID);
                return false;
            }
            if (dataRows.length > 1) {
                nsCommon.DispalyError("[ Cannot select multiple records. Please select a single record. ]", e.context.activeDivID);
                return false;
            }
        }
        return true;
    },

    //KnowtionAskaQuestion: function (e) {
    //    var portal = ""; var CultureLanguage = "";
    //    if (ns.SiteName == "NeoSpinMVVM") {
    //        portal = "Staff";
    //    }

    //    if (ns.Language != undefined) {
    //        CultureLanguage = ns.Language
    //    }

    //    var AskQuestionUrl = $("#KnowtionAskQuestionUrl").val();
    //    var UserSerialId = $("#UserSerialIdForKnowtion").val();

    //    var AskQuestionRedirectURL = ns.ReportPagePath + "AspxPages/wfmAuthenticateKnowtion.aspx?doSSO=true&Url=" + AskQuestionUrl + "?LOBPortal=" + portal + "&SId=" + UserSerialId;

    //    try {
    //        window.open(AskQuestionRedirectURL);
    //        return false;
    //    }
    //    catch (e) {
    //        console.log(e.message);
    //    }
    //},

    //ResetSearchKeyword: function () {
    //    try {
    //        $("#txtIstrSearchKeyword").val('');
    //        return false;
    //    }
    //    catch (e) {
    //        console.log(e.message);
    //    }
    //},

    SetUnSavedFormIcon: function (e) {
        if (e.context.activeDivID.indexOf("wfmInUploadFileMaintenance") == 0) {
            return false;
        }
        return true;
    },

    DeletePIRAttachments: function (e) {
        var result = confirm('Are you sure you want to delete this PIR attachment?');
        if (result) {
            var ActiveDivID = e.context.activeDivID;
            var $kgrid = $("#" + e.context.activeDivID + " #GridTable_" + "grvPirAttachment").data("neoGrid");
            var dataRows = $kgrid.dataSource.data;
            var lstrPIRID = $('#' + ActiveDivID).find("#lblPirId").text();
            if (dataRows && dataRows.length > 0) {
                var aRowIndex = e.getAttribute("rowindex");
                var aSelectedRecord;
                if (aRowIndex && aRowIndex != "") {
                    aSelectedRecord = dataRows[aRowIndex];
                }

                if (aSelectedRecord != undefined) {
                    aSourceValue = aSelectedRecord.dt_Name_0_0;
                    var Param = lstrPIRID + "\\" + aSourceValue;
                    var Parameters = {};
                    Parameters["aintPIRID"] = lstrPIRID;
                    Parameters["astrFileName"] = Param;
                    var data = {};
                    data = nsRequest.SyncPost("DeletePIRAttachment", Parameters, "POST");

                    if (data && data != null && data == true) {
                        sessionStorage.setItem("DeleteMessageStr", "Filename : " + aSourceValue + " successfully deleted");
                        $('#' + ActiveDivID).find("#btnCancel").trigger("click");
                        //nsCommon.DispalyMessage("Filename : " + aSourceValue + " successfully deleted", e.context.activeDivID);
                        return false;
                    }
                    else {
                        nsCommon.DispalyError("Delete failed.", e.context.activeDivID);
                        return false;
                    }
                }
            }
            return false;
        }
    },
    btnRemoveWSSAccess: function (e) {
        var ActiveDivID = e.context.activeDivID;
        var Parameters = {};
        Parameters["NdpersLoginId"] = $('#' + ActiveDivID).find("#lblNdpersLoginId").text();
        var data = {};
        data = nsRequest.SyncPost("RemoveWSSAccess", Parameters, "POST");
        if (data == undefined) {
            return false;
        }
        else {
            return true;
        }
    },
    DownLoadPDF: function (e) {
        var lstrFormName = nsCommon.GetFormNameFromDivID(e.context.activeDivID);
        var ldomDiv = e.context.idomActiveDiv != undefined ? e.context.idomActiveDiv : $("#" + e.context.activeDivID);
        
        var Parameters = {};
        
        if (e.context.activeDivID.indexOf("wfmPersonOverviewMaintenance") == 0) {
            var lobjBtnInfo = nsCommon.GetEventInfo(e);
            var FormContainerID = lobjBtnInfo.FormContainerID;
            var ActiveDivID = lobjBtnInfo.ActiveDivID;
            var lbtnSelf = lobjBtnInfo.lbtnSelf;
            var lstrMASSelectionID = "";
            var lstrReportName = "";
            var lintSelectedIndex = lobjBtnInfo.lintSelectedIndex;
            var RelatedGridID = MVVMGlobal.GetControlAttribute(lbtnSelf, nsConstants.SFW_RELATED_CONTROL, ActiveDivID);
            if (RelatedGridID === null) {
                alert(DefaultMessages.GridNotFound);
                return false;
            }
            var lobjRelatedControl = nsCommon.CheckGridOrListView(ActiveDivID, RelatedGridID);
            if (lobjRelatedControl.NotFound)
                return false;
            var lblnIsListView = lobjRelatedControl.blnIsListView;
            var lobjGridWidget = nsCommon.GetWidgetByActiveDivIdAndControlId(ActiveDivID, lobjRelatedControl.RelatedControlId);
            if (lobjGridWidget === undefined || lobjGridWidget.jsObject === undefined) {
                return false;
            }
            //getting navigation params
            var ldictParams = nsCommon.GetNavigationParams(lbtnSelf, e);

            if (ldictParams.larrRows.length === 0 && RelatedGridID !== undefined) {
                nsCommon.DispalyError("No record selected. Please select record(s) and try again.", ActiveDivID);
                return false;
            }

            var larrRows = ldictParams.larrRows;
            lstrMASSelectionID = larrRows[0]["aintSelectionID"];
            lstrReportName = larrRows[0]["astrReportName"];
            
            Parameters["aintSelectionID"] = lstrMASSelectionID;
            Parameters["astrReportName"] = lstrReportName;
        }
        if ((e.context.activeDivID.indexOf("wfmViewRequestLifeEnrollmentMaintenanceLOB") == 0) ||
            (e.context.activeDivID.indexOf("wfmViewRequestPensionPlanDCRetirementEnrollmentMaintenanceLOB") == 0) ||
            (e.context.activeDivID.indexOf("wfmViewRequestPensionPlanMainRetirementOptionalEnrollmentMaintenanceLOB") == 0) ||
            (e.context.activeDivID.indexOf("wfmViewRequestPensionPlanDCOptionalEnrollmentMaintenanceLOB") == 0)) {
            var lstrPDFName = e.target.getAttribute("reportname");
            Parameters["astrPDFName"] = lstrPDFName
        }
        var winName = 'MyWindow';
        var windowoption = 'width=800,height=800,resizable=yes,toolbar=yes,scrollbars=yes,alwaysRaised=yes';
        var lstrAction = ["SenderID=", ns.SenderID, "&SenderForm=", ns.SenderForm, "&Action=", "OpenPDFRender", "&SenderKey=", ns.SenderKey].join('');
        var Prefix = MVVMGlobal.GetPrefixforAjaxCall();
        var url = [Prefix, "Home/OpenPDFRender?", lstrAction].join('');
        url = [url, "&WindowName=", window.name].join('');
        var form = document.createElement("form");
        form.setAttribute("method", "post");
        form.setAttribute("action", url);
        form.setAttribute("target", winName);

        var input = document.createElement('input');
        input.type = 'hidden';
        input.name = 'aobjDownload';
        input.value = JSON.stringify(Parameters);
        form.appendChild(input);
        var input_antiforgery = document.createElement('input');
        input_antiforgery.type = 'hidden';
        input_antiforgery.name = 'antiForgeryToken';
        input_antiforgery.value = $("#antiForgeryToken").val();
        form.appendChild(input_antiforgery);
        document.body.appendChild(form);
        window.open('', winName, windowoption);
        form.target = winName;
        form.submit();
        document.body.removeChild(form);
        return false;
    },
    UpdateComment: function (e) {
        var ldomDiv = e.context.idomActiveDiv != undefined ? e.context.idomActiveDiv : $("#" + e.context.activeDivID);
        var $kgrid = $("#" + e.context.activeDivID + " #GridTable_" + "grvPayrollDetail").data("neoGrid");
        return SetCommentAndIgnorStatus(e,$kgrid, ldomDiv, false);
    },
    SetIgnoreStatus: function (e) {
        var lstrFormName = nsCommon.GetFormNameFromDivID(e.context.activeDivID);
        var ldomDiv = e.context.idomActiveDiv != undefined ? e.context.idomActiveDiv : $("#" + e.context.activeDivID);        
        var $kgrid = $("#" + e.context.activeDivID + " #GridTable_" + "grvPayrollDetail").data("neoGrid");
        return SetCommentAndIgnorStatus(e, $kgrid, ldomDiv, true);
    },
    CorrespondenceSelectedIndexChanged: function (e) {
        nsCorr.CurrentCorr.CorrFilePath = null;
        if ($("#" + ns.viewModel.currentModel).find("#btnImageCorrespondence").attr('disabled') != 'undefined') {
            $("#" + ns.viewModel.currentModel).find("#btnImageCorrespondence").removeAttr('disabled');
        }
    },
    DeseasedPersonSelectedIndexChanged: function (e) {
        if (ns.viewModel && ns.viewModel.currentModel && ns.viewModel.currentModel.indexOf("wfmContactTicketMaintenance") == 0) {
            if ($("#" + ns.viewModel.currentModel).find('#ddlDeceasedName option:selected').text() == "Other") {
                $("#" + ns.viewModel.currentModel).find("#lblName").removeClass("hideControl");
                $("#" + ns.viewModel.currentModel).find("#txtDeceasedName").removeClass("hideControl");
            }
            else {
                $("#" + ns.viewModel.currentModel).find("#lblName").addClass("hideControl");
                $("#" + ns.viewModel.currentModel).find("#txtDeceasedName").addClass("hideControl");
                $("#" + ns.viewModel.currentModel).find("#txtDeceasedName").text('').trigger("change");
            }
        }
    },
    PublishToWSS: function (e) {        
        var selectedVal = $("#" + ns.viewModel.currentModel).find('#ddlCorrespondenceList option:selected').val();
        var Parameters = {};
        Parameters["astrFileName"] = nsCorr.CurrentCorr.CorrFilePath;
        Parameters["CorrTemplate"] = nsCorr.CurrentCorr.CorrTemplate;
        setTimeout(function (e) {
            var data = {};
            data = nsCommon.SyncPost("PublishToWSS", Parameters,"POST");
            if (data != null && data != undefined && data.indexOf("Correspondence is successfully published!") == 0) {
                nsCommon.DispalyMessage(data, ns.viewModel.currentModel);
                $("#" + ns.viewModel.currentModel).find("#btnImageCorrespondence").attr('disabled', 'true');
                return false;
            }
            else {
                nsCommon.DispalyMessage(data, ns.viewModel.currentModel);
                return false;
            }
        }, 10);
        return false;
    },
    CorrespondenceImage: function (e) {        
        var selectedVal = $("#" + ns.viewModel.currentModel).find('#ddlCorrespondenceList option:selected').val();
        var Parameters = {};
        Parameters["astrFileName"] = nsCorr.CurrentCorr.CorrFilePath;
        Parameters["CorrTemplate"] = nsCorr.CurrentCorr.CorrTemplate;
        setTimeout(function (e) {
            var data = {};
            data = nsCommon.SyncPost("CorrespondenceImage", Parameters, "POST");
            if (data != null && data != undefined && data.indexOf("Correspondence is successfully imaged!") == 0) {
                nsCommon.DispalyMessage(data, ns.viewModel.currentModel);
                $("#" + ns.viewModel.currentModel).find("#btnImageCorrespondence").attr('disabled', 'true');
                return false;
            }
            else {
                nsCommon.DispalyMessage(data, ns.viewModel.currentModel);
                return false;
            }
        }, 10);
        return false;
    },
    NewBenefitApplication : function (e) {
        var lActiveDivId = e.context.activeDivID;
        var ldomDiv = $("#" + lActiveDivId);
        if (lActiveDivId.indexOf("wfmRetirementApplicationLookup") >= 0) {
            if (ldomDiv.find("#ddlRetirementBenefitTypeValue").length > 0) {
                var valueOfType = ldomDiv.find("#ddlRetirementBenefitTypeValue").val();
                if (valueOfType == "") {
                    nsCommon.DispalyError("1901 Benefit Type is required.", lActiveDivId);
                    return false;
                }
                else {
                    return true;
                }
            }
        }
        return true;
    },
    NewPersonAccountGHDVHSA: function (e) {
        var lActiveDivId = e.context.activeDivID;
        var ldomDiv = $("#" + lActiveDivId);
        if (lActiveDivId.indexOf("wfmHealthDentalVisonMaintenance") >= 0) {
            if (ldomDiv.find("#lblHSAPreTaxAgreement").length > 0 && ldomDiv.find("#ddlHealthInsuranceTypeValue").length > 0 && ldomDiv.find("#ddlCobraTypeValue").length > 0) {
                var lvalueOfHSAPreTaxAgreement = ldomDiv.find("#lblHSAPreTaxAgreement").text();
                var lhealthInsuranceTypeValue = ldomDiv.find("#ddlHealthInsuranceTypeValue").val();
                var lcobraTypeValue = ldomDiv.find("#ddlCobraTypeValue").val();
                if ((lhealthInsuranceTypeValue.toLowerCase() == "st12" || lhealthInsuranceTypeValue == "ns12") && (lcobraTypeValue == "") && (lvalueOfHSAPreTaxAgreement == "false")) {
                    nsCommon.DispalyError("10372 Employer does not have HSA Pre Tax Agreement on file.", lActiveDivId);
                    return false;
                }
                else {
                    return true;
                }
            }
        }
        return true;
    },
    fnCheckRecordSelect: function (e) {

        var btnId = $(e)[0].target.id;
        var relatedGrid = MVVMGlobal.GetControlAttribute($($(e)[0].target), 'sfwRelatedControl', e.context.activeDivID, false); // templateAttr
        if (relatedGrid != null) {
            var lobjGridOrListView = nsCommon.GetWidgetByActiveDivIdAndControlId(e.context.activeDivID, relatedGrid);
            var dataRows = lobjGridOrListView.getSelectedRows();
            if (dataRows.length == 0) {
                nsCommon.DispalyError("[ NO RECORDS SELECTED. PLEASE SELECT A RECORD. ]", e.context.activeDivID);
                return false;
            }

            if (dataRows.length > 1) {
                nsCommon.DispalyError("[ CANNOT SELECT MULTIPLE RECORDS. PLEASE SELECT A SINGLE RECORD.] ]", e.context.activeDivID);
                return false;
            }

        }
        return true;
    },
    fnInactiveSelectedOrgContact: function (e) {

        var btnId = $(e)[0].target.id;
        var relatedGrid = MVVMGlobal.GetControlAttribute($($(e)[0].target), 'sfwRelatedControl', e.context.activeDivID, false); // templateAttr
        if (relatedGrid != null) {
            var lobjGridOrListView = nsCommon.GetWidgetByActiveDivIdAndControlId(e.context.activeDivID, relatedGrid);
            var dataRows = lobjGridOrListView.getSelectedRows();
            if (dataRows.length == 0) {
                nsCommon.DispalyError("[ NO RECORDS SELECTED. PLEASE SELECT A RECORD. ]", e.context.activeDivID);
                return false;
            }

            if (dataRows.length > 1) {
                var ldtrSelected = "";
                var ldtrSelectedRow;
                var aintContactId = $("#lblContactId").text();
                for (var i = 0; i < dataRows.length; i++) {
                    if (dataRows[i].rowSelect == true) {
                        ldtrSelectedRow = dataRows[i];
                        ldtrSelected += ldtrSelectedRow.PrimaryKey + ",";
                    }                    
                }
                var Parameters = {};
                Parameters["aintOrgIDs"] = ldtrSelected;
                Parameters["aintContactId"] = aintContactId;
                nsRequest.SyncPost("InactiveSelectedOrgContact", Parameters, "POST");
            }
        }
        return true;
    },
    ValidateSelectedFiles: function (e) {
        if (e.context.previouslySelectedFiles != undefined && e.context.previouslySelectedFiles.length == 1) {
            return false;
        }
        return true;
    },
    ReportOffline: function (e) {
        if ($("#" + ns.viewModel.currentModel).find("#ddlReports").length > 0) {
            var valueOfType = $("#ddlReports").val();
            if (valueOfType == "") {
                nsCommon.DispalyError("No report selected. Please select the report to be Saved.", ns.viewModel.currentModel);
                return false;
            }
            else {
                $('#' + ns.viewModel.currentForm).find('input[type="text"][configured-validator="true"]').each(function () { $(this).trigger('blur'); });


                var lselectelements = $('#' + ns.viewModel.currentForm).find('select[configured-validator="true"]');

                if (lselectelements.length > 0) {
                    lselectelements.each(function () {
                    if ($(this).val() == "" || $(this).val() == undefined || $(this).val() == null) {
                        $(this).val("").trigger("change");
                    }
                });
            }
                if ($('#' + ns.viewModel.currentForm).find('div[class="validator-error"]').length > 0) {
                    return false;
                }
                ns.displayActivity(true);
                var CallDownload = function () {
                    var ActiveDivID = e.context.activeDivID;
                    var lbtnSelf = e.target;
                    var ldictParams;
                    if (e != undefined && (e.tagName === "A" || e.tagName === nsConstants.LABEL_TAG)) {
                        ldictParams = nsCommon.GetNavigationParams(lbtnSelf, e);
                    }
                    else {
                        ldictParams = nsCommon.GetNavigationParams(lbtnSelf);
                    }
                    var lstrFormID = nsCommon.GetFormNameFromDivID(ActiveDivID);
                    if (lstrFormID.indexOf("wfmhtx") === 0) {
                        lstrFormID = lstrFormID.replace("wfmhtx", "htx");
                    }
                    var Params = {
                        "FormID": lstrFormID
                    };
                    var RptForm = nsRpt.CurrentRpt.RptForm;
                    var ReportParams = {};
                    var NewFormID = nsCommon.GetProperFormId(RptForm);
                    if (ns.Templates[NewFormID].HeaderData.tblCriteria !== undefined) {
                        var JsonsearchParams = ns.Templates[NewFormID].HeaderData.tblCriteria.toJSON();
                        var params = ns.Templates[NewFormID].HeaderData.tblCriteria;
                        for (var keyss in params) {
                            if (keyss.charAt(0) == 'm' && params[keyss] == "") {
                                nsCommon.DispalyError("Parameter Value is Required", nsRpt.CurrentRpt.RptDivID);
                                return false;
                            }
                            if ($.type(JsonsearchParams[keyss]) === "array") {
                                var temp = "";
                                for (var i = 0; i < JsonsearchParams[keyss].length; i++) {
                                    if (i === 0)
                                        temp = JsonsearchParams[keyss][i];
                                    else
                                        temp = [temp, ",", JsonsearchParams[keyss][i]].join('');
                                }
                                JsonsearchParams[keyss] = temp;
                            }
                            if (JsonsearchParams[keyss] != undefined && JsonsearchParams[keyss].value != undefined) {
                                JsonsearchParams[keyss] = Params[keyss].value;
                            }
                            if (JsonsearchParams[keyss] != undefined && ((nsRpt.iblnAddEmptyReportParams === true) || (JsonsearchParams[keyss] != "" && JsonsearchParams[keyss] !== 0 && JsonsearchParams[keyss] !== "0"))) {
                                ReportParams[keyss] = JsonsearchParams[keyss];
                            }
                        }
                    }
                    if (RptForm != undefined && RptForm != "") {
                        ReportParams["ReportName"] = RptForm;
                    }
                    if (lstrFormID.indexOf(nsConstants.MAINTENANCE) > 0 || lstrFormID.indexOf("Wizard") > 0) {
                        var lblnSetResponse = false;
                        var dm = { HeaderData: {}, DetailsData: {}, KeysData: {} };
                        if (ns.DirtyData[ActiveDivID] !== undefined && ns.DirtyData[ActiveDivID].HeaderData !== undefined && Object.keys(ns.DirtyData[ActiveDivID].HeaderData).length > 0) {
                            dm.HeaderData = ns.DirtyData[ActiveDivID].HeaderData;
                            lblnSetResponse = true;
                        }
                        if (ns.DirtyData[ActiveDivID] !== undefined && ns.DirtyData[ActiveDivID].DetailsData !== undefined) {
                            dm.DetailsData = ns.DirtyData[ActiveDivID].DetailsData;
                            lblnSetResponse = true;
                        }
                        if (ns.viewModel[ActiveDivID] != undefined && ns.viewModel[ActiveDivID].KeysData != undefined) {
                            dm.KeysData = ns.viewModel[ActiveDivID].KeysData;
                            lblnSetResponse = true;
                        }
                        if (lblnSetResponse === true) {
                            Params["ResponseData"] = dm;
                        }
                    }
                    var lstrAction = ["SenderID=", ns.SenderID, "&SenderForm=", ns.SenderForm, "&Action=", "InsertReportRequest", "&SenderKey=", ns.SenderKey].join('');
                    var Prefix = MVVMGlobal.GetPrefixforAjaxCall();
                    var url = [Prefix, "api/", ns.ControllerName, "/InsertReportRequest?", lstrAction].join('');
                    url = [url, "&WindowName=", window.name].join('');
                    var inputs = "<input type='hidden' name='aobjDownload' value='" + JSON.stringify(Params) + "' />";
                    inputs += "<input type='hidden' name='adictParams' value='" + JSON.stringify(ReportParams) + "' />";
                    inputs += "<input type='hidden' name='ablnOpenInNewTab' value='" + JSON.stringify(ns.iblnDownloadFileInNewTab) + "' />";
                    inputs += '<input type="hidden" name="__RequestVerificationToken" value="' + $("input[name='__RequestVerificationToken']").val() + '" />';
                    inputs += '<input type="hidden" id="antiForgeryToken" value="' + $("#antiForgeryToken").val() + '" />';
                    var lstrTarget = (ns.iblnDownloadFileInNewTab) ? "target='_blank'" : '';
                    inputs += '<form action="' + url + '" method="' + 'post' + '"' + lstrTarget + ' >' + inputs + '</form>';
                    var $iframe = $("#downloadFmiframeFile");
                    if ($iframe.length > 0) {
                        $iframe.remove();
                        $iframe = $("#downloadFmiframeFile");
                    }
                    if ($iframe.length == 0) {
                        var iFrame = document.createElement('iframe');
                        document.body.appendChild(iFrame);
                        nsCommon.setAttributes(iFrame, { id: "downloadFmiframeFile", height: "0", width: "0", border: "0", wmode: "Opaque" });
                        iFrame.setAttribute("id", "downloadFmiframeFile");
                        iFrame.style.position = "absolute";
                        iFrame.style.top = "-999";
                        iFrame.style.left = "-999";
                        iFrame.style.display = "none";
                        $iframe = $(iFrame);
                    }
                    var frameWindow = $iframe.get(0);
                    frameWindow = frameWindow.contentWindow || frameWindow.contentDocument || frameWindow;
                    var wdoc = frameWindow.document || frameWindow.contentDocument || frameWindow;
                    wdoc.write(HtmlWhitelistedSanitizer.sanitizeHTMLString(inputs));
                    if (wdoc.body && wdoc.body.querySelector('form') != null) {
                        wdoc.body.querySelector('form').submit();
                    }
                    wdoc.close();
                    var iframe = $iframe[0];
                    if (iframe != null && iframe.contentDocument != undefined && iframe.contentDocument.getElementsByTagName != undefined && iframe.contentDocument.getElementsByTagName('body')) {
                        iframe.contentDocument.getElementsByTagName('body')[0].innerHTML = "";
                    }
                    $iframe.contents().find('body').empty();
                    ns.displayActivity(false);
                };
                setTimeout(CallDownload, 200);

            }
        }
    },

    ConfirmPayPeriodAmount: function (e) {
    var ldomDiv = e.context.idomActiveDiv != undefined ? e.context.idomActiveDiv : $("#" + e.context.activeDivID);
    var valueOfTextBox1 = ldomDiv.find("#txtTextBox1").val();
    var valueOfPerPayPeriodContributionAmt1 = ldomDiv.find("#txtPerPayPeriodContributionAmt1").val();
    if (valueOfTextBox1 == "$0.00") {
        return confirm('The Amount Per Pay Period is Zero. Do you want to save the record?');
    }
    else if (valueOfPerPayPeriodContributionAmt1 == "$0.00") {
        return confirm('The Amount Per Pay Period is Zero. Do you want to save the record?');
    }
    else {
        return true;
    }

    },
    ConfirmIsDependentMedicareEsrd: function (e) {
        var ldomDiv = e.context.idomActiveDiv != undefined ? e.context.idomActiveDiv : $("#" + e.context.activeDivID);
        var valueOfIsDependentMedicareEsrd = $("#lblIsDependentMedicareEsrd").text();
        if (valueOfIsDependentMedicareEsrd == "Y") {
            return confirm('Please check ESRD Physician Statement.');
        }
        else {
            return true;
        }
    },

    btnMap_Click: function (e) {
        var FormContainerID = "";
        var ActiveDivID = "";
        var RelatedGridID = "";

        var lbtnSelf;
        var lintSelectedIndex = -1;
        if (e != undefined && e.tagName === "A") {
            lbtnSelf = $(e)[0];
            var FormContainerID = "#" + $(e).closest('div[role="group"]')[0].id;
            var ActiveDivID = $(e).closest('div[id^="wfm"]')[0].id;
            lbtnSelf = $(FormContainerID + " #" + ActiveDivID + " #" + MVVMGlobal.GetControlAttribute(lbtnSelf, "sfwRelatedControl", ActiveDivID))[0];
            lintSelectedIndex = e.getAttribute("rowIndex");
        } else {
            lbtnSelf = ns.viewModel.srcElement;
            var FormContainerID = "#" + $(lbtnSelf).closest('div[role="group"]')[0].id;
            ActiveDivID = $(lbtnSelf).closest('div[id^="wfm"]')[0].id;
        }

        RelatedGridID = MVVMGlobal.GetControlAttribute(lbtnSelf, "sfwRelatedControl", ActiveDivID);
        if (RelatedGridID !== null) {
            var lobjGridWidget = nsCommon.GetWidgetByActiveDivIdAndControlId(ActiveDivID, RelatedGridID);
            if (lobjGridWidget === undefined || lobjGridWidget.jsObject === undefined) {
                return false;
            }
        }

        var PrimaryId = 0;
        if (ActiveDivID.lastIndexOf("wfmBPMCaseMaintenance", 0) === 0) {
            PrimaryId = parseInt($("#" + e.context.activeDivID + " #lblCaseId").text());
        }
        else if (ActiveDivID.lastIndexOf("wfmBPMInitiationMaintenance", 0) === 0) {
            PrimaryId = parseInt($("#" + e.context.activeDivID + " #ddlProcessId").val());
        }
        else {
            var ldictParams = nsCommon.GetNavigationParams(lbtnSelf, e);
        }

        if (ActiveDivID.lastIndexOf("wfmDesignSpecificationMaintenance", 0) === 0) {
            nsCommon.localStorageSet("design_specification_bpm_map_id", ldictParams.lstrFirstID);
            //window.open(ns.SiteName + "/BPMExecution/MapRender.html", "_blank");
            window.open(ns.SiteName + "/Home/MapRender", "_blank");
        }
        else if (ActiveDivID.lastIndexOf("wfmBPMInitiationMaintenance", 0) === 0) {
            nsCommon.localStorageSet("ProcessId", PrimaryId);
            nsCommon.localStorageSet("CaseId", 0);
            window.open(ns.SiteName + "/Home/BPMNReadOnlyMap", "_blank");
        }
        else if (ActiveDivID.lastIndexOf("wfmBPMCaseLookup", 0) === 0 || ActiveDivID.lastIndexOf("wfmBPMCaseMaintenance", 0) === 0) {
            nsCommon.localStorageSet("ProcessId", 0);
            nsCommon.localStorageSet("CaseId", PrimaryId > 0 ? PrimaryId : ldictParams.lstrFirstID);
            var w = window.open(ns.SiteName + "/Home/BPMNReadOnlyMap", "_blank");
            w.name = window.name;
        }
        else {
            nsCommon.localStorageSet("CaseInstanceID", ldictParams.lstrFirstID);
            //window.open(ns.SiteName + "/BPMExecution/BPMNMap.html", "_blank");
            window.open(ns.SiteName + "/Home/BPMNMap", "_blank");
        }
    },
    // Added JS Method to confirm for 2nd activity on submit of 1st bpm activity on BPM wfmBpmActivityInstanceMaintenance screen
    InitiateSecondActivityConfirmation: function (e) {
        var data = $('#' + e.context.activeDivID);
        var result = true;

        if (e.currentTarget.id == "btnCompleteActivity") {
            var CheckNextActivity = data.find("#chkNextActivityInitiation").val();
            if ($('#chkNextActivityInitiation').is(':visible') && !($('#chkNextActivityInitiation').is(':checked'))) {
                result = confirm("Are you sure you do not want to Initiate ER Training activity?");
            }
        }
        return result;
    },
// Added JS Method to Handle Delete Confirmation on BPM wfmBpmActivityInstanceMaintenance screen
    SuspendAndTerminateConfirmation: function (e) {
        var data = $('#' + e.context.activeDivID);
        var result = true;

        if (e.currentTarget.id == "btnSuspendActivity") {
            var SuspensionReason = data.find("#ddlCMSuspensionReasonValue").val();
            var SuspensionDate = data.find("#txtCMSuspensionDate").val();
            var ResumeAction = data.find("#ddlCMResumeActionValue").val();
            var SuspensionNote = data.find("#txtCMComments").val();

            if (SuspensionReason !== "" && SuspensionDate !== "" && ResumeAction !== "" && SuspensionNote !== "") {
                result = confirm("Are you sure you want to Suspend the activity?");
            }
        }
        else if (e.currentTarget.id === "btnCancelActivity") {
            var TerminationReason = data.find("#txtTerminationReason").val();

            if (TerminationReason !== "") {
                result = confirm("Are you sure you want to terminate the workflow?");
            }
        }

        return result;
    },
    OpenFileNetUrl: function (e) {
        var relatedGrid = MVVMGlobal.GetControlAttribute($(e.srcElement), 'sfwRelatedControl', e.context.activeDivID, false); // templateAttr
        if (relatedGrid != null) {
            var ldictParams = nsCommon.GetNavigationParams($(e.srcElement), e);
            var aSelectedRecord = ldictParams.larrRows;
            if (aSelectedRecord != undefined && aSelectedRecord[0] != undefined) {
                try {
                    var ObjectStore = aSelectedRecord[0].astrObjectStore;
                    var VersionSeriesID = aSelectedRecord[0].astrVersionSeriesId;
                    var DocumentID = aSelectedRecord[0].astrDocumentId;
                    var DocumentTitle = aSelectedRecord[0].astrDocumentTitle;

                    var Parameters = {};
                    Parameters["astrObjectStore"] = ObjectStore;
                    Parameters["astrVersionSeriesID"] = VersionSeriesID;
                    Parameters["astrDocumentID"] = DocumentID;
                    Parameters["astrDocumentTitle"] = DocumentTitle;
                    var data = nsCommon.SyncPost("ReturnFileNetURL", Parameters, "POST"); 
                    if (data && data != undefined && data != "") {
                            window.open(
                                data, "Window" + Math.random().toString(16).slice(2),
                                "height=470px,width=850px,modal=yes,alwaysRaised=yes,top=100,left=200,toolbar=1,Location=0,Directories=0,Status=0,menubar=1,Scrollbars=1,Resizable=1");
                    }
                    else {
                        console.log(data);
                    }
                }
                catch (ex) {
                    console.log(ex);
                }
            }
            return false;
        }
    }
};





nsCommon.SetTitle = function (astrTitle) {
    $("#FormTitle").html(astrTitle);
    document.title = astrTitle;
    $("#LookupFormTitle").text(astrTitle);
    $('#lblForm').html(ns.viewModel.currentModel);
};



//nsCommon.GetPageTitle = function (title, primarykey) {
//    return title.replace("_PrimaryKey", '');
//}
//FW Upgrade PIR 17642 Framework PIR is raised for same issue. please remove the below "nsCommon.GetNavigationParams" method after framework provides fix. please check below method in framework for any changes in it for every migration.
nsCommon.GetNavigationParams = function (lbtnSelf, e) {
    var ActiveDivID = "";
    ActiveDivID = nsCommon.GetActiveDivId(lbtnSelf);
    var lintRowIndex = -1;
    if (e !== undefined && e !== null && e !== "") {
        if (e.getAttribute != null && e.getAttribute("rowIndex") != null)
            lintRowIndex = e.getAttribute("rowIndex");
    }
    else if ((lbtnSelf != null && $(lbtnSelf).length > 0 && lbtnSelf.getAttribute && lbtnSelf.getAttribute("NoRowIndexForSelect") == undefined && (lbtnSelf.getAttribute("rowIndex") != undefined)) && (lbtnSelf.getAttribute("GridID") != undefined || lbtnSelf.getAttribute("ListViewID") != undefined)) {
        lintRowIndex = lbtnSelf.getAttribute("rowIndex");
    }
    if (lintRowIndex == null)
        lintRowIndex = -1;
    var lstrGrid = MVVMGlobal.GetControlAttribute(lbtnSelf, nsConstants.SFW_RELATED_CONTROL, ActiveDivID);
    if ((lstrGrid == null || lstrGrid == undefined) && $(lbtnSelf).attr("gridid") != null) {
        lstrGrid = $(lbtnSelf).attr("gridid");
    }
    var lstrObjectField = MVVMGlobal.GetControlAttribute(lbtnSelf, "sfwActiveFormField", ActiveDivID);
    if (lstrObjectField == null) {
        var lstrObjectField = MVVMGlobal.GetControlAttribute(lbtnSelf, "sfwObjectField", ActiveDivID);
        if (lstrObjectField == null)
            var lstrObjectField = MVVMGlobal.GetControlAttribute(lbtnSelf, "sfwEntityField", ActiveDivID);
    }
    var lstrActiveForm = MVVMGlobal.GetControlAttribute(lbtnSelf, "sfwActiveForm", ActiveDivID);
    var sfwMethodName = MVVMGlobal.GetControlAttribute(lbtnSelf, "sfwMethodName", ActiveDivID);
    var ldctActiveForms = {};
    var lstrObjectFieldValue = null;
    if (lstrObjectField != null && lstrActiveForm != undefined) {
        if (lstrObjectField.length > 1) {
            var ldomActive = $("#" + ns.viewModel.currentForm).find("#" + lstrObjectField);
            if (ldomActive != undefined && ldomActive.length)
                lstrObjectFieldValue = ldomActive[0].value;
        }
        var larrFormList = lstrActiveForm.split(";");
        var larrTemp = [];
        for (var i = 0; i < larrFormList.length; i++) {
            larrTemp = larrFormList[i].split("=");
            if (larrTemp.length === 2) {
                ldctActiveForms[larrTemp[0]] = larrTemp[1] === undefined ? larrTemp[0] : larrTemp[1];
            }
        }
    }
    var lstrFirstID = "";
    var lstrNavParam = MVVMGlobal.GetControlAttribute(lbtnSelf, "sfwNavigationParameter", ActiveDivID);
    var larrRows = [];
    var larrNodeInfo = [];
    var ldtrSelected;
    if (lstrNavParam != null && lstrNavParam.trim() != "") {
        var larrNavField = lstrNavParam.split(";");
        var sfwTitleField = MVVMGlobal.GetControlAttribute(lbtnSelf, "sfwFormTitleField", ActiveDivID);
        var sfwToolTipField = MVVMGlobal.GetControlAttribute(lbtnSelf, "sfwFormToolTipField", ActiveDivID);
        var PrimaryKey = "";
        if (lstrGrid != null && sfwMethodName !== "btnGridSearch_Click" && sfwMethodName !== "btnGridSearchCriteriaReq_Click" && sfwMethodName !== "btnExportAllToExcel_Click") {
            var lobjGridOrListView = nsCommon.GetWidgetByActiveDivIdAndControlId(ActiveDivID, lstrGrid);
            if (lobjGridOrListView == undefined) {
                lobjGridOrListView = nsCommon.GetWidgetByActiveDivIdAndControlId(ActiveDivID, [nsConstants.LISTVIEW_CONTAINER_UNDERSCORE, lstrGrid].join(''));
                if (lobjGridOrListView == undefined && (lbtnSelf != null && $(lbtnSelf).length > 0 && lbtnSelf.getAttribute("NoRowIndexForSelect") != undefined && lbtnSelf.getAttribute("rowIndex") != undefined)) {
                    lobjGridOrListView = nsCommon.GetWidgetByActiveDivIdAndControlId(ActiveDivID, [lstrGrid, lbtnSelf.getAttribute("rowIndex")].join(''));
                }
            }
            var dataRows = [];
            var gridColumns;
            if (lobjGridOrListView != undefined && lobjGridOrListView.jsObject != undefined) {
                dataRows = lobjGridOrListView.getSelectedRows(lintRowIndex, false, true);
                gridColumns = lobjGridOrListView.iarrAllColumns;
                for (var i = 0; i < dataRows.length; i++) {
                    if (dataRows[i].rowSelect == true || lintRowIndex >= 0) {
                        PrimaryKey = "";
                        var ldtrSelected = "";
                        var ldctParams = {};
                        var ldctNodeInfo = {};
                        larrNavField.forEach(function (astrNavField) {
                            var larrField = astrNavField.split("=");
                            var lstrParamName = larrField[0];
                            var lstrParamValue = larrField[1] === undefined ? larrField[0] : larrField[1];
                            if (lintRowIndex >= 0) {
                                var SelectedItems = $.grep(dataRows, function (a) {
                                    return a.rowIndex == lintRowIndex;
                                });
                                ldtrSelected = SelectedItems[0];
                            }
                            else {
                                ldtrSelected = dataRows[i];
                            }
                            if (ldtrSelected != undefined && ldtrSelected.rowSelect != undefined) {
                                if (ActiveDivID.indexOf("wfmCorTrackingLookup") == 0 && ns.viewModel != undefined && ns.viewModel.srcElement != undefined && ns.viewModel.srcElement.id != undefined && ns.viewModel.srcElement.id == "Edit" && ldtrSelected.rowSelect == true) {
                                    ldtrSelected.rowSelect = true;
                                }
                                else {
                                    ldtrSelected.rowSelect = false;
                                }
                            }
                            if (lstrFirstID === "") {
                                lstrFirstID = MVVMGlobal.GetValueOf(lstrParamValue, ldtrSelected, ActiveDivID);
                            }
                            if (PrimaryKey === "") {
                                PrimaryKey = ldtrSelected.PrimaryKey;
                            }
                            ldctParams[lstrParamName] = MVVMGlobal.GetValueOf(lstrParamValue, ldtrSelected, ActiveDivID);
                            ldctParams["rowIndex"] = ldtrSelected.rowIndex;
                            var arrGridColumns = [];
                            arrGridColumns = $.grep(gridColumns, function (a) {
                                return a.field == lstrParamValue;
                            });
                            if (arrGridColumns.length == 1 && arrGridColumns[0].format != undefined && arrGridColumns[0].format != "" && (arrGridColumns[0].format == "{0:MM/dd/yyyy}" || arrGridColumns[0].format == "{0:d}")) {
                                ldctParams[lstrParamName] = ns.ApplyCustomFormatForGrid(arrGridColumns[0].format, ldtrSelected[lstrParamValue]);
                            }
                            if (lstrObjectField !== null && lstrObjectField !== undefined && ldctActiveForms != undefined && Object.keys(ldctActiveForms).length > 0) {
                                var lstrFieldValue = ldtrSelected[lstrObjectField];
                                var lstrCaption = lstrObjectField;
                                arrGridColumns = $.grep(gridColumns, function (a) {
                                    return a.field == lstrObjectField;
                                });
                                if (arrGridColumns != undefined && arrGridColumns.length === 1 && arrGridColumns[0].title != undefined && arrGridColumns[0].title != "") {
                                    lstrCaption = arrGridColumns[0].title;
                                }
                                if (lstrFieldValue == "" || lstrFieldValue == null) {
                                    nsCommon.DispalyError(DefaultMessages.UnableToSelectActiveForm.replace("{0}", lstrCaption), ActiveDivID);
                                    return;
                                }
                                lstrActiveForm = ldctActiveForms[lstrFieldValue];
                                if (lstrActiveForm == undefined) {
                                    nsCommon.DispalyError(DefaultMessages.InvalidControlValue.replace("{0}", lstrCaption).replace("{1}", lstrFieldValue), ActiveDivID);
                                    return;
                                }
                            }
                            ldctNodeInfo["ActiveForm"] = lstrActiveForm;
                        });
                        ldctNodeInfo["PrimaryKey"] = PrimaryKey;
                        if (sfwTitleField === null) {
                            var NavigationTitle = MVVMGlobal.GetNavigationTitle(lstrActiveForm);
                            ldctNodeInfo["Title"] = NavigationTitle;
                        }
                        else {
                            var NavigationTitle = MVVMGlobal.GetValueOf(sfwTitleField, ldtrSelected, ActiveDivID);
                            ldctNodeInfo["Title"] = NavigationTitle;
                        }
                        if (sfwToolTipField === null) {
                            ldctNodeInfo["ToolTip"] = PrimaryKey;
                        }
                        else {
                            ldctNodeInfo["ToolTip"] = MVVMGlobal.GetValueOf(sfwToolTipField, ldtrSelected, ActiveDivID);
                        }
                        larrRows.push(ldctParams);
                        larrNodeInfo.push(ldctNodeInfo);
                        if (lintRowIndex >= 0)
                            break;
                    }
                }
                lobjGridOrListView.refresh();
            }
        }
        else {
            var ldctParams = {};
            var ldctNodeInfo = {};
            larrNavField.forEach(function (astrNavField) {
                var larrField = astrNavField.split("=");
                var lstrParamName = larrField[0];
                var lstrParamValue = larrField[1] === undefined ? larrField[0] : larrField[1];
                var lstrConstantParam = false;
                var GetDataFromDict = {};
                if (ActiveDivID.indexOf(nsConstants.LOOKUP) > 0) {
                    GetDataFromDict = ns.Templates[ActiveDivID].HeaderData["tblCriteria"];
                    if (lstrParamValue.indexOf(nsConstants.HASH) !== 0) {
                        var controlAttributes = ns.Templates[ActiveDivID].ControlAttribites;
                        for (var control in controlAttributes) {
                            if (controlAttributes[control].sfwDataField === lstrParamValue) {
                                lstrParamValue = control;
                                break;
                            }
                        }
                    }
                }
                else {
                    GetDataFromDict = ns.viewModel[ActiveDivID].HeaderData.MaintenanceData;
                }
                if (lstrFirstID === "") {
                    if (GetDataFromDict[lstrParamValue] != undefined && GetDataFromDict[lstrParamValue] != null)
                        lstrFirstID = GetDataFromDict[lstrParamValue].toString();
                }
                var sfwMethodName = MVVMGlobal.GetControlAttribute(ns.viewModel.srcElement, "sfwMethodName", ActiveDivID);
                if (GetDataFromDict[lstrParamValue] !== undefined && GetDataFromDict[lstrParamValue] != null && sfwMethodName !== "btnGridSearch_Click" && sfwMethodName !== "btnGridSearchCriteriaReq_Click" && sfwMethodName !== "btnExportAllToExcel_Click") {
                    ldctParams[lstrParamName] = GetDataFromDict[lstrParamValue].toString();
                }
                else if (sfwMethodName == "btnNew_Click" && $("#" + ActiveDivID + " #" + lstrParamValue).length == 1) {
                    ldctParams[lstrParamName] = $("#" + ActiveDivID + " #" + lstrParamValue).val();
                }
                if (lstrParamValue.indexOf(nsConstants.HASH) === 0) {
                    lstrParamValue = lstrParamValue.replace(nsConstants.HASH, "");
                    lstrConstantParam = true;
                }
                if (lstrConstantParam === true) {
                    ldctParams[lstrParamName] = lstrParamValue;
                }
                else if (ActiveDivID.indexOf(nsConstants.LOOKUP) < 0 && (sfwMethodName === "btnGridSearch_Click" || sfwMethodName === "btnGridSearchCriteriaReq_Click" || sfwMethodName === "btnExportAllToExcel_Click")) {
                    var CriteriaControl = $([nsConstants.HASH, ActiveDivID, nsConstants.SPACE_HASH, lstrParamValue].join(''));
                    if (CriteriaControl.length > 0) {
                        var value = CriteriaControl.val();
                        var lblnCheckBoxList = (CriteriaControl[0].getAttribute(nsConstants.DATA_SFW_CONTROL_TYPE) != null && CriteriaControl[0].getAttribute(nsConstants.DATA_SFW_CONTROL_TYPE).toLowerCase() == nsConstants.SFW_CHECKBOX_LIST_LOWER);
                        if (lblnCheckBoxList) {
                            value = nsCommon.GetUnformatedValue(GetDataFromDict, lstrParamValue, ActiveDivID);
                        }
                        else if (CriteriaControl.attr(nsConstants.TYPE) != null && CriteriaControl[0].getAttribute(nsConstants.TYPE).toLowerCase() == nsConstants.CHECKBOX) {
                            if (CriteriaControl.is(":checked")) {
                                var sfwValueChecked = CriteriaControl[0].getAttribute("sfwValueChecked");
                                if (sfwValueChecked == null)
                                    sfwValueChecked = "Y";
                                value = sfwValueChecked;
                            }
                            else {
                                value = "";
                            }
                        }
                        if (value == null) {
                            value = "";
                        }
                        if (value != "" && value != null) {
                            if (CriteriaControl[0].tagName === nsConstants.SELECT_TAG && CriteriaControl[0].getAttribute("multiple") === "multiple") {
                                ldctParams[lstrParamValue] = value.join();
                            }
                            else if (lblnCheckBoxList === undefined)
                                ldctParams[lstrParamValue] = value;
                            else if (ldctParams[lstrParamValue] === undefined)
                                ldctParams[lstrParamValue] = value;
                            else
                                ldctParams[[lstrParamValue, "_2"].join('')] = value;
                        }
                    }
                }
                if (lstrObjectField != null) {
                    if (Object.keys(ldctActiveForms).length == 1) {
                        lstrActiveForm = Object.keys(ldctActiveForms)[0];
                        return;
                    }
                    var lstrFieldValue = GetDataFromDict[lstrObjectField];
                    lstrActiveForm = ldctActiveForms[lstrFieldValue] === undefined ? lstrActiveForm : ldctActiveForms[lstrFieldValue];
                }
                ldctNodeInfo["ActiveForm"] = lstrActiveForm;
            });
            ldctNodeInfo["PrimaryKey"] = lstrFirstID;
            if (sfwTitleField === null) {
                ldctNodeInfo["Title"] = lstrFirstID;
            }
            else {
                if (ldtrSelected !== undefined && ldtrSelected[sfwTitleField] !== undefined) {
                    ldctNodeInfo["Title"] = ldtrSelected[sfwTitleField];
                }
                else {
                    if (ns.viewModel[ActiveDivID] !== undefined && ns.viewModel[ActiveDivID].HeaderData !== undefined && ns.viewModel[ActiveDivID].HeaderData.MaintenanceData !== undefined && ns.viewModel[ActiveDivID].HeaderData.MaintenanceData[sfwTitleField] !== undefined) {
                        ldctNodeInfo["Title"] = ns.viewModel[ActiveDivID].HeaderData.MaintenanceData[sfwTitleField];
                    }
                    else {
                        ldctNodeInfo["Title"] = lstrFirstID;
                    }
                }
            }
            if (sfwToolTipField === null) {
                ldctNodeInfo["ToolTip"] = lstrFirstID;
            }
            else {
                ldctNodeInfo["ToolTip"] = ldtrSelected[sfwToolTipField];
            }
            larrRows.push(ldctParams);
            larrNodeInfo.push(ldctNodeInfo);
        }
    }
    else if (lstrObjectField != undefined && ldctActiveForms != undefined && Object.keys(ldctActiveForms).length > 0) {
        var sfwTitleField = MVVMGlobal.GetControlAttribute(lbtnSelf, "sfwFormTitleField", ActiveDivID);
        var sfwToolTipField = MVVMGlobal.GetControlAttribute(lbtnSelf, "sfwFormToolTipField", ActiveDivID);
        var PrimaryKey = "";
        if (lstrGrid != null && sfwMethodName !== "btnGridSearch_Click" && sfwMethodName !== "btnGridSearchCriteriaReq_Click") {
            var lobjGridOrListView = nsCommon.GetWidgetByActiveDivIdAndControlId(ActiveDivID, lstrGrid);
            if (lobjGridOrListView == undefined) {
                lobjGridOrListView = nsCommon.GetWidgetByActiveDivIdAndControlId(ActiveDivID, [nsConstants.LISTVIEW_CONTAINER_UNDERSCORE, lstrGrid].join(''));
                if (lobjGridOrListView == undefined && (lbtnSelf != null && $(lbtnSelf).length > 0 && lbtnSelf.getAttribute("NoRowIndexForSelect") != undefined && lbtnSelf.getAttribute("rowIndex") != undefined)) {
                    lobjGridOrListView = nsCommon.GetWidgetByActiveDivIdAndControlId(ActiveDivID, [lstrGrid, lbtnSelf.getAttribute("rowIndex")].join(''));
                }
            }
            var dataRows = [];
            var gridColumns;
            if (lobjGridOrListView != undefined && lobjGridOrListView.jsObject != undefined) {
                dataRows = lobjGridOrListView.getSelectedRows(lintRowIndex, false, true);
                gridColumns = lobjGridOrListView.iarrAllColumns;
                if (dataRows.length > 0) {
                    if (dataRows[0].rowSelect == true || lintRowIndex >= 0) {
                        PrimaryKey = "";
                        ldtrSelected = "";
                        var ldctParams = {};
                        var ldctNodeInfo = {};
                        if (lintRowIndex >= 0) {
                            var SelectedItems = $.grep(dataRows, function (a) {
                                return a.rowIndex == lintRowIndex;
                            });
                            ldtrSelected = SelectedItems[0];
                        }
                        else {
                            ldtrSelected = dataRows[0];
                        }
                        if (ldtrSelected != undefined && ldtrSelected.rowSelect != undefined) {
                            if (ActiveDivID.indexOf("wfmCorTrackingLookup") == 0 && ns.viewModel != undefined && ns.viewModel.srcElement != undefined && ns.viewModel.srcElement.id != undefined && ns.viewModel.srcElement.id == "Edit" && ldtrSelected.rowSelect == true) {
                                ldtrSelected.rowSelect = true;
                            }
                            else {
                                ldtrSelected.rowSelect = false;
                            }
                        }
                        if (PrimaryKey === "") {
                            PrimaryKey = ldtrSelected.PrimaryKey;
                        }
                        ldctParams["rowIndex"] = ldtrSelected.rowIndex;
                        var arrGridColumns = [];
                        if (lstrObjectField !== null && lstrObjectField !== undefined && ldctActiveForms != undefined && Object.keys(ldctActiveForms).length > 0) {
                            var lstrFieldValue = ldtrSelected[lstrObjectField];
                            var lstrCaption = lstrObjectField;
                            arrGridColumns = $.grep(gridColumns, function (a) {
                                return a.field == lstrObjectField;
                            });
                            if (arrGridColumns != undefined && arrGridColumns.length === 1 && arrGridColumns[0].title != undefined && arrGridColumns[0].title != "") {
                                lstrCaption = arrGridColumns[0].title;
                            }
                            if (lstrFieldValue == "" || lstrFieldValue == null) {
                                nsCommon.DispalyError(DefaultMessages.UnableToSelectActiveForm.replace("{0}", lstrCaption), ActiveDivID);
                                return;
                            }
                            lstrActiveForm = ldctActiveForms[lstrFieldValue];
                            if (lstrActiveForm == undefined) {
                                nsCommon.DispalyError(DefaultMessages.InvalidControlValue.replace("{0}", lstrCaption).replace("{1}", lstrFieldValue), ActiveDivID);
                                return;
                            }
                        }
                        ldctNodeInfo["ActiveForm"] = lstrActiveForm;
                        ldctNodeInfo["PrimaryKey"] = PrimaryKey;
                        if (sfwTitleField === null) {
                            var NavigationTitle = MVVMGlobal.GetNavigationTitle(lstrActiveForm);
                            ldctNodeInfo["Title"] = NavigationTitle;
                        }
                        else {
                            var NavigationTitle = MVVMGlobal.GetValueOf(sfwTitleField, ldtrSelected, ActiveDivID);
                            ldctNodeInfo["Title"] = NavigationTitle;
                        }
                        if (sfwToolTipField === null) {
                            ldctNodeInfo["ToolTip"] = PrimaryKey;
                        }
                        else {
                            ldctNodeInfo["ToolTip"] = MVVMGlobal.GetValueOf(sfwToolTipField, ldtrSelected, ActiveDivID);
                        }
                        larrRows.push(ldctParams);
                        larrNodeInfo.push(ldctNodeInfo);
                    }
                }
                lobjGridOrListView.refresh();
            }
            else if (lstrObjectFieldValue != null || lstrObjectFieldValue.length > 1) {
                lstrActiveForm = ldctActiveForms[lstrObjectFieldValue];
            }
        }
        else {
            var ldctParams = {};
            var ldctNodeInfo = {};
            var GetDataFromDict = {};
            if (ActiveDivID.indexOf(nsConstants.LOOKUP) > 0) {
                GetDataFromDict = ns.Templates[ActiveDivID].HeaderData["tblCriteria"];
            }
            else {
                GetDataFromDict = ns.viewModel[ActiveDivID].HeaderData.MaintenanceData;
            }
            if (lstrObjectField != null) {
                if (Object.keys(ldctActiveForms).length == 1) {
                    lstrActiveForm = Object.keys(ldctActiveForms)[0];
                    return;
                }
                var lstrFieldValue = GetDataFromDict[lstrObjectField];
                lstrActiveForm = ldctActiveForms[lstrFieldValue] === undefined ? lstrActiveForm : ldctActiveForms[lstrFieldValue];
            }
            ldctNodeInfo["ActiveForm"] = lstrActiveForm;
            ldctNodeInfo["PrimaryKey"] = lstrFirstID;
            if (sfwTitleField === null) {
                ldctNodeInfo["Title"] = lstrFirstID;
            }
            else {
                if (ldtrSelected != undefined && ldtrSelected[sfwTitleField] !== undefined) {
                    ldctNodeInfo["Title"] = ldtrSelected[sfwTitleField];
                }
                else {
                    if (ns.viewModel[ActiveDivID] !== undefined && ns.viewModel[ActiveDivID].HeaderData !== undefined && ns.viewModel[ActiveDivID].HeaderData.MaintenanceData !== undefined && ns.viewModel[ActiveDivID].HeaderData.MaintenanceData[sfwTitleField] !== undefined) {
                        ldctNodeInfo["Title"] = ns.viewModel[ActiveDivID].HeaderData.MaintenanceData[sfwTitleField];
                    }
                    else {
                        ldctNodeInfo["Title"] = lstrFirstID;
                    }
                }
            }
            if (sfwToolTipField === null) {
                ldctNodeInfo["ToolTip"] = lstrFirstID;
            }
            else {
                ldctNodeInfo["ToolTip"] = ldtrSelected[sfwToolTipField];
            }
            larrRows.push(ldctParams);
            larrNodeInfo.push(ldctNodeInfo);
            if (lstrObjectFieldValue != null || lstrObjectFieldValue.length > 1) {
                lstrActiveForm = ldctActiveForms[lstrObjectFieldValue];
            }
        }
    }
    if (!nsCommon.isNumber(lstrFirstID)) {
        lstrFirstID = 0;
    }
    var Params = { larrRows: larrRows, lstrActiveForm: lstrActiveForm, lstrFirstID: lstrFirstID, larrNodeInfo: larrNodeInfo };
    return Params;
}



//To Display Message on Correspondence Pop up by clicking edit Button
function SignalRCallToEditCorr(data) {
    ns.displayActivity(true);
    nsCorr.FileData = "";
    if ($.CorrHubConnection) {
        nsCorr.RegisterClientFunctions();
        $.CorrHubConnection.start().done(function () {
            $.CorrHubConnection.proxies.corrsignalrhub.server.setWindowName(window.name).done(function () {
            });
           // alert(data.ResponseMessage.istrMessage);
             nsCommon.DispalyMessage(data.ResponseMessage.istrMessage, ns.viewModel.currentModel);
            var CorrData = data.DomainModel.OtherData.CorrData;
            var Base64Chunks = CorrData.Base64String;
            delete CorrData.Base64String;
            $.CorrHubConnection.proxies.corrsignalrhub.server.createCorrInstance(JSON.stringify(CorrData)).done(function () {
                nsCorr.SendChunkByIndex(0, Base64Chunks);
            });
        }).fail(function () {
            alert(DefaultMessages.CorrEditorServiceNotRunning);
            console.log('Could not Connect!');
        });
    }
    ns.displayActivity(false);
}

nsCorr.SignalRCallToEditCorr = SignalRCallToEditCorr; 


function setHelpFileFunction() {
    var data = {};
    data = nsRequest.SyncPost("SetHelpFile", "POST");

    if (data != undefined && data != "" && data != null) {
        window.open(data, null, 'left=200,top=100,width=1100,height=650,resizable=yes,toolbar=yes,scrollbars=yes,alwaysRaised=yes');
    }
}
function SetCommentAndIgnorStatus(e, kgrid, ldomDiv, ablnIsSetIgnorStatus)
{
    var dataRows = kgrid.dataSource.data;
    var lstrComment = ldomDiv.find("#txtDetailComment").val();
    var lstrEmploymentDetailID = "";
    if (dataRows.length != 0) {
        for (var i = 0; i < dataRows.length; i++) {
            if (dataRows[i].rowSelect == true) {
                if (lstrEmploymentDetailID == "") {
                    lstrEmploymentDetailID = dataRows[i].dt_PayrollDetailID_0_0;
                }
                else {
                    lstrEmploymentDetailID = lstrEmploymentDetailID + "," + dataRows[i].dt_PayrollDetailID_0_0;
                }
            }
        }
    }
    if (lstrEmploymentDetailID == "") {
        nsCommon.DispalyError("No record selected. Please select record(s) and try again.", e.context.activeDivID);
        return false;
    }
    var Parameters = {};
    Parameters["PayrollDeatilsIDs"] = lstrEmploymentDetailID;
    Parameters["Comment"] = lstrComment;
    Parameters["ablnIsSetIgnorStatus"] = ablnIsSetIgnorStatus;
    var data = nsRequest.SyncPost("btnIgnoreSelectedRows_Click", Parameters, "POST");

    if (data && data != null && data != "") {
        sessionStorage.setItem("DisplayDetailLookupMessage", data);
        ldomDiv.find("#txtDetailComment").val("");
        ldomDiv.find("#btnSearch").trigger("click");
        return false;
    }
    return false;
}
var FM_DispalyMessage = nsCommon.DispalyMessage;
nsCommon.DispalyMessage = function (astrMessage, ActiveDivID) {
    // FW Upgrade - PIR ID - 3589 - Change display message when clicked on Apply Filter button
    if (ActiveDivID.indexOf("wfmReassignWorkMaintenance") == 0 || ActiveDivID.indexOf("wfmMyBasketMaintenance") == 0) {
        var lFormModel = ns.viewModel[ns.viewModel.currentModel];
        var lstrApplyFilterMessage = lFormModel.OtherData["istrApplyFilterMessage"];
        var lstrApplyFilterButtonId = lFormModel.OtherData["istrApplyFilterButtonId"];
        astrMessage = lstrApplyFilterMessage;
    }
    if (ActiveDivID.indexOf("wfmPirMaintenance") == 0) {
        if (sessionStorage.getItem("DeleteMessageStr") && astrMessage.indexOf(" [ All changes successfully cancelled. ]") == 0) {
            astrMessage = sessionStorage.getItem("DeleteMessageStr");
            sessionStorage.removeItem("DeleteMessageStr");
        }
        if (astrMessage.indexOf("Successfully uploaded ") == 0) {
            sessionStorage.setItem("UploadMessageStr", astrMessage);
        }
        if (sessionStorage.getItem("UploadMessageStr") && astrMessage.indexOf(" [ All changes successfully cancelled. ]") == 0) {
            astrMessage = sessionStorage.getItem("UploadMessageStr");
            sessionStorage.removeItem("UploadMessageStr");
        }        
    }
    if (ActiveDivID.indexOf("wfmEmployerPayrollDetailLookup") == 0) {
        if (sessionStorage.getItem("DisplayDetailLookupMessage") && astrMessage.indexOf(" Records met the search criteria.") >= 0) {
            astrMessage = sessionStorage.getItem("DisplayDetailLookupMessage");
            sessionStorage.removeItem("DisplayDetailLookupMessage");
        }
    }

    if (ActiveDivID.indexOf("wfmInUploadFileMaintenance") == 0 && astrMessage.indexOf(" [ All changes successfully cancelled. ]") == 0) {
          
        astrMessage = "[ Record displayed ]";
    }
    if (ActiveDivID.indexOf("wfmDeathNotificationMaintenance") == 0 && astrMessage.indexOf(" [ All changes successfully cancelled. ]") == 0) {

        astrMessage = "[ All changes successfully saved. ]";
    }

    FM_DispalyMessage(astrMessage, ActiveDivID);
}

var base_ns_PositionCursor = ns.PositionCursor;
ns.PositionCursor = function (astrFormID, adomDiv) {
    var lblnRestoredScrollPostion = ns.iblnRestoredScrollPostion;
    ns.iblnRestoredScrollPostion = false;
    var lblnCanPositionCursor = true;
    var ldomDiv = adomDiv;
    if (adomDiv == undefined) {
        ldomDiv = $(astrFormID);
    }
    var fn = nsUserFunctions["CanPositionCursor"];
    if (typeof fn === 'function') {
        lblnCanPositionCursor = fn(astrFormID, ldomDiv);
    }
    if (!lblnCanPositionCursor) {
        ns.blnLoading = true;
        var ldomControl, ldomControls = ldomDiv[0].querySelectorAll("input[type='text'][sfwExtendSSN],input[type='text'][sfwExtendSIN],input[type='text'][sfwExtendLast4SIN],input[type='text'][sfwExtendCurrency],input[type='text'][sfwExtendPercentage],input[type='text'][sfwExtendCustom],input[type='text'][sfwExtendPhone]");
        for (var i = 0, iLen = ldomControls.length; i < iLen; i++) {
            if (ldomControls[i] != document.activeElement) {
                ldomControl = $(ldomControls[i]);
                var lstrControlValue = String(ldomControl.val());
                if (lstrControlValue != undefined && lstrControlValue != null && lstrControlValue.trim() !== "" && nsCommon.jQClosest(ldomControls[i], function (el) {
                    return el.tagName === "DIV" && el.classList.contains("ListViewItems");
                }) == null && nsCommon.jQClosest(ldomControls[i], function (el) {
                    return el.classList.contains(nsConstants.GRID_DATA_ROW_CLASS) && el.getAttribute('rowIndex') != undefined;
                }) == null) {
                    ldomControl.trigger("blur");
                }
            }
        }
        ns.blnLoading = false;
        return;
    }
    ns.blnLoading = true;
    var ldomControl, ldomControls = ldomDiv[0].querySelectorAll("input[type='text'][sfwExtendSSN],input[type='text'][sfwExtendSIN],input[type='text'][sfwExtendLast4SIN],input[type='text'][sfwExtendCurrency],input[type='text'][sfwExtendPercentage],input[type='text'][sfwExtendCustom],input[type='text'][sfwExtendPhone]");
    for (var i = 0, iLen = ldomControls.length; i < iLen; i++) {
        ldomControl = $(ldomControls[i]);
        var lstrControlValue = String(ldomControl.val());
        if (lstrControlValue != undefined && lstrControlValue != null && lstrControlValue.trim() !== "" && nsCommon.jQClosest(ldomControls[i], function (el) {
            return el.tagName === "DIV" && el.classList.contains("ListViewItems");
        }) == null && nsCommon.jQClosest(ldomControls[i], function (el) {
            return el.classList.contains(nsConstants.GRID_DATA_ROW_CLASS) && el.getAttribute('rowIndex') != undefined;
        }) == null) {
            ldomControl.trigger("blur");
        }
    }
    if (ldomDiv[0].getAttribute("ViewOnlyForm") === "true") {
        $(nsConstants.SCROLL_DIV).scrollTop(0);
        ns.blnLoading = false;
        return;
    }
    var oFirstControl = ldomDiv.find(":not([gridid]):not([listviewid]):not([data-sfwcontroltype='dropdown']):not(.filter):not(input.check_row):not(input.s-grid-check-all):not(input.ellipse-input-pageHolder):not(input.s-grid-common-filterbox):input[type !='button']:input[type !='submit']:input[type !='image']:input[sfwretrieval !='True']:input[sfwretrieval !='true']:input[type='text']:visible:enabled:first");
    //if (oFirstControl.length > 0) {
    //    oFirstControl.trigger("focus").trigger("blur");
    //    if (!(ns.iblnIsMobileMedia || ns.iblnIsDiabledByMobileMedia))
    //        oFirstControl.trigger("focus");
    //    if (!lblnRestoredScrollPostion) {
    //        var lintScrollPosition = $(nsConstants.SCROLL_DIV).scrollTop();
    //        var lintScrollToMinus = nsConstants.SCROLLTOP_MINUS_FOCUS_CONTROL || 12;
    //        if (ns.iblnFreezeBreadCrumToolBar === true || ns.iblnFreezeBreadCrum === true || ns.iblnFreezeButtonToolBar === true) {
    //            var ldomCrumToolbarContainer = $("." + nsConstants.FREEZED_CRUM_TOOLBAR_CONTAINER_CLASS);
    //            if (ldomCrumToolbarContainer.length > 0 && ldomCrumToolbarContainer.is(":visible")) {
    //                lintScrollToMinus = parseInt(ldomCrumToolbarContainer.height(), 10);
    //            }
    //        }
    //        var lintScrollTopPosition = (lintScrollPosition - lintScrollToMinus);
    //        if (lintScrollTopPosition < 0) {
    //            lintScrollTopPosition = 0;
    //        }
    //        if (lintScrollTopPosition >= 0 && lintScrollPosition >= lintScrollTopPosition) {
    //            var fnSetTimeout = function () {
    //                $(nsConstants.SCROLL_DIV).scrollTop(lintScrollTopPosition);
    //                $(".page-container").scrollTop(lintScrollTopPosition);
    //            };
    //            setTimeout(fnSetTimeout, 250);
    //        }
    //    }
    //} else
    {
        var fnSetTimeout = function () {
            //$(nsConstants.SCROLL_DIV).scrollTop(0);
            //$(".page-container").scrollTop(0);
        };
        setTimeout(fnSetTimeout, 250);
        $(".page-slideout-container-wrapper").scrollTop(0);
        if ((oFirstControl != undefined) && (oFirstControl.length >= 0)) {
            oFirstControl.trigger("focus");
            oFirstControl.trigger("select");
        }
    }
    ns.blnLoading = false;
}

//F/W Upgrade PIR 15697 - Displaying current datetime
function GetCurrentDateTime(dt) {
    var res = "";
    res += formatdigits(dt.getMonth() + 1);
    res += "/";
    res += formatdigits(dt.getDate());
    res += "/";
    res += formatdigits(dt.getFullYear());
    res += " ";
    res += formatdigits(dt.getHours() > 12 ? dt.getHours() - 12 : dt.getHours());
    res += ":";
    res += formatdigits(dt.getMinutes());
    res += ":";
    res += formatdigits(dt.getSeconds());
    res += " " + dt.getHours() > 11 ? " PM" : " AM";
    return res;
}
function formatdigits(val) {
    val = val.toString();
    return val.length == 1 ? "0" + val : val;
}
var showDivBase = MVVMGlobal.showDiv;
MVVMGlobal.showDiv = function (astrDivID, aobjDataItem, adomDiv, astrFormContainerID) {
    showDivBase(astrDivID, aobjDataItem, adomDiv, astrFormContainerID);
    if ($('#lblCurrTime').length > 0) {
        $('#lblCurrTime').text(GetCurrentDateTime(new Date()));
    }
}
MVVM.JQueryControls.GridView.prototype.beforeInitBase = MVVM.JQueryControls.GridView.prototype.beforeInit;
MVVM.JQueryControls.GridView.prototype.beforeInit = function () {
    this.beforeInitBase();
    if ($('#lblCurrTime').length > 0) {
        $('#lblCurrTime').text(GetCurrentDateTime(new Date()));
    }
    if (ns.viewModel != undefined && ns.viewModel.currentForm != undefined && ns.viewModel.currentForm.Length > 0 && ns.viewModel.currentForm.toLowerCase().indexOf("lookup") >= 0) {
        var ldomDiv = $("#" + ns.viewModel.currentModel);
        if (ldomDiv.length > 0) {
            ns.PositionCursor(ns.viewModel.currentModel, ldomDiv);
        }    
    }
}
var base_btnStoreUserDefaults_Click = nsEvents.btnStoreUserDefaults_Click;
nsEvents.btnStoreUserDefaults_Click = function (e) {
    var ldomDiv = $("#" + ns.viewModel.currentModel);
    if (ldomDiv.length > 0) {
        ns.PositionCursor(ns.viewModel.currentModel, ldomDiv);
    }
    return base_btnStoreUserDefaults_Click(e);
}
function btnReadyToPostClick() {
    if ($('#lblTotalEmployerInterestCalculated').text() == "$0.00" || $('#chkInterestWaiverFlag').is(':checked')) {
        return confirm('Are you sure you are Ready To Post?');
    }
    else if ($('#lblBenefitTypeValue').text() == "Retirement" && $('#lblTotalEmployerInterestCalculated').text() != "$0.00" && !$('#chkInterestWaiverFlag').is(':checked')) {
        return confirm('Interest has accrued. Are you sure you want to post?');
    }
    return true;
}
var base_OndeleteFormClick = nsEvents.OnDeleteFormClick;
nsEvents.OnDeleteFormClick = function (astrDivId, CanDisplayParentForm) {
    if (ns.viewModel && ns.viewModel.currentModel && (ns.viewModel.currentModel.indexOf("wfmInUploadFileMaintenance") == 0)) {
        delete ns.DirtyData[ns.viewModel.currentModel];
    }
    base_OndeleteFormClick(astrDivId, CanDisplayParentForm);
}
var FM_DispalyError = nsCommon.DispalyError;
nsCommon.DispalyError = function (astrMessage, ActiveDivID, ablnScrollToTop) {
    if (((ns.viewModel.currentForm && ns.viewModel.currentForm.indexOf("wfmReportClientMVVM_RptDiv") >= 0) && (astrMessage != null && astrMessage != undefined && astrMessage.indexOf("Validation error on page. Please fix and try again.") >= 0))) {
        astrMessage = "";
    }
    FM_DispalyError(astrMessage, ActiveDivID, ablnScrollToTop);
    if (!astrMessage) {
        var ldomDiv = $("#" + ns.viewModel.currentModel);
        if (ldomDiv.length > 0) {
            ns.PositionCursor(ns.viewModel.currentModel, ldomDiv);
        }
    }
}
var GenerateCorrespondence = nsCorr.AfterGenerated;
nsCorr.AfterGenerated = function (data) {
    window.clearInterval(intervalId);
    var mygeneDiv = $('#myGeneDisplay');
    if (mygeneDiv.length > 0) {
        mygeneDiv.hide();
    }
    var temp = window.alert;
    window.alert = function () { };
    GenerateCorrespondence(data);
    $("#" + ns.viewModel.currentModel).find("#btnImageCorrespondence").attr('disabled', 'false');
    $("#" + ns.viewModel.currentModel).find("#btnImageCorrespondence").removeAttr("disabled");
    $("#" + ns.viewModel.currentModel).find("#btnExecuteRefreshFromObject").removeClass("hideControl");
    window.alert = temp;
    nsCommon.DispalyMessage(data.ResponseMessage.istrMessage, ns.viewModel.currentModel);
    var lstrActiveDivID = data.ActiveForm;
    var btnGenerateCorrespondence = $('#' + lstrActiveDivID).find("#btnGenerateCorrespondence");
    $("#" + ns.viewModel.currentModel).find('#lblEssError').addClass('hideControl');
    $("#" + ns.viewModel.currentModel).find('#lblMssError').addClass('hideControl');
}

/*  
    F/W Upgrade PIR 15996 - wfmServicePurchaseDetailUserraMaintenance Month year field throwing invalid date error after refresh button click.
    Added condition lstrControlVal != "__/____"
 */
MVVMGlobal.Extend_MonthYear = function(DivToApplyUI, astrActiveDivID) {
    var ldomControls = DivToApplyUI[0].querySelectorAll("input[type='text'][sfwExtendMonthYear]:not([gridid]):not([isneogrid='true'])");
    for (var i = 0, iLen = ldomControls.length; i < iLen; i++) {
        var lControl = $(ldomControls[i]);
        var lblnAllowPartialMask = !(lControl[0].hasAttribute("AllowPartialMask") && lControl[0].getAttribute("AllowPartialMask").toUpperCase() === "TRUE");
        lControl.mask('99/9999', { "ActiveDivId": astrActiveDivID, firstFocus: true, firstBlur: true, autoclear: lblnAllowPartialMask }).trigger("focus").trigger("blur");
        lControl.off(".neoValidDate").on("blur" + ".validDate", function (e) {
            var lControl = $(this);
            var lstrControlVal = lControl.val();
            if (lstrControlVal != nsConstants.BLANK_STRING && lstrControlVal != "__/____") {
                var ldtParseDateValue = Sagitec.nsFormatting.DateTimeFormatter.ParseDateTime(String(lstrControlVal), lControl[0].getAttribute("sfwdataformat"));
                if (ldtParseDateValue == null || (ldtParseDateValue.getDate == null || isNaN(ldtParseDateValue.getDate()))) {
                    var lblnShowAlertOnInvalidDate = MVVMGlobal.GetControlAttribute(lControl, "sfwShowAlertOnInvalidDate");
                    var lblnAlert = (lblnShowAlertOnInvalidDate == null && ns.iblnShowAlertForInvalidDate) || (lblnShowAlertOnInvalidDate != null && lblnShowAlertOnInvalidDate.toString().toUpperCase() == "TRUE");
                    MVVMGlobal.currentHtmlElement = lControl;
                    nsCommon.Alert(DefaultMessages.InvalidDate, lControl, lblnAlert);
                    var lblnClearValueOnInvalidDate = MVVMGlobal.GetControlAttribute(lControl, "sfwClearValueOnInvalidDate");
                    var lblnAllowPartialMask = !(lControl[0].hasAttribute("AllowPartialMask") && lControl[0].getAttribute("AllowPartialMask").toUpperCase() === "TRUE");
                    var lblnClearValue = ((lblnClearValueOnInvalidDate == null && ns.iblnClearValueForInvalidDate) || (lblnClearValueOnInvalidDate != null && lblnClearValueOnInvalidDate.toString().toUpperCase() == "TRUE")) && lblnAllowPartialMask;
                    if (lblnClearValue) {
                        lControl.val(nsConstants.BLANK_STRING);
                    }
                }
            }
            if (ns.iblnErrorOnFocusOut === true) {
                if (((lControl.hasClass(nsConstants.CLASS_EXECUTE_CONSTRAINTS_ON_FOCUSOUT) && lControl[0].getAttribute(nsConstants.ATTR_EXECUTE_CONSTRAINTS_ON_FOCUSOUT) === "true") || (ns.iblnErrorOnFocusOut == true && lControl[0].getAttribute(nsConstants.CONFIGURED_VALIDATOR) === "true")) && lControl[0].getAttribute("sfwretrieval") == undefined) {
                    MVVMGlobal.OnConstraintChange(lControl, e);
                }
            }
        });
    }
}
function StartShowGenerating() {
    if ($('#myGeneDisplay').length == 0) {
        $("<div id='myGeneDisplay'></div>").insertBefore('#pnlNewPanel');
    }
    else {
        var mydiv = $('#myGeneDisplay');
        var output;
        output = 'Generating';
        dots++;
        if (dots >= dotmax)
            dots = 1;
        for (var x = 0; x < dots; x++) {
            output += '.';
        }
        mydiv.text(output);
        if (!mydiv.is(':visible')) {
            mydiv.show();
        }
    }
}
function switchVisible() {
    var control = document.getElementById("SlideOutSecurityTree");
    if (control != undefined) {
        var innerControl = document.getElementById("TabsSecurityTree");
        if (innerControl != undefined) {
            innerControl.innerHTML = "";
            var like = "SecurityMessageDiv-" + ns.viewModel.currentForm;
            var ldomControls = document.querySelectorAll(["div[id^='", like, "']"].join(""));
            if (ldomControls != undefined && ldomControls[0] != undefined) {
                var splitString = ldomControls[0].innerHTML.split(';');
                for (var i = 0; i < splitString.length; i++) {
                    var childdiv = document.createElement('div');
                    childdiv.innerHTML = "*" + splitString[i];
                    innerControl.appendChild(childdiv);
                    var br = document.createElement("br");
                    innerControl.appendChild(br);
                }

            }
        }
    }
}
//FW Upgrade - Print functionality global issue , Solution side changes provided by FW team 
function btnPrintPage_Click(e) {
    $([nsConstants.HASH, nsConstants.CENTER_SPLITTER].join("")).find("div[id^='wfm']:not([id$='" + nsConstants.ERROR_DIV + "']):visible").find("div:not([id$='" + nsConstants.ERROR_DIV + "']):first").trigger('mouseup');
    if (ns.viewModel != undefined && ns.viewModel.currentModel != undefined
        && (ns.viewModel.currentModel != "" && $("#" + ns.viewModel.currentModel).length == 0) && (ns.viewModel.currentForm != "" && $("#" + ns.viewModel.currentForm).length == 0)) {
        ns.viewModel.currentModel = ns.viewModel.currentForm;
    }
    if (ns.viewModel != undefined && ns.viewModel.currentModel != undefined
        && (ns.viewModel.currentModel != "" && $("#" + ns.viewModel.currentModel).length > 0)) {
        if (parseInt($("#MainSplitter").css("opacity")) == 0) {
        }
        ns.iblnPrint = true;
        ns.istrPrintPage = ns.viewModel.currentModel;
        nsEvents.sfwActionPrintPage.PrintPage(e, ns.viewModel.currentModel);
    }
    return false;
}
nsEvents.btnPrintPage_Click = btnPrintPage_Click;
var sfwActionPrintPage = (function () {
    function sfwActionPrintPage() {
    }
    sfwActionPrintPage.LoadCssFiles = function (aarrAllStyles, afnCallBack, astrSelector) {
        if (astrSelector === void 0) { astrSelector = ":not([linkusercsstheme='true'])"; }
        if (aarrAllStyles == undefined) {
            aarrAllStyles = {};
        }
        var llstLinks = $("link[rel=stylesheet][href]" + astrSelector);
        var llintTotalLength = llstLinks.length;
        if ((Object.keys(aarrAllStyles).length == 0 || astrSelector === "[linkusercsstheme='true']") && llintTotalLength > 0) {
            var lintCount = 0;
            llstLinks.each(function () {
                var href = $(this).attr("href");
                if (href) {
                    if (aarrAllStyles[href] == undefined) {
                        $.get(href).done(function (astrData) {
                            lintCount++;
                            if (astrData != null) {
                                var lstrCss = nsCommon.ReplaceAll(astrData, "../image", "/image");
                                lstrCss = nsCommon.ReplaceAll(lstrCss, "../StaticResources", "/StaticResources");
                                aarrAllStyles[href] = lstrCss;
                            }
                            if (llintTotalLength == lintCount && afnCallBack) {
                                afnCallBack();
                            }
                        }).fail(function () {
                            lintCount++;
                            if (llintTotalLength == lintCount && afnCallBack) {
                                afnCallBack();
                            }
                        });
                    }
                    else {
                        lintCount++;
                        if (llintTotalLength == lintCount && afnCallBack) {
                            afnCallBack();
                        }
                    }
                }
            });
        }
        else if (afnCallBack) {
            afnCallBack();
        }
    };
    sfwActionPrintPage.PrintPage = function (e, astrActiveDivID) {
        ns.iintPrintMaxWidth = ns.iintPrintMaxWidth || 1248;
        if (astrActiveDivID == undefined || astrActiveDivID == ""
            || astrActiveDivID.startsWith("body")
            || ($("#" + astrActiveDivID).length > 0 && $("#" + astrActiveDivID)[0].tagName === "BODY")) {
            astrActiveDivID = ns.viewModel.currentModel;
        }
        var ldomCurrentModel = $([nsConstants.HASH, astrActiveDivID].join(""));
        if (ldomCurrentModel != undefined && ldomCurrentModel.length > 0) {
            var lobjDataItem = nsCommon.GetDataItemFromDivID(astrActiveDivID);
            var lstrTitle_1 = (lobjDataItem != null) ? lobjDataItem.title || document.title : document.title;
            ns.iblnPrint = true;
            ns.istrPrintPage = astrActiveDivID;
            $("#pnlLoading").css("display", "block");
            ns.displayActivity(true);
            var ldomDivToPrint = ldomCurrentModel.clone(true);
            ldomDivToPrint = $("<span/>").append(ldomDivToPrint);
            var fn = nsUserFunctions[nsConstants.USER_FUNCTION_BEFORE_PRINT_PAGE];
            if (typeof fn === 'function') {
                var Context = {
                    activeDivID: astrActiveDivID,
                    DivToPrint: ldomDivToPrint
                };
                var e = {};
                e.context = Context;
                fn(e);
            }
            ldomCurrentModel.attr(nsConstants.ATTR_ID, astrActiveDivID + "_jQueryPrint");
            nsEvents.sfwActionPrintPage.PrepareControlsToPrint(ldomDivToPrint, ldomCurrentModel);
            var ldomGrids = ldomCurrentModel.find(".s-gridparent:visible");
            ldomCurrentModel.hide();
            var lblnIsIEBrowser = false;
            lblnIsIEBrowser = nsCommon.detectIE();
            var lbnIE = lblnIsIEBrowser;
            var ldomGrp = ldomCurrentModel.closest('[role="group"]');
            if (ldomGrp != undefined)
                ldomGrp.prepend(ldomDivToPrint);
            ldomDivToPrint.show();
            if (ns.iblnPrintAllPagesOnLookup === true && astrActiveDivID.indexOf(nsConstants.LOOKUP) > 0) {
                ldomGrids.each(function (aintIndex, adomElement) {
                    var ldomGrid = $(adomElement);
                    var lstrActiveDivId = nsCommon.GetActiveDivId(ldomGrid);
                    var lobjGridWidget = nsCommon.GetWidgetByActiveDivIdAndControlId(lstrActiveDivId.replace("_jQueryPrint", ""), adomElement.id);
                    if (lobjGridWidget != undefined && lobjGridWidget.jsObject != undefined) {
                        var lobjGrid = ldomGrid.data("neoGrid");
                        var options = _.cloneDeep(lobjGrid.options);
                        var lobjGridStoredInfo = lobjGridWidget.getStoredObject();
                        options.pageable = false;
                        options.RestorableObject = lobjGridStoredInfo;
                        options.iblnPrintPage = true;
                        var ldomGridElement = ldomDivToPrint.find("#" + ldomGrid[0].id);
                        ldomGridElement.find('.s-pager').hide();
                        if (ns.iblnKeepToolBarForPrintForIEnFF !== true && ((lbnIE != undefined && lbnIE != false) || navigator.userAgent.search("Firefox") > -1) && options.iblnShowToolBar === true) {
                            ldomDivToPrint.find(".s-grid-toolbar-button-hide").removeClass("s-grid-toolbar-button-hide").removeAttr("toolbar-grid");
                            options.iblnShowToolBar = false;
                            options.iobjToolBarPanel = null;
                            ldomGridElement.empty();
                        }
                        if (lobjGrid.iblnTable && ldomGridElement.closest(".s-grid-helper").length > 0) {
                            var ldomHelper = ldomGridElement.closest(".s-grid-helper");
                            ldomGridElement.insertAfter(ldomHelper);
                            ldomGridElement.html(lobjGrid.istrTableInnerHTML);
                            ldomHelper.remove();
                        }
                        ldomGridElement.neoGrid(options);
                        lobjGridWidget.jsObject.options.iblnPrintPage = false;
                    }
                });
            }
            var Prefix = MVVMGlobal.GetPrefixforAjaxCall();
            if (Prefix == "///") {
                Prefix = "/";
            }
            nsEvents.sfwActionPrintPage.MarkControlsAsChecked(ldomDivToPrint);
            var lstrParentStyle = "";
            var ldomScrollDiv = $(nsConstants.SCROLL_DIV);
            if (ldomScrollDiv.length > 0 && ldomScrollDiv[0].getAttribute("style") != undefined && ldomScrollDiv[0].getAttribute("style") != "") {
                var DivStyle = ldomScrollDiv[0].getAttribute("style");
                lstrParentStyle = ["style='", DivStyle, "' "].join("");
            }
            if (ldomScrollDiv.length > 0 && $(nsConstants.SCROLL_DIV).attr("class") != undefined && ldomScrollDiv[0].getAttribute("class") != "") {
                lstrParentStyle = [lstrParentStyle, " class='", ldomScrollDiv[0].getAttribute("class"), "'"].join("");
            }
            var ldomPrintPageWithScroll = $("<div id='divPrintPageWithScroll' " + lstrParentStyle + "></div>");
            ldomScrollDiv.hide();
            if (!lblnIsIEBrowser) {

                ldomPrintPageWithScroll.insertAfter(ldomScrollDiv);
                ldomDivToPrint.appendTo(ldomPrintPageWithScroll);
                ldomDivToPrint.css({ "page-break-after": "always", "width": "100%", "overflow": "auto" });

            }
            else {
                ldomPrintPageWithScroll = ldomDivToPrint;
                ldomDivToPrint.css({ "page-break-after": "always", "width": "100%", "height": "100%", "overflow": "auto" });
            }
            ldomPrintPageWithScroll.show();
            var ldomPrintDiv = lblnIsIEBrowser ? ldomDivToPrint : $("#divPrintPageWithScroll");
            if (ldomPrintDiv.width() > ns.iintPrintMaxWidth) {
                ldomPrintDiv.width(ns.iintPrintMaxWidth);
            }
            ldomDivToPrint.width(ldomPrintDiv.width());
            var lintWidth = ldomDivToPrint.width();
            ldomPrintDiv.find(nsConstants.TAB_CONTAINER_SELECTOR).width(lintWidth);
            ldomPrintDiv.find(nsConstants.TAB_CONTAINER_SELECTOR).parent().width(lintWidth);
            ldomPrintDiv.find("ul.s-ulControlTabs").css("width", "100%");
            ldomPrintDiv.find(nsConstants.PANEL_CONTROL_TYPE_SELECTOR).width(lintWidth);
            ldomPrintDiv.find("li.s-liControlTabSheet").css("float", "left");
            ldomPrintDiv.find("li.s-liControlTabSheet").find("a").css("float", "left");
            var llstGrids = ldomPrintDiv.find(".s-gridparent");
            if (ns.iblnKeepToolBarForPrintForIEnFF !== true && ((lbnIE != undefined && lbnIE != false) || navigator.userAgent.search("Firefox") > -1)) {
                ldomDivToPrint.find(".s-grid-toolbar-button-hide").removeClass("s-grid-toolbar-button-hide").removeAttr("toolbar-grid");
                ldomDivToPrint.find(".s-grid-toolbar-button-container").remove();
            }
            llstGrids.each(function () {
                var lobjGrid = $(this);
                lobjGrid.parent().css("width", "100%");
                lobjGrid.css("width", "99%");
                var lintGrindWidth = lobjGrid.width();
                if (lintGrindWidth > lintWidth) {
                    lobjGrid.css("width", lintWidth + "px");
                    lobjGrid.find("td, th").css({ "word-wrap": "break-word", "max-width": "" + (ns.iintGridColumnPrintMaxWidth || 120) + "px", "white-space": "pre-wrap", "-ms-word-break": "break-word", "word-break": "break-word" }).addClass("s-PrintCellWrap");
                }
                else {
                    lobjGrid.find("td, th").css({ "word-wrap": "break-word", "white-space": "pre-wrap", "-ms-word-break": "break-word", "word-break": "break-word" }).addClass("s-PrintCellWrap");
                }
            });
            ldomPrintDiv.css("zoom", "80%");
            var beforePrint_1 = function () {
                nsEvents.sfwActionPrintPage.MarkControlsAsChecked(ldomPrintPageWithScroll);
            };
            var fnAfterPrint_1 = function () {
                $("#divPrintPageWithScroll").remove();
                ldomPrintPageWithScroll = null;
                $(nsConstants.SCROLL_DIV).show();
                ldomDivToPrint.hide();
                ldomDivToPrint.remove();
                nsEvents.sfwActionPrintPage.MarkControlsAsChecked(ldomCurrentModel);
                ldomCurrentModel.attr(nsConstants.ATTR_ID, astrActiveDivID).show();
                ldomDivToPrint = null;
                ns.displayActivity(false);
                $("#pnlLoading").css("display", "none");
            };
            ldomPrintPageWithScroll = lblnIsIEBrowser ? ldomDivToPrint : $("#divPrintPageWithScroll");
            var fnCallThemePrint = function () {
                var ldomThemeLink = $("link[rel=stylesheet][href][linkusercsstheme='true']");
                if (ldomThemeLink.length > 0 && !sfwActionPrintPage.iarrThemeStyles[ldomThemeLink[0].getAttribute("href")]) {
                    var fnCallPrint = function () {
                        nsEvents.sfwActionPrintPage.PrintCurrentPage(ldomPrintPageWithScroll, lstrTitle_1, beforePrint_1, fnAfterPrint_1, ns.iintPrintDelay);
                    };
                    sfwActionPrintPage.LoadCssFiles(sfwActionPrintPage.iarrThemeStyles, fnCallPrint, "[linkusercsstheme='true']");
                }
                else {
                    nsEvents.sfwActionPrintPage.PrintCurrentPage(ldomPrintPageWithScroll, lstrTitle_1, beforePrint_1, fnAfterPrint_1, ns.iintPrintDelay);
                }
            };
            sfwActionPrintPage.LoadCssFiles(sfwActionPrintPage.iarrAllStyles, fnCallThemePrint);
        }
        return false;
    };
    sfwActionPrintPage.PrepareControlsToPrint = function (adomDivToPrint, adomOriginalDiv) {
        adomDivToPrint.find("[data-bind]").each(function (aintIndex, adomElement) {
            var ldomControl = $(this);
            var lstrValue;
            var lstrID = ldomControl[0].getAttribute(nsConstants.ATTR_ID);
            if (lstrID != undefined && lstrID != "") {
                if (adomOriginalDiv.find("#" + lstrID).length > 0) {
                    if (ldomControl[0].getAttribute(nsConstants.TYPE) === "text") {
                        var lstrValue = adomOriginalDiv.find("#" + lstrID)[0].value;
                        if (lstrValue != undefined && lstrValue != "") {
                            ldomControl[0].setAttribute("value", lstrValue);
                        }
                    }
                    else if (ldomControl[0].tagName === "SELECT") {
                        lstrValue = adomOriginalDiv.find("#" + lstrID)[0].value;
                        if (lstrValue != undefined && lstrValue != "" && ldomControl.find("option[value='" + lstrValue + "']").length > 0) {
                            ldomControl.find("option[value='" + lstrValue + "']").attr("selected", "selected");
                        }
                    }
                    else if (ldomControl[0].getAttribute(nsConstants.TYPE) === "radio" && ldomControl.closest("[islistcontrol]").length > 0) {
                        lstrValue = adomOriginalDiv.find("#" + ldomControl.closest("[islistcontrol]").attr(nsConstants.ATTR_ID)).find("input:checked");
                        if (lstrValue != undefined && lstrValue.length > 0) {
                            var lblnChecked = lstrValue.is(":checked");
                            if (neo.IsChrome && (lblnChecked === true || lblnChecked === "on"))
                                lblnChecked = "on";
                            else if (neo.IsChrome && (lblnChecked === false || lblnChecked === "off"))
                                lblnChecked = "off";
                            ldomControl.closest("[islistcontrol]").find("#" + lstrValue[0].id).attr("checked", lblnChecked);
                            ldomControl.closest("[islistcontrol]").find("#" + lstrValue[0].id)[0].checked = ((lblnChecked === "on") ? true : (lblnChecked === "off" ? false : lblnChecked));
                        }
                    }
                    else if (ldomControl[0].getAttribute(nsConstants.TYPE) === "radio" && ldomControl.closest("[islistcontrol]").length <= 0) {
                        lstrValue = adomOriginalDiv.find("#" + lstrID)[0].checked;
                        if (lstrValue === true) {
                            ldomControl[0].setAttribute("checked", lstrValue);
                            ldomControl[0].checked = lstrValue;
                        }
                    }
                    else if (ldomControl[0].getAttribute(nsConstants.TYPE) === "checkbox" && ldomControl.closest("[islistcontrol]").length > 0) {
                        lstrValue = adomOriginalDiv.find("#" + ldomControl.closest("[islistcontrol]").attr(nsConstants.ATTR_ID)).find("input:checked");
                        if (lstrValue != undefined && lstrValue.length > 0) {
                            lstrValue.each(function () {
                                ldomControl.closest("[islistcontrol]").find("#" + $(this).attr(nsConstants.ATTR_ID)).attr("checked", this.checked);
                                ldomControl.closest("[islistcontrol]").find("#" + $(this).attr(nsConstants.ATTR_ID))[0].checked = this.checked;
                            });
                        }
                    }
                    else if (ldomControl[0].getAttribute(nsConstants.TYPE) === "checkbox" && ldomControl.closest("islistcontrol").length <= 0) {
                        lstrValue = adomOriginalDiv.find("#" + lstrID)[0].checked;
                        if (lstrValue === true) {
                            ldomControl[0].setAttribute("checked", lstrValue);
                            ldomControl[0].checked = lstrValue;
                        }
                    }
                    else if (ldomControl[0].tagName === "TEXTAREA") {
                        lstrValue = adomOriginalDiv.find("#" + lstrID)[0].value;
                        if (lstrValue != undefined && lstrValue != "") {
                            ldomControl.text(lstrValue);
                        }
                    }
                    else {
                        var lstrValue = adomOriginalDiv.find("#" + lstrID)[0].value;
                        if (lstrValue != undefined && lstrValue != "") {
                            ldomControl[0].setAttribute("value", lstrValue);
                        }
                    }
                }
            }
        });
        adomDivToPrint.find("input[type='text'],input[type='radio'],input[type='checkbox'],select")
            .each(function (aintIndex, adomElement) {
                var $field = $(this);
                var lstrValue;
                var lstrID = $field[0].getAttribute(nsConstants.ATTR_ID);
                var ldomControl = adomOriginalDiv.find("#" + lstrID);
                var lblnChecked;
                if (lstrID != undefined && lstrID != "" && ldomControl.length > 0) {
                    if ($field.is("[type='radio']")) {
                        lblnChecked = ldomControl.is(":checked");
                        if (neo.IsChrome) {
                            if (lblnChecked === true || lblnChecked === "on") {
                                lblnChecked = "on";
                            }
                            else if (lblnChecked === false || lblnChecked === "off") {
                                lblnChecked = "off";
                            }
                        }
                        $field.attr("checked", lblnChecked);
                        $field[0].checked = ((lblnChecked === "on") ? true : (lblnChecked === "off" ? false : lblnChecked));
                    }
                    else if ($field.is("[type='checkbox']")) {
                        lblnChecked = ldomControl.is(":checked");
                        $field.attr("checked", lblnChecked);
                        $field.prop("checked", lblnChecked);
                        $field[0].checked = ((lblnChecked === "on") ? true : (lblnChecked === "off" ? false : lblnChecked));
                    }
                    else if ($field[0].tagName === "SELECT") {
                        lstrValue = ldomControl[0].value;
                        if (lstrValue != undefined && lstrValue != "" && $field.find("option[value='" + lstrValue + "']").length > 0) {
                            $field.find("option[value='" + lstrValue + "']").attr("selected", "selected");
                        }
                    }
                    else {
                        $field.attr("value", ldomControl.val());
                    }
                }
            });
    };
    sfwActionPrintPage.MarkControlsAsChecked = function (adomDiv) {
        var lstrSelector = "input[type='radio'][checked='checked'],input[type='radio'][checked='true'],input[type='radio'][checked='on'],input[type='checkbox'][checked='checked'],input[type='checkbox'][checked='true'],input[type='checkbox'][checked='on']";
        adomDiv.find(lstrSelector)
            .each(function () {
                var $field = $(this);
                $field[0].checked = true;
            });
    };
    sfwActionPrintPage.PrintCurrentPage = function (adomElementToPrint, astrTiTle, beforePrintFunction, afterPrintFunction, aintPrintDelay) {
        if (aintPrintDelay === void 0) { aintPrintDelay = 333; }
        if (!astrTiTle || astrTiTle == "") {
            astrTiTle = document.title;
        }
        var lobjThemeCss = {};
        var ldomThemeLink = $("link[rel=stylesheet][href][linkusercsstheme='true']");
        if ((ldomThemeLink.length > 0)) {
            var lsrHref = ldomThemeLink[0].getAttribute("href");
            if (lsrHref && sfwActionPrintPage.iarrThemeStyles[lsrHref]) {
                lobjThemeCss[lsrHref] = sfwActionPrintPage.iarrThemeStyles[lsrHref];
            }
        }
        adomElementToPrint.printThis({
            debug: false,
            importCSS: false,
            importStyle: true,
            printContainer: true,
            loadCSS: "",
            pageTitle: astrTiTle || "",
            removeInline: false,
            removeInlineSelector: "*",
            printDelay: aintPrintDelay || 333,
            header: null,
            footer: null,
            base: false,
            formValues: true,
            canvas: true,
            doctypeString: '<!DOCTYPE html>',
            removeScripts: true,
            copyTagClasses: true,
            beforePrintEvent: null,
            beforePrint: beforePrintFunction,
            afterPrint: afterPrintFunction,
            iobjCssLinks: sfwActionPrintPage.iarrAllStyles,
            iobjCssThemeLinks: lobjThemeCss,
            iintBodyWidth: ns.iintPrintMaxWidth || 1248
        });
    };
    sfwActionPrintPage.iarrAllStyles = {};
    sfwActionPrintPage.iarrThemeStyles = {};
    return sfwActionPrintPage;

}());
nsEvents.sfwActionPrintPage = sfwActionPrintPage;
function LaunchFileNetImageURLs(aProcessInstanceId)
{
    var Parameters = {};
    Parameters["aintProcessInstanceID"] = aProcessInstanceId;
    var lvarFileNetUrls = {};
    lvarFileNetUrls = nsRequest.SyncPost("GetFileNameImageURLs", Parameters, "POST");
    var lstrFeatures = "width=1000px,height=800px,center=yes,help=no, resizable=yes, status=no, top=25, scrollbars=yes,toolbar=yes,location=yes,directories=yes,status=yes,menubar=yes";
    var lstrScript = "";
    var lstrFinalDisplayURL = "";
    if (lvarFileNetUrls && lvarFileNetUrls != undefined && lvarFileNetUrls != null && lvarFileNetUrls.length > 0) {
        var lblnIsIEBrowser = false;
        lblnIsIEBrowser = /*@cc_on!@*/false || !!document.documentMode;
        /*Here we detect browser is IE or Other than IE */
        for (indexURL = 0; indexURL < lvarFileNetUrls.length; indexURL++) {
            if (lblnIsIEBrowser) {
                if (indexURL == 0) {
                    lstrScript = "var newWindow = window.open('', '_blank', '" + lstrFeatures + "');";
                    lstrFirstUrl = lvarFileNetUrls[indexURL];
                }
                else {
                    lstrScript = "var newWindow" + indexURL + " = newWindow.window.open('', '_blank');";
                    lstrScript += "newWindow" + indexURL + ".location.href = '" + lvarFileNetUrls[indexURL] + "';";
                }
                lstrFinalDisplayURL += lstrScript;                
            }
            else {
                var URL = lvarFileNetUrls[indexURL];
                var theTop = 100 + (indexURL * 100);
                var theLeft = 100 + (indexURL * 100);
                window.open(
                    URL, "Window_name" + indexURL,
                    "height=470px,width=850px,modal=yes,alwaysRaised=yes,top="+theTop +",left="+theLeft+",toolbar=1,Location=0,Directories=0,Status=0,menubar=1,Scrollbars=1,Resizable=1");
            }            
        }
        if (lblnIsIEBrowser) {
            lstrFinalDisplayURL += "newWindow.location.href = '" + lstrFirstUrl + "';";
            var OpenurlImages = "function() { " + lstrFinalDisplayURL + " }";
            var OpenUrlImagesFunction = new Function('return ' + OpenurlImages)();
            OpenUrlImagesFunction();
        }
    }
}
ns.SessionStorePageState = function (FormID, controlType, controlID, value, ablnForceStore, ablnTabNav) {
    if (ablnTabNav === void 0) { ablnTabNav = true; }
    if (!ns.CanStoreInSession() && ablnForceStore !== true) {
        return;
    }
    FormID = FormID.replace(nsConstants.HASH, '');
    var scrollPos = $(nsConstants.SCROLL_DIV).scrollTop();
    var lobjInfo = ns.GetSessionStoredInfo(FormID);
    if (lobjInfo === null) {
        lobjInfo = {
            tabs: {},
            grids: {},
            panels: {},
            scrollTop: scrollPos,
            listviews: {}
        };
    }
    lobjInfo.scrollTop = scrollPos;
    if (controlType === "panelnavigator") {
        if (nsCommon.IsTabNavigator(FormID)) {
            lobjInfo["tabstrip"] = { id: controlID };
        }
    }
    else if (controlType === "tab") {
        lobjInfo["tabs"][controlID] = value;
        if (nsCommon.IsTabNavigator(FormID) && ablnTabNav === true) {
            lobjInfo["tabstrip"] = { id: controlID, index: value };
        }
    }
    else if (controlType === "grid") {
        lobjInfo["grids"][controlID] = value;
    }
    else if (controlType === "panel") {
        if (value === true) {
            lobjInfo["panels"][controlID] = value;
            if (nsCommon.IsTabNavigator(FormID) && ablnTabNav === true) {
                lobjInfo["tabstrip"] = { id: controlID };
            }
        }
        else {
            delete lobjInfo["panels"][controlID];
        }
        nsCommon.SetTextForCollapseAllButtonFromTreeView(lobjInfo, FormID);
    }
    else if (controlType === "scroll") {
        lobjInfo.scrollTop = value;
    }
    else if (controlType === "listview") {
        lobjInfo["listviews"][controlID] = value;
    }
    lobjInfo.scrollTop = 0;
    nsCommon.sessionSet(["pageState_", FormID].join(''), lobjInfo);
}  

var base_Reset_Click = nsEvents.btnReset_Click;
nsEvents.btnReset_Click = function (e) {     
    base_Reset_Click(e);   
    setTimeout(function () {
        if ($("#" + ns.viewModel.currentModel).find("#txtAdvBRetirementDate").val() != "")
            $("#" + ns.viewModel.currentModel).find("#txtAdvBRetirementDate").val("").trigger("change");
    }, 3);  

    if (ns.viewModel.currentForm.indexOf("wfmOutOfOfficeLookup") === 0)
    {
        setTimeout(function () {
            $("#" + ns.viewModel.currentModel).find("#txtStartDate").val("");
        }, 3);
    }
}


SessionEvents.InitTimer = function (anumSessionTimeout) {
    var SessionTimeout = anumSessionTimeout;
    if (SessionTimeout == undefined) {
        SessionTimeout = 45;
        SessionEvents.iintSessionTimeout = 45;
    }
    else {
        SessionEvents.iintSessionTimeout = SessionTimeout;
    }
    var total = ((SessionTimeout * 60) - 124) * 1000;
    if ($.idleTimer == undefined)
         return;

     $.idleTimer(total);

    $(document).off("idle.idleTimer");
    $(document).on("idle.idleTimer", function (event, elem, obj) {
        if (ns.iblnFileUploadInProgress === true || ns.blnLoading === true) {
            ns.refreshSession();
            nsUserFunctions.idteLastRequestCompletedTime = new Date();
            if ($('#lblCurrTime').length > 0) {
                $('#lblCurrTime').text(GetCurrentDateTime(new Date()));
            }
        }
        else {
            SessionEvents.TimerReset = false;
            SessionEvents.ShowTimeoutWarning(SessionEvents.iintSessionRemainingTimer, true);
            SessionEvents.countdown(SessionEvents.iintSessionRemainingTimer, ns.logoutSesssion);
        }
    });
}
SessionEvents.ResetTimer = function (aintSessionTimeout) {
    if (SessionEvents.iintSessionTimeout === -1) {
        return;
    }
    if ($(document).data("idleTimerObj") == undefined) {
        SessionEvents.InitTimer(SessionEvents.iintSessionTimeout);
    }
    $(document).idleTimer("reset");
    SessionEvents.TimerReset = true;
    SessionEvents.ShowTimeoutWarning(1, false);
    nsUserFunctions.idteLastRequestCompletedTime = new Date();
}
$(document).on('visibilitychange', function () {
    if (document.visibilityState == 'visible') {
        if (ns.iblnFileUploadInProgress === true || ns.blnLoading === true) {
            ns.refreshSession();
            nsUserFunctions.idteLastRequestCompletedTime = new Date();
            if ($('#lblCurrTime').length > 0) {
                $('#lblCurrTime').text(GetCurrentDateTime(new Date()));
            }
            return;
        }
        var ldteCurrentTime = new Date();
        var TimeDifBetLastRequestAndCurrentTime = ldteCurrentTime.getTime() - nsUserFunctions.idteLastRequestCompletedTime.getTime();        
        var lserverConfiguredSessionTimeOut = ((((SessionEvents.iintSessionTimeout * 60))) * 1000);
        var ldteTimeElapsedSinceLastRequest = ((lserverConfiguredSessionTimeOut - TimeDifBetLastRequestAndCurrentTime) / 1000);
        if ((TimeDifBetLastRequestAndCurrentTime >= lserverConfiguredSessionTimeOut)) {
            ns.logoutSesssion("Session Timed Out");
        }
        else if (ldteTimeElapsedSinceLastRequest > 0) {
            if ($.idleTimer == undefined)
                return;
            ldteTimeElapsedSinceLastRequest = Math.round(ldteTimeElapsedSinceLastRequest);
            var ltmSessionPopupTime = (ldteTimeElapsedSinceLastRequest >= 125) ? ((ldteTimeElapsedSinceLastRequest - 125) * 1000) : ((ldteTimeElapsedSinceLastRequest) * 1000);
            
            $.idleTimer(ltmSessionPopupTime);
            if (ldteTimeElapsedSinceLastRequest <= 125) {
                SessionEvents.TimerReset = false;
                SessionEvents.iintSessionRemainingTimer = ldteTimeElapsedSinceLastRequest <= 120 ? ldteTimeElapsedSinceLastRequest : 120;
                SessionEvents.ShowTimeoutWarning(SessionEvents.iintSessionRemainingTimer, true);
                SessionEvents.countdown(SessionEvents.iintSessionRemainingTimer, ns.logoutSesssion);
            }
            
        }
    }
});

nsCommon.GetNextPreviousButtons = function (astrDivID, aDataItem) {
    var DataItem = ns.CustomTabsTree.getNodeDataByDivID(astrDivID);
    if (DataItem == undefined) {
        DataItem = ns.tabsTreeView.getNodeDataByDivID(astrDivID);
    }
    var treeLink = ns.CustomTabsTree.findByUid(DataItem.uid);
    var classText = "disabled class='formNavigationPrevDisabled'";
    var lblnSkipDisabled = false;
    var PreviousNodeText = nsCommon.GetTreePrevAndNextExceptCenterLeft(treeLink, "prev");
    var PreviousNodeDivID = "";
    if (PreviousNodeText && PreviousNodeText.length > 0) {
        var PreviousNodeSpan = PreviousNodeText.find(".FormNode");
        PreviousNodeDivID = PreviousNodeSpan.attr("sli_linkedto");
        //if (PreviousNodeDivID == undefined)
        //    PreviousNodeDivID = PreviousNodeSpan.attr("sli_linkedto");
        classText = ["class='formNavigation formNavigationPrev' title='", Sagitec.DefaultText.FORM_NAVIGATION_PREVIOUS, "'"].join('');
    }
    else
        PreviousNodeDivID = "";
    if (PreviousNodeDivID != undefined && PreviousNodeDivID.indexOf(nsConstants.LOOKUP) > 0) {
        if (ns.iblnLandingPageAsTreeViewRoot === true) {
            var lobjPrevNode = ns.CustomTabsTree.getNodeDataByDivID(PreviousNodeDivID);
            if (lobjPrevNode != undefined && lobjPrevNode.parentNode() != undefined) {
                lblnSkipDisabled = true;
            }
        }
        if (!lblnSkipDisabled) {
            PreviousNodeDivID = "";
            classText = "disabled class='formNavigationPrevDisabled'";
        }
    }
    lblnSkipDisabled = false;
    var lstrButtonHtml = ["<input accesskey='p' aria-label='Move Previous Page'  type='button'  value='  ' ", classText, " MoveTo='", PreviousNodeDivID, "'>"].join('');
    var pageinfo = MVVMGlobal.GetPageOfInfo(astrDivID, DataItem);
    if (pageinfo != undefined)
        lstrButtonHtml = [lstrButtonHtml, "<span class='pageinfo'>", pageinfo, "</span>"].join('');
    classText = "disabled class='formNavigationNextDisabled'";
    var NextNode = nsCommon.GetTreePrevAndNextExceptCenterLeft(treeLink, "next");
    var NextNodeDivID = "";
    if (NextNode && NextNode.length > 0) {
        var NextNodeSpan = NextNode.find(".FormNode");
        NextNodeDivID = NextNodeSpan.attr("sli_linkedto");
        //if (NextNodeDivID == undefined)
        //    NextNodeDivID = NextNodeSpan.attr("sli_linkedto");
        classText = ["class='formNavigation formNavigationNext' title='", Sagitec.DefaultText.FORM_NAVIGATION_NEXT, "'"].join('');
    }
    else
        NextNodeDivID = "";
    if (NextNodeDivID != "" && NextNodeDivID.indexOf(nsConstants.LOOKUP) > 0) {
        if (ns.iblnLandingPageAsTreeViewRoot === true) {
            var lobjNextNode = ns.CustomTabsTree.getNodeDataByDivID(NextNodeDivID);
            if (lobjNextNode != undefined && lobjNextNode.parentNode() != undefined) {
                lblnSkipDisabled = true;
            }
        }
        if (!lblnSkipDisabled) {
            NextNodeDivID = "";
            classText = "disabled class='formNavigationNextDisabled'";
        }
    }
    lstrButtonHtml = [lstrButtonHtml, "<input accesskey='n' aria-label='Move Next Page'  type='button' value='  ' ", classText, " MoveTo='", NextNodeDivID, "'>"].join('');
    return lstrButtonHtml;
}
nsCommon.AddTreeNodesFromNavigationParamsBase = nsCommon.AddTreeNodesFromNavigationParams;
nsCommon.AddTreeNodesFromNavigationParams = function (ldictParams, ParentNode, astrParentDivID) {
    var customParentNode = ParentNode;
    if (ParentNode != undefined && ParentNode.divID != undefined && ParentNode.divID != "wfmEmployerPayrollHeaderLookup" && ParentNode.divID != "wfmEmployerPayrollDetailLookup"
        && ParentNode.divID != "wfmPaymentHistoryLookup" && ParentNode.divID != "wfmPayeeAccountLookup") {
        customParentNode = ns.CustomTabsTree.getNodeDataByDivID(ParentNode.divID);
        if (customParentNode && customParentNode.children.length > 0) {
            customParentNode.items.forEach(function (element) {
                ns.CustomTabsTree.remove(element);
            });
        }
    }
    nsCommon.AddTreeNodesFromNavigationParamsBase(ldictParams, customParentNode, astrParentDivID);
}
MVVMGlobal.GetPageOfInfo = function (astrActiveDivID, aDataItem) {
    var dataItem = ns.CustomTabsTree.getNodeDataByDivID(astrActiveDivID);
    if (aDataItem == undefined)
        return "";
    var lobjParentNode = aDataItem.parentNode();
    if (lobjParentNode != undefined) {
        var total = lobjParentNode.items.length;
        var number = 0;
        for (var i in lobjParentNode.items) {
            number++;
            if (lobjParentNode.items[i].uid == aDataItem.uid) {
                break;
            }
        }
        return Sagitec.DefaultText.PAGE_DISPLAYING_TEXT_BREADCRUMP.replace("{number}", number.toString()).replace("{total}", total);
    }
}
MVVM.JQueryControls.TreeView.prototype.getNodeDataByDivID = function (astrDivID) {
    var instance = this;
    var lstrDivID = astrDivID.indexOf(instance.istrNodePrefix) == 0 ? astrDivID : [instance.istrNodePrefix, astrDivID].join('');

    var dataItemNode = this.jsObject.get_node(lstrDivID);
    if (dataItemNode == undefined || dataItemNode == false) {
        return undefined;
    }
    var dataItem = dataItemNode.data;
    if (dataItem != undefined && dataItem != false) {
        if (dataItem.data == null) {
            dataItem.children = dataItemNode.children;
            dataItem.parent = dataItemNode.parent;
        }
        if (dataItem.data != null) {
            var larrKeys = Object.keys(dataItem.data);
            var key = "";
            for (var i = 0, iLen = larrKeys.length; i < iLen; i++) {
                key = larrKeys[i];
                if (key != "id" && key != nsConstants.TEXT) {
                    dataItem[key] = dataItem.data[key];
                }
            }
        }
        dataItem.items = [];
        var iLen = dataItem.children.length;
        var myCustomObj = this;
        if (iLen > 0) {
            for (var i = 0; i < iLen; i++) {
                dataItem.items[i] = myCustomObj.getNodeDataByDivID(typeof dataItem.children[i] == "string" ? dataItem.children[i] : dataItem.children[i].id);
            }
        }
        dataItem.parentNode = function () {
            if (dataItem.parent != undefined && dataItem.parent != "" && dataItem.parent != nsConstants.HASH) {
                return ns.CustomTabsTree.getNodeDataByDivID(dataItem.parent);
            }
            return undefined;
        };
        return dataItem;
    }
    return undefined;
};
MVVMGlobal.StoreTreeViewInSessionStore = function (ablnSkipTemplate, ablnSkipTreeView) {
    if (ablnSkipTreeView !== true) {
        nsCommon.sessionSet(["TreeViewDataSource", ns.SiteName].join(''), ns.tabsTreeView.getDataSource());
        nsCommon.sessionSet(["TreeViewCustomDataSource", ns.SiteName].join(''), ns.CustomTabsTree.getDataSource());
    }
    if (ablnSkipTemplate !== true) {
        var newObject = neo.Clone(ns.Templates);
        nsCommon.sessionSet(["Templates", ns.SiteName].join(''), newObject);
    }
}

function RemoveForm(destroyedForms, dataitem) {
    destroyedForms.push(dataitem.title);
    if (dataitem.items.length > 0) {
        for (var idx = dataitem.items.length - 1; idx >= 0; idx--) {
            MVVMGlobal.RemoveForm(destroyedForms, dataitem.items[idx]);
        }
    }
    if (dataitem.parentNode() === undefined) {
        var rootData = ns.CustomTabsTree.iobjDataSource.data();
        if (rootData.length <= 1)
            return destroyedForms;
    }
    ns.destroyAll(dataitem.divID, true);
    ns.RemoveSessionStoredInfo(dataitem.divID);
    var nodeToRemove = ns.CustomTabsTree.findByUid(dataitem.uid);
    try {
        ns.CustomTabsTree.remove(nodeToRemove);
    }
    catch (e) {
        console.log(e.message);
    }

    nodeToRemove = ns.tabsTreeView.findByUid(dataitem.divID);
    try {
        ns.tabsTreeView.remove(nodeToRemove);
    }
    catch (e) {
        console.log(e.message);
    }
    for (var i = 0; i < ns.arrFormsOpened.length; i++) {
        if (ns.arrFormsOpened[i] === dataitem.divID) {
            ns.arrFormsOpened.splice(i, 1);
        }
    }
    MVVMGlobal.StoreTreeViewInSessionStore();
    return destroyedForms;
}
MVVMGlobal.RemoveForm = RemoveForm;

function OnDeleteFormClick(astrDivId, CanDisplayParentForm) {
    if ((ns.CanDisplayParentForm == true || ns.CanDisplayParentForm == undefined) && CanDisplayParentForm == undefined) {
        ns.CanDisplayParentForm = true;
    }
    var dataitem = ns.CustomTabsTree.getNodeDataByDivID(astrDivId);
    if (dataitem != undefined) {
        if (dataitem.divID == nsConstants.BPM_WORKFLOW_CENTERLEFT_MAINTENANCE || ns.iarrCenterLeftForms.indexOf(dataitem.divID) >= 0)
            return;
        var TreeLink = ns.CustomTabsTree.findByUid(dataitem.uid);
        if (TreeLink.length > 0) {
            var ItemToRemove = $(TreeLink).find(".sli_delete-link");
            ItemToRemove.trigger("click");
        }
    }
    ns.CanDisplayParentForm = true;
}
nsEvents.OnDeleteFormClick = OnDeleteFormClick;

nsEvents.OnDeleteNodeClick = function (e) {
    if (window.event != undefined && window.event.preventDefault != undefined) {
        window.event.preventDefault();
    }
    $("#ToolTipDiv").hide();
    var ItemToRemove = $(e).closest(".k-item");
    var dataitem = ns.CustomTabsTree.dataItem(ItemToRemove);
    var formToDisplay = "";
    var lobjAlternateFormInfo = undefined;
    var lobjOnBeforeDeleteNodeClick = undefined;
    var lblnAlterFormCalled = false;
    lobjOnBeforeDeleteNodeClick = nsEvents.OnBeforeDeleteNodeClick(dataitem);
    if (lobjOnBeforeDeleteNodeClick === false)
        return;
    if (dataitem === undefined)
        return;
    if (dataitem != undefined) {
        var rootDataSource = ns.CustomTabsTree.iobjDataSource.data();
        var lintHiddenNodes = 0;
        if (ns.iarrCenterLeftForms.indexOf(dataitem.divID) < 0 && rootDataSource.length > (1 + ns.iarrCenterLeftForms.length)) {
            lintHiddenNodes = _.filter(rootDataSource, { data: { HideNode: true } }).length;
        }
        var lblnDataItemWithMyBasket = false;
        var lobjParentNode = dataitem.parentNode();
        var lblnMyBasket = false;
        var lstrNonCenterleftForm;
        var lblnNonCenterleftForm = false;
        if (ns.iarrCenterLeftForms.indexOf(dataitem.divID) >= 0)
            return;
        else if (dataitem.HideNode !== true && rootDataSource.length == (1 + ns.iarrCenterLeftForms.length + lintHiddenNodes)) {
            if (lobjParentNode != undefined && ns.iarrCenterLeftForms.indexOf(lobjParentNode.divID) >= 0 && lobjParentNode.items != undefined && lobjParentNode.items.length > 0) {
                lblnNonCenterleftForm = true;
                lblnCenterLeftParent = false;
            }
            for (var i = 0; i < rootDataSource.length; i++) {
                if (lblnNonCenterleftForm && dataitem.divID != rootDataSource[i].divID &&
                    ns.iarrCenterLeftForms.indexOf(rootDataSource[i].divID) < 0 && rootDataSource[i].data.HideNode !== true) {
                    lstrNonCenterleftForm = rootDataSource[i].divID;
                }
                if (dataitem.divID == rootDataSource[i].divID) {
                    lblnDataItemWithMyBasket = true;
                }
                else if (ns.iarrCenterLeftForms.indexOf(rootDataSource[i].divID) >= 0 || rootDataSource[i].data.HideNode === true) {
                    lblnMyBasket = true;
                }
            }
            if (lblnDataItemWithMyBasket && lblnMyBasket) {
                lblnAlterFormCalled = true;
                lobjAlternateFormInfo = nsEvents.CheckForAlternateFormToDisplayOnClose(dataitem);
                if (lobjAlternateFormInfo == undefined) {
                    return;
                }
                else {
                    formToDisplay = lobjAlternateFormInfo.formToDisplay || formToDisplay;
                }
            }
        }
        else if (lobjParentNode != undefined && ns.iarrCenterLeftForms.indexOf(lobjParentNode.divID) >= 0 && lobjParentNode.items != undefined && lobjParentNode.items.length > 0) {
            if (lobjParentNode != undefined && ns.iarrCenterLeftForms.indexOf(lobjParentNode.divID) >= 0 && lobjParentNode.items != undefined && lobjParentNode.items.length > 0) {
                lblnNonCenterleftForm = true;
                lblnCenterLeftParent = true;
            }
            if (lblnNonCenterleftForm) {
                for (var i = 0; i < rootDataSource.length; i++) {
                    if (lblnNonCenterleftForm && dataitem.divID != rootDataSource[i].divID &&
                        ns.iarrCenterLeftForms.indexOf(rootDataSource[i].divID) < 0 && rootDataSource[i].data.HideNode !== true) {
                        lstrNonCenterleftForm = rootDataSource[i].divID;
                        break;
                    }
                }
            }
        }
    }
    if (!ns.blnSkipConfirmationForDeleteOrNew) {
        if (!MVVMGlobal.CanBeDeleted(dataitem) && ns.blnIsNewFormSaved === false) {
            var Goahead = confirm(DefaultMessages.ConfirmDeleteFormIfUnsaved);
            if (!Goahead) {
                return;
            }
        }
    }
    if (dataitem != undefined)
        nsCommon.sessionRemove([dataitem.divID, nsConstants.UNDERSCORE_ACTIVITY_INSTANCE_DETAILS].join(''));
    ns.blnFromDeleteTreeNode = true;
    var breakformloop = false;
    var lblnCenterLeftParent = false;
    lobjParentNode = dataitem.parentNode();
    if (lobjParentNode != undefined) {
        if (ns.blnDeleteFromSaveNewClicked) {
            formToDisplay = lobjParentNode.divID;
            ns.blnDeleteFromSaveNewClicked = false;
        }
        else {
            if (lobjParentNode.items.length > 1) {
                var items = lobjParentNode.items;
                for (var i = 0; i < items.length; i++) {
                    if (dataitem.divID !== items[i].divID) {
                        if ((ns.FormOpenedOnRight !== undefined && items[i].divID === ns.FormOpenedOnRight.divID))
                            continue;
                        else {
                            formToDisplay = items[i].divID;
                            if (breakformloop) {
                                break;
                            }
                        }
                    }
                    else {
                        breakformloop = true;
                    }
                }
            }
        }
        if (formToDisplay === "") {
            if (lobjParentNode.divID.toLowerCase().indexOf("centerleft") < 0 && ns.iarrCenterLeftForms.indexOf(lobjParentNode.divID) < 0) {
                formToDisplay = lobjParentNode.divID;
            }
            else {
                if (ns.viewModel.previousDiv == undefined || ns.viewModel.previousDiv == "" || ((dataitem.divID.toLowerCase().indexOf("centerleft") >= 0 || ns.iarrCenterLeftForms.indexOf(dataitem.divID) >= 0))) {
                    ns.viewModel.previousDiv = ns.LandingPage;
                    var lstrRootNodeId = nsCommon.GetProperFormId(ns.viewModel.previousDiv.trim());
                    if (lstrRootNodeId.indexOf(nsConstants.LOOKUP) < 0 && lstrRootNodeId.lastIndexOf("0") != (lstrRootNodeId.length - 1)) {
                        var no = lstrRootNodeId.replace(nsConstants.REGX_NUMBER, '');
                        if (no == undefined || no.trim() === "") {
                            lstrRootNodeId += "0";
                        }
                    }
                    ns.viewModel.previousDiv = lstrRootNodeId;
                }
                var temp = nsCommon.GetDataItemFromDivID(ns.viewModel.previousDiv);
                if (temp != null) {
                    formToDisplay = ns.viewModel.previousDiv;
                    lblnCenterLeftParent = true;
                }
            }
        }
    }
    else {
        var rootData = ns.CustomTabsTree.iobjDataSource.data();
        if (rootData.length > 1) {
            for (var i = 0; i < rootData.length; i++) {
                if (dataitem.divID !== rootData[i].divID && rootData[i].data.HideNode !== true) {
                    if ((ns.FormOpenedOnRight !== undefined && rootData[i].divID === ns.FormOpenedOnRight.divID) || ns.FormOpenedOnLeft !== undefined && rootData[i].divID === ns.FormOpenedOnLeft.divID)
                        continue;
                    else if (rootData[i].divID.toLowerCase().indexOf("centerleft") < 0 && ns.iarrCenterLeftForms.indexOf(rootData[i].divID) < 0)
                        formToDisplay = rootData[i].divID;
                }
            }
        }
        else {
            var temp = true;
            if (ns.blnShowConfirmMsgForChildNodeDelete) {
                temp = confirm(DefaultMessages.MessageForChildNodeDelete.replace("{0}", dataitem.title));
            }
            if (!temp)
                return;
            else if (dataitem.divID.toLowerCase().indexOf("centerleft") < 0 && ns.iarrCenterLeftForms.indexOf(dataitem.divID) < 0)
                formToDisplay = dataitem.divID;
        }
    }
    if (ns.blnInNewMode) {
        MVVMGlobal.GetIntoNewMode(false);
        ns.blnInNewMode = false;
    }
    if (dataitem.divID == formToDisplay) {
        var roottreenodes = ns.CustomTabsTree.iobjDataSource.data();
        for (var i = 0; i < roottreenodes.length; i++) {
            if (roottreenodes[i].divID.toLowerCase().indexOf("centerleft") < 0 && ns.iarrCenterLeftForms.indexOf(roottreenodes[i].divID) < 0 && (roottreenodes[i].data.HideNode !== true)) {
                formToDisplay = roottreenodes[i].divID;
            }
        }
    }
    var leftSideDestroyed = false;
    var rightSideDestroyed = false;
    var destroyedForms = MVVMGlobal.RemoveForm([], dataitem);
    for (var i = 0; i < destroyedForms.length; i++) {
        if (ns.FormOpenedOnLeft !== undefined && destroyedForms[i] === ns.FormOpenedOnLeft.title) {
            leftSideDestroyed = true;
        }
        if (ns.FormOpenedOnRight !== undefined && destroyedForms[i] === ns.FormOpenedOnRight.title) {
            rightSideDestroyed = true;
            ns.FormOpenedOnRight = undefined;
            $("#crumDivRight").html("");
        }
    }
    if (!leftSideDestroyed && ns.FormOpenedOnLeft != undefined && ns.FormOpenedOnLeft.divID != dataitem.divID) {
        var roottreenodes = ns.CustomTabsTree.iobjDataSource.data();
        for (var i = 0; i < roottreenodes.length; i++) {
            if (roottreenodes[i].divID == ns.FormOpenedOnLeft.divID &&
                roottreenodes[i].divID.toLowerCase().indexOf("centerleft") < 0 && ns.iarrCenterLeftForms.indexOf(roottreenodes[i].divID) < 0 && (roottreenodes[i].data.HideNode !== true)) {
                formToDisplay = roottreenodes[i].divID;
                break;
            }
        }
    }
    ns.blnFromTreeview = true;
    if (leftSideDestroyed) {
        $(nsConstants.CRUM_DIV_SELECTOR).html("");
    }
    if (rightSideDestroyed) {
        $("#crumDivRight").html("");
        if (ns.idictSpitter != undefined && ns.idictSpitter[nsConstants.MIDDLE_SPLITTER] != undefined) {
            var lobjSplitter = ns.idictSpitter[nsConstants.MIDDLE_SPLITTER];
            if (lobjSplitter.jsObject != undefined) {
                lobjSplitter.collapse(nsConstants.RIGHT_SPLITTER_SELECTOR);
            }
        }
    }
    if (ns.viewModel.currentModel != undefined && ns.viewModel.currentForm != undefined
        && ns.viewModel.currentModel.indexOf(ns.viewModel.currentForm) < 0) {
        ns.viewModel.currentForm = nsCommon.GetFormNameFromDivID(ns.viewModel.currentModel);
    }
    if (!/wfp/.test(ns.viewModel.currentForm))
        var lstrCurrentForm = ns.viewModel.currentForm;
    else {
        if (typeof (ns.viewModel.CurrentModel) === "undefined")
            var lstrCurrentForm = ns.viewModel.currentForm;
        else
            var lstrCurrentForm = ns.viewModel.CurrentModel;
    }
    if (ns.viewModel.currentForm.indexOf(nsConstants.LOOKUP) > 0 && dataitem.divID.indexOf("wfmwfp") === 0 && lstrCurrentForm.indexOf("wfp") === 0) {
        lstrCurrentForm = [nsConstants.WFM, lstrCurrentForm].join('');
    }
    if (ns.viewModel.currentForm.indexOf(nsConstants.LOOKUP) > 0 && lstrCurrentForm.indexOf("wfp") !== 0 && lstrCurrentForm != dataitem.divID) {
        if (!(($([nsConstants.HASH, lstrCurrentForm].join('')).length > 0 && $([nsConstants.HASH, lstrCurrentForm].join('')).closest(nsConstants.MY_TASK_SELECTOR).length > 0)
            || (ns.iarrCenterLeftForms != undefined && ns.iarrCenterLeftForms.indexOf(lstrCurrentForm) >= 0))) {
            formToDisplay = lstrCurrentForm;
        }
    }
    if ((formToDisplay == undefined || formToDisplay === "")
        && (lstrNonCenterleftForm == undefined || lstrNonCenterleftForm.trim() === "")) {
        formToDisplay = ns.LandingPage;
    }
    else if (lstrNonCenterleftForm != undefined && lstrNonCenterleftForm.trim() !== "") {
        formToDisplay = lstrNonCenterleftForm;
    }
    if (formToDisplay != "") {
        var FormToDisplayItem = nsCommon.GetDataItemForNodeDelete(formToDisplay.trim());
        if (FormToDisplayItem == undefined && lblnCenterLeftParent === true && ns.LandingPage != undefined && ns.LandingPage.trim() !== "") {
            formToDisplay = ns.LandingPage.trim();
            FormToDisplayItem = nsCommon.GetDataItemForNodeDelete(formToDisplay);
        }
        if (lstrNonCenterleftForm != undefined && lstrNonCenterleftForm.trim() !== "" && FormToDisplayItem == undefined) {
            formToDisplay = lstrNonCenterleftForm.trim();
            FormToDisplayItem = nsCommon.GetDataItemForNodeDelete(formToDisplay);
        }
        if (FormToDisplayItem == undefined) {
            ns.blnFromTreeview = false;
            ns.blnFromDeleteTreeNode = false;
            if (lblnAlterFormCalled) {
                ns.viewModel.currentForm = formToDisplay;
                MVVMGlobal.GetIntoNewMode(false);
                ns.HashChangedFormCode = false;
                ns.spaRouter.navigate(["/spa/", formToDisplay, '/0'].join(''));
                return;
            }
            else {
                return;
            }
        }
        if (formToDisplay.indexOf(nsConstants.LOOKUP) > 0 || formToDisplay.indexOf("Quick") >= 0) {
            ns.blnFromTreeview = false;
            ns.blnFromDeleteTreeNode = false;
            ns.CustomTabsTree.selectByUID(FormToDisplayItem.uid);
            return;
        }
        if (leftSideDestroyed && rightSideDestroyed) {
            var NodeToSelect = ns.CustomTabsTree.findByUid(FormToDisplayItem.uid);
            if (FormToDisplayItem.formID != null && FormToDisplayItem.formID.indexOf("CenterLeft") < 0) {
                ns.CustomTabsTree.select(NodeToSelect, true);
                MVVMGlobal.LoadBreadCrums(FormToDisplayItem.divID);
            }
            ns.CustomTabsTree.select(NodeToSelect);
            if (ns.idictSpitter != undefined && ns.idictSpitter[nsConstants.MIDDLE_SPLITTER] != undefined) {
                var lobjSplitterDetails = ns.idictSpitter[nsConstants.MIDDLE_SPLITTER];
                if (lobjSplitterDetails.jsObject != undefined) {
                    lobjSplitterDetails.collapse(nsConstants.RIGHT_SPLITTER_SELECTOR);
                    lobjSplitterDetails.expand(nsConstants.CENTER_LEFT_SELECTOR);
                }
            }
        }
        else if (leftSideDestroyed && !rightSideDestroyed) {
            var NodeToSelect = ns.CustomTabsTree.findByUid(FormToDisplayItem.uid);
            if (FormToDisplayItem.formID != null && FormToDisplayItem.formID.indexOf("CenterLeft") < 0) {
                ns.CustomTabsTree.select(NodeToSelect, true);
                MVVMGlobal.LoadBreadCrums(FormToDisplayItem.divID);
            }
            ns.CustomTabsTree.select(NodeToSelect);
            if (ns.FormOpenedOnRight != undefined && formToDisplay === ns.FormOpenedOnRight.title) {
                ns.FormOpenedOnRight = undefined;
                if (ns.idictSpitter != undefined && ns.idictSpitter[nsConstants.MIDDLE_SPLITTER] != undefined) {
                    var lobjSplitterDetails = ns.idictSpitter[nsConstants.MIDDLE_SPLITTER];
                    if (lobjSplitterDetails.jsObject != undefined) {
                        lobjSplitterDetails.collapse(nsConstants.RIGHT_SPLITTER_SELECTOR);
                    }
                }
            }
        }
        else if (!leftSideDestroyed && rightSideDestroyed) {
            if (ns.idictSpitter != undefined && ns.idictSpitter[nsConstants.MIDDLE_SPLITTER] != undefined) {
                var lobjSplitterDetails = ns.idictSpitter[nsConstants.MIDDLE_SPLITTER];
                if (lobjSplitterDetails.jsObject != undefined) {
                    lobjSplitterDetails.collapse(nsConstants.RIGHT_SPLITTER_SELECTOR);
                    lobjSplitterDetails.expand(nsConstants.CENTER_LEFT_SELECTOR);
                }
            }
        }
        else if (!leftSideDestroyed && !rightSideDestroyed) {
            var SelectedNode = ns.tabsTreeView.getSelectedNode();
            var NodeToSelect;
            if (SelectedNode != undefined && SelectedNode != null && SelectedNode.divID != undefined && SelectedNode.divID != null) {
                NodeToSelect = ns.CustomTabsTree.findByUid(SelectedNode.uid);
                ns.CustomTabsTree.select(NodeToSelect, true);
                MVVMGlobal.LoadBreadCrums(SelectedNode.divID);
            }
            if (NodeToSelect != null) {
                ns.CustomTabsTree.select(NodeToSelect);
            }
        }
    }
    ns.blnFromTreeview = false;
    ns.blnFromDeleteTreeNode = false;
}

nsCommon.AddNodeToTreeViewBase = nsCommon.AddNodeToTreeView;
nsCommon.AddNodeToTreeView = function AddNodeToTreeView(aobjTreeNode, aobjParentNode, ablnDoNotAddToHistory) {
    var customParentNode = aobjParentNode;
    if (aobjParentNode != undefined) {
        customParentNode = ns.CustomTabsTree.getNodeDataByDivID(aobjParentNode.divID);
        if (customParentNode && customParentNode.children.length > 0) {
            customParentNode.items.forEach(function (element) {
                ns.CustomTabsTree.remove(element);
            });
        }
    }
    nsCommon.AddNodeToTreeViewBase(aobjTreeNode, customParentNode, ablnDoNotAddToHistory);
}

MVVMGlobal.LoadBreadCrums = function (astrActiveDivID, aDataItem, adomDiv, adomStepDiv) {
    if (ns.MaxNoOfBreadCrums == undefined || isNaN(ns.MaxNoOfBreadCrums) || ns.MaxNoOfBreadCrums <= 0) {
        ns.MaxNoOfBreadCrums = 4;
    }
    var lintBreadCrumsToDisplay = ns.MaxNoOfBreadCrums;
    var lstrCurmHtml = "";
    var fn = nsUserFunctions["GetLoadBreadCrumsUserTemplate"];
    if (typeof fn === 'function') {
        var Context = {
            activeDivID: astrActiveDivID,
        };
        var e = {};
        e.context = Context;
        lstrCurmHtml = fn(e);
    }
    if (lstrCurmHtml === "") {
        var larrHtml = [];
        var dataItem = ns.CustomTabsTree.getNodeDataByDivID(astrActiveDivID);
        if (dataItem == undefined) {
            dataItem = ns.tabsTreeView.getNodeDataByDivID(astrActiveDivID);
        }
        var tempArray = [];
        var parentDataItem = undefined;
        while (dataItem !== undefined) {
            tempArray.push(dataItem);
            parentDataItem = dataItem.parentNode();
            if (parentDataItem !== undefined) {
                if (parentDataItem.divID == astrActiveDivID) {
                    dataItem = undefined;
                }
                else {
                    dataItem = parentDataItem;
                }
            }
            else {
                dataItem = parentDataItem;
            }
        }
        larrHtml.push("<input type='button' aria-label='Go To Form Contents' class='GoToLinksTrigger'/><div class='GoToLinks'><strong>", Sagitec.DefaultText.TEXT_CONTENTS, "</strong><hr/><br/><div class='s-divGotoLinkUlContents'>", ns.PopulateGoToLinks(tempArray[0].divID, adomStepDiv), "</div></div>", '<table role="presentation"><tr><td>', "<ul class='breadcrumb'>");
        var star = "";
        var tempArrLen = tempArray.length;
        if (tempArrLen > lintBreadCrumsToDisplay) {
            tempArrLen = tempArrLen - (tempArrLen - lintBreadCrumsToDisplay);
        }
        for (var i = tempArrLen - 1; i >= 0; i--) {
            star = "";
            if (ns.DirtyData[tempArray[i].divID] !== undefined)
                star = "*";
            var title = tempArray[i].title;
            if (title == undefined && tempArray[i].divID.indexOf("wfmrul") === 0) {
                title = "Rule Result";
            }
            var croptitle = nsCommon.GetCropedTitleForBreadcrumbs(title, nsConstants.BREADCRUM_CROP_TITLE_INDEXES.indexOf(i) >= 0, i, tempArray);
            var linkedToString = ["class='crumLinks' linkedTo='", tempArray[i].divID, "' "].join('');
            if (ns.blnInNewMode) {
                linkedToString = "";
            }
            var closeHtml = "";
            if (i == 0) {
                closeHtml = "<input aria-label='Close " + tempArray[i].title + "' style='float:left' type='button' onclick='nsEvents.OnDeleteFormClick(\"" + tempArray[i].divID + "\")' class='delete-form'></input>";
            }
            larrHtml.push("<li title='", title, "' formid='", tempArray[i].divID, "'><a title='", title, "' ", linkedToString, ">", croptitle, star, "</span></a>", closeHtml, "</li>");
        }
        larrHtml.push('</ul>', "<div class='breadcrumb-page-info'>", nsCommon.GetNextPreviousButtons(tempArray[0].divID), "</div></td></tr></table>");
        lstrCurmHtml = larrHtml.join('');
    }
    if (ns.isRightSideForm === undefined)
        ns.isRightSideForm = false;
    var fn = nsUserFunctions["DisplayBreadCrums"];
    var lblnDisplayBreadCrums = true;
    if (typeof fn === 'function') {
        var Context = {
            activeDivID: astrActiveDivID,
        };
        var e = {};
        e.context = Context;
        lblnDisplayBreadCrums = fn(e);
    }
    if (adomDiv == undefined)
        adomDiv = $([nsConstants.HASH, astrActiveDivID].join(''));
    if (adomDiv.length > 0 && (adomDiv.closest(nsConstants.MY_TASK_SELECTOR).length > 0 || (ns.iarrCenterLeftForms != undefined && ns.iarrCenterLeftForms.indexOf(astrActiveDivID) >= 0))) {
        return false;
    }
    if (ns.isRightSideForm === false) {
        var domCrum = document.getElementById(nsConstants.CRUM_DIV_SELECTOR.replace("#", ""));
        if (domCrum != null) {
            if (lblnDisplayBreadCrums) {
                domCrum.innerHTML = lstrCurmHtml;
                domCrum.style.display = "block";
            }
            else {
                domCrum.style.display = "none";
            }
        }
    }
    else {
        if (lblnDisplayBreadCrums) {
            $("#crumDivRight").html(lstrCurmHtml).show();
        }
        else {
            $("#crumDivRight").hide();
        }
    }
}
function AfterGetTemplate(astrFormID, ablnCenterLeft, data, astrPostFix) {
    astrFormID = [astrFormID, astrPostFix].join('');
    var NewFormid = nsCommon.GetProperFormId(astrFormID);
    var lobjPageStateData = null;
    if (ns.iblnShowGridStoreStateButtons === true && ablnCenterLeft !== true) {
        var lobjData = data;
        if (lobjData.DomainModel.OtherData != undefined && lobjData.DomainModel.OtherData["PageStateData"] != undefined) {
            try {
                lobjPageStateData = jQuery.parseJSON(HtmlWhitelistedSanitizer.sanitizeHTMLString(lobjData.DomainModel.OtherData["PageStateData"]));
            }
            catch (ex) {
                try {
                    lobjData.DomainModel.OtherData["PageStateData"] = lobjData.DomainModel.OtherData["PageStateData"].replaceAll('\\"', "'");
                    lobjPageStateData = jQuery.parseJSON(HtmlWhitelistedSanitizer.sanitizeHTMLString(lobjData.DomainModel.OtherData["PageStateData"]));
                }
                catch (ex1) {
                    lobjPageStateData = jQuery.parseJSON(lobjData.DomainModel.OtherData["PageStateData"]);
                }
            }
        }
    }
    var larrCodeValues = (data != undefined && data.DomainModel != undefined && data.DomainModel.OtherData != undefined) ? data.DomainModel.OtherData["FormLoadSourceValues"] : null;
    if (data.ExtraInfoFields["FormType"] == "Report" || data.ExtraInfoFields["FormType"] == nsConstants.LOOKUP || data.ExtraInfoFields["FormType"] == "FormLinkLookup") {
        var lblnIsReportForm = false;
        if (data.ExtraInfoFields["FormType"] == "Report") {
            lblnIsReportForm = true;
        }
        if (ns.iblnShowGridStoreStateButtons === true && lobjPageStateData != null) {
            nsCommon.SetPageStateData(lobjPageStateData, NewFormid);
        }
        if (lblnIsReportForm) {
            var lstrSenderForm = astrFormID;
        }
        else {
            var lstrSenderForm = nsCommon.GetProperFormName(astrFormID);
        }
        if (ns.SenderForm != lstrSenderForm) {
            ns.setSenderData("", lstrSenderForm, "");
        }
        if (nsConstants.UNDERSCORE_RETRIEVE === astrPostFix) {
            lstrSenderForm = [lstrSenderForm, astrPostFix].join('');
        }
        var lstrUserDefaults = nsCommon.sessionGet(nsConstants.USER_STORED_DEFAULTS_FOR_LOOKUP);
        if (lstrUserDefaults != undefined && lstrUserDefaults[lstrSenderForm] != undefined) {
            var headerData = lstrUserDefaults[lstrSenderForm];
            if (headerData != undefined) {
                for (var k in headerData) {
                    if (k != "ControlList") {
                        data.DomainModel.HeaderData[k] = headerData[k];
                    }
                }
            }
        }
        var lblnHasDefaultControls = false;
        if (data != undefined && data.ControlAttribites != undefined) {
            lblnHasDefaultControls = _.filter(data.ControlAttribites, function (item) {
                return (item["sfwDefaultValue"] != undefined && item["sfwDefaultValue"] != "") || (item["sfwDefaultType"] != undefined && item["sfwDefaultType"] != "");
            }).length > 0;
        }
        var lblnControlDefaultsCalled = false;
        var lobjControlList = nsCommon.ManageLookupControlList(astrFormID);
        if (lobjControlList == undefined) {
            var lobjSenderData = nsCommon.GetSenderData(lstrSenderForm, lstrSenderForm, lstrSenderForm, ablnCenterLeft ? "CenterLeft" : "");
            var lobjLookupControlList = nsRequest.SyncPost(["GetLookupControlList?astrFormID=", lstrSenderForm, "&ablnIsCenterLeft=", ablnCenterLeft, "&ablnGetDefaults=", lblnHasDefaultControls, "&ablnCallList=true"].join(''), null, null, "GET", lobjSenderData);
            lblnControlDefaultsCalled = true;
            if (lobjLookupControlList != null) {
                lobjControlList = lobjLookupControlList.ControlList;
                if (lobjControlList != undefined) {
                    nsCommon.ManageLookupControlList(astrFormID, lobjControlList);
                    if (!lblnIsReportForm) {
                        data.DomainModel.HeaderData.ControlList = lobjControlList;
                    }
                }
                if (lobjLookupControlList.DefaultControlValues != undefined) {
                    var larrDefaultKeys = Object.keys(lobjLookupControlList.DefaultControlValues), lstrControlkey;
                    for (var iC = 0, iClen = larrDefaultKeys.length; iC < iClen; iC++) {
                        lstrControlkey = larrDefaultKeys[iC];
                        if (data.DomainModel.HeaderData["tblCriteria"] != undefined && data.DomainModel.HeaderData["tblCriteria"][lstrControlkey] != undefined && ((data.DomainModel.HeaderData["tblCriteria"][lstrControlkey] === ""))) {
                            data.DomainModel.HeaderData["tblCriteria"][lstrControlkey] = lobjLookupControlList.DefaultControlValues[lstrControlkey];
                        }
                        else if (!lblnIsReportForm && data.DomainModel.HeaderData["tblAdvCriteria"] != undefined && data.DomainModel.HeaderData["tblAdvCriteria"][lstrControlkey] != undefined && ((data.DomainModel.HeaderData["tblAdvCriteria"][lstrControlkey] === ""))) {
                            data.DomainModel.HeaderData["tblAdvCriteria"][lstrControlkey] = lobjLookupControlList.DefaultControlValues[lstrControlkey];
                        }
                    }
                }
            }
        }
        else if (data.DomainModel != undefined && data.DomainModel.HeaderData != undefined) {
            data.DomainModel.HeaderData.ControlList = lobjControlList;
        }
        if (!lblnIsReportForm) {
            if (lobjControlList != undefined && lobjControlList["SecurityMessage"] != undefined) {
                data.ExtraInfoFields.SecurityMessage = lobjControlList["SecurityMessage"];
            }
        }
        if (lblnControlDefaultsCalled !== true && lblnHasDefaultControls === true) {
            var lobjSenderData = nsCommon.GetSenderData(lstrSenderForm, lstrSenderForm, lstrSenderForm, ablnCenterLeft ? "CenterLeft" : "");
            var lobjLookupControlList = nsRequest.SyncPost(["GetLookupControlList?astrFormID=", lstrSenderForm, "&ablnIsCenterLeft=", ablnCenterLeft, "&ablnGetDefaults=", lblnHasDefaultControls, "&ablnCallList=false"].join(''), null, null, "GET", lobjSenderData);
            if (lobjLookupControlList != null && lobjLookupControlList.DefaultControlValues != undefined) {
                var larrDefaultKeys = Object.keys(lobjLookupControlList.DefaultControlValues), lstrControlkey;
                for (var iC = 0, iClen = larrDefaultKeys.length; iC < iClen; iC++) {
                    lstrControlkey = larrDefaultKeys[iC];
                    if (data.DomainModel.HeaderData["tblCriteria"] != undefined && data.DomainModel.HeaderData["tblCriteria"][lstrControlkey] != undefined && ((data.DomainModel.HeaderData["tblCriteria"][lstrControlkey] === ""))) {
                        data.DomainModel.HeaderData["tblCriteria"][lstrControlkey] = lobjLookupControlList.DefaultControlValues[lstrControlkey];
                    }
                    else if (data.DomainModel.HeaderData["tblAdvCriteria"] != undefined && data.DomainModel.HeaderData["tblAdvCriteria"][lstrControlkey] != undefined && ((data.DomainModel.HeaderData["tblAdvCriteria"][lstrControlkey] === ""))) {
                        data.DomainModel.HeaderData["tblAdvCriteria"][lstrControlkey] = lobjLookupControlList.DefaultControlValues[lstrControlkey];
                    }
                }
            }
        }
        data.DomainModel.HeaderData.ClientVisibility = data.ClientVisibility;
        headerData = data.DomainModel.HeaderData;
        nsCommon.sessionSet(["UserDefaults_", NewFormid].join(''), headerData);
        ns.Templates[NewFormid] = {
            "FormType": nsConstants.LOOKUP,
            "Template": data.Template,
            "ExtraInfoFields": data.ExtraInfoFields,
            "InnerTemplates": data.InnerTemplates,
            "HeaderData": headerData,
            "ControlAttribites": data.ControlAttribites,
            "DetailsData": {},
            "ControlsHaveingVisibility": {},
            "WidgetControls": {},
            "PageStateData": lobjPageStateData,
            "LoadSourceValues": larrCodeValues
        };
    }
    else {
        ns.Templates[NewFormid] = {
            "FormType": nsConstants.MAINTENANCE,
            "Template": data.Template,
            "ExtraInfoFields": data.ExtraInfoFields,
            "InnerTemplates": data.InnerTemplates,
            "ControlAttribites": data.ControlAttribites,
            "ClientVisibility": data.ClientVisibility,
            "WidgetControls": {},
            "PageStateData": lobjPageStateData,
            "LoadSourceValues": larrCodeValues
        };
        if (NewFormid.indexOf("wfmwfp") === 0 || NewFormid.indexOf("wfp") === 0) {
            ns.Templates[NewFormid]["DomainModel"] = data.DomainModel;
        }
    }
    if (ns.Templates[NewFormid] != undefined && ns.Templates[NewFormid].ExtraInfoFields != undefined && data.ExtraInfoFields != undefined && data.ExtraInfoFields.SecurityMessage != undefined) {
        ns.Templates[NewFormid].ExtraInfoFields.SecurityMessage = data.ExtraInfoFields.SecurityMessage;
    }
    if (ns.Templates[NewFormid] != undefined && ns.Templates[NewFormid].ExtraInfoFields
        && ns.Templates[NewFormid].ExtraInfoFields.ShortCutKeys) {
        ns.Templates[NewFormid].ShortCutKeys = JSON.parse(ns.Templates[NewFormid].ExtraInfoFields.ShortCutKeys);
    }
    if (ns.Templates[NewFormid] != undefined && ns.Templates[NewFormid].ControlAttribites != null) {
        var lCodeValues, lstrCodeValue, lstrLoadType;
        if (larrCodeValues == undefined) {
            lCodeValues = [];
        }
        Object.freeze(ns.Templates[NewFormid].ControlAttribites);
        var larrKeys = Object.keys(ns.Templates[NewFormid].ControlAttribites), key;
        for (var i = 0, len = larrKeys.length; i < len; i++) {
            key = larrKeys[i];
            Object.freeze(ns.Templates[NewFormid].ControlAttribites[key]);
            if (lCodeValues != undefined) {
                lstrLoadType = ns.Templates[NewFormid].ControlAttribites[key].sfwLoadType;
                if (lstrLoadType === "CodeGroup") {
                    lstrCodeValue = ns.Templates[NewFormid].ControlAttribites[key].sfwLoadSource;
                    if (lstrCodeValue != undefined && lstrCodeValue.trim() != "" && lCodeValues.indexOf(lstrCodeValue.trim()) < 0) {
                        lCodeValues.push(lstrCodeValue.trim());
                    }
                }
            }
        }
        if (lCodeValues != undefined && larrCodeValues == undefined) {
            ns.Templates[NewFormid].LoadSourceValues = lCodeValues;
        }
    }
}
MVVMGlobal.AfterGetTemplate = AfterGetTemplate;

function LaunchFileNetImageURL(aintProcessInstanceAttachmentId) {
    var Parameters = {};
    Parameters["aintProcessInstanceAttachmentId"] = aintProcessInstanceAttachmentId;
    var lvarFileNetUrls = {};
    lvarFileNetUrls = nsRequest.SyncPost("GetFileNameImageURLs", Parameters, "POST");
    var lstrFeatures = "width=1000px,height=800px,center=yes,help=no, resizable=yes, status=no, top=25, scrollbars=yes,toolbar=yes,location=yes,directories=yes,status=yes,menubar=yes";
    var lstrScript = "";
    var lstrFinalDisplayURL = "";
    if (lvarFileNetUrls && lvarFileNetUrls != undefined && lvarFileNetUrls != null && lvarFileNetUrls.length > 0) {
        var lblnIsIEBrowser = false;
        lblnIsIEBrowser = /*@cc_on!@*/false || !!document.documentMode;
        /*Here we detect browser is IE or Other than IE */
        for (indexURL = 0; indexURL < lvarFileNetUrls.length; indexURL++) {
            if (lblnIsIEBrowser) {
                if (indexURL == 0) {
                    lstrScript = "var newWindow = window.open('', '_blank', '" + lstrFeatures + "');";
                    lstrFirstUrl = lvarFileNetUrls[indexURL];
                }
                else {
                    lstrScript = "var newWindow" + indexURL + " = newWindow.window.open('', '_blank');";
                    lstrScript += "newWindow" + indexURL + ".location.href = '" + lvarFileNetUrls[indexURL] + "';";
                }
                lstrFinalDisplayURL += lstrScript;
            }
            else {
                var URL = lvarFileNetUrls[indexURL];
                var theTop = 100 + (indexURL * 100);
                var theLeft = 100 + (indexURL * 100);
                window.open(
                    URL, "Window_name" + indexURL,
                    "height=470px,width=850px,modal=yes,alwaysRaised=yes,top=" + theTop + ",left=" + theLeft + ",toolbar=1,Location=0,Directories=0,Status=0,menubar=1,Scrollbars=1,Resizable=1");
            }
        }
        if (lblnIsIEBrowser) {
            lstrFinalDisplayURL += "newWindow.location.href = '" + lstrFirstUrl + "';";
            var OpenurlImages = "function() { " + lstrFinalDisplayURL + " }";
            var OpenUrlImagesFunction = new Function('return ' + OpenurlImages)();
            OpenUrlImagesFunction();
        }
    }
}
function ClearCommentsOnDetailLookup() {
    if (ns.viewModel.currentForm.indexOf("wfmEmployerPayrollDetailLookup") === 0) {
        var ldomForm = $("#" + ns.viewModel.currentModel);
        if (ldomForm.length > 0) {
            var ldomElement = ldomForm.find("#txtDetailComment");
            if (ldomElement.length > 0) {
                ldomElement.val('');
                ldomElement.attr('autocomplete', 'off');
            }
        }
    }
}

function CheckIfRefundOptionsSelected() {
    
    var lChecklist = document.getElementById("cblReasonForRefund").getElementsByTagName('input');
    var lcheckBoxSelectedItems1 = new Array();
    for (var i = 0; i < lChecklist.length; i++) {
        if (lChecklist[i].type == 'checkbox') {
            if (lChecklist[i].checked) {
                lcheckBoxSelectedItems1.push(lChecklist[i].labels[0].innerText);
            }
        }
        if (lcheckBoxSelectedItems1.length > 0 && lcheckBoxSelectedItems1.includes("Change in Coverage - Spouse Passed")) {
            $("#" + ns.viewModel.currentModel).find("#capSpouseMedicare").removeClass("hideControl");
            $("#" + ns.viewModel.currentModel).find("#chkSpouseMedicare").removeClass("hideControl");
        }
        else {
            $("#" + ns.viewModel.currentModel).find("#capSpouseMedicare").addClass("hideControl");
            $("#" + ns.viewModel.currentModel).find("#chkSpouseMedicare").addClass("hideControl");
        }

        if (lcheckBoxSelectedItems1.length > 0 &&
            (lcheckBoxSelectedItems1.includes("PERS Error (Incorrect coverage code/rate structure)") ||
            lcheckBoxSelectedItems1.includes("LIS") ||
            lcheckBoxSelectedItems1.includes("LEP")))
        {
            $("#" + ns.viewModel.currentModel).find("#capOldRate").removeClass("hideControl");
            $("#" + ns.viewModel.currentModel).find("#txtOldRate").removeClass("hideControl");
            $("#" + ns.viewModel.currentModel).find("#capNewRate").removeClass("hideControl");
            $("#" + ns.viewModel.currentModel).find("#txtNewRate").removeClass("hideControl");
            $("#" + ns.viewModel.currentModel).find("#capDifferencePremiums").removeClass("hideControl");
            $("#" + ns.viewModel.currentModel).find("#txtDiffInPremium").removeClass("hideControl");
            $("#" + ns.viewModel.currentModel).find("#capDateRangeFrom").removeClass("hideControl");
            $("#" + ns.viewModel.currentModel).find("#txtFromDateRange").removeClass("hideControl");
            $("#" + ns.viewModel.currentModel).find("#capDateRangeTo").removeClass("hideControl");
            $("#" + ns.viewModel.currentModel).find("#txtToDateRange").removeClass("hideControl");
        }
        else {
            $("#" + ns.viewModel.currentModel).find("#capOldRate").addClass("hideControl");
            $("#" + ns.viewModel.currentModel).find("#txtOldRate").addClass("hideControl");
            $("#" + ns.viewModel.currentModel).find("#capNewRate").addClass("hideControl");
            $("#" + ns.viewModel.currentModel).find("#txtNewRate").addClass("hideControl");
            $("#" + ns.viewModel.currentModel).find("#capDifferencePremiums").addClass("hideControl");
            $("#" + ns.viewModel.currentModel).find("#txtDiffInPremium").addClass("hideControl");
            $("#" + ns.viewModel.currentModel).find("#capDateRangeFrom").addClass("hideControl");
            $("#" + ns.viewModel.currentModel).find("#txtFromDateRange").addClass("hideControl");
            $("#" + ns.viewModel.currentModel).find("#capDateRangeTo").addClass("hideControl");
            $("#" + ns.viewModel.currentModel).find("#txtToDateRange").addClass("hideControl");
        }

        if (lcheckBoxSelectedItems1.length > 0 &&
            (lcheckBoxSelectedItems1.includes("Change in Coverage - Spouse Passed") ||
                lcheckBoxSelectedItems1.includes("Change in Coverage – Member’s Request")))
        {
            $("#" + ns.viewModel.currentModel).find("#capOldRate").removeClass("hideControl");
            $("#" + ns.viewModel.currentModel).find("#txtOldRate").removeClass("hideControl");
            $("#" + ns.viewModel.currentModel).find("#capOldRate2").removeClass("hideControl");
            $("#" + ns.viewModel.currentModel).find("#txtOldRate2").removeClass("hideControl");
            $("#" + ns.viewModel.currentModel).find("#capOldRate3").removeClass("hideControl");
            $("#" + ns.viewModel.currentModel).find("#txtOldRate3").removeClass("hideControl");
            $("#" + ns.viewModel.currentModel).find("#capNewRate").removeClass("hideControl");
            $("#" + ns.viewModel.currentModel).find("#txtNewRate").removeClass("hideControl");
            $("#" + ns.viewModel.currentModel).find("#capNewRate2").removeClass("hideControl");
            $("#" + ns.viewModel.currentModel).find("#txtNewRate2").removeClass("hideControl");
            $("#" + ns.viewModel.currentModel).find("#capNewRate3").removeClass("hideControl");
            $("#" + ns.viewModel.currentModel).find("#txtNewRate3").removeClass("hideControl");
            $("#" + ns.viewModel.currentModel).find("#capDifferencePremiums").removeClass("hideControl");
            $("#" + ns.viewModel.currentModel).find("#txtDiffInPremium").removeClass("hideControl");
            $("#" + ns.viewModel.currentModel).find("#capDifferencePremiums2").removeClass("hideControl");
            $("#" + ns.viewModel.currentModel).find("#txtDiffInPremium2").removeClass("hideControl");
            $("#" + ns.viewModel.currentModel).find("#capDifferencePremiums3").removeClass("hideControl");
            $("#" + ns.viewModel.currentModel).find("#txtDiffInPremium3").removeClass("hideControl");
            $("#" + ns.viewModel.currentModel).find("#capDateRangeFrom").removeClass("hideControl");
            $("#" + ns.viewModel.currentModel).find("#txtFromDateRange").removeClass("hideControl");
            $("#" + ns.viewModel.currentModel).find("#capDateRangeTo").removeClass("hideControl");
            $("#" + ns.viewModel.currentModel).find("#txtToDateRange").removeClass("hideControl");
        }
        else {
            $("#" + ns.viewModel.currentModel).find("#capOldRate").addClass("hideControl");
            $("#" + ns.viewModel.currentModel).find("#txtOldRate").addClass("hideControl");
            $("#" + ns.viewModel.currentModel).find("#capOldRate2").addClass("hideControl");
            $("#" + ns.viewModel.currentModel).find("#txtOldRate2").addClass("hideControl");
            $("#" + ns.viewModel.currentModel).find("#capOldRate3").addClass("hideControl");
            $("#" + ns.viewModel.currentModel).find("#txtOldRate3").addClass("hideControl");
            $("#" + ns.viewModel.currentModel).find("#capNewRate").addClass("hideControl");
            $("#" + ns.viewModel.currentModel).find("#txtNewRate").addClass("hideControl");
            $("#" + ns.viewModel.currentModel).find("#capNewRate2").addClass("hideControl");
            $("#" + ns.viewModel.currentModel).find("#txtNewRate2").addClass("hideControl");
            $("#" + ns.viewModel.currentModel).find("#capNewRate3").addClass("hideControl");
            $("#" + ns.viewModel.currentModel).find("#txtNewRate3").addClass("hideControl");
            $("#" + ns.viewModel.currentModel).find("#capDifferencePremiums").addClass("hideControl");
            $("#" + ns.viewModel.currentModel).find("#txtDiffInPremium").addClass("hideControl");
            $("#" + ns.viewModel.currentModel).find("#capDifferencePremiums2").addClass("hideControl");
            $("#" + ns.viewModel.currentModel).find("#txtDiffInPremium2").addClass("hideControl");
            $("#" + ns.viewModel.currentModel).find("#capDifferencePremiums3").addClass("hideControl");
            $("#" + ns.viewModel.currentModel).find("#txtDiffInPremium3").addClass("hideControl");
            $("#" + ns.viewModel.currentModel).find("#capDateRangeFrom").addClass("hideControl");
            $("#" + ns.viewModel.currentModel).find("#txtFromDateRange").addClass("hideControl");
            $("#" + ns.viewModel.currentModel).find("#capDateRangeTo").addClass("hideControl");
            $("#" + ns.viewModel.currentModel).find("#txtToDateRange").addClass("hideControl");
        }

    }
    
    }

function CustomPopupToInactiveContact(message) {
    $("<div id='popUpDiv'>" + message + "</div>").dialog({
        title: 'Alert',
        show: "slide",
        modal: true,
        resizable: false,
        height: "auto",
        width: 450,
        buttons: {
            "No": function () {
                $(this).dialog('close');
            },
            "Yes": function () {
                $(this).dialog('close');
                var ldomStatusDD = $("#" + ns.viewModel.currentModel);
                ldomStatusDD.find("#btnInactiveContact").click();
            }
        },
        create: function () {
            $(this).closest(".ui-dialog")
                .find(".ui-button") // the first button
                .addClass("button");
        }
    });
    $(".ui-dialog-titlebar-close").text('X');
    $(".ui-dialog-titlebar").attr("style", "background:#556080;color:Black;");
}

nsCorr.HttpPortRange = "";
nsCorr.HttpsPortRange = "";
nsCorr.HttpPortLow = 0;
nsCorr.HttpPortHigh = 0;
nsCorr.HttpsPortLow = 0;
nsCorr.HttpsPortHigh = 0;
nsCorr.HttpPort = 8081;
nsCorr.HttpsPort = 8082;
nsCorr.ConnectedPort = 0;
nsCorr.RangeSpecified = false;
nsCorr.LoggedInUser = "";
nsCorr.Connected = false;
nsCorr.FindingPort = false;

function InitializeSignalrForCorrTool() {
    if (nsCorr.HttpPortRange && nsCorr.HttpPortRange != "") {
        nsCorr.HttpPortLow = Number(nsCorr.HttpPortRange.split(":")[0]);
        nsCorr.HttpPortHigh = Number(nsCorr.HttpPortRange.split(":")[1]);
    }
    if (nsCorr.HttpsPortRange && nsCorr.HttpsPortRange != "") {
        nsCorr.HttpsPortLow = Number(nsCorr.HttpsPortRange.split(":")[0]);
        nsCorr.HttpsPortHigh = Number(nsCorr.HttpsPortRange.split(":")[1]);
    }
    (function ($, window, undefined) {
        "use strict";
        if (typeof ($.signalR) !== "function") {
            console.log("SignalR: SignalR is not loaded. Please ensure jquery.signalR-x.js is referenced before ~/signalr/js.");
            return;
        }
        function makeProxyCallback(hub, callback) {
            return function () {
                callback.apply(hub, $.makeArray(arguments));
            };
        }
        function registerHubProxies(instance, shouldSubscribe) {

            var key, hub, memberKey, memberValue, subscriptionMethod;
            for (key in instance) {
                if (instance.hasOwnProperty(key)) {
                    hub = instance[key];
                    if (!(hub.hubName)) {
                        continue;
                    }
                    if (shouldSubscribe) {
                        subscriptionMethod = hub.on;
                    }
                    else {
                        subscriptionMethod = hub.off;
                    }
                    for (memberKey in hub.client) {
                        if (hub.client.hasOwnProperty(memberKey)) {
                            memberValue = hub.client[memberKey];
                            if (!$.isFunction(memberValue)) {
                                continue;
                            }
                            subscriptionMethod.call(hub, memberKey, makeProxyCallback(hub, memberValue));
                        }
                    }
                }
            }
        }
        $.hubConnection.prototype.createHubProxies = function () {
            var proxies = $.connection.hub.proxies || {};
            this.starting(function () {
                registerHubProxies(proxies, true);
                this._registerSubscribedHubs();
            }).disconnected(function () {
                registerHubProxies(proxies, false);
                setTimeout(function () {
                    $.CorrHubConnection.start().done(function () {
                        console.log("Reconnected");
                        $.CorrHubConnection.proxies.corrsignalrhub.server.setWindowName(window.name).done(function () {
                            console.log("reset window name");
                        });
                    }).fail(function () {
                        console.log('Could not Connect!');
                    });
                }, 500);
            });
            proxies['CorrSignalRHub'] = this.createHubProxy('CorrSignalRHub');
            proxies['CorrSignalRHub'].client = {};
            proxies['CorrSignalRHub'].server = {
                createCorrInstance: function (message) {
                    return proxies['CorrSignalRHub'].invoke.apply(proxies['CorrSignalRHub'], $.merge(["CreateCorrInstance"], $.makeArray(arguments)));
                },
                statusUpdateSuccess: function (astrMessage, ablnError) {
                    return proxies['CorrSignalRHub'].invoke.apply(proxies['CorrSignalRHub'], $.merge(["StatusUpdateSuccess"], $.makeArray(arguments)));
                },
                setWindowName: function (astrWindowName) {
                    return proxies['CorrSignalRHub'].invoke.apply(proxies['CorrSignalRHub'], $.merge(["SetWindowName"], $.makeArray(arguments)));
                },
                getAssemblyVersion: function () {
                    return proxies['CorrSignalRHub'].invoke.apply(proxies['CorrSignalRHub'], $.merge(["getAssemblyVersion"], $.makeArray(arguments)));
                },
                MatchAppUserName: function (astrAppUserName) {
                    return proxies['CorrSignalRHub'].invoke.apply(proxies['CorrSignalRHub'], $.merge(["MatchAppUserName"], $.makeArray(arguments)));
                }
            };
            return proxies;
        };
        if (nsCorr.ConnectedPort == 0 && !nsCorr.RangeSpecified) {
            if (document.location.protocol.indexOf("https") == 0) {
                nsCorr.ConnectedPort = nsCorr.HttpsPort;
            }
            else {
                nsCorr.ConnectedPort = nsCorr.HttpPort;
            }
        }
        var ConnectoToThisPort = nsCorr.ConnectedPort;
        if (!nsCorr.RangeSpecified || ConnectoToThisPort != 0) {
            if (document.location.protocol.indexOf("https") == 0) {
                if (ConnectoToThisPort != null) {
                    nsCorr.HttpsPort = ConnectoToThisPort;
                }
                $.CorrHubConnection = $.hubConnection("https://localhost:" + nsCorr.HttpsPort + "/signalr", { useDefaultPath: false });
            }
            else {
                if (ConnectoToThisPort != null) {
                    nsCorr.HttpPort = ConnectoToThisPort;
                }
                $.CorrHubConnection = $.hubConnection("http://localhost:" + nsCorr.HttpPort + "/signalr", { useDefaultPath: false });
            }
            $.CorrHub = $.CorrHubConnection.createHubProxies("CorrSignalRHub");
        }
        else {
            nsCorr.FindingPort = true;
            nsCorr.Connected = false;
            if (document.location.protocol.indexOf("https") == 0) {
                TryConnect(nsCorr.HttpsPortLow, nsCorr.HttpsPortHigh, "https://localhost:");
            }
            else {
                TryConnect(nsCorr.HttpPortLow, nsCorr.HttpPortHigh, "http://localhost:");
            }
        }
    }(window.jQuery, window));
    if (nsCommon.sessionGet("ConnectedToCorrTool") == true) {
        if ($.CorrHubConnection) {
            nsCorr.RegisterClientFunctions();
            $.CorrHubConnection.start().done(function () {
                $.CorrHubConnection.proxies.corrsignalrhub.server.setWindowName(window.name).done(function () {
                });
            }).fail(function () {
                console.log('Could not Connect!');
            });
        }
    }
}
nsCorr.InitializeSignalrForCorrTool = InitializeSignalrForCorrTool;

function RegisterClientFunctions() {
    $.CorrHubConnection.proxies.corrsignalrhub.client.invokeResponseMessage = function (OtherData, astrFileData, ablnEOF) {
        if (astrFileData != undefined) {
            nsCorr.FileData += astrFileData;
        }
        if (ablnEOF) {
            if (nsCorr.FileData != "" && nsCorr.FileData != undefined)
                OtherData.CorrFileData = nsCorr.FileData;
            var ApiAction = OtherData.ApiAction;
            nsCorr.FileData = "";
            console.dir(OtherData);
            function UpdateCorrespondenceStatusCallback(jqXHR, textStatus, errorThrown, nsDeferred) {
                if (textStatus == "success") {
                    $.CorrHubConnection.proxies.corrsignalrhub.server.statusUpdateSuccess(jqXHR == null ? textStatus : jqXHR.responseText, false, false).done(function () {
                    });
                }
                else {
                    if (jqXHR.status == 400) {
                        $.CorrHubConnection.proxies.corrsignalrhub.server.statusUpdateSuccess(jqXHR.responseText, false, true).done(function () {
                        });
                    }
                    else {
                        $.CorrHubConnection.proxies.corrsignalrhub.server.statusUpdateSuccess(jqXHR.responseText, true, false).done(function () {
                        });
                    }
                }
            }
            var testr = nsRequest.OtherAjaxRequest("Storage/" + ApiAction, { param: OtherData }, null, false, "POST", UpdateCorrespondenceStatusCallback);
            if (testr != undefined) {
                UpdateCorrespondenceStatusCallback(null, "success", "null", null);
                console.dir(testr);
            }
        }
    };
    $.CorrHubConnection.proxies.corrsignalrhub.client.invokeResponseMessageNew = function (newMessage) {
        alert(newMessage);
    };
    $.CorrHubConnection.proxies.corrsignalrhub.client.keepConnectionActive = function (message) {
        console.log(message);
        ns.refreshSession();
        $.CorrHubConnection.proxies.corrsignalrhub.server.setWindowName(window.name).done(function () {
        });
    };
}
nsCorr.RegisterClientFunctions = RegisterClientFunctions;
