(function () {
    'use strict';

    angular.module(APPNAME)
        .controller('AdminCompetencyManageController', AdminCompetencyManageController)

    AdminCompetencyManageController.$inject = ['$scope', '$baseController', '$adminCompetencyService', '$stateParams', '$location']

    function AdminCompetencyManageController($scope, $baseController, $adminCompetencyService, $stateParams, $location) {

        var vm = this;

        $baseController.merge(vm, $baseController);

        // services
        vm.$scope = $scope;
        vm.$adminCompetencyService = $adminCompetencyService;

        // variables 
        vm.users = null;
        vm.user = null;
        vm.competencies = null;
        vm.competencyPayload = {};
        vm.competencyUserId = $stateParams.competencyUserId;
        vm.breadCrumb = null;
        vm.formTitle = null;


        //bindable functions
        vm.submitCompetency = _submitCompetency;

        // function calls
        initialize();


        function initialize() {
            vm.dtOptions = vm.$dataTableService.dataTableOptions();
            vm.$adminCompetencyService.getCompetencies().then(_getCompetenciesSuccess, _errorHandler);
            if (vm.competencyUserId) {
                vm.$adminCompetencyService.getByUserCompetencyId(vm.competencyUserId).then(_getUserByIdSuccess, _errorHandler);
                vm.breadCrumb = "Edit";
                vm.formTitle = "Edit Staff Competency";
            } else {
                vm.$adminCompetencyService.getNewUsers().then(_getUsersSuccess, _errorHandler);
                vm.breadCrumb = "Add";
                vm.formTitle = "Add Staff Competency";
            }
        };

        function _getUsersSuccess(data) {
            console.log("success getting users");
            vm.users = data.items;
            console.log(vm.users);
            
        };

        function _getCompetenciesSuccess(data) {
            console.log("success getting competencies");
            vm.competencies = data.items;
            console.log(vm.competencies);
            
        };

        function _getUserByIdSuccess(data) {
            console.log(data);
            vm.competencyPayload = data.item;
            console.log('current user', vm.competencyPayload);
        };

        function _submitCompetency() {

            if (vm.competencyUserId) {
                vm.$adminCompetencyService.update(vm.competencyPayload).then(_insertSuccess, _errorHandler);
            } else {
                vm.$adminCompetencyService.insert(vm.competencyPayload).then(_insertSuccess, _errorHandler);
            }
            console.log('competency payload', vm.competencyPayload);
        };



        function _insertSuccess() {
            if (vm.competencyUserId) {
                vm.$alertService.success('User Successfully Updated');
                $location.path('/');
            } else {
                vm.$alertService.success('User Successfully Created');
                $location.path('/');
            }
        };
        
        function _errorHandler(data) {
            console.log("error: " + data);
            vm.$alertService.error('Please check all fields and try again');

        };
    };
})();