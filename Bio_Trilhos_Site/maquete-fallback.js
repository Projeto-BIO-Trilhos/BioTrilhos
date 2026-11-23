document.addEventListener('DOMContentLoaded', () => {
  const nav = document.querySelector('[data-section="maquete"]');
  const canvas = document.getElementById('threeCanvas');
  if (!nav || !canvas) return;

  const checkCanvas = () => {
    const panel = canvas.parentElement;
    const status = panel.querySelector('.maquete-load-status');
    if (status) status.remove();
    try {
      const rect = canvas.getBoundingClientRect();
      if (rect.width < 2 || rect.height < 2) {
        const message = document.createElement('div');
        message.className = 'maquete-load-status';
        message.textContent = 'Preparando a maquete 3D...';
        panel.appendChild(message);
      }
    } catch (_) {}
  };

  nav.addEventListener('click', () => setTimeout(checkCanvas, 100));
});