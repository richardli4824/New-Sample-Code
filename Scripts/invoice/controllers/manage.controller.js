(function () {
    'use strict';

    angular.module(APPNAME)
        .controller('ManageController', ManageController)
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


    ManageController.$inject = ['$scope', '$baseController', '$state', '$adminInvoiceService', '$uibModal', '$stateParams']

    function ManageController($scope, $baseController, $state, $adminInvoiceService, $uibModal, $stateParams) {

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
        vm.quotePayload = {};
        vm.combinedLineItemPrice = null;
        vm.daysTilExpiration = 90;
        vm.expiredContainer = false;
        vm.expirationSubHeading = 'Please Pay Immediately';
        vm.expirationMessage = "";
        // variables 

        //bindable functions
        vm.submitInvoice = _submitInvoice;
        vm.printInvoice = _printInvoice;

        // function calls
        initialize();


        function initialize() {
            vm.$adminInvoiceService.getQuoteByQuoteId(vm.clientId, vm.invoiceId).then(_onGetQuoteSuccess, _commonErrorHandler);
            vm.$adminInvoiceService.getLineItemByInvoiceId(vm.invoiceId).then(_onGetLineItemSuccess, _commonErrorHandler);
            vm.$adminInvoiceService.getComments(vm.invoiceId).then(_getCommentsSuccess, _commonErrorHandler);

        };

        function _onGetQuoteSuccess(data) {
            vm.quote = data.item;

            if (vm.quote.quoteAccepted == 1) {
                vm.invoiceHeading = "Invoice";
                vm.submitButton = "Pay Invoice";
            } else {
                vm.invoiceHeading = "Quote";
                vm.submitButton = "Accept Quote";
            }
            console.log('quote data', vm.quote);
            vm.calculateExpDate = new Date(vm.quote.createdDate);
            vm.expirationDate = vm.calculateExpDate.setDate(vm.calculateExpDate.getDate() + vm.daysTilExpiration);

            var today = new Date();
            var expired = new Date(vm.expirationDate);
            var timeDiff = Math.ceil(expired.getTime() - today.getTime());
            var DayDifferential = Math.ceil(timeDiff / (1000 * 3600 * 24));

            switch (true) {
                case (DayDifferential < 0 && DayDifferential >= -29):
                    vm.expirationMessage = "BALANCE OVERDUE";
                    vm.expiredContainer = true;
                    break;
                case (DayDifferential <= -30 && DayDifferential >= -59):
                    vm.expirationMessage = Math.abs(DayDifferential) + ' DAYS OVERDUE';
                    vm.expiredContainer = true;
                    break;
                case (DayDifferential <= -60 && DayDifferential >= -88):
                    vm.expirationMessage = Math.abs(DayDifferential + 90) + ' DAYS UNTIL CANCELLATION';
                    vm.expiredContainer = true;
                    break;
                case (DayDifferential == -89):
                    vm.expirationMessage = Math.abs(DayDifferential + 90) + ' DAY UNTIL CANCELLATION';
                    vm.expiredContainer = true;
                    break;
                case (DayDifferential <= -90):
                    vm.expirationMessage = 'CANCELLED';
                    vm.expirationSubHeading = 'PLEASE CONTACT US';
                    vm.expiredContainer = true;
                    break;
                default:
                    vm.expiredContainer = false;
            }
        };

        function _onGetLineItemSuccess(data) {
            vm.lineItems = data.items;
            if (vm.lineItems != null) {
                for (var i = 0; i < vm.lineItems.length; i++) {
                    vm.combinedLineItemPrice += vm.lineItems[i].lineItemPrice;
                }
            }

        };

        function _submitInvoice() {

            if (vm.quote.quoteAccepted == 0) {
                vm.quotePayload.id = vm.invoiceId;
                vm.quotePayload.quoteAccepted = 1;
                console.log('quote payload', vm.quotePayload);
                vm.$adminInvoiceService.acceptQuote(vm.quotePayload).then(_onAcceptQuoteSuccess, _commonErrorHandler);

            } else {
                console.log('button not doing anything');
            }
        };

        function _printInvoice(invoice) {
            var innerContents = $('#invoice').html();
            var popupWinindow = window.open('', '_blank', 'width=600,height=700,scrollbars=no,menubar=no,toolbar=no,location=no,status=no,titlebar=no');
            popupWinindow.document.open();
            popupWinindow.document.write('<html><head><link href="/Content/bootstrap.min.css" rel="stylesheet"><link href="/Content/bootstrap.min.css" rel="stylesheet"><link href="/Content/inspinia/style.css" rel="stylesheet"><link href="/Content/inspinia/custom.css" rel="stylesheet"></head><body onload="printFunction()">' + innerContents + '<script> function printFunction() {window.print();}</script></html>');
            popupWinindow.document.close();
        };

        function _onAcceptQuoteSuccess() {
            console.log('success accepting quote');
            setTimeout(function () {
                $state.reload();
            }, 1000);
        };

        function _getCommentsSuccess(data) {
            vm.comments = data.items;
            console.log('comments', vm.comments);
        }

        function _commonErrorHandler(data) {
            console.log('there was an error handling your request ', data);
        };
    };
})();




