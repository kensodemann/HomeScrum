test('All Child Data Hidden if cannot have children', function () {
   $("#CanHaveChildren").val("False");
   Details.init();
   ok($(".ChildData").is(":hidden"));
});

test('All Child Data Visible if can have parent', function () {
   $("#CanHaveChildren").val("True");
   Details.init();
   ok($(".ChildData").is(":visible"));
});

test('Add New Tasks Button Hidden if cannot have children', function () {
   $("#CanHaveChildren").val("False");
   Details.init();
   ok($("#AddNewButton").is(":hidden"));
});

test('Add New Tasks Button Visible if can have parent', function () {
   $("#CanHaveChildren").val("True");
   Details.init();
   ok($("#AddNewButton").is(":visible"));
});