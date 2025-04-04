"use strict"

// Tạo kết nối SignalR
var connection = new signalR.HubConnectionBuilder()
    .withUrl("/hub")
    .withAutomaticReconnect()
    .build();

// Xử lý khi nhận được thông báo
connection.on("ReceiveNotification", function (message) {
    console.log("Received notification:", message);

    // Tạo thông báo mới
    var notificationItem = document.createElement("a");
    notificationItem.classList.add("dropdown-item");
    notificationItem.href = "/UserTasks/Index";
    notificationItem.textContent = message;

    // Thêm thông báo vào dropdown
    var dropdownMenu = document.querySelector(".notification-dropdown .dropdown-menu");
    if (dropdownMenu) {
        // Thêm thông báo vào đầu danh sách
        dropdownMenu.insertBefore(notificationItem, dropdownMenu.firstChild);

        // Hiển thị badge với số lượng thông báo
        updateNotificationBadge();

        // Hiển thị dropdown
        var notificationBell = document.getElementById("notificationBell");
        if (notificationBell) {
            notificationBell.click();
        }
    }
});

// Cập nhật số lượng thông báo
function updateNotificationBadge() {
    const badge = document.querySelector(".notification-dropdown .badge");
    const count = document.querySelectorAll(".notification-dropdown .dropdown-item").length;
    if (badge) {
        badge.textContent = count;
        badge.style.display = count > 0 ? "inline" : "none";
    }
}

// Bắt đầu kết nối SignalR
connection.start()
    .then(function () {
        console.log("SignalR connection established.");
    })
    .catch(function (err) {
        console.error("SignalR connection error: ", err.toString());
        // Thử kết nối lại sau 5 giây nếu thất bại
        setTimeout(() => connection.start(), 5000);
    });

// Cập nhật badge khi tải trang
document.addEventListener("DOMContentLoaded", function () {
    updateNotificationBadge();
});