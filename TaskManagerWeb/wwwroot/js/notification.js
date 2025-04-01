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

console.log("User ID: " + connection.connectionId);

connection.on("SendNotification", function (message) {
    console.log("User ID: " + connection.connectionId);
    Swal.fire({
        title: "<strong>HTML <u>"+ message +"</u></strong>",
        icon: "info",
        html: `
            You can use <b>bold text</b>,
    <a href="#" autofocus>links</a>,
            and other HTML tags
          `,
        showCloseButton: true,
        showCancelButton: true,
        focusConfirm: false,
        confirmButtonText: `
    <i class="fa fa-thumbs-up"></i> Great!
          `,
        confirmButtonAriaLabel: "Thumbs up, great!",
        cancelButtonText: `
    <i class="fa fa-thumbs-down"></i>
          `,
        cancelButtonAriaLabel: "Thumbs down"
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