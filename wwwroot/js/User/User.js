
function openUserModal(id) {
    $('#userForm')[0].reset();
    $('#modalUserId').val(id);
    $('#modalPassword').val('');
    if (id == 0) {
        $('#userModalTitle').text('Add New User');
        $('#passwordLabel').text('PASSWORD');
        $('#passwordHint').text('');
        $('#modalIsActive').prop('checked', true);
        $('#userModal').modal('show');
    }
    else {
        $('#userModalTitle').text('Edit User');
        $('#passwordLabel').text('NEW PASSWORD (Optional)');
        $('#passwordHint').text('Leave blank to keep current password.');
        $.ajax({
            url: '/Users/GetUser/' + id,
            type: 'GET',
            success: function (data) {
                $('#modalUsername').val(data.username);
                $('#modalEmail').val(data.email);
                $('#modalPhone').val(data.phoneNumber);
                $('#modalRoleId').val(data.roleId);
                $('#modalIsActive').prop('checked', data.isActive);
                $('#userModal').modal('show');
            },
            error: function () {
                Swal.fire({
                    icon: 'error',
                    title: 'Error',
                    text: 'Failed to load user data'
                });
            }
        });
    }
}
$('#userForm').submit(function (e) {
    e.preventDefault();
    if ($('#modalUsername').val() == '' ||
        $('#modalEmail').val() == '') {
        Swal.fire({
            icon: 'warning',
            title: 'Validation',
            text: 'Please fill all required fields'
        });
        return;
    }
    let formData = {
        UserId: $('#modalUserId').val(),
        Username: $('#modalUsername').val(),
        Email: $('#modalEmail').val(),
        PhoneNumber: $('#modalPhone').val(),
        RoleId: $('#modalRoleId').val(),
        Password: $('#modalPassword').val(),
        IsActive: $('#modalIsActive').is(':checked')
    };
    $.ajax({
        url: '/Users/SaveUser',
        type: 'POST',
        data: formData,
        success: function (data) {
            if (data.success) {
                $('#userModal').modal('hide');
                $('body').removeClass('modal-open');
                $('.modal-backdrop').remove();
                $('body').css('padding-right', '');
                $('#userForm')[0].reset();
                Swal.fire({
                    icon: 'success',
                    title: 'Success',
                    text: 'User saved successfully',
                    timer: 1500,
                    showConfirmButton: false
                });
                setTimeout(function () {
                    location.reload();
                }, 1500);
            }
            else {
                Swal.fire({
                    icon: 'error',
                    title: 'Error',
                    text: data.message || 'Failed to save user'
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
function confirmDelete(userId, username) {
    Swal.fire({
        title: 'Delete User?',
        text: 'Are you sure you want to delete ' + username + '?',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#dc3545',
        cancelButtonColor: '#6c757d',
        confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: '/Users/Delete/' + userId,
                type: 'POST',
                success: function (data) {
                    if (data.success) {
                        Swal.fire({
                            icon: 'success',
                            title: 'Deleted',
                            text: 'User deleted successfully',
                            timer: 1500,
                            showConfirmButton: false
                        });
                        setTimeout(function () {
                            location.reload();
                        }, 1500);
                    }
                    else {
                        Swal.fire({
                            icon: 'error',
                            title: 'Error',
                            text: data.message || 'Delete failed'
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
        }
    });
}
$(document).ready(function () {
    $('#userModal').on('hidden.bs.modal', function () {
        $('body').removeClass('modal-open');
        $('.modal-backdrop').remove();
        $('body').css('padding-right', '');
    });

    $('#applyFilters').click(function () {
        let username = $('#usernameSearch').val();
        let email = $('#emailSearch').val();
        let phone = $('#phoneSearch').val();
        let role = $('#roleSearch').val();
        let status = $('#statusSearch').val();

        let url = '/Users/Index?';
        if (username) url += 'username=' + encodeURIComponent(username) + '&';
        if (email) url += 'email=' + encodeURIComponent(email) + '&';
        if (phone) url += 'phone=' + encodeURIComponent(phone) + '&';
        if (role) url += 'role=' + encodeURIComponent(role) + '&';
        if (status) url += 'status=' + encodeURIComponent(status) + '&';

        window.location.href = url.slice(0, -1);
    });

    $('#resetFilters').click(function () {
        window.location.href = '/Users/Index';
    });
});