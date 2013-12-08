define(['knockout'], function (ko) {
   var model = {
      workItemStatus: mapToObservable,
      workItemType: mapToObservable
   };

   return model;

   function mapToObservable(dto) {
      var mapped = {};
      for (prop in dto) {
         if (dto.hasOwnProperty(prop)) {
            mapped[prop] = ko.observable(dto[prop]);
         }
      }
      return mapped;
   }
});