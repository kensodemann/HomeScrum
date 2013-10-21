module('setupShowHideButton', {
   setup: function () {
   },
   teardown: function () {
   }
});

test('Button Initially Says Show', function () {
   setupShowHideButton("Me", $("#ShowHideToggle"), "Foo");
   strictEqual($("#ShowHideToggleLabel").text(), "Show Foo", "Button Label: Show Foo");
});

test('Initial State Hides Elements', function () {
   setupShowHideButton("Me", $("#ShowHideToggle"), "Foo");
   strictEqual($(".FooItemRow:hidden").length, 2, "Foo Items are Hidden");
   strictEqual($(".BarItemRow:hidden").length, 0, "Bar Items are not hidden");
});