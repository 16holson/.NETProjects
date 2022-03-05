/* WCS FORM DESIGNER - DATA
 * For Weber State Univerity CS Scholarships
 * JS by Jeffrey hart
 */

var uri_forms = "../api/Forms"; //URI for forms API
var componentcount = 0; //Id's for new components
var showError = false;

//Default JSON data
//Returns default component options for new form components
function newComponent(type) {
    var title = "";
    var id = --componentcount;
    options = null;

    switch (type) {
        case "Requirement":
            return {
                Id: id,
                ProfileField: "",
                Operator: "",
                Value: ""
            }
            break;
        case "Rating":
            title = "";
            return {
                Id: id,
                Title: title
            };
            break;
        case "Number Box":
            title = ""
            break;
        case "Text Box":
            title = ""
            break;
        case "Text Area":
            title = "";
            options = { MinWords: 0 }
            break;
        case "Selection Box":
            title = "";
            options = {
                Items: [
                    "Sample Selection Item"
                ],
                ChangedCallback: function () { }
            };
            break;
    }

    return {
        Type: type,
        Id: id,
        Title: title,
        Required: true,
        Options: options
    };
}

//Types of components
var componentTypes = [
    "Number Box",
    "Selection Box",
    "Text Area",
    "Text Box"
]

//Data for building requirements
var requirementData = {
    ProfileFields: [
        {
            Name: "Current Academic Level",
            Type: "Text"
        },
        {
            Name: "Gender",
            Type: "Text"
        },
        {
            Name: "Marital Status",
            Type: "Text"
        },
        {
            Name: "Overall GPA",
            Type: "Number"
        },
        {
            Name: "Major GPA",
            Type: "Number"
        }
    ],

    TextOperators: [
        "Equals",
        "Not Equals",
        "Contains",
        "Not Contains"
    ],

    NumberOperators: [
        "Equals",
        "Greater Than",
        "Greater Than or Equal to",
        "Less Than",
        "Less Than or Equal to"
    ]
}

//API FUNCTIONS

function newForm(id, callback) {
    form = {
        Id: id,
        Title: "New Application",
        Description: "<strong>Optional</strong> description of the scholarship."
    };

    $.ajax({
        url: uri_forms,
        type: 'POST',
        accepts: 'text/plain',
        contentType: 'application/json',
        data: JSON.stringify(form),
        error: function (jqXHR, textStatus, errorThrown) {
            console.error(jqXHR);
            alert("There was an error. Check the console for details");
            allowNew(true);
        },
        success: function (data) {
            callback(JSON.parse(data));
        }
    });
}

function updateForm(form, callback) {
    $.ajax({
        url: uri_forms + '/' + form.Id,
        type: 'PUT',
        accepts: 'application/json',
        contentType: 'application/json',
        data: JSON.stringify(form),
        error: function (jqXHR, textStatus, errorThrown) {
            console.error(jqXHR);
            if ($(".formdesigner").hasClass("formdesigner"))
                $(".formdesigner").formdesigner("statusError", errorThrown);
            else
                alert("There was an error processing your request. Please check the console for details.");
        },
        success: function (result) {
            callback(true);
        }
    });
}

function getForm(id, callback) {
    $.ajax({
        type: 'GET',
        url: uri_forms + "/" + id,
        error: function (jqXHR, textStatus, errorThrown) {
            console.error(jqXHR);
            if ($(".formdesigner").hasClass("formdesigner"))
                $(".formdesigner").formdesigner("statusError", jqXHR);
            else
                alert("There was an error processing your request. Please check the console for details.");
        },
        success: function (data) {
            callback(JSON.parse(data));
        }
    });
}




