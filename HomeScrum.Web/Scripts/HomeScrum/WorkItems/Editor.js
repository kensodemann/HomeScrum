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

   function ShowHideDataItems(effect) {
      ShowHideParentWorkItem(effect);
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