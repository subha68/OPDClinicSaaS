function setSelectedFileName(input, targetId) {
    if (input.files && input.files.length > 0) {
        document.getElementById(targetId).value =
            input.files[0].name;
    }
}