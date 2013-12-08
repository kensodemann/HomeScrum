define(['knockout', 'services/dataService'], function (ko, dataService) {
   var items = ko.observableArray();

   var vm = {
      displayName: 'Work Item Types',
      items: items,
      activate: activate
   };

   return vm;

   function activate() {
      dataService.getWorkItemTypes(items);
   }
});