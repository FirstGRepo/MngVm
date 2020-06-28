
var array=[1,2,3,4,5,6,7,8,9]
$(function () {

})

$('#btnSubmit').click(function (e) {
    e.preventDefault();
    if (validateForm()) {
        if ($('#Password').val().trim() == $('#confPassword').val().trim()) {
            var Data = $('#frmUser').serializeArray();
            $.ajax({
                url: '/Admin/AddUser',
                type: 'POST',
                data: Data,
                dataType: 'json',
                success: function (data) {
                    var status = data.split('|')[0];
                    var message = data.split('|')[1];
                    if (status == "0")
                        $('#success-alert').removeClass().addClass('alert customalert alert-danger').show().fadeOut(5000);
                    else if (status == "1") {
                        $('#frmUser input,#frmUser select').val('');
                        $('#success-alert').removeClass().addClass('alert customalert alert-success').show().fadeOut(5000);
                    }
                    else
                        $('#success-alert').addClass('alert customalert alert-warning').show().fadeOut(5000);
                    $('#message').text(message);
                    loadList();
                    // $("#success-alert").fadeToggle('1000');
                },
                error: function (request, error) {
                    alert("Request: " + JSON.stringify(request));
                    //  $('#success-alert').addClass('alert-danger');
                }
            });
        }
        else {
            $('#Password,#confPassword').addClass('requiredBorder');
            $('#success-alert').addClass('customalert alert-warning').show().fadeOut(5000);
            $('#message').text('Password does not matched.');
        }

    }
});

$(document).on('click', '.btnDelete', function (e) {
    var username = $(this).attr('username');
    var flag = confirm('Are you sure, you want to delete "' + username + '"?');
    if (flag) {
        $.ajax({
            url: '/Admin/Delete',
            type: 'POST',
            data: { userName: username },
            dataType: 'json',
            success: function (data) {
                if (data)
                    loadList();
            },
            error: function (request, error) {
                alert("Request: " + JSON.stringify(request));
                //  $('#success-alert').addClass('alert-danger');
            }
        });
    }
});

function loadList() {
    $('#userist').load('/Admin/List');
}

function validateForm() {
    var count = 0;
    $('.required').each(function () {
        if ($(this).val() == '' || $(this).val() == undefined) {
            count = count + 1;
            $(this).addClass('requiredBorder');
        }
        else {
            count = count--;
            $(this).removeClass('requiredBorder');
        }
    });
    return count == 0;
}

function validateEmail(email) {
    var emailReg = /^([\w-\.]+@([\w-]+\.)+[\w-]{2,4})?$/;
    return emailReg.test(email);
}

function checkPassword(str) {
    // at least one number, one lowercase and one uppercase letter
    // at least six characters
    if (str.trim().length >= 8) {
        var re = /(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).{6,}/;
        return re.test(str);
    }
    else
        return false;
}

$('#Password').focusout(function () {
    if ($(this).val().trim() != '') {
        var msg = "at least one number, one lowercase and one uppercase letter.";
        msg += "At least 8 characters.";
        if (!checkPassword($(this).val().trim())) {
            $('#success-alert').addClass('alert customalert alert-warning').show().fadeOut(10000);
            $('#message').text(msg);
            $(this).val('');
        }
    }
})

$('#UserName').focusout(function () {
    if ($(this).val().trim() != '') {
        if (validateEmail($(this).val())) {
            $('#success-alert').addClass('alert customalert alert-warning').show().fadeOut(5000);
            $('#message').text('Please enter valid username');
            (this).val('')
        }
    }
})

$('#chngpasBtn').click(function (e) {
    e.preventDefault();
    if ($('#Password').val().trim() == $('#confPassword').val().trim()) {
        //  $('#frmChangePass').submit();
        var Data = $('#frmChangePass').serializeArray();
        $.ajax({
            url: '/Admin/ChangePassword',
            type: 'POST',
            data: Data,
            dataType: 'json',
            success: function (data) {
                $('#message').text('');
                if (data) {
                    $('#frmChangePass input').val('');
                    //  $('#Password,#confPassword').val('');
                    $('#message').text('Password has been changed successfuly.').addClass('green');
                }
                else
                    $('#message').text('Error while udating password.').addClass('red');
            },
            error: function (request, error) {
                alert("Request: " + JSON.stringify(request));
                //  $('#success-alert').addClass('alert-danger');
            }
        });
    }
    else {
        $('#Password,#confPassword').addClass('requiredBorder');
        alert('Password does not matched.');
    }
})

$('#HostPoolName').change(function (e) {
    e.preventDefault();
    if ($(this).val().trim() != '') {
        var hostpoolName = $(this).val();

        $.ajax({
            url: '/Admin/GetVmsByHostpool',
            type: 'GET',
            data: { hostPool: hostpoolName },
            dataType: 'json',
            success: function (data) {
                $('#message').text('');
                if (data) {
                    $('#vmlist').empty();
                    var list = '<li class="list-group-item active text-center">Machines</li>';
                    $.each(data, function (i, v) {
                        list += '<li class="list-group-item">' + v + '</li>';
                    });

                    $('#vmlist').append(list);
                    $('#messageSection').show();
                }
                else
                    $('#message').text('Error while getting Vm info.').addClass('red');
            },
            error: function (request, error) {
                alert("Request: " + JSON.stringify(request));
                //  $('#success-alert').addClass('alert-danger');
            }
        });
    }
    //else {
    //    $('#Password,#confPassword').addClass('requiredBorder');
    //    alert('Password does not matched.');
    //}
})