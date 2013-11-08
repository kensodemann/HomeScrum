var EditorBase = (function () {
   var editor = {
      init: init
   };

   return editor;

   function init() {
      SetSubmitButtonText();
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

})();