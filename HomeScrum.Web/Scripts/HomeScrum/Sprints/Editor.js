﻿var Editor = (function () {
   function BacklogIsClosed() {
      var isClosed = $("#StatusId").find(":selected").attr("data-BacklogIsClosed");
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

   function SetAccess() {
      SetTextInputAccess($("#Name"));
      SetTextInputAccess($("#Description"));
      SetTextInputAccess($("#Goal"));
      SetProjectIdAccess();
      SetCapacityAccess();
   }

   function SetupStatusSelectList() {
      $("#StatusId").change(function () {
         SetAccess();
      });
   }

   function SetupDates() {
      $("#StartDate").datetimepicker({
         pickTime: false
      });

      $("#EndDate").datetimepicker({
         pickTime: false
      });
   }

   function SetupCapacity() {
      SetCapacityAccess();
   }

   function SetCapacityAccess() {
      var c = $("#Capacity");
      if (BacklogIsClosed()) {
         c.prop("disabled", true);
      } else {
         c.prop("disabled", false);
      }
   }

   function SetupSubmitButton() {
      $("#SubmitButton").click(function () {
         $(".MainData:disabled").prop("disabled", false);
         $("form#Editor").submit();
         return false;
      });
   }

   var init = function () {
      SetupStatusSelectList();
      SetupDates();
      SetupCapacity();
      SetupSubmitButton();
      SetAccess();
   };

   return {
      init: init
   };
})();