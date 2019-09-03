// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

var connection = new signalR.HubConnectionBuilder()
    .withUrl("/oasTagHub")
    .configureLogging(signalR.LogLevel.Information)
    .build();

$(function () {

    var store;
    var store2;

    connection.on("displayVersion", function (version, networkNode) {
        $("#txtVersion").dxTextBox("instance").option('value', version);
    });

    connection.on("loadTags", function (tagList) {
        store = new DevExpress.data.CustomStore({
            key: "tagId",
            load: function () {

                return tagList;
            }
        });
        $("#tagList").dxDataGrid({
            dataSource: store,
            visible: true
        });
    });

    connection.on("updateTagValue", function (tagList) {
        store.push([{ type: "update", key: tagList.tagId, data: tagList }]);
    });

    connection.on("populateGroups", function (groups) {
        var serverGroups = $("#serverGroups").dxSelectBox("instance");
        serverGroups.option('visible', true);
        serverGroups.option('dataSource', groups);
    });

    connection.on("loadChart", function (chartData) {
        window.console.log("inside loadchart....");
        store2 = new DevExpress.data.CustomStore({
            load: function () {
                return chartData;
            },
            key: "timeStamp"
        });
        $("#testChart").dxChart({
            dataSource: store2
        });
    });

    connection.on("insertChartData", function (data) {
        window.console.log("Made it in the insertChartData function");
        window.console.log("Store in insertChartData: " + store);
        store2.push([{ type: "insert", key: data.timeStamp, data: data }]);
    });

    connection.on("updateGaugeValue", function (value) {
        $("#testGauge").dxCircularGauge("instance").option("value", value);
    });

    connection.start().catch(function (err) {
        return console.error(err.toString());
    });

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
}

function OnChartLoad() {
    $.ajax({
        type: "POST",
        url: "/LoadChart",
        success: function () {

        }
    })
}

function TestFunction() {
    $.ajax({
        type: "POST",
        url: "/Test",
        success: function () {

        }
    })
}

