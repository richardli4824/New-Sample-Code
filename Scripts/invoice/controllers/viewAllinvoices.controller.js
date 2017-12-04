(function () {
    'use strict';

    angular.module(APPNAME)
        .controller('ViewAllInvoicesController', ViewAllInvoicesController)
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


    ViewAllInvoicesController.$inject = ['$scope', '$baseController', '$state', '$adminInvoiceService', '$uibModal', '$stateParams', 'DTOptionsBuilder', 'DTColumnBuilder']

    function ViewAllInvoicesController($scope, $baseController, $state, $adminInvoiceService, $uibModal, $stateParams, DTOptionsBuilder, DTColumnBuilder) {

        var vm = this;

        $baseController.merge(vm, $baseController);

        // services
        vm.$scope = $scope;
        vm.$state = $state;
        vm.$adminInvoiceService = $adminInvoiceService;
        vm.$uibModal = $uibModal;
        vm.clientId = $stateParams.clientId;
        vm.invoiceId = $stateParams.invoiceId;
        vm.invoiceHeading = null;

        // variables 
        vm.dataTableOptions = _dataTableOptions;

        //bindable functions

        // function calls
        initialize();


        function initialize() {
            vm.$adminInvoiceService.getInvoiceByClientId(vm.clientId).then(_onGetQuoteSuccess, _commonErrorHandler);
            vm.dataTableOptions();

        };

        function _dataTableOptions() {

            vm.dtOptions = DTOptionsBuilder.newOptions()
                .withDOM('<"html5buttons"B>lTfgitp')
                .withButtons([
                    //{ extend: 'colvis'},
                    { extend: 'copy' },
                    { extend: 'csv' },
                    { extend: 'excel', title: 'ExampleFile' },
                    { extend: 'pdf', title: 'ExampleFile' },
                    {
                        extend: 'print',
                        customize: function (win) {
                            $(win.document.body).addClass('white-bg');
                            $(win.document.body).css('font-size', '10px');

                            $(win.document.body).find('table')
                                .addClass('compact')
                                .css('font-size', 'inherit');
                        }
                    }
                ]);
        };

        function _onGetQuoteSuccess(data) {
            vm.invoice = data.items;
            console.log('invoices', vm.invoice);
        };

        function _commonErrorHandler(data) {
            console.log('there was an error handling your request ', data);
        };
    };
})();




