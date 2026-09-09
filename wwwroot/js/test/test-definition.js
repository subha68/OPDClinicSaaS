(function () {

"use strict";

// ============================================================
// CONFIGURATION
// ============================================================

const config = window.testDefinitionConfig || {};

// ============================================================
// DOCUMENT READY
// ============================================================

$(document).ready(function () {

    initializePanelEvents();
    initializeParameterEvents();
    initializePanelSorting();
});

// ============================================================
// PANEL EVENTS
// ============================================================

function initializePanelEvents() {

    // --------------------------------------------------------
    // Expand / Collapse
    // --------------------------------------------------------

    $(document).on(
        "click",
        ".btn-expand-panel",
        function (e) {

            e.preventDefault();

            const panel =
                $(this).closest(".test-panel");

            togglePanel(panel);

        }
    );

    $(document).on(
        "click",
        ".panel-display",
        function (e) {

        if ($(e.target).closest(
            ".btn-expand-panel, .btn-edit-panel, .form-delete-panel").length) {
            return;
        }

        const panel = $(this).closest(".test-panel");

        panel.find(".btn-expand-panel").trigger("click");

    });    

    // ============================================================
    // ADD PANEL
    // ============================================================

    $(document).on(
        "click",
        ".btn-add-panel",
        function (e) {

            e.preventDefault();

            // Do not create multiple new panels
            if ($("#new-panel").length) {

                $("#new-panel")
                    .find(".panel-code-input")
                    .trigger("focus");

                return;
            }

            openAddPanel();

        }
    );

    // ============================================================
    // CANCEL NEW PANEL
    // ============================================================

    $(document).on(
        "click",
        ".btn-cancel-new-panel",
        function (e) {

            e.preventDefault();

            $("#new-panel").remove();

        }
    );

    // --------------------------------------------------------
    // Edit Panel
    // --------------------------------------------------------

    $(document).on(
        "click",
        ".btn-edit-panel",
        function (e)
        {
            e.preventDefault();

            const panelId =
                $(this).data("panel-id");

            openEditPanel(panelId);
        });

    // --------------------------------------------------------
    // Cancel Panel Edit
    // --------------------------------------------------------

    $(document).on(
        "click",
        ".btn-cancel-panel-edit",
        function ()
        {
            $("#panelEditorHost")
                .empty();
        });

    // --------------------------------------------------------
    // Delete Panel
    // --------------------------------------------------------

    $(document).on(
        "submit",
        ".form-delete-panel",
        function (e) {

            const confirmed = confirm(
                "Are you sure you want to delete this test panel?\n\n" +
                "All parameters associated with this panel will also be removed from the panel."
            );

            if (!confirmed) {
                e.preventDefault();
            }

        }
    );

    $(document)
        .off("submit", "#testPanelForm")
        .on(
            "submit",
            "#testPanelForm",
            function (e)
            {
                e.preventDefault();
                submitPanelForm($(this));
            });
}

function reloadPanels()
{
    location.reload();
}

function submitPanelForm(form)
{
    $.ajax({

        url: form.attr("action"),

        type: "POST",

        data: form.serialize(),

        headers: {
            "RequestVerificationToken":
                getAntiForgeryToken()
        },

        success: function(response)
        {
            if (response.success)
            {
                location.reload();
            }
        }
    });
}

// ============================================================
// CREATE NEW PANEL
// ============================================================

function openEditPanel(panelId)
{
    $.get(
        config.editPanelUrl,
        {
            panelId: panelId
        },
        function (html)
        {
            $("#panelEditorHost")
                .html(html);
        });
}


// ============================================================
// PARAMETER EVENTS
// ============================================================

function initializeParameterEvents() {

    // --------------------------------------------------------
    // Add Parameter
    // --------------------------------------------------------

    $(document).on(
        "click",
        ".btn-add-parameter",
        function (e) {

            e.preventDefault();

            const button = $(this);

            const panelId = button
                            .closest(".parameter-list")
                            .data("panel-id");

            if (!panelId) {

                alert("Invalid panel.");

                return;
            }

            openAddParameter(
                panelId,
                button
            );

        }
    );

    // --------------------------------------------------------
    // Edit Parameter
    // --------------------------------------------------------

    $(document).on(
        "click",
        ".btn-edit-parameter",
        function (e) {

            e.preventDefault();

            const button =
                $(this);

            const parameterId =
                button.data("parameter-id");

            const panelId =
                button.data("panel-id");

            if (!parameterId || !panelId) {

                alert(
                    "Invalid parameter or panel."
                );

                return;
            }

            openEditParameter(
                parameterId,
                panelId,
                button
            );

        }
    );

    // --------------------------------------------------------
    // Cancel Parameter Edit
    // --------------------------------------------------------

    $(document).on(
        "click",
        ".btn-cancel-parameter-edit",
        function (e)
        {
            e.preventDefault();

            const editor = $(this).closest(".parameter-editor");

            const isNew = editor.data("is-new");

            const panel = editor.closest(".test-panel");
                
            if (isNew)
            {
                editor.remove();
            }
            else
            {
                editor.replaceWith(
                    editor.data("original-html")
                );
            }

            setParameterEditMode(panel, false);
        });

    // --------------------------------------------------------
    // Add/Edit Parameter Form Submit
    // --------------------------------------------------------

    $(document).on(
        "submit",
        "#testParameterForm",
        function (e) {

            e.preventDefault();

            submitParameterForm(
                $(this)
            );

        }
    );

    // --------------------------------------------------------
    // Dissociate Parameter
    // --------------------------------------------------------

    $(document).on(
        "click",
        ".btn-dissociate-parameter",
        function (e) {

            e.preventDefault();

            dissociateParameter(
                $(this)
            );

        }
    );

    $(document).on(
        "click",
        ".btn-associate-parameter",
        function (e)
        {
            e.preventDefault();

            const button = $(this);

            const panelId = button
                            .closest(".parameter-list")
                            .data("panel-id");

            openAssociateParameter(panelId, button);
        }); 
        
    $(document).on(
        "click",
        ".btn-save-associated-parameter",
        function (e)
        {
            e.preventDefault();

            const button = $(this);

            const panel = button.closest(".parameter-list");

            saveAssociatedParameter(panel);
        });

        $(document).on(
            "click",
            ".btn-cancel-associate-parameter",
            function ()
            {
                const button = $(this);

                const panel =
                    button.closest(".parameter-list");

                $(this)
                    .closest(".associate-parameter-container")
                    .remove();

                setParameterEditMode(panel, false);
            });

}


// ============================================================
// EXPAND / COLLAPSE
// ============================================================

function togglePanel(panel) {

    const parameters =
        panel.find(".panel-parameters");

    const button =
        panel.find(".btn-expand-panel");

    const icon =
        button.find(".panel-expand-icon");

    const expanded =
        button.attr("aria-expanded") === "true";

    if (expanded) {

        panel.removeClass("expanded");

        parameters
            .stop(true, true)
            .slideUp(150, function () {

                $(this).addClass("d-none");

            });

        button.attr(
            "aria-expanded",
            "false"
        );

        return;
    }

    panel.addClass("expanded");

    parameters
        .removeClass("d-none")
        .stop(true, true)
        .slideDown(150);

    button.attr(
        "aria-expanded",
        "true"
    );

    if (
        parameters.attr("data-loaded") === "true"
    ) {
        return;
    }

    loadParameters(panel);

}

function openAddPanel()
{
    $.get(
        config.addPanelUrl,
        function (html)
        {
            $("#panelEditorHost")
                .html(html);
        });
}

$(document).on(
    "click",
    ".btn-cancel-panel-edit",
    function ()
    {
        $("#panelEditorHost").empty();
    });

// ============================================================
// LOAD PARAMETERS
// ============================================================

function loadParameters(panel) {

    const panelId =
        panel.data("panel-id");

    const content =
        panel.find(".parameter-content");

    if (!panelId) {
        return;
    }

    content.html(
        '<div class="text-center text-muted py-3">' +
            '<span class="spinner-border spinner-border-sm me-1"></span>' +
            'Loading parameters...' +
        '</div>'
    );

    $.ajax({

        url: config.listParameterUrl,

        type: "GET",

        data: {
            panelId: panelId
        },

        success: function (html) {

            content.html(html);

            panel
                .find(".panel-parameters")
                .attr("data-loaded", "true");

            initializeParameterSorting(panelId);

        },

        error: function (xhr) {

            console.error(
                "Error loading parameters:",
                xhr
            );

            content.html(
                '<div class="alert alert-danger mb-0">' +
                    'Unable to load parameters.' +
                '</div>'
            );

        }

    });

}

// ============================================================
// OPEN ADD PARAMETER
// ============================================================

function openAddParameter(
    panelId,
    button
) {

    const panel =
        button.closest(".test-panel");

    const content =
        panel.find(".parameter-content");

    if (!content.length) {
        return;
    }

    // Prevent multiple editors

    if (
        content.find(".parameter-editor").length
    ) {
        return;
    }

    setParameterEditMode(panel, true);

    const originalButtonHtml =
        button.html();

    button
        .prop("disabled", true)
        .html(
            '<span class="spinner-border spinner-border-sm me-1"></span>' +
            'Loading...'
        );

    const loadingEditor =
        $(
            '<div class="parameter-editor-host">' +
                '<div class="text-center text-muted py-3">' +
                    '<span class="spinner-border spinner-border-sm me-1"></span>' +
                    'Loading parameter form...' +
                '</div>' +
            '</div>'
        );

    content.prepend(
        loadingEditor
    );

    $.ajax({

        url: config.addParameterUrl,

        type: "GET",

        data: {
            panelId: panelId
        },

        success: function (html) {

            loadingEditor
                .html(html);

            loadingEditor.data(
                "is-new",
                true
            );

            button
                .prop("disabled", false)
                .html(originalButtonHtml);

            

        },

        error: function (xhr) {

            console.error(
                "Error loading add parameter form:",
                xhr
            );

            loadingEditor.remove();

            button
                .prop("disabled", false)
                .html(originalButtonHtml);

            alert(
                "Unable to load the parameter form."
            );

        }

    });

}

// ============================================================
// OPEN EDIT PARAMETER
// ============================================================

function openEditParameter(
    parameterId,
    panelId,
    button
) {

    const parameter =
        button.closest(".test-parameter");

    if (!parameter.length) {
        return;
    }

    // Prevent duplicate editing

    if (
        parameter.hasClass("parameter-editing")
    ) {
        return;
    }

    const originalHtml =
        parameter.prop("outerHTML");

    const editor =
        $(
            '<div class="parameter-editor parameter-editing border rounded bg-white p-3 mb-2">' +
                '<div class="text-center text-muted py-3">' +
                    '<span class="spinner-border spinner-border-sm me-1"></span>' +
                    'Loading parameter...' +
                '</div>' +
            '</div>'
        );

    editor.data(
        "original-html",
        originalHtml
    );

    parameter.replaceWith(
        editor
    );

    $.ajax({

        url: config.editParameterUrl,

        type: "GET",

        data: {

            id: parameterId,

            panelId: panelId

        },

        success: function (html) {

            editor.html(html);

        },

        error: function (xhr) {

            console.error(
                "Error loading parameter:",
                xhr
            );

            editor.html(
                '<div class="alert alert-danger mb-2">' +
                    'Unable to load the parameter.' +
                '</div>' +
                '<button type="button" ' +
                    'class="btn btn-sm btn-secondary btn-cancel-parameter-edit">' +
                    'Cancel' +
                '</button>'
            );

        }

    });

}

// ============================================================
// SUBMIT PARAMETER FORM
// ============================================================

function submitParameterForm(form) {

    const editor =
        form.closest(".parameter-editor");

    const submitButton =
        form.find(
            'button[type="submit"]'
        );

    const originalButtonHtml =
        submitButton.html();

    const panelId =
        form
            .find(
                '[name="Association.TestPanelId"]'
            )
            .val();

    if (!panelId) {

        showParameterError(
            form,
            "Invalid panel."
        );

        return;
    }

    submitButton
        .prop("disabled", true)
        .html(
            '<span class="spinner-border spinner-border-sm me-1"></span>' +
            'Saving...'
        );

    $.ajax({

        url: form.attr("action"),

        type: "POST",

        data: form.serialize(),

        success: function (response) {

            // ------------------------------------------------
            // Validation failed
            // Server returned the partial view
            // ------------------------------------------------

            if (
                typeof response === "string"
            ) {

                editor
                    .html(response);

                return;
            }

            // ------------------------------------------------
            // Successful Add/Edit
            // ------------------------------------------------

            if (
                response &&
                response.success
            ) {

                const panel =
                    $('.test-panel[data-panel-id="' +
                        response.panelId +
                    '"]');

                if (panel.length) {

                    refreshParameters(
                        panel
                    );

                    showPanelSuccess(
                        panel,
                        response.message ||
                        "Test parameter saved successfully."
                    );

                }

                setParameterEditMode(panel, false);

                return;
            }

            // ------------------------------------------------
            // Unexpected response
            // ------------------------------------------------

            showParameterError(
                form,
                response?.message ||
                "Unable to save the parameter."
            );

            restoreParameterButton(
                submitButton,
                originalButtonHtml
            );

        },

        error: function (xhr) {

            console.error(
                "Error saving parameter:",
                xhr
            );

            let message =
                "Unable to save the parameter.";

            if (
                xhr.responseJSON &&
                xhr.responseJSON.message
            ) {

                message =
                    xhr.responseJSON.message;
            }

            showParameterError(
                form,
                message
            );

            restoreParameterButton(
                submitButton,
                originalButtonHtml
            );

        }

    });

}

// ============================================================
// REFRESH PARAMETERS
// ============================================================

function refreshParameters(panel) {

    const parameters =
        panel.find(".panel-parameters");

    parameters.attr(
        "data-loaded",
        "false"
    );

    loadParameters(
        panel
    );

}

function saveAssociatedParameter(panel)
{
    const association = {

        TestPanelId:
            $("#Associate_TestPanelId").val(),

        TestParameterId:
            $("#Associate_TestParameterId").val(),

        DisplayOrder:
            $("#Associate_DisplayOrder").val(),

        IsActive: true
    };

    if (!association.TestParameterId)
    {
        alert(
            "Please select a parameter."
        );

        return;
    }

    $.ajax({

        url: config.associateParameterUrl,

        type: "POST",

        data: association,

        headers: {

            "RequestVerificationToken":
                getAntiForgeryToken()
        },

        success: function (response)
        {
            if (response && response.success)
            {
                reloadPanelParameters(
                    association.TestPanelId
                );

                $(".associate-parameter-container")
                    .remove();

                setParameterEditMode(panel, false);

                return;
            }

            alert(
                response?.message ||
                "Unable to associate parameter."
            );
        },

        error: function (xhr)
        {
            console.error(
                "Associate parameter failed:",
                xhr
            );

            alert(
                xhr.responseJSON?.message ||
                "An error occurred while associating parameter."
            );
        }
    });
}

function openAssociateParameter(panelId, button)
{
    const panel = button.closest(".parameter-list");

    setParameterEditMode(panel, true);
            
    $.get(
        config.associateParameterFormUrl,
        {
            panelId: panelId
        },
        function (html)
        {
            const panelContainer =
                $(`.parameter-list[data-panel-id='${panelId}']`);

            panelContainer
                .find(".associate-parameter-container")
                .remove();

            panelContainer
                .find(".associate-parameter-host")
                .html(html);

            initializeAssociateParameterAutocomplete();

        });
}

function initializeAssociateParameterAutocomplete()
{
    wireAutocomplete(

        "#Associate_ParameterName",

        config.searchUnassociatedParameterUrl,

        function (item) {

            return {

                label:
                    item.code +
                    " - " +
                    item.name,

                value:
                    item.name,

                id:
                    item.id
            };
        },

        function (item) {

            $("#Associate_TestParameterId")
                .val(item.id);
        },

        ".associate-parameter-container",

        function () {

            $("#Associate_TestParameterId")
                .val("");
        },

        function () {

            return {

                panelId:
                    $("#Associate_TestPanelId")
                        .val()
            };
        }
    );
}



// ============================================================
// DISSOCIATE PARAMETER
// ============================================================

function dissociateParameter(button)
{
    const panelId =
        button.data("panel-id");

    const parameterId =
        button.data("parameter-id");

    if (!panelId || !parameterId)
    {
        alert("Invalid panel or parameter.");
        return;
    }

    if (!confirm("Dissociate this parameter from the panel?"))
        return;

    const originalButtonHtml =
        button.html();

    button
        .prop("disabled", true)
        .html(
            '<span class="spinner-border spinner-border-sm me-1"></span>'
        );

    $.ajax(
    {
        url: config.dissociateParameterUrl,

        type: "POST",

        data:
        {
            panelId: panelId,
            parameterId: parameterId
        },

        headers:
        {
            "RequestVerificationToken":
                getAntiForgeryToken()
        },

        success: function (response)
        {
            if (response && response.success)
            {
                reloadPanelParameters(panelId);
                initializeParameterSorting(panelId);
                return;
            }

            button
                .prop("disabled", false)
                .html(originalButtonHtml);

            alert(
                response?.message ||
                "Unable to dissociate parameter."
            );
        },

        error: function ()
        {
            button
                .prop("disabled", false)
                .html(originalButtonHtml);

            alert(
                "An error occurred while dissociating parameter."
            );
        }
    });
}

// ============================================================
// CANCEL PANEL EDIT
// ============================================================

function cancelEditMode(panel) {

    const original =
        panel.data("original-values");

    if (original) {

        panel
            .find(".panel-code-input")
            .val(original.code);

        panel
            .find(".panel-name-input")
            .val(original.name);

        panel
            .find(".panel-department-input")
            .val(original.departmentName);

        panel
            .find(".panel-description-input")
            .val(original.description);

        panel
            .find(".panel-display-order-input")
            .val(original.displayOrder);

        panel
            .find(".panel-active-input")
            .prop(
                "checked",
                original.isActive
            );

    }

    removePanelMessage(panel);

    exitEditMode(panel);

}

// ============================================================
// PANEL SUCCESS
// ============================================================

function showPanelSuccess(
    panel,
    message
) {

    if (!panel || !panel.length) {
        return;
    }

    removePanelMessage(panel);

    const html =
        '<div class="alert alert-success alert-dismissible fade show panel-message mt-2 mb-0">' +
            escapeHtml(message) +
            '<button type="button" class="btn-close" data-bs-dismiss="alert"></button>' +
        '</div>';

    panel
        .find(".card-body")
        .append(html);

    setTimeout(function () {

        removePanelMessage(panel);

    }, 3000);

}

function initializePanelSorting() {

    const container =
        document.getElementById(
            "testPanelContainer"
        );

    if (!container || typeof Sortable === "undefined") {
        return;
    }

    Sortable.create(container, {

        animation: 150,

        handle: ".drag-handle",

        draggable: ".test-panel",

        onEnd: function (evt) {

            const panel =
                $(evt.item);

            const panelId =
                panel.data("panel-id");

            const newDisplayOrder =
                $("#testPanelContainer .test-panel")
                    .index(evt.item) + 1;

            $.ajax({

                url: config.reorderPanelUrl,

                type: "POST",

                data: {

                    panelId: panelId,

                    displayOrder:
                        newDisplayOrder

                },

                headers: {

                    "RequestVerificationToken":
                        getAntiForgeryToken()

                }

            });

            renumberPanelBadges();
        }
    });
}

function initializeParameterSorting(panelId)
{
    const container =
        document.querySelector(
            `.parameter-list[data-panel-id='${panelId}']`
        );

    if (!container || typeof Sortable === "undefined")
        return;

    if (container.dataset.sortableInitialized === "true")
        return;

    container.dataset.sortableInitialized = "true";

    Sortable.create(container,
    {
        animation: 150,

        handle: ".drag-handle",

        draggable: ".test-parameter",

        onEnd: function (evt)
        {
            const parameter =
                $(evt.item);

            const parameterId =
                parameter.data("parameter-id");

            const newDisplayOrder =
                $(container)
                    .children(".test-parameter")
                    .index(evt.item) + 1;

            $.ajax(
            {
                url: config.reorderParameterUrl,

                type: "POST",

                data:
                {
                    panelId: panelId,
                    parameterId: parameterId,
                    newDisplayOrder: newDisplayOrder
                },

                headers:
                {
                    "RequestVerificationToken":
                        getAntiForgeryToken()
                }
            });

            renumberParameterBadges(container);
        }
    });
}

function renumberPanelBadges() {

    $("#testPanelContainer")
        .find(".test-panel")
        .each(function(index) {

            $(this)
                .find(".badge")
                .first()
                .text("#" + (index + 1));

        });
}

function renumberParameterBadges(container)
{
    $(container)
        .children(".test-parameter")
        .each(function (index)
        {
            $(this)
                .find(".badge")
                .first()
                .text("#" + (index + 1));
        });
}

// ============================================================
// PARAMETER ERROR
// ============================================================

function showParameterError(
    form,
    message
) {

    form
        .find(".parameter-ajax-error")
        .remove();

    const html =
        '<div class="alert alert-danger parameter-ajax-error mt-2">' +
            escapeHtml(message) +
        '</div>';

    form.prepend(html);

}

// ============================================================
// PANEL ERROR
// ============================================================

function showPanelError(
    panel,
    message
) {

    removePanelMessage(panel);

    const html =
        '<div class="alert alert-danger alert-dismissible fade show panel-message mt-2 mb-0">' +
            escapeHtml(message) +
            '<button type="button" class="btn-close" data-bs-dismiss="alert"></button>' +
        '</div>';

    panel
        .find(".panel-edit")
        .append(html);

}

// ============================================================
// REMOVE PANEL MESSAGE
// ============================================================

function removePanelMessage(panel) {

    panel
        .find(".panel-message")
        .remove();

}

// ============================================================
// RESTORE PARAMETER BUTTON
// ============================================================

function restoreParameterButton(
    button,
    html
) {

    button
        .prop("disabled", false)
        .html(html);

}

// ============================================================
// ANTI-FORGERY TOKEN
// ============================================================

function getAntiForgeryToken() {

    return $(
        'input[name="__RequestVerificationToken"]'
    )
        .first()
        .val();

}

function reloadPanelParameters(panelId)
{
    const panel =
        $(`.test-panel[data-panel-id='${panelId}']`);

    if (panel.length === 0)
        return;

    panel
        .find(".panel-parameters")
        .attr("data-loaded", "false");

    loadParameters(panel);
}

// ============================================================
// ESCAPE HTML
// ============================================================

function escapeHtml(value) {

    if (
        value === null ||
        value === undefined
    ) {
        return "";
    }

    return String(value)
        .replace(
            /&/g,
            "&amp;"
        )
        .replace(
            /</g,
            "&lt;"
        )
        .replace(
            />/g,
            "&gt;"
        )
        .replace(
            /"/g,
            "&quot;"
        )
        .replace(
            /'/g,
            "&#039;"
        );

}

function setParameterEditMode(panel, isEditing)
{

    panel.find(
        ".btn-associate-parameter, .btn-add-parameter"
    ).toggle(!isEditing);
}

})();