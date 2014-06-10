//depends on ajaxHelper object

(function ($, undefined) {
	var cms = window.cms = window.cms || {};
	(function () {
		//a url object with some url-related utility functions
		cms.menuitem = new (function () {
			return {
				//				functionName: function (args) {
				//					//body
				//				}

			};
		});


		cms.menuitem.NewExternalLinkManager = function (options) {
			/*
			options:
			form, 
			error, 
			loader, 
			success
			*/
			this.form = $(options.form);
			//this.form.data('manager', this);
			this.error = $(options.error);
			this.loader = $(options.loader);
			this.referenceNodeIDInput = this.form.find('#reference-node-id');
			this.createRelationshipInput = this.form.find('#create-relationship');
			this.title = this.form.find('#ExternalLinkMenuItem_Title');
			this.linkurl = this.form.find('#ExternalLinkMenuItem_ExternalLinkUrl');
			this.success = options.success;


		};
		cms.menuitem.NewExternalLinkManager.prototype = function () {
			var ajaxFailure = {}, //these need to be defined in the JS for the host interface for scope reasons
				ajaxSuccess = {},
				showForm = function (referenceNodeID, createRelationship) {//, callback) {
					
					this.referenceNodeIDInput.val(referenceNodeID);
					this.createRelationshipInput.val(createRelationship);
					this.title.val('New External Link');
					this.form.dialog({
						buttons: {
							'Create': function () {

								$(this).find('form').submit();
							}
						},
						modal: true,
						resizable: false,
						title: this.title.val(),
						width: 400,
						autoOpen:false
					});
					this.resetForm();
					this.title.select();
					this.form.dialog('open');
				},
				showError = function (errorMessage) {
					this.error.html(errorMessage);
					this.error.fadeIn();
				},
				hideError = function () {
					this.error.html('');
					this.error.hide();
				},
				resetForm = function () {
					this.form.dialog('close');
					this.hideError();
					this.loader.hide();
					this.form.hide();
					this.title.val(this.title.prop('defaultValue'));
					this.linkurl.val(this.linkurl.prop('defaultValue'));
					this.referenceNodeIDInput.val(this.referenceNodeIDInput.prop('defaultValue'));
					this.createRelationshipInput.val(this.createRelationshipInput.prop('defaultValue'));
				};

			return {
				ajaxFailure: ajaxFailure,
				ajaxSuccess: ajaxSuccess,
				showForm: showForm,
				hideError: hideError,
				showError: showError,
				resetForm: resetForm
			};
		} ();

	})();

})(jQuery);
