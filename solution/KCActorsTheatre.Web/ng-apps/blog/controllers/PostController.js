'use strict';
blogApp.controller('PostController',
    function PostController($scope, $log, $routeParams, $timeout, dataService, promises) {
        $scope.ensureCurrentPost($routeParams.postID);
    }
);