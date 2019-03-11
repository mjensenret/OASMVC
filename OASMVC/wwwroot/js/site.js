// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$(function () {

    var connection = new signalR.HubConnectionBuilder()
        .withUrl("/oasTagHub")
        .configureLogging(signalR.LogLevel.Information)
        .build();

    connection.on("displayVersion", function (version) {
        window.console.log("displayVersion: " + version);
        $("#txtVersion").dxTextBox("instance").option('value', version);
    });

    connection.on("updateTagList", function (tagList) {
        console.log(tagList);
        var store = new DevExpress.data.CustomStore({
            key: "tagName",
            load: function () {

                return tagList;
            }
        });
        $("#tagList").dxDataGrid({
            dataSource: store,
            visible: true
        });
        //store.push([{ type: "update", key: data.tagName, data: data }]);
    });

    connection.start();
    window.console.log("connection ran...");

})

function changeNode(data) {
    window.console.log("Data: " + data.value);
    $.ajax({
        type: "POST",
        url: "/" + data.value,
        success: function (result) {
            window.console.log("success");
        }
    });

}

