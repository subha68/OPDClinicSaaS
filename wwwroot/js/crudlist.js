$(function () {

    if ($(".crudListContainer").length === 0)
        return;

    const pageSize = parseInt($("#crudTable").data("pagesize")) || 10;

    let currentPage = 1;

    function refreshGrid() {

        let search = ($("#txtSearch").val() || "").toLowerCase();

        let rows =
            $("#crudBody tr");

        let filtered =
            rows.filter(function () {

                let text =
                    ($(this).data("search") || "")
                    .toString()
                    .toLowerCase();

                return text.includes(search);

            });

        rows.hide();

        let totalRows = filtered.length;

        let totalPages =
            Math.max(1,
                Math.ceil(totalRows / pageSize));

        if (currentPage > totalPages)
            currentPage = totalPages;

        let start =
            (currentPage - 1) * pageSize;

        filtered
            .slice(start, start + pageSize)
            .show();

        renderPager(totalPages);

        $("#recordInfo").html(
            `Showing ${totalRows} record(s)`
        );
    }

    function renderPager(totalPages) {

        let pager = $("#pager");

        pager.empty();

        pager.append(
            `<li class="page-item ${currentPage==1?'disabled':''}">
                <a class="page-link"
                href="#"
                data-page="${currentPage-1}">
                Prev
                </a>
            </li>`
        );

        for(let i=1;i<=totalPages;i++)
        {
            pager.append(
                `<li class="page-item ${i==currentPage?'active':''}">
                    <a class="page-link"
                    href="#"
                    data-page="${i}">
                    ${i}
                    </a>
                </li>`
            );
        }

        pager.append(
            `<li class="page-item ${currentPage==totalPages?'disabled':''}">
                <a class="page-link"
                href="#"
                data-page="${currentPage+1}">
                Next
                </a>
            </li>`
        );
    }

    $("#txtSearch").on("keyup", function () {

        currentPage = 1;

        refreshGrid();

    });

    $(document).on(
        "click",
        "#pager a",
        function(e){

            e.preventDefault();

            currentPage =
                parseInt($(this).data("page"));

            refreshGrid();
        });

    refreshGrid();

});
