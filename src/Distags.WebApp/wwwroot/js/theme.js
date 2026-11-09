// Color mode handling (light / dark / auto).
// The initial theme is applied by the inline script in App.razor before the first paint;
// this module is used by the ThemeSwitcher component to read and change it at runtime.

const storageKey = "distags-theme";
const modes = ["light", "dark", "auto"];
const darkQuery = window.matchMedia("(prefers-color-scheme: dark)");

export function getStoredTheme() {
    try {
        const mode = localStorage.getItem(storageKey);
        return modes.includes(mode) ? mode : "auto";
    } catch {
        return "auto";
    }
}

export function setTheme(mode) {
    if (!modes.includes(mode))
        mode = "auto";

    try {
        localStorage.setItem(storageKey, mode);
    } catch {
        // Storage unavailable (e.g. blocked site data): the choice lasts for this page only.
    }

    applyTheme(mode);
}

function applyTheme(mode) {
    const resolved = mode === "auto" ? (darkQuery.matches ? "dark" : "light") : mode;
    document.documentElement.setAttribute("data-bs-theme", resolved);
}

darkQuery.addEventListener("change", () => {
    if (getStoredTheme() === "auto")
        applyTheme("auto");
});
