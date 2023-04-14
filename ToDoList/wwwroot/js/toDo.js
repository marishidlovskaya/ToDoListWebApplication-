$(document).ready(function () { 

    $('.wraper-totoitems').each(function () {
        var listId = $(this).find('#list-Id-').prop('value');

        var cookies = readCookie("showComplitedItems" + listId);
        var checkBoxHtml = ($(this).children().children().eq(1)).children().eq(1).children(":first").find('.form-check-input-items');
        var liHtml = ($(this).children().children().eq(1)).children(":first").children().children().children().find('.list-group-itemi');
        if (cookies == "true") {
            $(checkBoxHtml).prop('checked', true)
            $(($(this).children().children().eq(1)).children(":first").children().children().children().find('.list-group-itemi')).each(function () {
                $(this).css("display", "block");
            })
        }
        if (cookies == "false") {
            $(checkBoxHtml).prop('checked', false)
            $(($(this).children().children().eq(1)).children(":first").children().children().children().find('.list-group-itemi')).each(function () {
                if ($(this).find('.status-text').text() == "") {
                    $(this).css("display", "none");
                }

            })
        }
    })


})

/*-- ToDoList starts -- */


$('#CloseListBtn, #CloseListBtn2').on('click', function () {
    $('#fname').val("");
    $('#switchVisibilityofList').prop('checked', false);
})

$('.todoListArea').on('click', function () {
    window.location.href = "/ToDo/Index?id=" + $(this).attr("id");
})

$('#itemsDueToday').on('click', function () {
    window.location.href = "/ToDo/GetListsDueToday";
})

$('#allItems').on('click', function () {
    window.location.href = "/ToDo/GetPlannedLists";
})

$('.edit-item').on('click', function () {
    var id = $(this).attr("id");
    $.ajax({
        type: "get",
        url: "/Home/isToDoListHidden",
        data: { id: id },
        success: function (res) {
            $('#switchVisibilityofList').prop('checked', res);
        }
    })
    $('#exampleModal').modal('show');
    $('.list-id-hidden').attr("id", id);
    $('#fname').val($(this).prop('name'));
})

$('.delete-item').on('click', function () {
    var id = $(this).attr("id");
    $('#myModalDelete').modal('show');
    $('#confirmdeletion').on("click", function () {
        $('#myModalDelete').modal('hide');
        $('#' + id).parent().parent().remove();
        $.ajax({
            url: "/Home/DeleteToDoListById",
            type: "POST",
            async: false,
            dataType: "json",
            data: { id: id },
            success: function (data) {
                $('#fname').val("");
            }
        });
        location.reload();
    })
})

$('.copy-item').on('click', function () {
    var id = $(this).attr("id");
    $('#myModalCopy').modal('show');
    $('#confirmCopy').on("click", function () {
        $('#myModalCopy').modal('hide');
        $.ajax({
            type: "get",
            url: "/Home/CopyToDoList",
            data: { id: id },
            success: function (res) {
                $('.list-id-hidden').attr("id", res);
                $('#toDolists').load("/Home/GetToDoLists");
                location.reload();
            }
        })
    })
})

$('#switchVisibilityAllLists').on('change', function () {
    event.preventDefault();
    var status = $('#switchVisibilityAllLists').is(":checked");
    $.ajax({
        url: "/Home/ChangeToDoListHiddenStatus",
        type: "POST",
        dataType: "json",
        async: false,
        data: { status: status },
        success: function (data) {
        }
    });
    $('#toDolists').load("/Home/GetToDoLists");
})
    /*-- ToDoList ends -- */

function createCookie(name, value, days) {
    if (days) {
        var date = new Date();
        date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
        var expires = "; expires=" + date.toGMTString();
    }
    else var expires = "";

    document.cookie = name + "=" + value + expires + "; path=/";
}

function readCookie(name) {
    var nameEQ = name + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') c = c.substring(1, c.length);
        if (c.indexOf(nameEQ) == 0) return c.substring(nameEQ.length, c.length);
    }
    return null;
}

function eraseCookie(name) {
    createCookie(name, "", -1);
}


$('.form-check-input-items').on('change', function () {
    var status = $(this).prop('checked');
    var Html = $($(this).parent().parent().parent().children(":first").children().children().children());
    var toDoListId = Html.parent().parent().parent().parent().parent().find('.list-Id').prop("value");
    $(Html).find('li').each(function () {
        if (status == false && $(this).find('.status-text').text() == "") {
            $(this).css("display", "none");
            location.reload();
        }
        else {
            $(this).css("display", "block");
            location.reload();
        }
    })

    if (status == false) {
        createCookie("showComplitedItems" + toDoListId, "false", 365);
    } else {
        createCookie("showComplitedItems" + toDoListId, "true", 365);
    }
})
