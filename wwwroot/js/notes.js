$(function() {
	$('#selectAll').click(function () {
		$('input[name=id][type=checkbox]').prop('checked', function () {
			if ($('#selectAll').is(':checked')) {
				return true;
			}
			return false;
		});
	});
});