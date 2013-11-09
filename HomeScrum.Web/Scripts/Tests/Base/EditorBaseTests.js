﻿// Initial state tests
test('Submit Button label is Edit if mode is ReadOnly on init', function () {
   $("#EditMode").val('ReadOnly');
   EditorBase.init();
   strictEqual($('#SubmitButton').text(), 'Edit');
});

test('Submit Button label is Done Editing if mode is Edit on init', function () {
   $("#EditMode").val('Edit');
   EditorBase.init();
   strictEqual($('#SubmitButton').text(), 'Done Editing');
});

test('Submit Button label is Create if mode is Create on init', function () {
   $("#EditMode").val('Create');
   EditorBase.init();
   strictEqual($('#SubmitButton').text(), 'Create');
});

test('Main Data text inputs read-only on init if EditMode is ReadOnly', function () {
   $('#EditMode').val('ReadOnly');
   EditorBase.init();
   strictEqual($("#ParentText").prop("readonly"), true, "ParentText is Readonly");
   ok($("#ParentText").hasClass("disabled"), "ParentText has Disabled Class");
});

test('Main Data non-text inputs disbled on init if EditMode is ReadOnly', function () {
   $('#EditMode').val('ReadOnly');
   EditorBase.init();
   strictEqual($("#ParentCheckbox").prop('disabled'), true, "ParentCheckbox is disabled");
   strictEqual($("#ParentSelect").prop('disabled'), true, "ParentSelect is disabled");
});

test('Child Data text inputs not read-only on init if EditMode is ReadOnly', function () {
   $('#EditMode').val('ReadOnly');
   EditorBase.init();
   strictEqual($("#ChildText").prop("readonly"), false, "ChildText not Readonly");
   ok(!($("#ChildText").hasClass("disabled")), "ChildText does not have Disabled Class");
});

test('Main Data non-text inputs not disabled on init if EditMode is ReadOnly', function () {
   $('#EditMode').val('ReadOnly');
   EditorBase.init();
   strictEqual($("#ChildCheckbox").prop('disabled'), false, "ChildCheckbox is not disabled");
   strictEqual($("#ChildSelect").prop('disabled'), false, "ChildSelect is not disabled");
});

// "Button" press 6tests