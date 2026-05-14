$(document).ready(function () {
    $('#applyFilters').click(function () {
        let employee = $('#employeeSearch').val();
        let leaveType = $('#leaveTypeSearch').val();
        let status = $('#statusSearch').val();

        let url = '/LeaveApprovals/Index?';
        if (employee) url += 'employee=' + encodeURIComponent(employee) + '&';
        if (leaveType) url += 'leaveType=' + encodeURIComponent(leaveType) + '&';
        if (status) url += 'status=' + encodeURIComponent(status) + '&';

        window.location.href = url.slice(0, -1);
    });

    $('#resetFilters').click(function () {
        window.location.href = '/LeaveApprovals/Index';
    });
});
