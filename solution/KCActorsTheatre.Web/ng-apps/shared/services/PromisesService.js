baseApp.factory('promises', function ($http, $q, $log, $timeout) {
	
	return {
		watch: function (options) {
			//promise, scope, succeeded, failed, always) {

			if (!angular.isDefined(options.showAlert)) {
				options.showAlert = true;
			}


			function doFail(message, sessionExpired) {
				if (angular.isFunction(options.failed)) {
					options.failed();
				}
				if (options.showAlert) {
					options.scope.showAlert(message, sessionExpired);
				}
			};

			function doAlways() {
				if (angular.isFunction(options.always)) {
					options.always();
				}
			};

			options.promise.then(function (e) {
				if (e.succeeded === false) {
					//ajax call succeeded, but response from server indicates operation failed
					doFail(e.message, e.sessionExpired);
				}
				else {
					//ajax call succeeded, operation succeeded
					if (angular.isFunction(options.succeeded)) {
						options.succeeded(e);
					}
				};
				doAlways();
			}, function (data, args) {
				//500 error from server
				doFail('A 500 Internal Server Error occurred.');
				doAlways();
			})
			;

			return options.promise;
		},

		doPost: function (url, data) {
			var deferred = $q.defer();
			$http({ method: "POST", url: url, data: data }).
			  success(function (data, status, headers, config) {
			  	deferred.resolve(data);
			  }).
			  error(function (data, status, headers, config) {
			  	deferred.reject(status);
			  });

			return deferred.promise;
		}
	};

});