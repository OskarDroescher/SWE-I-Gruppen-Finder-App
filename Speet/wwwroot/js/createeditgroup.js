var participantsslider = document.getElementById('participantsslider');
var participantslabel = document.getElementById('participantsslidervalue');
var meetupCalender = document.getElementById('datetimecalender');

//Update slider label
participantslabel.value = participantsslider.value;

participantsslider.oninput = function () {
    participantslabel.value = participantsslider.value;
}

participantslabel.oninput = function () {
    participantsslider.value = participantslabel.value;
}

//Validate slider label input
participantslabel.onblur = function () {
    if (participantslabel.value > participantsslider.max) {
        participantslabel.value = participantsslider.max;
    }

    if (participantslabel.value < participantsslider.min) {
        participantslabel.value = participantsslider.min;
    }
}