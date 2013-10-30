test('Items Enabled on init if sprint open', function () {
   $("#selStatus").attr("data-IsOpenStatus", "True");
   Editor.init();
   assertItemsAreActive();
});

test('Items Disabled on init if sprint not open', function () {
   $("#selStatus").attr("data-IsOpenStatus", "False");
   Editor.init();
   assertItemsAreNotActive();
});

function assertItemsAreActive() {
   strictEqual($("#Name").prop("readonly"), false, "Name not Readonly");
   ok(!($("#Name").hasClass("disabled")), "Name Disabled does not have Class");
   strictEqual($("#Description").prop("readonly"), false, "Description not Readonly");
   ok(!($("#Description").hasClass("disabled")), "Description Disabled does not have Class");
   strictEqual($("#Goal").prop("readonly"), false, "Goal not Readonly");
   ok(!($("#Goal").hasClass("disabled")), "Goal Disabled does not have Class");
   strictEqual($("#SelectProjectId").prop('disabled'), false, "Project is not disabled");
}

function assertItemsAreNotActive() {
   strictEqual($("#Name").prop("readonly"), true, "Name Readonly");
   ok($("#Name").hasClass("disabled"), "Name Disabled has Class");
   strictEqual($("#Description").prop("readonly"), true, "Description Readonly");
   ok($("#Description").hasClass("disabled"), "Description Disabled has Class");
   strictEqual($("#Goal").prop("readonly"), true, "Goal Readonly");
   ok($("#Goal").hasClass("disabled"), "Goal Disabled has Class");
   strictEqual($("#SelectProjectId").prop('disabled'), true, "Project is disabled");
}

// The following items are enable/disabled based on sprint status
//   * Project Id
//
// The following are shown/hid based on sprint status
//   * backlog link
//   * task link
//
// Hide the backlog link if backlog is closed
// Hide the task link if task list is closed
//
// The Project Id is disabled if a backlog item is associated with the sprint
// The Project Id is disabled if a task is associated with the sprint
//
// Sync Project Id