'use strict';
blogApp.controller('DefaultController',
    function DefaultController($rootScope, $log, postsService, promises) {
        postsService.getPosts($rootScope);
    }
);