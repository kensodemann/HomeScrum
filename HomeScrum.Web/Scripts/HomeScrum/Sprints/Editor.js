var Editor = (function () {
   function BacklogIsClosed() {
      var isClosed = $("#StatusId").find(":selected").attr("data-BacklogIsClosed");
      return (isClosed == "True");
   }

   function TaskListIsClosed() {
      var isClosed = $("#StatusId").find(":selected").attr("data-TaskListIsClosed");
      return (isClosed == "True");
   }

   function SprintIsClosed() {
      var isOpen = $("#StatusId").find(":selected").attr("data-IsOpenStatus");
      return (isOpen == "False");
   }

   function SetNameAccess() {
      if (SprintIsClosed()) {
         $("#Name").prop('readonly', true);
         $("#Name").addClass("disabled");
      }
      else {
         $("#Name").prop('readonly', false);
         $("#Name").removeClass("disabled");
      }
   }

   function ShowHideDataItems() {
   }

   function SetAccess() {
      SetNameAccess();
   }

   var init = function () {
      ShowHideDataItems();
      SetAccess();
   };

   return {
      init: init
   };
})();