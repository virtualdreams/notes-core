var notes = notes || {};
notes = (function ($) {
	function split(val) {
		return val.split(' ');
	}

	function extractLast(term) {
		return split(term).pop();
	}

	$("#notebook").autocomplete({
		source: '/search/notebook',
		minLength: 3,
	});

	$("#tags").autocomplete({
		source: function (request, response) {
			$.getJSON('/search/tags', {
				term: extractLast(request.term)
			}, response);
		},
		search: function () {
			var term = extractLast(this.value);
			if (term.length < 3) {
				return false;
			}
		},
		focus: function () {
			return false;
		},
		select: function (event, ui) {
			var terms = split(this.value);
			terms.pop();
			terms.push(ui.item.value);
			terms.push("");
			this.value = terms.join(" ");
			return false;
		}
	});

	$('[data-toggle="popover"]').popover();

	$('[data-toggle="tooltip"]').tooltip({ boundary: 'window' })

	$('[data-href]').click(function () {
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

	$('#trashSubmit').click(function () {
		var id = $('#trash').data('id');
		$.ajax({
			type: "POST",
			url: '/note/remove/' + id
		}).done(function () {
			location.href = '/';
		}).fail(function () {
			$('#error').html('<div class="alert alert-danger">Failed to delete or restore note!</div>');
		});
	});

	$('#delAccountSubmit').click(function () {
		var id = $('#delAccount').data('id');
		$.ajax({
			type: "POST",
			url: '/admin/account/delete/' + id
		}).done(function (data) {
			if (data != null && data.success === false) {
				$('#error').html('<div class="alert alert-danger">' + data.error + '</div>');
			}
			else {
				location.href = '/admin/account/';
			}
		}).fail(function () {
			$('#error').html('<div class="alert alert-danger">Failed to delete account!</div>');
		});
	});

	function getFormData($form) {
		var unindexed_array = $form.serializeArray();
		var indexed_array = {};

		$.map(unindexed_array, function (n, i) {
			indexed_array[n['name']] = n['value'];
		});

		return indexed_array;
	}

	$('#saveContinue').click(function () {
		if ($('#note-form').valid()) {
			var data = getFormData($('#note-form'));
			$.ajax({
				type: "POST",
				url: '/note/edit/',
				dataType: 'json',
				data: data
			}).done(function (data) {
				if (data != null && data.success) {
					$('#id').val(data.id);

					$('#result').html('<div class="alert alert-success"><button type="button" class="close">×</button>Note has been successfully saved.</div>');
					window.setTimeout(function () {
						$(".alert").fadeTo(500, 0).slideUp(500, function () {
							$(this).remove();
						});
					}, 5000);

					$('.alert .close').on("click", function (e) {
						$(this).parent().fadeTo(500, 0).slideUp(500);
					});

				} else {
					error();
				}
			}).fail(function () {
				error();
			});
		}

		var error = function () {
			$('#result').html('<div class="alert alert-danger"><button type="button" class="close">×</button>Failed to save note!</div>');

			$('.alert .close').on("click", function (e) {
				$(this).parent().fadeTo(500, 0).slideUp(500);
			});
		}
	});

	$('#preview').click(function () {
		if ($('#editor-source').is(':visible')) {
			var data = getFormData($('#note-form'));
			$.ajax({
				type: "POST",
				url: '/note/preview/',
				dataType: 'json',
				data: data
			}).done(function (d) {
				if (data != null) {
					$('#preview-content').html(d.content);
					$('#preview').html('<i class="fa fa-pencil"></i> Edit');

					$('#editor-source').toggle();
					$('#editor-preview').toggle();
				} else {
					error();
				}
			}).fail(function () {
				error();
			});

			var error = function () {
				$('#result').html('<div class="alert alert-danger"><button type="button" class="close">×</button>Failed to generate preview!</div>');

				$('.alert .close').on("click", function (e) {
					$(this).parent().fadeTo(500, 0).slideUp(500);
				});
			}
		}
		else {
			$('#preview').html('<i class="fa fa-eye"></i> Preview');

			$('#editor-source').toggle();
			$('#editor-preview').toggle();
		}
	});

	$('#help').click(function () {
		$('#editor-help').slideToggle();
	});

	$.validator.setDefaults({
		highlight: function (element) {
			$(element).addClass('is-invalid');
		},
		unhighlight: function (element) {
			$(element).removeClass('is-invalid');
		}
	});

	$.validator.addMethod(
		"regex",
		function (value, element, regexp) {
			return this.optional(element) || value.match(regexp);
		},
		"This field has the wrong format."
	);

	$.validator.addMethod(
		'nowhitespace',
		function (value, element) {
			return this.optional(element) || value.trim() != ''
		},
		'This field is required.'
	);

	$.validator.addMethod(
		'stringarrayitemmaxlength',
		function (value, element, length) {
			var array = value.split(' ');
			for (var i = 0; i < array.length; i++) {
				if (array[i].trim().length > length) {
					return false;
				}
			}
			return true;
		},
		'Length of word too long.'
	);

	$("#login-form").validate({
		rules: {
			'username': {
				required: true,
				nowhitespace: true,
				email: true
			},
			'password': {
				required: true,
				nowhitespace: true,
				minlength: 8
			}
		}
	});

	$("#forgot-form").validate({
		rules: {
			'username': {
				required: true,
				nowhitespace: true,
				email: true
			}
		}
	});

	$("#reset-form").validate({
		rules: {
			'newpassword': {
				required: true,
				nowhitespace: true,
				minlength: 8
			},
			'confirmpassword': {
				required: true,
				nowhitespace: true,
				minlength: 8,
				equalTo: '#newpassword'
			}
		},
		messages: {
			confirmpassword: {
				equalTo: "The passwords doesn't match."
			}
		}
	});

	$("#note-form").validate({
		rules: {
			'title': {
				required: true,
				nowhitespace: true,
				maxlength: 100
			},
			'content': {
				required: true,
				nowhitespace: true
			},
			'notebook': {
				maxlength: 50
			},
			'tags': {
				stringarrayitemmaxlength: 50
			}
		},
		messages: {
			'tags': {
				stringarrayitemmaxlength: "Tag too long."
			}
		}
	});

	$("#settings-form").validate({
		rules: {
			'displayname': {
				nowhitespace: true,
				maxlength: 50
			},
			'items': {
				required: true,
				nowhitespace: true,
				min: 1,
				max: 100
			}
		}
	});

	$("#security-form").validate({
		rules: {
			'oldpassword': {
				required: true,
				nowhitespace: true,
				minlength: 8
			},
			'newpassword': {
				required: true,
				nowhitespace: true,
				minlength: 8
			},
			'confirmpassword': {
				required: true,
				nowhitespace: true,
				minlength: 8,
				equalTo: '#newpassword'
			}
		},
		messages: {
			confirmpassword: {
				equalTo: "The passwords doesn't match."
			}
		}
	});

	$("#account-form").validate({
		rules: {
			'username': {
				required: true,
				email: true
			},
			'password': {
				minlength: 8
			},
			'displayname': {
				nowhitespace: true,
				maxlength: 50
			}
		}
	});

}(jQuery));