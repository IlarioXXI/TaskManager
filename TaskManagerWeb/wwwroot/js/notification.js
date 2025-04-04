"use strict";

var connection = new signalR.HubConnectionBuilder()
    .withUrl("/notificationHub")
    .build();

connection.start()
    .then(() => {
        console.log("User ID: " + connection.connectionId);
        console.log("Connected to NotificationHub")
    }
       )
    .catch(err => console.error(err.toString()));

function writeEl(items) {
    var output = "";
    for (var i = 0; i < items.length; i++) { 
        var obj = items[i]; 
        for (var key in obj) {
            if (key=="Title") {
                var value = obj[key];
                output += "<div><strong>" + key + ":</strong> " + value + "</div>"; 
            }
            if (key == "DueDate") {
                var value = obj[key];
                output += "<div><strong>" + key + ":</strong> " + new Date(value).toLocaleDateString() + "</div>";
            }
            
        }
    }
    return output; 
}

connection.on("SendNotification", function (task) {
    var items = JSON.parse(task);
    Swal.fire({
        title: "<strong>Tasks to do</strong>", 
        icon: "info",
        html: `
            <div style="font-size: 16px; line-height: 1.5;">
                ${writeEl(items)}
            </div>
            <a href="/" style="color: #007bff; text-decoration: underline;">Click here to see all tasks</a>
        `,
        showCloseButton: true,
        showCancelButton: false,
        focusConfirm: false,
        confirmButtonText: `
            <i class="fa fa-thumbs-up"></i> Great!
          `,
        customClass: {
            title: 'swal-title',
            html: 'swal-html',
            confirmButton: 'swal-confirm-button'
        },
        backdrop: true,
        toast: false,
        position: 'center',
        animation: true,
        timer: 20000
    });
});





//$(document).on("click", ".delete", function (e) {
//    e.preventDefault();

//    Swal.fire({
//        title: "<strong>HTML <u>example</u></strong>",
//        icon: "info",
//        html: `
//            You can use <b>bold text</b>,
//    <a href="#" autofocus>links</a>,
//            and other HTML tags
//          `,
//        showCloseButton: true,
//        showCancelButton: true,
//        focusConfirm: false,
//        confirmButtonText: `
//    <i class="fa fa-thumbs-up"></i> Great!
//          `,
//        confirmButtonAriaLabel: "Thumbs up, great!",
//        cancelButtonText: `
//    <i class="fa fa-thumbs-down"></i>
//          `,
//        cancelButtonAriaLabel: "Thumbs down"
//    });
//});