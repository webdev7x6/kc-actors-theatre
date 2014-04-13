/// <reference path="http://clickfarmcdn.localhost/common/js/jquery/1.7.2/jquery-vsdoc.js" />
/// <reference path="http://clickfarmcdn.localhost/common/js/kendo/2012.2.710/kendo.all-vsdoc.js" />
(function ($, undefined) {
	var cms = window.cms = window.cms || {};

	(function () {
		//a membership object with some membership-related utility functions
		cms.membership = new (function () {
			return {
				setMemberManager: function (domContainer, memberManager) {
					$(domContainer).data('memberMgr', memberManager);
				},
				getMemberManager: function (domContainer) {
					return $(domContainer).data('memberMgr');
				},
				getClosestMemberManager: function (ancestor) {
					//use this to retrieve a member manager when all you have reference to is a
					//dom object somewhere inside the domContainer where the mgr is stored
					return $(ancestor).closest('.member-container').data('memberMgr');
				},

				initEditInPlace: function (tableRow) {
					var editMgr = new EditableManager($(tableRow), { memberid: $(tableRow).attr('data-member-id') }, '/Admin/Membership/EditInPlace');
					editMgr.initTextboxes();
					editMgr.initYesNoBoolean();
				},
				buttonizeDeleteLinks: function (domContainer) {
					cms.buttonizeDeleteLinks(domContainer, '.delete-member');
					return this;
				},
				uniqueIDForMemberTab: function (memberID) {
					return 'edit-member-' + memberID;
				},
				initShowAllMembersLink: function (linkSelector, viewModel, domContainer) {
					$(linkSelector, domContainer).button({
						icons: {
							primary: 'ui-icon-gear'
						}
					}).on('click', function (event) {
						event.preventDefault();
						ajaxHelper.ajax('/Admin/Membership/All', {
							data: {},
							type: 'POST',
							success: function (data, textStatus, jqXHR) {
								viewModel.clearMembers();
								if (data.Succeeded && data.Properties.Members.length > 0) {
									$.each(data.Properties.Members, function (index, item) {
										//push results into view model's observable collection of views
										viewModel.addMember(new cms.membership.Member(item.FirstName, item.LastName, item.Email, item.Phone, item.Username, item.Password, item.Status, item.MemberID));
									});
									viewModel.anyMembers(true);
									cms.membership.buttonizeDeleteLinks(domContainer);
									domContainer.find('.member-table tbody tr').each(function (index, item) {
										cms.membership.initEditInPlace(item);
									});

								}
								else {
									viewModel.anyMembers(false);
								}
								viewModel.findResultsVisible(true);

							},
							beforeSend: function (jqXHR, settings) {
								viewModel.retrievingMembers(true);
							},
							complete: function (jqXHR, textStatus) {
								viewModel.retrievingMembers(false);
							}
						});
					});
				},

				//this function used to initialize the search Members by keyword input
				initFindMemberInput: function (input, viewModel, domContainer) {
					$(input).keyup($.debounce(350, function () {
						if (input.val().length > 1) {
							ajaxHelper.ajax('/Admin/Membership/Find', {
								data: {
									term: input.val()//,
									//appID: $('#app').attr('data-app-id')
								},
								type: 'POST',
								success: function (data, textStatus, jqXHR) {
									viewModel.clearMembers();
									if (data.Succeeded && data.Properties.Members.length > 0) {
										$.each(data.Properties.Members, function (index, item) {
											//push results into view model's observable collection of views
											viewModel.addMember(new cms.membership.Member(item.FirstName, item.LastName, item.Email, item.Phone, item.Username, item.Password, item.Status, item.MemberID));
										});
										viewModel.anyMembers(true);
										cms.membership.buttonizeDeleteLinks(domContainer);
										domContainer.find('.member-table tbody tr').each(function (index, item) {
											cms.membership.initEditInPlace(item);
										});

									}
									else {
										viewModel.anyMembers(false);
									}
									viewModel.findResultsVisible(true);

								},
								beforeSend: function (jqXHR, settings) {
									viewModel.retrievingMembers(true);
								},
								complete: function (jqXHR, textStatus) {
									viewModel.retrievingMembers(false);
								}
							});
						}
					})
				);

				}

			}
		});

		cms.membership.MemberManager = function (options) {
			/*
			domContainer: the dom element within which the member interface operates
			ajaxUrl: the AJAX URL called to find members by a search term
			ajaxData: an object containing data to pass to the jQuery AJAX call
			applying_bindings: callback for when the MemberManager is about to apply Knockout bindings
			find_succeeded: callback for when ajax call to find members succeeds
			find_failed: callback for when AJAX call to find members fails
			*/
			//this.urlViewModel = urlViewModel;
			this.options = options;
			this.domContainer = $(options.domContainer);
			cms.membership.setMemberManager(this.domContainer, this);
			this.ajaxUrl = options.ajaxUrl;
			this.ajaxData = options.ajaxData;
			this.memberForm = this.domContainer.find('.new-member-form');
		};

		cms.membership.MemberManager.prototype = function () {

			var init = function () {
				var viewModel = new cms.membership.MemberViewModel(this);
				this.setViewModel.call(this, viewModel);
				cms.doCallback(this.options.applying_bindings);
				ko.applyBindings(viewModel, this.domContainer.get(0));
				//this.initMemberOptionLinks.call(this);
				//cms.url.initializeUrlInput($('input[type="text"]', this.urlForm));
			},

			//		getUrlContainer = function (container) {
			//			return $(container).find('.url-container');
			//		},
		setViewModel = function (newViewModel) {
			this.domContainer.data('memberViewModel', newViewModel)
		},

		getViewModel = function () {
			return this.domContainer.data('memberViewModel');
		},

		refreshMembers = function () {
			var refreshLink = this.domContainer.find('.refresh-members'),
			thisMemberMgr = this,
			viewModel = getViewModel.call(this);
			if (!viewModel) { throw "While attempting to refresh Members, the view model is not properly initialized."; }
			viewModel.retrievingMembers(true);
			ajaxHelper.ajax(this.ajaxUrl, {
				type: 'POST',
				data: this.ajaxData,
				success: function (data, textStatus, jqXHR) {
					viewModel.clearMembers();
					$.each(data.Properties.Members, function (index, item) {
						var member = new cms.membership.Member(item.FirstName, item.LastName, item.Email, item.Phone, item.Username, item.Password, item.Status);
						viewModel.addMember(member);
					});

					viewModel.retrievingMembers(false);
					refreshLink.off('click').on('click', function (event) {
						event.preventDefault();
						thisMemberMgr.refreshMembers();
					});
					var domContainer = thisMemberMgr.domContainer;
					//init edit in place for each Url in the table
					domContainer.find('.member-table tbody tr').each(function (index, item) {
						cms.membersship.initEditInPlace(item);
					});
					//init Member links
					//cms.url.setUrlLinkHref(domContainer).buttonizeUrlLinks(domContainer)
					thisMemberMgr.buttonizeDeleteLinks(domContainer);
					cms.doCallback(this.options.find_succeeded);
				},
				failure: function (data, textStatus, jqXHR, formattedMessage) {
					//callback for handling failure, i.e. data.Succeeded === false
					viewModel.retrievingMembers(false);
					
				},
				failureDoAlert: true,
				errorDoAlert: true
			});
		},

		initMemberOptionLinks = function () {
			this.domContainer.find('.refresh-members').button({
				icons: {
					primary: 'ui-icon-refresh'
				},
				text: false
			});
			var thisMemberMgr = this;
			this.domContainer.find('.new-member')
				.button({
					icons: {
						primary: 'ui-icon-circle-plus'
					},
					text: false
				})
				.off('click')
				.on('click', function (event) {
					event.preventDefault();
					this.handleNewMemberClick(thisMemberMgr);
				});

			this.domContainer.find('.cancel-new-member').off('click').on('click', function (event) {
				event.preventDefault();
				resetNewMemberForm(thisMemberMgr);
			});
		},

		handleNewMemberClick = function (aMemberMgr) {
			this.resetNewMemberForm(aMemberMgr);
			aMemberMgr.memberForm.show();
			aMemberMgr.memberForm.find('input.first-name').select();
		},

		resetNewMemberForm = function (aMemberMgr) {
			var firstNameInput = aMemberMgr.memberForm.find('input.first-name');
			firstNameInput.val(firstNameInput.prop('defaultValue'));
			var lastNameInput = aMemberMgr.memberForm.find('input.last-name');
			lastNameInput.val(lastNameInput.prop('defaultValue'));
			//need to add others here if the form collects more than first/last name

			aMemberMgr.hideNewMemberError();
			aMemberMgr.memberForm.find('.loader').hide();
			aMemberMgr.memberForm.find('.new-member-error').hide();
			aMemberMgr.memberForm.dialog('close');
			aMemberMgr.memberForm.hide();
		},

		showNewMemberError = function (errorMessage) {
			this.memberForm.find('.new-member-error').html(errorMessage);
			this.memberForm.find('.new-member-error').fadeIn();
		},

		hideNewMemberError = function () {
			this.memberForm.find('.new-member-error').html('');
			this.memberForm.find('.new-member-error').hide();
		};

			//		deleteUrl = function (url) {
			//			var viewModel = getViewModel.call(this);
			//			ajaxHelper.ajax('/Admin/Url/Delete', {
			//				type: 'POST',
			//				data: {
			//					id: url.urlID
			//				},
			//				success: function (data, textStatus, jqXHR) {
			//					appViewModel.registerUnappliedChange();
			//					//var viewModel = getViewModel();
			//					viewModel.urls.remove(url);
			//				},
			//				failureMessageFormat: 'An error occurred trying to delete the URL: [[errorThrown]]'
			//			});
			//		},






			return {
				init: init,
				refreshMembers: refreshMembers,
				setViewModel: setViewModel,
				getViewModel: getViewModel,
				showNewMemberError: showNewMemberError,
				hideNewMemberError: hideNewMemberError,
				//initEditInPlace: initEditInPlace,
				resetNewMemberForm: resetNewMemberForm

			};
		} ();


		cms.membership.MemberViewModel = function (memberMgr, member_removed) {
			this.memberMgr = memberMgr;
			this.member_removed = member_removed;//callback for when a member is removed from the viewmodel
		};

		cms.membership.MemberViewModel.prototype = function () {
			var members = ko.observableArray([]),
				addMember = function (member) {
					member.setViewModel(this);
					this.members.push(member);
				},
				anyMembers = function () {
					return this.members().length > 0;
				},
				clearMembers = function () {
					this.members([]);
				},
				removeMember = function (memberID) {
					this.members.remove(function (item) {
						return parseInt(item.memberID) === parseInt(memberID)
					});
					if ($.isFunction(this.member_removed)) {
						this.member_removed(memberID);
					}
				},
				closeFindResults = function () {
					this.findResultsVisible(false);
				},
				retrievingMembers = ko.observable(true),
				findResultsVisible = ko.observable(false),
			//findingResults = ko.observable(false),
				showEditMember = function (member) {
					var memberID = member.memberID;
					member.viewModel.tabMgr.addTab(new TabInfo(
						'/Admin/Membership/Edit/' + memberID,
						member.firstName + ' ' + member.lastName,
						'edit-member',
						cms.membership.uniqueIDForMemberTab(memberID)
					));
				},

				recentMembers = ko.observableArray([]),
				addRecentMember = function (member) {
					var ix = this.findRecentIndexByID(member.memberID());
					if (ix > -1) {
						this.recentMembers.splice(ix, 1);
					}
					else {
						//only allow 10 recent entities
						while (this.recentMembers().length >= 10) {
							this.recentMembers.pop()
						}
					}
					this.recentMembers.unshift(member);
				},
				findRecentIndexByID = function (memberID) {
					var ixOf = -1;
					$.each(this.recentMembers(), function (ix, item) {
						if (item.memberID() === memberID) {
							ixOf = ix;
							return false;
						}
					});
					return ixOf;
				},
				removeRecentByID = function (memberID) {
					this.recentMembers.remove(function (item) {
						return item.memberID() === memberID
					});
				}
			;

			return {
				members: members,
				addMember: addMember,
				anyMembers: anyMembers,
				retrievingMembers: retrievingMembers,
				clearMembers: clearMembers,
				removeMember: removeMember,
				findResultsVisible: findResultsVisible,
				closeFindResults: closeFindResults,
				showEditMember: showEditMember,
				recentMembers: recentMembers
			};

		} ();

		cms.membership.Member = function (firstName, lastName, email, phone, username, password, status, memberID) {
			this.firstName = firstName;
			this.lastName = lastName;
			this.email = email;
			this.username = username;
			this.password = password;
			this.phone = phone;
			this.status = status;
			this.memberID = memberID;
			this.viewModel = {}; //a reference to the Knockout viewModel within which this object is contained
		};
		cms.membership.Member.prototype = function () {
			var fullName = function () {
				return this.firstName + ' ' + this.lastName;
			},
			fullNameReverse = function () {
				return this.lastName + ', ' + this.firstName;
			},
			setViewModel = function (viewModel) {
				this.viewModel = viewModel;
			}
			return {
				fullName: fullName,
				fullNameReverse: fullNameReverse,
				setViewModel: setViewModel
			};
		} ();

		cms.membership.NewMemberManager = function (formSelector) {
			this.form = $(formSelector); //'#create-member-form');
			this.errorElement = this.form.find('#create-member-error');
			this.loader = this.form.find('#create-member-loading');
			this.firstNameInput = this.form.find('#Member_FirstName');
			this.lastNameInput = this.form.find('#Member_LastName');
			this.emailInput = this.form.find('#Member_Email');
			this.passwordInput = this.form.find('#Member_Password');
			//pswdConfEl = form.find('#create-cms-user-confirm-password')

		};
		cms.membership.NewMemberManager.prototype = function () {
			var init = function () {
				var theForm = this.form;
				this.form.dialog({
					autoOpen: false,
					buttons: {
						'Create': function () {
							theForm.find('form').submit();
						}
					},
					modal: true,
					resizable: false,
					width: 400
				});
			},
				showCreateMemberForm = function () {
					this.resetCreateMemberForm();
					this.form.dialog('open');
					this.firstNameInput.select();
				},

				resetCreateMemberForm = function () {
					this.form.dialog('close');
					this.hideError();
					this.loader.hide();
					this.firstNameInput.val('');
					this.lastNameInput.val('');
					this.emailInput.val('');
					this.passwordInput.val('');
				},
			//you pretty much have to override these ajax methods when you create the NewMemberManager object 
			//and create the mgr object as a var that will be in closure scope for the override methods
			//if you're using an an ASP.NET Ajax form helper since the reference to "this" will be the Ajax 
			//object when they get called. see cms.membership.index.js for an example.
				ajaxBegin = function (jqXHR, settings) {
					this.hideError();
				},

				ajaxSuccess = ajaxHelper.success(
					function (data, textStatus, jqXHR) {
						this.resetCreateMemberForm(); //TODO: "this" reference will break here
						var member = new cms.membership.Member(data.FirstName, data.LastName, data.Email, data.Phone, data.Username, data.Password, data.Status, data.MemberID);
						membersViewModel.addResult(member);
						member.editMember();
					},
					function (data, textStatus, jqXHR, formattedMessage) {
						this.showError(formattedMessage); //TODO: "this" reference will break here
					},
					false
				),

				ajaxFailure = ajaxHelper.error(
					function (jqXHR, textStatus, errorThrown, formattedMessage) {
						this.showError(formattedMessage); //TODO: "this" reference will break here
					},
					false
				),

				showError = function (msg) {
					this.errorElement.html(msg).fadeIn();
				},

				hideError = function () {
					this.errorElement.html('').hide();
				}

			return {
				init: init,
				showCreateMemberForm: showCreateMemberForm,
				resetCreateMemberForm: resetCreateMemberForm,
				ajaxBegin: ajaxBegin,
				ajaxSuccess: ajaxSuccess,
				ajaxFailure: ajaxFailure,
				showError: showError,
				hideError: hideError
			};
		} ();
	})();
})(jQuery);

