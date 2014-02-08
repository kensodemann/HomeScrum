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