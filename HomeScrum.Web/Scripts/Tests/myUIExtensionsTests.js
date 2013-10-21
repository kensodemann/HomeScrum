module('setupShowHideButton', {
   setup: function () {
   },
   teardown: function () {
      $.removeItem("TestShowFoo");
   }
});

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