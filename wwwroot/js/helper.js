function wireAutocomplete(
    selector,
    url,
    mapper,
    onSelected,
    appendToSelector)
{
    $(selector).autocomplete({

        appendTo: appendToSelector || "body",

        minLength: 2,

        source: function (request, response) {

            $.getJSON(
                url,
                { term: request.term },
                function (data) {

                    response(
                        $.map(data, mapper)
                    );
                });
        },

        select: function (event, ui) {

            $(this).val(ui.item.value);

            if (onSelected)
                onSelected(ui.item);

            return false;
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