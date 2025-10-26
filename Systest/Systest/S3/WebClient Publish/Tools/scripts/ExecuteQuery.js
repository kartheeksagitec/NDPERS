var ExecuteQueryCount = 1;
app.controller('executequerycontroller', ["$scope", "$rootScope", "$interval", function ($scope, $rootScope, $interval) {

    hubMain.server.getExecuteQueryDetails().done(function (data) {

        $scope.$apply(function () {
            $scope.QueryHeader = "Execute Query" + "(" + ExecuteQueryCount++ + ")";
            $scope.QueryDivID = "ExecuteQuery" + ExecuteQueryCount;
            if ($rootScope.IsOracleDB) {
                $scope.lstExecuteQueryTables = [];
                if (data && data.length > 0) {
                    if (data[0].NAME) {
                        for (var i = 0, len = data.length; i < len; i++) {
                            data[i].name = data[i].NAME;
                            //$scope.lstExecuteQueryTables.push({ name: data[i].NAME });
                        }
                    }
                }
            }
            //else {
            $scope.lstExecuteQueryTables = data;
            //}
            $scope.Query = "";
            $scope.TableName = "";
            $scope.chkbxFetchRows = true;
            $scope.setValueToQueryEditor();
        });
    });

    $scope.setValueToQueryEditor = function () {
        if (!$scope.QueryEditor) {
            var divId = "Queryeditor_" + $scope.QueryDivID;
            var ExecuteQuerypromise = $interval(function () {
                if ($("#" + divId).length > 0) {
                    $scope.QueryEditor = ace.edit(divId);
                    $scope.QueryEditor.getSession().setMode("ace/mode/sql");
                    $scope.QueryEditor.resize(true);
                    $scope.QueryEditor.setFontSize(13);
                    $scope.QueryEditor.renderer.setShowGutter(false);
                    $scope.QueryEditor.getSession().setValue($scope.Query);
                    $scope.QueryEditor.getSession().on('change', function (e) {
                        var strQuery = $scope.QueryEditor.getValue();
                        if ($scope.QueryEditor.curOp && $scope.QueryEditor.curOp.command.name) {
                            $scope.Query = strQuery;
                        } else if (strQuery && strQuery != $scope.Query) {
                            //In ace editor if we set value using setValue function change event will fire for that we are checking editor data and current query data
                            $scope.setValueToQueryEditor();
                        }
                    });
                    $interval.cancel(ExecuteQuerypromise);
                }
            }, 500);
        }
        else if ($scope.QueryEditor) {
            $scope.QueryEditor.getSession().setValue($scope.Query);
        }

    };

    $scope.selectedTab = "data";
    $scope.selectTab = function (tabName) {
        $scope.selectedTab = tabName;
    };

    $scope.selectedTable = undefined;
    $scope.selectTable = function (table) {
        $scope.selectedTable = table;
    };
    $scope.onChange = function (val) {
        $scope.chkbxFetchRows = val;
    }
    $scope.ExecutetableQuery = function (btn) {
        var strQuery;
        $scope.btn = btn;

        var returnResult;
        if ($scope.QueryEditor) {
            returnResult = $scope.QueryEditor.getSelectedText();
            if (!returnResult) {
                returnResult = $scope.QueryEditor.getValue();
            }
        }
        if (returnResult) {
            strQuery = returnResult;
        }

        if ($scope.chkbxFetchRows && returnResult && !$rootScope.IsOracleDB) {

            if (returnResult.indexOf("select") > -1) {
                strQuery = [returnResult.slice(0, returnResult.toLowerCase().indexOf("select") + 7), " Top 50 ", returnResult.slice(returnResult.toLowerCase().indexOf("select") + 7)].join('');

            }

        }

        if (strQuery) {
            $rootScope.IsLoading = true;
            hubMain.server.executeQueryTool(strQuery, true, "");
        }
        else {
            $scope.lstExecuteQuery1 = undefined;
        }
    };

    $scope.getQuery = function (caretPositionStart, caretPositionEnd) {
        var line;
        var txtarea;
        var txtQuery = $("div[id='" + $rootScope.currentopenfile.file.FileName + "']").find("#taQuery");
        txtarea = txtQuery[0];

        if (caretPositionStart == caretPositionEnd) {
            //Text Not selected , Only click
            var LineNumber = txtarea.value.substr(0, txtarea.selectionStart).split("\n").length;
            var lines = $("div[id='" + $rootScope.currentopenfile.file.FileName + "']").find("#taQuery").val().split('\n');
            line = lines[LineNumber - 1];
            return line;
        }
        else {
            //Text selected
            line = txtarea.value.substring(caretPositionStart, caretPositionEnd);
            return line.replace("\n", "").replace("\r", "");
        }
    };

    $scope.receieveExecuteQuery = function (data) {
        $scope.$evalAsync(function () {
            $scope.lstdata = JSON.parse(data);
            $rootScope.IsLoading = false;
            if ($scope.btn == 'btn1') {
                $scope.lstExecuteQuery1 = $scope.lstdata;
            }
            else {
                $scope.lstExecuteQuery2 = $scope.lstdata;
            }
        });
    };


    $scope.RunAll = function () {
        if ($scope.Query != undefined && $scope.Query != "") {
            var strQuery = $scope.Query;

            hubMain.server.runAllQueries(strQuery).done(function (data) {
                $scope.$apply(function () {
                    $scope.strErrorMsgdata = data;
                    alert("Complete");
                });
            });
        }
    };



}]);


app.directive('customtabledraggable', [function () {
    return {
        restrict: 'A',
        scope: {
            dragdata: '='
        },
        link: function (scope, element, attributes) {
            var el = element[0];
            if (el.localName == "li") {
                //alert(el.localName);
                //alert(scope.dragdata);
            }
            el.draggable = true;

            el.addEventListener('dragstart', handleDragStart, false);
            function handleDragStart(e) {
                if (scope.dragdata != undefined && scope.dragdata != '') {
                    e.dataTransfer.setData("text", JSON.stringify(scope.dragdata));
                    dragDropData = scope.dragdata;
                }
            }
        },
    };
}]);


app.directive("droptable", [function () {
    return {
        restrict: "A",
        scope: {
            dropdata: '=',
            //  selectedRuleFromGroup: '='
        },
        link: function (scope, element, attributes) {

            var el = element[0];
            el.addEventListener("dragover", DragOver, false);
            el.addEventListener("drop", Drop, false);
            //el.addEventListener('dragleave', DragLeave, false);


            function DragOver(e) {
                e.dataTransfer.dropEffect = 'copy';

                if (scope.dropdata != undefined && scope.dropdata != '') {
                    if (e.preventDefault) {
                        e.preventDefault();
                    }
                }

                return false;

            }
            function Drop(e) {
                //var Scope; 
                //var currentelement = $('div[ng-controller="executequerycontroller"]');
                //if (currentelement && currentelement.length > 0) {
                //    Scope=angular.element(currentelement).scope();
                //}
                var Scope = getCurrentFileScope();
                //var Scope = getCurrentControllerScope();
                e.preventDefault();
                var data = dragDropData;
                Scope.$apply(function () {
                    if (data) {
                        Scope.TableName = data.name;
                        if (Scope.Query == "") {
                            Scope.Query += "select * from" + " " + data.name + ";";
                        }
                        else {
                            Scope.Query += "\r\n" + "select * from" + " " + data.name + ";";
                        }
                        if (Scope.setValueToQueryEditor) {
                            Scope.setValueToQueryEditor();
                        }
                        dragDropData = null;
                    }
                });

                if (e.stopPropagation) e.stopPropagation();
            }

            function DragLeave(e) {

            }

        }
    };
}]);