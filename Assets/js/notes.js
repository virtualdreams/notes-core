var notes = notes || {};
notes = (function($){
	$('[data-toggle=popover]').popover();
	
	$('[data-href]').click(function() {
		var href = $(this).data('href');
		location.href = href;
	});
	
	$('#selectAll').click(function () {
		$('input[name=id][type=checkbox]').prop('checked', function() {
			if ($('#selectAll').is(':checked')) {
				return true;
			}
			return false;
		});
	});

	$('#trashSubmit').click(function() {
		var id = $('#trash').data('id');
		$.ajax({
			type: "POST",
			url: '/note/trash/' + id
		}).done(function() {
			location.href = '/';
		}).fail(function() {
			alert('Failed to delete note.');
		});
	});

	$('#notebookDelete').click(function() {
		$('#notebookValue').val('')
	});

	$('#notebookSubmit').click(function() {
		var id = $('#notebook').data('id');

		var d = {
			id: id,
			notebook: $('#notebookValue').val()
		};

		$.ajax({
			type: "POST",
			url: '/note/notebook/',
			dataType: 'json',
			data: d
		}).done(function() {
			location.reload();
		}).fail(function() {
			alert('Failed to write notebook.');
		});
	});

	$('#tagsDelete').click(function() {
		$('#tagsValue').val('')
	});

	$('#tagsSubmit').click(function() {
		var id = $('#tags').data('id');

		var d = {
			id: id,
			tags: $('#tagsValue').val()
		};

		$.ajax({
			type: "POST",
			url: '/note/tags/',
			dataType: 'json',
			data: d
		}).done(function() {
			location.reload();
		}).fail(function() {
			alert('Failed to write tags.');
		});
	});
}(jQuery));