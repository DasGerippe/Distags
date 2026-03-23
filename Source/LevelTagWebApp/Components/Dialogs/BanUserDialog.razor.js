export function setUpModal(modalId, dotNetReference) {
    const modal = document.getElementById(modalId);
    if (!modal)
        return;

    const modalShowHandler = async function (event) {
        const button = event.relatedTarget;
        if (!button)
            return;

        const steamId = button.getAttribute("data-steamid");
        const steamName = button.getAttribute("data-steamname");
        await setUserValues(steamId, steamName, dotNetReference);
    };

    const modalHideHandler = async function (event) {
        await setUserValues(null, null, dotNetReference);
    }

    modal._showHandler = modalShowHandler;
    modal._hideHandler = modalHideHandler;
    modal.addEventListener("show.bs.modal", modalShowHandler);
    modal.addEventListener("hide.bs.modal", modalHideHandler);
}

export function cleanUpModal(modalId) {
    const modal = document.getElementById(modalId);
    if (!modal)
        return;

    if (modal._showHandler) {
        modal.removeEventListener("show.bs.modal", modal._showHandler);
        delete modal._showHandler;
    }

    if (modal._hideHandler) {
        modal.removeEventListener("hide.bs.modal", modal._hideHandler);
        delete modal._hideHandler;
    }
}

async function setUserValues(steamId, steamName, dotNetReference) {
    document.getElementById("banUserSteamIdInput").value = steamId;
    document.getElementById("banUserSteamNameInput").value = steamName;
    await dotNetReference.invokeMethodAsync("SetUserInputValues", steamId, steamName);
}