/* THESE FUNCTIONS ARE SPECIFIC TO JUDGEBYSTUDENT */

//Shows the apps for a specific student, hides the currently active one
function showStudentPack(studentId) {
    currentActive = $(".visiblepack");

    $.each(currentActive.find(".accordion-header"), function (index, value) {
        if (!$(value).hasClass("collapsed"))
            $(value).trigger("click");
    });

    currentActive.removeClass(".visiblepack").slideUp();

    $("#studentpack" + studentId).addClass("visiblepack").slideDown();
    $(".table-container").addClass("table-collapsed");
}

//Updates the Shield Icon and Subtitle for the form
//Used after rating a form
function updateRatingsCount(element) {
    while (!$(element).hasClass("form-container"))
        element = $(element).parent();
    ratings = element.find(".form-rating");
    unratedCount = 0;
    for (var i = 0; i < ratings.length; i++) {
        if ($(ratings[i]).formrating("getRating") < 0)
            ++unratedCount;
    }

    icon = element.find(".form-title-icon");
    rated = element.find(".form-title-rated");
    if (unratedCount > 0) {
        icon.attr("src", icon.attr("src").replace("red", "orange"));
        rated.html("You have partially rated this application");
    } else {
        icon.attr("src", icon.attr("src").replace("orange", "green").replace("red", "green"));
        rated.html("You have rated this application");
        element.addClass("form-rated");
    }

    //Update the Student List ratings count and status icon
    while (!$(element).hasClass("student-pack"))
        element = $(element).parent();

    ratedCount = $(element).find(".form-rated").length;

    id = $(element).attr("id").replace("pack", "");
    row = $("#" + id);
    elRatedCount = row.find(".ratedcount");
    elTotalCount = row.find(".totalcount");
    totalCount = parseInt(elTotalCount.html());
    elStatusIcon = row.find(".student-list-status-icon");

    elRatedCount.html(ratedCount);

    if (ratedCount === totalCount) {
        elStatusIcon
            .attr("title", "You have rated all applications from this student")
            .attr("src", elStatusIcon.attr("src").replace("orange", "green").replace("red", "green"));

        if (!showAll && row.find(".student-profile-icon").attr("src").includes("green"))
            row.addClass("student-list-row-hidden");
    }
    else if (ratedCount > 0)
        elStatusIcon.attr("src", elStatusIcon.attr("src").replace("red", "orange"));

    calcUnrated();
}