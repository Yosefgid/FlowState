const chartInstances = {};

window.renderWeeklyChart = (canvasId, counts) => {
    // Destroy existing instance if it exists
    if (chartInstances[canvasId]) {
        chartInstances[canvasId].destroy();
        delete chartInstances[canvasId];
    }

    const ctx = document.getElementById(canvasId);
    if (!ctx) return;

    chartInstances[canvasId] = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun'],
            datasets: [{
                label: 'Completed',
                data: counts,
                backgroundColor: '#5DCAA5',
                borderColor: '#1D9E75',
                borderWidth: 1,
                borderRadius: 6,
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: { legend: { display: false } },
            scales: {
                y: { beginAtZero: true, ticks: { stepSize: 1 } }
            }
        }
    });
};