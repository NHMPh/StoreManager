function showTab(tabId) {
    document.querySelectorAll('.tab-content').forEach(tab => tab.style.display = 'none');
    document.getElementById(tabId).style.display = 'block';

    document.querySelectorAll('.sidebar .tab').forEach(tab => tab.classList.remove('active'));
    document.querySelector(`.sidebar .tab[onclick="showTab('${tabId}')"]`).classList.add('active');
}

function showAddProductPopup() {
    console.log('showAddProductPopup called');
    showBlurredBackground();
    document.getElementById('add-product-popup').style.display = 'block';
}
function showBlurredBackground() {
    document.getElementById('popup-overlay').style.display = 'block';
}
function hideBlurredBackground() {
    document.getElementById('popup-overlay').style.display = 'none';
}
function hideAddProductPopup() {
    console.log('hideAddProductPopup called');
    document.getElementById('add-product-popup').style.display = 'none';
    hideBlurredBackground();
}

function searchProducts() {
    const searchInput = document.getElementById('productSearch').value.toLowerCase();
    const productCards = document.querySelectorAll('.flash-card');
    productCards.forEach(card => {
        const productName = card.querySelector('.product-details h4').textContent.toLowerCase();
        card.style.display = productName.includes(searchInput) ? 'flex' : 'none';
    });
}

function searchSuppliers() {
    const searchInput = document.getElementById('supplierSearch').value.toLowerCase();
    const supplierCards = document.querySelectorAll('#supplierContainer .flash-card');
    supplierCards.forEach(card => {
        const supplierName = card.querySelector('.supplier-details h4').textContent.toLowerCase();
        card.style.display = supplierName.includes(searchInput) ? 'flex' : 'none';
    });
}

function editProduct(productId) {
    const productCard = document.querySelector(`.flash-card[data-product-id="${productId}"]`);
    if (productCard) {
        const productName = productCard.querySelector('.product-details h4').textContent;
        const unit = productCard.querySelector('.product-details p:nth-child(2)').textContent.split(': ')[1];
        const costPerUnit = productCard.querySelector('.product-details p:nth-child(3)').textContent.split(': ')[1];
        const stock = productCard.querySelector('.product-details p:nth-child(4)').textContent.split(': ')[1];

        document.getElementById('productId').value = productId;
        document.getElementById('productName').value = productName;
        document.getElementById('unit').value = unit;
        document.getElementById('costPerUnit').value = parseFloat(costPerUnit.replace('$', ''));
        document.getElementById('stock').value = parseInt(stock);

        showAddProductPopup();
    } else {
        alert(`Product with ID: ${productId} not found.`);
    }
}

function deleteProduct(productId) {
    if (confirm(`Are you sure you want to delete product with ID: ${productId}?`)) {
        alert(`Product with ID: ${productId} deleted.`);
        // Add logic to delete the product
    }
}

function editSupplier(supplierId) {
    alert(`Edit supplier with ID: ${supplierId}`);
    // Add logic to edit the supplier
}

function deleteSupplier(supplierId) {
    if (confirm(`Are you sure you want to delete supplier with ID: ${supplierId}?`)) {
        alert(`Supplier with ID: ${supplierId} deleted.`);
        // Add logic to delete the supplier
    }
}

function showAddSupplierPopup() {
    document.getElementById('add-supplier-popup').style.display = 'block';
    let overlay = document.getElementById('popup-overlay');
    if (!overlay) {
        overlay = document.createElement('div');
        overlay.id = 'popup-overlay';
        overlay.className = 'popup-overlay';
        document.body.appendChild(overlay);
    }
    overlay.style.display = 'block'; // Show overlay
}

function hideAddSupplierPopup() {
    document.getElementById('add-supplier-popup').style.display = 'none';
    removePopupOverlay(); // Remove overlay
}

function showAddCustomerPopup() {
    document.getElementById('add-customer-popup').style.display = 'block';
    let overlay = document.getElementById('popup-overlay');
    if (!overlay) {
        overlay = document.createElement('div');
        overlay.id = 'popup-overlay';
        overlay.className = 'popup-overlay';
        document.body.appendChild(overlay);
    }
    overlay.style.display = 'block'; // Show overlay
}

function hideAddCustomerPopup() {
    removePopupOverlay(); // Remove overlay
    document.getElementById('add-customer-popup').style.display = 'none';
}

function searchCustomers() {
    const searchInput = document.getElementById('customerSearch').value.toLowerCase();
    const customerCards = document.querySelectorAll('#customerContainer .flash-card');
    customerCards.forEach(card => {
        const customerName = card.querySelector('.customer-details h4').textContent.toLowerCase();
        card.style.display = customerName.includes(searchInput) ? 'flex' : 'none';
    });
}

function editCustomer(customerId) {
    alert(`Edit customer with ID: ${customerId}`);
    // Add logic to edit the customer
}

function deleteCustomer(customerId) {
    if (confirm(`Are you sure you want to delete customer with ID: ${customerId}?`)) {
        alert(`Customer with ID: ${customerId} deleted.`);
        // Add logic to delete the customer
    }
}

function updateProductInfo() {
    const productSelect = document.getElementById('productSelect');
    const selectedOption = productSelect.options[productSelect.selectedIndex];
    const stock = selectedOption.getAttribute('data-stock') || 'N/A';
    const price = selectedOption.getAttribute('data-price') || 'N/A';

    document.getElementById('productStock').textContent = stock;
    document.getElementById('productPrice').textContent = price;
    updateTotalAmount();
}

function updateTotalAmount() {
    const quantity = parseInt(document.getElementById('productQuantity').value) || 0;
    const price = parseFloat(document.getElementById('productPrice').textContent) || 0;
    const totalAmount = quantity * price;

    document.getElementById('totalAmount').value = totalAmount.toFixed(2);
    document.getElementById('totalAmount').textContent = totalAmount.toFixed(2);
}

function addPendingOrder() {
    const supplierId = document.getElementById('supplierSelect').value;
    const supplierSelect = document.getElementById('supplierSelect');
    const supplierName = supplierSelect.options[supplierSelect.selectedIndex].text;
    const productId = document.getElementById('productSelect').value;
    const productSelect = document.getElementById('productSelect');
    const productName = productSelect.options[productSelect.selectedIndex].text;
    const quantity = parseInt(document.getElementById('productQuantity').value) || 0;
    const total = parseFloat(document.getElementById('totalAmount').value) || 0;

    if (!supplierId || !productId || quantity <= 0) {
        alert('Please fill in all fields.');
        return;
    }

    const table = document.getElementById('pendingOrdersTable');
    const row = table.insertRow();
    // Add visible cells
    row.innerHTML = `
        <td>${supplierName}</td>
        <td>${productId}</td>
        <td>${quantity}</td>
        <td>$${total.toFixed(2)}</td>
        <td><button class="delete-button" onclick="removePendingOrder(this)">Remove</button></td>
        <td style="display:none;">${supplierId}</td>
    `;

    updateTotalPayable();
}

function removePendingOrder(button) {
    const row = button.parentElement.parentElement;
    row.remove();
    updateTotalPayable();
}

function updateTotalPayable() {
    const table = document.getElementById('pendingOrdersTable');
    let totalPayable = 0;
    for (const row of table.rows) {
        const totalCell = row.cells[3];
        if (totalCell && totalCell.textContent) {
            totalPayable += parseFloat(totalCell.textContent.replace('$', '')) || 0;
        }
    }
    document.getElementById('totalPayable').value = `$${totalPayable.toFixed(2)}`;
    updateRemainingBalance();
}

function updateRemainingBalance() {
    const totalPayable = parseFloat((document.getElementById('totalPayable').value || '').replace('$', '')) || 0;
    const amountPaid = parseFloat(document.getElementById('amountPaid').value) || 0;
    const remainingBalance = totalPayable - amountPaid;
    document.getElementById('remainingBalance').textContent = remainingBalance.toFixed(2);
}

// function processPayment() {
//     const remainingBalance = parseFloat(document.getElementById('remainingBalance').textContent) || 0;

//     if (remainingBalance > 0) {
//         alert('Please pay the full amount.');
//         return;
//     }

//     alert('Payment successful!');
//     document.getElementById('pendingOrdersTable').innerHTML = '';
//     document.getElementById('totalPayable').value = '$0.00';
//     document.getElementById('amountPaid').value = '';
//     document.getElementById('remainingBalance').textContent = '0.00';
// }

function updateProductInfoOut() {
    const productSelect = document.getElementById('productSelectOut');
    const selectedOption = productSelect.options[productSelect.selectedIndex];
    const stock = selectedOption.getAttribute('data-stock') || 'N/A';
    const price = selectedOption.getAttribute('data-price') || 'N/A';

    document.getElementById('productStockOut').textContent = stock;
    document.getElementById('productPriceOut').textContent = price;
    updateTotalAmountOut();
}

function updateTotalAmountOut() {
    const quantity = parseInt(document.getElementById('productQuantityOut').value) || 0;
    const price = parseFloat(document.getElementById('productPriceOut').textContent) || 0;
    const totalAmount = quantity * price;
    // Set both value and textContent for compatibility with input and span
    const totalAmountOut = document.getElementById('totalAmountOut');
    if (totalAmountOut.tagName.toLowerCase() === 'input') {
        totalAmountOut.value = totalAmount.toFixed(2);
    } else {
        totalAmountOut.textContent = totalAmount.toFixed(2);
    }
}

function addPendingOrderOut() {
    const customerSelect = document.getElementById('customerSelect');
    const customerId = customerSelect.selectedIndex > 0 ? customerSelect.options[customerSelect.selectedIndex].getAttribute('data-customer-id') : null;
    const customerName = customerSelect.options[customerSelect.selectedIndex].text;
    const productSelect = document.getElementById('productSelectOut');
    const selectedProductOption = productSelect.options[productSelect.selectedIndex];
    const productId = selectedProductOption.getAttribute('data-product-id');
    const productName = productSelect.value;
    const quantity = parseInt(document.getElementById('productQuantityOut').value);
    const unitPrice = parseFloat(document.getElementById('productPriceOut').textContent);
    const stock = parseInt(document.getElementById('productStockOut').textContent);

    // Check if stock is less than quantity
    if (!isNaN(stock) && quantity > stock) {
        alert('Not enough stock for this product!');
        return;
    }

    const total = (!isNaN(unitPrice) && !isNaN(quantity)) ? unitPrice * quantity : 0;
    if (!customerId || !productId || !quantity || isNaN(unitPrice)) {
        alert('Please select customer, product, and enter quantity.');
        return;
    }
    const table = document.getElementById('pendingOrdersTableOut');
    const row = table.insertRow();
    // Add visible and hidden cells using innerHTML (like your example)
    row.innerHTML = `
            <td>${customerName}</td>
            <td>${productName}</td>
            <td>${quantity}</td>
            <td>$${total.toFixed(2)}</td>
            <td><button class="btn btn-danger btn-sm remove-pending-order-out">Remove</button></td>
            <td style="display:none;">${customerId}</td>
        `;
    // Attach event for remove button to ensure it works after using innerHTML
    row.querySelector('.remove-pending-order-out').onclick = function() {
        row.remove();
        updateTotalPayableOut();
    };
    updateTotalPayableOut();
}

function removePendingOrderOut(button) {
    const row = button.parentElement.parentElement;
    row.remove();
    updateTotalPayableOut();
}

function updateTotalPayableOut() {
    const table = document.getElementById('pendingOrdersTableOut');
    let totalPayable = 0;

    for (const row of table.rows) {
        const totalCell = row.cells[3];
        const total = parseFloat(totalCell.textContent.replace('$', '')) || 0;
        totalPayable += total;
    }

    document.getElementById('totalPayableOut').value = `$${totalPayable.toFixed(2)}`;
    updateRemainingBalanceOut();
}

function updateRemainingBalanceOut() {
    const totalPayable = parseFloat(document.getElementById('totalPayableOut').value.replace('$', '')) || 0;
    const amountPaid = parseFloat(document.getElementById('amountPaidOut').value) || 0;
    const remainingBalance = totalPayable - amountPaid;

    document.getElementById('remainingBalanceOut').textContent = remainingBalance.toFixed(2);
}

 function getProductById(id) {
        if (!window.productsList) return null;
        return window.productsList.find(p => String(p.productId ?? p.id) === String(id));
    }
function processPaymentOut() {
        // Build details from pending orders table
        const table = document.getElementById('pendingOrdersTableOut');
        const rows = Array.from(table.rows);
        if (rows.length === 0) {
            alert('Please add at least one product to pending orders.');
            return;
        }
        // Group details by customerId (assume customerId is in a hidden cell, e.g., cell[5])
        let customerGroups = {};
        for (const row of rows) {
            if (row.cells.length < 6) continue; // skip malformed rows
            const customerId = row.cells[5] ? row.cells[5].textContent : null;
            if (!customerId) continue;
            const productName = row.cells[1] ? row.cells[1].textContent : null;
            let productObj = null;
            let productId = null;
            if (productName && window.productsList) {
                productObj = window.productsList.find(p => p.productName === productName);
                if (productObj) productId = productObj.productId || productObj.ProductId || productObj.id;
            }
            // Ensure productId is always a valid integer
            if (!productId && productObj && (productObj.ProductId || productObj.productId || productObj.id)) {
                productId = productObj.ProductId || productObj.productId || productObj.id;
            }
            productId = parseInt(productId);
            if (isNaN(productId)) productId = null;
            const quantity = row.cells[2] ? parseInt(row.cells[2].textContent) : 0;
            let unitPrice = 0;
            if (!isNaN(quantity) && quantity > 0 && row.cells[3]) {
                const total = parseFloat(row.cells[3].textContent.replace('$',''));
                unitPrice = !isNaN(total) ? total / quantity : 0;
            }
            if (!customerGroups[customerId]) {
                customerGroups[customerId] = {
                    details: []
                };
            }
            customerGroups[customerId].details.push({
                productId: productId, // will be int or null
                product: getProductById(productId), // always send null to avoid circular reference
                quantity: quantity,
                unitPrice: unitPrice
            });
        }
        const customerIds = Object.keys(customerGroups);
        if (customerIds.length === 0) {
            alert('No valid customers found in pending orders.');
            return;
        }
        // For each customer, create a form, set JSON, and submit sequentially
        function submitNext(index) {
            if (index >= customerIds.length) {
                // Clear the table after all submissions
                document.getElementById('pendingOrdersTableOut').innerHTML = '';
                updateTotalPayableOut();
                return;
            }
            const customerId = customerIds[index];
            const group = customerGroups[customerId];
            const transactionOut = {
                customerId: parseInt(customerId),
                transactionDate: new Date().toISOString(),
                details: group.details
            };
            // Create a hidden form dynamically
            const form = document.createElement('form');
            form.method = 'post';
            form.action = document.getElementById('addTransactionOutForm').action;
            form.style.display = 'none';
            // CSRF token
            const csrfInput = document.querySelector('#addTransactionOutForm input[name=__RequestVerificationToken]');
            if (csrfInput) {
                const tokenInput = document.createElement('input');
                tokenInput.type = 'hidden';
                tokenInput.name = '__RequestVerificationToken';
                tokenInput.value = csrfInput.value;
                form.appendChild(tokenInput);
            }
            // JSON input
            const jsonInput = document.createElement('input');
            jsonInput.type = 'hidden';
            jsonInput.name = 'transactionOutJson';
            jsonInput.value = JSON.stringify(transactionOut);
            console.log('Submitting transactionOut:', transactionOut);
            form.appendChild(jsonInput);
            document.body.appendChild(form);
            form.submit();
            setTimeout(() => {
                document.body.removeChild(form);
                submitNext(index + 1);
            }, 500);
        }
        submitNext(0);
    }

function createPopupOverlay() {
    let overlay = document.getElementsByClassName('popup-overlay');
    console.log(overlay.length);
    console.log(overlay);
    if (!overlay) {
        overlay = document.createElement('div');
        overlay.className = 'popup-overlay';
        overlay.id = 'popup-overlay';
        document.body.appendChild(overlay);
    }
}

function removePopupOverlay() {
    const overlay = document.getElementById('popup-overlay');
    if (overlay) {
        overlay.remove();
    }
}

function searchTransactions() {
    const searchInput = document.getElementById('transactionSearch').value.toLowerCase();

    // Filter Transaction In table
    const transactionInRows = document.querySelectorAll('#transactionInTable tr');
    transactionInRows.forEach(row => {
        const transactionId = row.querySelector('td:first-child').textContent.toLowerCase();
        row.style.display = transactionId.includes(searchInput) ? '' : 'none';
    });

    // Filter Transaction Out table
    const transactionOutRows = document.querySelectorAll('#transactionOutTable tr');
    transactionOutRows.forEach(row => {
        const transactionId = row.querySelector('td:first-child').textContent.toLowerCase();
        row.style.display = transactionId.includes(searchInput) ? '' : 'none';
    });
}

function showAddWarehouseManagerPopup() {
    document.getElementById('add-warehouse-manager-popup').style.display = 'block';
    let overlay = document.getElementById('popup-overlay');
    if (!overlay) {
        overlay = document.createElement('div');
        overlay.id = 'popup-overlay';
        overlay.className = 'popup-overlay';
        document.body.appendChild(overlay);
    }
    overlay.style.display = 'block'; // Show overlay
}

function hideAddWarehouseManagerPopup() {
    document.getElementById('add-warehouse-manager-popup').style.display = 'none';
    removePopupOverlay(); // Remove overlay
}

function searchWarehouseManagers() {
    const searchInput = document.getElementById('warehouseManagerSearch').value.toLowerCase();
    const managerCards = document.querySelectorAll('#warehouseManagerContainer .flash-card');
    managerCards.forEach(card => {
        const managerName = card.querySelector('.manager-details h4').textContent.toLowerCase();
        card.style.display = managerName.includes(searchInput) ? 'flex' : 'none';
    });
}

function editWarehouseManager(managerId) {
    alert(`Edit warehouse manager with ID: ${managerId}`);
    // Add logic to edit the warehouse manager
}

function deleteWarehouseManager(managerId) {
    if (confirm(`Are you sure you want to delete warehouse manager with ID: ${managerId}?`)) {
        alert(`Warehouse manager with ID: ${managerId} deleted.`);
        // Add logic to delete the warehouse manager
    }
}

function showAddSalesManagerPopup() {
    document.getElementById('add-sales-manager-popup').style.display = 'block';
    let overlay = document.getElementById('popup-overlay');
    if (!overlay) {
        overlay = document.createElement('div');
        overlay.id = 'popup-overlay';
        overlay.className = 'popup-overlay';
        document.body.appendChild(overlay);
    }
    overlay.style.display = 'block'; // Show overlay
}

function hideAddSalesManagerPopup() {
    document.getElementById('add-sales-manager-popup').style.display = 'none';
    removePopupOverlay(); // Remove overlay
}

function searchSalesManagers() {
    const searchInput = document.getElementById('salesManagerSearch').value.toLowerCase();
    const managerCards = document.querySelectorAll('#salesManagerContainer .flash-card');
    managerCards.forEach(card => {
        const managerName = card.querySelector('.manager-details h4').textContent.toLowerCase();
        card.style.display = managerName.includes(searchInput) ? 'flex' : 'none';
    });
}

function editSalesManager(managerId) {
    alert(`Edit sales manager with ID: ${managerId}`);
    // Add logic to edit the sales manager
}

function deleteSalesManager(managerId) {
    if (confirm(`Are you sure you want to delete sales manager with ID: ${managerId}?`)) {
        alert(`Sales manager with ID: ${managerId} deleted.`);
        // Add logic to delete the sales manager
    }
}

function ensureButtonsVisible() {
    const buttons = document.querySelectorAll('.flash-card .edit-button, .flash-card .delete-button');
    buttons.forEach(button => {
        button.style.display = 'inline-block'; // Ensure buttons are displayed
        button.style.visibility = 'visible'; // Ensure buttons are visible
        button.style.opacity = '1'; // Fully opaque
    });
}

// Call this function after dynamically loading or updating flashcards
ensureButtonsVisible();
