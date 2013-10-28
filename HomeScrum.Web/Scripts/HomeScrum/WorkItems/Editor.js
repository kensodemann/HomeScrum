var Editor = (function() {
   function ShowHideParentWorkItem(effect) {
      var canHaveParent = $("#SelectWorkItemTypeId").find(":selected").attr("data-CanHaveParent");
      if (canHaveParent == "True") {
         $("#ParentWorkItemDiv").show(effect);
      }
      else {
         $("#ParentWorkItemDiv").hide(effect);
      }
   }

   function ShowHideAssignedUser(effect) {
      var canHaveParent = $("#SelectWorkItemTypeId").find(":selected").attr("data-CanBeAssigned");
      if (canHaveParent == "True") {
         $("#AssignedToUserDiv").show(effect);
      }
      else {
         $("#AssignedToUserDiv").hide(effect);
      }
   }

   function ShowHideTaskList(effect) {
      var canHaveChildren = $("#SelectWorkItemTypeId").find(":selected").attr("data-CanHaveChildren");
      if (canHaveChildren == "True") {
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
      var selectedProject = $("#SelectProjectId").val();
      var backlogProject = $("#SelectParentWorkItemId").find(":selected").attr("data-ProjectId");
      if (selectedProject != backlogProject &&
          backlogProject != "00000000-0000-0000-0000-000000000000") {
         $("#SelectProjectId").val(backlogProject);
         $("#ProjectId").val(backlogProject);
      }
   }

   function SetSprintToParentWorkItemSprint() {
      var selectedSprint = $("#SelectSprintId").val();
      var backlogSprint = $("#SelectParentWorkItemId").find(":selected").attr("data-SprintId");
      if (selectedSprint != backlogSprint &&
          backlogSprint != "00000000-0000-0000-0000-000000000000") {
         $("#SelectSprintId").val(backlogSprint);
         $("#SprintId").val(backlogSprint);
      }
   }

   function SetProjectToSprintProject() {
      var selectedProject = $("#SelectProjectId").val();
      var sprintProject = $("#SelectSprintId").find(":selected").attr("data-ProjectId");
      if (selectedProject != sprintProject &&
          sprintProject != "00000000-0000-0000-0000-000000000000") {
         $("#SelectProjectId").val(sprintProject);
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
         $("#SelectWorkItemTypeId").prop('disabled', true);
      }
      else {
         $("#SelectWorkItemTypeId").prop('disabled', false);
      }
   }

   function SetParentWorkItemAccess() {
      if (WorkItemIsClosed()) {
         $("#SelectParentWorkItemId").prop('disabled', true);
      }
      else {
         $("#SelectParentWorkItemId").prop('disabled', false);
      }
   }

   function SetProjectAccess() {
      var backlogItem = $("#SelectParentWorkItemId").val();
      var sprint = $("#SelectSprintId").val();
      if (backlogItem != "00000000-0000-0000-0000-000000000000" ||
          sprint != "00000000-0000-0000-0000-000000000000" ||
          WorkItemIsClosed()) {
         $("#SelectProjectId").prop('disabled', true);
      }
      else {
         $("#SelectProjectId").prop('disabled', false);
      }
   }

   function SetSprintAccess() {
      var backlogItem = $("#SelectParentWorkItemId").val();
      if (backlogItem != "00000000-0000-0000-0000-000000000000" || WorkItemIsClosed()) {
         $("#SelectSprintId").prop('disabled', true);
      }
      else {
         $("#SelectSprintId").prop('disabled', false);
      }
   }

   function SetAssignedToUserAccess() {
      if (WorkItemIsClosed()) {
         $("#SelectAssignedToUserId").prop('disabled', true);
      }
      else {
         $("#SelectAssignedToUserId").prop('disabled', false);
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
      var taskListIsClosed = $("#SelectSprintId").find(":selected").attr("data-TaskListIsClosed");
      return (taskListIsClosed == "True");
   }

   function WorkItemIsClosed() {
      var statusIsOpen = $("#StatusId").find(":selected").attr("data-IsOpenStatus");
      return (statusIsOpen == "False");
   }

   function SetupWorkItemTypeSelectList() {
      var selectList = $("#SelectWorkItemTypeId");
      Utilities.syncHiddenElement(selectList.get(0));
      selectList.change(function () {
         Utilities.syncHiddenElement(this);
         ShowHideDataItems("fade");
      });
   }

   function SetupStatusSelectList() {
      var selectList = $("#StatusId");
      Utilities.syncHiddenElement(selectList.get(0));
      selectList.change(function () {
         Utilities.syncHiddenElement(this);
         SetAccess();
      });
   }

   function SetupParentWorkItemSelectList() {
      var selectList = $("#SelectParentWorkItemId");
      Utilities.syncHiddenElement(selectList.get(0));
      selectList.change(function () {
         Utilities.syncHiddenElement(this);
         SetProjectToParentWorkItemProject();
         SetSprintToParentWorkItemSprint();
         SetProjectAccess();
         SetSprintAccess();
      });
   }

   function SetupSprintSelectList() {
      var selectList = $("#SelectSprintId");
      Utilities.syncHiddenElement(selectList.get(0));
      selectList.change(function () {
         Utilities.syncHiddenElement(this);
         SetProjectToSprintProject();
         SetCreateNewTaskAccess();
         SetProjectAccess();
      });
   }

   var init = function () {
      ShowHideDataItems();
      SetAccess();

      SetupStatusSelectList();
      SetupWorkItemTypeSelectList();
      SetupParentWorkItemSelectList();
      SetupSprintSelectList();
   };

   return {
      init: init
   };
})();