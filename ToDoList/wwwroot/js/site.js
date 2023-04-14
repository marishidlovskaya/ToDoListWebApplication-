$(document).ready(function () {
    /*-- ToDoLists starts -- */
    $('#SaveAddNewListBtn').on('click', function () {
        event.preventDefault();
        var object = {};
        var _id = parseInt($('.list-id-hidden').attr("id"));
        if (isNaN(_id)) _id = 0;
        object.Id = _id;
        object.Name = $('#fname').val();
        object.isListHidden = $('#switchVisibilityofList').prop('checked');
        $.ajax({
            type: "post",
            url: "/Home/AddNewToDoList",
            data: { todolist: JSON.stringify(object) },
            success: function (res) {
                $('#exampleModal').modal('hide');
                $('.list-id-hidden').attr("id", "");
                $('#toDolists').load("/Home/GetToDoLists");
                location.reload();
            }
        })
        
        $('#switchVisibilityofList').prop('checked', false);
        $('#fname').val("");
    })
    /*-- ToDoLists ends -- */




    /*-- ToDoItem starts -- */

    $('.addNewItemBtn').on('click', function () {
        var listId = $(this).children().attr("id");    
        $('#additemmodal').modal('show');
        $('#listIdForm').attr("value", listId);
    })

    $('.btn-done').on('click', function () {
        var taskId = $(this).attr("id");
        $(this).css("visibility", "hidden");
        $('#status-text-' + taskId).css("visibility", "hidden");
        var parentHtml = $($(this).parent().parent());
        var cldHtmlText = (($(parentHtml).children(":first")).children(":first")).children(":first");
        $(cldHtmlText).css("text-decoration", "line-through");

        $.ajax({
            url: "/ToDo/ChangeTaskStatus",
            type: "POST",
            dataType: "json",
            data: { taskId: taskId, status: 'Completed' },
            success: function (data) {
                location.reload();
            }
        });
    })

    $('.btn-delete').on('click', function () {
        var taskId = $(this).attr("id");
        var liHtml = $(this).parent().parent().parent().parent();      
        var html = $(liHtml.parent().find('.list-group-itemi'))
              
        if ($(html).length > 1) {
            liHtml.remove();
        }
        else {
            var HtmlToDelete = $($(this).parent().parent().parent().parent().parent().parent().parent().parent().parent().parent());
            HtmlToDelete.remove();
        }

        $.ajax({
            url: "/ToDo/DeleteTaskById",
            type: "POST",
            dataType: "json",
            data: { taskId: taskId },
            success: function (data) {
            }
        })
    })

    $('.addNewItemForm').submit(function (e) {
        e.preventDefault();
        const date = new Date(`${$('#date-start').val()}T${$('#time-start').val()}`);

        var item = new Object();
        item.Name = $('#inputItemtext').val();
        item.Description = $('#inputItemDescr').val();
        item.DueDate = new Date($('#inputItemDueDate').val());
        item.DateReminder = date.toISOString().replace(/\.\d{3}Z$/, '');
        item.Note = $('#inputItemNote').val();
        item.Status = $('#statuses').val();
        item.ToDoListId = $('#listIdForm').attr("value");
        console.log(item);
        var id = $('#task-id').val();

        if (id == "") {
            AddNewItem(item);
            $('#listIdForm').attr("value", "");
        }
        else {
            item.Id = id;
            UpdateItem(item)
        }
    })

    $('.btn-edit-item').on('click', function () {
        var id = $(this).attr("id");
        var listId = $($(this).parent().parent().parent().parent().parent().parent().parent().parent().parent().parent().children(":first").children().find('.list-Id')).attr("value");

        $('#additemmodal').modal('show');
        $('#listIdForm').attr("value", listId);

        $.ajax({
            url: "/api/ToDoItem/GetItem",
            type: "GET",
            contentType: 'application/json',
            data: { id: id },
            success: function (item) {
                MarkRedIfDateExpired(item.dueDate);
                

                if (item.dateReminder != null) {
                    const ReminderDateString = (item.dateReminder).substr(0, 10);
                    const ReminderTimeString = item.dateReminder.substr(11, 5);
                    $('#date-start').val(ReminderDateString);
                    $("#time-start").val(ReminderTimeString);

                    $('.reminder-button').css("display", "none");
                    $('#trash-img').css("display", "block");
                    $('#label-add-datetime').css("display", "block");
                    $('#time-start').css("display", "flex");
                    $('#date-start').css("display", "flex");
                }
                
                $('#additemmodal').modal('show');
                $('#task-id').val(id);
                $('#inputItemtext').val(item.name);
                $('#inputItemDescr').val(item.description);
                if (item.dueDate) {
                    const dueDateString = (item.dueDate).substr(0, 10);
                    $('#inputItemDueDate').val(dueDateString);
                }
                $('#inputItemNote').val(item.note);
                $('#statuses').val(item.status);                
            }
        });
    })

    function MarkRedIfDateExpired(date) {
        const dateTime = new Date(date);
        const now = new Date();
        const dateTimeDate = new Date(dateTime.getFullYear(), dateTime.getMonth(), dateTime.getDate());
        const nowDate = new Date(now.getFullYear(), now.getMonth(), now.getDate());
        if (dateTimeDate.getTime() < nowDate.getTime()) {

        }
    }

    function ResetInputs() {
        $('#inputItemtext').val("");
        $('#inputItemDescr').val("");
        $('#inputItemDueDate').val("");
        $('#inputItemNote').val("");
        $('#statuses').val("Not started");
        $('#task-id').val("");
    }

    function AddNewItem(item) {
        $.ajax({
            url: "/api/ToDoItem/AddItem",
            type: "POST",
            dataType: "text",
            contentType: 'application/json',
            data: JSON.stringify(item),
            success: function (data) {
                if (data == 'Ok') {
                    $('#allCurrentItems').load("/ToDo/GetToDoItems?id=" + item.ToDoListId);
                    location.reload();
                    $('#additemmodal').modal('hide');
                    ResetInputs();
                }
                else {
                    alert('error occurs during adding the task')
                }
            }
        });
    }

    function UpdateItem(item) {
        $.ajax({
            url: "/api/ToDoItem/UpdateItem",
            type: "PUT",
            dataType: "text",
            contentType: 'application/json',
            data: JSON.stringify(item),
            success: function (data) {
                if (data == 'Ok') {
                    $('#allCurrentItems').load("/ToDo/GetToDoItems?id=" + item.ToDoListId);
                    location.reload();
                    $('#additemmodal').modal('hide');
                    ResetInputs();
                }
                else {
                    alert('error occurs during adding the task')
                }
            }
        });
    }



    $('#CloseListBtn3').on('click', function () {
        ResetInputs();
    })

    $('.btn-add-reminder').on('click', function () {
        $('.reminder-button').css("display", "none");
        $('#trash-img').css("display", "block");
        $('#label-add-datetime').css("display", "block");
        $('#time-start').css("display", "flex");
        $('#date-start').css("display", "flex");        
    })

    $('.btn-delete-reminder').on('click', function () {
        $('.reminder-button').css("display", "flex");
        $('#trash-img').css("display", "none");
        $('#label-add-datetime').css("display", "none");
        $('#time-start').css("display", "none");
        $('#date-start').css("display", "none"); 
    })

    $('body').delegate('.close-reminder-btn', 'click', function () {
        var inReminderList = localStorage.getItem("items");
        var obj = [];
        obj = JSON.parse(inReminderList);
        var toDoListId = $(this).prop("id");
        $(this).parent().parent().parent().parent().remove();

        for (var i = 0; i < obj.length; i++) {
            if (obj[i].itemId == toDoListId) {
                obj.splice(i, 1);
            }
        }
        localStorage.setItem("items", JSON.stringify(obj));
    })

})












