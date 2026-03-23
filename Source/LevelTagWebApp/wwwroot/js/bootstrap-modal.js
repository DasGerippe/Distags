export function showModal(modalSelector) {
    const modalElement = document.querySelector(modalSelector);
    const modal = bootstrap.Modal.getOrCreateInstance(modalElement);
    modal.show();
}

export function hideModal(modalSelector) {
    const modalElement = document.querySelector(modalSelector);
    const modal = bootstrap.Modal.getInstance(modalElement);
    if (modal)
        modal.hide();
}