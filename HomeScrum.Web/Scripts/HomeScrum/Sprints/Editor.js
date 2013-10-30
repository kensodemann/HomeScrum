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

   function SetTextInputAccess(input) {
      if (SprintIsClosed()) {
         input.prop('readonly', true);
         input.addClass("disabled");
      }
      else {
         input.prop('readonly', false);
         input.removeClass("disabled");
      }
   }

   function SetProjectIdAccess() {
      if (SprintIsClosed()) {
         $("#SelectProjectId").prop("disabled", true);
      }
      else {
         $("#SelectProjectxId").prop("disabled", false);
      }
   }

   function ShowHideDataItems() {
   }

   function SetAccess() {
      SetTextInputAccess($("#Name"));
      SetTextInputAccess($("#Description"));
      SetTextInputAccess($("#Goal"));
      SetProjectIdAccess();
   }

   var init = function () {
      ShowHideDataItems();
      SetAccess();
   };

   return {
      init: init
   };
})();