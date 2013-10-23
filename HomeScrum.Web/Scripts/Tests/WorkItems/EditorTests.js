test('Test', function () {
   strictEqual(Editor().test(), 'test');
});

test('Echo Test', function () {
   strictEqual(Editor().echo('this is a test'), 'this is a test');
});