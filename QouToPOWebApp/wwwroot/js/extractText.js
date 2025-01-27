
$(document).ready(function () {
    getThumbnail();

    var supplierId = $("#Quotation_Supplier_ID").val();

    getSupplierAddress(supplierId);
});

$("#Quotation_Supplier_ID").change(function () {
    const id = $(this).val();
    getSupplierAddress(id);
});

$("#deliveryID").change(function () {
    const id = $(this).val();
    getDeliveryAddress(id);
});

function submitForm() {
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
        // Perform the AJAX request using fetch
        const response = await fetch('/Pdf/UploadPdf', {
            method: 'POST',
            body: formData
        });

        const data = await response.json();

        renderPreview(data);
    } catch (error) {
        console.error('Upload error:', error);
    }
}

function nextPage() {
    const currentPage = parseInt($("#currentPage").text());
    getThumbnail(currentPage);
}

function prevPage() {
    const currentPage = parseInt($("#currentPage").text()) - 2;
    getThumbnail(currentPage);
}

function getThumbnail(pageIndex) {
    const preview = $('#pdfPreview');
    const filePath = preview.attr("filename");
    if (!filePath) {
        return;
    }

    if (pageIndex == null) {
        pageIndex = 0;
    }

    $.ajax({
        url: '/Pdf/GenerateThumbnail',
        type: 'POST',
        data: {
            filePath: filePath,
            pageIndex: pageIndex
        },
        success: function (data) {
            renderPreview(data);
        },
        error: function (response) {
            console.log(response);
        }
    });
}

function renderPreview(data) {
    if (data.image) {
        $('#pdfPreview').attr("src", data.image);
        $('#pdfPreview').attr("filename", data.filePath);
        $('#Quotation_File_path').val(data.filePath);
        $('#preview').removeClass("visually-hidden");

        $("#currentPage").text(data.pageIndex + 1);
        $("#totalPage").text(data.totalPages);

        $("#prev").toggleClass("disabled", data.pageIndex == 0);
        $("#next").toggleClass("disabled", data.totalPages - data.pageIndex == 1);
        console.log($('#Quotation_File_path').val());
    }
}

function getSupplierAddress(id) {
    if (id != null) {
        $.ajax({
            url: '/Info/GetSupplierAddress',
            type: 'POST',
            data: {
                id: id
            },
            success: function (response) {
                $("#supplierAddress").val(response);
            },
            error: function (response) {
                console.log(response);
            }
        });
    }
}

function getDeliveryAddress(id) {
    if (id != null) {
        $.ajax({
            url: '/Info/GetDeliveryAddress',
            type: 'POST',
            data: {
                id: id
            },
            success: function (response) {
                $("#deliveryAddress").val(response);
            },
            error: function (response) {
                console.log(response);
            }
        });
    }
}