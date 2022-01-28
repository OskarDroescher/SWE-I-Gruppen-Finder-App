function addParticipantsCopyToClipboardEventHandler() {
    var clipboardbutton = document.getElementById('clipboardbutton');
    clipboardbutton.addEventListener('click', function () { copyInviteToClipboard() });
}

function copyInviteToClipboard() {
    var invitetextfield = document.getElementById('invitetextfield');
    invitetextfield.select();
    document.execCommand('copy');
    document.getSelection().removeAllRanges()
}