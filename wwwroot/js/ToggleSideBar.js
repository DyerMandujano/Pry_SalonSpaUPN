document.addEventListener("DOMContentLoaded", function () {
    const toggleBtn = document.getElementById("toggleBtn");
    const sidebar = document.getElementById("sidebar");
    const content = document.getElementById("mainContent");

    toggleBtn.addEventListener("click", () => {
        sidebar.classList.toggle("collapsed"); 
        content.classList.toggle("expanded"); 
    });
});
