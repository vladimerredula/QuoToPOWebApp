$(document).ready(function () {
    taxSwitch();
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

    itemTable.rows().deselect();

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

$("#Contact_person_ID").change(function () {
    const id = $(this).val();
    getContactPersonAddress(id);
});

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

$("#Delivery_address_ID").change(function () {
    const id = $(this).val();
    getDeliveryAddress(id);
});

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

$("#customPaymentTerm").on("click", function () {
    $("#paymentTermAddModal").modal("show");
});

function addPaymentTerm() {
    if ($("#paymentTermForm").valid()) {
        $.ajax({
            url: '/Info/AddCustomPaymentTerm',
            type: 'POST',
            data: {
                PaymentTermName: $("#payTermName").val(),
                PaymentTermNameJpn: $("#payJpnName").val()
            },
            success: function (response) {
                var paymentTerm = $("#Payment_term_ID");
                paymentTerm.append(new Option(response.paymentTermName, response.paymentTermId));
                paymentTerm.val(response.paymentTermId);
                $("#paymentTermAddModal").modal("hide");
            },
            error: function (response) {
                console.log(response);
            }
        });

        $("#payTermName").val(null);
        $("#payJpnName").val(null);
    }
}

$("#customDeliveryTerm").on("click", function () {
    $("#deliveryTermAddModal").modal("show");
});

function addDeliveryTerm() {
    if ($("#deliveryTermForm").valid()) {
        $.ajax({
            url: '/Info/AddCustomDeliveryTerm',
            type: 'POST',
            data: {
                DeliveryTermName: $("#delTermName").val(),
                DeliveryTermNameJpn: $("#delJpnName").val()
            },
            success: function (response) {
                var deliveryTerm = $("#Delivery_term_ID");
                deliveryTerm.append(new Option(response.deliveryTermName, response.deliveryTermId));
                deliveryTerm.val(response.deliveryTermId);
                $("#deliveryTermAddModal").modal("hide");
            },
            error: function (response) {
                console.log(response);
            }
        });

        $("#delTermName").val(null);
        $("#delJpnName").val(null);
    }
}