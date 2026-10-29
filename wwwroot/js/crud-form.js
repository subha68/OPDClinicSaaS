function setSelectedFileName(input, targetId) {
    if (input.files && input.files.length > 0) {
        document.getElementById(targetId).value =
            input.files[0].name;
    }
}

$(document).on(
    "click",
    ".crud-action",
    function () {

        const spinnerText =
            $(this).data("spinner-text");

        if (!spinnerText) {
            return;
        }

        if (typeof showLoading === "function") {

            showLoading(spinnerText);

        } else {

            $("#loadingOverlay")
                .removeClass("d-none");

            $("#loadingMessage")
                .text(spinnerText);
        }
    });
/*
 * Generic support for related dropdown/manual-input fields.
 *
 * Expected markup:
 *   [data-related-control]
 *     data-related-selector="..."
 *     data-related-input="..."
 *     data-related-trigger="..."
 *     data-related-required="true|false"
 *     select[data-related-selector-control]
 *     input[data-related-value]        <-- the only model-bound control
 *     [data-related-input-container]
 *       input[data-related-manual-input]
 *
 * The dropdown and manual input must NOT have name attributes. The hidden
 * data-related-value input should be the only successful control named for
 * the model property, preventing duplicate values in MVC model binding.
 */
(function () {
    "use strict";

    function initializeRelatedControl(container) {
        const selector = document.getElementById(
            container.dataset.relatedSelector
        );
        const manualInput = document.getElementById(
            container.dataset.relatedInput
        );
        const triggerValue = container.dataset.relatedTrigger;
        const hiddenValue = container.querySelector("[data-related-value]");
        const inputContainer = container.querySelector(
            "[data-related-input-container]"
        );
        const isRequired = container.dataset.relatedRequired === "true";

        if (!selector || !manualInput || !hiddenValue || !inputContainer) {
            return;
        }

        function synchronize() {
            const useManualInput = selector.value === triggerValue;

            inputContainer.hidden = !useManualInput;
            manualInput.required = useManualInput && isRequired;

            if (useManualInput) {
                hiddenValue.value = manualInput.value.trim();
            } else {
                hiddenValue.value = selector.value;
                manualInput.value = "";
            }
        }

        selector.addEventListener("change", synchronize);

        manualInput.addEventListener("input", function () {
            if (selector.value === triggerValue) {
                hiddenValue.value = manualInput.value.trim();
            }
        });

        // Preserve an existing/manual value when the page first loads.
        if (selector.value === triggerValue) {
            inputContainer.hidden = false;
            manualInput.required = isRequired;
            hiddenValue.value = manualInput.value.trim();
        } else {
            inputContainer.hidden = true;
            manualInput.required = false;
            hiddenValue.value = selector.value;
        }
    }

    document.addEventListener("DOMContentLoaded", function () {
        document.querySelectorAll("[data-related-control]")
            .forEach(initializeRelatedControl);
    });
})();
