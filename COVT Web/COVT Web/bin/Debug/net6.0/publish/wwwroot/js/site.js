// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(document).ready(function () {
    function searchPatsienti() {
        var searchText = $("#searchInput").val();
        $.ajax({
            url: "/Patsienti/SearchPatsienti",
            type: "POST",
            data: { searchText: searchText },
            success: function (result) {
                $("#searchResults").html(result);
            }
        });
    }
});