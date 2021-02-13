(function ($) {
    var substringMatcher = function (strs) {
        return function findMatches(q, cb) {
            var matches, substringRegex;
            matches = [];
            substrRegex = new RegExp(q, 'i');
            $.each(strs, function (i, str) {
                if (substrRegex.test(str)) {
                    matches.push(str);
                }
            });
            cb(matches);
        };
    };
    var states = ['Alabama', 'Alaska', 'Arizona', 'Arkansas', 'California',
        'Colorado', 'Connecticut', 'Delaware', 'Florida', 'Georgia', 'Hawaii',
        'Idaho', 'Illinois', 'Indiana', 'Iowa', 'Kansas', 'Kentucky', 'Louisiana',
        'Maine', 'Maryland', 'Massachusetts', 'Michigan', 'Minnesota',
        'Mississippi', 'Missouri', 'Montana', 'Nebraska', 'Nevada', 'New Hampshire',
        'New Jersey', 'New Mexico', 'New York', 'North Carolina', 'North Dakota',
        'Ohio', 'Oklahoma', 'Oregon', 'Pennsylvania', 'Rhode Island',
        'South Carolina', 'South Dakota', 'Tennessee', 'Texas', 'Utah', 'Vermont',
        'Virginia', 'Washington', 'West Virginia', 'Wisconsin', 'Wyoming'
    ];
    $('#the-basics .typeahead').typeahead({
        hint: true,
        highlight: true,
        minLength: 1
    },
        {
            name: 'states',
            source: substringMatcher(states)
        });
    var states = new Bloodhound({
        datumTokenizer: Bloodhound.tokenizers.whitespace,
        queryTokenizer: Bloodhound.tokenizers.whitespace,
        local: states
    });
    $('#bloodhound .typeahead').typeahead({
        hint: true,
        highlight: true,
        minLength: 1
    },
        {
            name: 'states',
            source: states
        });
    var countries = new Bloodhound({
        datumTokenizer: Bloodhound.tokenizers.whitespace,
        queryTokenizer: Bloodhound.tokenizers.whitespace,
        prefetch: '../assets/js/typeahead/data/countries.json'
    });
    $('#prefetch .typeahead').typeahead(null, {
        name: 'countries',
        source: countries
    });
    var bestPictures = new Bloodhound({
        datumTokenizer: Bloodhound.tokenizers.obj.whitespace('value'),
        queryTokenizer: Bloodhound.tokenizers.whitespace,
        prefetch: './../assets/js/typeahead/data/films/post_1960.json',
        remote: {
            url: '../assets/js/typeahead/data/films/queries/%QUERY.json',
            wildcard: '%QUERY'
        }
    });
    $('#remote .typeahead').typeahead(null, {
        name: 'best-pictures',
        display: 'value',
        source: bestPictures
    });
    var nflTeams = new Bloodhound({
        datumTokenizer: Bloodhound.tokenizers.obj.whitespace('team'),
        queryTokenizer: Bloodhound.tokenizers.whitespace,
        identify: function (obj) { return obj.team; },
        prefetch: '../assets/js/typeahead/data/nfl.json'
    });
    function nflTeamsWithDefaults(q, sync) {
        if (q === '') {
            sync(nflTeams.get('Detroit Lions', 'Green Bay Packers', 'Chicago Bears'));
        }
        else {
            nflTeams.search(q, sync);
        }
    }
    $('#default-suggestions .typeahead').typeahead({
        minLength: 0,
        highlight: true
    },
        {
            name: 'nfl-teams',
            display: 'team',
            source: nflTeamsWithDefaults
        });
    $('#custom-templates .typeahead').typeahead(null, {
        name: 'best-pictures',
        display: 'value',
        source: bestPictures,
        templates: {
            empty: [
                '<div class="empty-message">',
                'unable to find any Best Picture winners that match the current query',
                '</div>'
            ].join('\n'),
            suggestion: Handlebars.compile('<div><strong>{{value}}</strong> 鈥� {{year}}</div>')
        }
    });
    var nbaTeams = new Bloodhound({
        datumTokenizer: Bloodhound.tokenizers.obj.whitespace('team'),
        queryTokenizer: Bloodhound.tokenizers.whitespace,
        prefetch: '../assets/js/typeahead/data/nba.json'
    });
    var nhlTeams = new Bloodhound({
        datumTokenizer: Bloodhound.tokenizers.obj.whitespace('team'),
        queryTokenizer: Bloodhound.tokenizers.whitespace,
        prefetch: '../assets/js/typeahead/data/nhl.json'
    });
    $('#multiple-datasets .typeahead').typeahead({
        highlight: true
    },
        {
            name: 'nba-teams',
            display: 'team',
            source: nbaTeams,
            templates: {
                header: '<h3 class="league-name">NBA Teams</h3>'
            }
        },
        {
            name: 'nhl-teams',
            display: 'team',
            source: nhlTeams,
            templates: {
                header: '<h3 class="league-name">NHL Teams</h3>'
            }
        });
    $('#scrollable-dropdown-menu .typeahead').typeahead(null, {
        name: 'countries',
        limit: 10,
        source: countries
    });
    var arabicPhrases = new Bloodhound({
        datumTokenizer: Bloodhound.tokenizers.whitespace,
        queryTokenizer: Bloodhound.tokenizers.whitespace,
        local: [
            "India",
            "USA",
            "Australia",
            "UEA",
            "China"
        ]
    });
    $('#rtl-support .typeahead').typeahead({
        hint: false
    },
        {
            name: 'arabic-phrases',
            source: arabicPhrases
        });
})(jQuery);

$(document).ready(function () {
    var engine, remoteHost, template, empty;

    $.support.cors = true;

    remoteHost = 'https://typeahead-js-twitter-api-proxy.herokuapp.com';
    template = Handlebars.compile($(".result-template").html());
    empty = Handlebars.compile($(".empty-template").html());

    engine = new Bloodhound({
        identify: function (o) { return o.id_str; },
        queryTokenizer: Bloodhound.tokenizers.whitespace,
        datumTokenizer: Bloodhound.tokenizers.obj.whitespace('name', 'screen_name'),
        dupDetector: function (a, b) { return a.id_str === b.id_str; },
        prefetch: remoteHost + '/demo/prefetch',
        remote: {
            url: remoteHost + '/demo/search?q=%QUERY',
            wildcard: '%QUERY'
        }
    });

    // ensure default users are read on initialization
    engine.get('1090217586', '58502284', '10273252', '24477185')

    function engineWithDefaults(q, sync, async) {
        if (q === '') {
            sync(engine.get('1090217586', '58502284', '10273252', '24477185'));
            async([]);
        }

        else {
            engine.search(q, sync, async);
        }
    }

    $('.demo-input').typeahead({
        hint: $('.Typeahead-hint'),
        menu: $('.Typeahead-menu'),
        minLength: 0,
        classNames: {
            open: 'is-open',
            empty: 'is-empty',
            cursor: 'is-active',
            suggestion: 'Typeahead-suggestion',
            selectable: 'Typeahead-selectable'
        }
    }, {
        source: engineWithDefaults,
        displayKey: 'screen_name',
        templates: {
            suggestion: template,
            empty: empty
        }
    })
        .on('typeahead:asyncrequest', function () {
            $('.Typeahead-spinner').show();
        })
        .on('typeahead:asynccancel typeahead:asyncreceive', function () {
            $('.Typeahead-spinner').hide();
        });

});