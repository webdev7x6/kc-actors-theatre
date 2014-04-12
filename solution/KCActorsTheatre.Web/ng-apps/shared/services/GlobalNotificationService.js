baseApp.factory('globalNotification', function ($timeout, $log) {
	var minimumDisplayTime = 400,
		init = function (scope) {
			scope.notification = {
				show: false,
				showSpinner: false,
				showDismiss: false,
				message: '',
				timeStarted: null,
				cssClass: '',
				alertClass: 'alert-info',
				spinnerUrl: '/common/img/ajax-loader.gif'
			};
		},
		success = function (scope, message) {
			if (angular.isDefined(message) && angular.isDefined(scope) && angular.isDefined(scope.notification)) {
				$timeout(function () {
					scope.notification.message = message;
					scope.notification.alertClass = 'alert-success';
					scope.notification.showSpinner = false;
					scope.notification.show = true;

					$timeout(function () {
						init(scope);
					}, 1000);
				});

			}
		}
		showNotification = function (scope, message, alertClass) {
			$timeout(function () {
				scope.notification.message = message;
				scope.notification.show = true;
				scope.notification.alertClass = alertClass;
				scope.notification.showSpinner = false;
				scope.notification.showDismiss = true;
				scope.notification.cssClass = 'large';
			});
		},
		showSuccess = function (scope, message) {
			showNotification(scope, message, 'alert-success');
		},
		showInfo = function (scope, message) {
			showNotification(scope, message, 'alert-info');
		},
		showWarning = function (scope, message) {
			showNotification(scope, message, 'alert');
		},
		showError = function (scope, message) {
			showNotification(scope, message, 'alert-error');
		},
		hideNotification = function (scope) {
			scope.notification.message = '';
			scope.notification.alertClass = 'alert-success';
			scope.notification.showSpinner = false;
			scope.notification.show = false;
		}
		;

	return {
		init: init,

		startProgress: function (scope, showSpinner, message) {
			$timeout(function () {
				scope.notification.showSpinner = showSpinner;
				scope.notification.alertClass = 'alert-info';
				scope.notification.spinnerUrl = '/common/img/ajax-loader.gif';
				scope.notification.message = message;
				scope.notification.show = true;
				scope.notification.timeStarted = new Date().getTime();
			});
		},

		endProgress: function (scope, confirmationMessage) {
			var endTime = new Date().getTime(),
				timeDiff = endTime - scope.notification.timeStarted,
				timeout = Math.max(0, minimumDisplayTime - timeDiff);
			;
			//calculating a minimum timeout for display of notification. this ensures the progress message shows for at least a half second
			$timeout(function () {
				if (angular.isDefined(confirmationMessage) && angular.isString(confirmationMessage) && $.trim(confirmationMessage).length > 0) {
					scope.notification.show = false;
					$timeout(function () {
						success(scope, confirmationMessage);
					}, 40);
				} else {
					init(scope);
				}

			}, timeout);
		},
		success: success,
		showNotification: showNotification,
		showSuccess: showSuccess,
		showInfo: showInfo,
		showWarning: showWarning,
		showError: showError,
		hideNotification: hideNotification
	};
});