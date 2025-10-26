app.controller("FormLinkController", ["$scope", "$http", "$rootScope", "share", "$EntityIntellisenseFactory", "$sce", "hubcontext", "$filter", "$NavigateToFileService", "ConfigurationFactory", "$getQueryparam", "$timeout", "$ValidationService", "CONSTANTS", function ($scope, $http, $rootScope, share, $EntityIntellisenseFactory, $sce, hubcontext, $filter, $NavigateToFileService, ConfigurationFactory, $getQueryparam, $timeout, $ValidationService, CONST) {
    $scope.sharedata = share;
    $scope.isDirty = false;
    $scope.isFormLink = true;
    $rootScope.IsLoading = true;
    $scope.LstDisplayedEntities = [];
    $scope.showPortalOptions = false;
    $scope.currentfile = $rootScope.currentopenfile.file;
    $scope.lstPortals = [{ PortalName: "" }];
    $scope.validationErrorList = [];
    //#region Populate htx model, load HTML and select the Control

    hubMain.server.getModel($scope.currentfile.FilePath, $scope.currentfile.FileType).done(function (data) {
        if (data) {
            $scope.receiveformlinkmodel(data);
        }
        else {
            $rootScope.closeFile($scope.currentfile.FileName);
        }

    });

    $scope.receiveformlinkmodel = function (data, isRefresh) {

        $scope.$evalAsync(function () {
            if (data) {
                $scope.lstLoadedEntityTrees = [];
                $scope.lstLoadedEntityColumnsTree = [];
                $scope.FormLinkModel = data;

                $scope.templateFile = $scope.FormLinkModel.dictAttributes.sfwTemplate;
                var lstInitialLoad = $scope.FormLinkModel.Elements.filter(function (x) {
                    return x.Name == "initialload";
                });
                if (lstInitialLoad && lstInitialLoad.length > 0) {
                    $scope.InitialLoad = lstInitialLoad[0];
                }
                if (!$scope.InitialLoad) {
                    $scope.InitialLoad = {
                        Name: 'initialload', Value: '', dictAttributes: {}, Elements: [], Children: []
                    };
                    $scope.FormLinkModel.Elements.splice(0, 0, $scope.InitialLoad);
                }
                $scope.itemsModel = $scope.FormLinkModel.Elements.filter(function (x) {
                    return x.Name == "items";
                });
                if ($scope.itemsModel[0] && $scope.itemsModel[0].Elements && $scope.itemsModel[0].Elements[0]) {
                    $scope.selectedHtxControl = $scope.itemsModel[0].Elements[0];
                    $scope.selectedControlId = $scope.selectedHtxControl.dictAttributes.ID ? $scope.selectedHtxControl.dictAttributes.ID : null;
                    if (isRefresh) {
                        $scope.selectElement();
                    }
                }

                if ($scope.FormLinkModel.dictAttributes.sfwType == "FormLinkLookup") {
                    $scope.PopulateQueryId(true);
                }

                var objnew = { EntityName: $scope.FormLinkModel.dictAttributes.sfwEntity, IsVisible: false, selectedobjecttreefield: undefined, lstselectedobjecttreefields: [], IsQuery: false };

                $scope.lstLoadedEntityTrees.push(objnew);

                $scope.FormLinkModel.IsCommonPropertiesOpen = true;
                if ($scope.FormLinkModel.dictAttributes.sfwRemoteObject && $scope.FormLinkModel.dictAttributes.sfwRemoteObject != "") {
                    $scope.FormLinkModel.isSrvMethodChecked = "true";
                }
                //$scope.FormLinkModel.RemoteObjectCollection = [];
                $.connection.hubForm.server.getRemoteObjectList().done(function (data) {
                    $scope.$evalAsync(function () {
                        $scope.FormLinkModel.RemoteObjectCollection = data;
                        if ($scope.FormLinkModel.RemoteObjectCollection && $scope.FormLinkModel.RemoteObjectCollection.length > 0) {
                            $scope.FormLinkModel.RemoteObjectCollection.splice(0, 0, { dictAttributes: { ID: "" } });
                        }
                    });
                });

                SetFormLinkSelectedControl();

                $scope.sharedata.FormLinkModel = $scope.FormLinkModel;
                $scope.setParentVM($scope.FormLinkModel);
                $scope.getTemplateUrl(isRefresh);
                // Get Extra field
                $scope.objFormHtmlExtraFields = [];
                $scope.objFormHtmlExtraFields = $filter('filter')($scope.FormLinkModel.Elements, {
                    Name: 'ExtraFields'
                });
                if ($scope.objFormHtmlExtraFields.length > 0) {
                    $scope.objFormHtmlExtraFields = $scope.objFormHtmlExtraFields[0];
                    //$scope.removeExtraFieldsDataInToMainModel();
                }

                if ($scope.objFormHtmlExtraFields.length == 0) {
                    $scope.objFormHtmlExtraFields = {
                        Name: "ExtraFields", Value: '', dictAttributes: {}, Elements: []
                    };
                }

                $scope.InitialLoadSection();
                $scope.ValidateNew = $filter('filter')($scope.FormLinkModel.Elements, {
                    Name: 'validatenew'
                })[0];
            } else {
                $rootScope.closeFile($scope.currentfile.FileName);
            }

        });
        //below logic is to get HtmlTemplatesLocation
    };

    $scope.PopulateQueryId = function (isVisible) {
        $scope.lstQueryID = [];
        var initialload = $scope.FormLinkModel.Elements.filter(function (x) { return x.Name == 'initialload'; });

        if (initialload.length > 0) {
            for (i = 0; i < initialload[0].Elements.length; i++) {
                $scope.lstQueryID.push(initialload[0].Elements[i]);
            }
        }

        if ($scope.lstQueryID.length > 0) {
            $scope.SelectedQuery = $scope.lstQueryID[0];
            $scope.populateQueryFields($scope.SelectedQuery, isVisible);
        }
    };

    $scope.populateQueryFields = function (query, isVisible) {

        if (query) {
            var objnew;
            for (var i = 0; i < $scope.lstLoadedEntityColumnsTree.length; i++) {
                if ($scope.lstLoadedEntityColumnsTree[i].EntityName == query.dictAttributes.ID && $scope.lstLoadedEntityColumnsTree[i].IsQuery == true) {
                    $scope.lstLoadedEntityColumnsTree[i].IsVisible = true;
                    $scope.lstLoadedEntityColumnsTree[i].lstselectedobjecttreefields = [];
                    //$scope.entityTreeName = $scope.lstLoadedEntityColumnsTree[i].EntityName;
                    $scope.lookupTreeObject = $scope.lstLoadedEntityColumnsTree[i];
                    objnew = $scope.lstLoadedEntityColumnsTree[i];
                }
                else {
                    //if ($scope.FormModel.IsLookupCriteriaEnabled) {
                    $scope.lstLoadedEntityColumnsTree[i].IsVisible = false;
                    //}
                }
            }
            if (!objnew) {
                objnew = { EntityName: query.dictAttributes.ID, IsVisible: isVisible, selectedobjecttreefield: undefined, lstselectedobjecttreefields: [], IsQuery: true, QueryRef: query.dictAttributes.sfwQueryRef, SortedColumns: [] };
                $scope.lstLoadedEntityColumnsTree.push(objnew);
                $scope.lookupTreeObject = objnew;
                // $scope.entityTreeName = query.dictAttributes.ID;
            }
            objnew.SortedColumns = objnew.lstselectedobjecttreefields;
            $scope.ColumnsLimitCount = 50;

            if (query.dictAttributes.sfwQueryRef != undefined && query.dictAttributes.sfwQueryRef != "") {
                $.connection.hubForm.server.getEntityQueryColumns(query.dictAttributes.sfwQueryRef, 'LoadQueryFieldsForLookup').done(function (data) {
                    $scope.receiveQueryFields(data, query.dictAttributes.sfwQueryRef);
                });
            }
        }
    };

    $scope.receiveQueryFields = function (lstqueryfields, sfwQueryRef) {
        $scope.$evalAsync(function () {
            function iAddselectedobjecttreefields(query) {
                var datatype = "";
                if (query.Data1 && query.Data1 != null) {
                    datatype = query.Data1.toLowerCase();
                }
                else if (query.DataType) {
                    datatype = query.DataType.toLowerCase();
                }
                var newquery = {
                    ID: query.CodeID, DisplayName: query.CodeID, Value: query.CodeID, DataType: datatype, Entity: "", Direction: "", IsPrivate: "", Type: "", KeyNo: "", CodeID: ""
                };
                objnew.lstselectedobjecttreefields.push(newquery);

            }
            var lst = $scope.lstQueryID.filter(function (itm) {
                return itm.dictAttributes.sfwQueryRef == sfwQueryRef;
            });

            if (lst && lst.length > 0) {
                var lst1 = $scope.lstLoadedEntityColumnsTree.filter(function (itm) {
                    return itm.EntityName == lst[0].dictAttributes.ID;
                });
                if (lst1 && lst1.length > 0) {
                    var objnew = lst1[0];
                    $scope.lookupTreeObject = objnew;
                    if (lstqueryfields) {
                        sortListBasedOnproperty(lstqueryfields, "", "CodeID");

                        angular.forEach(lstqueryfields, iAddselectedobjecttreefields);
                    }
                }
            }
        });
    };

    $scope.receiveRemoteObjectCollection = function (data) {

        $scope.$evalAsync(function () {
            $scope.FormLinkModel.RemoteObjectCollection = data;
            $scope.FormLinkModel.RemoteObjectCollection.splice(0, 0, {
                dictAttributes: { ID: "" }
            });
            $scope.onRemoteObjectChange();
            if ($scope.FormLinkModel.dictAttributes.sfwType == "FormLinkWizard" || $scope.FormLinkModel.dictAttributes.sfwType == "FormLinkMaintenance") {
                if ($scope.InitialLoad.Elements.length > 0 && $scope.InitialLoad.Elements[0]) {
                    var newMethodName = $scope.InitialLoad.Elements[0].dictAttributes.sfwMethodName;
                    $scope.populateSrvParamtersForNew(newMethodName);
                }
                if ($scope.InitialLoad.Elements.length > 1 && $scope.InitialLoad.Elements[1]) {
                    var updateMethodName = $scope.InitialLoad.Elements[1].dictAttributes.sfwMethodName;
                    $scope.populateSrvParamtersForUpdate(updateMethodName);
                }
            }
        });
    };

    $scope.setParentVM = function (parent) {
        if (parent) {
            angular.forEach(parent.Elements, function (currentelement) {
                currentelement.ParentVM = [];
                currentelement.ParentVM = parent;
                $scope.setParentVM(currentelement);
            });
        }
    };


    // $scope.htmlTemplateFile = "HtmlTemplate.html";

    $scope.getTemplateUrl = function (ablnChangePortal) {
        $rootScope.IsLoading = true;

        $.connection.hubFormLink.server.getHtmlTemplateFilePath().done(function (data) {
            var $iframe = $("#" + $scope.currentfile.FileName).find("#iframe" + $scope.FormLinkModel.dictAttributes.ID);
          //  $iframe[0].contentWindow.location.reload(true);
            var iframeDoc = $iframe[0].contentDocument || $iframe[0].contentWindow.document;
            iframeDoc.documentElement.innerHTML = "";
            $scope.$evalAsync(function () {
                $scope.HtmlTemplatePath = data;
                if ($scope.HtmlTemplatePath != undefined && $scope.HtmlTemplatePath != "") {
                    var filePath = $scope.HtmlTemplatePath + "\\" + $scope.templateFile;
                    var convertedFilePath = filePath;
                    filePath = filePath.replace(/\\/g, "//");
                    $scope.htmlfilePath = "file:///" + filePath;

                    $.connection.hubFormLink.server.getHtmlfileContents(convertedFilePath).done(function (data) {
                        if (data == "file not found") {
                            alert("Html File Path is incorrect.");
                            $scope.$evalAsync(function () {
                                $rootScope.IsLoading = false;
                            });
                            return;
                        }
                        $scope.$evalAsync(function () {
                            //$scope.htmlContent = data;
                            //var div = document.getElementById("htmlDiv");
                            //div.innerHTML = $scope.htmlContent;

                            $iframe.contents().find("body").html("");

                            var $head = $iframe.contents().find("head");
                            $head.find('style').remove();
                            $head.find('link').remove();
                            $head.find('script').remove();
                            var projectDetails = ConfigurationFactory.getLastProjectDetails();



                            $head.append($("<link/>",
                                { rel: "stylesheet", href: "css/htx_form.css", type: "text/css" }));

                            var script = document.createElement('script');
                            script.type = 'text/javascript';
                            script.src = 'scripts/jquery.smartWizard.js';
                            $head.append(script);

                            var script1 = document.createElement('script');
                            script1.type = 'text/javascript';
                            //script1.src = 'scripts/kendo.all.min.js';
                            script1.src = 'scripts/kendo.ui.core.min.js';
                            $head.append(script1);

                            var script2 = document.createElement('script');
                            script2.type = 'text/javascript';
                            script2.src = 'scripts/initializeStudio.js';
                            $head.append(script2);

                            $iframe.ready(function () {
                                if ($scope.selectedPortalDetails && $scope.selectedPortalDetails.PortalName && angular.isArray(projectDetails.htxPortals) && projectDetails.htxPortals.length > 0 && ablnChangePortal == true) {
                                    $scope.LoadNewStyle(data);
                                }
                                else if (!ablnChangePortal && angular.isArray(projectDetails.htxPortals) && projectDetails.htxPortals.length > 0) {
                                    $scope.showPortalOptions = true;
                                    if ($scope.lstPortals.length == 1) {
                                        $scope.lstPortals = $scope.lstPortals.concat(projectDetails.htxPortals);
                                    }
                                    if ($scope.lstPortals) {
                                        $scope.selectedPortalDetails = $scope.lstPortals.filter(function (p) { if (p && p.IsDefault && p.IsDefault.toLowerCase() == "true") { return p; } })[0];
                                        $scope.LoadNewStyle(data);
                                    }
                                    if (!$scope.selectedPortalDetails) {
                                        $scope.selectedPortalDetails = $scope.lstPortals[0];
                                    }
                                }
                                else {
                                    $scope.bindIframeEvents(data);
                                } 
                            });
                            $rootScope.IsLoading = false;
                            $scope.selectElement();
                        });

                    });
                }
            });
        }
        );
    };

    $scope.bindIframeEvents = function (data) {
        var $iframe = $("#iframe" + $scope.FormLinkModel.dictAttributes.ID);
        var elementScope = angular.element(document.getElementById("htmlDiv" + $scope.FormLinkModel.dictAttributes.ID)).scope();
        var $iframeBody = $iframe.contents().find("body");
        var lHtmlElement = document.createElement("div");
        lHtmlElement.innerHTML = data;
        var lstScript = lHtmlElement.querySelectorAll("script");
        if (lstScript.length > 0) {
            for (var iScript = 0; iScript < lstScript.length; iScript++) {
                lHtmlElement.removeChild(lstScript[iScript]);
            }
        }
        $iframeBody.append(lHtmlElement.innerHTML);
        $iframeBody.find('[onclick]').removeAttr('onclick');

        $iframeBody.unbind("click", fnBodyClick);
        $iframeBody.unbind('dblclick', fnBodyDbClick);
        $iframeBody.unbind("keydown", fnKeyDown);
        $iframeBody.find("input").each(function () {
            $(this).unbind("click", fnInputClick);
        });

        $iframeBody.find("input").each(fnEachInputElement);
        $iframeBody.on('click', fnBodyClick);
        $iframeBody.on('dblclick', fnBodyDbClick);

        $scope.controlList = [""];
        var list = [];

        //input,button,select,a
        $iframeBody.find('[id]').each(function () {
            if ($(this).attr("id") && !$(this).attr("id").startsWith("ListViewItem_")) {
                list.push($(this).attr("id"));
                // console.log($(this));
            }
            $.each(list, function (i, e) {
                if ($.inArray(e, $scope.controlList) == -1) $scope.controlList.push(e);
            });
        });

        $iframeBody.on("keydown", fnKeyDown);

        function fnKeyDown(e) {
            if (e && e.shiftKey && e.keyCode == $.ui.keyCode.TAB) {
                $(window.parent.document).trigger(e); // calls parent document keydown method for switch active files
            }
        };
        function fnEachInputElement(e) {
            $(this).on("click", fnInputClick);
        };
        function fnInputClick(event) {
            elementScope.handleClick(event);
            event.stopPropagation();
            return false;
        };
        function fnBodyClick(event) {
            elementScope.handleClick(event);
            event.preventDefault();
        };
        function fnBodyDbClick() {
            elementScope.handledblClick(event);
        };
        $iframeBody.find(":hidden").not("option").show();  //beacuse in HTML applied style:none to element so we are removeing after rendering the element in iframe, Bug 10530:For few HTX forms ,the searched control is not getting highlighted.
        $rootScope.IsLoading = false;
    };

    //$scope.trustSrc = function (src) {
    //    return $sce.trustAsResourceUrl(src);
    //};


    $scope.findElementId = function (event) {
        if (event && event.target) {
            var controlId = "";
            var parentElement = null;
            $scope.selectedHtxControl = $scope.selectedControlId = null;
            var iframeBody = $('iframe').filter(":visible").contents().find("body");
            iframeBody.find(".s3-table-selector-parent").remove();
            var jElement = $(event.target);
            var tdElement = jElement.prop('tagName') == "TH" ? jElement : jElement.closest('td');
            controlId = jElement.attr('id');
            if (controlId) {
                $scope.selectControlByID(controlId);
            }
            else if (tdElement && tdElement.length) { // for grid/inside grid element selection
                var tableElement = tdElement.closest('table');
                var gridId, tdIndex = tdElement.index();
                if (tableElement && tableElement.length) {
                    gridId = tableElement.attr('id');
                }
                if (gridId) {
                    $("<div class='s3-table-selector-parent'><span class='s3-table-selector' title='Select Grid'></span></div>").insertBefore(tableElement);
                    $scope.selectedControlId = controlId;
                    SelectedHtmlControl(controlId, gridId, tdElement.prop("tagName"), tdElement.prop("tagName"), tdIndex);
                }
                else {
                    parentElement = jElement.closest('[id]');
                    if (parentElement && parentElement.length) {
                        controlId = parentElement.attr('id');
                        $scope.selectControlByID(controlId, parentElement);
                    }
                }
            }
            else {
                var lstParentLi = jElement.parent("li"); // for tab sheet selection
                var headerId = null;
                if (lstParentLi.length > 0) {
                    var objParent = $(lstParentLi[0]);
                    if (objParent[0].id && objParent[0].id.trim().endsWith("_Header")) {
                        var lstID = objParent[0].id.trim().split("_Header");
                        controlId = lstID[0];
                    }
                } else if (jElement.attr('id')) { // if element having id then select it
                    controlId = jElement.attr('id');
                } else {
                    parentElement = jElement.closest('[id]');
                    if (parentElement && parentElement.length) {
                        controlId = parentElement.attr('id');
                    }
                }

                $scope.selectControlByID(controlId, parentElement);
            }
            iframeBody.find(".s3-table-selector-parent").on('click', function () {
                $scope.selectedControlId = $(this).next().attr("id");
                $scope.selectedHtxControl = FindControlByID($scope.FormLinkModel, $scope.selectedControlId);
                $scope.handleClick(null);
            });
        }
    };

    $scope.selectControlByID = function (astrControlID, aparentElement) {
        if (astrControlID) {
            var currentelement = FindControlByID($scope.FormLinkModel, astrControlID);
            if (currentelement && currentelement.Name != "") {
                if (currentelement.Name == "sfwPanel" && aparentElement && aparentElement.length) {
                    $("<div class='s3-table-selector-parent'><span class='s3-table-selector' title='Select Panel'></span></div>").insertBefore(aparentElement);
                } else {
                    $scope.selectedHtxControl = currentelement;
                    $scope.selectedControlId = astrControlID;
                }
            }
        }
    };
    $scope.handleClick = function (event) {
        var iframeBody = $("#iframe" + $scope.FormLinkModel.dictAttributes.ID).contents().find("body");
        iframeBody.find('*').removeClass("selected-button selected-item selected-label selected-grid selected-checkbox s3-selected-tabsheet");
        if (event) {
            if ($(event.target).hasClass("s3-table-selector") || $(event.target).hasClass("s3-table-selector-parent")) {
                return;
            } else {
                $scope.findElementId(event);
            }
        }

        $timeout(function () {
            $scope.openCommonPropertiesClick();

            if ($scope.selectedHtxControl && $scope.selectedHtxControl.Name && $scope.selectedControlId) {
                if ($scope.selectedHtxControl.Name == 'sfwLabel' || $scope.selectedHtxControl.Name == 'sfwLinkButton' || $scope.selectedHtxControl.Name == "sfwChart" || $scope.selectedHtxControl.Name == "sfwCalendar") {
                    iframeBody.find('#' + $scope.selectedControlId.replace(/([ #;&,.+*~\':"!^$[\]()=>|\/@])/g, '\\$1')).addClass("selected-label");
                }
                else if ($scope.selectedHtxControl.Name == 'sfwButton') {
                    iframeBody.find('#' + $scope.selectedControlId.replace(/([ #;&,.+*~\':"!^$[\]()=>|\/@])/g, '\\$1')).addClass("selected-button");
                }
                else if ($scope.selectedHtxControl.Name == "sfwCheckBox") {
                    iframeBody.find('#' + $scope.selectedControlId.replace(/([ #;&,.+*~\':"!^$[\]()=>|\/@])/g, '\\$1')).addClass("selected-checkbox");
                }
                else if ($scope.selectedHtxControl.Name == 'sfwGridView') {
                    iframeBody.find('#' + $scope.selectedControlId.replace(/([ #;&,.+*~\':"!^$[\]()=>|\/@])/g, '\\$1')).addClass("selected-item");
                }
                else if ($scope.selectedHtxControl.Name != "sfwTabSheet") {
                    iframeBody.find('#' + $scope.selectedControlId.replace(/([ #;&,.+*~\':"!^$[\]()=>|\/@])/g, '\\$1')).addClass("selected-item");
                } else if ($scope.selectedHtxControl.Name == "sfwTabSheet") {
                    var domTabSheet = iframeBody.find('#' + $scope.selectedControlId + "_Header");
                    if (domTabSheet && domTabSheet.length) {
                        domTabSheet.addClass("s3-selected-tabsheet");
                    }
                }
            }
            else if ($scope.selectedControlId) {
                iframeBody.find('#' + $scope.selectedControlId.replace(/([ #;&,.+*~\':"!^$[\]()=>|\/@])/g, '\\$1')).addClass("selected-item");
            }
        });
    };
    //#region Code Value receive
    $scope.handledblClick = function (event) {
        if ($scope.selectedHtxControl && $scope.selectedHtxControl.Name) {
            var model = $scope.selectedHtxControl;
            if (model.Name == "sfwListPicker" || model.Name == "sfwSourceList" || model.Name == "sfwRadioButtonList" || model.Name == "sfwCascadingDropDownList" || model.Name == "sfwDropDownList" || model.Name == "sfwCheckBoxList" || model.Name == "sfwMultiSelectDropDownList" || model.Name == "sfwListBox") {
                if ((!model.dictAttributes.sfwLoadType || model.dictAttributes.sfwLoadType == "" || model.dictAttributes.sfwLoadType == "CodeGroup" || (model.dictAttributes.sfwLoadType == "Items" && model.Elements.length == 0)) && (model.dictAttributes.sfwLoadSource || model.placeHolder)) {
                    //#region Code Value receive
                    // clear selection if the dialog opens for code value 
                    $rootScope.IsLoading = true;

                    if (event) {
                        $(event.target).blur();
                        window.getSelection().removeAllRanges();
                    }
                    var codeID = "";
                    if (model.dictAttributes.sfwLoadSource && model.dictAttributes.sfwLoadType == "CodeGroup") {
                        codeID = model.dictAttributes.sfwLoadSource;
                    }
                    else {
                        codeID = model.placeHolder;
                    }
                    $.connection.hubMain.server.getCodeValuesForDropDown(codeID).done(function (data) {
                        $rootScope.IsLoading = false;

                        $scope.$evalAsync(function () {

                            var DialogScope = $scope.$new();
                            DialogScope.ErrorMessage = "";
                            if (data && data.length > 0) {
                                DialogScope.lstCodeValues = data;
                            }
                            else {
                                DialogScope.ErrorMessage = "No code value exist.";
                            }
                            dialog = $rootScope.showDialog(DialogScope, "Code Values", "Form/views/CodeValueListDialog.html");
                            DialogScope.closeDialog = function () {
                                dialog.close();
                            };
                        });
                    });
                }
            }
        }
    };
    //#endregion
    /*
    $scope.findHtmlControlInHtx = function (items, event) {
        function iIsTHFound(item) {
            if (!isTHFound) {
                if (item.innerHTML != null) {
                    if (item.innerHTML != tableTHInnerHtml) {
                        elementPosition++;
                    }
                    else if (item.innerHTML == tableTHInnerHtml) {
                        isTHFound = true;
                    }
                }
            }
        }

        function addControls(aCtrl) {
            lstControls.push(aCtrl);
        }

        function checkCurrentElementIndex(aCtrl) {
            if (!isFound) {
                if (tdElement == aCtrl)
                    isFound = true;
                else
                    elementPosition++;
            }
        }
        //if (event.target.tagName != "") {
        //    if (event.target.tagName == "SPAN") {
        //        return;
        //    }
        //}
        var elementPosition = 0;
        var controlID = event.target.id;
        var tagName = "";
        var tagParentName = "";
        var tableControlId = "";

        if (event.target.parentElement != null)
            tagParentName = event.target.parentElement.tagName;

        tagName = event.target.tagName;
        if (tagName != "") {
            var elementTag = tagName;
            var isTHFound = false;
            switch (tagName.toUpperCase()) {
                case "TH":
                    var tableTHInnerHtml = event.target.innerHTML;
                    while (elementTag.toUpperCase() != "TR") {
                        event = event.target.parentElement;
                        elementTag = event.tagName;
                    }
                    if (event.children != null) {

                        angular.forEach(event.children, iIsTHFound);
                    }
                    break;
                case "DIV":
                    event = event.target;
                    while (event.tagName.toUpperCase() == "DIV" && event.id == "") {
                        if (event.target)
                            event = event.target.parentElement;
                        else
                            event = event.parentElement;
                    }
                    if (event.id != "") {
                        controlID = event.id;
                    }
                    break;
                default:
                    if (controlID == "" && tagParentName.toUpperCase() == "TD") {
                        //keep current TD in one variable and find in TR's childrens
                        //then its a label
                        var tdElement = null;
                        var isFound = false;
                        tdElement = event.target;
                        var lstControls = [];
                        $(event.target).parents("tr").first().find("td").each(function () {
                            if (this.children && this.children.length > 0) {

                                angular.forEach(this.children, addControls);
                            }
                        });

                        angular.forEach(lstControls, checkCurrentElementIndex);
                        while (elementTag.toUpperCase() != "TD") {
                            tdElement = tdElement.parentElement;
                            elementTag = tdElement.tagName;
                        }
                        while (elementTag.toUpperCase() != "TR") {
                            if (event.target)
                                event = event.target.parentElement;
                            else
                                event = event.parentElement;

                            elementTag = event.tagName;
                        }
                    }
                    break;
            }

            while (event.parentElement != null && event.parentElement.tagName.toUpperCase() != "TABLE") {
                event = event.parentElement;
            }
        }
        if (event != null) {
            var parentId = "";
            if (event.parentElement) {
                parentId = event.parentElement.id;
            }
            else if (event.target) {
                parentId = event.target.parentElement.id;
            }
            SelectedHtmlControl(controlID, parentId, tagName, tagParentName, elementPosition);
        }
    };
    */
    function SelectedHtmlControl(controlId, tableControlId, tagName, tagParentName, index) {
        if (controlId) {
            var currentelement = FindControlByID($scope.FormLinkModel, controlId);
            if (currentelement && currentelement.Name != "") {
                $scope.selectedHtxControl = currentelement;
            }
        }
        else {
            var controlVM = FindControlByID($scope.FormLinkModel, tableControlId);
            if (controlVM) {
                if (tagName.toUpperCase() == "TH") {
                    if (controlVM && controlVM.Name == "sfwGridView") {
                        var columns = controlVM.Elements.filter(function (x) {
                            return x.Name.toLowerCase() == "columns";
                        });
                        if (columns) {
                            $scope.selectedHtxControl = columns[0].Elements[index];
                        }
                    }
                }
                else if (tagParentName.toUpperCase() == "TD") {
                    if (controlVM.Name == "sfwGridView") {
                        var columns = controlVM.Elements.filter(function (x) {
                            return x.Name.toLowerCase() == "columns";
                        });
                        if (columns && columns.length > 0 && columns[0].Elements) {
                            var elementPosition = -1;
                            var lstTemplateFields = columns[0].Elements;
                            for (var i = 0; i < lstTemplateFields.length; i++) {
                                if (lstTemplateFields[i].Elements && lstTemplateFields[i].Elements.length > 0) {
                                    var objItemTemplate = lstTemplateFields[i].Elements.filter(function (item) {
                                        return item.Name && item.Name.toLowerCase() == "itemtemplate";
                                    });
                                    if (objItemTemplate && objItemTemplate.length > 0) {
                                        for (var j = 0; j < objItemTemplate[0].Elements.length; j++) {
                                            if (objItemTemplate[0].Elements[j].Name != "cellformat") {
                                                elementPosition++;
                                                if (elementPosition == index) {
                                                    $scope.selectedHtxControl = objItemTemplate[0].Elements[j];
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                                if (elementPosition == index) {
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    $scope.InitialLoadSection = function () {
        $scope.SubQueryCollection = [];
        $scope.MainQuery = undefined;
        function iteration(objcustommethod) {
            if (objcustommethod.Name == "session") {
                $scope.SessionFields = objcustommethod;
                $scope.SessionFields.lstselectedobjecttreefields = [];
                $scope.SessionFields.LstDisplayedEntities = [];
            }
            else if (objcustommethod.Name == "callmethods") {
                if (!objcustommethod.dictAttributes.sfwMode) {
                    $scope.SelectedNewMethod = objcustommethod;
                    $scope.SelectedUpdateMethod = "";
                    $scope.FormLinkModel.IsSameAsNew = true;
                }
                if (objcustommethod.dictAttributes.sfwMode == 'New' || objcustommethod.dictAttributes.sfwMode == 'All') {
                    $scope.SelectedNewMethod = objcustommethod;
                }
                else if (objcustommethod.dictAttributes.sfwMode == 'Update' || objcustommethod.dictAttributes.sfwMode == 'All') {
                    $scope.SelectedUpdateMethod = objcustommethod;
                }
            }
            else if (objcustommethod.Name == "query") {
                var strQuery = objcustommethod.dictAttributes.sfwQueryRef;
                if ($scope.IsSubQuery(strQuery)) {

                    $scope.SubQueryCollection.push(objcustommethod);
                    $scope.SelectedSubQuery = objcustommethod;
                }
                else if (!$scope.MainQuery) {
                    $scope.MainQuery = objcustommethod;
                    $scope.StrMainQuery = objcustommethod.dictAttributes.ID + " - " + strQuery;

                    //****** NEED TO IMPLEMENT*******//

                    //foreach (CustomMethodDetails strNew in LoadMethodCollection)
                    //{
                    //    if (strNew.Description.StartsWith(objcustommethod[ApplicationConstants.XMLFacade.SFWRETURNTYPE] + " " + objcustommethod[ApplicationConstants.XMLFacade.SFWMETHODNAME]))
                    //{
                    //    SelectedLoadMethod = strNew.Name;
                    //    break;
                    //}

                }
                else if (objcustommethod) {
                    $scope.SubQueryCollection.push(objcustommethod);
                }
            }
        }

        angular.forEach($scope.InitialLoad.Elements, iteration);
        if ($scope.FormLinkModel.dictAttributes.sfwType == "FormLinkMaintenance") {
            if ($scope.InitialLoad) {
                if ($scope.SessionFields == undefined) {
                    $scope.SessionFields = {
                        Name: 'session', Value: '', dictAttributes: {}, Elements: [], Children: []
                    };
                    $scope.SessionFields.lstselectedobjecttreefields = [];
                    $scope.SessionFields.LstDisplayedEntities = [];
                }
            }
        }
    };

    $scope.IsSubQuery = function (strQuery) {
        function iterator(Query) {
            if (!retValue) {
                if (Query.ID == strQueryName && Query.QueryType.toLowerCase() == "subselectquery") {
                    retValue = true;
                }
            }
        }

        var retValue = false;
        if (strQuery != "" && strQuery != undefined) {
            var strCDOName = strQuery.substring(0, strQuery.indexOf("."));
            var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
            var lstObj = entityIntellisenseList.filter(function (x) {
                return x.DisplayName == strCDOName;
            });
            if (lstObj && lstObj.length > 0 && lstObj[0].Queries) {
                var strQueryName = strQuery.substring(strQuery.indexOf(".") + 1);

                angular.forEach(lstObj[0].Queries, iterator);
            }
        }

        return retValue;
    };


    //#endregion

    //#region Open Common Properties
    $scope.openCommonPropertiesClick = function () {
        $scope.FormLinkModel.IsCommonPropertiesOpen = true;
        SetFormLinkSelectedControl();
    };

    function SetFormLinkSelectedControl() {
        $scope.IsSwitchCheckBox = false;
        if ($scope.selectedHtxControl && $scope.selectedHtxControl.dictAttributes && $scope.selectedHtxControl.dictAttributes.sfwIsSwitch && $scope.selectedHtxControl.dictAttributes.sfwIsSwitch.toLowerCase() == "true") {
            $scope.IsSwitchCheckBox = true;
        }
        else {
            $scope.IsSwitchCheckBox = false;
            $scope.FormLinkModel.SelectedControl = $scope.selectedHtxControl;
        }
        $scope.$evalAsync(function () {
            $scope.FormLinkModel.SelectedControl = $scope.selectedHtxControl;
        });
        //$scope.FormLinkModel.SelectedControl.IsSelected = true;
    }
    //#endregion


    //#region Xml source Start
    $scope.selectedDesignSource = false;
    $scope.ShowDesign = function () {
        if ($scope.selectedDesignSource == true) {
            var xmlstring = $scope.editor.getValue();
            if (xmlstring != null && xmlstring != "") {
                $rootScope.IsLoading = true;
                if (xmlstring.length < 32000) {
                    hubMain.server.getXmlString(xmlstring, $scope.currentfile);
                }
                else {
                    var strpacket = "";
                    var lstDataPackets = [];
                    var count = 0;
                    for (var i = 0; i < xmlstring.length; i++) {
                        count++;
                        strpacket = strpacket + xmlstring[i];
                        if (count == 32000) {
                            count = 0;
                            lstDataPackets.push(strpacket);
                            strpacket = "";
                        }
                    }
                    if (count != 0) {
                        lstDataPackets.push(strpacket);
                    }

                    SendDataPacketsToServer(lstDataPackets, $scope.currentfile, "SourceToDesignCommon");
                }
                $scope.receivexmlobject = function (data) {
                    $scope.selectedDesignSource = false;
                    if ($scope.isSourceDirty) {
                        $scope.receiveformlinkmodel(data);
                        $scope.isSourceDirty = false;
                    }
                    //else {
                    //    $scope.removeExtraFieldsDataInToMainModel();
                    //}
                    $scope.$evalAsync(function () {
                        $rootScope.IsLoading = false;
                    });
                };
            }
        }
    };
    //$scope.isSourceDirty;
    //$scope.sourceChanged = function () {
    //    $scope.isSourceDirty = true;
    //}
    var SendDataPacketsToServer = function (lstpackets, filedetails, operationtoperform) {
        for (var i = 0; i < lstpackets.length; i++) {
            hubMain.server.receiveDataPackets(lstpackets[i], lstpackets.length, filedetails, i, operationtoperform, null);
        }
    };

    $scope.ShowSource = function () {
        if ($scope.selectedDesignSource == false) {
            $scope.selectedDesignSource = true;
            if ($scope.FormLinkModel) {
                $rootScope.IsLoading = true;
                // $scope.addExtraFieldsDataInToMainModel();

                if ($scope.FormLinkModel.dictAttributes.sfwType != "FormLinkLookup") {
                    $scope.dummyLstLoadDetails = LoadDetails($scope.FormLinkModel, $scope.objLoadDetails, false, $rootScope, true);
                }

                var objreturn1 = GetBaseModel($scope.FormLinkModel);

                if (objreturn1 != "") {

                    var strobj = JSON.stringify(objreturn1);
                    if (strobj.length < 32000) {
                        if ($scope.selectedHtxControl && $scope.selectedHtxControl.dictAttributes && $scope.selectedHtxControl.dictAttributes.ID) {
                            hubMain.server.getFormLinkXml(strobj, $scope.currentfile, $scope.selectedHtxControl.dictAttributes.ID);
                        }
                        else {
                            hubMain.server.getFormLinkXml(strobj, $scope.currentfile, null);
                        }
                    }
                    else {
                        var strpacket = "";
                        var lstDataPackets = [];
                        var count = 0;
                        for (var i = 0; i < strobj.length; i++) {
                            count++;
                            strpacket = strpacket + strobj[i];
                            if (count == 32000) {
                                count = 0;
                                lstDataPackets.push(strpacket);
                                strpacket = "";
                            }
                        }
                        if (count != 0) {
                            lstDataPackets.push(strpacket);
                        }
                        var controlIds = [];
                        if ($scope.selectedHtxControl && $scope.selectedHtxControl.dictAttributes && $scope.selectedHtxControl.dictAttributes.ID) {
                            controlIds.push($scope.selectedHtxControl.dictAttributes.ID);
                        }

                        for (var i = 0; i < lstDataPackets.length; i++) {
                            hubMain.server.receiveDataPackets(lstDataPackets[i], lstDataPackets.length, $scope.currentfile, i, "DesignToSourceFormLink", controlIds);
                        }
                    }
                    $scope.receiveFormLinkXml = function (xmlstring, lineno) {
                        $scope.$evalAsync(function () {
                            var ID = $scope.currentfile.FileName;
                            setDataToEditor($scope, xmlstring, lineno, ID);
                            $scope.$evalAsync(function () {
                                $rootScope.IsLoading = false;
                            });

                            if (window.navigator.userAgent.toLowerCase().contains("chrome")) {
                                $scope.$evalAsync(function () {
                                    $rootScope.IsLoading = false;
                                });
                            }
                        });
                    };
                }
            }
        }
    };

    //#endregion

    //#region OnRefreshClick
    $scope.OnRefreshClick = function () {
        //if ($scope.refreshedModel)
        //    $scope.refreshedModel = GetBaseModel($scope.refreshedModel);
        //else

        $scope.dummyLstLoadDetails = LoadDetails($scope.FormLinkModel, $scope.objLoadDetails, false, $rootScope, true);

        $scope.refreshedModel = GetBaseModel($scope.FormLinkModel);
        $scope.SaveModelWithPackets($scope.refreshedModel, undefined, false);
    };

    $scope.SaveModelWithPackets = function (scopeobject, controls, isRefresh) {

        var strobj = JSON.stringify(scopeobject);
        $rootScope.IsLoading = true;
        if (strobj.length < 32000 && !isRefresh) {
            $.connection.hubFormLink.server.onRefreshFormLinkModel(strobj, true);
        }
        else {
            var strpacket = "";
            var lstDataPackets = [];
            var count = 0;
            for (var i = 0; i < strobj.length; i++) {
                count++;
                strpacket = strpacket + strobj[i];
                if (count == 32000) {
                    count = 0;
                    lstDataPackets.push(strpacket);
                    strpacket = "";
                }
            }
            if (count != 0) {
                lstDataPackets.push(strpacket);
            }

            SendHtxDataPacketsToServer(lstDataPackets, controls, isRefresh, false);
        }
    };

    var SendHtxDataPacketsToServer = function (lstpackets, controls, isRefresh, isDisplayControlMapping) {

        for (var i = 0; i < lstpackets.length; i++) {
            $.connection.hubFormLink.server.receiveDataPackets(lstpackets[i], lstpackets.length, i, controls, isRefresh, isDisplayControlMapping);
        }
    };

    $scope.receiveMapUnmapControls = function (controlLists, controlTypes, newmodel, oldModel) {
        $scope.$evalAsync(function () {
            $rootScope.IsLoading = false;
        });

        var data = JSON.parse(controlLists);
        $scope.OldModel = oldModel;
        $scope.MapControlList = [];
        $scope.MappedControlList = [];
        $scope.IgnoredControlList = [];
        $scope.RemovedControlList = [];
        $scope.InvalidControlList = [];

        function iAddInMapControlList(cntrl) {
                var item = {
                    ControlID: cntrl.ControlID, HtmlControlType: cntrl.HtmlControlType, ControlType: cntrl.ControlType, IsValid: cntrl.IsValid
                };
                var tempDescendent = getDescendents(oldModel);
                if (tempDescendent && tempDescendent.length > 0) {
                    var IsSwitch = tempDescendent.filter(function (x) { return x.dictAttributes.ID == cntrl.ControlID });
                    if (IsSwitch && IsSwitch.length > 0 && IsSwitch[0].Name == "sfwCheckBox") {
                        if (IsSwitch[0].dictAttributes && IsSwitch[0].dictAttributes.sfwIsSwitch && IsSwitch[0].dictAttributes.sfwIsSwitch.toLowerCase() == "true") {
                            item.sfwIsSwitch = "True";
                        }
                    }
                }

                $scope.MapControlList.push(item);
        }

        function iAddInMappedControlList(cntrl) {
                var item = {
                    ControlID: cntrl.ControlID, HtmlControlType: cntrl.HtmlControlType, ControlType: cntrl.ControlType, IsValid: cntrl.IsValid
                };
                $scope.MappedControlList.push(item);
        }
        function iAddInRemovedControlList(cntrl) {
            var item = {
                ControlID: cntrl.ControlID, HtmlControlType: cntrl.HtmlControlType, ControlType: cntrl.ControlType, IsValid: cntrl.IsValid
            };
            $scope.RemovedControlList.push(item);
        }
        function iAddInIgnoredControlList(cntrl) {
            var item = {
                ControlID: cntrl.ControlID, HtmlControlType: cntrl.HtmlControlType, ControlType: cntrl.ControlType, IsValid: cntrl.IsValid
            };
            $scope.IgnoredControlList.push(item);
        }
        function iAddInInvalidControlList(cntrl) {
            var item = {
                ControlID: cntrl.ControlID, HtmlControlType: cntrl.HtmlControlType, ControlType: cntrl.ControlType, IsValid: cntrl.IsValid
            };
            $scope.InvalidControlList.push(item);
        }
        if (data.Map || data.Mapped || data.Removed || data.Ignored || data.Invalid) {
            if (data.Map && data.Map.length > 0) {

                angular.forEach(data.Map, iAddInMapControlList);
            }
            if (data.Mapped && data.Mapped.length > 0) {

                angular.forEach(data.Mapped, iAddInMappedControlList);
            }
            if (data.Removed && data.Removed.length > 0) {

                angular.forEach(data.Removed, iAddInRemovedControlList);
            }
            if (data.Ignored && data.Ignored.length > 0) {

                angular.forEach(data.Ignored, iAddInIgnoredControlList);
            }
            if (data.Invalid && data.Invalid.length > 0) {

                angular.forEach(data.Invalid, iAddInInvalidControlList);
            }

            var data = JSON.parse(controlTypes);
            $scope.$evalAsync(function () {
                var newScope = $scope.$new();
                $scope.activeTab = "MAP";
                newScope.isMappedControlClick = false;
                newScope.OldModel = oldModel;
                newScope.NewModel = newmodel;
                newScope.MapControls = $scope.MapControlList;
                newScope.MappedControls = $scope.MappedControlList;
                newScope.RemovedControls = $scope.RemovedControlList;
                newScope.IgnoredControls = $scope.IgnoredControlList;
                newScope.InvalidControls = $scope.InvalidControlList;
                newScope.ControlTypes = data;
                if ($scope.FormLinkModel.dictAttributes.sfwType == "FormLinkLookup") {
                    var index = newScope.ControlTypes.indexOf("sfwHyperLink");
                    newScope.ControlTypes.splice(index, 1);
                    index = newScope.ControlTypes.indexOf("sfwJSONData");
                    if (index >= 0) {
                        newScope.ControlTypes.splice(index, 1);
                    }
                    index = newScope.ControlTypes.indexOf("sfwCalendar");
                    if (index >= 0) {
                        newScope.ControlTypes.splice(index, 1);
                    }
                    index = newScope.ControlTypes.indexOf("sfwScheduler");
                    if (index >= 0) {
                        newScope.ControlTypes.splice(index, 1);
                    }
                }
                if ($scope.FormLinkModel.dictAttributes.sfwType == "FormLinkMaintenance" || $scope.FormLinkModel.dictAttributes.sfwType == "FormLinkWizard") {
                    newScope.ControlTypes.push("sfwListView");
                }
                if (newScope.MapControls.some(function (x) {
                    if (x.sfwIsSwitch && x.sfwIsSwitch.toLowerCase() == "true") {
                        x.ControlType = "sfwSwitchCheckBox";
                        return true;
                    }
                }));

                newScope.onOKClick = function () {
                    //close the dialog
                    newScope.isOkClick = true;
                    var mapUnmapcontrols = newScope.MapControls.concat(newScope.MappedControls);
                    newScope.updateDataFields(newScope.isOkClick, mapUnmapcontrols, newScope.RemovedControls);
                    newScope.closeDialog();
                };
                newScope.closeDialog = function () {
                    if (newScope.dialog) {
                        newScope.isOkClick = false;
                        //newScope.updateDataFields(newScope.isOkClick, undefined, undefined);
                        $rootScope.IsLoading = false;
                        newScope.dialog.close();
                    }
                };

                newScope.setActiveTab = function (tab) {
                    newScope.activeTab = tab;
                    if (tab == "MAPPED")
                        newScope.isMappedControlClick = true;
                };



                newScope.updateDataFields = function (isOkClick, mapCntrls, removedCntrls) {
                    //create new htx model
                    if (isOkClick) {
                        $scope.SaveModelWithPackets(newScope.NewModel, mapCntrls, true);

                    }
                    else {
                        $scope.SaveModelWithPackets(newScope.NewModel, null, true);
                    }
                    $scope.OldModel = newScope.OldModel;

                };

                newScope.dialog = $rootScope.showDialog(newScope, "Control Mapping", "CreateNewObject/views/FormLink/FormLinkMapUnmapControls.html", {
                    width: 600, height: 400
                });
            });
        }
        else {
            alert("HTML Controls not exist.");
        }
    };

    //#region GetModel and UpdateDataFields related functions\
    function updateAttributesOfControl(aNewModel, aOldModel) {
        function iterator(newChildModel) {
            var oldChildModel = GetModel(newChildModel, aOldModel);
            if (oldChildModel && oldChildModel.length > 0 && oldChildModel[0].Name && oldChildModel[0].Name != "ListItem") {
                if (oldChildModel) {
                    UpdateModel(newChildModel, oldChildModel[0]);
                }
                if (newChildModel.Name != "sfwGridView" && newChildModel.Name != "cellformat") {
                    updateAttributesOfControl(newChildModel, aOldModel);
                }
            }
        }
        if (aNewModel.length > 0) {
            if (aNewModel.Elements) {
                angular.forEach(aNewModel.Elements, iterator);
            }
            else {
                angular.forEach(aNewModel[0].Elements, iterator);
            }
        }
        else {
            if (aNewModel.Elements)
                angular.forEach(aNewModel.Elements, iterator);
        }
    }

    function GetModel(newChildModel, aOldModel) {
        var retVal = [];
        if (newChildModel.Name == "TemplateField") {
            var oldDescendents = [];
            if (aOldModel) {
                if (aOldModel.length > 0)
                    oldDescendents = getDescendents(aOldModel[0]);
                else
                    oldDescendents = getDescendents(aOldModel);

                retVal = oldDescendents.filter(function (ele) {
                    return ele.Name == newChildModel.Name &&
                        ele.dictAttributes.HeaderText == newChildModel.dictAttributes.HeaderText
                        && IsSameGridID(newChildModel, ele);
                });
            }
        }
        else if (newChildModel.ParentVM.Name == "ItemTemplate") {
            var templateFieldModel = GetParentModel(newChildModel, "TemplateField");
            if (templateFieldModel) {
                var oldTemplateModel = GetModel(templateFieldModel, aOldModel);
                if (oldTemplateModel && oldTemplateModel.length > 0 && oldTemplateModel[0].Elements) {
                    var itemTemplateModel = oldTemplateModel[0].Elements.filter(function (ele) {
                        return ele.Name == "ItemTemplate";
                    });
                    if (itemTemplateModel) {
                        var itemTemplatedescendents = [];
                        if (itemTemplateModel.length > 0)
                            itemTemplatedescendents = getDescendents(itemTemplateModel[0]);
                        else
                            itemTemplatedescendents = getDescendents(itemTemplateModel);

                        retVal = itemTemplatedescendents.filter(function (ele) {
                            if (ele.dictAttributes.ID) {
                                return ele.Name == newChildModel.Name && ele.dictAttributes.ID == newChildModel.dictAttributes.ID;
                            }
                            else {
                                return ele.Name == newChildModel.Name;
                            }
                        });
                    }
                }
            }
        }
        else {
            var oldDescendents = [];
            if (aOldModel) {
                if (aOldModel.length > 0)
                    oldDescendents = getDescendents(aOldModel[0]);
                else
                    oldDescendents = getDescendents(aOldModel);
                retVal = oldDescendents.filter(function (ele) {
                    return ele.Name == newChildModel.Name && ele.dictAttributes.ID == newChildModel.dictAttributes.ID;
                });
            }
        }

        return retVal;
    }

    function GetParentModel(curModel, astrModelName) {
        var retVal = {
            prefix: "", Name: "", Value: "", dictAttributes: [], Elements: [], Children: []
        };
        var parentObj = curModel;
        while (null != parentObj) {
            if (parentObj.Name == "form") {
                break;
            }
            else if (parentObj.Name == astrModelName) {
                retVal = parentObj;
                break;
            }

            parentObj = parentObj.ParentVM;
        }

        return retVal;
    }

    function IsSameGridID(aNewModel, aOldModel) {
        var retVal = false;
        var newGridModel = GetParentModel(aNewModel, "sfwGridView");
        var oldGridModel = GetParentModel(aOldModel, "sfwGridView");
        if (null != newGridModel && null != oldGridModel) {
            retVal = newGridModel.dictAttributes.ID == oldGridModel.dictAttributes.ID;
        }
        return retVal;
    }

    function UpdateModel(aNewModel, aOldModel) {
        var tempID = aNewModel.dictAttributes.ID;
        if (aOldModel) {
            aNewModel.dictAttributes = aOldModel.dictAttributes;
        }

        aNewModel.dictAttributes.ID = tempID;

        if (aNewModel.Name == "sfwChart" || aNewModel.Name == "sfwListView" || aNewModel.Name == "sfwImage"
            || aNewModel.Name == "sfwDropDownList" || aNewModel.Name == "sfwCascadingDropDownList" || aNewModel.Name == "sfwMultiSelectDropDownList"
            || aNewModel.Name == "sfwListPicker" || aNewModel.Name == "sfwListBox" || aNewModel.Name == "sfwRadioButtonList"
            || aNewModel.Name == "sfwCheckBoxList" || aNewModel.Name == "sfwSourceList") {
            if (aOldModel) {
                angular.forEach(aOldModel.Elements, function (ele) {
                    aNewModel.Elements.push(ele);
                });
            }
        }

        if (aNewModel.Name == "sfwGridView") {
            if (aOldModel && aOldModel.Elements != null) {
                //var lst = aNewModel.Elements.filter(function (itm) { itm.Name != "TemplateField" });
                var newColumns = aNewModel.Elements.filter(function (itm) { return itm.Name == "Columns"; });
                var oldColumns = aOldModel.Elements.filter(function (itm) { return itm.Name == "Columns"; });
                if (newColumns && newColumns.length > 0 && oldColumns && oldColumns.length > 0) {
                    angular.forEach(oldColumns[0].Elements, function (ele) {
                        if (ele.Name != 'TemplateField') {
                            newColumns[0].Elements.push(ele);
                        }
                        angular.forEach(ele.Elements, function (itm) {
                            var lst = newColumns[0].Elements.filter(function (item) { return item.dictAttributes && item.dictAttributes.HeaderText == ele.dictAttributes.HeaderText; });
                            if (lst && lst.length > 0) {
                                var field = lst[0];
                                if (itm.Name != "ItemTemplate") {
                                    field.Elements.push(itm);
                                }
                                else {

                                    var lstItemTemplate = field.Elements.filter(function (itemTemplate) {
                                        return itemTemplate.Name == "ItemTemplate";
                                    });
                                    if (lstItemTemplate && lstItemTemplate.length > 0) {
                                        angular.forEach(itm.Elements, function (cell) {
                                            if (cell.Name == "cellformat") {
                                                lstItemTemplate[0].Elements.push(cell);
                                            }
                                        });
                                        updateAttributesOfControl(lstItemTemplate[0], aOldModel);
                                    }
                                }
                            }
                        });
                    });
                }
                var oldParameters = aOldModel.Elements.filter(function (itm) { return itm.Name == "Parameters"; });
                if (oldParameters && oldParameters.length) {
                    aNewModel.Elements.push(oldParameters[0]);
                }

                var oldRowFormats = aOldModel.Elements.filter(function (itm) { return itm.Name == "rowformat"; });
                if (oldRowFormats && oldRowFormats.length) {
                    aNewModel.Elements.push(oldRowFormats[0]);
                }
            }
        }

    }
    //#endregion

    $scope.receiveRefreshedModel = function (data) {
        $scope.refreshedModel = JSON.parse(data);
        //below code is to update each control with old values in model
        $scope.setParentVM($scope.refreshedModel);
        $scope.setParentVM($scope.OldModel);

        var newItemsModel = $scope.refreshedModel.Elements.filter(function (x) {
            return x.Name == "items";
        });
        for (var i = 0; i < $scope.OldModel.Elements.length; i++) {
            if ($scope.OldModel.Elements[i].Name == "items") {
                $scope.OldModel.Elements[i] = newItemsModel[0];
            }
        }

        var curItemsModel = $scope.OldModel.Elements.filter(function (x) {
            return x.Name == "items";
        });

        //if (newItemsModel && curItemsModel) {
        //    updateAttributesOfControl(newItemsModel, curItemsModel);
        //}
        var saveNewModel = {
            Name: $scope.refreshedModel.Name, prefix: $scope.refreshedModel.prefix, Value: $scope.refreshedModel.Value, dictAttributes: $scope.refreshedModel.dictAttributes, Elements: [], Children: []
        };
        var lstInitialLoad = $scope.OldModel.Elements.filter(function (itm) { return itm.Name == "initialload"; });
        if (lstInitialLoad && lstInitialLoad.length > 0) {
            var initialLoadModel = GetBaseModel(lstInitialLoad[0]);
            saveNewModel.Elements.push(initialLoadModel);
        }
        var itemsModel = GetBaseModel(newItemsModel[0]);
        saveNewModel.Elements.push(itemsModel);
        saveNewModel.objExtraData = $scope.refreshedModel.objExtraData;
        var lstLoadDetails = $scope.OldModel.Elements.filter(function (itm) { return itm.Name == "loaddetails"; });
        if (lstLoadDetails && lstLoadDetails.length > 0) {
            var loadDetailsModel = GetBaseModel(lstLoadDetails[0]);
            saveNewModel.Elements.push(loadDetailsModel);
        }

        $rootScope.SaveModelWithPackets(saveNewModel, $rootScope.currentopenfile.file, $rootScope.currentopenfile.file.FileType, false);
        $scope.FormLinkModel = saveNewModel;
        $scope.receiveformlinkmodel(saveNewModel, true);

        $rootScope.IsLoading = false;
    };

    $scope.receiveRemovedControls = function (data, oldModel) {
        $scope.OldModel = oldModel;
        $scope.$evalAsync(function () {
            var newScope = $scope.$new();
            newScope.RemovedControls = data;
            var model = JSON.stringify($scope.refreshedModel);
            newScope.onOKClick = function () {

                //close the dialog
                newScope.isOkClick = true;

                $.connection.hubFormLink.server.onRefreshFormLinkModel(model, false);

                //$.connection.hubFormLink.server.getRemoveControlAction(newScope.isOkClick, model);
                newScope.closeDialog();
            };
            newScope.closeDialog = function () {
                if (newScope.dialog) {
                    newScope.isOkClick = false;
                    //   $.connection.hubFormLink.server.getRemoveControlAction(newScope.isOkClick, undefined);
                    newScope.dialog.close();
                }
            };
            $rootScope.IsLoading = false;
            newScope.dialog = $rootScope.showDialog(newScope, "Removed Controls", "FormLink/views/RemovedControls.html", {
                width: 500, height: 500
            });
        });
    };


    //#endregion

    //#region FormLink Common Methods

    $scope.OnDisplayUnmapColumnclick = function () {

        var objModel = GetBaseModel($scope.FormLinkModel);
        var strobj = JSON.stringify(objModel);
        $rootScope.IsLoading = true;


        if (strobj.length < 32000) {
            $.connection.hubFormLink.server.displayControlMapping(strobj);
        }
        else {
            var strpacket = "";
            var lstDataPackets = [];
            var count = 0;
            for (var i = 0; i < strobj.length; i++) {
                count++;
                strpacket = strpacket + strobj[i];
                if (count == 32000) {
                    count = 0;
                    lstDataPackets.push(strpacket);
                    strpacket = "";
                }
            }
            if (count != 0) {
                lstDataPackets.push(strpacket);
                SendHtxDataPacketsToServer(lstDataPackets, null, false, true);
            }
        }


    };

    $scope.receiveControlMappingForDisplay = function (data) {
        $scope.$evalAsync(function () {
            $rootScope.IsLoading = false;
        });
        if (data) {
            var newScope = $scope.$new();
            if (data.length == 3) {
                newScope.lstHtmlMapControl = data[0];
                newScope.lstHtmlIgnoreControl = data[1];
                newScope.lstHtmlInvalidControl = data[2];
            }

            newScope.SelectedTab = "UnMapControl";

            newScope.selectControlMappingTab = function (selectedTab) {
                newScope.SelectedTab = selectedTab;
            };

            newScope.dialog = $rootScope.showDialog(newScope, "Display Control Mapping", "FormLink/views/DisplayControlMapping.html", {
                width: 550, height: 550
            });

        }
        else {
            alert("HTML file not exist.");
        }
    };

    //#endregion

    //#region FormLink Details

    $scope.onDetailClick = function () {
        $rootScope.IsLoading = true;

        var newScope = $scope.$new();
        newScope.MainQuery = undefined;
        newScope.objExtraFields = [];
        newScope.objDirFunctions = {
        };
        newScope.showExtraFieldsTab = false;
        newScope.formName = "HTML";
        newScope.FormLinkModel = {};
        newScope.SelectedNewMethod = null;
        newScope.SelectedUpdateMethod = null;
        if ($scope.InitialLoad) {
            newScope.InitialLoad = {};
            $scope.InitialLoad.ParentVM = null;
            angular.copy($scope.InitialLoad, newScope.InitialLoad);
        }
        else {
            newScope.InitialLoad = { Name: 'initialload', Value: '', dictAttributes: {}, Elements: [], Children: [] };
        }

        newScope.InitialLoadSectionForDetail = function () {
            newScope.$evalAsync(function () {
                newScope.SubQueryCollection = { Elements: [] };

                if (newScope.InitialLoad) {
                    angular.forEach(newScope.InitialLoad.Elements, function (objcustommethod) {
                        if (objcustommethod.Name == "callmethods") {
                            if (!objcustommethod.dictAttributes.sfwMode) {
                                newScope.SelectedNewMethod = objcustommethod;
                                newScope.SelectedUpdateMethod = "";
                                newScope.FormLinkModel.IsSameAsNew = true;
                            }
                            if (objcustommethod.dictAttributes.sfwMode == 'New' || objcustommethod.dictAttributes.sfwMode == 'All') {
                                newScope.SelectedNewMethod = objcustommethod;
                            }
                            if (objcustommethod.dictAttributes.sfwMode == 'Update' || objcustommethod.dictAttributes.sfwMode == 'All') {
                                newScope.SelectedUpdateMethod = objcustommethod;
                            }
                        }
                        else if (objcustommethod.Name == "query") {
                            var strQuery = objcustommethod.dictAttributes.sfwQueryRef;
                            if ($scope.IsSubQuery(strQuery)) {

                                newScope.SubQueryCollection.Elements.push(objcustommethod);
                                newScope.SelectedSubQuery = objcustommethod;
                            }

                            else if (!newScope.MainQuery) {
                                newScope.MainQuery = objcustommethod;
                            }
                            else {
                                newScope.SubQueryCollection.Elements.push(objcustommethod);
                            }
                        }
                        else if (objcustommethod.Name == "session") {
                            newScope.SessionFields = objcustommethod;
                            newScope.SessionFields.lstselectedobjecttreefields = [];
                            newScope.SessionFields.LstDisplayedEntities = [];
                        }
                    });
                }
                if ($scope.FormLinkModel.dictAttributes.sfwType == "FormLinkMaintenance") {
                    if (newScope.InitialLoad) {
                        if (newScope.SessionFields == undefined) {
                            newScope.SessionFields = {
                                Name: 'session', Value: '', dictAttributes: {}, Elements: [], Children: []
                            };
                            newScope.SessionFields.lstselectedobjecttreefields = [];
                            newScope.SessionFields.LstDisplayedEntities = [];
                        }
                    }
                }
            });
        };

        newScope.FormLinkModel = { dictAttributes: {} };
        angular.forEach($scope.FormLinkModel.dictAttributes, function (val, key) {
            newScope.FormLinkModel.dictAttributes[key] = val;
        });

        newScope.FormLinkModel.RemoteObjectCollection = [];
        if ($scope.FormLinkModel.RemoteObjectCollection) {
            angular.copy($scope.FormLinkModel.RemoteObjectCollection, newScope.FormLinkModel.RemoteObjectCollection);
        }
        newScope.$evalAsync(function () {
            newScope.IsPrototypeDetails = false;
        });

        if ($scope.FormLinkModel.dictAttributes.ID.startsWith("wfp")) {
            newScope.$evalAsync(function () {
                newScope.IsPrototypeDetails = true;
            })
        }
        var entityname = $scope.FormLinkModel.dictAttributes.sfwEntity;
        $.connection.hubMain.server.getFormExtraData(entityname).done(function (extradata) {
            newScope.$evalAsync(function () {
                newScope.FormLinkModel.objExtraData = extradata;
                newScope.populateXmlMethods(newScope.FormLinkModel.isSrvMethodChecked);
                newScope.Init();
            });
        });


        newScope.populateXmlMethods = function (isSrvChecked) {
            newScope.XmlNewMethodsCollection = [];
            newScope.XmlUpdateMethodsCollection = [];
            if (isSrvChecked == "false") {
                newScope.paramtersForNewMethod = [];
                newScope.paramtersForUpdateMethod = [];
            }
            if (newScope.FormLinkModel.objExtraData) {
                var dummyObj = {
                    Name: '', Value: '', dictAttributes: { ID: "" }, Elements: [], Children: []
                };
                if (newScope.FormLinkModel.objExtraData.lstMethodsList) {
                    var methodsModel = newScope.FormLinkModel.objExtraData.lstMethodsList.filter(function (x) {
                        return x.Name.toLowerCase() == "methods"
                    });
                    newScope.XmlUpdateMethodsCollection.push(dummyObj);
                    newScope.XmlNewMethodsCollection.push(dummyObj);
                    var dummyObj = {
                        Name: '', Value: '', dictAttributes: { ID: "NewObject" }, Elements: [], Children: []
                    };
                    newScope.XmlNewMethodsCollection.push(dummyObj);
                    if (methodsModel && methodsModel.length > 0) {
                        for (var i = 0; i < methodsModel.length; i++) {
                            for (j = 0; j < methodsModel[i].Elements.length; j++) {
                                var item = methodsModel[i].Elements[j];
                                if (item.Name == "method") {
                                    if (item.dictAttributes.sfwMode == 'New' || item.dictAttributes.sfwMode == 'All') {
                                        newScope.XmlNewMethodsCollection.push(item);
                                    }
                                    if (item.dictAttributes.sfwMode == 'Update' || item.dictAttributes.sfwMode == 'All') {
                                        newScope.XmlUpdateMethodsCollection.push(item);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };

        if ($scope.FormLinkModel.dictAttributes.sfwType != "FormLinkLookup") {
            newScope.populateXmlMethods();
        }
        newScope.RemoteObjectCollection = [];
        newScope.IsEntityTabSelected = false;
        newScope.IsGroupTabSelected = false;
        newScope.IsSessionTabSelected = false;
        newScope.SrvMethodCollection = [];
        newScope.lstWebsite = [];
        var objNewDialog = undefined;
        var temObj = ConfigurationFactory.getLastProjectDetails();

        if (temObj && temObj != null) {
            var tempWbsite = ConfigurationFactory.getLastProjectDetails().Website;
            if (tempWbsite && tempWbsite != null && tempWbsite.contains(";")) {
                newScope.lstWebsite = ConfigurationFactory.getLastProjectDetails().Website.split(";");
            }
            else {
                newScope.lstWebsite.push(ConfigurationFactory.getLastProjectDetails().Website);
            }
            newScope.lstWebsite.splice(0, 0, "");
        }

        newScope.Init = function () {
            newScope.IsSameAsNewDisabled = true;

            newScope.FormLinkModel.isSrvMethodChecked = "false";
            if (newScope.FormLinkModel.dictAttributes.sfwRemoteObject != undefined && newScope.FormLinkModel.dictAttributes.sfwRemoteObject != "") {
                newScope.FormLinkModel.isSrvMethodChecked = "true";
                $.connection.hubForm.server.getRemoteObjectList().done(function (data) {
                    newScope.$evalAsync(function () {
                        if (data) {
                            newScope.FormLinkModel.RemoteObjectCollection = data;
                            if (newScope.FormLinkModel.RemoteObjectCollection && newScope.FormLinkModel.RemoteObjectCollection.length > 0) {
                                newScope.FormLinkModel.RemoteObjectCollection.splice(0, 0, {
                                    dictAttributes: {
                                        ID: ""
                                    }
                                });
                            }
                            newScope.PopulateRemoteObject();
                        }
                    });
                });
                //newScope.PopulateRemoteObject();
            }
            else {
                if (newScope.SelectedNewMethod) {
                    newScope.populateParamtersForNew(newScope.SelectedNewMethod);
                } else {
                    newScope.SelectedNewMethod = { Name: 'callmethods', Value: '', dictAttributes: { sfwMode: "New" }, Elements: [] };
                }
                if (newScope.SelectedUpdateMethod) {
                    newScope.populateParamtersForUpdate(newScope.SelectedUpdateMethod);
                } else {
                    newScope.SelectedUpdateMethod = { Name: 'callmethods', Value: '', dictAttributes: { sfwMode: "Update" }, Elements: [] };
                }
            }

            if (newScope.FormLinkModel.objExtraData && newScope.FormLinkModel.objExtraData.lstRulesList) {
                newScope.lstLogRules = newScope.FormLinkModel.objExtraData.lstRulesList;
                newScope.lstLogRules.splice(0, 0, { dictAttributes: { ID: '' } });
            }

            newScope.lstButtons = [];
            lstTable = $scope.FormLinkModel.Elements.filter(function (item) {
                return item.Name == "items";
            });
            if (lstTable && lstTable.length > 0) {
                FindControlListByName(lstTable[0], "sfwButton", newScope.lstButtons);
            }
            if ($scope.FormLinkModel.dictAttributes.sfwType == "FormLinkLookup") {
                if (!newScope.MainQuery) {
                    newScope.MainQuery = { Name: 'query', Value: '', dictAttributes: {}, Elements: [] };
                    newScope.InitialLoad.Elements.push(newScope.MainQuery);
                }
                //else {
                //    newScope.MainQuery = {};
                //    angular.copy($scope.MainQuery, newScope.MainQuery);
                //}
            }

            $rootScope.IsLoading = false;
            objNewDialog = $rootScope.showDialog(newScope, "Form Details", "FormLink/views/FormLinkDetails.html", {
                width: 700, height: 700
            });
        };

        //#region Method type methods
        newScope.PopulateRemoteObject = function () {
            newScope.RemoteObjectCollection = newScope.FormLinkModel.RemoteObjectCollection;
            if (newScope.RemoteObjectCollection && newScope.RemoteObjectCollection.length > 0) {
                if (newScope.RemoteObjectCollection[0].dictAttributes.ID != "") {
                    newScope.RemoteObjectCollection.splice(0, 0, { dictAttributes: { ID: "" } });
                }
                newScope.onRemoteObjectChange();
            }
            if (newScope.SelectedNewMethod) {
                newScope.populateSrvParamtersForNew(newScope.SelectedNewMethod);
            } else {
                newScope.SelectedNewMethod = { Name: 'callmethods', Value: '', dictAttributes: { sfwMode: "New" }, Elements: [] };
            }
            if (newScope.SelectedUpdateMethod) {
                newScope.populateSrvParamtersForUpdate(newScope.SelectedUpdateMethod);
            } else {
                newScope.SelectedUpdateMethod = { Name: 'callmethods', Value: '', dictAttributes: { sfwMode: "Update" }, Elements: [] };
            }
        };

        newScope.onRemoteObjectChange = function () {
            newScope.isReset = true;
            if (newScope.FormLinkModel.dictAttributes.sfwRemoteObject != undefined && newScope.FormLinkModel.dictAttributes.sfwRemoteObject != "") {
                function filterRemoteObj(itm) {
                    return itm.dictAttributes.ID == newScope.FormLinkModel.dictAttributes.sfwRemoteObject;
                }
                var lst = newScope.RemoteObjectCollection.filter(filterRemoteObj);
                if (lst && lst.length > 0) {
                    newScope.SrvMethodCollection = lst[0].Elements;
                    newScope.SrvNewMethodCollection = lst[0].Elements.filter(function (itm) { return itm.dictAttributes.sfwMode == "New" || itm.dictAttributes.sfwMode == "All" });
                    newScope.SrvUpdateMethodCollection = lst[0].Elements.filter(function (itm) { return itm.dictAttributes.sfwMode == "Update" || itm.dictAttributes.sfwMode == "All" });
                    newScope.SrvMethodCollection.splice(0, 0, { dictAttributes: { ID: '' } });
                }

            }
            else {
                newScope.SrvNewMethodCollection = [];
                newScope.SrvUpdateMethodCollection = [];
                newScope.SrvMethodCollection = [];
            }
        };

        newScope.onMethodTypeChange = function () {
            newScope.PopulateRemoteObject();
            if (newScope.MainQuery && newScope.MainQuery.dictAttributes) {
                newScope.MainQuery.dictAttributes.sfwMethodName = "";
            }
            if (newScope.SelectedNewMethod) {
                newScope.SelectedNewMethod.dictAttributes.sfwMethodName = "";
            }
            if (newScope.SelectedUpdateMethod) {
                newScope.SelectedUpdateMethod.dictAttributes.sfwMethodName = "";
            }

            newScope.FormLinkModel.dictAttributes.sfwRemoteObject = "";

            newScope.FormLinkModel.dictAttributes.sfwGridCollection = "";


            newScope.paramtersForNewMethod = [];


            newScope.paramtersForUpdateMethod = [];


            newScope.SrvMethodCollection = [];


            newScope.SrvNewMethodCollection = [];


            newScope.SrvUpdateMethodCollection = [];

        };

        newScope.selectXmlMethodClick = function (aXmlMethodID) {
            if (aXmlMethodID && aXmlMethodID != "" && aXmlMethodID != "NewObject") {
                //objNewDialog.close();
                newScope.OkClick();
                $NavigateToFileService.NavigateToFile(newScope.FormLinkModel.dictAttributes.sfwEntity, "methods", aXmlMethodID);
            }
        };

        //#endregion

        //#region Main Query Methods
        newScope.openMainQueryDialog = function () {
            var newQueryScope = newScope.$new();
            newQueryScope.strSelectedQuery = newScope.MainQuery.dictAttributes.sfwQueryRef;

            newQueryScope.QueryDialog = $rootScope.showDialog(newQueryScope, "Browse For Query", "Form/views/BrowseForQuery.html", { width: 1000, height: 700 });

            newQueryScope.$on('onQueryClick', function (event, data) {
                if (!$scope.IsSubQuery(data)) {
                    if (data.contains('.')) {
                        newScope.MainQuery.dictAttributes.ID = data.split('.')[0];
                    }

                    newScope.MainQuery.dictAttributes.sfwQueryRef = data;
                }
                if (event) {
                    event.stopPropagation();
                }
            });

        };

        newScope.onMainQueryChange = function (data) {
            newScope.updateMainQueryChange();
            if (!$scope.IsSubQuery(data)) {
                if (!newScope.MainQuery) {
                    newScope.MainQuery = {
                        Name: 'query', Value: '', dictAttributes: {}, Elements: []
                    };
                    newScope.InitialLoad.Elements.push(newScope.MainQuery);
                }
                if (data && data.contains('.')) {
                    if (!newScope.MainQuery.dictAttributes.ID) {
                        newScope.MainQuery.dictAttributes.ID = data.split('.')[0];
                        newScope.MainQueryIDChange(newScope.MainQuery);
                    }
                }
                else if (!data) {
                    newScope.MainQuery.dictAttributes.ID = "";
                    newScope.MainQueryIDChange(newScope.MainQuery);
                }
            }
        };

        newScope.MainQueryIDChange = function (aobjMainQuery) {
            if (newScope.InitialLoad && newScope.InitialLoad.Elements) {
                for (var i = 0; i < newScope.InitialLoad.Elements.length; i++) {
                    if (newScope.InitialLoad.Elements[i].dictAttributes && newScope.InitialLoad.Elements[i].dictAttributes.sfwQueryRef == aobjMainQuery.dictAttributes.sfwQueryRef) {
                        newScope.InitialLoad.Elements[i].dictAttributes.ID = aobjMainQuery.dictAttributes.ID;
                    }
                }
            }
        };

        newScope.updateMainQueryChange = function () {
            if (newScope.InitialLoad && newScope.InitialLoad.Elements) {
                for (var i = 0; i < newScope.InitialLoad.Elements.length; i++) {
                    if (newScope.InitialLoad.Elements[i].dictAttributes && newScope.InitialLoad.Elements[i].dictAttributes.ID == newScope.MainQuery.dictAttributes.ID) {
                        newScope.InitialLoad.Elements[i].dictAttributes.sfwQueryRef = newScope.MainQuery.dictAttributes.sfwQueryRef;
                    }
                }
            }
        };

        newScope.onSubQueryClick = function (obj) {
            if (obj) {
                newScope.SelectedSubQuery = obj;
            }
        };

        //#endregion

        //#region Group functionality Implemented
        newScope.getGroupList = function (event) {
            var input = $(event.target);
            var lstGroupList = [];
            lstGroupList = createGroupList();
            if (lstGroupList.length > 0 && lstGroupList[0].Elements.length > 0) {
                var data = lstGroupList[0].Elements;
                if (event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE) {
                    if ($(input).data('ui-autocomplete')) {
                        $(input).autocomplete("search", $(input).val());
                    }
                    event.preventDefault();
                }
                else {
                    setSingleLevelAutoComplete(input, data, newScope, "ID");
                }
            }
            else {
                if (event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE) {
                    event.preventDefault();
                }
            }
        };

        newScope.showGroupList = function (event) {
            var inputElement = $(event.target).prevAll("input[type='text']");
            var lstGroupList = [];

            lstGroupList = createGroupList();
            if (lstGroupList.length > 0 && lstGroupList[0].Elements.length > 0) {
                var data = lstGroupList[0].Elements;
                inputElement.focus();

                setSingleLevelAutoComplete(inputElement, data, newScope, "ID");
                if ($(inputElement).data('ui-autocomplete')) $(inputElement).autocomplete("search", $(inputElement).val());
            }
            else {
                setSingleLevelAutoComplete(inputElement, [], newScope, "ID");
            }

            if (event) {
                event.stopPropagation();
            }
        };

        var createGroupList = function () {
            if (newScope.FormLinkModel.objExtraData) {

                var lstGroupList = newScope.FormLinkModel.objExtraData.lstGroupsList;

                if (lstGroupList && lstGroupList.length > 0) {
                    return lstGroupList;
                } else {
                    lstGroupList = [];
                    return lstGroupList;
                }
            }
        };

        newScope.selectGroupClick = function (aGroupID, property) {
            if ((newScope.FormLinkModel.errors && !newScope.FormLinkModel.errors[property])) {
                if (aGroupID && aGroupID != " ") {
                    objNewDialog.close();
                    $NavigateToFileService.NavigateToFile(newScope.FormLinkModel.dictAttributes.sfwEntity, "groupslist", aGroupID);
                }
            }
        };

        //#endregion


        //#region InitialLoad for CallMethods(Maintenance Entity Tab)

        //#region Populate Xml Methods while changing the Entity Name
        newScope.populateXmlMethodsForEntityChange = function (entityName) {
            newScope.isReset = true;
            var entity = entityName;
            newScope.XmlNewMethodsCollection = [];
            newScope.XmlUpdateMethodsCollection = [];
            newScope.lstLogRules = [];
            newScope.FormLinkModel.objExtraData = {
            };
            if (newScope.FormLinkModel.isSrvMethodChecked == "false") {

                if (newScope.SelectedNewMethod && newScope.SelectedNewMethod.dictAttributes.sfwMethodName) {
                    newScope.SelectedNewMethod.dictAttributes.sfwMethodName = "";
                }
                if (newScope.SelectedUpdateMethod && newScope.SelectedUpdateMethod.dictAttributes.sfwMethodName) {
                    newScope.SelectedUpdateMethod.dictAttributes.sfwMethodName = "";
                }
                newScope.paramtersForUpdateMethod = [];
                newScope.paramtersForNewMethod = [];
            }
            var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
            var lst = entityIntellisenseList.filter(function (x) { return x.ID == entityName });
            //$scope.entityTreeName = entityName;
            if (lst && lst.length > 0) {
                $.connection.hubMain.server.getFormExtraData(entity).done(function (extradata) {
                    newScope.FormLinkModel.objExtraData = extradata;
                    newScope.populateXmlMethods(newScope.FormLinkModel.isSrvMethodChecked);
                    newScope.lstLogRules = newScope.FormLinkModel.objExtraData.lstRulesList;
                    newScope.lstLogRules.splice(0, 0, { dictAttributes: { ID: '' } });
                });
            }
        };

        //#endregion

        //#region Load Parameters for New Method
        newScope.populateParamtersForNew = function (xmlMethod) {
            newScope.paramtersForNewMethod = [];
            if (!newScope.SelectedNewMethod) {
                newScope.SelectedNewMethod = {
                    Name: "callmethods", Value: '', Elements: [], dictAttributes: { sfwMethodName: xmlMethod.dictAttributes.sfwMethodName }
                };
            }
            if (newScope.InitialLoad.Elements.indexOf(newScope.SelectedNewMethod) <= -1) {
                newScope.InitialLoad.Elements.push(newScope.SelectedNewMethod);
            }
            if (xmlMethod.dictAttributes.sfwMethodName && newScope.XmlNewMethodsCollection) {

                var lst = newScope.XmlNewMethodsCollection.filter(function (x) {
                    return x.dictAttributes.ID == xmlMethod.dictAttributes.sfwMethodName && x.dictAttributes.sfwMode != "Update"
                });
                if (lst && lst.length > 0) {

                    angular.forEach(lst[0].Elements, function (itm) {
                        if (itm.Name == "parameter") {
                            newScope.paramtersForNewMethod.push(itm);
                        }
                    });

                }
            }

            if (newScope.XmlUpdateMethodsCollection) {
                var lst = newScope.XmlUpdateMethodsCollection.filter(function (x) {
                    return xmlMethod.dictAttributes.sfwMethodName && x.dictAttributes.ID == xmlMethod.dictAttributes.sfwMethodName && x.dictAttributes.sfwMode != "New"
                });
                if (lst && lst.length > 0) {
                    newScope.IsSameAsNewDisabled = false;
                }
                else {
                    newScope.IsSameAsNewDisabled = true;
                    newScope.FormLinkModel.IsSameAsNew = false;
                }
            }

            if (!newScope.FormLinkModel.IsSameAsNew && newScope.SelectedNewMethod) {
                newScope.SelectedNewMethod.dictAttributes.sfwMode = "New";
            }
        };

        newScope.populateSrvParamtersForNew = function (srvMethod) {
            if (!newScope.SelectedNewMethod) {
                newScope.SelectedNewMethod = {
                    Name: "callmethods", Value: '', Elements: [], dictAttributes: { sfwMethodName: srvMethod.dictAttributes.sfwMethodName }
                };
            }
            if (newScope.InitialLoad.Elements.indexOf(newScope.SelectedNewMethod) <= -1) {
                newScope.InitialLoad.Elements.push(newScope.SelectedNewMethod);
            }
            newScope.paramtersForNewMethod = [];

            if (srvMethod.dictAttributes.sfwMethodName && newScope.SrvNewMethodCollection) {

                var lst = newScope.SrvNewMethodCollection.filter(function (x) {
                    return x.dictAttributes.ID == srvMethod.dictAttributes.sfwMethodName && x.dictAttributes.sfwMode != "Update"
                });
                if (lst && lst.length > 0) {

                    angular.forEach(lst[0].Elements, function (itm) {
                        if (itm.Name == "parameter") {
                            newScope.paramtersForNewMethod.push(itm);
                        }
                    });

                }
            }
            if (newScope.SrvUpdateMethodCollection) {
                var lst = newScope.SrvUpdateMethodCollection.filter(function (x) { return x.dictAttributes.ID == srvMethod.dictAttributes.sfwMethodName && x.dictAttributes.sfwMode != "New" });
                if (lst && lst.length > 0) {
                    newScope.IsSameAsNewDisabled = false;
                }
                else {
                    newScope.IsSameAsNewDisabled = true;
                    newScope.FormLinkModel.IsSameAsNew = false;
                }
            }
            if (!newScope.FormLinkModel.IsSameAsNew && newScope.SelectedNewMethod) {
                newScope.SelectedNewMethod.dictAttributes.sfwMode = "New";
            }
        };

        //#endregion

        //#region Load Parameters for Update Method
        newScope.populateParamtersForUpdate = function (xmlMethod) {
            newScope.paramtersForUpdateMethod = [];
            if (!newScope.SelectedUpdateMethod) {
                newScope.SelectedUpdateMethod = {
                    Name: "callmethods", Value: '', Elements: [], dictAttributes: { sfwMethodName: xmlMethod.dictAttributes.sfwMethodName, sfwMode: "Update" }
                };
            }
            if (newScope.InitialLoad.Elements.indexOf(newScope.SelectedUpdateMethod) <= -1) {
                newScope.InitialLoad.Elements.push(newScope.SelectedUpdateMethod);
            }
            xmlMethod.dictAttributes.sfwMode = "Update";
            newScope.paramtersForUpdateMethod = [];
            if (xmlMethod.dictAttributes.sfwMethodName && newScope.XmlUpdateMethodsCollection) {

                var lst = newScope.XmlUpdateMethodsCollection.filter(function (x) {
                    return x.dictAttributes.ID == xmlMethod.dictAttributes.sfwMethodName && x.dictAttributes.sfwMode != "New"
                });
                if (lst && lst.length > 0) {

                    angular.forEach(lst[0].Elements, function (itm) {
                        if (itm.Name == "parameter") {
                            newScope.paramtersForUpdateMethod.push(itm);
                        }
                    });

                }
            }
        };

        newScope.populateSrvParamtersForUpdate = function (srvMethod) {
            if (!newScope.SelectedUpdateMethod) {
                newScope.SelectedUpdateMethod = {
                    Name: "callmethods", Value: '', Elements: [], dictAttributes: { sfwMethodName: srvMethod.dictAttributes.sfwMethodName, sfwMode: "Update" }
                };
            }
            if (newScope.InitialLoad.Elements.indexOf(newScope.SelectedUpdateMethod) <= -1) {
                newScope.InitialLoad.Elements.push(newScope.SelectedUpdateMethod);
            }
            srvMethod.dictAttributes.sfwMode = "Update";
            newScope.paramtersForUpdateMethod = [];
            if (srvMethod.dictAttributes.sfwMethodName && newScope.SrvUpdateMethodCollection) {

                var lst = newScope.SrvUpdateMethodCollection.filter(function (x) {
                    return x.dictAttributes.ID == srvMethod.dictAttributes.sfwMethodName && x.dictAttributes.sfwMode != "New"
                });
                if (lst && lst.length > 0) {

                    angular.forEach(lst[0].Elements, function (itm) {
                        if (itm.Name == "parameter") {
                            newScope.paramtersForUpdateMethod.push(itm);
                        }
                    });

                }
            }
        };

        //#endregion

        //#endregion

        //#region SubQuery Dialog

        newScope.openSubQueryDialog = function () {
            var newQueryScope = $scope.$new();

            newQueryScope.subQueryType = "SubSelectQuery";

            newQueryScope.QueryDialog = $rootScope.showDialog(newQueryScope, "Browse For Query", "Form/views/BrowseForQuery.html", { width: 1000, height: 700 });


            newQueryScope.$on('onQueryClick', function (event, data) {

                var objSubQuery = {
                    Name: 'query', Value: '', dictAttributes: {}, Elements: [], Children: []
                };


                if (data.contains('.')) {
                    var lstQuery = newScope.InitialLoad.Elements.filter(function (itm) { return itm.dictAttributes.ID && itm.dictAttributes.ID.contains(data.split('.')[0]) });
                    if (lstQuery && lstQuery.length > 0) {
                        objSubQuery.dictAttributes.ID = GetInitialLoadQueryID(data.split('.')[0], newScope.InitialLoad.Elements, 1);
                    }
                    else {
                        objSubQuery.dictAttributes.ID = GetInitialLoadQueryID(data.split('.')[0], newScope.SubQueryCollection.Elements, 1);
                    }
                }
                objSubQuery.dictAttributes.sfwQueryRef = data;
                if (newScope.InitialLoad) {
                    newScope.InitialLoad.Elements.push(objSubQuery);
                    newScope.SubQueryCollection.Elements.push(objSubQuery);

                    newScope.SelectedSubQuery = objSubQuery;
                }
                if (event) {
                    event.stopPropagation();
                }
            });

        };

        newScope.onChangeSubQuery = function (QueryID) {
            if (newScope.SelectedSubQuery) {
                if (QueryID && QueryID.contains('.')) {
                    var strQueryId = newScope.GetQueryId(QueryID.split('.')[0]);
                    newScope.SelectedSubQuery.dictAttributes.ID = strQueryId;
                }
            }
        };

        newScope.GetQueryId = function (QueryId) {
            var iItemNum = 0;
            var strItemKey = QueryId;

            var strItemName = strItemKey;
            if (newScope.InitialLoad) {
                var newTemp = newScope.InitialLoad.Elements.filter(function (x) {
                    return x.dictAttributes.ID == strItemName;
                });

                while (newTemp && newTemp.length > 0) {
                    iItemNum++;
                    strItemName = strItemKey + iItemNum;
                    newTemp = newScope.InitialLoad.Elements.filter(function (x) {
                        return x.dictAttributes.ID == strItemName;
                    });
                }
            }
            return strItemName;
        };

        //#endregion

        //#region Edit Query
        newScope.openEditQueryDialog = function () {
            var newQueryScope = newScope.$new();
            if (newScope.SelectedSubQuery) {
                newQueryScope.strSelectedQuery = newScope.SelectedSubQuery.dictAttributes.sfwQueryRef;
                newQueryScope.subQueryType = "SubSelectQuery";
                newQueryScope.QueryDialog = $rootScope.showDialog(newQueryScope, "Browse For Query", "Form/views/BrowseForQuery.html", { width: 1000, height: 700 });

                newQueryScope.$on('onQueryClick', function (event, data) {
                    if (data.contains('.')) {
                        newScope.SelectedSubQuery.dictAttributes.ID = data.split('.')[0];
                    }
                    newScope.SelectedSubQuery.dictAttributes.sfwQueryRef = data;
                });
                if (event) {
                    event.stopPropagation();
                }
            }
        };

        //#endregion

        //#region delete Subquery Item
        newScope.deleteSelectedSubQuery = function () {
            if (newScope.SelectedSubQuery) {
                var Fieldindex = -1;

                Fieldindex = newScope.SubQueryCollection.Elements.indexOf(newScope.SelectedSubQuery);
                if (Fieldindex > -1) {
                    newScope.SubQueryCollection.Elements.splice(Fieldindex, 1);
                }

                var index = -1;
                if (newScope.SelectedSubQuery) {
                    index = newScope.InitialLoad.Elements.indexOf(newScope.SelectedSubQuery);
                }
                if (index > -1) {
                    newScope.InitialLoad.Elements.splice(index, 1);
                }
                newScope.SelectedSubQuery = undefined;
                $timeout(function () {
                    if (Fieldindex < newScope.SubQueryCollection.Elements.length) {
                        newScope.SelectedSubQuery = newScope.SubQueryCollection.Elements[Fieldindex];
                    }
                    else if (newScope.SubQueryCollection.Elements.length > 0) {
                        newScope.SelectedSubQuery = newScope.SubQueryCollection.Elements[Fieldindex - 1];
                    }
                });
            }
        };

        // disable if there is no element for SFW row
        newScope.canDelete = function () {
            if (newScope.SelectedSubQuery && newScope.SubQueryCollection.Elements.length > 0) {
                return true;
            }
            else {
                return false;
            }
        };
        //#endregion

        //#region Move up for Subquery
        newScope.moveUpClick = function () {
            if (newScope.SelectedSubQuery) {
                var index = newScope.SubQueryCollection.Elements.indexOf(newScope.SelectedSubQuery);
                var item = newScope.SubQueryCollection.Elements[index - 1];
                newScope.SubQueryCollection.Elements[index - 1] = newScope.SelectedSubQuery;
                newScope.SubQueryCollection.Elements[index] = item;
                $scope.scrollBySelectedField(".manage-subquery-scroll", ".selected", { offsetTop: 460, offsetLeft: 0 });
            }
        };

        // disable the move up button if there is no element to move up
        newScope.canmoveUp = function () {
            newScope.Flag = true;
            if (newScope.SelectedSubQuery != undefined) {
                for (var i = 0; i < newScope.SubQueryCollection.Elements.length; i++) {
                    if (newScope.SubQueryCollection.Elements[i] == newScope.SelectedSubQuery) {
                        if (i > 0) {
                            newScope.Flag = false;
                        }
                    }
                }
            }

            return newScope.Flag;
        };

        //#endregion

        //#region Move down for Sub query    

        newScope.moveDownClick = function () {
            if (newScope.SelectedSubQuery) {
                var index = newScope.SubQueryCollection.Elements.indexOf(newScope.SelectedSubQuery);
                var item = newScope.SubQueryCollection.Elements[index + 1];
                newScope.SubQueryCollection.Elements[index + 1] = newScope.SelectedSubQuery;
                newScope.SubQueryCollection.Elements[index] = item;
                $scope.scrollBySelectedField(".manage-subquery-scroll", ".selected", { offsetTop: 450, offsetLeft: 0 });
            }
        };


        // disable move down when there is no element to move down
        newScope.canMoveDown = function () {
            newScope.Flag = true;
            if (newScope.SelectedSubQuery != undefined) {
                for (var i = 0; i < newScope.SubQueryCollection.Elements.length; i++) {
                    if (newScope.SubQueryCollection.Elements[i] == newScope.SelectedSubQuery) {
                        if (i < newScope.SubQueryCollection.Elements.length - 1) {
                            newScope.Flag = false;
                        }
                    }
                }

            }

            return newScope.Flag;
        };
        //#endregion

        //#region Default Button functionality 
        newScope.getButtonList = function (event) {
            if (newScope.lstButtons && newScope.lstButtons.length > 0) {
                if (!newScope.inputElement) {
                    newScope.inputElement = $(event.target);
                }

                if (event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE) {
                    setSingleLevelAutoComplete($(newScope.inputElement), newScope.lstButtons, newScope, "ID");
                    if ($(newScope.inputElement).data('ui-autocomplete')) $(newScope.inputElement).autocomplete("search", $(newScope.inputElement).val());
                    event.preventDefault();
                }
                setSingleLevelAutoComplete($(newScope.inputElement), newScope.lstButtons, newScope, "ID");
            }
        };

        newScope.showButtonList = function (event) {
            if (!newScope.inputElement) {
                newScope.inputElement = $(event.target).prevAll("input[type='text']");
            }
            newScope.inputElement.focus();
            if (newScope.inputElement) {
                setSingleLevelAutoComplete(newScope.inputElement, newScope.lstButtons, newScope, "ID");
                if ($(newScope.inputElement).data('ui-autocomplete')) $(newScope.inputElement).autocomplete("search", $(newScope.inputElement).val());
            }
            if (event) {
                event.stopPropagation();
            }
        };

        newScope.validateButtonName = function () {
            var list = [];
            if (newScope.lstButtons) {
                angular.forEach(newScope.lstButtons, function (btnObj) {
                    if (btnObj.dictAttributes && btnObj.dictAttributes.ID) {
                        list.push(btnObj.dictAttributes.ID);
                    }
                });
            }
            $ValidationService.checkValidListValue(list, newScope.FormLinkModel, $(newScope.inputElement).val(), "sfwDefaultButtonID", "sfwDefaultButtonID", CONST.VALIDATION.BUTTON_NOT_EXISTS, $scope.validationErrorList);
        };
        //#endregion

        //#region Session Mathods
        //#region adding session fields and object fields

        newScope.CheckAndAddSessionFields = function () {
            if (newScope.InitialLoad) {
                if (!newScope.InitialLoad.Elements.some(function (itm) { return itm.Name == "session" })) {
                    newScope.InitialLoad.Elements.push(newScope.SessionFields);
                }
            }
        };

        newScope.addSessionFields = function () {
            newScope.CheckAndAddSessionFields();
            var lst = [];
            lst = GetSelectedFieldList(newScope.SessionFields.lstselectedobjecttreefields, lst)//GetSelectedFieldList($scope.SessionFields.lstEntity[0].Attributes, lst);
            if (lst && lst.length > 0) {

                angular.forEach(lst, function (item) {
                    var DisplayedEntity = getDisplayedEntity(newScope.SessionFields.LstDisplayedEntities);
                    var itempath = item.ID;
                    if (DisplayedEntity && DisplayedEntity.strDisplayName != "") {
                        itempath = DisplayedEntity.strDisplayName + "." + item.ID;
                    }
                    if (item.IsSelected.toLowerCase() == "true") {
                        if (!newScope.SessionFields.Elements.some(function (x) { return x.dictAttributes.ID == item.ID })) {
                            var strField = itempath;// GetItemPathForEntityObject(item);
                            var objField = {
                                Name: 'field', Value: '', dictAttributes: { ID: item.ID, sfwEntityField: strField }, Elements: []
                            };
                            newScope.SessionFields.Elements.push(objField);
                        }
                        item.IsRecordSelected = false;
                    }
                });
            }
            if (newScope.SessionFields.lstselectedobjecttreefields && newScope.SessionFields.lstselectedobjecttreefields.length > 0) {
                ClearSelectedFieldList(newScope.SessionFields.lstselectedobjecttreefields);
            }
        };

        //#endregion

        //#region click on selected sessionfield row
        newScope.selectedSessionFieldClick = function (obj) {
            if (obj) {
                newScope.selectedCurrentSessionRow = obj;
            }
        };

        //#endregion

        //#region Delete for  Session Fields

        // delete selected column details
        newScope.deleteSelectedRow = function () {
            var Fieldindex = -1;
            if (newScope.selectedCurrentSessionRow) {
                Fieldindex = newScope.SessionFields.Elements.indexOf(newScope.selectedCurrentSessionRow);
            }
            newScope.SessionFields.Elements.splice(Fieldindex, 1);
            newScope.selectedCurrentSessionRow = undefined;

            if (Fieldindex < newScope.SessionFields.Elements.length) {

                newScope.selectedCurrentSessionRow = newScope.SessionFields.Elements[Fieldindex];
            }
            else if (newScope.SessionFields.Elements.length > 0) {
                newScope.selectedCurrentSessionRow = newScope.SessionFields.Elements[Fieldindex - 1];
            }

        };

        // disable if there is no element for SFW row
        newScope.canDeleteRow = function () {
            if (newScope.selectedCurrentSessionRow) {
                return true;
            }
            else {
                return false;
            }
        };
        //#endregion


        //#region  Move up for session Fields
        // Move up functionality for Row from Record Layout
        newScope.moveSelectedRowUp = function () {
            if (newScope.selectedCurrentSessionRow) {
                var index = newScope.SessionFields.Elements.indexOf(newScope.selectedCurrentSessionRow);
                var index = newScope.SessionFields.Elements.indexOf(newScope.selectedCurrentSessionRow);
                var item = newScope.SessionFields.Elements[index - 1];
                newScope.SessionFields.Elements[index - 1] = newScope.selectedCurrentSessionRow;
                newScope.SessionFields.Elements[index] = item;
                $scope.scrollBySelectedField(".details-session-panel-body", ".selected", { offsetTop: 300, offsetLeft: 0 });
            }
        };

        // disable the move up button if there is no element to move up
        newScope.canmoveSelectedRowUp = function () {
            var Flag = true;
            if (newScope.selectedCurrentSessionRow != undefined) {
                for (var i = 0; i < newScope.SessionFields.Elements.length; i++) {
                    if (newScope.SessionFields.Elements[i] == newScope.selectedCurrentSessionRow) {
                        if (i > 0) {
                            Flag = false;
                        }
                    }
                }

            }

            return Flag;
        };
        //#endregion


        //#region Move down for session fields
        // Move Down function for Row from Record Layout

        newScope.moveSelectedRowDown = function () {
            if (newScope.selectedCurrentSessionRow) {
                var index = newScope.SessionFields.Elements.indexOf(newScope.selectedCurrentSessionRow);
                var index = newScope.SessionFields.Elements.indexOf(newScope.selectedCurrentSessionRow);
                var item = newScope.SessionFields.Elements[index + 1];
                newScope.SessionFields.Elements[index + 1] = newScope.selectedCurrentSessionRow;
                newScope.SessionFields.Elements[index] = item;
                $scope.scrollBySelectedField(".details-session-panel-body", ".selected", { offsetTop: 300, offsetLeft: 0 });
            }
        };


        // disable move down when there is no element to move down
        newScope.canmoveSelectedRowDown = function () {
            var Flag = true;
            if (newScope.selectedCurrentSessionRow != undefined) {
                for (var i = 0; i < newScope.SessionFields.Elements.length; i++) {
                    if (newScope.SessionFields.Elements[i] == newScope.selectedCurrentSessionRow) {
                        if (i < newScope.SessionFields.Elements.length - 1) {
                            Flag = false;
                        }
                    }
                }

            }

            return Flag;
        };

        //#endregion

        //#endregion

        //#region Call Init Method
        newScope.InitialLoadSectionForDetail();
        //newScope.Init();

        //#endregion

        //#region Select Tab
        newScope.OnSelectDetailTab = function (tabName) {
            newScope.selectedDetailTab = tabName;
            if (tabName == 'Entity') {
                if (!newScope.IsEntityTabSelected) {
                    newScope.IsEntityTabSelected = true;
                }
            }
            if (tabName == 'Group') {
                if (!newScope.IsGroupTabSelected) {
                    newScope.IsGroupTabSelected = true;
                }
            }
            if (tabName == 'Session') {
                if (!newScope.IsSessionTabSelected) {
                    newScope.IsSessionTabSelected = true;
                }
            }
            if (tabName == 'ExtraFields') {
                if (!newScope.IsExtraFieldTabSelected) {
                    newScope.IsExtraFieldTabSelected = true;
                }
            }
        };
        //#endregion

        //#region Ok click
        newScope.OkClick = function () {
            objNewDialog.close();


            if (entityname && entityname != newScope.FormLinkModel.dictAttributes.sfwEntity) {
                $scope.entityTreeName = newScope.FormLinkModel.dictAttributes.sfwEntity;
                for (var i = 0; i < $scope.lstLoadedEntityTrees.length; i++) {
                    if ($scope.lstLoadedEntityTrees[i].EntityName == entityname) {
                        $scope.lstLoadedEntityTrees[i].EntityName = newScope.FormLinkModel.dictAttributes.sfwEntity;
                        break;
                    }
                }
            }

            $rootScope.UndRedoBulkOp("Start");
            if (newScope.objDirFunctions.prepareExtraFieldData) {
                newScope.objDirFunctions.prepareExtraFieldData();// calling extraFieldDirective function for getting extra field data
            }
            angular.forEach(newScope.FormLinkModel.dictAttributes, function (val, key) {
                $rootScope.EditPropertyValue($scope.FormLinkModel.dictAttributes[key], $scope.FormLinkModel.dictAttributes, key, val);
                // $scope.FormLinkModel.dictAttributes[key] = val;
            });

            if (newScope.MainQuery && $scope.MainQuery && newScope.MainQuery.dictAttributes.ID != $scope.MainQuery.dictAttributes.ID) {
                var objCriteriaPanel = GetCriteriaPanel($scope.FormLinkModel);
                $scope.setNewQueryIdForLookupControl(objCriteriaPanel, $scope.MainQuery.dictAttributes.ID, newScope.MainQuery.dictAttributes.ID, true);
            }

            if ($scope.FormLinkModel.dictAttributes.sfwType == "FormLinkLookup") {

                if (newScope.MainQuery && !newScope.MainQuery.dictAttributes.sfwQueryRef) {
                    newScope.MainQuery = undefined;
                    newScope.InitialLoad = undefined;
                    $rootScope.EditPropertyValue($scope.MainQuery, $scope, "MainQuery", undefined);
                    $rootScope.EditPropertyValue($scope.SelectedQuery, $scope, "SelectedQuery", undefined);
                }

                if (newScope.MainQuery && newScope.SubQueryCollection.Elements && $scope.SubQueryCollection && $scope.MainQuery) {
                    var objCriteriaPanel = GetCriteriaPanel($scope.FormLinkModel);
                    angular.forEach($scope.SubQueryCollection, function (itemOld) {
                        var IsSubQueryIDChanged = false;
                        var IsSubQueryDeleted = true;
                        var newQueryID = "";
                        if (newScope.SubQueryCollection && newScope.SubQueryCollection.Elements.length > 0) {
                            angular.forEach(newScope.SubQueryCollection.Elements, function (item) {
                                if (itemOld.dictAttributes.ID != item.dictAttributes.ID && itemOld.dictAttributes.sfwQueryRef == item.dictAttributes.sfwQueryRef) {
                                    IsSubQueryIDChanged = true;
                                    newQueryID = item.dictAttributes.ID;
                                    IsSubQueryDeleted = false;
                                }
                                if (itemOld.dictAttributes.ID == item.dictAttributes.ID) {
                                    IsSubQueryDeleted = false;
                                }
                            });
                        }
                        else {
                            IsSubQueryIDChanged = true;
                        }
                        if (IsSubQueryIDChanged) {
                            $scope.setNewQueryIdForLookupControl(objCriteriaPanel, itemOld.dictAttributes.ID, newQueryID);
                        }
                        if (IsSubQueryDeleted) {
                            $scope.setNewQueryIdForLookupControl(objCriteriaPanel, itemOld.dictAttributes.ID, "");
                        }
                    });

                }
                else if ($scope.SubQueryCollection) {
                    var objCriteriaPanel = GetCriteriaPanel($scope.FormLinkModel);
                    angular.forEach($scope.SubQueryCollection, function (item) {
                        if (item.dictAttributes && item.dictAttributes.ID) {
                            $scope.setNewQueryIdForLookupControl(objCriteriaPanel, item.dictAttributes.ID, "");
                        }
                    });
                }
                else if (!newScope.MainQuery && $scope.MainQuery && $scope.MainQuery.dictAttributes && $scope.MainQuery.dictAttributes.ID) {
                    var objCriteriaPanel = GetCriteriaPanel($scope.FormLinkModel);
                    $scope.setNewQueryIdForLookupControl(objCriteriaPanel, $scope.MainQuery.dictAttributes.ID, newScope.MainQuery.dictAttributes.ID, true);
                }
                if (newScope.MainQuery && newScope.InitialLoad) {
                    for (var i = 0; i < newScope.InitialLoad.Elements.length; i++) {
                        if (newScope.InitialLoad.Elements[i].dictAttributes.sfwQueryRef == newScope.MainQuery.dictAttributes.sfwQueryRef) {
                            newScope.InitialLoad.Elements[i] = newScope.MainQuery;
                            break;
                        }
                    }
                    if ($scope.FormLinkModel.Elements.some(function (x) {
                        return x.Name == "initialload"
                    })) {
                        for (var i = 0; i < $scope.FormLinkModel.Elements.length; i++) {
                            if ($scope.FormLinkModel.Elements[i].Name == "initialload") {
                                $rootScope.DeleteItem($scope.FormLinkModel.Elements[i], $scope.FormLinkModel.Elements);
                                $rootScope.InsertItem(newScope.InitialLoad, $scope.FormLinkModel.Elements, i);
                                $scope.FormLinkModel.Elements[i] = newScope.InitialLoad;
                                break;
                            }
                        }
                    }
                    else if (newScope.InitialLoad && !$scope.FormLinkModel.Elements.some(function (x) {
                        return x.Name == "initialload"
                    })) {
                        $rootScope.InsertItem(newScope.InitialLoad, $scope.FormLinkModel.Elements, 0);
                    }
                }
                else {
                    for (var i = 0; i < $scope.FormLinkModel.Elements.length; i++) {
                        if ($scope.FormLinkModel.Elements[i].Name == "initialload") {
                            $rootScope.DeleteItem($scope.FormLinkModel.Elements[i], $scope.FormLinkModel.Elements);
                            break;
                        }
                    }
                }
            }
            else if ($scope.FormLinkModel && ($scope.FormLinkModel.dictAttributes.sfwType == "FormLinkMaintenance" || $scope.FormLinkModel.dictAttributes.sfwType == "FormLinkWizard")) {

                if (newScope.SelectedNewMethod && (newScope.SelectedNewMethod.dictAttributes.sfwMethodName == undefined || newScope.SelectedNewMethod.dictAttributes.sfwMethodName == "")) {
                    var NewMethodIndex = newScope.InitialLoad.Elements.indexOf(newScope.SelectedNewMethod);
                    if (NewMethodIndex > -1) {
                        newScope.InitialLoad.Elements.splice(NewMethodIndex, 1);
                        newScope.SelectedNewMethod = undefined;
                    }
                }
                if (newScope.SelectedUpdateMethod && (newScope.SelectedUpdateMethod.dictAttributes.sfwMethodName == undefined || newScope.SelectedUpdateMethod.dictAttributes.sfwMethodName == "")) {
                    var UpdateMethodIndex = newScope.InitialLoad.Elements.indexOf(newScope.SelectedUpdateMethod);
                    if (UpdateMethodIndex > -1) {
                        newScope.InitialLoad.Elements.splice(UpdateMethodIndex, 1);
                        newScope.SelectedUpdateMethod = undefined;
                    }
                }


                if (!newScope.InitialLoad.Elements.some(function (itm) {
                    return itm.Name == "session"
                })) {
                    if (newScope.SessionFields && newScope.SessionFields.Elements.length > 0) {
                        newScope.InitialLoad.Elements.push(newScope.SessionFields);
                    }
                }
                else {
                    for (var i = 0; i < newScope.InitialLoad.Elements.length; i++) {
                        if (newScope.InitialLoad.Elements[i].Name == "session") {
                            if (newScope.SessionFields && newScope.SessionFields.Elements.length > 0) {
                                newScope.InitialLoad.Elements[i] = newScope.SessionFields;
                            }
                            else {
                                newScope.InitialLoad.Elements.splice(i, 1);
                            }
                            break;
                        }
                    }
                }

                if (!newScope.SelectedNewMethod && !newScope.SelectedUpdateMethod && (!newScope.SessionFields || (newScope.SessionFields && newScope.SessionFields.Elements.length == 0))) {
                    newScope.InitialLoad = undefined;
                }

                if (newScope.InitialLoad && !$scope.FormLinkModel.Elements.some(function (x) {
                    return x.Name == "initialload"
                })) {
                    $rootScope.InsertItem(newScope.InitialLoad, $scope.FormLinkModel.Elements, 0);
                }
                else if (newScope.InitialLoad && $scope.FormLinkModel.Elements.some(function (x) {
                    return x.Name == "initialload"
                })) {
                    for (var i = 0; i < $scope.FormLinkModel.Elements.length; i++) {
                        if ($scope.FormLinkModel.Elements[i].Name == "initialload") {
                            $rootScope.DeleteItem($scope.FormLinkModel.Elements[i], $scope.FormLinkModel.Elements);
                            $rootScope.InsertItem(newScope.InitialLoad, $scope.FormLinkModel.Elements, i);
                            break;
                        }
                    }
                }
                else if (!newScope.InitialLoad) {
                    for (var i = 0; i < $scope.FormLinkModel.Elements.length; i++) {
                        if ($scope.FormLinkModel.Elements[i].Name == "initialload") {
                            $rootScope.DeleteItem($scope.FormLinkModel.Elements[i], $scope.FormLinkModel.Elements);
                            //$scope.FormLinkModel.Elements.splice(i, 1);
                            break;
                        }
                    }
                }

            }

            $rootScope.EditPropertyValue($scope.InitialLoad, $scope, "InitialLoad", newScope.InitialLoad);
            //$scope.InitialLoad = newScope.InitialLoad;
            $scope.InitialLoadSection();

            $rootScope.UndRedoBulkOp("End");

            if ($scope.FormLinkModel.dictAttributes.sfwType == "FormLinkLookup") {
                var isVisible = false;
                if ($scope.FormLinkModel.IsLookupCriteriaEnabled) {
                    isVisible = true;
                }
                $scope.PopulateQueryId(isVisible);
            }
            //  createRuledata();
        };

        $scope.setNewQueryIdForLookupControl = function (aModel, astrOldQueryID, astrNewQueryID, aIsMainQuery) {
            if (aModel) {
                angular.forEach(aModel.Elements, function (objModel) {
                    if (objModel.dictAttributes && objModel.dictAttributes.sfwQueryID && objModel.dictAttributes.sfwQueryID == astrOldQueryID) {
                        if (astrNewQueryID && astrNewQueryID != "") {
                            $rootScope.EditPropertyValue(objModel.dictAttributes.sfwQueryID, objModel.dictAttributes, "sfwQueryID", "");
                            objModel.dictAttributes.sfwQueryID = astrNewQueryID;
                        }
                        else {
                            $rootScope.EditPropertyValue(objModel.dictAttributes.sfwQueryID, objModel.dictAttributes, "sfwQueryID", "");
                            objModel.dictAttributes.sfwQueryID = "";
                            if (!aIsMainQuery) {
                                $rootScope.EditPropertyValue(objModel.dictAttributes.sfwDataField, objModel.dictAttributes, "sfwDataField", "");
                                objModel.dictAttributes.sfwDataField = "";
                            }
                        }
                    }
                    if (objModel.Elements) {
                        $scope.setNewQueryIdForLookupControl(objModel, astrOldQueryID, astrNewQueryID, aIsMainQuery);
                    }

                });
            }
        };

        newScope.validateFormDetails = function () {
            if (!newScope.IsPrototypeDetails) {
                newScope.FormDetailsErrorMessage = "";
                if (newScope.objDirFunctions.getExtraFieldData) {
                    newScope.objExtraFields = newScope.objDirFunctions.getExtraFieldData(); // getting extra field data from extraFieldDirective
                }

                var flag = validateExtraFields(newScope);
                return flag;
            }

        };

        newScope.NavigateToEntityQuery = function (aQueryID) {
            if (aQueryID && aQueryID != "" && aQueryID.contains(".")) {
                //objNewDialog.close();
                newScope.OkClick();
                var query = aQueryID.split(".");
                $NavigateToFileService.NavigateToFile(query[0], "queries", query[1]);
            };
        };

        newScope.openEntityClick = function (aEntityID) {
            if (aEntityID && aEntityID != "") {
                //objNewDialog.close();
                newScope.OkClick();
                $NavigateToFileService.NavigateToFile(aEntityID, "", "");
            }
        };

        newScope.openQueryEditClick = function (obj, astrTitle) {
            var newQueryScope = newScope.$new();
            newQueryScope.Title = astrTitle;
            newQueryScope.newID = "";
            if (obj && obj.dictAttributes.ID) {
                newQueryScope.newID = obj.dictAttributes.ID;
            }
            newQueryScope.QueryEditDialog = $rootScope.showDialog(newQueryScope, newQueryScope.Title, "Form/views/SetQueryName.html", {
                width: 500, height: 150
            });
            newQueryScope.setQueryName = function () {
                obj.dictAttributes.ID = newQueryScope.newID;
                newQueryScope.closeQueryDialog();
            }

            newQueryScope.closeQueryDialog = function () {
                newQueryScope.QueryEditDialog.close();
            }
        };

        newScope.IsSameAsNewChecked = function (value) {
            newScope.SelectedUpdateMethod = {
                Name: "callmethods", Value: '', Elements: [], dictAttributes: {
                    sfwMethodName: "", sfwMode: "Update"
                }
            };
            if (newScope.FormLinkModel.IsSameAsNew) {
                newScope.paramtersForUpdateMethod = [];
                var indexforupdate = -1;

                angular.forEach(newScope.InitialLoad.Elements, function (objcustommethod) {
                    if (objcustommethod.Name == "callmethods") {

                        if (objcustommethod.dictAttributes.sfwMode == 'New') {
                            objcustommethod.dictAttributes.sfwMode = "";
                        }
                        if (objcustommethod.dictAttributes.sfwMode == 'Update') {
                            indexforupdate = newScope.InitialLoad.Elements.indexOf(objcustommethod);
                        }
                    }

                });
                if (indexforupdate > -1) {
                    newScope.InitialLoad.Elements.splice(indexforupdate, 1);
                    //$rootScope.DeleteItem($scope.InitialLoad.Elements[indexforupdate], $scope.InitialLoad.Elements);
                }
            }
            else {
                var blnfound = false;

                angular.forEach(newScope.InitialLoad.Elements, function (objcustommethod) {
                    if (objcustommethod.Name == "callmethods") {
                        if (objcustommethod.dictAttributes.sfwMode == 'Update') {
                            blnfound = true;
                        }
                        if (!objcustommethod.dictAttributes.sfwMode) {
                            objcustommethod.dictAttributes.sfwMode = 'New';
                        }
                    }

                });

                if (!blnfound) {
                    var acallmethods = {
                        Name: "callmethods", value: '', prefix: "", dictAttributes: {}, Elements: [], Children: []
                    };
                    acallmethods.ParentVM = $scope.InitialLoad;
                    acallmethods.dictAttributes.sfwMode = 'Update';
                    //$rootScope.PushItem(acallmethods, $scope.InitialLoad.Elements);
                    newScope.InitialLoad.Elements.push(acallmethods);
                    newScope.SelectedUpdateMethod = acallmethods;
                }
            }
        };

        newScope.closeDetailDialog = function () {
            objNewDialog.close();
        };
    }

    //#endregion

    //$scope.removeExtraFieldsDataInToMainModel = function () {
    //    if ($scope.objFormHtmlExtraFields) {
    //        var index = $scope.FormLinkModel.Elements.indexOf($scope.objFormHtmlExtraFields);
    //        if (index > -1) {
    //            $scope.FormLinkModel.Elements.splice(index, 1);
    //        }
    //    }
    //}

    $scope.addExtraFieldsDataInToMainModel = function () {
        if ($scope.objFormHtmlExtraFields) {
            var index = $scope.FormLinkModel.Elements.indexOf($scope.objFormHtmlExtraFields);
            if (index == -1) {
                $scope.FormLinkModel.Elements.push($scope.objFormHtmlExtraFields);
            }
        }
    };

    $scope.BeforeSaveToFile = function () {
        $scope.addExtraFieldsDataInToMainModel();
    };

    $scope.AfterSaveToFile = function () {
        if ($scope.FormLinkModel && $scope.FormLinkModel.dictAttributes && $scope.FormLinkModel.dictAttributes.sfwType != "FormLinkLookup") {
            $scope.dummyLstLoadDetails = LoadDetails($scope.FormLinkModel, $scope.objLoadDetails, false, $rootScope, true);
        }
    };


    //#region Load Details
    $scope.OpenLoadDetailsDialog = function (IsSave) {
        var newScope = $scope.$new();
        $scope.dummyLstLoadDetails = LoadDetails($scope.FormLinkModel, $scope.objLoadDetails, true, $rootScope, true);
        dialog = $rootScope.showDialog(newScope, "Load Details", "Form/views/LoadDetails.html", {
            width: 500, height: 400
        });
        newScope.onOkClick = function () {

            dialog.close();
        };


    };

  

    //#endregion

    //#region Validate New
    $scope.OpenValidateNewDialog = function () {
        var newScope = $scope.$new();
        newScope.objValidateNew = {};
        newScope.lstButton = [];
        newScope.lstButton = PopulateButtonID($scope.FormLinkModel, newScope.lstButton);

        if ($scope.ValidateNew) {
            angular.copy($scope.ValidateNew, newScope.objValidateNew);
            if (newScope.objValidateNew.Elements.length > 0) {
                for (var j = 0; j < newScope.objValidateNew.Elements.length; j++) {
                    newScope.objValidateNew.Elements[j].IsFieldVisibility = false;
                }
            }
        }
        else {
            newScope.objValidateNew = {
                dictAttributes: {}, Children: [], Elements: [], Name: "validatenew", Value: ""
            };

        }
        dialog = $rootScope.showDialog(newScope, "Set New Validation", "Form/views/ValidateNewDialog.html", {
            width: 1280, height: 450
        });

        newScope.onOkClick = function () {
            if ($scope.ValidateNew) {
                $rootScope.UndRedoBulkOp("Start");
                $rootScope.EditPropertyValue($scope.ValidateNew.Elements, $scope.ValidateNew, "Elements", []);

                if (newScope.objValidateNew && newScope.objValidateNew.Elements) {
                    for (var i = 0; i < newScope.objValidateNew.Elements.length > 0; i++) {
                        $rootScope.PushItem(newScope.objValidateNew.Elements[i], $scope.ValidateNew.Elements);
                    }
                }
                $rootScope.UndRedoBulkOp("End");
            }
            else {
                $rootScope.UndRedoBulkOp("Start");
                $rootScope.PushItem(newScope.objValidateNew, $scope.FormLinkModel.Elements);
                $rootScope.EditPropertyValue($scope.ValidateNew, $scope, "ValidateNew", newScope.objValidateNew);
                $rootScope.UndRedoBulkOp("End");
            }

            dialog.close();
        };
        newScope.onCancelClick = function () {
            dialog.close();
        };

        newScope.GetButtonIntellisense = function (event) {
            if (event.type == 'click') {
                var input = $(event.target).prevAll('input');
                if (input) {
                    $(input).focus();
                    setSingleLevelAutoComplete(input, newScope.lstButton, newScope, "ID");
                    if ($(input).data('ui-autocomplete')) $(input).autocomplete("search", $(input).val());
                }
                event.preventDefault();
            }
            else {
                var input = $(event.target);
                if (event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE && $(input).data('ui-autocomplete')) {
                    $(input).autocomplete("search", $(input).val());
                    event.preventDefault();
                }
                else {
                    setSingleLevelAutoComplete(input, newScope.lstButton, newScope, "ID");
                }
            }
        };
        newScope.validateButtonID = function (model) {
            var list = [];
            if (newScope.lstButton && newScope.lstButton.length > 0) {
                angular.forEach(newScope.lstButton, function (btn) {
                    if (btn.dictAttributes && btn.dictAttributes.ID) {
                        list.push(btn.dictAttributes.ID);
                    }
                });
            }
            //$ValidationService.checkValidListValue(list, model, model.dictAttributes.ButtonID, "ButtonID", "inValid_id", CONST.VALIDATION.NOT_VALID_ID, $scope.validationErrorList);
        };
        newScope.AddNewValidationRule = function () {
            var objNewRule = {
                dictAttributes: {}, Elements: [], Children: [], Name: "button", Value: ""
            };
            newScope.objValidateNew.Elements.push(objNewRule);
            newScope.objSelectedRule = newScope.objValidateNew.Elements[newScope.objValidateNew.Elements.length - 1];
        };
        newScope.selectedValidateRule = function (obj) {
            newScope.selectedItem = null;
            newScope.objSelectedRule = obj;
        };
        newScope.canDeleteRule = function () {
            if (newScope.objSelectedRule) {
                return true;
            }
            else {
                return false;
            }
        };
        newScope.DeleteValidationRule = function () {
            var index = newScope.objValidateNew.Elements.indexOf(newScope.objSelectedRule);
            if (index > -1) {
                newScope.objValidateNew.Elements.splice(index, 1);
                if (newScope.objValidateNew.Elements.length == 0) {
                    newScope.objSelectedRule = undefined;
                }
                else if (newScope.objValidateNew.Elements.length > index) {
                    newScope.objSelectedRule = newScope.objValidateNew.Elements[index];
                }
                else if (newScope.objValidateNew.Elements.length > 0) {
                    newScope.objSelectedRule = newScope.objValidateNew.Elements[index - 1];
                }
            }
        };
        newScope.ExpandRule = function (objPara) {
            if (!objPara.IsFieldVisibility && newScope.objValidateNew.Elements.length > 0) {
                for (var j = 0; j < newScope.objValidateNew.Elements.length; j++) {
                    newScope.objValidateNew.Elements[j].IsFieldVisibility = false;
                }
            }
            objPara.IsFieldVisibility = !objPara.IsFieldVisibility;
        };
        newScope.selectItem = function (item, obj) {
            newScope.selectedValidateRule(obj);
            newScope.selectedItem = item;
        };
        newScope.canMoveItemsDown = function () {
            if (newScope.selectedItem) {
                var index = newScope.objSelectedRule.Elements.indexOf(newScope.selectedItem);
                if (index < newScope.objSelectedRule.Elements.length - 1) {
                    return true;
                }
                else {
                    return false;
                }
            }
            else {
                return false;
            }
        };
        newScope.canMoveItemsUp = function () {
            if (newScope.selectedItem) {
                var index = newScope.objSelectedRule.Elements.indexOf(newScope.selectedItem);
                if (index <= newScope.objSelectedRule.Elements.length && index > 0) {
                    return true;
                }
                else {
                    return false;
                }
            }
            else {
                return false;
            }
        };
        newScope.canDeleteItems = function () {
            if (newScope.selectedItem) {
                if (newScope.objSelectedRule) {
                    if (newScope.objSelectedRule.Elements.indexOf(newScope.selectedItem) > -1) {
                        return true;
                    }
                }
            }
            else {
                return false;
            }
        };
        newScope.moveItemsDown = function () {
            var index = newScope.objSelectedRule.Elements.indexOf(newScope.selectedItem);
            if (index > -1) {
                var item = newScope.objSelectedRule.Elements[index];
                newScope.objSelectedRule.Elements[index] = newScope.objSelectedRule.Elements[index + 1];
                newScope.objSelectedRule.Elements[index + 1] = item;
            }
        };
        newScope.moveItemsUp = function () {
            var index = newScope.objSelectedRule.Elements.indexOf(newScope.selectedItem);
            if (index > -1) {
                var item = newScope.objSelectedRule.Elements[index];
                newScope.objSelectedRule.Elements[index] = newScope.objSelectedRule.Elements[index - 1];
                newScope.objSelectedRule.Elements[index - 1] = item;
            }
        };
        newScope.deleteItems = function () {
            var index = newScope.objSelectedRule.Elements.indexOf(newScope.selectedItem);
            if (index > -1) {
                newScope.objSelectedRule.Elements.splice(index, 1);
                if (newScope.objSelectedRule.Elements.length == 0) {
                    newScope.selectedItem = undefined;
                }
                else if (newScope.objSelectedRule.Elements.length > index) {
                    newScope.selectedItem = newScope.objSelectedRule.Elements[index];
                }
                else if (newScope.objSelectedRule.Elements.length > 0) {
                    newScope.selectedItem = newScope.objSelectedRule.Elements[index - 1];
                }
            }
        };
        newScope.EditItems = function (objSelectedRule, objItem) {
            newScope.selectedItem = objItem;
            newScope.addItems(objItem, "Update");
        }
        newScope.addItems = function (obj, Flag) {
            var newItemScope = $scope.$new();
            newItemScope.lstControlID = [];
            newItemScope.lstControlID = PopulateControlID($scope.FormLinkModel, newItemScope.lstControlID);
            newItemScope.lstMethodParam = [];
            if (newScope.lstButton.length > 0 && newScope.objSelectedRule.dictAttributes.ButtonID) {
                for (var i = 0; i < newScope.lstButton.length; i++) {
                    if (newScope.lstButton[i].dictAttributes.ID == newScope.objSelectedRule.dictAttributes.ButtonID) {
                        if (newScope.lstButton[i].dictAttributes.sfwNavigationParameter != "" && newScope.lstButton[i].dictAttributes.sfwNavigationParameter != undefined) {
                            var templstNavParam = newScope.lstButton[i].dictAttributes.sfwNavigationParameter.split(";");
                            for (var j = 0; j < templstNavParam.length; j++) {
                                if (templstNavParam[i] != "") {
                                    var tmpPara = templstNavParam[j].split("=");
                                    newItemScope.lstMethodParam.push(tmpPara[0]);
                                }

                            }

                        }
                    }
                }
            }
            if (Flag == "Add") {
                newItemScope.objItem = {
                    dictAttributes: { sfwValidationType: "Required", sfwRequired: 'True' }, Name: "item", Value: "", Children: [], Elements: []
                };
            }
            else {
                newItemScope.objItem = obj;
            }
            newItemScope.lstoperator = ["", "=", "!=", "<", ">", "<=", ">="];
            dialogItem = $rootScope.showDialog(newItemScope, "Add New Items", "Form/views/AddNewItemsInValidationRule.html", { width: 690, height: 420 });
            newItemScope.onOkClick = function (objItemParam) {
                if (Flag == "Add") {
                    $rootScope.PushItem(objItemParam, newScope.objSelectedRule.Elements);
                    //newScope.objSelectedRule.Elements.push();
                }
                dialogItem.close();
            };
            newItemScope.getControlIDIntellisense = function (event) {
                if (event) {
                    if (event.type == 'click') {
                        var input = $(event.target).prevAll('input');
                        if (input) {
                            setSingleLevelAutoComplete(input, newItemScope.lstControlID);
                            if ($(input).data('ui-autocomplete')) $(input).autocomplete("search", $(input).val());
                        }
                        event.preventDefault();
                    }
                    else {
                        var input = $(event.target);
                        if (event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE && $(input).data('ui-autocomplete')) {
                            $(input).autocomplete("search", $(input).val());
                            event.preventDefault();
                        }
                        else {
                            setSingleLevelAutoComplete(input, newItemScope.lstControlID);
                        }
                    }
                }
            };
            newItemScope.validateNewItem = function (obj) {
                if (obj) {
                    var list = newItemScope.lstControlID && newItemScope.lstControlID.length > 0 ? newItemScope.lstControlID : [];
                    //$ValidationService.checkValidListValue(list, obj, obj.dictAttributes.sfwControlID, "sfwControlID", "sfwControlID", CONST.VALIDATION.NOT_VALID_ID, $scope.validationErrorList);
                }
            };
            newItemScope.QueryChange = function () {
                newItemScope.objItem.dictAttributes.sfwParameters = "";
            };
            if (newItemScope.objItem && !newItemScope.objItem.TempQueryParameters) {
                if (newItemScope.objItem.dictAttributes.sfwQueryID != undefined && newItemScope.objItem.dictAttributes.sfwQueryID != "") {
                    newItemScope.objItem.TempQueryParameters = $getQueryparam.get(newItemScope.objItem.dictAttributes.sfwQueryID);
                }
            }
            newItemScope.openQueryParameterDialog = function () {
                var newParamScope = newItemScope.$new();
                newParamScope.objParameter = [];
                newParamScope.lstParameters = newItemScope.lstMethodParam;
                if (newItemScope.objItem.dictAttributes.sfwParameters) {
                    var tempparameter = newItemScope.objItem.dictAttributes.sfwParameters.split(";");
                    for (var i = 0; i < tempparameter.length; i++) {
                        var paraField = tempparameter[i].split("=");
                        if (paraField[0] != "") {
                            newParamScope.objParameter.push({
                                ParameterField: paraField[0], ParameterValue: paraField[1]
                            });
                        }
                    }
                }
                if (newItemScope.objItem.TempQueryParameters) {
                    var lstTempQueryParameter = newItemScope.objItem.TempQueryParameters.split(";");
                    for (var i = 0; i < lstTempQueryParameter.length; i++) {
                        var flag = true;
                        for (var j = 0; j < newParamScope.objParameter.length; j++) {
                            if (newParamScope.objParameter[j].ParameterField == lstTempQueryParameter[i].replace("=", "")) {
                                flag = false;
                                break;
                            }
                        }
                        if (flag && lstTempQueryParameter[i] != "") {
                            newParamScope.objParameter.push({
                                ParameterField: lstTempQueryParameter[i].replace("=", ""), ParameterValue: ""
                            });
                        }
                    }
                }
                dialogParam = $rootScope.showDialog(newParamScope, "Set Query Parameters", "Form/views/SetQueryParameter.html", {
                    width: 550, height: 300
                });
                newParamScope.paraOkClick = function (objItemParam) {
                    var strpaameter = "";
                    for (var i = 0; i < newParamScope.objParameter.length; i++) {
                        if (newParamScope.objParameter[i].ParameterValue && newParamScope.objParameter[i].ParameterValue != "") {
                            strpaameter += newParamScope.objParameter[i].ParameterField + "=" + newParamScope.objParameter[i].ParameterValue + ";";
                        }
                    }
                    newItemScope.objItem.dictAttributes.sfwParameters = strpaameter;
                    dialogParam.close();
                };
                newParamScope.paraCancelClick = function () {
                    dialogParam.close();
                };
            };
        };
    };
    $scope.scrollBySelectedField = function (parentDiv, selectedElement, settings) {
        var $divDom = $(parentDiv);
        if ($divDom && $divDom.hasScrollBar()) {
            $divDom.scrollTo($divDom.find(selectedElement), settings, null);
            return false;
        }
    }
    //#endregion

    $scope.selectElement = function (paramId) {
        if (paramId) {
            $scope.selectedControlId = paramId;
        }
        if ($scope.selectedControlId) {
            var selectedElement = $("#iframe" + $scope.FormLinkModel.dictAttributes.ID).contents().find("#" + $scope.selectedControlId);
            $scope.$evalAsync(function () {
                if ($(selectedElement).length) $(selectedElement).trigger('click');
                $timeout(function () {
                    var iframeBody = $("#iframe" + $scope.FormLinkModel.dictAttributes.ID).filter(":visible").contents().find("body");
                    var elem = iframeBody.find('.selected-button');
                    if (elem.length <= 0) {
                        elem = iframeBody.find('.selected-item');
                    }
                    if (elem.length <= 0) {
                        elem = iframeBody.find(".selected-label");
                        elem = elem && elem.length > 0 ? elem.parent() : elem;
                    }
                    if (elem.length <= 0) {
                        elem = iframeBody.find('.selected-grid');
                    }
                    if (elem.length > 0 && $scope.FormLinkModel.dictAttributes.sfwType == "FormLinkWizard") {
                        $scope.findParentWizardStep(elem, selectedElement);
                    }
                    if (elem.length <= 0){
                        elem = iframeBody.find(".s3-selected-tabsheet");
                        elem = elem && elem.length > 0 ? elem.parent() : elem;
                    }
                    if (elem && elem.length > 0) {
                        $("#iframe" + $scope.FormLinkModel.dictAttributes.ID).contents().find('html, body').animate({ scrollTop: $(elem).offset().top - 200 }, 'slow');
                    }
                });
            });
        }
    };
    $scope.selectElementById = function (obj) {
        if (obj && obj.dictAttributes.ID) {
            $scope.selectedControlId = obj.dictAttributes.ID;
            $scope.selectElement();
        }
    };
    $scope.findParentWizardStep = function (elem, selectedElement) {
        var iframeBody = $("#iframe" + $scope.FormLinkModel.dictAttributes.ID).contents().find('body');
        var elemParents = $(elem).parents();
        var parent = null;
        $(elemParents).each(function () {
            if ($(this).attr("sfwcontroltype") && $(this).attr("sfwcontroltype") == "stepDiv") {
                var id = "#" + $(this).attr("id");
                var domWizardStep = iframeBody.find("a[href=" + id + "]");
                if (domWizardStep && domWizardStep.length) {
                    domWizardStep[0].click();
                    if ($(selectedElement).length) $(selectedElement).trigger('click');
                    parent = $(this);
                    $timeout(function () {
                        parent.animate({ scrollTop: $(elem).offset().top }, 'slow');
                    });
                }
            }
        });
    };

    $scope.LoadNewStyle = function (astrHTMLData) {
        var $iframe = $("#iframe" + $scope.FormLinkModel.dictAttributes.ID);
        var $head = $iframe.contents().find("head");
        var $body = $iframe.contents().find("body");
        if ($scope.selectedPortalDetails && $scope.selectedPortalDetails.PortalPath) {
            $scope.$evalAsync(function () {
                $rootScope.IsLoading = true;
            });
            enableOrDisableCss(true, $head, $body);
            var projectDetails = ConfigurationFactory.getLastProjectDetails();
            $.connection.hubFormLink.server.createStyleForHtx(projectDetails.BaseDirectory, $scope.selectedPortalDetails).done(function (data) {
                $scope.$evalAsync(function () {
                    if (data) {
                        //$head.append('<style type="text/css">' + data[0] + '</style>');

                        var style = $iframe[0].contentWindow.document.createElement("style");
                        style.type = 'text/css';
                        if (style.styleSheet) {
                            style.styleSheet.cssText = data[0];
                        } else {
                            style.appendChild($iframe[0].contentWindow.document.createTextNode(data[0]));
                        }
                        $head[0].appendChild(style);

                        var script = $iframe[0].contentWindow.document.createElement("script");
                        script.type = "text/javascript";
                        script.id = "portalScript";
                        script.innerHTML = data[1];
                        $head[0].appendChild(script);
                        $scope.bindIframeEvents(astrHTMLData);
                        //$iframe[0].contentWindow.document.body.appendChild(script);
                        // $body.append('<script type="text/javascript">' + data[1] + '</script>');
                    }
                    else {
                        $rootScope.IsLoading = false;
                    }
                });

            });
        } else {
            $scope.bindIframeEvents(astrHTMLData);
            enableOrDisableCss(false, $head, $body);
        }
    };

    var enableOrDisableCss = function (diableStyle, $head, $body) {
        $head.find('style').remove();
        $head.find('script#portalScript').remove();
        //$head.find('link[href="css/custom-kendo.css"]').prop('disabled', diableStyle);
        //$head.find('link[href="css/kendo.blueopal.min.css"]').prop('disabled', diableStyle);
        //$head.find('link[href="css/kendo.common.min.css"]').prop('disabled', diableStyle);
        //$head.find('link[href="css/kendo.default.min.css"]').prop('disabled', diableStyle);
        //$head.find('link[href="css/smart_wizard_vertical.css"]').prop('disabled', diableStyle);
    };

    $scope.openProjectSettings = function () {
        var scope = angular.element($('body[ng-controller="MainController"]')).scope();
        if (scope && scope.openConfigureSettings) {
            $rootScope.openFromHtx = true;
            scope.openConfigureSettings(true);
        }
    };
}]);
