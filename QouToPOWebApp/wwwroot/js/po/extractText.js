
$(document).ready(function () {
    //getThumbnail();
    getPdfRender();
    taxSwitch();

    var contactPersonId = $("#Contact_person_ID").val();
    getContactPersonAddress(contactPersonId);
    var companyId = $("#Delivery_address_ID").val();
    getDeliveryAddress(companyId);
});

var itemTable = $('table.datatable').DataTable({
    info: false,
    responsive: true,
    fixedHeader: true,
    paging: false,
    select: {
        style: 'single',
        info: false
    },
    filter: false,
    columnDefs: [
        {
            className: 'reorder',
            render: function () {
                return '≡';
            },
            targets: 0
        },
        {
            targets: [0, 1],
            width: "50px"
        },
        { orderable: false, targets: '_all' }
    ],
    order: [[1, 'asc']],
    rowReorder: {
        cancelable: true,
        dataSrc: 1
    }
});

$("#selectFileBtn").on("click", function () {
    $("#pdfFile").click(); // Trigger the hidden file input
});

$("#Contact_person_ID").change(function () {
    const id = $(this).val();
    getContactPersonAddress(id);
});

$("#Delivery_address_ID").change(function () {
    const id = $(this).val();
    getDeliveryAddress(id);
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
    $("#dropArea").addClass("visually-hidden");

    $('#pdfPreview').attr("src", data.image + "#toolbar=0&view=Fit&navpanes=0");
    $('#pdfPreview').attr("filename", data.filePath);
    $('#filePath').val(data.filePath);
    $('#selectFileInput').val(data.fileName);
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

function renderPreview(data) {
    if (data.image) {
        $('#pdfPreview').attr("src", data.image);
        $('#pdfPreview').attr("filename", data.filePath);
        $('#filePath').val(data.filePath);
        $('#selectFileInput').val(data.fileName);
        $('#preview').removeClass("visually-hidden");

        $("#currentPage").text(data.pageIndex + 1);
        $("#totalPage").text(data.totalPages);

        $("#prev").toggleClass("disabled", data.pageIndex == 0);
        $("#next").toggleClass("disabled", data.totalPages - data.pageIndex == 1);
    }
}

function getContactPersonAddress(id) {
    if (id != null) {
        $.ajax({
            url: '/Info/GetContactPersonAddress',
            type: 'POST',
            data: {
                id: id
            },
            success: function (response) {
                $("#contactPersonAddress").val(response);
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

$("#Include_tax").on("change", function () {
    taxSwitch();
});


// adding item
$("#submitItem").on("click", function () {
    if ($("#itemForm").valid()) {
    var itemname = $('#itemName').val();
    var quantity = parseInt($('#quantity').val());
        var unit = $('#unit').val();
    var price = parseFloat($('#price').val());
        var itemorder = itemTable.rows().count() + 1;
        var totalprice = parseFloat(quantity * price);

        $("#itemModal").modal("hide");
        $('#itemName').val(null);
        $('#unit').val(null);
        $('#quantity').val(null);
        $('#price').val(null);

        itemTable.row.add([
            "",
            `<div><span>${itemorder}</span><input hidden name='Quotation_items[${itemTable.rows().count()}].Order' value='${itemorder}'></div>`,
            `<div><span>${itemname}</span><input hidden name='Quotation_items[${itemTable.rows().count()}].Item_name' value='${itemname}'></div>`,
            `<div><span>${quantity}</span><input hidden name='Quotation_items[${itemTable.rows().count()}].Item_quantity' value='${quantity}'></div>`,
            `<div><span>${unit}</span><input hidden name='Quotation_items[${itemTable.rows().count()}].Unit' value='${unit}'></div>`,
            `<div><span>${price.toLocaleString()}</span><input hidden name='Quotation_items[${itemTable.rows().count()}].Item_price' value='${price}'></div>`,
            `<span class='itemPrice'>${totalprice.toLocaleString()}</span>`
        ]).draw();
        calculateTotal();
    }
});

// updating item
$("#saveItem").on("click", function () {
    var itemname = $('#itemEditName').val();
    var quantity = parseInt($('#itemEditQuantity').val());
    var unit = $('#itemEditUnit').val();
    var price = parseFloat($('#itemEditPrice').val());
    var totalprice = parseFloat(quantity * price);

        $("#itemEditModal").modal("hide");
    $('#itemEditOrder').val(null);
        $('#itemEditName').val(null);
        $('#itemEditQuantity').val(null);
    $('#itemEditUnit').val(null);
        $('#itemEditPrice').val(null);

        var row = itemTable.row(".selected");
        var rowData = row.data();

    var itemName = $(rowData[2]);
    itemName.find("input").attr("value", itemname);
    itemName.find("span").text(itemname);
    var itemQty = $(rowData[3]);
    itemQty.find("input").attr("value", quantity);
    itemQty.find("span").text(quantity);
    var itemUnit = $(rowData[4]);
    itemUnit.find("input").attr("value", unit);
    itemUnit.find("span").text(unit);
    var itemPrice = $(rowData[5]);
    itemPrice.find("input").attr("value", price);
    itemPrice.find("span").text(price.toLocaleString());

    rowData[2] = itemName.prop("outerHTML");
    rowData[3] = itemQty.prop("outerHTML");
    rowData[4] = itemUnit.prop("outerHTML");
    rowData[5] = itemPrice.prop("outerHTML");
    rowData[6] = `<span class="itemPrice">${totalprice.toLocaleString()}</span>`;

        row.data(rowData).draw();

        calculateTotal();
});

$("#customContactPerson").on("click", function () {
    $("#contactPersonModal").modal("show");
});

$("#addItem").on("click", function () {
    $("#itemModal").modal("show");
});

// Enable button when a row is selected
itemTable.on('select.dt', function () {
    $('#editItem').removeClass('disabled');
    $('#removeItem').removeClass('disabled');
});

// Disable button when no row is selected
itemTable.on('deselect.dt', function () {
    if (!itemTable.rows('.selected').any()) {
        $('#editItem').addClass('disabled');
        $('#removeItem').addClass('disabled');
    }
});

$("#removeItem").on("click", function () {
    itemTable.row(".selected").remove().draw(false);
    calculateTotal();
});

$("#editItem").on("click", function () {
    var row = itemTable.row(".selected");
    var rowData = row.data();

    var itemName = $(rowData[2]).find("input").val();
    var itemQty = $(rowData[3]).find("input").val();
    var itemUnit = $(rowData[4]).find("input").val();
    var itemPrice = $(rowData[5]).find("input").val();

    $("#itemEditName").val(itemName);
    $("#itemEditQuantity").val(itemQty);
    $("#itemEditUnit").val(itemUnit);
    $("#itemEditPrice").val(getFloatValue(itemPrice));

    $("#itemEditModal").modal("show");
});

function addCustomContactPerson() {
    if ($("#contactPersonForm").valid()) {
        $.ajax({
            url: '/Info/AddCustomContactPerson',
            type: 'POST',
            data: {
                Company_name: $("input[name=Company_name]").val(),
                Company_address: $("textarea[name=Company_address]").val(),
                Telephone: $("input[name=Telephone]").val(),
                Fax: $("input[name=Fax]").val(),
                Contact_person: $("input[name=Contact_person]").val()
            },
            success: function (response) {
                var contactPerson = $("#Contact_person_ID");
                contactPerson.append(new Option(response.companyName, response.supplerId));
                contactPerson.val(response.supplerId).trigger("change");
                $("#contactPersonModal").modal("hide");
            },
            error: function (response) {
                console.log(response);
            }
        });
    }
};

function taxSwitch() {
    if ($("#Include_tax").prop("checked")) {
        $("#tax").removeClass("visually-hidden");
        $("#totalTax").text("Including");
    } else {
        $("#tax").addClass("visually-hidden");
        $("#totalTax").text("Without");
    }

    calculateTotal();
}

function calculateTotal() {
    var totalPrice = 0.0;
    var itemPrices = $(".itemPrice");

    itemPrices.each(function (index, element) {
        totalPrice += parseFloat($(element).text().replace(/,/g, ''));
    });

    $("#subTotal").text(totalPrice.toLocaleString());


    if ($("#Include_tax").prop("checked")) {
        var taxValue = totalPrice * 0.1;

        $("#taxValue").text(taxValue.toLocaleString());
        totalPrice += parseFloat(taxValue);
    }

    $("#totalAmount").text(totalPrice.toLocaleString());
}

function getFloatValue(string) {
    return parseFloat(string.replace(/,/g, ''))
}