var EditorBase = (function () {
   var editor = {
      init: init,
      submitButtonClicked: HandleSubmitButtonClicked,
      cancelButtonClicked: HandleCancelButtonClicked,
   };

   return editor;

   function init(submitClickHandler, cancelClickHandler) {
      SetupCancelButton(cancelClickHandler || HandleCancelButtonClicked);
      SetupSubmitButton(submitClickHandler || HandleSubmitButtonClicked);
      SetMainDataAccess();
      SetChildDataVisibility();
      ShowHideButtons();
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

   function ShowHideButtons(effect) {
      ShowHideCancelButton(effect);
      ShowHideCloseButton(effect);
   }

   function ShowHideCancelButton(effect) {
      var editMode = $("#Mode").val();
      if (editMode === "ReadOnly") {
         $("#CancelButton").hide(effect);
      }
      else {
         $("#CancelButton").show(effect);
      }
   }

   function ShowHideCloseButton(effect) {
      var editMode = $("#Mode").val();
      if (editMode === "ReadOnly") {
         $("#CloseButton").show(effect);
      }
      else {
         $("#CloseButton").hide(effect);
      }
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
         HandleModeChange();
      } else {
         $(".MainData:disabled").prop("disabled", false);
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

   function HandleCancelButtonClicked() {
      // In a Create, the cancel button should be wired to go back to the calling screen
      // In Read-Only mode, the cance button should be hidden
      // Edit should be the only mode we have to worry about, ever
      var editMode = $("#Mode").val();
      if (editMode === "Edit") {
         $("#Mode").val("ReadOnly");
         HandleModeChange();
      }
   }

   function SetupCancelButton(cancelClickHandler) {
      $("#CancelButton").click(function () {
         cancelClickHandler();
      })
   }

   function HandleModeChange() {
      SetSubmitButtonText();
      ShowHideButtons('fade');
      SetChildDataVisibility('fade');
      SetMainDataAccess();
   }
})();