﻿"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/answers").build();

//Disable send button until connection is established
document.getElementById("sendButton").disabled = true;

connection.on("ReceiveMessage", function (user, message) {
    var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    var encodedMsg = user + " -- " + "\n" + msg;
    var li = document.createElement("div");
    li.textContent = encodedMsg;
    document.getElementById("messagesList").appendChild(li);
});

connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    var user = document.getElementById("username").value;
    var message = document.getElementById("textMessage").value;
    connection.invoke("SendMessage", user, message).catch(function (err) {
        //return console.error(err.toString());
    });
    event.preventDefault();
});