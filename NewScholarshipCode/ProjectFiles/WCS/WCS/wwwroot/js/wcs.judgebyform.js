/* THESE FUNCTIONS ARE SPECIFIC TO JUDGEBYFORM */

function navigate() {
    window.location.href = document.getElementById("formlinks").value;
}

//Shows the apps for a specific student, hides the currently active one
function showStudentPack(studentId) {
    currentActive = $(".visiblepack");
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

    if (unratedCount == 0) {
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
            .attr("title", "You have rated this student's applications")
            .attr("src", elStatusIcon.attr("src").replace("orange", "green").replace("red", "green"));

        if (!showAll && row.find(".student-profile-icon").attr("src").includes("green"))
            row.addClass("student-list-row-hidden");
    }
    else if (ratedCount > 0)
        elStatusIcon.attr("src", elStatusIcon.attr("src").replace("red", "orange"));

    calcUnrated();
}