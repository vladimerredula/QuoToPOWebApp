$(document).ready(function () {
    initializeEventHandlers();
    getPdfRender();
});

function initializeEventHandlers() {
    const dropZone = $("#dropZone");
    const fileInput = $("#pdfFile");

    dropZone
        .on("mouseenter", () => toggleDropZoneHighlight(true))
        .on("mouseleave", () => toggleDropZoneHighlight(false))
        .on("dragover", (e) => handleDragOver(e))
        .on("dragleave", () => toggleDropZoneHighlight(false))
        .on("drop", (e) => handleFileDrop(e));

    $("#selectFileBtn").on("click", () => fileInput.click());
}

function toggleDropZoneHighlight(isHighlight) {
    const dropZone = $("#dropZone");
    dropZone.toggleClass("bg-light", isHighlight);
    dropZone.toggleClass("bg-light-subtle", !isHighlight);
}

function handleDragOver(event) {
    event.preventDefault();
    toggleDropZoneHighlight(true);
}

function handleFileDrop(event) {
    event.preventDefault();
    toggleDropZoneHighlight(false);

    const files = event.originalEvent.dataTransfer.files;
    if (files.length > 0) {
        const file = files[0];

        if (isValidPdfFile(file)) {
            $("#pdfFile")[0].files = files; // Assign files to input
            $("#pdfFile").change();
            getPdfRender();
        } else {
            alert("Please upload a valid PDF file.");
        }
    }
}

function isValidPdfFile(file) {
    return file.type === "application/pdf" || file.name.toLowerCase().endsWith(".pdf");
}

function submitForm() {
    $("#extractBtn").addClass("visually-hidden");
    $("#loadingBtn").removeClass("visually-hidden");
    $("#quoForm").submit();
}

async function uploadPdf(fileInput) {
    const file = fileInput.files[0];
    if (!file) return;

    const formData = new FormData();
    formData.append('pdfFile', file);

    try {
        $.ajax({
            url: "/Pdf/UploadPdf",
            type: "POST",
            data: formData,
            contentType: false,
            processData: false,
            success: (data) => renderPdf(data),
            error: handleAjaxError
        });
    } catch (error) {
        console.error('Upload error:', error);
    }
}

function renderPdf(data) {
    $("#divPreview").removeClass("visually-hidden");
    $("#extractSettings").removeClass("visually-hidden");
    $("#dropArea").addClass("visually-hidden");

    $('#pdfPreview').attr("src", `${data.image}#toolbar=0&view=Fit&navpanes=0`);
    $('#pdfPreview').attr("filename", data.filePath);
    $('#filePath').val(data.filePath);
    $('#selectFileInput').val(data.fileName);
}

function getPdfRender() {
    const preview = $('#pdfPreview');
    const filePath = preview.attr("filename");

    if (!filePath) return;

    $.ajax({
        url: '/Pdf/GetPdfData',
        type: 'POST',
        data: { filePath },
        success: (data) => renderPdf(data),
        error: handleAjaxError
    });
}

function handleAjaxError(response) {
    console.error("Error:", response.status, response.responseJSON?.message);
}
