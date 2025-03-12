$(document).ready(function () {
    initializeEventHandlers("#dropZone1", "#quoFile");
    initializeEventHandlers("#dropZone2", "#drawFile");
});

$("#save").on("click", function () {

});

function initializeEventHandlers(dropZoneSelector, fileInputSelector) {
    const dropZone = $(dropZoneSelector);
    const fileInput = $(fileInputSelector);

    dropZone
        .on("mouseenter", () => toggleDropZoneHighlight(dropZone, true))
        .on("mouseleave", () => toggleDropZoneHighlight(dropZone, false))
        .on("dragover", (e) => handleDragOver(e, dropZone))
        .on("dragleave", () => toggleDropZoneHighlight(dropZone, false))
        .on("drop", (e) => handleFileDrop(e, fileInput, dropZone));

    dropZone.find(".selectFileBtn").on("click", () => fileInput.click());
}

function toggleDropZoneHighlight(dropZone, isHighlight) {
    dropZone.toggleClass("bg-light", isHighlight);
    dropZone.toggleClass("bg-light-subtle", !isHighlight);
}

function handleDragOver(event, dropZone) {
    event.preventDefault();
    toggleDropZoneHighlight(dropZone, true);
}

function handleFileDrop(event, fileInput, dropZone) {
    event.preventDefault();
    toggleDropZoneHighlight(dropZone, false);

    const files = event.originalEvent.dataTransfer.files;
    if (files.length > 0) {
        const file = files[0];

        if (isValidPdfFile(file)) {
            fileInput[0].files = files; // Assign files to input
            fileInput.change();
            //getPdfRender(fileInput); // Modify this function to handle multiple inputs if needed
        } else {
            alert("Please upload a valid PDF file.");
        }
    }
}

function isValidPdfFile(file) {
    return file.type === "application/pdf" || file.name.toLowerCase().endsWith(".pdf");
}

// Example placeholder function - modify as needed
function getPdfRender(fileInput) {
    console.log("Rendering PDF for:", fileInput.attr('id'));
}

$("#quoFile").on("change", function () {
    if (this.files.length === 0) {
        alert("No file selected.");
        return;
    }

    for (let i = 0; i < this.files.length; i++) {
        uploadFile(this.files[i]);
    }
});

async function uploadFile(file) {
    if (!isValidPdfFile(file)) {
        alert("Invalid file type: " + file.name);
        return;
    }

    var formData = new FormData();
    formData.append("quoFile", file);

    var alert = $("<div>")
        .addClass("alert alert-light alert-dismissible")

    var filename = $("<strong>").text(file.name);

    var loading = $("<span>")
        .addClass("text-secondary loading-indicator") // Added class to easily find it later if needed
        .append("&nbsp;&nbsp;Uploading... ")
        .append("<div class='spinner-border spinner-border-sm' role='status'><span class='visually-hidden'>Loading...</span></div>");


    alert.append(filename);
    alert.append(loading);
    $("#quoFiles").append(alert);

    await delay(1000);

    $.ajax({
        url: "/Pdf/UploadQuotation",
        type: "POST",
        data: formData,
        contentType: false,
        processData: false,
        success: function (response) {
            console.log(response.message);

            if (response.success) {
                loading.remove();
                filename.remove();

                alert.addClass("shadow-sm");
                alert.prepend(`<i class="bi bi-check-lg text-success"></i> <a href="#" class="alert-link text-decoration-none" onclick=previewQuotation(this) data-filepath="${response.filePath}">${file.name}</a>`);
            } else {
                loading.remove();
                alert.removeClass("alert-light").addClass("alert-danger").append(" - Failed to upload.");
            }

            alert.append(`<button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>`);
        },
        error: function (response) {
            console.log(response);
        }
    });
}

$(".alert-link").on("click", function () {
    //var filePath = this.data("filepath");
    console.log(yes);
});

function previewQuotation(thisLink) {
    var e = $(thisLink);
    $('#pdfFrame').attr("src", `${e.data("filepath")}#toolbar=0&view=Fit&navpanes=0`);
    $("#pdfPreviewModal").modal("show");
}

function delay(ms) {
    return new Promise(resolve => setTimeout(resolve, ms));
}

function handleAjaxError(response) {
    console.error("Error:", response.status, response.responseJSON?.message);
}