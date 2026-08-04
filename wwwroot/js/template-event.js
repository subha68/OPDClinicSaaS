$(document).on(
    "click",
    ".toggle-section, .insert-before, .insert-after, .toggle-field, .insert-field-before, .insert-field-after",
    function (e) {
        e.stopPropagation();
    }
);
