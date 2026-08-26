$(document).ready(function () {
    $('.tree-node').click(function (e) {
        e.stopPropagation();

        const $node = $(this);
        const unitId = $node.data('unit-id');

        $('.tree-node').removeClass('selected');
        $node.addClass('selected');

        loadWorkers(unitId);
    });

    $('.toggle-icon').click(function (e) {
        e.stopPropagation();

        const $icon = $(this);
        const $children = $icon.closest('.tree-node').next('.tree-children');

        $icon.toggleClass('expanded');
        $children.toggleClass('expanded');
    });
});

function loadWorkers(unitId) {
    const $container = $('#workersTableContainer');
    const $loading = $('.loading-overlay');

    $loading.addClass('active');

    $.ajax({
        url: '/Workers/GetWorkersByUnit', 
        type: 'GET',
        data: { unitId: unitId },
        success: function (data) {
            $container.html(data);

            const $selectedNode = $(`.tree-node[data-unit-id="${unitId}"]`);
            let unitName = $selectedNode.find('span:not(.toggle-icon):not(.node-icon):not(.badge)').text().trim();

            if (!unitName) {
                unitName = unitId;
            }

            $('#selectedUnitName').text(unitName);

            const count = $(data).find('tbody tr').length || 0;
            $('#workerCount').text(count);
        },
        error: function (xhr) {
            $container.html(`
                <div class="alert alert-danger m-3">
                    <i class="bi bi-exclamation-triangle"></i>
                    Ошибка при загрузке данных: ${xhr.responseJSON?.error || 'Неизвестная ошибка'}
                </div>
            `);
            $('#workerCount').text('0');
        },
        complete: function () {
            $loading.removeClass('active');
        }
    });
}