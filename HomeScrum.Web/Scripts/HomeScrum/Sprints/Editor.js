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
      var numberOfWorkItems = $("#BacklogItems table tr").length + $("#Tasks table tr").length - 2;
      if (numberOfWorkItems > 0 || SprintIsClosed()) {
         $("#SelectProjectId").prop("disabled", true);
      }
      else {
         $("#SelectProjectId").prop("disabled", false);
      }
   }

   function ShowHideDataItems() {
      ShowHideBacklogLink();
      ShowHideTaskListLink();
   }

   function ShowHideBacklogLink() {
      if (BacklogIsClosed() || SprintIsClosed()) {
         $("#BacklogLink").hide();
      }
      else {
         $("#BacklogLink").show();
      }
   }

   function ShowHideTaskListLink() {
      if (TaskListIsClosed() || SprintIsClosed()) {
         $("#TaskListLink").hide();
      }
      else {
         $("#TaskListLink").show();
      }
   }

   function SetAccess() {
      SetTextInputAccess($("#Name"));
      SetTextInputAccess($("#Description"));
      SetTextInputAccess($("#Goal"));
      SetProjectIdAccess();
   }

   function SetupStatusSelectList() {
      $("#StatusId").change(function () {
         ShowHideDataItems();
         SetAccess();
      });
   }

   function SetupProjectSelectList() {
      var selectList = $("#SelectProjectId");
      Utilities.syncHiddenElement(selectList.get(0));
      selectList.change(function () {
         Utilities.syncHiddenElement(this);
      });
   }

   var init = function () {
      ShowHideDataItems();
      SetAccess();

      SetupProjectSelectList();
      SetupStatusSelectList();

      EditorBase.init();
   };

   return {
      init: init
   };
})();