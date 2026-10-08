$(function () {

    wireApprovalEvents();

    loadApprovalBillHeader();

});


function wireApprovalEvents() {

    /*
     * View refund from the pending approval list.
     *
     * Delegated event because the list/rows may be
     * rendered dynamically.
     */
    $(document)
        .off("click", ".btn-view-refund")
        .on("click", ".btn-view-refund", function () {

            const refundId = $(this).data("refund-id");

            if (!refundId)
                return;

            loadApprovalDetails(refundId);
        });


    /*
     * Approve refund.
     */
    $(document)
        .off("click", "#btnApproveRefund")
        .on("click", "#btnApproveRefund", function () {

            const refundId = $(this).data("refund-id");

            if (!refundId)
                return;

            approveRefund(refundId);
        });


    /*
     * Reject refund.
     */
    $(document)
        .off("click", "#btnRejectRefund")
        .on("click", "#btnRejectRefund", function () {

            const refundId = $(this).data("refund-id");

            if (!refundId)
                return;

            openRejectModal(refundId);
        });


    /*
     * Final rejection from modal.
     */
    $(document)
        .off("click", "#btnConfirmRejectRefund")
        .on("click", "#btnConfirmRejectRefund", function () {

            const refundId = $(this).data("refund-id");

            if (!refundId)
                return;

            rejectRefund(refundId);
        });

}


/* =========================================================
   LOAD APPROVAL DETAILS
   ========================================================= */

function loadApprovalDetails(refundId) {

    const container = $("#refundApprovalContainer");

    if (!container.length) {
        console.error(
            "Refund approval container not found."
        );
        return;
    }


    container.html(`
        <div class="text-center py-5">
            <div class="spinner-border text-primary"
                 role="status">
            </div>

            <div class="mt-2 text-muted">
                Loading refund details...
            </div>
        </div>
    `);


    $.get(
        "/Refund/ApprovalDetails",
        {
            id: refundId
        }
    )
    .done(function (html) {

        container.html(html);

        /*
         * No explicit event wiring is necessary here because
         * Approve/Reject handlers are delegated from document.
         */

        loadApprovalBillHeader();

    })
    .fail(function (xhr) {

        let message = "Unable to load refund details.";

        if (xhr.responseJSON && xhr.responseJSON.message) 
        {
            message = xhr.responseJSON.message;
        }

        container.html(`
            <div class="alert alert-danger">
                <i class="fa-solid fa-circle-exclamation me-2"></i>
                ${escapeHtml(message)}
            </div>
        `);

    });

}

function loadApprovalBillHeader() {

    const billId = $("#BillId").val();

    if (!billId)
        return;

    $.get(
        "/Refund/PrepareBillHeader",
        {
            billId: billId
        }
    )
    .done(function (bill) {

        $("#BillNo").val(bill.billNo);
        $("#BillDate").val(formatDate(bill.billDate));
        $("#VisitDate").val(formatDate(bill.visitDate));
        $("#BillAmount").val(bill.billAmount);

    })
    .fail(function () {

        console.error(
            "Unable to load bill details for refund approval."
        );

    });
}

/* =========================================================
   APPROVE
   ========================================================= */

function approveRefund(refundId) {

    if (!confirm("Are you sure you want to approve this refund?")) {
        return;
    }


    const button = $("#btnApproveRefund");

    setButtonBusy(button, true, "Approving...");


    $.post(
        "/Refund/Approve",
        {
            id: refundId
        }
    )
    .done(function () {

        alert("Refund approved successfully.");

        /*
         * Reload the Approval page so the approved refund
         * disappears from PendingApproval.
         */
        window.location.href = "/Refund/Approval";

    })
    .fail(function (xhr) {

        let message = "Unable to approve the refund.";

        if (xhr.responseJSON && xhr.responseJSON.message) 
        {

            message = xhr.responseJSON.message;
        }

        alert(message);

        setButtonBusy(button, false, "Approve");

    });

}


/* =========================================================
   OPEN REJECTION MODAL
   ========================================================= */

function openRejectModal(refundId) {

    const modalElement = document.getElementById("refundRejectModal");

    if (!modalElement) {

        console.error("Refund rejection modal not found.");
        return;
    }


    /*
     * Clear previous reason.
     */
    $("#RefundRejectionReason").val("");


    /*
     * Store RefundId on confirmation button.
     */
    $("#btnConfirmRejectRefund")
        .data("refund-id", refundId);


    /*
     * Clear validation.
     */
    $("#refundRejectionValidation")
        .addClass("d-none")
        .text("");


    const modal = bootstrap.Modal.getOrCreateInstance(modalElement);

    modal.show();

}


/* =========================================================
   REJECT
   ========================================================= */

function rejectRefund(refundId) {

    const reason = $.trim($("#RefundRejectionReason").val());

    /*
     * Rejection reason is mandatory.
     */
    if (!reason) {

        $("#refundRejectionValidation")
            .removeClass("d-none")
            .text("Please enter a reason for rejection.");

        $("#RefundRejectionReason").focus();

        return;
    }


    /*
     * Final confirmation.
     */
    if (!confirm(
        "Are you sure you want to reject this refund?\n\n" +
        "Rejected refunds cannot be returned to Draft."
    )) {
        return;
    }


    const button = $("#btnConfirmRejectRefund");

    setButtonBusy(button, true, "Rejecting...");


    $.post(
        "/Refund/Reject",
        {
            id: refundId,
            reason: reason
        }
    )
    .done(function () {

        /*
         * Close modal.
         */
        const modalElement = document.getElementById("refundRejectModal");
        const modal = bootstrap.Modal.getInstance(modalElement);

        if (modal)
            modal.hide();


        alert("Refund rejected successfully.");


        /*
         * Reload approval queue.
         */
        window.location.href = "/Refund/Approval";

    })
    .fail(function (xhr) {

        let message = "Unable to reject the refund.";

        if (xhr.responseJSON && xhr.responseJSON.message)
        {

            message = xhr.responseJSON.message;
        }

        alert(message);

        setButtonBusy(button, false, "Reject");

    });

}


/* =========================================================
   BUTTON STATE
   ========================================================= */

function setButtonBusy(button, busy, busyText) {

    if (!button || !button.length)
        return;


    if (busy) {

        button.data("original-html", button.html());

        button.prop("disabled", true);

        button.html(`
            <span class="spinner-border spinner-border-sm me-1"
                  role="status">
            </span>
            ${busyText}
        `);

    }
    else {

        const originalHtml = button.data("original-html");

        button.prop("disabled", false);

        if (originalHtml)
            button.html(originalHtml);
    }

}


/* =========================================================
   HTML ENCODING
   ========================================================= */

function escapeHtml(value) {

    if (value === null ||
        value === undefined) {

        return "";
    }

    return $("<div>")
        .text(value)
        .html();
}