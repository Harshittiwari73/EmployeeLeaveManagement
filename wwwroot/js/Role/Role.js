
function openRoleModal(id) {
    $('#roleId').val(id);
    $('#modalTitle').text(id === 0 ? "Add New Role" : "Edit Role");

    $.get('/Roles/GetRoleWithPermissions/' + id, function (data) {
        $('#roleName').val(data.roleName || '');
        $('#isActive').prop('checked', data.isActive);

        let html = '';
        data.modules.forEach((module, mIdx) => {
            html += `<div class="col-md-6">
                        <fieldset class="border rounded p-3 h-100">
                            <legend class="float-none w-auto px-2 fs-6 fw-bold mb-0 text-primary">
                                <div class="form-check d-inline-block">
                                    <input class="form-check-input module-check" type="checkbox" id="mod_${mIdx}">
                                    <label class="form-check-label" for="mod_${mIdx}">${module.moduleName}</label>
                                </div>
                            </legend>
                            <div class="row g-2 mt-1">`;

            module.permissions.forEach((perm, pIdx) => {
                let labelName = perm.permissionName + ' ' + module.moduleName;
                if (perm.permissionName === 'Create') {
                    labelName = 'Add ' + module.moduleName;
                }
                if (module.moduleName === "LeaveRequest") {
                    labelName = perm.permissionName === 'Create' ? 'Add Leave Request' : perm.permissionName + ' Leave Request';
                }
                if (module.moduleName === "LeaveType") {
                    labelName = perm.permissionName === 'Create' ? 'Add Leave Type' : perm.permissionName + ' Leave Type';
                }
                if (module.moduleName === "Dashboard") {
                    labelName = perm.permissionName + ' Dashboard';
                }

                html += `           <div class="col-6">
                                        <div class="form-check">
                                            <input class="form-check-input perm-check perm-check-mod-${mIdx}" type="checkbox" id="perm_${mIdx}_${pIdx}"
                                                   data-module-idx="${mIdx}" data-perm-idx="${pIdx}"
                                                   data-perm-id="${perm.permissionId}"
                                                   ${perm.isAllowed ? 'checked' : ''}>
                                            <label class="form-check-label text-secondary" style="font-size: 0.9rem;" for="perm_${mIdx}_${pIdx}">${labelName}</label>
                                        </div>
                                    </div>`;
            });

            html += `           </div>
                        </fieldset>
                    </div>`;
        });
        $('#permissionsGrid').html(html);
        updateModuleChecks();
        $('#selectAllPerms').prop('checked', $('.perm-check:checked').length === $('.perm-check').length);
        $('#roleModal').modal('show');
    });
}

$('#selectAllPerms').on('change', function () {
    $('.perm-check, .module-check').prop('checked', $(this).is(':checked'));
});

$(document).on('change', '.module-check', function () {
    let modIdx = $(this).attr('id').split('_')[1];
    $(`.perm-check-mod-${modIdx}`).prop('checked', $(this).is(':checked'));
    $('#selectAllPerms').prop('checked', $('.perm-check:checked').length === $('.perm-check').length);
});

$(document).on('change', '.perm-check', function () {
    let modIdx = $(this).data('module-idx');
    let total = $(`.perm-check-mod-${modIdx}`).length;
    let checked = $(`.perm-check-mod-${modIdx}:checked`).length;
    $(`#mod_${modIdx}`).prop('checked', total === checked && total > 0);
    $('#selectAllPerms').prop('checked', $('.perm-check:checked').length === $('.perm-check').length);
});

function updateModuleChecks() {
    $('.module-check').each(function () {
        let modIdx = $(this).attr('id').split('_')[1];
        let total = $(`.perm-check-mod-${modIdx}`).length;
        let checked = $(`.perm-check-mod-${modIdx}:checked`).length;
        $(this).prop('checked', total === checked && total > 0);
    });
}

$(document).on('submit', '#roleForm', function (e) {
    e.preventDefault();
    console.log("Submitting role form...");

    let modules = [];
    $('fieldset').each(function () {
        let moduleName = $(this).find('.module-check').next('label').text();
        let permissions = [];
        $(this).find('.perm-check').each(function () {
            permissions.push({
                PermissionId: $(this).data('perm-id'),
                IsAllowed: $(this).is(':checked')
            });
        });
        modules.push({ ModuleName: moduleName, Permissions: permissions });
    });

    let formData = {
        RoleId: $('#roleId').val(),
        RoleName: $('#roleName').val(),
        IsActive: $('#isActive').is(':checked'),
        Modules: modules
    };

    console.log("Form Data:", formData);

    $.ajax({
        url: '/Roles/SaveRole',
        type: 'POST',
        data: JSON.stringify(formData),
        contentType: 'application/json; charset=utf-8',
        success: function (data) {
            $('#loader-overlay').hide();
            if (data.success) {
                Swal.fire('Success', 'Role saved successfully', 'success').then(() => location.reload());
            } else {
                Swal.fire('Error', data.message || 'Failed to save role', 'error');
            }
        },
        error: function (err) {
            $('#loader-overlay').hide();
            console.error("Save error:", err);
            Swal.fire('Error', 'Server error occurred', 'error');
        }
    });
});

function deleteRole(id, name) {
    Swal.fire({
        title: 'Delete Role?',
        text: "Are you sure you want to delete " + name + "?",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
        if (result.isConfirmed) {
            $.post('/Roles/Delete/' + id, function (data) {
                if (data.success) {
                    location.reload();
                }
            });
        }
    });
}
