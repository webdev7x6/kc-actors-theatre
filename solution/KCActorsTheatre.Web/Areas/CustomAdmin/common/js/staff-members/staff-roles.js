/// <reference path="http://ajax.aspnetcdn.com/ajax/jQuery/jquery-1.8.3-vsdoc.js" />
/// <reference path="http://clickfarmcdn.localhost/common/js/knockout/2.1.0/knockout.js" />
/// <reference path="http://clickfarmcdn.localhost/common/js/kendo/2012.2.710/kendo.all-vsdoc.js" />

(function ($, undefined) {
    var admin = window.admin = window.admin || {};
    admin.staff = admin.staff = admin.staff || {};
    (function () {
        admin.staff.roles = (function () {
            window.MEMBER_ID = null;
            window.ROLE_TYPE = null;
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
                            var
                                type = link.attr('data-role-type'),
                                memberID = link.attr('data-member-id');
                            //if (type != 'Service') {
                                admin.staff.roles.addRole(type, memberID);
                            //}
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
                                memberID = link.attr('data-member-id'),
                                roleID = link.attr('data-role-id');
                            admin.staff.roles.deleteRole(memberID, roleID);
                        });
                    });

                },
                deleteRole: function (memberID, roleID) {
                    dialogHelper.confirm(
                        admin.deleteRoleDescription,
                        'Delete Role',
                        function () {
                            ajaxHelper.ajax(admin.deleteRoleURL, {
                                data: {
                                    memberID: memberID,
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
                addRole: function (type, memberID) {
                    window.ROLE_TYPE = type;
                    window.MEMBER_ID = memberID;
                    createRoleManager.showCreateRoleForm(type, memberID);
                }
            };
        })();
    })();
})(jQuery);