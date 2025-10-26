app.filter('entityAttributesbytype', function () {
    return function (attributeList, attrType) {
        return attributeList.filter(function (attr) {
            if (attr && attr.hasOwnProperty("dictAttributes")) {
                if (angular.isArray(attrType)) {
                    if (attrType.indexOf(attr.dictAttributes.sfwType) > -1) {
                        return attr;
                    }
                }
                else {
                    if (attr.dictAttributes.sfwType === attrType) {
                        return attr;
                    }
                }
            }
        });
    };
});