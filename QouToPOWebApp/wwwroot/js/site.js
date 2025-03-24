// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

var idleTime = 0;
setInterval(timerIncrement, 60000); // 1 minute interval

function timerIncrement() {
    idleTime++;
    if (idleTime >= 30) { // 30 minutes of inactivity
        window.location.href = '/Access/LogOut'; // Redirect to logout
    }
}

$(this).mousemove(function () {
    idleTime = 0; // Reset idle time on mouse move
});

$(this).keypress(function () {
    idleTime = 0; // Reset idle time on key press
});

$(this).on('touchstart touchmove', function () {
    idleTime = 0; // Reset idle time on touch start or touch move
});

$(document).on("input", "textarea", function () {
    if ($(this).val().trim() != "") {
        this.style.height = "auto";
        this.style.height = (this.scrollHeight) + "px";
    } else {
        this.style.height = "38px";
    }
});

$(document).ready(function () {
    document.getElementById('passwordForm').addEventListener('submit', function (event) {
        event.preventDefault(); // Prevent form submission

        const newPassword = document.getElementById('newPassword');
        const confirmPassword = document.getElementById('confirmPassword');
        const errorMessage = document.getElementById('errorMessage');

        // Reset the error message
        errorMessage.textContent = '';

        // Validate the passwords
        if (newPassword.value === '' || confirmPassword.value === '') {
            errorMessage.textContent = 'Both fields are required.';
        } else if (newPassword.value !== confirmPassword.value) {
            errorMessage.textContent = 'New Passwords do not match.';
        } else {
            $("#passwordForm").submit();
        }
        //else if (!validatePassword(newPassword)) {
        //    errorMessage.textContent = 'Password does not meet the criteria.';
        //}
    });

    // Function to validate the password criteria
    function validatePassword(password) {
        // Example criteria: at least 8 characters, 1 uppercase, 1 lowercase, and 1 number
        const criteria = /^(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).{8,}$/;
        return criteria.test(password);
    }

    if ($("#haserror").length) {
        if ($("#haserror").val() != "") {
            $("#wrongpass").append($("#haserror").val());
            $("#changePassModal").modal("show");
        }
    }
});

function showToast(message, type = "success", duration = 7000) {
    let toastContainer = $("#toastContainer"); // Ensure toast container exists

    if (!toastContainer.length) {
        $("body").append('<div id="toastContainer" class="toast-container position-fixed bottom-0 end-0 p-3"></div>');
        toastContainer = $("#toastContainer");
    }

    // Create a unique toast element
    let toast = $(`
        <div class="toast align-items-center text-bg-${type} border-0 shadow fade" role="alert" aria-live="assertive" aria-atomic="true" data-bs-delay="${duration}">
            <div class="d-flex">
                <div class="toast-body">${message}</div>
                <button type="button" class="btn-close me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>
            </div>
        </div>
    `);

    toastContainer.append(toast);

    // Initialize and show the toast
    let bsToast = new bootstrap.Toast(toast[0]);
    bsToast.show();

    // Automatically remove the toast after it disappears
    toast.on("hidden.bs.toast", function () {
        $(this).remove();
    });
}