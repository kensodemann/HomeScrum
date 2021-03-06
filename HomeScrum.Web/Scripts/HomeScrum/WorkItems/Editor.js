﻿var Editor = (function () {
   function ShowHideParentWorkItem(effect) {
      var canHaveParent = $("#WorkItemTypeId").find(":selected").attr("data-CanHaveParent");
      if (canHaveParent == "True") {
         $("#ParentWorkItemDiv").show(effect);
      }
      else {
         $("#ParentWorkItemDiv").hide(effect);
      }
   }

   function ShowHideAssignedUser(effect) {
      var canHaveParent = $("#WorkItemTypeId").find(":selected").attr("data-CanBeAssigned");
      if (canHaveParent == "True") {
         $("#AssignedToUserDiv").show(effect);
      }
      else {
         $("#AssignedToUserDiv").hide(effect);
      }
   }

   function ShowHidePoints(effect) {
      if (CanHaveChildren()) {
         $("#PointsArea").hide(effect);
      } else {
         $("#PointsArea").show(effect);
      }
   }

   function ShowHideSprints() {
      var selectedProjectId = $("#ProjectId").val();
      ShowAllSprints();
      if (selectedProjectId !== "00000000-0000-0000-0000-000000000000") {
         HideOtherProjectSprints(selectedProjectId);
      }
   }

   function ShowAllSprints() {
      $("#SprintId option").each(function (i, e) {
         if ($(e).parent().is('span')) {
            $(e).unwrap();
         }
      });
   }

   function HideOtherProjectSprints(id) {
      var domQuery = "#SprintId option[data-projectId != '00000000-0000-0000-0000-000000000000'][data-projectId != '" + id + "']";
      $(domQuery).wrap('<span>');
   }

   function ShowHideDataItems(effect) {
      ShowHideParentWorkItem(effect);
      ShowHideAssignedUser(effect);
      ShowHidePoints(effect);
   }

   function SetProjectToParentWorkItemProject() {
      var selectedProject = $("#ProjectId").val();
      var backlogProject = $("#ParentWorkItemId").find(":selected").attr("data-ProjectId");
      if (selectedProject != backlogProject &&
          backlogProject != "00000000-0000-0000-0000-000000000000") {
         $("#ProjectId").val(backlogProject);
         $("#ProjectId").val(backlogProject);
      }
   }

   function SetSprintToParentWorkItemSprint() {
      var selectedSprint = $("#SprintId").val();
      var backlogSprint = $("#ParentWorkItemId").find(":selected").attr("data-SprintId");
      if (selectedSprint != backlogSprint &&
          backlogSprint != "00000000-0000-0000-0000-000000000000") {
         $("#SprintId").val(backlogSprint);
         $("#SprintId").val(backlogSprint);
      }
   }

   function SetProjectToSprintProject() {
      var selectedProject = $("#ProjectId").val();
      var sprintProject = $("#SprintId").find(":selected").attr("data-ProjectId");
      if (selectedProject != sprintProject &&
          sprintProject != "00000000-0000-0000-0000-000000000000") {
         $("#ProjectId").val(sprintProject);
         $("#ProjectId").val(sprintProject);
      }
   }

   function SetAccess() {
      SetNameAccess();
      SetDescriptionAccess();
      SetAssignedToUserAccess();
      SetCreateNewTaskAccess();
      SetParentWorkItemAccess();
      SetProjectAccess();
      SetSprintAccess();
      SetWorkItemTypeAccess();
      SetPointsAccess();
      SetPointsRemainingAccess();
   }

   function SetNameAccess() {
      if (WorkItemIsClosed()) {
         $("#Name").prop('readonly', true);
         $("#Name").addClass("disabled");
      }
      else {
         $("#Name").prop('readonly', false);
         $("#Name").removeClass("disabled");
      }
   }

   function SetDescriptionAccess() {
      if (WorkItemIsClosed()) {
         $("#Description").prop('readonly', true);
         $("#Description").addClass("disabled");
      }
      else {
         $("#Description").prop('readonly', false);
         $("#Description").removeClass("disabled");
      }
   }

   function SetWorkItemTypeAccess() {
      if (WorkItemIsClosed()) {
         $("#WorkItemTypeId").prop('disabled', true);
      }
      else {
         $("#WorkItemTypeId").prop('disabled', false);
      }
   }

   function SetParentWorkItemAccess() {
      if (WorkItemIsClosed()) {
         $("#ParentWorkItemId").prop('disabled', true);
      }
      else {
         $("#ParentWorkItemId").prop('disabled', false);
      }
   }

   function SetProjectAccess() {
      var backlogItem = $("#ParentWorkItemId").val();
      var sprint = $("#SprintId").val();
      if (backlogItem != "00000000-0000-0000-0000-000000000000" ||
          sprint != "00000000-0000-0000-0000-000000000000" ||
          WorkItemIsClosed()) {
         $("#ProjectId").prop('disabled', true);
      }
      else {
         $("#ProjectId").prop('disabled', false);
      }
   }

   function SetSprintAccess() {
      var backlogItem = $("#ParentWorkItemId").val();
      if (backlogItem != "00000000-0000-0000-0000-000000000000" || WorkItemIsClosed()) {
         $("#SprintId").prop('disabled', true);
      }
      else {
         $("#SprintId").prop('disabled', false);
      }
   }

   function SetAssignedToUserAccess() {
      if (WorkItemIsClosed()) {
         $("#AssignedToUserId").prop('disabled', true);
      }
      else {
         $("#AssignedToUserId").prop('disabled', false);
      }
   }

   function SetCreateNewTaskAccess() {
      if (WorkItemIsClosed() || TaskListIsClosed()) {
         $("#CreateNewTask").prop('disabled', true);
      }
      else {
         $("#CreateNewTask").prop('disabled', false);
      }
   }

   function SetPointsAccess() {
      if (WorkStartedOnWorkItem()) {
         $("#Points").prop("disabled", true);
      } else {
         $("#Points").prop("disabled", false);
      }
   }

   function SetPointsRemainingAccess() {
      if (WorkItemIsClosed()) {
         $("#PointsRemaining").prop("disabled", true);
      } else {
         $("#PointsRemaining").prop("disabled", false);
      }
   }

   function ClosePointsRemainingIfStatusIsClosed() {
      if (WorkItemIsClosed()) {
         $("#PointsRemaining").val(0);
      }
   }

   function TaskListIsClosed() {
      var taskListIsClosed = $("#SprintId").find(":selected").attr("data-TaskListIsClosed");
      return (taskListIsClosed == "True");
   }

   function WorkItemIsClosed() {
      var statusIsOpen = $("#StatusId").find(":selected").attr("data-IsOpenStatus");
      return (statusIsOpen == "False");
   }

   function WorkStartedOnWorkItem() {
      var workStarted = $("#StatusId").find(":selected").attr("data-WorkStarted");
      return (workStarted == "True");
   }

   function CanHaveChildren() {
      return ($("#WorkItemTypeId").find(":selected").attr("data-CanHaveChildren") === "True");
   }

   function SetupWorkItemTypeSelectList() {
      $("#WorkItemTypeId").change(function () {
         ShowHideDataItems("fade");
         SetAccess();
      });
   }

   function SetupStatusSelectList() {
      var selectList = $("#StatusId");
      selectList.change(function () {
         SetAccess();
         ClosePointsRemainingIfStatusIsClosed();
      });
   }

   function SetupParentWorkItemSelectList() {
      $("#ParentWorkItemId").change(function () {
         SetProjectToParentWorkItemProject();
         SetSprintToParentWorkItemSprint();
         SetProjectAccess();
         SetSprintAccess();
      });
   }

   function SetupSprintSelectList() {
      $("#SprintId").change(function () {
         SetProjectToSprintProject();
         SetCreateNewTaskAccess();
         SetProjectAccess();
      });
   }

   function SetupProjectSlectList() {
      $("#ProjectId").change(function () {
         ShowHideSprints();
      });
   }

   function SetupPointsInputs() {
      var points = $("#Points").val();
      SetPointsAccess();
      SetPointsRemainingAccess();

      $("#Points").change(function () {
         SyncPointsRemaining();
      });
   }

   function SyncPointsRemaining() {
      var points = $("#Points").val();
      $("#PointsRemaining").val(points)
   }

   function SetupSubmitButton() {
      $("#SubmitButton").click(function () {
         EnableInputs();
         $("form#Editor").submit();
         return false;
      });
   }

   function EnableInputs() {
      $("#Points").prop("disabled", false);
      $("#PointsRemaining").prop("disabled", false);
      $(".MainData:disabled").prop("disabled", false);
   }

   var init = function () {
      ShowHideDataItems();
      ShowHideSprints();

      SetupStatusSelectList();
      SetupWorkItemTypeSelectList();
      SetupParentWorkItemSelectList();
      SetupSprintSelectList();
      SetupProjectSlectList();

      SetupPointsInputs();

      SetupSubmitButton();

      SetAccess();
   };

   return {
      init: init
   };
})();