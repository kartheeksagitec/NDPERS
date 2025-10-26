$(function () {
    ns.iblnNavigateInNewMode = true;
    ns.iblnAddCustomButtonsToGridToolbar = true;
    ns.iblnShowHardErrorAssociatedToControl = false;
    ns.iblnShowHardErrorAssociatedToGridControl = false;
    ns.iblnHighlightAllErrorControls = true;
    //ns.iblnNonCollapsiblePanels = true;
    if (ns.iblnADATesting) { return false; }
    ns.blnLoading = true;
    ns.blnUseSlideoutForLookup = false; 
    ns.iblnHidePagesFromGridPaging = true;
    ns.iblnImagesForPaging = true;
    ns.iblnNavigateInNewMode = true;
    ns.iblnUseStoreDefaultsForLookup = false;
    //ns.iblnOpenRefreshedForm = true;
    nsVisi.iblnHideParentOnVisibility = false;
    //Code for stop collapsable pannel
    ns.iblnNonCollapsiblePanels = true;
    ns.iblnShowAlertForInvalidDate = true;
    ns.iblnClearValueForInvalidDate = true;
    ns.iblnDownloadFileInNewTab = true;
    //6.0.4.2 Changes
    ns.iblnErrorOnFocusOut = true;

    RegisterEvents();   
    ns.ReportPagePath = window.location.href.replace(window.location.hash, "") + "/";
 
    $("#CenterSplitter").scroll(function () {
        $('#ui-datepicker-div').css('display', 'none');
    });
    // 6.0.2.2 framework Changes Added support for displaying right aligned currency fields in the textbox
    //ns.iblnCurrencyRightAligned = true;

    // 6.0.3.0 Enhanced the UI functionality by freezing tool bar and breadcrumb from scrolling vertically
       ns.iblnFreezeBreadCrumToolBar = false;
    // ns.iblnHideCrumOnNonMaintenance=false; 
       
   
    //ns.LandingPage = "wfmESSHomeMaintenance";
    var astrFormID = $("#LandingPage").val();
    if (astrFormID === "") {
        astrFormID = "wfmESSActiveMemberHomeMaintenance";
    }

    ns.LandingPage = astrFormID;
    ns.iblnSetLandingPageFromInit = true;
    ns.viewModel.currentForm = ns.LandingPage;
    ns.viewModel.currentModel = ns.LandingPage;
    FrameworkInitilize();
    window.onerror = function (msg, url, linenumber) {
        nsUserFunctions.iblnMenuItemClicked = false;
        console.log(['Error message: ', msg, '\nURL: ', url, '\nLine Number: ', linenumber].join(''));
        ns.displayActivity(false);
        ns.blnLoading = false;
        ns.iblnBindingDialog = false;
        ns.istrDialogPanelID = "";
        ns.iblnBindingLeftForm = false;
        return true;
    };
    var MenuData = { MenuTemplate: ns.istrMenuTemplate };
    bindMenu(MenuData);
    //ns.BuildLeftForm("wfmBPMWorkflowCenterLeftMaintenance");
    kendo.bind($("#NotificationBar"), ns.NotificationModel.DirtyForms);
    if (DefaultMessages.MessageForChildNodeDelete) {
        DefaultMessages.MessageForChildNodeDelete = "This will close all pages except {0} \n  Do you want to proceed?";
    }// F/W Upgrade UAT PIR 12686 - Change Text On Alert Message. 
    if (DefaultMessages.ConfirmDeleteFormIfUnsaved) {
        DefaultMessages.ConfirmDeleteFormIfUnsaved = "There are unsaved changes, are you sure you want to remove the form(s)?";
    }
});

function bindMenu(data) {
    $("#cssmenu").html(data.MenuTemplate);    
    //var panelBar = $('#MenuUl').data('kendoPanelBar');
    //panelBar.expand($('#MenuUl li:first-child'), false);
    //cssmenu
    //$("#InitialPage").val(data.ExtraInfoFields["InitialPage"]);    
    //$("#uppAccordian").kendoPanelBar({
    //    expandMode: "multiple"
    //});
    //var panelBar = $('#uppAccordian').data('kendoPanelBar');
    //panelBar.expand($('#uppAccordian li:first-child'), false);

    ns.idictSpitter["MainSplitter"] = MVVM.Controls.Splitter.CreateInstance($("#MainSplitter"), "MainSplitter", {
        lstrOrientation: "vertical",
        larrPane: [
            { size: "110px", resizable: false },
            { resizable: false, scrollable: false },
            { size: "25px", resizable: false }
        ]
    });

    ns.idictSpitter[nsConstants.MIDDLE_SPLITTER] = MVVM.Controls.Splitter.CreateInstance($("#MiddleSplitter"), "MiddleSplitter", {
        lstrOrientation: "horizontal",
        larrPane: [
            //size: "280px",
            { resizable: false, collapsible: true, collapsed: false, scrollable: false },
            { resizable: true, scrollable: false }
        ]
    });
    $("[sfwCBPanel='true']").kendoPanelBar({});
    collapseAfterClickOnMenu();
}

//function LoadDashBoard() {
//    $("#DashboardList").sortable();

//    setTimeout(function (e) {
//        LoadDashBoardChart("wfmCorrChartLookup", "wfmCorTrackingLookup");
//    }, 500);

//    setTimeout(function (e) {
//        LoadDashBoardChart("wfmOrgChartLookup", "wfmOrganizationLookup");
//    }, 1000);
//}

//function LoadDashBoardChart(astrFormID, activeForm, defaultChartType) {
//    if (typeof defaultChartType === "undefined") { defaultChartType = "column"; }
//    $("#CenterSplitter").trigger('mousedown');

//    var data = ns.getTemplate(astrFormID, true);
//    var lstrTitle = ns.Templates[astrFormID].ExtraInfoFields.FormTitle;
//    var lstrContent = ns.Templates[astrFormID].Template;

//    //get html for dashboardItem
//    var NewItemHTML = "<div id='Parent" + astrFormID + "' style='width:380px; float:left'>";
//    NewItemHTML += "<div id='" + astrFormID + "'>";
//    NewItemHTML += "<ul controlType='panelbar' id='Panel" + astrFormID + "'>";
//    NewItemHTML += "<li class='k-state-active k-state-selected'>";
//    NewItemHTML += " <span>" + lstrTitle + "</span>";
//    NewItemHTML += "<div>";
//    NewItemHTML += "<input type='button' class='btnLookupChartConfig'>";
//    NewItemHTML += "<div class='TestChart' id='DashItem" + astrFormID + "'></div>";
//    NewItemHTML += "<div id='ChartCriteriaDiv" + astrFormID + "'></div>";
//    NewItemHTML += "</div>";
//    NewItemHTML += "</li>";
//    NewItemHTML += "</ul>";
//    NewItemHTML += "</div>";
//    NewItemHTML += "</div>";

//    var DashboardList = $("#DashboardList");

//    DashboardList = $("#DashboardList").append(NewItemHTML);

//    //$($(".PanelbarParent")[0]).append(NewItemHTML);
//    // $(".panel-container").kendoOBSortableGrid({});
//    // var NewPanelBar = $($(".PanelbarParent")[0]).find("#Panel" + astrFormID);
//    var ChartCriteriaDiv = DashboardList.find("#ChartCriteriaDiv" + astrFormID);

//    ChartCriteriaDiv.append("<div id='" + astrFormID + "'>" + lstrContent + "</div>").hide();

//    ns.Templates[astrFormID].HeaderData = kendo.observable(ns.Templates[astrFormID].HeaderData);

//    kendo.bind($("#DashBoard #" + astrFormID)[0], ns.Templates[astrFormID].HeaderData);
//    ns.Templates[astrFormID].ChartConfig;
//    ns.applyKendoUI("#DashBoard", astrFormID, astrFormID);

//    var options = {
//        Title: astrFormID,
//        FormID: astrFormID,
//        //Chart Axis
//        XAxisField: "",
//        YAxisField: "",
//        DefaultChartType: defaultChartType,
//        ChartTypes: ["column", "bar", "line"],
//        Height: 350,
//        Width: 350,
//        ChartCriteriaDivID: "ChartCriteriaDiv" + astrFormID,
//        ActiveForm: activeForm
//    };

//    var ChartDiv = DashboardList.find("#DashItem" + astrFormID);
//    ns.Templates[astrFormID].ChartConfig = ChartDiv.SagiChart(options).data("SagiChart");

//    //  setTimeout(function (e) {
//    $("#DashBoard #ChartCriteriaDiv" + astrFormID).find("input[value='Search']").trigger("click");
//    //}, 500);
//    // $('#Parent' + astrFormID).resizable();
//}

/* Added for framework version 6.0.0.18*****************************************************/
if (window['MVVMGlobal'] != undefined) {
    function RegisterEvents() {
        MVVMGlobal.RegisterEvents();
    }

    function FrameworkInitilize() {
        MVVMGlobal.FrameworkInitilize();
       // MVVMGlobal.CheckForSupportedBrowser();
    }

    nsCommon.SyncPost = nsRequest.SyncPost;

    function GetControlAttribute(astrControl, astrAttribute, astrActiveDivID) {
        return MVVMGlobal.GetControlAttribute(astrControl, astrAttribute, astrActiveDivID);
    }

    ns.getTemplate = nsRequest.getTemplate;

    function setRequestingForm(btnSelf) {
        MVVMGlobal.setRequestingForm(btnSelf);
    }

    function Extend_Custom(DivToApplyUI) {
        MVVMGlobal.Extend_Custom(DivToApplyUI);
    }

    function showDiv(astrDivID) {
        MVVMGlobal.showDiv(astrDivID);
    }
    function OnDeleteNodeClick(e) {
        nsEvents.OnDeleteNodeClick(e)
    }
}


//MVVMGlobal.AfterGetTemplateBase = MVVMGlobal.AfterGetTemplate;
//MVVMGlobal.AfterGetTemplate = function AfterGetTemplate(astrFormID, ablnCenterLeft, data, astrPostFix) {
//    var lstrTemplate;
//    if (data && data.Template && ns.SiteName != undefined && ns.SiteName != "" && ns.SiteName.toLowerCase() != "account") {
//        var lstrSiteName = ns.SiteName;
//        if (lstrSiteName.indexOf("/") == 0) {
//            lstrSiteName = lstrSiteName.replace("/", "");
//        }
//        lstrTemplate = data.Template;
//        var ldomDiv = $("<div></div>").html(lstrTemplate);
//        var allsrc = ldomDiv.find("[src^='" + lstrSiteName + "']");
//        if (allsrc.length > 0) {
//            for (var i = 0; i < allsrc.length; i++) {
//                var lstrSrc = allsrc[i].getAttribute("src");
//                if (lstrSrc != undefined && lstrSrc != "" && lstrSrc.indexOf(ns.SiteName) == 0) {
//                    lstrSrc = "/" + lstrSrc;
//                    allsrc[i].setAttribute("src", lstrSrc);
//                }
//            }
//            lstrTemplate = ldomDiv[0].innerHTML;
//            ldomDiv.remove();
//        }
//        data.Template = lstrTemplate;
//    }
//    return MVVMGlobal.AfterGetTemplateBase(astrFormID, ablnCenterLeft, data, astrPostFix);
//}

/* End - Added for framework version 6.0.0.18*****************************************************/

//Code added for collapse menu after click on left side menu item for ipad and below resolution devices.
function collapseAfterClickOnMenu() {
    if (ns.iblnIsMobileMedia) { // If media query matches
        $(".toggle-menu-inner-content").addClass("toggleForMobile")
        $("#btnHeaderSlideoutMenuDivCollapseExpand").trigger("click");
    }
    else {
        $(".toggle-menu-inner-content").removeClass("toggleForMobile")
        //$("#btnHeaderSlideoutMenuDivCollapseExpand").trigger("click");
    }
}