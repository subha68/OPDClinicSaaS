$("#DepartmentId").change(function () {

    let departmentId = $(this).val();

    $("#DoctorId").empty();

    $("#DoctorId").append(
        '<option value="">-- Select Doctor --</option>'
    );

    if (!departmentId)
    {
        $("#designerContainer").empty();
        return;
    }

    loadDoctors(departmentId);

    //updateShowButtonState();
    //loadDepartmentTemplate(departmentId);
});

function loadDoctors(departmentId)
{
    $.get(
        '/Doctor/GetDoctorsByDepartment',
        {
            departmentId: departmentId
        },
        function (data)
        {
            $.each(data, function (i, doctor)
            {
                $("#DoctorId").append(
                    `<option value="${doctor.doctorId}">
                        ${doctor.doctorName}
                     </option>`
                );
            });
        });
}

$("#DoctorId").change(function () {

    let doctorId = $(this).val();

    if (!doctorId)
        return;

    //loadDoctorTemplate(doctorId);
    //updateShowButtonState();
});

function updateShowButtonState() {
    $("#btnShowTemplate").prop("disabled", false);
}

// run on change
$("#DepartmentId, #DoctorId").on("change", updateShowButtonState);

// run once on page load
$(document).ready(updateShowButtonState);

$(document).on("change", "#showInactive", function () {
    applyInactiveFilter();
});

let designerCoreInitialized = false;

function initializeDesignerCore() {
    if (designerCoreInitialized) 
        return;

    designerCoreInitialized = true;

    initializeToggles();
    initializeInlineEditors();
    initializeInlineSelects();
    initializeSectionCheckboxes();
    initializeAccordionFirstTime();

}

function initializeInlineEditors() {
    // Inline Editing
    wireInlineEdit(
        ".section-name",
        "id",
        (id, name) => saveName(
            "/PrescriptionTemplate/SaveSectionName",
            {
                templateId: $("#TemplateId").val(),
                sectionId: id,
                sectionName: name
            })
    );

    wireInlineEdit(
        ".field-name",
        "id",
        (id, name) => saveName(
            "/PrescriptionTemplate/SaveFieldName",
            {
                templateId: $("#TemplateId").val(),
                fieldId: id,
                fieldName: name
            })
    );

    wireInlineEdit(
        ".option-name",
        "id",
        (id, name) => saveName(
            "/PrescriptionTemplate/SaveOptionName",
            {
                templateId: $("#TemplateId").val(),
                optionId: id,
                optionName: name
            })
    );
}

function initializeToggles() {

    //console.log("initializeToggles called");

    wireToggle({
        button: ".toggle-section",
        url: "/PrescriptionTemplate/ToggleSection",
        idName: "sectionId",
        dataAttribute: "section-id",
        cardSelector: ".section-card",
        activeIcon: "bi-eye-slash",
        inactiveIcon: "bi-eye"
    });

    wireToggle({
        button: ".toggle-field",
        url: "/PrescriptionTemplate/ToggleField",
        idName: "fieldId",
        dataAttribute: "field-id",
        cardSelector: ".field-card",
        activeIcon: "bi-eye-slash",
        inactiveIcon: "bi-eye"
    });

    wireToggle({
        button: ".toggle-option",
        url: "/PrescriptionTemplate/ToggleOption",
        idName: "optionId",
        dataAttribute: "option-id",
        cardSelector: ".option-card",
        activeIcon: "bi-eye-slash",
        inactiveIcon: "bi-eye"
    });

}

function initializeInlineSelects()
{
    wireInlineSelect({

        selector: ".field-type",
        templateSelector: "#fieldTypeTemplate",
        dataAttribute: "field-id",
        saveFunction: function(fieldId, fieldType, force){
            return $.post(
                "/PrescriptionTemplate/SaveFieldType",
                {
                    templateId: $("#TemplateId").val(),
                    fieldId: fieldId,
                    fieldTypeCode: fieldType,
                    force: force
                });
        }
    });

    wireInlineSelect({

        selector: ".field-source",
        templateSelector: "#lookupSourceTemplate",
        dataAttribute: "field-id",
        saveFunction: function (fieldId, fieldSource) {
            return $.post(
                "/PrescriptionTemplate/SaveFieldSource",
                {
                    templateId: $("#TemplateId").val(),
                    fieldId: fieldId,
                    fieldSource: fieldSource
                });

        }

    });    
}

function initializeSectionCheckboxes() {

    const designer = $("#designerContainer");

    designer
        .off("change", ".section-multiline")
        .on("change", ".section-multiline", function () {

            const chk = $(this);
            const sectioncontent = chk.closest(".section-content");

            saveSectionOptions(sectioncontent)
                .fail(function () {

                    alert("Unable to save.");

                    chk.prop(
                        "checked",
                        !chk.is(":checked")
                    );

                });

        });

    designer
        .off("change", ".section-rx")
        .on("change", ".section-rx", function () {

            const chk = $(this);
            const sectioncontent = chk.closest(".section-content");
            const multiline = sectioncontent.find(".section-multiline");

            const isRx = chk.is(":checked");

            // Rx implies Multiline
            multiline
                .prop("checked", isRx)
                .prop("disabled", isRx);

            // Rx excludes Vital/Test Result
            if (isRx) {

                sectioncontent.find(".section-vital")
                    .prop("checked", false);

                sectioncontent.find(".section-testresult")
                    .prop("checked", false);
            }

            saveSectionOptions(sectioncontent)
                .done(function () {

                    reloadTemplate();

                })
                .fail(function () {

                    alert("Unable to save.");

                    chk.prop(
                        "checked",
                        !isRx
                    );

                    multiline
                        .prop("checked", !isRx)
                        .prop("disabled", !isRx);

                });

        });

    designer
        .off("change", ".section-vital")
        .on("change", ".section-vital", function () {

            const chk = $(this);
            const sectioncontent = chk.closest(".section-content");

            const isVital = chk.is(":checked");

            if (isVital) {

                sectioncontent.find(".section-multiline")
                    .prop("checked", false)
                    .prop("disabled", false);

                sectioncontent.find(".section-rx")
                    .prop("checked", false);

                sectioncontent.find(".section-testresult")
                    .prop("checked", false);
            }

            saveSectionOptions(sectioncontent)
                .done(function () {

                    reloadTemplate();

                })
                .fail(function () {

                    alert("Unable to save.");

                    chk.prop(
                        "checked",
                        !isVital
                    );

                });

        });

    designer
        .off("change", ".section-testresult")
        .on("change", ".section-testresult", function () {

console.log("Test Results changed");

            const chk = $(this);
            const sectioncontent = chk.closest(".section-content");

            const isTestResult = chk.is(":checked");

            if (isTestResult) {

                sectioncontent.find(".section-multiline")
                    .prop("checked", false)
                    .prop("disabled", false);

                sectioncontent.find(".section-rx")
                    .prop("checked", false);

                sectioncontent.find(".section-vital")
                    .prop("checked", false);
            }

            saveSectionOptions(sectioncontent)
                .done(function () {

                    reloadTemplate();

                })
                .fail(function () {

                    alert("Unable to save.");

                    chk.prop(
                        "checked",
                        !isTestResult
                    );

                });

        });

}

function saveSectionOptions(sectioncontent) {

    const sectionId = sectioncontent.data("section-id");

    return $.post(
        "/PrescriptionTemplate/SaveSectionOptions",
        {
            templateId: $("#TemplateId").val(),
            sectionId: sectioncontent
                .find(".section-multiline,.section-rx,.section-vital,.section-testresult")
                .first()
                .data("section-id"),

            isMultiline: sectioncontent.find(".section-multiline").is(":checked"),
            isRx: sectioncontent.find(".section-rx").is(":checked"),
            isVital: sectioncontent.find(".section-vital").is(":checked"),
            isTestResult: sectioncontent.find(".section-testresult").is(":checked")
        });
}

function wireToggle(config) {

    const designer = $("#designerContainer");

    designer
        .off("click", config.button)
        .on("click", config.button, function (e) {

        //console.log("Toggle clicked");
    
        e.preventDefault();
        e.stopPropagation();

        var btn = $(this);

        var data = {
            templateId: $("#TemplateId").val()
        };

        //console.log("data attribute value:", btn.data(config.dataAttribute));
        //console.log("full data:", data);

        //data[config.idName] = btn.data(config.dataAttribute);
        data[config.idName] = btn.attr("data-" + config.dataAttribute);

        $.post(config.url, data, function (response) {

            //console.log(JSON.stringify(response));

            if (!response.success) {
                alert(response.message ?? "Toggle operation failed.");
                return;
            }

            var card = btn.closest(config.cardSelector);

            card.toggleClass("inactive", !response.isActive);

            var icon = btn.find(".toggle-icon");
            var text = btn.find(".toggle-text");

            if (response.isActive) {

                icon.removeClass(config.inactiveIcon)
                    .addClass(config.activeIcon);

                text.text("Deactivate");

            }
            else {

                icon.removeClass(config.activeIcon)
                    .addClass(config.inactiveIcon);

                text.text("Reactivate");

            }

            applyInactiveFilter();

        });

    });

}

function wireInlineEdit(selector, dataAttribute, saveFunction) {

    //console.log("Binding", selector);

    const designer = $("#designerContainer");

    designer
        .off("dblclick", selector)
        .on("dblclick", selector, function (e) {

            e.stopPropagation();

            const element = $(this);

            // Already editing
            if (element.find("input").length)
                return;

            const oldValue = $.trim(element.text());

            const input = $("<input>", {
                type: "text",
                class: "form-control form-control-sm",
                value: oldValue
            });

            element.empty().append(input);

            input.focus().select();

            let cancelled = false;

            function cancel() {

                cancelled = true;
                element.text(oldValue);

            }

            function save() {

                if (cancelled)
                    return;

                const newValue = $.trim(input.val());

                if (!newValue) {
                    element.text(oldValue);
                    return;
                }

                if (newValue === oldValue) {
                    element.text(oldValue);
                    return;
                }

            /*    console.log({
                    id: element.data(dataAttribute),
                    oldValue,
                    newValue
                }); */

                saveFunction(
                    element.data(dataAttribute),
                    newValue
                )
                .done(function () {

                    element.text(newValue);

                })
                .fail(function () {

                    alert("Unable to save.");
                    element.text(oldValue);
                });

            }

            input.on("keydown", function (e) {

                if (e.key === "Enter") {

                    e.preventDefault();
                    input.blur();
                }

                else if (e.key === "Escape") {

                    e.preventDefault();
                    cancel();
                }

            });

            input.on("blur", save);
        });

}

function wireInlineSelect(config) {

    $(document)
        .off("dblclick", config.selector)
        .on("dblclick", config.selector, function (e) {

            e.stopPropagation();

            const element = $(this);

            // Already editing
            if (element.find("select").length)
                return;

            const oldValue = element.data("value");

            const select = $(config.templateSelector)
                .clone()
                .removeAttr("id")
                .removeClass("d-none");

            select.val(oldValue);
            element.empty().append(select);
            select.focus();
            let cancelled = false;
            let saved = false;

            function getDisplayText() {
                if (config.displayText)
                    return config.displayText(select);

                return select.find("option:selected").text();
            }

            function cancel() {
                cancelled = true;
                element.text(oldValue);
                element.attr("data-value", oldValue);
            }

            function save() {

                if (cancelled || saved)
                    return;

                saved = true;

                const newValue = select.val();

                if (newValue === oldValue) {

                    element.text(getDisplayText());
                    element.attr("data-value", oldValue);

                    return;

                }

                config.saveFunction(
                    element.data(config.dataAttribute),
                    newValue
                )

                .done(function (response) {

                    if (response.requiresConfirmation) {

                        if (!confirm(response.message)) {

                            element.text(response.oldText);
                            element.attr("data-value", oldValue);

                            return;
                        }

                        config.saveFunction(
                            element.data(config.dataAttribute),
                            newValue,
                            true
                        )

                        .done(function (response) {

                            element.text(response.text ?? getDisplayText());
                            element.attr("data-value", response.value ?? newValue);

                            if (config.afterSave)
                                config.afterSave(response);
                        });

                        return;
                    }

                    element.text(response.text ?? getDisplayText());
                    element.attr("data-value", response.value ?? newValue);

                    if (config.afterSave)
                        config.afterSave(response);
                })

                .fail(function () {

                    alert("Unable to save.");

                    element.text(oldValue);
                    element.attr("data-value", oldValue);
                });
            }

            select.on("keydown", function (e) {

                if (e.key === "Escape") {

                    e.preventDefault();

                    cancelled = true;

                    element.text(select.find("option[value='" + oldValue + "']").text());

                    element.attr("data-value", oldValue);

                }

            });

            select.on("change", save);

            select.on("blur", function () {

                if (!saved && !cancelled) {
                    cancel();
                }

            });

        });

}

function initializeAccordionFirstTime() {

    wireInitialAccordion(
        ".section-collapse",
        ".section-card",
        ".section-preview",
        ".expand-section i"
    );

    wireInitialAccordion(
        ".field-collapse",
        ".field-card",
        ".field-preview",
        ".expand-field i"
    );

}

function wireInitialAccordion(
    collapseSelector,
    cardSelector,
    summarySelector,
    chevronSelector) {

    $(collapseSelector).each(function () {

        const card = $(this).closest(cardSelector);

        if ($(this).hasClass("show")) {

            card.find(summarySelector).hide();

            card.find(chevronSelector)
                .removeClass("bi-chevron-right")
                .addClass("bi-chevron-down");

        } else {

            card.find(summarySelector).show();

            card.find(chevronSelector)
                .removeClass("bi-chevron-down")
                .addClass("bi-chevron-right");
        }
    });
}


function saveName(url, payload) {

    payload.templateId = $("#TemplateId").val();

    return $.post(url, payload);

}

function saveOrder(url, payload) {

    return $.ajax({
        url: url,
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify(payload)
    })
    .done(function () {
        reloadTemplate();
    });
}

function insertItem(url, payload, promptText) {

    let name = prompt(promptText);

    if (!name)
        return;

    payload.name = name;

    $.post(url, payload, function () {

        reloadTemplate();

    });

}


