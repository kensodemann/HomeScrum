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

   function SetupWorkItemTypeSelectList() {
      var selectList = $("#SelectWorkItemTypeId");
      Utilities.syncHiddenElement(selectList.get(0));
      selectList.change(function () {
         Utilities.syncHiddenElement(this);
         ShowHideDataItems("fade");
      });
   }

   function SetupParentWorkItemSelectList() {
      var selectList = $("#SelectParentWorkItemId");
      Utilities.syncHiddenElement(selectList.get(0));
      selectList.change(function () {
         Utilities.syncHiddenElement(this);
         SetProjectToParentWorkItemProject();
      });
   }

   var init = function () {
      ShowHideDataItems();

      SetupWorkItemTypeSelectList();
      SetupParentWorkItemSelectList();
   };

   return {
      init: init
   };
})();