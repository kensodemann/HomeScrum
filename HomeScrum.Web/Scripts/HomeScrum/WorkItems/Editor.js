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

   function SetupWorkItemTypeSelectList() {
      var selectList = $("#SelectWorkItemTypeId");
      Utilities.syncHiddenElement(selectList.get(0));
      selectList.change(function () {
         Utilities.syncHiddenElement(this);
         ShowHideDataItems("fade");
      });
   }

   var init = function () {
      ShowHideDataItems();

      SetupWorkItemTypeSelectList();
   };

   return {
      init: init
   };
})();