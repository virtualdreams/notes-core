$(function() {
	$('[data-href]').click(function() {
		var href = $(this).data('href');
		location.href = href;
	});
	
	$('#selectAll').click(function () {
		$('input[name=id][type=checkbox]').prop('checked', function () {
			if ($('#selectAll').is(':checked')) {
				return true;
			}
			return false;
		});
	});
});