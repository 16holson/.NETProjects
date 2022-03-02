var uri_scholarships = "/api/Scholarships";
var imagesFolder = "/images/form/";

function scholarshipEditorStart() {
    waitOn();

    this.editorcontainer = $("<div></div>")
        .appendTo(document.body)
        .addClass("scholarship-editor-container")
        .hide();

    var editorinner = $("<div></div>")
        .appendTo(this.editorcontainer)
        .addClass("scholarship-editor-inner");

    /* HEADER */
    var scholarshipsheader = $("<div></div>")
        .appendTo(editorinner)
        .addClass("scholarship-editor-header");

    $("<span></span>")
        .appendTo(scholarshipsheader)
        .addClass("scholarship-editor-title")
        .html("Edit Scholarships and Funding");

    $("<img/>")
        .appendTo(scholarshipsheader)
        .addClass("toolbar-image scholarship-btn-save")
        .attr("src", imagesFolder + "save.svg")
        .attr("title", "Save Changes");

    $("<img/>")
        .appendTo(scholarshipsheader)
        .addClass("toolbar-image scholarship-btn-exit")
        .attr("src", imagesFolder + "exit.svg")
        .attr("title", "Close Scholarship Editor");

    /* BODY */
    var scholarshipscontainer = $("<div></div>")
        .appendTo(editorinner)
        .addClass("scholarship-container");

    var btnscholarshipadd = $("<button>New Scholarship</button>")
        .appendTo(scholarshipscontainer)
        .addClass("list-btn-new scholarship-btn-add");

    var btnfundscopy = $("<button>Copy Funds From Previous Award Cycle</button>")
        .appendTo(scholarshipscontainer)
        .addClass("list-btn-new scholarship-btn-copy");

    /* ACTIVE SCHOLARSHIPS */
    this.activescholarships = $("<div></div>")
        .appendTo(scholarshipscontainer)
        .addClass("scholarships-active");

    $(".scholarship-btn-exit").click(function () {
        if (confirm("You will lose all unsaved changes.\n\nClose Scholarship Editor?"))
            scholarshipEditorEnd();
    });

    $(".scholarship-btn-save").click(function () {
        if (validateScholarships()) {
            saveScholarships();
        } else {
            alert("Save failed. Please check components for errors.");
        }
    });

    btnscholarshipadd.click(function () {
        ships = $(".scholarships-active").find(".scholarship")
        if (ships === null || ships === undefined || ships.length < 1) {
            $("<div></div>")
                .appendTo($(".scholarships-active"))
                .scholarship();
        } else {
            $("<div></div>")
                .insertBefore(ships[0])
                .scholarship();
        }
    });

    btnfundscopy.click(function () {
        if (confirm("This function will copy all funding sources from the previous award cycle to this one.\n\nContinue?")) {
            copyScholarshipFunds();
        }
    });

    getScholarships();

    this.editorcontainer.fadeIn();
}

function scholarshipEditorEnd() {
    waitOn();
    this.editorcontainer.fadeOut(500, function () {
        location.reload();
    });
}

//Get all scholarships from API
function getScholarships() {
    $.ajax({
        type: 'GET',
        url: uri_scholarships,
        error: function (jqXHR, textStatus, errorThrown) {
            console.error(jqXHR);
            alert("There was a problem getting the scholarship data. Check the console for details.");
            scholarshipEditorEnd();
        },
        success: function (data) {
            fillScholarships(JSON.parse(data));
        }
    });
}

//Save all the scholarships
function saveScholarships() {
    waitOn("Saving");
    var carrier = {
        Scholarships: []
    }

    things = $(".scholarship");
    for (var i = 0; i < things.length; i++) {
        item = $(things[i]).scholarship("getItem");
        if (item.Name.trim() === "" && item.ScholarshipFunds.length < 1)
            continue;

        carrier.Scholarships.push(item);
    }

    console.log(carrier);

    $.ajax({
        type: 'POST',
        url: uri_scholarships,
        accepts: 'text/plain',
        contentType: 'application/json',
        data: JSON.stringify(carrier),
        error: function (jqXHR, textStatus, errorThrown) {
            console.error(jqXHR);
            alert("There was a problem saving the data. Check the console for details.");
            waitOff();
        },
        success: function (data) {
            getScholarships();
        }
    });
}

//Copy funding from previous cycle
function copyScholarshipFunds() {
    waitOn("Making Copies")
    $.ajax({
        type: 'PATCH',
        url: uri_scholarships,
        error: function (jqXHR, textStatus, errorThrown) {
            console.error(jqXHR);
            alert("There was a problem copying the data. Check the console for details.");
            waitOff();
        },
        success: function (data) {
            saveScholarships();
        }
    });
}

//Draw the scholarships onto the container
function fillScholarships(data) {
    if (data === null || data === undefined)
        return;

    //empty the list
    this.activescholarships.html("");

    for (var i = 0; i < data.Scholarships.length; i++) {
        $("<div></div>")
            .appendTo(this.activescholarships)
            .scholarship(data.Scholarships[i]);
    }

    waitOff();
}

//Check scholarship validation (checks if funding amounts are numbers)
function validateScholarships() {
    things = $(".scholarship");
    allvalid = true;
    for (var i = 0; i < things.length; i++) {
        if (!$(things[i]).scholarship("validate"))
            allvalid = false;
    }
    return allvalid;
}

//Scholarship Widget
$.widget("wcs.scholarship", {
    options: {
        Id: 0,
        Name: "",
        Active: true,
        ScholarshipFunds: []
    },

    _create: function () {
        this.element.addClass("scholarship").hide();
        this.element.attr("id", "options.Id");

        if (this.options.ScholarshipFunds === null)
            this.options.ScholarshipFunds = [];

        this._initFields();
        this.element.slideDown();
    },

    _initFields: function () {
        //add name field
        titlediv = $("<div></div>")
            .appendTo(this.element)
            .addClass("scholarship-name-div");
        $("<span></span>")
            .appendTo(titlediv)
            .addClass("scholarship-title")
            .html("Scholarship: ");
        this.namefield = $("<input type='text'/>")
            .appendTo(titlediv)
            .addClass("scholarship-input-name")
            .val(this.options.Name);

        //add funds table
        fundstable = $("<table></table>")
            .appendTo(this.element)
            .addClass("table table-list table-list-funds");
        caption = $("<caption></caption>")
            .appendTo(fundstable)
            .html("Funding Sources");
        btnaddfund = $("<button>Add Funding</button>")
            .appendTo(caption)
            .addClass("list-btn-new list-btn-caption");
        $("<thead><th> Name</th><th> Amount</th><th></th></thead>")
            .appendTo(fundstable);
        this.fundsbody = $("<tbody></tbody>")
            .appendTo(fundstable);

        //add funds
        for (var i = 0; i < this.options.ScholarshipFunds.length; i++)
            this._addFundRow(this.options.ScholarshipFunds[i]);

        this._on(btnaddfund, {
            click: function () {
                this._addFundRow();
            }
        });
    },

    //Add row to funding table
    _addFundRow: function (scholarshipFund) {
        row = $("<tr></tr>")
            .appendTo(this.fundsbody)
            .addClass("scholarship-fund-row")
            .attr("id", 0);
        td = $("<td></td>")
            .appendTo(row);
        nameinput = $("<input type='text'/>")
            .appendTo(td)
            .addClass("fund-input-name");
        td = $("<td></td>")
            .appendTo(row);
        $("<span style='font-weight: bold; color: darkgreen'>$ </span>")
            .appendTo(td);
        amountinput = $("<input type='number'/>")
            .appendTo(td)
            .addClass("fund-input-amount");
        td = $("<td></td>")
            .appendTo(row);
        removebtn = $("<img title='Remove this Funding Source'/>")
            .appendTo(td)
            .addClass("form-btn form-btn-delete")
            .attr("src", imagesFolder + "trash.svg");

        if (scholarshipFund !== null && scholarshipFund !== undefined) {
            row.attr("id", scholarshipFund.Id);
            nameinput.val(scholarshipFund.Name);
            amountinput.val(scholarshipFund.Amount);
        }

        removebtn.click(function (event) {
            thing = $(event.target);

            while (!thing.hasClass("scholarship-fund-row"))
                thing = thing.parent();

            if (thing.attr("id") === '0')
                thing.remove();
            else {
                thing.find(".fund-input-name").val("remove");
                thing.find(".fund-input-amount").val("0");
                thing.hide();
            }
        });

        nameinput.focus();
    },

    validate: function () {
        valid = true;
        fundrows = $(this.element).find(".scholarship-fund-row");

        for (var i = 0; i < fundrows.length; i++) {
            item = $(fundrows[i]);
            amount = item.find(".fund-input-amount");
            if (isNaN(parseFloat(amount.val()))) {
                valid = false;
                amount.addClass("input-error");
            } else {
                amount.removeClass("input-error");
            }
        }

        return valid;
    },

    getItem: function () {
        this.options.Name = this.namefield.val();
        this.options.ScholarshipFunds = [];
        fundrows = $(this.element).find(".scholarship-fund-row");

        for (var i = 0; i < fundrows.length; i++) {
            item = $(fundrows[i]);
            amount = item.find(".fund-input-amount").val()
            newFund = {
                Id: item.attr("id"),
                Name: item.find(".fund-input-name").val(),
                Amount: parseFloat(amount)
            };
            if (isNaN(newFund.Amount))
                newFund.Amount = 0;

            this.options.ScholarshipFunds.push(newFund);
        }

        return this.options;
    },

    destroy: function () {
        $(this.element).fadeOut();
        setTimeout(function () {
            $("#scholarshipform").remove();
        }, 1000);
    }
});