test('Backlog link visible if Can Add Backlog', function () {
   $('#CanAddBacklog').val('True');
   Details.init();
   ok($('#BacklogLink').is(':visible'), "Backlog link is visible");
});

test('Backlog link hidden if Cannot Add Backlog', function () {
   $('#CanAddBacklog').val('False');
   Details.init();
   ok($('#BacklogLink').is(':hidden'), "Backlog link is hidden");
});

test('Backlog link visible if Can Add Task', function () {
   $('#CanAddTasks').val('True');
   Details.init();
   ok($('#TaskListLink').is(':visible'), "Task link is visible");
});

test('Backlog link hidden if Cannot Add Task', function () {
   $('#CanAddTasks').val('False');
   Details.init();
   ok($('#TaskListLink').is(':hidden'), "Task link is hidden");
});