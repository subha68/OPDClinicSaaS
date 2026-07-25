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
            //const container = btn.closest(".field-wrapper")
            //                    .closest(".fields-container");
            //initializeFieldSortable(container);
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
            //const container = btn.closest(".field-wrapper")
            //                    .closest(".fields-container");
            //initializeFieldSortable(container);
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

function wireInsert(
    button,
    url,
    idName,
    dataAttribute,
    nameKey,
    promptText,
    position,
    extraPayload,
    successCallback) {

    const designer = $("#designerContainer");

    designer
        .off("click", button)
        .on("click", button, function (e) {

            console.count("wireInsert fired");

            e.preventDefault();
            e.stopPropagation();

            const btn = $(this);

            //console.log(button, position);

            const value = prompt(promptText);

            if (!value)
                return;

            const payload = {
                templateId: $("#TemplateId").val(),
                position: position
            };

            payload[idName] = btn.data(dataAttribute);
            payload[nameKey] = value;

            if (extraPayload)
                Object.assign(payload, extraPayload(btn));

            $.post(url, payload)
                .done(function (html) {

                    if (successCallback)
                        successCallback(btn, html);
                    else
                        reloadTemplate();

                })
                .fail(function () {
                    alert("Unable to insert.");
                });

        });

}


function wireAccordion(
    collapseSelector,
    cardSelector,
    summarySelector,
    chevronSelector) {

    const designer = $("#designerContainer");

    designer
        .off("show.bs.collapse", collapseSelector)
        .on("show.bs.collapse", collapseSelector, function () {

            const card = $(this).closest(cardSelector);

            card.find(summarySelector).hide();

            card.find(chevronSelector)
                .removeClass("bi-chevron-right")
                .addClass("bi-chevron-down");

        })

        .off("hide.bs.collapse", collapseSelector)
        .on("hide.bs.collapse", collapseSelector, function () {

            const card = $(this).closest(cardSelector);

            card.find(summarySelector).show();

            card.find(chevronSelector)
                .removeClass("bi-chevron-down")
                .addClass("bi-chevron-right");

        });

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

