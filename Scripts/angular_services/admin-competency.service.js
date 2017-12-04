(function () {
    'use strict';

    angular.module(APPNAME)
        .factory('$adminCompetencyService', AdminCompetencyService);

    AdminCompetencyService.$inject = ['$baseService', '$sample', '$http'];

    function AdminCompetencyService($baseService, $sample, $http) {

        var service = {
            getAllUsers: getAllUsers,
            getNewUsers: getNewUsers,
            getCompetencies: getCompetencies,
            insert: insert,
            update: update,
            getByUserCompetencyId: getByUserCompetencyId,
            deleteCompetencyUser: deleteCompetencyUser

        };

        return service;

        //get all users with a competency ID
        function getAllUsers() {
            return $http.get('/api/admin/competency').then(handleSuccess).catch(handleError("Error getting users"));
        };

        //get all users with no competency
        function getNewUsers() {
            return $http.get('/api/admin/competency/new').then(handleSuccess).catch(handleError("Error getting users"));
        };

        //get all competencies
        function getCompetencies() {
            return $http.get('/api/admin/competency/competencies').then(handleSuccess).catch(handleError("Error getting users"));
        };

        //create new user
        function insert(competencyUser) {
            return $http.post('/api/admin/competency', competencyUser).then(handleSuccess).catch(handleError("Error inserting user competency."));
        };

        //update new user
        function update(competencyUser) {
            return $http.put('/api/admin/competency', competencyUser).then(handleSuccess).catch(handleError("Error inserting user competency."));
        };

        //get user by Id
        function getByUserCompetencyId(competencyUserId) {
            return $http.get('/api/admin/competency/edit/' + competencyUserId).then(handleSuccess).catch(handleError("Error getting users"));
        };

        //delete user
        function deleteCompetencyUser(competencyUserId) {
            return $http.delete('/api/admin/competency/' + competencyUserId).then(handleSuccess).catch(handleError("Error deleting inspection"));
        };

        //success handler
        function handleSuccess(response) {
            return response.data;
        };

        //error handler
        function handleError(error) {
            return {
                success: false,
                message: error.data
            }
        };
    }

})();