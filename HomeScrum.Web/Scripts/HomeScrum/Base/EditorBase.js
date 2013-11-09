﻿var EditorBase = (function () {
   var editor = {
      init: init
   };

   return editor;

   function init() {
      SetSubmitButtonText();
      SetMainDataAccess();
   }

   function SetSubmitButtonText() {
      var editMode = $("#EditMode").val();
      if (editMode === "ReadOnly") {
         $("#SubmitButton").text("Edit");
      } else if (editMode === "Edit") {
         $("#SubmitButton").text("Done Editing");
      } else if (editMode === "Create") {
         $("#SubmitButton").text("Create");
      }
   }

   function SetMainDataAccess() {
      var els = $(".MainData");
      if ($("#EditMode").val() === 'ReadOnly') {
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
      if (el.is(":text")) {
         el.prop('readonly', true);
         el.addClass('disabled');
      } else if (el.is(':input')) {
         el.prop('disabled', true);
      }
   }

   function Enable(element) {
      var el = $(element);
      if (el.is(":text")) {
         el.prop('readonly', false);
         el.removeClass('disabled');
      } else if (el.is(':input')) {
         el.prop('disabled', false);
      }
   }

})();