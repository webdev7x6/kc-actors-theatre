'use strict';

baseApp.directive('globalNotification', function () {
	return {
		restrict: 'E',
		templateUrl: '/ng-apps/shared/templates/directives/global-notification.html',
		replace: true
	}
});
