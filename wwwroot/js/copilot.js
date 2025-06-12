document.addEventListener('DOMContentLoaded', function() {
    const copilotToggle = document.getElementById('copilot-toggle');
    const copilotContainer = document.querySelector('.copilot-container');
    const copilotClose = document.getElementById('copilot-close');
    const copilotMinimize = document.getElementById('copilot-minimize');

    // Toggle copilot visibility
    copilotToggle.addEventListener('click', function() {
        copilotContainer.classList.toggle('open');
        if (copilotContainer.classList.contains('open')) {
            copilotContainer.style.display = 'flex';
        } else {
            copilotContainer.style.display = 'none';
        }
    });

    // Close copilot
    copilotClose.addEventListener('click', function() {
        copilotContainer.classList.remove('open');
        copilotContainer.style.display = 'none';
    });

    // Minimize/maximize copilot
    copilotMinimize.addEventListener('click', function() {
        const iframeContainer = document.querySelector('.copilot-iframe-container');
        iframeContainer.style.display = iframeContainer.style.display === 'none' ? 'block' : 'none';
        
        if (iframeContainer.style.display === 'none') {
            copilotMinimize.innerHTML = '<i class="fas fa-expand"></i>';
            copilotContainer.style.height = '60px';
        } else {
            copilotMinimize.innerHTML = '<i class="fas fa-minus"></i>';
            copilotContainer.style.height = '500px';
        }
    });
}); 