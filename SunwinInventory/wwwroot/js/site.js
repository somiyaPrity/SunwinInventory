document.addEventListener("DOMContentLoaded", function () {

    const sidebar = document.querySelector(".sidebar");
    const toggle = document.getElementById("menu-toggle");
    const mainWrapper = document.querySelector(".main-wrapper");

    // =========================
    // LOADER
    // =========================
    const loader = document.getElementById("loader");
    if (loader) {
        window.addEventListener("load", function () {
            loader.style.display = "none";
        });
    }

    // =========================
    // SIDEBAR TOGGLE (DESKTOP + MOBILE)
    // =========================
    if (toggle) {
        toggle.addEventListener("click", function () {

            // Mobile
            if (window.innerWidth <= 992) {
                sidebar.classList.toggle("show");
                return;
            }

            // Desktop collapse
            sidebar.classList.toggle("collapsed");

            if (mainWrapper) {
                mainWrapper.classList.toggle("expanded");
            }

        });
    }

    // =========================
    // CLOSE SIDEBAR ON OUTSIDE CLICK (MOBILE)
    // =========================
    document.addEventListener("click", function (e) {

        if (window.innerWidth > 992) return;

        const isClickInsideSidebar = sidebar.contains(e.target);
        const isClickToggle = toggle.contains(e.target);

        if (!isClickInsideSidebar && !isClickToggle) {
            sidebar.classList.remove("show");
        }

    });

});