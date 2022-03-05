/* WCS FORM DESIGNER
 * For Weber State Univerity CS Scholarships
 * JS by Jeffrey hart
 */

var imagesFolder = "/images/form/";
var imgSwitch_On = imagesFolder + "switch_off.svg";
var imgSwitch_Off = imagesFolder + "switch_off.svg";

/********** FORMS PAGE FUNCTIONS **********/

$(document).ready(function () {
    $(".form-btn-edit").on({
        click: function (event) {
            editForm($(event.target).attr("id"));
        }
    });

    $(".form-btn-delete").on({
        click: function (event) {
            if (confirm("Send this form to the recycle bin?")) {
                location.href = location.href.replace("Forms", "DeleteForm") + "/" + $(event.target).attr("id")
            }
        }
    });

    $(".form-btn-copy").on({
        click: function (event) {
            addForm($(event.target).attr("id"));
        }
    });
});

//Add a new form and open designer
function addForm(id) {
    if (id === null)
        id = 0;

    callback = function (data) {
        allowNew(true);
        editForm(data.Id);
    };

    allowNew(false);
    newForm(id, callback);
}


//Open designer on existing form
function editForm(id) {
    $("<div></div>").appendTo($(".formdesigner-container"))
        .formdesigner({ Id: id });
    $(".formdesigner-container").fadeIn(800);
}

//Toggle enabled on new form button
function allowNew(allow) {
    if (allow)
        $(".form-btn-new").prop("disabled", false);
    else
        $(".form-btn-new").prop("disabled", true);
}


/********** FORM DESIGNER WIDGET **********/

$.widget("wcs.formdesigner", {
    options: {
        Id: 0,
        Title: "",
        Description: "",
        Active: true,
        IncludeStudentRating: true,
        FormCriteria: [],
        FormFields: [],
        FormRequirements: []
    },

    _create: function () {
        this.element.addClass("formdesigner row");
        this.imgadd = imagesFolder + "add.svg";
        this.imgexit = imagesFolder + "exit.svg";
        this.imgsave = imagesFolder + "save.svg";
        this.imgcheck_on = imagesFolder + "check_on.svg";
        this.imgcheck_off = imagesFolder + "check_off.svg";
        this.loaded = false;

        /*     EDIT PANE    */
        this.formContainer = $("<div class='form-container col-sm-6'></div>")
            .appendTo(this.element);

        this._initStatusScreen();

        /*     TOOLBAR     */
        toolbar = $("<div></div>")
            .appendTo(this.formContainer)
            .addClass("form-toolbar");

        this.cmd_save = $("<div class='toolbar-icon-div toolbar-icon-save'></div>")
            .appendTo(toolbar);
        $("<img></img>")
            .appendTo(this.cmd_save)
            .attr("src", this.imgsave)
            .attr("title", "Save and Exit Form Designer")
            .addClass("toolbar-image");

        this.cmd_exit = $("<div class='toolbar-icon-div toolbar-icon-exit'></div>")
            .appendTo(toolbar);
        $("<img></img>")
            .appendTo(this.cmd_exit)
            .attr("src", this.imgexit)
            .attr("title", "Exit Form Designer")
            .addClass("toolbar-image");

        $("<p class='toolbar-title'>DESIGN SCHOLARSHIP FORM</p>")
            .appendTo(toolbar);

        this.elements = $("<div></div>")
            .appendTo(this.formContainer)
            .addClass("form-elements");

        titlediv = $("<div class='form-div'></div>")
            .appendTo(this.elements);
        $("<span class='form-label'>Scholarship Title:</span>")
            .appendTo(titlediv);
        this.titletext = $("<input type='text' class='form-textbox'/>")
            .appendTo(titlediv)
            .val(this.options.Title);

        descriptiondiv = $("<div class='form-div'></div>")
            .appendTo(this.elements);
        $("<span class='form-label'>Description:</span>")
            .appendTo(descriptiondiv);
        this.descriptiontext = $("<textarea class='form-textarea'></textarea>")
            .appendTo(descriptiondiv)
            .html(this.options.Description.replace(/<br\/>/g, "\n"));

        /*    REQUIREMENTS AREA     */
        componentdiv = $("<div class='form-div component-div'></div>")
            .appendTo(this.elements);

        $("<div class='component-area-label'></div>")
            .appendTo(componentdiv)
            .html("Requirements");

        this.requirementsdiv = $("<div class='component-area'></div>")
            .appendTo(componentdiv)
            .sortable({
                revert: true
            });

        adddiv = $("<div class='component-add-area'>")
            .appendTo(componentdiv);
        addbutton_requirements = $("<div class='add-button add-button-requirements'></div>")
            .appendTo(adddiv)
            .attr("title", "Add a Requirement");
        $("<img class='img-plus' src='" + this.imgadd + "'/>")
            .appendTo(addbutton_requirements);

        /*    RATINGS AREA     */
        componentdiv = $("<div class='form-div component-div'></div>")
            .appendTo(this.elements);

        componentlabel = $("<div class='component-area-label'></div>")
            .appendTo(componentdiv)
            .html("Rating Criteria");

        includestudentdiv = $("<div class='form-include-container'></div>")
            .appendTo(componentlabel)
            .attr("title", "If checked, the Past Performance rating will be included in rating calculations for this form");
        $("<span class='form-include-label'></span>")
            .appendTo(includestudentdiv)
            .html("Include Past Performance:");
        this.includestudent = $("<img/>")
            .appendTo(includestudentdiv)
            .addClass("form-include-input")
            .attr("src", this.imgcheck_off);

        this.ratingsdiv = $("<div class='component-area'></div>")
            .appendTo(componentdiv)
            .sortable({
                revert: true
            });

        adddiv = $("<div class='component-add-area'>")
            .appendTo(componentdiv);

        addbutton_ratings = $("<div class='add-button add-button-ratings'></div>")
            .appendTo(adddiv)
            .attr("title", "Add a Rating Criterion");

        $("<img class='img-plus' src='" + this.imgadd + "'/>")
            .appendTo(addbutton_ratings);

        /*    FIELDS AREA     */
        componentdiv = $("<div class='form-div component-div'></div>")
            .appendTo(this.elements);

        $("<div class='component-area-label'></div>")
            .appendTo(componentdiv)
            .html("Application Fields");

        this.fieldsdiv = $("<div class='component-area'></div>")
            .appendTo(componentdiv)
            .sortable({
                revert: true
            });

        adddiv = $("<div class='component-add-area'>")
            .appendTo(componentdiv);

        componentselect = $("<select class='component-type-select'></select>")
            .appendTo(adddiv)
            .attr("title", "Select a Field Type");

        addbutton_fields = $("<div class='add-button add-button-fields'></div>")
            .appendTo(adddiv)
            .attr("title", "Add a Form Field");
        $("<img class='img-plus' src='" + this.imgadd + "'/>")
            .appendTo(addbutton_fields);

        //Fill components
        for (var i = 0; i < componentTypes.length; i++) {
            $("<option></option>")
                .appendTo(componentselect)
                .html(componentTypes[i]);
        }

        this._drawPreviewPane();

        /*      FIELD LISTENERS     */
        this._on(this.titletext, {
            keyup: function () {
                this.options.Title = this.titletext.val();
                $(".form-title").html(this.options.Title);
            }
        });

        this._on($(".form-textarea"), {
            keyup: function () {
                this.options.Description = $(".form-textarea").val().replace(/&lt;/g, "<").replace(/&gt;/g, ">").replace(/\r?\n/g, '<br/>');
                this.previewdescription.html(this.options.Description);
            }
        });

        this._on(this.includestudent, {
            click: function (event) {
                chkbx = $(event.target);
                if (chkbx.hasClass("checked")) {
                    chkbx.removeClass("checked")
                        .attr("src", this.imgcheck_off);
                    this.options.IncludeStudentRating = false;
                } else {
                    chkbx.addClass("checked")
                        .attr("src", this.imgcheck_on);
                    this.options.IncludeStudentRating = true;
                }
            }
        });

        this._on(addbutton_requirements, {
            click: function () {
                this._addNewComponent("Requirement");
            }
        });

        this._on(addbutton_ratings, {
            click: function () {
                this._addNewComponent("Rating");
            }
        });

        this._on(addbutton_fields, {
            click: function () {
                this._addNewComponent(componentselect.val());
            }
        });

        this._on(this.ratingsdiv, {
            sortupdate: function () {
                this._reloadPreview_Ratings();
            }
        });

        this._on(this.fieldsdiv, {
            sortupdate: function () {
                this._reloadPreview_FormFields();
            }
        });

        this._initToolbarListeners();
        this._load();
    },

    /***** INITIALIZE THE STATUS SCREEN *****/
    _initStatusScreen: function () {
        /*     STATUS SCREEN    */
        this.statusScreen = $("<div></div>")
            .appendTo(this.formContainer)
            .addClass("form-status")
            .hide();

        /*     WAIT ANIMATION     */
        this.area_wait = $("<div></div>")
            .appendTo(this.statusScreen)
            .addClass("form-status-wait")
            .hide();

        $("<div class='wait-outer-ring'><div></div></div><div class='wait-inner-ring'><div></div></div>")
            .appendTo(this.area_wait);

        /*     WAIT MESSAGE     */
        this.wait_message = $("<div></div>")
            .appendTo(this.area_wait)
            .addClass("form-status-message")
            .html("Loading Form");

        /*     ERROR SECTION     */
        this.area_error = $("<div></div>")
            .appendTo(this.statusScreen)
            .addClass("form-status-error");
        $("<div>ERROR</div>")
            .appendTo(this.area_error)
            .addClass("form-status-error-header");

        this.error_message = $("<div></div>")
            .appendTo(this.area_error)
            .addClass("form-status-error-message")
            .html("There was some sort of error.");

        footer = $("<div></div>")
            .appendTo(this.area_error)
            .addClass("form-status-error-footer");
        this.btn_error_close = $("<button>Close</button>")
            .appendTo(footer)
            .addClass("form-status-error-close");

        this.statusOff();

        /*     CLOSE LISTENER     */
        this._on(this.btn_error_close, {
            click: function () {
                showError = false;
                this.statusOff();
            }
        });;
    },

    /***** DRAW THE PREVIEW PANE *****/
    _drawPreviewPane: function () {
        this.previewContainer = $("<div class='preview-container col-sm-6'></div>")
            .appendTo(this.element);
        $("<p class='form-header'>APPLICATION PREVIEW</p>")
            .appendTo(this.previewContainer);
        $("<p class='form-title'></p>")
            .appendTo(this.previewContainer)
            .html(this.options.Title);
        this.previewdescription = $("<p></p>")
            .appendTo(this.previewContainer)
            .addClass("form-description")
            .html(this.options.Description);

        ratingscontainer = $("<div class='area-ratings'></div>")
            .appendTo(this.previewContainer);

        $("<h5 class='preview-ratings-title'></h5>")
            .appendTo(ratingscontainer)
            .html("Application Ratings (not visible to applicants)<hr/>");

        this.previewratings = $("<hr/>").appendTo(ratingscontainer);

        this.previewfields = $("<div class='area-fields'></div>")
            .appendTo(this.previewContainer);
    },

    /***** INITIALIZE HANDLERS FOR TOOLBAR *****/
    _initToolbarListeners: function () {

        /*     SAVE     */
        this._on(this.cmd_save, {
            click: function () {
                this._save();
            }
        });

        /*     EXIT     */
        this._on(this.cmd_exit, {
            click: function () {
                if (confirm("You will lose all unsaved changes\nExit Form Designer?")) {
                    this.exit();
                }
            }
        });
    },

    /***** ADDS EXISTING COMPONENT TO THE VIEW *****/
    _addComponent: function (type, component) {
        var options = {
            Component: component,
            ChangedCallback: null,
            RemovedCallback: function (event, item) {
                $("#" + item.Type.replace(" ", "") + item.Id).addClass("delete").slideUp();
            }
        }

        switch (type) {
            case "Requirement":
                this._addComponent_Requirement(options)
                break;
            case "Rating":
                this._addComponent_Rating(options);
                break;
            case "Number Box":
                this._addComponent_NumberBox(options);
                break;
            case "Text Box":
                this._addComponent_TextBox(options);
                break;
            case "Text Area":
                this._addComponent_TextArea(options);
                break;
            case "Selection Box":
                this._addComponent_SelectionBox(options);
                break;
        }
    },

    /***** ADDS NEW COMPONENT TO THE VIEW *****/
    _addNewComponent: function (type) {
        var options = {
            Component: newComponent(type),
            ChangedCallback: null,
            RemovedCallback: function (event, item) {
                $("#" + item.Type.replace(" ", "") + item.Id).addClass("delete").slideUp();
            }
        }

        switch (type) {
            case "Requirement":
                this._addComponent_Requirement(options)
                break;
            case "Rating":
                this._addComponent_Rating(options);
                break;
            case "Number Box":
                this._addComponent_NumberBox(options);
                break;
            case "Text Box":
                this._addComponent_TextBox(options);
                break;
            case "Text Area":
                this._addComponent_TextArea(options);
                break;
            case "Selection Box":
                this._addComponent_SelectionBox(options);
                break;
        }
    },

    _addComponent_Requirement: function (options) {
        options.RemovedCallback = null;
        options.ChangedCallback = null;

        /*     COMPONENT     */
        $("<div></div>")
            .appendTo(this.requirementsdiv)
            .requirement(options);

        if (this.loaded)
            this.elements.animate({ scrollTop: this.elements.scrollTop() + $(".component-requirement").height() + 35 }, 500);
    },

    _addComponent_Rating: function (options) {
        options.ChangedCallback = function (event, item) {
            $(".preview-label-" + "Rating" + item.Id)
                .html(item.Title);
        };
        options.RemovedCallback = function (event, item) {
            $("#" + "Rating" + item.Id).addClass("delete").slideUp();
        }

        /*     COMPONENT     */
        $("<div></div>")
            .appendTo(this.ratingsdiv)
            .criterion(options);

        if (this.loaded)
            this.elements.animate({ scrollTop: this.elements.scrollTop() + $(".component-rating").height() + 35 }, 500);

        /*     PREVIEW ITEM     */
        this._addPreview_Rating(options.Component);
    },

    _addComponent_NumberBox: function (options) {
        options.ChangedCallback = function (event, item) {
            $(".preview-label-" + item.Id)
                .html(item.Title);
            if (item.Required)
                $(".preview-required-" + item.Id).show();
            else
                $(".preview-required-" + item.Id).hide();
        };

        options.Component.Options = { NumberType: "Double" }

        if (this.loaded)
            this._scrollElementsToBottom();

        /*     COMPONENT     */
        $("<div></div>")
            .appendTo(this.fieldsdiv)
            .formfield(options);

        /*     PREVIEW ITEM     */
        this._addPreview_FormField(options.Component);
    },

    _addComponent_TextBox: function (options) {
        options.ChangedCallback = function (event, item) {
            $(".preview-label-" + item.Id)
                .html(item.Title);
            if (item.Required)
                $(".preview-required-" + item.Id).show();
            else
                $(".preview-required-" + item.Id).hide();
        };

        /*     COMPONENT     */
        $("<div></div>")
            .appendTo(this.fieldsdiv)
            .formfield(options);

        if (this.loaded)
            this._scrollElementsToBottom();

        /*     PREVIEW ITEM     */
        this._addPreview_FormField(options.Component);
    },

    _addComponent_TextArea: function (options) {
        options.ChangedCallback = function (event, item) {
            $(".preview-label-" + item.Id)
                .html(item.Title);
            if (item.Required)
                $(".preview-required-" + item.Id).show();
            else
                $(".preview-required-" + item.Id).hide();
        };

        /*     COMPONENT     */
        $("<div></div>")
            .appendTo(this.fieldsdiv)
            .formfield(options);

        if (this.loaded)
            this._scrollElementsToBottom();

        /*     PREVIEW ITEM     */
        this._addPreview_FormField(options.Component);
    },

    _addComponent_SelectionBox: function (options) {
        options.ChangedCallback = function (event, item) {
            $(".preview-label-" + item.Id)
                .html(item.Title);

            if (item.Required)
                $(".preview-required-" + item.Id).show();
            else
                $(".preview-required-" + item.Id).hide();

            select = $(".preview-select-" + item.Id).html("");

            $("<option>Make a selection</option>")
                .appendTo(select);

            try {
                for (var i = 0; i < item.Options.Items.length; i++) {
                    $("<option></option>")
                        .appendTo(select)
                        .html(item.Options.Items[i]);
                }
            } catch (e) { }
        };

        /*     COMPONENT     */
        $("<div></div>")
            .appendTo(this.fieldsdiv)
            .formfield(options);

        if (this.loaded)
            this._scrollElementsToBottom();

        /*     PREVIEW ITEM     */
        this._addPreview_FormField(options.Component);
    },

    /***** ADD A RATING FIELD TO THE PREVIEW PANE *****/
    _addPreview_Rating: function (Component) {
        container = $("<div data-toggle='buttons'></div>")
            .insertBefore(this.previewratings)
            .addClass("preview-item preview-item-rating button-group")
            .attr("id", "Rating" + Component.Id)
            .hide();

        $("<div></div>")
            .appendTo(container)
            .addClass("preview-label label-rating preview-label-Rating" + Component.Id)
            .html(Component.Title);

        for (var i = 0; i < 6; i++) {
            $("<label class='btn btn-primary form-check-label button-radio'><input type='radio' name='" + Component.Id + "' value='" + i + "'>" + i + "</label>")
                .appendTo(container)
                .addClass("form-check-input");
        }

        container.slideDown();
    },

    /***** RELOAD THE PREVIEW FOR RATINGS AFTER A SORT *****/
    _reloadPreview_Ratings: function () {
        $(".preview-item-rating")
            .slideUp()
            .addClass("delete");

        setTimeout(function () {
            $(".delete").remove();
        }, 1000);

        components = $(".component-rating");

        for (var i = 0; i < components.length; i++) {
            this._addPreview_Rating($(components[i]).criterion("getComponent"));
        }
    },

    /***** ADD A FORM FIELD TO THE PREVIEW PANE *****/
    _addPreview_FormField: function (Component) {
        container = $("<div></div>")
            .appendTo(this.previewfields)
            .addClass("preview-item preview-item-formfield")
            .attr("id", Component.Type.replace(" ", "") + Component.Id)
            .hide();

        requiredlabel = $("<span> * </span>")
            .appendTo(container)
            .addClass("preview-required preview-required-" + Component.Id);
        $("<span></span>")
            .appendTo(container)
            .addClass("preview-label label-field preview-label-" + Component.Id)
            .html(Component.Title);

        if (!Component.Required)
            requiredlabel.hide();

        switch (Component.Type) {
            case "Number Box":
                $("<input type='text' style='text-align:center;width:100px;'/>")
                    .appendTo(container);
                break;
            case "Text Box":
                $("<input type='text' style='padding:1px 5px;'/>")
                    .appendTo(container);
                break;
            case "Text Area":
                textareadiv = $("<div></div>").appendTo(container);
                $("<textarea class='preview-text-area'></textarea>")
                    .appendTo(textareadiv)
                    .addClass("text-area-" + Component.Id)
                    .attr("id", Component.Id);
                wordcountdiv = $("<div class='preview-label label-wordcount'>Word Count: </div>")
                    .appendTo(container);
                $("<span class='word-count'>0</span>")
                    .appendTo(wordcountdiv)
                    .addClass("word-count-" + Component.Id);

                this._on($(".text-area-" + Component.Id), {
                    keyup: function (event) {
                        $(".word-count-" + $(event.target).attr("id"))
                            .html(wordCount($(event.target).val()));
                    }
                });
                break;
            case "Selection Box":
                selectbox = $("<select></select>")
                    .appendTo(container)
                    .addClass("preview-select preview-select-" + Component.Id);

                $("<option>Make a selection</option>")
                    .appendTo(selectbox);

                try {
                    for (var i = 0; i < Component.Options.Items.length; i++) {
                        $("<option></option>")
                            .appendTo(selectbox)
                            .html(Component.Options.Items[i]);
                    }
                } catch (e) { }
                break;
        }

        container.slideDown();
    },

    /***** RELOAD THE PREVIEW FOR FIELDS AFTER A SORT *****/
    _reloadPreview_FormFields: function () {
        $(".preview-item-formfield")
            .slideUp()
            .addClass("delete");

        setTimeout(function () {
            $(".delete").remove();
        }, 1000);

        components = $(".component-field");

        for (var i = 0; i < components.length; i++) {
            this._addPreview_FormField($(components[i]).formfield("getComponent"));
        }
    },

    /***** SCROLL ELEMENTS WINDOW TO THE BOTTOM *****/
    _scrollElementsToBottom: function () {
        setTimeout(function () {
            $(".form-elements").animate({ scrollTop: $(".form-elements").prop("scrollHeight") }, 500);
        }, 100);
    },

    /***** ENABLE STATUS SCREEN IN WAIT MODE *****/
    statusWait: function (message) {
        this.wait_message.html(message);
        this.area_error.hide(10);
        this.area_wait.show();
        this.statusScreen.fadeIn();
    },

    /***** ENABLE STATUS SCREEN IN ERROR MODE *****/
    statusError: function (message) {
        try {
            if (message === null || message.trim() === "")
                message = "There was a problem processing your request. Please check the log for details";
        } catch (e) {
            message = "There was a problem processing your request. Please check the log for details";
        }

        this.error_message.html(message);
        this.area_wait.hide(10);
        this.area_error.show();
        this.statusScreen.fadeIn();
        showError = true;
    },

    /***** DISABLE STATUS SCREEN *****/
    statusOff: function () {
        if (!showError) {
            this.statusScreen.fadeOut();
            this.area_error.hide();
            this.area_wait.hide();
        }
    },

    /***** VALIdATES THE FORM AND ALL COMPONENTS *****/
    _validate: function () {
        valid = true;
        reqs = $(".component-requirement");
        ratings = $(".component-rating");
        fields = $(".component-field");

        try {
            if (this.titletext.val().trim() === "") {
                this.titletext.addClass("component-error");
                return "Scholarship Title Cannot Be Empty";
            } else {
                this.titletext.removeClass("component-error");
            }

            for (var i = 0; i < reqs.length; i++) {
                if ($(reqs[i]).requirement("validate") !== true) {
                    valid = false;
                }
            }

            for (var i = 0; i < ratings.length; i++) {
                for (var i = 0; i < ratings.length; i++) {
                    if ($(ratings[i]).criterion("validate") !== true) {
                        valid = false;
                    }
                }
            }

            for (var i = 0; i < fields.length; i++) {
                for (var i = 0; i < fields.length; i++) {
                    if ($(fields[i]).formfield("validate") !== true) {
                        valid = false;
                    }
                }
            }

            if (!valid) {
                return "Save failed.<br/>There are some components in error state. Please fix these errors and try again."
            }
            else {
                return valid;
            }
        } catch (e) {
            console.error(e);
            return "There was a critical error.<br/>Please check the console for details.";
        }
    },

    /***** LOAD FORM DATA FROM DATABASE *****/
    _load: function () {
        this.statusWait("Loading Form Data");
        callback = function (data) {
            $(".formdesigner").formdesigner("loadCallback", data);
        }

        getForm(this.options.Id, callback);
    },

    loadCallback: function (data) {
        this.options = data;

        this.titletext.val(this.options.Title)
            .trigger("keyup");
        this.descriptiontext.html(this.options.Description.replace(/<br\/>/g, "\n"))
            .trigger("keyup");

        if (this.options.IncludeStudentRating) {
            this.includestudent.addClass("checked")
                .attr("src", this.imgcheck_on);
        }

        /*     ADD COMPONENTS     */
        try {
            if (this.options.FormRequirements !== null)
                for (var k = 0; k < this.options.FormRequirements.length; k++) {
                    this._addComponent("Requirement", this.options.FormRequirements[k]);
                }

            if (this.options.FormCriteria !== null)
                for (var j = 0; j < this.options.FormCriteria.length; j++) {
                    this._addComponent("Rating", this.options.FormCriteria[j]);
                }

            if (this.options.FormFields !== null)
                for (var m = 0; m < this.options.FormFields.length; m++) {
                    if (this.options.FormFields[m].Options !== null)
                        this.options.FormFields[m].Options = JSON.parse(this.options.FormFields[m].Options);

                    this._addComponent(this.options.FormFields[m].Type, this.options.FormFields[m]);
                }
        } catch (e) {
            this.statusError(e);
        }

        this.loaded = true;
        setTimeout(function () {
            $(".formdesigner").formdesigner("statusOff");
        }, 1000);
    },

    /***** SAVE FORM DATA TO DATABASE *****/
    _save: function () {
        this.statusWait("Saving Form Data");

        validation = this._validate();

        if (validation !== true) {
            this.statusError(validation)
        }
        else {
            this.options.FormRequirements = [];
            this.options.FormCriteria = [];
            this.options.FormFields = [];

            reqs = $(".component-requirement");
            criteria = $(".component-rating");
            fields = $(".component-field");
            removingFields = false;

            //TODO: Removal of components
            for (var i = 0; i < reqs.length; i++) {
                this.options.FormRequirements.push($(reqs[i]).requirement("getComponent"));

                if ($(reqs[i]).hasClass("form-remove"))
                    this.options.FormRequirements[i].Recycled = true;
            }
            for (var i = 0; i < criteria.length; i++) {
                this.options.FormCriteria.push($(criteria[i]).criterion("getComponent"));

                if ($(criteria[i]).hasClass("form-remove")) {
                    this.options.FormCriteria[i].Recycled = true;
                    removingFields = true;
                }
                else
                    this.options.FormCriteria[i].Order = i;
            }
            for (var i = 0; i < fields.length; i++) {
                this.options.FormFields.push($(fields[i]).formfield("getComponent"));
                if ($(fields[i]).hasClass("form-remove")) {
                    this.options.FormFields[i].Recycled = true;
                    removingFields = true;
                }
                else
                    this.options.FormFields[i].Order = i;

                this.options.FormFields[i].Options = JSON.stringify(this.options.FormFields[i].Options);
            }

            if (removingFields) {
                if (!confirm("Warning: You are removing existing components from this form. Removing these components will also remove all reated responses/ratings.\n\nContinue?"))
                    return;
            }

            callback = function (result) {
                $(".formdesigner").formdesigner("saveCallback");
            }

            updateForm(this.options, callback)
        }
    },

    saveCallback: function (result) {
        if (result !== true) {
            setTimeout(function () {
                $(".formdesigner").formdesigner("statusWait", "Save Successful");
            }, 1000);
            setTimeout(function () {
                $(".formdesigner").formdesigner("statusOff");
                $(".formdesigner").formdesigner("exit");
            }, 2500);
        } else {
            setTimeout(function () {
                $(".formdesigner").formdesigner("statusError", result);
            }, 1000);
        }
    },

    getOptions: function () {
        return this.options;
    },

    exit: function () {
        $(".formdesigner-container").fadeOut(1000, null, function () {
            $(".formdesigner").remove();
            location.reload();
        });
    }
});