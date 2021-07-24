
function rateEvent(uid, id, rating) {
	try {
		var ajaxData = { //json structure
			UID: uid,
			ID: id,
			Rating: rating
		}

		$.ajax({
			type: "POST",
			url: "../../Home/RateEvent",
			data: ajaxData,
			success: function (returnData) {
				var i;

				//remove all "checked" stars
				for (i = 1; i <= 5; i++) {
					$("#rate".concat(i)).removeClass("checked");
				}

				//add updated "checked" stars
				for (i = 1; i <= rating; i++) {
					$("#rate".concat(i)).addClass("checked");
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

function toggleEventLike(like_location, uid, id) {
	try {
		var ajaxData = { //json structure
			UID: uid,
			ID: id
		}

		var strURL;
		if (like_location == "index") //Home/Index
			strURL = "../Home/ToggleEventLike";
		else // Home/Event/22
			strURL = "../../Home/ToggleEventLike";

		$.ajax({
			type: "POST",
			url: strURL,
			data: ajaxData,
			success: function (returnData) {
				//1 = added; 0 = removed
				if (returnData.Status == 1) {//1 = added
					$("#like-user-i-".concat(id)).removeClass("fal fa-thumbs-up");
					$("#like-user-i-".concat(id)).addClass("fas fa-thumbs-up");
					var count = 0;
					count = parseInt($("#like-count-".concat(id)).text());
					count += 1;
					$("#like-count-".concat(id)).text(count);
				}
				else {//0 = removed
					$("#like-user-i-".concat(id)).removeClass("fas fa-thumbs-up");
					$("#like-user-i-".concat(id)).addClass("fal fa-thumbs-up");
					var count = 0;
					count = parseInt($("#like-count-".concat(id)).text());
					count -= 1;
					if (count < 0) count = 0;
					$("#like-count-".concat(id)).text(count);
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

