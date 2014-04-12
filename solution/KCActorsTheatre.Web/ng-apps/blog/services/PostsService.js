blogApp.factory('postsService', function ($timeout, dataService, promises) {
    return {
        getPosts: function ($scope) {
            $scope.startProgress('Loading posts...');
            promises.watch({
                scope: $scope,
                promise: dataService.getPosts($scope),
                succeeded: function (data) {
                    $scope.totalCount = data.totalCount;
                    $scope.$emit('posts-loaded', { posts: data.posts });
                    $scope.endProgress();
                },
                failed: function () {
                    $scope.endProgress();
                }
            });
        },
    }
});