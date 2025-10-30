"use strict";

var connection = new signalR.HubConnectionBuilder()
    .withUrl("/notificationHub")
    .build();

connection.start()
    .then(() => {
        console.log("User ID:", connection.connectionId);
    })
    .catch(err => console.error(err.toString()));

// 🔹 Genera HTML per l'elenco di task
function writeEl(items) {
    return items.map(obj => {
        let html = "";
        if (obj.Title) {
            html += `<div><strong class="text-success">Titolo:</strong> ${obj.Title}</div>`;
        }
        if (obj.DueDate) {
            const date = new Date(obj.DueDate).toLocaleDateString();
            html += `<div><strong class="text-muted">Scadenza:</strong> ${date}</div>`;
        }
        return `<div class="border-bottom pb-2 mb-2">${html}</div>`;
    }).join("");
}

connection.on("SendNotification", function (task) {
    const items = JSON.parse(task);

    Swal.fire({
        title: `<h3 style="color:#1c7d6f;"><i class="bi bi-bell-fill me-2"></i>Nuovi Task da completare</h3>`,
        icon: "info",
        html: `
            <div style="text-align:left; font-size:15px; line-height:1.6;">
                ${writeEl(items)}
            </div>
            <div class="mt-3">
                <a href="/" style="color:#1c7d6f; text-decoration:none; font-weight:500;">
                    <i class="bi bi-list-task me-1"></i> Vai alla lista task
                </a>
            </div>
        `,
        showCloseButton: true,
        showConfirmButton: false,
        background: "#f9fdfc",
        backdrop: `rgba(28,125,111,0.4)`,
        customClass: {
            popup: 'swal2-rounded',
            htmlContainer: 'swal2-content-custom'
        },
        timer: 15000,
        timerProgressBar: true
    });
});