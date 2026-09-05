// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Türkiye telefon formatı: yazarken "0 533 598 32 12" şeklinde otomatik boşluk ekler.
(function () {
    function formatTrPhone(value) {
        var digits = value.replace(/\D/g, "").slice(0, 11);
        var parts = [];
        if (digits.length > 0) parts.push(digits.slice(0, 1));
        if (digits.length > 1) parts.push(digits.slice(1, 4));
        if (digits.length > 4) parts.push(digits.slice(4, 7));
        if (digits.length > 7) parts.push(digits.slice(7, 9));
        if (digits.length > 9) parts.push(digits.slice(9, 11));
        return parts.join(" ");
    }

    document.addEventListener("input", function (e) {
        if (e.target && e.target.matches("[data-tr-phone]")) {
            var cursorAtEnd = e.target.selectionEnd === e.target.value.length;
            e.target.value = formatTrPhone(e.target.value);
            if (cursorAtEnd) {
                e.target.setSelectionRange(e.target.value.length, e.target.value.length);
            }
        }
    });
})();
