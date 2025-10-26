app.directive("ngScrolldown", [function () {
    return {
        scope: { callback: '&', ngpausescroll: '=', ngscrollheight: '=?' },
        link: function (scope, elements, attrs) {
            var col = $(elements);
            if (scope.ngscrollheight == undefined)
                scope.ngscrollheight = 0;
            $(elements).scroll(function () {
                if (!scope.ngpausescroll) {
                    if (col.outerHeight() + scope.ngscrollheight >= (col.get(0).scrollHeight - col.scrollTop())) {
                        scope.callback({ data: 1 });
                    }
                }
            });
        }
    };
}]);
