"use strict"
var connection = new signalR.HubConnectionBuilder().withUrl("/TaskHub").build();


console.log("SignalR connection created.");
connection.on("Notification-Task", function (item) {
    console.log(item);
    if (!item) {
        console.error("Item is null or undefined");
        return;
    }

}
connection.start().catch(function (err) {
    return console.error(err.toString());
});



"use strict"
var connection = new signalR.HubConnectionBuilder().withUrl("/TaskHub").build();

console.log("SignalR connection created.");

connection.on("Notification-Task", function (message) { 
    console.log(message);
    if (!message) {
        console.error("Message is null or undefined");
        return;
    }


    var notificationItem = document.createElement("a");
    notificationItem.classList.add("dropdown-item");
    notificationItem.href = "/UserTasks/Index"; 
    notificationItem.textContent = message; 

    // Thêm thông báo vào dropdown
    var dropdownMenu = document.querySelector(".notification-dropdown .dropdown-menu");
    if (dropdownMenu) {
        dropdownMenu.appendChild(notificationItem);
        // Hiển thị dropdown
        $(".notification-dropdown .dropdown-menu").dropdown("show");
        //cập nhật số lượng thông báo
        updateNotificationBadge();
    } else {
        console.error("Dropdown menu not found.");
    }
});

connection.start().catch(function (err) {
    return console.error(err.toString());
});

function updateNotificationBadge() {
    const badge = document.querySelector(".notification-dropdown .badge");
    const count = document.querySelectorAll(".notification-dropdown .dropdown-item").length;
    if (badge) {
        badge.textContent = count;
    }
}

// Gọi hàm cập nhật badge khi tải trang để hiển thị số lượng thông báo ban đầu
$(document).ready(function () {
    updateNotificationBadge();
});