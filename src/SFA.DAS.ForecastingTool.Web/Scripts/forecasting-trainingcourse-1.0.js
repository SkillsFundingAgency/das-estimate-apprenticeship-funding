(function () {
    init();

    function init() {
        addGrandTotalElement();

        $('form .grid-row .column-two-thirds').each(function (index, element) {
            var $container = $(element);

            addCostElement($container);
            attachHandlers($container);
        });
    }

    function addCostElement($container) {
        $container.append($('<div style="font-weight:700;">Cost</div><div class="cost">£<span class="price">0</span> over <span class="duration">0</span> months</div>'));
        updateApprenticeshipCost($container);
    }

    function addGrandTotalElement() {
        $('<div class="grand-total">Total cost: £<span>0</span></div>').insertBefore($('form > div:last-child'));
        updateGrandTotal();
    }

    function attachHandlers($container) {
        $container.find('select[name=standard]').change(inputChangedHandler);
        $container.find('input[name=cohorts]').change(inputChangedHandler);
    }

    function inputChangedHandler() {
        var $container = $(this).parent().parent().parent();
        updateApprenticeshipCost($container);
        updateGrandTotal();
    }

    function updateApprenticeshipCost($container) {
        var priceAndDuration = getApprenticeshipPriceAndDuration($container);

        $($container.find('div.cost > span.price')[0]).text(priceAndDuration.price.format(0));
        $($container.find('div.cost > span.duration')[0]).text(priceAndDuration.duration);
    }

    function updateGrandTotal() {
        var grandTotal = 0;

        $('form .grid-row .column-two-thirds').each(function (index, element) {
            var $container = $(element);
            var cost = getApprenticeshipCost($container);
            grandTotal += cost;
        });

        $('.grand-total > span').text(grandTotal.format(0));
    }

    function getApprenticeshipCost($container) {
        var apprenticeshipDetails = getApprenticeshipDetails($container);
        return apprenticeshipDetails.price * apprenticeshipDetails.qty;
    }
    function getApprenticeshipPriceAndDuration($container) {
        var apprenticeshipDetails = getApprenticeshipDetails($container);
        return {
            price: apprenticeshipDetails.price * apprenticeshipDetails.qty,
            duration: apprenticeshipDetails.duration
        };
    }

    function getApprenticeshipDetails($container) {
        var details = {
            code: '',
            name: '',
            price: 0,
            duration: 0,
            qty: 0,
            startDate: ''
        };

        var selectedStandardSelector = $container.find('select[name=standard] > option:selected');
        if (selectedStandardSelector.length > 0) {
            var selectedStandard = $(selectedStandardSelector[0]);
            details.code = selectedStandard.val();
            details.name = selectedStandard.text();
            details.price = parseInt(selectedStandard.attr('data-price'));
            details.duration = parseInt(selectedStandard.attr('data-duration'));
        }

        var cohortSelector = $container.find('input[name=cohorts]');
        if (cohortSelector.length > 0) {
            details.qty = parseInt($(cohortSelector[0]).val());
        }


        if (isNaN(details.price)) {
            details.price = 0;
        }
        if (isNaN(details.duration)) {
            details.duration = 0;
        }
        if (isNaN(details.qty)) {
            details.qty = 0;
        }

        return details;
    }
})();