var uri_studentforms = "../api/StudentForms"; //URI for forms API
var saveBtn;
var pageRefresh = false;
var formWarning = false;
var fieldWarning = false;

$(document).ready(function () {
    $(".form-btn-save").on({
        click: function (event) {
            submitStudentForm(event);
        }
    });

    $(".form-input-textarea").on({
        keyup: function (event) {
            target = $(event.target);
            updateWordCount(target);
        }
    });

    $.each($(".form-input-textarea"), function (index, value) {
        $(value).trigger("keyup");
    });

    //Widgetize data
    $(".studentform-data").studentform();
    $(".form-field-data").field();
    $(".form-response-data").response();
});

//Generate the StudentForm Object
function submitStudentForm(event) {
    try {
        formValid = true;
        formWarning = false;
        formId = $(event.target).attr("id");
        form = $("#" + formId + ".form");
        saveBtn = form.find(".form-btn-save");
        studentForm = form.find($(".studentform-data")).studentform("getComponent");
        fields = form.find(".form-field");

        pageRefresh = (studentForm.Id === 0);

        //Fill responses
        studentForm.StudentResponses = [];
        for (var i = 0; i < fields.length; i++) {
            if (validateField(fields[i])) {
                var response = $(fields[i]).find(".form-response-data").response("getComponent");
                response.Response = $(fields[i]).find(".form-field-input").val().trim();
                studentForm.StudentResponses.push(response);

                if (!fieldWarning) {
                    $(fields[i]).find(".form-field-error").html("");
                    $(fields[i]).find(".form-field-input").removeClass("field-error");
                } else {
                    fieldWarning = false;
                }
            } else {
                formValid = false;
            }
        }

        if (!formValid) {
            alert("There were errors on your application. Please fix them and try saving again.");
            return;
        } else if (formWarning) {
            if (!confirm("Not meeting the suggested word count may affect your rating for this scholarship.\n\nContinue?")) {
                return;
            }
        }

        //Post that $4!7
        waitOn("Saving");
        postStudentForm(studentForm, submitSuccess);

    } catch (e) {
        console.error(e);
        alert("There was an error processing your request.\n\nPlease try again.");

        saveBtn.prop("disabled", false)
            .html("Save Application");
    }
}

//Callback for a successful submission
function submitSuccess() {
    waitOn("Success");
    setTimeout(function () {
        if (pageRefresh)
            location.reload();
        else
            waitOff();
    }, 1500);

    $.each($(".accordion-header"), function (index, value) {
        if (!$(value).hasClass("collapsed"))
            $(value).trigger("click");
    });
}

//Validate a required field or one with special options
function validateField(field) {
    var formfield = $(field).find(".form-field-data").field("getComponent");
    var fieldvalue = $(field).find(".form-field-input").val().trim();
    var errorvalue = $(field).find(".form-field-error");
    var input = $(field).find(".form-field-input");

    if (formfield.Required && (fieldvalue === "" || fieldvalue === "Make a Selection")) {
        errorvalue.html("*This field is required");
        input.addClass("field-error");
        return false;
    }

    //deal with validations
    var options = JSON.parse(formfield.Options);
    switch (formfield.Type) {
        case "Number Box":
            if (isNaN(parseFloat(fieldvalue))) {
                errorvalue.html("*This answer must be a number");
                input.addClass("field-error");
                return false;
            }

            if (options.NumberType === "Integer") {
                input.val(parseInt(input.val()));
            }
            else {
                input.val(parseFloat(input.val()));
            }

            break;
        case "Text Area":
            if (formfield.Required && (fieldvalue === "" || wordCount(fieldvalue) < options.MinWords)) {
                formWarning = true;
                fieldWarning = true;
                input.addClass("field-warning");
                errorvalue.html("*Not meeting the suggested word count may affect your rating for this scholarship.");
                return true;
            }

            break;
    }
    return true;
}

$.widget("wcs.studentform", {
    options: {
        Component: null
    },
    _create: function () {
        this.options.Component = JSON.parse($(this.element).val());
        $(this.element).val("hidden");
    },
    getComponent: function () {
        return this.options.Component;
    }
});

$.widget("wcs.field", {
    options: {
        Component: null
    },
    _create: function () {
        this.options.Component = JSON.parse($(this.element).val());
        $(this.element).val("hidden");
    },
    getComponent: function () {
        return this.options.Component;
    }
});

$.widget("wcs.response", {
    options: {
        Component: null
    },
    _create: function () {
        this.options.Component = JSON.parse($(this.element).val());
        $(this.element).val("hidden");
    },
    getComponent: function () {
        return this.options.Component;
    }
});

//Post the form responses
function postStudentForm(studentform, successCallback) {
    $.ajax({
        url: uri_studentforms,
        type: 'POST',
        accepts: 'text/plain',
        contentType: 'application/json',
        data: JSON.stringify(studentform),
        error: function (jqXHR, textStatus, errorThrown) {
            console.error(jqXHR + textStatus);
            waitOff();
            alert("There was an error processing your request.\n\nPlease try again.");
        },
        success: function () {
            successCallback();
        }
    });
}