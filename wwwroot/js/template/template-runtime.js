function initializeDesignerRuntime() {

    bindInsertUI();

    initializeSectionSortables();
    initializeFieldSortables();
    initializeOptionSortables();

    initializeAccordion();

    syncInactiveFilterState(); // instead of applyInactiveFilter directly
}

function syncInactiveFilterState() {

    // ensures DOM + UI are in sync after render
    applyInactiveFilter();
}

function applyInactiveFilter() {

    const showInactive = $("#showInactive").prop("checked");

    $(".section-card.inactive").closest(".section-wrapper")
        .toggle(showInactive);

    $(".field-card.inactive").closest(".field-wrapper")
        .toggle(showInactive);
}


function bindInsertUI() {
    wireInsert(
        ".insert-before",
        "/PrescriptionTemplate/InsertSection",
        "sectionId",
        "section-id",
        "sectionName",
        "Section Name",
        "Before"
    );

    wireInsert(
        ".insert-after",
        "/PrescriptionTemplate/InsertSection",
        "sectionId",
        "section-id",
        "sectionName",
        "Section Name",
        "After"
    );

    wireInsert(
        ".insert-field-before",
        "/PrescriptionTemplate/InsertField",
        "fieldId",
        "field-id",
        "fieldName",
        "Field Label",
        "Before",

        function (btn) {
            return {
                sectionId: btn.closest(".fields-container").data("section-id")
            };
        },

        function (btn, html) {
            btn.closest(".field-wrapper").before(html);
        });

    wireInsert(
        ".insert-field-after",
        "/PrescriptionTemplate/InsertField",
        "fieldId",
        "field-id",
        "fieldName",
        "Field Label",
        "After",

        function (btn) {
            return {
                sectionId: btn.closest(".fields-container").data("section-id")
            };
        },

        function (btn, html) {
            btn.closest(".field-wrapper").after(html);
        });

    wireInsert(
        ".insert-before-option",
        "/PrescriptionTemplate/InsertOption",
        "optionId",
        "option-id",
        "optionName",
        "Option Label",
        "Before",

        function (btn) {
            return {
                fieldId: btn.closest(".options-container").data("field-id")
            };
        },

        function (btn, html) {
            btn.closest(".option-wrapper").before(html);
        }
    );

    wireInsert(
        ".insert-after-option",
        "/PrescriptionTemplate/InsertOption",
        "optionId",
        "option-id",
        "optionName",
        "Option Label",
        "After",

        function (btn) {
            return {
                fieldId: btn.closest(".options-container").data("field-id")
            };
        },

        function (btn, html) {
            btn.closest(".option-wrapper").after(html);
        }
    );

}

function initializeAccordion() {

    wireAccordion(
        ".section-collapse",
        ".section-card",
        ".section-preview",
        ".expand-section i"
    );

    wireAccordion(
        ".field-collapse",
        ".field-card",
        ".field-preview",
        ".expand-field i"
    );
}

$(".section-collapse").each(function () {

    const card = $(this).closest(".section-card");

    if ($(this).hasClass("show")) {

        card.find(".section-preview").hide();

        card.find(".expand-section i")
            .removeClass("bi-chevron-right")
            .addClass("bi-chevron-down");
    }
});

