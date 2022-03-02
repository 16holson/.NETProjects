var uri_formratings = "/api/FormRatings";
var uri_studentratings = "/api/StudentRatings";
var showAll = true;

$(document).ready(function () {
    calcUnrated();

    //Make all form-rating widgets fly
    $.each($(".form-rating"), function (index, value) {
        $(value).formrating(JSON.parse($(value).attr("id")));
    });

    $.each($(".student-rating"), function (index, value) {
        $(value).studentrating(JSON.parse($(value).attr("id")));
    });

    //Update word counts
    $.each($(".form-input-textarea"), function (index, value) {
        updateWordCount($(value));
    });

    //Add listeners for the list selection
    $(".student-list-row").on({
        click: function (event) {
            thing = $(event.target);

            while (!thing.hasClass("student-list-row"))
                thing = $(thing).parent();

            if (thing.hasClass("selected"))
                return;

            $(".student-list-row.selected").removeClass("selected");
            $(thing).addClass("selected");
            showStudentPack($(thing).attr("id").replace("student", ""));
        }
    });

    //Add Listener for the filter button
    $("#filterStudents").on({
        click: function (event) {
            showAll = !showAll;
            if (showAll) {
                //Show All
                var rows = $(".student-list-row");
                if (rows != undefined) {
                    for (var i = 0; i < rows.length; i++) {
                        $(rows[i]).removeClass("student-list-row-hidden");
                    }
                }
            }
            else {
                //Change Text
                var rows = $(".student-list-row");
                //Show only incomplete
                if (rows != undefined) {
                    for (var i = 0; i < rows.length; i++) {
                        var row = $(rows[i]);
                        var rated = parseInt(row.find(".ratedcount").html())
                        if (row.find(".student-profile-icon").attr("src").includes("green"))
                            rated++;
                        var total = parseInt(row.find(".totalcount").html()) + 1;
                        if (rated == total) {
                            row.addClass("student-list-row-hidden");
                        }
                    }
                }
            }

        }
    });

    //Add listener for the list expander
    $(".judge-list-expander").click(function () {
        $(".table-container").toggleClass("table-collapsed");
    });

    //Select the first student in the list
    students = $(".student-list-row");
    if (students !== null && students.length > 0)
        $(students[0]).trigger("click");
});

function calcUnrated() {
    //Calculate totals like a boss
    var unRated = $("#allUnrated");
    var totalstudents = 0;
    var totalrated = 0;
    //var totalthings = $(".totalcount");
    //var ratedthings = $(".ratedcount");

    //for (var i = 0; i < totalthings.length; i++) {
    //    total += parseInt($(totalthings[i]).html());
    //}

    //for (var i = 0; i < ratedthings.length; i++) {
    //    totalrated += parseInt($(ratedthings[i]).html());
    //}

    var rows = $(".student-list-row");
    //Show only incomplete
    if (rows != undefined) {
        for (var i = 0; i < rows.length; i++) {
            var row = $(rows[i]);
            var rated = parseInt(row.find(".ratedcount").html())
            if (row.find(".student-profile-icon").attr("src").includes("green"))
                rated++;
            var totalthings = parseInt(row.find(".totalcount").html()) + 1;
            if (rated === totalthings) {
                totalrated++;
            }

            totalstudents++;
        }
    }

    $(unRated).html((totalstudents - totalrated) + " / " + totalstudents);
}

/***** FORM RATING WIDGET - TAKES STUDENTRATINGPACK *****/
$.widget("wcs.formrating", {
    options: {
        FormRating: {
            Id: 0,
            StudentFormId: 0,
            FormCriterionId: 0,
            JudgeId: 0,
            Rating: null
        },
        FormCriterion: {
            Id: 0,
            Title: "",
            Order: 0
        }
    },

    _create: function () {
        this.element.attr("id", "rating" + this.options.FormCriterion.Id);
        this.previousRating;

        container = $("<div data-toggle='buttons'></div>")
            .appendTo(this.element)
            .addClass("preview-item preview-item-rating button-group");

        $("<div></div>")
            .appendTo(container)
            .addClass("form-rating-label")
            .html(this.options.FormCriterion.Title);

        for (var i = 0; i < 6; i++) {
            label = $("<label class='form-rating-button btn btn-primary form-check-label'><input type='radio' name='" +
                this.options.FormRating.Id + "' value='" + i + "'>" + i + "</label>")
                .appendTo(container)
                .addClass("form-check-input")
                .attr("id", "ratinginput" + i);

            if (this.options.FormRating.Rating === i) {
                label.addClass("active");
                this.element.addClass("green");
            }

            this._on(label, {
                click: function (event) {
                    this._submitRating($(event.target).find("input").val());
                }
            });
        }
    },

    _submitRating: function (rating) {
        this.previousRating = this.options.FormRating.Rating;
        this.options.FormRating.Rating = rating;

        this.element.addClass("processing").removeClass("error");

        $.ajax({
            url: uri_formratings,
            type: 'POST',
            accepts: 'text/plain',
            contentType: 'application/json',
            data: JSON.stringify(this.options.FormRating),
            error: function (jqXHR, textStatus, errorThrown) {
                console.error(jqXHR);
                $(".form-rating.processing").formrating("ratingError");
            },
            success: function (data) {
                $(".form-rating.processing").formrating("ratingSuccess", data);
            }
        });
    },

    ratingSuccess: function (data) {
        this.element.removeClass("processing").addClass("green");
        updateRatingsCount(this.element);
    },

    ratingError: function () {
        this.element.addClass("error").removeClass("processing");
        $(this.element).find("#ratinginput" + this.options.FormRating.Rating + ".form-input-button")
            .removeClass("active").removeClass("focus");

        if (this.previousRating > -1) {
            this.options.FormRating.Rating = this.previousRating;
        }
    },

    getRating: function () {
        return this.options.FormRating.Rating;
    }
});

/***** STUDENT RATING WIDGET - TAKES STUDENTRATING *****/
$.widget("wcs.studentrating", {
    options: {
        Id: 0,
        StudentProfileId: 0,
        AwardCycleId: 0,
        JudgeId: "",
        Rating: -1
    },

    _create: function () {
        this.element.attr("id", "studentrating" + this.options.Id);
        this.previousRating;

        container = $("<div data-toggle='buttons'></div>")
            .appendTo(this.element)
            .addClass("preview-item preview-item-rating button-group");

        $("<div></div>")
            .appendTo(container)
            .addClass("form-rating-label")
            .html("Rate Past Performance");

        for (var i = 0; i < 6; i++) {
            label = $("<label class='form-rating-button btn btn-primary form-check-label'><input type='radio' name='" +
                this.options.Id + "' value='" + i + "'>" + i + "</label>")
                .appendTo(container)
                .addClass("form-check-input")
                .attr("id", "ratinginput" + i);

            if (this.options.Rating === i) {
                label.addClass("active");
                this.element.addClass("green");
            }

            this._on(label, {
                click: function (event) {
                    this._submitRating($(event.target).find("input").val());
                }
            });
        }
    },

    _submitRating: function (rating) {
        this.previousRating = this.options.Rating;
        this.options.Rating = rating;
        this.element.addClass("processing").removeClass("error");

        $.ajax({
            url: uri_studentratings,
            type: 'POST',
            accepts: 'text/plain',
            contentType: 'application/json',
            data: JSON.stringify(this.options),
            error: function (jqXHR, textStatus, errorThrown) {
                console.error(jqXHR);
                $(".student-rating.processing").studentrating("ratingError");
            },
            success: function (data) {
                $(".student-rating.processing").studentrating("ratingSuccess", data);
            }
        });
    },

    ratingSuccess: function (data) {
        this.element.removeClass("processing").addClass("green");

        //Update list icon
        var thing = this.element;
        while (!$(thing).hasClass("student-pack"))
            thing = $(thing).parent();

        ratedCount = $(thing).find(".form-rated").length;

        id = $(thing).attr("id").replace("pack", "");
        row = $("#" + id);
        elStatusIcon = row.find(".student-profile-icon");

        elStatusIcon.attr("title", "You have rated all applications from this student")
            .attr("src", elStatusIcon.attr("src").replace("orange", "green").replace("red", "green"));

        elTotalCount = row.find(".totalcount");
        totalCount = parseInt(elTotalCount.html());

        if (!showAll && ratedCount === totalCount) {
            row.addClass("student-list-row-hidden");
        }
    },

    ratingError: function () {
        this.element.addClass("error").removeClass("processing");
        $(this.element).find("#ratinginput" + this.options.Rating + ".form-input-button")
            .removeClass("active").removeClass("focus");

        if (this.previousRating > -1) {
            this.options.Rating = this.previousRating;
        }
    },

    getRating: function () {
        return this.options.Rating;
    }
});