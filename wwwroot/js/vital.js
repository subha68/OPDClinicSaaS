(function () {

    "use strict";

    let vitalModal = null;
    let vitalPatient = null;

    // ============================================================
    // INITIALIZATION
    // ============================================================

    function initializeVitals() {

        const modalElement =
            document.getElementById("vitalModal");

        if (!modalElement) {
            return;
        }

        /*
         * Prevent duplicate initialization.
         */
        if (modalElement.dataset.initialized === "true") {
            return;
        }

        modalElement.dataset.initialized = "true";


        /*
         * Bootstrap modal instance.
         */
        vitalModal =
            bootstrap.Modal.getOrCreateInstance(
                modalElement
            );


        // ========================================================
        // VITALS MENU
        // ========================================================

        $(document).on(
            "click",
            '[data-menu-action="VITAL"]',
            async function (event) {

                event.preventDefault();
                event.stopPropagation();


                /*
                 * Patient selection is completely managed by
                 * PatientCache.
                 *
                 * refresh() will:
                 *
                 *  - retrieve selected patient
                 *  - update patient selection context
                 *  - update common patient header
                 *  - notify patient change handlers
                 *
                 * It returns the selected patient or null.
                 */

                const patient =
                    await PatientCache.refresh(
                        modalElement.querySelector(
                            "[data-patient-selection-context]"
                        )
                    );


                if (!patient) {

                    showVitalError(
                        "Please select a patient first."
                    );

                    vitalModal.show();

                    return;
                }


                /*
                 * Patient is available.
                 *
                 * Now Vitals takes over.
                 */

                await loadVitals();

            }
        );


        // ========================================================
        // SAVE BUTTON
        // ========================================================

        $(document).on(
            "click",
            "#btnSaveVitals",
            function () {

                saveVitals();

            }
        );


        // ========================================================
        // SOURCE VALUE CHANGE
        // ========================================================

        $(document).on(
            "change",
            "#vitalFields .vital-input",
            function () {

                /*
                 * Ignore derived inputs.
                 */
                if (
                    $(this).attr("data-is-derived") === "true"
                ) {
                    return;
                }

                calculateDerivedVitals();

                updateCompositionValues();

            }
        );

        // ========================================================
        // PATIENT CHANGE
        // ========================================================

        $(document).on(
            "patient:changed",
            async function (event) {

                const detail =
                    event.originalEvent?.detail;

                const patient =
                    detail?.patient;

                const context =
                    modalElement.querySelector(
                        "[data-patient-selection-context]"
                    );

                if (!patient || !context) {
                    return;
                }

                if (
                    detail.context &&
                    detail.context !== context
                ) {
                    return;
                }

                vitalPatient = patient;

                await loadVitals();
            }
        );

    }


    // ============================================================
    // LOAD VITALS
    // ============================================================

    async function loadVitals() {

        /*
        * Vital patient is local to this modal.
        * It must not depend on the globally selected patient.
        */

        const patient =
            vitalPatient;

        if (!patient || !patient.patientId) {

            showVitalError(
                "Please select a patient first."
            );

            vitalModal.show();

            return;
        }

        const fields =
            document.getElementById(
                "vitalFields"
            );

        clearVitalError();

        if (fields) {
            fields.innerHTML = "";
        }

        setVitalLoading(true);

        try {

            const url =
                "/Vital/GetVitals?patientId=" +
                encodeURIComponent(
                    patient.patientId
                );

            const response =
                await fetch(
                    url,
                    {
                        method: "GET",

                        credentials:
                            "same-origin",

                        headers: {
                            "X-Requested-With":
                                "XMLHttpRequest",

                            "Accept":
                                "application/json"
                        }
                    }
                );


            let result;


            try {

                result =
                    await response.json();

            }
            catch (jsonError) {

                console.error(
                    "GetVitals returned non-JSON response.",
                    jsonError
                );

                showVitalError(
                    "The server returned an unexpected response."
                );

                vitalModal.show();

                return;

            }


            if (
                !response.ok ||
                !result.success
            ) {

                showVitalError(
                    result.message ||
                    "Unable to load vitals."
                );

                vitalModal.show();

                return;

            }


            renderVitals(
                result.vitals || []
            );

            vitalModal.show();

        }
        catch (error) {

            console.error(
                "GetVitals request failed:",
                error
            );

            showVitalError(
                "An unexpected error occurred while loading vitals."
            );

            vitalModal.show();

        }
        finally {

            setVitalLoading(false);

        }

    }


    // ============================================================
    // RENDER VITALS
    // ============================================================

    function renderVitals(vitals) {

        const container =
            document.getElementById(
                "vitalFields"
            );

        if (!container) {
            return;
        }


        container.innerHTML = "";


        if (
            !Array.isArray(vitals) ||
            vitals.length === 0
        ) {

            showVitalError(
                "No vital definitions are available."
            );

            return;
        }


        /*
         * Build VitalCode lookup.
         */

        const byCode =
            new Map();


        vitals.forEach(function (vital) {

            if (vital.vitalCode) {

                byCode.set(
                    vital.vitalCode.toUpperCase(),
                    vital
                );

            }

        });


        /*
         * Render actual display elements.
         *
         * CompositionItem is rendered by its Composition.
         */

        vitals.forEach(function (vital) {

            const type =
                (vital.vitalType || "")
                    .toLowerCase();


            if (type === "compositionitem") {
                return;
            }


            let element;


            if (type === "composition") {

                element =
                    renderComposition(
                        vital,
                        byCode
                    );

            }
            else {

                element =
                    renderSingleVital(
                        vital
                    );

            }


            if (element) {

                container.appendChild(
                    element
                );

            }

        });


        /*
         * Calculate derived values after rendering.
         */

        calculateDerivedVitals();

    }


    // ============================================================
    // SINGLE VITAL
    // ============================================================

    function renderSingleVital(vital) {

        const column =
            document.createElement("div");

        column.className =
            "col-md-6";


        const wrapper =
            document.createElement("div");

        wrapper.className =
            "mb-2";


        const label =
            document.createElement("label");

        label.className =
            "form-label";

        label.textContent =
            vital.vitalLabel ||
            vital.vitalCode;


        const inputGroup =
            document.createElement("div");

        inputGroup.className =
            "input-group";


        const input =
            createVitalInput(vital);


        inputGroup.appendChild(
            input
        );


        if (vital.vitalUnit) {

            const unit =
                document.createElement("span");

            unit.className =
                "input-group-text";

            unit.textContent =
                vital.vitalUnit;

            inputGroup.appendChild(
                unit
            );

        }


        wrapper.appendChild(label);
        wrapper.appendChild(inputGroup);

        column.appendChild(wrapper);


        return column;

    }


    // ============================================================
    // CREATE INPUT
    // ============================================================

    function createVitalInput(vital) {

        const input =
            document.createElement("input");


        input.id =
            "vital_" +
            vital.vitalCode;


        input.className =
            "form-control vital-input";


        /*
         * Metadata required by SaveVitals.
         */

        input.dataset.vitalId = vital.vitalId || 0;
        input.dataset.vitalDefId = vital.vitalDefId || 0;
        input.dataset.vitalCode = vital.vitalCode || "";
        input.dataset.vitalType = vital.vitalType || "";
        input.dataset.dataType = vital.dataType || "";
        input.dataset.isDerived = vital.isDerived ? "true" : "false";
        input.dataset.isComposition = vital.isComposition ? "true" : "false";

        /*
         * HTML input type.
         */

        switch ((vital.dataType || "").toLowerCase())
        {
            case "decimal":
            case "number":

                input.type = "number";

                input.step =
                    (
                        vital.dataType || ""
                    ).toLowerCase() === "decimal"
                        ? "any"
                        : "1";

                break;


            case "boolean":

                input.type = "checkbox";

                break;


            case "date":

                input.type = "date";

                break;


            default:

                input.type = "text";

                break;

        }


        /*
         * Derived values are read-only.
         */

        if (vital.isDerived) {

            input.readOnly = true;

            input.classList.add(
                "bg-light"
            );

        }


        /*
         * Set current value.
         */

        if (
            vital.vitalValue !== null &&
            vital.vitalValue !== undefined
        ) {

            if (
                (vital.dataType || "").toLowerCase()
                === "boolean"
            ) {

                input.checked =
                    vital.vitalValue === "true" ||
                    vital.vitalValue === "1";

            }
            else {

                input.value =
                    vital.vitalValue;

            }

        }


        return input;

    }


    // ============================================================
    // COMPOSITION
    // ============================================================

    function renderComposition(composition, byCode) 
    {
        const column = document.createElement("div");
        column.className = "col-md-6";
        const wrapper = document.createElement("div");
        wrapper.className = "mb-2";

        const label = document.createElement("label");
        label.className = "form-label";
        label.textContent = composition.vitalLabel || composition.vitalCode;

        const inputGroup = document.createElement("div");
        inputGroup.className = "input-group";

        const hiddenComposition = document.createElement("input");
        hiddenComposition.type = "hidden";
        hiddenComposition.id = "vital_" + composition.vitalCode;
        hiddenComposition.className = "vital-input composition-value";
        hiddenComposition.dataset.vitalId = composition.vitalId || 0;
        hiddenComposition.dataset.vitalDefId = composition.vitalDefId || 0;
        hiddenComposition.dataset.vitalCode = composition.vitalCode || "";
        hiddenComposition.dataset.vitalType = "composition";
        hiddenComposition.dataset.dataType = composition.dataType || "";
        hiddenComposition.dataset.isDerived = "false";
        hiddenComposition.value = composition.vitalValue || "";
        wrapper.appendChild(hiddenComposition);
        hiddenComposition.dataset.displayComposition = composition.displayComposition || "";

        /*
         * Find CompositionItems.
         */

        const itemCodes =
            extractVitalCodes(
                composition.displayComposition
            );


        const items = [];


        itemCodes.forEach(function (code) {

            const item =
                byCode.get(
                    code.toUpperCase()
                );


            if (!item) {
                return;
            }


            if (
                (item.vitalType || "").toLowerCase()
                !== "compositionitem"
            ) {
                return;
            }


            items.push(item);

        });


        /*
         * Render CompositionItems.
         */

        if (items.length > 0) {

            items.forEach(
                function (item, index) {

                    const input =
                        createVitalInput(item);


                    input.classList.add(
                        "composition-input"
                    );


                    /*
                     * CompositionItems are editable.
                     */

                    input.readOnly = false;


                    inputGroup.appendChild(
                        input
                    );


                    if (
                        index <
                        items.length - 1
                    ) {

                        const separator =
                            getCompositionSeparator(
                                composition.displayComposition,
                                item.vitalCode,
                                items[index + 1].vitalCode
                            );


                        const separatorElement =
                            document.createElement("span");

                        separatorElement.className =
                            "input-group-text";

                        separatorElement.textContent =
                            separator;


                        inputGroup.appendChild(
                            separatorElement
                        );

                    }

                }
            );

        }
        else {

            /*
             * No CompositionItems found.
             */

            const input =
                document.createElement("input");

            input.type =
                "text";

            input.className =
                "form-control";

            input.readOnly =
                true;

            input.value =
                composition.vitalValue || "";


            inputGroup.appendChild(
                input
            );

        }


        /*
         * Composition unit.
         */

        if (composition.vitalUnit) {

            const unit =
                document.createElement("span");

            unit.className =
                "input-group-text";

            unit.textContent =
                composition.vitalUnit;

            inputGroup.appendChild(
                unit
            );

        }


        wrapper.appendChild(label);
        wrapper.appendChild(inputGroup);

        column.appendChild(wrapper);


        return column;

    }


    // ============================================================
    // EXTRACT COMPOSITION VITAL CODES
    // ============================================================

    function extractVitalCodes(
        displayComposition
    ) {

        if (!displayComposition) {
            return [];
        }


        const matches =
            displayComposition.match(
                /\b[A-Za-z][A-Za-z0-9_]*\b/g
            );


        return matches || [];

    }


    // ============================================================
    // GET COMPOSITION SEPARATOR
    // ============================================================

    function getCompositionSeparator(
        displayComposition,
        currentCode,
        nextCode
    ) {

        if (!displayComposition) {
            return "";
        }


        const expression =
            displayComposition;


        const currentIndex =
            expression.search(
                new RegExp(
                    "\\b" +
                    escapeRegExp(currentCode) +
                    "\\b",
                    "i"
                )
            );


        if (currentIndex < 0) {
            return "";
        }


        const afterCurrent =
            expression.substring(
                currentIndex +
                currentCode.length
            );


        const nextIndex =
            afterCurrent.search(
                new RegExp(
                    "\\b" +
                    escapeRegExp(nextCode) +
                    "\\b",
                    "i"
                )
            );


        if (nextIndex < 0) {
            return "";
        }


        const separator =
            afterCurrent.substring(
                0,
                nextIndex
            );


        return separator.trim();

    }


    // ============================================================
    // REGEX ESCAPE
    // ============================================================

    function escapeRegExp(value) {

        return value.replace(
            /[.*+?^${}()|[\]\\]/g,
            "\\$&"
        );

    }

    function updateCompositionValues() {

        document
            .querySelectorAll(
                '#vitalFields .composition-value')
            .forEach(function (compositionInput) {

                const compositionCode =
                    compositionInput.dataset.vitalCode;

                const displayComposition =
                    compositionInput.dataset.displayComposition;

                if (!displayComposition) {
                    return;
                }

                let value =
                    displayComposition;

                const codes =
                    extractVitalCodes(
                        displayComposition);

                codes.forEach(function (code) {

                    const source =
                        document.getElementById(
                            "vital_" + code);

                    const sourceValue =
                        source?.value || "";

                    value =
                        value.replace(
                            new RegExp(
                                "\\b" + code + "\\b",
                                "g"),
                            sourceValue);
                });

                compositionInput.value =
                    value;
            });
    }

    // ============================================================
    // COLLECT CURRENT VALUES
    // ============================================================

    function collectVitalValues() {

        const values = {};


        document
            .querySelectorAll(
                "#vitalFields .vital-input"
            )
            .forEach(function (input) {

                const code =
                    input.dataset.vitalCode;


                if (!code) {
                    return;
                }


                /*
                 * Derived values are calculated by server.
                 */

                if (
                    input.dataset.isDerived === "true"
                ) {
                    return;
                }


                if (
                    input.type === "checkbox"
                ) {

                    values[code] =
                        input.checked
                            ? "1"
                            : "0";

                }
                else {

                    values[code] =
                        input.value || "";

                }

            });


        return values;

    }


    // ============================================================
    // CALCULATE DERIVED VITALS
    // ============================================================

    async function calculateDerivedVitals() {

        const values =
            collectVitalValues();


        const hasValue =
            Object.values(values).some(
                function (value) {

                    return (
                        value !== null &&
                        value !== undefined &&
                        value.toString().trim() !== ""
                    );

                }
            );


        if (!hasValue) {
            return;
        }


        if (
            Object.keys(values).length === 0
        ) {
            return;
        }


        try {

            const params =
                new URLSearchParams();


            Object.keys(values)
                .forEach(function (code) {

                    params.append(
                        code,
                        values[code]
                    );

                });


            const response =
                await fetch(
                    "/Vital/CalculateDerivedVitals?" +
                    params.toString(),
                    {
                        method: "GET",

                        credentials:
                            "same-origin",

                        headers: {
                            "X-Requested-With":
                                "XMLHttpRequest",

                            "Accept":
                                "application/json"
                        }
                    }
                );


            let result;


            try {

                result =
                    await response.json();

            }
            catch (jsonError) {

                console.error(
                    "CalculateDerivedVitals returned non-JSON response.",
                    jsonError
                );

                return;

            }


            if (
                !response.ok ||
                !result.success
            ) {

                console.error(
                    result.message ||
                    "Unable to calculate derived vitals."
                );

                return;

            }


            if (
                Array.isArray(result.vitals)
            ) {

                result.vitals.forEach(
                    function (vital) {

                        updateVitalValue(vital.vitalCode, vital.vitalValue
                        );

                    }
                );

                updateCompositionValues();

            }

        }
        catch (error) {

            console.error(
                "Derived vital calculation failed:",
                error
            );

        }

    }


    // ============================================================
    // UPDATE VITAL VALUE
    // ============================================================

    function updateVitalValue(
        vitalCode,
        value
    ) {

        if (!vitalCode) {
            return;
        }


        const input =
            document.getElementById(
                "vital_" + vitalCode
            );


        if (!input) {
            return;
        }


        input.value =
            value === null ||
            value === undefined
                ? ""
                : value;

    }


    // ============================================================
    // SAVE VITALS
    // ============================================================

    async function saveVitals() {

        const button = document.getElementById("btnSaveVitals");

        clearVitalError();

        const vitals = [];

        document.querySelectorAll("#vitalFields .vital-input")
            .forEach(function (input) {

                const vital = {

                    vitalId:
                        parseInt(
                            input.dataset.vitalId || "0",
                            10
                        ),

                    vitalDefId:
                        parseInt(
                            input.dataset.vitalDefId || "0",
                            10
                        ),

                    vitalValue:
                        input.type === "checkbox"
                            ? (
                                input.checked
                                    ? "true"
                                    : "false"
                            )
                            : (
                                input.value || ""
                            )

                };


                vitals.push(vital);

            });


        if (vitals.length === 0) {

            showVitalError(
                "There are no vitals to save."
            );

            return;

        }


        setSaving(true);


        try {

                const patientId =
                    vitalPatient?.patientId;

                if (!patientId) {

                    showVitalError(
                        "Please select a patient first."
                    );

                    return;
                }


                const response =
                    await fetch(
                        "/Vital/SaveVitals?patientId=" +
                        encodeURIComponent(patientId),
                {
                    method: "POST",

                    credentials:
                        "same-origin",

                    headers: {
                        "Content-Type":
                            "application/json",

                        "X-Requested-With":
                            "XMLHttpRequest",

                        "Accept":
                            "application/json"
                    },

                    body:
                        JSON.stringify(vitals)
                }
            );


            let result;


            try {

                result =
                    await response.json();

            }
            catch (jsonError) {

                console.error(
                    "SaveVitals returned non-JSON response.",
                    jsonError
                );

                showVitalError(
                    "The server returned an unexpected response."
                );

                return;

            }


            if (
                !response.ok ||
                !result.success
            ) {

                showVitalError(
                    result.message ||
                    "Unable to save vitals."
                );

                return;

            }


            showVitalSuccess(
                result.message ||
                "Vitals saved successfully."
            );

        }
        catch (error) {

            console.error(
                "SaveVitals request failed:",
                error
            );

            showVitalError(
                "An unexpected error occurred while saving vitals."
            );

        }
        finally {

            setSaving(false);

        }

    }


    // ============================================================
    // LOADING STATE
    // ============================================================

    function setVitalLoading(
        isLoading
    ) {

        const loading =
            document.getElementById(
                "vitalLoading"
            );

        const fields =
            document.getElementById(
                "vitalFields"
            );

        const saveButton =
            document.getElementById(
                "btnSaveVitals"
            );


        if (loading) {

            loading.classList.toggle(
                "d-none",
                !isLoading
            );

        }


        if (fields) {

            fields.classList.toggle(
                "opacity-50",
                isLoading
            );

        }


        if (saveButton) {

            saveButton.disabled =
                isLoading;

        }

    }


    // ============================================================
    // SAVE BUTTON STATE
    // ============================================================

    function setSaving(
        isSaving
    ) {

        const button =
            document.getElementById(
                "btnSaveVitals"
            );


        if (!button) {
            return;
        }


        if (isSaving) {

            button.disabled =
                true;


            if (
                !button.dataset.originalHtml
            ) {

                button.dataset.originalHtml =
                    button.innerHTML;

            }


            button.innerHTML =
                '<span class="spinner-border spinner-border-sm me-1" ' +
                'role="status" aria-hidden="true"></span>' +
                'Saving...';

        }
        else {

            button.disabled =
                false;


            if (
                button.dataset.originalHtml
            ) {

                button.innerHTML =
                    button.dataset.originalHtml;

            }

        }

    }


    // ============================================================
    // ERROR / SUCCESS
    // ============================================================

    function showVitalError(
        message
    ) {

        const error =
            document.getElementById(
                "vitalError"
            );


        if (!error) {
            return;
        }


        error.textContent =
            message ||
            "Unable to load vitals.";


        error.classList.remove(
            "d-none"
        );

    }


    function clearVitalError() {

        const element =
            document.getElementById(
                "vitalError"
            );


        if (!element) {
            return;
        }


        element.textContent =
            "";


        element.classList.add(
            "d-none"
        );

    }


    function showVitalSuccess(
        message
    ) {

        const element =
            document.getElementById(
                "vitalSuccess"
            );


        if (!element) {
            return;
        }


        element.textContent =
            message ||
            "Vitals saved successfully.";


        element.classList.remove(
            "d-none"
        );


        setTimeout(
            function () {

                element.className =
                    "alert alert-success d-none";

                element.textContent =
                    "";

            },
            3000
        );

    }


    // ============================================================
    // START
    // ============================================================

    if (
        document.readyState === "loading"
    ) {

        document.addEventListener(
            "DOMContentLoaded",
            initializeVitals
        );

    }
    else {

        initializeVitals();

    }

})();
