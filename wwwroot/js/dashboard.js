window.initDashboardCharts = () => {
    console.log("Inicializando gráficos del dashboard...");

    const ctxVentas = document.getElementById("chartVentas");
    const ctxServicios = document.getElementById("chartServicios");

    if (ctxVentas) {
        new Chart(ctxVentas, {
            type: 'bar',
            data: {
                labels: ['Enero', 'Febrero', 'Marzo', 'Abril'],
                datasets: [{
                    label: 'Ventas',
                    data: [10, 20, 15, 30]
                }]
            }
        });
    }

    if (ctxServicios) {
        new Chart(ctxServicios, {
            type: 'pie',
            data: {
                labels: ['Corte', 'Tinte', 'Manicure'],
                datasets: [{
                    data: [50, 25, 25]
                }]
            }
        });
    }
};
