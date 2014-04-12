'use strict';

var blogApp = angular.module('blogApp', ['baseApp'])

.config(['$routeProvider', function ($routeProvider) {
	$routeProvider.when('/', {
	    templateUrl: '/ng-apps/blog/templates/default.html'
	});
	$routeProvider.when('/:postID/:title?', {
		templateUrl: '/ng-apps/blog/templates/post.html',
		controller: 'PostController'
	});
}])

// no longer used... wish there were actual requirements so i don't waste my time with assumptions
.filter('categoryFilter', function () {
    return function (posts, category) {
        if (!posts) {
            return posts;
        }

        if (!category) {
            return posts;
        }

        var returnArray = [];

        // loop through each post
        for (var i = 0; i < posts.length; i++) {
            if (posts[i].postCategories != null) {
                var categoryMatch = false;
                // loop through the post categories in each post
                for (var j = 0; j < posts[i].postCategories.length; j++) {
                    // check if any of the post categories matches the current category
                    if (posts[i].postCategories[j].name == category) {
                        categoryMatch = true;
                        break;
                    }
                }
                if (categoryMatch) {
                    returnArray.push(posts[i]);
                }
            }
        }

        return returnArray;
    }
})

.run(function ($rootScope, $location, $timeout, post, postsService) {

    // scoped variables
	$rootScope.posts = [];
	$rootScope.filteredCategoryID = 0;
	$rootScope.filteredMonth = null;
	$rootScope.filteredYear = null;
	$rootScope.blogType = BLOG_TYPE;
	$rootScope.totalCount = 0;

    // computed variables
	$rootScope.havePosts = function () {
	    return angular.isDefined($rootScope.posts) &&
			angular.isArray($rootScope.posts) &&
			$rootScope.posts.length > 0;
	};
	$rootScope.haveMorePosts = function () {
	    return $rootScope.havePosts &&
            angular.isDefined($rootScope.totalCount) &&
			$rootScope.posts.length < $rootScope.totalCount;
	};
	$rootScope.haveCurrentPost = function () {
	    var retVal = post.hasCurrentPost($rootScope);
	    return retVal;
	};

    // data functions
	$rootScope.getMorePosts = function () {
	    postsService.getPosts($rootScope);
	};
	$rootScope.getByCategory = function () {
	    var categoryID = $('#select-category').val();
	    if (categoryID != $rootScope.filteredCategoryID) {
	        $location.path('/'); // reset to default view in case they were viewing a post
	        $rootScope.posts = [];
	        $rootScope.totalCount = 0;
	        $rootScope.filteredMonth = null;
	        $rootScope.filteredYear = null;
	        $rootScope.filteredCategoryID = categoryID;
	        postsService.getPosts($rootScope);
	    }
	};
	$rootScope.getByMonth = function (month, year) {
	    $rootScope.posts = [];
	    $rootScope.totalCount = 0;
	    $rootScope.filteredCategoryID = 0;
	    $rootScope.filteredMonth = month;
	    $rootScope.filteredYear = year;
	    $location.path('/'); // reset to default view in case they were viewing a post
	    postsService.getPosts($rootScope);
	};

    // internal
	$rootScope.ensureCurrentPost = function (postID) {
	    post.ensureCurrentPost(postID, $rootScope);
	};
	$rootScope.$on('posts-loaded', function (event, args) {
	    if (angular.isDefined(args) && angular.isDefined(args.posts) && angular.isArray(args.posts)) {
	        $.each(args.posts, function (index, post) {
	            $rootScope.posts.push(post);
	        });
		}
	});
	$rootScope.$on('post-loaded', function (event, args) {
		if (angular.isDefined(args) &&
			angular.isDefined(args.currentPost) &&
			angular.isDefined(args.currentPost.postID) &&
			angular.isNumber(args.currentPost.postID) &&
			args.currentPost.postID > 0
			) {
		    post.setCurrentPost.call(post, args.currentPost, $rootScope);
		    if (angular.isDefined(args.isNewPost) && args.isNewPost === true) {
				$timeout(function () {
					$rootScope.posts.push($rootScope.currentPost);
				});
			}
		}
	});
})

;