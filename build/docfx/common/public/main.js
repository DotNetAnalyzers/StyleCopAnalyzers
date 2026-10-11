const locales = [
    { lang: "en", label: "English", prefix: "", searchLanguages: ["en"], languageLabel: "Language" },
    { lang: "ru", label: "Русский", prefix: "ru-ru/", searchLanguages: ["en", "ru"], languageLabel: "Язык" },
];
const currentLocale = locales.find(locale => locale.lang === document.documentElement.lang);
if (!currentLocale) {
    throw new Error(`No documentation locale configured for "${document.documentElement.lang}".`);
}

export default {
    lunrLanguages: currentLocale.searchLanguages,
    anchors: {
        ariaLabel: document.documentElement.lang === "ru" ? "Ссылка на раздел" : "Anchor",
    },
    start: () => {
        const russian = document.documentElement.lang === "ru";
        const root = new URL(document.querySelector('meta[name="docfx:rel"]').content || "./", window.location.href);
        const siteRoot = new URL("../".repeat(currentLocale.prefix.split("/").filter(Boolean).length) || "./", root);
        const page = window.location.pathname.slice(root.pathname.length) || "index.html";
        const languages = document.createElement("nav");
        languages.className = "language-picker dropdown ms-md-3 flex-shrink-0";
        languages.setAttribute("aria-label", currentLocale.languageLabel);
        const toggle = document.createElement("button");
        toggle.type = "button";
        toggle.id = "language-picker-toggle";
        toggle.className = "btn border-0 dropdown-toggle";
        toggle.setAttribute("data-bs-toggle", "dropdown");
        toggle.setAttribute("aria-expanded", "false");
        toggle.setAttribute("aria-controls", "language-picker-menu");
        toggle.setAttribute("aria-label", `${currentLocale.languageLabel}: ${currentLocale.label}`);
        toggle.textContent = currentLocale.label;
        const menu = document.createElement("ul");
        menu.id = "language-picker-menu";
        menu.className = "dropdown-menu dropdown-menu-end";
        menu.setAttribute("aria-labelledby", toggle.id);
        for (const { lang, label, prefix } of locales) {
            const item = document.createElement("li");
            const link = document.createElement("a");
            link.className = lang === currentLocale.lang ? "dropdown-item active" : "dropdown-item";
            link.href = new URL(prefix + page, siteRoot).href;
            link.lang = lang;
            link.hreflang = lang;
            link.textContent = label;
            if (lang === document.documentElement.lang) {
                link.setAttribute("aria-current", "true");
            }

            item.append(link);
            menu.append(item);
        }

        languages.append(toggle);
        languages.append(menu);
        document.getElementById("navpanel").append(languages);
        if (russian) {
            // These labels are not exposed through DocFX's token.json.
            const labels = {
                "Toggle navigation": "Открыть или скрыть меню",
                "Search": "Поиск",
                "Close": "Закрыть",
                "Show table of contents": "Показать содержание",
            };
            for (const element of document.querySelectorAll("[aria-label]")) {
                const translated = labels[element.getAttribute("aria-label")];
                if (translated) {
                    element.setAttribute("aria-label", translated);
                }
            }

            document.getElementById("tocOffcanvasLabel")?.replaceChildren("Содержание");
        }
    },
};
