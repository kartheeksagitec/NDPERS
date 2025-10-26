/// <reference path="reference.js" />
/// <reference path="UserDefinedFunctions.js" />
$(function () {
    ns.iblnAddCustomButtonsToGridToolbar = false;
    ns.iblnShowHardErrorAssociatedToControl = false;
    ns.iblnShowHardErrorAssociatedToGridControl = false;
    ns.iblnHighlightAllErrorControls = true;
    //ns.iblnNonCollapsiblePanels = true;
    if (ns.iblnADATesting) { return false; }
    ns.blnLoading = true;
    ns.blnUseSlideoutForLookup = false;
    ns.iblnNavigateInNewMode = true;
    ns.iblnShowGridCommonFilterBox = false;
    ns.iblnShowGridSettings = false;
    //ns.iblnOpenRefreshedForm = true;
    //Code for stop collapsable pannel
    ns.iblnNonCollapsiblePanels = true;
    ns.iblnShowAlertForInvalidDate = true;
    ns.iblnClearValueForInvalidDate = true;
    //6.0.4.2 Changes
    ns.iblnErrorOnFocusOut = true;
    ns.iblnDownloadFileInNewTab = true;
    ns.blnSkipConfirmationForDeleteOrNew = true;

    //F/W Upgrade : PIR - 27050,27051 Control Alignment Issues
    nsVisi.iblnHideParentOnVisibility = false;

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

    
    var astrFormID = $("#LandingPage").val();
    var lstrRetiree = $("#IsRetiree").val();
    var lstrExternalUser = $("#IsExternalUser").val();
    var lstrPersonCertify = $("#PersonCertify").val();
    var lstrEmailWaiver = $("#IsEmailFlagWaiver").val();

    ns.LandingPage = astrFormID;
    ns.lstrRetiree = lstrRetiree;
    ns.lstrExternalUser = lstrExternalUser;
    ns.lstrPersonCertify = lstrPersonCertify;
    ns.lstrEmailWaiver = lstrEmailWaiver;
    ns.iblnSetLandingPageFromInit = true;
    ns.viewModel.currentForm = ns.LandingPage;
    ns.viewModel.currentModel = ns.LandingPage;
    FrameworkInitilize();
    window.onerror = function (msg, url, linenumber) {
        nsUserFunctions.iblnMenuItemClicked = false;
        console.log(['Error message: ', msg, '\nURL: ', url, '\nLine Number: ', linenumber].join(''));
        ns.displayActivity(false);
        if ($("#Custom-loader").length > 0)
            $("#Custom-loader").hide();
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
    if (ns.lstrPersonCertify == "False" && ns.lstrEmailWaiver == "N") {
        $("li a[id = 'lnkMssHome']").css("pointer-events", "none");
        $("li a[id = 'lnkMssForms']").css("pointer-events", "none");
        $("li a[id = 'lnkContactUs']").css("pointer-events", "none");
        $("li a[title = 'Previous']").css("pointer-events", "none");        
        var IsEmailAddressWaived = DisplayMessageBox();
    }
});

function bindMenu(data) {
    $("#cssmenu").html(data.MenuTemplate);
   

    ns.idictSpitter[nsConstants.MIDDLE_SPLITTER] = MVVM.Controls.Splitter.CreateInstance($("#MiddleSplitter"), "MiddleSplitter", {
        lstrOrientation: "horizontal",
        larrPane: [
            //size: "280px",
            { resizable: false, collapsible: true, collapsed: false, scrollable: false },
            { resizable: true, scrollable: false }
        ]
    });
    $("[sfwCBPanel='true']").kendoPanelBar({});
    
    $("li[class='menuItem']>a").on("click", function (e) {   
        if (ns.lstrPersonCertify == "False" && ns.lstrEmailWaiver == "N") {
            var IsEmailAddressWaived = DisplayMessageBox();
            return false;
        }
        if ($("#popUpDiv").length > 0) 
            $("#popUpDiv").dialog("close");
            return true;
    });

    collapseAfterClickOnMenu();
    
}

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

    function DisplayMessageBox() {            
        var ActiveDivID = $("#" + ns.viewModel.currentModel);
        var ErrorDialogWindow = $("#DisplayMessageBoxtext");
        ErrorDialogWindow.show();
        if (ns.arrDialog['DisplayMessageBoxtext'] === undefined) {
            ns.arrDialog['DisplayMessageBoxtext'] = MVVM.Controls.Dialog.CreateInstance(ErrorDialogWindow, (ActiveDivID == undefined ? "" : ActiveDivID), {
                title: "Confirm",
                height: "150px",
                width: "200px",
                actions: [],
                close: function () { },
                deactivate: function () { },
                blnAlignCenter: true,
                dialogName: 'Confirm'
            });
            ns.arrDialog['DisplayMessageBoxtext'].open();
        }
        else {
            ns.arrDialog['DisplayMessageBoxtext'].open();
        }
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