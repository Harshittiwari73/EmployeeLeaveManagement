
$('#addEmployeeBtn').click(function () {

    $('#employeeForm')[0].reset();

    $('#EmployeeId').val('');

});

$('#saveEmployee').click(function () {
    $('#nameError').text('');
    $('#emailError').text('');
    $('#dateError').text('');

    $('#EmployeeName').removeClass('is-invalid');
    $('#Email').removeClass('is-invalid');
    $('#JoiningDate').removeClass('is-invalid');

    let isValid = true;

    let employeeName = $('#EmployeeName').val().trim();
    let email = $('#Email').val().trim();
    let joiningDate = $('#JoiningDate').val();
    if (employeeName === '') {

        $('#nameError').text('Employee Name is required');
        $('#EmployeeName').addClass('is-invalid');

        isValid = false;
    }
    if (email === '') {

        $('#emailError').text('Email is required');
        $('#Email').addClass('is-invalid');

        isValid = false;
    }
    else {
        let emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

        if (!emailPattern.test(email)) {

            $('#emailError').text('Enter valid email address');
            $('#Email').addClass('is-invalid');

            isValid = false;
        }
    }
    if (joiningDate === '') {

        $('#dateError').text('Joining Date is required');
        $('#JoiningDate').addClass('is-invalid');

        isValid = false;
    }
    else {

        let selectedDate = new Date(joiningDate);
        let today = new Date();

        today.setHours(0, 0, 0, 0);

        if (selectedDate > today) {

            $('#dateError').text('Joining Date cannot be future date');
            $('#JoiningDate').addClass('is-invalid');

            isValid = false;
        }
    }
    if (!isValid) {
        return;
    }

    let employee = {

        EmployeeId: $('#EmployeeId').val(),

        EmployeeName: employeeName,

        Email: email,

        Department: $('input[name="Department"]').val(),

        JoiningDate: joiningDate,

        IsActive: $('#IsActive').is(':checked')

    };

    let url = '/Employees/Create';

    if (employee.EmployeeId && employee.EmployeeId != 0) {

        url = '/Employees/Edit';

    }

    $.ajax({

        url: url,

        type: 'POST',

        data: employee,

        success: function (response) {

            if (response.success) {

                $('#employeeModal').modal('hide');

                Swal.fire({

                    icon: 'success',

                    title: 'Success',

                    text: response.message,

                    confirmButtonColor: '#0d6efd'

                }).then(() => {

                    location.reload();

                });

            }
            else {

                Swal.fire({

                    icon: 'error',

                    title: 'Error',

                    text: response.message || 'Something went wrong'

                });

            }

        },

        error: function () {

            Swal.fire({

                icon: 'error',

                title: 'Error',

                text: 'Server error occurred'

            });

        }

    });

});

$('.editBtn').click(function () {

    let id = $(this).data('id');

    $.ajax({

        url: '/Employees/GetById/' + id,

        type: 'GET',

        success: function (response) {

            if (response.success) {

                let employee = response.data;

                $('#EmployeeId').val(employee.employeeId);

                $('input[name="EmployeeName"]').val(employee.employeeName);

                $('input[name="Email"]').val(employee.email);

                $('input[name="Department"]').val(employee.department);

                $('input[name="JoiningDate"]').val(employee.joiningDate.split('T')[0]);

                $('#IsActive').prop('checked', employee.isActive);

                $('#employeeModal').modal('show');

            }

        }

    });

});

$('.deleteBtn').click(function () {

    let id = $(this).data('id');

    Swal.fire({

        title: 'Are you sure?',

        text: 'You will not be able to recover this employee!',

        icon: 'warning',

        showCancelButton: true,

        confirmButtonColor: '#dc3545',

        cancelButtonColor: '#6c757d',

        confirmButtonText: 'Yes, delete it!'

    }).then((result) => {

        if (result.isConfirmed) {

            $.ajax({

                url: '/Employees/Delete',

                type: 'POST',

                data: { id: id },

                success: function (response) {

                    if (response.success) {

                        Swal.fire({

                            icon: 'success',

                            title: 'Deleted!',

                            text: response.message

                        }).then(() => {

                            location.reload();

                        });

                    }
                    else {

                        Swal.fire({

                            icon: 'error',

                            title: 'Error',

                            text: response.message

                        });

                    }

                }

            });

        }

    });

});

$(document).ready(function () {
    $('#applyFilters').click(function () {
        let name = $('#nameSearch').val();
        let email = $('#emailSearch').val();
        let department = $('#departmentSearch').val();
        let joiningDate = $('#joiningDateSearch').val();
        let status = $('#statusSearch').val();

        let url = '/Employees/Index?';
        if (name) url += 'name=' + encodeURIComponent(name) + '&';
        if (email) url += 'email=' + encodeURIComponent(email) + '&';
        if (department) url += 'department=' + encodeURIComponent(department) + '&';
        if (joiningDate) url += 'joiningDate=' + encodeURIComponent(joiningDate) + '&';
        if (status) url += 'status=' + encodeURIComponent(status) + '&';

        window.location.href = url.slice(0, -1);
    });

    $('#resetFilters').click(function () {
        window.location.href = '/Employees/Index';
    });
});

