
function SaveReport(uid, id, problem_id) {
	try {
		var ajaxData = { //json structure
			UID: uid,
			IDToReport: id,
			ProblemID: problem_id
		}

		$.ajax({
			type: "POST",
			url: "../../Home/SaveReport",
			data: ajaxData,
			success: function (returnData) {	
				$("#report-submitting").hide();
				$("#report-content").hide();
				$("#report-submitted").show();

				$("#report-panel").delay(4000).hide("slow", function () {
					//set everything to default values
					$("#report-panel").hide();
					$("#report-submitting").hide();
					$("#report-submitted").hide();
					$("#report-content").show();
					$("#report-list").val(0);
					$("#report-failed").hide();
				});

			},
			error: function (xhr) {

				$("#report-submitting").hide();
				$("#report-content").hide();
				$("#report-submitted").hide();
				$("#report-failed").show();

				$("#report-panel").delay(6000).hide("slow", function () {
					//set everything to default values
					$("#report-panel").hide();
					$("#report-submitting").hide();
					$("#report-submitted").hide();
					$("#report-content").show();
					$("#report-list").val(0);
					$("#report-failed").hide();
				});
			}
		});
	}
	catch (err) {
		showError(err);
	}
}
