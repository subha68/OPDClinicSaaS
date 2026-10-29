$(document).ready(function () {

    $("#DepartmentId").on("change", updateCloneTemplateState);

    // Check the initially selected department.
    updateCloneTemplateState();

});

function updateCloneTemplateState() {

    const departmentId = $("#DepartmentId").val();
    const checkbox = $("#CloneTemplate");

    // No department selected: disable and uncheck.
    if (!departmentId) {
        checkbox.prop("checked", false);
        checkbox.prop("disabled", true);
        return;
    }

    // Avoid changes while checking the selected department.
    checkbox.prop("disabled", true);

    const checkUrl = checkbox.data("template-check-url");

    $.ajax({
        url: checkUrl,
        type: "GET",
        data: { departmentId: departmentId },
        dataType: "json",

        success: function (response) {

            const hasDoctorTemplate =
                checkbox.data("has-doctor-template") === true ||
                checkbox.data("has-doctor-template") === "true";

            const hasDepartmentTemplate =
                response.hasTemplate === true ||
                response.hasTemplate === "true";

            // An existing doctor template is already present.
            // Do not let the department availability check
            // override the user's checkbox selection.
            if (hasDoctorTemplate) {
                checkbox.prop("disabled", false);
                return;
            }

            // For a new doctor, cloning is possible only if
            // the selected department has a template.
            checkbox.prop("disabled", !hasDepartmentTemplate);

            if (!hasDepartmentTemplate) {
                checkbox.prop("checked", false);
            }
        },

        error: function () {
            // Fail safely if availability cannot be verified.
            checkbox.prop("checked", false);
            checkbox.prop("disabled", true);

            console.error("Unable to check department template availability.");
        }
    });
}