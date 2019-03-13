// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$(function () {

    var connection = new signalR.HubConnectionBuilder()
        .withUrl("/oasTagHub")
        .configureLogging(signalR.LogLevel.Information)
        .build();

    connection.on("displayVersion", function (version, networkNode) {
        window.console.log("displayVersion: " + version);
        window.console.log("networkNode: " + networkNode);
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

    connection.on("populateGroups", function (groups) {
        console.log(groups);
        var serverGroups = $("#serverGroups").dxSelectBox("instance");
        serverGroups.option('visible', true);
        serverGroups.option('dataSource', groups);
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

function changeGroup(data) {
    window.console.log("Group Data: " + data.value);
    var networkNode = $("#networkNode").dxSelectBox("instance").option('value');
    window.console.log(networkNode);
    $.ajax({
        type: "POST",
        url: "/" + data.value + "/" + networkNode,
        success: function () {
            window.console.log("Changed group");
        }
    })
}

