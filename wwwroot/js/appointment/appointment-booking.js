let fcCalendar = null;
let currentView = "list";

// ============================================================
// DEPARTMENT / DOCTOR
// ============================================================

$("#departmentId").change(function () {

    let deptId =
        $(this).val();

    $.get(
        '/AppointmentBooking/GetDoctors',
        {
            departmentId: deptId
        },
        function (data) {

            let ddl =
                $("#doctorId");

            ddl.empty();

            ddl.append(
                '<option value="">Select</option>'
            );

            $.each(
                data,
                function (i, v) {

                    ddl.append(
                        `<option value="${v.doctorId}">
                            ${v.doctorName}
                         </option>`
                    );

                }
            );

        }
    );

});

$("#doctorId").change(
    loadSlots
);

$("#appointmentDate").change(
    loadSlots
);

$("#btnList").click(function () {

    currentView =
        "list";

    loadSlots();

});

$("#btnCalendar").click(function () {

    currentView =
        "calendar";

    loadSlots();

});

// ============================================================
// LOAD SLOTS
// ============================================================

function loadSlots() {

    let doctorId =
        $("#doctorId").val();

    let appointmentDate =
        $("#appointmentDate").val();

    if (!doctorId)
        return;

    $.get(
        '/AppointmentBooking/GetDailySlots',
        {
            doctorId:
                doctorId,

            appointmentDate:
                appointmentDate
        },
        function (data) {

            renderSlots(data);

        }
    );

}

// ============================================================
// RENDER SLOTS
// ============================================================

function renderSlots(slots) {

    let html = '';

    slots.forEach(slot => {

        let financialStatus =
            slot.billingStatus === 'Unbilled'
                ? 'Unbilled'
                : slot.paymentStatus;

        html += `

            <div class="slot-stack">

                ${renderCancelledCards(
                    slot.cancelledAppointments
                )}

                <div class="slot-card ${
                    slot.isBooked
                        ? 'slot-booked'
                        : 'slot-available'
                }"
                onclick='selectSlot(
                    ${slot.appointmentId || 0},
                    "${slot.slotTime}",
                    "${slot.patientName || ''}",
                    "${slot.appointmentStatus}",
                    "${financialStatus}"
                )'>

                    ${
                        slot.isBooked &&
                        slot.queueNumber != null

                        ? `
                            <div class="queue-number">
                                Q: ${slot.queueNumber}
                            </div>
                          `

                        : ''
                    }

                    <div class="slot-time">
                        ${slot.slotTime}
                    </div>

                    <div class="slot-patient">
                        ${slot.patientName || ''}
                    </div>

                    <div class="status-row">

                        ${
                            !slot.isBooked

                            ? `
                                <span class="slot-status status-available">
                                    Available
                                </span>
                              `

                            : slot.paymentStatus === 'Paid'

                                ? `
                                    <span class="slot-status status-booked-paid">
                                        Booked & Paid
                                    </span>
                                  `

                                : `
                                    <span class="slot-status status-booked">
                                        Booked
                                    </span>

                                    <a href="#"
                                       class="bill-pay-link"
                                       onclick="
                                           event.stopPropagation();
                                           billAndPay(${slot.appointmentId});
                                           return false;
                                       ">

                                        Bill & Pay

                                    </a>
                                  `
                        }

                    </div>

                </div>

            </div>

        `;

    });

    $('#slotContainer')
        .html(html);

}

// ============================================================
// STATUS
// ============================================================

function getStatusClass(status) {

    switch (
        status?.toUpperCase()
    ) {

        case 'AVAILABLE':
            return 'status-available';

        case 'BOOKED':
            return 'status-booked';

        case 'IN CONSULTATION':
            return 'status-consultation';

        case 'COMPLETED':
            return 'status-complete';

        case 'CANCELLED':
            return 'status-cancelled';

        case 'NOSHOW':
            return 'status-noshow';

        default:
            return '';

    }

}

// ============================================================
// SELECT SLOT
// ============================================================

function selectSlot(
    appointmentId,
    slotTime,
    patientName,
    appointmentStatus,
    financialStatus
) {

    $('.modal-title')
        .text('Appointment Details');

    $('#appointmentDetailsSection')
        .show();

    $('#cancelledAppointmentsContainer')
        .hide()
        .html('');

    $('#financialStatusRow')
        .show();

    let appointmentDate =
        $("#appointmentDate").val();

    let appointmentDateTime =
        appointmentDate +
        "T" +
        slotTime;

    console.log(
        appointmentDateTime
    );

    /*
     * Available slot.
     *
     * Open booking modal.
     */

    if (!appointmentId) {

        clearBookingModal();

        $("#bookAppointmentDateTime")
            .val(appointmentDateTime);

        new bootstrap.Modal(
            document.getElementById(
                'bookingModal'
            )
        ).show();

        return;

    }

    switch (
        appointmentStatus?.toUpperCase()
    ) {

        case 'BOOKED':

            $('#modalAppointmentId')
                .val(appointmentId);

            $('#modalPatientName')
                .text(patientName);

            $('#modalAppointmentDate')
                .text(appointmentDate);

            $('#modalSlotTime')
                .text(slotTime);

            $('#modalStatus')
                .text(appointmentStatus);

            $("#financialStatusRow")
                .show();

            $('#modalFinancialStatus')
                .text(financialStatus);

            $('#btnReschedule')
                .show();

            $('#btnCancel')
                .show();

            new bootstrap.Modal(
                document.getElementById(
                    'appointmentModal'
                )
            ).show();

            return;

        case 'IN CONSULTATION':

            window.location.href =
                '/Consultation/Consult/' +
                appointmentId;

            return;

        case 'COMPLETED':

            $('#modalAppointmentId')
                .val(appointmentId);

            $('#modalPatientName')
                .text(patientName);

            $('#modalAppointmentDate')
                .text(appointmentDate);

            $('#modalSlotTime')
                .text(slotTime);

            $('#modalStatus')
                .text(appointmentStatus);

            $("#financialStatusRow")
                .show();

            $('#modalFinancialStatus')
                .text(financialStatus);

            $('#btnReschedule')
                .hide();

            $('#btnCancel')
                .hide();

            new bootstrap.Modal(
                document.getElementById(
                    'appointmentModal'
                )
            ).show();

            return;

    }

}

// ============================================================
// INITIAL LOAD
// ============================================================

function logPatientHeaders(source) {

    const headers =
        document.querySelectorAll(
            "[data-patient-header]"
        );

    console.log(
        "PATIENT HEADER CHECK:",
        source,
        "count =",
        headers.length
    );

    headers.forEach(function (header, index) {

        console.log(
            "Header",
            index + 1,
            header
        );

    });
}

$(document).ready(function () {

    logPatientHeaders("document ready");

    let deptId =
        $("#departmentId").val();

    let selectedDoctorId =
        '@Model.DoctorId';

    if (!deptId)
        return;

    $.get(
        '/AppointmentBooking/GetDoctors',
        {
            departmentId:
                deptId
        },
        function (data) {

            let ddl =
                $("#doctorId");

            ddl.empty();

            ddl.append(
                '<option value="">Select</option>'
            );

            $.each(
                data,
                function (i, v) {

                    let selected =
                        v.doctorId ==
                        selectedDoctorId
                            ? 'selected'
                            : '';

                    ddl.append(
                        `<option value="${v.doctorId}" ${selected}>
                            ${v.doctorName}
                         </option>`
                    );

                }
            );

            if (selectedDoctorId)
                loadSlots();

        }
    );

    logPatientHeaders("after load");

});

// ============================================================
// CANCEL APPOINTMENT
// ============================================================

let cancelling = false;

$('#btnCancel').click(
    function () {

        if (!cancelling) {

            cancelling = true;

            $('#cancelReasonSection')
                .show();

            $(this)
                .text('Confirm Cancel');

            return;

        }

        $.ajax({

            url:
                '/AppointmentBooking/Cancel',

            type:
                'POST',

            data: {

                appointmentId:
                    $('#modalAppointmentId')
                        .val(),

                cancellationReason:
                    $('#cancelReason')
                        .val()

            },

            success: function () {

                let departmentId =
                    $("#departmentId")
                        .val();

                let doctorId =
                    $("#doctorId")
                        .val();

                let appointmentDate =
                    $("#appointmentDate")
                        .val();

                window.location.href =
                    '/AppointmentBooking/Index' +
                    '?departmentId=' +
                    departmentId +
                    '&doctorId=' +
                    doctorId +
                    '&appointmentDate=' +
                    appointmentDate;

            }

        });

    }
);

// ============================================================
// CANCELLED APPOINTMENTS
// ============================================================

function renderCancelledCards(
    cancelledAppointments
) {

    if (
        !cancelledAppointments ||
        cancelledAppointments.length === 0
    )
        return '';

    let json =
        encodeURIComponent(
            JSON.stringify(
                cancelledAppointments
            )
        );

    return `

        <div class="cancelled-card"></div>

        <div class="cancelled-tab"
             onclick="
                 event.stopPropagation();
                 showCancelledAppointments('${json}')
             ">

            ${cancelledAppointments.length}

        </div>

    `;

}

function showCancelledAppointments(
    encodedAppointments
) {

    let appointments =
        JSON.parse(
            decodeURIComponent(
                encodedAppointments
            )
        );

    $('#appointmentDetailsSection')
        .hide();

    $('.modal-title')
        .text('Cancelled Appointments');

    $('#financialStatusRow')
        .hide();

    $('#btnReschedule')
        .hide();

    $('#btnCancel')
        .hide();

    let html = `

        <h6 class="mb-3">
            Cancelled Appointments
            (${appointments.length})
        </h6>

    `;

    appointments.forEach(a => {

        html += `

            <div class="card mb-2 border-danger">

                <div class="card-body p-2">

                    <div>
                        <strong>Patient:</strong>
                        ${a.patientName || ''}
                    </div>

                    <div>
                        <strong>Date:</strong>
                        ${a.appointmentDateDisplay || ''}
                    </div>

                    <div>
                        <strong>Time:</strong>
                        ${a.slotTimeDisplay || ''}
                    </div>

                    <div>
                        <strong>Reason:</strong>
                        ${a.cancellationReason || ''}
                    </div>

                </div>

            </div>

        `;

    });

    $('#cancelledAppointmentsContainer')
        .html(html)
        .show();

    new bootstrap.Modal(
        document.getElementById(
            'appointmentModal'
        )
    ).show();

}

// ============================================================
// CONFIRM BOOKING
// ============================================================

$("#btnConfirmBooking").click(
    async function () {

        /*
         * --------------------------------------------------------
         * BOOKING VALUES
         * --------------------------------------------------------
         *
         * These belong to Appointment Booking and therefore
         * remain here.
         */

        const departmentId =
            $("#departmentId").val();

        const doctorId =
            $("#doctorId").val();

        const appointmentDateTime =
            $("#bookAppointmentDateTime").val();

        const visitType =
            $("input[name='bookVisitType']:checked")
                .val();


        /*
         * --------------------------------------------------------
         * PATIENT CONTEXT
         * --------------------------------------------------------
         *
         * Do not access booking-specific patient IDs or fields.
         *
         * The common patient component owns all patient
         * selection and patient demographic state.
         */

        const bookingModal =
            document.getElementById(
                "bookingModal"
            );

        const patientContext =
            bookingModal.querySelector(
                "[data-patient-selection-context]"
            );


        if (!patientContext) {

            alert(
                "Patient selection is not available."
            );

            return;

        }


        /*
         * --------------------------------------------------------
         * SAVE SELECTED PATIENT
         * --------------------------------------------------------
         *
         * PatientCache.save() saves the selected patient into
         * the server-side cache and refreshes the parent header.
         */

        const patientSaved =
            await PatientCache.save(
                patientContext
            );


        if (!patientSaved) {

            alert(
                "Unable to save the selected patient."
            );

            return;

        }


        /*
         * --------------------------------------------------------
         * GET SELECTED PATIENT
         * --------------------------------------------------------
         *
         * No patient-specific DOM ID is used here.
         */

        const patient = PatientCache.getCurrentPatient();

        if (!patient) {

            alert(
                "Please select a patient first."
            );

            return;

        }


        const patientId =
            patient.patientId ||
            patient.id ||
            "";


        if (!patientId) {

            alert(
                "The selected patient does not have a valid Patient ID."
            );

            return;

        }


        console.log({

            doctorId:
                doctorId,

            departmentId:
                departmentId,

            patientId:
                patientId,

            visitType:
                visitType,

            appointmentDateTime:
                appointmentDateTime

        });


        /*
         * --------------------------------------------------------
         * BOOK APPOINTMENT
         * --------------------------------------------------------
         */

        $.post(
            '/AppointmentBooking/BookAjax',
            {

                doctorId:
                    doctorId,

                departmentId:
                    departmentId,

                patientId:
                    patientId,

                visitType:
                    visitType,

                appointmentDateTime:
                    appointmentDateTime

            },

            function (response) {

                if (!response.success) {

                    alert(
                        response.message ||
                        "Appointment booking failed."
                    );

                    return;

                }


                /*
                 * Patient header was already refreshed by
                 * PatientCache.save().
                 *
                 * Do not update patient demographics here.
                 */


                const modal =
                    bootstrap.Modal.getInstance(
                        document.getElementById(
                            'bookingModal'
                        )
                    );

                if (modal) {
                    modal.hide();
                }


                $("#departmentId")
                    .val(departmentId);

                $("#doctorId")
                    .val(doctorId);

                $("#appointmentDate")
                    .val(
                        $("#appointmentDate").val()
                    );


                loadSlots();

            }
        );

    }
);

// ============================================================
// CLEAR BOOKING MODAL
// ============================================================

function clearBookingModal() {

    /*
     * These are booking-specific values.
     *
     * Patient fields are deliberately NOT cleared here.
     * PatientCache owns those fields.
     */

    $("#bookAppointmentDateTime")
        .val('');

    $("input[name='bookVisitType']")
        .prop(
            "disabled",
            false
        );

    $("input[name='bookVisitType'][value='New']")
        .prop(
            "checked",
            true
        );

}

// ============================================================
// BILL & PAY
// ============================================================

function billAndPay(
    appointmentId
) {

    window.location.href =
        '/Billing/Create?appointmentId=' +
        appointmentId;

}
