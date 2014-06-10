(function ($, undefined) {
	var cms = window.cms = window.cms || {};
	cms.membership = cms.membership = cms.membership || {};
	(function () {
		cms.membership.index = new (function () {
			return {
				init: function (domContainer, viewModel) {
					//this.initTabs(domContainer);
					this.initButtons(domContainer);
					this.initAccordion(domContainer);
					this.newMemberMgr = new cms.membership.NewMemberManager('#create-member-form');
					this.newMemberMgr.init();
					var newMemberMgr = this.newMemberMgr; //this done so the following overrides will have ref to the object in closure scope
					//overwriting the ajax methods of the new member manager
					this.newMemberMgr.ajaxBegin = function (jqXHR, settings) {
						newMemberMgr.hideError();

					};
					this.newMemberMgr.ajaxSuccess = ajaxHelper.success(
						function (data, textStatus, jqXHR) {
							newMemberMgr.resetCreateMemberForm();
							var member = new cms.membership.Member(data.Properties.Member.FirstName, data.Properties.Member.LastName, data.Properties.Member.Email, data.Properties.Member.Phone, data.Properties.Member.Username, data.Properties.Member.Password, data.Properties.Member.Status, data.Properties.Member.MemberID);
							//membersViewModel.addResult(member);
							member.viewModel = viewModel;
							viewModel.showEditMember(member);

						},
						function (data, textStatus, jqXHR, formattedMessage) {
							newMemberMgr.showError(formattedMessage);
						},
						false
					);
					this.newMemberMgr.ajaxFailure = ajaxHelper.error(
						function (jqXHR, textStatus, errorThrown, formattedMessage) {
							newMemberMgr.showError(formattedMessage);
						},
						false
					);
					return this;
				},
				initButtons: function (domContainer) {
					$('#create-member', domContainer).button({
						icons: {
							primary: 'ui-icon-plusthick'
						}
					}).on('click', function (event) {
						event.preventDefault();
						cms.membership.index.newMemberMgr.showCreateMemberForm();

					});
					cms.makeCloseButton('#close-find-member-results');
				},
				initAccordion: function (domContainer) {
					$('#start-options', domContainer).accordion({
						fillSpace: true
					});

				},
				findMembersSucceeded: function (data) {
					alert('findMembersSucceeded called');
				},
				findMembersFailed: function (data) {
					alert('findMembersFailed called');
				},
				newMemberMgr: null,
				initEditMemberTab: function (panel) {
					var editableParent = panel.find('.editable-parent'),
						memberID = parseInt(editableParent.attr('data-member-id'));
					if (editableParent.length > 0) {
						var editMgr = new EditableManager(
								editableParent,
								{
									memberID: memberID
								},
								'/Admin/Membership/EditInPlace'
							);
						editMgr.initTextboxes();
						editMgr.initPasswords();
						editMgr.initSelects();
						//editMgr.initCheckboxLists();
					}

					editableParent.find('.refresh-member-link')
					.button({
						icons: {
							primary: 'ui-icon-refresh'
						},
						text: false
					})
					.on('click', function (e) {
						e.preventDefault();
						var memberMgr = cms.membership.getClosestMemberManager(panel);
						if (memberMgr) {
							var viewModel = memberMgr.getViewModel();
							if (viewModel) {
								viewModel.tabMgr.reloadTabByID(cms.membership.uniqueIDForMemberTab($(this).attr('data-member-id')));
							}
						}
						//cmsUsersViewModel.tabMgr.reloadTabByID(uniqueIDForCmsUserTab(cmsUserID));
					});

					editableParent.find('.delete-member-link')
						.button({
							icons: {
								primary: 'ui-icon-trash'
							},
							text: false
						})
						.on('click', function (e) {
							e.preventDefault();
							var memberID = $(this).attr('data-member-id');
							dialogHelper.confirm(
								'Are you sure you want to delete this member?',
								'Delete Member',
								function () {
									ajaxHelper.ajax('/Admin/Membership/Delete', {
										data: {
											memberID: memberID
										},
										type: 'POST',
										success: function (data, textStatus, jqXHR) {
											var memberMgr = cms.membership.getClosestMemberManager(panel);
											viewModel = memberMgr.getViewModel();
											viewModel.removeMember(memberID);
										},
										failureMessageFormat: 'An error occurred trying to delete the member: [[errorThrown]]'
									});
								}
							);
						});
				}
			};
		});

	})();
})(jQuery);


$(function () {
	var domContainer = $('#membership');
	//cms.membership.index.init(domContainer);
	/*				find_succeeded: callback for when ajax call to find members succeeds
	find_failed: callback for when AJAX call to find members fails
	domContainer: the dom element within which the member interface operates
	ajaxUrl: the AJAX URL called to find members by a search term
	ajaxData: an object containing data to pass to the jQuery AJAX call*/

	var memberManager = new cms.membership.MemberManager({
		domContainer: domContainer,
		ajaxUrl: '/Admin/Membership/Find',
		data: {},
		find_succeeded: cms.membership.index.findMembersSucceeded,
		find_failed: cms.membership.index.findMembersFailed,
		applying_bindings: function () {
			var viewModel = memberManager.getViewModel();
			viewModel.tabMgr = new TabManager({
				closable: true,
				container: $('#membership-tabs'),
				numStaticTabs: 1,

				load_tabTypeSelector: '.cms-members-index-tab',
				load_tabTypeActions: {
					"edit-member": cms.membership.index.initEditMemberTab
				},
				show_action: function (event, ui, tabMgr) {
					//					if (ui.index === 0) {
					//						cmsUsersViewModel.findNameEl.select();
					//					}
					//					else {
					//						addRecentCmsUserByTab.call(this, event, ui);
					//					}
				}

			});
		}
	});

	memberManager.init();
	var viewModel = memberManager.getViewModel();
	viewModel.member_removed = function (memberID) {
		this.tabMgr.removeTabByID(cms.membership.uniqueIDForMemberTab(memberID));
	};
	viewModel.retrievingMembers.subscribe(function (newValue) {
		if (!newValue) {
			domContainer.find('.impersonate-member').button({
				icons: {
					primary: 'ui-icon-person'
				},
				text: false
			});
		}
	});
	cms.membership.index.init(domContainer, viewModel);
	cms.membership.initFindMemberInput($('#find-members-name'), viewModel, domContainer);
	cms.membership.initShowAllMembersLink('#show-all-members', viewModel, domContainer);
});