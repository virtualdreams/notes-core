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
			url: '/note/remove/' + id
		}).done(function() {
			location.href = '/';
		}).fail(function() {
			$('#error').html('<div class="alert alert-danger">Failed to delete or restore note!</div>');
		});
	});

	$('#delAccountSubmit').click(function() {
		var id = $('#delAccount').data('id');
		$.ajax({
			type: "POST",
			url: '/admin/account/delete/' + id
		}).done(function(data) {
			if(data.success === false)
			{
				$('#error').html('<div class="alert alert-danger">' + data.error + '</div>');
			}
			else
			{
				location.href = '/admin/account/';
			}
		}).fail(function() {
			$('#error').html('<div class="alert alert-danger">Failed to delete account!</div>');
		});
	});

	function getFormData($form){
		var unindexed_array = $form.serializeArray();
		var indexed_array = {};

		$.map(unindexed_array, function(n, i){
			indexed_array[n['name']] = n['value'];
		});

		return indexed_array;
	}

	$('#saveContinue').click(function() {
		if($('#note-form').valid()) {
			var data = getFormData($('#note-form'));
			$.ajax({
				type: "POST",
				url: '/note/edit/',
				dataType: 'json',
				data: data
			}).done(function(d) {
				if(d.success) {
					$('#id').val(d.id);
					
					$('#result').html('<div class="alert alert-success"><button type="button" class="close">×</button>Successfully saved note!</div>');
					window.setTimeout(function() {
						$(".alert").fadeTo(500, 0).slideUp(500, function(){
                    		$(this).remove(); 
                		});
            		}, 5000);

          			$('.alert .close').on("click", function(e){
                		$(this).parent().fadeTo(500, 0).slideUp(500);
             		});

				} else {
					error();
				}
			}).fail(function() {
				error();
			});
		}

		var error = function() {
			$('#result').html('<div class="alert alert-danger"><button type="button" class="close">×</button>Failed to save note!</div>');

			$('.alert .close').on("click", function(e){
				$(this).parent().fadeTo(500, 0).slideUp(500);
			});
		}
	});

	$('#preview').click(function() {
		if($('#note-form').valid()) {
			if($('#editorPane').is(':visible'))
			{
				var data = getFormData($('#note-form'));
				$.ajax({
					type: "POST",
					url: '/note/preview/',
					dataType: 'json',
					data: data
				}).done(function(d) {
					$('#previewPane').html(d.content);
					$('#preview').text('Return to edit');

					$('#editorPane').toggle();
					$('#previewPane').toggle();
				});
			}
			else
			{
				$('#preview').text('Preview');

				$('#editorPane').toggle();
				$('#previewPane').toggle();
			}
		}
	});

	$.validator.setDefaults({
		highlight: function(element) {
			$(element).closest('.form-group').addClass('has-error');
		},
		unhighlight: function(element) {
			$(element).closest('.form-group').removeClass('has-error');
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
				nowhitespace: true
			},
			'content': {
				required: true,
				nowhitespace: true
			}
		}
	});

	$("#settings-form").validate({
		rules: {
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
				nowhitespace: true
			}
		}
	});

}(jQuery));