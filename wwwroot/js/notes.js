$(function() {
	$('#trashDelete').click(function () {
		$('input[name=id][type=checkbox]').prop('checked', function () {
			if ($('#trashDelete').is(':checked')) {
				return true;
			}
			return false;
		});
	});
});