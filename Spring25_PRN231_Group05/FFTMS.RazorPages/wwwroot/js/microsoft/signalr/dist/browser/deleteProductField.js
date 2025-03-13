"use strict";
var connection = new signalR.HubConnectionBuilder().withUrl("/productFieldHub").build();

console.log("SignalR connection deleteted.");


connection.on("ProductFieldDeleted", function (productFieldId) {

    console.log("Received delete notification for product FieldId:", productFieldId);
    const tbody = document.querySelector("#idTable tbody");
    if (!tbody) {
        console.error("Table body not found!");
        return;
    }

    const rowToRemove = tbody.querySelector(`tr[productfiel-id="${productFieldId}"]`);
    if (rowToRemove) {
        tbody.removeChild(rowToRemove);

        console.log(`Removed row for productfiel-id: ${productFieldId}`);
    } else {
        console.log(`Row not found for productfiel-id: ${productFieldId}`);
    }
});

connection.start()
    .then(() => console.log("SignalR connection delete started."))

    .catch(function (err) {
        console.error("Error starting SignalR connection:", err.toString());
    });