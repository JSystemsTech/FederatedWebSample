
(function (global, factory) {
    typeof exports === 'object' && typeof module !== 'undefined' ? factory(exports, require('jquery')) :
        typeof define === 'function' && define.amd ? define(['exports', 'jquery'], factory) :
            (global = typeof globalThis !== 'undefined' ? globalThis : global || self, factory(global.ajax_config = {}, global.jQuery));
}(this, (function (exports, $) {
    'use strict';

    $.fn.ajaxSetupConfig = $.fn.ajaxSetupConfig || {
        defaults: {
            errorResponseHandlers: {
                400: function () { },
                401: function () { },
                402: function () { },
                403: function () { },
                404: function () { },
                405: function () { }
            },
            done: function () { },
            antiForgeryForm: '#__AntiForgeryForm',
            pageLoadLink: '#__PageLoadLink',
            siteMap: null
        },
        _init: function (options) {
            var options = $.extend($.fn.ajaxSetupConfig.defaults, options || {}, true);
            var antiForgeryData = $(options.antiForgeryForm).serializeArray()[0];
            
            var pageLoadLink = $(options.pageLoadLink);
            $.ajaxSetup({
                beforeSend: function (xhr) {
                    antiForgeryData = $(options.antiForgeryForm).serializeArray()[0];
                    xhr.setRequestHeader(antiForgeryData.name, antiForgeryData.value);
                }
            });
            $.ajaxPrefilter(function (prefilterOptions, originalOptions, jqXHR) {
                if (prefilterOptions.type.toLowerCase() === 'post') {
                    // initialize `data` to empty string if it does not exist
                    prefilterOptions.data = prefilterOptions.data || '';

                    // add leading ampersand if `data` is non-empty
                    prefilterOptions.data += prefilterOptions.data ? '&' : '';

                    // add _token entry
                    console.log(prefilterOptions.data);
                    antiForgeryData = $(options.antiForgeryForm).serializeArray()[0];
                    if (prefilterOptions.data.indexOf(antiForgeryData.name + '=') < 0) {
                        var tokenParam = antiForgeryData.name + '=' + antiForgeryData.value;
                        prefilterOptions.data += tokenParam;
                    }                  

                }
            });
            $(document).ajaxError(function (event, jqxhr, settings, thrownError) {
                var errorHandler = options.errorResponseHandlers[jqxhr.status];
                if (typeof errorHandler === 'function') {
                    errorHandler(event, jqxhr, settings, thrownError);
                }
            }).ajaxComplete(function (event, request, settings) {
                var handler = options.done;
                if (typeof handler === 'function') {
                    handler(event, request, settings);
                }
            });
            $.GoToLink = function (url, data) {
                var linkUrl = url;
                if (Array.isArray(data) || (typeof data === 'object' && data !== null && Object.keys(data).length > 0)) {
                    linkUrl = linkUrl + '?' + $.param(data);
                }
                pageLoadLink.attr("href", linkUrl);
                pageLoadLink[0].click();
            };
            $.loadPage = function (html) {
                var newDoc = document.open("text/html", "replace");
                newDoc.write(html);
                newDoc.close();
            };
            $.fn.loadPartial = function (html) {
                this.html(html);
                return this;
            };
            if (typeof options.siteMap === 'string') {
                $.siteMap = JSON.parse(options.siteMap);


                $.each($.siteMap, function (i, area) {
                    $.each(area, function (i, controller) {
                        $.each(controller, function (i, action) {
                            if (action.AjaxMethod === 'POST') {
                                action.post = function (options) {
                                    if (action.IsLink === true) {
                                        return $.post(action.Url, options).done(function (response) {
                                            $.loadPage(response);
                                        });
                                    }
                                    return $.post(action.Url, options);
                                }
                            } else if (action.IsLink === true) {
                                action.go = function (data) {
                                    $.GoToLink(action.Url, data);
                                }
                            } else {
                                action.get = function (options) {
                                    return $.get(action.Url, options);
                                }
                            }

                        });
                    });
                });
            }
        }
    };
    $.configureAjaxSetup = $.fn.ajaxSetupConfig._init;
    

})));
