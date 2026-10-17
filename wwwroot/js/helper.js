function wireAutocomplete(
    selector,
    url,
    mapper,
    onSelected,
    appendToSelector,
    onCleared,
    additionalData)
{
    $(selector).autocomplete({

        appendTo: appendToSelector || "body",

        minLength: 2,

        source: function (request, response) {

            let data = {
                term: request.term
            };

            if (additionalData) {

                if (typeof additionalData === "function") {

                    data = {
                        ...data,
                        ...additionalData()
                    };
                }
                else {

                    data = {
                        ...data,
                        ...additionalData
                    };
                }
            }

            $.getJSON(
                url,
                data,
                function (result) {

                    const mapped = $.map(result, mapper);

                    response(mapped);
                });
        },

        select: function (event, ui) {

            event.preventDefault();

            const input = $(this);
            const value = ui.item.value;

            input.val(value);

            if (onSelected)
                onSelected.call(this, ui.item);

            return false;
        },

        change: function (event, ui) {

            const input = $(this);

            if (!ui.item) {

                const currentValue =
                    (input.val() || "").trim();

                input
                    .closest(".field-cell")
                    .find(".field-data-id")
                    .val("");

                if (currentValue === "" && onCleared) {
                    onCleared.call(this);
                }
            }
        }
    });
}

function wirePatientAutocomplete(
    selector,
    onSelected,
    appendToSelector)
{
    wireAutocomplete(

        selector,

        '/Patient/SearchPatient',

        function (item) {

            return {
                label: item.text,
                value: item.text,
                id: item.id,
                patientName: item.patientName,
                phone: item.phone,
                age: item.age,
                gender: item.gender
            };
        },

        onSelected,

        appendToSelector
    );
}

function wireChargeAutocomplete(
    selector,
    onSelected,
    appendToSelector)
{
    wireAutocomplete(

        selector,

        '/BillingCharge/SearchCharge',

        function (item) {

            const description =
                item.description ||
                item.text ||
                "";

            const rate =
                parseFloat(item.rate) || 0;

            return {
                label: `${description} - ₹${rate.toFixed(2)}`,
                value: description,
                id: item.id,
                rate: rate,
                description: description
            };
        },

        onSelected,

        appendToSelector
    );
}


function wireMedicineAutocomplete(selector, appendToSelector) {
    wireAutocomplete(
        selector,
        '/Medicine/SearchMedicine',
        function (item) {
            return {
                label: item.label,
                value: item.value,
                id: item.id,
                dosageForm: item.dosageForm,
                scheduleType: item.scheduleType,
                genericName: item.genericName
            };
        },
        function (item) {
            const input = $(this);
            const row = input.closest('tr');
            const fieldCell = input.closest('.field-cell');
            const fieldCode = input.attr('data-field-code');

            row.find('.field-data-id')
                .filter(function () {
                    return $(this).closest('.field-cell')
                        .is(fieldCell);
                })
                .val(item.id);

            // Find the merged field by its MergeWith relationship.
            const mergedCell = row.find('.merged-field-cell')
                .filter(function () {
                    return ($(this).attr('data-merge-with') || '')
                        .trim()
                        .toLowerCase() === (fieldCode || '')
                        .trim()
                        .toLowerCase();
                });

            const mergedInput = mergedCell
                .find('input[name$=".FieldValue"], textarea[name$=".FieldValue"]')
                .first();

            const genericName = (item.genericName || '').trim();

            if (mergedInput.length) {
                mergedInput.val(genericName).trigger('change');
            }

            // Update the visible second line under the parent field.
            let display = fieldCell.find('.merged-field-display');

            if (genericName) {
                if (!display.length) {
                    display = $('<div>', {
                        'class': 'merged-field-display text-muted small',
                        'data-merge-with': fieldCode
                    }).appendTo(fieldCell);
                }

                display.text(genericName).show();
            } else {
                display.text('').hide();
            }

            input.trigger('change');
        },
        appendToSelector
    );

    // If the medicine value is cleared, clear its linked generic name too.
    $(selector)
        .off('change.medicineGeneric')
        .on('change.medicineGeneric', function () {
            const input = $(this);
            const row = input.closest('tr');
            const fieldCode = input.attr('data-field-code');

            if (input.val()) {
                return;
            }

            const mergedCell = row.find('.merged-field-cell')
                .filter(function () {
                    return ($(this).attr('data-merge-with') || '')
                        .trim().toLowerCase() === (fieldCode || '')
                        .trim().toLowerCase();
                });

            mergedCell
                .find('input[name$=".FieldValue"], textarea[name$=".FieldValue"]')
                .val('')
                .trigger('change');

            row.find('.merged-field-display')
                .filter(function () {
                    return ($(this).attr('data-merge-with') || '')
                        .trim().toLowerCase() === (fieldCode || '')
                        .trim().toLowerCase();
                })
                .text('')
                .hide();

            input.closest('.field-cell')
                .find('.field-data-id')
                .val('');
        });
}


function wireTemplateAutocomplete(selector, appendToSelector) {

    wireAutocomplete(

        selector,

        '/PrescriptionTemplate/SearchTemplates',

        function (item) {

            return {
                label: item.text,
                value: item.text,
                id: item.id,
                templateName: item.templateName
            };
        },

        function (item) {

            const selectedTemplateId = parseInt(item.id || "0");
            $("#ToBeSourceTemplateId").val(selectedTemplateId);

        },

        appendToSelector,

        function () {

            $("#ToBeSourceTemplateId").val(0);

        }
    );
}

function wireReportLookupAutocomplete(selector,
                                        lookupCode,
                                        onSelected,
                                        appendToSelector)
{
    wireAutocomplete(

        selector,

        '/Report/SearchLookup?lookupCode=' +
            encodeURIComponent(lookupCode),

        function (item) {

            return {
                label: item.text,
                value: item.text,
                id: item.value
            };
        },

        onSelected,

        appendToSelector,

        function () {

            $(selector + "_value").val("");

            clearResult();
            hideMessage();
        }
    );
}

function showMessage(message, type = "success") {

    const alert = $("#appMessage");

    alert
        .removeClass(
            "d-none alert-success alert-danger alert-warning alert-info"
        )
        .addClass(`alert-${type}`)
        .html(message)
        .show();

    setTimeout(function () {
        alert.fadeOut();
    }, 4000);
}

function formatDate(dateValue) {

    if (!dateValue)
        return "";

    const date = new Date(dateValue);

    const months = [
        "Jan", "Feb", "Mar", "Apr",
        "May", "Jun", "Jul", "Aug",
        "Sep", "Oct", "Nov", "Dec"
    ];

    return String(date.getDate()).padStart(2, '0')
        + "-"
        + months[date.getMonth()]
        + "-"
        + date.getFullYear();
}