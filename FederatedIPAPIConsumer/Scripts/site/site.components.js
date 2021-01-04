
(function (global, factory) {
    typeof exports === 'object' && typeof module !== 'undefined' ? factory(exports, require('jquery')) :
        typeof define === 'function' && define.amd ? define(['exports', 'jquery'], factory) :
            (global = typeof globalThis !== 'undefined' ? globalThis : global || self, factory(global.ajax_config = {}, global.jQuery));
}(this, (function (exports, $) {
    'use strict';

    $.fn.actionButton = function (options) {
        var container = this;
        options = $.extend({
            title: 'default',
            classes: 'btn btn-primary',
            icon: null,
            link: null,
            init: function () { }
        }, options || {}, true);
        var content = options.title;
        if (typeof options.icon === 'string') {
            content = '<span><i class="' + options.icon + '"></i></span><span class="ml-1">' + options.title + '</span>';
        }
        var template = '<button type="button" class="' + options.classes + '">' + content + '</button>';
        if (typeof options.link === 'string') {
            template = '<a href="' + options.link + '" class="' + options.classes + '">' + content + '</a>';
        }
        var action = $(template);
        container.append(action);
        if (typeof options.init === 'function') {
            options.init(action, container);
        }
    };

    $.fn.modalAlertDefaults = {
        types: [
            { name: 'primary', icon: 'fa fa-question' },
            { name: 'secondary', icon: 'fa fa-question' },
            { name: 'info', icon: 'fa fa-info' },
            { name: 'success', icon: 'fa fa-check' },
            { name: 'warning', icon: 'fa fa-exclamation' },
            { name: 'danger', icon: 'fa fa-times' }]
    };
    $.modalAlert = {};
    var modalAlert = function (modalEl, config) {
        config = $.extend({
            name: 'default'
        }, config || {}, true);
        config = $.extend(config, modalEl.data(), true);
        modalEl.modal({
            keyboard: false,
            backdrop: 'static',
            show: false
        });

        $.modalAlert[config.name] = function (options) {
            options = $.extend({
                title: null,
                message: '',
                template: null,
                type: 'info',
                size: null,
                icon: 'fa fa-info',
                actions: []

            }, options || {});
            if (options.title === null || options.title.trim() === '') {
                options.title = options.type;
            }
            var size = 'modal-' + options.size;
            if (options.size !== 'sm' && options.size !== 'lg' && options.size !== 'xl') {
                size = '';
            }
            if (typeof options.template !== 'string') {
                options.template = '<p>' + options.message + '</p>';
            }
            var renderModal = function () {
                modalEl.find('.modal-dialog').attr('class', 'modal-dialog modal-dialog-centered modal-dialog-scrollable ' + size);
                modalEl.find('.modal-header').attr('class', 'modal-header py-1 bg-' + options.type);
                modalEl.find('.modal-content').attr('class', 'modal-content border-' + options.type);
                modalEl.find('.modal-title').text(options.title).attr('class', 'modal-title text-capitalize text-' + options.type + '-inverse');
                modalEl.find('.modal-icon-bg').attr('class', 'modal-icon-bg fa-stack-2x fa fa-circle text-' + options.type + '-inverse');
                modalEl.find('.modal-icon').attr('class', 'modal-icon fa-stack-1x ' + options.icon + ' text-' + options.type);
                modalEl.find('.modal-content-icon-bg').attr('class', 'modal-content-icon-bg fa fa-circle fa-stack-2x text-' + options.type);
                modalEl.find('.modal-content-icon').attr('class', 'modal-content-icon fa-stack-1x fa-inverse ' + options.icon);
                modalEl.find('.modal-message').empty().html(options.template);
                modalEl.find('.modal-footer').empty();
                if (Array.isArray(options.actions)) {
                    $.each(options.actions, function (i, actionOptions) {
                        modalEl.find('.modal-footer').actionButton(actionOptions);
                    });
                }

                modalEl.modal('show');
            };

            if (modalEl.hasClass('show')) {
                modalEl.one('hidden.bs.modal', renderModal);
                modalEl.modal('hide');
            } else {
                renderModal();
            }
        };
        $.modalAlert[config.name].hide = function () {
            modalEl.one('hidden.bs.modal', function () {
                modalEl.find('.modal-message').empty();
                modalEl.find('.modal-footer').empty();
            });
            modalEl.modal('hide');
        };
        var modalAlertType = function (options, type, icon) {
            options = options || {};
            options.type = type;
            options.icon = icon;
            $.modalAlert[config.name](options);
        };
        $.each($.fn.modalAlertDefaults.types, function (i, type) {
            $.modalAlert[config.name][type.name] = function (options) {
                modalAlertType(options, type.name, type.icon);
            };
        });
    };
    $.fn.modalAlert = function (options) {
        $.each(this, function (i, modal) {
            modalAlert($(modal), options);
        });
    };
    
    $.countDown = {
        getTimeout: function (options) {
            options = $.extend({
                days: 0,
                hours: 0,
                minutes: 0,
                seconds: 0,
                milliseconds: 0
            }, options || {}, true);
            var addDays = options.days * (24 * 60 * 60 * 1000);
            var addHours = options.hours * (60 * 60 * 1000);
            var addMinutes = options.minutes * (60 * 1000);
            var addSeconds = options.seconds * (1000);

            return addDays + addHours + addMinutes + addSeconds + options.milliseconds;
        },
        getDisplay: function (distance) {
            var daysDisplay = Math.floor(distance / (1000 * 60 * 60 * 24));
            var hoursDisplay = Math.floor((distance % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60));
            var minutesDisplay = Math.floor((distance % (1000 * 60 * 60)) / (1000 * 60));
            var secondsDisplay = Math.floor((distance % (1000 * 60)) / 1000);
            var showDays = daysDisplay > 0;
            var showHours = showDays || hoursDisplay > 0;
            var showMinutes = showHours || minutesDisplay > 0;
            if (secondsDisplay < 10) {
                secondsDisplay = '0' + secondsDisplay;
            }
            if (minutesDisplay < 10) {
                minutesDisplay = '0' + minutesDisplay;
            }

            var display = secondsDisplay;
            if (showMinutes) {
                display = minutesDisplay + ':' + display;
            }
            if (showHours) {
                display = hoursDisplay + ':' + display;
            }
            if (showDays) {
                display = daysDisplay + ':' + display;
            }
            return display;
        }
    };
    $.fn.countDown = function (options) {
        var $targets = this;
        options = $.extend({
            targetsSelector: null,
            days: 0,
            hours: 0,
            minutes: 0,
            seconds: 0,
            milliseconds: 0,
            intervalEvents: {},
            done: function () { }
        }, options || {}, true);

        var timeout = $.countDown.getTimeout(options);
        // Update the count down every 1 second
        var pause = false;
        var intervalCount = 0;
        var updateTargets = function () {
            var display = $.countDown.getDisplay(timeout - intervalCount);
            if (typeof options.targetsSelector === 'string') {
                $targets = $(options.targetsSelector);
            }
            $targets.text(display);
            if (typeof options.intervalEvents[display] === 'function') {
                options.intervalEvents[display]({
                    reset: function (timeoutReset) {
                        pause = true;
                        if (typeof timeoutReset !== 'undefined') {
                            timeout = timeoutReset;
                        }   
                        intervalCount = 0;
                        pause = false;
                        updateTargets();
                    },
                    pause: function () {
                        pause = true;
                    },
                    resume: function () {
                        pause = false;
                    },
                    stop: function () {
                        pause = true;
                        clearInterval(interval);
                        if (typeof options.done === 'function') {
                            options.done();
                        }
                    }
                });
            }
        };
        updateTargets();
        var interval = setInterval(function () {
            if (pause === false) {
                intervalCount += 1000;

                var distance = timeout - intervalCount;
                // If the count down is finished, write some text
                if (distance < 0) {
                    clearInterval(interval);
                    if (typeof options.done === 'function') {
                        options.done();
                    }
                } else {
                    // Time calculations for days, hours, minutes and seconds
                    updateTargets();
                }
            }
        }, 1000);
        return {
            reset: function (timeoutReset) {                
                pause = true;
                if (typeof timeoutReset !== 'undefined') {
                    timeout = timeoutReset;
                }                
                intervalCount = 0;
                pause = false;
                updateTargets();
            },
            pause: function () {
                pause = true;
            },
            resume: function () {
                pause = false;
            },
            stop: function () {
                pause = true;
                clearInterval(interval);
                if (typeof options.done === 'function') {
                    options.done();
                }
            }
        }
    };

    var ajaxFrom = function (form) {
        form.data('_submitting', false);
        form.on('submit', function (e) {
            form.addClass('submitting');
            e.preventDefault();
            if (form.data('_submitting') === false) {
                form.data('_submitting', true);
                var formData = $.extend({
                    action: form.attr('action')
                }, form.data());
                var data = form.serialize();
                $.post(formData.action, data).done(function (html) {
                    form.html(html);
                }).always(function () {
                    form.data('_submitting', false);
                    //form.removeClass('submitting');
                });
            }            
        });
    };
    
    $.fn.ajaxFrom = function (options) {
        $.each(this, function (i, form) {
            ajaxFrom($(form));
        });
    };
    $('[data-ajax="form"]').ajaxFrom();
    $('[data-modal="alert"]').modalAlert();
})));
