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
}

function assertItemsAreNotActive() {
   strictEqual($("#Name").prop("readonly"), true, "Name Readonly");
   ok($("#Name").hasClass("disabled"), "Name Disabled has Class");
}

// The following items are enable/disabled based on sprint status
//   * Name
//   * Description
//   * Goal
//   * Name
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