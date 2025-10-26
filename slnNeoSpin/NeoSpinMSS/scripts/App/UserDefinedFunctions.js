var nsUserFunctions = nsUserFunctions || {};


RefreshGridForInsuranceWizard: false,

nsUserFunctions = {
    iblnMenuItemClicked: false,
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
    PrintCenterMiddle: function (event) {
        $('#ContentSplitter').jqprint(); // This is the div that has to be printed.
        event.preventDefault();
        return false;
    },
    ShownOTPPanel: function (event) {
        $(".actionBar").hide();
        return false;
    },
    DisplayAppSubmitText: function (event) {
        var ldomDiv = $("#" + ns.viewModel.currentModel);
        var BenTypeValue = ldomDiv.find("#ddlSelBenType").val();
        var cntlLblApplicationSubmittedText = ldomDiv.find("#lblApplicationSubmittedText");
        if (cntlLblApplicationSubmittedText != null && cntlLblApplicationSubmittedText.text().length > 0) {
            CustomPopupAlert(cntlLblApplicationSubmittedText.text());
        }
        return true;
    },
    DisplayADECSubmitText: function (event) {
        var ldomDiv = $("#" + ns.viewModel.currentModel);
        var cntlLblADECSubmittedText = ldomDiv.find("#lblADECSubmittedText");
        if (cntlLblADECSubmittedText != null && cntlLblADECSubmittedText.text().length > 0) {
            if (confirm(cntlLblADECSubmittedText.text())) {
                ldomDiv.find("#btnUpdatePersonAccountADECPercentage").attr("disabled", "disabled");
                return true;
            }
            else {
                ldomDiv.find("#btnUpdatePersonAccountADECPercentage").removeAttr("disabled");
                return false;
            }

        }
    },
    DisplayRefundForOtherPlan: function (event) {
        var ldomDiv = $("#" + ns.viewModel.currentModel);
        var BenTypeValue = ldomDiv.find("#ddlSelBenType").val();
        var cntlLblMemberHasMultiplePlan = ldomDiv.find("#lblMemberHasMultiplePlan");
        var cntlLblRefundForOtherPlan = ldomDiv.find("#lblRefundForOtherPlan");
        if (cntlLblMemberHasMultiplePlan != null && cntlLblMemberHasMultiplePlan.text().length > 0 && BenTypeValue == "RFND") {
            if (cntlLblRefundForOtherPlan != null && cntlLblRefundForOtherPlan.text().length > 0)
            alert(cntlLblRefundForOtherPlan.text());
        }
        return true;
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

    // Framework version 6.0.2.1
    //GetChartTitle: function (e) {
    //    e.data 		// DomainModel Object
    //    e.chartId 	// Id of the chart
    //    var lvarAmount = 0;
    //    if (e.chartId == "chrGrossBenefitChart")
    //    {
    //        if (e.data.DetailsData.chrGrossBenefitChart.Records[0].idecGrossAmount != undefined)
    //        {
    //            lvarAmount = e.data.DetailsData.chrGrossBenefitChart.Records[0].idecGrossAmount;
    //        }
    //        return "Gross Benefit :" + "$" + lvarAmount;
    //    }
    //    else if (e.chartId == "chrMembershipAccountChart")
    //    {
    //        if (e.data.DetailsData.chrMembershipAccountChart.Records[0].idecCurrentMembershipBalance != undefined)
    //        {
    //            lvarAmount = e.data.DetailsData.chrMembershipAccountChart.Records[0].idecCurrentMembershipBalance;
    //        }
    //        return "Account Balance :" + "$" + lvarAmount;
    //    }
    //    else if (e.chartId == "chrBenefitChart")
    //    {
    //        return "BenefitChart ";
    //    }
    //    else if (e.chartId == "chrReplacementRatio")
    //    {
    //        return "Replacement Ratio";
    //    }
    //    else {
    //        return "Dynamic Title";
    //    }
    //},

    //AlertMessages: {
    //    "Messages": { "MessageCount": 0, "MessageList": "", "MessageVisible": true, "UnReadMessageCount": 0, "unreadMessageVisible": true }
    //},
    //TempSelect: function (e) {
    //    alert(e.context.value);
    //},

    //FileUploadSuccess: function (e) {
    //},

    InitilizeUserDefinedEvents: function (e) {
        //PIR 25699 set session item if tax withholding nav item is clicked
        $(document).on("click", "#MenuUl li[formid='wfmMSSPayeeAccountsMaintenance']:last", function () {
            nsCommon.sessionSet("IsTaxWithholdingNavItemClicked", "true");
        });
        
      $(document).on("change", "#ddlPlanEnrollmentOptionValueHealth", function () {
          setBenAppWizardVisibility();
       });
       $(document).on("change", "#ddlPlanEnrollmentOptionValueDental", function () {
           setBenAppWizardVisibility();
       });
       $(document).on("change", "#ddlPlanEnrollmentOptionValueVision", function () {
           setBenAppWizardVisibility();
        });
        $(document).on("click", "#RFAK", function () {
            $("#btnRolloverOptionPdfByLink").trigger("click");
        });
       $(document).on("change", "#ddlIsDependentMedicareEligible", function () {        
            var medicare = $('.Medicare');
            var esrd = $('.ESRD');
            var meddepspanel = $('.MedDepsPanel');
            var medicarePanel = $('.MedicarePanel');
            setMedicarePanelVisibility(medicare, esrd, meddepspanel, medicarePanel);           
       });       
        $(document).on("click", ".HomeLink", function () {
            ActiveDivID = $(this).closest('div[id^="wfm"]')[0].id;
            var lstrRelatedControl = GetControlAttribute($(this), "sfwRelatedControl", ActiveDivID);

            if (lstrRelatedControl != null) {
                var control = $("#" + ActiveDivID).find("#" + lstrRelatedControl);
                if (control.length > 0) {
                    control.trigger("click");
                }
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
           
        $(document).on("click", "#lnkSwitchMember", function (e) {
            var cofirm = dataLossPopUpMessage();
            if (!cofirm) {
                e.preventDefault();
            }
        });
        
        $(document).on("click", "#lnkSwitchMemberAccount", function (e) {      
            var cofirm = dataLossPopUpMessage();
            if (!cofirm) {
                e.preventDefault();
            }
        });

        // 11335 MSS -UI Issue on wfmMSSProfileMaintenance screen (if click on save button input box value select)
        $(document).on("click", "#btnSave", function () {
            if (ns.viewModel.currentModel.indexOf("wfmMSSProfileMaintenance") === 0) {
                $("#txtEmailAddress").select();
            }
        });
      
        //Function to make the text entered in Name, Address City etc textbox in Proper/Title case
        $(document).on('blur', '.titlecase', function (e) {
            //$(this)[0].value = $(this)[0].value.charAt(0).toUpperCase() + $(this)[0].value.slice(1);
            var name = $(this).val().split(" ");
            var properName = new Array();
            for (x in name) {
                properName.push(name[x].charAt(0).toUpperCase() + name[x].substr(1).toLowerCase());
            }
            $(this)[0].value = properName.join(" ").trim();

        });    

        //Stop to collapse left side menu.
        //$(document).on("click", "li[keytitle='Your Account(s)']>a, li[keytitle='Related Tasks']>a,span[keytitle='Your Account(s)'],span[keytitle='Related Tasks'],li[keytitle='Retirement']>a", function () {
        //    return false;
        //});
       
        $(document).on("click", "#chkLintSameAsPersonAddressNew", function (e) { 
            var ldomDiv = $("#" + ns.viewModel.currentModel);
            if (ns.viewModel.currentModel.indexOf("wfmMSSUpdateContactMaintenance") === 0) {
                chkLintSameAsPersonAddressNewVisibilty(ldomDiv);
            }
        });

        //Code added for collapse menu after click on left side menu item for ipad and below resolution devices.
        $(document).on("click", ".toggleForMobile .sub-menu li a", function (e) {
            $("#btnHeaderSlideoutMenuDivCollapseExpand").trigger("click");
        });
       
        $(document).off("change.ddlAddressCountryValue").on('change.ddlAddressCountryValue', "#ddlAddressCountryValue", function (e) {
            var ldomDiv = $("#" + ns.viewModel.currentModel);
            var lddlAddressCountryValue = ldomDiv.find("#ddlAddressCountryValue");
            if (ns.viewModel.currentModel.indexOf("wfmMSSUpdateContactMaintenance") === 0) {
                if (lddlAddressCountryValue != null) {
                    AddressCountryValueVisibility(ldomDiv);
                }
            }
        });   

        $(document).off("change.ddlAddrCountryValue").on('change.ddlAddrCountryValue', "#ddlAddrCountryValue", function (e) {
            var ldomDiv = $("#" + ns.viewModel.currentModel);
            var ddlCountryValue = ldomDiv.find("#ddlAddrCountryValue");
            if (ns.viewModel.currentModel.indexOf("wfmMSSAddressMaintenance") === 0) {
                if (ddlCountryValue.val() == "0001") {
                    ldomDiv.find("#capForeignProvince,#txtForeignProvince,#capForeignPostalCode,#txtForeignPostalCode").css("display", "none");
                    ldomDiv.find("#capAddrZipCode,#txtAddrZipCode,#txtAddrZip4Code,#capAddrStateValue,#ddlAddrStateValue").css("display", "block");
                }
                else {
                    ldomDiv.find("#capAddrZipCode,#txtAddrZipCode,#txtAddrZip4Code,#capAddrStateValue,#ddlAddrStateValue").css("display", "none");
                    ldomDiv.find("#capForeignProvince,#txtForeignProvince,#capForeignPostalCode,#txtForeignPostalCode").css("display", "block");
                }
               
            }
        });
        $(document).on('click', "#rblRBDeferredComp_0", function (e) {
            var ldomDiv = $("#" + ns.viewModel.currentModel);
            if (ldomDiv.find('#rblRBDeferredComp_0').length > 0 && ldomDiv.find('#rblRBDeferredComp_0')[0].checked == true) {
                ldomDiv.find("#pnlNo").hide();
                ldomDiv.find("#pnlYes").show();
            }                            
        });
        $(document).on("change", "#rblRadioButtonList", function (e) {
	    var ldomDiv = $("#" + ns.viewModel.currentModel);
	    if (ns.viewModel.currentModel.indexOf("wfmMSSSelectRetPlanMaintenance") === 0) {
	        ldomDiv.find("#lblLabel").text("");
	    }
        });

        $(document).on("change", "#ddlSelBenType", function (e) {
            var ldomDiv = $("#" + ns.viewModel.currentModel);
            var item = $(this);
            if (item.val() == "RFND")
            {
                ldomDiv.find("#lblBenOption").hide();
                ldomDiv.find("#ddlSelBenOption").hide();
                ldomDiv.find('#btnMainAndPublicSafetyRetirementOptionsPdf').hide();                
            }
            if (item.val() == "RETR") {
                var NRD = ldomDiv.find("#lblNRDForRetirementDate").html();
               
                if (NRD != null && NRD != undefined && NRD != NaN && NRD != "" && ldomDiv.find("#txtRetEffecDate").val() == "") {
                    var spitdate = NRD.split('/');
                    var month;
                    var year;
                    if (spitdate.length > 0) {
                        month = parseInt(spitdate[0]);
                        year = parseInt(spitdate[2]);
                        if (month < 10)
                            month = "0" + month;
                    }
                    ldomDiv.find("#txtRetEffecDate").val(month + "/" + year);
                    GetBenOptionsByEffectiveDate(ldomDiv);
                }
            }
        });
     
        $(document).off("change.ddlIstrSupplementalParticipation").on('change.ddlIstrSupplementalParticipation', "#ddlIstrSupplementalParticipation", function (e) {
            var ldomDiv = $("#" + ns.viewModel.currentModel);
            var ddlValue = ldomDiv.find("#ddlIstrSupplementalParticipation");
            var SuppAmount = ldomDiv.find("#lblSuppAmount").text();

            if (ns.viewModel.currentModel.indexOf("wfmLifeAnnualEnrollmentWizard") == 0 && $("#wzsStep1").is(":visible")) {

                if (ddlValue.val() == "N" || ddlValue.val() == "Y") {
                    ldomDiv.find("#ddlIstrSupplementalParticipation1").addClass("hideControl");
                    ldomDiv.find("#lblSuppParticipation1").addClass("hideControl");
                    ldomDiv.find("#ddlIstrSupplementalParticipation2").addClass("hideControl");
                    ldomDiv.find("#lblSuppParticipation2").addClass("hideControl");
                }

                if (ddlValue.val() == "O" && SuppAmount == 0) {
                    ldomDiv.find("#ddlIstrSupplementalParticipation1").addClass("hideControl");
                    ldomDiv.find("#lblSuppParticipation1").addClass("hideControl");                    
                }
                if (ddlValue.val() == "O" && SuppAmount > 0) {
                    ldomDiv.find("#ddlIstrSupplementalParticipation1").removeClass("hideControl");
                    ldomDiv.find("#lblSuppParticipation1").removeClass("hideControl");
                }
            }

        });

        $(document).off("change.ddlIstrSupplementalParticipation1").on('change.ddlIstrSupplementalParticipation1', "#ddlIstrSupplementalParticipation1", function (e) {
            var ldomDiv = $("#" + ns.viewModel.currentModel);
            var ddlValue = ldomDiv.find("#ddlIstrSupplementalParticipation1");
            var DepAmount = ldomDiv.find("#lblDepAmount").text();

            if (ns.viewModel.currentModel.indexOf("wfmLifeAnnualEnrollmentWizard") == 0 && $("#wzsStep1").is(":visible")) {

                if (ddlValue.val() == "N" || ddlValue.val() == "Y") {
                    ldomDiv.find("#ddlIstrSupplementalParticipation2").addClass("hideControl");
                    ldomDiv.find("#lblSuppParticipation2").addClass("hideControl");
                }

                if (ddlValue.val() == "O" && DepAmount == 0) {
                    ldomDiv.find("#ddlIstrSupplementalParticipation2").addClass("hideControl");
                    ldomDiv.find("#lblSuppParticipation2").addClass("hideControl");
                }
                if (ddlValue.val() == "O" && DepAmount > 0) {
                    ldomDiv.find("#ddlIstrSupplementalParticipation2").removeClass("hideControl");
                    ldomDiv.find("#lblSuppParticipation2").removeClass("hideControl");
                }
            }
        });

        $(document).off("change.ddlIstrSupplementalDependentParticipation1").on('change.ddlIstrSupplementalDependentParticipation1', "#ddlIstrSupplementalDependentParticipation1", function (e) {
            var ldomDiv = $("#" + ns.viewModel.currentModel);
            var ddlValue = ldomDiv.find("#ddlIstrSupplementalDependentParticipation1");
            var DepAmount = ldomDiv.find("#lblDepAmountStep2").text();

            if (ns.viewModel.currentModel.indexOf("wfmLifeAnnualEnrollmentWizard") == 0 && $("#wzsStep2").is(":visible")) {

                if (ddlValue.val() == "N" || ddlValue.val() == "Y") {
                    ldomDiv.find("#ddlSpouseSupp").addClass("hideControl");
                    ldomDiv.find("#lblSpouseSupp").addClass("hideControl");
                }

                if (ddlValue.val() == "O" && DepAmount == 0) {
                    ldomDiv.find("#ddlSpouseSupp").addClass("hideControl");
                    ldomDiv.find("#lblSpouseSupp").addClass("hideControl");
                }
                if (ddlValue.val() == "O" && DepAmount > 0) {
                    ldomDiv.find("#ddlSpouseSupp").removeClass("hideControl");
                    ldomDiv.find("#lblSpouseSupp").removeClass("hideControl");
                }
            }
        });


        $(document).on("keyup blur", "#txtRetEffecDate", function (e) {
            var ldomDiv = $("#" + ns.viewModel.currentModel);
            if (ldomDiv.find("#ddlSelBenType").val() != "RFND")
                GetBenOptionsByEffectiveDate(ldomDiv);
        });
        //$(document).on("keyup blur", "#txtPartialAmount", function (e) {
        //    var ldomDiv = $("#" + ns.viewModel.currentModel);
        //    SwitchPercent(ldomDiv);
        //});
        //$(document).on("blur", "#txtPercentageOfNetAmount", function (e) {
        //    var ldomDiv = $("#" + ns.viewModel.currentModel);
        //    SwitchAmount(ldomDiv);
        //});
        $(document).on("change", "#chkSameAsDepositeInfo", function (e) {

            if (ns.viewModel.currentModel.indexOf("wfmMSSBenefitApplicationWizard") === 0) {
                var ldomDiv = $("#" + ns.viewModel.currentModel);
                if (ldomDiv.find("#chkSameAsDepositeInfo").length > 0 && ldomDiv.find("#chkSameAsDepositeInfo")[0].checked != null){
                    if(ldomDiv.find("#chkSameAsDepositeInfo")[0].checked) {
                        ldomDiv.find("#txtRoutNumberIns").val(ldomDiv.find("#txtRNAppWizDisOne").val()).attr("disabled", "disabled");
                        ldomDiv.find("#lblBankNameInsPay").text(ldomDiv.find("#lblBNAppWizDisOne").text());
                        ldomDiv.find("#txtBankNameInsPay").val(ldomDiv.find("#txtBNAppWizDisOne").val()).attr("disabled", "disabled");
                        ldomDiv.find("#txtActNoInsPay").val(ldomDiv.find("#txtACHAccountNumber").val()).attr("disabled", "disabled");
                        ldomDiv.find("#ddlDropDownListInsPay").val(ldomDiv.find("#ddlBankAccountTypeValue").val()).attr("disabled", "disabled");
                        setBankNameVisibility($('#lblBankNameInsPay'), $('#txtBankNameInsPay'), $('#txtActNoInsPay'));
                    }
                    else {
                        ldomDiv.find("#txtRoutNumberIns").val("").removeAttr("disabled");
                        ldomDiv.find("#lblBankNameInsPay").text("");
                        ldomDiv.find("#txtBankNameInsPay").val("").removeAttr("disabled");
                        ldomDiv.find("#txtActNoInsPay").val("").removeAttr("disabled");
                        ldomDiv.find("#ddlDropDownListInsPay").val("").removeAttr("disabled");
                        setBankNameVisibility($('#lblBankNameInsPay'), $('#txtBankNameInsPay'), $('#txtActNoInsPay'));
                    }
                }
            }
        });
        $(document).on("change", "#ddlRolloverOptionValue", function (e) {

            if (ns.viewModel.currentModel.indexOf("wfmMSSBenefitApplicationWizard") === 0) {
                var ldomDiv = $("#" + ns.viewModel.currentModel);
                var item = $(this);
                var NonTaxableAmount = ldomDiv.find("#lblIdecMSSTotalNonTaxable").text();
                NonTaxableAmount = Number(NonTaxableAmount.replace(/[^0-9.-]+/g, ""));
                if (item.val() == "ALOG" && NonTaxableAmount > 0 ) {
                    ldomDiv.find("#lblRolloverText").show();
                    ldomDiv.find("#lblRolloverTextSumry").show();
                    ldomDiv.find("#lblRolloverTextPrnt").show();
                }
                else {
                    ldomDiv.find("#lblRolloverText").hide();
                    ldomDiv.find("#lblRolloverTextSumry").hide();
                    ldomDiv.find("#lblRolloverTextPrnt").hide();
                }
            }
        });
        
        $(document).on("change", "#ddlRolloverOptionValue", function (e) {

            if (ns.viewModel.currentModel.indexOf("wfmMSSBenefitApplicationWizard") === 0) {
                var ldomDiv = $("#" + ns.viewModel.currentModel);
                var item = $(this);
                var NonTaxableAmount = ldomDiv.find("#lblIdecMSSTotalNonTaxable").text();
                NonTaxableAmount = Number(NonTaxableAmount.replace(/[^0-9.-]+/g, ""));
                
                    if (item.val() == "ALOT" && NonTaxableAmount > 0 || item.val() == "DLOT" || item.val() == "PROT") {
                        ldomDiv.find("#pnlACHDepositinfo").show();
                        ldomDiv.find("#capRoutingNo").show();
                        ldomDiv.find("#capBankName").show();
                        ldomDiv.find("#txtRNAppWizDisOne").show();
                        ldomDiv.find("#lblBNAppWizDisOne").show();
                        ldomDiv.find("#txtBNAppWizDisOne").show();
                        ldomDiv.find("#capACHAccountNumber").show();
                        ldomDiv.find("#txtACHAccountNumber").show();
                        ldomDiv.find("#capBankAccountTypeValue").show();
                        ldomDiv.find("#ddlBankAccountTypeValue").show();
                    }
                    else {
                        ldomDiv.find("#pnlACHDepositinfo").hide();
                        ldomDiv.find("#capRoutingNo").hide();
                        ldomDiv.find("#capBankName").hide();
                        ldomDiv.find("#txtRNAppWizDisOne").hide();
                        ldomDiv.find("#lblBNAppWizDisOne").hide();
                        ldomDiv.find("#txtBNAppWizDisOne").hide();
                        ldomDiv.find("#capACHAccountNumber").hide();
                        ldomDiv.find("#txtACHAccountNumber").hide();
                        ldomDiv.find("#capBankAccountTypeValue").hide();
                        ldomDiv.find("#ddlBankAccountTypeValue").hide();

                    }
                
            }
        });
        $(document).on("change", "#ddlRolloverOptionValue", function (e) {

            if (ns.viewModel.currentModel.indexOf("wfmMSSBenefitApplicationWizard") === 0) {
                var ldomDiv = $("#" + ns.viewModel.currentModel);
                var item = $(this);
                var NonTaxableAmount = ldomDiv.find("#lblIdecMSSTotalNonTaxable").text();
                NonTaxableAmount = Number(NonTaxableAmount.replace(/[^0-9.-]+/g, ""));
                if (item.val() == "ALOT" && NonTaxableAmount > 0) {
                    ldomDiv.find("#lblRolloverOptionText").show();
                    
                }
                else {
                    ldomDiv.find("#lblRolloverOptionText").hide();
                    

                }
            }
        });
        $(document).on("change", "#ddlIstrRefundDistribution", function (e) {

            if (ns.viewModel.currentModel.indexOf("wfmMSSBenefitApplicationWizard") === 0) {
                var ldomDiv = $("#" + ns.viewModel.currentModel);
                var item = $(this);
                var RefundDistribution = ldomDiv.find("#ddlIstrRefundDistribution").val();
                if (item.val() == "RPAR") {
                    ldomDiv.find("#pnlRolloverDetail").show();
                    
                } else {
                    ldomDiv.find("#pnlRolloverDetail").hide();

                }
            }
        });
       
        
        $(document).on("change", "#ddlIstrRefundDistribution", function (e) {

            if (ns.viewModel.currentModel.indexOf("wfmMSSBenefitApplicationWizard") === 0) {
                var ldomDiv = $("#" + ns.viewModel.currentModel);
                var item = $(this);
                var RefundDistribution = ldomDiv.find("#ddlIstrRefundDistribution").val();
                if (item.val() == '') {
                    ldomDiv.find("#pnlACHDepositinfo").hide();
                    ldomDiv.find("#lblACHDepositeInformation").hide();
                    ldomDiv.find("#capRoutingNo").hide();
                    ldomDiv.find("#capBankName").hide();
                    ldomDiv.find("#txtRNAppWizDisOne").hide();
                    ldomDiv.find("#lblBNAppWizDisOne").hide();
                    ldomDiv.find("#txtBNAppWizDisOne").hide();
                    ldomDiv.find("#capACHAccountNumber").hide();
                    ldomDiv.find("#txtACHAccountNumber").hide();
                    ldomDiv.find("#capBankAccountTypeValue").hide();
                    ldomDiv.find("#ddlBankAccountTypeValue").hide();
                    ldomDiv.find("#pnlRolloverDetail").hide();
                    ldomDiv.find("#pnlLetterofAcceptance").hide();

                } 
            }
        });
        
        $(document).on("change", "#ddlRolloverTypeValue", function (e) {
            
            if (ns.viewModel.currentModel.indexOf("wfmMSSBenefitApplicationWizard") === 0) {
                var ldomDiv = $("#" + ns.viewModel.currentModel);
                var RolloverTypeValue = ldomDiv.find("#ddlRolloverTypeValue option:selected");
                if (RolloverTypeValue.val() == "RIRA") {
                    ldomDiv.find("#lblStateTax").show();
                    ldomDiv.find("#lblStateTaxReqInformation").show();
                    ldomDiv.find("#ddlStateTax").show();
                    ldomDiv.find("#ddlStateTax").val("STWH");                    
                }
                else {                    
                    ldomDiv.find("#lblStateTax").hide();
                    ldomDiv.find("#lblStateTaxReqInformation").hide();
                    ldomDiv.find("#ddlStateTax").hide();
                    ldomDiv.find("#ddlStateTax").val("");
                }                
            }
        });
        $(document).on("change", "#ddlPlanId", function (e) {
            if (ns.viewModel.currentModel.indexOf("wfmBenefitCalculationWebWizard") === 0) {
                var ldomDiv = $("#" + ns.viewModel.currentModel);
                GetBenefitAccountTypeByPlan(ldomDiv);
            }
        });
        $(document).on('click', "#rblRBDeferredComp_1", function (e) {
            var ldomDiv = $("#" + ns.viewModel.currentModel);            
            if (ldomDiv.find('#rblRBDeferredComp_1').length > 0 && ldomDiv.find('#rblRBDeferredComp_1')[0].checked == true) {
                ldomDiv.find("#pnlNo").show();
                ldomDiv.find("#pnlYes").hide();
            }
        });
        $(document).on("click", "#btnEstimateTax", function () {
            if (ns.viewModel.currentModel.indexOf("wfmMSSTaxWithholdingMaintenance") === 0) {
                var ldomDiv = $("#" + ns.viewModel.currentModel);         
                ldomDiv.find("#pnlEstimatetbTax").removeClass("hideControl");
                ldomDiv.find("#pnlEstimatetbTax").css("display", "");
            }
        });
        $(document).on("keyup blur", "#txtAnnualTaxablePaymentAmt, #txtPensionAnnuityAmt", function (e) {
            if (ns.viewModel.currentModel.indexOf("wfmMSSTaxWithholdingMaintenance") === 0 || ns.viewModel.currentModel.indexOf("wfmMSSBenefitApplicationWizard") === 0) {
                var ldomDiv = $("#" + ns.viewModel.currentModel);
                var ldecAnnualTaxPaymentAmt = ldomDiv.find("#txtAnnualTaxablePaymentAmt").val().replace("$", '').replace(/,/g, '') == "" ? 0 : parseFloat(ldomDiv.find("#txtAnnualTaxablePaymentAmt").val().replace("$", '').replace(/,/g, ''));
                var ldecPensionAnnuityAmt = ldomDiv.find("#txtPensionAnnuityAmt").val().replace("$", '').replace(/,/g, '') == "" ? 0 : parseFloat(ldomDiv.find("#txtPensionAnnuityAmt").val().replace("$", '').replace(/,/g, ''));
                var ldecTotal = ldecAnnualTaxPaymentAmt + ldecPensionAnnuityAmt;
                $("#txtTotalTaxablePensionAmount").val("$" + (ldecTotal.toLocaleString())).trigger('change');
            }
        });

        $(document).on("keyup blur", "#txtQualifyingChildrenAmt, #txtOtherDependentsAmt, #txtForeignEducationTaxCreditAmt", function (e) {
            if (ns.viewModel.currentModel.indexOf("wfmMSSTaxWithholdingMaintenance") === 0 || ns.viewModel.currentModel.indexOf("wfmMSSBenefitApplicationWizard") === 0) {
                var ldomDiv = $("#" + ns.viewModel.currentModel);
                var ldecQualifyingChildrenAmt = parseFloat(ldomDiv.find("#txtQualifyingChildrenAmt").val().replace("$", '').replace(/,/g, '')) == "" ? 0 : parseFloat(ldomDiv.find("#txtQualifyingChildrenAmt").val().replace("$", '').replace(/,/g, ''));
                var ldecOtherDependentsAmt = parseFloat(ldomDiv.find("#txtOtherDependentsAmt").val().replace("$", '').replace(/,/g, '')) == "" ? 0 : parseFloat(ldomDiv.find("#txtOtherDependentsAmt").val().replace("$", '').replace(/,/g, ''));
                var ldecForeignEducationTaxCreditAmt = parseFloat(ldomDiv.find("#txtForeignEducationTaxCreditAmt").val().replace("$", '').replace(/,/g, '')) == "" ? 0 : parseFloat(ldomDiv.find("#txtForeignEducationTaxCreditAmt").val().replace("$", '').replace(/,/g, ''));
                var ldecTotal = ldecQualifyingChildrenAmt + ldecOtherDependentsAmt + ldecForeignEducationTaxCreditAmt;
                $("#txtTotalClaimDependentAmount").val("$" + (ldecTotal.toLocaleString())).trigger('change');
            }
        });

        //FW Upgrade :: Mss-UI issue on wfmFlexEnrollmentWizard  (PIR - 11768)  
        $(document).on('click', ".Non-NdepersPtetax .GridCheckBox", function (e) {
                return false;
            });

        //FW Upgrade :: Re-register f/w event for go to content on the page because Scroll does not occuring (PIR - 11768)
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
                    var FormContainerID = ".page-container"
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

        //$(document).off('click.LogOutEvent', "a[href='https://www.nd.gov/']").on("click.LogOutEvent", "a[href='https://www.nd.gov/']", function () {
        //    var lControl = $(this);
        //    if (ns.viewModel.currentModel.indexOf("wfmMSSBenefitApplicationWizard") == 0 && lControl.hasClass("ignorLinkFromLogOutMsg"))
        //    {  }
        //    else 
        //    {
        //        var isLogout = confirm("You will be logged out of the system, Are you sure you want to continue?");
        //        if (isLogout == true) {
        //            var data = null;
        //            data = nsRequest.SyncPost("IsFromImageClick", "POST");
        //            if (data != null && data != undefined) {
        //                sessionStorage.setItem("ImageLinkToNavigate", $(this).attr("href"));
        //                ns.logoutSesssion();
        //            }
        //            return false;
        //        }
        //        else {
        //            return false;
        //        }
        //    }
        //});
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
    },
    fnOpenPopUpDialog: function (e) {
        var ldomDiv = $("#" + ns.viewModel.currentModel);
        var lstrSelection = e.target.getAttribute("rowIndex");

        if (lstrSelection != null && lstrSelection != undefined) {
            var ActiveDivID = nsCommon.GetActiveDivId(e);
            var RelatedGridID = GetControlAttribute(e.target, "sfwRelatedControl", ActiveDivID);
            var grid = $("#" + ActiveDivID + " #GridTable_" + RelatedGridID)
            var gridData = grid.data("neoGrid");
            var aPersonMaritalStatus = gridData.RenderData[lstrSelection].dt_MaritalStatus_10_0;
            if (aPersonMaritalStatus == 'Unknown') {
                nsCommon.DispalyError("Dependent/Beneficiary Marital Status is 'Unknown', please update marital status", ActiveDivID);
                return false;
            }
            else
                return true;

        }
    },

    // FMUpgrade : Added for Default page conversion of OnLoadComplete method
    fnChangeNameOnly: function (e) {
        
        var ldomDiv = e.context.idomActiveDiv != undefined ? e.context.idomActiveDiv : $("#" + e.context.activeDivID);
        if (e.target.id == "btnChangeNameOnly")
            ldomDiv.find("#txtNameOnly").val("true").trigger("change");

        ldomDiv.find(".buttonNext").trigger("click");
        return false;
    },
    // PIR 25920 DC 2025 Changes call next button from ADEC update wizard
    fnCallNextButton: function (e) {

        var ldomDiv = e.context.idomActiveDiv != undefined ? e.context.idomActiveDiv : $("#" + e.context.activeDivID);
        ldomDiv.find(".buttonNext").trigger("click");
        return false;
    },
    // FMUpgrade : Added for Default page conversion of btnBenAppGo_Click method
    fnBenAppGo_Click: function (e) {
        var ldomDiv = $("#" + ns.viewModel.currentModel);
        var BenAppsOption = ldomDiv.find("#ddlRetBenAppsOptions").val();
        var BenType = $("#lblbentype").text();
        if (BenType != '' && BenAppsOption != null && BenAppsOption != '' && BenAppsOption !="SARC" ) {
            if (BenType != BenAppsOption) {
                return alert("Please delete your existing application before you create a new application.");
                return false;
            }
        }
        switch (BenAppsOption) {
            case "RETR": case "RFND": case "DISA":
                    $("#Custom-loader").show();
                    var ldomDiv = $("#" + ns.viewModel.currentModel);
                    var Parameters = {};
                    var data = {};
                    data = nsCommon.SyncPost("checkSinglePlan", Parameters, "POST");
                    if (data != undefined) {
                        if (data == "new")
                            ldomDiv.find("#btnNewSelectBenType").trigger("click");
                        else if (data == "update")
                            ldomDiv.find("#btnOpenSaveContinueRecord").trigger("click");
                        else
                        {
                            $("#Custom-loader").hide();
                            nsCommon.sessionSet("benTypeValue", BenAppsOption);
                            if (BenAppsOption == "RFND") ldomDiv.find("#btnNewSelectBenType").trigger("click");
                            if (BenAppsOption == "RETR") ldomDiv.find("#btnNewSelectBenType").trigger("click");
                            if (BenAppsOption == "DISA") ldomDiv.find("#btnNewSelectBenType").trigger("click");
                        }
                    }
                break;
            case "VSRA":
                ldomDiv.find("#btnOpen2").trigger("click");
                break;
            case "SARC":
                ldomDiv.find("#btnOpen3").trigger("click");
                break;
        }

        return false;
    },
    fndelete: function (e) {
        var ldomDiv = $("#" + ns.viewModel.currentModel);
        ldomDiv.find("#btnDeleteDetail").trigger("click");
    },

    fnRedirectBenAppWizard: function (e) {
    	$("#Custom-loader").show();
        var ldomDiv = $("#" + ns.viewModel.currentModel);
        var BenAppID = parseInt(ldomDiv.find("#lblLabel").text());
        if (BenAppID > 0)
            ldomDiv.find("#btnOpenSaveContinueRecord").trigger("click");
        else
            ldomDiv.find("#btnNewSelectBenType").trigger("click");
        return false;
    },   

    //Function to trigger wizard next button
    fnBenefitEstimate: function (e) {
        var ldomDiv = e.context.idomActiveDiv != undefined ? e.context.idomActiveDiv : $("#" + e.context.activeDivID);      
        ldomDiv.find(".buttonNext").trigger("click");
        return false;
    },   
    fnAnnualEnrollment: function (e) {
        var ldomDiv = e.context.idomActiveDiv != undefined ? e.context.idomActiveDiv : $("#" + e.context.activeDivID);
        ldomDiv.find(".buttonNext").trigger("click");
        return false;
    },
    fnPreviousAnnualEnrollment: function (e) {
        var ldomDiv = e.context.idomActiveDiv != undefined ? e.context.idomActiveDiv : $("#" + e.context.activeDivID);
        ldomDiv.find(".buttonPrevious").trigger("click");
        return false;
    },    
    // FMUpgrade : Added for Default page conversion of btnBenefitPlanNew_Click method
    fnBenefitPlanNew: function (e) {
        return true;
    },

    NavigateToServicePurchase: function(e){
        var ldomDiv = $("#" + ns.viewModel.currentModel);
        if (ns.viewModel.currentModel.indexOf("wfmServicePurchaseMaintenance") === 0) {
            var ddlPurchaseType = ldomDiv.find("#ddlDropDownList option:selected");
            if(ddlPurchaseType.val()=="CONT")
                ldomDiv.find("#btnOpen").trigger("click");
            else if(ddlPurchaseType.val()=="ESTI")
                ldomDiv.find("#btnOpen1").trigger("click");
            else if (ddlPurchaseType.val() == "SREQ")
                ldomDiv.find("#btnOpen2").trigger("click");
            
        }
        return false;
    },
    NavigateToBenefitEstimate: function (e) {
        var ldomDiv = $("#" + ns.viewModel.currentModel);
        if (ns.viewModel.currentModel.indexOf("wfmBenefitEstimateMaintenance") === 0) {
            var ddlPurchaseType = ldomDiv.find("#ddlDropDownList option:selected");
            if (ddlPurchaseType.val() == "BENE")
                ldomDiv.find("#btnOpenViewBenefitEstimate").trigger("click");
            else if (ddlPurchaseType.val() == "ESTM")
                ldomDiv.find("#btnOpenBenefitCalculationWebWizard").trigger("click");
            else if (ddlPurchaseType.val() == "BREQ")
                ldomDiv.find("#btnOpenBenefitEstimate").trigger("click");

        }
        return false;
    },
    NavigateToSelectStatement: function (e) {
        var ldomDiv = $("#" + ns.viewModel.currentModel);
        if (ns.viewModel.currentModel.indexOf("wfmMSSSelectStatementMaintenance") === 0) {
            var ddlStatementType = ldomDiv.find("#ddlStatementType option:selected");
            if (ddlStatementType.val() == "ANNL")
                ldomDiv.find("#btnOpenMSSAnnualStatement").trigger("click");
            else if (ddlStatementType.val() == "T99R")
                ldomDiv.find("#btnOpenMSSOneZeroNineNineR").trigger("click");
        }
        return false;
    },
    fnOpenDirectDepositandPayeeAccount: function (e) {
        var ldomDiv = $("#" + ns.viewModel.currentModel);
        var lblPayeeAccountId = ldomDiv.find("#lblPayeeAccountId").text();
        var Parameters = {};
        Parameters["aintPayeeAccountId"] = lblPayeeAccountId;
        var data = {};
        data = nsCommon.SyncPost("IsEmailAddressNotWaived", Parameters, "POST");
        if (!data) {
            ldomDiv.find("#lblIsEmailAddressWaived").text("True");
            PopupHelp(ldomDiv.find("#lblPopUpMessageForCertify").html(), "btnDirectLinkButton");
            return false;
        }
        return true;
    },

    //F.W Upgrade
    fnRedirectPayeeAccounts: function (e) {
        var ldomDiv = $("#" + ns.viewModel.currentModel);
        var astrActivationCode = ldomDiv.find("#txtActivationCode").val();
        var Parameters = {};
        Parameters["AstrActivationCode"] = astrActivationCode;
        var data = {};
        data = nsCommon.SyncPost("btnVertifyOTP_Click", Parameters, "POST");       
        if (data != undefined && data.length > 0 && (data[0].istrErrorID == "10318" || data[0].istrErrorID == "10328"))
        {
            var text = data[0].istrErrorMessage;
            var ActiveDivID = nsCommon.GetActiveDivId(e);
            nsCommon.DispalyError(text, ActiveDivID);
            return false;          

        }
        else {
            return true;
        }
    },
    fnAllowAccessToAll: function(e){
         
            $("#PersonCertify").val(true);
            ns.lstrPersonCertify = "true";
            $("#IsEmailFlagWaiver").val(true);
            ns.IsEmailAddressWaived = "true";
            $("li a[id = 'lnkMssHome']").css("pointer-events", "");
            $("li a[id = 'lnkMssForms']").css("pointer-events", "");
            $("li a[id = 'lnkContactUs']").css("pointer-events", "");
            $("li a[title = 'Previous']").css("pointer-events", "");
            $("#lnkMssHome").trigger("click");
            return false;
    },
    BeforeAnnualEnrollmentBackButton: function (e) {
        var ldomDiv = $("#" + ns.viewModel.currentModel);
        if (ns.viewModel.currentModel.indexOf("wfmMSSDeferredCompEnrolledMaintenance") == 0) {
            nsEvents.OnDeleteFormClick(ns.viewModel.currentModel);
        }
    },
    DeseasedPersonSelectedIndexChanged: function (e) {
        var ldomDiv = $("#" + ns.viewModel.currentModel);
        if (ns.viewModel && ns.viewModel.currentModel && ns.viewModel.currentModel.indexOf("wfmMSSDeathNoticeMaintenance") == 0) {
            if (ldomDiv.find('#ddlDeceasedName option:selected').text() == "Other") {
                ldomDiv.find("#lblName").removeClass("hideControl");
                ldomDiv.find("#txtDeceasedName").removeClass("hideControl");
            }
            else {
                ldomDiv.find("#lblName").addClass("hideControl");
                ldomDiv.find("#txtDeceasedName").addClass("hideControl");
                ldomDiv.find("#txtDeceasedName").text('').trigger("change");
            }
        }
    },
    BeforeNavigate: function (e) {
        if (ns.viewModel.currentModel.indexOf("wfmMSSDeferredCompEnrolledMaintenance") === 0 && ns.SenderID == "btnAnnualEnrollmentBack") {
            nsEvents.OnDeleteFormClick(ns.viewModel.currentModel);
        }
        if (ns.viewModel.currentModel.indexOf("wfmMSSTaxWithholdingMaintenance") == 0) {
            nsEvents.OnDeleteFormClick(ns.viewModel.currentModel);
        }
        if (ns.viewModel.currentModel.indexOf("wfmMaritalChangeWizard") == 0) {
            nsEvents.OnDeleteFormClick(ns.viewModel.currentModel);
        }
        //F/W Upgrade PIR - 12300 Clear Previous wizard from dom before selecting new plan
        if (ns.viewModel.currentModel.indexOf("wfmMSSGHDVAnnualEnrollmentWizard") == 0 || ns.viewModel.currentModel.indexOf("wfmMSSGHDVInsuranceEnrollmentWizard") == 0) {
            nsEvents.OnDeleteFormClick(ns.viewModel.currentModel);
        }
        if (ns.viewModel.currentModel.indexOf("wfmMSSDirectDepositInformationMaintenance") == 0 && ns.SenderID == "btnRefreshData12") {
            delete ns.DirtyData[ns.viewModel.currentModel];
            nsEvents.OnDeleteFormClick(ns.viewModel.currentModel);
        }
        if (ns.viewModel.currentModel.indexOf("wfmMSSDirectDepositInformationMaintenance") == 0 && ns.SenderID == "btnOpenMSSProfile") {
            delete ns.DirtyData[ns.viewModel.currentModel];
            nsEvents.OnDeleteFormClick(ns.viewModel.currentModel);
        }
        //if (ns.viewModel.currentModel.indexOf("wfmMSSProfileMaintenance") == 0 && (ns.SenderID == "btnVerify1" || ns.SenderID == "btnCertify1")) {
        //    delete ns.DirtyData[ns.viewModel.currentModel];
        //    nsEvents.OnDeleteFormClick(ns.viewModel.currentModel);
        //}
        if (ns.viewModel.currentModel.indexOf("wfmMSSBenefitApplicationWizard") == 0 && (ns.SenderID != "btnNew" && ns.SenderID != "btnwssBenAppResend" && ns.SenderID != "btnWssBenAppVerify")) {
            
            if (ns.SenderID.contains("NavLandPage")) {
                if (confirm("Your application will be saved for 30 days for you to make changes prior to being deleted.")) {
                    delete ns.DirtyData[ns.viewModel.currentModel];
                    nsEvents.OnDeleteFormClick(ns.viewModel.currentModel);

                    var dataitem = nsCommon.GetDataItemFromDivID("wfmMSSSelectRetPlanMaintenance0");
                    if (dataitem != undefined && MVVMGlobal.CanBeDeleted(dataitem)) {
                        delete ns.DirtyData["wfmMSSSelectRetPlanMaintenance0"];
                        nsEvents.OnDeleteFormClick(dataitem.divID);
                    }
                }
                else
                    return false;
            }
        }
        if (ns.viewModel.currentModel.indexOf("wfmMSSContactNDPERSMaintenance") == 0) {
            nsEvents.OnDeleteFormClick(ns.viewModel.currentModel);
        }
        if (ns.viewModel.currentModel.indexOf("wfmMSSBenefitPlanDetailsMaintenance") == 0) {
            nsEvents.OnDeleteFormClick(ns.viewModel.currentModel);
        }        
        if (ns.viewModel.currentModel.indexOf("wfmMSSDirectDepositMaintenance") == 0) {
            nsEvents.OnDeleteFormClick(ns.viewModel.currentModel);
        }
        //if (ns.viewModel.currentModel.indexOf("wfmMSSProfileMaintenance") == 0) {
        //    nsEvents.OnDeleteFormClick(ns.viewModel.currentModel);
        //}
        if (ns.viewModel.currentModel.indexOf("wfmMSSGHDVMaintenance") == 0) {
            nsEvents.OnDeleteFormClick(ns.viewModel.currentModel);
        }
        if (ns.viewModel.currentModel.indexOf("wfmMSSLifeMaintenance") == 0) {
            nsEvents.OnDeleteFormClick(ns.viewModel.currentModel);
        }
        if (ns.viewModel.currentModel.indexOf("wfmMSSFlexCompMaintenance") == 0) {
            nsEvents.OnDeleteFormClick(ns.viewModel.currentModel);
        }
        if (ns.viewModel.currentModel.indexOf("wfmMSSAdditionalContributionMaintenance") == 0) {
            nsEvents.OnDeleteFormClick(ns.viewModel.currentModel);
        }
        if (ns.viewModel.currentModel.indexOf("wfmMSSAdditionalContributionWizard") == 0) {
            delete ns.DirtyData[ns.viewModel.currentModel];
            nsEvents.OnDeleteFormClick(ns.viewModel.currentModel);
        }
        return true; 
           
    },

    BeforeShowDiv: function (e) {
        var ldomDiv = $("#" + ns.viewModel.currentModel);
        if (ns.viewModel.currentForm.indexOf("wfmMSSGHDVMaintenance") === 0 && ns.viewModel.previousDiv.indexOf("wfmMSSGHDVInsuranceEnrollmentWizard") === 0) {
            ldomDiv.find("#btnBack").trigger("click");
        }
        if (ns.viewModel.currentForm.indexOf("wfmServicePurchaseWebMaintenance") === 0 && ns.viewModel.previousDiv.indexOf("wfmMSSServicePurchaseEstimateMaintenance") === 0) {
            ldomDiv.find("#btnBack").trigger("click");
        }
        nsUserFunctions.iblnMenuItemClicked = false;
        
    },
    ChangeDisplayError: function (e) {
        nsUserFunctions.iblnMenuItemClicked = false;
    },
    ////F.W Upgrade  Pop-up message should be displayed while navigating in case of unsaved wizard screens PIR(11767)
    BeforeMenuNavigate: function (e) {
        if (nsUserFunctions.iblnMenuItemClicked) {
            return false;
        }
        else {
            nsUserFunctions.iblnMenuItemClicked = dataLossPopUpMessage();
            return nsUserFunctions.iblnMenuItemClicked;
        }
    },

    fnServicePurchaseWebPanelVisibility: function (e) {        
        var ldomDiv = $("#" + ns.viewModel.currentModel);             
        var lddlPurchaseType = ldomDiv.find("#ddlServicePurchaseTypeValue option:selected");
        var lpnlPaymentSchedule = ldomDiv.find("#pnlPaymentSchedule");
        var lblamorText = ldomDiv.find("#lblLesseramount").text();
       
        if (lddlPurchaseType.val() != null && (lddlPurchaseType.val() == "BOTH" || lddlPurchaseType.val() == "ADDP")) {
            if (ns.viewModel[ns.viewModel.currentModel].WidgetControls["grvGridView"] != null && lblamorText != null) {
                if (lpnlPaymentSchedule != null) {
                    ldomDiv.find("#pnResults").removeClass("hideControl");
                    ldomDiv.find("#pnResults").css("display", "");
                    ldomDiv.find("#pnlPaymentSchedule").show();
                }
                if (ns.viewModel[ns.viewModel.currentModel].WidgetControls["grvGridView"].iintRecordLength == 0) {
                    ldomDiv.find("#grvGridView_GridHolder").hide();
                    ldomDiv.find("#lblLesseramount").show();
                }
                else {
                    ldomDiv.find("#grvGridView_GridHolder").show();
                    ldomDiv.find("#lblLesseramount").hide();
                }
            }                                                    
        }
        else if (lpnlPaymentSchedule != null) {
                ldomDiv.find("#pnResults").removeClass("hideControl");
                ldomDiv.find("#pnResults").css("display", "");
                ldomDiv.find("#pnlPaymentSchedule").hide();
            }        
        return false;
    },
    NavigateToHomePage: function (e) {
        if (nsWizard.lastActiveWizardDivID !== "") {
            if ($("#" + nsWizard.lastActiveWizardDivID).length > 0) {
                OnDeleteFormClick(nsWizard.lastActiveWizardDivID);
            }
            nsWizard.lastActiveWizardDivID = "";
        }
        StoreTreeViewInSessionStore();
        window.location.href = ns.SiteName;//+ '#/spa/wfmInteralMaintenance/0';
        // window.location.reload();
        e.preventDefault();
    },
    NavigateToPage: function (e) {
        var RedirecttoPage = $("#" + e.context.activeDivID).find("#lblRedirectForm");
        if (RedirecttoPage.length > 0) {
            RedirecttoPage = RedirecttoPage.text();
            if (RedirecttoPage == "btnRetirementWizard") {
                nsUserFunctions.RefreshGridForInsuranceWizard = true;
            }
            $("#" + e.context.activeDivID + " #" + RedirecttoPage).trigger("click");
            //OnDeleteFormClick(e.context.activeDivID);
        }
        return false;
    },

    showDivCallBack: function (e) {
        GetMessageCount();      

        //PIR 25699 start
        if (e.context.activeDivID.indexOf("wfmMSSPayeeAccountsMaintenance") === 0) {
            var ldomDiv = $("#" + ns.viewModel.currentModel);
            
            //session variable set on visibility of tax withholding panel data
            if (ns.viewModel[ns.viewModel.currentModel].HeaderData.MaintenanceData["lblStateTaxEffDate"] == undefined ||
                ns.viewModel[ns.viewModel.currentModel].HeaderData.MaintenanceData["lblStateTaxEffDate"] == null) {
                nsCommon.sessionSet("IsTaxWithholdingPanelVisibile", "false");
            }

            //On menu item click should scroll to the Tax Withholding panel
            var fnRefreshData = function () {
                if (nsCommon.sessionGet("IsTaxWithholdingNavItemClicked") == "true") {
                    if ($("#btnNewMonthlyBenefitPAM").is(":visible")) {
                        ldomDiv.find("#btnNewMonthlyBenefitPAM").trigger("focus");
                    }
                    else if ($("#btnNewFedTaxRefundPAM").is(":visible")) {
                        ldomDiv.find("#btnNewFedTaxRefundPAM").trigger("focus");
                    }
                }
            }
            setTimeout(fnRefreshData, 300);
        }
        if (ns.viewModel.currentModel.indexOf("wfmMSSPayeeAccountsMaintenance") == 0 && ns.SenderID == "btnCancel") {
            nsEvents.OnDeleteFormClick("wfmMSSTaxWithholdingMaintenance0");
        }

        if (ns.viewModel.currentModel.indexOf("wfmMSSSelectRetPlanMaintenance") == 0) {
            var ldomDiv = $("#" + ns.viewModel.currentModel);
            var RadioButtonList = ldomDiv.find("#rblRadioButtonList_1");
            if (RadioButtonList.length <= 0) {
                ldomDiv.find("#rblRadioButtonList").hide();

            }

        }
        //remove session variable when member is changing to a different payee account
        if (ns.viewModel.currentModel.indexOf("wfmMSSPensionPaymentDetailsMaintenance") == 0) {
            if ((nsCommon.sessionGet("IsTaxWithholdingPanelVisibile") != undefined)) {
                nsCommon.sessionRemove("IsTaxWithholdingPanelVisibile");
            }       
        }
        //PIR 25699 end

        if (e.context.activeDivID.indexOf("wfmInsurancePlansMaintenance") == 0 && ns.viewModel) {
            var fnRefreshData = function () {
                var ldomDiv = $("#" + ns.viewModel.currentModel);
                if (e.context.activeDivID.indexOf("wfmInsurancePlansMaintenance") == 0 && nsCommon.sessionGet("ResetTriggredMSSMessage") == undefined) {
                    nsCommon.sessionSet("ResetTriggredMSSMessage", "true");
                    ldomDiv.find("#btnCancel").trigger("click");
                }
            }
            setTimeout(fnRefreshData, 300);
        }
        if (e.context.activeDivID.indexOf("wfmMSSTaxWithholdingMaintenance") === 0 && ns.viewModel && $("#pnlEstimatetbTax").is(":visible")) {
            var fnRefreshData = function () {
                var ldomDiv = $("#" + ns.viewModel.currentModel); 
                ldomDiv.find("#btnFD22Calculate").trigger("focus");
            }
            setTimeout(fnRefreshData, 300);
        }
        else if (e.context.activeDivID.indexOf("wfmAnnualEnrollmentBenefitPlansMaintenance") == 0 && ns.viewModel) {
            var fnRefreshData = function () {
                var ldomDiv = $("#" + ns.viewModel.currentModel);
                if (e.context.activeDivID.indexOf("wfmAnnualEnrollmentBenefitPlansMaintenance") == 0 && nsCommon.sessionGet("ResetTriggredAnnualMSSMessage") == undefined) {
                    nsCommon.sessionSet("ResetTriggredAnnualMSSMessage", "true");
                    ldomDiv.find("#btnCancel").trigger("click");
                }
            }
            setTimeout(fnRefreshData, 300);
        }
        else if (e.context.activeDivID.indexOf("wfmActiveMemberBenefitPlansMaintenance") == 0 && ns.viewModel) {
            var fnRefreshData = function () {
                var ldomDiv = $("#" + ns.viewModel.currentModel);
                if (e.context.activeDivID.indexOf("wfmActiveMemberBenefitPlansMaintenance") == 0 && nsCommon.sessionGet("ResetTriggredBenefitPlansMSSMessage") == undefined) {
                    nsCommon.sessionSet("ResetTriggredBenefitPlansMSSMessage", "true");
                    ldomDiv.find("#btnCancel").trigger("click");
                }
            }
            setTimeout(fnRefreshData, 300);
        }
        else if (e.context.activeDivID.indexOf("wfmMSSRetBenAppsLandingScreenMaintenance") == 0 && ns.viewModel) {
            var fnRefreshData = function () {
                var ldomDiv = $("#" + ns.viewModel.currentModel);
                if (e.context.activeDivID.indexOf("wfmMSSRetBenAppsLandingScreenMaintenance") == 0 && nsCommon.sessionGet("ResetTriggredBenAppsLandingScreenMaintenance") == undefined) {
                    nsCommon.sessionSet("ResetTriggredBenAppsLandingScreenMaintenance", "true");                    
                    ldomDiv.find("#btnCancel").trigger("click");
                }
            }
            setTimeout(fnRefreshData, 300);
        }
        else if (e.context.activeDivID.indexOf("wfmMSSSelectRetPlanMaintenance") == 0 && ns.viewModel) {
            var fnRefreshData = function () {
                var ldomDiv = $("#" + ns.viewModel.currentModel);
                if (e.context.activeDivID.indexOf("wfmMSSSelectRetPlanMaintenance") == 0 && nsCommon.sessionGet("ResetTriggredSelectRetPlanMaintenance") == undefined) {
                    nsCommon.sessionSet("ResetTriggredSelectRetPlanMaintenance", "true");
                    ldomDiv.find("#btnCancel").trigger("click");
                }
            }
            setTimeout(fnRefreshData, 300);
        }
        else if (e.context.activeDivID.indexOf("wfmMSSActiveMemberHomeMaintenance") == 0 && ns.viewModel) {
            var fnRefreshData = function () {
                var ldomDiv = $("#" + ns.viewModel.currentModel);
                if (e.context.activeDivID.indexOf("wfmMSSActiveMemberHomeMaintenance") == 0 && nsCommon.sessionGet("ResetTriggredMSSActiveMemberHome") == undefined) {
                    nsCommon.sessionSet("ResetTriggredMSSActiveMemberHome", "true");
                    ldomDiv.find("#btnCancel").trigger("click");
                }
            }
            setTimeout(fnRefreshData, 300);
        }
        else if (e.context.activeDivID.indexOf("wfmMSSTaxWithholdingInformationMaintenance") == 0 && ns.viewModel) {
            var fnRefreshData = function () {
                var ldomDiv = $("#" + ns.viewModel.currentModel);
                if (e.context.activeDivID.indexOf("wfmMSSTaxWithholdingInformationMaintenance") == 0 && nsCommon.sessionGet("ResetTriggredMSSTaxWithholdingInfo") == undefined) {
                    nsCommon.sessionSet("ResetTriggredMSSTaxWithholdingInfo", "true");
                    ldomDiv.find("#btnCancel").trigger("click");
                }
            }
            setTimeout(fnRefreshData, 300);
        }
        else if (e.context.activeDivID.indexOf("wfmMSSTaxWithholdingInformationMaintenanceNew") == 0 && ns.viewModel) {
            var fnRefreshData = function () {
                var ldomDiv = $("#" + ns.viewModel.currentModel);
                if (e.context.activeDivID.indexOf("wfmMSSTaxWithholdingInformationMaintenanceNew") == 0 && nsCommon.sessionGet("ResetTriggredMSSTaxWithholdingInfo") == undefined) {
                    nsCommon.sessionSet("ResetTriggredMSSTaxWithholdingInfo", "true");
                    ldomDiv.find("#btnCancel").trigger("click");
                }
            }
            setTimeout(fnRefreshData, 300);
        }
        else if (e.context.activeDivID.indexOf("wfmMSSPersonAccountGhdvHsaHistoryMaintenance") == 0 && ns.viewModel) {
            var fnRefreshData = function () {
                var ldomDiv = $("#" + ns.viewModel.currentModel);
                if (e.context.activeDivID.indexOf("wfmMSSPersonAccountGhdvHsaHistoryMaintenance") == 0 && nsCommon.sessionGet("ResetTriggredMSSPersonAccountGhdvHsaHistory") == undefined) {
                    nsCommon.sessionSet("ResetTriggredMSSPersonAccountGhdvHsaHistory", "true");
                    ldomDiv.find("#btnCancel").trigger("click");
                }
            }
            setTimeout(fnRefreshData, 300);
        }
        else if (e.context.activeDivID.indexOf("wfmMSSDeathNoticeMaintenance") === 0) {
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
        else if (e.context.activeDivID.indexOf("wfmMSSProfileMaintenance") == 0 && ns.viewModel) {
            var fnRefreshData = function () {
                var ldomDiv = $("#" + ns.viewModel.currentModel);
                if (e.context.activeDivID.indexOf("wfmMSSProfileMaintenance") == 0 && ns.viewModel.previousDiv.indexOf("wfmMSSAddressMaintenance") == 0 && nsCommon.sessionGet("ResetTriggredMSSProfileMaintenance") == undefined) {
                    nsCommon.sessionSet("ResetTriggredMSSProfileMaintenance", "true");
                    ldomDiv.find("#btnCancel1").trigger("click");
                }
            }
            setTimeout(fnRefreshData, 300);
        }
        else {            
            var fnClearRefreshData = function () {
                nsCommon.sessionRemove("ResetTriggredMSSMessage");
                nsCommon.sessionRemove("ResetTriggredAnnualMSSMessage");
                nsCommon.sessionRemove("ResetTriggredBenefitPlansMSSMessage");
                nsCommon.sessionRemove("ResetTriggredBenAppsLandingScreenMaintenance");
                nsCommon.sessionRemove("ResetTriggredSelectRetPlanMaintenance");
                nsCommon.sessionRemove("ResetTriggredMSSProfileMaintenance");
                nsCommon.sessionRemove("ResetTriggredMSSActiveMemberHome");
                nsCommon.sessionRemove("ResetTriggredMSSTaxWithholdingInfo");
                nsCommon.sessionRemove("IsTaxWithholdingNavItemClicked");
                nsCommon.sessionRemove("ResetTriggredMSSPersonAccountGhdvHsaHistory");
            }
            if ((nsCommon.sessionGet("ResetTriggredMSSMessage") != undefined) || (nsCommon.sessionGet("ResetTriggredAnnualMSSMessage") != undefined) || (nsCommon.sessionGet("ResetTriggredBenefitPlansMSSMessage") != undefined) || (nsCommon.sessionGet("ResetTriggredBenAppsLandingScreenMaintenance") != undefined) || (nsCommon.sessionGet("ResetTriggredSelectRetPlanMaintenance") != undefined)
                || (nsCommon.sessionGet("ResetTriggredMSSProfileMaintenance") != undefined) || (nsCommon.sessionGet("ResetTriggredMSSTaxWithholdingInfo") != undefined) || (nsCommon.sessionGet("IsTaxWithholdingNavItemClicked") != undefined) ||
                (nsCommon.sessionSet("ResetTriggredMSSPersonAccountGhdvHsaHistory") != undefined)) {
                setTimeout(fnClearRefreshData, 300);
            }
        }        

        if (e.context.activeDivID.indexOf("wfmMSSRetirementApplicationWizard") >= 0 && nsUserFunctions.RefreshGridForInsuranceWizard) {
            nsUserFunctions.RefreshGridForInsuranceWizard = false;
            $(e.context.activeDivID + " #btnRefreshInsurance").trigger("click");
        }
        else if (e.context.activeDivID.indexOf("wfmMSSBenefitApplicationWizard") == 0 && ns.viewModel) {
            $("#Custom-loader").hide();
        }
        setTimeout(function () {
            if (ns.viewModel.currentModel.indexOf("wfmUpdatePaymentMethodWizard") == 0 && $("#wzsReviewAndAuthorize").is(":visible")
                || ns.viewModel.currentModel.indexOf("wfmFlexEnrollmentWizard") == 0 && $("#wzsReviewYourEnrollment").is(":visible")
                || ns.viewModel.currentModel.indexOf("wfmLifeEnrollmentWizard") == 0 && $("#wzsStep5").is(":visible")
                || ns.viewModel.currentModel.indexOf("wfmMSSDeferredCompWizard") == 0 && $("#wzsStep4").is(":visible")
                || ns.viewModel.currentModel.indexOf("wfmMSSGHDVInsuranceEnrollmentWizard") == 0 && $("#wzsStep6").is(":visible")
                || ns.viewModel.currentModel.indexOf("wfmMSSBenefitApplicationWizard") == 0 && $("#wzsSumE-Sig").is(":visible")) {
                $("div[class='actionBar'] a[class='buttonNext']").text("Finish");
                $("div[class='actionBar'] a[class='buttonFinish buttonDisabled']").hide();
            }
            else{
                $("div[class='actionBar'] a[class='buttonNext']").text("Next");
                $("div[class='actionBar'] a[class='buttonFinish buttonDisabled']").show();
            }
            if (ns.viewModel && ns.viewModel.currentModel && ((ns.viewModel.currentModel.indexOf("wfmMSSGHDVAnnualEnrollmentWizard") == 0 && $("#pnlMemberAccountexists").is(":visible"))
                || (ns.viewModel.currentModel.indexOf("wfmMaritalChangeWizard") == 0 && $("#wzsIntro").is(":visible"))
                || (ns.viewModel.currentModel.indexOf("wfmBenefitCalculationWebWizard") == 0 && $("#wzsIntroStep").is(":visible"))
                || (ns.viewModel.currentModel.indexOf("wfmMSSGHDVAnnualEnrollmentWizard") == 0 && $("#pnlNOMemberAccount").is(":visible"))
                || (ns.viewModel.currentModel.indexOf("wfmLifeAnnualEnrollmentWizard") == 0 && $("#pnlMemberAccountExists").is(":visible"))
                || (ns.viewModel.currentModel.indexOf("wfmLifeAnnualEnrollmentWizard") == 0 && $("#pnlNoMemberAccountExists").is(":visible"))
                //|| (ns.viewModel.currentModel.indexOf("wfmLifeAnnualEnrollmentWizard") == 0 && $("#wzsStep1").is(":visible"))
                //|| (ns.viewModel.currentModel.indexOf("wfmLifeAnnualEnrollmentWizard") == 0 && $("#wzsStep2").is(":visible"))
                //|| (ns.viewModel.currentModel.indexOf("wfmLifeAnnualEnrollmentWizard") == 0 && $("#wzsStep3").is(":visible"))
                //|| (ns.viewModel.currentModel.indexOf("wfmLifeAnnualEnrollmentWizard") == 0 && $("#wzsStep4").is(":visible"))
                || (ns.viewModel.currentModel.indexOf("wfmFlexAnnualEnrollmentWizard") == 0 && $("#wzsIntro").is(":visible"))
                || (ns.viewModel.currentModel.indexOf("wfmLifeEnrollmentWizard") == 0 && $("#wzsIntro").is(":visible"))
                || (ns.viewModel.currentModel.indexOf("wfmFlexEnrollmentWizard") == 0 && $("#wzsIntro").is(":visible"))
                || (ns.viewModel.currentModel.indexOf("wfmMSSGHDVInsuranceEnrollmentWizard") == 0 && $("#wzsIntro").is(":visible"))
                || (ns.viewModel.currentModel.indexOf("wfmMSSDeferredCompWizard") == 0 && $("#wzsIntro").is(":visible"))
                || (ns.viewModel.currentModel.indexOf("wfmMSSPensionPlanMainRetirementOptionalEnrollmentWizard") == 0 && $("#wzsIntro").is(":visible"))
                || (ns.viewModel.currentModel.indexOf("wfmUpdatePaymentMethodWizard") == 0 && $("#wzsPrintSummary").is(":visible")
                || (ns.viewModel.currentModel.indexOf("wfmFlexEnrollmentWizard") == 0 && $("#wzsPrint").is(":visible"))
                || (ns.viewModel.currentModel.indexOf("wfmLifeEnrollmentWizard") == 0 && $("#wzsStep6").is(":visible"))
                || (ns.viewModel.currentModel.indexOf("wfmMSSDeferredCompWizard") == 0 && $("#wzsStep6").is(":visible"))
                || (ns.viewModel.currentModel.indexOf("wfmMSSGHDVInsuranceEnrollmentWizard") == 0 && $("#wzsStep7").is(":visible"))
                || (ns.viewModel.currentModel.indexOf("wfmMSSBenefitApplicationWizard") == 0 && $("#wzsPrint").is(":visible")))
            )) {
                    $(".actionBar").hide();
                }                
            else {
                    $(".actionBar").show();
            }
            if (ns.viewModel.currentModel.indexOf("wfmMSSBenefitApplicationWizard") == 0 && ($("#wzsHealth").is(":visible") || $("#wzsDental").is(":visible") || $("#wzsVision").is(":visible"))) {
                setBenAppWizardVisibility();
            }
            if (ns.viewModel.currentModel.indexOf("wfmMSSBenefitApplicationWizard") == 0 && $("#wzsStepLifeInsurance").is(":visible")) {                
                $("#lblLifeStepDependantReqInformation_0").hide();
            }
            
            if (ns.viewModel.currentModel.indexOf("wfmFlexEnrollmentWizard") == 0) {
                $("#btnFinish11").removeClass("button");
                $("#btnPreviousAnnual").css("height", "33px");
                $("#btnNextAnnual").css("height", "33px");
            }
            if (ns.viewModel.currentModel.indexOf("Wizard") > 0) {
                var stepDiv = $("div[class='stepContainer'] div[data-sfwcontroltype='stepDiv']");
                if (stepDiv.length > 0) {
                    var titleAttr = stepDiv.attr('title');
                    if (titleAttr.length > 0) {
                        stepDiv.removeAttr('title');
                    }
                }
                var stepSpan = $("ul[class='ProgressBar'] li span[class='stepSpan']"); 
                if(stepSpan.length > 0)
                {
                    var titleAttribute = stepSpan.attr('title');
                    if(titleAttribute.length > 0)
                    {
                        stepSpan.removeAttr('title');
                    }
                }
            }

        }, 0);

        //Sub Menu functionality implementation
        if (e.context.activeDivID.indexOf("wfmMSSPensionPaymentDetailsMaintenance") == 0 || e.context.activeDivID.indexOf("wfmMSSAnnualStatementMaintenance") == 0
            || e.context.activeDivID.indexOf("wfmMSSProfileMaintenance") == 0 || e.context.activeDivID.indexOf("wfmMSSContactInfoMaintenance") == 0
            || e.context.activeDivID.indexOf("wfmMSSBenefitPlanInfoMaintenance") == 0 || e.context.activeDivID.indexOf("wfmMSSInboxMaintenance") == 0
            || e.context.activeDivID.indexOf("wfmMSSInboxMaintenance") == 0 || e.context.activeDivID.indexOf("wfmMSSContactNDPERSMaintenance") == 0
            || e.context.activeDivID.indexOf("wfmMSSAppointmentScheduleMaintenance") == 0 || e.context.activeDivID.indexOf("wfmMSSDeathNoticeMaintenance") == 0
            || e.context.activeDivID.indexOf("wfmMSSRetireeHomeMaintenance") == 0) {
            //payee_account_id = 0 
            sessionStorage.setItem("payee_account_id", "0");

        }

        var IsRetiree; var lblEnrolledInCOBRA; var lGlobalPlanId; var lGlobalPayeeAccStatus; var lGlobalPayeeAccountId; var lGlobalBenefitAccountType;
        var ldomDiv = $("#" + ns.viewModel.currentModel);
        if (ns.viewModel && ns.viewModel.currentModel && ns.viewModel.currentModel.indexOf("wfmMSSPayeeAccountsMaintenance") == 0) {
            IsRetiree = $("#IsRetiree").val(); //"IsRetiree"
            lblEnrolledInCOBRA = $("#lblGloablEnrolledInCOBRA").val(); //"EnrolledInCOBRA"

            sessionStorage.setItem("payee_account_id", ns.viewModel[ns.viewModel.currentModel].HeaderData.MaintenanceData["lblPayeeAccountId"]);//"payee_account_id"
            sessionStorage.setItem("plan_name", ns.viewModel[ns.viewModel.currentModel].HeaderData.MaintenanceData["lblPlanID"]); //"plan_name"
            //sessionStorage.setItem("payee_account_status", ns.viewModel[ns.viewModel.currentModel].HeaderData.MaintenanceData["lblPayeeAccStatus"]); //"payee_account_status"
            sessionStorage.setItem("benefit_account_type", ns.viewModel[ns.viewModel.currentModel].HeaderData.MaintenanceData["lblBenefitType1"]); //"benefit_account_type"

            var Parameters = {};
            Parameters["aintPayeeAccountId"] = ns.viewModel[ns.viewModel.currentModel].HeaderData.MaintenanceData["lblPayeeAccountId"];
            nsCommon.SyncPost("SetPayeeAccountId", Parameters, "POST");

            if (sessionStorage.getItem("plan_name") == "7" || sessionStorage.getItem("plan_name") == "23") {
                sessionStorage.setItem("plan_name", ns.viewModel[ns.viewModel.currentModel].HeaderData.MaintenanceData["lblPlanID"]);
                sessionStorage.setItem("payee_account_status", ns.viewModel[ns.viewModel.currentModel].HeaderData.MaintenanceData["lblPayeeAccStatus"]);
            }
            else {
                sessionStorage.setItem("plan_name", null);
                sessionStorage.setItem("payee_account_status", null);
            }
        }
        else {
            $("#lblGlobalPlanId,#lblGlobalPayeeAccStatus,#lblGlobalPayeeAccountId,#lblGlobalBenefitAccountType").val("");
        }
        if (sessionStorage.getItem("payee_account_id") > 0) {
            var myHtml = '';
            var myElement = $('#MenuUl li[formid="wfmMSSPensionPaymentDetailsMaintenance"]');
            if (myElement.nextAll().length > 0) {
                myElement.nextAll().each(function () {
                    myHtml += this.outerHTML;
                });
                if (!sessionStorage.getItem("retiree_html")) {
                    sessionStorage.setItem("retiree_html", myHtml);
                }
                else
                {
                    var myretireeHtml = sessionStorage.getItem("retiree_html");
                    myHtml = myretireeHtml;
                }
                myElement.nextAll().remove();
                var benActType = sessionStorage.getItem("benefit_account_type");
                if (benActType != null && benActType != undefined && benActType.length > 0) {
                    benActType = benActType.toString().trim();
                }
                var insertElement = '<li class="active" keytitle="' + benActType + '"><span class="left-menu-icon left-menu-collapsed" keytitle="' + benActType + '"></span><a>' + benActType + '</a><ul class="sub-menu">' + myHtml + '</ul></li>';
                $(insertElement).insertAfter('#MenuUl li[keytitle="Your Account(s)"]');
            }
        }
        else if (sessionStorage.getItem("payee_account_id") != undefined && sessionStorage.getItem("payee_account_id") != null && sessionStorage.getItem("payee_account_id") == 0 && sessionStorage.getItem("benefit_account_type") && sessionStorage.getItem("benefit_account_type") != undefined && sessionStorage.getItem("benefit_account_type") != null) {
            var myHtml1 = '';
            var retaindHtml = $('#MenuUl li[formid="wfmMSSOneZeroNineNineRMaintenance"]:first').nextAll();
            if (retaindHtml.length > 1) {
                retaindHtml.each(function () {
                    myHtml1 += this.outerHTML;
                });
                if ($('#MenuUl li[keytitle]:nth-child(2)').length > 0) {
                    if ($('#MenuUl li[keytitle]').length == 3) { //Ctrl+F5 issue.
                        $('#MenuUl li[keytitle]:nth-child(2)').remove();
                    }
                    $(retaindHtml).insertAfter('#MenuUl li[formid="wfmMSSPensionPaymentDetailsMaintenance"]');
                }
            }
        }
        var self = this;
        var MenuUl = $("#MenuUl");
        MenuUl.find("li").each(function () {
            var lstrFormId = $(this).attr("formid"), lstrFormTitle = $(this).text();

            if (self.IsRetiree && self.IsRetiree.value == "True") {

                if (lstrFormId != undefined && ((lstrFormId.indexOf("wfmMSSPayeeAccountsMaintenance") == 0 && lstrFormTitle == "Benefit Information Summary") ||
                   (lstrFormId.indexOf("wfmMSSBeneficiaryMaintenance") == 0 && lstrFormTitle == "Beneficiaries") ||
                   (lstrFormId.indexOf("wfmMSSPaymentHistorySummaryMaintenance") == 0 && lstrFormTitle == "Payment History Summary") ||
                   (lstrFormId.indexOf("wfmMSSPayeeAccountsMaintenance") == 0 && lstrFormTitle == "Tax Withholding Info") ||
                   (lstrFormId.indexOf("wfmMSSDirectDepositMaintenance") == 0 && lstrFormTitle == "Direct Deposit Info") ||
                   (lstrFormId.indexOf("wfmMSSOneZeroNineNineRMaintenance") == 0 && lstrFormTitle == "1099-R"))) {
                    $(this).hide();

                    if (sessionStorage.getItem("payee_account_id") != null && sessionStorage.getItem("payee_account_id") != "0") {
                        if (lstrFormId.indexOf("wfmMSSPayeeAccountsMaintenance") == 0 && lstrFormTitle == "Benefit Information Summary") {
                            $(this).show();
                        }
                        if (lstrFormId.indexOf("wfmMSSPayeeAccountsMaintenance") == 0 && lstrFormTitle == "Tax Withholding Info") {
                            if (nsCommon.sessionGet("IsTaxWithholdingPanelVisibile") == "false") {
                                $(this).hide();
                            }
                            else {
                                $(this).show();
                            }
                        }
                        else if (sessionStorage.getItem("plan_name") == "7" && sessionStorage.getItem("payee_account_status") == "DCRC") {
                            if (lstrFormId.indexOf("wfmMSSBeneficiaryMaintenance") == 0) {
                                $(this).show();
                            }
                        }
                        else {
                            if ((sessionStorage.getItem("plan_name") == "7" && sessionStorage.getItem("payee_account_status") != "DCRC")
                                || (sessionStorage.getItem("plan_name") == "23" && sessionStorage.getItem("payee_account_status") != "3RDP")
                                || (sessionStorage.getItem("plan_name") != null && sessionStorage.getItem("plan_name") != undefined && sessionStorage.getItem("plan_name") != "7" && sessionStorage.getItem("plan_name") != "23")) {
                                if (lstrFormId.indexOf("wfmMSSDirectDepositMaintenance") == 0
                                    || lstrFormId.indexOf("wfmMSSBeneficiaryMaintenance") == 0 || lstrFormId.indexOf("wfmMSSPaymentHistorySummaryMaintenance") == 0
                                    || lstrFormId.indexOf("wfmMSSOneZeroNineNineRMaintenance") == 0) {
                                    $(this).show();

                                }
                            }
                        }
 
                    }
                }
                else {
                    $(this).show();
                }
            }
        });
        if (e.context.activeDivID.indexOf("wfmUpdatePaymentMethodWizard") >= 0) {
            var ltxtRoutingNumber = $(e.context.activeDivID + " #txtRoutingNumber");
            if (ltxtRoutingNumber && ltxtRoutingNumber.text().length > 0)
            {
                ltxtRoutingNumber.trigger("change");
            }
        }
        if (e.context.activeDivID.indexOf("wfmAnnualEnrollmentBenefitPlansMaintenance") == 0 && ns.viewModel) {
            setTimeout(function () {
            var ldomDiv = $("#" + ns.viewModel.currentModel);
            if (ldomDiv.find('#rblRBDeferredComp_0').length > 0 && ldomDiv.find('#rblRBDeferredComp_0')[0].checked == true) {
                ldomDiv.find("#pnlNo").hide();
                ldomDiv.find("#pnlYes").show();
            }
            else if (ldomDiv.find('#rblRBDeferredComp_1').length > 0 && ldomDiv.find('#rblRBDeferredComp_1')[0].checked == true) {
                ldomDiv.find("#pnlNo").show();
                ldomDiv.find("#pnlYes").hide();
            }
            else {
                ldomDiv.find("#pnlYes").hide();
                ldomDiv.find("#pnlNo").hide();
            }
            }, 50);
        }
    },
    AfterApplyingUI: function (e) {
        if (e.context !== undefined) {                     
            $("#" + e.context.activeDivID).find(".k-tabstrip-items").each(function () {
                $(this).addClass("outofscreen");
            });

            setTimeout(function () {
                $("#" + e.context.activeDivID + " [data-role='dropdownlist']").each(function () {
                    nsCommon.SetDropDownWidth($(this));
                });
            }, 200);

            $("#" + e.context.activeDivID + " .HomeLink").removeAttr("base_click");
        }
        if (e.context.activeDivID.indexOf("wfmMSSHomeLimited") >= 0) {
            $(".page-content").css({ "width": "100%", "margin": "0px" });
            $(".page-header-wrap").css({ "width": "100%", "margin": "0px" });
            $(".menu-toggle,.welcome-text").css({ "display": "none" });         
        }
    },
    UploadDocument: function (e) {
        var lstrFormName = nsCommon.GetFormNameFromDivID(e.context.activeDivID);
        ns.DirtyData[e.context.activeDivID] = { HeaderData: {}, istrFormName: lstrFormName, KeysData: {} };
        ns.DirtyData[e.context.activeDivID].HeaderData = { MaintenanceData: {} };
        ns.DirtyData[e.context.activeDivID].HeaderData.MaintenanceData = ns.viewModel[e.context.activeDivID].HeaderData.MaintenanceData;
        ns.DirtyData[e.context.activeDivID].KeysData = ns.viewModel[e.context.activeDivID].KeysData;
        switch (lstrFormName) {
            case "wfmWssDocsUploadMaintenance":
                {
                    var ActiveDivID = nsCommon.GetActiveDivId(e);
                    if ($("#ddlWssDocumentTypes").val() == "") {
                        var strFileName = $(".s-fileupload-text").text();
                        var strErrorMessage = "Document Name Is Required, Your File " + strFileName +" Could Not Be Uploaded."
                        nsCommon.DispalyError(strErrorMessage, ActiveDivID);
                        $("span[class='s-fileupload-clear']").trigger("click");
                        return false;
                    }
                    $("#" + e.context.activeDivID).find("#ddlWssDocumentTypes").trigger("change");
                    var lstrFileId = ns.viewModel[e.context.activeDivID].HeaderData.MaintenanceData["ddlWssDocumentTypes"];
                    ns.DirtyData[e.context.activeDivID].HeaderData.MaintenanceData["ddlWssDocumentTypes"] = lstrFileId;
                }
                break;

        }      

        return true;
    },
    AfterBindFormData: function (e) {
        var ldomDiv = e.context.idomActiveDiv != undefined ? e.context.idomActiveDiv : $("#" + e.context.activeDivID);
        if (e.context.activeDivID && e.context.activeDivID.indexOf("wfmMSSUpdateContactMaintenance") >= 0) {
            chkLintSameAsPersonAddressNewVisibilty(ldomDiv);
        }
        if (e.context.activeDivID && e.context.activeDivID.indexOf("wfmMSSBenefitApplicationWizard") >= 0) {
            ldomDiv.find("#txtBNAppWizDisOne").hide();
            ldomDiv.find("#txtBNAppWizDisTwo").hide();
            ldomDiv.find("#txtBankNameInsPay").hide();
            ldomDiv.find("#pnlOTPVerify").hide();
        }



        if (e.context.activeDivID && e.context.activeDivID.indexOf("wfmMSSDirectDepositInformationMaintenance") >= 0) {
            ldomDiv.find("#lblBankName1,#lblBankName2").hide();
        }
        if (e.context.activeDivID && e.context.activeDivID.indexOf("wfmMSSUpdateContactMaintenance") >= 0) {
            AddressCountryValueVisibility(ldomDiv);
        }
        if (e.context.activeDivID && e.context.activeDivID.indexOf("wfmMSSVideoPlayer") == 0) {
            var str = ldomDiv.find("#lblLabel1").text();
            str = str.replace(".", "");
            var url1 = 'http://' + window.location.hostname + '/' + ns.SiteName + str;
            ldomDiv.find("#lblLabel>Video").attr("src", url1);        
            
        }
        $("#" + e.context.activeDivID + " .trow").each(function () {
            if ($(this).text() == "")
                $(this).remove();
        });
        $("#" + e.context.activeDivID).find("[class*='col-']").each(function () {
            if ($(this).text() == "")
                $(this).addClass("needtohide");
        });

        $("#" + e.context.activeDivID).find("[class*='col-']").not("[class*='CaptionDiv'],[class*='col-1']").each(function () {
            $(this).addClass("valuebox");
        });

        if (ns.viewModel != undefined && ns.viewModel.currentModel != undefined &&
             ns.viewModel[ns.viewModel.currentModel] !=undefined &&  
             ns.viewModel[ns.viewModel.currentModel].OtherData !=undefined && 
             ns.viewModel[ns.viewModel.currentModel].OtherData["MSSApplication_Graph"] !=undefined ) {
            var lobjData = ns.viewModel[ns.viewModel.currentModel].OtherData["MSSApplication_Graph"];
            if(lobjData !=undefined && lobjData.length)
                var lstrDivContent= '';
            {
                $.each(lobjData,function(index,value)
                {
                    if(index ==0)
                    {
                        lstrDivContent = "<div class='rowData ImageClass'>";
                    }
                    if (value["istrProgress"] != undefined) {
                        lstrDivContent += "<div class='columnData'><span>" + value["istrStatus"] + "</span>" + "<img src =/" + ns.SiteName + '/' + value["istrBaseImage"] + " /> <img src=/" + ns.SiteName + '/' + value["istrProgress"] + "  /> </div>";
                    } else {
                        lstrDivContent += "<div class='columnData'><span>" + value["istrStatus"] + "</span>" + "<img src =/" + ns.SiteName + '/' + value["istrBaseImage"] + " /> </div>";
                    }
                    

                });
                lstrDivContent +='</div>';

                var lstrActiveDivId = e.context.activeDivID;
                var ldomDiv = e.context.idomActiveDiv != undefined ? e.context.idomActiveDiv : $(["#", lstrActiveDivId].join(""));

                var lobjElement = document.createElement("div");

                lobjElement.innerHTML = lstrDivContent;
                var lojParent = ldomDiv.find("#lblLabel");
                if(lojParent !=undefined && lojParent.length)
                {
                    $(lobjElement).insertAfter(lojParent);
                }
            }
        }       
        // FMUpgrade : Added for Default page conversion of OnLoadComplete method
        if (e.context.activeDivID.indexOf("wfmMSSFormsNDPERSMaintenance") >= 0) {
            var lstrActiveDivId = e.context.activeDivID;
            var ldomDiv = e.context.idomActiveDiv != undefined ? e.context.idomActiveDiv : $(["#", lstrActiveDivId].join(""));
           
            if (ns.lstrRetiree == "True") {
                ldomDiv.find("#pnlActiveMember").hide();
                ldomDiv.find("#pnlRetireeMember").show();
            }
            else if (ns.lstrRetiree == "False") {
                ldomDiv.find("#pnlActiveMember").show();
                ldomDiv.find("#pnlRetireeMember").hide();
            }
              
        }
        if (e.context.activeDivID.indexOf("wfmServicePurchaseWebMaintenance") >= 0) {
            setTimeout(function () {
                var ldomDiv = e.context.idomActiveDiv != undefined ? e.context.idomActiveDiv : $(["#", lstrActiveDivId].join(""));
                var pnlClass = ldomDiv.find("#pnResults").css("display");
                if (ldomDiv.find("#pnResults").length > 0 && pnlClass != "" && pnlClass != "none")
                    nsCommon.DispalyMessage("[ Please scroll down to view the Service Purchase costs ]", e.context.activeDivID);
            }, 200);           
            
        }

        
        // FMUpgrade : Added for Default page conversion of OnLoadComplete method
        if (e.context.activeDivID.indexOf("wfmMSSProfileMaintenance") >= 0 || e.context.activeDivID.indexOf("wfmMSSBenefitApplicationWizard") >= 0) {            
            var lstrActiveDivId = e.context.activeDivID;
            var ldomDiv = e.context.idomActiveDiv != undefined ? e.context.idomActiveDiv : $(["#", lstrActiveDivId].join(""));
            var lstrURL = ldomDiv.find("#lblPeopleSoft").text();
            ldomDiv.find("#hypHyperLink").attr("href", lstrURL);
            ldomDiv.find("#hypHyperLink1").attr("href", lstrURL);
            ldomDiv.find("#hypHyperLink2").attr("href", lstrURL);
        }
        if (e.context.activeDivID.indexOf("wfmMSSActiveMemberHomeMaintenance") >= 0) {
            var lstrActiveDivId = e.context.activeDivID;
            var ldomDiv = e.context.idomActiveDiv != undefined ? e.context.idomActiveDiv : $(["#", lstrActiveDivId].join(""));
            var lstrURL = ldomDiv.find("#lblUpdActUrl").text();
            ldomDiv.find("#hypHyperLink").attr("href", lstrURL);
        }
        if (e.context.activeDivID.indexOf("wfmMSSAddressMaintenance") >= 0) {
            ldomDiv.find("#rblRadioButtonList").css("display", "none");
        }


        if (e.context.activeDivID.indexOf("wfmMSSAddressMaintenance") >= 0) {
            var ddlCountryValue = ldomDiv.find("#ddlAddrCountryValue");
            if (ddlCountryValue.val() == "0001")
                ldomDiv.find("#capForeignProvince,#txtForeignProvince,#capForeignPostalCode,#txtForeignPostalCode").css("display", "none");
        }
	//PIR-17683 Regular Health wizard landing page in MSS
        if (ldomDiv.find('#ddlPlanEnrollmentOptionValue').length > 0) {
            ldomDiv.find('#lblCancelMsgForNotInHDHP').hide();
            ldomDiv.find('#btnContinue').hide();
            ldomDiv.find('#btnDoNotContinue').hide();
            //ldomDiv.find('#lblIsUserLinkedInSTATDHSU').hide();
            ldomDiv.find('#lblIsHealthPlan').hide();
            ldomDiv.find('#ddlPlanEnrollmentOptionValue').change(ChangeControlVisibility);
            ChangeControlVisibility();
        }
        //if (e.context.activeDivID && e.context.activeDivID.indexOf("wfmMSSInboxMaintenance") === 0) {
        //    GetMessageCount();
        //}

        if (e.context.activeDivID && e.context.activeDivID.indexOf("wfmMSSBenefitApplicationWizard") >= 0) {         
            var DefDropDown = ldomDiv.find('#txtDeferalDateId');
            if (DefDropDown.length > 0) {

               DefDropDown.hide();
            }
        }
        

},
    PrintForm: function (divID) {
        $("#" + divID).jqprint({ debug: false });
        return false;
    },

    RemoveImage: function (e) {
        if (e != undefined && e.context != undefined && e.context.activeDivID != undefined) {
            var activeDiv = $("#" + e.context.activeDivID);
            if (activeDiv != undefined && activeDiv != null && activeDiv.length > 0) {
                var ImageDiv = activeDiv.find(".ImageClass");
                if (ImageDiv != undefined && ImageDiv != null && ImageDiv.length > 0) {
                    ImageDiv.remove();
                }
            }
        } 
        return true;
    },

SetPayeeAccountId: function (e) {

    var FormContainerID = "";
    var ActiveDivID = "";
    var RelatedGridID = "";
    var lbtnSelf;
    var event = undefined;
    if (e != undefined && e.tagName === "A") {
        event = e;
        lbtnSelf = $(e)[0];
        var FormContainerID = "#" + $(e).closest('div[role="group"]')[0].id;
        var ActiveDivID = nsCommon.GetActiveDivId(e);
        lbtnSelf = $(FormContainerID + " #" + ActiveDivID + " #" + GetControlAttribute(lbtnSelf, "sfwRelatedControl", ActiveDivID))[0];
        lintSelectedIndex = e.getAttribute("rowIndex");
    } else {
        lbtnSelf = e.target;
        ActiveDivID = nsCommon.GetActiveDivId(lbtnSelf);

    }

    RelatedGridID = GetControlAttribute(lbtnSelf, "sfwRelatedControl", ActiveDivID);
    var lobjGridWidget = nsCommon.GetWidgetByActiveDivIdAndControlId(ActiveDivID, (RelatedGridID != null ? RelatedGridID : ""));
    if (lobjGridWidget == undefined || lobjGridWidget.jsObject == undefined) {
        return false;
    }

    var ldictParams = nsCommon.GetNavigationParams(lbtnSelf, event);

    var aintPayeeAccountId = ldictParams.larrRows[0].aintPaymentPayeeAccountId;

    nsCommon.SyncPost("SetPayeeAccount?aintPayeeAccount=" + aintPayeeAccountId, aintPayeeAccountId);

    $("#hylnkBenefitInformationSummary").show();
    $("#hylnkPaymentSummary").show();
    $("#hylnkViewBenificiaries").show();
    $("#hylnkInsuranceApplication").show();
    $("#hyplnkTaxStatements").show();

    hylnkViewBenificiaries
    return true;
},
SelectPayeeAccount: function (e) {
    $("#hylnkBenefitInformationSummary").hide();
    $("#hylnkPaymentSummary").hide();
    $("#hylnkViewBenificiaries").hide();
    $("#hylnkInsuranceApplication").hide();
    $("#hyplnkTaxStatements").hide();
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
    if (ActiveDivID.lastIndexOf("wfmMSSSeminarMaintenance") === 0) {
        var lFileName = $("#" + ActiveDivID).find("#lblFullAddressToSearchInGoogle").text();
        window.open("http://"+ lFileName, "_blank", "width=800,height=800,resizable=yes,toolbar=yes,scrollbars=yes,alwaysRaised=yes");
    }
},


NavigateToLogin: function (e) {
    if (window.location.toString().indexOf('MSSUserRegistrationWizard') > -1) {
        var lstrMSSUserID =  $("#lblRegMSSUserID").text();
        nsCommon.SyncPost("CreateCookieForRegistration?astrUserID=" + lstrMSSUserID);
        window.location = window.location.origin + '/' + ns.SiteName + '/Account/LoginE'
    }
    else if (window.location.toString().indexOf('MSSUserLoginWizard') > -1 || window.location.toString().indexOf('MSSForgotUserNameWizard') > -1) {
        window.location = window.location.origin + '/' + ns.SiteName + '/Account/LoginE'
    }
    else {
        window.location = window.location.origin + '/' + ns.SiteName + '/Account/wfmLoginMI'
    }
    return false;
},

    SetAutoLoginForMSS: function (e) {
        ns.AutoLoginProp = 'lblPersonId';
        ns.AutoLoginUID = 'lblLoginUserID';
        ns.AutoLoginPwd = 'lblLoginPassword';
        nsRequest.AutoLogin();
        return false;
    },
    //FW Upgrade :: wfmDefault.aspx.cs code conversion(btn_OpenPDF method)
    btn_OpenPDF: function (e) {
        var FormContainerID = "", lbtnSelf, ldictParams, ActiveDivID = "";
        var lintSelectedIndex = -1;
        if (e != undefined && e.tagName === "A") {
            lbtnSelf = $(e)[0];
            //ldictParams = nsCommon.GetNavigationParams(lbtnSelf, e);
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
        if (ActiveDivID.lastIndexOf("wfmMSSLifeMaintenance") === 0 || ActiveDivID.lastIndexOf("wfmLifeEnrollmentWizard") === 0
            || ActiveDivID.lastIndexOf("wfmLifeAnnualEnrollmentWizard") === 0 || ActiveDivID.lastIndexOf("wfmMSSPensionPlanRetirementEnrollmentMaintenance") === 0
            || ActiveDivID.lastIndexOf("wfmMSSPensionPlanMaintenance") === 0 || ActiveDivID.lastIndexOf("wfmMSSLTCEnrollmentMaintenance") === 0
            || ActiveDivID.lastIndexOf("wfmMSSFlexCompEnrollmentMaintenance") === 0 || ActiveDivID.lastIndexOf("wfmServicePurchaseWebMaintenance") === 0
            || ActiveDivID.lastIndexOf("wfmMSSProfileMaintenance") === 0 || ActiveDivID.lastIndexOf("wfmActiveMemberBenefitPlansMaintenance") === 0
            || ActiveDivID.lastIndexOf("wfmAnnualEnrollmentBenefitPlansMaintenance") === 0 || ActiveDivID.lastIndexOf("wfmMSSOneZeroNineNineRMaintenance") === 0
            || ActiveDivID.lastIndexOf("wfmMSSAnnualStatementMaintenance") === 0 || ActiveDivID.lastIndexOf("wfmMSSBeneficiaryMaintenance") === 0
            || ActiveDivID.lastIndexOf("wfmMSSFormsNDPERSMaintenance") === 0 || ActiveDivID.lastIndexOf("wfmMSSBenefitPlanDetailsMaintenance") === 0
            || ActiveDivID.lastIndexOf("wfmMSSBenefitApplicationWizard") === 0
            //For button btnPdfCorrespondence_Click method, Below are the forms for particular method.
            || ActiveDivID.lastIndexOf("wfmMSSEAPMaintenance") === 0 || ActiveDivID.lastIndexOf("wfmMSSDBElectedOfficialMaintenance") === 0
            || ActiveDivID.lastIndexOf("wfmMSSActiveMemberProfileMaintenance") === 0 || ActiveDivID.lastIndexOf("wfmMSSGHDVMaintenance") === 0
            || ActiveDivID.lastIndexOf("wfmMSSLifeEnrollmentMaintenance") === 0 || ActiveDivID.lastIndexOf("wfmMSSLTCMaintenance") === 0
            || ActiveDivID.lastIndexOf("wfmMSSMemberDetailMaintenance") === 0 || ActiveDivID.lastIndexOf("wfmMSSMemberDetailWizard") === 0 || ActiveDivID.lastIndexOf("wfmMSSPensionPlanDCOptionalEnrollmentWizard") === 0
            || ActiveDivID.lastIndexOf("wfmMSSPensionPlanDCRetirementEnrollmentMaintenance") === 0 || ActiveDivID.lastIndexOf("wfmMSSPensionPlanMainRetirementOptionalEnrollmentWizard") === 0
            || ActiveDivID.lastIndexOf("wfmViewRequestLifeEnrollmentMaintenance") === 0 || ActiveDivID.lastIndexOf("wfmViewRequestPensionPlanDCOptionalEnrollmentMaintenance") === 0 || ActiveDivID.lastIndexOf("wfmViewRequestPensionPlanDCRetirementEnrollmentMaintenance") === 0
            || ActiveDivID.lastIndexOf("wfmViewRequestPensionPlanMainRetirementOptionalEnrollmentMaintenance") === 0 
            || ActiveDivID.lastIndexOf("wfmLifeAnnualEnrollmentPrintMaintenance") === 0) {
            var lFileName = ldictParams["larrRows"].map(function (item) { return item["pdf_file_name"] })[0];
            window.open(ns.SiteName + "/Home/OpenPDFRender?astrFileName=" + lFileName, "_blank", "width=800,height=800,resizable=yes,toolbar=yes,scrollbars=yes,alwaysRaised=yes");
        }
        
    },
    //GetPrefix: function () {
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
    //    if (ns.SiteName == "" && Prefix == "") {
    //        Prefix = "/";
    //    }
    //    return Prefix;
    //},
    //FW Upgrade :: wfmDefault.aspx.cs code conversion(btnPdfCorrespondence_Click method)
    btnPdfCorrespondence_Click: function (e) {
        nsUserFunctions.btn_OpenPDF(e);
    },
    ConfirmMSSAmount: function (e) {
        var ldomDiv = e.context.idomActiveDiv != undefined ? e.context.idomActiveDiv : $("#" + e.context.activeDivID);
        ldomDiv.find("#ddlIstrUpdateExistingProviderFlag").trigger("change");
        ldomDiv.find("#ddlIstrUpdateExistingEmployerMatchFlag").trigger("change");
        if (ldomDiv.find("#txtPerPayPeriodContributionAmt1").val() == "$0.00" || ldomDiv.find("#txtPerPayPeriodContributionAmt1").val() == "") {
            return confirm('The Amount Per Pay Period is Zero. Do you want to save the record?');
        }
        else if (ldomDiv.find("#txtPayPeriodAmount").val() == "$0.00" || ldomDiv.find("#txtPayPeriodAmount").val() == "") {
            return confirm('The Amount Per Pay Period is Zero. Do you want to save the record?');            
        }
        return true;
    },
     // F/W Upgrade PIR:11918 -MSS- Navigation issue Cancel Click 
    fnRemoveCurrentForm: function (e) {
        if (ns.viewModel.currentModel.indexOf("wfmMSSDirectDepositInformationMaintenance") >= 0) { 
            nsEvents.OnDeleteFormClick(ns.viewModel.currentModel);
        }
        return false;
    },
    ValidateSelectedFiles: function (e) {
        if (e.context.previouslySelectedFiles != undefined && e.context.previouslySelectedFiles.length == 1) {
            return false;
        }
        return true;
    },
    ShowBalletPanel: function (e) {
        var ldomDiv = $("#" + ns.viewModel.currentModel);
        var pnlMSSBoardBallet = ldomDiv.find("#pnlMSSBoardBallet");
        pnlMSSBoardBallet.removeClass('hideControl');
        pnlMSSBoardBallet.show();
        ldomDiv.find('#btnSubmitVote').focus(); 
        e.target.style.display = "none";
        return false;
    },
    OpenPopupSuccess: function (e) {
        var OpenPopupSuccessWindow = $('#OpenPopupSuccess');
        if (OpenPopupSuccessWindow.length > 0) {
            if (!OpenPopupSuccessWindow.data("kendoWindow")) {
                OpenPopupSuccessWindow.kendoWindow({
                    width: "600px",
                    height: "600px",
                    actions: [],
                    visible: false,
                    modal: true,
                    title: "You Voted!"
                });
            }
            OpenPopupSuccessWindow.data("kendoWindow").open().center();
            OpenPopupSuccessWindow.focus();
        }
    },
    ReturnToHomePage: function () {
        $('#OpenPopupSuccess').data("kendoWindow").close();
        $("#lnkMssHome").trigger("click");
        
    },
    OpenCandidateBio: function (e) {
        var nextElement = $(e).next();
        if (nextElement.hasClass('hideControl'))
        {
            nextElement.removeClass('hideControl');
            nextElement.show();
        }
        else
        {
            nextElement.addClass('hideControl');
            nextElement.hide();
        }
        return false;
    },
    IsBankInfoAlreadyExists: function (e) {
        if (ns.viewModel.currentModel.indexOf("wfmUpdatePaymentMethodWizard") >= 0) {
            if ($("#wzsBankAccountInfo").is(":visible")) {
                var ldomDiv = $("#" + ns.viewModel.currentModel);
                var RoutingNumber = ldomDiv.find("#txtRoutingNumber").val();
                var AccountNumber = ldomDiv.find("#txtTextBox2").val();
                var AccountType = ldomDiv.find("#ddlDropDownList1").val();

                var PersonAccountIds = "";
                if (ldomDiv.find("#ddlDropDownList").val() == 'PRDE') {
                    PersonAccountIds = ldomDiv.find("#lblHealthMedicarePersonAccountId").text() + ",";
                }
                if (ldomDiv.find("#ddlDropDownList2").val() == 'PRDE') {
                    PersonAccountIds += ldomDiv.find("#lblDentalPersonAccountId").text() + ",";
                }
                if (ldomDiv.find("#ddlDropDownList3").val() == 'PRDE') {
                    PersonAccountIds += ldomDiv.find("#lblVisionPersonAccountId").text() + ",";
                }
                if (ldomDiv.find("#ddlDropDownList4").val() == 'PRDE') {
                    PersonAccountIds += ldomDiv.find("#lblLifePersonAccountId").text() + ",";
                }
                var Parameters = {};
                Parameters["astrRoutingNumber"] = RoutingNumber;
                Parameters["astrAccountNumber"] = AccountNumber;
                Parameters["astrAccountType"] = AccountType;
                Parameters["astrPersonAccountIds"] = PersonAccountIds;
                var data = {};
                data = nsCommon.SyncPost("IsBankInfoAlreadyExists", Parameters, "POST");
                if (data) {
                   return alert("This banking information is already on file.");
                }
                return true;
            }
        }
    },

};

ClearBenefitCalculationComparasionData = function (e) {
    var dataitem = nsCommon.GetDataItemFromDivID("wfmMSSBenefitCalculationComparisonMaintenance0");
    if (dataitem != undefined) {
        OnDeleteFormClick("wfmMSSBenefitCalculationComparisonMaintenance0");
    }
    return true;
}


var FM_DispalyMessage = nsCommon.DispalyMessage;
nsCommon.DispalyMessage = function (astrMessage, ActiveDivID) {
    if ((astrMessage != null && astrMessage != undefined && (ActiveDivID.indexOf("wfmMSSVideoPlayer") == 0 && astrMessage.indexOf("[ Record displayed.]")))
         || ((ActiveDivID.indexOf("wfmMSSRetBenAppsLandingScreenMaintenance") >= 0) && astrMessage.indexOf("Record displayed.") >= 0)
        || ((ActiveDivID.indexOf("wfmMSSSelectRetPlanMaintenance") >= 0) && astrMessage.indexOf("Record displayed.") >= 0)
        || ((ActiveDivID.indexOf("wfmMSSActiveMemberHomeMaintenance") >= 0) && astrMessage.indexOf("Record displayed.") >= 0)        
        || ((ActiveDivID.indexOf("wfmMSSRetireeHomeMaintenance") >= 0) && astrMessage.indexOf("Record displayed.") >= 0)
        || ((ActiveDivID.indexOf("wfmMSSPersonAccountGhdvHsaHistoryMaintenance") >= 0) && astrMessage.indexOf("Record displayed.") >= 0)) {            
        astrMessage = "";
    }
    if (astrMessage != null && astrMessage != undefined && ((ActiveDivID.indexOf("wfmMSSDeferredCompEnrolledMaintenance") == 0 || ActiveDivID.indexOf("wfmActiveMemberBenefitPlansMaintenance") == 0 || ActiveDivID.indexOf("wfmAnnualEnrollmentBenefitPlansMaintenance") == 0 || ActiveDivID.indexOf("wfmInsurancePlansMaintenance") == 0 || ActiveDivID.indexOf("wfmMSSPersonAccountGhdvHsaHistoryMaintenance") == 0) && astrMessage.indexOf("[ All changes successfully cancelled. ]"))) {
        astrMessage = "";
    }
    if (astrMessage != null && astrMessage != undefined && (astrMessage.indexOf("[ Select Case. ]") >= 0 || astrMessage.indexOf("[ Select Process. ]") >= 0)) {
        astrMessage = "";
    }
    FM_DispalyMessage(astrMessage, ActiveDivID);
}

nsCommon.GetPageTitle = function (title, primarykey) {
    return title.replace("_PrimaryKey", '');
}
 


var Base_nsWizard_SetPositionOfActionBar = nsWizard.SetPositionOfActionBar;
nsWizard.SetPositionOfActionBar = function (adomStepDiv) {
    Base_nsWizard_SetPositionOfActionBar(adomStepDiv);
  
    setTimeout(function () {
        if (ns.viewModel.currentModel.indexOf("wfmUpdatePaymentMethodWizard") == 0 && $("#wzsReviewAndAuthorize").is(":visible")
            || ns.viewModel.currentModel.indexOf("wfmFlexEnrollmentWizard") == 0 && $("#wzsReviewYourEnrollment").is(":visible")
            || ns.viewModel.currentModel.indexOf("wfmLifeEnrollmentWizard") == 0 && $("#wzsStep5").is(":visible")
            || ns.viewModel.currentModel.indexOf("wfmMSSDeferredCompWizard") == 0 && $("#wzsStep4").is(":visible")
            || ns.viewModel.currentModel.indexOf("wfmMSSGHDVInsuranceEnrollmentWizard") == 0 && $("#wzsStep6").is(":visible")
            || ns.viewModel.currentModel.indexOf("wfmMSSBenefitApplicationWizard") == 0 && $("#wzsSumE-Sig").is(":visible")) {
            $("div[class='actionBar'] a[class='buttonNext']").text("Finish");
            
            $("div[class='actionBar'] a[class='buttonFinish buttonDisabled']").hide();
            if (ns.viewModel.currentModel.indexOf("wfmFlexEnrollmentWizard") == 0 && $("#wzsReviewYourEnrollment").is(":visible")) {
                $("#rblReviewUPLAOptions").find("input").attr("disabled", "disabled");
            }
            if (ns.viewModel.currentModel.indexOf("wfmMSSBenefitApplicationWizard") == 0 && $("#wzsSumE-Sig").is(":visible")) {
                var BenAppVal = ns.viewModel[ns.viewModel.currentModel].OtherData["SelectedBenefitOptionValue"];
                if (BenAppVal == "RGRD") {
                    $("div[class='actionBar'] a[class='buttonNext']").hide();
                    $("div[class='actionBar'] input[class='customActionBarButton']").addClass('button');
                    
                    if($("#pnlOTPVerify").is(":visible")) {
                        $("btnSaveAndNextInOTPPanel").show();
                        $("div[class='actionBar'] a[class='buttonPrevious']").hide();
                        $("div[class='actionBar'] a[class='button btnWizardNext_Click_button']").hide();
                        $("div[class='actionBar'] a[class='buttonPrevious wizard-hideonvisblerule buttonDisabled']").hide();
                        $("div[class='actionBar'] input[class='customActionBarButton button']").hide();
                        $("#btnNextOTPPanel").hide();
                    }
                }
                else {
                    $("#btnSaveAndNextInOTPPanel").hide();
                    $("div[class='actionBar'] a[class='buttonPrevious wizard-hideonvisblerule buttonDisabled']").show();
                    $("div[class='actionBar'] a[class='buttonNext']").show();
                }
                $("div[class='actionBar'] a[class='buttonNext']").bind("click", function () {
		            if ($("div[class='actionBar'] a[class='buttonNext']").text().toLowerCase() == "finish") {
	                    setOTPPanelVisibility(true);
	                    return true;
		            }
                });
                $("#btnNextOTPPanel").bind("click", function () {
                    setOTPPanelVisibility(true);
                    return true;
                });
                $("#btnPreviousClick").bind("click", function () {
                    setOTPPanelVisibility(false);
                    return true;
                });
            }
        }
        else {
            $("div[class='actionBar'] a[class='buttonNext']").show();
            $("div[class='actionBar'] a[class='buttonNext']").text("Next");
            $("div[class='actionBar'] a[class='buttonFinish buttonDisabled']").show();
        }
        if (ns.viewModel && ns.viewModel.currentModel && ((ns.viewModel.currentModel.indexOf("wfmMSSGHDVAnnualEnrollmentWizard") == 0 && $("#pnlMemberAccountexists").is(":visible"))
            || (ns.viewModel.currentModel.indexOf("wfmMaritalChangeWizard") == 0 && $("#wzsIntro").is(":visible")) ||
            (ns.viewModel.currentModel.indexOf("wfmBenefitCalculationWebWizard") == 0 && $("#wzsIntroStep").is(":visible"))
            || (ns.viewModel.currentModel.indexOf("wfmMSSGHDVAnnualEnrollmentWizard") == 0 && $("#pnlNOMemberAccount").is(":visible"))
            || (ns.viewModel.currentModel.indexOf("wfmLifeAnnualEnrollmentWizard") == 0 && $("#pnlMemberAccountExists").is(":visible"))
            || (ns.viewModel.currentModel.indexOf("wfmLifeAnnualEnrollmentWizard") == 0 && $("#pnlNoMemberAccountExists").is(":visible"))
            //|| (ns.viewModel.currentModel.indexOf("wfmLifeAnnualEnrollmentWizard") == 0 && $("#wzsStep1").is(":visible"))
            //|| (ns.viewModel.currentModel.indexOf("wfmLifeAnnualEnrollmentWizard") == 0 && $("#wzsStep2").is(":visible"))
            //|| (ns.viewModel.currentModel.indexOf("wfmLifeAnnualEnrollmentWizard") == 0 && $("#wzsStep3").is(":visible"))
            //|| (ns.viewModel.currentModel.indexOf("wfmLifeAnnualEnrollmentWizard") == 0 && $("#wzsStep4").is(":visible"))
            || (ns.viewModel.currentModel.indexOf("wfmFlexAnnualEnrollmentWizard") == 0 && $("#wzsIntro").is(":visible"))
            || (ns.viewModel.currentModel.indexOf("wfmLifeEnrollmentWizard") == 0 && $("#wzsIntro").is(":visible"))
            || (ns.viewModel.currentModel.indexOf("wfmFlexEnrollmentWizard") == 0 && $("#wzsIntro").is(":visible"))
            || (ns.viewModel.currentModel.indexOf("wfmFlexEnrollmentWizard") == 0 && $("#wzsNonNDPERSPreTax").is(":visible"))
            || (ns.viewModel.currentModel.indexOf("wfmMSSGHDVInsuranceEnrollmentWizard") == 0 && $("#wzsIntro").is(":visible"))
            || (ns.viewModel.currentModel.indexOf("wfmMSSDeferredCompWizard") == 0 && $("#wzsIntro").is(":visible"))
            || (ns.viewModel.currentModel.indexOf("wfmMSSPensionPlanMainRetirementOptionalEnrollmentWizard") == 0 && $("#wzsIntro").is(":visible"))
            || (ns.viewModel.currentModel.indexOf("wfmUpdatePaymentMethodWizard") == 0 && $("#wzsPrintSummary").is(":visible"))
            || (ns.viewModel.currentModel.indexOf("wfmFlexEnrollmentWizard") == 0 && $("#wzsPrint").is(":visible"))
            || (ns.viewModel.currentModel.indexOf("wfmLifeEnrollmentWizard") == 0 && $("#wzsStep6").is(":visible")
            || (ns.viewModel.currentModel.indexOf("wfmMSSDeferredCompWizard") == 0 && $("#wzsStep6").is(":visible"))
            || (ns.viewModel.currentModel.indexOf("wfmMSSGHDVInsuranceEnrollmentWizard") == 0 && $("#wzsStep7").is(":visible"))
            || (ns.viewModel.currentModel.indexOf("wfmMSSBenefitApplicationWizard") == 0 && $("#wzsPrint").is(":visible")))
            )) {
            $(".actionBar").hide();
            if (ns.viewModel.currentModel.indexOf("wfmFlexEnrollmentWizard") == 0 && $("#wzsPrint").is(":visible")) {
                var ldomDiv = $("#" + ns.viewModel.currentModel);
                if (ldomDiv.find('#rblPrintUPLAOptions_0')[0].checked){
                    ldomDiv.find('#rblPrintUPLAOptions_0').attr("checked", "true");
                }
                if (ldomDiv.find('#rblPrintUPLAOptions_1')[0].checked) {
                    ldomDiv.find('#rblPrintUPLAOptions_1').attr("checked", "true");
                }
                if (ldomDiv.find('#rblPrintUPLAOptions_2')[0].checked) {
                    ldomDiv.find('#rblPrintUPLAOptions_2').attr("checked", "true");
                }
                $("#rblPrintUPLAOptions").find("input").attr("disabled", "disabled");
            }
        }
        else {
            $(".actionBar").show();
        }

        if (ns.viewModel.currentModel.indexOf("wfmMSSDeferredCompWizard") == 0 && $("#wzsStep4").is(":visible") && $("#pnlQuickAcknow").is(":visible")) {
            $("div[class='actionBar'] a[class='buttonNext']").text("Finish");
        }
        if (ns.viewModel.currentModel.indexOf("wfmMSSDeferredCompWizard") == 0 && $("#wzsStep4").is(":visible") && $("#pnlRegAcknow").is(":visible")) {
            $("div[class='actionBar'] a[class='buttonNext']").text("Complete Enrollment");
        }

        if (ns.viewModel.currentModel.indexOf("wfmMSSAdditionalContributionWizard") == 0 && ($("#wzsAddContryAck").is(":visible") || $("#wzsAddContryPrint").is(":visible"))) {
            $("div[class='actionBar'] a[class='buttonNext']").hide();
            $("div[class='actionBar'] a[class='buttonFinish buttonDisabled']").hide();
            if($("#wzsAddContryPrint").is(":visible"))
                $("div[class='actionBar'] a[class='buttonPrevious buttonDisabled']").hide();
        }
        if (ns.viewModel.currentModel.indexOf("wfmMSSBenefitApplicationWizard") == 0 ){
            var ldomDiv = $("#" + ns.viewModel.currentModel);
            if (ldomDiv.find("#lblUpdatePersonAddress").text() == "true") {
                ldomDiv.find("#btnAddress").closest('div').css("display", "block");
                ldomDiv.find("#btnAddress").css("display", "block");
            }
        }
        if (ns.viewModel.currentModel.indexOf("wfmMSSBenefitApplicationWizard") == 0 && $("#wzsDistribution").is(":visible")) {
            var ldomDiv = $("#" + ns.viewModel.currentModel);
            if (ldomDiv.find("#ddlIstrRefundDistribution").val() == "RPAR" || ldomDiv.find("#ddlIstrRefundDistribution").val() == "RPMR") {
                ldomDiv.find("#pnlRolloverDetail").closest('div').css("display", "block");
                ldomDiv.find("#pnlRolloverDetail").css("display", "block");
            }

            if (ldomDiv.find("#lblbentypeRetirement").text() == "RETR") {
                ldomDiv.find("#lblACHDepositeInformation").closest('div').css("display", "block");
                ldomDiv.find("#lblACHDepositeInformation").css("display", "block");
            }
            
            var NonTaxableAmount = ldomDiv.find("#lblIdecMSSTotalNonTaxable").text();
            NonTaxableAmount = Number(NonTaxableAmount.replace(/[^0-9.-]+/g, ""));
            var ldomDiv = $("#" + ns.viewModel.currentModel);
            if (ldomDiv.find("#ddlIstrRefundDistribution").val() == "RPAR" && ldomDiv.find("#ddlRolloverOptionValue").val() == "ALOT" && NonTaxableAmount > 0 || ldomDiv.find("#ddlRolloverOptionValue").val() == "DLOT" || ldomDiv.find("#ddlRolloverOptionValue").val() == "PROT") {

                ldomDiv.find("#pnlACHDepositinfo").closest('div').css("display", "block");
                ldomDiv.find("#pnlACHDepositinfo").css("display", "block");
                ldomDiv.find("#capRoutingNo").closest('div').css("display", "block");
                ldomDiv.find("#capRoutingNo").css("display", "block");
                ldomDiv.find("#capBankName").closest('div').css("display", "block");
                ldomDiv.find("#capBankName").css("display", "block");
                ldomDiv.find("#txtRNAppWizDisOne").closest('div').css("display", "block");
                ldomDiv.find("#txtRNAppWizDisOne").css("display", "block");
                ldomDiv.find("#lblBNAppWizDisOne").closest('div').css("display", "block");
                ldomDiv.find("#lblBNAppWizDisOne").css("display", "block");
                ldomDiv.find("#txtBNAppWizDisOne").closest('div').css("display", "block");
                ldomDiv.find("#txtBNAppWizDisOne").css("display", "block");
                ldomDiv.find("#capACHAccountNumber").closest('div').css("display", "block");
                ldomDiv.find("#capACHAccountNumber").css("display", "block");
                ldomDiv.find("#txtACHAccountNumber").closest('div').css("display", "block");
                ldomDiv.find("#txtACHAccountNumber").css("display", "block");
                ldomDiv.find("#capBankAccountTypeValue").closest('div').css("display", "block");
                ldomDiv.find("#capBankAccountTypeValue").css("display", "block");
                ldomDiv.find("#ddlBankAccountTypeValue").closest('div').css("display", "block");
                ldomDiv.find("#ddlBankAccountTypeValue").css("display", "block");
                
                

            }
            $(document).on("change", "#ddlIstrRefundDistribution", function (e) {
                if (ns.viewModel.currentModel.indexOf("wfmMSSBenefitApplicationWizard") == 0) {
                    var ldomDiv = $("#" + ns.viewModel.currentModel);
                    var ddlChildValue = ldomDiv.find("#ddlRolloverOptionValue");
                    if (ldomDiv.find("#ddlIstrRefundDistribution").val() == "RPAR") {
                        ldomDiv.find("#pnlACHDepositinfo").show();
                        ldomDiv.find("#pnlACHDepositinfo").closest('div').css("display", "block");
                        ldomDiv.find("#pnlACHDepositinfo").css("display", "block");
                        ldomDiv.find("#capRoutingNo").closest('div').css("display", "block");
                        ldomDiv.find("#capRoutingNo").css("display", "block");
                        ldomDiv.find("#capBankName").closest('div').css("display", "block");
                        ldomDiv.find("#capBankName").css("display", "block");
                        ldomDiv.find("#txtRNAppWizDisOne").closest('div').css("display", "block");
                        ldomDiv.find("#txtRNAppWizDisOne").css("display", "block");
                        ldomDiv.find("#lblBNAppWizDisOne").closest('div').css("display", "block");
                        ldomDiv.find("#lblBNAppWizDisOne").css("display", "block");
                        ldomDiv.find("#txtBNAppWizDisOne").closest('div').css("display", "block");
                        ldomDiv.find("#txtBNAppWizDisOne").css("display", "block");
                        ldomDiv.find("#capACHAccountNumber").closest('div').css("display", "block");
                        ldomDiv.find("#capACHAccountNumber").css("display", "block");
                        ldomDiv.find("#txtACHAccountNumber").closest('div').css("display", "block");
                        ldomDiv.find("#txtACHAccountNumber").css("display", "block");
                        ldomDiv.find("#capBankAccountTypeValue").closest('div').css("display", "block");
                        ldomDiv.find("#capBankAccountTypeValue").css("display", "block");
                        ldomDiv.find("#ddlBankAccountTypeValue").closest('div').css("display", "block");
                        ldomDiv.find("#ddlBankAccountTypeValue").css("display", "block");
                        ldomDiv.find("#ddlRolloverOptionValue").trigger("change");
                        
                    }
                }
            });
            
            
            if (ldomDiv.find("#ddlRolloverTypeValue")!= null && ldomDiv.find("#ddlRolloverTypeValue").val() == "RIRA") {
                ldomDiv.find("#lblStateTax").show();
                ldomDiv.find("#lblStateTaxReqInformation").show();
                ldomDiv.find("#ddlStateTax").show();
            }
            else if (ldomDiv.find("#lblStateTax") != null && ldomDiv.find("#lblStateTaxReqInformation") != null &&  ldomDiv.find("#ddlStateTax") != null) {
                ldomDiv.find("#lblStateTax").hide();
                ldomDiv.find("#lblStateTaxReqInformation").hide();
                ldomDiv.find("#ddlStateTax").hide();
            }
            var item = ldomDiv.find("#ddlRolloverOptionValue");
            var NonTaxableAmount = ldomDiv.find("#lblIdecMSSTotalNonTaxable").text();
            NonTaxableAmount = Number(NonTaxableAmount.replace(/[^0-9.-]+/g, ""));
            if (item != null && item.val() == "ALOG" && NonTaxableAmount > 0) {
                ldomDiv.find("#lblRolloverText").show();
                ldomDiv.find("#lblRolloverTextSumry").show();
                ldomDiv.find("#lblRolloverTextPrnt").show();
            }
            else {
                ldomDiv.find("#lblRolloverText").hide();
                ldomDiv.find("#lblRolloverTextSumry").hide();
                ldomDiv.find("#lblRolloverTextPrnt").hide();
            }
	    
            SetVisibilityPostRetreivalOne();
        }
        if (ns.viewModel.currentModel.indexOf("wfmMSSBenefitApplicationWizard") == 0 && ($("#wzsHealth").is(":visible") || $("#wzsDental").is(":visible") || $("#wzsVision").is(":visible")))
        {
            setBenAppWizardVisibility();
        }
        if (ns.viewModel.currentModel.indexOf("wfmMSSBenefitApplicationWizard") == 0 && $("#wzsStepLifeInsurance").is(":visible")) {
            $("#lblLifeStepDependantReqInformation_0").hide();
        }
        if (ns.viewModel.currentModel.indexOf("wfmMSSBenefitApplicationWizard") == 0 && $("#wzsSelectBenefitType").is(":visible")) {
            var ldomDiv = $("#" + ns.viewModel.currentModel);	    
	        var BenAppVal = ns.viewModel[ns.viewModel.currentModel].OtherData["SelectedBenefitOptionValue"];
            if (BenAppVal == undefined) {
                ldomDiv.find("#lblBenOption").hide();
                ldomDiv.find("#ddlSelBenOption").hide();
                ldomDiv.find("#btnMainAndPublicSafetyRetirementOptionsPdf").hide();
            }
            else {
                GetBenOptionsByEffectiveDate(ldomDiv);
                ldomDiv.find("#ddlSelBenOption").val(BenAppVal);
            }
            var BenType = nsCommon.sessionGet("benTypeValue");
            if (BenType != undefined || BenType != null) {
                ldomDiv.find("#ddlSelBenType").val(BenType);
                ldomDiv.find("#ddlSelBenType").trigger("change");
            }
            
            if (ldomDiv.find("#ddlSelBenType").val() == "RETR" && ldomDiv.find("#txtRetEffecDate").val() != "" && !ldomDiv.find('#lblBenOption').is(":visible"))
            {
                ldomDiv.find("#txtRetEffecDate").trigger("blur");
            }
            $("div[class='actionBar'] a[class='buttonPrevious buttonDisabled']").hide();
            $("div[class='actionBar'] a[class='buttonNext']").hide();
            $("div[class='actionBar'] input[class='customActionBarButton']").addClass('button');
        }
        else {
            if ($("#pnlOTPVerify").is(":visible")) {
                $("div[class='actionBar'] a[class='buttonPrevious buttonDisabled']").hide();
                $("div[class='actionBar'] a[class='buttonPrevious wizard-hideonvisblerule buttonDisabled']").hide();
            }
            else {
                $("div[class='actionBar'] a[class='buttonPrevious']").show();
            }
            $("div[class='actionBar'] input[class='customActionBarButton']").addClass('button');
        }
    }, 0)
}

var base_nsWizard_onFinishCallback = nsWizard.onFinishCallback;
nsWizard.onFinishCallback = function (obj, context) {
    base_nsWizard_onFinishCallback(obj, context);
    
    // FMUpgrade : Added for Default page conversion of OnLoadComplete method
    if (ns.viewModel && ns.viewModel.currentModel && ns.viewModel.currentModel.indexOf("wfmMaritalChangeWizard") == 0) {
        
        if (parseInt($("#lblEligible").text()) == 0 && parseInt($("#lblEnroll").text()) == 0)
            $("#" + ns.viewModel.currentModel).find("#btnOpenActive").trigger("click");
        else if (ns.lstrExternalUser == "True" && ns.lstrPersonCertify == "False")
            $("#" + ns.viewModel.currentModel).find("#btnOpenProfile").trigger("click");
        else
            $("#" + ns.viewModel.currentModel).find("#btnOpenInsurance").trigger("click");
    }
}

var FM_DispalyError = nsCommon.DispalyError;
nsCommon.DispalyError = function (astrMessage, ActiveDivID, ablnScrollToTop) {
    if ((ns.viewModel.currentForm && ns.viewModel.currentForm.indexOf("wfmMaritalChangeWizard") >= 0               
        && astrMessage.indexOf("Errors found.  Record not saved.") >= 0)
        || (ns.viewModel.currentForm && ns.viewModel.currentForm.indexOf("wfmBenefitCalculationWebWizard") >= 0 
            && astrMessage.indexOf("Errors found.  Record not saved.") >= 0) ||
         (ns.viewModel.currentForm && ns.viewModel.currentForm.indexOf("wfmMSSGHDVAnnualEnrollmentWizard") >= 0
            && astrMessage.indexOf("Errors found.  Record not saved.") >= 0)
        || (ns.viewModel.currentForm && ns.viewModel.currentForm.indexOf("wfmLifeAnnualEnrollmentWizard") >= 0
            && astrMessage.indexOf("Errors found.  Record not saved.") >= 0)
        || (ns.viewModel.currentForm && ns.viewModel.currentForm.indexOf("wfmUpdatePaymentMethodWizard") >= 0
            && astrMessage.indexOf("Errors found.  Record not saved.") >= 0)
        || (ns.viewModel.currentForm && ns.viewModel.currentForm.indexOf("wfmMSSDeferredCompWizard") >= 0
            && astrMessage.indexOf("Errors found.  Record not saved.") >= 0)
        || (ns.viewModel.currentForm && ns.viewModel.currentForm.indexOf("wfmMSSGHDVInsuranceEnrollmentWizard") >= 0
            && astrMessage.indexOf("Errors found.  Record not saved.") >= 0)
        || (ns.viewModel.currentForm && ns.viewModel.currentForm.indexOf("wfmFlexAnnualEnrollmentWizard") >= 0
            && astrMessage.indexOf("Errors found.  Record not saved.") >= 0)
        || (ns.viewModel.currentForm && ns.viewModel.currentForm.indexOf("wfmMSSAdditionalContributionWizard") >= 0
            && astrMessage.indexOf("Errors found.  Record not saved.") >= 0))
    {
        ns.viewModel.currentModel = ns.viewModel.previousForm;
        return;
    }
    var ldomDiv = $("#" + ns.viewModel.currentModel);
    if ((ns.viewModel.currentForm && ns.viewModel.currentForm.indexOf("wfmMSSAddressMaintenance") >= 0)
        && (ldomDiv.find("div[id='wfmMSSAddressMaintenance0ErrorDiv']>ul>li[errormessage='Warning: Address is Invalid. Please enter a valid address']").length > 0)) {
        ldomDiv.find("#capIstrSuppressWarning,#rblRadioButtonList").css("display", "block");
    }
    else {
        if (ns.viewModel.currentForm.indexOf("wfmMSSAddressMaintenance") >= 0) {
            ldomDiv.find("#capIstrSuppressWarning,#rblRadioButtonList").css("display", "none");
        }
    }

    if ((ns.viewModel.currentForm && ns.viewModel.currentForm.indexOf("wfmMSSBenefitEstimateMaintenance") >= 0)
        && (ldomDiv.find("div[id='wfmMSSBenefitEstimateMaintenance0ErrorDiv']>ul>li[errormessage='Retirement type for an Estimate is mandatory.']").length > 0)) {
        ldomDiv.find('#cblRetirementTypeValue').attr("style", "border:solid white 1px !important");
        ldomDiv.find('#ChkBoxGrp').css("border", "solid red 1px");
    }
    else {
        ldomDiv.find('#ChkBoxGrp').css("border", "solid white 1px");
    }
   
    FM_DispalyError(astrMessage, ActiveDivID, ablnScrollToTop);
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

//FMUpgrade: Added from NSHeadaer for wfmMSSProfileMaintenance btnSave.
function IsEmailWaiverFlagSelected() {      
    if ($('#chkEmailWaiverFlag').length > 0 && $('#chkEmailWaiverFlag')[0].checked != null && $('#chkEmailWaiverFlag')[0].checked) {
        //return alert($$('lblIstrIsEmailWaiverFlagSelected').text());
        if ($('#lblIstrIsEmailWaiverFlagSelected').text() != "") {
            var res = confirm($('#lblIstrIsEmailWaiverFlagSelected').text());
            if (res) {
                return true;
            }
            return false;
        }
    }
    return true;
}

function chkLintSameAsPersonAddressNewVisibilty(ldomDiv) {
    //ldomDiv.find('#chkLintSameAsPersonAddressNew').is(":checked")
    if (ldomDiv.find('#chkLintSameAsPersonAddressNew').is(":checked")) {
        ldomDiv.find('#lbladdressLine1,#lbladdressLine2,#lblCity,#lbladdressState,#lblCountry,#lblZipCode').show();
        ldomDiv.find('#txtaddressLine1,#lblLabel8,#txtaddressLine2,#lblLabel9,#txtCity,#ddlAddressStateValue,#ddlAddressCountryValue,#txtAddressZipCode,#txtZip4Code,#txtForeignPostalCode,#txtForeignProvince').hide();
    }
    else {
        ldomDiv.find('#txtaddressLine1,#lblLabel8,#txtaddressLine2,#lblLabel9,#txtCity,#ddlAddressStateValue,#ddlAddressCountryValue,#txtAddressZipCode,#txtZip4Code,#txtForeignPostalCode,#txtForeignProvince').show();
        ldomDiv.find('#lbladdressLine1,#lbladdressLine2,#lblCity,#lbladdressState,#lblCountry,#lblZipCode').hide();
    } 
}

function AddressCountryValueVisibility(ldomDiv) {
    var AddressCountryValue = ldomDiv.find("#ddlAddressCountryValue option:selected");
    if (AddressCountryValue.val() == '0001') {
        ldomDiv.find('#lblStateMand,#lblZipCodeMand').show();
    }
    else {
        ldomDiv.find('#lblStateMand,#lblZipCodeMand').hide();
    }
}
ns.FormatError = function (id, error) {
    return [error].join('');
};
nsWizard.hideStepsFromProgressBar = function (WizardObj, WizardData) {
        if (WizardData == undefined) {
            $(WizardObj.ProgressItems).find(".HideStepByRule").closest("li.HideStepByRule").show().removeClass("HideStepByRule").end().show().removeClass("HideStepByRule");
            $(WizardObj.target).find(".HideStepByRule").show().removeClass("HideStepByRule");
            $(WizardObj.steps).find(".HideStepByRule").show().removeClass("HideStepByRule");
            return;
        }              
}
function PopulateBankName() {
    if ($("#" + ns.viewModel.currentModel).find('#lblRoutingNumberExists').text() == "True" || $("#" + ns.viewModel.currentModel).find('#lblnRNExists').text() == "True") {
        $("#" + ns.viewModel.currentModel).find('#lblBankName').show();
        $("#" + ns.viewModel.currentModel).find('#txtBankName').hide();
        $("#" + ns.viewModel.currentModel).find('#lblRoutingNumberExists').hide();
        $("#" + ns.viewModel.currentModel).find('#lblnRNExists').hide();
    }
    else {
        $("#" + ns.viewModel.currentModel).find('#lblBankName').hide();
        $("#" + ns.viewModel.currentModel).find('#lblBankName').val("");
        $("#" + ns.viewModel.currentModel).find('#txtBankName').show();
        $("#" + ns.viewModel.currentModel).find('#lblRoutingNumberExists').hide();
        $("#" + ns.viewModel.currentModel).find('#lblnRNExists').hide();
    }
}
function PopulateBankNameIfRoutingNoExists1() {
    if ($("#" + ns.viewModel.currentModel).find('#lblRoutingNumberExists1').text() == "True" || $("#" + ns.viewModel.currentModel).find('#lblnRNExists1').text() == "True") {
        $("#" + ns.viewModel.currentModel).find('#lblBankName1').show();
        $("#" + ns.viewModel.currentModel).find('#txtBankName1').hide();
        $("#" + ns.viewModel.currentModel).find('#lblRoutingNumberExists1').hide();
        $("#" + ns.viewModel.currentModel).find('#lblnRNExists1').hide();
    }
    else {
        $("#" + ns.viewModel.currentModel).find('#lblBankName1').hide();
        $("#" + ns.viewModel.currentModel).find('#lblBankName1').val("");
        $("#" + ns.viewModel.currentModel).find('#txtBankName1').show();
        $("#" + ns.viewModel.currentModel).find('#lblRoutingNumberExists1').hide();
        $("#" + ns.viewModel.currentModel).find('#lblnRNExists1').hide();
    }
}
function PopulateBankNameIfRoutingNoExists2() {
    if ($("#" + ns.viewModel.currentModel).find('#lblRoutingNumberExists2').text() == "True" || $("#" + ns.viewModel.currentModel).find('#lblnRNExists2').text() == "True") {
        $("#" + ns.viewModel.currentModel).find('#lblBankName2').show();
        $("#" + ns.viewModel.currentModel).find('#txtBankName2').hide();
        $("#" + ns.viewModel.currentModel).find('#lblRoutingNumberExists2').hide();
        $("#" + ns.viewModel.currentModel).find('#lblnRNExists2').hide();
    }
    else {
        $("#" + ns.viewModel.currentModel).find('#lblBankName2').hide();
        $("#" + ns.viewModel.currentModel).find('#lblBankName2').val("");
        $("#" + ns.viewModel.currentModel).find('#txtBankName2').show();
        $("#" + ns.viewModel.currentModel).find('#lblRoutingNumberExists2').hide();
        $("#" + ns.viewModel.currentModel).find('#lblnRNExists2').hide();
    }
}

function ChangeLabelVisibility() {
    if ($('#ddlDefCompCascadingDropDownList').val() == "" || $('#ddlDefCompCascadingDropDownList').val() == null) {
        $('#lblLiteral').hide();
    }
    else {
        if ($('#ddlIstrOrgCodeID1').val() != "700008")
            $('#lblLiteral').show();
    }
    
    //if ($("#ddlIstrOrgCodeID1 option:selected").text() == $('#lblApplyEmployerMatchingContributionOnCompany').text() || $('#lblApplyEmployerMatchingContributionOnCompany').text() == "") {
    //    $('#capIsAppyEmployerMathchingContribution').show();
    //    $('#chkIsAppyEmployerMathchingContribution').show();
    //}
    //else {
    //    $('#capIsAppyEmployerMathchingContribution').hide();
    //    $('#chkIsAppyEmployerMathchingContribution').hide();
    //}
}

//FW Upgrade :: Hide percentage textbox if amount is non-zero (PIR - 11159)
function CheckIfPartialAmountOfPrimaryEntered() {
    if ($("#" + ns.viewModel.currentModel).find('#lblPartialAmount1').text() == "True") {
        $("#" + ns.viewModel.currentModel).find('#txtPercentage2').hide();
        $("#" + ns.viewModel.currentModel).find('#lblPartialAmount1').hide();
    }
    else {
        $("#" + ns.viewModel.currentModel).find('#txtPercentage2').show();
        $("#" + ns.viewModel.currentModel).find('#lblPartialAmount1').hide();
    }
}

//FW upgrade :: Override F/w code to scroll up the page while opening 
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
                    return el.classList.contains("s-grid");
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
            return el.classList.contains("s-grid");
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
    
    {
        var fnSetTimeout = function () {
            //$(nsConstants.SCROLL_DIV).scrollTop(0);
            //$(".page-container").scrollTop(0);
        };
        setTimeout(fnSetTimeout, 250);
        $(".page-container").scrollTop(0);
    }
    ns.blnLoading = false;
}

var Basebtn_GoPreviousPage = nsEvents.btn_GoPreviousPage;
nsEvents.btn_GoPreviousPage = function(e) {
    var reportpage = window.location.href.indexOf(nsConstants.RPT_FROMNAME_WFMREPORTCLIENT) > 0;
    var tempFormName = "";
    if ((ns.viewModel.currentModel.indexOf("Lookup") < 0 && !reportpage) || (ns.viewModel.currentModel.indexOf("Maintenance") < 0 && !reportpage)) {
        if (ns.DirtyData[ns.viewModel.currentModel] == undefined) {
            tempFormName = ns.viewModel.currentModel;
        }
    }
    if (ns.viewModel.currentModel.indexOf("wfmMSSDeferredCompEnrolledMaintenance") >= 0) {
        $('ul#MenuUl li[formid="wfmActiveMemberBenefitPlansMaintenance"]').trigger("click");
    }
    else { 
        var cofirm = dataLossPopUpMessage();
        if (cofirm) {
            Basebtn_GoPreviousPage(e);
        }
    }
    if (tempFormName != "") {
        setTimeout(function () {
            var dataitem = nsCommon.GetDataItemFromDivID(tempFormName);
            if (MVVMGlobal.CanBeDeleted(dataitem)) {
                nsEvents.OnDeleteFormClick(tempFormName, false);
            }
        }, 200);
    }
}
//PIR-17683 Regular Health wizard landing page in MSS
function ChangeControlVisibility() {
    if ($('#ddlPlanEnrollmentOptionValue').val() == "CANL" && $('#lblIsUserLinkedInSTATDHSU').text() == "false" && $('#lblIsHealthPlan').text() == "true") {
        $('#btnBeginClick').hide();
        $('#lblL').hide();
        $('#lblIsHealthPlan').hide();
        $('#lblCancelMsgForNotInHDHP').show();
        $('#btnContinue').show();
        $('#btnDoNotContinue').show();
    }
    else {
        $('#btnBeginClick').show();
        $('#lblL').show();
        $('#lblCancelMsgForNotInHDHP').hide();
        $('#btnContinue').hide();
        $('#btnDoNotContinue').hide();
        $('#lblIsHealthPlan').hide();
    }
}

//PIR 18493 - Ben options needed to be retrieved based on the retirement date 
//entered by the user as an effective date.
function GetBenOptionsByEffectiveDate(ldomDiv) {
    var astrBenType = ldomDiv.find("#ddlSelBenType")[0].value;
    var astrRetDate = ldomDiv.find("#txtRetEffecDate").val();
    var aintBenProvisionId = ldomDiv.find("#lblBenProvId").text();
    var astrMaritalStatusValue = ldomDiv.find("#lblMaritalStatusValue").text();
    var bendropdown = ldomDiv.find('#ddlSelBenOption');
    var month = 0;
    var year = 0;
    
    var Parameters = {};
    var data = {};
    Parameters["astrBenType"] = astrBenType;
    Parameters["astrRetDate"] = astrRetDate;
    Parameters["aintBenProvisionId"] = aintBenProvisionId;
    Parameters["astrMaritalStatusValue"] = astrMaritalStatusValue;

    if (astrRetDate != null && astrRetDate != undefined && astrRetDate != ""){
        var spitdate = astrRetDate.split('/');
        if (spitdate.length > 0) {
            month = parseInt(spitdate[0]);
            year = parseInt(spitdate[1]);
        }
    }
    
    if (month>0 && year > 0 && year.toString().length == 4 ) {
        if (month > 0 && month <= 12 && year >= 1753 && year <= 9999) {
            data = nsCommon.SyncPost("GetBenOptionsByEffectiveDate1", Parameters, "POST");
            if (data != undefined) {
                var jsonObject = JSON.parse(data);
                bendropdown.empty();
                if (jsonObject.length > 0) {
                    bendropdown.append('<option value=""></option>');
                    $.each(jsonObject, function (index, value) {
                        bendropdown.append('<option value="' + value.code_value + '">' + value.description + '</option>');
                    });
                    bendropdown.show();
                    bendropdown.focus();
                    $('#lblBenOption').show();
                    $('#btnMainAndPublicSafetyRetirementOptionsPdf').show();
                }
            }
        }
        else {
            alert("Invalid Date");
            bendropdown.empty();
            ldomDiv.find("#txtRetEffecDate").val('');
            ldomDiv.find("#txtRetEffecDate").trigger("focus");
	    ldomDiv.find("#txtRetEffecDate").focus();
            bendropdown.hide();            
            $('#lblBenOption').hide();
            $('#btnMainAndPublicSafetyRetirementOptionsPdf').hide();
        }
    }
    else {
        bendropdown.empty();
        bendropdown.hide();        
        $('#lblBenOption').hide();
        $('#btnMainAndPublicSafetyRetirementOptionsPdf').hide();
    }
}
//function SwitchPercent(ldomDiv) {
//    var astrPartialAmount = ldomDiv.find("#txtPartialAmount").val();
    
//    if (astrPartialAmount != null && astrPartialAmount != undefined && astrPartialAmount != "") {
//        if (astrPartialAmount != "$0.00")
//        {
//            ldomDiv.find("#txtPercentageOfNetAmount").val("0.00%");
//        }
//    }
//}
//function SwitchAmount(ldomDiv) {
//    var astrPercentage = ldomDiv.find("#txtPercentageOfNetAmount").val();
    
//    if (astrPercentage != null && astrPercentage != undefined && astrPercentage != "") {
//        if (astrPercentage != "0.00%") {
//            ldomDiv.find("#txtPercentageOfNetAmount1").show();
//            //ldomDiv.find("#txtPartialAmount").val("$0.00");
//        }
//    }
//}
//PIR 24240 - Get Benefit Account type value on plan selection
function GetBenefitAccountTypeByPlan(ldomDiv) {
    var aintPlanId = ldomDiv.find('#ddlPlanId')[0].value;
    var bendropdown = ldomDiv.find('#ddlBenefitAccountTypeValueonPlan');
    
    var Parameters = {};
    var data = {};
    Parameters["aintPlanId"] = aintPlanId;

    if (aintPlanId > 0) {
        data = nsCommon.SyncPost("GetBenefitAccountTypeByPlan", Parameters, "POST");
            if (data != undefined) {
                var jsonObject = JSON.parse(data);
                bendropdown.empty();
                if (jsonObject.length > 0) {
                    bendropdown.append('<option value=""></option>');
                    $.each(jsonObject, function (index, value) {
                        bendropdown.append('<option value="' + value.code_value + '">' + value.description + '</option>');
                    });
                    bendropdown.show();
                    ldomDiv.find("#ddlBenefitAccountTypeValueNONDC").hide();
                    bendropdown.focus();
                }
        }
    }
    else {
        bendropdown.empty();
    }
}
function setOTPPanelVisibility(visibilityOfOTPPanel) {
    var ldomDiv = $("#" + ns.viewModel.currentModel);
    var txtBenAppActCode = ldomDiv.find("#txtBenAppActCode").val().trim();
    var BenTypeValue = ldomDiv.find("#lblSelBenType1Summary").text();
    if (visibilityOfOTPPanel) {
        if (BenTypeValue == "Refund/Rollover") {
            ldomDiv.find("#pnlOTPVerify").show();
            ldomDiv.find("#pnltblsumsigInfo").hide();
            $("div[class='actionBar'] a[class='buttonPrevious wizard-hideonvisblerule buttonDisabled']").hide();
            $(".actionBar").hide();
            ldomDiv.find("#btnSaveAndNextInOTPPanel").show();
        }
    }
    else {
        ldomDiv.find("#pnlOTPVerify").hide();
        ldomDiv.find("#pnltblsumsigInfo").show();
        //ldomDiv.find("#btnPreviousSummary").show();
        $("div[class='actionBar'] a[class='buttonPrevious wizard-hideonvisblerule buttonDisabled']").show();
        $(".actionBar").show();
        ldomDiv.find("#btnSaveAndNextInOTPPanel").hide();
    }
    
    return true;
}
function setBenAppWizardVisibility() {
    var planEnroll = $('.PlanEnroll');
    if (planEnroll.length > 0) {
        //var othercoverage = $('.OtherCoverage');
        //var othercovpanel = $('.OtherCovPanel');
        //var workcomp = $('.WorkComp');
        //var nofault = $('.NoFault');
        var workcomppanel = $('.WorkCompPanel');
        var medicare = $('.Medicare');
        var esrd = $('.ESRD');
        var meddepspanel = $('.MedDepsPanel');
        var medicarePanel = $('.MedicarePanel');
        if (planEnroll.val() == "ENRL") {
            //if (othercoverage.val() == "Y") {
            //    othercovpanel.show();
            //}
            //else {
            //    othercovpanel.hide();
            //}
            //setWorkCompPanelVisibility(workcomp, nofault, workcomppanel);
            setMedicarePanelVisibility(medicare, esrd, meddepspanel, medicarePanel);
        }
        else {
            //othercovpanel.hide();
            //workcomppanel.hide();
            meddepspanel.hide();
            medicarePanel.hide();
        }
    }
}

function setWorkCompPanelVisibility(workcomp, nofault, workcomppanel) {
    if (workcomp.val() == "Y" || nofault.val() == "Y") {
        workcomppanel.show();
    }
    else {
        workcomppanel.hide();
    }
}
function setMedicarePanelVisibility(medicare, esrd, meddepspanel, medicarePanel) {
    var intMemberAge = $("#capMemberAge").text();    
    if (medicare.val() == "Y" || esrd.val() == "Y" || intMemberAge > 65) {        
        meddepspanel.show();
        medicarePanel.show();
    }
    else {
        meddepspanel.hide();
        medicarePanel.hide();
    }
}

function SetDeferralDateVisibility(ldomDiv) {

    var DefDropDown = ldomDiv.find('#ddlDefRetDropDown');
    if (DefDropDown.length > 0) {
        SetDefDateVisibility();
        DefDropDown.change(function () {
            SetDefDateVisibility();
        });
    }
}
function SetDefDateVisibility(ldomDiv) {
    if (ldomDiv.find('ddlDefRetDropDown').val() == "OTHR") {
        ldomDiv.find('txtDeferalDate').show();
    }
    else {
        ldomDiv.find('txtDeferalDate').hide();
    }
}
    $(document).on("change", "#ddlDefRetDropDown", function (e) {
        var ldomDiv = $("#" + ns.viewModel.currentModel);
        if (ldomDiv.find('#ddlDefRetDropDown').val() == "OTHR") {
            ldomDiv.find('#txtDeferalDateId').show();
        }
        else {
            ldomDiv.find('#txtDeferalDateId').hide();
        }
    });





function GetBankNameRetrivalMethod(section,ldomDiv) {
    var lblBNAppWiz;
    var txtBNAppWiz;
    if (section == 1) {
        lblBNAppWiz = ldomDiv.find("#lblBNAppWizDisOne");
        txtBNAppWiz = ldomDiv.find("#txtBNAppWizDisOne");
    }
    else if (section == 2) {
        lblBNAppWiz = ldomDiv.find("#lblBNAppWizDisTwo");
        txtBNAppWiz = ldomDiv.find("#txtBNAppWizDisTwo");
    }
    else if (section == 3) {
        lblBNAppWiz = ldomDiv.find("#lblBankNameInsPay");
        txtBNAppWiz = ldomDiv.find("#txtBankNameInsPay");
    }

    if (lblBNAppWiz.text() != "") {
        txtBNAppWiz.hide();
    }
}
function setInitialBankNameVisibility(ldomDiv) {
    setBenAppDisBankNamesVis(ldomDiv.find('#lblBNAppWizDisOne'));
    setBenAppDisBankNamesVis(ldomDiv.find('#lblBNAppWizDisTwo'));
    setBenAppDisBankNamesVis(ldomDiv.find('#lblBankNameInsPay'));
}
function setBenAppDisBankNamesVis(lblBankName) {
    if (lblBankName.length > 0) {
        lblBankName.show();
    }
}

//PIR 18493
function SetVisibilityPostRetreivalOne() {
    
    setBankNameVisibility($('#lblBNAppWizDisOne'), $('#txtBNAppWizDisOne'), $('#txtACHAccountNumber'));
}
function SetVisibilityPostRetreivalTwo() {
    setBankNameVisibility($('#lblBNAppWizDisTwo'), $('#txtBNAppWizDisTwo'), $('#txtTextBox13'));
}
function SetVisibilityPostRetreivalInsPremium() {
    setBankNameVisibility($('#lblBankNameInsPay'), $('#txtBankNameInsPay'), $('#txtActNoInsPay'));
}
function setBankNameVisibility(lblBankName, txtBankName, txtAccNumber) {
    if (lblBankName.text().length > 0) {
        if (txtBankName.length > 0) {
            txtBankName.hide();
        }
        txtAccNumber.focus();
        lblBankName.show();
    }
    else {
        if (txtBankName.length > 0) {
            txtBankName.show();
            txtBankName.focus();
        }
        lblBankName.hide();
    }
}
//End of PIR 18493

var base_SPARouteGet = MVVMGlobal.SPARouteGet;
MVVMGlobal.SPARouteGet = function (event, urlParams) {
    if (sessionStorage.getItem("IsBrowserBackButtonPressed") == "True") {
        sessionStorage.setItem("IsBrowserBackButtonPressed", "False");
        location.replace(ns.SiteName + "/Account/wfmLoginMI");        
    }
    else {
        base_SPARouteGet(event, urlParams);
    }
}

//Left menu not to be collapsible
MVVMGlobal.toggleMenuHeader = function (adomSpan) {
    return false;
}; 

function GetMessageCount() {    
    var ldomDiv = $("#" + ns.viewModel.currentModel);
    var aintPersonID = ldomDiv.find("#PersonID").text();
    var Parameters = {};
    Parameters["aintPersonID"] = aintPersonID;
    var AlertMessages = {};
    AlertMessages = nsCommon.SyncPost("GetMSSUnreadMessagesCountNeo", Parameters, "POST");
    var alertMsgCount = parseInt(AlertMessages);
    if (alertMsgCount > 0) {
        $("#imgNewMsg").show();
        $("a[id='HLMSSMessageBoard']").text("You have " + alertMsgCount + " messages");
    }
    else {
        $("#imgNewMsg").hide();
    }
}

function dataLossPopUpMessage() {
    var ObjFormChanged = $(ns.NotificationModel.DirtyForms.DirtyFormList).find("a[divid]").attr("divid");
    if (ObjFormChanged == null || ObjFormChanged == undefined)
        return true;
    var activeWizardStepText = $('li.ProgressHighlight span').text();
    if (ObjFormChanged != undefined && ObjFormChanged.contains('Wizard') && (!(activeWizardStepText.length > 0 && /print/i.test(activeWizardStepText))) && ns.viewModel.currentForm.indexOf("wfmBenefitEstimateMaintenance") != 0) {
        var result;
        if (ns.viewModel.currentForm.indexOf("wfmMSSBenefitApplicationWizard") == 0)
            result = confirm("Your application will be saved for 30 days for you to make changes prior to being deleted.");
        else
            result = confirm("Are you sure you want to navigate away from this page? All information will be lost unless it is submitted.");
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
function ToggelHelpPopup(btnHelpID, helpLblMessageID) {
    var ldomDiv = $("#" + ns.viewModel.currentModel);
    var elementID = ldomDiv.find("#" + btnHelpID);
    if (elementID.text() == "Learn More") {
        PopupHelp(ldomDiv.find("#" + helpLblMessageID).html(), btnHelpID);
        elementID.text("Hide");
    }
    else {
        $(".ui-dialog").hide();
        elementID.text("Learn More");
    }
    return false;
}
function PopupHelp(message, nextElement) {
    var ldomDiv = $("#" + ns.viewModel.currentModel);
    $("<div id='popUpDiv'>" + message + "</div>").dialog({
        resizable: false,
        height: "auto",
        width: 450,
        position: {
            my: 'left',
            at: 'right',
            of: ldomDiv.find("#" + nextElement)
        }
    });
    $(".ui-dialog-titlebar-close").text('');
}
function CustomPopupAlert(message) {

    $("<div id='popUpDiv'>" + message + "</div>").dialog({
        title: 'Confirm',
        show: "slide",
        modal: true,
        resizable: false,
        height: "auto",
        width: 450,
        buttons: {
            Ok: function () { $(this).dialog("close"); }
        },
        create:function () {
        $(this).closest(".ui-dialog")
            .find(".ui-button") // the first button
            .addClass("button");
        }
    });
    $(".ui-dialog-titlebar-close").text('').removeClass("button");
    $(".ui-dialog-titlebar").attr("style", "background:#556080;color:White;");
}
//Code added for collapse handburg menu for window resize.
window.addEventListener('resize', function (event) {
    var width = $(window).width();
    if ($(".page-header-fixed").hasClass('page-slideout-body-collaped')) {
        if (width >= 1025) {
            $(".web-view-head").css("left", "-262px");
            $(".crumDiv").css("margin-left", "0px");
        } else{
            $(".web-view-head").css("left", "10px");
            $(".crumDiv").css("margin-left", "30px");
        }
    }
    else if ($(".page-header-fixed").hasClass('page-slideout-body-fixed')) {
        $(".web-view-head").css("left", "10px");
        $(".crumDiv").css("margin-left", "30px");
    }
});

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
nsNeoControl.sfwAutoComplete.prototype.onResponse = function (item, astrRequestTerm) {
    var instance = this;
    var jsonobj = new Object();
    if (instance.istrSelfMappingSource == "")
        jsonobj["value"] = item[instance.iarrTotalFields[0]];
    else
        jsonobj["value"] = item[instance.istrSelfMappingSource];
    var arrTotalFields = instance.iarrTotalFields;
    var arrfields = instance.iarrFields;
    $('.divFilterBoxButtonFieldsContainer').css('overflow-y', 'scroll');
    for (var i = 0; i < arrTotalFields.length; i++) {
        if (item["rowindex"] != undefined) {
            jsonobj["value"] = astrRequestTerm;
            jsonobj["rowindex"] = item["rowindex"];
            if (i <= arrfields.length)
                jsonobj[arrTotalFields[i]] = item[arrTotalFields[i]];
        }
        else {
            //if (item[arrTotalFields[i]].indexOf("/") > 0) {
            //    jsonobj[arrTotalFields[i]] = MVVMGlobal.GetFormatedDate(item[arrTotalFields[i]]);
            //    if (jsonobj[arrTotalFields[i]] == "NaN/NaN/NaN") {
            //        jsonobj[arrTotalFields[i]] = item[arrTotalFields[i]];
            //    }
            //}
            //else
                jsonobj[arrTotalFields[i]] = item[arrTotalFields[i]];
        }
    }
    return jsonobj;
}