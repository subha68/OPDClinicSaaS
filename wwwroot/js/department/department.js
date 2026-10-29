/*
 * Department form behaviour.
 * Keeps the related-dropdown/input behaviour local to this page while the
 * generic _CrudForm remains reusable by other screens.
 */
(function () {
    "use strict";

    function initializeDepartmentForm() {
        const form = document.querySelector(".glass-card form");
        if (!form) return;

        const relatedControl = form.querySelector(
            '[data-related-control][data-related-target="DepartmentCode"]'
        );

        if (!relatedControl) return;

        const selector = relatedControl.querySelector(
            "[data-related-selector-control]"
        );
        const manualInput = relatedControl.querySelector(
            "[data-related-manual-input]"
        );
        const inputContainer = relatedControl.querySelector(
            "[data-related-input-container]"
        );
        const hiddenValue = relatedControl.querySelector(
            '[name="DepartmentCode"][data-related-value]'
        );
        const triggerValue = relatedControl.dataset.relatedTrigger || "__OTHER__";
        const isRequired = relatedControl.dataset.relatedRequired === "true";

        if (!selector || !manualInput || !inputContainer || !hiddenValue) {
            console.error(
                "Department Code control is missing required related-input elements.",
                relatedControl
            );
            return;
        }

        function synchronizeDepartmentCode() {
            const useManualInput = selector.value === triggerValue;

            inputContainer.hidden = !useManualInput;
            manualInput.disabled = !useManualInput;
            manualInput.required = useManualInput && isRequired;

            if (useManualInput) {
                hiddenValue.value = manualInput.value.trim();
            } else {
                hiddenValue.value = selector.value.trim();
                manualInput.value = "";
                manualInput.setCustomValidity("");
            }
        }

        selector.addEventListener("change", synchronizeDepartmentCode);
        manualInput.addEventListener("input", synchronizeDepartmentCode);

        form.addEventListener("submit", function (event) {
            synchronizeDepartmentCode();

            if (!hiddenValue.value.trim()) {
                event.preventDefault();

                if (selector.value === triggerValue) {
                    manualInput.setCustomValidity(
                        "Please enter a department code."
                    );
                    manualInput.reportValidity();
                    manualInput.focus();
                } else {
                    selector.setCustomValidity(
                        "Please select a Department Code or choose Other and enter a new code."
                    );
                    selector.reportValidity();
                    selector.focus();
                }
                return;
            }

            selector.setCustomValidity("");
            manualInput.setCustomValidity("");
            hiddenValue.value = hiddenValue.value.trim();
        });

        selector.addEventListener("change", function () {
            selector.setCustomValidity("");
            manualInput.setCustomValidity("");
        });

        // Run immediately so this also works if the script is loaded after
        // DOMContentLoaded has already fired.
        synchronizeDepartmentCode();
    }

    if (document.readyState === "loading") {
        document.addEventListener("DOMContentLoaded", initializeDepartmentForm);
    } else {
        initializeDepartmentForm();
    }
})();
