/// <reference path="http://ajax.aspnetcdn.com/ajax/jQuery/jquery-1.8.3-vsdoc.js" />
/// <reference path="http://clickfarmcdn.localhost/common/js/knockout/2.1.0/knockout.js" />
/// <reference path="http://clickfarmcdn.localhost/common/js/kendo/2012.2.710/kendo.all-vsdoc.js" />

(function ($, undefined) {
    var admin = window.admin = window.admin || {};
    admin.congregations = admin.congregations = admin.congregations || {};
    (function () {
        admin.congregations.contacts = (function () {
            var $dlg = null,
                closeCallback = null,
                $associatedContacts = null
            ;

            window.associatedContactsViewModel = {
                contacts: ko.observableArray([]),
                removeContact: function (contact, event) {
                    event.preventDefault();
                    dialogHelper.confirm(
                        'Are you sure you want to delete this congregation contact?',
                        'Remove Congregation Contact',
                        function () {
                            ajaxHelper.ajax('/CustomAdmin/Congregations/DeleteCongregationContactAjax', {
                                type: 'POST',
                                data: {
                                    id: contact.contactID
                                },
                                success: function (data, textStatus, jqXHR) {
                                    window.associatedContactsViewModel.contacts.remove(contact);
                                },
                                failureMessageFormat: 'An error occurred trying to delete the congregation contact: [[errorThrown]]'
                            });

                            
                        }
                    );            
                }
            };

            window.CONGREGATION_ID = null;

            return {
                init: function _init($modal) {
                    $associatedContacts = $('#contacts-tbody');
                    $dlg = $modal.dialog({
                        autoOpen: false,
                        modal: true,
                        resizable: false,
                        width: 930,
                        buttons: {
                            Close: function _closeBtn_onClick() {
                                $dlg.dialog('close');
                            }
                        },
                        close: function _dlg_onClose() {

                            var $list = $('.editable-parent[data-item-id="' + window.CONGREGATION_ID + '"] .edit-associated-contacts ul');
                            $list.html('');

                            $associatedContacts.find('tr').each(function _contacts_onEach(ix, el) {
                                var $el = $(el);
                                if ($el.css('display') != 'none') // only add if not hidden
                                {
                                    var id = $el.attr('data-contact-id'),
                                        name = $el.find('td [data-property-name="Name"]').text(),
                                        position = $el.find('td [data-property-name="Position"]').text(),
                                        email = $el.find('td [data-property-name="Email"]').text(),
                                        phone = $el.find('td [data-property-name="Phone"]').text()
                                    ;

                                    $list.append('<li data-contact-phone="' + phone + '" data-contact-email="' + email + '" data-contact-position="' + position + '" data-contact-id="' + id + '">' + name + '</li>');

                                }
                            });
                        }
                    });
                    ko.applyBindings(window.associatedContactsViewModel, $associatedContacts[0]);
                },
                initButtons: function _initButtons() {
                    $('.delete-contact-link').each(function (index, link) {
                        var lnk = $(link);
                        lnk.button({
                            icons: {
                                primary: 'ui-icon-trash'
                            },
                            text: false
                        });
                    });
                },

                initEditInPlace: function (contactTableRow) {
                    var contactEditMgr = new EditableManager($(contactTableRow), { contactID: $(contactTableRow).attr('data-contact-id') }, '/CustomAdmin/Congregations/EditContactInPlace');
                    contactEditMgr.initAllTypes();
                },

                // congregation contact list in edit view is clicked
                contactClick: function (container, parent) {
                    var
                        $container = $(container),
                        contacts = [],
                        $contacts = $container.find('ul')
                    ;

                    $contacts.find('li').each(function _contacts_onEach(ix, el) {
                        var $el = $(el);
                        contacts.push({
                            contactID: $el.attr('data-contact-id'),
                            name: $el.text(),
                            position: $el.attr('data-contact-position'),
                            email: $el.attr('data-contact-email'),
                            phone: $el.attr('data-contact-phone')
                        });
                    });

                    admin.congregations.contacts.show(contacts, parent.attr('data-item-id'));
                },

                // displays contact edit modal
                show: function _show(contacts, congregationID, onClose) {
                    associatedContactsViewModel.contacts([]);
                    $associatedContacts.empty();

                    // push each item to table
                    $.each(contacts, function _contacts_onEach(ix, item) {
                        associatedContactsViewModel.contacts.push(item);
                    });

                    // make each row editable
                    $('.contacts-table tbody tr').each(function (index, item) {
                        admin.congregations.contacts.initEditInPlace(item);
                    });

                    // make rows sortable
                    $('.contacts-table tbody').sortable({
                        handle: '.sort-order',
                        update: function (event, ui) {
                            var contactIDs = $(this).sortable('toArray');
                            admin.congregations.contacts.updateContactDisplayOrder(contactIDs);
                        }
                    });

                    admin.congregations.contacts.initButtons();
                    window.CONGREGATION_ID = congregationID;
                    closeCallback = onClose;
                    $dlg.dialog('open');
                },
                updateContactDisplayOrder: function (contactIDs) {
                    ajaxHelper.ajax('/CustomAdmin/Congregations/UpdateContactDisplayOrder', {
                        data: { contactIDs: contactIDs },
                        type: 'POST',
                        traditional: true,
                        failureMessageFormat: 'An error occurred trying to set the contacts display order: [[errorThrown]]'
                    });
                }

            };
        })();
    })();
})(jQuery);