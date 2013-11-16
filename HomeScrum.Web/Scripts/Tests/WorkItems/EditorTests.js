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

test('Task List Hidden if cannot have children on change', function () {
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

test('Task List Visible if can have children on init', function () {
   $("#selWorkItemType").attr("data-CanHaveChildren", "True");
   Editor.init();
   ok($("#TaskListDiv").is(":visible"));
});

test('Task List Visible if can have children on change', function () {
   $("#selWorkItemType").attr("data-CanHaveChildren", "False");
   Editor.init();
   $("#selWorkItemType").attr("data-CanHaveChildren", "True");
   $("#SelectWorkItemTypeId").change();
   ok($("#TaskListDiv").is(":visible"));
});

test('Task List Hidden if mode is create on init', function () {
   $("#selWorkItemType").attr("data-CanHaveChildren", "True");
   $("#Mode").val("Create");
   Editor.init();
   ok($("#TaskListDiv").is(":hidden"));
});

test('Task List remains hidden on change if Mode is Create', function () {
   $("#selWorkItemType").attr("data-CanHaveChildren", "False");
   $("#Mode").val("Create");
   Editor.init();
   $("#SelectWorkItemTypeId").change();
   ok($("#TaskListDiv").is(":hidden"));
});


// Show / Hide Task Button
test('Task Button Hidden if cannot have children on init', function () {
   $("#selWorkItemType").attr("data-CanHaveChildren", "False");
   Editor.init();
   ok($("#CreateNewTask").is(":hidden"));
});

test('Task Button Hidden if cannot have children on change', function () {
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

test('Task Button Visible if can have children on init', function () {
   $("#selWorkItemType").attr("data-CanHaveChildren", "True");
   Editor.init();
   ok($("#CreateNewTask").is(":visible"));
});

test('Task Button Visible if can have children on change', function () {
   $("#selWorkItemType").attr("data-CanHaveChildren", "False");
   Editor.init();
   $("#selWorkItemType").attr("data-CanHaveChildren", "True");
   $("#SelectWorkItemTypeId").change();
   ok($("#CreateNewTask").is(":visible"));
});

test('Task Button Hidden if mode is Create on init', function () {
   $("#selWorkItemType").attr("data-CanHaveChildren", "True");
   $("#Mode").val("Create");
   Editor.init();
   ok($("#CreateNewTask").is(":hidden"));
});

test('Task Button remains hidden on change if mode is Create', function () {
   $("#selWorkItemType").attr("data-CanHaveChildren", "True");
   $("#Mode").val("Create");
   Editor.init();
   $("#SelectWorkItemTypeId").change();
   ok($("#CreateNewTask").is(":hidden"));
});


// Project & Sprint Setting
test("Project ID is set to parent's when parent selected", function () {
   Editor.init();
   var expected = "3";
   $("#selParentWorkItem").attr("value", "1");
   $("#selParentWorkItem").attr("data-ProjectId", expected);
   $("#SelectParentWorkItemId").change();
   strictEqual($("#SelectProjectId").val(), expected);
   strictEqual($("#ProjectId").val(), expected);
});

test("Sprint ID is set to parent's when parent selected", function () {
   Editor.init();
   var expected = "5";
   $("#selParentWorkItem").attr("value", "1");
   $("#selParentWorkItem").attr("data-SprintId", expected);
   $("#SelectParentWorkItemId").change();
   strictEqual($("#SelectSprintId").val(), expected);
   strictEqual($("#SprintId").val(), expected);
});

test("Project ID is set to sprint's when sprint selected", function () {
   Editor.init();
   var expected = "7";
   $("#selSprint").attr("value", "1");
   $("#selSprint").attr("data-ProjectId", expected);
   $("#SelectSprintId").change();
   strictEqual($("#SelectProjectId").val(), expected);
   strictEqual($("#ProjectId").val(), expected);
});

// Active & Inactive based on status
test('Items Active on status change if status not complete', function () {
   $("#selStatus").attr("data-IsOpenStatus", "False");
   Editor.init();
   $("#selStatus").attr("data-IsOpenStatus", "True");
   $("#StatusId").change();
   assertItemsAreActive();
});

test('Items Inactive on status change if status complete', function () {
   $("#selStatus").attr("data-IsOpenStatus", "True");
   Editor.init();
   $("#selStatus").attr("data-IsOpenStatus", "False");
   $("#StatusId").change();
   assertItemsAreInactive();
});

test('Add New Task not disabled on status change if task list is not closed', function () {
   $("#selSprint").attr("data-TaskListIsClosed", "True");
   Editor.init();
   $("#selSprint").attr("data-TaskListIsClosed", "False");
   $("#SelectSprintId").change();
   strictEqual($("#CreateNewTask").prop("disabled"), false, "Create New Task not Disabled");
});

test('Add New Task disabled on status change if task list is closed', function () {
   $("#selSprint").attr("data-TaskListIsClosed", "False");
   Editor.init();
   $("#selSprint").attr("data-TaskListIsClosed", "True");
   $("#SelectSprintId").change();
   strictEqual($("#CreateNewTask").prop("disabled"), true, "Create New Task Disabled");
});

test('Project not disabled on parent change if no parent and no sprint', function () {
   $("#selParentWorkItem").attr("value", "1");
   $("#selSprint").attr("value", "1");
   Editor.init();
   $("#selParentWorkItem").attr("value", "00000000-0000-0000-0000-000000000000");
   $("#selSprint").attr("value", "00000000-0000-0000-0000-000000000000");
   $("#selParentWorkItem").change();
   strictEqual($("#SelectProjectId").prop("disabled"), false, "Project not Disabled");
});

test('Project not disabled on sprint change if no parent and no sprint', function () {
   $("#selParentWorkItem").attr("value", "1");
   $("#selSprint").attr("value", "1");
   Editor.init();
   $("#selParentWorkItem").attr("value", "00000000-0000-0000-0000-000000000000");
   $("#selSprint").attr("value", "00000000-0000-0000-0000-000000000000");
   $("#selSprint").change();
   strictEqual($("#SelectProjectId").prop("disabled"), false, "Project not Disabled");
});

test('Project disabled on parent change if parent', function () {
   $("#selParentWorkItem").attr("value", "00000000-0000-0000-0000-000000000000");
   $("#selSprint").attr("value", "00000000-0000-0000-0000-000000000000");
   Editor.init();
   $("#selParentWorkItem").attr("value", "1");
   $("#selSprint").attr("value", "00000000-0000-0000-0000-000000000000");
   $("#selParentWorkItem").change();
   strictEqual($("#SelectProjectId").prop("disabled"), true, "Project Disabled");
});

test('Project disabled on sprint change if sprint', function () {
   $("#selParentWorkItem").attr("value", "00000000-0000-0000-0000-000000000000");
   $("#selSprint").attr("value", "00000000-0000-0000-0000-000000000000");
   Editor.init();
   $("#selParentWorkItem").attr("value", "00000000-0000-0000-0000-000000000000");
   $("#selSprint").attr("value", "1");
   $("#selSprint").change();
   strictEqual($("#SelectProjectId").prop("disabled"), true, "Project Disabled");
});

test('Sprint not disabled on parent change if no parent', function () {
   $("#selParentWorkItem").attr("value", "1");
   Editor.init();
   $("#selParentWorkItem").attr("value", "00000000-0000-0000-0000-000000000000");
   $("#selParentWorkItem").change();
   strictEqual($("#SelectSprintId").prop("disabled"), false, "Sprint not Disabled");
});

test('Sprint disabled on parent change if parent', function () {
   $("#selParentWorkItem").attr("value", "00000000-0000-0000-0000-000000000000");
   Editor.init();
   $("#selParentWorkItem").attr("value", "1");
   $("#selParentWorkItem").change();
   strictEqual($("#SelectSprintId").prop("disabled"), true, "Sprint Disabled");
});

test('Items in Sync on init', function () {
   Editor.init();
   strictEqual($("#SelectWorkItemTypeId").val(), $("#WorkItemTypeId").val(), "Work Item Type in sync");
   strictEqual($("#SelectProjectId").val(), $("#ProjectId").val(), "Project in sync");
   strictEqual($("#SelectSprintId").val(), $("#SprintId").val(), "Sprint in sync");
   strictEqual($("#SelectParentWorkItemId").val(), $("#ParentWorkItemId").val(), "Parent in sync");
   strictEqual($("#SelectAssignedToUserId").val(), $("#AssignedToUserId").val(), "Assigned to user in sync");
});

test('Work Item Type in sync after change', function () {
   $("#SelectWorkItemTypeId").val("1");
   Editor.init();
   $("#SelectWorkItemTypeId").val("2");
   $("#SelectWorkItemTypeId").change();
   strictEqual($("#SelectWorkItemTypeId").val(), $("#WorkItemTypeId").val(), "Work Item Type in sync");
});

test('Parent Work Item in sync after change', function () {
   $("#SelectParentWorkItemId").val("1");
   Editor.init();
   $("#SelectParentWorkItemId").val("2");
   $("#SelectParentWorkItemId").change();
   strictEqual($("#SelectParentWorkItemId").val(), $("#ParentWorkItemId").val(), "Parent in sync");
});

test('Project in sync after change', function () {
   $("#SelectProjectId").val("1");
   Editor.init();
   $("#SelectProjectId").val("2");
   $("#SelectProjectId").change();
   strictEqual($("#SelectProjectId").val(), $("#ProjectId").val(), "Project in sync");
});
test('Sprint in sync after change', function () {
   $("#SelectSprintId").val("1");
   Editor.init();
   $("#SelectSprintId").val("2");
   $("#SelectSprintId").change();
   strictEqual($("#SelectSprintId").val(), $("#SprintId").val(), "Sprint in sync");
});
test('Assigned To User in sync after change', function () {
   $("#SelectAssignedToUserId").val("1");
   Editor.init();
   $("#SelectAssignedToUserId").val("2");
   $("#SelectAssignedToUserId").change();
   strictEqual($("#SelectAssignedToUserId").val(), $("#AssignedToUserId").val(), "Assigned To User in sync");
});

function assertItemsAreActive() {
   strictEqual($("#Name").prop("readonly"), false, "Name not Readonly");
   ok(!($("#Name").hasClass("disabled")), "Name Disabled not Class");
   strictEqual($("#Description").prop("readonly"), false, "Description not Readonly");
   ok(!($("#Description").hasClass("disabled")), "Description not Disabled Class");
   strictEqual($("#SelectWorkItemTypeId").prop("disabled"), false, "Work Item Type not Disabled");
   strictEqual($("#SelectParentWorkItemId").prop("disabled"), false, "Parent Work Item not Disabled");
   strictEqual($("#SelectProjectId").prop("disabled"), false, "Project not Disabled");
   strictEqual($("#SelectSprintId").prop("disabled"), false, "Sprint not Disabled");
   strictEqual($("#SelectAssignedToUserId").prop("disabled"), false, "Assigned To User not Disabled");
   strictEqual($("#CreateNewTask").prop("disabled"), false, "Create New Task not Disabled");
}

function assertItemsAreInactive() {
   strictEqual($("#Name").prop("readonly"), true, "Name Readonly");
   ok($("#Name").hasClass("disabled"), "Name Disabled Class");
   strictEqual($("#Description").prop("readonly"), true, "Description Readonly");
   ok($("#Description").hasClass("disabled"), "Description Disabled Class");
   strictEqual($("#SelectWorkItemTypeId").prop("disabled"), true, "Work Item Type Disabled");
   strictEqual($("#SelectParentWorkItemId").prop("disabled"), true, "Parent Work Item Disabled");
   strictEqual($("#SelectProjectId").prop("disabled"), true, "Project Disabled");
   strictEqual($("#SelectSprintId").prop("disabled"), true, "Sprint Disabled");
   strictEqual($("#SelectAssignedToUserId").prop("disabled"), true, "Assigned To User Disabled");
   strictEqual($("#CreateNewTask").prop("disabled"), true, "Create New Task Disabled");
}