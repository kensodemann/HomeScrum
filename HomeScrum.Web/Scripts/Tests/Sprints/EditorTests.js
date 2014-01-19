test('Items Shown on init if sprint open', function () {
   $("#selStatus").attr("data-IsOpenStatus", "True");
   Editor.init();
   assertItemsAreShown();
});

test('Items Hid on init if sprint not open', function () {
   $("#selStatus").attr("data-IsOpenStatus", "False");
   Editor.init();
   assertItemsAreHidden();
});

test('Backlog and Task List links shown on init if status open, task list open, and backlog open', function () {
   $("#selStatus").attr("data-IsOpenStatus", "True");
   $("#selStatus").attr("data-BacklogIsClosed", "False");
   $("#selStatus").attr("data-TaskListIsClosed", "False");
   Editor.init();
   ok($("#TaskListLink").is(":visible"), "Task List link is visible");
   ok($("#BacklogLink").is(":visible"), "Backlog link is visible");
});

test('Backlog link hidden on init if status open and backlog closed', function () {
   $("#selStatus").attr("data-IsOpenStatus", "True");
   $("#selStatus").attr("data-BacklogIsClosed", "True");
   $("#selStatus").attr("data-TaskListIsClosed", "False");
   Editor.init();
   ok($("#TaskListLink").is(":visible"), "Task List link is visible");
   ok($("#BacklogLink").is(":hidden"), "Backlog link is hidden");
});

test('Task List Link hidden on init if status open and task list closed', function () {
   $("#selStatus").attr("data-IsOpenStatus", "True");
   $("#selStatus").attr("data-BacklogIsClosed", "False");
   $("#selStatus").attr("data-TaskListIsClosed", "True");
   Editor.init();
   ok($("#TaskListLink").is(":hidden"), "Task List link is hidden");
   ok($("#BacklogLink").is(":visible"), "Backlog link is visible");
});

test('Items Enabled on sprint change if sprint open', function () {
   $("#selStatus").attr("data-IsOpenStatus", "False");
   Editor.init();
   $("#selStatus").attr("data-IsOpenStatus", "True");
   $("#StatusId").change();
   assertItemsAreActive();
});

test('Items Disabled on sprint change if sprint not open', function () {
   $("#selStatus").attr("data-IsOpenStatus", "True");
   Editor.init();
   $("#selStatus").attr("data-IsOpenStatus", "False");
   $("#StatusId").change();
   assertItemsAreNotActive();
});

test('Items Shown on sprint change if sprint open', function () {
   $("#selStatus").attr("data-IsOpenStatus", "False");
   Editor.init();
   $("#selStatus").attr("data-IsOpenStatus", "True");
   $("#StatusId").change();
   assertItemsAreShown();
});

test('Items Hidden on status change if sprint not open', function () {
   $("#selStatus").attr("data-IsOpenStatus", "True");
   Editor.init();
   $("#selStatus").attr("data-IsOpenStatus", "False");
   $("#StatusId").change();
   assertItemsAreHidden();
});

test('Backlog and Task List links shown on status change if status open, task list open, and backlog open', function () {
   $("#selStatus").attr("data-IsOpenStatus", "True");
   $("#selStatus").attr("data-BacklogIsClosed", "True");
   $("#selStatus").attr("data-TaskListIsClosed", "True");
   Editor.init();
   $("#selStatus").attr("data-BacklogIsClosed", "False");
   $("#selStatus").attr("data-TaskListIsClosed", "False");
   $("#StatusId").change();
   ok($("#TaskListLink").is(":visible"), "Task List link is visible");
   ok($("#BacklogLink").is(":visible"), "Backlog link is visible");
});

test('Backlog link hidden on status change if status open and backlog closed', function () {
   $("#selStatus").attr("data-IsOpenStatus", "True");
   $("#selStatus").attr("data-BacklogIsClosed", "False");
   $("#selStatus").attr("data-TaskListIsClosed", "False");
   Editor.init();
   $("#selStatus").attr("data-BacklogIsClosed", "True");
   $("#StatusId").change();
   ok($("#TaskListLink").is(":visible"), "Task List link is visible");
   ok($("#BacklogLink").is(":hidden"), "Backlog link is hidden");
});

test('Task List Link hidden on status change if status open and task list closed', function () {
   $("#selStatus").attr("data-IsOpenStatus", "True");
   $("#selStatus").attr("data-BacklogIsClosed", "False");
   $("#selStatus").attr("data-TaskListIsClosed", "False");
   Editor.init();
   $("#selStatus").attr("data-TaskListIsClosed", "True");
   $("#StatusId").change();
   ok($("#TaskListLink").is(":hidden"), "Task List link is hidden");
   ok($("#BacklogLink").is(":visible"), "Backlog link is visible");
});

test('Capacity Enabled if backlog open on status change', function () {
   $("#selStatus").attr("data-IsOpenStatus", "True");
   $("#selStatus").attr("data-BacklogIsClosed", "True");
   $("#selStatus").attr("data-TaskListIsClosed", "False");
   Editor.init();
   $("#selStatus").attr("data-BacklogIsClosed", "False");
   $("#StatusId").change();
   strictEqual($("#Capacity").prop("disabled"), false);
});

test('Capacity Disabled if backlog closed on status change', function () {
   $("#selStatus").attr("data-IsOpenStatus", "True");
   $("#selStatus").attr("data-BacklogIsClosed", "False");
   $("#selStatus").attr("data-TaskListIsClosed", "False");
   Editor.init();
   $("#selStatus").attr("data-BacklogIsClosed", "True");
   $("#StatusId").change();
   strictEqual($("#Capacity").prop("disabled"), true);
});

function assertItemsAreActive() {
   strictEqual($("#Name").prop("readonly"), false, "Name not Readonly");
   ok(!($("#Name").hasClass("disabled")), "Name Disabled does not have Class");
   strictEqual($("#Description").prop("readonly"), false, "Description not Readonly");
   ok(!($("#Description").hasClass("disabled")), "Description Disabled does not have Class");
   strictEqual($("#Goal").prop("readonly"), false, "Goal not Readonly");
   ok(!($("#Goal").hasClass("disabled")), "Goal Disabled does not have Class");
   strictEqual($("#ProjectId").prop('disabled'), false, "Project is not disabled");
}

function assertItemsAreNotActive() {
   strictEqual($("#Name").prop("readonly"), true, "Name Readonly");
   ok($("#Name").hasClass("disabled"), "Name Disabled has Class");
   strictEqual($("#Description").prop("readonly"), true, "Description Readonly");
   ok($("#Description").hasClass("disabled"), "Description Disabled has Class");
   strictEqual($("#Goal").prop("readonly"), true, "Goal Readonly");
   ok($("#Goal").hasClass("disabled"), "Goal Disabled has Class");
   strictEqual($("#ProjectId").prop('disabled'), true, "Project is disabled");
}

function assertItemsAreShown() {
   ok($("#TaskListLink").is(":visible"), "Task List link is visible");
   ok($("#BacklogLink").is(":visible"), "Backlog link is visible");
}

function assertItemsAreHidden() {
   ok($("#TaskListLink").is(":hidden"), "Task List link is hidden");
   ok($("#BacklogLink").is(":hidden"), "Backlog link is hidden");
}