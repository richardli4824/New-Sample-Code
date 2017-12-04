(function () {
    'use strict';

    angular.module(APPNAME)
           .config(['$stateProvider', '$urlRouterProvider', function ($stateProvider, $urlRouterProvider) {

               $stateProvider.state('adminCompetencyIndex', {
                   url: '/',
                   templateUrl: '/Scripts/sample/application/admin/competency/templates/index.html',
                   controller: 'AdminCompetencyIndexController',
                   controllerAs: 'acic'
               }).state('create', {
                   url: '/create',
                   templateUrl: '/Scripts/sample/application/admin/competency/templates/manage.html',
                   controller: 'AdminCompetencyManageController',
                   controllerAs: 'acmc'
               }).state('edit', {
                   url: '/edit/:competencyUserId',
                   templateUrl: '/Scripts/sample/application/admin/competency/templates/manage.html',
                   controller: 'AdminCompetencyManageController',
                   controllerAs: 'acmc'
               }).state('otherwise', {
                   url: '*path',
                   templateUrl: '/Scripts/sample/application/error_page/templates/page_404_error.html'
               })
           }]);
})();