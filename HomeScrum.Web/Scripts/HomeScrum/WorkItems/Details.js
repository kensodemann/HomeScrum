var Details = (function () {

   function init() {
      showHideChildData();
   }

   function showHideChildData() {
      if ($("#CanHaveChildren").val() === "True") {
         $(".ChildData").show();
         $("#AddNewButton").show();
      } else {
         $(".ChildData").hide();
         $("#AddNewButton").hide();
      }
   }


   return {
      init: init
   };
})();