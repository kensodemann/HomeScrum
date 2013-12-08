define(['jquery', 'services/model'], function ($, model) {
   var service = {
      getWorkItemTypes: getWorkItemTypes,
      getWorkItemStatuses: getWorkItemStatuses
   };

   return service;

   function getWorkItemStatuses(observableArray) {
      return getData('WorkItemStatuses', model.workItemStatus, observableArray);
   }

   function getWorkItemTypes(observableArray) {
      return getData('WorkItemTypes', model.workItemType, observableArray);
   }


   function getData(endpoint, constructModel, container) {
      container([]);

      var url = '/api/' + endpoint;

      return $.ajax({
         url: url,
         type: 'GET',
         timeout: 30000,
         dataType: 'json'
      }).then(processData);

      function processData(data) {
         var items = [];

         data.forEach(function (item) {
            var i = constructModel(item);
            items.push(i);
         });

         container(items);
      }
   }
});