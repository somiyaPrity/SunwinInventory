$(function () {

    $('.datatable').DataTable({

        responsive: true,

        pageLength: 10,

        autoWidth: false,

        language: {

            search: "Search :",

            lengthMenu: "Show _MENU_ entries"

        }

    });

});

$(document).on("click", ".btnDelete", function (e) {

    e.preventDefault();

    var form = $(this).closest("form");

    Swal.fire({

        title: "Delete Warehouse?",

        text: "This action cannot be undone.",

        icon: "warning",

        showCancelButton: true,

        confirmButtonColor: "#dc3545",

        cancelButtonColor: "#6c757d",

        confirmButtonText: "Delete"

    }).then((result) => {

        if (result.isConfirmed) {

            form.submit();

        }

    });

});