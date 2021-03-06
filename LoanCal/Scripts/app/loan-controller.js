﻿angular.module('LoanCalculatorApp', ["ngTable"])
    .controller('LoanCtrl', function ($scope, $http, NgTableParams) {
        $scope.data = [];
        $scope.loans = [];
        //create first elemetn in UI
        var loan = new Object();
        loan.balance = '';
        loan.rate = '';
        loan.term = '';
        $scope.loans.push(loan);
        //function to add each line of loan 
        $scope.addLoan = function () {
           var loan = new Object();
            loan.balance ='';
            loan.rate = '';
            loan.term = '';
            $scope.loans.push(loan);
        }
        $scope.removeLoan = function (index) {
           
            $scope.loans.splice(index, 1);
        }
      
        $scope.tableArray=[]
        //calling the api
        $scope.calculate = function () {
            console.log($scope.loans);
            $scope.working = true;
            $scope.tableArray = [];
            $scope.data = [];
            $http.post('/api/Loan/Calculate', $scope.loans).then(function (data, status, headers, config) {
                // when calculation completes with success
                $scope.working = false;
                console.log(data);
                $scope.data = data.data;
                for (var i = 0; i < $scope.data.length; i++) {
                    var table = new Object();
                    table.tableParams = new NgTableParams({}, { dataset: $scope.data[i] });
                    $scope.tableArray.push(table);
                }
            }, function (data, status, headers, config) {
                // when calculation completes with error
                $scope.title = "Oops... something went wrong during the calculations";
                $scope.working = false;
            });
        }
    });