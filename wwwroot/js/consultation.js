const RowState = {
    Unchanged: 0,
    Added: 1,
    Modified: 2,
    Deleted: 3
};

const Selectors = {
    Grid: ".consultation-grid",
    Section: ".consultation-section",
    Row: "tr[data-line]",
    TemplateRow: "tr[data-template='true']",
    AddLine: ".add-line",
    DeleteLine: ".delete-line",
    RowState: ".row-state",
    RowGuid: ".row-guid",
    LineNumber: ".line-number-hidden",
    OptionList: ".option-list",
    TableBody: "tbody"
};

const Consultation = {

    initialize() {
        this.initializeRows();
        this.wireEvents();
        this.renumberAllSections();
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

    handleClick(e) {

        const addButton = e.target.closest(Selectors.AddLine);

        if (addButton) {
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

        if (e.target.matches(`${Selectors.OptionList} input[type=radio]`)) {
            this.handleRadioButton(e.target);
        }

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
        const template = section.querySelector(Selectors.TemplateRow);

        if (!template)
            return null;

        const row = template.cloneNode(true);

        row.removeAttribute("data-template");
        row.setAttribute("data-line", "");
        row.classList.remove("d-none");
        row.style.display = "";

        return row;
    },

    addLine(button) {

    },

    deleteLine(button) {

    },

    markRowModified(control) {

    },

    renumberSection(section) {

    },

    renumberAllSections() {

    },

    getRow(element) {

        return element.closest(Selectors.Row);

    },

    getTable(element) {

        return element.closest(Selectors.Grid);

    },

    getSection(element) {

        return element.closest(Selectors.Section);

    },

    getRowState(row) {

        return row.querySelector(Selectors.RowState);

    },

    getLineNumber(row) {

        return row.querySelector(Selectors.LineNumber);

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

