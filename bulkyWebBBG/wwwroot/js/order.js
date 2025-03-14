var dataTable;


$(document).ready(function () {
    var url = window.location.search;
    if (url.includes("approved")) {
        loadDataTable("approved");
    } else { 
        if (url.includes("pending")) {
            loadDataTable("pending");
        } else { 
            if (url.includes("completed")) {
                loadDataTable("completed");
            }
            else {
                loadDataTable("");
            }
        }
    }
    loadDataTable();
});

function loadDataTable(){

$('#tblData').DataTable({
   "ajax": {url: '/admin/order/getall'},
   "columns": [

       { data: 'id', "width": "15%" },
       { data: 'name', "width": "15%" },
       { data: 'phoneNumber', "width": "15%" },
       { data: 'applicationUser.email', "width": "15%" },
       { data: 'orderStatus', "width": "15%" },
       { data: 'orderTotal', "width": "15%" },
       {
           data: 'id',
           "render": function (data) {
               return ` <div class="w-75 btn-group" role="group"> 
               <a href="/admin/order/details?orderId=${data}" class="btn btn-primary mx-2"> 
               <i class="bi bi-pencil-square"></i></a</div>`
           },
           "width": "15%"
       }


    ]
});

}
