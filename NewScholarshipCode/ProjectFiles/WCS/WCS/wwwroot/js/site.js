$(document).ready(function () {
    initFullScreenWait();
    onbeforeunload = waitOn;
});

// RETURNS WORD COUNT FOR A STRING
function wordCount(thing) {
    return thing.match(/\S+/g).length;
}

// UPDATES THE WORDCOUNT FOR A TEXTAREA FIELD
function updateWordCount(target) {
    if (target.val().trim() === "")
        count = 0;
    else
        count = wordCount(target.val());

    while (!target.hasClass("form-field-textarea")) {
        target = target.parent();
    }

    countText = target.find(".wordcount-count");
    countText.html(count.toString());

    minText = target.find($(".wordcount-min")).html();
    if (minText === "")
        return;

    min = parseInt(minText);


    if (count < min) {
        countText.addClass("wordcount-notmet");
    } else {
        countText.removeClass("wordcount-notmet");
    }
}

function initFullScreenWait() {
    /*     STATUS SCREEN    */
    this.statusScreen = $("<div></div>")
        .appendTo(document.body)
        .addClass("status-full")
        .hide();

    /*     WAIT ANIMATION     */
    var area_wait = $("<div></div>")
        .appendTo(this.statusScreen)
        .addClass("status-full-wait");

    $("<div class='wait-ring full-wait-outer-ring'><div></div></div>"
        + "<div class='wait-ring full-wait-outer-special-ring'> <div></div></div >"
        + "<div class='wait-ring full-wait-inner-ring'> <div></div></div>"
        + "<div class='wait-ring full-wait-inner-special-ring'><div></div></div>")
        .appendTo(area_wait);

    /*     WAIT MESSAGE     */
    $("<div></div>")
        .appendTo(area_wait)
        .addClass("status-full-message");
}

function waitOn(message) {
    if (message !== null && message !== undefined && typeof message == "string") {
        $(".status-full-message").html(message);
    } else {
        $(".status-full-message").html("Loading");
    }

    this.statusScreen.fadeIn();
}

function waitOff() {
    this.statusScreen.fadeOut();
}
