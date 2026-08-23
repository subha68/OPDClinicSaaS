(function () {

    "use strict";

    let currentReportResult = null;

    // ============================================================
    // INITIALIZATION
    // ============================================================

    document.addEventListener("DOMContentLoaded", function () {

        const form = document.getElementById("reportForm");

        if (!form) {
            return;
        }

        form.addEventListener("submit", function (event) {

            event.preventDefault();

            if (!validateFilters()) {
                return;
            }

            executeReport();
        });


        const clearButton = document.getElementById("btnClearFilters");

        if (clearButton) {

            clearButton.addEventListener("click", function () {

                clearFilters();

            });

        }

        const exportCsvButton = document.getElementById("btnExportCsv");

        if (exportCsvButton) {

            exportCsvButton.addEventListener("click", function () {

                    exportCsv();

                }
            );

        }

        const exportExcelButton = document.getElementById("btnExportExcel");

        if (exportExcelButton) {

            exportExcelButton.addEventListener("click", function () {

                    exportExcel();

                }
            );

        }
        
        const printButton = document.getElementById("btnPrintReport");

        if (printButton) {

            printButton.addEventListener("click", function () {

                    printReport();

                }
            );

        }

        wireReportLookups();

    });

    // ============================================================
    // DATE PERIOD
    // ============================================================

    document.querySelectorAll(".report-period-radio")
        .forEach(function (radio) {

            radio.addEventListener(
                "change",
                function () {

                    if (!this.checked) {
                        return;
                    }

                    applyDatePeriod(this.value);
                    clearResult("Filters changed. Click <strong>Run Report</strong> to refresh the report.");
                }
            );

        });

    document.querySelectorAll(".report-date-filter")
        .forEach(function (input) {

            input.addEventListener(
                "change",
                function () {

                    document
                        .querySelectorAll(".report-period-radio")
                        .forEach(function (radio) {

                            radio.checked = false;

                        });

                }
            );

        });

        document.querySelectorAll("#reportForm input:not([type='hidden']), " + "#reportForm select")
            .forEach(function (control) {

                control.addEventListener(
                    "change",
                    function () {

                        clearResult("Filters changed. Click <strong>Run Report</strong> to refresh the report.");

                    }
                );

            });

    // Apply the default Today selection on page load.

    const selectedPeriod = document.querySelector(".report-period-radio:checked");

    if (selectedPeriod) {
        applyDatePeriod(selectedPeriod.value);
    }

    // ============================================================
    // APPLY DATE PERIOD
    // ============================================================

    function applyDatePeriod(period) {

        const dateInputs = Array.from(document.querySelectorAll(".report-date-filter"));

        if (dateInputs.length === 0) {
            return;
        }

        const today = getTodayDate();

        // ========================================================
        // TODAY
        // ========================================================

        if (period === "today") {

            dateInputs.forEach(function (input) {

                input.value = today;

            });

            return;
        }


        // A range is required for the remaining options.

        if (dateInputs.length < 2) {
            return;
        }


        const firstDateInput = dateInputs[0];
        const secondDateInput = dateInputs[1];

        // ========================================================
        // CALENDAR YEAR
        // ========================================================

        if (period === "calendar-year") {
            const year = new Date().getFullYear();
            firstDateInput.value = year + "-01-01";
            secondDateInput.value = today;
            return;
        }

        // ========================================================
        // FINANCIAL YEAR
        // ========================================================

        if (period === "financial-year") {
            const currentDate = new Date();
            const currentYear = currentDate.getFullYear();
            const currentMonth = currentDate.getMonth() + 1;
            const financialYearStart = currentMonth >= 4
                                            ? currentYear
                                            : currentYear - 1;
            firstDateInput.value = financialYearStart + "-04-01";
            secondDateInput.value = today;
            return;
        }

        // ========================================================
        // THIS MONTH
        // ========================================================

        if (period === "this-month") {
            const todayDate = new Date();
            const year = todayDate.getFullYear();
            const month = String(todayDate.getMonth() + 1)
                            .padStart(2, "0");
            firstDateInput.value = year + "-" + month + "-01";
            secondDateInput.value = today;
            return;
        }
    }    

    // ============================================================
    // EXECUTE REPORT
    // ============================================================

    async function executeReport() {

        const reportCode = document.getElementById("reportCode").value;
        const form = document.getElementById("reportForm");
        const button = document.getElementById("btnRunReport");
        const spinner = document.getElementById("runReportSpinner");
        const buttonText = document.getElementById("runReportText");

        hideMessage();
        setLoading(true);

        try {
            const formData = new FormData(form);

            const response = await fetch("/Report/Execute?reportCode="
                                            + encodeURIComponent(reportCode),
                                            {
                                                method: "POST",
                                                body: formData
                                            }
                                        );


            const contentType = response.headers.get("content-type") || "";

            let data;

            if (contentType.includes("application/json")) {

                data = await response.json();

            }
            else {

                throw new Error(
                    "The server returned an unexpected response."
                );

            }


            if (!response.ok) {

                const message =
                    data?.message ||
                    "Unable to execute the report.";

                throw new Error(message);

            }


            renderResult(data);

        }
        catch (error) {

            console.error(
                "Report execution failed:",
                error
            );

            showMessage(
                error.message ||
                "Unable to execute the report.",
                "danger"
            );

            clearResult();

        }
        finally {

            setLoading(false);

        }

    }


    // ============================================================
    // RENDER RESULT
    // ============================================================

    function renderResult(data) {

        currentReportResult = data;
        const resultContainer = document.getElementById("reportResult");

        if (!resultContainer) {
            return;
        }

        resultContainer.innerHTML = "";

        const columns = [...(data?.columns || [])]
                            .sort(function (a, b) {
                                return (a.displayOrder || 0) -
                                    (b.displayOrder || 0);
                            });

        const rows = data?.rows || [];
        const summary = data?.summary || [];

        console.log("Report columns:", columns);
        console.log("Report rows:", rows);
        console.log("Report summary:", summary);

        if (columns.length === 0) {

            showMessage(
                "No visible columns are defined for this report.",
                "warning"
            );

            return;
        }

        // Summary
        renderSummary(summary, resultContainer);

        // Table
        renderReportTable(columns, rows, resultContainer);

        // Export buttons
        showExportButtons();

        hideMessage();
    }

    function renderSummary(summary, container) {

        if (!summary || summary.length === 0) {
            return;
        }

        const summaryContainer =
            document.createElement("div");

        summaryContainer.className =
            "report-summary mb-3";

        summary.forEach(function (item) {

            const summaryItem =
                document.createElement("div");

            summaryItem.className =
                "report-summary-item";

            const label =
                document.createElement("span");

            label.className =
                "report-summary-label";

            label.textContent =
                item.label || "";

            const value =
                document.createElement("span");

            value.className =
                "report-summary-value";

            value.textContent =
                item.value ?? "";

            summaryItem.appendChild(label);
            summaryItem.appendChild(value);

            summaryContainer.appendChild(summaryItem);
        });

        container.appendChild(summaryContainer);
    }

    function renderReportTable(columns, rows, container)
    {
        // ========================================================
        // TABLE WRAPPER
        // ========================================================

        const tableWrapper =
            document.createElement("div");

        tableWrapper.className =
            "table-responsive";


        // ========================================================
        // TABLE
        // ========================================================

        const table =
            document.createElement("table");

        table.className =
            "table table-bordered table-striped table-sm";


        // ========================================================
        // HEADER
        // ========================================================

        const thead =
            document.createElement("thead");

        const headerRow =
            document.createElement("tr");


        columns.forEach(function (column) {

            const th =
                document.createElement("th");

            th.textContent =
                column.displayLabel ||
                column.columnCode;

            if (isNumericColumn(column)) {

                th.classList.add("text-end");

            }

            headerRow.appendChild(th);

        });


        thead.appendChild(headerRow);

        table.appendChild(thead);


        // ========================================================
        // BODY
        // ========================================================

        const tbody =
            document.createElement("tbody");


        if (rows.length === 0) {

            const tr =
                document.createElement("tr");

            const td =
                document.createElement("td");

            td.colSpan =
                columns.length;

            td.className =
                "text-center text-muted py-3";

            td.textContent =
                "No data found for the selected filters.";

            tr.appendChild(td);

            tbody.appendChild(tr);

        }
        else {

            rows.forEach(function (row) {

                const tr =
                    document.createElement("tr");


                columns.forEach(function (column) {

                    const td =
                        document.createElement("td");

                    const value =
                        getRowValue(
                            row,
                            column.columnCode
                        );

                    td.textContent = value ?? ""; 

                    if (isNumericColumn(column)) {

                        td.classList.add("text-end");

                    }


                    tr.appendChild(td);

                });


                tbody.appendChild(tr);

            });

        }


        table.appendChild(tbody);


        // ========================================================
        // DISPLAY
        // ========================================================

        tableWrapper.appendChild(table);

        container.appendChild(tableWrapper);
    }

    function isNumericColumn(column) {

        const dataType = (column.dataType || "").toLowerCase();

        return dataType === "integer" ||
            dataType === "decimal" ||
            dataType === "number";
    }

    function getRowValue(row, columnCode) {

        if (!row) {
            return null;
        }

        if (Object.prototype.hasOwnProperty.call(row, columnCode)) {

            return row[columnCode];
        }

        const normalizedColumn = normalizeName(columnCode);

        const key = Object.keys(row).find(function (key) {

                return normalizeName(key) === normalizedColumn;

            });

        return key ? row[key] : null;
    }

    // ============================================================
    // CLEAR FILTERS
    // ============================================================

    function clearFilters() {

        const form =
            document.getElementById("reportForm");


        if (!form) {
            return;
        }


        form.querySelectorAll(
            "input:not([type='hidden']), select"
        ).forEach(function (control) {

            if (control.type === "checkbox" ||
                control.type === "radio") {

                control.checked = false;

            }
            else {

                control.value = "";

            }

        });


        clearResult();
        hideMessage();

    }


    // ============================================================
    // LOADING STATE
    // ============================================================

    function setLoading(isLoading) {

        const button =
            document.getElementById("btnRunReport");

        const spinner =
            document.getElementById("runReportSpinner");

        const buttonText =
            document.getElementById("runReportText");


        if (button) {

            button.disabled =
                isLoading;

        }


        if (spinner) {

            spinner.style.display =
                isLoading ? "inline-block" : "none";

        }


        if (buttonText) {

            buttonText.textContent =
                isLoading
                    ? "Running..."
                    : "Run Report";

        }

    }


    // ============================================================
    // MESSAGE
    // ============================================================

    function showMessage(message, type) {

        const element =
            document.getElementById("reportMessage");


        if (!element) {
            return;
        }


        element.className =
            "alert alert-" + type;


        element.textContent =
            message;


        element.style.display =
            "block";

    }


    function hideMessage() {

        const element =
            document.getElementById("reportMessage");


        if (!element) {
            return;
        }


        element.style.display =
            "none";

        element.textContent =
            "";

    }


    // ============================================================
    // CLEAR RESULT
    // ============================================================

    function clearResult(message) {

        currentReportResult = null;
        const container = document.getElementById("reportResult");

        if (container) {
            const displayMessage =
                message ||
                "Enter the filters and click <strong>Run Report</strong>.";

            container.innerHTML =
                '<div class="text-muted">' +
                displayMessage +
                '</div>';
        }

        const summary = document.getElementById("reportSummary");
        const summaryItems = document.getElementById("reportSummaryItems");

        if (summary) {
            summary.style.display = "none";
        }

        if (summaryItems) {
            summaryItems.innerHTML = "";
        }

        // Hide export buttons

        const exportCsvButton = document.getElementById("btnExportCsv");

        if (exportCsvButton) {

            exportCsvButton.style.display = "none";
        }
        
        const exportExcelButton = document.getElementById("btnExportExcel");

        if (exportExcelButton) {

            exportExcelButton.style.display = "none";
        }
        
        const printButton = document.getElementById("btnPrintReport");

        if (printButton) {

            printButton.style.display = "none";
        }
    }


    // ============================================================
    // NAME NORMALIZATION
    // ============================================================

    function normalizeName(value) {

        return String(value)
            .replace(/_/g, "")
            .toLowerCase();

    }

    function getTodayDate() {

        const today = new Date();
        const year = today.getFullYear();
        const month = String(today.getMonth() + 1).padStart(2, "0");
        const day = String(today.getDate()).padStart(2, "0");

        return year + "-" + month + "-" + day;
    }

    function validateFilters() {

        const form = document.getElementById("reportForm");

        if (!form) {
            return false;
        }

        clearFilterValidation();

        let isValid = true;

        const filters = window.reportDefinition?.filters || [];

        filters.forEach(function (filter) {

            const inputId =
                "filter_" + filter.filterCode;

            const control =
                document.getElementById(inputId);

            if (!control) {
                return;
            }

            const value =
                getFilterValue(control);

            let errorMessage = "";


            // ====================================================
            // REQUIRED
            // ====================================================

            if (filter.isRequired &&
                isEmptyFilterValue(value)) {

                errorMessage =
                    filter.filterLabel +
                    " is required.";
            }


            // ====================================================
            // DATE VALIDATION
            // ====================================================

            if (!errorMessage &&
                filter.controlType?.toLowerCase() === "date" &&
                value) {

                if (!isValidDateValue(value)) {

                    errorMessage =
                        filter.filterLabel +
                        " must be a valid date.";
                }
            }


            // ====================================================
            // NUMBER VALIDATION
            // ====================================================

            if (!errorMessage &&
                filter.controlType?.toLowerCase() === "number" &&
                value) {

                if (isNaN(Number(value))) {

                    errorMessage =
                        filter.filterLabel +
                        " must be a valid number.";
                }
            }


            // ====================================================
            // DISPLAY ERROR
            // ====================================================

            if (errorMessage) {

                setFilterValidationError(
                    control,
                    errorMessage
                );

                isValid = false;
            }

        });

        return isValid;
    }

    function getFilterValue(control) {

        if (control.type === "checkbox") {

            return control.checked
                ? control.value
                : "";
        }

        return control.value?.trim() || "";
    }

    function isEmptyFilterValue(value) {

        return value === null ||
            value === undefined ||
            String(value).trim() === "";
    }

    function isValidDateValue(value) {

        if (!/^\d{4}-\d{2}-\d{2}$/.test(value)) {
            return false;
        }

        const parts =
            value.split("-");

        const year =
            Number(parts[0]);

        const month =
            Number(parts[1]);

        const day =
            Number(parts[2]);

        const date =
            new Date(year, month - 1, day);

        return date.getFullYear() === year &&
            date.getMonth() === month - 1 &&
            date.getDate() === day;
    }

    function setFilterValidationError(control, message) {

        control.classList.add("is-invalid");

        const error = control.parentElement
                ?.querySelector(".report-filter-error");

        if (error) {

            error.textContent = message;
            error.style.display = "block";
        }
    }

    function clearFilterValidation() {

        const form =
            document.getElementById("reportForm");

        if (!form) {
            return;
        }

        form.querySelectorAll(".is-invalid")
            .forEach(function (control) {

                control.classList.remove("is-invalid");

            });

        form.querySelectorAll(".report-filter-error")
            .forEach(function (error) {

                error.textContent = "";
                error.style.display = "none";

            });
    }

    function showExportButtons() {

        const canExport = window.reportDefinition?.canExport === true;
        const canPrint = window.reportDefinition?.canPrint === true;

        const exportCsvButton = document.getElementById("btnExportCsv");
        const exportExcelButton = document.getElementById("btnExportExcel");
        const printButton = document.getElementById("btnPrintReport");

        if (exportCsvButton) {
            exportCsvButton.style.display = canExport ? "inline-block" : "none";
        }

        if (exportExcelButton) {
            exportExcelButton.style.display = canExport ? "inline-block" : "none";
        }

        if (printButton) {
            printButton.style.display = canPrint ? "inline-block" : "none";
        }
    }

    async function exportCsv() {

        const reportCode = document.getElementById("reportCode").value;
        const reportId = document.getElementById("reportId").value;
        const form = document.getElementById("reportForm");
        const button = document.getElementById("btnExportCsv");
        const spinner = document.getElementById("exportCsvSpinner");
        const buttonText = document.getElementById("exportCsvText");

        if (!form) {
            return;
        }

        // ============================================================
        // VALIDATE FILTERS
        // ============================================================

        if (!validateFilters()) {
            return;
        }

        // ============================================================
        // LOADING STATE
        // ============================================================

        if (button) {
            button.disabled = true;
        }

        if (spinner) {
            spinner.style.display = "inline-block";
        }

        if (buttonText) {
            buttonText.textContent = "Exporting...";
        }

        try {
            const formData = new FormData(form);

            const response = await fetch("/Report/ExportCsv?reportId=" +
                                            encodeURIComponent(reportId) +
                                            "&reportCode=" +
                                            encodeURIComponent(reportCode),
                                            {
                                                method: "POST",
                                                body: formData
                                            }
                                        );


            if (!response.ok) {

                let message = "Unable to export the report.";

                const contentType = response.headers.get("content-type") || "";

                if (contentType.includes("application/json")) {
                    const data = await response.json();
                    message = data?.message || message;
                }

                throw new Error(message);
            }


            // ========================================================
            // DOWNLOAD FILE
            // ========================================================

            const blob = await response.blob();

            const disposition = response.headers.get("Content-Disposition");

            let fileName = reportCode + ".csv";

            if (disposition) {
                const match = disposition.match(/filename\*?=(?:UTF-8''|")?([^";]+)/i);

                if (match && match[1]) {
                    fileName = decodeURIComponent(match[1].replace(/"/g, ""));
                }
            } 

            const url = window.URL.createObjectURL(blob);
            const link = document.createElement("a");

            link.href = url;
            link.download = fileName;

            document.body.appendChild(link);
            link.click();
            link.remove();
            window.URL.revokeObjectURL(url);
        }
        catch (error) {

            console.error("CSV export failed:", error);

            showMessage(error.message || "Unable to export the report.",
                "danger"
            );

        }
        finally {

            if (button) {
                button.disabled = false;
            }

            if (spinner) {
                spinner.style.display = "none";
            }

            if (buttonText) {
                buttonText.textContent = "Export CSV";
            }

        }
    }

    async function exportExcel() {

        const reportCode =
            document.getElementById("reportCode").value;

        const reportId =
            document.getElementById("reportId").value;

        const form =
            document.getElementById("reportForm");

        const button =
            document.getElementById("btnExportExcel");

        const spinner =
            document.getElementById("exportExcelSpinner");

        const buttonText =
            document.getElementById("exportExcelText");


        if (!form) {
            return;
        }


        // ============================================================
        // VALIDATE FILTERS
        // ============================================================

        if (!validateFilters()) {
            return;
        }


        // ============================================================
        // LOADING STATE
        // ============================================================

        if (button) {
            button.disabled = true;
        }

        if (spinner) {
            spinner.style.display = "inline-block";
        }

        if (buttonText) {
            buttonText.textContent = "Exporting...";
        }


        try {

            const formData =
                new FormData(form);

            const response =
                await fetch(
                    "/Report/ExportExcel?reportId=" +
                    encodeURIComponent(reportId) +
                    "&reportCode=" +
                    encodeURIComponent(reportCode),
                    {
                        method: "POST",
                        body: formData
                    }
                );


            // ========================================================
            // ERROR RESPONSE
            // ========================================================

            if (!response.ok) {

                let message =
                    "Unable to export the report.";

                const contentType =
                    response.headers.get("content-type") || "";

                if (contentType.includes("application/json")) {

                    const data =
                        await response.json();

                    message =
                        data?.message ||
                        message;
                }

                throw new Error(message);
            }


            // ========================================================
            // DOWNLOAD FILE
            // ========================================================

            const blob =
                await response.blob();

            const disposition =
                response.headers.get(
                    "Content-Disposition"
                );

            let fileName =
                reportCode + ".xlsx";


            if (disposition) {

                const match =
                    disposition.match(
                        /filename\*?=(?:UTF-8''|")?([^";]+)/i
                    );

                if (match && match[1]) {

                    fileName =
                        decodeURIComponent(
                            match[1].replace(/"/g, "")
                        );
                }
            }


            const url =
                window.URL.createObjectURL(blob);

            const link =
                document.createElement("a");

            link.href = url;
            link.download = fileName;

            document.body.appendChild(link);

            link.click();

            link.remove();

            window.URL.revokeObjectURL(url);
        }
        catch (error) {

            console.error(
                "Excel export failed:",
                error
            );

            showMessage(
                error.message ||
                "Unable to export the report.",
                "danger"
            );

        }
        finally {

            if (button) {
                button.disabled = false;
            }

            if (spinner) {
                spinner.style.display = "none";
            }

            if (buttonText) {
                buttonText.textContent =
                    "Export Excel";
            }

        }
    }

async function printReport() {

    if (!currentReportResult) {

        showMessage(
            "Please run the report before printing.",
            "warning"
        );

        return;
    }

    const button =
        document.getElementById("btnPrintReport");

    const spinner =
        document.getElementById("printReportSpinner");

    const buttonText =
        document.getElementById("printReportText");


    if (button) {
        button.disabled = true;
    }

    if (spinner) {
        spinner.style.display = "inline-block";
    }

    if (buttonText) {
        buttonText.textContent = "Preparing...";
    }


    try {

        preparePrintLayout();


        // --------------------------------------------------------
        // Clean up after the browser finishes the print operation.
        //
        // This fires whether the user:
        //   - actually prints
        //   - cancels the print dialog
        // --------------------------------------------------------

        window.addEventListener(
            "afterprint",
            cleanupPrintLayout,
            { once: true }
        );


        // Give the browser time to render the print DOM.
        await new Promise(function (resolve) {

            setTimeout(resolve, 100);

        });


        window.print();

    }
    catch (error) {

        console.error(
            "Print failed:",
            error
        );

        cleanupPrintLayout();

        showMessage(
            error.message ||
            "Unable to print the report.",
            "danger"
        );

    }
    finally {

        if (button) {
            button.disabled = false;
        }

        if (spinner) {
            spinner.style.display = "none";
        }

        if (buttonText) {
            buttonText.textContent = "Print";
        }

    }
}

function cleanupPrintLayout() {

    const printHeader =
        document.getElementById("reportPrintHeader");

    if (printHeader) {
        printHeader.remove();
    }

}

function preparePrintLayout() {

    if (!currentReportResult) {
        return;
    }

        const resultContainer = document.getElementById("reportResult");

        if (!resultContainer) {
            return;
        }

        // Remove an existing print header
        const existingHeader = document.getElementById("reportPrintHeader");

        if (existingHeader) {
            existingHeader.remove();
        }

        const header = document.createElement("div");

        header.id = "reportPrintHeader";
        header.className = "report-print-header";

        // ============================================================
        // REPORT TITLE - not required as screen has Report Title
        // ============================================================

    /*    const title =
            document.createElement("h2");

        title.textContent =
            currentReportResult.reportTitle ||
            window.reportDefinition?.report?.reportName ||
            "Report";

        header.appendChild(title); */


        // ============================================================
        // DESCRIPTION - not required as screen has Report description
        // ============================================================

    /*    const description =
            window.reportDefinition?.report?.description;

        if (description) {

            const descriptionElement =
                document.createElement("div");

            descriptionElement.className =
                "report-print-description";

            descriptionElement.textContent =
                description;

            header.appendChild(descriptionElement);
        } */


        // ============================================================
        // GENERATED DATE
        // ============================================================

        if (currentReportResult.generatedOn) {

            const generated =
                document.createElement("div");

            generated.className =
                "report-print-generated";

            generated.textContent =
                "Generated On: " +
                formatGeneratedDate(
                    currentReportResult.generatedOn
                );

            header.appendChild(generated);
        }


        // ============================================================
        // FILTERS
        // ============================================================

        const filters =
            currentReportResult.filters || [];

        if (filters.length > 0) {

            const filterTitle =
                document.createElement("h5");

            filterTitle.textContent =
                "Filters";

            filterTitle.className =
                "report-print-section-title";

            header.appendChild(filterTitle);


            const filterTable =
                document.createElement("table");

            filterTable.className =
                "table table-sm table-bordered report-print-filters";


            filters.forEach(function (filter) {

                const row =
                    document.createElement("tr");

                const labelCell =
                    document.createElement("th");

                labelCell.textContent =
                    filter.filterLabel ||
                    filter.filterCode;

                const valueCell =
                    document.createElement("td");

                valueCell.textContent =
                    formatFilterValue(filter.value);

                row.appendChild(labelCell);
                row.appendChild(valueCell);

                filterTable.appendChild(row);
            });


            header.appendChild(filterTable);
        }


        // ============================================================
        // SUMMARY
        // ============================================================

        const summary =
            currentReportResult.summary || [];

        if (summary.length > 0) {

            const summaryTitle =
                document.createElement("h5");

            summaryTitle.textContent =
                "Summary";

            summaryTitle.className =
                "report-print-section-title";

            header.appendChild(summaryTitle);


            const summaryTable =
                document.createElement("table");

            summaryTable.className =
                "report-print-summary";


            summary.forEach(function (item) {

                const row =
                    document.createElement("tr");


                const labelCell =
                    document.createElement("td");

                labelCell.className =
                    "report-print-summary-label";

                labelCell.textContent =
                    item.label || "";


                const separatorCell =
                    document.createElement("td");

                separatorCell.className =
                    "report-print-summary-separator";

                separatorCell.textContent =
                    ":";


                const valueCell =
                    document.createElement("td");

                valueCell.className =
                    "report-print-summary-value";

                valueCell.textContent =
                    item.value ?? "";


                row.appendChild(labelCell);
                row.appendChild(separatorCell);
                row.appendChild(valueCell);

                summaryTable.appendChild(row);
            });


            header.appendChild(summaryTable);
        }


        // Put the print header before the actual report.
        resultContainer.prepend(header);
    }

    function formatGeneratedDate(value) {

        if (!value) {
            return "";
        }

        const date = new Date(value);

        if (isNaN(date.getTime())) {
            return String(value);
        }

        const day =
            String(date.getDate()).padStart(2, "0");

        const month =
            String(date.getMonth() + 1).padStart(2, "0");

        const year =
            date.getFullYear();

        const hours =
            String(date.getHours()).padStart(2, "0");

        const minutes =
            String(date.getMinutes()).padStart(2, "0");

        const seconds =
            String(date.getSeconds()).padStart(2, "0");

        return `${day}-${month}-${year} ${hours}:${minutes}:${seconds}`;
    }

    function formatFilterValue(value) {

        if (value === null ||
            value === undefined) {

            return "All";
        }

        // Handle empty strings
        if (typeof value === "string" &&
            value.trim() === "") {

            return "All";
        }

        // Handle boolean values
        if (typeof value === "boolean") {

            return value
                ? "Yes"
                : "No";
        }

        return String(value);
    }

    function wireReportLookups() {

        const filters = window.reportDefinition?.filters || [];

        filters.forEach(function (filter) {

            if ((filter.controlType || "").toLowerCase() !== "lookup") {
                return;
            }

            if (!filter.lookupCode) {
                return;
            }

            const inputId = "#filter_" + filter.filterCode;
            const hiddenId = inputId + "_value";

            if (!document.querySelector(inputId)) {
                return;
            }

            wireReportLookupAutocomplete(inputId,
                                            filter.lookupCode,
                                            function (item)
                {

                    $(hiddenId).val(item.id);

                    clearResult();
                    hideMessage();

                },
                "#reportForm"
            );

        });
    }    

})();   