﻿$(document).ready(function () {
    changeLang();
    taxSwitch();

    $(".autocomplete-input").autocomplete({
        source: function (request, response) {
            $.ajax({
                url: "/Po/GetItemSuggestions",
                type: "GET",
                data: { term: request.term },
                success: function (data) {
                    response(data); // Return results to autocomplete
                }
            });
        },
        minLength: 1 // Start searching after typing 1 character
    });

    setCustomTermIndicator();
});

const toolbarOptions = [
    ['bold', 'italic', 'underline', 'strike'],        // toggled buttons
    [{ 'list': 'ordered' }, { 'list': 'bullet' }],
    ['clean']                                         // remove formatting button
];

var quill = new Quill("#editor", {
    modules: {
        toolbar: toolbarOptions
    },
    placeholder: 'Custom term...',
    theme: 'snow'
});

quill.on("text-change", (delta, oldDelta, source) => {
    // Get the HTML from the Quill editor
    var html = quill.container.firstChild.innerHTML;

    // Update the hidden textarea using jQuery
    $("#Po_Custom_term").val(html);

    setCustomTermIndicator();
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
        { targets: 7, visible: false },
        { orderable: false, targets: '_all' },
        { targets: 2, className: "text-start" } // Bootstrap class for left alignment
    ],
    order: [[1, 'asc']],
    rowReorder: {
        cancelable: true,
        dataSrc: 1
    }
});

/// Event Handlers
itemTable.on('row-reorder', function (e, details, edit) {
    console.log("Rows reordered!");

    reorderTable();
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

$("#submitItem").on("click", function () {
    if ($("#itemForm").valid()) {
        addItem();
    }
});

$("#saveItem").on("click", function () {
    if ($("#itemEditForm").valid()) {
        updateItem();
    }
});

$("#Po_Include_tax").on("change", function () {
    taxSwitch();
});

$("#Po_Po_language").on("change", function () {
    changeLang();
});

$("#Po_Po_date").change(function () {
    let dateValue = $(this).val(); // Get the selected date (YYYY-MM-DD)
    formatDateToPoNumber(dateValue);
});

$("#customContactPerson").on("click", function () {
    $("#contactPersonModal").modal("show");
});

$("#addItem").on("click", function () {
    $("#itemModal").modal("show");
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
    $("#templateForm").submit();
})

$("#templateForm").on("submit", function (e) {
    if ($(this).valid()) {
        e.preventDefault();
        reorderTable();
        this.submit();
    }
});

$("#poPreview").on("click", function () {
    var form = $("#templateForm");
    if (form.valid()) {
        reorderTable();
        var formData = form.serialize();
        previewPo(formData);
    }
});

$("#templateSave").on("click", function () {
    if ($("#templateForm").valid()) {
        $("#templateModal").modal("show");
    }
});

$("#saveTemplate").on("click", function () {
    if ($("#templateForm").valid()) {
        let formData = new FormData($("#templateForm")[0]); // Convert form to FormData
        formData.append("templateName", $("#templateName").val());

        $.ajax({
            url: '/Po/SavePoTemplate',
            type: 'POST',
            data: formData,
            processData: false,
            contentType: false,
            success: function (response) {
                $("#templateModal").modal("hide");
                showToast(response.message);
            },
            error: function (error) {
                $("#templateError").text(error.responseJSON.message);
            }
        });
    }
});

$("#Po_Currency").on("change", function () {
    var currencyCode = $("#Currency").val();
    $(".currencySign").text(currencyCode == 'USD' ? '$' : '¥');
    calculateTotal();
});

$("#customTermBtn").on("click", function () {
    $("#customTermModal").modal("show");
});

// Functions
function formatCurrency(amount) {
    if (amount == null || isNaN(amount)) {
        console.warn("Invalid amount:", amount);
        return '';
    }

    let currencyCode = $("#Po_Currency").val();
    let numericAmount = Number(amount);

    if (currencyCode === 'JPY') {
        // Round to whole number for Yen
        return Math.round(numericAmount).toLocaleString('ja-JP', {
            minimumFractionDigits: 0,
            maximumFractionDigits: 0
        });
    } else {
        // For USD or others: format with up to 2 decimals
        return numericAmount.toLocaleString('en-US', {
            minimumFractionDigits: 0,
            maximumFractionDigits: 2
        });
    }
}

function setTerm(term) {
    var selected = $(term).text();
    $(term).closest('.input-group').find('input').first().val(selected);
}

function formatDate(date) {
    let d = new Date(date);
    return {
        day: d.getDate().toString().padStart(2, "0"),
        month: (d.getMonth() + 1).toString().padStart(2, "0"),
        year: d.getFullYear()
    };
}

async function formatDateToPoNumber(dateString) {
    let { day, month, year } = formatDate(dateString);
    let id = "000";
    let poCount = await getPoCount();

    $.ajax({
        url: "/User/PersonnelID",
        method: 'GET',
        success: function (response) {
            id = response.toString().padStart(3, '0');

            $("#Po_Po_number").val(`${year}${month}${day}/FF-${poCount}-${id}`);
        },
        error: handleAjaxError
    });
}

async function getPoCount() {
    let poCount = "000";

    await $.ajax({
        url: "/Po/GetPoCount",
        method: 'GET',
        success: function (response) {
            let count = (response.poCount + 1).toString().padStart(3, '0');

            poCount = count;
        },
        error: handleAjaxError
    });

    return poCount;
}

function changeLang() {
    var lang = $("#Po_Po_language").val();
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
    toggleRequired("#Po_Po_title", isEnglish);
    toggleRequired("#Po_Quotation_number", isEnglish);
    toggleRequired("#Po_Correspondent_ID", isEnglish);

    toggleDisabled("#Po_Quotation_number", !isEnglish);
    toggleDisabled("#Po_Correspondent_ID", !isEnglish);

    // Adjust label styling
    $("#correspondentLabel").toggleClass("text-secondary", !isEnglish);
    $("#quoNumberLabel").toggleClass("text-secondary", !isEnglish);
}

function addItem() {
    var item = getItemDetails('#itemName', '#quantity', '#unit', '#price');
    var itemorder = itemTable.rows().count() + 1;

    clearItemForm();
    $("#itemModal").modal("hide");

    addTableRow(itemorder, item);

    calculateTotal();
}

function updateItem() {
    var item = getItemDetails('#itemEditName', '#itemEditQuantity', '#itemEditUnit', '#itemEditPrice');

    clearEditForm();
    $("#itemEditModal").modal("hide");

    updateTableRow(item);
    calculateTotal();
}

// Helper: Get item details from form
function getItemDetails(nameSelector, qtySelector, unitSelector, priceSelector) {
    return {
        name: $(nameSelector).val(),
        quantity: parseInt($(qtySelector).val()),
        unit: $(unitSelector).val(),
        price: parseFloat($(priceSelector).val())
    };
}

// Helper: Clear add item form
function clearItemForm() {
    $('#itemName').val(null);
    $('#unit').val(null);
    $('#quantity').val(null);
    $('#price').val(null);
}

// Helper: Clear edit item form
function clearEditForm() {
    $('#itemEditOrder').val(null);
    $('#itemEditName').val(null);
    $('#itemEditQuantity').val(null);
    $('#itemEditUnit').val(null);
    $('#itemEditPrice').val(null);
}

// Helper: Clear editor form
function clearEditor() {
    quill.setContents([]);
    quill.setSelection(0); // Move cursor to start
    quill.history.clear(); 
}

function setCustomTermIndicator() {
    if (quill.getLength() > 1) {
        $("#customTermIndicator").html('<i class="bi bi-check-circle-fill"></i>');
    } else {
        $("#customTermIndicator").html('<i class="bi bi-circle"></i>');
    }
}

// Helper: Add new row to table
function addTableRow(order, item) {
    var itemIndex = itemTable.rows().count();
    var itemnameDisplay = item.name.replace(/\n/g, "<br>");
    var totalprice = item.quantity * item.price;

    itemTable.row.add([
        "",
        `<span>${order}</span>`,
        createCellWithTextarea(itemnameDisplay, item.name, `Po.Po_items[${itemIndex}].Item_name`),
        createCell(item.quantity, `Po.Po_items[${itemIndex}].Item_quantity`),
        createCell(item.unit, `Po.Po_items[${itemIndex}].Unit`),
        createCell(item.price, `Po.Po_items[${itemIndex}].Item_price`, item.price),
        `<span class='itemPrice'>${formatCurrency(totalprice)}</span>`,
        `<div><input hidden name='Po.Po_items[${itemIndex}].Order' value='${order}'></div>`
    ]).draw();
}

// Helper: Update existing row
function updateTableRow(item) {
    var row = itemTable.row(".selected");
    var rowData = row.data();
    var totalprice = item.quantity * item.price;

    updateCell(rowData, 2, item.name, `textarea`, item.name.replace(/\n/g, "<br>"));
    updateCell(rowData, 3, item.quantity);
    updateCell(rowData, 4, item.unit);
    updateCell(rowData, 5, item.price, `input`, item.price);

    rowData[6] = `<span class="itemPrice">${formatCurrency(totalprice)}</span>`;

    row.data(rowData).draw();
}

// Helper: Create table cell (span + hidden input)
function createCell(displayText, inputName, inputValue = displayText) {
    return `
        <div>
            <span>${displayText ?? ""}</span>
            <input hidden name='${inputName}' value='${inputValue ?? ""}'>
        </div>`;
}

// Helper: Create table cell (span + hidden textarea)
function createCellWithTextarea(displayText, textareaValue, inputName) {
    return `
        <div>
            <span>${displayText}</span>
            <textarea hidden name='${inputName}'>${textareaValue}</textarea>
        </div>`;
}

// Helper: Update existing table cell
function updateCell(rowData, index, value, inputType = "input", displayText = value) {
    var cell = $(rowData[index]);
    cell.find(inputType).val(value).attr("value", value);
    cell.find("span").html(displayText);
    rowData[index] = cell.prop("outerHTML");
}

function reorderTable() {

    // function to update the asp item names according to the row order in table
    function updateCell(data, rowIndex, colIndex, inputType, inputName) {
        var element = $(data[colIndex]);

        element.find(inputType).attr("name", `Po.Po_items[${rowIndex}].${inputName}`);

        if (inputName === "Order") {
            element.find("input").val(rowIndex + 1);
        }

        data[colIndex] = element.prop("outerHTML");
    }

    // update this array if you need to add/remove columns
    var columns = [
        { colIndex: 2, inputType: "textarea", inputName: "Item_name" },
        { colIndex: 3, inputType: "input", inputName: "Item_quantity" },
        { colIndex: 4, inputType: "input", inputName: "Unit" },
        { colIndex: 5, inputType: "input", inputName: "Item_price" },
        { colIndex: 7, inputType: "input", inputName: "Order" }
    ];

    var rowIndex = 0;
    itemTable.rows().every(function () {
        var data = this.data();

        columns.forEach(function (col) {
            updateCell(data, rowIndex, col.colIndex, col.inputType, col.inputName);
        });

        this.data(data);
        rowIndex++;
    });

    if (rowIndex > 0) {
        console.log("Table reordered!");
    }
}

function previewPo(formData) {
    $.ajax({
        url: "/Po/PreviewPo",
        method: 'POST',
        data: formData,
        success: function (response) {
            $('#pdfFrame').attr('src', response.pdfDataUrl + "#toolbar=0&view=Fit&navpanes=0&scrollbar=0");
            $("#pdfPreviewModal").modal("show");
        },
        error: handleAjaxError
    });
}

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
            error: handleAjaxError
        });
    }
};

function taxSwitch() {
    $("#tax").toggleClass("visually-hidden", !$("#Po_Include_tax").prop("checked"));
    $("#totalTax").text($("#Po_Include_tax").prop("checked") ? "Including" : "Without");

    calculateTotal();
}

function calculateTotal() {
    calculateItemPrices();
    var totalPrice = 0.0;
    var itemPrices = $(".itemAmount");

    itemPrices.each(function (index, element) {
        totalPrice += parseFloat($(element).text().replace(/,/g, ''));
    });

    $("#subTotal").text(totalPrice.toLocaleString());


    if ($("#Po_Include_tax").prop("checked")) {
        var taxValue = totalPrice * 0.1;

        $("#taxValue").text(taxValue.toLocaleString());
        totalPrice += parseFloat(taxValue);
    }

    $("#totalAmount").text(totalPrice.toLocaleString());
}

function calculateItemPrices() {
    itemTable.rows().every(function () {
        var data = this.data();

        var itemQuantity = $(data[3]).find("input").val();
        var itemPrice = $(data[5]).find("input").val();
        var totalPrice = itemQuantity * itemPrice;

        data[6] = `<span class="itemAmount">${formatCurrency(totalPrice)}</span>`;

        this.data(data);
    });
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

                var lang = $("#Po_Po_language").val();

                $("#Po_Payment_term").val(lang == "en" ? ptEng : ptJpn)
                $("#paymentTermAddModal").modal("hide");
            },
            error: handleAjaxError
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

                var lang = $("#Po_Po_language").val();

                $("#Po_Delivery_term").val(lang == "en" ? dtEng : dtJpn)
                $("#deliveryTermAddModal").modal("hide");
            },
            error: handleAjaxError
        });

        $("#delTermName").val(null);
        $("#delJpnName").val(null);
    }
}

function handleAjaxError(xhr, status, error) {
    console.error('AJAX Error:', status, error);
}