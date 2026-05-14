$('#applyLeaveForm').submit(function (e) {
    e.preventDefault();
    $('#fromDateError, #toDateError, #reasonError').text('');
    $('.is-invalid').removeClass('is-invalid');

    let isValid = true;
    let fromDate = $('#fromDate').val();
    let toDate = $('#toDate').val();
    let reason = $('#reason').val().trim();
    let employeeId = $('#employeeId').val();
    let leaveTypeId = $('#leaveTypeId').val();
    if (!employeeId) {
        $('#employeeId').addClass('is-invalid');
        isValid = false;
    }
    if (!leaveTypeId) {
        $('#leaveTypeId').addClass('is-invalid');
        isValid = false;
    }
    if (!fromDate) {
        $('#fromDateError').text('Please select a start date');
        $('#fromDate').addClass('is-invalid');
        isValid = false;
    }
    if (!toDate) {
        $('#toDateError').text('Please select an end date');
        $('#toDate').addClass('is-invalid');
        isValid = false;
    }
    if (fromDate && toDate && new Date(fromDate) > new Date(toDate)) {
        $('#toDateError').text('End date cannot be earlier than start date');
        $('#toDate').addClass('is-invalid');
        isValid = false;
    }
    if (reason.length < 10) {
        $('#reasonError').text('Reason must be at least 10 characters long');
        $('#reason').addClass('is-invalid');
        isValid = false;
    }

    if (!isValid) return;
    const $btn = $('#btnSubmit');
    const $spinner = $('#submitSpinner');
    $btn.prop('disabled', true);
    $spinner.removeClass('d-none');

    let formData = {
        EmployeeId: parseInt(employeeId),
        LeaveTypeId: parseInt(leaveTypeId),
        FromDate: fromDate,
        ToDate: toDate,
        Reason: reason
    };

    $.ajax({
        url: '/LeaveRequests/ApplyLeave',
        type: 'POST',
        data: JSON.stringify(formData),
        contentType: 'application/json; charset=utf-8',
        success: function (data) {
            if (data.success) {
                $('#applyLeaveModal').modal('hide');
                Swal.fire({
                    icon: 'success',
                    title: 'Submitted!',
                    text: 'Your leave request has been sent for approval.',
                    timer: 2000,
                    showConfirmButton: false
                }).then(() => {
                    location.reload();
                });
            } else {
                Swal.fire({
                    icon: 'error',
                    title: 'Submission Failed',
                    text: data.message || 'Something went wrong.'
                });
            }
        },
        error: function () {
            Swal.fire({
                icon: 'error',
                title: 'Server Error',
                text: 'We couldn\'t connect to the server. Please try again later.'
            });
        },
        complete: function() {
            $btn.prop('disabled', false);
            $spinner.addClass('d-none');
        }
    });
});

$(document).ready(function () {
    $('#applyFilters').click(function () {
        let employee = $('#employeeSearch').val();
        let leaveType = $('#leaveTypeSearch').val();
        let fromDate = $('#fromDateSearch').val();
        let status = $('#statusSearch').val();

        let url = '/LeaveRequests/Index?';
        if (employee) url += 'employee=' + encodeURIComponent(employee) + '&';
        if (leaveType) url += 'leaveType=' + encodeURIComponent(leaveType) + '&';
        if (fromDate) url += 'fromDate=' + encodeURIComponent(fromDate) + '&';
        if (status) url += 'status=' + encodeURIComponent(status) + '&';

        window.location.href = url.slice(0, -1);
    });

    $('#resetFilters').click(function () {
        window.location.href = '/LeaveRequests/Index';
    });
});