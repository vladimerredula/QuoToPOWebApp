
$(document).ready(function () {
    getPdfRender();
});

const dropZone = $("#dropZone");
const fileInput = document.getElementById("pdfFile");

dropZone.on("mouseenter", function () {
    $(this).removeClass("bg-light-subtle").addClass("bg-light");
});

dropZone.on("mouseleave", function () {
    $(this).removeClass("bg-light").addClass("bg-light-subtle");
});

dropZone.on("dragover", function (event) {
    event.preventDefault();
    $(this).removeClass("bg-light-subtle").addClass("bg-light");
});

dropZone.on("dragleave", function () {
    $(this).addClass("bg-light-subtle").removeClass("bg-light");
});

dropZone.on("drop", function (event) {
    event.preventDefault();
    $(this).addClass("bg-light-subtle").removeClass("bg-light");

    let files = event.originalEvent.dataTransfer.files;
    if (files.length > 0) {
        let file = files[0];

        if (file.type === "application/pdf" || file.name.endsWith(".pdf")) {
            $("#pdfFile")[0].files = files; // Assign files to input
            $("#pdfFile").change();
            getPdfRender();
        } else {
            alert("Please upload a valid PDF file.");
        }
    }
});

$("#selectFileBtn").on("click", function () {
    $("#pdfFile").click(); // Trigger the hidden file input
});

function submitForm() {
    $("#extractBtn").addClass("visually-hidden");
    $("#loadingBtn").removeClass("visually-hidden");
    $("#quoForm").submit();
}

async function uploadPdf(fileInput) {
    const file = fileInput.files[0];

    if (!file) {
        return;
    }
    const formData = new FormData();
    formData.append('pdfFile', file);

    try {
        $.ajax({
            url: "/Pdf/UploadPdf",
            type: "POST",
            data: formData,
            contentType: false,
            processData: false,
            success: function (data) {
                console.log(data);
                renderPdf(data);
            },
            error: function (response) {
                console.error("Error:", response.status, response.responseJSON.message);
                alert("Error: " + response.responseJSON.message);
            }
        });
    } catch (error) {
        console.error('Upload error:', error);
    }
}

function renderPdf(data) {
    $("#divPreview").removeClass("visually-hidden");
    $("#extractSettings").removeClass("visually-hidden");
    $("#dropArea").addClass("visually-hidden");

    $('#pdfPreview').attr("src", data.image + "#toolbar=0&view=Fit&navpanes=0");
    $('#pdfPreview').attr("filename", data.filePath);
    $('#filePath').val(data.filePath);
    $('#selectFileInput').val(data.fileName);
}

function getPdfRender() {
    const preview = $('#pdfPreview');
    const filePath = preview.attr("filename");

    if (!filePath) {
        return;
    }

    $.ajax({
        url: '/Pdf/GetPdfData',
        type: 'POST',
        data: {
            filePath: filePath
        },
        success: function (data) {
            renderPdf(data);
        },
        error: function (response) {
            console.log(response);
        }
    });
}