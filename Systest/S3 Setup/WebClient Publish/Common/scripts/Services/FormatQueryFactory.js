app.factory('$FormatQueryFactory', ['hubcontext', '$http', function (hubcontext, $http) {
    var reservewords;
    var item = { sqlReserveWords: reservewords };
    if (hubcontext.hubMain) {
        hubcontext.hubMain.client.setsqlserverreservedwords = function (data) {
            var arrReserveWords = JSON.parse(data);
            item.sqlReserveWords = arrReserveWords;
        };
    }
    return {
        getSqlReserveWords: function () {
            return item.sqlReserveWords;
        },
        formatQuery: function (query) {
            if (query) {
                return $.connection.hubEntityModel.server.formatQuery(query);
            }
        },
        createQueryWithNoLock: function (query) {
            if (query) {
                return $.connection.hubEntityModel.server.createQueryWithNoLock(query);
            }
        }
    };
}]);

