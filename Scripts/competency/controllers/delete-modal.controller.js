(function (){
    'use strict';

    angular.module(APPNAME)
           .controller('AdminCompetencyDeleteModalController', AdminCompetencyDeleteModalController);

    AdminCompetencyDeleteModalController.$inject = ['$scope', '$baseController', '$adminCompetencyService', '$uibModalInstance', 'competencyUserId']

    function AdminCompetencyDeleteModalController(
        $scope
        , $baseController
        , $adminCompetencyService
        , $uibModalInstance
        , competencyUserId) {

        var vm = this;

        $baseController.merge(vm, $baseController);

        // services 
        vm.$scope = $scope;
        vm.$uibModalInstance = $uibModalInstance;
        vm.$adminCompetencyService = $adminCompetencyService;

        // variables
        vm.competencyUserId = competencyUserId;

        // bindable functions
        vm.delete = _delete;
        vm.cancel = _cancel;

        // function definitions
        function _delete() {
            vm.$adminCompetencyService.deleteCompetencyUser(vm.competencyUserId).then(_success, _error);
        };

        function _success() {
            console.log("successfully deleted");
            vm.$uibModalInstance.close();
            vm.$alertService.success('User Successfully Deleted');
        };

        function _error(jqXhr, error) {
            console.error(error, "error deleting user.");
            vm.$alertService.error('Unable to Delete User');
        };

        function _cancel() {
            vm.$uibModalInstance.dismiss("cancel");
        };
    };
})();