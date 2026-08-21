document.addEventListener("click", function (event) {

    const button =
        event.target.closest(".password-toggle");

    if (!button) {
        return;
    }

    const targetId =
        button.getAttribute("data-target");

    const input =
        document.getElementById(targetId);

    if (!input) {
        return;
    }

    const icon =
        button.querySelector("i");

    if (input.type === "password") {

        input.type = "text";

        if (icon) {
            icon.classList.remove("fa-eye");
            icon.classList.add("fa-eye-slash");
        }

        button.title = "Hide password";
        button.setAttribute(
            "aria-label",
            "Hide password"
        );

    } else {

        input.type = "password";

        if (icon) {
            icon.classList.remove("fa-eye-slash");
            icon.classList.add("fa-eye");
        }

        button.title = "Show password";
        button.setAttribute(
            "aria-label",
            "Show password"
        );
    }
});