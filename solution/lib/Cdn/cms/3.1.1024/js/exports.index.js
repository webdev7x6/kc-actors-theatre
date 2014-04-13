/// <reference path="http://clickfarmcdn.localhost/common/js/jquery/1.7.2/jquery-vsdoc.js" />
/// <reference path="http://clickfarmcdn.localhost/common/js/kendo/2012.2.710/kendo.all-vsdoc.js" />
/// <reference path="jquery.ui.dialog.js" />

$(function () {
    $('#exports-index-tabs').tabs();
    
    $('a.parameterized-export').on('click', function () {
        var queryStringSeparator = '?';
        var $link = $(this);
        var url = $link.data('url');
        var params = $link.data('parameters').slice(0);

        var curParam = params.shift();
        
        var onClose = function () {
            var $paramVal = $('#export-parameter-value');
            //TODO Validate value using curParam.DataType
            url = url + queryStringSeparator + curParam.Name + '=' + $paramVal.val();
            queryStringSeparator = '&';

            if (params.length > 0) {
                curParam = params.shift();
                $('#export-parameter-dialog').dialog('open');
            } else {
                window.location = url;
            }
        };

        $('#export-parameter-dialog').dialog(
        {
            title: "Export Parameter",
            buttons: {
                "Ok": function () {
                    $('#export-parameter-dialog').dialog('close');
                }
            },
            open: function () {
                $('#export-parameter-label').text(curParam.Prompt);
                $('#export-parameter-value').val('');
            },
            close: onClose
        });



    });
});
