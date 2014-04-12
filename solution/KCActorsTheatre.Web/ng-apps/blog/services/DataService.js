blogApp.factory('dataService', function ($http, $q, $log, promises) {
	return {
		getPosts: function ($scope) {
		    return promises.doPost("/Blog/GetPosts", { blogType: $scope.blogType, skip: $scope.posts.length, categoryID: $scope.filteredCategoryID, month: $scope.filteredMonth, year: $scope.filteredYear });
		},
		getPost: function (postID) {
		    return promises.doPost("/Blog/GetPost", { postID: postID });
		},
	};
});