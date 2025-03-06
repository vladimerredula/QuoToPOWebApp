$(document).ready(function () {
    changeLang();
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

$("#Po_language").on("change", function () {
    changeLang();
});


function changeLang() {
    var lang = $("#Po_language").val();
    var isEnglish = (lang === "en");

    function toggleVisibility(selector, isVisible) {
        $(selector).toggleClass("visually-hidden", !isVisible);
    }

    function toggleRequired(selector, isRequired) {
        $(selector).prop("required", isRequired);
    }

    function toggleDisabled(selector, isDisabled) {
        $(selector).prop("disabled", isDisabled);
    }

    // Toggle language-specific elements
    toggleVisibility(".en", isEnglish);
    toggleVisibility(".jp", !isEnglish);

    // Toggle required indicators
    toggleVisibility("#poTitleRequired", isEnglish);
    toggleVisibility("#quotationNumberRequired", isEnglish);
    toggleVisibility("#correspondentRequired", isEnglish);

    // Set input states
    toggleRequired("#Po_title", isEnglish);
    toggleRequired("#Quotation_number", isEnglish);
    toggleRequired("#Correspondent_ID", isEnglish);

    toggleDisabled("#Quotation_number", !isEnglish);
    toggleDisabled("#Correspondent_ID", !isEnglish);

    // Adjust label styling
    $("#correspondentLabel").toggleClass("text-secondary", !isEnglish);
    $("#quoNumberLabel").toggleClass("text-secondary", !isEnglish);
}

function setTerm(term) {
    var selected = $(term).text();
    $(term).closest('.input-group').find('input').first().val(selected);
}

// Auto adjust the height of textarea based on its content
$("textarea").on("input", function () {
    this.style.height = "auto"; // Reset height
    this.style.height = (this.scrollHeight) + "px"; // Set new height based on content
});

$("#Quotation_date").change(function () {
    let dateValue = $(this).val(); // Get the selected date (YYYY-MM-DD)
    formatDateToPoNumber(dateValue);
});


// Function to format date from YYYY-MM-DD to DD/MM/YYYY
function formatDateToPoNumber(dateString) {
    let date = new Date(dateString);
    let day = date.getDate().toString().padStart(2, "0");
    let month = (date.getMonth() + 1).toString().padStart(2, "0");
    let year = date.getFullYear();
    let id = "000";

    $.ajax({
        url: "/User/PersonnelID",
        method: 'GET',
        success: function (response) {
            id = response.toString().padStart(3, '0');

            $("#Po_number").val(`${year}${month}${day}/FF-000-${id}`);
        },
        error: function (xhr, status, error) {
            console.log(error);
        }
    });
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

        var itemnameDisplay = itemname.replace(/\n/g, "<br>");

        itemTable.row.add([
            "",
            `<div><span>${itemorder}</span><input hidden name='Quotation_items[${itemTable.rows().count()}].Order' value='${itemorder}'></div>`,
            `<div><span>${itemnameDisplay}</span><textarea hidden name='Quotation_items[${itemTable.rows().count()}].Item_name'>${itemname}</textarea></div>`,
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
    itemName.find("textarea").val(itemname);
    itemName.find("textarea").text(itemname);

    var itemnameDisplay = itemname.replace(/\n/g, "<br>");
    itemName.find("span").html(itemnameDisplay);

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

    itemTable.rows().deselect();

    reorderTable();

    $('#editItem').addClass('disabled');
    $('#removeItem').addClass('disabled');

    calculateTotal();
});

$("#editItem").on("click", function () {
    var row = itemTable.row(".selected");
    var rowData = row.data();

    var itemName = $(rowData[2]).find("textarea").val();
    var itemQty = $(rowData[3]).find("input").val();
    var itemUnit = $(rowData[4]).find("input").val();
    var itemPrice = $(rowData[5]).find("input").val();

    $("#itemEditName").val(itemName);
    $("#itemEditQuantity").val(itemQty);
    $("#itemEditUnit").val(itemUnit);
    $("#itemEditPrice").val(getFloatValue(itemPrice));

    $("#itemEditModal").modal("show");
});

$("#generatePo").on("click", function () {
    $("#poForm").submit();
})

function reorderTable() {

    // function to update the asp item names according to the row order in table
    function updateCell(data, rowIndex, colIndex, inputType, inputName, spanUpdate = false) {
        var element = $(data[colIndex]);

        element.find(inputType).attr("name", `Quotation_items[${rowIndex}].${inputName}`);

        if (spanUpdate) {
            element.find("span").text(rowIndex + 1);  // For Order column
        }

        data[colIndex] = element.prop("outerHTML");
    }

    // update this array if you need to add/remove columns
    var columns = [
        { colIndex: 1, inputType: "input", inputName: "Order", hasSpan: true },
        { colIndex: 2, inputType: "textarea", inputName: "Item_name" },
        { colIndex: 3, inputType: "input", inputName: "Item_quantity" },
        { colIndex: 4, inputType: "input", inputName: "Unit" },
        { colIndex: 5, inputType: "input", inputName: "Item_price" }
    ];

    var rowIndex = 0;
    itemTable.rows().every(function () {
        var data = this.data();

        columns.forEach(function (col) {
            updateCell(data, rowIndex, col.colIndex, col.inputType, col.inputName, col.hasSpan || false);
        });

        this.data(data);
        rowIndex++;
    });
}

$("#poPreview").on("click", function () {
    var form = $("#poForm");
    if (form.valid()) {
        reorderTable();

        // Serialize form data
        var formData = form.serialize();

        $.ajax({
            url: "/Po/PreviewPo",
            method: 'POST',
            data: formData,
            success: function (response) {
                var dataUrl = response.pdfDataUrl;
                $('#pdfFrame').attr('src', dataUrl + "#toolbar=0&view=Fit&navpanes=0&scrollbar=0");
                $("#pdfPreviewModal").modal("show");
            },
            error: function (xhr, status, error) {
                console.log(error);
            }
        });
    }
})

$("#poForm").on("submit", function (e) {
    if ($(this).valid()) {
        e.preventDefault();

        reorderTable();

        this.submit();
    }
});

function addCustomContactPerson() {
    if ($("#contactPersonForm").valid()) {
        $.ajax({
            url: '/Info/AddCustomContactPerson',
            type: 'POST',
            data: {
                Company_name: $("#Company_name").val(),
                Company_name_jpn: $("#Company_name_jpn").val(),
                Address: $("#Company_address").val(),
                Address_jpn: $("#Company_address_jpn").val(),
                Telephone: $("#Telephone").val(),
                Fax: $("#Fax").val(),
                Postal_code: $("#Postal_code").val(),
                Contact_person: $("#Contact_person").val(),
                Contact_person_jpn: $("#Contact_person_jpn").val()
            },
            success: function (response) {
                var contactPerson = $("#Contact_person_ID");
                contactPerson.append(new Option(response.companyName, response.contactPersonId));
                contactPerson.val(response.contactPersonId).trigger("change");
                $("#contactPersonModal").modal("hide");

                $("#Company_name").val(null);
                $("#Company_name_jpn").val(null);
                $("#Company_address").val(null);
                $("#Company_address_jpn").val(null);
                $("#Telephone").val(null);
                $("#Fax").val(null);
                $("#Postal_code").val(null);
                $("#Contact_person").val(null);
                $("#Contact_person_jpn").val(null);
            },
            error: function (response) {
                console.log(response);
            }
        });
    }
};

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
    if (typeof string == "string") {
        return Number(string.replace(/,/g, ''));
    }

    return string;
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
                var ptEng = response.paymentTermName;
                var ptJpn = response.paymentTermNameJpn;

                var paymentTerm = $("#paymentTermDropdown");
                paymentTerm.append($("<li>")
                    .addClass("en visually-hidden")
                    .append(`<a class='dropdown-item' href='#' onclick='setTerm(this)'>${ptEng}</a>`)
                );

                paymentTerm.append($("<li>")
                    .addClass("jp visually-hidden")
                    .append(`<a class='dropdown-item' href='#' onclick='setTerm(this)'>${ptJpn}</a>`)
                );

                changeLang();

                var lang = $("#Po_language").val();

                $("#Payment_term").val(lang == "en" ? ptEng : ptJpn)
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
                var dtEng = response.deliveryTermName;
                var dtJpn = response.deliveryTermNameJpn;

                var deliveryTerm = $("#deliveryTermDropdown");
                deliveryTerm.append($("<li>")
                    .addClass("en visually-hidden")
                    .append(`<a class='dropdown-item' href='#' onclick='setTerm(this)'>${dtEng}</a>`)
                );

                deliveryTerm.append($("<li>")
                    .addClass("jp visually-hidden")
                    .append(`<a class='dropdown-item' href='#' onclick='setTerm(this)'>${dtJpn}</a>`)
                );

                changeLang();

                var lang = $("#Po_language").val();

                $("#Delivery_term").val(lang == "en" ? dtEng : dtJpn)
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