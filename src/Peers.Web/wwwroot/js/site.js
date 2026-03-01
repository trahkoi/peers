(() => {
  const forms = document.querySelectorAll('.js-busy-form');

  forms.forEach((form) => {
    form.addEventListener('submit', (event) => {
      const submitter = event.submitter;
      if (!(submitter instanceof HTMLButtonElement)) {
        return;
      }

      const confirmMessage = submitter.dataset.confirm;
      if (confirmMessage && !window.confirm(confirmMessage)) {
        event.preventDefault();
        return;
      }

      const busyText = submitter.dataset.busyText;
      if (busyText) {
        submitter.dataset.originalText = submitter.textContent ?? '';
        submitter.textContent = busyText;
      }

      submitter.disabled = true;
    });
  });
})();
