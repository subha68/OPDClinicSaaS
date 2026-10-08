document.addEventListener("DOMContentLoaded", function () {

    const paymentForm = document.getElementById("paymentForm");

    if (!paymentForm)
        return;

    const balance = parseFloat(paymentForm.dataset.balance || "0");
    const paymentType = paymentForm.dataset.paymentType;
    const amountBox = document.getElementById("paymentAmount");
    const fullPayment = document.getElementById("fullPayment");
    const partialPayment = document.getElementById("partialPayment");

    if (!amountBox || !fullPayment || !partialPayment)
        return;
    
    function setFullPayment() {

        amountBox.value = balance.toFixed(2);
        amountBox.readOnly = true;

    }


    function setPartialPayment() {

        amountBox.readOnly = false;

        amountBox.focus();
        amountBox.select();

    }


    fullPayment.addEventListener(
        "change",
        function () {

            if (this.checked)
                setFullPayment();

        });


    partialPayment.addEventListener(
        "change",
        function () {

            if (this.checked)
                setPartialPayment();

        });


    // Default
    setFullPayment();

});