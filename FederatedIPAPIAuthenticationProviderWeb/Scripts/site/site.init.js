
(function (global, factory) {
    typeof exports === 'object' && typeof module !== 'undefined' ? factory(exports, require('jquery')) :
        typeof define === 'function' && define.amd ? define(['exports', 'jquery'], factory) :
            (global = typeof globalThis !== 'undefined' ? globalThis : global || self, factory(global.ajax_config = {}, global.jQuery));
}(this, (function (exports, $) {
    'use strict';
    
    var siteInit = function (options) {
        options = $.extend({
            siteMap: null,
            authenticated: false,
            sessionTimeout: 15*60*60,
            sessionWarning: 2
        }, options || {}, true);
        if (options.authenticated) {
            var warningIntervalMiliseconds = $.countDown.getTimeout({
                minutes: options.sessionWarning
            });
            var warningInterval = $.countDown.getDisplay(warningIntervalMiliseconds);
            var intervalEvents = {};
            intervalEvents[warningInterval] = function (countdown) {
                $.modalAlert.sessionTimeout.warning({
                    template: '<p class="text-center w-100 mb-0">System will automatically logout in: </p><div class="logout-countdown display-4 text-center w-100"><i class="fa fa-sync fa-spin"></i> Loading Counter</div><p class="text-center w-100 mb-0">Do you wish to renew your current session?</p>',
                    actions: [{
                        title: 'Yes',
                        classes: 'btn btn-sm btn-secondary text-capitalize',
                        icon: 'fa fa-check-circle',
                        init: function (button) {
                            button.one('click', function () {
                                countdown.pause();
                                $.siteMap.Default.Home.RenewSessionPOST.post().done(function () {
                                    $.modalAlert.sessionTimeout.hide();
                                });
                            });
                        }
                    }, {
                        title: 'No',
                        classes: 'btn btn-sm btn-secondary text-capitalize',
                        icon: 'fa fa-time-circle',
                        init: function (button) {
                            button.one('click', function () {
                                countdown.stop();
                            });
                        }
                    }]
                });
            };
            var sessionTimoutCountDown = $('.logout-countdown').countDown({
                targetsSelector: '.logout-countdown',
                milliseconds: options.sessionTimeout,
                intervalEvents: intervalEvents,
                done: function () {
                    $.siteMap.Default.Home.LogoutGET.go();
                }
            });

            var logoff = false;
            $.configureAjaxSetup({
                siteMap: options.siteMap,
                errorResponseHandlers: {
                    401: function () {
                        logoff = true;
                    },
                    403: function () {
                        alert('403');
                    },
                    404: function () {
                        alert('404');
                    }
                },
                done: function (event, request, settings) {                    
                    if (logoff === true) {
                        sessionTimoutCountDown.stop();
                    } else {
                        if (typeof request.responseJSON != 'undefined' && typeof request.responseJSON.sessionTimeout != 'undefined') {
                            sessionTimoutCountDown.reset(request.responseJSON.sessionTimeout);
                        } else {
                            sessionTimoutCountDown.reset();
                        }                        
                        $.modalAlert.sessionTimeout.hide();
                    }

                }
            });
        } else {
            $.configureAjaxSetup({
                siteMap: options.siteMap
            });
        }
    };


    var _initialized = false;
    var _init = function (options) {
        if (_initialized === false) {
            siteInit(options);
            _initialized = true;
        }
    };
    $.siteInit = _init
})));
