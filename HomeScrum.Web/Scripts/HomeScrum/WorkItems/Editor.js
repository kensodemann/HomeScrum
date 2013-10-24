var Editor = (function() {

   var init = function () {
   };

   var test = function () {
      return "test";
   }

   var echo = function (val) {
      return val;
   }

   var vm = {
      init: init,
      test: test,
      echo: echo
   };

   return vm;
})();