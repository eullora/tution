function getAnalysis(id) {
    
    var url = `/api/${id}/analysis`;
    $.ajax({
        url: url,
        type: 'GET',
        dataType: 'json',
        success: function (response) {
            printAnalysisGraph(response.resultGroup);
            printExamChart(response.examResults);
            generateAttenceChart(response.attedances);
        }
    });
}

function printAnalysisGraph(result) {
    const labels = result.map(item => item.subjectName);
    const marks = result.map(item => item.marks);
    const ctx = document.getElementById('barChart').getContext('2d');
    const myBarChart = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: labels,
            datasets: [{
                label: 'Marks',
                data: marks,
                backgroundColor: ['rgba(0, 128, 255)'],
                borderColor: ['rgba(0, 128, 255)'],
                borderWidth: 1
            }]
        },
        options: {
            plugins: {
                title: {
                    display: true,
                    text: 'Subject Group'                    
                },
                legend: {
                    display: false
                }
            },
            scales: {                
                y: {
                    beginAtZero: true
                }

            }
        }
    });
}
function printExamChart(result) {
    const labels = result.map(item => item.name);
    const marks = result.map(item => item.marks);
    const ctx = document.getElementById('horizontalBarChart').getContext('2d');
    const myBarChart = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: labels,
            datasets: [{
                label: 'Marks',
                data: marks,
                backgroundColor: ['rgba(255, 0, 127)'],
                borderColor: ['rgba(255, 0, 127)'],
                borderWidth: 1
            }]
        },
        options: {
            plugins: {
                title: {
                    display: true,
                    text: 'Exam Marks',
                },
                legend: {
                    display: false
                }
            },
            indexAxis: 'y',
            scales: {
                x: {
                    beginAtZero: true
                }
            }
        }

    });
}
function getExamGrouping(resultId) {

}
function generateAttenceChart(result) {
    console.log(JSON.stringify(result));    
    const labels = result.map(item => item.month);
    const values = result.map(item => item.daysPresent);
    const backgroundColors = [
        '#FF6384', '#36A2EB', '#FFCE56', '#4BC0C0',
        '#9966FF', '#FF9F40', '#E6E6FA', '#FFD700',
        '#87CEEB', '#32CD32', '#FF4500', '#800080'
    ];

    console.log(labels, values);
    const ctx = document.getElementById('attendanceChart').getContext('2d');
    const myPieChart = new Chart(ctx, {
        type: 'pie', 
        data: {
            labels: labels, 
            datasets: [{
                data: values, 
                backgroundColor: backgroundColors,
                hoverBackgroundColor: backgroundColors
            }]
        },
        options: {
            responsive: true,
            plugins: {
                legend: {
                    position: 'right' 
                },
                title: {
                    display: true,
                    text: 'Attendance',
                },
                datalabels: {
                    color: '#ffffff', // Label color
                    font: {
                        weight: 'bold',
                        size: 14
                    },
                    formatter: (value, context) => {
                        return `${value}`; 
                    }
                }

            }
        }, plugins: [ChartDataLabels] 
    });
}
