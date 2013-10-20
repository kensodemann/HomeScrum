module('setupShowHideButton', {
    setup: function () {
    },
    teardown: function () {
    }
});

test('Button Initially Says Show', function () {
    var button = $("#ShowHideToggle");
    setupShowHideButton("Me", $("#ShowHideToggle"), "Foo");
    strictEqual($("#ShowHideToggleLabel").text(), "Show Foo");
});