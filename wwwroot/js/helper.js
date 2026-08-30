function wireAutocomplete(
    selector,
    url,
    mapper,
    onSelected,
    appendToSelector,
    onCleared)
{
    $(selector).autocomplete({

        appendTo: appendToSelector || "body",

        minLength: 2,

        source: function (request, response) {

            $.getJSON(
                url,
                { term: request.term },
                function (data) {

                    const mapped = $.map(data, mapper);
                    response(mapped);                
                }
            );
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

            return {
                label: item.text,
                value: item.text,
                id: item.id,
                rate: item.rate
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
                scheduleType: item.scheduleType
            };
        },

        function (item) {

            const input = $(this);

            const row = input.closest("tr");

            // Set the selected medicine ID
            row.find(".field-data-id")
                .val(item.id);

            // Explicitly trigger change on the actual
            // FieldValue input so Consultation.js
            // marks the row as Modified.
            input.trigger("change");

        },

        appendToSelector
    );
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

