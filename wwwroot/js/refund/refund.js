$(document).ready(function () {

    initializeRefundIndex();

});


/* ============================================================
   INITIALIZE REFUND INDEX
   ============================================================ */

function initializeRefundIndex() {

    const patientId = $("#PatientId").val();

    if (patientId) {

        loadRefunds(patientId);
        wireInitiateRefund(patientId);

    }

    wireRefundNavigation();

}


/* ============================================================
   LOAD REFUNDS
   ============================================================ */

function loadRefunds(patientId) {

    if (!patientId) {
        return;
    }

    console.log("loadRefunds called:", patientId);

    $("#refundContainer").html(`
        <div class="alert alert-info mb-0">
            Loading refunds...
        </div>
    `);

    $("#refundContainer").load(
        "/Refund/ListByPatient?patientId=" +
        encodeURIComponent(patientId)
    );

}


/* ============================================================
   PATIENT CHANGED
   ============================================================ */

window.onPatientChanged = async function (patient) {

    if (!patient) {
        return;
    }

    const patientId =
        patient.patientId ||
        patient.id ||
        "";

    if (!patientId) {
        return;
    }

    console.log(
        "Refund patient changed:",
        patientId
    );

    loadRefunds(patientId);

    wireInitiateRefund(patientId);

};


/* ============================================================
   INITIATE REFUND
   ============================================================ */

function wireInitiateRefund(patientId) {

    if (!patientId) {
        return;
    }

    $("#btnInitiateRefund")
        .removeClass("d-none")
        .off("click")
        .on("click", async function (e) {

            e.preventDefault();

            const patientSaved =
                await savePatientCache();

            if (!patientSaved) {
                return;
            }

            window.location =
                "/Refund/Create?patientId=" +
                encodeURIComponent(patientId);

        });

}


/* ============================================================
   GET CURRENT PATIENT
   ============================================================ */

function getRefundPatient() {

    /*
     * First try the common PatientCache state.
     */
    let patient =
        PatientCache.getCurrentPatient();

    if (patient) {

        console.log(
            "Refund patient obtained from PatientCache:",
            patient
        );

        return patient;
    }


    /*
     * If the page was restored through browser Back /
     * BFCache, PatientCache may have restored the
     * selection context without populating its internal
     * currentPatient variable.
     *
     * Therefore use the hidden patient ID as fallback.
     */
    const patientContext =
        document.querySelector(
            "#refundPatientContext [data-patient-selection-context]"
        );

    if (!patientContext) {
        return null;
    }

    const patientIdInput =
        patientContext.querySelector(
            "[data-patient-id]"
        );

    if (!patientIdInput) {
        return null;
    }

    const patientId =
        patientIdInput.value;

    if (!patientId) {
        return null;
    }

    console.log(
        "Refund patient obtained from restored patient context:",
        patientId
    );

    /*
     * We only need the ID here because that is sufficient
     * for saving the patient cache and for navigation.
     *
     * PatientCache.saveSilent() will use the current
     * patient context.
     */
    return {
        patientId: patientId
    };

}


/* ============================================================
   SAVE PATIENT CACHE
   ============================================================ */

async function savePatientCache() {

    const refundContext =
        document.getElementById(
            "refundPatientContext"
        );

    if (!refundContext) {

        console.error(
            "Refund patient context not found."
        );

        return false;
    }

    const patientContext =
        refundContext.querySelector(
            "[data-patient-selection-context]"
        );

    if (!patientContext) {

        console.error(
            "Patient selection context not found."
        );

        return false;
    }


    /*
     * Get patient either from PatientCache or from
     * the restored patient-selection context.
     */
    const patient =
        getRefundPatient();

    if (!patient) {

        alert(
            "Please select a patient first."
        );

        return false;
    }


    const patientId =
        patient.patientId ||
        patient.id ||
        "";

    if (!patientId) {

        alert(
            "The selected patient does not have a valid Patient ID."
        );

        return false;
    }


    /*
     * Make sure the hidden patient ID is synchronized
     * with the patient we are about to save.
     */
    const patientIdInput =
        patientContext.querySelector(
            "[data-patient-id]"
        );

    if (patientIdInput) {

        patientIdInput.value =
            patientId;

    }


    /*
     * Save silently so that the save operation does
     * not trigger onPatientChanged() and reload the
     * refund list unnecessarily.
     */
    const patientSaved =
        await PatientCache.saveSilent(
            patientContext
        );

    if (!patientSaved) {

        alert(
            "Unable to save the selected patient."
        );

        return false;
    }

    console.log(
        "Refund patient saved to cache:",
        patientId
    );

    return true;

}


/* ============================================================
   SAVE PATIENT BEFORE NAVIGATION
   ============================================================ */

function wireRefundNavigation() {

    $("#refundContainer")
        .off("click.refundNavigation")
        .on(
            "click.refundNavigation",
            "a[href]",
            async function (e) {

                const link =
                    this;

                const href =
                    link.getAttribute("href");

                if (!href) {
                    return;
                }

                if (
                    href === "#" ||
                    href.startsWith("javascript:")
                ) {
                    return;
                }

                e.preventDefault();

                console.log(
                    "Refund navigation intercepted:",
                    href
                );

                const patientSaved =
                    await savePatientCache();

                if (!patientSaved) {
                    return;
                }

                window.location.href =
                    link.href;

            }
        );

}


/* ============================================================
   RESTORE PATIENT AFTER BROWSER BACK / BFCache
   ============================================================ */

window.addEventListener(
    "pageshow",
    async function (event) {

        console.log(
            "Refund Index pageshow:",
            event.persisted
        );

        const patientId =
            $("#PatientId").val();


        /*
         * On a normal initial load where PatientId is
         * already supplied by the controller, document.ready
         * has already initialized the page.
         *
         * On browser Back (BFCache), force restoration.
         *
         * If the page was freshly loaded without PatientId,
         * also attempt restoration from PatientCache.
         */
        if (
            !event.persisted &&
            patientId
        ) {
            return;
        }


        const refundContext =
            document.getElementById(
                "refundPatientContext"
            );

        if (!refundContext) {
            return;
        }


        const patientContext =
            refundContext.querySelector(
                "[data-patient-selection-context]"
            );

        if (!patientContext) {
            return;
        }


        try {

            const patient =
                await PatientCache.refresh(
                    patientContext
                );

            if (!patient) {

                console.log(
                    "No patient found in patient cache."
                );

                return;
            }


            const restoredPatientId =
                patient.patientId ||
                patient.id ||
                "";


            if (!restoredPatientId) {

                console.log(
                    "Cached patient has no valid Patient ID."
                );

                return;
            }


            /*
             * Explicitly synchronize the hidden
             * PatientId field as well.
             */
            const patientIdInput =
                patientContext.querySelector(
                    "[data-patient-id]"
                );

            if (patientIdInput) {

                patientIdInput.value =
                    restoredPatientId;

            }


            console.log(
                "Restored patient from cache:",
                restoredPatientId
            );


            loadRefunds(
                restoredPatientId
            );

            wireInitiateRefund(
                restoredPatientId
            );

        }
        catch (error) {

            console.error(
                "Unable to restore patient from cache:",
                error
            );

        }

    }
);
