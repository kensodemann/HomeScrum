var EditorBase = (function () {
   var editor = {
      init: init,
      submitButtonClicked: HandleSubmitButtonClicked,
   };

   return editor;

   function init(submitClickHandler) {
      SetupSubmitButton(submitClickHandler || HandleSubmitButtonClicked);
      SetMainDataAccess();
      SetChildDataVisibility();
   }

   function SetSubmitButtonText() {
      $('#SubmitButton').button("destroy");
      var editMode = $("#Mode").val();
      if (editMode === "ReadOnly") {
         $("#SubmitButton").text("Edit");
      } else if (editMode === "Edit") {
         $("#SubmitButton").text("Done Editing");
      } else if (editMode === "Create") {
         $("#SubmitButton").text("Create");
      }
      $('#SubmitButton').button();
   }

   function SetMainDataAccess() {
      var els = $(".MainData");
      if ($("#Mode").val() === 'ReadOnly') {
         for (var i = 0; i < els.length; i++) {
            Disable(els[i]);
         }
      } else {
         for (var i = 0; i < els.length; i++) {
            Enable(els[i]);
         }
      }
   }

   function Disable(element) {
      var el = $(element);
      if (el.is(":text") || el.is("textarea")) {
         el.prop('readonly', true);
         el.addClass('disabled');
      } else if (el.is(':input')) {
         el.prop('disabled', true);
      }
   }

   function Enable(element) {
      var el = $(element);
      if (el.is(":text") || el.is("textarea")) {
         el.prop('readonly', false);
         el.removeClass('disabled');
      } else if (el.is(':input')) {
         el.prop('disabled', false);
      }
   }

   function SetChildDataVisibility(effect) {
      var els = $(".ChildData");
      if ($("#Mode").val() === 'ReadOnly') {
         for (var i = 0; i < els.length; i++) {
            $(els[i]).show(effect);
         }
      } else {
         for (var i = 0; i < els.length; i++) {
            $(els[i]).hide(effect);
         }
      }
   }

   function HandleSubmitButtonClicked() {
      if ($('#Mode').val() === 'ReadOnly') {
         $('#Mode').val('Edit');
         SetSubmitButtonText();
         SetChildDataVisibility('fade');
         SetMainDataAccess();
      } else {
         $("form#Editor").submit();
      }
   }

   function SetupSubmitButton(submitClickHandler) {
      $('#SubmitButton').button();
      SetSubmitButtonText();

      $('#SubmitButton').click(function () {
         submitClickHandler();
      });
   }
})();