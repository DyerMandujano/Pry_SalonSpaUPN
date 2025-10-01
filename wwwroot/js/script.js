// script.js

window.initDashboardCharts = () => {
    // Ejemplo usando Chart.js para generar gráficos
    const ctxVentas = document.getElementById("chartVentas");
    if (ctxVentas) {
        new Chart(ctxVentas, {
            type: "bar",
            data: {
                labels: ["Enero", "Febrero", "Marzo", "Abril", "Mayo"],
                datasets: [{
                    label: "Ventas",
                    data: [1200, 1500, 1000, 1800, 2000],
                    borderWidth: 1
                }]
            },
            options: {
                responsive: true,
                scales: {
                    y: { beginAtZero: true }
                }
            }
        });
    }

    const ctxServicios = document.getElementById("chartServicios");
    if (ctxServicios) {
        new Chart(ctxServicios, {
            type: "doughnut",
            data: {
                labels: ["Corte", "Coloración", "Peinado", "Tratamiento"],
                datasets: [{
                    label: "Servicios",
                    data: [40, 25, 20, 15]
                }]
            },
            options: {
                responsive: true
            }
        });
    }
};
