/// http://jsfiddle.net/zeelux/4p8Vk/5/

(function () {
    'use strict';
    ko.extenders.paging = function(target, pageSize) {
        var _pageSize = ko.observable(pageSize || 10),
            // default pageSize to 10
            _currentPage = ko.observable(1); // default current page to 1

        target.pageSize = ko.computed({
            read: _pageSize,
            write: function(newValue) {
                if (newValue > 0) {
                    _pageSize(newValue);
                }
                else {
                    _pageSize(10);
                }
            }
        });

        target.currentPage = ko.computed({
            read: _currentPage,
            write: function(newValue) {
                if (newValue > target.pageCount()) {
                    _currentPage(target.pageCount());
                }
                else if (newValue <= 0) {
                    _currentPage(1);
                }
                else {
                    _currentPage(newValue);
                }
            }
        });

        target.pageCount = ko.computed(function() {
            return Math.ceil(target().length / target.pageSize()) || 1;
        });

        target.currentPageData = ko.computed(function() {
            var pageSize = _pageSize(),
                pageIndex = _currentPage(),
                startIndex = pageSize * (pageIndex - 1),
                endIndex = pageSize * pageIndex;

            return target().slice(startIndex, endIndex);
        });

        target.pageNumbers = ko.computed(function () {
            var start = 1;
            var end = target.pageCount();

            var windowSize = 5 > target.pageCount() ? target.pageCount() : 5;

            if (windowSize <= 0) {
                return [];
            }

            var startIndex = target.currentPage() - 2;
            var endIndex = target.currentPage() + 2;

            var index;

            if (start > startIndex) {
                index = start;
            } else if (endIndex > end) {
                index = end - windowSize + 1;
            } else {
                index = startIndex;
            }

            var list = [];

            for (var i = index; i < windowSize + index; i++) {
                list.push(i);
            }

            return list;
        });

        target.movePrev = function() {
            target.currentPage(target.currentPage() - 1);
        };
        target.moveNext = function() {
            target.currentPage(target.currentPage() + 1);
        };
        target.movePage = function (i) {
            target.currentPage(i);
        };

        return target;
    };
}());