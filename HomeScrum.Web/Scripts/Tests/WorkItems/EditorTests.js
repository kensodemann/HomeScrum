test('Parent Work Item Hidden if cannot have parent on init', function () {
   $("#selWorkItemType").attr("data-CanHaveParent", "False");
   Editor.init();
   ok($("#ParentWorkItemDiv").is(":hidden"));
});

test('Parent Work Item Hidden if cannot have parent on change', function () {
   $("#selWorkItemType").attr("data-CanHaveParent", "True");
   Editor.init();
   $("#selWorkItemType").attr("data-CanHaveParent", "False");
   $("#SelectWorkItemTypeId").change();
   stop();
   setTimeout(function () {
      ok($("#ParentWorkItemDiv").is(":hidden"));
      start();
   }, 1000);
});

test('Parent Work Item Visible if can have parent on init', function () {
   $("#selWorkItemType").attr("data-CanHaveParent", "True");
   Editor.init();
   ok($("#ParentWorkItemDiv").is(":visible"));
});

test('Parent Work Item Visible if can have parent on change', function () {
   $("#selWorkItemType").attr("data-CanHaveParent", "False");
   Editor.init();
   $("#selWorkItemType").attr("data-CanHaveParent", "True");
   $("#SelectWorkItemTypeId").change();
   ok($("#ParentWorkItemDiv").is(":visible"));
});

// Here are the behaviors that need to be tested:
//   * When backlog item, following items are hidden/shown:
//     ** Backlog Item (hide)
//     ** Assigned To (hide)
//     ** Task List (show)
//     ** Add New Task Button (show)
//   * When task item, opposite of above
//   * Set project ID to parent when parent selected
//   * Set sprint ID to parent when parent selected
//   * Set project ID to sprint's project when sprint selected
//   * Status not completed, all items active
//   * Status completed, all items inactive except status, items are:
//     ** Name
//     ** Description
//     ** Work Item Type
//     ** Backlog Item
//     ** Project
//     ** Sprint
//     ** Assigned To
//     ** Add New Task
//   * Sync Hidden on following:
//     ** Work Item Type
//     ** Backlog Item
//     ** Project
//     ** Sprint
//     ** Assigned To