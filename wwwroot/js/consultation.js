const RowState = {
    Unchanged: 0,
    Added: 1,
    Modified: 2,
    Deleted: 3
};

const Selectors = {
    Grid: ".consultation-grid",
    Body: ".consultation-body",
    Template: ".consultation-template",
    AddLine: ".add-line",
    DeleteLine: ".delete-line",

    Row: "tr, .consultation-row",

    LineNo: ".line-no",
    RowNumber: ".row-number-hidden",
    RowGuid: ".row-guid",
    RowState: ".row-state",

    Section: ".consultation-section",

    OptionList: ".option-list",

    TableBody: "tbody"
};

const Consultation = {

    initialize() {
        this.initializeRows();
        this.wireEvents();
        this.renumberAllSections();
        this.initializeMedicineAutocomplete(document);
    },

    wireEvents() {

        document.addEventListener("click", this.handleClick.bind(this));

        document.addEventListener("change", this.handleChange.bind(this));

        document.addEventListener("input", this.handleInput.bind(this));

        document.querySelector(".consultation-form")
                ?.addEventListener("submit", this.handleSubmit.bind(this));

    },

    initializeRows() {

        document.querySelectorAll(`${Selectors.Grid} ${Selectors.TableBody} ${Selectors.Row}`)
            .forEach(row => {

                const guid = row.querySelector(Selectors.RowGuid);

                if (guid && !guid.value.trim())
                    guid.value =  this.generateGuid();

                const state = row.querySelector(Selectors.RowState);

                if (state && !state.value.trim())
                    state.value = RowState.Unchanged;

            });
    },

    initializeMedicineAutocomplete(container) {

        $(container)
            .find('.consultation-autocomplete[data-source="Medicines"]')
            .each(function () {

                const input = $(this);

                // Already initialized by jQuery UI
                if (input.autocomplete("instance"))
                    return;

                wireMedicineAutocomplete(this);
            });
    },

    handleClick(e) {
        console.log("Click detected");

        const addButton = e.target.closest(Selectors.AddLine);

        if (addButton) {
            console.log("Add Line clicked");
            e.preventDefault();
            this.addLine(addButton);
            return;
        }

        const deleteButton = e.target.closest(Selectors.DeleteLine);

        if (deleteButton) {
            e.preventDefault();
            this.deleteLine(deleteButton);
            return;
        }

    },

    handleChange(e) {

        // Handle option groups
        if (e.target.matches(
            `${Selectors.OptionList} input[type=radio], ` +
            `${Selectors.OptionList} input[type=checkbox], ` +
            `${Selectors.OptionList} select`
        )) {
            if (e.target.matches(`${Selectors.OptionList} input[type=radio]`)) {
                this.handleRadioButton(e.target);
            }
        }

        // Any changed field/option modifies the containing row
        this.markRowModified(e.target);
    },

    handleInput(e) {

        this.markRowModified(e.target);

    },

    handleRadioButton(radio) {

        const optionList = radio.closest(Selectors.OptionList);

        optionList.querySelectorAll('input[type=radio]').forEach(r => {

            if (r !== radio)
                r.checked = false;

        });

    },

    handleSubmit(e) {

        // Future:
        // - Remove deleted Added rows
        // - Validate multiline sections
        // - Ensure line numbers are sequential

    },

    cloneTemplateRow(section) {
        const templateBody = section.querySelector(Selectors.Template);

        if (!templateBody)
            return null;

        const templateRow = templateBody.querySelector("tr");

        if (!templateRow)
            return null;

        const row = templateRow.cloneNode(true);

        // Make visible
        row.classList.remove("d-none");
        row.removeAttribute("data-template");
        row.style.display = "";

        // Reset row attributes
        row.dataset.rowid = "0";
        row.dataset.rownumber = "";
        row.dataset.rowguid = "";

        return row;
    },

    prepareNewRow(row, rowIndex) {
        // Replace TemplateRow -> Rows[rowIndex]
        row.querySelectorAll("*").forEach(element => {

            ["name", "id", "for"].forEach(attr => {

                const value = element.getAttribute(attr);

                if (!value)
                    return;

                let updated = value;

                // name="Sections[2].TemplateRow.Fields[0].FieldValue"
                updated = updated.replace(
                    /\.TemplateRow\./g,
                    `.Rows[${rowIndex}].`
                );

                // id="Sections_2__TemplateRow_Fields_0__FieldValue"
                updated = updated.replace(
                    /_TemplateRow_/g,
                    `_Rows_${rowIndex}__`
                );

                element.setAttribute(attr, updated);

            });

        });

        // Row metadata
        row.dataset.rowid = "0";
        row.dataset.rownumber = (rowIndex + 1).toString();
        const guid = this.generateGuid();
        row.dataset.rowguid = guid;

        // Set Hidden fields
        const rowGuid = row.querySelector(Selectors.RowGuid);

        if (rowGuid)
            rowGuid.value = guid;

        const rowNumber = row.querySelector(Selectors.RowNumber);

        if (rowNumber)
            rowNumber.value = rowIndex + 1;

        const rowState = row.querySelector(Selectors.RowState);

        if (rowState)
            rowState.value = RowState.Added;

      //  row.querySelectorAll("input[name$='.RowState']").forEach(state => {
      //                              state.value = RowState.Added;
      //                          });

        // Reset hidden keys
        const rowId = row.querySelector("input[name$='.RowId']");

        if (rowId)
            rowId.value = "0";

        // Clear editors
        this.clearEditors(row);

        // Display line number
        const lineNo = row.querySelector(Selectors.LineNo);

        if (lineNo)
            lineNo.textContent = rowIndex + 1;

        const deleteButton = row.querySelector(Selectors.DeleteLine);

        if (deleteButton) {
            deleteButton.dataset.rowid = "0";
        }
    },

    clearEditors(row) {
        // Text inputs (excluding hidden)
        row.querySelectorAll("input[type='text'], input[type='number'], input[type='date'], input[type='time']")
            .forEach(input => input.value = "");

        // Textareas
        row.querySelectorAll("textarea")
            .forEach(t => t.value = "");

        // Selects
        row.querySelectorAll("select")
            .forEach(s => s.selectedIndex = 0);

        // Checkboxes
        row.querySelectorAll("input[type='checkbox']")
            .forEach(c => c.checked = false);

        // Radios
        row.querySelectorAll("input[type='radio']")
            .forEach(r => r.checked = false);

        // Lookup backing IDs
        row.querySelectorAll(".field-data-id")
            .forEach(input => input.value = "");
    },

    addLine(button) {
        console.log("Inside addLine");
        const table = this.getTable(button);
        console.log(table);
        const body = table.querySelector(Selectors.Body);

        if (!body)
            return; 

        const row = this.cloneTemplateRow(table);
        console.log(row);
        
        if (!row)
            return;

        const rowIndex = body.querySelectorAll("tr").length;
        this.prepareNewRow(row, rowIndex);
        body.appendChild(row);
        this.initializeMedicineAutocomplete(row);
        this.renumberSection(table);
    },

    deleteLine(button) {
        const table = this.getTable(button);
        const row = this.getRow(button);

        if (!row)
            return;

        if (!confirm("Delete this row?"))
            return;

        const rowId = parseInt(
            row.querySelector("input[name$='.RowId']")?.value || "0"
        );

        // New row - never saved
        if (rowId === 0) {
            row.remove();
            this.renumberSection(table);
            return;
        }

        // Existing row
        const rowState = row.querySelector(Selectors.RowState);

        if (rowState)
            rowState.value = RowState.Deleted;

        row.style.display = "none";
        this.renumberSection(table);
    },

    markRowModified(control) {

        if (!control)
            return;

        // Ignore hidden inputs.
        // RowState itself is hidden, and changing it programmatically
        // should never recursively trigger modification logic.
        if (control.type === "hidden")
            return;

        // Ignore buttons
        if (control.matches(
            "button, input[type='button'], input[type='submit'], input[type='reset']"
        ))
            return;

        // Find the row containing the changed control.
        //
        // This works for:
        //   <tr>...</tr>                  -> multiline
        //   <div class="consultation-row">...</div> -> normal section
        const row = control.closest(Selectors.Row);

        if (!row)
            return;

        // RowState is directly under the row.
        const rowState = row.querySelector(":scope > .row-state");

        if (!rowState)
            return;

        const currentState = parseInt(rowState.value, 10);

        // Added rows must remain Added.
        if (currentState === RowState.Added)
            return;

        // Deleted rows must remain Deleted.
        if (currentState === RowState.Deleted)
            return;

        // Only change Unchanged -> Modified.
        if (currentState === RowState.Unchanged) {
            rowState.value = RowState.Modified;
        }
    },

    renumberSection(table) {

        if (!table)
            return;

        const body = table.querySelector(Selectors.Body);

        if (!body)
            return;

        let lineNo = 1;

        body.querySelectorAll("tr").forEach(row => {

            if (getComputedStyle(row).display === "none")
                return;

            row.dataset.rownumber = lineNo;

            const lineSpan = row.querySelector(Selectors.LineNo);
            if (lineSpan)
                lineSpan.textContent = lineNo;

            const hiddenRowNumber = row.querySelector(Selectors.RowNumber);
            if (hiddenRowNumber)
                hiddenRowNumber.value = lineNo;

            const displayOrder = row.querySelector("input[name$='.DisplayOrder']");
            if (displayOrder)
                displayOrder.value = lineNo;

            lineNo++;
        });
    },

    renumberAllSections() {

    },

    getRow(element) {

        return element.closest(Selectors.Row);

    },

    getTable(element) {
        if (!element)
            return null;

        if (element.matches("table"))
            return element;

        return element.closest("table");
    },

    getSection(element) {

        return element.closest(Selectors.Section);

    },

    getRowState(row) {

        return row.querySelector(Selectors.RowState);

    },

    getLineNumber(row) {

        return row.querySelector(Selectors.LineNo);

    },

    getRowGuid(row) {

        return row.querySelector(Selectors.RowGuid);

    },

    generateGuid() {

        if (window.crypto && crypto.randomUUID)
            return crypto.randomUUID();

        return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {

            const r = Math.random() * 16 | 0;
            const v = c === 'x' ? r : (r & 0x3 | 0x8);

            return v.toString(16);

        });

    },

};

document.addEventListener("DOMContentLoaded", () => Consultation.initialize());

