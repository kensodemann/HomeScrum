var Details = (function () {

   function init() {
      ShowHideBacklogLink();
      ShowHideTaskLink();    
   }

   function ShowHideBacklogLink() {
      if ($('#CanAddBacklog').val() === 'True') {
         $('#BacklogLink').show();
      } else {
         $('#BacklogLink').hide();
      }
   }

   function ShowHideTaskLink() {
      if ($('#CanAddTasks').val() === 'True') {
         $('#TaskListLink').show();
      } else {
         $('#TaskListLink').hide();
      }
   }

   return {
      init: init
   };
})();