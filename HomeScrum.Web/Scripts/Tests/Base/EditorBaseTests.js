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