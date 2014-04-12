'use strict';
var baseApp = angular.module('baseApp', ['ngRoute'])

.config(['$locationProvider', function ($locationProvider) {
    //// configure html5 to get links working on jsfiddle
    //$locationProvider.html5Mode(true); 
}])

// allow html
.filter('unsafe', function ($sce) {
    return function (val) {
        return $sce.trustAsHtml(val);
    };
})

.run(function ($rootScope, $location, globalNotification, $log, $timeout) {
    $rootScope.errorAlert = { message: '' };
    $rootScope.hasErrorAlert = function () {
        return angular.isDefined($rootScope.errorAlert) &&
			angular.isDefined($rootScope.errorAlert.message) &&
			$rootScope.errorAlert.message.length > 0;
    }
    $rootScope.dismissErrorAlert = function () {
        $rootScope.errorAlert.message = '';
    }
    $rootScope.showAlert = function (message, sessionExpired) {
        if (angular.isDefined($rootScope.errorAlert)) {
            $rootScope.errorAlert.message = message;
            ;
        }
        else {
            alert(message);
        }
        if (sessionExpired === true) {
            $timeout(function () {
                window.location.reload();
            }, 2000);
        }
    }

    // global notification
    globalNotification.init($rootScope);
    $rootScope.startProgress = function (message) {
        globalNotification.startProgress($rootScope, true, message);
    }
    $rootScope.endProgress = function (confirmationMessage) {
        globalNotification.endProgress($rootScope, confirmationMessage);
    }
    $rootScope.showSuccess = function (message) {
        globalNotification.showSuccess($rootScope, message);
    }
    $rootScope.showInfo = function (message) {
        globalNotification.showInfo($rootScope, message);
    },
	$rootScope.showWarning = function (message) {
	    globalNotification.showWarning($rootScope, message);
	}
    $rootScope.showError = function (message) {
        globalNotification.showError($rootScope, message);
    }
    $rootScope.hideNotification = function () {
        globalNotification.hideNotification($rootScope);
    }

})

;