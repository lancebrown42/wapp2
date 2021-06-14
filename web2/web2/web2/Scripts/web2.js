
function SetActiveMenus(iconMenu, profileMenu) {
	try {
		if (iconMenu != "")
			$("#icon-bar-".concat(iconMenu)).addClass("active");
		if (profileMenu != undefined)
			$("#my-stuff-".concat(profileMenu)).addClass("active");
	}
	catch (Exception) { /* ignore errors here */}
}
