﻿
function SetActiveMenus(iconMenu, profileMenu) {
	try {
		if (iconMenu != "")
			$("#icon-bar-".concat(iconMenu)).addClass("active");
		if (profileMenu != undefined)
			$("#my-stuff-".concat(profileMenu)).addClass("active");
	}
	catch (Exception) { /* ignore errors here */}
}

function deleteImageAjax(deleteType, uid, id) {
	try {
		var ajaxData = { //json structure
			UID: uid,
			ID: id
		};

		var strURL;
		if (deleteType == "profile")
			strURL = "../Profile/DeleteImage";
		else //event
			strURL = "../../Profile/DeleteEventImage";

		$.ajax({
			type: "POST",
			url: strURL,
			data: ajaxData,
			success: function (returnData) {
				if (returnData.Status == 1) {
					$("#image-".concat(id)).hide();
				}
				else {
					alert('Unable to remove image.');
				}
			},
			error: function (xhr) {
				debugger;
			}
		});
	}
	catch (err) {
		showError(err);
	}
}

function SetActiveMenus(iconMenu, profileMenu) {
	try {
		if (iconMenu != "")
			$("#icon-bar-".concat(iconMenu)).addClass("active");
		if (profileMenu != undefined)
			$("#profile-".concat(profileMenu)).addClass("active");
	}
	catch (Exception) { /* ignore errors here */ }
}

