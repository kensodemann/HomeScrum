test('Test', function () {
   strictEqual(Editor.test(), 'test');
});

test('Echo Test', function () {
   strictEqual(Editor.echo('this is a test'), 'this is a test');
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