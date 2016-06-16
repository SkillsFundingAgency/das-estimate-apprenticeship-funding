(function () {
    $(document).ready(function () {
        addCostColumn();
        attachHandlers();
    });


    function addCostColumn() {
        var $headerRow = $('#trainingCourses > thead > tr');
        $headerRow.append($('<th>Cost</th>'));
        $('#trainingCourses > tbody > tr').each(function (index, element) {
            var $row = $(element);
            $row.append($('<td class="cost"></td>'));

            updateRowCost($row);
        });
    }
    function attachHandlers() {
        $('select[name=standard]').change(cohortPriceChange);
        $('input[name=cohorts]').change(cohortPriceChange);
    }
    function cohortPriceChange() {
        var $row = $(this).parent().parent();
        updateRowCost($row);
    }
    function updateRowCost($row) {
        var $costCell = $($row.find('td.cost')[0]);
        var standard = getSelectedStandard($row);
        var size = getCohortSize($row);

        var totalCost = standard.price * size;
        $costCell.text('£' + totalCost.format(0));
    }
    function getSelectedStandard($row) {
        var standard = { code: '', name: '', price: 0 };

        var selectedOptions = $row.find('select[name=standard] > option:selected');
        if (selectedOptions.length == 0) {
            return standard;
        }

        var $option = $(selectedOptions[0]);
        var code = parseInt($option.val());
        if (isNaN(code)) {
            return standard;
        }

        standard.code = code;
        standard.name = $option.text();
        standard.price = parseInt($option.attr('data-price'));

        return standard;
    }
    function getCohortSize($row) {
        var $size = $($row.find('input[name=cohorts]')[0]);
        var size = parseInt($size.val());
        return isNaN(size) ? 0 : size;
    }
})();