var appReminder = angular.module('testApp', []);
appReminder.factory('testFactory', function ($http) {
return {}
});
        
appReminder.controller('testController', function ($scope, $http) {
alert("hai");

$http.get("errorlog.txt").success(function (response) { $scope.employees=response; });
});
