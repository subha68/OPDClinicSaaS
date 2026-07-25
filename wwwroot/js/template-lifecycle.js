function initializeSectionSortables() {
    wireSortable(
        "#sectionsContainer",
        ".section-wrapper",
        ".section-drag",
        "section-id",
        function (container, sectionIds) {
            const model = sectionIds.map((id, index) => ({
                templateId: $("#TemplateId").val(),
                sectionId: id,
                displayOrder: index + 1
            }));

            saveOrder(
                "/PrescriptionTemplate/SaveSectionOrder",
                model
            );
        });

}

function initializeFieldSortables() {

    $(".fields-container").each(function () {
        //console.log("Initializing field sortable:", this.id);

        wireSortable(
            "#" + this.id,
            ".field-wrapper",
            ".field-drag",
            "field-id",
            function (container, fieldIds) {

                //console.log("Field IDs:", fieldIds);

                const model = fieldIds.map((id, index) => ({
                    templateId: $("#TemplateId").val(),
                    sectionId: $(container).data("section-id"),
                    fieldId: id,
                    displayOrder: index + 1
                }));

                return saveOrder(
                    "/PrescriptionTemplate/SaveFieldOrder",
                    model
                );

            });

    });

}

function initializeOptionSortables() {

    $(".options-container").each(function () {
        console.log("Initializing option sortable:", this.id);

        wireSortable(
            "#" + this.id,
            ".option-wrapper",
            ".option-drag",
            "option-id",
            function (container, optionIds) {

                console.log("Option IDs:", optionIds);

                const model = optionIds.map((id, index) => ({
                    templateId: $("#TemplateId").val(),
                    //sectionId: $(container).data("section-id"),
                    fieldId: $(container).data("field-id"),
                    optionId: id,
                    displayOrder: index + 1
                }));

                return saveOrder(
                    "/PrescriptionTemplate/SaveOptionOrder",
                    model
                );

            });

    });

}

function wireSortable(
    containerSelector,
    itemSelector,
    handleSelector,
    dataAttribute,
    saveFunction) 
{

    document.querySelectorAll(containerSelector).forEach(container => {

        // -----------------------------------------
        // 1. DESTROY EXISTING SORTABLE (CRITICAL)
        // -----------------------------------------
        if (container._sortableInstance) {
            container._sortableInstance.destroy();
            container._sortableInstance = null;
        }

        // -----------------------------------------
        // 2. CREATE SORTABLE
        // -----------------------------------------
        const sortable = new Sortable(container, {

            animation: 150,
            handle: handleSelector,
            draggable: itemSelector,

            onEnd: function () {

                const ids = [];

                $(container)
                    .children(itemSelector)
                    .each(function () {
                        ids.push($(this).data(dataAttribute));
                    });

                // -----------------------------------------
                // 3. SAFE SAVE CALL (UNCHANGED CONTRACT)
                // -----------------------------------------
                if (typeof saveFunction === "function") {
                    saveFunction(container, ids);
                }
            }
        });

        // -----------------------------------------
        // 4. STORE INSTANCE FOR FUTURE CLEANUP
        // -----------------------------------------
        container._sortableInstance = sortable;
    });
}

function getSortableInstance(el) {
    return el._sortableInstance || null;
}

function setSortableInstance(el, instance) {
    el._sortableInstance = instance;
}

function destroyAllSortables(scope = document) {

    scope.querySelectorAll('[data-sortable-initialized="true"]').forEach(el => {
        if (el._sortableInstance) {
            el._sortableInstance.destroy();
            el._sortableInstance = null;
        }
    });
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

function loadDepartmentTemplate(departmentId)
{
    $.get(
        '/PrescriptionTemplate/GetByDepartment',
        {
            departmentId: departmentId
        },
        function (html)
        {
            destroyAllSortables(document.getElementById("designerContainer"));

            $("#designerContainer").html(html);

            initializeDesignerRuntime();

            applyInactiveFilter();

        }
    );
}

function loadDoctorTemplate(doctorId)
{
    $.get(
        '/PrescriptionTemplate/GetByDoctor',
        {
            doctorId: doctorId
        },
        function (html)
        {
            destroyAllSortables(document.getElementById("designerContainer"));

            $("#designerContainer").html(html);

            initializeDesignerRuntime();

            applyInactiveFilter();

        }
    );
}

function reloadTemplate()
{
    console.count("reloadTemplate");
    
    let doctorId = $("#DoctorId").val();
    let departmentId = $("#DepartmentId").val();

    if (doctorId)
    {
        loadDoctorTemplate(doctorId);
    }
    else if (departmentId)
    {
        loadDepartmentTemplate(departmentId);
    }
}

