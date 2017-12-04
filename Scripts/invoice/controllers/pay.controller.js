(function () {
    'use strict';

    angular.module(APPNAME)
           .controller('PayModalController', PayModalController);

    PayModalController.$inject = ['$scope', '$baseController', '$adminInvoiceService', '$stateParams', '$clientService']

    function PayModalController(
        $scope
        , $baseController
        , $adminInvoiceService
        , $stateParams
        , $clientService) {

        var vm = this;

        $baseController.merge(vm, $baseController);

        // services 
        vm.$scope = $scope;
        vm.$adminInvoiceService = $adminInvoiceService;
        vm.$clientService = $clientService;
        vm.clientId = $stateParams.clientId;
        vm.invoiceId = $stateParams.invoiceId;

        // variables

        // bindable functions

        vm.cancel = _cancel;
        // function definitions
        initialize();


        function initialize() {
            vm.$adminInvoiceService.getQuoteByQuoteId(vm.clientId, vm.invoiceId).then(_onGetQuoteSuccess, _commonErrorHandler);
            vm.$clientService.getClientById(vm.clientId).then(_onGetClientSuccess, _commonErrorHandler);

        };

        function _onGetQuoteSuccess(data) {
            console.log('success', data.item);
            vm.invoice = data.item;
        };

        function _onGetClientSuccess(data) {
            console.log('client', data.item);
            vm.client = data.item;
        };

        function _commonErrorHandler(data) {
            console.log('there was an error handling your request ', data);

        };

        function _success() {
            console.log("successfully deleted");
        }

        function _error(jqXhr, error) {
            console.error(error, "error deleting inspection.");
        };

        function _cancel() {
            console.log('invoice', vm.invoiceId);
        };
    };
})();