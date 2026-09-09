(function () {

    "use strict";

    let testResultPatient = null;

    // ============================================================
    // CONFIGURATION
    // ============================================================

    const modalElement =
        document.getElementById("testResultModal");

    if (!modalElement) {
        return;
    }

    const testResultModal =
        new bootstrap.Modal(
            modalElement
        );

    window.onPatientChanged =
        async function (
            patient,
            context
        ) {

            if (!patient) {
                return;
            }

            testResultPatient =
                patient;

            await loadTestResults(
                patient.patientId
            );

        };

    // ============================================================
    // DOCUMENT READY
    // ============================================================

    $(document).ready(function () {

        initializeMenuAction();
        initializeReportActions();
        initializeEditorActions();
    });

    // ============================================================
    // MENU ACTION
    // ============================================================

    function initializeMenuAction() {

        $(document).on(
            "click",
            '[data-menu-action="TEST_RESULT"]',
            async function (event) {

                event.preventDefault();
                event.stopPropagation();

                try {

                    clearMessages();

                    const patient =
                        await PatientCache.refresh(
                            modalElement.querySelector(
                                "[data-patient-selection-context]"
                            )
                        );

                    if (!patient) {

                        showError(
                            "Please select a patient first."
                        );

                        testResultModal.show();

                        return;
                    }

                    testResultModal.show();

                    await loadTestResults(
                        patient.patientId
                    );

                }
                catch (error) {

                    console.error(error);

                    showError(
                        "Unable to load test results."
                    );

                    testResultModal.show();

                }

            }
        );

    }

    // ============================================================
    // REPORT ACTIONS
    // ============================================================

    function initializeReportActions() {

        $(document).on(
            "click",
            "#btnAddTestReport",
            async function () {

                const patient =
                    await refreshSelectedPatient();

                if (!patient) {
                    return;
                }

                await openAddReport(
                    patient.patientId
                );            
            }
        );

        $(document).on(
            "click",
            ".btn-edit-report",
            async function () {

                const reportId =
                    $(this).data(
                        "report-id"
                    );

                await openEditReport(
                    reportId
                );

            }
        );

        $(document).on(
            "click",
            ".btn-view-report",
            async function () {

                const reportId =
                    $(this).data(
                        "report-id"
                    );

                await openViewReport(
                    reportId
                );

            }
        );

    }

    function initializeEditorActions() {

        $(document).on(
            "click",
            "#btnAddResultRow",
            async function () {

                const index =
                    $("#resultRows .result-row")
                        .length;

                const html =
                    await $.get(
                        "/TestResult/AddResultRow",
                        {
                            index: index
                        }
                    );

                $("#resultRows")
                    .append(html);

                const row = $("#resultRows .result-row:last");

                wireParameterAutocomplete(row);

            }
        );

        $(document).on(
            "click",
            ".btn-delete-row",
            function () {

                const row =
                    $(this)
                        .closest(".result-row");

                row.remove();

                renumberRows();

            }
        );
    
        $(document).on(
            "click",
            "#btnBackToReports",
            async function () {

                try {

                    const patient =
                        await PatientCache.refresh(
                            modalElement.querySelector(
                                "[data-patient-selection-context]"
                            )
                        );

                    if (!patient) {
                        return;
                    }

                    await loadTestResults(
                        patient.patientId
                    );

                }
                catch (error) {

                    console.error(error);

                    showError(
                        "Unable to load reports."
                    );

                }

            }
        );

        $(document).on(
            "change",
            ".panel-select",
            function () {

                const select = $(this);

                const row =
                    select.closest(".result-row");

                const option =
                    select.find("option:selected");

                row.find(".test-panel-id")
                .val(select.val());

                row.find(".test-panel-code")
                .val(option.attr("data-code") || "");

                row.find(".test-panel-name")
                .val(option.attr("data-name") || "");

            }
        );

        $(document).on(
            "submit",
            "#testResultForm",
            async function (event) {

                event.preventDefault();

                const form =
                    $(this);

                try {

                    showLoading();

                    const response =
                        await $.ajax({

                            url:
                                "/TestResult/Add",

                            type:
                                "POST",

                            data:
                                form.serialize()

                        });

                    if (response.success) {

                        const patient =
                            await PatientCache.refresh(
                                modalElement.querySelector(
                                    "[data-patient-selection-context]"
                                )
                            );

                        await loadTestResults(
                            patient.patientId
                        );

                    }
                    else {

                        showError(
                            response.message
                        );

                    }

                }
                catch (error) {

                    console.error(error);

                    showError(
                        "Unable to save report."
                    );

                }
                finally {

                    hideLoading();

                }

            }
        );

    }

    // ============================================================
    // LOAD REPORT LIST
    // ============================================================

    async function loadTestResults(
        patientId
    ) {

        showLoading();
        clearMessages();

        try {

            const html =
                await $.get(
                    "/TestResult/ListReports",
                    {
                        patientId:
                            patientId
                    }
                );

            $("#testResultContent")
                .html(html);

        }
        catch (error) {

            console.error(error);

            showError(
                "Unable to load previous reports."
            );

        }
        finally {

            hideLoading();

        }

    }

    // ============================================================
    // ADD REPORT
    // ============================================================

    async function openAddReport(
        patientId
    )
    {
        showLoading();

        try {

            const html =
                await $.get(
                    "/TestResult/Add",
                    {
                        patientId:
                            patientId
                    }
                );

            $("#testResultContent")
                .html(html);

            wireParameterAutocomplete($("#resultRows .result-row"));

        }
        catch (error) {

            console.error(error);

            showError(
                "Unable to load report editor."
            );

        }
        finally {

            hideLoading();

        }
    }

    // ============================================================
    // EDIT REPORT
    // ============================================================

    async function openEditReport(
        reportId
    ) {

        showLoading();

        try {

            const html =
                await $.get(
                    "/TestResult/Edit",
                    {
                        id: reportId
                    }
                );

            $("#testResultContent")
                .html(html);

            wireParameterAutocomplete("#resultRows");

        }
        catch (error) {

            console.error(error);

            showError(
                "Unable to load report."
            );

        }
        finally {

            hideLoading();

        }

    }

    // ============================================================
    // VIEW REPORT
    // ============================================================

    async function openViewReport(
        reportId
    ) {

        showLoading();

        try {

            const html =
                await $.get(
                    "/TestResult/ViewReport",
                    {
                        id: reportId
                    }
                );

            $("#testResultContent")
                .html(html);

        }
        catch (error) {

            console.error(error);

            showError(
                "Unable to load report."
            );

        }
        finally {

            hideLoading();

        }

    }

    function wireParameterAutocomplete(
        container
    ) {

        $(container)
            .find(".parameter-search")
            .autocomplete({

                minLength: 2,

                source: async function (
                    request,
                    response
                ) {

                    try {

                        const data =
                            await $.get(
                                "/TestDefinition/SearchParameter",
                                {
                                    term:
                                        request.term
                                }
                            );

                        response(

                            $.map(
                                data,
                                function (
                                    item
                                ) {

                                    return {

                                        label:
                                            item.code +
                                            " - " +
                                            item.name,

                                        value:
                                            item.code,

                                        parameter:
                                            item

                                    };

                                }
                            )

                        );

                    }
                    catch (error) {

                        console.error(
                            error
                        );

                        response([]);

                    }

                },

                select: async function (
                    event,
                    ui
                ) {

                    const row =
                        $(this)
                            .closest(
                                ".result-row"
                            );

                    const parameter =
                        ui.item.parameter;

                    // ------------------------------------
                    // Parameter
                    // ------------------------------------

                    row.find(
                        ".test-parameter-id"
                    )
                    .val(
                        parameter.id
                    );

                    row.find(
                        ".test-parameter-code"
                    )
                    .val(
                        parameter.code
                    );

                    row.find(
                        ".test-parameter-name"
                    )
                    .val(
                        parameter.name
                    );

                    row.find(
                        ".parameter-name"
                    )
                    .text(
                        parameter.name
                    );

                    // ------------------------------------
                    // Range
                    // ------------------------------------

                    let rangeText =
                        parameter.normalRange;

                    if (

                        !rangeText &&

                        parameter.normalMin != null &&

                        parameter.normalMax != null

                    ) {

                        rangeText =
                            parameter.normalMin +
                            " - " +
                            parameter.normalMax;

                    }

                    row.find(
                        ".normal-range-display"
                    )
                    .text(
                        rangeText || ""
                    );

                    row.find(
                        ".normal-range"
                    )
                    .val(
                        rangeText || ""
                    );

                    row.find(
                        ".normal-min"
                    )
                    .val(
                        parameter.normalMin
                    );

                    row.find(
                        ".normal-max"
                    )
                    .val(
                        parameter.normalMax
                    );

                    // ------------------------------------
                    // Unit
                    // ------------------------------------

                    row.find(
                        ".unit-display"
                    )
                    .text(
                        parameter.unit || ""
                    );

                    row.find(
                        ".unit"
                    )
                    .val(
                        parameter.unit || ""
                    );

                    // ------------------------------------
                    // Data Type
                    // ------------------------------------

                    row.find(
                        ".data-type"
                    )
                    .val(
                        parameter.dataType
                    );

                    // ------------------------------------
                    // Test Method
                    // ------------------------------------

                    row.find(
                        'input[name$=".TestMethod"]'
                    )
                    .val(
                        parameter.testMethod || ""
                    );

                    // ------------------------------------
                    // Panels
                    // ------------------------------------

                    await loadParameterPanels(
                        row,
                        parameter.id
                    );

                    // ------------------------------------
                    // Previous Result
                    // ------------------------------------

                    if (
                        testResultPatient
                    ) {

                        await loadPreviousResult(
                            row,
                            parameter.id
                        );

                    }

                }

            });

    }

    // ============================================================
    // UI HELPERS
    // ============================================================

    function showLoading() {

        $("#testResultLoading")
            .removeClass("d-none");

    }

    function hideLoading() {

        $("#testResultLoading")
            .addClass("d-none");

    }

    function clearMessages() {

        $("#testResultError")
            .addClass("d-none")
            .empty();

        $("#testResultSuccess")
            .addClass("d-none")
            .empty();

    }

    function showError(
        message
    ) {

        $("#testResultError")
            .removeClass("d-none")
            .text(message);

    }

    async function refreshSelectedPatient() {

        const patient =
            await PatientCache.refresh(
                modalElement.querySelector(
                    "[data-patient-selection-context]"
                )
            );

        if (patient) {
            testResultPatient = patient;
        }

        return patient;

    }

    function renumberRows() 
    {

        $("#resultRows .result-row")
            .each(function (index) {

                $(this)
                    .find(
                        "input, select, textarea"
                    )
                    .each(function () {

                        const element =
                            $(this);

                        const name =
                            element.attr("name");

                        if (!name) {
                            return;
                        }

                        element.attr(
                            "name",
                            name.replace(
                                /Results\[\d+\]/,
                                `Results[${index}]`
                            )
                        );

                    });

            });

    }

    async function loadParameterPanels(row, parameterId )
    {

        try {

            const panels =
                await $.get(
                    "/TestDefinition/GetParameterPanels",
                    {
                        parameterId: parameterId
                    }
                );

            const select = row.find(".panel-select");

            const hiddenPanelId = row.find(".test-panel-id");
            const hiddenPanelCode = row.find(".test-panel-code");
            const hiddenPanelName = row.find(".test-panel-name");

            select.empty();

            // No panels

            if (!panels || panels.length === 0) {

                hiddenPanelId.val("");
                hiddenPanelCode.val("");
                hiddenPanelName.val("");

                select
                    .append(
                        $("<option>")
                            .val("")
                            .text("-- No Panel --")
                    )
                    .prop("disabled", true);

                return;
            }

            // Single panel

            if (panels.length === 1) {

                const panel =
                    panels[0];

                hiddenPanelId.val(panel.id);
                hiddenPanelCode.val(panel.code);
                hiddenPanelName.val(panel.name);

                select
                    .append(
                        $("<option>")
                            .val(panel.id)
                            .attr("data-code", panel.code)
                            .attr("data-name", panel.name)
                            .text(panel.code + " - " + panel.name)
                    )
                    .val(panel.id)
                    .prop("disabled", true);

                // Optional:
                // hide dropdown when only one panel exists

                //select.closest(".panel-container")
                //        .addClass("d-none");

                return;
            }

            // Multiple panels

            hiddenPanelId.val("");
            hiddenPanelCode.val("");
            hiddenPanelName.val("");

            //select.closest(".panel-container")
            //    .removeClass("d-none");

            select
                .prop("disabled", false)
                .append(
                    $("<option>")
                        .val("")
                        .text("-- Select Panel --")
                );

            $.each(
                panels,
                function (
                    index,
                    panel
                ) {

                    select.append(
                        $("<option>")
                            .val(panel.id)
                            .attr("data-code", panel.code)
                            .attr("data-name", panel.name)
                            .text(panel.code + " - " + panel.name)
                    );

                }
            );

        }
        catch (error) {

            console.error(
                "Error loading parameter panels",
                error
            );

        }

    }

    async function loadPreviousResult(
        row,
        parameterId
    ) {
        try {

            const patientId =
                $("#PatientId").val();

            const testDate =
                $("#TestDate").val();

            const result =
                await $.get(
                    "/TestResult/GetPreviousResult",
                    {
                        patientId: patientId,
                        parameterId: parameterId,
                        beforeDate: testDate
                    }
                );

                if (!result || !result.value) {

                    row.find(".previous-result")
                        .text("-");

                    return;
                }

                row.find(".previous-result")
                    .text(
                        `${result.value} (${result.testDate})`
                    );

        }
        catch (error) {

            console.error(
                "Unable to load previous result",
                error
            );

            row.find(".previous-result")
                .text("-");

        }
    }

})();