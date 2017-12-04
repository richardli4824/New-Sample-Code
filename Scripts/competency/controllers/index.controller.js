(function () {
    'use strict';

    angular.module(APPNAME)
        .controller('AdminCompetencyIndexController', AdminCompetencyIndexController)
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


    AdminCompetencyIndexController.$inject = ['$scope', '$baseController', '$adminCompetencyService', '$uibModal']

    function AdminCompetencyIndexController($scope, $baseController, $adminCompetencyService, $uibModal) {

        var vm = this;

        $baseController.merge(vm, $baseController);

        // services
        vm.$scope = $scope;
        vm.$adminCompetencyService = $adminCompetencyService;
        vm.$uibModal = $uibModal;

        // variables 
        vm.competencies = null;

        //bindable functions
        vm.openDeleteModal = _openDeleteModal;

        // function calls
        initialize();


        function initialize() {
            vm.dtOptions = vm.$dataTableService.dataTableOptions();
            vm.$adminCompetencyService.getAllUsers().then(_getUsersSuccess, _getUsersError);
        };

        function _openDeleteModal(competencyUserId) {
            var modalInstance = vm.$uibModal.open({
                animation: true,
                templateUrl: '/Scripts/sample/application/admin/competency/templates/delete-modal.html',
                controller: 'AdminCompetencyDeleteModalController as dmc',
                size: 'md',
                resolve: {
                    competencyUserId: function () {
                        return competencyUserId;
                    }
                }
            });
            modalInstance.result.then(function () {
                vm.$adminCompetencyService.getAllUsers(competencyUserId).then(_getUsersSuccess, _getUsersError);
            }), function () {
                console.log('Modal dismissed at: ' + new Date());
            }
            console.log('delete userCompetency info:', competencyUserId);
        }

        function _getUsersSuccess(data) {
            console.log("success getting users");
            vm.competencies = data.items;
            console.log(vm.competencies);
            console.log(data.totalItems);
            console.log(data);
        };

        function _getUsersError(data) {
            console.log("error getting users", data);
        }
    };
})();




