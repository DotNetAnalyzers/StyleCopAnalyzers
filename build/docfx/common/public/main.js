const locales = [
    { lang: "en", label: "English", prefix: "", searchLanguages: ["en"], languageLabel: "Language", anchorLabel: "Anchor" },
    { lang: "ru", label: "Русский", prefix: "ru-ru/", searchLanguages: ["en", "ru"], languageLabel: "Язык", anchorLabel: "Ссылка на раздел" },
    { lang: "cs", label: "Čeština", prefix: "cs-cz/", searchLanguages: ["en", "cs"], languageLabel: "Jazyk", anchorLabel: "Odkaz na oddíl" },
];
const currentLocale = locales.find(locale => locale.lang === document.documentElement.lang);
if (!currentLocale) {
    throw new Error(`No documentation locale configured for "${document.documentElement.lang}".`);
}

export default {
    lunrLanguages: currentLocale.searchLanguages,
    anchors: {
        ariaLabel: currentLocale.anchorLabel,
    },
    start: () => {
        const localizedLabels = {
            ru: {
                "Toggle navigation": "Открыть или скрыть меню",
                "Search": "Поиск",
                "Close": "Закрыть",
                "Show table of contents": "Показать содержание",
                toc: "Содержание",
            },
            cs: {
                "Toggle navigation": "Přepnout navigaci",
                "Search": "Hledat",
                "Close": "Zavřít",
                "Show table of contents": "Zobrazit obsah",
                toc: "Obsah",
            },
        }[currentLocale.lang];
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
        if (localizedLabels) {
            // These labels are not exposed through DocFX's token.json.
            for (const element of document.querySelectorAll("[aria-label]")) {
                const translated = localizedLabels[element.getAttribute("aria-label")];
                if (translated) {
                    element.setAttribute("aria-label", translated);
                }
            }

            document.getElementById("tocOffcanvasLabel")?.replaceChildren(localizedLabels.toc);
        }
    },
};
