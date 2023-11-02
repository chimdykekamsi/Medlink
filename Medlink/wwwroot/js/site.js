// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
window.addEventListener('load', function () {
    var preloader = document.getElementById('preloader');

    // Add the 'hidden' class to fade out the preloader
    preloader.classList.add('hidden');
});

    $(document).ready(function () {
        $('#searchid').on('input', function () {
            var searchValue = $(this).val().toLowerCase();
            var noResult = true;
            $('.card').each(function () {
                var fullname = $(this).find('h5').text().toLowerCase();
                var username = $(this).find('i').text().toLowerCase();
                var specialty = $(this).find('h6').text().toLowerCase();

                if (fullname.includes(searchValue) || username.includes(searchValue) || specialty.includes(searchValue)) {
                    $(this).show();
                    noResult = false;
                } else {
                    $(this).hide();
                }
            });

            if (noResult) {
                $('#noResults').show();
            } else {
                $('#noResults').hide();
            }
        });
    });


function update(ele){
    if (ele.checked == true) {
        document.querySelector("#visibility").value = "true";
    } else {
        document.querySelector("#visibility").value = "false";
    }
}
