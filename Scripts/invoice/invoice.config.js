(function () {
    'use strict';

    angular.module(APPNAME)
           .config(['$stateProvider', '$urlRouterProvider', function ($stateProvider, $urlRouterProvider) {

               $stateProvider.state('invoice', {
                   url: '/manage/:clientId/:invoiceId',
                   templateUrl: '/Scripts/sample/application/invoice/templates/manage.html',
                   controller: 'ManageController',
                   controllerAs: 'qc'
               }).state('quotes', {
                   url: '/quotes/:clientId',
                   templateUrl: '/Scripts/sample/application/invoice/templates/openQuotes.html',
                   controller: 'ViewAllQuotesController',
                   controllerAs: 'vaq'
               }).state('invoices', {
                   url: '/invoices/:clientId',
                   templateUrl: '/Scripts/sample/application/invoice/templates/openInvoices.html',
                   controller: 'ViewAllInvoicesController',
                   controllerAs: 'vai'
               }).state('payment', {
                   url: '/payment/:clientId/:invoiceId',
                   templateUrl: '/Scripts/sample/application/invoice/templates/pay.html',
                   controller: 'PayModalController',
                   controllerAs: 'pmc'
               }).state('renewal', {
                   url: '/renewal/:clientId/:projectId',
                   templateUrl: '/Scripts/sample/application/invoice/templates/renewal.html',
                   controller: 'RenewalController',
                   controllerAs: 'rc'
               }).state('otherwise', {
                   url: '*path',
                   templateUrl: '/Scripts/sample/application/error_page/templates/page_404_error.html'
               })
           }]);
})();