var uri_admin = "/api/ScholarshipsAdmin";
var newAward, sId, currentAward, scholarshipMap;
var editorPack;

//Add listeners for all the buttons!
$(document).ready(function () {
    //Listeners for the student ratings list to add/edit awards
    $.each($(".btn-student-award"), function (index, item) {
        if ($(item).hasClass("has-award"))
            $(item).attr("title", $(item).attr("title") + "\n*Click to Edit");
        else
            $(item).attr("title", $(item).attr("title") + "\n*Click to Award a Scholarship to this Student");

        $(item).addClass("admin"); //For clickable styles

        //Click to add/edit the award
        $(item).click(function () {
            initAwardEditor(parseInt($(this).attr("id")), $(this).hasClass("has-award"));
        });
    });

    //Edit an award
    $(".btn-award-edit").click(function () {
        if ($(this).hasClass("btn-award-edit"))
            initAwardEditor(parseInt($(this).attr("id")), true);
    });

    //Delete an award
    $(".btn-award-remove").click(function (event) {
        if ($(this).hasClass("btn-award-remove") && confirm("Delete this Scholarship Award?"))
            location.href = "/ScholarshipAwards/DeleteAward/?id=" + $(this).attr("id");
        event.stopPropagation();
    });
});

/***** SCHOLARSHIP AWARD ADD/EDIT *****/
//Start here
function initAwardEditor(studentId, editAward) {
    waitOn();

    currentAward = {
        Id: 0,
        StudentProfileId: 0,
        ScholarshipId: 0,
        AwardCycleId: 0,
        AwardMonies: null
    }

    if (studentId === null || studentId === undefined) {
        newAward = true;
        sId = 0;
    } else {
        sId = studentId;
        if (editAward === null || editAward === undefined) {
            newAward = true;
        } else {
            newAward = !editAward;
        }
    }

    getEditorPack();
}

//Callback from getEditorPack()
function fillDataMaps() {
    studentMap = new Map();
    scholarshipMap = new Map();

    for (var i = 0; i < editorPack.Students.length; i++) {
        if (!newAward && editorPack.Students[i].Id === sId && editorPack.Students[i].AwardedScholarship !== null) {
            currentAward = editorPack.Students[i].AwardedScholarship;
        }
    }

    for (var j = 0; j < editorPack.Scholarships.length; j++) {
        scholarshipMap.set(editorPack.Scholarships[j].Id, editorPack.Scholarships[j]);
    }

    awardEditorStart();
}

//Draw the UI
function awardEditorStart() {
    this.awardsouter = $("<div></div>")
        .appendTo(document.body)
        .addClass("award-editor-container");

    var awardsinner = $("<div></div>")
        .appendTo(this.awardsouter)
        .addClass("award-editor-inner");

    /* HEADER */
    var awardsheader = $("<div></div>")
        .appendTo(awardsinner)
        .addClass("award-editor-header");

    $("<span></span>")
        .appendTo(awardsheader)
        .addClass("award-editor-title")
        .html("Add/Edit Scholarship Award");

    var awardcontainer = $("<div></div>")
        .appendTo(awardsinner)
        .addClass("award-container");

    var studentcontainer = $("<div></div>")
        .appendTo(awardcontainer)
        .addClass("award-student-container");

    var scholarshipcontainer = $("<div></div>")
        .appendTo(awardcontainer)
        .addClass("award-scholarship-container");

    $("<span></span>")
        .appendTo(studentcontainer)
        .addClass("award-select-title")
        .html("Student:");

    var studentselect = $("<select></select>")
        .appendTo(studentcontainer)
        .addClass("award-select-list")
        .attr("id", "studentselect");;

    $("<span></span>")
        .appendTo(scholarshipcontainer)
        .addClass("award-select-title")
        .html("Scholarship:");

    this.scholarshipselect = $("<select></select>")
        .appendTo(scholarshipcontainer)
        .addClass("award-select-list")
        .attr("id", "scholarshipselect");

    this.fundscontainer = $("<div></div>")
        .appendTo(awardcontainer)
        .addClass("award-funds-container");

    btnsave = $("<button>Save</button>")
        .appendTo(awardcontainer)
        .addClass("btn btn-success list-btn-caption award-btn-save");

    btncancel = $("<button>Cancel</button>")
        .appendTo(awardcontainer)
        .addClass("btn btn-default list-btn-caption award-btn-cancel");

    btncancel.click(function () {
        if (confirm("Cancel adding/editing this scholarship award?"))
            $(".award-editor-container").remove();
    });

    btnsave.click(function () {
        saveAward();
    });

    fillStudentSelect(studentselect);
    fillScholarshipSelect(this.scholarshipselect);
    waitOff();
}

function fillStudentSelect(list) {
    for (var i = 0; i < editorPack.Students.length; i++) {
        student = editorPack.Students[i];

        //Skip students with an award already. Power!
        if (student.AwardedScholarship !== null && student.Id !== sId)
            continue;

        thing = $("<option></option>")
            .appendTo(list)
            .val(student.Id)
            .html(student.FullName);

        if (student.Id === sId)
            thing.prop("selected", true);
    }

    $(list).on({
        change: function () {
            currentAward.StudentProfileId = parseInt($(this).val());
        }
    })

    currentAward.StudentProfileId = parseInt($(list).val());
}

function fillScholarshipSelect(list) {
    for (var i = 0; i < editorPack.Scholarships.length; i++) {
        scholarship = editorPack.Scholarships[i];

        thing = $("<option></option>")
            .appendTo(list)
            .val(scholarship.Id)
            .html(scholarship.Name);

        if (scholarship.Id === currentAward.ScholarshipId)
            thing.prop("selected", true);
    }

    $(list).on({
        change: function () {
            currentAward.ScholarshipId = parseInt($(this).val());
            fillScholarshipFunds();
        }
    })

    currentAward.ScholarshipId = parseInt($(list).val());
    fillScholarshipFunds();
}

//Update funds when scholarship is selected
function fillScholarshipFunds() {
    this.fundscontainer.html("");
    fundstable = $("<table></table>")
        .appendTo(this.fundscontainer)
        .addClass("table table-list table-list-awardfunds");
    $("<caption></caption>")
        .appendTo(fundstable)
        .html("Award Money");
    $("<thead><th>Funding Source</th><th>Award</th><th></th><th>Total Funds</th></thead>")
        .appendTo(fundstable);
    fundsbody = $("<tbody></tbody>")
        .appendTo(fundstable);

    var funds = scholarshipMap.get(currentAward.ScholarshipId).ScholarshipFunds;

    for (var i = 0; i < funds.length; i++) {
        row = $("<tr></tr>")
            .appendTo(fundsbody)
            .addClass("fund-row")
            .attr("id", funds[i].Id);

        $("<td></td>")
            .appendTo(row)
            .html(funds[i].Name);

        inputarea = $("<td></td>")
            .appendTo(row)
            .html("$ ");

        $("<input type='number'/>")
            .appendTo(inputarea)
            .addClass("fund-input")
            .attr("max", funds[i].Amount)
            .attr("min", 0)
            .attr("id", funds[i].Id)
            .val(funds[i].Amount);

        $("<td></td>")
            .appendTo(row)
            .html("/");

        $("<td></td>")
            .appendTo(row)
            .html("$ " + funds[i].Amount);
    }

    if (currentAward.AwardMonies !== null && currentAward.AwardMonies !== undefined) {
        $(".fund-input").val(0);
        for (var i = 0; i < currentAward.AwardMonies.length; i++) {
            $("#" + currentAward.AwardMonies[i].ScholarshipFundId + ".fund-input")
                .val(currentAward.AwardMonies[i].Amount);
        }
    }
}

function saveAward() {
    waitOn("Saving");
    
    if (isNaN(currentAward.StudentProfileId)) {
        waitOff();
        alert("There is no student selected for this award")
        return;
    }

    if (currentAward.AwardMonies == null)
        currentAward.AwardMonies = [];

    var fundInputs = $(".fund-input");

    //Get all the funds!
    for (var i = 0; i < fundInputs.length; i++) {
        var amt = parseFloat($(fundInputs[i]).val());
        var id = parseInt($(fundInputs[i]).attr("id"));

        if (isNaN(amt))
            amt = 0;

        var fundItem = null;

        for (var k = 0; k < currentAward.AwardMonies.length; k++) {
            if (currentAward.AwardMonies[k].ScholarshipFundId === id)
                fundItem = currentAward.AwardMonies[k];
        }

        if (fundItem == null) {
            fundItem = {
                Id: 0,
                ScholarshipAwardId: currentAward.Id,
                ScholarshipFundId: id,
                Amount: amt
            }
            currentAward.AwardMonies.push(fundItem);
        } else {
            fundItem.Amount = amt;
        }
    }
    postAward(currentAward);
}

/***** AJAX FUNCTIONS *****/

function getEditorPack() {
    $.ajax({
        type: 'GET',
        url: uri_admin,
        error: function (jqXHR, textStatus, errorThrown) {
            console.error(jqXHR);
            alert("There was an error processing your request. Please check the console for details.");
            waitOff();
        },
        success: function (data) {
            editorPack = JSON.parse(data);
            fillDataMaps();
        }
    });
}

function postAward(award) {
    $.ajax({
        type: 'POST',
        url: uri_admin,
        accepts: 'text/plain',
        contentType: 'application/json',
        data: JSON.stringify(award),
        error: function (jqXHR, textStatus, errorThrown) {
            console.error(jqXHR);
            alert("There was an error processing your request. Please check the console for details.");
            waitOff();
        },
        success: function () {
            waitOn("Success");
            setTimeout(function () {
                location.reload();
            }, 1000);
        }
    });
}


function closeAwardCycle() {
    if (confirm("Close this Award Cycle and view the Report?")) {
        window.open("/Admin/CloseAwardCycle");
        location.reload();
    }
}
