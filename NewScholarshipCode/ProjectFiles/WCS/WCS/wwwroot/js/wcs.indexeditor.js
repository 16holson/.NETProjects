var uri_frontPage = "/api/Settings";

//Start SummerNote
$('.summernote').summernote({
    height: 400,
    focus: true
});

//Update the Home Page
$('#updateFrontPage').click(function () {
    waitOn("Saving");

    var markupText = $('#fpsetting').summernote('code');
    setting = {
        SettingName: "Front Page Message",
        SettingValue: markupText
    }

    $.ajax({
        url: uri_frontPage,
        type: 'POST',
        accepts: 'text/plain',
        contentType: 'application/json',
        data: JSON.stringify(setting),
        error: function (jqXHR, textStatus, errorThrown) {
            console.error(jqXHR);
            alert("There was an error trying to save.");
            waitOff();
        },
        success: function (data) {
            waitOn("Success");
            setTimeout(waitOff, 1500);
        }
    });
});

//Update the App Page
$('#updateAppPage').click(function () {
    waitOn("Saving");

    var markupText = $('#apsetting').summernote('code');
    setting = {
        SettingName: "HelpPageMessage",
        SettingValue: markupText
    }

    $.ajax({
        url: uri_frontPage,
        type: 'POST',
        accepts: 'text/plain',
        contentType: 'application/json',
        data: JSON.stringify(setting),
        error: function (jqXHR, textStatus, errorThrown) {
            console.error(jqXHR);
            alert("There was an error trying to save.");
            waitOff();
        },
        success: function (data) {
            waitOn("Success");
            setTimeout(waitOff, 1500);
        }
    });
});