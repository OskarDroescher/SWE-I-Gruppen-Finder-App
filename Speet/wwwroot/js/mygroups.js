//Confirm deletion and call controller
var table = document.getElementsByClassName('sportgroupstable')[0];
var deleteButtons = document.getElementsByClassName('dangerbutton');
for (const button of deleteButtons) {
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
            method: 'delete',
        }).done(function () { location.reload(true) });
    }
}
