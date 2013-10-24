module('setupShowHideButton', {
   setup: function () {
   },
   teardown: function () {
      $.removeItem("TestShowFoo");
   }
});

// Button Label Tests
test('Button Initially Says Show', function () {
   setupShowHideButton("Test", $("#ShowHideToggle"), "Foo");
   strictEqual($("#ShowHideToggleLabel").text(), "Show Foo", "Button Label: Show Foo");
});

test('Button Toggles to Hide After Click', function () {
   setupShowHideButton("Test", $("#ShowHideToggle"), "Foo");
   $("#ShowHideToggle").click();
   strictEqual($("#ShowHideToggleLabel").text(), "Hide Foo", "Button Label: Hide Foo");
});

test('Button Toggles Back to Show After Another Click', function () {
   setupShowHideButton("Test", $("#ShowHideToggle"), "Foo");
   $("#ShowHideToggle").click();
   $("#ShowHideToggle").click();
   strictEqual($("#ShowHideToggleLabel").text(), "Show Foo", "Button Label: Show Foo");
});

// Hide/Show elements test
test('Initial State Hides Elements', function () {
   setupShowHideButton("Test", $("#ShowHideToggle"), "Foo");
   strictEqual($(".FooItemRow:hidden").length, 2, "Foo Items are Hidden");
   strictEqual($(".BarItemRow:hidden").length, 0, "Bar Items are not hidden");
});

test('Elements Shown After Click', function () {
   setupShowHideButton("Test", $("#ShowHideToggle"), "Foo");
   $("#ShowHideToggle").click();
   strictEqual($(".FooItemRow:hidden").length, 0, "Foo Items are not hidden");
   strictEqual($(".BarItemRow:hidden").length, 0, "Bar Items are not hidden");
});

test('Elements Hidden After Another Click', function () {
   setupShowHideButton("Test", $("#ShowHideToggle"), "Foo");
   $("#ShowHideToggle").click();
   $("#ShowHideToggle").click();
   $(".FooItemRow").each(function () {
      strictEqual($(this).css('opacity'), '0', "Foo Items are going opaque");
   });
   strictEqual($(".BarItemRow:hidden").length, 0, "Bar Items are not hidden");
});

// Local Storage Write
test('Show/Hide State starts out null', function () {
   setupShowHideButton("Test", $("#ShowHideToggle"), "Foo");
   strictEqual($.localStorage("TestShowFoo"), null, "Show Is Stored");
});

test('Show State Stored in Local Storage', function () {
   setupShowHideButton("Test", $("#ShowHideToggle"), "Foo");
   $("#ShowHideToggle").click();
   strictEqual($.localStorage("TestShowFoo"), true, "Show Is Stored");
});

test('Hide State Stored in Local Storage', function () {
   setupShowHideButton("Test", $("#ShowHideToggle"), "Foo");
   $("#ShowHideToggle").click();
   $("#ShowHideToggle").click();
   strictEqual($.localStorage("TestShowFoo"), false, "Show Is Stored");
});

// Local Storage Read
test('Initial State Shows Elements if TestShowFoo is true', function () {
   $.localStorage("TestShowFoo", true);
   setupShowHideButton("Test", $("#ShowHideToggle"), "Foo");
   strictEqual($(".FooItemRow:hidden").length, 0, "Foo Items are Hidden");
   strictEqual($(".BarItemRow:hidden").length, 0, "Bar Items are not hidden");
});