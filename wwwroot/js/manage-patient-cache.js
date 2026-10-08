"use strict";

// ============================================================
// CONFIGURATION
// ============================================================

const DEFAULT_GET_PATIENT_URL =
    "/Patient/GetSelectedPatient";

const DEFAULT_SAVE_PATIENT_URL =
    "/Patient/SetSelectedPatient";


// ============================================================
// STATE
// ============================================================

let currentPatient = null;


// ============================================================
// INITIALIZATION
// ============================================================

function initializePatientCache() {

    /*
     * Initialize every patient-selection component on the page.
     *
     * This allows the same page to contain one or more patient
     * selection modals/components.
     */

    document
        .querySelectorAll(
            "[data-patient-selection-context]"
        )
        .forEach(function (context) {

            initializePatientContext(context);

        });


    /*
     * Patient modal open.
     *
     * The modal itself can have:
     *
     * data-patient-modal
     *
     * so that the currently cached patient is retrieved every
     * time the modal is opened.
     */

    document.addEventListener(
        "shown.bs.modal",
        async function (event) {

            const modal =
                event.target;

            /*
            * Only process modals that contain the common
            * patient-selection component.
            */

            const context =
                modal.querySelector(
                    "[data-patient-selection-context]"
                );

            if (!context) {
                return;
            }

            await refreshPatientModal(
                context
            );

        }
    );


    /*
     * Generic patient-save button.
     *
     * Example:
     *
     * <button data-patient-save>
     *     Save
     * </button>
     */

    document.addEventListener(
        "click",
        async function (event) {

            const button =
                event.target.closest(
                    "[data-patient-save]"
                );

            if (!button) {
                return;
            }


            event.preventDefault();


            const context =
                button.closest(
                    "[data-patient-selection-context]"
                );

            if (!context) {
                return;
            }


            await saveSelectedPatient(
                context,
                button
            );

        }
    );

}


// ============================================================
// INITIALIZE PATIENT CONTEXT
// ============================================================

function initializePatientContext(
    context
) {

    /*
     * Prevent duplicate initialization.
     */

    if (
        context.dataset.initialized === "true"
    ) {
        return;
    }


    context.dataset.initialized =
        "true";


    const searchInput =
        context.querySelector(
            "[data-patient-search]"
        );


    if (!searchInput) {
        return;
    }


    /*
     * Wire the existing patient autocomplete implementation.
     *
     * wirePatientAutocomplete is assumed to already exist
     * elsewhere in the application.
     */

    if (
        typeof wirePatientAutocomplete !==
        "function"
    ) {

        console.error(
            "wirePatientAutocomplete is not available."
        );

        return;

    }

    console.log(
        "Initializing patient autocomplete:",
        searchInput
    );

    wirePatientAutocomplete(
        searchInput,
        async function (patient) {

            console.log(
                "Patient autocomplete selected:",
                patient
            );

            await handlePatientSelected(
                context,
                patient
            );

        },
        getAutocompleteContainer(context)
    );

}

// ============================================================
// PATIENT SELECTED
// ============================================================

async function handlePatientSelected(
    context,
    patient
) {

    if (!patient) {
        return;
    }


    currentPatient =
        normalizePatient(patient);


    /*
     * Immediately refresh the modal demographics.
     */

    setPatientContext(
        context,
        currentPatient
    );


    /*
     * Allow the parent page/modal to react to a patient
     * change if it has page-specific requirements.
     *
     * Example:
     *
     * data-patient-changed="refreshVitals"
     *
     * or a JavaScript callback can be registered.
     */

    await notifyPatientChanged(
        context,
        currentPatient
    );

}


// ============================================================
// REFRESH PATIENT MODAL
// ============================================================

async function refreshPatientModal(
    context
) {

    if (!context) {
        return false;
    }


    try {

        const url =
            context.dataset.patientGetUrl ||
            DEFAULT_GET_PATIENT_URL;


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
                "GetSelectedPatient returned non-JSON response.",
                jsonError
            );

            clearPatientContext(
                context
            );

            return false;

        }


        if (
            !response.ok ||
            !result.success ||
            !result.patient
        ) {

            currentPatient = null;

            clearPatientContext(
                context
            );

            return false;

        }


        currentPatient =
            normalizePatient(
                result.patient
            );


        setPatientContext(
            context,
            currentPatient
        );


        /*
         * Give the parent page an opportunity to refresh
         * page/modal-specific content for the selected patient.
         */

        await notifyPatientChanged(
            context,
            currentPatient
        );


        return true;

    }
    catch (error) {

        console.error(
            "Unable to retrieve selected patient:",
            error
        );

        currentPatient = null;

        clearPatientContext(
            context
        );

        return false;

    }

}


// ============================================================
// SET PATIENT CONTEXT
// ============================================================

function setPatientContext(
    context,
    patient
) {

    if (!context || !patient) {
        return;
    }

    const patientId =
        patient.patientId ||
        patient.id ||
        "";

    const patientName =
        patient.fullName ||
        patient.patientName ||
        "";

    const gender =
        patient.gender ||
        "";

    const age =
        patient.age > 0
            ? patient.age
            : "";

    const phone =
        patient.phone ||
        "";


    const patientIdInput =
        context.querySelector(
            "[data-patient-id]"
        );

    const searchInput =
        context.querySelector(
            "[data-patient-search]"
        );

    const genderAgeInput =
        context.querySelector(
            "[data-patient-gender-age]"
        );

    const phoneInput =
        context.querySelector(
            "[data-patient-phone]"
        );


    if (patientIdInput) {
        patientIdInput.value = patientId;
    }

    if (searchInput) {
        searchInput.value = patientName;
    }

    if (genderAgeInput) {

        genderAgeInput.value =
            gender +
            (age ? " / " + age : "");

    }

    if (phoneInput) {
        phoneInput.value = phone;
    }
}


// ============================================================
// CLEAR PATIENT CONTEXT
// ============================================================

function clearPatientContext(
    context
) {

    if (!context) {
        return;
    }


    const fields = [
        "[data-patient-id]",
        "[data-patient-search]",
        "[data-patient-gender-age]",
        "[data-patient-phone]"
    ];


    fields.forEach(function (selector) {

        const field =
            context.querySelector(
                selector
            );

        if (field) {

            field.value =
                "";

        }

    });

}

async function refreshPatientSelection(context, onPatientChanged) {

    if (!context) {
        console.error(
            "refreshPatientSelection: patient context not found."
        );

        return null;
    }

    try {

        const response =
            await fetch(
                "/Patient/GetSelectedPatient",
                {
                    method: "GET",
                    credentials: "same-origin",
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
                "GetSelectedPatient returned non-JSON response.",
                jsonError
            );

            return null;
        }


        if (
            !response.ok ||
            !result.success ||
            !result.patient
        ) {

            console.warn(
                result.message ||
                "No selected patient was found."
            );

            clearPatientContext(context);

            return null;
        }


        const patient =
            result.patient;

        /*
        * IMPORTANT:
        * Restore the global currentPatient state as well.
        */
        currentPatient = normalizePatient(patient);

        /*
         * Update the common patient-selection component.
         */

        setPatientContext(context, currentPatient);

        /*
         * Update the parent page's common
         * Patient Header.
         */

        updatePatientHeader(currentPatient);


        /*
         * Allow the page/modal to perform any
         * patient-specific refresh it requires.
         *
         * Example:
         *
         * refresh appointment data
         * refresh vitals
         * refresh visit information
         * etc.
         */

        if (
            typeof onPatientChanged ===
            "function"
        ) {

            await onPatientChanged(currentPatient);

        }


        return currentPatient;

    }
    catch (error) {

        console.error(
            "Unable to refresh selected patient:",
            error
        );

        return null;
    }
}

// ============================================================
// SAVE SELECTED PATIENT
// ============================================================

async function saveSelectedPatient(
    context,
    button,
    notify = true
) {

    if (!currentPatient) {

        showPatientError(
            context,
            "Please select a patient first."
        );

        return false;

    }


    const patientId =
        currentPatient.patientId ||
        currentPatient.id;


    if (!patientId) {

        showPatientError(
            context,
            "The selected patient is invalid."
        );

        return false;

    }


    const url =
        context.dataset.patientSaveUrl ||
        DEFAULT_SAVE_PATIENT_URL;


    setPatientSaving(
        button,
        true
    );


    clearPatientError(
        context
    );


    try {

        const response =
            await fetch(
                url,
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
                        JSON.stringify(
                            currentPatient
                        )
                }
            );


        let result;


        try {

            result =
                await response.json();

        }
        catch (jsonError) {

            console.error(
                "SetSelectedPatient returned non-JSON response.",
                jsonError
            );

            showPatientError(
                context,
                "The server returned an unexpected response."
            );

            return false;

        }


        if (
            !response.ok ||
            !result.success
        ) {

            showPatientError(
                context,
                result.message ||
                "Unable to save the selected patient."
            );

            return false;

        }


        /*
         * The backend may return the canonical patient
         * object after saving it to cache.
         */

        if (result.patient) {

            currentPatient =
                normalizePatient(
                    result.patient
                );

        }


        /*
         * Refresh the modal using the canonical value.
         */

        setPatientContext(
            context,
            currentPatient
        );


        /*
         * Refresh the patient header on the parent page.
         */

        updatePatientHeader(
            currentPatient
        );


        /*
        * Notify the parent page that the patient has
        * successfully changed.
        *
        * Silent saves intentionally skip this notification.
        */
        if (notify) {

            await notifyPatientChanged(
                context,
                currentPatient
            );

        }

        /*
         * Allow the consuming page to close the modal.
         *
         * The default behavior is intentionally not to
         * close it. The page can decide what it wants to do.
         */

        dispatchPatientEvent(
            "patient:saved",
            currentPatient,
            context
        );


        return true;

    }
    catch (error) {

        console.error(
            "SetSelectedPatient request failed:",
            error
        );

        showPatientError(
            context,
            "An unexpected error occurred while saving the patient."
        );

        return false;

    }
    finally {

        setPatientSaving(
            button,
            false
        );

    }

}

// ============================================================
// SILENT SAVE SELECTED PATIENT
// ============================================================

async function saveSelectedPatientSilent(
    context
) {

    /*
     * Save the selected patient to the server-side cache
     * without notifying the consuming page that the patient
     * has changed.
     *
     * This is useful when leaving a page where the patient
     * has already been processed, for example:
     *
     * Refund Index
     *     -> View
     *     -> Pay
     *     -> Print
     *     -> Initiate Refund
     */
    return await saveSelectedPatient(
        context,
        null,
        false
    );

}


// ============================================================
// UPDATE PATIENT HEADER
// ============================================================

function updatePatientHeader(patient) {

    const header =
        document.querySelector(
            "[data-patient-header]"
        );

    if (!header) {

        console.warn(
            "Patient header not found."
        );

        return;

    }


    const patientId =
        patient?.patientId ||
        patient?.id ||
        0;


    const name =
        header.querySelector(
            "[data-patient-header-name]"
        );

    const id =
        header.querySelector(
            "[data-patient-header-id]"
        );

    const demographics =
        header.querySelector(
            "[data-patient-header-demographics]"
        );

    const phone =
        header.querySelector(
            "[data-patient-header-phone]"
        );


    /*
     * No valid patient.
     *
     * Keep the HTML in the DOM but hide it.
     */

    if (!patientId || Number(patientId) <= 0) {

        header.classList.add("d-none");

        return;

    }


    /*
     * Populate header.
     */

    if (name) {

        name.textContent =
            patient.fullName || "";

    }


    if (id) {

        id.textContent =
            "(ID: " +
            patientId +
            ")";

    }


    if (demographics) {

        demographics.textContent =
            formatGenderAge(
                patient.gender,
                patient.age
            );

    }


    if (phone) {

        phone.textContent =
            patient.phone || "";

        phone.href =
            patient.phone
                ? "tel:" + patient.phone
                : "#";

    }


    /*
     * Patient is valid.
     *
     * Show the header.
     */

    header.classList.remove("d-none");

}

// ============================================================
// FORMAT GENDER / AGE
// ============================================================

function formatGenderAge(
    gender,
    age
) {

    let value = "";


    if (gender) {

        value =
            gender;

    }


    if (
        age !== null &&
        age !== undefined &&
        age !== "" &&
        Number(age) > 0
    ) {

        if (value) {

            value +=
                " / ";

        }

        value +=
            age;

    }


    return value;

}


// ============================================================
// NORMALIZE PATIENT
// ============================================================

function normalizePatient(
    patient
) {

    if (!patient) {
        return null;
    }


    return {

        patientId:
            patient.patientId ||
            patient.id ||
            "",

        fullName:
            patient.fullName ||
            patient.patientName ||
            "",

        gender:
            patient.gender ||
            "",

        age:
            patient.age ||
            0,

        phone:
            patient.phone ||
            ""

    };

}


// ============================================================
// AUTOCOMPLETE CONTAINER
// ============================================================

function getAutocompleteContainer(context) {

    const modal =
        context.closest(".modal");

    if (modal && modal.id) {
        return "#" + modal.id;
    }

    if (context.id) {
        return "#" + context.id;
    }

    return context;
}


// ============================================================
// PATIENT CHANGE NOTIFICATION
// ============================================================

async function notifyPatientChanged(
    context,
    patient
) {

    if (!patient) {
        return;
    }


    /*
     * Dispatch a DOM event so that a consuming page can
     * react without modifying this common JavaScript.
     *
     * Example:
     *
     * document.addEventListener(
     *     "patient:changed",
     *     function (event) {
     *
     *         const patient =
     *             event.detail.patient;
     *
     *         // Refresh page-specific content
     *     }
     * );
     */

    dispatchPatientEvent(
        "patient:changed",
        patient,
        context
    );


    /*
     * Optional page-specific callback.
     *
     * A page may expose:
     *
     * window.onPatientChanged = async function(patient) {
     *     ...
     * };
     */

    if (
        typeof window.onPatientChanged ===
        "function"
    ) {

        try {

            await window.onPatientChanged(
                patient,
                context
            );

        }
        catch (error) {

            console.error(
                "onPatientChanged callback failed:",
                error
            );

        }

    }

}


// ============================================================
// DISPATCH PATIENT EVENT
// ============================================================

function dispatchPatientEvent(
    eventName,
    patient,
    context
) {

    document.dispatchEvent(
        new CustomEvent(
            eventName,
            {
                detail: {
                    patient:
                        patient,

                    context:
                        context
                }
            }
        )
    );

}


// ============================================================
// ERROR HANDLING
// ============================================================

function showPatientError(
    context,
    message
) {

    const element =
        context.querySelector(
            "[data-patient-error]"
        );


    if (!element) {

        console.error(
            message
        );

        return;

    }


    element.textContent =
        message ||
        "Unable to process patient selection.";


    element.classList.remove(
        "d-none"
    );

}


function clearPatientError(
    context
) {

    const element =
        context.querySelector(
            "[data-patient-error]"
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


// ============================================================
// SAVE BUTTON STATE
// ============================================================

function setPatientSaving(
    button,
    isSaving
) {

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
            "Saving...";

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
// PUBLIC API
// ============================================================

/*
 * Expose only the functions that consuming pages may need.
 */

window.PatientCache = {

    getSelectedPatient: function () {
        return currentPatient;
    },

    getCurrentPatient: function () {
        return currentPatient;
    },

    getPatientId: function (context) {

        const patientIdInput =
            context
                ? context.querySelector(
                    "[data-patient-id]"
                )
                : null;

        return patientIdInput
            ? patientIdInput.value
            : "";
    },

    refresh: refreshPatientSelection,

    refreshModal: refreshPatientModal,

    save: saveSelectedPatient,

    saveSilent: saveSelectedPatientSilent,

    updateHeader: updatePatientHeader

};




// ============================================================
// START
// ============================================================

if (
    document.readyState === "loading"
) {

    document.addEventListener(
        "DOMContentLoaded",
        initializePatientCache
    );

}
else {

    initializePatientCache();

}
