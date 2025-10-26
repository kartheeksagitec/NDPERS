nsConstants.MY_TASK_SELECTOR = "#divMyTaskBasketContainer"
$(function () {
    if (neo.elGridEmptyRow) {
        neo.elGridEmptyRow = neo.htmlToElement('<tr class="s-grid-empty-row"><td style="text-align:left"></td></tr>', "tbody");
    }
    ns.iblnDownloadFileInNewTab = true;
    //FM 6.0.10.0 - Added Support for Shortcut Keys on different forms which can also be configured at the form level.
    ns.iblnIsShortCutRequired = true;
    //FM 6.0.10.0 - Application Level Configuration provided for Showing Alert on Invalid Date.
    ns.iblnShowAlertForInvalidDate = true;
    //FM 6.0.10.0 - Application Level Configuration provided for Clearing the Invalid Date.
    ns.iblnClearValueForInvalidDate = true;    
    //FM 6.0.4.2 - Execution of constraints on focus out can be enabled by setting flag.
    ns.iblnErrorOnFocusOut = true;
    ns.iblnMobileGrids = false;
    ns.iblnOpenRefreshedForm = true;
    //FM 6.0.3.0 - Enhanced the UI functionality by freezing toolbar and breadcrumb from scrolling vertically.
    ns.iblnFreezeBreadCrumToolBar = true;
    ns.iblnHideCrumOnNonMaintenance = true;
    ns.iblnNavigateInNewMode = true; //FM 6.0.0.32 to override default behaviour of not allowing to navigate in new mode.
    nsCenterLeftRefresh.iblnShowMyBasketInCenterLeft = false;
    ns.iblnAddCustomButtonsToGridToolbar = true;
    nsRpt.iblnAddEmptyReportParams = true;
    //ns.iblnShowHardErrorAssociatedToControl = true;
    //ns.iblnShowHardErrorAssociatedToGridControl = true;
    ns.iblnHighlightAllErrorControls = true;
    ns.iblnNonCollapsiblePanels = false;

    ns.iblnPrintAllPagesOnLookup = false;
    ns.iintPrintMaxWidth = 1248;
    ns.iintGridColumnPrintMaxWidth = 120;
    nsVisi.iblnHideParentOnVisibility = false;

    if (ns.iblnADATesting) { return false; }
    ns.blnLoading = true;
    ns.blnUseSlideoutForLookup = false;
    RegisterEvents();
    ns.ReportPagePath = window.location.href.replace(window.location.hash, "") + "/";
    nsCorr.UseLocalTool = true;
    ns.iblnHidePagesFromGridPaging = true;
    ns.iblnImagesForPaging = true;
    nsCenterLeftRefresh.iblnShowMyBasketInCenterLeft = true;
  //  ns.iobjCenterLeftContainers[nsConstants.KNOWTION_CENTERLEFT_MAINTENANCE] = "KnowtionQuickSearch";

    //FM 6.0.10.0- To display Message on clicking the Delete Button without selecting any record
    DefaultMessages.NoRowSelectedforGridViewDelete = "No rows were selected.  Please select the row(s) to be deleted.";
    ns.CenterLeftPanelBar = $("#CenterLeft" + " ul[controlType='accordian']").kendoPanelBar({
        expandMode: "single"
    }).data("kendoPanelBar");

    if (ns.blnUseSlideoutForLookup) {

        $('#SlideOutLookup').slidePanel({
            triggerName: '#SearchTriger',
            position: 'fixed',
            triggerTopPos: '28px',
            panelTopPos: '87px',
            panelOpacity: 1,
            clickOutsideToClose: true,
            speed: "slow"
        });
    }
    else {
        $("#SearchTriger").hide();
        $("#crumDiv").after($("#LookupName"));
    }

    $('#SlideOutTree').slidePanel({
        triggerName: '#navTreeTriger',
        position: 'fixed',
        triggerTopPos: '22px',
        panelTopPos: '100px',
        panelOpacity: 1,
        clickOutsideToClose: true,
        speed: "slow"
    });
    $('#SlideOutSecurityTree').slidePanel({
        triggerName: '#SecurityTree',
        position: 'fixed',
        triggerTopPos: '22px',
        panelTopPos: '480px',
        panelOpacity: 1,
        clickOutsideToClose: true,
        speed: "slow"
    });
    //Need to remove in next FM release
    //SetAuditInfoOnTopTemplate();
    ns.LandingPage = ($("#LandingPage").length > 0 && $.trim($("#LandingPage").val()) != "" ? $("#LandingPage").val() : "wfmPersonLookup");
    ns.viewModel.currentForm = ns.LandingPage;
    ns.viewModel.currentModel = ns.LandingPage;
    ns.iblnSkipWhiteSpacesFromSearch = true;
    nsCorr.WindowWidth = "1200px";
    nsCorr.WindowHeight = "600px";
    var lobjData = nsCommon.sessionGet(["TreeViewCustomDataSource", ns.SiteName].join('')) === null ? [] : nsCommon.sessionGet(["TreeViewCustomDataSource", ns.SiteName].join(''));
    ns.CustomTabsTree = MVVM.Controls.TreeView.CreateInstance($("#CustomTabsTree"), { data: lobjData, iblnDoNotSetData: true, istrNodePrefix: "sli_" });
    FrameworkInitilize();
    window.onerror = function (msg, url, linenumber) {
        nsUserFunctions.iblnMenuItemClicked = false;
        nsUserFunctions.idteLastRequestCompletedTime = new Date();
        console.log(['Error message: ', msg, '\nURL: ', url, '\nLine Number: ', linenumber].join(''));
        ns.displayActivity(false);
        ns.blnLoading = false;
        ns.iblnBindingDialog = false;
        ns.istrDialogPanelID = "";
        ns.iblnBindingLeftForm = false;
        var mygeneDiv = $('#myGeneDisplay');
        if (mygeneDiv.length > 0) {
            mygeneDiv.hide();
        }
        return true;
    };
    var MenuData = { MenuTemplate: ns.istrMenuTemplate };
    bindMenu(MenuData);
    ns.BuildLeftForm("wfmBPMWorkflowCenterLeftMaintenance");
    nsUserFunctions.BuildLeftFormById("wfmCenterLeftPersonLookup", "#PersonQuickSearch");
    nsUserFunctions.BuildLeftFormById("wfmCenterLeftOrganizationLookup", "#OrganizationQuickSearch");

    kendo.bind($("#NotificationBar"), ns.NotificationModel.DirtyForms);
    //This property sets the Correcpondence Dialogue title.
    nsCorr.WindowTitle = "Communication";
    SessionEvents.iintSessionRemainingTimer = 120; //Show session popup 2 minutes before session times out
    var control = document.getElementById("ShowSecurityMessage");
    var securitydiv = document.getElementById("SecurityTree");
    if (securitydiv != undefined) {
        if (control != undefined && control.value == 'True') {
            securitydiv.style.display = 'block';
        }
        else {
            securitydiv.style.display = 'none';
        }
    }
});

function bindMenu(data) {
    $("#cssmenu").html(data.MenuTemplate);    
    var ldomMyTaskBasket = document.getElementById(nsConstants.MY_TASK_DIV_CONTAINER);
    if (ldomMyTaskBasket != null) {
        ldomMyTaskBasket.style.display = "none";
        $(ldomMyTaskBasket).find(".my-task-panel[controltype='panelbar']").hide();
        $(ldomMyTaskBasket).find(".my-task-panel[controltype='panelbar']:first").show();
        ns.RenderPanelBar($(ldomMyTaskBasket), "body", nsConstants.MY_TASK_DIV_CONTAINER, nsConstants.MY_TASK_DIV_CONTAINER, nsConstants.MY_TASK_DIV_CONTAINER, false, false, {});
    }
    ns.idictSpitter["MainSplitter"] = MVVM.Controls.Splitter.CreateInstance($("#MainSplitter"), "MainSplitter", {
        lstrOrientation: "vertical",
        larrPane: [
            { size: "104px", resizable: false },
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
    ns.idictSpitter["CenterMiddle"] = MVVM.Controls.Splitter.CreateInstance($("#CenterMiddle"), "CenterMiddle", {
        lstrOrientation: "horizontal",
        larrPane: [
            { resizable: false, collapsible: false, size: "15px", scrollable: false },
            { resizable: true, scrollable: true },
            { resizable: true, collapsible: true, collapsed: true, scrollable: true }
        ]
    });

    $("[sfwCBPanel='true']").kendoPanelBar({});

    var temp = selectnav('MenuUl', {
        label: '### Table of content ### ',
        nested: true,
        indent: '--'
    });

    $("#selectnav1").on("change", function (e) {
        if ($(this).val() !== "")
            $("#cssmenu").find("li[formid='" + $(this).val() + "']").trigger("click");
    });
}

function CreateCustomInstance() {
    ns.tabsTreeView.push = function (aobjDataItem, aobjParentDataItem, ablnFromSelect) {
        var customDataObject = ns.CustomTabsTree.getNodeDataByDivID(aobjDataItem.divID);
        if (customDataObject != undefined) {
            ablnFromSelect = true;
        }
        var lobjCustomChildObject = neo.Clone(aobjDataItem);
        var instance = ns.tabsTreeView;
        var lobjData = aobjDataItem.data != null ? aobjDataItem.data : aobjDataItem;
        var lobjTreeNode = instance.createNode(lobjData);
        var lstrPosition = "first";
        //if (lobjTreeNode.data.divID == nsConstants.BPM_WORKFLOW_CENTERLEFT_MAINTENANCE) {
        //    lstrPosition = "first";
        //}
        if (lobjTreeNode.data.divID != nsConstants.BPM_WORKFLOW_CENTERLEFT_MAINTENANCE) {
            instance.jsObject.deselect_all();
        }
        instance.jsObject.create_node(null, lobjTreeNode, lstrPosition, function () {
        }, false);
        if (lobjData.navParams != undefined)
            nsCommon.sessionSet([lobjData.divID, "_navParams"].join(''), lobjData.navParams);
        if (ablnFromSelect !== true) {
            if (aobjParentDataItem != undefined && aobjParentDataItem.uid.indexOf("sli_") !== 0) {
                var customParentDataObject = ns.CustomTabsTree.getNodeDataByDivID(aobjParentDataItem.divID);
                if (customParentDataObject != undefined)
                    aobjParentDataItem = customParentDataObject;
            }
            ns.CustomTabsTree.push(lobjCustomChildObject, aobjParentDataItem, ablnFromSelect);
        }
    }
    ns.tabsTreeView.onSelectBase = MVVM.JQueryControls.TreeView.prototype.onSelect;
    ns.tabsTreeView.onSelect = function (e, data) {
        ns.tabsTreeView.onSelectBase(e, data);
        if (data != undefined) {
            var dataItem = data.node.data;
            ns.tabsTreeView.remove(dataItem);
            ns.tabsTreeView.push(dataItem, undefined, true);
        }
    }
    
    ns.CustomTabsTree.findByUid = function (astrUid) {
        if (astrUid != undefined && typeof astrUid == "string" && !astrUid.startsWith('s')) {
            astrUid = ["s", astrUid].join('');
        }
        var instance = this;
        if (instance.jsObject != undefined) {
            var lstrDivID = astrUid.indexOf(instance.istrNodePrefix) == 0 ? astrUid : [instance.istrNodePrefix, astrUid].join('');
            var ldomTreeLink = instance.jsObject.get_node(lstrDivID, true);
            if (ldomTreeLink && ldomTreeLink.length > 0) {
                return $(ldomTreeLink[0]);
            }
            else {
                instance.jsObject._open_to(lstrDivID);
                var ldomTreeLink = instance.jsObject.get_node(lstrDivID, true);
                if (ldomTreeLink && ldomTreeLink.length > 0) {
                    return $(ldomTreeLink[0]);
                }
                return $();
            }
        }
        else
            return $();
    };
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

//    kendo.bind($("#wfmDashBoard #" + astrFormID)[0], ns.Templates[astrFormID].HeaderData);
//    ns.Templates[astrFormID].ChartConfig;
//    ns.applyKendoUI("#wfmDashBoard", astrFormID, astrFormID);

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
//    $("#wfmDashBoard #ChartCriteriaDiv" + astrFormID).find("input[value='Search']").trigger("click");
//    //}, 500);
//    // $('#Parent' + astrFormID).resizable();
//}

if (window['MVVMGlobal'] != undefined) {
    function RegisterEvents() {
        MVVMGlobal.RegisterEvents();
    }

    function FrameworkInitilize() {
        MVVMGlobal.FrameworkInitilize();
        MVVMGlobal.CheckForSupportedBrowser();
    }

    nsCommon.SyncPost = nsRequest.SyncPost;

    function GetControlAttribute(astrControl, astrAttribute, astrActiveDivID) {
        return MVVMGlobal.GetControlAttribute(astrControl, astrAttribute, astrActiveDivID);
    }

    ns.getTemplate = nsRequest.getTemplate;

    function setRequestingForm() {
        MVVMGlobal.setRequestingForm();
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

function person() {    
    document.getElementById('OrganizationQuickSearch').style.display = "none";
    if ($("#OrganizationQuickSearch").parent().find("span").find("span.k-icon").hasClass("k-icon k-i-arrow-n k-panelbar-collapse")) {
        $("#OrganizationQuickSearch").parent().find("span").find("span.k-icon").removeClass("k-i-arrow-n k-panelbar-collapse");
        $("#OrganizationQuickSearch").parent().find("span").find("span.k-icon").addClass("k-i-arrow-s k-panelbar-expand");
    }
}

function organization() {    
    document.getElementById('PersonQuickSearch').style.display = "none";
    if ($("#PersonQuickSearch").parent().find("span").find("span.k-icon").hasClass("k-icon k-i-arrow-n k-panelbar-collapse")) {
        $("#PersonQuickSearch").parent().find("span").find("span.k-icon").removeClass("k-i-arrow-n k-panelbar-collapse");
        $("#PersonQuickSearch").parent().find("span").find("span.k-icon").addClass("k-i-arrow-s k-panelbar-expand");
    }
}
MVVMGlobal.SetSPARoutingBase = MVVMGlobal.SetSPARouting;
MVVMGlobal.SetSPARouting = function () {
    CreateCustomInstance();
    MVVMGlobal.SetSPARoutingBase();

}

MVVM.JQueryControls.TreeView.prototype.onSelect = function (e, data) {
    var instance = this.id == "TabsTree" ? ns.tabsTreeView : ns.CustomTabsTree;
    ns.blnFromTreeview = false;
    if (data.node.data.divID != nsConstants.BPM_WORKFLOW_CENTERLEFT_MAINTENANCE) {
        MVVMGlobal.StoreTreeViewInSessionStore();
    }
    var lobjInfo = ns.GetSessionStoredInfo(data.node.data.divID);
    if (lobjInfo == null) {
        var ldomControl = $("#" + nsConstants.PANEL_COLLAPSE_ALL_BUTTON_ID);
        nsCommon.SetTextForCollapseAllButton(false, ldomControl);
    }
    else {
        nsCommon.SetTextForCollapseAllButtonFromTreeView(lobjInfo, data.node.data.divID);
    }
    if (ns.blnFromDeleteTreeNode) {
        ns.blnFromDeleteTreeNode = false;
        e.preventDefault();
    }
    if (instance.iblnTriggerSelect == undefined || instance.iblnTriggerSelect == false) {
        e.preventDefault();
        ns.tabsTreeView.iblnTriggerSelect = true;
        instance.iblnTriggerSelect = true;
        return;
    }
    if (ns.CanDisplayParentForm == false) {
        ns.CanDisplayParentForm = true;
        return;
    }
    var dataItem = instance.dataItem(data.selected[0]);
    if (!nsCommon.CanNavigateTo(dataItem.divID)) {
        nsCommon.DispalyError(DefaultMessages.NoNavigationOnUnsavedFormLimit);
        return;
    }
    ns.SenderID = "formNavigationTreeNode";
    if (dataItem.divID.indexOf(nsConstants.LOOKUP) > 0) {
        var FormContainer = nsConstants.CONTENT_SPLITTER_SELECTOR;
        if (ns.blnUseSlideoutForLookup) {
            FormContainer = nsConstants.LOOKUP_HOLDER_SELECTOR;
        }
        if ($([FormContainer, nsConstants.SPACE_HASH, dataItem.divID].join('')).length == 0) {
            ns.LookupWasNotInDiv = true;
            ns.viewModel.currentForm = dataItem.formID;
            ns.viewModel.currentModel = dataItem.divID;
            MVVMGlobal.UpdateUrl(dataItem.formID, 0);
            ns.buildView(dataItem.formID, false);
            ns.activityComplete();
            return;
        }
        else {
            if (ns.iblnHasKnowtionSearch) {
                var lstrFormId = nsCommon.GetProperFormName(dataItem.formID);
                if (nsCommon.sessionGet("FMknowtionSearchFormId") !== lstrFormId) {
                    nsCommon.BindKnowtionForm(lstrFormId);
                }
            }
            ns.viewModel.currentForm = dataItem.formID;
            ns.viewModel.currentModel = dataItem.divID;
        }
        if (dataItem.divID !== ns.activeLookup.divID)
            MVVMGlobal.hideDiv([FormContainer, nsConstants.SPACE_HASH, ns.activeLookup.divID].join(''));
        MVVMGlobal.showDiv([FormContainer, nsConstants.SPACE_HASH, dataItem.divID].join(''));
        if (ns.blnUseSlideoutForLookup == false) {
            ns.FormOpenedOnLeft = dataItem;
        }
        MVVMGlobal.UpdateUrl(dataItem.formID, 0);
        ns.Templates[dataItem.divID].HeaderData = MVVM.ServiceLoad.GetObservable(ns.Templates[dataItem.divID].HeaderData);
        if ($([FormContainer, nsConstants.SPACE_HASH, dataItem.divID].join('')).length > 0) {
            nsCommon.ApplyBindingToForm($([FormContainer, nsConstants.SPACE_HASH, dataItem.divID].join('')), ns.Templates[dataItem.divID].HeaderData);
        }
        var fnSetTimeout = function () {
            ns.PositionCursor([nsConstants.HASH, dataItem.divID].join(''));
        };
        setTimeout(fnSetTimeout, 10);
        MVVMGlobal.setLookupFormTitle(dataItem.title);
        ns.activeLookup = dataItem;
        if (ns.lblnCanSetLookupParams) {
            ns.lblnCanSetLookupParams = false;
        }
        else {
            ns.activityComplete();
        }
        MVVMGlobal.LoadLookupNames();
        ns.refreshSession();
        if (ns.blnUseSlideoutForLookup && ($(nsConstants.SLIDEOUT_LOOKUP_SELECTOR).length > 0 && $(nsConstants.SLIDEOUT_LOOKUP_SELECTOR)[0].style.display === "none" || $(nsConstants.SLIDEOUT_LOOKUP_SELECTOR)[0].style.display === "")) {
            $("#SearchTriger").trigger('click');
        }
    }
    else {
        ns.blnFromTreeview = true;
        ns.blnCanTriggerTreeSelect = false;
        ns.isRightSideForm = false;
        if ($([nsConstants.HASH, dataItem.divID].join("")).length > 0) {
            ns.refreshSession();
        }
        try {
            if (dataItem.divID === nsConstants.BPM_WORKFLOW_CENTERLEFT_MAINTENANCE) {
                $("#MyBasketTriger").trigger('click');
                ns.blnFromTreeview = false;
            }
            else if (dataItem.divID === nsConstants.KNOWTION_CENTERLEFT_MAINTENANCE) {
                ns.blnFromTreeview = false;
            }
            else if (ns.iarrCenterLeftForms != undefined && ns.iarrCenterLeftForms.indexOf(dataItem.divID) >= 0) {
                ns.blnFromTreeview = false;
            }
            else {
                MVVMGlobal.OpenFormOnLeft(dataItem);
                ns.blnFromTreeview = false;
            }
        }
        finally {
            ns.blnCanTriggerTreeSelect = true;
        }
        if (ns.blnUseSlideoutForLookup && ($(nsConstants.SLIDEOUT_LOOKUP_SELECTOR).length > 0 && $(nsConstants.SLIDEOUT_LOOKUP_SELECTOR)[0].style.display === "block")) {
            $("#SearchTriger").trigger('click');
        }
    }
   
    //if (data.event.hasOwnProperty("currentTarget")) {
    //sli_ when click on delete button
    if (!data.node.id.startsWith("sli_")) {

        //current visited at top in navigator
        var dataItem = data.node.data;
        ns.tabsTreeView.remove(dataItem);
        ns.tabsTreeView.push(dataItem, undefined, true);

        ////last visited at top in navigator
        //var previousDataItem = ns.tabsTreeView.getNodeDataByDivID(data.node.data.previousForm);
        //ns.tabsTreeView.remove(previousDataItem);
        //ns.tabsTreeView.push(previousDataItem, undefined, true);
        //console.log(previousDataItem);
    }
    ClearCommentsOnDetailLookup();
    e.preventDefault();
    e.stopPropagation();
};
MVVM.JQueryControls.TreeView.prototype.dataItem = function (aobjNode) {
    var customTabsTreeInst = this;
    var flag = this.id == "TabsTree" ? true : false;
        if (aobjNode != undefined && typeof aobjNode == "string") {
            if (!flag && !aobjNode.charAt(0) == 's') {
                aobjNode = ["s", aobjNode].join('');
            }
            return customTabsTreeInst.getNodeDataByDivID(aobjNode);
        }
        if (aobjNode != undefined && aobjNode.length > 0) {
            var nodeID = aobjNode[0].id;
            if (!flag && !nodeID.startsWith('s'))
            {
                nodeID = ["s", nodeID].join('');
            }
            return customTabsTreeInst.getNodeDataByDivID(nodeID);
        }
        return undefined;
};

if (!window.getClientRects) {
    window.getClientRects = function () {
        return [];
    }
}