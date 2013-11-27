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
         $("#ProjectId").prop("disabled", true);
      }
      else {
         $("#ProjectId").prop("disabled", false);
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
      SetDateAccess($("#StartDate"));
      SetDateAccess($("#EndDate"));
      SetCapacityAccess();
   }

   function SetupStatusSelectList() {
      $("#StatusId").change(function () {
         ShowHideDataItems();
         SetAccess();
      });
   }

   function handleSubmitClicked() {
      EditorBase.submitButtonClicked();
      SetAccess();
   }

   function SetupDates() {
      $("#StartDate").datepicker({
         changeMonth: true,
         changeYear: true
      });
      SetDateAccess($("#StartDate"));

      $("#EndDate").datepicker({
         changeMonth: true,
         changeYear: true
      });
      SetDateAccess("#EndDate");
   }

   function SetDateAccess(datePicker) {
      if (EditorBase.readOnlyMode()) {
         $(datePicker).datepicker("disable");
      } else {
         $(datePicker).datepicker("enable");
      }
   }

   function SetupCapacity() {
      $("#Capacity").spinner({
         min: 1,
         max: 32767,
      });
      SetCapacityAccess();
   }

   function SetCapacityAccess() {
      var c = $("#Capacity");
      if (EditorBase.readOnlyMode()) {
         c.spinner("disable");
         c.prop("disabled", true);
      } else {
         c.spinner("enable");
         c.prop("disabled", false);
      }
   }

   var init = function () {
      ShowHideDataItems();

      SetupStatusSelectList();
      SetupDates();
      SetupCapacity();

      EditorBase.init(handleSubmitClicked);
   };

   return {
      init: init
   };
})();