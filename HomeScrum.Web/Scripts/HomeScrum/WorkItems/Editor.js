var Editor = (function () {
   function EnableInputs() {
      $("#Points").spinner("enable");
      $("#PointsRemaining").spinner("enable");
   }

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

   function ShowHideTaskList(effect) {
      var canHaveChildren = $("#WorkItemTypeId").find(":selected").attr("data-CanHaveChildren");
      var mode = $("#Mode").val();
      if (canHaveChildren == "True" && $("#Mode").val() != "Create") {
         $("#TaskListDiv").show(effect);
         $("#CreateNewTask").show(effect);
      }
      else {
         $("#TaskListDiv").hide(effect);
         $("#CreateNewTask").hide(effect);
      }
   }

   function ShowHideDataItems(effect) {
      ShowHideParentWorkItem(effect);
      ShowHideAssignedUser(effect);
      ShowHideTaskList(effect);
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

   function TaskListIsClosed() {
      var taskListIsClosed = $("#SprintId").find(":selected").attr("data-TaskListIsClosed");
      return (taskListIsClosed == "True");
   }

   function WorkItemIsClosed() {
      var statusIsOpen = $("#StatusId").find(":selected").attr("data-IsOpenStatus");
      return (statusIsOpen == "False");
   }

   function SetupWorkItemTypeSelectList() {
      $("#WorkItemTypeId").change(function () {
         ShowHideDataItems("fade");
      });
   }

   function SetupStatusSelectList() {
      var selectList = $("#StatusId");
      selectList.change(function () {
         SetAccess();
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

   function SetupPointsSpinners() {
      $("#Points").spinner({ min: 1, max: 12 });
      $("#PointsRemaining").spinner({ min: 0, max: 12 });

      if ($("#Mode").val() === "ReadOnly") {
         $("#Points").spinner("disable");
         $("#PointsRemaining").spinner("disable");
      }
   }

   function HandleSubmitClicked() {
      EnableInputs();
      EditorBase.submitButtonClicked();
      SetAccess();
   }

   var init = function () {
      EditorBase.init(HandleSubmitClicked);
      ShowHideDataItems();

      SetupStatusSelectList();
      SetupWorkItemTypeSelectList();
      SetupParentWorkItemSelectList();
      SetupSprintSelectList();

      SetupPointsSpinners();
   };

   return {
      init: init
   };
})();