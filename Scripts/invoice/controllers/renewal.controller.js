(function () {
	'use strict';

	angular.module(APPNAME)
        .controller('RenewalController', RenewalController)
        .filter('UTCToNow', function () {
        	return function (input, format) {
        		if (format) {
        			return moment.utc(input).local().format('MM/DD/YY');
        		}
        		else {
        			return moment.utc(input).local();
        		}
        	};
        });


	RenewalController.$inject = ['$scope', '$baseController', '$state', '$adminInvoiceService', '$uibModal', '$stateParams', '$projectService']

	function RenewalController($scope, $baseController, $state, $adminInvoiceService, $uibModal, $stateParams, $projectService) {

		var vm = this;

		$baseController.merge(vm, $baseController);

		// services
		vm.$scope = $scope;
		vm.$state = $state;
		vm.$adminInvoiceService = $adminInvoiceService;
		vm.$projectService = $projectService;
		vm.$uibModal = $uibModal;
		vm.clientId = $stateParams.clientId;
		vm.projectId = $stateParams.projectId;
		vm.printInvoice = _printInvoice;


		// variables 

		//bindable functions


		// function calls
		initialize();


		function initialize() {
		    vm.$projectService.getProjectById(vm.projectId).then(_onGetProjectSuccess, _commonErrorHandler);
		};

		function _printInvoice(invoice) {
		    var innerContents = document.getElementById(invoice).innerHTML;
		    var popupWinindow = window.open('', '_blank', 'width=600,height=700,scrollbars=no,menubar=no,toolbar=no,location=no,status=no,titlebar=no');
		    popupWinindow.document.open();
		    popupWinindow.document.write('<html><head><link href="/Content/bootstrap.min.css" rel="stylesheet"><link href="/Content/unify/main-css/style.css" rel="stylesheet"><link href="/Content/unify/main-css/custom.css" rel="stylesheet"></head><body onload="window.print()">' + innerContents + '</html>');
		    popupWinindow.document.close();
		};

		function _onGetProjectSuccess(data) {
		    console.log('project', data.item);
		    vm.project = data.item;
		};

		function _commonErrorHandler(data) {
			console.log('there was an error handling your request ', data);
		};
	};
})();




