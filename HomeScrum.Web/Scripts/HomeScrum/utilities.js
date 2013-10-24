var Utilities = (function () {
   ShowHideButton = (function () {
      var _hideThese = "";
      var _controller = "";

      function showItems(toggleButton, effect) {
         $("." + _hideThese + "ItemRow").show(effect);
         toggleButton.button("option", "label", "Hide " + _hideThese);
      }

      function hideItems(toggleButton, effect) {
         $("." + _hideThese + "ItemRow").hide(effect);
         toggleButton.button("option", "label", "Show " + _hideThese);
      }

      function shouldShow() {
         return $.localStorage(_controller + "Show" + _hideThese);
      }

      function saveShowState(value) {
         $.localStorage(_controller + "Show" + _hideThese, value);
      }

      function setInitialState(toggleButton) {
         if (shouldShow()) {
            toggleButton.prop("checked", true);
            showItems(toggleButton);
         }
         else {
            toggleButton.prop("checked", false);
            hideItems(toggleButton);
         }
         toggleButton.button("refresh");
      }

      function init(controller, toggleButton, hideThese) {
         _controller = controller;
         _hideThese = hideThese;
         toggleButton.button();
         setInitialState(toggleButton);

         toggleButton.click(
             function () {
                if ($(this).is(":checked")) {
                   showItems($(this), "fade");
                }
                else {
                   hideItems($(this), "fade");
                }
                saveShowState($(this).is(":checked"));
             });
      }

      return {
         init: init
      };
   })();

   function syncHiddenElement(fromElement) {
      var hiddenElementId = fromElement.id.substr(6);
      $("#" + hiddenElementId).val($(fromElement).val());
   }

   return {
      setupShowHideButton: ShowHideButton.init,
      syncHiddenElement: syncHiddenElement
   };
})();

