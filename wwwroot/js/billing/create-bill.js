
// ============================================================
// State
// ============================================================

let lineIndex = 0;

let billingContextRequest = null;

let consultationChargeId = null;

const billForm = $("#billForm");

const urls = {
    billingContext: billForm.data("get-billing-context-url"),
    previousBills: billForm.data("get-previous-bills-url")
};

// ============================================================
// Initial page load
// ============================================================

$(function () {

    initializePatientAutocomplete();

    initializeDoctorSelection();

    initializePreviousBills();

    initializeBillDate();

    /*
        * If Create was opened from an appointment,
        * PatientId is already populated by the server.
        *
        * Trigger the same workflow that happens when the
        * patient is selected through autocomplete.
        */
    let patientId = $("#PatientId").val();

    if (patientId) {

        initializeAppointmentContext();

        loadBillingContext();

    }
});

function initializeAppointmentContext()
{
    const container =
        $("#doctorContainer");

    const appointmentId =
        container.data("appointment-id");

    const doctorId =
        container.data("doctor-id");

    const doctorName =
        container.data("doctor-name");


    if (!appointmentId || !doctorId)
        return;


    /*
     * Billing was opened from a specific appointment.
     *
     * The appointment already determines the doctor.
     * Do not require the user to select a doctor again.
     */
    $("#DoctorDisplay")
        .val(doctorName || "")
        .show();

    $("#DoctorId")
        .val(doctorId)
        .hide();

    container.show();
}


// ============================================================
// Bill date changed
// ============================================================

function initializeBillDate()
{
    $("#BillDate").on("change", function () {

        let patientId = $("#PatientId").val();

        if (!patientId)
            return;

        clearBillingContext();

        loadBillingContext();
    });
}


// ============================================================
// Patient autocomplete
// ============================================================

function initializePatientAutocomplete()
{
    wirePatientAutocomplete(
        "#PatientSearch",

        function (patient) {

            $("#PatientId").val(patient.id);

            /*
                * Clear the previous appointment/visit context
                * before loading the new patient's context.
                */
            clearBillingContext();

            loadBillingContext();
        }
    );
}


// ============================================================
// Billing context
// ============================================================

function loadBillingContext()
{
    let patientId = $("#PatientId").val();
    let billDate = $("#BillDate").val();
    let appointmentId = $("#AppointmentId").val();

    if (!patientId || !billDate)
        return;


    /*
        * Cancel a previous request if the user changes
        * patient/date quickly.
        */
    if (billingContextRequest) {
        billingContextRequest.abort();
    }

    billingContextRequest = $.get(
        urls.billingContext,
        {
            patientId: patientId,
            billDate: billDate,
            appointmentId: appointmentId || null
        }
    )
    .done(function (res) {

        if (!res || !res.success) {

            clearBillingContext();

            showContextMessage(
                res?.message || "Unable to retrieve billing information.",
                "danger"
            );

            return;
        }


        /*
            * Keep the patient selected by autocomplete,
            * but make sure the displayed patient name is
            * synchronized with the server.
            */
        if (res.patient) {

            $("#PatientId").val(res.patient.patientId);

            $("#PatientSearch").val(res.patient.patientName);
        }


        $("#VisitId").val(res.visitId || "");

        consultationChargeId = res.consultationChargeId || null;

        const previousBillCount = Number(res.previousBillCount) || 0;

        if (previousBillCount > 0)
        {
            $("#previousBillsCount").text(previousBillCount);
            $("#previousBillsButton").show();
        }
        else
        {
            $("#previousBillsCount").text("0");
            $("#previousBillsButton").hide();
            $("#previousBillsContent").empty()
                                        .removeData("loaded");
        }


        /*
            * Build doctor/appointment UI.
            */
        const appointmentId = $("#AppointmentId").val();

        populateDoctorContext(res.appointments || []);

    })
    .fail(function (xhr, status) {

        if (status === "abort")
            return;

        clearBillingContext();

        showContextMessage(
            "Unable to retrieve visit information.",
            "danger"
        );

    })
    .always(function () {

        billingContextRequest = null;

    });
}


// ============================================================
// Doctor / appointment context
// ============================================================

function populateDoctorContext(appointments)
{
    resetDoctorContext();

    if (!appointments || appointments.length === 0)
    {
        $("#AppointmentId").val("");
        return;
    }

    if (appointments.length === 1)
    {
        const appointment = appointments[0];

        $("#AppointmentId").val(
            appointment.appointmentId
        );

        $("#DoctorDisplay")
            .val(appointment.doctorName)
            .show();

        $("#DoctorId")
            .hide();

        $("#doctorContainer")
            .show();

        // Automatically add consultation charge
        addConsultationLine(appointment);

        return;
    }

    // Multiple appointments
    $("#DoctorId").empty();

    $("#DoctorId").append(
        '<option value="">-- select doctor --</option>'
    );

    $.each(appointments, function (i, appointment)
    {
        const option = $("<option>", {
            value: appointment.appointmentId,
            text: appointment.doctorName
        });

        option.data("fee", appointment.fee);

        $("#DoctorId").append(option);
    });

    $("#DoctorDisplay").hide();

    $("#DoctorId").show();

    $("#doctorContainer").show();

    $("#AppointmentId").val("");
}

// ============================================================
// Doctor selection
// ============================================================

function initializeDoctorSelection()
{
    $(document).on(
        "change",
        "#DoctorId",
        function ()
        {
            let appointmentId = $(this).val();

            $("#AppointmentId").val(
                appointmentId || ""
            );


            /*
                * No doctor selected.
                *
                * Do not automatically add consultation.
                */
            if (!appointmentId)
                return;


            let option =
                $(this).find("option:selected");

            let fee =
                parseFloat(option.data("fee")) || 0;

            let doctorName =
                option.text();


            let appointment = {
                appointmentId: appointmentId,
                doctorName: doctorName,
                fee: fee
            };


            addConsultationLine(appointment);
        }
    );
}


// ============================================================
// Reset billing context
// ============================================================

function clearBillingContext()
{
    $("#VisitId").val("");

    $("#AppointmentId").val("");

    consultationChargeId = null;

    $("#billTable tbody").empty();

    lineIndex = 0;

    calculateTotal();

    resetDoctorContext();

    $("#previousBillsButton").hide();

    $("#previousBillsCount").text("0");

    $("#previousBillsContent").empty()
                                .removeData("loaded");

    $("#billingContextMessage").hide().text("");

}



function resetDoctorContext()
{
    $("#doctorContainer").hide();

    $("#DoctorDisplay")
        .val("")
        .hide();

    $("#DoctorId")
        .empty()
        .append(
            '<option value="">-- select doctor --</option>'
        )
        .val("")
        .hide();
}

// ============================================================
// Add consultation line
// ============================================================

function addConsultationLine(appointment)
{
    if (!appointment)
        return;

    console.log(
        "Adding consultation line:",
        appointment,
        "chargeId:",
        consultationChargeId
    );

    const fee =
        parseFloat(appointment.fee) || 0;

    /*
     * Don't create duplicate consultation lines.
     */
    const existing =
        $("#billTable tbody tr.consultation-line");

    if (existing.length > 0)
    {
        existing.find(".rate")
            .val(fee.toFixed(2));

        calculateRow(existing);

        return;
    }

    /*
     * Consultation charge must be configured.
     */
    if (
        consultationChargeId === null ||
        consultationChargeId === undefined ||
        consultationChargeId === ""
    )
    {
        console.error(
            "Consultation charge ID is missing."
        );

        return;
    }

    const row = `
        <tr class="consultation-line">

            <td>

                <input type="text"
                       class="form-control chargeSearch"
                       value="Consultation"
                       readonly />

                <input type="hidden"
                       name="Bill.BillLines[${lineIndex}].ReferenceId"
                       class="chargeId"
                       value="${consultationChargeId}" />

            </td>

            <td>

                <input type="number"
                       step="0.01"
                       name="Bill.BillLines[${lineIndex}].Rate"
                       value="${fee.toFixed(2)}"
                       class="form-control rate" />

            </td>

            <td>

                <input type="number"
                       step="1"
                       name="Bill.BillLines[${lineIndex}].Quantity"
                       value="1"
                       class="form-control qty" />

            </td>

            <td>

                <input type="number"
                       step="0.01"
                       name="Bill.BillLines[${lineIndex}].DiscountPercent"
                       value="0"
                       class="form-control disc" />

            </td>

            <td>

                <input type="text"
                       name="Bill.BillLines[${lineIndex}].LineAmount"
                       class="form-control lineAmount"
                       readonly />

            </td>

            <td>

                <button type="button"
                        class="btn btn-danger remove">
                    X
                </button>

            </td>

        </tr>
    `;

    $("#billTable tbody").append(row);

    const newRow =
        $("#billTable tbody tr.consultation-line").last();

    calculateRow(newRow);

    lineIndex++;

    console.log(
        "Consultation line added."
    );
}



// ============================================================
// Add bill line
// ============================================================

$("#addLine").click(function () {

    let row = `
        <tr>

            <td>

                <input type="text"
                        class="form-control chargeSearch"
                        placeholder="Search charge..."
                        autocomplete="off" />

                <input type="hidden"
                        name="Bill.BillLines[${lineIndex}].ReferenceId"
                        class="chargeId" />

            </td>


            <td>

                <input name="Bill.BillLines[${lineIndex}].Rate"
                        class="form-control rate" />

            </td>


            <td>

                <input name="Bill.BillLines[${lineIndex}].Quantity"
                        value="1"
                        class="form-control qty" />

            </td>


            <td>

                <input name="Bill.BillLines[${lineIndex}].DiscountPercent"
                        value="0"
                        class="form-control disc" />

            </td>


            <td>

                <input name="Bill.BillLines[${lineIndex}].LineAmount"
                        class="form-control lineAmount"
                        readonly />

            </td>


            <td>

                <button type="button"
                        class="btn btn-danger remove">
                    X
                </button>

            </td>

        </tr>
    `;


    $("#billTable tbody").append(row);


    initializeChargeAutocomplete(
        $("#billTable tbody tr:last")
    );


    lineIndex++;
});


// ============================================================
// Calculate line
// ============================================================

$(document).on(
    "keyup change",
    ".rate,.qty,.disc",
    function () {

        let row = $(this).closest("tr");

        calculateRow(row);
    }
);


function calculateRow(row)
{
    let rate =
        parseFloat(row.find(".rate").val()) || 0;

    let qty =
        parseFloat(row.find(".qty").val()) || 0;

    let disc =
        parseFloat(row.find(".disc").val()) || 0;


    let net =
        rate * qty * (1 - disc / 100);


    row.find(".lineAmount")
        .val(net.toFixed(2));


    calculateTotal();
}


// ============================================================
// Calculate total
// ============================================================

function calculateTotal()
{
    let total = 0;


    $(".lineAmount").each(function () {

        total +=
            parseFloat($(this).val()) || 0;

    });


    $("#Total")
        .val(total.toFixed(2));
}


// ============================================================
// Remove line
// ============================================================

$(document).on(
    "click",
    ".remove",
    function () {

        $(this)
            .closest("tr")
            .remove();

        calculateTotal();
    }
);


// ============================================================
// Charge autocomplete
// ============================================================

function initializeChargeAutocomplete(row)
{
    wireChargeAutocomplete(

        row.find(".chargeSearch"),

        function (charge) {

            row.find(".chargeId")
                .val(charge.id);

            row.find(".rate")
                .val(charge.rate);

            calculateRow(row);
        }
    );
}


// ============================================================
// Previous bills modal
// ============================================================

function initializePreviousBills()
{
    $("#previousBillsModal").on(
        "show.bs.modal",
        function () {

            let visitId =
                $("#VisitId").val();


            if (!visitId) {

                $("#previousBillsLoading")
                    .hide();

                $("#previousBillsContent")
                    .html(`
                        <div class="text-center text-muted py-4">
                            No previous bills found.
                        </div>
                    `);

                return;
            }


            let content =
                $("#previousBillsContent");


            /*
                * Don't retrieve again once loaded.
                */
            if (content.data("loaded"))
                return;


            $("#previousBillsLoading")
                .show();

            content.empty();


            $.get(
                urls.previousBills,
                {
                    visitId: visitId
                }
            )
            .done(function (bills) {

                $("#previousBillsLoading")
                    .hide();


                if (!bills || bills.length === 0) {

                    content.html(`
                        <div class="text-center text-muted py-4">
                            No previous bills found.
                        </div>
                    `);

                    content.data("loaded", true);

                    return;
                }


                let html = `

                    <div class="table-responsive">

                        <table class="table table-hover align-middle">

                            <thead>

                                <tr>
                                    <th>Bill</th>
                                    <th>Date</th>
                                    <th class="text-end">Amount</th>
                                    <th class="text-end">Paid</th>
                                    <th>Status</th>
                                    <th></th>
                                </tr>

                            </thead>

                            <tbody>
                `;


                $.each(
                    bills,
                    function (i, bill) {

                        let statusClass =
                            "bg-danger";


                        if (bill.status === "Paid")
                            statusClass =
                                "bg-success";

                        else if (
                            bill.status === "Partially Paid"
                        )
                            statusClass =
                                "bg-warning text-dark";


                        html += `

                            <tr>

                                <td>
                                    <strong>
                                        ${bill.billNo}
                                    </strong>
                                </td>


                                <td>
                                    ${bill.billDate}
                                </td>


                                <td class="text-end">
                                    ${Number(bill.amount).toFixed(2)}
                                </td>


                                <td class="text-end">
                                    ${Number(bill.paid).toFixed(2)}
                                </td>


                                <td>
                                    <span class="badge ${statusClass}">
                                        ${bill.status}
                                    </span>
                                </td>


                                <td class="text-end">

                                    <a href="/Billing/Print?id=${bill.billId}"
                                        target="_blank"
                                        class="btn btn-sm btn-outline-primary">
                                        View
                                    </a>

                                </td>

                            </tr>
                        `;
                    }
                );


                html += `

                            </tbody>

                        </table>

                    </div>
                `;


                content.html(html);

                content.data("loaded", true);

            })
            .fail(function () {

                $("#previousBillsLoading")
                    .hide();

                content.html(`
                    <div class="alert alert-danger">
                        Unable to retrieve previous bills.
                    </div>
                `);

            });
        }
    );
}


// ============================================================
// Prevent accidental submission without patient
// ============================================================

$("#billForm").on("submit", function (e) {

    let patientId =
        $("#PatientId").val();


    if (!patientId) {

        e.preventDefault();

        alert("Please select a patient.");

        $("#PatientSearch").focus();

        return false;
    }


    /*
        * If there are multiple appointments, the user must
        * explicitly select a doctor.
        */
    if (
        $("#DoctorId").is(":visible") &&
        !$("#AppointmentId").val()
    ) {

        e.preventDefault();

        alert("Please select a doctor.");

        $("#DoctorId").focus();

        return false;
    }


    return true;
});