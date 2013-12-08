define(['knockout', 'services/dataService'], function (ko, dataService) {
   var items = ko.observableArray();

   var vm = {
      displayName: 'Work Item Statuses',
      items: items,
      activate: activate
   };

   return vm;

   function activate() {
      dataService.getWorkItemStatuses(items);
   }
});