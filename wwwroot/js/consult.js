let rowIndex = 0;

function addRow() {

    let row = `
    <tr>
        <input type="hidden" name="PharmacologyAdvices[${rowIndex}].MedicineId"
                class="medicine-id"/>

        <td>
            <input name="PharmacologyAdvices[${rowIndex}].MedicineName"
                   class="form-control medicine-name"/>
        </td>

        <td>
            <input name="PharmacologyAdvices[${rowIndex}].Dosage"
                   class="form-control dosage"/>
        </td>

        <td>
            <input name="PharmacologyAdvices[${rowIndex}].Frequency"
                   class="form-control frequency"/>
        </td>

        <td>
            <input name="PharmacologyAdvices[${rowIndex}].Duration"
                   class="form-control duration"/>
        </td>

        <td>
            <input name="PharmacologyAdvices[${rowIndex}].Instructions"
                   class="form-control instructions"/>
        </td>

        <td>
            <button type="button"
                    class="btn btn-danger"
                    onclick="removeRow(this)">
                X
            </button>
        </td>
    </tr>`;

    $("#medicineTable tbody").append(row);

    rowIndex++;
}

function removeRow(btn) {
    $(btn).closest("tr").remove();
}

$(document).on("focus", ".medicine-name", function () {
    if ($(this).data("ac"))
        return;

    $(this).data("ac", true);

    $(this).autocomplete({
        minLength: 2,
        delay: 300,
        
        source: function (request, response) {

            console.log("Searching:", request.term);

            $.getJSON(
                "/Medicine/SearchMedicine",
                { term: request.term },
                response);
        },

        select: function(event, ui) {
            $(this)
                .closest("tr")
                .find(".medicine-id")
                .val(ui.item.id);
        }
    });

});




