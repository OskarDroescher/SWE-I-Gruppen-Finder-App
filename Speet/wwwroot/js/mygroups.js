//Confirm deletion and call controller
var table = document.getElementsByClassName('sportgroupstable')[0];
var deletebuttons = document.getElementsByClassName('dangerbutton');
for (const button of deletebuttons) {
    button.addEventListener('click', function () { handleDeleteButtonClick(button) })
}

function handleDeleteButtonClick(button) {
    var tableRow = button.closest(".tablerow");
    var groupName = tableRow.querySelector('#groupname').innerText;
    var groupId = tableRow.querySelector('#groupid').innerText;

    if (confirm('Soll die Gruppe "' + groupName + '" wirklich gelöscht werden?')) {
        $.ajax({
            url: '/SportGroup/DeleteGroup',
            data: { groupId: groupId },
        }).done(function () { location.reload(true) });
    }
}

//Confirm leaving and call controller
var joinbuttons = document.getElementsByClassName('leavebutton');
for (const button of joinbuttons) {
    button.addEventListener('click', function () { handleJoinButtonClick(button) })
}

function handleJoinButtonClick(button) {
    var tableRow = button.closest(".tablerow");
    var groupName = tableRow.querySelector('#groupname').innerText;
    var groupId = tableRow.querySelector('#groupid').innerText;

    if (confirm('Soll die Gruppe "' + groupName + '" wirklich verlassen werden?')) {
        $.ajax({
            url: '/SportGroup/LeaveGroup',
            data: { groupId: groupId },
        }).done(function () { location.reload(true) });
    }
}

