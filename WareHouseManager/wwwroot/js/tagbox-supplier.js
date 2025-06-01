// Tagbox logic for supplier products
(function() {
    const tagbox = document.getElementById('supplierProductsTagbox');
    const select = document.getElementById('supplierProductSelect');
    const hidden = document.getElementById('products');
    let tags = [];

    function renderTags() {
        // Remove all tags except the select
        tagbox.querySelectorAll('.tag').forEach(tag => tag.remove());
        tags.forEach((tag, idx) => {
            const tagEl = document.createElement('span');
            tagEl.className = 'tag';
            tagEl.textContent = tag;
            const remove = document.createElement('span');
            remove.className = 'remove-tag';
            remove.textContent = '×';
            remove.title = 'Remove';
            remove.onclick = function() {
                tags.splice(idx, 1);
                renderTags();
            };
            tagEl.appendChild(remove);
            tagbox.insertBefore(tagEl, select);
        });
        // Always update hidden input, even if tags is empty
        hidden.value = tags.join(',');
    }

    // Listen for select changes
    select.addEventListener('change', function() {
        const val = select.value;
        if (val && !tags.includes(val)) {
            tags.push(val);
            renderTags();
        }
        select.value = '';
    });

    // If editing, prepopulate from hidden input
    if (hidden.value) {
        tags = hidden.value.split(',').map(t => t.trim()).filter(Boolean);
        renderTags();
    } else {
        // Ensure hidden input is empty if no tags
        hidden.value = '';
    }
})();
