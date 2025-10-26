app.controller("SelectEntitiesDiagram", ["$scope", "$rootScope", "$EntityIntellisenseFactory", "$Chart", function ($scope, $rootScope, $EntityIntellisenseFactory, $Chart) {
    $rootScope.IsLoading = true;
    var lstTempEntity = $EntityIntellisenseFactory.getEntityIntellisense();
    var strString = JSON.stringify(lstTempEntity);
    $scope.lstEntity = JSON.parse(strString);
    $rootScope.IsLoading = false;
   
    $scope.onShowEntityDiagramClick = function () {
        $scope.objEntityDiagram = [];
        $scope.lstCreatedEntity = [];
        $scope.exampleNodes = [];
        $scope.exampleLinks = [];
        var lstSelectedEntity = $scope.lstEntity.filter(function (aobjEntity) { return aobjEntity.IsSelected == true; });
        if (lstSelectedEntity.length > 0) {
            angular.forEach(lstSelectedEntity, function (aobjEntity) {
                $scope.CreateEntityDiagram(aobjEntity, $scope.objEntityDiagram);
            });
        }
        else {
            toastr.warning("Please select entity.");
        }
        angular.forEach($scope.objEntityDiagram, function (aobjEntity) {
            $scope.CreateEntityDiagramLinks(aobjEntity);
        });
        
        //var exampleNodes = JSON.stringify($scope.exampleNodes);
        //var exampleLinks = JSON.stringify($scope.exampleLinks);
        var strFileName = GetNewERDiagramName("ERDiagram", $rootScope.lstopenedfiles, 0);
        $Chart.setData($scope.exampleNodes, $scope.exampleLinks);
        if (lstSelectedEntity.length > 0) {
            var searchFiles = {
                Data: null,
                FileName: strFileName,
                FilePath: "",
                FileType: "ERDiagram",
                TFSState: "None",
                TimeModified: null
            };
            $rootScope.openFile(searchFiles);
            $scope.onEntityDiagramCancelClick();
        }
    };
    function CheckForDuplicateID(strId, alstOpenFile) {
        var blnReturn = false;
        if (alstOpenFile.length > 0) {
            for (var i = 0; i < alstOpenFile.length; i++) {
                if (alstOpenFile[i].file.FileName == strId) {
                    blnReturn = true;
                    break;
                }
            }
        }
        return blnReturn;
    };

    function GetNewERDiagramName(strItemKey, lstOpenFile, iItemNum) {
        var strItemName = String.format("{0}{1}", strItemKey, iItemNum.toString());
        while (CheckForDuplicateID(strItemName, lstOpenFile)) {
            iItemNum++;
            strItemName = String.format("{0}{1}", strItemKey, iItemNum.toString());
        }
        return strItemName;
    };
    $scope.CreateEntityDiagram = function (aobjCurrentEntity, aobjEntityDiagram) {
        var lstIsEntityPresent = $scope.lstCreatedEntity.filter(function (astrEntity) { return astrEntity && astrEntity.toLowerCase() == aobjCurrentEntity.EntityName.toLowerCase() });
        if (lstIsEntityPresent.length == 0) {
            var objResult = { lstOneToOne: [], lstOneToMany: [], entityName: aobjCurrentEntity.EntityName, lstActiveAttributes: [] };
            aobjEntityDiagram.push(objResult);
            var lstAttributes = [];
            angular.forEach(aobjCurrentEntity.Attributes, function (aobjAttribute) {
                if (aobjAttribute.ID && aobjAttribute.Type && (aobjAttribute.Type == "Object" || aobjAttribute.Type == "Collection" || aobjAttribute.Type == "List" || aobjAttribute.Type == "CDOCollection")) {
                    lstAttributes.push(aobjAttribute.ID);
                }
            });
            $scope.exampleNodes.push({ "type": "Entity", id: aobjCurrentEntity.EntityName, name: aobjCurrentEntity.EntityName, attributes: lstAttributes });
            $scope.lstCreatedEntity.push(aobjCurrentEntity.EntityName);
            if (aobjCurrentEntity.Attributes) {
                var lstOneToOneAttributes = aobjCurrentEntity.Attributes.filter(function (aobjAttribute) { return aobjAttribute.Type && aobjAttribute.Type.toLowerCase() == "object" });
                if (lstOneToOneAttributes.length > 0) {
                    angular.forEach(lstOneToOneAttributes, function (aobjOneToOneAttribute) {
                        if (aobjOneToOneAttribute.Entity) {
                            var lstTempEntity = $scope.lstEntity.filter(function (aobjEntityItem) { return aobjEntityItem.EntityName && aobjEntityItem.EntityName == aobjOneToOneAttribute.Entity });
                            if (lstTempEntity.length > 0) {
                                objResult.lstActiveAttributes.push(aobjOneToOneAttribute);
                                var lstIsEntityPresent = $scope.lstCreatedEntity.filter(function (astrEntity) { return astrEntity && astrEntity.toLowerCase() == aobjOneToOneAttribute.Entity.toLowerCase() });
                                if (lstIsEntityPresent.length == 0) {

                                    var objResultOneToOne = { lstOneToOne: [], lstOneToMany: [], entityName: lstTempEntity[0].EntityName, lstActiveAttributes: [] };
                                    objResult.lstOneToOne.push(objResultOneToOne);
                                    var lstOneToOneAttributes = [];
                                    angular.forEach(lstTempEntity[0].Attributes, function (aobjAttribute) {
                                        if (aobjAttribute.ID && aobjAttribute.Type && (aobjAttribute.Type == "Object" || aobjAttribute.Type == "Collection" || aobjAttribute.Type == "List")) {
                                            lstOneToOneAttributes.push(aobjAttribute.ID);
                                        }
                                    });
                                    $scope.exampleNodes.push({ "type": "Entity", id: lstTempEntity[0].EntityName, name: lstTempEntity[0].EntityName, attributes: lstOneToOneAttributes });
                                    $scope.lstCreatedEntity.push(lstTempEntity[0].EntityName);

                                    //$scope.CreateEntityDiagram(lstTempEntity[0], objResult.lstOneToOne);
                                }
                            }
                        }
                    });
                }
                var lstOneToManyAttributes = aobjCurrentEntity.Attributes.filter(function (aobjAttribute) { return aobjAttribute.Type && (aobjAttribute.Type.toLowerCase() == "collection" || aobjAttribute.Type.toLowerCase() == "list" || aobjAttribute.Type.toLowerCase() == "cdocollection") });
                if (lstOneToManyAttributes.length > 0) {
                    angular.forEach(lstOneToManyAttributes, function (aobjOneToManyAttribute) {
                        if (aobjOneToManyAttribute.Entity) {
                            var lstTempEntity = $scope.lstEntity.filter(function (aobjEntityItem) { return aobjEntityItem.EntityName == aobjOneToManyAttribute.Entity });
                            if (lstTempEntity.length > 0) {
                                objResult.lstActiveAttributes.push(aobjOneToManyAttribute);
                                var lstIsEntityPresent = $scope.lstCreatedEntity.filter(function (astrEntity) { return astrEntity && astrEntity.toLowerCase() == aobjOneToManyAttribute.Entity.toLowerCase() });
                                if (lstIsEntityPresent.length == 0) {

                                    var objResultOneToMany = { lstOneToOne: [], lstOneToMany: [], entityName: lstTempEntity[0].EntityName, lstActiveAttributes: [] };
                                    objResult.lstOneToMany.push(objResultOneToMany);
                                    var lstOneToManyAttributes = [];
                                    angular.forEach(lstTempEntity[0].Attributes, function (aobjAttribute) {
                                        if (lstOneToManyAttributes.length < 10 && aobjAttribute.ID && aobjAttribute.Type && (aobjAttribute.Type == "Object" || aobjAttribute.Type == "Collection" || aobjAttribute.Type == "List" || aobjAttribute.Type == "CDOCollection")) {
                                            lstOneToManyAttributes.push(aobjAttribute.ID);
                                        }
                                    });
                                    $scope.exampleNodes.push({ "type": "Entity", id: lstTempEntity[0].EntityName, name: lstTempEntity[0].EntityName, attributes: lstOneToManyAttributes });
                                    $scope.lstCreatedEntity.push(lstTempEntity[0].EntityName);
                                    // $scope.CreateEntityDiagram(lstTempEntity[0], objResult.lstOneToMany);
                                }
                            }
                        }
                    });
                }
            }
        }
        else {
            var createdEntityObject = $scope.GetCreatedEntityObject(aobjCurrentEntity);
            if (aobjCurrentEntity.Attributes && createdEntityObject) {

                var lstOneToOneAttributes = aobjCurrentEntity.Attributes.filter(function (aobjAttribute) { return aobjAttribute.Type && aobjAttribute.Type.toLowerCase() == "object" });
                if (lstOneToOneAttributes.length > 0) {
                    angular.forEach(lstOneToOneAttributes, function (aobjOneToOneAttribute) {
                        if (aobjOneToOneAttribute.Entity) {
                            var lstTempEntity = $scope.lstEntity.filter(function (aobjEntityItem) { return aobjEntityItem.EntityName && aobjEntityItem.EntityName == aobjOneToOneAttribute.Entity });
                            if (lstTempEntity.length > 0) {
                                createdEntityObject.lstActiveAttributes.push(aobjOneToOneAttribute);
                                var lstIsEntityPresent = $scope.lstCreatedEntity.filter(function (astrEntity) { return astrEntity && astrEntity.toLowerCase() == aobjOneToOneAttribute.Entity.toLowerCase() });
                                if (lstIsEntityPresent.length == 0) {

                                    var objResultOneToOne = { lstOneToOne: [], lstOneToMany: [], entityName: lstTempEntity[0].EntityName, lstActiveAttributes: [] };
                                    createdEntityObject.lstOneToOne.push(objResultOneToOne);
                                    var lstOneToOneAttributes = [];
                                    angular.forEach(lstTempEntity[0].Attributes, function (aobjAttribute) {
                                        if (lstOneToOneAttributes.length < 5 && aobjAttribute.ID && aobjAttribute.Type && (aobjAttribute.Type == "Object" || aobjAttribute.Type == "Collection" || aobjAttribute.Type == "List" || aobjAttribute.Type == "CDOCollection")) {
                                            lstOneToOneAttributes.push(aobjAttribute.ID);
                                        }
                                    });
                                    $scope.exampleNodes.push({ "type": "Entity", id: lstTempEntity[0].EntityName, name: lstTempEntity[0].EntityName, attributes: lstOneToOneAttributes });
                                    $scope.lstCreatedEntity.push(lstTempEntity[0].EntityName);

                                    //$scope.CreateEntityDiagram(lstTempEntity[0], objResult.lstOneToOne);
                                }
                            }
                        }
                    });
                }
                var lstOneToManyAttributes = aobjCurrentEntity.Attributes.filter(function (aobjAttribute) { return aobjAttribute.Type && (aobjAttribute.Type.toLowerCase() == "collection" || aobjAttribute.Type.toLowerCase() == "list" || aobjAttribute.Type.toLowerCase() == "cdocollection") });
                if (lstOneToManyAttributes.length > 0) {
                    angular.forEach(lstOneToManyAttributes, function (aobjOneToManyAttribute) {
                        if (aobjOneToManyAttribute.Entity) {
                            var lstTempEntity = $scope.lstEntity.filter(function (aobjEntityItem) { return aobjEntityItem.EntityName == aobjOneToManyAttribute.Entity });
                            if (lstTempEntity.length > 0) {
                                createdEntityObject.lstActiveAttributes.push(aobjOneToManyAttribute);
                                var lstIsEntityPresent = $scope.lstCreatedEntity.filter(function (astrEntity) { return astrEntity && astrEntity.toLowerCase() == aobjOneToManyAttribute.Entity.toLowerCase() });
                                if (lstIsEntityPresent.length == 0) {

                                    var objResultOneToMany = { lstOneToOne: [], lstOneToMany: [], entityName: lstTempEntity[0].EntityName, lstActiveAttributes: [] };
                                    createdEntityObject.lstOneToMany.push(objResultOneToMany);
                                    var lstOneToManyAttributes = [];
                                    angular.forEach(lstTempEntity[0].Attributes, function (aobjAttribute) {
                                        if (aobjAttribute.ID && aobjAttribute.Type && (aobjAttribute.Type == "Object" || aobjAttribute.Type == "Collection" || aobjAttribute.Type == "List" || aobjAttribute.Type == "CDOCollection")) {
                                            lstOneToManyAttributes.push(aobjAttribute.ID);
                                        }
                                    });
                                    $scope.exampleNodes.push({ "type": "Entity", id: lstTempEntity[0].EntityName, name: lstTempEntity[0].EntityName, attributes: lstOneToManyAttributes });
                                    $scope.lstCreatedEntity.push(lstTempEntity[0].EntityName);
                                    // $scope.CreateEntityDiagram(lstTempEntity[0], objResult.lstOneToMany);
                                }
                            }
                        }
                    });
                }
            }
        }
    }

    $scope.CreateEntityDiagramLinks = function (aobjEntity) {
        if (aobjEntity.lstActiveAttributes && aobjEntity.lstActiveAttributes.length > 0) {
            angular.forEach(aobjEntity.lstActiveAttributes, function (aobjAttribute) {
                var objAttribute = { source: aobjEntity.entityName, target: aobjAttribute.Entity, value: 5, type: "" };
                if (aobjAttribute.Type && aobjAttribute.Type == "Object") {
                    objAttribute.type = "onetoone";
                }
                else if (aobjAttribute.Type && (aobjAttribute.Type == "Collection" || aobjAttribute.Type == "List" || aobjAttribute.Type == "CDOCollection")) {
                    objAttribute.type = "onetomany";
                }
                var IsLinkAlreadyPresent = $scope.exampleLinks.filter(function (aobjLink) { return aobjLink.source == objAttribute.source && aobjLink.target == objAttribute.target && aobjLink.type == objAttribute.type });
                if (IsLinkAlreadyPresent.length == 0) {
                    $scope.exampleLinks.push(objAttribute);
                }
            });
            if (aobjEntity.lstOneToOne && aobjEntity.lstOneToOne.length > 0) {
                angular.forEach(aobjEntity.lstOneToOne, function (aobjOneToOneEntity) {
                    $scope.CreateEntityDiagramLinks(aobjOneToOneEntity);
                });
            }
            if (aobjEntity.lstOneToMany && aobjEntity.lstOneToMany.length > 0) {
                angular.forEach(aobjEntity.lstOneToMany, function (aobjOneToManyEntity) {
                    $scope.CreateEntityDiagramLinks(aobjOneToManyEntity);
                });
            }
        }
    }

    $scope.GetCreatedEntityObject = function (aobjEntity) {
        var result=null;
        for (var i = 0; i < $scope.objEntityDiagram.length; i++) {
            if ($scope.objEntityDiagram[i].entityName.toLowerCase() == aobjEntity.EntityName.toLowerCase()) {
                result = $scope.objEntityDiagram[i];
            }
            if (!result && $scope.objEntityDiagram[i].lstOneToOne && $scope.objEntityDiagram[i].lstOneToOne.length > 0) {
                var lstOneToOneCreatedEntity = $scope.objEntityDiagram[i].lstOneToOne;
                for (var j = 0; j < lstOneToOneCreatedEntity.length; j++) {
                    if (lstOneToOneCreatedEntity[j].entityName.toLowerCase() == aobjEntity.EntityName.toLowerCase()) {
                        result = lstOneToOneCreatedEntity[j];
                        break;
                    }
                }
            }
            if (!result && $scope.objEntityDiagram[i].lstOneToMany && $scope.objEntityDiagram[i].lstOneToMany.length > 0) {
                var lstOneToManyCreatedEntity = $scope.objEntityDiagram[i].lstOneToMany;
                for (var k = 0; k < lstOneToManyCreatedEntity.length; k++) {
                    if (lstOneToManyCreatedEntity[k].entityName.toLowerCase() == aobjEntity.EntityName.toLowerCase()) {
                        result = lstOneToManyCreatedEntity[j];
                        break;
                    }
                }
            }
            
        }
        return result;
    };
   
}]);