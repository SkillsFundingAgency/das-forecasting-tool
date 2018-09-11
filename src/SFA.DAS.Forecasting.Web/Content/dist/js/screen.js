var sfa = sfa || {};

var chart = (function () { 

    var dataContainer = document.getElementsByClassName('chart-data')[0];

    var dates = dataContainer.dataset.dates.split(',');
    var values = dataContainer.dataset.values.split(',');

    var chart = c3.generate({
        bindto: '#chart',
        legend: { show: false },
        point: { show: false },
        interaction: { enabled: false },
        data: {
            x: 'x',
            columns: [
                ['x'].concat(dates),
                ['data1'].concat(values)
            ],
            type: 'spline',
            axes: { data1: 'y2' },
        },
        axis: {
            y2: {
                show: true,
                tick: {
                    format: d3.format(",")
                }
            },
            y: {
                show: true,
                label: {
                    text: "Future funds (\243)",
                    position: 'outer-middle',
                    rotate: 180
                }
            },
            x: {
                type: 'timeseries',
                tick: {
                    fit: true,
                    count: 6,
                    format: "%b %y"
                }
            }
        },
        padding: {
            
        },
    });

    document.getElementsByClassName('chart-container')[0].style.display = "block";

});

var tabs = (function () {

    $(".tabs-menu a").click(function (e) {
        e.preventDefault();
        $(this).parent().addClass("current");
        $(this).parent().siblings().removeClass("current");
        var tab = $(this).attr("href");
        $(".tab-content").not(tab).css("display", "none");
        $(tab).fadeIn();

        if (history.replaceState)
            history.replaceState(null, null, tab);
        else
            location.hash = tab;
    });
});

window.onload = function () {

    if (document.getElementsByClassName('chart-container').length !== 0)
    {
        chart();
    }

    tabs();

    tippy('.tippy',
        {
            arrow: true,
            size: 'large',
            duration: 0,
            offset: '0,10',
            theme: 'forecasting'
        });

    $('#BalanceSheet-Tables table.balancesheet a').click(function (e) {
        e.preventDefault();
    });
};

//OR use Object http://jsfiddle.net/etuwo8mz/57/
//http://stackabuse.com/how-to-format-dates-in-javascript/


sfa.navigation = {
    elems: {
        userNav: $('nav#user-nav > ul'),
        levyNav: $('ul#global-nav-links')
    },
    init: function () {
        this.setupMenus(this.elems.userNav);
        this.setupEvents(this.elems.userNav);
    },
    setupMenus: function (menu) {
        menu.find('ul').addClass("js-hidden").attr("aria-hidden", "true");
    },
    setupEvents: function (menu) {
        var that = this;
        menu.find('li.has-sub-menu > a').on('click', function (e) {
            var $that = $(this);
            that.toggleMenu($that, $that.next('ul'));
            e.stopPropagation();
            e.preventDefault();
        });
        // Focusout event on the links in the dropdown menu
        menu.find('li.has-sub-menu > ul > li > a').on('focusout', function (e) {
            // If its the last link in the drop down menu, then close
            var $that = $(this);
            if ($(this).parent().is(':last-child')) {
                that.toggleMenu($that, $that.next('ul'));
            }
        });

    },
    toggleMenu: function (link, subMenu) {
        var $li = link.parent();
        if ($li.hasClass("open")) {
            // Close menu
            $li.removeClass("open");
            subMenu.addClass("js-hidden").attr("aria-hidden", "true");
        } else {
            // Open menu
            this.closeAllOpenMenus();
            $li.addClass("open");
            subMenu.removeClass("js-hidden").attr("aria-hidden", "false");
        }
    },
    closeAllOpenMenus: function () {
        this.elems.userNav.find('li.has-sub-menu.open').removeClass('open').find('ul').addClass("js-hidden").attr("aria-hidden", "true");
        this.elems.levyNav.find('li.open').removeClass('open').addClass("js-hidden").attr("aria-hidden", "true");
    },
    linkSettings: function () {
        var $settingsLink = $('a#link-settings'),
            that = this;
        this.toggleUserMenu();
        $settingsLink.attr("aria-hidden", "false");
        $settingsLink.on('click touchstart', function (e) {
            var target = $(this).attr('href');
            $(this).toggleClass('open');
            that.toggleUserMenu();
            e.preventDefault();
        });
    },
    toggleUserMenu: function () {
        var $userNavParent = this.elems.userNav.parent();
        if ($userNavParent.hasClass("close")) {
            //open it
            $userNavParent.removeClass("close").attr("aria-hidden", "false");
        } else {
            // close it 
            $userNavParent.addClass("close").attr("aria-hidden", "true");
        }
    }
}

sfa.backLink = {
    init: function () {
        var backLink = $('<a>')
            .attr({ 'href': '#', 'class': 'link-back' })
            .text('Back')
            .on('click', function (e) { window.history.back(); e.preventDefault(); });
        $('#js-breadcrumbs').html(backLink);
    }
};

if ($('#js-breadcrumbs')) {
    sfa.backLink.init();
}


//floating menu
$(window).scroll(function () {
    if ($(window).scrollTop() >= 110) {
        $('.floating-menu').addClass('fixed-header');
        $('.js-float').addClass('width-adjust');
    }
    else {
        $('.floating-menu').removeClass('fixed-header');
        $('.js-float').removeClass('width-adjust');
    }
});

sfa.navigation.init();

$('ul#global-nav-links').collapsableNav();

$(function () {
    $('.error-summary-with-sticky-header a').on('click touch', function (e) {

        var target = $(this).prop('hash'),
            $target = $(target);

        if ($target.length === 0) {
            return;
        }

        window.scrollTo(0, $target.offset().top - 130);

        if ($target.is('input, select')) {
            $target.focus();
        }

        e.preventDefault();
    });
});