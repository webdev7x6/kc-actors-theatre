blogApp.factory('post', function ($timeout, dataService, promises) {
	this.currentPostInternal = null;

	this.ensureCurrentPost = function (postID, scope) {
		var $this = this;
		scope.retrievingPost = false;

		if (angular.isString(postID)) {
		    postID = parseInt(postID);
		}

		if (!this.haveValidCurrentPost(scope, postID)) {
		    if (!this.haveValidInternalCurrentPost(postID)) {
				//try to find the post already loaded in memory (in scope.posts array)
				var foundPostInMemory = false;
				if (scope.havePosts()) {
					var thisPost = _.find(scope.posts, function (aPost) {
					    return aPost.postID === postID;
					});
					if (thisPost) {
					    foundPostInMemory = true;
					    scope.$emit('post-loaded', { currentPost: thisPost });
					}
				}

				if (!foundPostInMemory) {
					//need to get new current post if there's not one on the scope that matches our postID and there's not one internally stored that does as well
					scope.startProgress('Retrieving post...');
					scope.retrievingPost = true;
					promises.watch({
						promise: dataService.getPost(postID),
						scope: scope,
						succeeded: function (post) {
							scope.$emit('post-loaded', { currentPost: post });
							scope.endProgress();
							scope.retrievingPost = false;
						},
						failed: function () {
							scope.endProgress();
							$this.setCurrentPost.call($this, null, scope);
							scope.retrievingPost = false;
						}
					});
				}
			}
			else {
				scope.$emit('post-loaded', { currentPost: this.currentPostInternal });
			}
		}
	}

	this.setCurrentPost = function (post, scope) {
		var $this = this;
		$timeout(function () {
			scope.currentPost = post;
			$this.currentPostInternal = post;
		});
	}

	this.haveValidCurrentPost = function (scope, postID) {
		if (angular.isString(postID)) {
			postID = parseInt(postID);
		}

		var haveValidPostOnScope = this.hasCurrentPost(scope) &&
			angular.isDefined(scope.currentPost.postID) &&
			angular.isNumber(scope.currentPost.postID) &&
			scope.currentPost.postID > 0;

		return haveValidPostOnScope && scope.currentPost.postID === postID;
	}

	this.haveValidInternalCurrentPost = function (postID) {
		var haveValidInternalPost =
			angular.isDefined(this.currentPostInternal) &&
			this.currentPostInternal != null &&
			angular.isNumber(this.currentPostInternal.postID) &&
			this.currentPostInternal.postID > 0;

		return this.currentPostInternal !== null && haveValidInternalPost && this.currentPostInternal.postID === postID;
	}

	this.hasCurrentPost = function (scope) {
		return angular.isDefined(scope) && angular.isDefined(scope.currentPost) && scope.currentPost !== null;
	}

	this.hasInternalCurrentPost = function () {
		return angular.isDefined(this.currentPostInternal) && this.currentPostInternal != null;
	}

	return {
		currentPost: this.currentPost,
		ensureCurrentPost: this.ensureCurrentPost,
		haveValidCurrentPost: this.haveValidCurrentPost,
		haveValidInternalCurrentPost: this.haveValidInternalCurrentPost,
		hasCurrentPost: this.hasCurrentPost,
		setCurrentPost: this.setCurrentPost
	};
});