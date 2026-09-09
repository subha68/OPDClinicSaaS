function setSelectedFileName(input, targetId) {
    if (input.files && input.files.length > 0) {
        document.getElementById(targetId).value =
            input.files[0].name;
    }
}

$(document).on(
    "click",
    ".crud-action",
    function () {

        const spinnerText =
            $(this).data("spinner-text");

        if (!spinnerText) {
            return;
        }

        if (typeof showLoading === "function") {

            showLoading(spinnerText);

        } else {

            $("#loadingOverlay")
                .removeClass("d-none");

            $("#loadingMessage")
                .text(spinnerText);
        }
    });