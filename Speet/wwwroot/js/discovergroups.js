var maxdistanceslider = document.getElementById('maxdistanceslider');
var maxdistancelabel = document.getElementById('maxdistanceslidervalue');
var participantsslider = document.getElementById('participantsslider');
var participantslabel = document.getElementById('participantsslidervalue');

//Update slider labels
maxdistancelabel.value = maxdistanceslider.value;
participantslabel.value = participantsslider.value;

maxdistanceslider.oninput = function () {
    maxdistancelabel.value = maxdistanceslider.value;
}

maxdistancelabel.oninput = function () {
    maxdistanceslider.value = maxdistancelabel.value;
}

participantsslider.oninput = function () {
    participantslabel.value = participantsslider.value;
}

participantslabel.oninput = function () {
    participantsslider.value = participantslabel.value;
}

//Validate slider label input
maxdistancelabel.onblur = function () {
    if (maxdistancelabel.value > maxdistanceslider.max) {
        maxdistancelabel.value = maxdistanceslider.max;
    }

    if (maxdistancelabel.value < maxdistanceslider.min) {
        maxdistancelabel.value = maxdistanceslider.min;
    }
}

participantslabel.onblur = function () {
    if (participantslabel.value > participantsslider.max) {
        participantslabel.value = participantsslider.max;
    }

    if (participantslabel.value < participantsslider.min) {
        participantslabel.value = participantsslider.min;
    }
}

//Reset filter
var resetbutton = document.getElementById('resetbutton');
var reseticon = document.getElementById('resetbuttonicon');
resetbutton.addEventListener('click', resetFilterAndSubmit);
reseticon.addEventListener('click', resetFilterAndSubmit);

var activitycheckboxes = document.getElementsByClassName('activitygrid')[0].querySelectorAll('input[type=checkbox]');
var gendercheckboxes = document.getElementsByClassName('gendergrid')[0].querySelectorAll('input[type=checkbox]');
var mindatecalender = document.getElementById('mindate');
var maxdatecalender = document.getElementById('maxdate');
var submitbutton = document.getElementById('submitbutton');

function resetFilterAndSubmit() {
    activitycheckboxes.forEach(checkbox => checkbox.checked = false);
    gendercheckboxes.forEach(checkbox => checkbox.checked = false);
    maxdistanceslider.value = maxdistanceslider.max;
    participantsslider.value = participantsslider.max;
    maxdistancelabel.value = maxdistanceslider.value;
    participantslabel.value = participantsslider.value;
    mindatecalender.value = '';
    maxdatecalender.value = '';
    submitbutton.click();
}

//Confirm join and call controller
var joinbuttons = document.getElementsByClassName('joinbutton');
for (const button of joinbuttons) {
    button.addEventListener('click', function () { handleJoinButtonClick(button) })
}

function handleJoinButtonClick(button) {
    var tableRow = button.closest(".tablerow");
    var groupName = tableRow.querySelector('#groupname').innerText;
    var groupId = tableRow.querySelector('#groupid').innerText;

    if (confirm('Soll der Gruppe "' + groupName + '" wirklich beigetreten werden?')) {
        $.ajax({
            url: '/SportGroup/JoinGroup',
            data: { groupId: groupId },
        }).done(function () { location.reload(true) });
    }
}

//Show participants popup
var participantsbuttons = document.getElementsByClassName('participantsbutton');
for (const button of participantsbuttons) {
    button.addEventListener('click', function () { handleParticipantsButtonClick(button) })
}

function handleParticipantsButtonClick(button) {
    var contentlayer = document.getElementById('contentlayer');
    var popuplayer = document.getElementById('popuplayer');
    var tableRow = button.closest(".tablerow");
    var groupId = tableRow.querySelector('#groupid').innerText;

    $.ajax({
        url: '/SportGroup/GetParticipantsPartial',
        data: { groupId: groupId },
        async: false,
        dataType: 'html',
        success: function (result) {
            popuplayer.innerHTML = result;
            contentlayer.classList.add('blurred', 'ignoreclicks');
            popuplayer.classList.remove('hidden');
        }
    });
}