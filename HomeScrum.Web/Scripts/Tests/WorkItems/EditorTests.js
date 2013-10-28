// Show / Hide Parent Work Item
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


// Show / Hide Assigned To User
test('Assign To User Hidden if cannot be assigned on init', function () {
   $("#selWorkItemType").attr("data-CanBeAssigned", "False");
   Editor.init();
   ok($("#AssignedToUserDiv").is(":hidden"));
});

test('Assign To User Hidden if cannot be assigned on change', function () {
   $("#selWorkItemType").attr("data-CanBeAssigned", "True");
   Editor.init();
   $("#selWorkItemType").attr("data-CanBeAssigned", "False");
   $("#SelectWorkItemTypeId").change();
   stop();
   setTimeout(function () {
      ok($("#AssignedToUserDiv").is(":hidden"));
      start();
   }, 1000);
});

test('Assign To User Visible if can be assigned on init', function () {
   $("#selWorkItemType").attr("data-CanBeAssigned", "True");
   Editor.init();
   ok($("#AssignedToUserDiv").is(":visible"));
});

test('Assign To User Visible if can be assigned on change', function () {
   $("#selWorkItemType").attr("data-CanBeAssigned", "False");
   Editor.init();
   $("#selWorkItemType").attr("data-CanBeAssigned", "True");
   $("#SelectWorkItemTypeId").change();
   ok($("#AssignedToUserDiv").is(":visible"));
});


// Show / Hide Task List
test('Task List Hidden if cannot have children on init', function () {
   $("#selWorkItemType").attr("data-CanHaveChildren", "False");
   Editor.init();
   ok($("#TaskListDiv").is(":hidden"));
});

test('Task List User Hidden if cannot have children on change', function () {
   $("#selWorkItemType").attr("data-CanHaveChildren", "True");
   Editor.init();
   $("#selWorkItemType").attr("data-CanHaveChildren", "False");
   $("#SelectWorkItemTypeId").change();
   stop();
   setTimeout(function () {
      ok($("#TaskListDiv").is(":hidden"));
      start();
   }, 1000);
});

test('Task List User Visible if can have children on init', function () {
   $("#selWorkItemType").attr("data-CanHaveChildren", "True");
   Editor.init();
   ok($("#TaskListDiv").is(":visible"));
});

test('Task List User Visible if can have children on change', function () {
   $("#selWorkItemType").attr("data-CanHaveChildren", "False");
   Editor.init();
   $("#selWorkItemType").attr("data-CanHaveChildren", "True");
   $("#SelectWorkItemTypeId").change();
   ok($("#TaskListDiv").is(":visible"));
});


// Show / Hide Task Button
test('Task Button Hidden if cannot have children on init', function () {
   $("#selWorkItemType").attr("data-CanHaveChildren", "False");
   Editor.init();
   ok($("#CreateNewTask").is(":hidden"));
});

test('Task Button User Hidden if cannot have children on change', function () {
   $("#selWorkItemType").attr("data-CanHaveChildren", "True");
   Editor.init();
   $("#selWorkItemType").attr("data-CanHaveChildren", "False");
   $("#SelectWorkItemTypeId").change();
   stop();
   setTimeout(function () {
      ok($("#CreateNewTask").is(":hidden"));
      start();
   }, 1000);
});

test('Task Button User Visible if can have children on init', function () {
   $("#selWorkItemType").attr("data-CanHaveChildren", "True");
   Editor.init();
   ok($("#CreateNewTask").is(":visible"));
});

test('Task Button User Visible if can have children on change', function () {
   $("#selWorkItemType").attr("data-CanHaveChildren", "False");
   Editor.init();
   $("#selWorkItemType").attr("data-CanHaveChildren", "True");
   $("#SelectWorkItemTypeId").change();
   ok($("#CreateNewTask").is(":visible"));
});

// Project & Sprint Setting
test("Project ID is set to parent's when parent selected", function () {
   Editor.init();
   $("#SelectParentWorkItemId").change();
   var expected = $("#SelectParentWorkItemId").find(":selected").attr("data-ProjectId");
   strictEqual($("#SelectProjectId").val(), expected);
   strictEqual($("#ProjectId").val(), expected);
});

test("Sprint ID is set to parent's when parent selected", function () {
   Editor.init();
   $("#SelectParentWorkItemId").change();
   var expected = $("#SelectParentWorkItemId").find(":selected").attr("data-SprintId");
   strictEqual($("#SelectSprintId").val(), expected);
   strictEqual($("#SprintId").val(), expected);
});

test("Project ID is set to sprint's when sprint selected", function () {
   Editor.init();
   $("#SelectSprintId").change();
   var expected = $("#SelectSprintId").find(":selected").attr("data-ProjectId");
   strictEqual($("#SelectProjectId").val(), expected);
   strictEqual($("#ProjectId").val(), expected);
});

// Active & Inactive based on status
test('Name Active on init if status not complete', function () {
   $("#selStatus").attr("data-IsOpenStatus", "True");
   Editor.init();
   strictEqual($("#Name").prop("readonly"), false);
   ok(!($("#Name").hasClass("disabled")));
});

test('Name Inactive on init if status complete', function () {
   $("#selStatus").attr("data-IsOpenStatus", "False");
   Editor.init();
   strictEqual($("#Name").prop("readonly"), true);
   ok($("#Name").hasClass("disabled"));
});

test('Name Active on status change if status not complete', function () {
   $("#selStatus").attr("data-IsOpenStatus", "False");
   Editor.init();
   $("#selStatus").attr("data-IsOpenStatus", "True");
   $("#StatusId").change();
   strictEqual($("#Name").prop("readonly"), false);
   ok(!($("#Name").hasClass("disabled")));
});

test('Name Inactive on status change if status complete', function () {
   $("#selStatus").attr("data-IsOpenStatus", "True");
   Editor.init();
   $("#selStatus").attr("data-IsOpenStatus", "False");
   $("#StatusId").change();
   strictEqual($("#Name").prop("readonly"), true);
   ok($("#Name").hasClass("disabled"));
});

// Here are the behaviors that need to be tested:
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