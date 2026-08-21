(function () {

    function initializeChangePasswordModal() {

        const modalElement =
            document.getElementById("changePasswordModal");

        const form =
            document.getElementById("changePasswordForm");

        if (!modalElement || !form) {
            return;
        }

        /*
         * Prevent this script from being initialized twice.
         */
        if (form.dataset.initialized === "true") {
            return;
        }

        form.dataset.initialized = "true";


        /*
         * ---------------------------------------------------------
         * Bootstrap modal
         * ---------------------------------------------------------
         */

        const isFirstLogin =
            modalElement.dataset.firstLogin === "true";

        const modal =
            bootstrap.Modal.getOrCreateInstance(
                modalElement,
                {
                    backdrop: isFirstLogin ? "static" : true,
                    keyboard: !isFirstLogin
                }
            );


        /*
         * ---------------------------------------------------------
         * Show modal automatically for first login
         * ---------------------------------------------------------
         */

        if (isFirstLogin) {
            modal.show();
        }

        /*
         * ---------------------------------------------------------
         * Show message at top of current page
         * ---------------------------------------------------------
         */

        function showAppMessage(message, type) {

            const appMessage =
                document.getElementById("appMessage");

            if (!appMessage) {
                return;
            }

            appMessage.className =
                "alert alert-" + type;

            appMessage.textContent =
                message || "";

            appMessage.classList.remove("d-none");

            /*
             * Scroll to top so user sees the message.
             */
            window.scrollTo({
                top: 0,
                behavior: "smooth"
            });

        }


        /*
         * ---------------------------------------------------------
         * Clear modal validation errors
         * ---------------------------------------------------------
         */

        function clearErrors() {

            modalElement
                .querySelectorAll(".text-danger")
                .forEach(function (element) {

                    element.textContent = "";

                });

            const errorAlert =
                document.getElementById(
                    "changePasswordError"
                );

            if (errorAlert) {
                errorAlert.textContent = "";
                errorAlert.classList.add("d-none");
            }

        }


        /*
         * ---------------------------------------------------------
         * Show modal error
         * ---------------------------------------------------------
         */

        function showError(message) {

            const errorAlert =
                document.getElementById(
                    "changePasswordError"
                );

            if (!errorAlert) {
                return;
            }

            errorAlert.textContent =
                message || "Unable to change password.";

            errorAlert.classList.remove("d-none");
        }


        /*
         * ---------------------------------------------------------
         * Display field-level validation errors
         * ---------------------------------------------------------
         */

        function showFieldErrors(errors) {

            if (!Array.isArray(errors)) {
                return;
            }

            errors.forEach(function (error) {

                if (!error.field) {
                    return;
                }

                const errorElement =
                    document.getElementById(
                        error.field + "Error"
                    );

                if (errorElement) {

                    errorElement.textContent =
                        error.message || "Invalid value.";

                }

            });

        }


        /*
         * ---------------------------------------------------------
         * Submit button state
         * ---------------------------------------------------------
         */

        function setSubmitting(isSubmitting) {

            const button =
                document.getElementById(
                    "changePasswordSubmit"
                );

            if (!button) {
                return;
            }

            if (isSubmitting) {

                button.disabled = true;

                button.dataset.originalHtml =
                    button.innerHTML;

                button.innerHTML =
                    '<span class="spinner-border spinner-border-sm me-1" ' +
                    'role="status" aria-hidden="true"></span>' +
                    'Changing...';

            }
            else {

                button.disabled = false;

                if (button.dataset.originalHtml) {

                    button.innerHTML =
                        button.dataset.originalHtml;

                }

            }

        }


        /*
         * ---------------------------------------------------------
         * FORM SUBMISSION
         * ---------------------------------------------------------
         *
         * This is the important part.
         *
         * preventDefault() prevents the browser from navigating
         * to the JSON response.
         * ---------------------------------------------------------
         */

        form.addEventListener("submit", async function (event) {

            event.preventDefault();
            event.stopPropagation();

            clearErrors();


            /*
             * Browser validation
             */

            if (!form.checkValidity()) {

                form.reportValidity();

                return;

            }


            const currentPassword =
                document.getElementById(
                    "CurrentPassword"
                )?.value || "";

            const newPassword =
                document.getElementById(
                    "NewPassword"
                )?.value || "";

            const confirmPassword =
                document.getElementById(
                    "ConfirmPassword"
                )?.value || "";


            /*
             * New password must be different.
             */

            if (currentPassword === newPassword) {

                const error =
                    document.getElementById(
                        "NewPasswordError"
                    );

                if (error) {

                    error.textContent =
                        "The new password must be different from the current password.";

                }

                return;
            }


            /*
             * Confirm password.
             */

            if (newPassword !== confirmPassword) {

                const error =
                    document.getElementById(
                        "ConfirmPasswordError"
                    );

                if (error) {

                    error.textContent =
                        "The new passwords do not match.";

                }

                return;
            }


            /*
             * Anti-forgery token
             */

            const token =
                form.querySelector(
                    'input[name="__RequestVerificationToken"]'
                );

            if (!token) {

                showError(
                    "Security token is missing. Please refresh the page."
                );

                return;
            }


            /*
             * FormData contains:
             *
             * CurrentPassword
             * NewPassword
             * ConfirmPassword
             * __RequestVerificationToken
             */

            const formData =
                new FormData(form);


            setSubmitting(true);


            try {

                /*
                 * IMPORTANT:
                 *
                 * fetch() prevents the browser from navigating
                 * to the JSON response.
                 */

                const response =
                    await fetch(
                        form.action,
                        {
                            method: "POST",
                            body: formData,
                            credentials: "same-origin",
                            headers: {
                                "X-Requested-With": "XMLHttpRequest",
                                "Accept": "application/json"
                            }
                        }
                    );


                /*
                 * Read JSON response.
                 */

                let result;

                try {

                    result =
                        await response.json();

                }
                catch (jsonError) {

                    console.error(
                        "Change Password returned non-JSON response."
                    );

                    showError(
                        "The server returned an unexpected response."
                    );

                    return;
                }


                /*
                 * -------------------------------------------------
                 * FAILURE
                 * -------------------------------------------------
                 */

                if (!response.ok || !result.success) {

                    showError(
                        result.message ||
                        "Unable to change password."
                    );

                    showFieldErrors(
                        result.errors
                    );

                    return;
                }


                /*
                 * -------------------------------------------------
                 * SUCCESS
                 * -------------------------------------------------
                 */

                /*
                 * Update first-login state locally.
                 */

                modalElement.dataset.firstLogin =
                    "false";


                /*
                 * Clear password fields.
                 */

                const current =
                    document.getElementById(
                        "CurrentPassword"
                    );

                const newPasswordInput =
                    document.getElementById(
                        "NewPassword"
                    );

                const confirm =
                    document.getElementById(
                        "ConfirmPassword"
                    );

                if (current) {
                    current.value = "";
                }

                if (newPasswordInput) {
                    newPasswordInput.value = "";
                }

                if (confirm) {
                    confirm.value = "";
                }


                /*
                 * Close the modal FIRST.
                 */

                modal.hide();


                /*
                 * Then display success message on the
                 * CURRENT PAGE.
                 */

                showAppMessage(
                    result.message ||
                    "Your password has been changed successfully.",
                    "success"
                );


            }
            catch (error) {

                console.error(
                    "Change password request failed:",
                    error
                );

                showError(
                    "An unexpected error occurred while changing your password."
                );

            }
            finally {

                setSubmitting(false);

            }

        });

    }


    /*
     * ---------------------------------------------------------
     * Initialize after DOM is ready.
     * ---------------------------------------------------------
     */

    if (document.readyState === "loading") {

        document.addEventListener(
            "DOMContentLoaded",
            initializeChangePasswordModal
        );

    }
    else {

        initializeChangePasswordModal();

    }

})();