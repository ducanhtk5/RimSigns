function makeButtons() {
    $allButtons = $(".button, button, input[type='submit']");

    $allButtons.each(function (i) {
        var $this = $(this);
        if ($this.hasClass("down-arrow")) {
            $this.button({
                icons: {
                    secondary: "ui-icon-triangle-1-s"
                }
            });
        } else {
            $this.button();
        };
    });
}

function completedAjaxRequestCustoer(obj) {
    var returnObj = eval('(' + obj.responseText + ')');

    if (returnObj.Status == "success") {
        if (returnObj.Message != null) {
            makeSuccessHighlightCustoer(returnObj.Message);
        }
        else if (returnObj.RedirectUrl != null) {
            window.location.href = returnObj.RedirectUrl;
        }
    } else {
        makeErrorHighlightCustoer(returnObj.Message);
    }

    return false;
}

function completedAjaxRequestCode(obj) {
    var returnObj = eval('(' + obj.responseText + ')');

    if (returnObj.Status == "success") {
        if (returnObj.Message != null) {
            makeSuccessHighlight(returnObj.Message);
        }
        else if (returnObj.RedirectUrl != null) {
            window.location.href = returnObj.RedirectUrl;
        }
    } else {
        makeErrorHighlight(returnObj.Message);
    }

    return false;
}

function makeErrorHighlightCustoer(messageStr) {
    var $highlight = $("<div style='margin-bottom:5px; width: inherit;'><p><span class='ui-icon ui-icon-alert' style='float: left; margin-right: .3em;'></span><strong>Alert: </strong>" + messageStr + "</p></div>");
    $highlight.addClass("ui-state-highlight ui-corner-all");
    $highlight.prependTo(".warning-area");
    $highlight.delay(4000).fadeOut(500);
}

function makeSuccessHighlightCustoer(messageStr) {
    var $highlight = $("<div style='margin-bottom:5px; width: inherit;'><p><span class='ui-icon ui-icon-info' style='float: left; margin-right: .3em;'></span><strong>Success: </strong>" + messageStr + "</p></div>");
    $highlight.addClass("ui-state-highlight ui-corner-all");
    $highlight.prependTo(".warning-area");
    $highlight.delay(4000).fadeOut(500);
}

function makeDialog(dialogTitle, dialogId, dialogWidth, href) {
    var $dialog = $("<div></div>");
    $dialog.appendTo("body")
            .addClass("dialog")
            .attr("id", dialogId);

    $dialog.dialog({
        title: dialogTitle,
        modal: true,
        autoOpen: true,
        width: dialogWidth,
        resizable: true,
        bgiframe: true,
        draggable: true,
        resizeStop: function(event, ui) {
              var y = $(event.target).height();
              repEditor.resize("99%", y - 10);
          },
          position: ['middle', 50],
          close: function (event, ui) {
              $('.dialog').dialog("destroy");
              $('.dialog').remove();
         },
        zIndex : 1200
    }).load(href);
}


function makeEventDialog(selector) {
    $(selector).click(function (e) {
        e.preventDefault();
        makeDialog($(this).attr("data-dialog-title"),
        $(this).attr("data-dialog-id"),
        800,
        this.href);
    });
}
function makeEventDialog2(selector) {
    $(selector).click(function (e) {
        e.preventDefault();
        makeDialog($(this).attr("data-dialog-title"),
        $(this).attr("data-dialog-id"),
        1000,
        this.href);
    });
}



$(document).ready(function () {
    //makeButtons();
    //makeEventDialog("a.addPopup"); 
});