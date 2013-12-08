define(['jquery', 'services/model'], function ($, model) {
   var service = {
      getWorkItemTypes: getWorkItemTypes
   };

   return service;

   function getWorkItemTypes(workItemTypesObservable) {
      workItemTypesObservable([]);

      var url = 'api/WorkItemTypes';

      return $.ajax({
         url: url,
         type: 'GET',
         timeout: 30000,
         dataType: 'json'
      }).then(processData);

      function processData(data) {
         var workItemTypes = [];

         data.forEach(function (item) {
            var wit = model.workItemType(item);
            workItemTypes.push(wit);
         });

         workItemTypesObservable(workItemTypes);
      }
   }
});