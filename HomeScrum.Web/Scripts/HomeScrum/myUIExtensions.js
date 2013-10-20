function setupShowHideButton(controller, toggleButton, hideThese) {
    var classToShowHide = "." + hideThese + "ItemRow";

    function showItems(toggleButton, effect) {
        $(classToShowHide).show(effect);
        toggleButton.button("option", "label", "Hide " + hideThese);
    }

    function hideItems(toggleButton, effect) {
        $("." + hideThese + "ItemRow").hide(effect);
        toggleButton.button("option", "label", "Show " + hideThese);
    }

    function shouldShow() {
        return $.localStorage(controller + "Show" + hideThese);
    }

    function saveShowState(value) {
        $.localStorage(controller + "Show" + hideThese, value);
    }

    function initializeShowHide(toggleButton) {
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

    toggleButton.button();
    initializeShowHide(toggleButton);

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