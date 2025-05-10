function showLoginPopup(role) {
    document.getElementById('role').value = role;
    document.getElementById('login-popup').style.display = 'block';
    document.getElementById('popup-overlay').style.display = 'block';
}

function hideLoginPopup() {
    document.getElementById('login-popup').style.display = 'none';
    document.getElementById('popup-overlay').style.display = 'none';
}
