
$(document).ready(function () {
    getThumbnail();
    taxSwitch();

    var supplierId = $("#Quotation_Supplier_ID").val();

    getSupplierAddress(supplierId);
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
        $('#selectFileInput').val(data.fileName);
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

$("#Include_tax").on("change", function () {
    taxSwitch();
});

$("#submitItem").on("click", function () {
    var itemname = $('#itemName').val();
    var quantity = parseInt($('#quantity').val());
    var price = parseFloat($('#price').val());
    var itemno = itemTable.rows().count() + 1;
    var totalPrice = parseFloat(quantity * price).toLocaleString();

    $("#itemModal").modal("hide");
    $('#itemName').val(null);
    $('#quantity').val(null);
    $('#price').val(null);

    itemTable.row.add([
        "",
        itemno,
        `<span>${itemname}</span><input hidden name='Quotation_items[${itemTable.rows().count()}].Item_name' value='${itemname}'>`,
        `<span>${quantity}</span><input hidden name='Quotation_items[${itemTable.rows().count()}].Item_quantity' value='${quantity}'>`,
        `<span>${price.toLocaleString()}</span><input hidden name='Quotation_items[${itemTable.rows().count()}].Item_price' value='${price}'>`,
        `<span class='itemPrice'>${totalPrice}</span>`
    ]).draw();
    calculateTotal();
});

$("#saveItem").on("click", function () {
    var itemname = $('#itemEditName').val();
    var quantity = parseInt($('#itemEditQuantity').val());
    var price = parseFloat($('#itemEditPrice').val());
    var totalPrice = parseFloat(quantity * price).toLocaleString();

    $("#itemEditModal").modal("hide");
    $('#itemEditName').val(null);
    $('#itemEditQuantity').val(null);
    $('#itemEditPrice').val(null);

    var row = itemTable.row(".selected");
    var rowData = row.data();

    rowData[2] = `<span>${itemname}</span><input hidden name='Quotation_items[${itemTable.rows().count()}].Item_name' value='${itemname}'>`;
    rowData[3] = `<span>${quantity}</span><input hidden name='Quotation_items[${itemTable.rows().count()}].Item_quantity' value='${quantity}'>`;
    rowData[4] = `<span>${price.toLocaleString()}</span><input hidden name='Quotation_items[${itemTable.rows().count()}].Item_price' value='${price}'>`;
    rowData[5] = `<span class='itemPrice'>${totalPrice}</span>`;

    row.data(rowData).draw();

    calculateTotal();
});

$("#customSupplier").on("click", function () {
    $("#supplierModal").modal("show");
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

    var itemName = $(rowData[2]).next("input").val();
    var itemQty = $(rowData[3]).next("input").val();
    var itemPrice = $(rowData[4]).next("input").val();

    $("#itemEditName").val(itemName);
    $("#itemEditQuantity").val(itemQty);
    $("#itemEditPrice").val(getFloatValue(itemPrice));

    $("#itemEditModal").modal("show");
});

function addCustomSupplier() {
    $.ajax({
        url: '/Info/AddCustomSupplier',
        type: 'POST',
        data: {
            Company_name: $("input[name=Company_name]").val(),
            Company_address: $("textarea[name=Company_address]").val(),
            Telephone: $("input[name=Telephone]").val(),
            Fax: $("input[name=Fax]").val(),
            Contact_person: $("input[name=Contact_person]").val()
        },
        success: function (response) {
            var supplier = $("#Quotation_Supplier_ID");
            supplier.append(new Option(response.companyName, response.supplerId));
            supplier.val(response.supplerId).trigger("change");
            $("#supplierModal").modal("hide");
        },
        error: function (response) {
            console.log(response);
        }
    });
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