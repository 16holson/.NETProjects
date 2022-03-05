var popupProfileMap = new Map();
var popupRatingsMap = new Map();

$(document).ready(function () {
    //Change displayed student list when form selection changes
    $(".studentratings-selector").change(displayRatingsForForm);
    displayRatingsForForm();
    $('.profileLink').on('click', displayStudentProfile);
    $('.ratingsLink').on('click', displayStudentRatings);

    //Remove Collapse
    $(".panel-heading").attr('data-toggle', "")

    //Gather popup windows and store in a map
    var popupWindows = $(".popup-profile");
    $(".popup-profile").remove();
    $.each(popupWindows, function (index, value) {
        $(value).show(1);
        popupProfileMap.set(value.id, value);
    });

    $.each($(".popup-ratings"), function (index, value) {
        $(value).show(1);
        popupRatingsMap.set(value.id, value);
        $(value).remove();
    });
});

function displayRatingsForForm() {
    //Hide current table
    $(".ratingstable").hide();
    //Get selected form and show it.
    var selectedForm = $(".studentratings-selector").val();
    $("#form-" + selectedForm).show();
}

function displayStudentProfile() {
    var studentId = $(this).attr('id');
    this.popupProfile = $("<div></div>")
        .appendTo(document.body)
        .addClass("popup-container")
        .on('click', closePopup)
        .hide();
    this.popupInner = $("<div></div>")
        .appendTo(this.popupProfile)
        .addClass("popup-inner")
        .click(function (event) {
            event.stopPropagation();
        });
    this.closeButton = $("<button>X</button>")
        .appendTo(this.popupInner)
        .addClass("popup-close-btn")
        .attr("title", "Close")
        .on('click', closePopup);
    this.profile = $("<div></div>")
        .appendTo(this.popupInner)
        .html(popupProfileMap.get("profile-" + studentId));

    this.popupProfile.fadeIn();
}

function closePopup() {
    $('.popup-container').remove();
}

function displayStudentRatings() {
    var studentId = $(this).attr('id');
    this.popupProfile = $("<div></div>")
        .appendTo(document.body)
        .addClass("popup-container")
        .on('click', closePopup)
        .hide();
    this.popupInner = $("<div></div>")
        .appendTo(this.popupProfile)
        .addClass("popup-inner")
        .click(function (event) {
            event.stopPropagation();
        });;
    this.closeButton = $("<button>X</button>")
        .appendTo(this.popupInner)
        .addClass("popup-close-btn")
        .attr("title", "Close")
        .on('click', closePopup);
    this.ratings = $("<div></div>")
        .appendTo(this.popupInner)
        .html(popupRatingsMap.get(studentId));

    this.popupProfile.fadeIn();
}