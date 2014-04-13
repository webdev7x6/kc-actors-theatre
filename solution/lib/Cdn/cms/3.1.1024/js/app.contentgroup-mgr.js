/// <reference path="http://clickfarmcdn.localhost/common/js/jquery/1.7.2/jquery-vsdoc.js" />
/// <reference path="http://clickfarmcdn.localhost/common/js/kendo/2012.2.710/kendo.all-vsdoc.js" />

var contentGroupMgr = new contentGroupManager();

function contentGroupManager() {
	//this is copied, need to find a single home for this method to be used globally
	//this.findTabPanelForPage = function (pageID) {
		//return $('div.app-index-tab[data-page-id="' + pageID.toString() + '"]');
	//};

	this.showWorking = function (panel) {
		panel.find('.content-groups-working').show();
	};

	this.hideWorking = function (panel) {
		panel.find('.content-groups-working').fadeOut();
	};

	this.refreshContentGroups = function (panel, appID, pageID, controllerDisplayName) {
		var list = $('.content-groups', panel);
		if (controllerDisplayName) {
			contentGroupMgr.showWorking(panel);
			ajaxHelper.ajax('/Admin/Page/ContentGroups', {
				type: 'POST',
				data: {
					appID: appID,
					controllerDisplayName: controllerDisplayName
				},
				success: function (data, textStatus, jqXHR) {
					var working = $('.content-groups-working', list);
					working.appendTo(panel);
					list.empty();
					$.each(data.Properties.ContentGroups, function (index, contentGroup) {
						var li = $('<a class="content-group-link" href="#" data-content-group="' + contentGroup + '">' + contentGroup + '</a>');
						li
							.on('click', function (event) {
								event.preventDefault();
								var contentGroupName = $(this).attr('data-content-group');
								contentGroupMgr.showContentGroupModal(panel, pageID, appID, contentGroupName);
							})
							.button({
								icons: {
									primary: 'ui-icon-newwin'
								}
							})
						;
						list.append(li);
					});
					working.appendTo(list);

					//initialize refresh content group links
					var contentGroupModal = $('.content-group-modal', panel);
					contentGroupModal.on('click.refreshContentGroup', '.refresh-content-group', { panel: panel, appID: appID, pageID: pageID },
							function refreshContentGroup(event) {
								event.preventDefault();
								var contentGroupName = $(this).closest('.content-group-container').attr('data-group-name');
								contentGroupMgr.showContentGroupModal(event.data.panel, event.data.pageID, event.data.appID, contentGroupName);
							});

					contentGroupMgr.hideContentGroupModal(contentGroupModal);//necessary for when user changes a template for a page and the content groups change

					//set up choose existing content links
					panel.on('click', '.choose-content-link', function (event) {
						event.preventDefault();
						var link = $(this),
							contentGroupMemberID = link.attr('data-content-group-member-id');
						ajaxHelper.ajax('/Admin/ContentGroup/AddContentToContentGroupMember', {
							type: 'POST',
							data: {
								contentID: link.attr('data-content-id'),
								contentGroupMemberID: contentGroupMemberID
							},
							success: function (data, textStatus, jqXHR) {
								var $data = $(data);
								if ($data.find('.error').length > 0) {
									//an error message has returned
									link.closest('.shared-content-list').replaceWith($data);
								}
								else {
									var contentDisplay = link.closest('.content-display'),
										contentTypeName = $('.content-display', $data).attr('data-content-type'),
										groupContentDiv = $('.content-editor', $data),
										contentGroupModal = link.closest('.content-group-modal'),
										contentMenuItem = contentGroupMgr.findGroupMemberMenuItem(contentGroupModal, contentGroupMemberID);

									if (contentMenuItem.length > 0) {
										contentMenuItem.text(contentGroupMgr.getContentDescription($data));
									}

									contentGroupMgr.initEditInPlaceForContent(groupContentDiv);
									contentGroupMgr.initContentPropertiesPanel(groupContentDiv);
									contentGroupMgr.toggleNoContentMessage(contentGroupModal);
									contentDisplay.replaceWith($data);
									contentGroupMgr.initDeleteRemoveContentLinks(panel, contentGroupModal);
									appViewModel.registerUnappliedChange();
								}
							}
						});
					});

					//hide the content group 

					//list.attr('data-controller-display-name', controllerDisplayName); not sure if this is needed
					contentGroupMgr.hideWorking(panel);
				},
				failure: function (data, textStatus, jqXHR, formattedMessage) {
					//callback for handling failure, i.e. data.Succeeded === false
					contentGroupMgr.hideWorking(panel);
				},
				error: function (jqXHR, textStatus, errorThrown, formattedMessage) {
					contentGroupMgr.hideWorking(panel);
				},
				failureDoAlert: true,
				errorDoAlert: true
			});
		}
		else {
			list.append('This page doesn\'t have any content groups.');
			contentGroupMgr.hideWorking(panel);
		}
	}

	this.showContentGroupModal = function (panel, pageID, appID, contentGroupName) {
		contentGroupMgr.showWorking(panel);
		ajaxHelper.ajax('/Admin/ContentGroup/Index', {
			data: { pageID: pageID, appID: appID, group: contentGroupName },
			success: function (data, textStatus, jqXHR) {
				var contentGroupModal = panel.find('.content-group-modal');
				contentGroupModal.html(data);
				//initialize edit in place
				contentGroupModal.find('.content-editor').each(function (index, element) {
					contentGroupMgr.initEditInPlaceForContent(element);
					contentGroupMgr.initContentPropertiesPanel(element);
				});
				//initialize group menu scrolling when clicked
				contentGroupModal.find('.group-menu-items ol.sortable li a').each(function (index, anchor) {
					contentGroupMgr.bindGroupMenuItemClick(contentGroupModal, anchor)
				});
				//if this group's of type List then implement drag/drop sorting for menu
				var groupMenu = contentGroupModal.find('.content-group-menu');
				var groupContainer = contentGroupModal.find('.content-group-container');
				var groupType = groupContainer.attr('data-group-type');
				if (groupType == 'List') {
					var deleteContentDrop = $('.delete-content-drop', contentGroupModal),
						sortableList = contentGroupModal.find('.group-menu-items .sortable');
					sortableList.sortable({
						placeholder: 'ui-state-highlight',
						connectWith: deleteContentDrop,
						update: function (event, ui) {
							var groupMembers = [];
							$(ui.item).parent().children().each(function (index, item) {
								groupMembers.push({ ContentGroupMemberID: $(item).find('a').attr('data-group-member-id'), DisplayOrder: index });
							});
							if (groupMembers.length > 1) {
								//this update handler gets called when an item is dragged out of this list
								contentGroupMgr.handleGroupMemberOrderChange(panel, contentGroupModal, groupMembers, sortableList);
							}
						}
					});
					deleteContentDrop.sortable({
						receive: function (event, ui) {
							var theUI = ui;
							contentGroupMgr.showWorking(panel);
							ajaxHelper.ajax('/Admin/ContentGroup/DeleteGroupMembers', {
								data: { contentGroupMemberID: ui.item.find('a').attr('data-group-member-id'), deleteAssociatedContent: true },
								type: 'POST',
								success: function (data, textStatus, jqXHR) {
									var menuItem = $(theUI.item),
										groupMemberID = $(theUI.item).find('a').attr('data-group-member-id'),
										editor = $('#edit-group-member-' + groupMemberID, panel);
									contentGroupMgr.removeContentEditor(panel, menuItem, editor, function () {
										contentGroupMgr.toggleNoContentMessage(contentGroupModal);
										contentGroupMgr.hideWorking(panel);
										appViewModel.registerUnappliedChange();
									});
								},
								failure: function (data, textStatus, jqXHR, formattedMessage) {
									sortableList.sortable('cancel');
									contentGroupMgr.hideWorking(panel);
								},
								failureMessageFormat: 'An error occurred trying to delete the content group member: [[errorThrown]]',
								error: function (jqXHR, textStatus, errorThrown, formattedMessage) {
									sortableList.sortable('cancel');
									contentGroupMgr.hideWorking(panel);
								},
								errorMessageFormat: 'An error occurred deleting the content group member: [[errorThrown]]'
							});
						}
					});
				}

				//initialize links for creating new/selecting existing content
				contentGroupMgr.initContentLinks(panel, contentGroupModal);
				//initialize link for applying content inheritance
				contentGroupModal.find('.content-inheritance-checkbox').on('change', function (event) {
					contentGroupMgr.handleApplyContentInheritance(panel, $(this));
				});
				//initialize close content group link
				contentGroupModal.find('.close-content-link')
				.button({
					icons: {
						primary: 'ui-icon-circle-close'
					},
					text: false
				})
				.on('click', function (event) {
					event.preventDefault();
					contentGroupMgr.hideContentGroupModal(contentGroupModal);
				});
				//initialize sticky float content group menu
				groupMenu.stickyfloat(
				{
					duration: 200//,
					//easing: 'easeOutElastic'
				});
				//show content group
				//contentGroupModal.dialog('option', 'title', contentGroupName).dialog('open');
				contentGroupModal.fadeIn(function () {
					groupContainer.css('min-height', groupMenu.outerHeight(true).toString() + 'px');
				});
				contentGroupMgr.toggleNoContentMessage(contentGroupModal);
				contentGroupMgr.hideWorking(panel);
			},
			failure: function (data, textStatus, jqXHR, formattedMessage) {
				//callback for handling failure, i.e. data.Succeeded === false
				contentGroupMgr.hideWorking(panel);
			},
			error: function (jqXHR, textStatus, errorThrown, formattedMessage) {
				contentGroupMgr.hideWorking(panel);
			},
			errorMessageFormat: 'An error occurred loading the window for content group ' + contentGroupName + ': [[errorThrown]]'
		});
	};

	this.hideContentGroupModal = function (contentGroupModal) {
		contentGroupModal.fadeOut('fast', function () {
			contentGroupModal.empty();
		});
	};

	this.initContentLinks = function (panel, contentGroupModal) {
		contentGroupMgr.initCreateNewSelectExistingContentLinks(panel, contentGroupModal);
		contentGroupMgr.initDeleteRemoveContentLinks(panel, contentGroupModal);
	};

	this.initCreateNewSelectExistingContentLinks = function (panel, contentGroupModal) {
		//initialize any links for create new/select existing content
		contentGroupModal.find('.create-new-content').off('click').on('click', function (event) {
			event.preventDefault();
			contentGroupMgr.handleNewContentClick(panel, contentGroupModal, $(this));
		});
		contentGroupModal.find('.choose-existing-content').off('click').on('click', function (event) {
			event.preventDefault();
			contentGroupMgr.handleSelectExistingContentClick(panel, contentGroupModal, $(this));
		});
	};

	this.initDeleteRemoveContentLinks = function (panel, contentGroupModal) {
		contentGroupModal.find('.delete-content').on('click', function (event) {
			event.preventDefault();
			var link = $(this),
				isShared = link.attr('data-is-shared') === 'True',
				title = isShared ? 'Remove Content?' : 'Delete Content?',
				msg = isShared
					? 'Are you sure you want to <strong>remove</strong> this content? Note: Because it <em>is</em> shared, it <strong>will not</strong> be deleted permanently.'
					: 'Are you sure you want to <strong>delete</strong> this content? Note: Because it <em>is not</em> shared, it <strong>will</strong> be deleted permanently.'
				,
				contentGroupMemberID = link.attr('data-member-id'),
				displayTargetID = link.attr('data-display-target-id')
			;

			dialogHelper.confirm(msg, title, function () {
				contentGroupMgr.handleDeleteContentClick(panel, contentGroupModal, displayTargetID, contentGroupMemberID);
			});
		}).button({
			icons: {
				primary: 'ui-icon-trash'
			},
			text: false
		});
	};

	this.handleDeleteContentClick = function (panel, contentGroupModal, displayTargetID, contentGroupMemberID) {
		var displayTarget = contentGroupModal.find('#' + displayTargetID),
			successDone = function () {
				contentGroupMgr.toggleNoContentMessage(contentGroupModal);
				contentGroupMgr.hideWorking(panel);
				appViewModel.registerUnappliedChange();
			},
			menuItemLink = function () {
				return $('.content-group-modal', panel).find('.group-menu-items ol li a[data-group-member-id="' + contentGroupMemberID + '"]');
			}
		;

		contentGroupMgr.showWorking(panel);
		ajaxHelper.ajax('/Admin/ContentGroup/DeleteContentForMember', {
			type: 'POST',
			data: {
				contentGroupMemberID: contentGroupMemberID
			},
			success: function (data, textStatus, jqXHR) {
				if (data) {
					//deleted content from a fixed group; keep group member panel
					newEditor = $(data),
					contentGroupMemberID = newEditor.attr('data-group-member-id');
					displayTarget.replaceWith(newEditor); // html(newEditor.html());
					contentGroupMgr.initCreateNewSelectExistingContentLinks(panel, contentGroupModal);
					//set link text back to default for content menu item of deleted content 
					menuItemLink().text(newEditor.attr('data-group-member-name'));
					successDone();
				}
				else {
					//deleted content from a list group; remove entire group member panel
					var menuItem = menuItemLink().closest('li');
					contentGroupMgr.removeContentEditor(panel, menuItem, displayTarget, successDone);
				}
			},
			errorMessageFormat: 'An error occurred deleting the content: [[errorThrown]]',
			failure: function (data, textStatus, jqXHR, formattedMessage) {
				//callback for handling failure, i.e. data.Succeeded === false
				contentGroupMgr.hideWorking(panel);
			},
			error: function (jqXHR, textStatus, errorThrown, formattedMessage) {
				contentGroupMgr.hideWorking(panel);
			}
		});
	};

	this.removeContentEditor = function (context, menuItem, editor, callback) {
		var contentGroupEditorsElement = $('#content-group-editors', context);
		menuItem.fadeOut(250, function () {
			//remove the content menu item for the deleted content
			menuItem.remove();
			editor.fadeOut(250, function () {
				//remove the content editor for the deleted content
				editor.remove();
				//re-number the content editors for the remaining content group members
				$('div.content-display', contentGroupEditorsElement).each(function (index, element) {
					$(element).find('span.content-position').html(index + 1);
				});

				if ($.isFunction(callback)) {
					callback();
				}
			});
		});
	};

	this.handleNewContentClick = function (panel, contentGroupModal, linkClicked) {
		var displayTarget = contentGroupModal.find('#' + linkClicked.attr('data-display-target-id')),
			contentType = linkClicked.attr('data-content-type'),
			contentTypeName = linkClicked.attr('data-content-type-name'),
			contentGroupID = linkClicked.attr('data-content-group-id'),
			contentGroupMemberID = linkClicked.attr('data-member-id'),
			groupMemberConfigName = linkClicked.attr('data-group-member-config-name'),
			groupType = contentGroupModal.find('.content-group-container').attr('data-group-type'),
			targetAction = linkClicked.attr('data-target-action'),
			iconCssClass = linkClicked.attr('data-icon-css-class')
			;
		contentGroupMgr.showWorking(panel);
		ajaxHelper.ajax('/Admin/ContentGroup/NewContent', {
			data: {
				contentType: contentType,
				contentGroupMemberID: contentGroupMemberID,
				contentGroupID: contentGroupID,
				groupMemberConfigName: groupMemberConfigName
			},
			success: function (data, textStatus, jqXHR) {
				var editInPlaceContext,
					newEditor = $(data),
					description = contentGroupMgr.getContentDescription(newEditor);
				contentGroupMemberID = newEditor.attr('data-group-member-id');
				if (targetAction == 'appendTo') {
					newEditor.appendTo(displayTarget);
					editInPlaceContext = newEditor;
				}
				else {
					displayTarget.html(newEditor.html());
					editInPlaceContext = displayTarget;
				}
				contentGroupMgr.initEditInPlaceForContent($('.content-editor', editInPlaceContext));
				contentGroupMgr.initContentPropertiesPanel($('.content-editor', editInPlaceContext));
				contentGroupMgr.checkMenuItemExistsForNewContent(contentTypeName, iconCssClass, contentGroupModal, contentGroupMemberID, description);
				contentGroupMgr.initDeleteRemoveContentLinks(panel, contentGroupModal);
				contentGroupMgr.toggleNoContentMessage(contentGroupModal);
				contentGroupMgr.hideWorking(panel);
				appViewModel.registerUnappliedChange();
			},
			errorMessageFormat: 'An error occurred creating new content of type ' + contentType + ' for group member with ID ' + contentGroupMemberID + ': [[errorThrown]]',
			failure: function (data, textStatus, jqXHR, formattedMessage) {
				//callback for handling failure, i.e. data.Succeeded === false
				contentGroupMgr.hideWorking(panel);
			},
			error: function (jqXHR, textStatus, errorThrown, formattedMessage) {
				contentGroupMgr.hideWorking(panel);
			}
		});
	};

	this.getContentDescription = function (editorElement) {
		return editorElement.find('span.display-name').text();
	};

	this.toggleNoContentMessage = function (context) {
		var element = contentGroupMgr.getNoContentElement(context),
			numEditors = $('#content-group-editors', context).find('.content-display').length;
		if (numEditors > 0) {
			element.hide();
		}
		else {
			element.fadeIn();
		}
	};

	this.getNoContentElement = function (context) {
		return $('.no-content-message', context);
	};

	this.handleSelectExistingContentClick = function (panel, contentGroupModal, linkClicked) {
		var displayTargetID = linkClicked.attr('data-display-target-id'),
			displayTarget = contentGroupModal.find('#' + displayTargetID),
			contentType = linkClicked.attr('data-content-type'),
			contentGroupMemberID = linkClicked.attr('data-member-id'),
			contentGroupID = linkClicked.attr('data-content-group-id'),
			groupMemberConfigName = linkClicked.attr('data-group-member-config-name'),
			contentTypeName = linkClicked.attr('data-content-type-name'),
		//groupType = contentGroupModal.find('.content-group-container').attr('data-group-type'),
			targetAction = linkClicked.attr('data-target-action'),
			iconCssClass = linkClicked.attr('data-icon-css-class')
			;

		contentGroupMgr.showWorking(panel);
		ajaxHelper.ajax('/Admin/ContentGroup/ChooseExistingContent', {
			data: {
				contentType: contentType,
				contentGroupMemberID: contentGroupMemberID,
				contentGroupID: contentGroupID,
				groupMemberConfigName: groupMemberConfigName
			},
			success: function (data, textStatus, jqXHR) {
				var newEditor = $(data),
					description = newEditor.find('div span.display-name').text(),
					contentGroupMemberID = newEditor.attr('data-group-member-id');
				if (targetAction == 'appendTo') {
					newEditor.appendTo(displayTarget);
				}
				else {
					displayTarget.html(newEditor.html());
				}

				//init cancel link
				var listType = $('.content-group-container', contentGroupModal).attr('data-group-type');
				displayTarget.find('.cancel-choose-existing-content').on('click', function (event) {
					event.preventDefault();
					if (listType === 'Fixed') {
						contentGroupMgr.handleViewContentClick(panel, contentGroupModal, $(this));
					}
					else {
						contentGroupMgr.handleDeleteContentClick(panel, contentGroupModal, newEditor.attr('id'), contentGroupMemberID);
					}
				}).button();
				contentGroupMgr.checkMenuItemExistsForNewContent(contentTypeName, iconCssClass, contentGroupModal, contentGroupMemberID, description);
				contentGroupMgr.toggleNoContentMessage(contentGroupModal);
				contentGroupMgr.hideWorking(panel);
			},
			errorMessageFormat: 'An error occurred showing the select new content interface for content of type ' + contentType + ' for group member with ID ' + contentGroupMemberID + ': [[errorThrown]]',
			failure: function (data, textStatus, jqXHR, formattedMessage) {
				//callback for handling failure, i.e. data.Succeeded === false
				contentGroupMgr.hideWorking(panel);
			},
			error: function (jqXHR, textStatus, errorThrown, formattedMessage) {
				contentGroupMgr.hideWorking(panel);
			}
		});
	};

	this.handleViewContentClick = function (panel, contentGroupModal, linkClicked) {
		var displayTarget = contentGroupModal.find('#' + linkClicked.attr('data-display-target-id')),
			contentGroupMemberID = linkClicked.attr('data-member-id'),
			targetAction = linkClicked.attr('data-target-action'),
			iconCssClass = linkClicked.attr('data-icon-css-class')
			;
		contentGroupMgr.showWorking(panel);
		ajaxHelper.ajax('/Admin/ContentGroup/ViewContent', {
			data: {
				contentGroupMemberID: contentGroupMemberID
			},
			success: function (data, textStatus, jqXHR) {
				//var editInPlaceContext,
				newEditor = $(data),
				//description = newEditor.find('div span.display-name').text();
				contentGroupMemberID = newEditor.attr('data-group-member-id');
				if (targetAction == 'appendTo') {
					newEditor.appendTo(displayTarget);
					//editInPlaceContext = newEditor;
				}
				else {
					displayTarget.html(newEditor.html());
					//editInPlaceContext = displayTarget;
				}
				contentGroupMgr.initCreateNewSelectExistingContentLinks(panel, contentGroupModal);
				contentGroupMgr.initDeleteRemoveContentLinks(panel, contentGroupModal);
				contentGroupMgr.hideWorking(panel);
			},
			errorMessageFormat: 'An error occurred retrieving content view for group member with ID ' + contentGroupMemberID + ': [[errorThrown]]',
			failure: function (data, textStatus, jqXHR, formattedMessage) {
				//callback for handling failure, i.e. data.Succeeded === false
				contentGroupMgr.hideWorking(panel);
			},
			error: function (jqXHR, textStatus, errorThrown, formattedMessage) {
				contentGroupMgr.hideWorking(panel);
			}
		});
	};

	this.handleGroupMemberOrderChange = function (panel, contentGroupModal, groupMembers, list) {
		contentGroupMgr.showWorking(panel);
		ajaxHelper.ajax('/Admin/ContentGroup/ReorderGroupMembers?groupID=' + contentGroupModal.find('.content-group-container').attr('data-group-id'), {
			data: JSON.stringify(groupMembers),
			type: 'POST',
			contentType: "application/json; charset=utf-8",
			success: function (data, textStatus, jqXHR) {
				var orderedEditors = [],
					editorsContainer = contentGroupModal.find('#content-group-editors'),
					editor,
					i,
					noContentElement = contentGroupMgr.getNoContentElement(contentGroupModal);
				for (i = 0; i < groupMembers.length; i++) {
					var member = groupMembers[i];
					var id = member.ContentGroupMemberID;
					editor = contentGroupModal.find('#edit-group-member-' + id).clone(true, true);
					editor.find('span.content-position').html(i + 1);
					orderedEditors.push(editor);
				}

				editorsContainer.empty();
				for (i = 0; i < orderedEditors.length; i++) {
					editor = orderedEditors[i];
					contentGroupMgr.initEditInPlaceForContent($(editor).find('.content-editor'));
					contentGroupMgr.initContentPropertiesPanel($(editor).find('.content-editor'));
					editorsContainer.append(editor);
				}
				editorsContainer.prepend(noContentElement);
				contentGroupMgr.toggleNoContentMessage(contentGroupModal);
				contentGroupMgr.hideWorking(panel);
				appViewModel.registerUnappliedChange();
			},
			failure: function (data, textStatus, jqXHR, formattedMessage) {
				list.sortable('cancel');
				contentGroupMgr.hideWorking(panel);
			},
			failureMessageFormat: 'An error occurred trying to re-order the content: [[errorThrown]]',
			error: function (jqXHR, textStatus, errorThrown, formattedMessage) {
				list.sortable('cancel');
				contentGroupMgr.hideWorking(panel);
			},
			errorMessageFormat: 'An error occurred reordering the content group members: [[errorThrown]]'
		});
	};

	this.contentDescriptionEdited = function (element) {
		var editorParent = element.closest('.content-display'),
		groupMemberID = editorParent.attr('data-group-member-id'),
		contentGroupModal = element.closest('.content-group-modal');

		if (element.text()) {
			var newValue = element.text();
			var contentType = editorParent.attr('data-content-type');
			contentGroupMgr.setGroupMemberDisplayName(contentGroupModal, groupMemberID, newValue, editorParent);
		}
		else {
			var configMemberName = editorParent.attr('data-group-member-name');
			if (configMemberName) {
				contentGroupMgr.setGroupMemberDisplayName(contentGroupModal, groupMemberID, configMemberName, editorParent);
			}
		}
	};

	this.isSharedEdited = function (element) {
		var contentDisplayElement = element.closest('.content-display'),
			deleteLink = contentDisplayElement.find('.delete-content'),
			descriptionField = contentDisplayElement.find('div[data-property-name="Description"]'),
			description = $.trim(descriptionField.text()),
			isShared = element.text() === 'Yes';
		if (deleteLink.length > 0) {
			deleteLink.attr('data-is-shared', isShared ? 'True' : 'False');
		}
		if ((!description || description === 'Click to edit') && isShared) {
			dialogHelper.open(
				'When you share a piece of content we recommend you give it a Content Description so it will be easy to find.',
				'Give Shared Content a Description',
				null,
				{
					'OK': function () {
						dialogHelper.close();
						descriptionField.click();
					}
				}
			);
		}
	};

	this.setGroupMemberDisplayName = function(contentGroupModal, groupMemberID, newText, editorParent) {
		var contentMenuItem = contentGroupMgr.findGroupMemberMenuItem(contentGroupModal, groupMemberID);
		if (contentMenuItem && contentMenuItem.length > 0) {
			contentMenuItem.text(newText);
			editorParent.find('div span.display-name').text(newText);
		}
	}

	this.initEditInPlaceForContent = function (element) {
		element = $(element);
		var contentid = element.attr('data-content-id'),
			editMgr = new EditableManager(element, { contentid: contentid }, '/Admin/ContentGroup/Edit');
		//-- Initialize editable types for CMS content
		editMgr.initTypesForContent();
	};

	this.initContentPropertiesPanel = function (container) {
		$(container).find('.content-properties').accordion({
			collapsible: true,
			active: false,
			clearStyle: true
		});
	};

	this.checkMenuItemExistsForNewContent = function (contentTypeName, iconCssClass, contentGroupModal, contentGroupMemberID, description) {
		if (contentGroupMgr.findGroupMemberMenuItem(contentGroupModal, contentGroupMemberID).length <= 0) {
			var anchor = $('<a data-group-member-id="' + contentGroupMemberID + '" href="#">' + description + '</a>');
			contentGroupMgr.bindGroupMenuItemClick(contentGroupModal, anchor);
			var listItem = $('<li class="ui-state-default ui-corner-all"><span title="' + contentTypeName + '" class="ui-icon ' + iconCssClass + '"></span></li>');
			listItem.append(anchor);
			contentGroupModal.find('.group-menu-items ol').append(listItem);
			anchor.click();
		}
	};

	this.bindGroupMenuItemClick = function (contentGroupModal, anchor) {
		//this binds the click event to the content menu item in the content group modal so it scrolls to the correct piece of content when clicked
		$(anchor).off('click').on('click', function (event) {
			event.preventDefault();
			var groupMemberID = $(anchor).attr('data-group-member-id'),
			scrollTo = contentGroupModal.find('#group-member-' + groupMemberID).offset().top - 25;
			$('html, body').animate({
				scrollTop: scrollTo
			}, 90);
		});
	};

	this.findGroupMemberMenuItem = function (contentGroupModal, contentGroupMemberID) {
		return contentGroupModal.find('.group-menu-items ol li a[data-group-member-id="' + contentGroupMemberID + '"]');
	};

	this.handleApplyContentInheritance = function (panel, checkboxElement) {
		var checkbox = $(checkboxElement),
			contentGroupID = checkbox.attr('data-content-group-id'),
			currentPageID = checkbox.attr('data-page-id'),
			currentAppID = checkbox.attr('data-app-id'),
			applyInheritance = (checkbox.attr('checked') === 'checked');
		contentGroupMgr.showWorking(panel);
		ajaxHelper.ajax('/Admin/ContentGroup/ApplyInheritance', {
			data: {
				contentGroupID: contentGroupID,
				currentPageID: currentPageID,
				currentAppID: currentAppID,
				applyInheritance: applyInheritance
			},
			type: 'POST',
			success: function (data, textStatus, jqXHR) {
				appViewModel.registerUnappliedChange();
				contentGroupMgr.hideWorking(panel);
			},
			failure: function () {
				contentGroupMgr.hideWorking(panel);
			},
			error: function () {
				contentGroupMgr.hideWorking(panel);
			}
		});
	}
}
