app.service('$getQueryparam', ["$rootScope", "$filter", "$EntityIntellisenseFactory", function ($rootScope, $filter, $EntityIntellisenseFactory) {
    // will give a string like "@param_id=person;@param_id2=org" with input as queryID;
    this.get = function (queryId) {
        var queryparam = "";
        var temp = queryId.split('.');
        if (temp.length > 1) {
            var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
            var list = entityIntellisenseList;
            var queries = $filter('filter')(entityIntellisenseList, { ID: temp[0] }, true);
            if (queries.length == 1) {
                var mainquery = $filter('filter')(queries[0].Queries, { ID: temp[1] }, true);
                // valid query ID
                if (mainquery.length == 1) {
                    var Parameters = mainquery[0].Parameters;
                    // selected query has parameters
                    if (Parameters.length > 0) {
                        var queryParametersDisplay = [];
                        function iterator(value) {
                            this.push(value.ID + '=');
                        }
                        angular.forEach(Parameters, iterator, queryParametersDisplay);
                        queryparam = queryParametersDisplay.join(";") + ";";
                    }
                }
            }
        }
        return queryparam;
    };
    // will return an array for all ids present in the node;
    this.getMapVariableIds = function (ele) {
        var queryParametersValues = [];
        function iteratorqueryParametersValues(value) {
            if (value.dictAttributes && value.dictAttributes.id) {
                this.push(value.dictAttributes.id);
            }
        }
        angular.forEach(ele, iteratorqueryParametersValues, queryParametersValues);
        return queryParametersValues;
    };
    // will return an array of object as {id , value} pair for parameters
    this.getQueryparamfromString = function (mapObj, paramName, splitChar) {
        var queryParameters = [];
        if (mapObj[paramName]) {
            var temp = mapObj[paramName].split(splitChar);
            function iteratorqueryParameters(value) {
                if (value) {
                    var temp1 = value.split("=");
                    this.push({ ID: temp1[0], Value: temp1[1] });
                }
            }
            angular.forEach(temp, iteratorqueryParameters, queryParameters);
        }
        return queryParameters;
    };
    this.getQueryparamfromObjArray = function (arr, idpropname, valuepropname) {
        if (arr && idpropname && valuepropname) {
            var queryParameters = [];
            for (var index = 0; index < arr.length; index++) {
                var obj = { ID: getPropertyValue(arr[index], idpropname), Value: getPropertyValue(arr[index], valuepropname) };
                queryParameters.push(obj);
            }
            return queryParameters;
        }
    };
}]);