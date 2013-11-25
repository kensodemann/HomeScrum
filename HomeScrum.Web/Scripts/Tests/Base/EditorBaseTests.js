// Initial state tests
test('Submit Button label is Edit if mode is ReadOnly on init', function () {
   $("#Mode").val('ReadOnly');
   EditorBase.init();
   strictEqual($('#SubmitButton').text(), 'Edit');
});

test('Submit Button label is Done Editing if mode is Edit on init', function () {
   $("#Mode").val('Edit');
   EditorBase.init();
   strictEqual($('#SubmitButton').text(), 'Done Editing');
});

test('Submit Button label is Create if mode is Create on init', function () {
   $("#Mode").val('Create');
   EditorBase.init();
   strictEqual($('#SubmitButton').text(), 'Create');
});

test('Main Data text inputs read-only on init if Mode is ReadOnly', function () {
   $('#Mode').val('ReadOnly');
   EditorBase.init();
   strictEqual($("#ParentText").prop("readonly"), true, "ParentText is Readonly");
   ok($("#ParentText").hasClass("disabled"), "ParentText has Disabled Class");
});

test('Main Data non-text inputs disbled on init if Mode is ReadOnly', function () {
   $('#Mode').val('ReadOnly');
   EditorBase.init();
   strictEqual($("#ParentCheckbox").prop('disabled'), true, "ParentCheckbox is disabled");
   strictEqual($("#ParentSelect").prop('disabled'), true, "ParentSelect is disabled");
});

test('Child Data text inputs not read-only on init if Mode is ReadOnly', function () {
   $('#Mode').val('ReadOnly');
   EditorBase.init();
   strictEqual($("#ChildText").prop("readonly"), false, "ChildText not Readonly");
   ok(!($("#ChildText").hasClass("disabled")), "ChildText does not have Disabled Class");
});

test('Main Data non-text inputs not disabled on init if Mode is ReadOnly', function () {
   $('#Mode').val('ReadOnly');
   EditorBase.init();
   strictEqual($("#ChildCheckbox").prop('disabled'), false, "ChildCheckbox is not disabled");
   strictEqual($("#ChildSelect").prop('disabled'), false, "ChildSelect is not disabled");
});

test('Main Data text inputs enabled on init if Mode is Create', function () {
   $('#Mode').val('Create');
   EditorBase.init();
   strictEqual($("#ParentText").prop("readonly"), false, "ParentText is not Readonly");
   ok(!($("#ParentText").hasClass("disabled")), "ParentText does not have Disabled Class");
});

test('Main Data non-text inputs enbled on init if Mode is Create', function () {
   $('#Mode').val('Create');
   EditorBase.init();
   strictEqual($("#ParentCheckbox").prop('disabled'), false, "ParentCheckbox is enabled");
   strictEqual($("#ParentSelect").prop('disabled'), false, "ParentSelect is enabled");
});

test('Child Data visible on init if Mode is ReadOnly', function () {
   $('#Mode').val('ReadOnly');
   EditorBase.init();
   var childElements = $('.ChildData');
   for (var i = 0; i < childElements.length; i++) {
      ok($(childElements[i]).is(':visible'));
   }
});

test('Child Data hidden on init if Mode is Create', function () {
   $('#Mode').val('Create');
   EditorBase.init();
   var childElements = $('.ChildData');
   for (var i = 0; i < childElements.length; i++) {
      ok($(childElements[i]).is(':hidden'));
   }
});


// Cancel & Close button tests
test('Cancel button visible in Edit mode on init', function () {
   $("#Mode").val("Edit");
   EditorBase.init();
   ok($("#CancelButton").is(":visible"));
});

test('Cancel button visible in Create mode on init', function () {
   $("#Mode").val("Create");
   EditorBase.init();
   ok($("#CancelButton").is(":visible"));
});

test('Cancel button hidden in Read-Only mode on init', function () {
   $("#Mode").val("ReadOnly");
   EditorBase.init();
   ok($("#CancelButton").is(":hidden"));
});

test('Close button hidden in Edit mode on init', function () {
   $("#Mode").val("Edit");
   EditorBase.init();
   ok($("#CloseButton").is(":hidden"));
});

test('Close button hidden in Create mode on init', function () {
   $("#Mode").val("Create");
   EditorBase.init();
   ok($("#CloseButton").is(":hidden"));
});

test('Close button visible in Read-Only mode on init', function () {
   $("#Mode").val("ReadOnly");
   EditorBase.init();
   ok($("#CloseButton").is(":visible"));
});

test('Cancel button visible Close button hidden in Edit mode on submit click', function () {
   // Mode should go from read-only to Edit on click (see test that verifies this)
   $("#Mode").val("ReadOnly");
   EditorBase.init();
   $("#SubmitButton").click();
   ok($("#CancelButton").is(":visible"));
   stop();
   setTimeout(function () {
      ok($("#CloseButton").is(":hidden"));
      start();
   }, 1000);
});

test('Mode goes from Edit to ReadOnly on cancel click', function () {
   $('#Mode').val('Edit');
   EditorBase.init();
   $("#CancelButton").click();
   strictEqual($('#Mode').val(), 'ReadOnly');
});

test("Child Data Visible on cancel click", function () {
   $('#Mode').val('Edit');
   EditorBase.init();
   $("#CancelButton").click();
   var els = $('.ChildData');
   for (var i = 0; i < els.length; i++) {
      ok($(els[i]).is(':visible'));
   }
});

test("Main Data Disabled on cancel click", function () {
   $('#Mode').val('ReadOnly');
   EditorBase.init();
   $("#CancelButton").click();
   var els = $('.MainData');
   for (var i = 0; i < els.length; i++) {
      var el = $(els[i]);
      if (el.is(':text') || el.is('textarea')) {
         ok($(els[i]).hasClass("disabled"), "Has Disabled Class");
         strictEqual($(els[i]).prop('readonly'), true, 'Is readonly');
      } else if (el.is(':input')) {
         strictEqual($(els[i]).prop('disabled'), true, 'Is disabled');
      }
   }
});

// Submit Button press tests
test('Mode goes from ReadOnly to Edit on submit click', function () {
   $('#Mode').val('ReadOnly');
   EditorBase.init();
   $("#SubmitButton").click();
   strictEqual($('#Mode').val(), 'Edit');
});

test("Button Text goes to 'Done Editing' on submit click", function () {
   $('#Mode').val('ReadOnly');
   EditorBase.init();
   $("#SubmitButton").click();
   strictEqual($('#SubmitButton').text(), 'Done Editing');
});

test("Child Data Hidden on submit click", function () {
   $('#Mode').val('ReadOnly');
   EditorBase.init();
   $("#SubmitButton").click();
   var els = $('.ChildData');
   stop();
   setTimeout(function () {
      for (var i = 0; i < els.length; i++) {
         ok($(els[i]).is(':hidden'));
      }
      start();
   }, 1000);
});

test("Main Data Enabled on submit click", function () {
   $('#Mode').val('ReadOnly');
   EditorBase.init();
   $("#SubmitButton").click();
   var els = $('.MainData');
   for (var i = 0; i < els.length; i++) {
      var el = $(els[i]);
      if (el.is(':text') || el.is('textarea')) {
         ok(!($(els[i]).hasClass("disabled")), "Does not have Disabled Class");
         strictEqual($(els[i]).prop('readonly'), false, 'Is not readonly');
      } else if (el.is(':input')) {
         strictEqual($(els[i]).prop('disabled'), false, 'Is not disabled');
      }
   }
});