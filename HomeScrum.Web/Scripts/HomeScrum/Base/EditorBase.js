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
      }
   }

})();