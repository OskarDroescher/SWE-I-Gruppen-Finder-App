var participantsslider = document.getElementById('participantsslider');
var participantslabel = document.getElementById('participantsslidervalue');
var meetupCalender = document.getElementById('datetimecalender');

//Update slider label
participantslabel.value = participantsslider.value;

participantsslider.oninput = function () {
    participantslabel.value = participantsslider.value;
    validateForm();
}

participantslabel.oninput = function () {
    participantsslider.value = participantslabel.value;
}

//Validate slider label input
participantslabel.onblur = function () {
    if (parseInt(participantslabel.value, 10) > parseInt(participantsslider.max, 10)) {
        participantslabel.value = participantsslider.max;
    }

    if (parseInt(participantslabel.value, 10) < parseInt(participantsslider.min, 10)) {
        participantslabel.value = participantsslider.min;
    }

    participantslabel.value = participantslabel.value.replace(/^0+/, ''); //Removes leading zeros
    validateForm();
}

//Validate form input
document.addEventListener('DOMContentLoaded', validateForm);

var activitycheckboxes = document.getElementsByClassName('activitygrid')[0].querySelectorAll('input[type=checkbox]');
var checkedactivitiescount = Array.prototype.slice.call(activitycheckboxes).filter(checkbox => checkbox.checked == true).length;
activitycheckboxes.forEach(checkbox => checkbox.addEventListener('click', function () { updateCheckedActivitiesCounter(checkbox) }));

var groupnameinput = document.getElementById('groupname');
groupnameinput.addEventListener('input', validateForm );

var meetuptimeinput = document.getElementById('datetimecalender');
meetuptimeinput.addEventListener('input', validateForm );

var groupnameheading = document.getElementById('groupnameheading');
var activityheading = document.getElementById('activityheading');
var meetuptimeheading = document.getElementById('meetuptimeheading');
var maxparticipantsheading = document.getElementById('maxparticipantsheading');
var participantsheading = document.getElementById('participantsheading');
var submitbutton = document.getElementById('submitbutton');
function validateForm() {
    var forminvalid = false;

    if (groupnameinput.value != '') {
        groupnameheading.classList.remove('invalidtextcolor');
    } else {
        forminvalid = true;
        groupnameheading.classList.add('invalidtextcolor');
    }

    if (checkedactivitiescount > 0) {
        activityheading.classList.remove('invalidtextcolor');
    } else {
        forminvalid = true;
        activityheading.classList.add('invalidtextcolor');
    }

    if (meetuptimeinput.value != '') {
        meetuptimeheading.classList.remove('invalidtextcolor');
    } else {
        forminvalid = true;
        meetuptimeheading.classList.add('invalidtextcolor');
    }

    var participantentries = document.getElementsByClassName('participantentry');
    if (participantentries.length <= participantsslider.value) {
        maxparticipantsheading.classList.remove('invalidtextcolor');
        participantsheading.classList.remove('invalidtextcolor');
    } else {
        forminvalid = true;
        maxparticipantsheading.classList.add('invalidtextcolor');
        participantsheading.classList.add('invalidtextcolor');
    }

    submitbutton.disabled = forminvalid;
}

function updateCheckedActivitiesCounter(checkbox) {
    if (checkbox.checked) {
        checkedactivitiescount++;
    } else {
        checkedactivitiescount--;
    }
    validateForm();
}

//Remove participant
var removeparticipantbuttons = document.getElementsByClassName('removeparticipantbutton');
for (const button of removeparticipantbuttons) {
    button.addEventListener('click', function () { handleRemoveParticipantButtonClick(button) })
}

function handleRemoveParticipantButtonClick(button) {
    var participantEntry = button.closest(".participantentry");
    participantEntry.remove();
    validateForm();
}