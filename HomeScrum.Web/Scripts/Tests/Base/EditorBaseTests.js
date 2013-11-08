test('Submit Button label is Edit if mode is ReadOnly on init', function () {
   $("#EditMode").val('ReadOnly');
   EditorBase.init();
   strictEqual($('#SubmitButton').text(), 'Edit');
});