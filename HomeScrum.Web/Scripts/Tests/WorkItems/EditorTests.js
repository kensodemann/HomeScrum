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
   $("#WorkItemTypeId").change();
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
   $("#WorkItemTypeId").change();
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
   $("#WorkItemTypeId").change();
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
   $("#WorkItemTypeId").change();
   ok($("#AssignedToUserDiv").is(":visible"));
});

// Project & Sprint Setting
test("Project ID is set to parent's when parent selected", function () {
   Editor.init();
   var expected = "3";
   $("#selParentWorkItem").attr("value", "1");
   $("#selParentWorkItem").attr("data-ProjectId", expected);
   $("#ParentWorkItemId").change();
   strictEqual($("#ProjectId").val(), expected);
});

test("Sprint ID is set to parent's when parent selected", function () {
   Editor.init();
   var expected = "5";
   $("#selParentWorkItem").attr("value", "1");
   $("#selParentWorkItem").attr("data-SprintId", expected);
   $("#ParentWorkItemId").change();
   strictEqual($("#SprintId").val(), expected);
});

test("Project ID is set to sprint's when sprint selected", function () {
   Editor.init();
   var expected = "7";
   $("#selSprint").attr("value", "1");
   $("#selSprint").attr("data-ProjectId", expected);
   $("#SprintId").change();
   strictEqual($("#ProjectId").val(), expected);
});

// Active & Inactive based on status
test('Items Active on status change if status not complete', function () {
   $("#selStatus").attr("data-IsOpenStatus", "False");
   $("#Mode").val("Edit");
   Editor.init();
   $("#selStatus").attr("data-IsOpenStatus", "True");
   $("#StatusId").change();
   assertItemsAreActive();
});

test('Items Inactive on status change if status complete', function () {
   $("#selStatus").attr("data-IsOpenStatus", "True");
   $("#Mode").val("Edit");
   Editor.init();
   $("#selStatus").attr("data-IsOpenStatus", "False");
   $("#StatusId").change();
   assertItemsAreInactive();
});

test('Add New Task not disabled on status change if task list is not closed', function () {
   $("#selSprint").attr("data-TaskListIsClosed", "True");
   Editor.init();
   $("#selSprint").attr("data-TaskListIsClosed", "False");
   $("#SprintId").change();
   strictEqual($("#CreateNewTask").prop("disabled"), false, "Create New Task not Disabled");
});

test('Add New Task disabled on status change if task list is closed', function () {
   $("#selSprint").attr("data-TaskListIsClosed", "False");
   Editor.init();
   $("#selSprint").attr("data-TaskListIsClosed", "True");
   $("#SprintId").change();
   strictEqual($("#CreateNewTask").prop("disabled"), true, "Create New Task Disabled");
});

test('Project not disabled on parent change if no parent and no sprint', function () {
   $("#selParentWorkItem").attr("value", "1");
   $("#selSprint").attr("value", "1");
   Editor.init();
   $("#selParentWorkItem").attr("value", "00000000-0000-0000-0000-000000000000");
   $("#selSprint").attr("value", "00000000-0000-0000-0000-000000000000");
   $("#selParentWorkItem").change();
   strictEqual($("#ProjectId").prop("disabled"), false, "Project not Disabled");
});

test('Project not disabled on sprint change if no parent and no sprint', function () {
   $("#selParentWorkItem").attr("value", "1");
   $("#selSprint").attr("value", "1");
   Editor.init();
   $("#selParentWorkItem").attr("value", "00000000-0000-0000-0000-000000000000");
   $("#selSprint").attr("value", "00000000-0000-0000-0000-000000000000");
   $("#selSprint").change();
   strictEqual($("#ProjectId").prop("disabled"), false, "Project not Disabled");
});

test('Project disabled on parent change if parent', function () {
   $("#selParentWorkItem").attr("value", "00000000-0000-0000-0000-000000000000");
   $("#selSprint").attr("value", "00000000-0000-0000-0000-000000000000");
   Editor.init();
   $("#selParentWorkItem").attr("value", "1");
   $("#selSprint").attr("value", "00000000-0000-0000-0000-000000000000");
   $("#selParentWorkItem").change();
   strictEqual($("#ProjectId").prop("disabled"), true, "Project Disabled");
});

test('Project disabled on sprint change if sprint', function () {
   $("#selParentWorkItem").attr("value", "00000000-0000-0000-0000-000000000000");
   $("#selSprint").attr("value", "00000000-0000-0000-0000-000000000000");
   Editor.init();
   $("#selParentWorkItem").attr("value", "00000000-0000-0000-0000-000000000000");
   $("#selSprint").attr("value", "1");
   $("#selSprint").change();
   strictEqual($("#ProjectId").prop("disabled"), true, "Project Disabled");
});

test('Sprint not disabled on parent change if no parent', function () {
   $("#selParentWorkItem").attr("value", "1");
   Editor.init();
   $("#selParentWorkItem").attr("value", "00000000-0000-0000-0000-000000000000");
   $("#selParentWorkItem").change();
   strictEqual($("#SprintId").prop("disabled"), false, "Sprint not Disabled");
});

test('Sprint disabled on parent change if parent', function () {
   $("#selParentWorkItem").attr("value", "00000000-0000-0000-0000-000000000000");
   Editor.init();
   $("#selParentWorkItem").attr("value", "1");
   $("#selParentWorkItem").change();
   strictEqual($("#SprintId").prop("disabled"), true, "Sprint Disabled");
});

// Points / Points Remaining Enable / Disable
test('Points enabled on init if work not started', function () {
   $("#selStatus").attr("data-WorkStarted", "False");
   $("#Mode").val("Edit");
   Editor.init();
   strictEqual($("#Points").prop("disabled"), false);
});

test('Points disable on init if work started', function () {
   $("#selStatus").attr("data-WorkStarted", "True");
   $("#Mode").val("Edit");
   Editor.init();
   strictEqual($("#Points").prop("disabled"), true);
});

test('Points Remaining enabled on init if status is open', function () {
   $("#selStatus").attr("data-IsOpenStatus", "True");
   $("#selStatus").attr("data-WorkStarted", "True");
   $("#Mode").val("Edit");
   Editor.init();
   strictEqual($("#PointsRemaining").prop("disabled"), false);
});

test('Points Remaining disable on init if status is not open', function () {
   $("#selStatus").attr("data-isopenstatus", "False");
   $("#selStatus").attr("data-WorkStarted", "True");
   $("#Mode").val("Edit");
   Editor.init();
   strictEqual($("#PointsRemaining").prop("disabled"), true);
});

test('Points Enabled on status change if work not started', function () {
   $("#selStatus").attr("data-WorkStarted", "True");
   $("#Mode").val("Edit");
   Editor.init();
   $("#selStatus").attr("data-WorkStarted", "False");
   $("#StatusId").change();
   strictEqual($("#Points").prop("disabled"), false);
});

test('Points Disabled on status change if work started', function () {
   $("#selStatus").attr("data-WorkStarted", "False");
   $("#Mode").val("Edit");
   Editor.init();
   $("#selStatus").attr("data-WorkStarted", "True");
   $("#StatusId").change();
   strictEqual($("#Points").prop("disabled"), true);
});

test('Points hidden on init for items that can have child tasks', function () {
   $("#selStatus").attr("data-WorkStarted", "False");
   $("#Mode").val("Edit");
   $("#selWorkItemType").attr("data-canhavechildren", "True");
   Editor.init();
   ok($("#PointsArea").is(":hidden"));
});

test('Points Remaining hidden on type change items that can have child tasks', function () {
   $("#selStatus").attr("data-WorkStarted", "False");
   $("#Mode").val("Edit");
   $("#selWorkItemType").attr("data-canhavechildren", "False");
   Editor.init();
   $("#selWorkItemType").attr("data-canhavechildren", "True");
   $("#WorkItemTypeId").change();
   stop();
   setTimeout(function () {
      ok($("#PointsArea").is(":hidden"));
      start();
   }, 1000);
});

// Points and Points Remaining Values
test('Points Remaining Value is Points Value on Points Value Change', function () {
   $("#Points").val(8);
   $("#PointsRemaining").val(2);
   Editor.init();
   $("#Points").val(4);
   $("#Points").change();
   equal($("#PointsRemaining").val(), 4);
});

test('Points Remaining set to zero on status change if status not open', function () {
   $("#selStatus").attr("data-IsOpenStatus", "True");
   $("#Points").val(8);
   $("#PointsRemaining").val(2);
   Editor.init();
   $("#selStatus").attr("data-IsOpenStatus", "False");
   $("#StatusId").change();
   equal($("#PointsRemaining").val(), 0);
});


function assertItemsAreActive() {
   strictEqual($("#Name").prop("readonly"), false, "Name not Readonly");
   ok(!($("#Name").hasClass("disabled")), "Name Disabled not Class");
   strictEqual($("#Description").prop("readonly"), false, "Description not Readonly");
   ok(!($("#Description").hasClass("disabled")), "Description not Disabled Class");
   strictEqual($("#WorkItemTypeId").prop("disabled"), false, "Work Item Type not Disabled");
   strictEqual($("#ParentWorkItemId").prop("disabled"), false, "Parent Work Item not Disabled");
   strictEqual($("#ProjectId").prop("disabled"), false, "Project not Disabled");
   strictEqual($("#SprintId").prop("disabled"), false, "Sprint not Disabled");
   strictEqual($("#AssignedToUserId").prop("disabled"), false, "Assigned To User not Disabled");
   strictEqual($("#CreateNewTask").prop("disabled"), false, "Create New Task not Disabled");
   strictEqual($("#PointsRemaining").prop("disabled"), false, "Points Remaining not Disabled");
}

function assertItemsAreInactive() {
   strictEqual($("#Name").prop("readonly"), true, "Name Readonly");
   ok($("#Name").hasClass("disabled"), "Name Disabled Class");
   strictEqual($("#Description").prop("readonly"), true, "Description Readonly");
   ok($("#Description").hasClass("disabled"), "Description Disabled Class");
   strictEqual($("#WorkItemTypeId").prop("disabled"), true, "Work Item Type Disabled");
   strictEqual($("#ParentWorkItemId").prop("disabled"), true, "Parent Work Item Disabled");
   strictEqual($("#ProjectId").prop("disabled"), true, "Project Disabled");
   strictEqual($("#SprintId").prop("disabled"), true, "Sprint Disabled");
   strictEqual($("#AssignedToUserId").prop("disabled"), true, "Assigned To User Disabled");
   strictEqual($("#CreateNewTask").prop("disabled"), true, "Create New Task Disabled");
   strictEqual($("#PointsRemaining").prop("disabled"), true, "Points Remaining Disabled");
}