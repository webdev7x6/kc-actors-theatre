//The cms objec is the root global object for centralizing cms-related javascript functionality.
//Add objects onto cms related to specific areas of the CMS, e.g. cms.url, cms.menuitem, etc.
//These additions should be kept in separate files (see cms.url.js for an example) 

(function ($, undefined) {
    var cms = window.cms = window.cms || {};

    (function () {
        cms.findTabPanelForPage = function (pageID) {
            return $('div.app-index-tab[data-page-id="' + pageID.toString() + '"]');
        };

        cms.makeCloseButton = function () {
            $.each(arguments, function (index, selector) {
                $(selector).button({
                    icons: {
                        primary: 'ui-icon-circle-close'
                    },
                    text: false
                });
            });
        };

        cms.doCallback = function (callback, args) {
            if (callback && $.isFunction(callback)) {
                callback(args);
            }
        };

        cms.buttonizeDeleteLinks = function (domContainer, selector) {
            $(domContainer).find(selector).each(function (index, link) {
                var lnk = $(link);
                lnk.button({
                    icons: {
                        primary: 'ui-icon-trash'
                    },
                    text: false
                });
            });
        };

        cms.truncateStr = function (str, cutoff) {
            if (str.indexOf(' ', cutoff) > -1) {
                str = $.trim(str).substring(0, str.indexOf(' ', cutoff)) + '...';
            }
            return str;
        };

        cms.parseCallback = function (callback) {
            var cb = null,
                t
            ;
            if (callback) {
                if (window[callback]) {
                    cb = window[callback];
                }
                else {
                    try {
                        t = eval(callback);
                    }
                    catch (e) { }
                    if ($.isFunction(t)) {
                        cb = t;
                    }
                }
            }
            return cb;
        };

        cms.parseParamsString = function (a) {
            var ch = a.charAt(0);
            if (ch === '?' || ch === '#') {
                a = a.substr(1);
            }
            a = a.split('&');
            if (a === '') return {};
            var b = {};
            for (var i = 0; i < a.length; ++i) {
                var p = a[i].split('=');
                if (p.length != 2) continue;
                b[p[0]] = decodeURIComponent(p[1].replace(/\+/g, ' '));
            }
            return b;
        };
        cms.parseParamsString.queryString = cms.parseParamsString(window.location.search);
        cms.parseParamsString.fragment = cms.parseParamsString(window.location.hash);

        //based on http://stackoverflow.com/a/614397/3447 and http://textmechanic.com/Remove-Letter-Accents.html
        //BA 2012-11-30
        cms.removeDiacritics = (function () {
            var patternLetters = /[ÀÁÂÃÄÅàáâãäåÇçÐÈÉÊËèéêëðÌÍÎÏìíîïÑñÒÓÔÕÕÖØòóôõöøŠšÙÚÛÜùúûüŸÿýŽž]/g,
                lookupLetters = {
                    'À': 'A', 'Á': 'A', 'Â': 'A', 'Ã': 'A', 'Ä': 'A', 'Å': 'A', 'à': 'a', 'á': 'a',
                    'â': 'a', 'ã': 'a', 'ä': 'a', 'å': 'a', 'Ç': 'C', 'ç': 'c', 'Ð': 'D', 'È': 'E',
                    'É': 'E', 'Ê': 'E', 'Ë': 'E', 'è': 'e', 'é': 'e', 'ê': 'e', 'ë': 'e', 'ð': 'e',
                    'Ì': 'I', 'Í': 'I', 'Î': 'I', 'Ï': 'I', 'ì': 'i', 'í': 'i', 'î': 'i', 'ï': 'i',
                    'Ñ': 'N', 'ñ': 'n', 'Ò': 'O', 'Ó': 'O', 'Ô': 'O', 'Õ': 'O', 'Õ': 'O', 'Ö': 'O',
                    'Ø': 'O', 'ò': 'o', 'ó': 'o', 'ô': 'o', 'õ': 'o', 'ö': 'o', 'ø': 'o', 'Š': 'S',
                    'š': 's', 'Ù': 'U', 'Ú': 'U', 'Û': 'U', 'Ü': 'U', 'ù': 'u', 'ú': 'u', 'û': 'u',
                    'ü': 'u', 'Ÿ': 'Y', 'ÿ': 'y', 'ý': 'y', 'Ž': 'Z', 'ž': 'z'
                },
                letterTranslator = function (match) {
                    return lookupLetters[match] || match;
                }
            ;

            return function (strIn) {
                //console.log(strIn)
                return strIn.replace(patternLetters, letterTranslator);
            }
        })();
    })();
})(jQuery);
