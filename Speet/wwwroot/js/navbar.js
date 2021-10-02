var sidebar = document.getElementsByClassName('sidenavbar')[0];
var sidebarbutton = document.getElementById('sidebarbutton');
sidebarbutton.addEventListener('click', playSidebarAnimation);

var sidebarextended = false;
function playSidebarAnimation() {
    if (sidebarextended) {
        sidebar.classList.remove('extendsidebaranimation');
        void sidebar.offsetWidth;
        sidebar.classList.add('retractsidebaranimation');
    } else {
        sidebar.classList.remove('retractsidebaranimation');
        void sidebar.offsetWidth;
        sidebar.classList.add('extendsidebaranimation');
    }

    sidebarextended = !sidebarextended;
}