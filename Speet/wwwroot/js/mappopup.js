var latitude;
var longitude;

function initiateMap() {
    var map = L.map('map');
    L.tileLayer('https://api.maptiler.com/maps/openstreetmap/{z}/{x}/{y}.jpg?key=qj5J53o4WLphIOc6xElB', {
        attribution: '<a href="https://www.maptiler.com/copyright/" target="_blank">&copy; MapTiler</a> <a href="https://www.openstreetmap.org/copyright" target="_blank">&copy; OpenStreetMap contributors</a>'
    }).addTo(map);

    latitude = document.getElementById('latitude').value;
    longitude = document.getElementById('longitude').value;

    var groupname = document.getElementById('groupname').value;
    var meetupDate = document.getElementById('meetupdate').value;
    var marker = L.marker([latitude, longitude]).addTo(map)
    var popup = marker.bindPopup(`<center><b>${groupname}</b><br>${meetupDate}<br><input class="primarybutton" type="button" onclick="openInGoogleMaps()" value="In Google Maps öffnen"/>`);
    map.setView([latitude, longitude], 15);
    popup.openPopup();
}

function openInGoogleMaps() {
    window.open(`https://www.google.de/maps/search/${latitude}, ${longitude}`, '_blank').focus();
}