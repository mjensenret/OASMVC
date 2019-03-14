// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

var connection = new signalR.HubConnectionBuilder()
    .withUrl("/oasTagHub")
    .configureLogging(signalR.LogLevel.Information)
    .build();

$(function () {

    var store;

    connection.on("displayVersion", function (version, networkNode) {
        $("#txtVersion").dxTextBox("instance").option('value', version);
    });

    connection.on("loadTags", function (tagList) {
        store = new DevExpress.data.CustomStore({
            key: "tagName",
            load: function () {

                return tagList;
            }
        });
        window.console.log(store);
        $("#tagList").dxDataGrid({
            dataSource: store,
            visible: true
        });
    });
    connection.on("updateTagValue", function (tagList) {
        window.console.log("in update tag values...")
        store.push([{ type: "update", key: tagList.tagName, data: tagList }]);
    });
    //connection.on("updateTagValues", function (tagListModel) {
    //    var store = $("#tagList").dxDataGrid("getDataSource").store();
    //    store.push([{ type: "update", key: tagListModel.tagName, data: tagListModel }]);
    //    window.console.log(test);
    //})

    connection.on("populateGroups", function (groups) {
        var serverGroups = $("#serverGroups").dxSelectBox("instance");
        serverGroups.option('visible', true);
        serverGroups.option('dataSource', groups);
    });

    connection.start();

});




function changeNode(data) {
    $.ajax({
        type: "POST",
        url: "/" + data.value,
        success: function (result) {
            window.console.log("success");
            
        }
    });

}

function changeGroup(data) {

    var networkNode = $("#networkNode").dxSelectBox("instance").option('value');

    $.ajax({
        type: "POST",
        url: "/" + data.value + "/" + networkNode,
        success: function (result) {
            //window.console.log("Changed group");
            //window.console.log(result);

        }
    })

    };


