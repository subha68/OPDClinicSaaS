let originalBillId = null;
let isEditMode = $("#Refund_RefundId").val() > 0;

$(function () {

    originalBillId = $("#BillId").val();

    wireEvents();

    const billId = $("#BillId").val();

    if (billId) {
        loadBillHeader(billId);
    }

    calculateTotals();

});

function wireEvents() {

    const isDraft = $("#IsDraft").val() === "true";

    if (isDraft) {

        $("#BillId")
            .on("change", onBillChanged);

        $("#btnAddLine")
            .on("click", openBillLineModal);

        $(document)
            .on("change", ".refund-qty", onQuantityChanged);

        $(document)
            .on("click", ".delete-line", deleteLine);
    }

}

function onBillChanged() {

    const billId = $("#BillId").val();

console.log("isEditMode =", isEditMode);
console.log("originalBillId =", originalBillId);
console.log("currentBillId =", billId);
console.log("lineCount =", $("#refundLines tr").length);

    if (!billId)
        return;

    const hasLines = $("#refundLines tr").length > 0;

    if (isEditMode &&
        hasLines &&
        originalBillId &&
        originalBillId !== billId) {

        const confirmed = confirm("Changing the bill will remove all existing refund lines. Continue?");

        if (!confirmed) {

            $("#BillId").val(originalBillId);
            return;
        }

        $("#refundLines").empty();
        calculateTotals();
    }

    clearBillHeader();

    loadBillHeader(billId);

    originalBillId = billId;

    autoAddSingleRefundLine(billId);
}

function loadBillHeader(billId) {

    $.get("/Refund/PrepareBillHeader",
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

        alert("Unable to load bill details.");

    });

}

function clearBillHeader() {

    $("#BillNo").val("");
    $("#BillDate").val("");
    $("#VisitDate").val("");
    $("#BillAmount").val("");

}

function openBillLineModal() {

    const billId = $("#BillId").val();

    if (!billId) {

        alert("Please select a bill first.");
        return;
    }

    $("#billLineContainer").html(

        '<div class="text-center p-4">' +
        '<div class="spinner-border"></div>' +
        '<div class="mt-2">Loading bill lines...</div>' +
        '</div>'

    );

    const modal = new bootstrap.Modal(
            document.getElementById("billLineModal")
        );

    modal.show();

    $("#billLineContainer").load(
        "/Refund/ListBillLines?billId=" + billId
    );

}

function addBillLine(billLineId) {

    const billId = $("#BillId").val();

    const index =
        $("#refundLines tr").length;

    $.get("/Refund/RefundLineTemplate",
        {
            billId: billId,
            billLineId: billLineId,
            index: index,
            isDraft: true
        })
        .done(function (html) {

            $("#refundLines").append(html);

            calculateTotals();

            const modalElement =
                document.getElementById("billLineModal");

            const modal =
                bootstrap.Modal.getInstance(modalElement);

            if (modal)
                modal.hide();

        })
        .fail(function () {

            alert("Unable to add refund line.");

        });

}

function deleteLine() {

    if (!confirm("Remove selected refund line?"))
        return;

    $(this)
        .closest("tr")
        .remove();

    renumberLines();

    calculateTotals();

}

function onQuantityChanged() {

    const row = $(this).closest("tr");

    recalculateRow(row);

    calculateTotals();

}

function recalculateRow(row) {

    const qty = parseFloat(
            row.find(
                ".refund-qty"
            ).val() || 0
        );

    const rate = parseFloat(
            row.find(
                "input[name$='.Rate']"
            ).val() || 0
        );

    const discountPercent = parseFloat(
            row.find(
                "input[name$='.DiscountPercent']"
            ).val() || 0
        );

    const taxPercent = parseFloat(
            row.find(
                "input[name$='.TaxPercent']"
            ).val() || 0
        );

    const gross = qty * rate;
    const discount = gross * discountPercent / 100;
    const taxable = gross - discount;
    const tax = taxable * taxPercent / 100;
    const net = taxable + tax;

    row.find(".refund-gross")
        .text(gross.toFixed(2));

    row.find(".refund-discount")
        .text(discount.toFixed(2));

    row.find(".refund-taxable")
        .text(taxable.toFixed(2));

    row.find(".refund-tax")
        .text(tax.toFixed(2));

    row.find(".refund-net")
        .text(net.toFixed(2));

}

function calculateTotals() {

    let gross = 0;
    let discount = 0;
    let taxable = 0;
    let tax = 0;
    let net = 0;

    $("#refundLines tr")
        .each(function () {

            gross += parseFloat(
                $(this)
                    .find(".refund-gross")
                    .text() || 0);

            discount += parseFloat(
                $(this)
                    .find(".refund-discount")
                    .text() || 0);

            taxable += parseFloat(
                $(this)
                    .find(".refund-taxable")
                    .text() || 0);

            tax += parseFloat(
                $(this)
                    .find(".refund-tax")
                    .text() || 0);

            net += parseFloat(
                $(this)
                    .find(".refund-net")
                    .text() || 0);

        });

    $("#Refund_GrossAmount").val(gross.toFixed(2));
    $("#Refund_DiscountAmount").val(discount.toFixed(2));
    $("#Refund_TaxableAmount").val(taxable.toFixed(2));
    $("#Refund_TaxAmount").val(tax.toFixed(2));
    $("#Refund_NetAmount").val(net.toFixed(2));

}

function renumberLines() {

    $("#refundLines tr")
        .each(function(index) {

            $(this)
                .find(".line-no")
                .text(index + 1);
        });
}

function autoAddSingleRefundLine(billId) {

    $.get("/Refund/GetEligibleRefundLines",
        {
            billId: billId
        })
        .done(function (lines) {

            if (!lines)
                return;

            if (lines.length === 1) {

                addBillLine(lines[0].billLineId);
            }
        });
}
