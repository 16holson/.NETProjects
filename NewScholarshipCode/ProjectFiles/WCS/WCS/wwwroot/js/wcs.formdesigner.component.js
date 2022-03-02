/* WCS FORM DESIGNER - COMPONENTS
 * For Weber State Univerity CS Scholarships
 * JS by Jeffrey hart
 */

imgError = imagesFolder + "form_error.svg";

/********** FORM FIELD WIdGET **********/

$.widget("wcs.formfield", {
    options: {
        Component: {
            Type: "Component Type",
            Id: 0,
            Order: 0,
            Title: "Component Title",
            Required: false,
            Options: null
        },

        ChangedCallback: function (event, item) { },
        RemovedCallback: function (event, item) { }
    },

    _create: function () {
        this.imgComponentBlue = imagesFolder + "file_blue.svg";
        this.imgComponentGreen = imagesFolder + "file_green.svg";
        this.imgComponentOrange = imagesFolder + "file_orange.svg";
        this.imgComponentPurple = imagesFolder + "file_purple.svg";
        this.currentIcon;

        this.element.addClass("component")
            .addClass("component-field")
            .addClass("component-" + this.options.Component.Type.replace(" ", ""))
            .attr("id", "component-" + this.options.Component.Id)
            .hide();

        removebutton = $("<img class='component-remove-button'/>")
            .appendTo(this.element)
            .prop("src", imagesFolder + "trash.svg")
            .attr("title", "Remove this component");

        this._on(removebutton, {
            click: function (event) {
                this._destroy(event);
            }
        });


        /*     ACTIONS BASED ON TYPE     */
        switch (this.options.Component.Type) {
            case "Number Box":
                this._initNumberBox();
                break;
            case "Text Box":
                this._initTextbox();
                break;
            case "Text Area":
                this._initTextArea();
                break;
            case "Selection Box":
                this._initSelectBox();
                break;
        }

        //Check if required
        this.required.prop("checked", this.options.Component.Required);

        this.element.slideDown();
        this.input.focus();
    },

    /***** INITIALIZE NUMBER BOX *****/
    _initNumberBox: function () {
        this.currentIcon = this.imgComponentGreen;

        row = $("<tr></tr>")
            .appendTo($("<table></table>")
                .appendTo(this.element));
        section = $("<td style='vertical-align: middle;'></td>").appendTo(row);

        /*     COMPONENT ICON     */

        clabel = $("<div></div>")
            .appendTo(section)
            .addClass("component-type-label")
        this.icon = $("<img/>")
            .appendTo(clabel)
            .addClass("component-icon")
            .attr("src", this.currentIcon)
            .attr("title", "Field Type: " + this.options.Component.Type);
        $("<div class='component-icon-label'></div>")
            .appendTo(clabel)
            .html("NB");

        /*     COMPONENT OPTIONS     */

        section = $("<td></td>").appendTo(row);

        textboxdiv = $("<div></div>").appendTo(section);
        $("<span class='component-label'>Title: </span>")
            .appendTo(textboxdiv);
        this.input = $("<input type='text'/>")
            .appendTo(textboxdiv)
            .addClass("input-component-title")
            .addClass("input-" + this.options.Component.Id + "-title")
            .val(this.options.Component.Title);
        reqdiv = $("<div style='margin-top: 5px;'></div>").appendTo(section);
        $("<span class='component-label'>Required: </span>")
            .appendTo(reqdiv);
        this.required = $("<input type='checkbox'></input>")
            .appendTo(reqdiv)
            .addClass("input-component-required");

        $("<span class='spacer-20'></span>")
            .appendTo(reqdiv);
        $("<span class='component-label'>Type: </span>")
            .appendTo(reqdiv);
        this.validation = $("<select></select>")
            .appendTo(reqdiv);
        $("<option>Integer</option><option>Double</option>")
            .appendTo(this.validation);

        /*          PRESET VALUES          */

        try {
            if (this.options.Component.Options !== null)
                this.validation.val(this.options.Component.Options.NumberType)
            else
                this.options.Component.Options = { NumberType: "Integer" };
        } catch (e) {
            console.error(e);
        }

        /*          LISTENERS          */

        this._on(this.input, {
            keyup: function (event) {
                this.options.Component.Title = this.input.val();
                this._trigger("ChangedCallback", event, this.options.Component);
            }
        });

        this._on(this.required, {
            click: function (event) {
                this.options.Component.Required = this.required.prop("checked");
                this._trigger("ChangedCallback", event, this.options.Component);
            }
        });

        this._on(this.validation, {
            change: function (event) {
                this.options.Component.Options = {
                    NumberType: this.validation.val()
                }
            }
        });
    },

    /***** INITIALIZE SINGLE-LINE BOX *****/
    _initTextbox: function () {
        this.currentIcon = this.imgComponentBlue;

        row = $("<tr></tr>")
            .appendTo($("<table></table>")
                .appendTo(this.element));
        section = $("<td style='vertical-align: middle;'></td>").appendTo(row);

        /*     COMPONENT ICON     */

        clabel = $("<div></div>")
            .appendTo(section)
            .addClass("component-type-label")
        this.icon = $("<img/>")
            .appendTo(clabel)
            .addClass("component-icon")
            .attr("src", this.currentIcon)
            .attr("title", "Field Type: " + this.options.Component.Type);
        $("<div class='component-icon-label'></div>")
            .appendTo(clabel)
            .html("TB");

        /*     COMPONENT OPTIONS     */

        section = $("<td></td>").appendTo(row);

        textboxdiv = $("<div></div>").appendTo(section);
        $("<span class='component-label'>Title: </span>")
            .appendTo(textboxdiv);
        this.input = $("<input type='text'/>")
            .appendTo(textboxdiv)
            .addClass("input-component-title")
            .addClass("input-" + this.options.Component.Id + "-title")
            .val(this.options.Component.Title);
        reqdiv = $("<div></div>").appendTo(section);
        $("<span class='component-label'>Required: </span>")
            .appendTo(reqdiv);
        this.required = $("<input type='checkbox'></input>")
            .appendTo(reqdiv)
            .addClass("input-component-required");

        /*          LISTENERS          */

        this._on(this.input, {
            keyup: function (event) {
                this.options.Component.Title = this.input.val();
                this._trigger("ChangedCallback", event, this.options.Component);
            }
        });

        this._on(this.required, {
            click: function (event) {
                this.options.Component.Required = this.required.prop("checked");
                this._trigger("ChangedCallback", event, this.options.Component);
            }
        });
    },

    /***** INITIALIZE MULTI-LINE TEXT AREA *****/
    _initTextArea: function () {
        this.currentIcon = this.imgComponentOrange;

        row = $("<tr></tr>")
            .appendTo($("<table></table>")
                .appendTo(this.element));
        section = $("<td style='vertical-align: middle;'></td>").appendTo(row);

        /*     COMPONENT ICON     */

        clabel = $("<div></div>")
            .appendTo(section)
            .addClass("component-type-label")
        this.icon = $("<img/>")
            .appendTo(clabel)
            .addClass("component-icon")
            .attr("src", this.currentIcon)
            .attr("title", "Field Type: " + this.options.Component.Type);
        $("<div class='component-icon-label'></div>")
            .appendTo(clabel)
            .html("TA");

        /*     COMPONENT OPTIONS     */

        section = $("<td></td>").appendTo(row);

        textboxdiv = $("<div></div>").appendTo(section);
        $("<span class='component-label'>Title: </span>")
            .appendTo(textboxdiv);
        this.input = $("<input type='text'/>")
            .appendTo(textboxdiv)
            .addClass("input-component-title")
            .addClass("input-" + this.options.Component.Id + "-title")
            .val(this.options.Component.Title);
        reqdiv = $("<div class='component-reqdiv'></div>").appendTo(section);
        $("<span class='component-label'>Required: </span>")
            .appendTo(reqdiv);
        this.required = $("<input type='checkbox'></input>")
            .appendTo(reqdiv)
            .addClass("input-component-required");

        $("<span class='spacer-20'></span>")
            .appendTo(reqdiv);
        $("<span class='component-label'>MinWords: </span>")
            .appendTo(reqdiv);
        this.minwords = $("<input type='text'/>")
            .appendTo(reqdiv)
            .addClass("input-component-minwords");

        /*          PRESET VALUES          */

        try {
            if (this.options.Component.Options !== null)
                this.minwords.val(this.options.Component.Options.MinWords);
        } catch (e) {
            console.error(e);
        }

        /*          LISTENERS          */

        this._on(this.input, {
            keyup: function (event) {
                this.options.Component.Title = this.input.val();
                this._trigger("ChangedCallback", event, this.options.Component);
            }
        });

        this._on(this.required, {
            click: function (event) {
                this.options.Component.Required = this.required.prop("checked");
                this._trigger("ChangedCallback", event, this.options.Component);
            }
        });

        this._on(this.minwords, {
            keyup: function (event) {
                try {
                    val = parseInt(this.minwords.val());

                    if (isNaN(val))
                        this.minwords.addClass("input-error");
                    else {
                        this.options.Component.Options = { MinWords: val };
                        this.minwords.removeClass("input-error");
                    }
                } catch (e) {
                    this.minwords.addClass("input-error");
                }
            }
        });
    },

    /***** INITIALIZE SELECT BOX *****/
    _initSelectBox: function () {
        this.currentIcon = this.imgComponentPurple;

        row = $("<tr></tr>")
            .appendTo($("<table></table>")
                .appendTo(this.element));
        section = $("<td style='vertical-align: middle;'></td>").appendTo(row);

        /*     COMPONENT ICON     */

        clabel = $("<div></div>")
            .appendTo(section)
            .addClass("component-type-label")
        this.icon = $("<img/>")
            .appendTo(clabel)
            .addClass("component-icon")
            .attr("src", this.currentIcon)
            .attr("title", "Field Type: " + this.options.Component.Type);
        $("<div class='component-icon-label'></div>")
            .appendTo(clabel)
            .html("SB");

        /*     COMPONENT OPTIONS     */

        section = $("<td></td>").appendTo(row);

        textboxdiv = $("<div></div>").appendTo(section);
        $("<span class='component-label'>Title: </span>")
            .appendTo(textboxdiv);
        this.input = $("<input type='text'/>")
            .appendTo(textboxdiv)
            .addClass("input-component-title")
            .addClass("input-" + this.options.Component.Id + "-title")
            .val(this.options.Component.Title);
        reqdiv = $("<div></div>").appendTo(section);
        $("<span class='component-label'>Required: </span>")
            .appendTo(reqdiv);
        this.required = $("<input type='checkbox'></input>")
            .appendTo(reqdiv)
            .addClass("input-component-required");
        $("<span class='spacer-20'></span>").appendTo(reqdiv);
        this.btntoggle = $("<button>Show Options</button>")
            .appendTo(reqdiv)
            .addClass("btn-options-toggle");

        //Add option list
        options = {
            ChangedCallback: function () {
                $(".component-SelectionBox").formfield("updateSBOptions");
            },
            Items: this.options.Component.Options.Items
        };

        this.optionlist = $("<div></div>")
            .appendTo(this.element)
            .optionlist(options)
            .hide();

        /*          LISTENERS          */

        this._on(this.input, {
            keyup: function (event) {
                this.options.Component.Title = this.input.val();
                this._trigger("ChangedCallback", event, this.options.Component);
            }
        });

        this._on(this.required, {
            click: function (event) {
                this.options.Component.Required = this.required.prop("checked");
                this._trigger("ChangedCallback", event, this.options.Component);
            }
        });

        this._on(this.btntoggle, {
            click: function (event) {
                if ($(event.target).html() === "Show Options") {
                    this.optionlist.slideDown();
                    $(".form-elements").animate({ scrollTop: $(".form-elements").scrollTop() + 225 }, 500);
                    $(event.target).html("Hide Options");
                }
                else {
                    this.optionlist.slideUp();
                    $(".form-elements").animate({ scrollTop: $(".form-elements").scrollTop() - 225 }, 500);
                    $(event.target).html("Show Options");
                }
            }
        });

        this.updateSBOptions();
    },

    /***** UPDATE SELECTION BOX OPTIONS FROM OPTIONLIST *****/
    updateSBOptions: function () {
        newlist = $(this.optionlist).optionlist("getItems");

        try {
            if (newlist.length >= 0)
                this.options.Component.Options.Items = newlist;
        } catch (e) {
            console.error(e);
        }

        this._trigger("ChangedCallback", null, this.options.Component);
    },

    /***** PLACES THE COMPONENT IN AN ERROR STATE *****/
    _setErrorState: function (message) {
        this.element.addClass("component-error");

        this.icon.attr("src", imgError)
            .attr("title", "ERROR: " + message);
    },

    /***** REMOVES THE COMPONENT FROM ERROR STATE *****/
    _removeErrorState: function () {
        if (this.element.hasClass("component-error")) {
            this.element.removeClass("component-error");
            this.icon.attr("src", this.currentIcon)
                .attr("title", "Field Type: " + this.options.Component.Type);
        }
    },

    /***** VALIDATES THIS COMPONENT *****/
    validate: function () {
        msg = "";

        if (this.options.Component.Title.trim() === "")
            msg += "Title cannot be empty";

        if (msg !== "") {
            this._setErrorState(msg);
            return false;
        }
        else {
            this._removeErrorState();
            return true;
        }
    },

    getComponent: function () {
        return this.options.Component;
    },

    _destroy: function (event) {
        if (this.options.Component.Id < 0) {
            this.element.addClass("delete").slideUp();
            setTimeout(function () {
                $(".delete").remove();
            }, 1000);
        }
        else {
            this.element.addClass("form-remove")
                .slideUp();
        }
        this._trigger("RemovedCallback", event, this.options.Component);
    }
});

/********** OPTION LIST WIdGET **********/

$.widget("wcs.optionlist", {
    options: {
        Items: [
            "Apples",
            "Oranges",
            "Tacos",
        ],
        ChangedCallback: function () { }
    },

    _create: function () {
        this.element.addClass("optionlist");

        $("<div></div>").appendTo(this.element)
            .addClass("optionlist-label")
            .html("Available Options");

        this.list = $("<ul></ul>")
            .appendTo(this.element)
            .addClass("optionlist-list")
            .addClass("sortable");

        addarea = $("<div></div>")
            .appendTo(this.element)
            .addClass("optionlist-addarea");

        this.newname = $("<input type='text'/>")
            .appendTo(addarea)
            .addClass("optionlist-input-name");
        this.addbtn = $("<button>Add</button>")
            .appendTo(addarea)
            .addClass("optionlist-btn-add");

        /*     Preset Items     */

        try {
            for (var i = 0; i < this.options.Items.length; i++) {
                this._insertItem(this.options.Items[i]);
            }
        } catch (e) {
            console.error(e);
        }

        this.list.sortable({ revert: true });

        /*     Add Item Listener     */

        this._on(this.addbtn, {
            click: function (event) {
                if (this.newname.val().trim() !== "") {
                    this._insertItem(this.newname.val());
                    this.newname.val("");
                    this.list.scrollTop = this.list.scrollHeight;
                }
            }
        });

        this._on(this.newname, {
            keyup: function (event) {
                if (event.keyCode === 13 && this.newname.val().trim() !== "") {
                    this._insertItem(this.newname.val());
                    this.newname.val("");
                    this.list.scrollTop(this.list.prop("scrollHeight"));
                }
            }
        });

        this._on(this.list, {
            sortupdate: function (event) {
                this._trigger("ChangedCallback");
                event.stopPropagation();
            }
        });
    },

    /***** ADD AN ITEM TO THE OPTION LIST *****/
    _insertItem: function (value) {
        item = $("<li></li>").appendTo(this.list)
            .html($("<span class='optionlist-value'>" + value + "</span><span class='optionlist-remove' title='Remove This Option'>X</span>"))
            .addClass("optionlist-item")
            .addClass("ui-state-default");
        this._trigger("ChangedCallback");

        //Add remove listener
        this._on(item.find(".optionlist-remove"), {
            click: function (event) {
                $(event.target).parent().remove();
                this._trigger("ChangedCallback");
            }
        });
    },

    /***** GET AN ARRAY OF OPTION ITEMS *****/
    getItems: function () {
        retval = [];
        things = this.element.find(".optionlist-item");

        for (var i = 0; i < things.length; i++) {
            retval.push($(things[i]).find(".optionlist-value").html());
        }
        return retval;
    }
});

/********** RATING CRITERION WIdGET **********/

$.widget("wcs.criterion", {
    options: {
        Component: {
            Id: 0,
            Order: 0,
            Title: "Ratings Criterion"
        },

        ChangedCallback: function (event, item) { },
        RemovedCallback: function (event, item) { }
    },

    _create: function () {
        this.imgRating = imagesFolder + "rating.svg";

        this.element.addClass("component")
            .addClass("component-rating")
            .hide();

        removebutton = $("<img class='component-remove-button'/>")
            .appendTo(this.element)
            .prop("src", imagesFolder + "trash.svg")
            .attr("title", "Remove this component");

        this._on(removebutton, {
            click: function (event) {
                this._destroy(event);
            }
        });

        this._initRating();

        this.element.slideDown();
        this.input.focus();
    },

    /***** INITIALIZE RATINGS CRITERION *****/
    _initRating: function () {
        row = $("<tr></tr>")
            .appendTo($("<table></table>")
                .appendTo(this.element));
        section = $("<td style='vertical-align: middle;'></td>").appendTo(row);

        /*     COMPONENT ICON     */

        clabel = $("<div></div>")
            .appendTo(section)
            .addClass("component-type-label")
        this.icon = $("<img/>")
            .appendTo(clabel)
            .addClass("component-icon")
            .attr("src", this.imgRating)
            .attr("title", "Rating Criterion");

        /*     COMPONENT OPTIONS     */

        section = $("<td></td>").appendTo(row);

        $("<span class='component-label'>Title: </span>")
            .appendTo(section);
        this.input = $("<input type='text'/>")
            .appendTo(section)
            .addClass("input-component-title")
            .addClass("input-" + this.options.Component.Id + "-title")
            .val(this.options.Component.Title);

        this._on(this.input, {
            keyup: function (event) {
                this.options.Component.Title = this.input.val();
                this._trigger("ChangedCallback", event, this.options.Component);
            }
        });
    },

    /***** PLACES THE COMPONENT IN AN ERROR STATE *****/
    _setErrorState: function (message) {
        this.element.addClass("component-error");

        this.icon.attr("src", imgError)
            .attr("title", "ERROR: " + message);
    },

    /***** REMOVES THE COMPONENT FROM ERROR STATE *****/
    _removeErrorState: function () {
        if (this.element.hasClass("component-error")) {
            this.element.removeClass("component-error");
            this.icon.attr("src", this.imgRating)
                .attr("title", "Rating Criterion");
        }
    },

    /***** VALIdATES THIS COMPONENT *****/
    validate: function () {
        msg = "";

        if (this.options.Component.Title.trim() === "")
            msg += "Title cannot be empty";

        if (msg !== "") {
            this._setErrorState(msg);
            return false;
        }
        else {
            this._removeErrorState();
            return true;
        }
    },

    getComponent: function () {
        return this.options.Component;
    },

    _destroy: function (event) {
        if (this.options.Component.Id < 0) {
            this.element.addClass("delete").slideUp();
            setTimeout(function () {
                $(".delete").remove();
            }, 1000);
        }
        else {
            this.element.addClass("form-remove").slideUp();
        }
        this._trigger("RemovedCallback", event, this.options.Component);
    }
});


/********** REQUIREMENT WIdGET **********/

$.widget("wcs.requirement", {
    options: {
        Component: {
            Id: 0,
            ProfileField: null,
            Operator: null,
            Value: null
        }
    },

    _create: function () {
        this.imgRequirement = imagesFolder + "visibility.svg";
        this.fieldTypes;
        this._initFieldTypes();

        this.element.addClass("component")
            .addClass("component-requirement")
            .hide();

        removebutton = $("<img class='component-remove-button'/>")
            .appendTo(this.element)
            .prop("src", imagesFolder + "trash.svg")
            .attr("title", "Remove this component");

        this._on(removebutton, {
            click: function (event) {
                this._destroy(event);
            }
        });

        this._initRequirement();

        this.element.slideDown();
    },

    /***** MAP FIELD NAMES WITH FIELD TYPES  *****/
    _initFieldTypes: function () {
        this.fieldTypes = new Map();

        try {
            for (var i = 0; i < requirementData.ProfileFields.length; i++) {
                this.fieldTypes.set(requirementData.ProfileFields[i].Name, requirementData.ProfileFields[i].Type);
            }
        } catch (e) {
            console.error(e);
        }
    },

    /***** INITIALIZE REQUIREMENT COMPONENT *****/
    _initRequirement: function () {
        row = $("<tr></tr>")
            .appendTo($("<table></table>")
                .appendTo(this.element));
        section = $("<td style='vertical-align: middle;'></td>").appendTo(row);

        /*     COMPONENT ICON     */

        clabel = $("<div></div>")
            .appendTo(section)
            .addClass("component-type-label")
        this.icon = $("<img/>")
            .appendTo(clabel)
            .addClass("component-icon")
            .attr("src", this.imgRequirement)
            .attr("title", "Scholarship Requirement");

        /*     COMPONENT OPTIONS     */

        section = $("<td></td>").appendTo(row);

        table = $("<table></table>")
            .appendTo(section);

        row = $("<tr></tr>")
            .appendTo(table);

        $("<td style='padding-right:10px'></td>")
            .appendTo(row)
            .addClass("component-label")
            .html("Profile Field: ");

        segment = $("<td></td>")
            .appendTo(row);

        this.select_field = $("<select></select>")
            .appendTo(segment)
            .addClass("requirement-select");

        $("<option></option>")
            .appendTo(this.select_field)
            .attr("id", "temp")
            .html("Select . . .");

        row = $("<tr></tr>")
            .appendTo(table);

        $("<td></td>")
            .appendTo(row)
            .addClass("component-label")
            .html("Operator: ");

        segment = $("<td></td>")
            .appendTo(row);

        this.select_operator = $("<select></select>")
            .appendTo(segment)
            .addClass("requirement-select");

        $("<option></option>")
            .appendTo(this.select_operator)
            .html("Select a Profile Field");

        row = $("<tr></tr>")
            .appendTo(table);

        $("<td></td>")
            .appendTo(row)
            .addClass("component-label")
            .html("Value: ");

        segment = $("<td></td>")
            .appendTo(row);

        this.input_value = $("<input type='text'/>")
            .appendTo(segment)
            .addClass("requirement-input");

        //Fill ProfileFields
        try {
            for (var i = 0; i < requirementData.ProfileFields.length; i++) {
                $("<option></option>")
                    .appendTo(this.select_field)
                    .attr("name", requirementData.ProfileFields[i].Type)
                    .html(requirementData.ProfileFields[i].Name);
            }
        } catch (e) {
            console.error(e);
        }

        /*     LISTENERS     */

        this._on(this.select_field, {
            change: function () {
                this.options.Component.ProfileField = this.select_field.val();
                this._loadOperators();
                this.options.Component.Operator = this.select_operator.val();
            }
        });

        this._on(this.select_operator, {
            change: function () {
                this.options.Component.Operator = this.select_operator.val();
            }
        });

        this._on(this.input_value, {
            keyup: function () {
                this.options.Component.Value = this.input_value.val();
            }
        });

        if (this.options.Component.Id > 0)
            this._loadComponent();
    },

    /***** LOADS OPERATORS SELECT OPTIONS BASED ON SELECTED FIELD TYPE *****/
    _loadOperators: function () {
        this.select_operator.html("");
        $(this.select_field).find("#temp").remove();

        optype = this.fieldTypes.get(this.select_field.val()) + "Operators";

        operators = requirementData[optype];

        for (var i = 0; i < operators.length; i++) {
            $("<option></option>")
                .appendTo(this.select_operator)
                .html(operators[i]);
        }
    },

    /***** LOADS AN EXISTING COMPONENT *****/
    _loadComponent: function () {
        this.select_field.val(this.options.Component.ProfileField);
        this._loadOperators();
        this.select_operator.val(this.options.Component.Operator);
        this.input_value.val(this.options.Component.Value);
    },

    /***** PLACES THE COMPONENT IN AN ERROR STATE *****/
    _setErrorState: function (message) {
        this.element.addClass("component-error");

        this.icon.attr("src", imgError)
            .attr("title", "ERROR: " + message);
    },

    /***** REMOVES THE COMPONENT FROM ERROR STATE *****/
    _removeErrorState: function () {
        if (this.element.hasClass("component-error")) {
            this.element.removeClass("component-error");
            this.icon.attr("src", this.imgRequirement)
                .attr("title", "Scholarship Requirement");
        }
    },

    /***** VALIdATES THIS COMPONENT *****/
    validate: function () {
        msg = "";

        if (this.options.Component.ProfileField === "")
            msg += "Profile Field is not selected";

        if (msg !== "") {
            this._setErrorState(msg);
            return false;
        }
        else {
            this._removeErrorState();
            return true;
        }
    },

    /***** RETURNS THE COMPONENT IN JSON *****/
    getComponent: function () {
        return this.options.Component;
    },

    /***** REMOVE THE COMPONENT *****/
    _destroy: function (event) {
        if (this.options.Component.Id < 0) {
            this.element.addClass("delete").slideUp();
            setTimeout(function () {
                $(".delete").remove();
            }, 1000);
        }
        else {
            this.element.addClass("form-remove").slideUp();
        }
    }
});