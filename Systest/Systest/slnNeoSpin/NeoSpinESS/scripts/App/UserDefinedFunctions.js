var nsUserFunctions = nsUserFunctions || {};
//ns.AutoLoginProp = 'lblOrgId';
//ns.AutoLoginUID = 'lblEssUserId';
//ns.AutoLoginPwd = 'lblPassword';
nsUserFunctions = {
    iblnMenuItemClicked : false,
    // Framework version 6.0.0.30.0
    ApplyUserDefinedFormat: function (e) {
        //e.context = {             
        //    DataFormat: astrFormat, //Format                 
        //    Value: astrValue, //value in case of label                
        //    Control: adomControl, // Control in case of editable textbox              
        //    IsLabel: ablnIsLabel,  //if formating control is label                
        //    FormatedValue: astrFormatedValue, // Framework formated value in case of label               
        //    IsFromGrid: ablnFromGrid  //if called from the grid for editable controls    
        //}
        return null;
    },

    SetLanguage: function (e) {
        // add solution side logic to get language.  
        var lstrLanguage = 'en-US';
        return lstrLanguage;
    },

    // Framework version 6.0.0.28.0
    //ClearReassignTreeNode: function (e) {
    //    //ns.tabsTreeView.remove("wfmBpmReassignWorkMaintenance0");
    //    var dataitem = ns.tabsTreeView.getNodeDataByDivID("wfmBpmReassignWorkMaintenance0");
    //    if (dataitem != undefined)
    //        MVVMGlobal.RemoveForm([], dataitem);
    //    return true;
    //},

    //// Framework version 6.0.2.1
    //GetChartTitle: function (e) {
    //    e.data 		// DomainModel Object
    //    e.chartId 	// Id of the chart

    //    if (e.chartId == "chrtPayrollDetailsStatus") {
    //        return " ";
    //    }
    //    else
    //        return "Dyanamic Title";
    //},
    //  FMUpgrade:Added for Default page conversion of OnLoadComplete method
    InitilizeUserDefinedEvents: function (e) {
        var ldomDiv = $("#" + ns.viewModel.currentModel);
        $(document).off("change.ddlPayrollReport").on('change.ddlPayrollReport', "#ddlPayrollReport", function (e) {
            if (ns.viewModel.currentModel.indexOf("wfmCreatePayrollMaintenance") === 0) {
                fnddlPayrollReportToggleVisibilty(ldomDiv);
            }
        });
        $(document).off("change.ddlBenefitType").on('change.ddlBenefitType', "#ddlBenefitType", function (e) {
            if (ns.viewModel.currentModel.indexOf("wfmCreatePayrollMaintenance") === 0) {
                fnddlBenefitTypeToggleVisibilty(ldomDiv);
            }
        });
        $(document).off("blur.dtpNewEligibleFromDate").on('blur.dtpNewEligibleFromDate', "#dtpNewEligibleFromDate", function (e) {
            if (ns.viewModel.currentModel.indexOf("wfmEssACAEligibilityCertificationMaintenance") === 0) {
                PopulateToDateAcaCert();
            }
        });
        //Conversion of LOAVisibility() under NSHeader.js 
        $(document).off("change.ddlHealth").on('change.ddlHealth', "#ddlHealth", function (e) {
            var ldomDiv = $("#" + ns.viewModel.currentModel);
            var lddlHealth = $("#ddlHealth");
            if (ns.viewModel.currentModel.indexOf("wfmUpdateTermEmploymentLOAWithoutPayWizard") === 0) {
                if (lddlHealth != null) {
                    LOAVisibility(ldomDiv);
                }
            }
        });
        $(document).off("change.ddlDental").on('change.ddlDental', "#ddlDental", function (e) {
            var ldomDiv = $("#" + ns.viewModel.currentModel);
            var lddlDental = $("#ddlDental");
            if (ns.viewModel.currentModel.indexOf("wfmUpdateTermEmploymentLOAWithoutPayWizard") === 0) {
                if (lddlDental != null) {
                    LOAVisibility(ldomDiv);
                }
            }
        });
        $(document).off("change.ddlVision").on('change.ddlVision', "#ddlVision", function (e) {
            var ldomDiv = $("#" + ns.viewModel.currentModel);
            var lddlVision = $("#ddlVision");
            if (ns.viewModel.currentModel.indexOf("wfmUpdateTermEmploymentLOAWithoutPayWizard") === 0) {
                if (lddlVision != null) {
                    LOAVisibility(ldomDiv);
                }
            }
        });
        $(document).off("change.ddlLife").on('change.ddlLife', "#ddlLife", function (e) {
            var ldomDiv = $("#" + ns.viewModel.currentModel);
            var lddlLife = $("#ddlLife");
            if (ns.viewModel.currentModel.indexOf("wfmUpdateTermEmploymentLOAWithoutPayWizard") === 0) {
                if (lddlLife != null) {
                    LOAVisibility(ldomDiv);
                }
            }
        });
        $(document).off("change.ddlEAP").on('change.ddlEAP', "#ddlEAP", function (e) {
            var ldomDiv = $("#" + ns.viewModel.currentModel);
            var lddlEAP = $("#ddlEAP");
            if (ns.viewModel.currentModel.indexOf("wfmUpdateTermEmploymentLOAWithoutPayWizard") === 0) {
                if (lddlEAP != null) {
                    LOAVisibility(ldomDiv);
                }
            }
        });
        $(document).off("change.ddlLTC").on('change.ddlLTC', "#ddlLTC", function (e) {
            var ldomDiv = $("#" + ns.viewModel.currentModel);
            var lddlLTC = $("#ddlLTC");
            if (ns.viewModel.currentModel.indexOf("wfmUpdateTermEmploymentLOAWithoutPayWizard") === 0) {
                if (lddlLTC != null) {
                    LOAVisibility(ldomDiv);
                }
            }
        });

        $(document).on("click", "#ExportChkSelectAll, #ExportChkClearAll", function () {
            if (this.id === "ExportChkSelectAll") {
                CheckUnCheckExportCols(true);
            }
            else if (this.id === "ExportChkClearAll") {
                CheckUnCheckExportCols(false);
            }
        });
        $(document).on("click", "#btnCertifyButtonClick", function () {
            var ldomDiv = $("#" + ns.viewModel.currentModel);
            if (ns.viewModel.currentModel.indexOf("wfmEssACAEligibilityCertificationMaintenance") === 0) {
                ldomDiv.find("#capMetReq").hide();
                ldomDiv.find("#ddlMetReq").hide();

            }
        });
        var width = $(window).width();

            $(document).on("click", ".web-view-head", function () {

                if ($(".web-view-head").css('left') == "-262px") {
                    $(".web-view-head").css("left", "10px");
                    $(".crumDiv").css("margin-left", "30px");
                }
                else {
                var width1 = $(window).width();
                if (width1 >= 1025) {
                    $(".web-view-head").css("left", "-262px");
                    $(".crumDiv").css("margin-left", "0px");
                }
                else if (width1 <= 1024) {
                    $(".web-view-head").css("left", "10px");
                    $(".crumDiv").css("margin-left", "30px");

                }
        }
        });

        $(document).off("change.ddlFlex").on('change.ddlFlex', "#ddlFlex", function (e) {
            var ldomDiv = $("#" + ns.viewModel.currentModel);
            var lddlFlex = $("#ddlFlex");
            if (ns.viewModel.currentModel.indexOf("wfmUpdateTermEmploymentLOAWithoutPayWizard") === 0) {
                if (lddlFlex != null) {
                    LOAVisibility(ldomDiv);
                }
            }
        });

        //Code added for collapse menu after click on left side menu item for ipad and below resolution devices.
        $(document).on("click", ".toggleForMobile .sub-menu li a", function (e) {
            $("#btnHeaderSlideoutMenuDivCollapseExpand").trigger("click");
        });

        //Conversion of SeasonalVisibility() under NSHeader.js 
        $(document).off("change.ddl12Months").on('change.ddl12Months', "#ddl12Months", function (e) {
            var ldomDiv = $("#" + ns.viewModel.currentModel);
            var lddl12Months = $("#ddl12Months");
            if (ns.viewModel.currentModel.indexOf("wfmUpdateTermEmploymentWizard") === 0) {
                if (lddl12Months != null) {
                    SeasonalVisibility(ldomDiv);
                }
            }
        });

        //  FMUpgrade:Added for Default page conversion of btn_OpenEssForms method
        $(document).on('click', "a[class='OpenHyperlinkDownload']", function (e) {
            if (ns.viewModel.currentModel.indexOf("wfmESSFormMaintenance") === 0 || ns.viewModel.currentModel.indexOf("wfmESSOrganizationMaintenance") === 0 || ns.viewModel.currentModel.indexOf("wfmESSContactMaintenance") === 0 || ns.viewModel.currentModel.indexOf("wfmCreatePayrollMaintenance") === 0 || ns.viewModel.currentModel.indexOf("wfmESSActiveNoEmployerPayrollHeaderMaintenance") === 0 || ns.viewModel.currentModel.indexOf("wfmESSActiveEmployerPayrollHeaderMaintenance") === 0) {
                var lcapLabel = $($(this).next()).text();
                window.open(ns.SiteName + "/Home" + "/" + "OpenESSForms?astrFileName=" + lcapLabel + "&aintESSFlag=0", "_blank", "width=800,height=800,resizable=yes,toolbar=yes,scrollbars=yes,alwaysRaised=yes");
            }
        });
        $(window).off("beforeunload.neoCustomWindowEvents").on("beforeunload.neoCustomWindowEvents", function (e) {
            if (ns.viewModel.currentModel != undefined && (ns.viewModel.currentModel.indexOf("wfmESSEmployerPayrollHeaderLookup") == 0 || ns.viewModel.currentModel.indexOf("wfmESSCombinedPayrollDetailLookup") == 0 || ns.viewModel.currentModel.indexOf("wfmESSEmployerPayrollHeaderHomeLookup") == 0 ||
                ns.viewModel.currentModel.indexOf("wfmESSRetirementEmployerPayrollHeaderLookup") === 0 || ns.viewModel.currentModel.indexOf("wfmESSInsuranceEmployerPayrollHeaderLookup") === 0 || ns.viewModel.currentModel.indexOf("wfmESSDefferedCompEmployerPayrollHeaderLookup") === 0 ||
                ns.viewModel.currentModel.indexOf("wfmESSServicePurchaseEmployerPayrollHeaderLookup") === 0) || ns.viewModel.currentModel.indexOf("wfmESSRetirementCombinedPayrollDetailLookup") == 0 || ns.viewModel.currentModel.indexOf("wfmInsuranceESSCombinedPayrollDetailLookup") == 0 || ns.viewModel.currentModel.indexOf("wfmESSDeferredCompCombinedPayrollDetailLookup") == 0 ||
                 ns.viewModel.currentModel.indexOf("wfmESSServicePurchaseCombinedPayrollDetailLookup") == 0) {
                if (ns.Templates[ns.viewModel.currentModel].HeaderData != null && ns.Templates[ns.viewModel.currentModel].HeaderData != undefined) {
                    var searchParams = ns.Templates[ns.viewModel.currentModel].HeaderData["tblCriteria"];
                    if (searchParams != null && searchParams != undefined) {
                        if (searchParams.toJSON)
                            searchParams = searchParams.toJSON();
                        var larrRows = [searchParams];
                        nsCommon.sessionSet(ns.viewModel.currentModel, larrRows);
                        nsCommon.sessionSet(ns.viewModel.currentModel + "_Refresh", "true");
                    }

                }
            }

        });
        $(document).on('click', "#btnRefreshData", function (e) {
            if (ns.viewModel.currentModel.indexOf("wfmESSEmployerPayrollHeaderMaintenance") === 0) {
                var ldomDiv = $("#" + ns.viewModel.currentModel);
                var txtContactNDPERSFlag = ldomDiv.find('#txtContactNDPERSFlag').val();
                if(txtContactNDPERSFlag == "true")
                    alert('Please contact NDPERS to post this report.');
                return false;
            }            
        });

        $(document).on('click', "#btnSave", function (e) {      
            var Txtservicecredit = $("#" + ns.viewModel.currentModel).find("#lblBenefitTypeDescription").text();
            if (ns.viewModel.currentModel.indexOf("wfmESSEmployerPayrollDetailMaintenance") === 0 && Txtservicecredit == "Service Credit Purchase") {
              var ldomDiv = $("#" + ns.viewModel.currentModel);   
              var txtLastNameText = ldomDiv.find("#txtLastName").val();
              var txtFirstNameText = ldomDiv.find("#txtFirstName").val();
              var txtSSN = ldomDiv.find("#txtSSN").val();
              if (txtLastNameText == "") {
                  ldomDiv.find("#capp1").css("display", "inline-block");
              }
              else {
                  ldomDiv.find("#capp1").css("display", "none");
              }
              if (txtFirstNameText == "") {
                  ldomDiv.find("#capp2").css("display", "inline-block");
              }
              else {
                  ldomDiv.find("#capp2").css("display", "none");
              }
              if (txtSSN == "") {
                  ldomDiv.find("#capp3").css("display", "inline-block");
              }
              else {
                  ldomDiv.find("#capp3").css("display", "none");
              }
            
            }
        });

        //FW Upgrade :: Re-register f/w event for go to content on the page because Scroll does not occuring on #CenterSplitter and #CenterMiddle (PIR - 11256)
        $(document).off('click.neoEvents', '.scrollit').on('click.neoEvents', '.scrollit', function (e) {
            var target = $(e.target)[0];
            var scrollTo = target.getAttribute("scrollTo");
            var tabindex = target.getAttribute("tabindex");
            var tabstripid = target.getAttribute("tabstripid");
            var divToScroll = target.getAttribute("divToScroll");
            if (scrollTo != null) {
                var parent = $(divToScroll);
                var lblnTabNavigatorCall = false;
                var lobjPanelBar;
                var lstrPanelBarId = [divToScroll, nsConstants.SPACE_HASH, scrollTo].join('');
                lobjPanelBar = nsCommon.GetWidgetControl($(lstrPanelBarId));
                if (lobjPanelBar != undefined && lobjPanelBar instanceof MVVM.Controls.Panel) {
                    var item = lobjPanelBar.select();
                    lobjPanelBar.expand(item);
                    lblnTabNavigatorCall = true;
                }
                if (tabindex != undefined && tabindex != null) {
                    var lstrTabId = [divToScroll, nsConstants.SPACE_HASH, tabstripid].join('');
                    var lobjTabStrip = nsCommon.GetWidgetControl($(lstrTabId));
                    if (lobjTabStrip != undefined && lobjTabStrip instanceof MVVM.Controls.TabContainer) {
                        var ldomTab = lobjTabStrip.getTabByIndex(parseInt(tabindex, 10));
                        item = lobjTabStrip.select(parseInt(tabindex, 10));
                        lobjTabStrip.selectItem(item);
                        if (ldomTab != null && $(ldomTab)[0].classList.contains(nsConstants.TABSHEET_ACTIVE_CLASS)) {
                            nsCommon.SetActiveTabNavigator(tabstripid, tabindex, scrollTo, divToScroll.replace(nsConstants.HASH, nsConstants.BLANK_STRING));
                            nsCommon.ToggleNavigatorPanel(parent[0], true, lobjPanelBar.element[0], divToScroll.replace(nsConstants.HASH, nsConstants.BLANK_STRING));
                        }
                        lblnTabNavigatorCall = false;
                    }
                }
                if (lblnTabNavigatorCall) {
                    nsCommon.SetActiveTabNavigator(undefined, -1, scrollTo, divToScroll.replace(nsConstants.HASH, nsConstants.BLANK_STRING));
                    nsCommon.ToggleNavigatorPanel(parent[0], true, lobjPanelBar.element[0], divToScroll.replace(nsConstants.HASH, nsConstants.BLANK_STRING), false, true);
                }
                if (!target.classList.contains("s-no-scroll")) {
                    //var FormContainerID = [nsConstants.HASH, $(divToScroll).closest(nsConstants.FORMCONTAINER_SELECTOR)[0].id].join('');
                    //In our case, Scroll is occuring on ".page-slideout-container-wrapper" instead of "#CenterMiddle" and "#CenterSplitter"
                    var FormContainerID = ".page-slideout-container-wrapper"
                    $(FormContainerID).scrollTo([divToScroll, nsConstants.SPACE_HASH, scrollTo].join(''));
                }
                else {
                    target.classList.remove("s-no-scroll");
                }
                if (!target.classList.contains("s-tab-navigator-tab-caption") && !target.classList.contains("s-tab-navigator-panel-caption")) {
                    var targetSection = $([divToScroll, nsConstants.SPACE_HASH, scrollTo].join('')).find(nsConstants.DIV_TAG)[0];
                    $(targetSection).addClass('highlighted');
                    var fnSetTimeout = function () {
                        $(targetSection).removeClass('highlighted');
                    };
                    setTimeout(fnSetTimeout, 500);
                }
            }
        });
        $(document).off('click.LogOutEvent', "a[href*='www']").on("click.LogOutEvent", "a[href*='www']", function () {
            var isLogout = confirm("You will be logged out of the system, Are you sure you want to continue?");
            if (isLogout == true) {
                var data = null;
                data = nsRequest.SyncPost("IsFromImageClick", "POST");
                if (data != null && data != undefined) {
                    sessionStorage.setItem("ImageLinkToNavigate", $(this).attr("href"));
                    ns.logoutSesssion();

                }
                return false;                             
            }
            else {
                return false;
            }
        });
        $(document).keypress(function (event) {
            var keycode = (event.keyCode ? event.keyCode : event.which);
            if (keycode == '13') {
                var ldomDiv = $("#" + ns.viewModel.currentModel);
                var saveButton = ldomDiv.find('.btnSave_Click_button,.btnNoChangesSave_Click_button[type="button"]:visible');
                if (saveButton && saveButton.length > 0) {
                    saveButton.focus();
                }
            }
            return true;
        });

        $(document).on("click", "#lnkSwitchOrganization", function (e) {
            var cofirm = dataLossPopUpMessage();
            if (!cofirm) {
                e.preventDefault();
            }
        });
        $(document).on("click", "#lnkSwitchMember", function (e) {
            var cofirm = dataLossPopUpMessage();
            if (!cofirm) {
                e.preventDefault();
            }
        });

        $(document).on("click", ".formNavigationPrev, .formNavigationNext", function () {
            var ldomDiv = $("#" + ns.viewModel.currentModel);
            if (ns.viewModel.currentModel.indexOf("wfmESSEmployerPayrollDetailMaintenance") == 0) {               
                ldomDiv.find("#btnCancel").trigger("click");
            }
            else if(ns.viewModel.currentModel.indexOf("wfmESSEmployerPayrollHeaderMaintenance")==0) {
                ldomDiv.find("#btnCancel").trigger("click");
            }
        });

    },
    BeforeShowDiv: function (e) {
        var ldomDiv = $("#" + ns.viewModel.currentModel);
        if (ldomDiv && ldomDiv.length > 0 && (ns.SenderID === "formNavigationTreeNode" || ns.SenderID ==="")) {
            ns.PositionCursor(ns.viewModel.currentModel, ldomDiv);
        }
        nsUserFunctions.iblnMenuItemClicked = false;
    },
    PutFocusOnRightField: function (e) {
        var ldomDiv = $("#" + ns.viewModel.currentModel);
        var ltxtFirstRtmtTextBox = ldomDiv.find("input[type='text'][gridid='grvBusEmployerPayrollDetailForRetirment']:first");
        if (ltxtFirstRtmtTextBox && ltxtFirstRtmtTextBox.length > 0) {
            $(ltxtFirstRtmtTextBox).focus();
            $(ltxtFirstRtmtTextBox).select();
        }
        else {
            var ltxtFirstDefTextBox = ldomDiv.find("input[type='text'][gridid='grvBusEmployerPayrollDetailForDeferredCompensation']:first");
            if (ltxtFirstDefTextBox && ltxtFirstDefTextBox.length > 0) {
                $(ltxtFirstDefTextBox).focus();
                $(ltxtFirstDefTextBox).select();
            }
        }
        return false;
    },
    ChangeDisplayError: function (e) {
        nsUserFunctions.iblnMenuItemClicked = false;
    },
    // F/W Upgrade PIR:11228 - ESS- Navigation issue on wfmESSEmployeeMaintenance screen.
    //Removed wfmESSEmployeeLookup lookup from cache.
    BeforeMenuNavigate: function (e) {
        if (ns.viewModel && ns.viewModel.currentModel) {
            if (ns.viewModel["wfmESSEmployeeLookup"] != undefined) {
                nsEvents.OnDeleteFormClick("wfmESSEmployeeLookup");
            }
        }
        if (nsUserFunctions.iblnMenuItemClicked) {
            return false;
        }
        else {
            nsUserFunctions.iblnMenuItemClicked = dataLossPopUpMessage();
            return nsUserFunctions.iblnMenuItemClicked;
        }
    },
    BeforeNavigate: function (e) {
        if (ns.viewModel && (ns.viewModel.currentModel == "wfmESSEmployerPayrollHeaderLookup" || ns.viewModel.currentModel.indexOf("wfmESSRetirementEmployerPayrollHeaderLookup") === 0 || ns.viewModel.currentModel.indexOf("wfmESSInsuranceEmployerPayrollHeaderLookup") === 0 || ns.viewModel.currentModel.indexOf("wfmESSDefferedCompEmployerPayrollHeaderLookup") === 0 ||
            ns.viewModel.currentModel.indexOf("wfmESSServicePurchaseEmployerPayrollHeaderLookup") === 0) && ns.SenderID == "btnOpenLookup") {
            var ldomDiv = null;
            var ddlBenefitType = $('#ddl_benefit_type_value').val();
            for (var property in ns.viewModel) {
                if (property.toString().indexOf("wfmCreatePayrollMaintenance") >= 0) {
                    var ldomDiv = $("#" + property);
                    ns.viewModel[property].HeaderData.MaintenanceData["ddlDropDownVUPR"] = "";
                }
            }
            if(ldomDiv != undefined && ldomDiv != null)
            {
                //if (ddlBenefitType == null) {
                    ldomDiv.find('#ddlDropDownVUPR').val('').trigger('change');
                //}
                var myControl = ldomDiv.find('#ddlPayrollReport');
                myControl.val('CRPR').trigger('change');
                ldomDiv.find('#ddlBenefitType').val(ddlBenefitType).trigger('change');
                nsEvents.OnDeleteFormClick(ns.viewModel.currentModel);
            }
        }
        //if (ns.viewModel.currentModel.indexOf("Lookup") > 0) {
        //    nsEvents.OnDeleteFormClick(ns.viewModel.currentModel);
        //}
        if (ns.viewModel.currentModel.indexOf("wfmESSViewRequestsMaintenance") == 0) {
            nsEvents.OnDeleteFormClick(ns.viewModel.currentModel);
        }
        if (ns.viewModel.currentModel.indexOf("wfmESSContactMaintenance") == 0) {
            nsEvents.OnDeleteFormClick(ns.viewModel.currentModel);
        }
        if (ns.viewModel.currentModel.indexOf("wfmESSOrganizationMaintenance") == 0) {
            nsEvents.OnDeleteFormClick(ns.viewModel.currentModel);
        }
        if (ns.viewModel.currentModel.indexOf("wfmESSEmployerPayrollHeaderMaintenance") == 0) {
            nsEvents.OnDeleteFormClick(ns.viewModel.currentModel);
        }
        if (ns.viewModel.currentModel.indexOf("wfmESSEmployerPayrollDetailMaintenance") == 0) {
            nsEvents.OnDeleteFormClick(ns.viewModel.currentModel);
        }
        return true;
    },
    ReportTypeAdjustment: function () {
        var ldomDiv = $("#" + ns.viewModel.currentModel);
        if (ldomDiv.find('#lblReportTypeDescription').text() != "Adjustment") {
            return confirm('Are you sure you are ready to post this Report Header?');
        }
        return true;
    },
    ReportTypeAdjustment: function () {
        var ldomDiv = $("#" + ns.viewModel.currentModel);
        if (ldomDiv.find('#lblReportTypeDescription').text() != "Adjustment") {
            return confirm('Are you sure you are ready to post this Report Header?');
        }
        return true;
    },

    // FMUpgrade : Added for Default page conversion of btnEmployeeGo_Click method
    fnEmployeeGo_Click: function (e) {

        var ldomDiv = $("#" + ns.viewModel.currentModel);
        var EmployeeTasksVal = ldomDiv.find("#ddlEmployeeTasks").val();

        switch (EmployeeTasksVal) {
            case "TEEM":
                nsCommon.sessionSet("EmployeeTask", "Terminate");
                ldomDiv.find("#btnOpenLookup").trigger("click");
                break;
            case "UPED":
                nsCommon.sessionSet("EmployeeTask", "UpdateTerminate");
                ldomDiv.find("#btnOpenLookup").trigger("click");
                break;
            case "VIED":
                nsCommon.sessionSet("EmployeeTask", "ViewUpdate");
                ldomDiv.find("#btnOpenLookup").trigger("click");
                break;
            case "OTRP":
                nsCommon.sessionSet("EmployeeTask", "EnrollOther457");
                ldomDiv.find("#btnOpenLookup").trigger("click");
                break;
            case "SENE":
                ldomDiv.find("#btnOpen").trigger("click");
                break;
            case "VIEP":
                ldomDiv.find("#btnOpen").trigger("click");
                break;

        }

        return false;
    },

    // FMUpgrade : Added for Default page conversion of btnEmpUpdateGo_Click method
    fnEmpUpdateGo_Click: function (e) {
        var ldomDiv = $("#" + ns.viewModel.currentModel);
        var ChangeTypeValue = ldomDiv.find("#ddlIstrTypeOfChange").val();

        switch (ChangeTypeValue) {
            case "CLSC":
                    ldomDiv.find("#btnNewChangeEmploymentInfo").trigger("click");
                break;
            case "LOAL":
                    ldomDiv.find("#btnNewChangeEmploymentLOAWithoutPayInfo").trigger("click");               
                break;        
        }
        return false;
    },

    // FMUpgrade : Added for Default page conversion of btnOpen_Click method for istrFormName == "wfmESSEmployeeLookup"
    fnbtnTerminateButtonOpen: function (e) {

        var ldomDiv = $("#" + ns.viewModel.currentModel);
        var aRelatedControlObject = {};
        var aGridData = nsCommon.GetWidgetByActiveDivIdAndControlId(ns.viewModel.currentForm, "dgrResult");

        if (aGridData && aGridData.jsObject && aGridData.jsObject.dataSource && aGridData.jsObject.dataSource.data && aGridData.jsObject.dataSource.data.length > 0) {
            var aRowIndex = $(e).attr("rowindex");
            var aSelectedRecord;
            if (aRowIndex && aRowIndex != "") {
                //Single Record selected
                aSelectedRecord = aGridData.jsObject.dataSource.data[aRowIndex];
            }

            if (aSelectedRecord != undefined) {
                //Single record has been selected, Add further code to manipulate single record
                aPersonEmploymentid = aSelectedRecord["dt_personemploymentid_22_0"];
                aPersonEmploymentDtlid = aSelectedRecord["dt_personemploymentdtlid_23_0"];
                var Parameters = {};
                Parameters["aintplanid"] = 0;
                Parameters["aintemploymentId"] = aPersonEmploymentid;
                Parameters["aintemploymentdetailid"] = aPersonEmploymentDtlid;
                var data = {};
                if (data && data != null) {
                    ldomDiv.find("#lblPersonEmploymentID").val(aPersonEmploymentid);
                    ldomDiv.find("#lblPersonEmploymentDtlID").val(aPersonEmploymentDtlid);
                    ldomDiv.find("#btnNewTerminateEmployment").trigger("click");
                }
            }
            return false;
        }
    },

    // FMUpgrade : Added for Default page conversion of btn_OpenPDF method
    fnOpenPDF: function (e) {

        if (ns.viewModel.currentModel.indexOf("wfmESSResourceLibraryMaintenance") === 0) {
            var currentmodel = ns.viewModel.currentModel;
            var selectedGrid;
            var ActiveDivID = nsCommon.GetActiveDivId(ns.viewModel.srcElement);
            var sfwRelatedGrid = MVVMGlobal.GetControlAttribute(ns.viewModel.srcElement, "sfwRelatedControl", ActiveDivID);
            if (ns.viewModel && ns.viewModel[currentmodel]
                && ns.viewModel[currentmodel].WidgetControls
                && ns.viewModel[currentmodel].WidgetControls[sfwRelatedGrid]) {
                var aRowIndex = $(e).attr("rowindex");
                selectedGrid = ns.viewModel[currentmodel].WidgetControls[sfwRelatedGrid];
                aSelectedRecord = selectedGrid.jsObject.dataSource.data[aRowIndex];
                if (aSelectedRecord != undefined) {
                    astrFileName = aSelectedRecord["dt_FileName_2_0"];
                    window.open(ns.SiteName + "/Home" + "/" + "OpenESSForms?astrFileName=" + astrFileName + "&aintESSFlag=1", "_blank", "width=800,height=800,resizable=yes,toolbar=yes,scrollbars=yes,alwaysRaised=yes");
                    return false;
                }
            }
        }
        else if (ns.viewModel.currentModel.indexOf("wfmESSViewSeminarsMaintenance") === 0) {
            window.open(ns.SiteName + "/Home" + "/" + "OpenESSForms?astrFileName=SFN-53176.pdf&aintESSFlag=0", "_blank", "width=800,height=800,resizable=yes,toolbar=yes,scrollbars=yes,alwaysRaised=yes");
        }
        else if (ns.viewModel.currentModel.indexOf("wfmTerminateEmploymentWizard") === 0) {
            window.open(ns.SiteName + "/Home" + "/" + "OpenESSForms?astrFileName=Delayed Payroll Memo.pdf&aintESSFlag=1", "_blank", "width=800,height=800,resizable=yes,toolbar=yes,scrollbars=yes,alwaysRaised=yes");
        }

    },
    //GetPrefix : function(){
    //    var Prefix = MVVMGlobal.GetPrefixforAjaxCall();
 
    //    if (location.pathname === "/") {
    //        Prefix = "";
    //    }
    //    else if (location.pathname === "/" + ns.SiteName + "/") {
    //        Prefix = "";
    //    }
    //    else if (location.pathname === "/" + ns.SiteName + "/") {
    //        Prefix = "/" + ns.SiteName + "/";
    //    }
    //    else if (Prefix == "") {
    //        Prefix = "/";
    //    } 
    //    if(ns.SiteName == "" && Prefix == ""){
    //        Prefix = "/";
    //    }
    //    return Prefix;
    //},
    //ExporTestcaseClick: function (e) {
    //    var lstrActiveForm = GetControlAttribute(e.target, "sfwActiveForm", e.context.activeDivID);
    //    var lUsecaseKey = $("#" + e.context.activeDivID + " #lblUsecaseKey").text();
    //    var lstrTestcaseType = e.target.defaultValue;

    //    var DownloadUrl = ns.SiteName + "/api/Neo/ExportDataExcel?astrFormID=" + lstrActiveForm + "&aintUsecaseKey=" + lUsecaseKey;
    //    if (lstrTestcaseType.indexOf('Unit') > 0)
    //        DownloadUrl = DownloadUrl + "&astrTestcaseType=UTC";
    //    else
    //        DownloadUrl = DownloadUrl + "&astrTestcaseType=STC";

    //    ns.activityStart();
    //    nsCommon.DispalyMessage("Export To Excel Started,Please wait till File available to Download.", e.context.activeDivID);

    //    $.ajax
    //        ({
    //            type: "POST",
    //            url: DownloadUrl,
    //            success: function (fileDownloadUrl) {
    //                ns.sessionStartTime = new Date().getTime();
    //                nsCommon.DispalyMessage("Export To Excel Completed.", e.context.activeDivID);
    //                window.location.href = fileDownloadUrl;
    //                ns.activityComplete();
    //            },
    //            error: function (xhr, ajaxOptions, thrownError) {
    //                if (xhr.status === 403) {
    //                    ns.logoutSesssion();
    //                }
    //                else {
    //                    nsCommon.DispalyError("Export To Excel Failed.", e.context.activeDivID);
    //                    ns.activityComplete();
    //                }
    //            }
    //        });

    //    return false;
    //},
    // FMUpgrade : Added for Default page conversion of btnDownloadAttachment_Click method
    //fnOpenReport: function (e) {

    //    var currentmodel = ns.viewModel.currentModel;
    //    var selectedGrid;

    //    if (ns.viewModel && ns.viewModel[currentmodel]
    //        && ns.viewModel[currentmodel].WidgetControls
    //        && ns.viewModel[currentmodel].WidgetControls["grvBusWssMessageDetail"]) {
    //        selectedGrid = ns.viewModel[currentmodel].WidgetControls["grvBusWssMessageDetail"];
    //        var aRowIndex = $(e).attr("rowindex");
    //        if (aRowIndex && aRowIndex != "") {
    //            //Single Record selected
    //            aSelectedRecord = selectedGrid.jsObject.dataSource.data[aRowIndex];
    //        }
    //        if (aSelectedRecord != undefined) {
    //            //Single record has been selected, Add further code to manipulate single record
    //            astrReportName = aSelectedRecord["dt_ReportName_0_0"];
    //            aintOrgId = aSelectedRecord["dt_OrgID_1_0"];
    //            var Parameters = {};
    //            Parameters["astrReportName"] = astrReportName;
    //            Parameters["aintOrgId"] = aintOrgId;

    //            var data = {};
    //            data = nsCommon.SyncPost("ViewEssReport_Click", Parameters, "POST");

    //            if (data && data != null) {
    //                if (window.navigator && window.navigator.msSaveOrOpenBlob) {
    //                    var blob = base64toBlob(data);
    //                    var report = astrReportName + '.pdf';
    //                    window.navigator.msSaveOrOpenBlob(blob, report, "width=800,height=800,resizable=yes,toolbar=yes,scrollbars=yes,alwaysRaised=yes");
    //                    return false;
    //                }
    //                else {
    //                    var arrrayBuffer = base64ToArrayBuffer(data); //data is the base64 encoded string 
    //                    var file = new Blob([arrrayBuffer], { type: 'application/pdf' });
    //                    var fileURL = URL.createObjectURL(file);
    //                    window.open(fileURL, "_blank", "width=800,height=800,resizable=yes,toolbar=yes,scrollbars=yes,alwaysRaised=yes");
    //                    return false;
    //                }
    //            }
    //        }
    //        return true;
    //    }
    //},
    // FMUpgrade : Added for Default page conversion of btnValidateExecuteBusinessMethod_Click method 
    fnRedirectCreateReport: function (e) {
        
        var ldomDiv = $("#" + ns.viewModel.currentModel);
        var lstrCreateReportNav = ldomDiv.find("#lblCreateReportNav").text();
        if(lstrCreateReportNav == "PRDL")
            ldomDiv.find("#btnOpenPayrollDetailLookup").trigger("click");
        else if (lstrCreateReportNav == "EMPL") {
            nsCommon.sessionSet("EnrollInOther457", true);
            ldomDiv.find("#btnNewEmployee").trigger("click");
        }
        return false;
    },

    // FMUpgrade : Added for Default page conversion of btn_OpenPDFFromNavigationParam method
    fnOpenPDFFromNavigationParam: function (e) {

        var ldomDiv = $("#" + ns.viewModel.currentModel);
        var aRelatedControlObject = {};
        var aGridData = nsCommon.GetWidgetByActiveDivIdAndControlId(ns.viewModel.currentForm, "dgrResult");
        if (aGridData && aGridData.jsObject && aGridData.jsObject.dataSource && aGridData.jsObject.dataSource.data && aGridData.jsObject.dataSource.data.length > 0) {
            var aRowIndex = $(e)[0].currentTarget.getAttribute("rowindex");
            var aSelectedRecord;
            if (aRowIndex && aRowIndex != "") {
                //Single Record selected
                aSelectedRecord = aGridData.jsObject.dataSource.data[aRowIndex];
            }
            if (aSelectedRecord != undefined) {
                //Single record has been selected, Add further code to manipulate single record
                aintEmployerPayrollHeaderId = aSelectedRecord["dt_ReportID_0_0"];
                var Parameters = {};
                Parameters["aintEmployerPayrollHeaderId"] = aintEmployerPayrollHeaderId;

                //FW Upgrade : PIR 11480 - To Open report in new tab in IE and Chrome , modification in the below code
                if (ns.viewModel.currentModel.indexOf("wfmESSEmployerPayrollHeaderHomeLookup") == 0) {
                    var lstrFilePath = nsCommon.SyncPost("CreateRemittanceReportPath", Parameters, "POST");
                    window.open(ns.SiteName + "/Home/OpenPDFRender?astrFileName=" + lstrFilePath, "_blank", "width=800,height=800,resizable=yes,toolbar=yes,scrollbars=yes,alwaysRaised=yes");
                    return false;
                }
                var data = {};
                data = nsCommon.SyncPost("CreateRemittanceReport", Parameters, "POST");

                if (data && data != null) {
                    if (window.navigator && window.navigator.msSaveOrOpenBlob) {
                        var blob = base64toBlob(data);
                        var report = aintEmployerPayrollHeaderId + '.pdf';
                        window.navigator.msSaveOrOpenBlob(blob, report, "width=800,height=800,resizable=yes,toolbar=yes,scrollbars=yes,alwaysRaised=yes");
                        return false;
                    }
                    else {
                        var arrrayBuffer = base64ToArrayBuffer(data); //data is the base64 encoded string 
                        var file = new Blob([arrrayBuffer], { type: 'application/pdf' });
                        var fileURL = URL.createObjectURL(file);
                        window.open(fileURL, "_blank", "width=800,height=800,resizable=yes,toolbar=yes,scrollbars=yes,alwaysRaised=yes");
                        return false;
                    }
                }
            }
            return false;
        }
    },
    // FMUpgrade : Added for Default page conversion of btn_OpenPDFFromNavigationParam method
    fnOpenPDFFromNavigationParameter: function (e) {

        var ldomDiv = $("#" + ns.viewModel.currentModel);
        var lblEmployerPayrollHeaderId = ldomDiv.find("#lblEmployerPayrollHeaderId").text();
        var Parameters = {};
        Parameters["aintEmployerPayrollHeaderId"] = lblEmployerPayrollHeaderId;

        var data = {};
        data = nsCommon.SyncPost("CreateRemittanceReport", Parameters, "POST");
        if (data && data != null) {
            if (window.navigator && window.navigator.msSaveOrOpenBlob) {
                var blob = base64toBlob(data);
                var report = lblEmployerPayrollHeaderId + '.pdf';
                window.navigator.msSaveOrOpenBlob(blob, report, "width=800,height=800,resizable=yes,toolbar=yes,scrollbars=yes,alwaysRaised=yes");
                return false;
            }
            else {
                var arrrayBuffer = base64ToArrayBuffer(data); //data is the base64 encoded string 
                var file = new Blob([arrrayBuffer], { type: 'application/pdf' });
                var fileURL = URL.createObjectURL(file);
                window.open(fileURL, "_blank", "width=800,height=800,resizable=yes,toolbar=yes,scrollbars=yes,alwaysRaised=yes");
                return false;
            }
        }
        return false;
    },
    //DownLoadPirAttachment: function (e) {
    //    ns.activityStart();
    //    var lstrActiveForm = nsCommon.GetFormNameFromDivID(e.context.activeDivID);
    //    var lstrPirattachmentId = e.text;
    //    var DownloadUrl = ns.SiteName + "/api/Neo/DownLoadAttachment?astrFormID=" + lstrActiveForm + "&aintPirattachmentId=" + lstrPirattachmentId;
    //    $.download(DownloadUrl, e.context, "post");
    //    nsCommon.DispalyMessage("Pir Attachment is downloaded.", e.context.activeDivID);
    //    ns.activityComplete();
    //    return false;
    //},
    //OnUcsChangeRefreshChart: function (e) {
    //    var lstrDetails = e.context.value;
    //    $("#" + e.context.activeDivID + " #btnRefreshChart").trigger("click");
    //    return true;
    //},
    //BeforeAssignUTCToPir: function (e) {
    //    var UnitTestcaseId = $("#" + e.context.activeDivID + " #txtUnitTestcase").val();

    //    if (!$.isNumeric(UnitTestcaseId)) {
    //        UnitTestcaseId = "0";
    //    }
    //    UnitTestcaseId = parseInt(UnitTestcaseId);
    //    if (UnitTestcaseId <= 0) {
    //        nsCommon.DispalyError("Unit Testcase ID is not valid.", e.context.activeDivID);
    //        return false;
    //    }
    //    return true;
    //},

    //BeforeSaveFileForm: function (e) {
    //    var FileId = $("#" + e.context.activeDivID + " #txtFileId").val();

    //    if (!$.isNumeric(FileId)) {
    //        nsCommon.DispalyError("File ID must be Numeric.", e.context.activeDivID);
    //        return false;
    //    }
    //    FileId = parseInt(FileId);
    //    if (FileId <= 0) {
    //        nsCommon.DispalyError("File ID is not valid.", e.context.activeDivID);
    //        return false;
    //    }
    //    return true;
    //},
    //BeforeSaveCodeForm: function (e) {
    //    var CodeId = $("#" + e.context.activeDivID + " #txtCodeID").val();

    //    if (!$.isNumeric(CodeId)) {
    //        nsCommon.DispalyError("Code ID must be Numeric.", e.context.activeDivID);
    //        return false;
    //    }
    //    CodeId = parseInt(CodeId);
    //    if (CodeId <= 0) {
    //        nsCommon.DispalyError("Code ID is not valid.", e.context.activeDivID);
    //        return false;
    //    }
    //    return true;
    //},
    //BeforeSaveMessageForm: function (e) {
    //    var MessageId = $("#" + e.context.activeDivID + " #lblValuemessage_id").val();

    //    if (!$.isNumeric(MessageId)) {
    //        nsCommon.DispalyError("Message ID must be Numeric.", e.context.activeDivID);
    //        return false;
    //    }
    //    MessageId = parseInt(MessageId);
    //    if (MessageId <= 0) {
    //        nsCommon.DispalyError("Message ID is not valid.", e.context.activeDivID);
    //        return false;
    //    }
    //    return true;
    //},
    //btnMap_Click: function (e) {
    //    var FormContainerID = "";
    //    var ActiveDivID = "";
    //    var RelatedGridID = "";

    //    var lbtnSelf;
    //    var lintSelectedIndex = -1;
    //    if (e != undefined && e.tagName === "A") {
    //        lbtnSelf = $(e)[0];
    //        var FormContainerID = "#" + $(e).closest('div[role="group"]')[0].id;
    //        var ActiveDivID = $(e).closest('div[id^="wfm"]')[0].id;
    //        lbtnSelf = $(FormContainerID + " #" + ActiveDivID + " #" + GetControlAttribute(lbtnSelf, "sfwRelatedControl", ActiveDivID))[0];
    //        lintSelectedIndex = e.getAttribute("rowIndex");
    //    } else {
    //        lbtnSelf = ns.viewModel.srcElement;
    //        var FormContainerID = "#" + $(lbtnSelf).closest('div[role="group"]')[0].id;
    //        ActiveDivID = $(lbtnSelf).closest('div[id^="wfm"]')[0].id;
    //    }

    //    RelatedGridID = GetControlAttribute(lbtnSelf, "sfwRelatedControl", ActiveDivID);
    //    if (RelatedGridID != null) {
    //        var lobjGridWidget = nsCommon.GetWidgetByActiveDivIdAndControlId(ActiveDivID, RelatedGridID);
    //        if (lobjGridWidget == undefined || lobjGridWidget.jsObject == undefined) {
    //            return false;
    //        }
    //    }

    //    var PrimaryId = 0;
    //    if (ActiveDivID.lastIndexOf("wfmBPMCaseMaintenance", 0) === 0) {
    //        PrimaryId = parseInt($("#" + e.context.activeDivID + " #lblCaseId").text());
    //    }
    //    else if (ActiveDivID.lastIndexOf("wfmBPMInitiationMaintenance", 0) === 0) {
    //        PrimaryId = parseInt($("#" + e.context.activeDivID + " #ddlProcessId").val());
    //    }
    //    else {
    //        var ldictParams = nsCommon.GetNavigationParams(lbtnSelf, e);
    //    }

    //    if (ActiveDivID.lastIndexOf("wfmDesignSpecificationMaintenance", 0) === 0) {
    //        nsCommon.localStorageSet("design_specification_bpm_map_id", ldictParams.lstrFirstID);
    //        //window.open(ns.SiteName + "/BPMExecution/MapRender.html", "_blank");
    //        window.open(ns.SiteName + "/Home/MapRender", "_blank");
    //    }
    //    else if (ActiveDivID.lastIndexOf("wfmBPMInitiationMaintenance", 0) === 0) {
    //        nsCommon.localStorageSet("ProcessId", PrimaryId);
    //        nsCommon.localStorageSet("CaseId", 0);
    //        window.open(ns.SiteName + "/Home/BPMNReadOnlyMap", "_blank");
    //    }
    //    else if (ActiveDivID.lastIndexOf("wfmBPMCaseLookup", 0) === 0 || ActiveDivID.lastIndexOf("wfmBPMCaseMaintenance", 0) === 0) {
    //        nsCommon.localStorageSet("ProcessId", 0);
    //        nsCommon.localStorageSet("CaseId", PrimaryId > 0 ? PrimaryId : ldictParams.lstrFirstID);
    //        var w = window.open(ns.SiteName + "/Home/BPMNReadOnlyMap", "_blank");
    //        w.name = window.name;
    //    }
    //    else {
    //        nsCommon.localStorageSet("CaseInstanceID", ldictParams.lstrFirstID);
    //        //window.open(ns.SiteName + "/BPMExecution/BPMNMap.html", "_blank");
    //        window.open(ns.SiteName + "/Home/BPMNMap", "_blank");
    //    }
    //},
    //Sameer: Same control ID within wizard is not allowed so dynamically setting AutoLogin properties based on various finish buttons
    //SetAutoLoginPropertiesForSecureAccessStep: function (e) {
    //    ns.AutoLoginProp = 'lblSecureAccecssOrgId';
    //    ns.AutoLoginUID = 'lblSecureAccecssEmail';
    //    ns.AutoLoginPwd = 'lblSecureAccecssPassword';
    //    nsRequest.AutoLogin();
    //    return false;
    //},
    //SetAutoLoginPropertiesForRegisterMachineStep: function (e) {
    //    ns.AutoLoginProp = 'lblRegisterMachineOrgId';
    //    ns.AutoLoginUID = 'lblRegisterMachineUserID';
    //    ns.AutoLoginPwd = 'lblRegisterMachinePassword';
    //    nsRequest.AutoLogin();
    //    return false;
    //},
    UploadDocument: function (e) {
        //Pushawart: Need to make hard error show up through javascript with the help of Amol
        //var FileType = $("#" + e.context.activeDivID).find("#ddlFileType").val();
        //if (FileType === undefined || FileType === "") {
        //    nsCommon.DispalyError("File type must be selected.", e.context.activeDivID);
        //    return false;
        //}
        var lstrFormName = nsCommon.GetFormNameFromDivID(e.context.activeDivID);
        if (ns.DirtyData != undefined && ns.DirtyData[e.context.activeDivID] == undefined) {
            ns.DirtyData[e.context.activeDivID] = { HeaderData: {}, istrFormName: lstrFormName, KeysData: {} };
            ns.DirtyData[e.context.activeDivID].HeaderData = { MaintenanceData: {} };
            ns.DirtyData[e.context.activeDivID].HeaderData.MaintenanceData = ns.viewModel[e.context.activeDivID].HeaderData.MaintenanceData;
        }
        switch (lstrFormName) {
            //case "wfmMSSMEHPInsuranceApplicationWizard":
            //    {
            //        ns.DirtyData[e.context.activeDivID].HeaderData.MaintenanceData["PersonId"] = $("#" + e.context.activeDivID).find("#lblApplicantPersonId").text();
            //    }
            //    break;
            //case "wfmRefundApplicationWizard":
            //    {
            //        ns.DirtyData[e.context.activeDivID].HeaderData.MaintenanceData["ddlRefundFileDesc"] = $("select#ddlRefundFileDesc option:selected").val();
            //    }
            //    break;
            case "wfmUploadFileMaintenance":
                {
                    $("#" + e.context.activeDivID).find("#ddlFileType").trigger("change");
                    var lstrFileId = ns.viewModel[e.context.activeDivID].HeaderData.MaintenanceData["ddlFileType"];
                    ns.DirtyData[e.context.activeDivID].HeaderData.MaintenanceData["ddlFileType"] = lstrFileId;
                }
                break;
        }
        //Pushawart: Not deleting the code to call the button click to refresh the screen after uploading the document to show all uploaded files in the grid.
        //var executeButton = $("#" + e.context.activeDivID).find("#btnExecuteRefreshData");
        //setTimeout(function (e) {
        //    executeButton.trigger("click");
        //}, 3000);

        return true;
    },
    //Sameer : After user hit on forget password link in the ESS External user login wizard
    //system send OTP to the user and Navigate to login page
    //NavigateToLogin: function (e) {
    //    if (window.location.toString().indexOf('EssUserLoginWizard') > -1)
    //        window.location = window.location.origin + '/' + ns.SiteName + '/Account/LoginE'
    //    else
    //        window.location = window.location.origin + '/' + ns.SiteName + '/Account/Login'
    //    return false;
    //},
    //DownloadDocument: function (e) {
    //    if (ns.DirtyData[e.context.activeDivID] === undefined) {
    //        var lstrFormName = nsCommon.GetFormNameFromDivID(e.context.activeDivID);
    //        ns.DirtyData[e.context.activeDivID] = { HeaderData: {}, istrFormName: lstrFormName };
    //        ns.DirtyData[e.context.activeDivID].HeaderData = { MaintenanceData: {} };
    //        ns.DirtyData[e.context.activeDivID].HeaderData.MaintenanceData = ns.viewModel[e.context.activeDivID].HeaderData.MaintenanceData;
    //    }
    //    else {
    //        var lstrFormName = nsCommon.GetFormNameFromDivID(e.context.activeDivID);
    //        ns.DirtyData[e.context.activeDivID].HeaderData.istrFormName = lstrFormName;
    //    }
    //    return true;
    //},
    FileUploadSuccess: function (e) {
        delete ns.DirtyData[e.context.activeDivID];
        MVVMGlobal.PopulateDirtyFormList();
        var tempFormName = ns.viewModel.currentModel;

        var ActiveDivID = e.context.activeDivID;
        if (ActiveDivID.indexOf("wfmUploadFileMaintenance") == 0) {
            var executeButton = $("#" + ActiveDivID).find("#btnNew");
            //$("#" + ActiveDivID).find(".s-fileupload-clear").trigger("click");
            executeButton.trigger("click");
        }

        if (tempFormName != "" && tempFormName.indexOf("wfmESSUploadDocumentsMaintenance") != 0) {
            setTimeout(function () {
                var dataitem = nsCommon.GetDataItemFromDivID(tempFormName);
                if (MVVMGlobal.CanBeDeleted(dataitem)) {
                    nsEvents.OnDeleteFormClick(tempFormName, false);
                }
            }, 10);
        }
    },    
    //  FMUpgrade:Added for Default page conversion of FrameworkInit method
    AfterApplyingUI: function (e) {

        var ldomDiv = e.context.idomActiveDiv != undefined ? e.context.idomActiveDiv : $("#" + e.context.activeDivID);
        if (e.context.activeDivID != undefined && (e.context.activeDivID.indexOf("wfmESSEmployerPayrollHeaderLookup") === 0 || e.context.activeDivID.indexOf("wfmESSUnBalancedEmployerPayrollHeaderLookup") === 0 || e.context.activeDivID.indexOf("wfmESSEmployerPayrollHeaderHomeLookup") === 0
            ||e.context.activeDivID.indexOf("wfmESSRetirementEmployerPayrollHeaderLookup") === 0 ||e.context.activeDivID.indexOf("wfmESSInsuranceEmployerPayrollHeaderLookup") === 0 || e.context.activeDivID.indexOf("wfmESSDefferedCompEmployerPayrollHeaderLookup") === 0||
            e.context.activeDivID.indexOf("wfmESSServicePurchaseEmployerPayrollHeaderLookup") === 0)) {

            ldomDiv.find("#ddlBalancingStatusValue").append(new Option("Unbalanced AND No Remittance", "'UNBL', 'NOR'"));
        }
        if (e.context.activeDivID != undefined && (e.context.activeDivID.indexOf("wfmESSEmployerPayrollHeaderLookup") === 0 || e.context.activeDivID.indexOf("wfmESSCombinedPayrollDetailLookup") === 0 || ns.viewModel.currentModel.indexOf("wfmESSRetirementCombinedPayrollDetailLookup") == 0 || ns.viewModel.currentModel.indexOf("wfmInsuranceESSCombinedPayrollDetailLookup") == 0 || ns.viewModel.currentModel.indexOf("wfmESSDeferredCompCombinedPayrollDetailLookup") == 0 ||
                 ns.viewModel.currentModel.indexOf("wfmESSServicePurchaseCombinedPayrollDetailLookup") == 0 || e.context.activeDivID.indexOf("wfmESSEmployerPayrollHeaderHomeLookup") === 0 ||
            e.context.activeDivID.indexOf("wfmESSRetirementEmployerPayrollHeaderLookup") === 0 ||e.context.activeDivID.indexOf("wfmESSInsuranceEmployerPayrollHeaderLookup") === 0 || e.context.activeDivID.indexOf("wfmESSDefferedCompEmployerPayrollHeaderLookup") === 0||
            e.context.activeDivID.indexOf("wfmESSServicePurchaseEmployerPayrollHeaderLookup") === 0)) {
            if (nsCommon.sessionGet(ns.viewModel.currentModel + "_Refresh") != undefined && nsCommon.sessionGet(ns.viewModel.currentModel + "_Refresh") == "true") {
                setTimeout(function(){
                    nsCommon.SetLookupFormParams(nsConstants.CONTENT_SPLITTER_SELECTOR, e.context.activeDivID);
                    nsCommon.sessionRemove(ns.viewModel.currentModel + "_Refresh");
                }, 0);
            }
        }
        //if (e.context.activeDivID.indexOf("wfmESSEmployerPayrollDetailMaintenance") === 0) {
        //    setTimeout(function () {
        //        ldomDiv.find("input[type='text'][sfwdataformat='{0:C}'][sfwextendcurrency='{0:C}']:visible").trigger("blur");
        //    }, 10);
        //}
    },
    AfterBindFormData: function (e) {

        var ldomDiv = e.context.idomActiveDiv != undefined ? e.context.idomActiveDiv : $("#" + e.context.activeDivID);
        if (e.context.activeDivID != undefined && e.context.activeDivID.indexOf("wfmUploadFileMaintenance") === 0) {
            setTimeout(function () {
                $("#ddlFileType").val($("#ddlFileType option:first").val()).trigger("change");
            }, 0);
        }

        if (ns.viewModel.currentModel.indexOf("wfmESSEmployerPayrollDetailMaintenance") == 0) {
            if (ldomDiv.find('#lblStatus1').text() == 'Review') {
                ldomDiv.find('#capComments').attr('class', 'CommentInReview');
            }
            else {
                ldomDiv.find('#capComments').removeAttr('class');
            }
        }
        if (ns.viewModel.currentModel.indexOf("wfmESSEmployerPayrollDetailMaintenance") == 0) {
            if (ldomDiv.find('#lblStatus1').text() == 'Review') {
                ldomDiv.find('#txtComments').attr('class', 'CommentBoxInReview');
            }
            else {
                ldomDiv.find('#txtComments').removeAttr('class');
            }
        }
        if (e.context.activeDivID && e.context.activeDivID.indexOf("wfmESSHomeMaintenance") == 0) {
            var lstrURL = ldomDiv.find("#lblLabel").text();
            ldomDiv.find("#btnUpdateYourAccount1").attr("href", lstrURL);
        }
        //  FMUpgrade:Added for Default page conversion of OnLoadComplete method
        if (e.context.activeDivID && e.context.activeDivID.indexOf("wfmESSActiveNoEmployerPayrollHeaderMaintenance") === 0) {
            ToggleVisibilityLinksForNo(ldomDiv);
        }
        if (e.context.activeDivID && e.context.activeDivID.indexOf("wfmESSActiveEmployerPayrollHeaderMaintenance") === 0) {
            ToggleVisibilityLinksForYes(ldomDiv);
        }
        if (e.context.activeDivID && e.context.activeDivID.indexOf("wfmESSDeathNoticeMaintenance") === 0) {
            setTimeout(function () {
                CheckIsPersonEnrolledInRetirementPlan();
            },100);
            
        }
        if (e.context.activeDivID != undefined && e.context.activeDivID.indexOf("wfmESSEmployeeMaintenance") === 0) {
            
            ldomDiv.find("#lblPOBoxMessage").css('display') == "none" ? ldomDiv.find("#divPOBoxMessage").hide() : ldomDiv.find("#divPOBoxMessage").show();
            ldomDiv.find("#lblLabel9").css('display') == "none" ? ldomDiv.find("#divLabel9").hide() : ldomDiv.find("#divLabel9").show();
        }

        if (e.context.activeDivID && e.context.activeDivID.indexOf("wfmESSEmployerPayrollDetailMaintenance") == 0) {
            ldomDiv.find("#capp1,#capp2,#capp3").css("display", "none");
        }
        if (e.context.activeDivID.indexOf("wfmESSActiveMemberHomeMaintenance") >= 0) {
            var lstrActiveDivId = e.context.activeDivID;
            var ldomDiv = e.context.idomActiveDiv != undefined ? e.context.idomActiveDiv : $(["#", lstrActiveDivId].join(""));
            var lstrURL = ldomDiv.find("#lblUpdActUrl").text();
            ldomDiv.find("#hypHyperLink").attr("href", lstrURL);
        }
        if (e.context.activeDivID && e.context.activeDivID.indexOf("wfmESSActiveEmployerPayrollHeaderMaintenance") === 0) {
            if (ldomDiv.find('#lblHeaderTypeDescription').text() == 'Retirement' && ldomDiv.find('#lblReportTypeDescription').text() == 'Regular') {
                var neoGridData = ldomDiv.find("#GridTable_grvBusEmployerPayrollDetailForRetirment").data("neoGrid").RenderData;
                var neoGridDataLength = ldomDiv.find("#GridTable_grvBusEmployerPayrollDetailForRetirment").data("neoGrid").RenderData.length;
                for (var i = 0; i < neoGridDataLength; i++) {
                    neoGridData[i]["dt_EligibleWages_10_0"] = '';
                }
                ldomDiv.find('input[data-field="dt_EligibleWages_10_0"]').val('').trigger('change');
            }
        }

        ns.arrNeedToRefresh["wfmESSRetirementEmployerPayrollHeaderLookup"] = true;
        ns.arrNeedToRefresh["wfmESSInsuranceEmployerPayrollHeaderLookup"] = true;
        ns.arrNeedToRefresh["wfmESSDefferedCompEmployerPayrollHeaderLookup"] = true;
        ns.arrNeedToRefresh["wfmESSServicePurchaseEmployerPayrollHeaderLookup"] = true;
        ns.arrNeedToRefresh["wfmESSRetirementCombinedPayrollDetailLookup"] = true;
        ns.arrNeedToRefresh["wfmESSInsuranceCombinedPayrollDetailLookup"] = true;
        ns.arrNeedToRefresh["wfmESSDeferredCompCombinedPayrollDetailLookup"] = true;
        ns.arrNeedToRefresh["wfmESSServicePurchaseCombinedPayrollDetailLookup"] = true;
        ns.arrNeedToRefresh["wfmESSRemittanceLookup"] = true;
        ns.arrNeedToRefresh["wfmESSDepositLookup"] = true;
        ns.arrNeedToRefresh["wfmESSFileHdrLookup"] = true;
    },
    //  FMUpgrade:Added for Default page conversion of FrameworkInit method
    showDivCallBack: function (e) {
        GetMessageCount();
        var ldomDiv = e.context.idomActiveDiv != undefined ? e.context.idomActiveDiv : $("#" + e.context.activeDivID);
        if (e.context.activeDivID != undefined && e.context.activeDivID.indexOf("wfmESSFileHdrLookup") === 0) {

            //F/W Upgrade PIR - 11036 - All is displayed two times one All is removed. 
            //ldomDiv.find("#ddl_benefit_type_value").prepend(new Option("All", ""));
            ldomDiv.find("#ddl_benefit_type_value").val(ldomDiv.find("#ddl_benefit_type_value option:first").val()).trigger("change");

        }
        if (e.context.activeDivID != undefined && (e.context.activeDivID.indexOf("wfmESSRemittanceLookup") === 0) || (e.context.activeDivID.indexOf("wfmESSDepositLookup") === 0) ||
            (e.context.activeDivID.indexOf("wfmESSEmployerPayrollHeaderLookup") === 0)||e.context.activeDivID.indexOf("wfmESSRetirementEmployerPayrollHeaderLookup") === 0 ||e.context.activeDivID.indexOf("wfmESSInsuranceEmployerPayrollHeaderLookup") === 0 || e.context.activeDivID.indexOf("wfmESSDefferedCompEmployerPayrollHeaderLookup") === 0||
            e.context.activeDivID.indexOf("wfmESSServicePurchaseEmployerPayrollHeaderLookup") === 0 || (e.context.activeDivID.indexOf("wfmESSEmployerPayrollHeaderHomeLookup") === 0) || (e.context.activeDivID.indexOf("wfmESSRetirementPayrollDetailLookup") === 0) ||
            (e.context.activeDivID.indexOf("wfmESSInsurancePayrollDetailLookup") === 0) || (e.context.activeDivID.indexOf("wfmESSServicePurchasePayrollDetailLookup") === 0) ||
            (e.context.activeDivID.indexOf("wfmESSDeferredCompPayrollDetailLookup") === 0) || (e.context.activeDivID.indexOf("wfmESSCombinedPayrollDetailLookup") === 0) ||
            ns.viewModel.currentModel.indexOf("wfmESSRetirementCombinedPayrollDetailLookup") == 0 || ns.viewModel.currentModel.indexOf("wfmInsuranceESSCombinedPayrollDetailLookup") == 0 || ns.viewModel.currentModel.indexOf("wfmESSDeferredCompCombinedPayrollDetailLookup") == 0 ||
                 ns.viewModel.currentModel.indexOf("wfmESSServicePurchaseCombinedPayrollDetailLookup") == 0 ||
            (e.context.activeDivID.indexOf("wfmESSUnBalancedEmployerPayrollHeaderLookup") === 0) || (e.context.activeDivID.indexOf("wfmESSEmployeeLookup") === 0)) {
            nsCommon.DispalyMessage("[ Please enter search criteria and press SEARCH. ]", e.context.activeDivID);
        }
        
        if (e.context.activeDivID != undefined && (e.context.activeDivID.indexOf("wfmESSRetirementEmployerPayrollHeaderLookup") === 0 || e.context.activeDivID.indexOf("wfmESSInsuranceEmployerPayrollHeaderLookup") === 0 || e.context.activeDivID.indexOf("wfmESSDeferredCompEmployerPayrollHeaderLookup") === 0 ||
            e.context.activeDivID.indexOf("wfmESSServicePurchaseEmployerPayrollHeaderLookup") === 0 || (e.context.activeDivID.indexOf("wfmESSEmployerPayrollHeaderHomeLookup") === 0) || (e.context.activeDivID.indexOf("wfmESSRetirementPayrollDetailLookup") === 0) ||
            (e.context.activeDivID.indexOf("wfmESSInsurancePayrollDetailLookup") === 0) || (e.context.activeDivID.indexOf("wfmESSServicePurchasePayrollDetailLookup") === 0))) {
            if (ldomDiv.find("#ddl_benefit_type_value") == "" || ldomDiv.find("#ddl_benefit_type_value").val() == null) {
                ldomDiv.find("#ddl_benefit_type_value").val(ldomDiv.find("#ddl_benefit_type_value option:first").val()).trigger("change");
            }
        }

        if (e.context.activeDivID.indexOf("wfmESSEmployerPayrollHeaderHomeLookup") === 0) {
            ldomDiv.find("#btnRefresh").off('click').on('click', function () {
                RefreshEmployerPayrollHeaderLookup(e);
                return false;
            });
            RefreshEmployerPayrollHeaderLookup(e);
        }

        if (e.context.activeDivID.indexOf("wfmESSEmployeeLookup") === 0) {
	      ns.arrNeedToRefresh["wfmESSEmployeeLookup"] = true;         	             
        }

        // FW upgrade:Added to disable the ddl_benefit_type_value value
        if (e.context.activeDivID != undefined && e.context.activeDivID.indexOf("wfmESSRetirementEmployerPayrollHeaderLookup") === 0 || e.context.activeDivID.indexOf("wfmESSInsuranceEmployerPayrollHeaderLookup") === 0 || e.context.activeDivID.indexOf("wfmESSDeferredCompEmployerPayrollHeaderLookup") === 0
            || e.context.activeDivID.indexOf("wfmESSServicePurchaseEmployerPayrollHeaderLookup") === 0 ) {
            if (ldomDiv.find("#ddl_benefit_type_value").val() != undefined || ldomDiv.find("#ddl_benefit_type_value").val() != "") {
                ldomDiv.find("#ddl_benefit_type_value").attr("disabled", "disabled");                        
            }
        }
        if (e.context.activeDivID != undefined && e.context.activeDivID.indexOf("wfmESSCombinedPayrollDetailLookup") === 0 || ns.viewModel.currentModel.indexOf("wfmESSRetirementCombinedPayrollDetailLookup") == 0 || ns.viewModel.currentModel.indexOf("wfmInsuranceESSCombinedPayrollDetailLookup") == 0 || ns.viewModel.currentModel.indexOf("wfmESSDeferredCompCombinedPayrollDetailLookup") == 0 ||
            ns.viewModel.currentModel.indexOf("wfmESSServicePurchaseCombinedPayrollDetailLookup") == 0 || ns.viewModel.currentModel.indexOf("wfmESSInsuranceCombinedPayrollDetailLookup") == 0) {
            if (ldomDiv.find("#ddlBenefitTypeValue").val() != undefined || ldomDiv.find("#ddlBenefitTypeValue").val() != "") {
                ldomDiv.find("#ddlBenefitTypeValue").attr("disabled", "disabled");
                ldomDiv.find("#ddlBenefitTypeValue").attr("class", "aspNetDisabled");
                ldomDiv.find("#ddlPlanId").removeAttr("disabled");
            }
            if (ldomDiv.find("#ddlBenefitTypeValue").val() != null && ldomDiv.find("#ddlBenefitTypeValue").val() != "") {
                ShowAllSearchFieldsForPayrollDetailLookup(ldomDiv);
                var lBenifitType = ldomDiv.find("#ddlBenefitTypeValue").val();
                if (lBenifitType == "RETR" || lBenifitType == "INSR") {
                    if (lBenifitType == "RETR") {
                        ldomDiv.find("#lblContributionsDisplayed").show();
                    }
                    else if (lBenifitType == "INSR") {
                        ldomDiv.find("#lblContributionsDisplayed").hide();
                    } 
                    ldomDiv.find("#lblReportingMonthFrom,#txtReportingPeriodFrom,#lblReportingMonthTo,#txtReportingPeriodTo").show();
                }
                else if (lBenifitType == "PRCH") {

                    ldomDiv.find("#capEphSubmittedDatefrm,#txtEphSubmittedDatefrm,#capEphSubmittedDateTo,#txtEphSubmittedDateTo").show();
                    ns.ShowControl(ldomDiv.find("#txtEphSubmittedDatefrm,#txtEphSubmittedDateTo"));
                    ldomDiv.find("#lblReportingMonthTo,#txtReportingPeriodTo,#capReportTypeValue,#capPlanId,#ddlPlanId,#lblContributionsDisplayed").hide();
                }

                else if (lBenifitType == "DEFF") {
                    ldomDiv.find("#capPayPeriodStartDate,#txtPayPeriodStartDate,#capPayPeriodEndDate,#txtPayPeriodEndDate,#capPayCheckDateFrom,#txtPayCheckDateFrom,#capPayCheckDateTo,#txtPayCheckDateTo").show();
                    ns.ShowControl(ldomDiv.find("#txtPayPeriodStartDate,#txtPayCheckDateFrom,#txtPayPeriodEndDate,#txtPayCheckDateTo"));
                    ldomDiv.find("#lblContributionsDisplayed").hide();
                }
            }

        }
        if (e.context.activeDivID && e.context.activeDivID.indexOf("wfmCreatePayrollMaintenance") === 0) {
            fnddlBenefitTypeToggleVisibilty(ldomDiv);
            //$("#ddlPayrollReport").val($("#ddlPayrollReport option:First").val());
        }
        if (e.context.activeDivID && e.context.activeDivID.indexOf("wfmUpdateTermEmploymentLOAWithoutPayWizard") === 0) {
            LOAVisibility(ldomDiv);
        }
        if (e.context.activeDivID && e.context.activeDivID.indexOf("wfmESSMemberRecordDataWizard") === 0) {
            ldomDiv.find("#chkCheckBox,#chkbx2,#lblLabel57").css("display", "none");
        }
        if (e.context.activeDivID && e.context.activeDivID.indexOf("wfmESSDeathNoticeMaintenance") === 0) {
            ldomDiv.find("#chkCheckBox,#chkbx2,#lblLabel").css("display", "none");
        }
        if (e.context.activeDivID && e.context.activeDivID.indexOf("wfmESSOrganizationMaintenance") === 0) {
            ldomDiv.find("#chkCheckBox,#chkbx2,#lblLabel2").css("display", "none");
        }
        if (e.context.activeDivID && e.context.activeDivID.indexOf("wfmESSContactMaintenance") === 0) {
            ldomDiv.find("#chkCheckBox,#chkbx2,#lblLabel2").css("display", "none");
        }
        if ((e.context.activeDivID && e.context.activeDivID.indexOf("wfmESSContactMaintenance") === 0)
            && (ldomDiv.find("li[errormessage='Address is Invalid. Please enter a valid Address.']").length > 0)) {
            ldomDiv.find("#chkCheckBox,#chkbx2,#lblLabel2").css("display", "block");
        }
        if (e.context.activeDivID && e.context.activeDivID.indexOf("wfmESSEmployeeMaintenance") === 0) {
            ldomDiv.find("#lblLabel8,#chkbx2,#chkCheckBox").css("display", "none");
        }

        var oFirstControl = ldomDiv.find(":not([gridid]):not([listviewid]):not(.filter):not(input.check_row):not(input.s-grid-check-all):not(input.ellipse-input-pageHolder):not(input.s-grid-common-filterbox):input[type !='button']:input[type !='submit']:input[type !='image']:input[sfwretrieval !='True']:input[sfwretrieval !='true']:visible:enabled:first");
        if ((oFirstControl != undefined) && (oFirstControl.length >= 0)) {
            if ((ns.viewModel.currentModel && ns.viewModel.currentModel.indexOf("wfmESSDeathNoticeMaintenance") >= 0 || (ns.viewModel.currentModel && ns.viewModel.currentModel.indexOf("wfmESSNewReportAProblemMaintenance") >= 0))) {
                ldomDiv.find('#txtPersonId').trigger("focus");
                ldomDiv.find('#txtCallbackPhone').trigger("focus");
            }
            else {
                oFirstControl.trigger("focus");
            }
        }        
        setTimeout(function () {
            if (ns.viewModel.currentModel.indexOf("Wizard") > 0) {
                var stepDiv = $("div[class='stepContainer'] div[data-sfwcontroltype='stepDiv']");
                if (stepDiv.length > 0) {
                    var titleAttr = stepDiv.attr('title');
                    if (titleAttr.length > 0) {
                        stepDiv.removeAttr('title');
                    }
                }
                var stepSpan = $("ul[class='ProgressBar'] li span[class='stepSpan']");
                if (stepSpan.length > 0) {
                    var titleAttribute = stepSpan.attr('title');
                    if (titleAttribute.length > 0) {
                        stepSpan.removeAttr('title');
                    }
                }
            }
        }, 0);

        // Code for ESS Payroll header refresh
        if (e.context.activeDivID.indexOf("wfmESSEmployerPayrollHeaderMaintenance") == 0 && ns.viewModel) {
            var fnRefreshData = function () {
                var ldomDiv = $("#" + ns.viewModel.currentModel);
                if (e.context.activeDivID.indexOf("wfmESSEmployerPayrollHeaderMaintenance") == 0 && nsCommon.sessionGet("ResetTriggredESSHeader") == undefined) {
                    nsCommon.sessionSet("ResetTriggredESSHeader", "true");
                    ldomDiv.find("#btnCancel").trigger("click");
                }
            }
            setTimeout(fnRefreshData, 50);
        }
        else if (e.context.activeDivID.indexOf("wfmESSEmployerPayrollDetailMaintenance") == 0 && ns.viewModel) {
            var fnRefreshData = function () {
                var ldomDiv = $("#" + ns.viewModel.currentModel);
                if (e.context.activeDivID.indexOf("wfmESSEmployerPayrollDetailMaintenance") == 0 && nsCommon.sessionGet("ResetTriggredESSDetail") == undefined) {
                    nsCommon.sessionSet("ResetTriggredESSDetail", "true");                    
                    if(ns.viewModel.srcElement && ns.viewModel.srcElement.id && (ns.viewModel.srcElement.id != "btnSaveAndNew"))
                        ldomDiv.find("#btnCancel").trigger("click");
                }
            }
            setTimeout(fnRefreshData, 50);
        }
        else {
            var fnClearRefreshData = function () {
                nsCommon.sessionRemove("ResetTriggredESSDetail");
                nsCommon.sessionRemove("ResetTriggredESSHeader");
            }
            if ((nsCommon.sessionGet("ResetTriggredESSDetail") != undefined) || (nsCommon.sessionGet("ResetTriggredESSHeader") != undefined)) {
                setTimeout(fnClearRefreshData, 50);
            }
        }

    },
    btnGo_Click: function (e) {
        var ldomDiv = e.context.idomActiveDiv != undefined ? e.context.idomActiveDiv : $("#" + e.context.activeDivID);
        var lstrPayrollReport = ldomDiv.find("#ddlPayrollReport").val();
        var lstrIsNewclick = ldomDiv.find("#txtTextBox1").val();
        if (lstrPayrollReport != undefined && lstrPayrollReport != null && (lstrPayrollReport == "VUPR")) {
            var ddlBenefitType = ldomDiv.find("#ddlDropDownVUPR").val();
            if (ddlBenefitType != undefined && ddlBenefitType != "" && ddlBenefitType == "RETR")
            {
                ldomDiv.find("#btnOpenLookupRetirement").trigger("click");
            }
            if (ddlBenefitType != undefined && ddlBenefitType != "" && ddlBenefitType == "INSR") {
                ldomDiv.find("#btnOpenLookupInsurance").trigger("click");
            }
            if (ddlBenefitType != undefined && ddlBenefitType != "" && ddlBenefitType == "DEFF") {
                ldomDiv.find("#btnOpenLookupDefferedComp").trigger("click");
            }
            if (ddlBenefitType != undefined && ddlBenefitType != "" && ddlBenefitType == "PRCH") {
                ldomDiv.find("#btnOpenLookupServicePurchase").trigger("click");
            }
            return false;
        }
        else if (lstrPayrollReport != undefined && lstrPayrollReport != null && lstrPayrollReport == "INPR") {
            ldomDiv.find("#btnOpenLookup2").trigger("click");
            return false;
        }
        else if (lstrPayrollReport != undefined && lstrPayrollReport != null && lstrPayrollReport == "CRPR") {
            var lstrBenefitType = ldomDiv.find("#ddlBenefitType").val();
            if ((lstrBenefitType == "INSR" || lstrBenefitType == "RETR") && lstrIsNewclick != undefined && lstrIsNewclick == "true") {
                ldomDiv.find("#btnNew").trigger("click");
            }
            else if ((lstrBenefitType == "INSR" || lstrBenefitType == "RETR") && lstrIsNewclick != undefined && lstrIsNewclick == "false") {
                ldomDiv.find("#btnNoNew").trigger("click");
            }
            else if (lstrBenefitType == "PRCH" && lstrIsNewclick != undefined && lstrIsNewclick == "true") {
                ldomDiv.find("#btnNew").trigger("click");
            }
            else if (lstrBenefitType == "DEFF" && lstrIsNewclick != undefined && lstrIsNewclick == "true") {
                ldomDiv.find("#btnNew").trigger("click");
            }
            else if (lstrBenefitType == "DEFF" && lstrIsNewclick != undefined && lstrIsNewclick == "false") {             
                ldomDiv.find("#btnNoNew").trigger("click");
            }
            return false;
        }
        if (lstrPayrollReport != undefined && lstrPayrollReport != null && lstrPayrollReport == "SCPR") {
            var ddlBenefitType = ldomDiv.find("#ddlBenefitTypeSearchDetail").val();
            if (ddlBenefitType != undefined && ddlBenefitType != "" && ddlBenefitType == "RETR") {
                ldomDiv.find("#btnOpenLookup1").trigger("click");
            }
            if (ddlBenefitType != undefined && ddlBenefitType != "" && ddlBenefitType == "INSR") {
                ldomDiv.find("#btnOpenLookup3").trigger("click");
            }
            if (ddlBenefitType != undefined && ddlBenefitType != "" && ddlBenefitType == "DEFF") {
                ldomDiv.find("#btnOpenLookup4").trigger("click");
            }
            if (ddlBenefitType != undefined && ddlBenefitType != "" && ddlBenefitType == "PRCH") {
                ldomDiv.find("#btnOpenLookup5").trigger("click");
            }
            return false;            
        }
        return false;
    },
    TriggerChange: function (e) {
        var ldomDiv = e.context.idomActiveDiv != undefined ? e.context.idomActiveDiv : $("#" + e.context.activeDivID);
        if (e.context.activeDivID.indexOf("wfmCreatePayrollMaintenance") === 0)
        {
            ldomDiv.find("#txtPayPeriod").trigger("change");
        }
        if (e.context.activeDivID.indexOf("wfmTerminateEmploymentWizard") === 0) {
            if (ldomDiv.find("#txtIstrLastRetirementTransmittalOfDeduction").is(":visible")) 
                ldomDiv.find("#txtIstrLastRetirementTransmittalOfDeduction").trigger("change");
            if (ldomDiv.find("#txtIstrLastMonthOnEmployerBilling").is(":visible"))
                     ldomDiv.find("#txtIstrLastMonthOnEmployerBilling").trigger("change");
        }
        if (e.context.activeDivID.indexOf("wfmUpdateTermEmploymentLOAWithoutPayWizard") === 0) {
            ldomDiv.find("#txtTextBox").trigger("change");
        } 
        if (e.context.activeDivID.indexOf("wfmESSDeathNoticeMaintenance") === 0) {
            ldomDiv.find("#txtLastReportingMonthForRetrPlan").trigger("change");
        }
        if (e.context.activeDivID.indexOf("wfmESSEmployerPayrollHeaderLookup") === 0 ||
            e.context.activeDivID.indexOf("wfmESSRetirementEmployerPayrollHeaderLookup") === 0 ||e.context.activeDivID.indexOf("wfmESSInsuranceEmployerPayrollHeaderLookup") === 0) {
            var fromHTextbox = ldomDiv.find("#txtPayrollPaidDateFrom");
            var toHTextbox = ldomDiv.find("#txtPayrollPaidDateTo");
            if (!IsValidDate(fromHTextbox) || !IsValidDate(toHTextbox))
            {
                alert('Invalid Date');
                return;
            }
            ldomDiv.find("#txtPayrollPaidDateFrom").trigger("change");
            ldomDiv.find("#txtPayrollPaidDateTo").trigger("change");
        }
        if (e.context.activeDivID.indexOf("wfmESSCombinedPayrollDetailLookup") === 0 || ns.viewModel.currentModel.indexOf("wfmESSRetirementCombinedPayrollDetailLookup") == 0 || ns.viewModel.currentModel.indexOf("wfmInsuranceESSCombinedPayrollDetailLookup") == 0 || ns.viewModel.currentModel.indexOf("wfmESSDeferredCompCombinedPayrollDetailLookup") == 0 ||
                 ns.viewModel.currentModel.indexOf("wfmESSServicePurchaseCombinedPayrollDetailLookup") == 0) {
            var fromLTextbox = ldomDiv.find("#txtReportingPeriodFrom");
            var toLTextbox = ldomDiv.find("#txtReportingPeriodTo");
            if (!IsValidDate(fromLTextbox) || !IsValidDate(toLTextbox)) {
                alert('Invalid Date');
                return;
            }
            ldomDiv.find("#txtReportingPeriodFrom").trigger("change");
            ldomDiv.find("#txtReportingPeriodTo").trigger("change");
        }
        if (e.context.activeDivID.indexOf("wfmESSEmployerPayrollDetailMaintenance") === 0) {
            if (ldomDiv.find("#txtPayPeriodDate").is(":visible")) {
                ldomDiv.find("#txtPayPeriodDate").trigger("change");
            }
            if (ldomDiv.find("#txtPayPeriodEndMonthForBonus").is(":visible")) {
                     ldomDiv.find("#txtPayPeriodEndMonthForBonus").trigger("change");
            }
        }
        if (e.context.activeDivID.indexOf("wfmESSEmployerPayrollDetailMaintenance") === 0) {
            
            if (((ldomDiv.find('#txtLabel37').val() != ldomDiv.find('#txtEligibleWages').text()) ||
                (ldomDiv.find('#txtLabel38').val() != ldomDiv.find('#txtEeContributionReported').text()) ||
                (ldomDiv.find('#txtLabel39').val() != ldomDiv.find('#txtEePreTaxReported').text()) ||
                (ldomDiv.find('#txtLabel40').val() != ldomDiv.find('#txtEeEmployerPickupReported').text()) ||
                (ldomDiv.find('#txtLabel41').val() != ldomDiv.find('#txtErContributionReported').text()) ||
                (ldomDiv.find('#txtLabel42').val() != ldomDiv.find('#txtRhicErContributionReported').text()) ||
                (ldomDiv.find('#txtLabel43').val() != ldomDiv.find('#txtRhicEeContributionReported').text()) ||
                (ldomDiv.find('#txtEEPreTaxAddlReported').val() != ldomDiv.find('#txtEEPreTaxAddlReportedOld').text()) ||
                (ldomDiv.find('#txtEEPostTaxAddlReported').val() != ldomDiv.find('#txtEEPostTaxAddlReportedOld').text()) ||
                (ldomDiv.find('#txtERPreTaxMatchReported').val() != ldomDiv.find('#txtERPreTaxMatchReportedOld').text()) ||
                (ldomDiv.find('#txtADECAmtReported').val() != ldomDiv.find('#txtADECAmtReportedOld').text())) &&
                (ldomDiv.find('#txtComments').val() == ""))
            {
                CustomPopupAlert('Please enter comment explaining the reason wages and/or contributions were changed. <br><br> Changes will <b style="color: red;">Not be Saved</b> unless a comment is provided.');
                return;
            }
        }
        return true;
    },
    
    TriggerChangeReview: function (e) {
        var ldomDiv = e.context.idomActiveDiv != undefined ? e.context.idomActiveDiv : $("#" + e.context.activeDivID);
        if (e.context.activeDivID.indexOf("wfmESSEmployerPayrollHeaderHomeLookup") === 0) {
            var ldomDiv = $("#" + ns.viewModel.currentModel);
            var grid = document.getElementById("GridTable_dgrResult");
            for (var i = 0; i < grid.rows.length; i++) {

                for (var j = 0; j < grid.rows[i].cells.length; j++) {
                   
                    var cellInnerText = grid.rows[i].cells[j].innerText;
                    if (cellInnerText == "Review") {
                        ldomDiv.find('#capLblText').removeAttr('class', 'hideControl');
                    } 
                }
            }
        } 

    },
    ErrorPanelScrollIntoView: function (e) {
        var ldomDiv = e.context.idomActiveDiv != undefined ? e.context.idomActiveDiv : $("#" + e.context.activeDivID);
        if (e.context.activeDivID.indexOf("wfmESSEmployerPayrollHeaderMaintenance") === 0) {
            ldomDiv.find("#ScrollToThisDiv").trigger("focus");
        }
    },

    ConfirmESSAmount: function (e) {
        var ldomDiv = e.context.idomActiveDiv != undefined ? e.context.idomActiveDiv : $("#" + e.context.activeDivID);
        if (ldomDiv.find("#txtIdecAmountPerPayPeriod").val() == "$0.00" || ldomDiv.find("#txtIdecAmountPerPayPeriod").val() == "") {
            return confirm('The Amount Per Pay Period is Zero. Do you want to save the record?');
        }
        return true;
    },

    //FW Upgrade : Code conversion (View ESS Report)
    ExecuteESSReportViewClick: function (e) {
        var FormContainerID = "", lbtnSelf, ldictParams, ActiveDivID = "";
        var lintSelectedIndex = -1;
        if (e != undefined && e.tagName === "A") {
            lbtnSelf = $(e)[0];
            //ldictParams = nsCommon.GetNavigationParams(lbtnSelf, e);
            var FormContainerID = "#" + $(e).closest('div[role="group"]')[0].id;
            var ActiveDivID = $(e).closest('div[id^="wfm"]')[0].id;
            lintSelectedIndex = e.getAttribute("rowIndex");

            if ($(lbtnSelf).text().trim() == "PDF" || $(lbtnSelf).text().trim() == "CSV") {
                var lRowData = ns.viewModel[ns.viewModel.currentModel]["DetailsData"]["grvBusWssMessageDetail"]["Records"][lintSelectedIndex];
                var lReportName = lRowData["dt_ReportName_0_1"];
                $("#" + ActiveDivID).find("#txtReportNameText").text(lReportName).trigger("change");
            }
            if ($(lbtnSelf).text().trim() == "Benefit Enrollment Report") {
                if (lintSelectedIndex > -1) {
                    var lRowData = ns.viewModel[ns.viewModel.currentModel]["DetailsData"]["grvBusWssMessageDetail"]["Records"][lintSelectedIndex];
                    var lOrgId = lRowData["dt_OrgID_1_0"];
                    var lContactId = lRowData["dt_ContactID_2_0"]; 
                    var lReportName = lRowData["dt_ReportName_0_0"];

                    $("#" + ActiveDivID).find("#txtOrgId").val(lOrgId).trigger("change");
                    $("#" + ActiveDivID).find("#txtContactId").val(lContactId).trigger("change");
                    $("#" + ActiveDivID).find("#txtReportName").val(lReportName).trigger("change");
                }
                sessionStorage.setItem("CallOpenPDF", "false");
                $("#" + ActiveDivID).find("#btnOpenBenefitEnrollment").trigger("click");
                
                return false;
            }
            else if ($(lbtnSelf).text().trim() == "Transaction History") {
                if (lintSelectedIndex > -1) {
                    var lRowData = ns.viewModel[ns.viewModel.currentModel]["DetailsData"]["grvBusWssMessageDetail"]["Records"][lintSelectedIndex];
                    var lOrgId = lRowData["dt_OrgID_1_0"];
                    var lContactId = lRowData["dt_ContactID_2_0"];
                    var lReportName = lRowData["dt_ReportName_0_0"];

                    $("#" + ActiveDivID).find("#txtOrgId").val(lOrgId).trigger("change");
                    $("#" + ActiveDivID).find("#txtContactId").val(lContactId).trigger("change");
                    $("#" + ActiveDivID).find("#txtReportName").val(lReportName).trigger("change");
                }
                sessionStorage.setItem("CallOpenPDF", "false");
                $("#" + ActiveDivID).find("#btnESSAgencyStatement").trigger("click");
                
                return false;
            }
            else if ($(lbtnSelf).text().trim() == "Payroll Reporting") {
                if (lintSelectedIndex > -1) {
                    var lRowData = ns.viewModel[ns.viewModel.currentModel]["DetailsData"]["grvBusWssMessageDetail"]["Records"][lintSelectedIndex];
                    var lOrgId = lRowData["dt_OrgID_1_0"];
                    var lContactId = lRowData["dt_ContactID_2_0"];
                    var lReportName = lRowData["dt_ReportName_0_0"];

                    $("#" + ActiveDivID).find("#txtOrgId").val(lOrgId).trigger("change");
                    $("#" + ActiveDivID).find("#txtContactId").val(lContactId).trigger("change");
                    $("#" + ActiveDivID).find("#txtReportName").val(lReportName).trigger("change");
                }
                sessionStorage.setItem("CallOpenPDF", "false");
                $("#" + ActiveDivID).find("#btnCreatePayroll").trigger("click");
                return false;
            }
            else if ($(lbtnSelf).text().trim() == "Retirement Contributions - Audit Confirmation") {
                if (lintSelectedIndex > -1) {
                    var lRowData = ns.viewModel[ns.viewModel.currentModel]["DetailsData"]["grvBusWssMessageDetail"]["Records"][lintSelectedIndex];
                    var lOrgId = lRowData["dt_OrgID_1_0"];
                    var lContactId = lRowData["dt_ContactID_2_0"];
                    var lReportName = lRowData["dt_ReportName_0_0"];

                    $("#" + ActiveDivID).find("#txtOrgId").val(lOrgId).trigger("change");
                    $("#" + ActiveDivID).find("#txtContactId").val(lContactId).trigger("change");
                    $("#" + ActiveDivID).find("#txtReportName").val(lReportName).trigger("change");
                }
                sessionStorage.setItem("CallOpenPDF", "false");
                $("#" + ActiveDivID).find("#btnOpenESSDownloadReports").trigger("click");
                return false;
            }
            else {
                sessionStorage.setItem("CallOpenPDF", "true");
            }
        }
        return true;
    },

    //FW Upgrade : Code conversion (View ESS Report) and download pdf file
    btn_OpenPDF: function (e) {
        if (sessionStorage.getItem("CallOpenPDF") == "false") {
            sessionStorage.setItem("CallOpenPDF", "true");
            return false;
        }
        var FormContainerID = "", lbtnSelf, ldictParams, ActiveDivID = "";
        var lintSelectedIndex = -1;
        if (e != undefined && e.tagName === "A") {
            lbtnSelf = $(e)[0];
            var FormContainerID = "#" + $(e).closest('div[role="group"]')[0].id;
            var ActiveDivID = $(e).closest('div[id^="wfm"]')[0].id;
            lbtnSelf = $(FormContainerID + " #" + ActiveDivID + " #" + MVVMGlobal.GetControlAttribute(lbtnSelf, "sfwRelatedControl", ActiveDivID))[0];
            lintSelectedIndex = e.getAttribute("rowIndex");
            ldictParams = nsCommon.GetNavigationParams(lbtnSelf, e);
        }
        else {
            lbtnSelf = ns.viewModel.srcElement;
            var FormContainerID = "#" + $(lbtnSelf).closest('div[role="group"]')[0].id;
            ActiveDivID = $(lbtnSelf).closest('div[id^="wfm"]')[0].id;
            ldictParams = nsCommon.GetNavigationParams(lbtnSelf, e);
        }
        if (ActiveDivID.lastIndexOf("wfmESSReportsMaintenance") === 0) {
            var lFileName = ldictParams["larrRows"].map(function (item) { return item["pdf_file_name"] })[0];
            window.open(ns.SiteName + "/Home/OpenPDFRender?astrFileName=" + lFileName, "_blank", "width=800,height=800,resizable=yes,toolbar=yes,scrollbars=yes,alwaysRaised=yes");
        }

    },
    //FW Upgrade :: wfmDefault.aspx.cs code conversion(btn_OpenStaticPage method)
    btn_OpenStaticPage: function (e) {
        var FormContainerID = "", lbtnSelf, ldictParams, ActiveDivID = "";
        var lintSelectedIndex = -1;
        if (e != undefined && e.tagName === "A") {
            lbtnSelf = $(e)[0];
            var FormContainerID = "#" + $(e).closest('div[role="group"]')[0].id;
            var ActiveDivID = $(e).closest('div[id^="wfm"]')[0].id;
            lbtnSelf = $(FormContainerID + " #" + ActiveDivID + " #" + MVVMGlobal.GetControlAttribute(lbtnSelf, "sfwRelatedControl", ActiveDivID))[0];
            lintSelectedIndex = e.getAttribute("rowIndex");
            ldictParams = nsCommon.GetNavigationParams(lbtnSelf, e);
        }
        else {
            lbtnSelf = ns.viewModel.srcElement;
            var FormContainerID = "#" + $(lbtnSelf).closest('div[role="group"]')[0].id;
            ActiveDivID = $(lbtnSelf).closest('div[id^="wfm"]')[0].id;
            ldictParams = nsCommon.GetNavigationParams(lbtnSelf, e);
        }
        if (ActiveDivID.lastIndexOf("wfmESSPlanMaintenance") === 0 || ActiveDivID.lastIndexOf("wfmFormListingMaintenance") === 0) {
            var lFileName = ldictParams["larrRows"].map(function (item) { return item["fileName"] })[0];
            window.open(lFileName, "_blank", "width=800,height=800,resizable=yes,toolbar=yes,scrollbars=yes,alwaysRaised=yes");
        }
    },

       SetUnSavedFormIcon: function (e) {
        var lstrFormName = nsCommon.GetProperFormName(e.context.activeDivID);
        if (nsUserFunctions.iarrFormsToSkipFromUnSavedIcon.indexOf(lstrFormName) >= 0) {
            return false;
        }
        return true;
    },
    ValidateSelectedFiles: function (e) {
        if (e.context.previouslySelectedFiles != undefined && e.context.previouslySelectedFiles.length == 1) {
            return false;
        }
        return true;
    },

    btnCreateReportClick: function () {
        var ldomDiv = $("#" + ns.viewModel.currentModel);
        if (ns.viewModel.currentModel.indexOf("wfmESSActiveEmployerPayrollHeaderMaintenance") === 0) {
            if (ldomDiv.find('#lblHeaderTypeDescription').text() == 'Retirement' && ldomDiv.find('#lblReportTypeDescription').text() == 'Regular') {
                var gridData = $("#" + nsCommon.GetActiveDivId() + " #GridTable_grvBusEmployerPayrollDetailForRetirment").data("neoGrid").RenderData;
                for (var i = 0; i < gridData.length; i++) {
                    if (gridData[i].dt_EligibleWages_10_0.toLocaleString() == '') {
                        var ltxtFirstRtmtTextBox = ldomDiv.find("input[type='text'][gridid='grvBusEmployerPayrollDetailForRetirment']:first");
                        if (ltxtFirstRtmtTextBox && ltxtFirstRtmtTextBox.length > 0) {
                            $(ltxtFirstRtmtTextBox).focus();
                            $(ltxtFirstRtmtTextBox).select();
                        }
                        CustomCreateReportPopupAlert("Report cannot be created because individual wage amounts were not entered. To proceed, enter eligible wages for each employee and select Create Report.");                      
                        return;
                    }
                }
            }
        }
        return true;
    },
    iarrFormsToSkipFromUnSavedIcon: ["wfmESSEmployeeHomeMaintenance", "wfmUpdateTermEmploymentMaintenance", "wfmESSReportsMaintenance", "wfmCreatePayrollMaintenance", "wfmUploadFileMaintenance"]
};

nsCommon.SetTitle = function (astrTitle) {
    $("#FormTitle").html(astrTitle);
    document.title = astrTitle;
    $("#LookupFormTitle").text(astrTitle);
    $('#lblForm').html(ns.viewModel.currentModel);
};

nsCommon.GetPageTitle = function (title, primarykey) {
    return title.replace("_PrimaryKey", '');
};


/*
* FMUpgrade: Changes for ToggleVisibilty
*/
function fnddlBenefitTypeToggleVisibilty(ldomDiv) {

    if (ldomDiv == undefined || (ldomDiv && ldomDiv.length == 0)) {
        ldomDiv = $("#" + ns.viewModel.currentModel);
    }
    var lPayrollReport = ldomDiv.find("#ddlPayrollReport").val();
    var BenefitTypeValue = ldomDiv.find("#ddlBenefitType option:selected");
    var BenefitTypeValueVUPR = ldomDiv.find("#ddlDropDownVUPR option:selected");
    var RetirementReportTypesValue = ldomDiv.find("#ddlRetirementReportTypes option:selected");
    var ReportTypesValue = ldomDiv.find("#ddlReportType option:selected");

    if ((lPayrollReport != '' && lPayrollReport != 'VUPR' && lPayrollReport != 'SCPR' && lPayrollReport != 'INPR') && (BenefitTypeValue.val() == 'RETR' )) {
        ldomDiv.find('#capAdtPayCheckDate,#txtAdtPayCheckDate,#capAdtPayPeriodStartDate,#txtAdtPayPeriodStartDate,#capAdtPayPeriodEndDate,#txtAdtPayPeriodEndDate,#lblReportType,#ddlReportType').hide();
        ldomDiv.find('#capPayrollPaidDate,#txtPayPeriod,#capReportTypeRetirement,#ddlRetirementReportTypes,#hypHyperLinkRetirement').show();
        if (RetirementReportTypesValue.val() == 'ADJS' || RetirementReportTypesValue.val() == 'BONS')
            ldomDiv.find('#capForAllEmployeesRet,#ddlForAllDropDownListRet').show();
        ldomDiv.find('#capForAllEmployees,#ddlForAllDropDownList').hide();
    }
    else if (BenefitTypeValue.val() == 'DEFF') {
        ldomDiv.find('#capAdtPayCheckDate,#txtAdtPayCheckDate,#capAdtPayPeriodStartDate,#txtAdtPayPeriodStartDate,#capAdtPayPeriodEndDate,#txtAdtPayPeriodEndDate,#lblReportType,#ddlReportType').show();
        ldomDiv.find('#capPayrollPaidDate,#capForAllEmployees,#ddlForAllDropDownList,#txtPayPeriod,#capReportTypeRetirement,#ddlRetirementReportTypes,#hypHyperLinkDeferredComp').hide();
        if (ReportTypesValue.val() == 'ADJS')
            ldomDiv.find('#capForAllEmployees,#ddlForAllDropDownList').show();
        ldomDiv.find('#capForAllEmployeesRet,#ddlForAllDropDownListRet').hide();
    }
    else if ((lPayrollReport != '' && lPayrollReport != 'VUPR' && lPayrollReport != 'SCPR' && lPayrollReport != 'INPR') && BenefitTypeValueVUPR.val() == 'DEFF') {
        ldomDiv.find('#capAdtPayCheckDate,#capReportTypeRetirement,#txtAdtPayCheckDate,#ddlRetirementReportTypes,#capAdtPayPeriodStartDate,#txtAdtPayPeriodStartDate,#capAdtPayPeriodEndDate,#txtAdtPayPeriodEndDate,#lblReportType,#ddlReportType,#hypHyperLinkRetirement').show();
        ldomDiv.find('#capForAllEmployees,#ddlForAllDropDownList,#txtPayPeriod,#capPayrollPaidDate').hide();

        if (ReportTypesValue.val() == 'ADJS')
            ldomDiv.find('#capForAllEmployees,#ddlForAllDropDownList').show();
        ldomDiv.find('#capForAllEmployeesRet,#ddlForAllDropDownListRet').hide();
    }

    else if (BenefitTypeValue.val() == 'PRCH' || BenefitTypeValueVUPR.val() == 'PRCH') {
        ldomDiv.find('#capAdtPayCheckDate,#txtAdtPayCheckDate,#capAdtPayPeriodStartDate,#txtAdtPayPeriodStartDate,#capAdtPayPeriodEndDate,#txtAdtPayPeriodEndDate,#capPayrollPaidDate,#txtPayPeriod,#lblReportType,#ddlReportType,#capReportTypeRetirement,#ddlRetirementReportTypes,#capForAllEmployees,#ddlForAllDropDownList,#capForAllEmployeesRet,#ddlForAllDropDownListRet').hide();
    }
    else if ((lPayrollReport != '' && lPayrollReport != 'VUPR' && lPayrollReport != 'SCPR' && lPayrollReport != 'INPR') && BenefitTypeValueVUPR.val() == 'INSR') {
        ldomDiv.find('#capAdtPayCheckDate,#txtAdtPayCheckDate,#capAdtPayPeriodStartDate,#txtAdtPayPeriodStartDate,#capAdtPayPeriodEndDate,#txtAdtPayPeriodEndDate,#lblReportType,#ddlReportType').hide();
        ldomDiv.find('#capReportTypeRetirement,#ddlRetirementReportTypes,#hypHyperLinkRetirement').show();
        if (RetirementReportTypesValue.val() == 'ADJS' || RetirementReportTypesValue.val() == 'BONS')
            ldomDiv.find('#capForAllEmployeesRet,#ddlForAllDropDownListRet').show();
        ldomDiv.find('#capForAllEmployees,#ddlForAllDropDownList').hide();
    }
    else if (lPayrollReport != null && lPayrollReport != undefined && (BenefitTypeValue.val() == '' || BenefitTypeValue.val() == undefined)) {
        ldomDiv.find('#capAdtPayCheckDate,#txtAdtPayCheckDate,#capAdtPayPeriodStartDate,#txtAdtPayPeriodStartDate,#capAdtPayPeriodEndDate,#txtAdtPayPeriodEndDate,#capPayrollPaidDate,#txtPayPeriod,#capReportTypeRetirement,#ddlRetirementReportTypes,#capForAllEmployees,#ddlForAllDropDownList,#capForAllEmployeesRet,#ddlForAllDropDownListRet').hide();
        ldomDiv.find('#lblReportType,#ddlReportType,#hypHyperLinkRetirement,#hypHyperLinkDeferredComp').hide();
    }
    else {
        ldomDiv.find('#capAdtPayCheckDate,#txtAdtPayCheckDate,#capAdtPayPeriodStartDate,#txtAdtPayPeriodStartDate,#capAdtPayPeriodEndDate,#txtAdtPayPeriodEndDate,#capPayrollPaidDate,#txtPayPeriod,#capReportTypeRetirement,#ddlRetirementReportTypes,#capForAllEmployees,#ddlForAllDropDownList,#capForAllEmployeesRet,#ddlForAllDropDownListRet').hide();
        ldomDiv.find('#lblBenefitType,#ddlBenefitType,#lblReportType,#ddlReportType,#lblBenefitTypeSearchDetail,#ddlBenefitTypeSearchDetail,#lblBenefitVUPR,#ddlDropDownVUPR,#hypHyperLinkRetirement,#hypHyperLinkDeferredComp').hide();
    }
}

function fnddlPayrollReportToggleVisibilty(ldomDiv) {

    if (ldomDiv == undefined || (ldomDiv && ldomDiv.length == 0)) {
        ldomDiv = $("#" + ns.viewModel.currentModel);
    }
    ldomDiv.find("#ddlBenefitType").val("").trigger("change");
    //ldomDiv.find("#ddlBenefitType").val(ldomDiv.find("#ddlBenefitType option:first").attr('selected', 'selected')).trigger("change");
}

function ToggleVisibilityLinksForNo(ldomDiv) {
    if (ldomDiv == undefined || (ldomDiv && ldomDiv.length == 0)) {
        ldomDiv = $("#" + ns.viewModel.currentModel);
    }
    var benefit_value = ldomDiv.find('#lblHeaderTypeDescription').text();
    var reportType_value = ldomDiv.find('#lblReportTypeDescription').text();

    ldomDiv.find('#hypHyperLinkAdjRetirement_One,#hypHyperLinkBonusRetirement_One,#hypHyperLinkAdjDeffComp_One').hide();
    if (benefit_value == "Retirement" && reportType_value == "Adjustment") {
        ldomDiv.find('#hypHyperLinkAdjRetirement_One').show();
    }
    else if (benefit_value == "Retirement" && reportType_value == "Bonus/Retro Pay") {
        ldomDiv.find('#hypHyperLinkBonusRetirement_One').show();
    }
    else if (benefit_value == "Deferred Compensation" && reportType_value == "Adjustment") {
        ldomDiv.find('#hypHyperLinkAdjDeffComp_One').show();
    }
}

function ToggleVisibilityLinksForYes(ldomDiv) {
    if (ldomDiv == undefined || (ldomDiv && ldomDiv.length == 0)) {
        ldomDiv = $("#" + ns.viewModel.currentModel);
    }
    var benefit_value = ldomDiv.find('#lblHeaderTypeDescription').text();
    var reportType_value = ldomDiv.find('#lblReportTypeDescription').text();
    ldomDiv.find('#hypHyperLinkReportRetirement,#hypHyperLinkAdjRetirement_All,#hypHyperLinkBonusRetirement_All,#hypHyperLinkRegularDeffComp,#hypHyperLinkAdjDeffComp_All,#hypHyperLinkAdjServicePurchase').hide();

    if (benefit_value == "Retirement" && reportType_value == "Regular") {
        ldomDiv.find('#hypHyperLinkReportRetirement').show();
    }
    else if (benefit_value == "Retirement" && reportType_value == "Adjustment") {
        ldomDiv.find('#hypHyperLinkAdjRetirement_All').show();
    }
    else if (benefit_value == "Retirement" && reportType_value == "Bonus/Retro Pay") {
        ldomDiv.find('#hypHyperLinkBonusRetirement_All').show();
    }
    else if (benefit_value == "Deferred Compensation" && reportType_value == "Regular") {
        ldomDiv.find('#hypHyperLinkRegularDeffComp').show();
    }
    else if (benefit_value == "Deferred Compensation" && reportType_value == "Adjustment") {
        ldomDiv.find('#hypHyperLinkAdjDeffComp_All').show();
    }
    else if (benefit_value == "Service Credit Purchase" && reportType_value == "Regular") {
        ldomDiv.find('#hypHyperLinkAdjServicePurchase').show();
    }
}

function LOAVisibility(ldomDiv) {
    if (ldomDiv == undefined || (ldomDiv && ldomDiv.length == 0)) {
        ldomDiv = $("#" + ns.viewModel.currentModel);
    }
    var Health = ldomDiv.find("#ddlHealth option:selected");
    var Dental = ldomDiv.find("#ddlDental option:selected");
    var Vision = ldomDiv.find("#ddlVision option:selected");
    var Life = ldomDiv.find("#ddlLife option:selected");
    var EAP = ldomDiv.find("#ddlEAP option:selected");
    var LTC = ldomDiv.find("#ddlLTC option:selected");
    var Flex = ldomDiv.find("#ddlFlex option:selected");

    if (Health.val() == 'NO' || Dental.val() == 'NO' || Vision.val() == 'NO' || Life.val() == 'NO' || EAP.val() == 'NO' || LTC.val() == 'NO' || Flex.val() == 'NO') {
        ldomDiv.find('#lblLabel18,#lblLabel9,#txtTextBox,#lblCoverageNote').show();
    }
    else {
        ldomDiv.find('#lblLabel18,#lblLabel9,#txtTextBox,#lblCoverageNote').hide();
    }
}

function SeasonalVisibility(ldomDiv) {
    if (ldomDiv == undefined || (ldomDiv && ldomDiv.length == 0)) {
        ldomDiv = $("#" + ns.viewModel.currentModel);
    }
    var EmployeeWorks = ldomDiv.find("#ddl12Months option:selected");
    if (EmployeeWorks.val() == 'Yes') {
        ldomDiv.find('#ddlSeasonal,#lblLabel24,#lblSeasonal').show();
    }
    else {
        ldomDiv.find('#ddlSeasonal,#lblLabel24,#lblSeasonal').hide();
    }
}

function base64ToArrayBuffer(base64) {
    var binaryString = window.atob(base64);
    var binaryLen = binaryString.length;
    var bytes = new Uint8Array(binaryLen);
    for (var i = 0; i < binaryLen; i++) {
        var ascii = binaryString.charCodeAt(i);
        bytes[i] = ascii;
    }
    return bytes;
}

function base64toBlob(data) {
    var byteCharacters = atob(data);
    var byteNumbers = new Array(byteCharacters.length);
    for (var i = 0; i < byteCharacters.length; i++) {
        byteNumbers[i] = byteCharacters.charCodeAt(i);
    }
    var byteArray = new Uint8Array(byteNumbers);
    var blob = new Blob([byteArray], {
        type: 'application/pdf'
    });
    return blob;
}

function RefreshEmployerPayrollHeaderLookup(e)
{
    var ldomDiv = e.context.idomActiveDiv != undefined ? e.context.idomActiveDiv : $("#" + e.context.activeDivID);
    //ldomDiv.find("#ddlBalancingStatusValue").val("'UNBL', 'NOR'").trigger("change");
    ldomDiv.find("#ltrRepReqPayments").show();
    ldomDiv.find("#capPayCheckDateTo,#txtPayCheckDateTo,#capPayCheckDate,#txtPayCheckDate").hide();
    ldomDiv.find('#btnSearch').trigger("click");
    var ldomDiv = e.context.idomActiveDiv != undefined ? e.context.idomActiveDiv : $("#" + e.context.activeDivID);
    if (e.context.activeDivID.indexOf("wfmESSEmployerPayrollHeaderHomeLookup") === 0) {
        var ldomDiv = $("#" + ns.viewModel.currentModel);
        var grid = document.getElementById("GridTable_dgrResult");
        for (var i = 0; i < grid.rows.length; i++) {

            for (var j = 0; j < grid.rows[i].cells.length; j++) {

                var cellInnerText = grid.rows[i].cells[j].innerText;
                if (cellInnerText == "Review") {
                    ldomDiv.find('#capLblText').removeAttr('class', 'hideControl');
                } else {
                    ldomDiv.find('#capLblText').attr('class', 'hideControl');
                }
            }
        }
    } 

}

//Overidden framework method to hide columns and buttons in grid.FWUpgrade : code conversion for PopulateHiddenValues() in default.aspx
MVVM.JQueryControls.GridView.prototype.beforeInitBase = MVVM.JQueryControls.GridView.prototype.beforeInit;

MVVM.JQueryControls.GridView.prototype.beforeInit = function (e) {

    this.beforeInitBase();
    if (ns.viewModel.currentModel.indexOf("wfmESSEmployeeLookup") === 0) {
        
        var ldomDiv = $("#" + ns.viewModel.currentModel);

        var dgrEmployeeResult = ldomDiv.find("#dgrResult");
        var btnOpen = ldomDiv.find("#btnOpen");
        var btnUpdateTerminateButton = ldomDiv.find("#btnUpdateTerminateButton");
        var btnTerminateButton = ldomDiv.find("#btnTerminateButton");
        var btnEnrollOther = ldomDiv.find("#btnEnrollOther");

        if (btnEnrollOther.length > 0) {
            btnEnrollOther.hide();
        }

        if (dgrEmployeeResult.length > 0 && btnOpen.length > 0 && btnUpdateTerminateButton.length > 0) {
            if (nsCommon.sessionGet("EmployeeTask") != undefined && nsCommon.sessionGet("EmployeeTask") != null) {
                if (nsCommon.sessionGet("EmployeeTask") == "UpdateTerminate") {

                    btnOpen.hide();
                    btnUpdateTerminateButton.show();
                    $.each(this.iarrGridColumns, function (key, value) {
                        if (value.title == "PERSLink ID" || value.title == "PERSLinkID") {
                            if (value.field == "dt_PERSLinkID_1_0") {
                                value.hidden = false;
                            }
                            else {
                                value.hidden = true;
                            }
                        }
                    });

                }
                else if (nsCommon.sessionGet("EmployeeTask") == "ViewUpdate") {

                    btnOpen.show();
                    btnUpdateTerminateButton.hide();
                    $.each(this.iarrGridColumns, function (key, value) {
                        if (value.title == "PERSLink ID" || value.title == "PERSLinkID") {
                            if (value.field == "dt_PERSLinkID_0_0") {
                                value.hidden = false;
                            }
                            else {
                                value.hidden = true;
                            }
                        }
                    });
                }
                else if (nsCommon.sessionGet("EmployeeTask") == "Terminate") {

                    btnOpen.hide();
                    btnUpdateTerminateButton.hide();
                    btnTerminateButton.hide();
                    $.each(this.iarrGridColumns, function (key, value) {
                        if (value.title == "PERSLink ID" || value.title == "PERSLinkID") {
                            if (value.field == "dt_PERSLinkID_2_0") {
                                value.hidden = false;
                            }
                            else {
                                value.hidden = true;
                            }
                        }
                    });

                }
                else if (nsCommon.sessionGet("EmployeeTask") == "EnrollOther457") {

                    btnOpen.hide();
                    btnUpdateTerminateButton.hide();
                    btnTerminateButton.hide();
                    btnEnrollOther.show();
                    $.each(this.iarrGridColumns, function (key, value) {
                        if (value.title == "PERSLink ID" || value.title == "PERSLinkID") {
                            if (value.field == "dt_PERSLinkID_3_0") {
                                value.hidden = false;
                            }
                            else {
                                value.hidden = true;
                            }
                        }
                    });

                }
            }
        }

    }

    
    // FMUpgrade : Added for Default page conversion of btnValidateExecuteBusinessMethod_Click method 
    if (ns.viewModel.currentModel.indexOf("wfmESSEmployeeLookup") === 0 && (ns.viewModel.previousForm.indexOf("wfmESSActiveEmployerPayrollHeaderMaintenance") === 0 || ns.viewModel.previousForm.indexOf("wfmESSActiveNoEmployerPayrollHeaderMaintenance") === 0 || ns.viewModel.previousForm.indexOf("wfmESSEmployeeMaintenance") === 0)) {

        var ldomDiv = $("#" + ns.viewModel.currentModel);
        var btnOpen = ldomDiv.find("#btnOpen");
        
        var EnrollInOther457 = ldomDiv.find("#lblEnrollInOther457");
        var btnEnrollOther = ldomDiv.find("#btnEnrollOther");

        if (btnEnrollOther.length > 0) {
            btnEnrollOther.hide();
        }
        if (nsCommon.sessionGet("EnrollInOther457")) {
            btnOpen.show();
            EnrollInOther457.removeClass("hideControl");
            nsCommon.sessionSet("EnrollInOther457", null);
            $.each(this.iarrGridColumns, function (key, value) {
                if (value.title == "PERSLink ID" || value.title == "PERSLinkID") {
                    if (value.field == "dt_PERSLinkID_0_0") {
                        value.hidden = false;
                    }
                    else {
                        value.hidden = true;
                    }
                }
            });
        }
        else {
            EnrollInOther457.addClass("hideControl");
        }
    }
    //FMUpgrade : Added for change the title 'Reporting Month' to 'Begin Month for Bonus / Retro Pay' in grvBusEmployerPayrollDetailForRetirment on button click AddNewDetails
    if (ns.viewModel.currentModel.indexOf("wfmESSActiveNoEmployerPayrollHeaderMaintenance") === 0) {
        var ldomDiv = $("#" + ns.viewModel.currentModel);

        var lReportType = ldomDiv.find("#lblReportTypeDescription").text();
        if (lReportType != undefined && lReportType != null && lReportType == "Bonus/Retro Pay") {
            $.each(this.iarrGridColumns, function (index, value) {

                if (value.title == "Reporting Month") {
                    value.title = "Begin Month for<br/> Bonus / Retro Pay";
                }
            });

        }
    }    
}
    
    

function ShowAllSearchFieldsForHeaderLookup(ldomDiv) {
    if (ldomDiv == undefined || (ldomDiv && ldomDiv.length == 0)) {
        ldomDiv = $("#" + ns.viewModel.currentModel);
    }
    ldomDiv.find("#capPayrollPaidDate,#txtPayrollPaidDateFrom,#lblPayrollPaidDateTo,#txtPayrollPaidDateTo,#lblpay_period_start_date,#txt_pay_period_start_date,#lblpay_period_end_date,#txt_pay_period_end_date").hide();
    ldomDiv.find("#capPayCheckDate,#txtPayCheckDate,#capPayCheckDateTo,#txtPayCheckDateTo,#capReceivedDate,#txtReceivedDateFrom,#capSubmittedDateTo,#txtReceivedDateTo").hide();
}
function ShowAllSearchFieldsForPayrollDetailLookup(ldomDiv) {
    if (ldomDiv == undefined || (ldomDiv && ldomDiv.length == 0)) {
        ldomDiv = $("#" + ns.viewModel.currentModel);
    }
    ldomDiv.find("#lblReportingMonthFrom,#txtReportingPeriodFrom,#lblReportingMonthTo,#txtReportingPeriodTo,#capPayPeriodStartDate,#txtPayPeriodStartDate,#capPayPeriodEndDate,#txtPayPeriodEndDate").hide();
    ldomDiv.find("#capPayCheckDateFrom,#txtPayCheckDateFrom,#capPayCheckDateTo,#txtPayCheckDateTo,#capEphSubmittedDatefrm,#txtEphSubmittedDatefrm,#capEphSubmittedDateTo,#txtEphSubmittedDateTo,#ddlMessageId,#lblError").hide();
    ns.HideControl(ldomDiv.find("#txtEphSubmittedDatefrm,#txtEphSubmittedDateTo,#txtPayPeriodStartDate,#txtPayCheckDateFrom,#txtPayPeriodEndDate,#txtPayCheckDateTo"));
    
}
function confirmation() {
    var res = confirm('Are you sure you want to Logoff, please click OK to Logoff.');
    if (res) {
        localStorage.clear();
        sessionStorage.clear();
        window.name = "";
        return ns.logoutSesssion();
    }
    else {
        return false;
    }
};
// Remove < br > and < div >tag
NeoGrid.exportDataToExcel = function (aobjData, astrFileName) {  
    var headerCell = aobjData.arrHeaderCell;
    if (headerCell != undefined && headerCell != null) {
        aobjData.arrHeaderCell.forEach(function (value, index, array) {
            //array[index] = value.replace(/<br>/ig, " ");
            array[index] = value.replace(/(<|&lt;)br\s*\/*(>|&gt;)|(<|&lt;)div\s*\/*(>|&gt;)|(<|&lt;)\s*\/*div(>|&gt;)/ig, " ");
        });
    }   
    var oo;
    if (aobjData.albnIsGrouped) {
        oo = NeoGrid.generateRowsFromGroupedJSON(aobjData);
    }
    var ranges = aobjData.albnIsGrouped && oo != undefined ? oo[1] : [];
    var data = aobjData.albnIsGrouped && oo != undefined ? oo[0] : aobjData.aData;
    var ws_name = (astrFileName != null && astrFileName != undefined && astrFileName != "" &&
        astrFileName.indexOf('.') > 0) ?
        astrFileName.substr(0, astrFileName.lastIndexOf('.')) : "ExcelJS";
    var wb = new Workbook(), ws = NeoGrid.sheet_from_array_of_arrays(data, aobjData.aobjFormatData);
    ws['!merges'] = ranges;
    wb.SheetNames.push(ws_name);
    wb.Sheets[ws_name] = ws;
    //Need to get column-wise max length from a 2D array
    var ldataLengthArray = [];
    if (data.length > 0) {
        ldataLengthArray = data[0].map(function (mycolumn, index) {
            return data.map(function (row) { return row[index].toString().length });
        });
    }
    var ldataMaxLengthArray = [];
    if (ldataLengthArray.length > 0) {
        ldataMaxLengthArray = ldataLengthArray.map(function (row) { return Math.max.apply(Math, row); });
    }
    var wscols = _.map(aobjData.arrHeaderCell, function (col, key) {
        return (col == null) ? {
            wch: 2
        } : {
            wch: (key < ldataMaxLengthArray.length) ? ldataMaxLengthArray[key] : 20
        };
    });
    ws['!cols'] = wscols;
    var wbout = XLSX.write(wb, {
        bookType: 'xlsx',
        bookSST: false,
        type: 'binary'
    });
    astrFileName = astrFileName || "Excel.xlsx";
    NeoGrid.saveAs(new Blob([NeoGrid.s2ab(wbout)], {
        type: "application/octet-stream"
    }), astrFileName);
};

function CheckUnCheckExportCols(ablnChecked) {
    var ldomDivExportCols = $("#DivExportCols");
    var selectAllCheckBox = $("#chkExportToExcelCheckUnCheckAll");
    if (ldomDivExportCols.length > 0) {
        ldomDivExportCols.find("input[type='checkbox'][value]").each(function () {
            var that = $(this);
            if (ablnChecked) {
                that.attr("checked", "checked");
                that[0].checked = true;
            }
            else {
                that.removeAttr("checked", "checked");
                that[0].checked = false;
            }

        });
    }
    if (selectAllCheckBox.length > 0) {
        if (ablnChecked) {
            selectAllCheckBox.attr("checked", "checked");
            selectAllCheckBox[0].checked = true;
        }
        else {
            selectAllCheckBox.removeAttr("checked", "checked");
            selectAllCheckBox[0].checked = false;
        }
    }
}
// F.W Upgrade6.0 override the Display message
var FM_DispalyMessage = nsCommon.DispalyMessage;
nsCommon.DispalyMessage = function (astrMessage, ActiveDivID) {
    if ((astrMessage != null && astrMessage != undefined && (ActiveDivID.indexOf("wfmESSHomeMaintenance") >= 0) && astrMessage.indexOf("All changes successfully saved") >= 0)
        || (astrMessage != null && astrMessage != undefined && (ActiveDivID.indexOf("wfmESSHomeMaintenance") >= 0) && astrMessage.indexOf(" [ All changes successfully cancelled. ]") >= 0)
        || ((ActiveDivID.indexOf("wfmUpdateTermEmploymentLOAWithoutPayWizard") >= 0) && astrMessage.indexOf(" [ Record displayed.  Please make changes and press SAVE. ]") >= 0)
        || (ActiveDivID.indexOf("wfmESSNewReportAProblemMaintenance") >= 0)
        || (astrMessage != null && astrMessage != undefined && (ActiveDivID.indexOf("wfmCreatePayrollMaintenance") >= 0) && astrMessage.indexOf("All changes successfully saved") >= 0))
        
    {
        astrMessage = "";
    }
    FM_DispalyMessage(astrMessage, ActiveDivID);
}

function confirmIgnore(strMessage, strTitle) {
    $("#popupdiv").dialog({
        title: strTitle,
        //dialogClass: 'modalBackground',
        //position: 'center',
        width: 350,
        height: 150,
        modal: true,
        minHeight: 50,
        closeText: '',
        buttons: {
            No: function () {
                $(this).dialog('close');
                return false;
            },
            Yes: function () {
                $(this).dialog('close');
                $('#btnIgnore').attr("onclientclick", "");
                $('#btnIgnore').trigger("click");
                return true;
            }
        },
    });
    $("#popupdiv").html(strMessage);
    $("#popupdiv").siblings(".ui-resizable-handle").hide();
    $(".ui-dialog-buttonset").css("padding-right", "130px");
    return false;
}

function CustomPopupAlert(message) {

    $("<div id='popUpDiv'>" + message + "</div>").dialog({
        title: 'Confirm',
        show: "slide",
        modal: true,
        resizable: false,
        height: "auto",
        top: 150.493,
        width: 450,
        buttons: {
            'Add Comment Now': function () { $(this).dialog("close"); }
        },
        create: function () {
            $(this).closest(".ui-dialog")
                .find(".ui-button") // the first button
                .addClass("button");
        }
    });
    $(".ui-dialog-titlebar-close").text('').removeClass("button");
    $(".ui-dialog-titlebar").attr("style", "background:#556080;color:White;");
}

function CheckIsPersonEnrolledInRetirementPlan() {
    if ($("#lblisEnrolledinRetrPlan").text() != undefined && $("#lblisEnrolledinRetrPlan").text().toLowerCase() == "true") {
        $('#lblAstrikRegularPayCheck,#lblLastDateOfRegularPayCheck,#lblLastDateOfRegularPayCheck,#lblLastDateOfRegularPayCheck,#txtLastDateOfRegularPayCheck,#lblNote1,#lblAstrikLastReportingMonth,#lblLastReportingMonthForRetrPlan,#txtLastReportingMonthForRetrPlan,#lblNote2,#capdv1').show();
        ns.ShowControl($('#txtLastDateOfRegularPayCheck'));
        $('#lblisEnrolledinRetrPlan').hide();
    }
    else {
        $('#lblAstrikRegularPayCheck,#lblLastDateOfRegularPayCheck,#lblLastDateOfRegularPayCheck,#lblLastDateOfRegularPayCheck,#txtLastDateOfRegularPayCheck,#lblNote1,#lblAstrikLastReportingMonth,#lblLastReportingMonthForRetrPlan,#txtLastReportingMonthForRetrPlan,#lblNote2,#capdv1').hide();
        ns.HideControl($('#txtLastDateOfRegularPayCheck'));
    }
}

var base_nsWizard_onFinishCallback = nsWizard.onFinishCallback;
nsWizard.onFinishCallback = function (obj, context) {
    base_nsWizard_onFinishCallback(obj, context);
    var ldomDiv = $("#" + ns.viewModel.currentModel);
    if (ns.viewModel && ns.viewModel.currentModel && ns.viewModel.currentModel.indexOf("wfmTerminateEmploymentWizard") == 0 && ldomDiv.find("#chkIstrIsTermsAndConditionsAgreed").is(':checked')) {
        ldomDiv.find("#btnOpenCompletion").trigger("click");
    }
    else if (ns.viewModel && ns.viewModel.currentModel && ns.viewModel.currentModel.indexOf("wfmESSMemberRecordDataWizard") == 0 && ldomDiv.find("#chkIstrIsTermsAndConditionsAgreed").is(':checked')) {
        ldomDiv.find("#btnOpenCompletion").trigger("click");
    }
}

// F.W Upgrade6.0 Ignore warning message the runtime fix this by override the DisplayError
var FM_DispalyError = nsCommon.DispalyError;
nsCommon.DispalyError = function (astrMessage, ActiveDivID, ablnScrollToTop) {
    var ldomDiv = $("#" + ns.viewModel.currentModel);
    if ((ns.viewModel.currentForm && ns.viewModel.currentForm.indexOf("wfmESSDeathNoticeMaintenance") >= 0)
        && (ldomDiv.find("li[errormessage='Address is Invalid. Please enter a valid Address.']").length > 0)) {
        ldomDiv.find("#chkCheckBox,#chkbx2,#lblLabel").css("display", "block");
    }
    else {
        if (ns.viewModel.currentForm.indexOf("wfmESSDeathNoticeMaintenance") >= 0) {
            ldomDiv.find("#chkCheckBox,#chkbx2,#lblLabel").css("display", "none");
        }
    }

    if ((ns.viewModel.currentForm && ns.viewModel.currentForm.indexOf("wfmESSEmployeeMaintenance") >= 0)
        && (ldomDiv.find("li[errormessage='Address is Invalid. Please enter a valid Address.']").length > 0)) {
        ldomDiv.find("#lblLabel8,#chkbx2,#chkCheckBox").css("display", "block");
    }
    else {
        if (ns.viewModel.currentForm.indexOf("wfmESSEmployeeMaintenance") >= 0) {
            ldomDiv.find("#lblLabel8,#chkbx2,#chkCheckBox").css("display", "none");
        }
    }

    if ((ns.viewModel.currentForm && ns.viewModel.currentForm.indexOf("wfmESSOrganizationMaintenance") >= 0)
        && (ldomDiv.find("li[errormessage='Address is Invalid. Please enter a valid Address.']").length > 0)) {
        ldomDiv.find("#chkCheckBox,#chkbx2,#lblLabel2").css("display", "block");
    }
    else {
        if (ns.viewModel.currentForm.indexOf("wfmESSOrganizationMaintenance") >= 0) {
            ldomDiv.find("#chkCheckBox,#chkbx2,#lblLabel2").css("display", "none");
        }
    }
    if ((ns.viewModel.currentForm && ns.viewModel.currentForm.indexOf("wfmESSContactMaintenance") >= 0)
       && (ldomDiv.find("li[errormessage='Address is Invalid. Please enter a valid Address.']").length > 0)) {
        ldomDiv.find("#chkCheckBox,#chkbx2,#lblLabel2").css("display", "block");
    }
    else if (ns.viewModel.currentForm.indexOf("wfmESSContactMaintenance") >= 0) {
        ldomDiv.find("#chkCheckBox,#chkbx2,#lblLabel2").css("display", "none");
    }

    if ((ns.viewModel.currentForm && ns.viewModel.currentForm.indexOf("wfmESSMemberRecordDataWizard") >= 0)
        && (ldomDiv.find("ul>li[errormessage='Address is Invalid. Please enter a valid Address.']").length > 0)) {
        ldomDiv.find("#chkCheckBox,#chkbx2,#lblLabel57").css("display", "inline-block");
    }
    else {
        if (ns.viewModel.currentForm.indexOf("wfmESSMemberRecordDataWizard") >= 0) {
            ldomDiv.find("#chkCheckBox,#chkbx2,#lblLabel57").css("display", "none");
        }
    }
 
    if ((ns.viewModel.currentForm && ns.viewModel.currentForm.indexOf("wfmESSEmployerPayrollDetailMaintenance") >= 0)) {
        ldomDiv.find("li[errorcontrolid = 'txtLastName']").css("text-decoration", "underline");
        ldomDiv.find("li[errorcontrolid = 'txtFirstName']").css("text-decoration", "underline");
        ldomDiv.find("li[errorcontrolid = 'txtSSN']").css("text-decoration", "underline");
        ldomDiv.find("li[errorcontrolid = 'ddlRecordTypeValue']").css("text-decoration", "underline");
    }   
    setTimeout(function (e) {
        if (ns.viewModel.currentForm && ns.viewModel.currentForm.indexOf("wfmCreatePayrollMaintenance") == 0 && ldomDiv.find("div[id = 'wfmCreatePayrollMaintenance0ErrorDiv'] > ul > li").length > 0) {
            var test = ldomDiv.find("div[id = 'wfmCreatePayrollMaintenance0ErrorDiv'] > ul > li[errormessage]")[0].innerText;
            if (test.indexOf("AdtPayPeriodDate - Invalid date:") >= 0)
            {
                ldomDiv.find("li[errormessage]>a").text("Reporting Month is Invalid.");
                ldomDiv.find('#txtPayPeriod').attr('class','HighlightError');
                return;
            }
        }
    }, 150);
    FM_DispalyError(astrMessage, ActiveDivID, ablnScrollToTop);
}
function IsValidDate(element)
{
    var Origvalue = element.val();
    value = Origvalue.split("/");
    if(value != undefined && value != null && value.length == 2)
    {
        return Origvalue.match("(0[123456789]|10|11|12)([/])([1-2][0-9][0-9][0-9])");
    }
    return true;
}
//F.W Upgrade6.0  added to remove the message Id
ns.FormatError = function (id, error) {
    return [error].join('');
};

//Override reset button because There is no need to reset value of benefit type dropdown 
var base_Reset_Click = nsEvents.btnReset_Click;

nsEvents.btnReset_Click = function (e) {
   
    if ((ns.viewModel.currentForm && ns.viewModel.currentForm.indexOf("wfmESSCombinedPayrollDetailLookup") == 0 || ns.viewModel.currentModel.indexOf("wfmESSRetirementCombinedPayrollDetailLookup") == 0 || ns.viewModel.currentModel.indexOf("wfmInsuranceESSCombinedPayrollDetailLookup") == 0 || ns.viewModel.currentModel.indexOf("wfmESSDeferredCompCombinedPayrollDetailLookup") == 0 ||
                 ns.viewModel.currentModel.indexOf("wfmESSServicePurchaseCombinedPayrollDetailLookup") == 0)) {
        var benefitTypeValue = $("#ddlBenefitTypeValue").val();
        var OrgID = $("#txbOrgId").val();
        var ContactID = $("#txbContactId").val();
        base_Reset_Click(e);
        $("#ddlBenefitTypeValue").val(benefitTypeValue).trigger('change');
        $("#txbOrgId").val(OrgID).trigger('change');
        $("#txbContactId").val(ContactID).trigger('change');
        return false;
    }

    else if ((ns.viewModel.currentForm &&( ns.viewModel.currentForm.indexOf("wfmESSEmployerPayrollHeaderLookup") >= 0) ||
        ns.viewModel.currentForm.indexOf("wfmESSRetirementEmployerPayrollHeaderLookup") === 0 || ns.viewModel.currentForm.indexOf("wfmESSInsuranceEmployerPayrollHeaderLookup") === 0 || ns.viewModel.currentForm.indexOf("wfmESSDefferedCompEmployerPayrollHeaderLookup") === 0 ||
            ns.viewModel.currentForm.indexOf("wfmESSServicePurchaseEmployerPayrollHeaderLookup") === 0)) {
        var benefitTypeValue = $("#ddl_benefit_type_value").val();
        var OrgID = $("#txbOrgId").val();
        var ContactID = $("#txbContactId").val();
        base_Reset_Click(e);
        $("#ddl_benefit_type_value").val(benefitTypeValue).trigger('change');
        $("#txbOrgId").val(OrgID).trigger('change');
        $("#txbContactId").val(ContactID).trigger('change');
        return false;
    }
    else {
        base_Reset_Click(e);
    }
}

nsWizard.hideStepsFromProgressBar = function (WizardObj, WizardData) {
    if (WizardData == undefined) {
        $(WizardObj.ProgressItems).find(".HideStepByRule").closest("li.HideStepByRule").show().removeClass("HideStepByRule").end().show().removeClass("HideStepByRule");
        $(WizardObj.target).find(".HideStepByRule").show().removeClass("HideStepByRule");
        $(WizardObj.steps).find(".HideStepByRule").show().removeClass("HideStepByRule");
        return;
    }
}

//temporary fix for open lookup.Need to remove this code once we get framework fix

nsEvents.btnOpenLookup_Click = function (e) {
    var lobjBtnInfo = nsCommon.GetEventInfo(e);
    var ActiveDivID = lobjBtnInfo.ActiveDivID;
    var lbtnSelf = lobjBtnInfo.lbtnSelf;
    if (ns.blnInNewMode === true && MVVMGlobal.NavigateInNewMode(ActiveDivID) !== true) {
        return false;
    }
    var ldictParams = nsCommon.GetNavigationParams(lbtnSelf, e);
    var lstrActiveForm = ldictParams.lstrActiveForm;
    var larrRows = ldictParams.larrRows;
    var FormContainer = [nsConstants.HASH, nsConstants.CENTER_SPLITTER].join('');
    if (ns.blnUseSlideoutForLookup) {
        FormContainer = nsConstants.LOOKUP_HOLDER_SELECTOR;
    }
    else {
        ns.SessionStorePageState(lstrActiveForm, "scroll", null, $(FormContainer).scrollTop());
    }
    nsCommon.sessionSet(lstrActiveForm, larrRows);
    if (larrRows != undefined && larrRows.length > 0 && larrRows[0] != undefined) {
        ns.lblnCanSetLookupParams = true;
    }
    else {
        ns.lblnCanSetLookupParams = false;
    }
    ns.iblnBtnOpenLookup_Click = true;
    var dataItem = nsCommon.GetDataItemFromDivID(lstrActiveForm);
    if ($([nsConstants.HASH, lstrActiveForm].join('')).length > 0 && dataItem != undefined) {
        var NodeToSelect = ns.tabsTreeView.findByUid(dataItem.uid);
        if (NodeToSelect.length !== 0) {
            nsCommon.SetLookupFormParams(FormContainer, lstrActiveForm);
            ns.tabsTreeView.select(NodeToSelect, true);
            if (larrRows != undefined && larrRows.length > 0 && larrRows[0] != undefined) {
                ns.iblnTriggeredSearch = true;
                $([nsConstants.HASH, lstrActiveForm, nsConstants.SPACE_HASH, "btnSearch"].join('')).trigger('click');
            }
            if (dataItem.parentNode() != undefined)
                MVVMGlobal.LoadBreadCrums(lstrActiveForm, dataItem);
        }
    }
    else {
        ns.spaRouter.navigate(["/spa/", lstrActiveForm, '/0'].join(''));
    }
}

//FW upgrade :: Override F/w code to scroll up the page while opening (PIR - 11236)
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
    var oFirstControl = ldomDiv.find(":not([gridid]):not([listviewid]):not(.filter):not(input.check_row):not(input.s-grid-check-all):not(input.ellipse-input-pageHolder):not(input.s-grid-common-filterbox):input[type !='button']:input[type !='submit']:input[type !='image']:input[sfwretrieval !='True']:input[sfwretrieval !='true']:visible:enabled:first");
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
        if ((oFirstControl != undefined) && (oFirstControl.length >= 0) && ns.viewModel.currentModel.indexOf("wfmESSEmployerPayrollHeaderMaintenance") != 0) {

        if ((ns.viewModel.currentModel && ns.viewModel.currentModel.indexOf("wfmESSDeathNoticeMaintenance") >= 0 || (ns.viewModel.currentModel && ns.viewModel.currentModel.indexOf("wfmESSNewReportAProblemMaintenance") >= 0))) {
            ldomDiv.find('#txtPersonId').trigger("focus");
            ldomDiv.find('#txtCallbackPhone').trigger("focus");
        }
        else {
            oFirstControl.trigger("focus");  
        }
    }
    }
    ns.blnLoading = false;
}
//Left menu not to be collapsible
MVVMGlobal.toggleMenuHeader = function(adomSpan) {
    return false;
};

var base_SPARouteGet = MVVMGlobal.SPARouteGet;
MVVMGlobal.SPARouteGet = function (event, urlParams) {
    if (sessionStorage.getItem("IsBrowserBackButtonPressed") == "True") {
        sessionStorage.setItem("IsBrowserBackButtonPressed", "False");
        location.replace(ns.SiteName + "/Account/wfmLoginEI");
    }
    else { 
        base_SPARouteGet(event, urlParams);
    }    
}

//F/W upgade PIR 12104
function GetMessageCount() {
    var ldomDiv = $("#" + ns.viewModel.currentModel);
    var aintPersonID = ldomDiv.find("#PersonID").text();
    var Parameters = {};
    Parameters["aintPersonID"] = aintPersonID;
    var AlertMessages = {};
    AlertMessages = nsCommon.SyncPost("GetESSUnreadMessagesCountNeo", Parameters, "POST");
    $("a[id='HLMSSMessageBoard']").text("You have " + AlertMessages + " messages");
        
    if (AlertMessages > 0) {
        $("#imgNewMsg").show();
    }
    else {
        $("#imgNewMsg").hide();
    }    
}


function LoadBreadCrums(astrActiveDivID, aDataItem, adomDiv, adomStepDiv) {
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
        var dataItem = aDataItem;
        if (dataItem == undefined || dataItem.divID !== astrActiveDivID) {
            dataItem = nsCommon.GetDataItemFromDivID(astrActiveDivID);
        }
        var tempArray = [];
        while (dataItem !== undefined) {
            tempArray.push(dataItem);
            dataItem = dataItem.parentNode();
        }
        larrHtml.push("<input type='button' aria-label='Go To Form Contents' class='GoToLinksTrigger'/><div class='GoToLinks'><strong>", Sagitec.DefaultText.TEXT_CONTENTS, "</strong><hr/><br/><div class='s-divGotoLinkUlContents'>", ns.PopulateGoToLinks(tempArray[0].divID, adomStepDiv), "</div></div>", '<table role="presentation"><tr><td>', "<ul class='breadcrumb'>");
        var star = "";
        var tempArrLen = tempArray.length;
        if (tempArrLen > lintBreadCrumsToDisplay) {
            tempArrLen = tempArrLen - (tempArrLen - lintBreadCrumsToDisplay);
        }
        for (var i = tempArrLen - 1; i >= 0; i--) {
            star = "";
            var lblnSetUnsavedFormIcon = true;
            var fnSetUnSavedIcon = nsUserFunctions["SetUnSavedFormIcon"];
            if (typeof fnSetUnSavedIcon === 'function') {
                var lobjContext = {
                    activeDivID: tempArray[i].divID,
                };
                var e = {};
                e.context = lobjContext;
                lblnSetUnsavedFormIcon = fnSetUnSavedIcon(e);
            }
            if (ns.DirtyData[tempArray[i].divID] !== undefined && lblnSetUnsavedFormIcon !== false)
                star = "*";
            var title = tempArray[i].title;
            if (title == undefined && tempArray[i].divID.indexOf("wfmrul") === 0) {
                title = "Rule Result";
            }
            var croptitle = nsCommon.GetCropedTitleForBreadcrumbs(title, nsConstants.BREADCRUM_CROP_TITLE_INDEXES.indexOf(i) >= 0, i, tempArray);
            var linkedToString = ["class='crumLinks'   linkedTo='", tempArray[i].divID, "' "].join('');
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
    delete nsUserFunctions.iarrFormsToSkipFromUnSavedIcon["wfmESSEmployerPayrollDetailMaintenance"];
}
MVVMGlobal.LoadBreadCrums = LoadBreadCrums;

//Code added for collapse handburg menu for window resize.
window.addEventListener('resize', function (event) {
    var width = $(window).width();
    if ($(".page-slideout-body").hasClass('page-slideout-body-collaped')) {
        if (width >= 1025) {
            $(".web-view-head").css("left", "-262px");
            $(".crumDiv").css("margin-left", "0px");
        } else {
            $(".web-view-head").css("left", "10px");
            $(".crumDiv").css("margin-left", "30px");
        }
    }
    else if ($(".page-slideout-body").hasClass('page-slideout-body-fixed')) {
        $(".web-view-head").css("left", "10px");
        $(".crumDiv").css("margin-left", "30px");
    }
});

//nsUserFunctions.DisplayBreadCrums = function (e) { return false; }
function dataLossPopUpMessage() {
    var ObjFormChanged = $(ns.NotificationModel.DirtyForms.DirtyFormList).find("a[divid]").attr("divid");
    if (ObjFormChanged == null || ObjFormChanged == undefined)
        return true;
    if (ObjFormChanged != undefined && (ObjFormChanged.contains('Wizard') || ns.viewModel.currentForm.indexOf("wfmESSActiveNoEmployerPayrollHeaderMaintenance") != 0
        || ns.viewModel.currentForm.indexOf("wfmESSActiveEmployerPayrollHeaderMaintenance") != 0)) {
        var result = confirm("Are you sure you want to navigate away from this page? All information will be lost unless it is submitted.");
        if (result) {
            if (ObjFormChanged != null || ObjFormChanged != undefined) {
                delete ns.DirtyData[nsCommon.GetActiveDivId()];
            }

            return true;
        }
        else {
            return false;
        }
    }
    return true;
}
var Basebtn_GoPreviousPage = nsEvents.btn_GoPreviousPage;
nsEvents.btn_GoPreviousPage = function (e) {
    var cofirm = dataLossPopUpMessage();
    if (cofirm) {
        Basebtn_GoPreviousPage(e);
    }
    $(document).on("click", ".formNavigationPrev, .formNavigationNext", function () {
        var ldomDiv = $("#" + ns.viewModel.currentModel);
        if (ns.viewModel.currentModel.indexOf("wfmESSEmployerPayrollDetailMaintenance") == 0) {
            ldomDiv.find("#btnCancel").trigger("click");
        }
        else if (ns.viewModel.currentModel.indexOf("wfmESSEmployerPayrollHeaderMaintenance") == 0) {
            ldomDiv.find("#btnCancel").trigger("click");
        }
    });
}
MVVMGlobal.Extend_MonthYear = function (DivToApplyUI, astrActiveDivID) {
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
function PopulateToDateAcaCert() {
    try {
        var ldomTargetDiv = $("#" + ns.viewModel.currentModel);
        if (ldomTargetDiv.length > 0) {
            var lddlMethodElement = ldomTargetDiv.find('#ddlMethod');
            var lddlMeasureElement = ldomTargetDiv.find('#ddlLbMeasure');
            if (lddlMethodElement.length > 0 && lddlMeasureElement.length > 0) {
                var lddlMethodElementValue = lddlMethodElement.val();
                var lddlMeasureElementValue = lddlMeasureElement.val();
                if (lddlMethodElementValue == "LBM" & lddlMeasureElementValue == "NEWE") {
                    var ltxtFromDate = ldomTargetDiv.find('#dtpNewEligibleFromDate');
                    if (ltxtFromDate.length > 0) {
                        var ltxtFromDateValue = ltxtFromDate.val();
                        var validformat = /^\d{2}\/\d{2}\/\d{4}$/ //Basic check for format validity
                        if (!validformat.test(ltxtFromDateValue)) {

                        }
                        else { //Detailed check for valid date ranges
                            var monthfield = ltxtFromDateValue.split("/")[0];
                            var dayfield = ltxtFromDateValue.split("/")[1];
                            var yearfield = ltxtFromDateValue.split("/")[2];
                            var dayobj = new Date(yearfield, monthfield - 1, dayfield)
                            if ((dayobj.getMonth() + 1 != monthfield) || (dayobj.getDate() != dayfield) || (dayobj.getFullYear() != yearfield))
                                alert("Invalid Day, Month, or Year range detected. Please correct and submit again.")
                            else {
                                var ltxtFromDate = ldomTargetDiv.find('#dtpNewEligibleToDate');
                                if (ltxtFromDate.length > 0) {
                                    var ldtcurrentDate = new Date(ltxtFromDateValue);
                                    var ldtNextYearDate = new Date(ldtcurrentDate.setFullYear(ldtcurrentDate.getFullYear() + 1));
                                    ldtNextYearDate.setDate(ldtNextYearDate.getDate() - 1);
                                    var lintmonth = ldtNextYearDate.getMonth() + 1;
                                    var ldteToDate = ('0' + lintmonth).slice(-2) + "/" + ('0' + ldtNextYearDate.getDate()).slice(-2) + "/" + ldtNextYearDate.getFullYear();
                                    ltxtFromDate.val(ldteToDate);
                                    ltxtFromDate.attr("disabled", "disabled")
                                }
                            }
                        }
                    }

                }
            }
        }
    }
    catch (ex) {
        console.log(ex);
    }
}
$(document).on("change", "#ddlLbMeasure, #ddlMethod", function (e) {
    var ldomDiv = $("#" + ns.viewModel.currentModel);
    if (ns.viewModel.currentModel.indexOf("wfmEssACAEligibilityCertificationMaintenance") === 0) {
        var ltxtFromDate = ldomDiv.find('#dtpNewEligibleToDate');
        ltxtFromDate.val('');
        ltxtFromDate.attr("disabled", "disabled");
    }
});

function CustomCreateReportPopupAlert(message) {

    $("<div id='popUpDiv'>" + message + "</div>").dialog({
        title: 'Confirm',
        show: "slide",
        modal: true,
        resizable: false,
        height: "auto",
        width: 450,
        buttons: {
            'Enter Wages Now': function () {  $(this).dialog("close"); }
        },
        create: function () {
            $(this).closest(".ui-dialog")
                .find(".ui-button") // the first button
                .addClass("button");
        }
    });
    $(".ui-dialog-titlebar-close").text('').removeClass("button");
    $(".ui-dialog-titlebar").attr("style", "background:#556080;color:White;");
}
