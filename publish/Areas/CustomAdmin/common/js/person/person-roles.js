/// <reference path="http://ajax.aspnetcdn.com/ajax/jQuery/jquery-1.8.3-vsdoc.js" />
/// <reference path="http://clickfarmcdn.localhost/common/js/knockout/2.1.0/knockout.js" />
/// <reference path="http://clickfarmcdn.localhost/common/js/kendo/2012.2.710/kendo.all-vsdoc.js" />

(function ($, undefined) {
    var admin = window.admin = window.admin || {};
    admin.people = admin.people = admin.people || {};
    (function () {
        admin.people.roles = (function () {
            window.PERSON_ID = null;
            return {
                initButtons: function () {
                    $('.create-role').each(function (index, link) {
                        var link = $(link);
                        link.button({
                            icons: {
                                primary: 'ui-icon-plusthick'
                            }
                        }).on('click', function (e) {
                            e.preventDefault();
                            console.log(link);
                            var
                                personID = link.attr('data-person-id');
                                admin.people.roles.addRole(personID);
                        });
                    });

                    $('.delete-role-link').each(function (index, link) {
                        var link = $(link);
                        link.button({
                            icons: {
                                primary: 'ui-icon-trash'
                            },
                            text: false
                        }).on('click', function (e) {
                            e.preventDefault();
                            var
                                personID = link.attr('data-person-id'),
                                roleID = link.attr('data-role-id');
                            admin.people.roles.deleteRole(personID, roleID);
                        });
                    });

                },
                deleteRole: function (personID, roleID) {
                    dialogHelper.confirm(
                        admin.deleteRoleDescription,
                        'Delete Role',
                        function () {
                            ajaxHelper.ajax(admin.deleteRoleURL, {
                                data: {
                                    personID: personID,
                                    roleID: roleID
                                },
                                type: 'POST',
                                success: function (data, textStatus, jqXHR) {
                                    var $tr = $('tr[data-role-definition-id="' + roleID + '"]');
                                    $tr.remove();
                                },
                                failureMessageFormat: 'An error occurred trying to delete the Role: [[errorThrown]]'
                            });
                        }
                    );
                },
                addRole: function (personID) {
                    window.PERSON_ID = personID;
                    createRoleManager.showCreateRoleForm(personID);
                }
            };
        })();
    })();
})(jQuery);