//Show popup on load
document.addEventListener('DOMContentLoaded', showJoinConfirmationPopup);

function showJoinConfirmationPopup() {
    var groupId = document.getElementById('joinpopupgroupid').value;

    $.ajax({
        url: '/SportGroup/GetConfirmJoinPartial',
        data: { groupId: groupId },
        async: false,
        dataType: 'html',
        success: function (result) {
            popuplayer.innerHTML = result;
            contentlayer.classList.add('blurred', 'ignoreclicks');
            popuplayer.classList.remove('hidden');

            var confirmjoinbutton = document.getElementById('confirmjoinbutton');
            confirmjoinbutton.addEventListener('click', function () { handleConfirmJoinButtonClick(); location.reload(true); });

            var canceljoinbutton = document.getElementById('canceljoinbutton');
            canceljoinbutton.addEventListener('click', closepopup);
        }
    });
}

//Handle click events of popup
function handleConfirmJoinButtonClick() {
    var groupId = document.getElementById('joinpopupgroupid').value;

    $.ajax({
        url: '/SportGroup/JoinGroup',
        data: { groupId: groupId },
        async: false
    }).done(function () { closepopup() });
}

