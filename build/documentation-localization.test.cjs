const assert = require("node:assert/strict");
const { spawnSync } = require("node:child_process");
const fs = require("node:fs");
const path = require("node:path");
const test = require("node:test");
const vm = require("node:vm");

const root = path.join(__dirname, "..");
const read = (...parts) => fs.readFileSync(path.join(root, ...parts), "utf8");
const pages = fs.readdirSync(path.join(root, "documentation")).filter(file => file.endsWith(".md")).sort();

test("the generated status inventory contains each diagnostic ID once", () => {
    const report = JSON.parse(read("docs", "StyleCop.Analyzers.Status.json"));
    const ids = report.diagnostics.map(diagnostic => diagnostic.Id);
    assert.equal(new Set(ids).size, ids.length);
});

for (const [locale, characters] of [["ru-ru", /[А-Яа-яЁё]/], ["zh-cn", /[\u3400-\u9fff]/]]) {
    test(`every English page has a ${locale} translation with the same code examples`, () => {
        const translatedPages = fs.readdirSync(path.join(root, "documentation", locale))
            .filter(file => file.endsWith(".md")).sort();
        assert.deepEqual(translatedPages, pages);
        for (const file of pages) {
            const english = read("documentation", file);
            const translated = read("documentation", locale, file);
            assert.match(translated, characters, file);
            const codeBlocks = text => Array.from(text.matchAll(/^```[^\r\n]*\r?\n([\s\S]*?)^```/gm),
                match => match[1].replaceAll("\r\n", "\n"));
            assert.deepEqual(codeBlocks(translated), codeBlocks(english), `${file}: code examples must remain unchanged`);
        }
    });
}

test("the localized sidebars cover the same pages", () => {
    const hrefs = text => Array.from(text.matchAll(/href:\s*([^\s}]+)/g), match => match[1]);
    assert.deepEqual(hrefs(read("documentation", "ru-ru", "toc.yml")), hrefs(read("documentation", "toc.yml")));
    assert.deepEqual(hrefs(read("documentation", "cs-cz", "toc.yml")), hrefs(read("documentation", "toc.yml")));
    assert.deepEqual(hrefs(read("documentation", "zh-cn", "toc.yml")), hrefs(read("documentation", "toc.yml")));
});

test("rule pages use descriptive headings as their compiled sidebar captions without metadata tables", () => {
    const decodeHtml = text => text.replace(/&#x([0-9a-f]+);/gi, (_, value) => String.fromCodePoint(parseInt(value, 16)))
        .replace(/&#(\d+);/g, (_, value) => String.fromCodePoint(Number(value)))
        .replaceAll("&quot;", '"').replaceAll("&lt;", "<").replaceAll("&gt;", ">").replaceAll("&amp;", "&");
    const rulePages = pages.filter(file => /^S[AX]\d+S?\.md$/.test(file));
    for (const locale of ["", "ru-ru", "cs-cz", "zh-cn"]) {
        const toc = read("documentation", locale, "toc.yml");
        const sidebar = decodeHtml(read("_site", locale, "navigation", "toc.html"));
        for (const file of rulePages) {
            const id = file.replace(".md", "");
            const source = read("documentation", locale, file);
            const title = source.match(/^\uFEFF?# (S[AX]\d+S?: [^\r\n]+)\r?$/m)?.[1];
            assert.ok(title?.startsWith(`${id}: `), `${locale}/${file}: missing descriptive page title`);
            assert.doesNotMatch(source, /<td>(?:TypeName|CheckId|Category|Категория|Název typu|ID kontroly|Kategorie)<\/td>/);
            assert.ok(toc.includes(`- href: ${file}`), `${locale}/${file}: use a heading-derived caption`);
            assert.doesNotMatch(toc, new RegExp(`name: ${id}\\b`));
            const html = decodeHtml(read("_site", locale, file.replace(".md", ".html")));
            const heading = html.match(/<h1\b[^>]*>([\s\S]*?)<\/h1>/)?.[1].replace(/<[^>]+>/g, "").trim();
            assert.equal(heading, title, `${locale}/${file}: compiled page heading`);
            assert.ok(sidebar.includes(`>${title}</a>`), `${locale}/${file}: compiled sidebar caption`);
        }
    }
});

test("the Czech sidebar uses Czech labels in both source and compiled navigation", () => {
    const labels = [
        "Přehled",
        "Konfigurace",
        "Rozdíly oproti StyleCop Classic",
        "Oblasti pravidel",
        "Speciální pravidla",
        "Pravidla pro mezery",
        "Pravidla čitelnosti",
        "Pravidla uspořádání",
        "Pravidla pojmenování",
        "Pravidla udržovatelnosti",
        "Pravidla rozložení",
        "Pravidla dokumentace",
        "Alternativní pravidla",
    ];
    const sourceLabels = Array.from(read("documentation", "cs-cz", "toc.yml")
        .matchAll(/^\s*- name: (.+)\r?$/gm), match => match[1]);
    assert.deepEqual(sourceLabels, labels);
    const html = read("_site", "cs-cz", "navigation", "toc.html")
        .replace(/&#(\d+);/g, (_, value) => String.fromCodePoint(Number(value)));
    for (const label of labels) {
        assert.ok(html.includes(`>${label}</a>`), `Missing compiled Czech sidebar label: ${label}`);
    }
});

test("Czech rule descriptions are fully translated or use complete English fallback paragraphs", () => {
    const translations = new Map([
        ["SA1101.md", "K porušení tohoto pravidla dochází, když kód obsahuje volání člena instance místní třídy nebo základní třídy bez předpony `this.`. Výjimkou je případ, kdy existuje místní přepsání člena základní třídy a kód má v úmyslu volat přímo člena základní třídy, čímž místní přepsání obejde. V takovém případě lze místo `this.` použít předponu `base.`."],
        ["SA1120.md", "K porušení tohoto pravidla dochází, když kód obsahuje komentář C#, který neobsahuje žádný text."],
        ["SA1123.md", "K porušení tohoto pravidla dochází, když je oblast umístěna uvnitř těla prvku kódu. V mnoha editorech, včetně Visual Studia, bude oblast ve výchozím nastavení sbalená a její obsah skrytý. Skrývat kód uvnitř těla prvku je obecně nevhodné, protože to může při pozdější údržbě vést ke špatným rozhodnutím."],
        ["SA1124.md", "K porušení tohoto pravidla dochází, když je oblast umístěna kdekoli v kódu. V mnoha editorech, včetně Visual Studia, bude oblast ve výchozím nastavení sbalená a její obsah skrytý. Skrývat kód je obecně nevhodné, protože to může při pozdější údržbě vést ke špatným rozhodnutím."],
        ["SA1125.md", "K porušení tohoto pravidla dochází, když je typ Nullable definován bez použití zkráceného zápisu jazyka C#."],
        ["SA1126.md", "K porušení tohoto pravidla dochází, když kód obsahuje volání člena bez správné předpony."],
        ["SA1127.md", "K porušení tohoto pravidla dochází, když kód obsahuje jednu nebo více klauzulí `where` s omezeními obecných typů na stejném řádku jako samotná deklarace typu nebo metody."],
        ["SA1128.md", "K porušení tohoto pravidla dochází, když je inicializátor konstruktoru v kódu zčásti nebo zcela na stejném řádku jako deklarace konstruktoru."],
        ["SA1130.md", "K porušení tohoto pravidla dochází, když kód obsahuje anonymní metodu používající „starý“ zápis `delegate (parameters) { }`."],
        ["SA1131.md", "K porušení tohoto pravidla dochází, když kód obsahuje porovnání literálu nebo konstantní hodnoty s hodnotou proměnné a proměnná se ve výrazu nachází na pravé straně."],
        ["SX1101.md", "K porušení tohoto pravidla dochází, když kód obsahuje volání člena instance místní třídy nebo základní třídy s předponou `this.`. Výjimkou je případ, kdy název parametru metody koliduje s identifikátorem za předponou `this.`; v takovém případě se porušení nehlásí. Toto pravidlo je ve výchozím nastavení zakázáno. Pokud jej povolíte, měli byste zakázat pravidlo [SA1101](SA1101.md)."],
    ]);
    for (const [file, translation] of translations) {
        const english = read("documentation", file).replaceAll("\r\n", "\n");
        const czech = read("documentation", "cs-cz", file).replaceAll("\r\n", "\n");
        const paragraph = english.match(/^A violation of this rule occurs whenever[\s\S]*?(?=\n\n|$)/m)?.[0];
        assert.ok(paragraph, `${file}: missing neutral fallback paragraph`);
        const normalize = text => text.replace(/\s+/g, " ");
        assert.ok(normalize(czech).includes(normalize(paragraph)) || normalize(czech).includes(normalize(translation)),
            `${file}: use a complete English fallback or the complete Czech translation`);
    }
});

test("the Czech edition is included in the DocFX build and shared locale picker", () => {
    const configPath = path.join(root, "docfx", "docfx.cs-cz.json");
    assert.ok(fs.existsSync(configPath), "Missing Czech DocFX build configuration");
    assert.match(read("build", "documentation.ps1"), /Join-Path 'docfx' 'docfx\.cs-cz\.json'/);
    assert.match(read("build", "docfx", "common", "public", "main.js"), /lang: "cs"/);
    assert.match(read("documentation", "cs-cz", "RuleStatus.md"), /status\.cs-cz\.md/);
    assert.ok(fs.readdirSync(path.join(root, "docs")).includes("status.cs-cz.md"), "Missing Czech status page fragment (exact case)");
    assert.ok(fs.existsSync(path.join(root, "build", "docfx", "cs-cz", "token.json")), "Missing Czech DocFX tokens");
    assert.ok(fs.existsSync(path.join(root, "documentation", "cs-cz", "navbar", "toc.yml")), "Missing Czech top navigation");
});

test("DocFX configurations live under docfx and resolve paths from that directory", () => {
    assert.deepEqual(fs.readdirSync(root).filter(file => /^docfx.*\.json$/.test(file)), []);
    for (const [file, output] of [
        ["docfx.json", "_site"],
        ["docfx.ru-ru.json", "_site/ru-ru"],
        ["docfx.cs-cz.json", "_site/cs-cz"],
        ["docfx.zh-cn.json", "_site/zh-cn"],
    ]) {
        const directory = path.join(root, "docfx");
        const { build } = JSON.parse(read("docfx", file));
        assert.equal(path.resolve(directory, build.dest), path.resolve(root, output));
        for (const item of [...build.content, ...build.resource]) {
            assert.ok(item.src, `${file}: source must be explicit`);
            assert.ok(fs.existsSync(path.resolve(directory, item.src)), `${file}: missing source ${item.src}`);
        }
        for (const template of build.template.filter(template => template !== "default" && template !== "modern")) {
            assert.ok(fs.existsSync(path.resolve(directory, template)), `${file}: missing template ${template}`);
        }
        assert.ok(read("build", "documentation.ps1").includes(`Join-Path 'docfx' '${file}'`));
    }
});

for (const culture of ["ru-RU", "zh-CN"]) {
test(`${culture} resources have exactly the neutral keys and preserve format placeholders`, () => {
    const directory = path.join(root, "src", "StyleCop.Analyzers");
    const resources = text => {
        text = text.replace(/<!--[\s\S]*?-->/g, "");
        const entries = Array.from(text.matchAll(/<data\s+name="([^"]+)"[^>]*>([\s\S]*?)<\/data>/g),
            match => [match[1], match[2].match(/<value(?:\s[^>]*)?>([\s\S]*?)<\/value>/)?.[1] || ""]);
        assert.equal(new Set(entries.map(([name]) => name)).size, entries.length, "Duplicate resource keys");
        return new Map(entries);
    };
    for (const folder of fs.readdirSync(directory, { withFileTypes: true }).filter(entry => entry.isDirectory())) {
        for (const file of fs.readdirSync(path.join(directory, folder.name)).filter(file => /^[^.]+\.resx$/.test(file))) {
            const neutral = resources(read("src", "StyleCop.Analyzers", folder.name, file));
            const translated = resources(read("src", "StyleCop.Analyzers", folder.name,
                file.replace(".resx", `.${culture}.resx`)));
            assert.deepEqual(Array.from(translated.keys()).sort(), Array.from(neutral.keys()).sort(), file);
            for (const [name, value] of neutral) {
                if (culture === "zh-CN") {
                    const localized = translated.get(name);
                    assert.equal(localized.trim().length === 0, value.trim().length === 0,
                        `${file}: ${name}: do not introduce empty translations`);
                    const tags = text => Array.from(text.matchAll(/&lt;\/?([a-zA-Z][\w:.-]*)\b/g),
                        match => match[1]).sort();
                    assert.deepEqual(tags(localized), tags(value), `${file}: ${name}: preserve XML tag names`);
                    if (file === "SpacingResources.resx" && name === "SpacingNot") {
                        assert.equal(localized.match(/^\s*/)[0], "", `${file}: ${name}: leading whitespace`);
                    } else {
                        assert.equal(localized.match(/^\s*/)[0], value.match(/^\s*/)[0],
                            `${file}: ${name}: leading whitespace`);
                    }
                    assert.equal(localized.match(/\s*$/)[0], value.match(/\s*$/)[0],
                        `${file}: ${name}: trailing whitespace`);
                }

                const placeholders = text => Array.from(text.matchAll(/(?<!\{)\{(\d+)(?:[^{}]*)\}(?!\})/g),
                    match => match[1]).sort();
                if (name.endsWith("SecondPart") && neutral.has(name.replace("SecondPart", "FirstPart"))) {
                    continue;
                }

                const secondPart = name.endsWith("FirstPart") ? name.replace("FirstPart", "SecondPart") : "";
                const englishValue = value + (neutral.get(secondPart) || "");
                const russianValue = translated.get(name) + (translated.get(secondPart) || "");
                assert.deepEqual(placeholders(russianValue), placeholders(englishValue), `${file}: ${name}`);
            }
        }
    }
});
}

function loadHeader(lang, url, relativeRoot, extraLocales = [], script = read("build", "docfx", "common", "public", "main.js")) {
    const elements = [];
    const createElement = tag => ({
        tag,
        attributes: {},
        children: [],
        setAttribute(name, value) { this.attributes[name] = value; },
        getAttribute(name) { return this.attributes[name]; },
        append(child) { this.children.push(child); },
    });
    const navpanel = createElement("div");
    const document = {
        documentElement: { lang },
        querySelector: () => ({ content: relativeRoot }),
        querySelectorAll: () => elements,
        createElement,
        getElementById: id => id === "navpanel" ? navpanel : null,
    };
    const context = { document, window: { location: new URL(url) }, URL };
    vm.runInNewContext(script
        .replace("const currentLocale =", `locales.push(...${JSON.stringify(extraLocales)});\nconst currentLocale =`)
        .replace("export default", "var options ="), context);
    context.options.start();
    const picker = navpanel.children[0];
    return { options: context.options, picker, toggle: picker.children[0], links: picker.children[1].children.map(item => item.children[0]) };
}

test("the language dropdown includes Simplified Chinese and preserves the current page", () => {
    const site = "https://dotnetanalyzers.github.io/StyleCopAnalyzers/";
    const english = loadHeader("en", `${site}SA1000.html`, "");
    const chinese = english.links.find(link => link.lang === "zh-CN");
    assert.ok(chinese, "Missing Simplified Chinese language option");
    assert.equal(chinese.textContent, "简体中文");
    assert.equal(chinese.href, `${site}zh-cn/SA1000.html`);
    const localized = loadHeader("zh-CN", `${site}zh-cn/SA1000.html`, "");
    assert.equal(localized.toggle.textContent, "简体中文");
    assert.equal(localized.toggle.attributes["aria-label"], "语言: 简体中文");
    assert.equal(localized.links.find(link => link.lang === "zh-CN").attributes["aria-current"], "true");
    assert.equal(localized.links[0].href, `${site}SA1000.html`);
    assert.deepEqual(Array.from(localized.options.lunrLanguages), ["en"]);
    assert.equal(localized.options.anchors.ariaLabel, "章节链接");
});

test("language links preserve the page and the GitHub Pages project prefix", () => {
    for (const site of ["http://localhost:8080/", "https://dotnetanalyzers.github.io/StyleCopAnalyzers/"]) {
        for (const file of ["index.html", "SA1000.html", "RuleStatus.html"]) {
            for (const [lang, prefix] of [["en", ""], ["ru", "ru-ru/"], ["cs", "cs-cz/"], ["zh-CN", "zh-cn/"]]) {
                const { links, options, toggle } = loadHeader(lang, `${site}${prefix}${file}#heading`, "");
                assert.equal(links[0].href, `${site}${file}`);
                assert.equal(links[1].href, `${site}ru-ru/${file}`);
                assert.equal(links[2].href, `${site}cs-cz/${file}`);
                assert.equal(links[3].href, `${site}zh-cn/${file}`);
                const current = links.find(link => link.lang === lang);
                assert.equal(current.attributes["aria-current"], "true");
                assert.equal(current.hreflang, lang);
                assert.equal(toggle.textContent, current.textContent);
                assert.equal(toggle.tag, "button");
                assert.equal(toggle.attributes["data-bs-toggle"], "dropdown");
                assert.equal(toggle.attributes["aria-expanded"], "false");
                assert.deepEqual(Array.from(options.lunrLanguages),
                    lang === "ru" ? ["en", "ru"] : lang === "cs" ? ["en", "cs"] : ["en"]);
            }
        }
    }

    const nested = loadHeader("ru", "http://localhost:8080/ru-ru/navigation/toc.html", "../");
    assert.equal(nested.links[0].href, "http://localhost:8080/navigation/toc.html");
    assert.equal(nested.links[1].href, "http://localhost:8080/ru-ru/navigation/toc.html");
});

test("the shared locale list supports additional languages without adding header controls", () => {
    const extraLocales = [
        { lang: "fr", label: "Français", prefix: "fr-fr/", searchLanguages: ["en", "fr"], languageLabel: "Langue" },
        { lang: "de", label: "Deutsch", prefix: "de-de/", searchLanguages: ["en", "de"], languageLabel: "Sprache" },
        { lang: "ja", label: "日本語", prefix: "ja-jp/", searchLanguages: ["en", "ja"], languageLabel: "言語" },
    ];
    const { links, picker, toggle, options } = loadHeader("fr",
        "https://dotnetanalyzers.github.io/StyleCopAnalyzers/fr-fr/KnownChanges.html", "", extraLocales);
    assert.equal(picker.children.length, 2);
    assert.equal(links.length, 7);
    assert.equal(toggle.textContent, "Français");
    assert.equal(toggle.attributes["aria-label"], "Langue: Français");
    assert.deepEqual(Array.from(options.lunrLanguages), ["en", "fr"]);
    for (const [index, prefix] of ["", "ru-ru/", "cs-cz/", "zh-cn/", "fr-fr/", "de-de/", "ja-jp/"].entries()) {
        assert.equal(links[index].href, `https://dotnetanalyzers.github.io/StyleCopAnalyzers/${prefix}KnownChanges.html`);
    }
    assert.throws(() => loadHeader("unknown", "http://localhost:8080/index.html", ""), /No documentation locale configured/);

    const czech = loadHeader("cs", "http://localhost:8080/cs-cz/SA1000.html", "../");
    assert.equal(czech.toggle.textContent, "Čeština");
    assert.equal(czech.toggle.attributes["aria-label"], "Jazyk: Čeština");
    assert.equal(czech.options.anchors.ariaLabel, "Odkaz na oddíl");
});

test("all builds produce every page with separate search indexes and correct page languages", () => {
    for (const [prefix, lang] of [["", "en"], ["ru-ru", "ru"], ["cs-cz", "cs"], ["zh-cn", "zh-CN"]]) {
        for (const page of pages) {
            const html = read("_site", prefix, page.replace(".md", ".html"));
            assert.match(html, new RegExp(`<html lang="${lang}">`), page);
            assert.match(html, /name="docfx:navrel" content="toc\.html"/, page);
            if (page !== "RuleStatus.md") {
                assert.match(html, /name="docfx:tocrel" content="navigation\/toc\.html"/, page);
            }
        }

        const index = JSON.parse(read("_site", prefix, "index.json"));
        assert.ok(index["SA1000.html"]);
        assert.ok(index["SA1652.html"]);
        assert.equal(Object.keys(index).some(key => key.startsWith("ru-ru/")), false);
        assert.equal(Object.keys(index).some(key => key.startsWith("cs-cz/")), false);
        assert.equal(Object.keys(index).some(key => key.startsWith("zh-cn/")), false);
    }

    const russian = read("_site", "ru-ru", "SA1000.html");
    assert.match(russian, /placeholder="Поиск"/);
    if (read("_site", "SA1000.html").includes("Edit this page")) {
        assert.match(russian, /Изменить эту страницу/);
    }
    assert.match(russian, /name="loc:inThisArticle" content="В этой статье"/);
    const czech = read("_site", "cs-cz", "SA1000.html");
    assert.match(czech, /<html lang="cs">/);
    assert.match(czech, /name="loc:inThisArticle" content="V tomto čl(?:&#225;|á)nku"/);
    const chinese = read("_site", "zh-cn", "SA1000.html");
    assert.match(chinese, /placeholder="搜索"/);
    assert.match(chinese, /name="loc:inThisArticle" content="本文内容"/);
});

test("compiled DocFX assets add the Chinese dropdown option in every edition", () => {
    for (const [prefix, lang] of [["", "en"], ["ru-ru", "ru"], ["cs-cz", "cs"], ["zh-cn", "zh-CN"]]) {
        const html = read("_site", prefix, "SA1000.html");
        assert.match(html, /id="navpanel"/);
        assert.match(html, /<script type="module" src="\.\/public\/docfx\.min\.js"><\/script>/);
        const scripts = fs.readdirSync(path.join(root, "_site", prefix, "public")).filter(file => file.endsWith(".js"));
        const loader = scripts.find(file => read("_site", prefix, "public", file).includes('import("./main.js")'));
        assert.ok(loader, `${prefix}: missing custom header loader`);
        assert.ok(read("_site", prefix, "public", "docfx.min.js").includes(loader),
            `${prefix}: custom header loader is not imported by DocFX`);
        const { links } = loadHeader(lang, `https://dotnetanalyzers.github.io/StyleCopAnalyzers/${prefix ? `${prefix}/` : ""}SA1000.html`,
            "", [], read("_site", prefix, "public", "main.js"));
        const chinese = links.find(link => link.lang === "zh-CN");
        assert.ok(chinese, `${prefix}: missing Chinese dropdown option in compiled output`);
        assert.equal(chinese.textContent, "简体中文");
        assert.equal(chinese.href, "https://dotnetanalyzers.github.io/StyleCopAnalyzers/zh-cn/SA1000.html");
    }
});

test("Chinese configuration retains the English section bookmarks", () => {
    const english = read("_site", "Configuration.html");
    const chinese = read("_site", "zh-cn", "Configuration.html");
    for (const heading of english.matchAll(/<h[1-6]\b[^>]*\bid="([^"]+)"/g)) {
        assert.ok(chinese.includes(`id="${heading[1]}"`), `Missing Chinese configuration bookmark: ${heading[1]}`);
    }
});

test("Chinese status uses its localized report without changing implementation data", () => {
    assert.match(read("documentation", "zh-cn", "RuleStatus.md"), /\.\.\/\.\.\/docs\/status\.zh-cn\.md/);
    const fragment = read("docs", "status.zh-cn.md");
    assert.doesNotMatch(fragment, /<!DOCTYPE|<\/?(?:html|head|body|header|footer|iframe)\b/i);
    assert.doesNotMatch(fragment, /\r?\n\s*\r?\n/);
    const html = read("_site", "zh-cn", "RuleStatus.html");
    assert.match(html, /StyleCop\.Analyzers\.Status\.zh-CN\.json/);
    const report = JSON.parse(read("docs", "StyleCop.Analyzers.Status.json"));
    const chinese = JSON.parse(read("_site", "zh-cn", "status", "StyleCop.Analyzers.Status.zh-CN.json"));
    assert.deepEqual(chinese.git, report.git);
    assert.equal(chinese.diagnostics.length, report.diagnostics.length);
    for (const [index, diagnostic] of report.diagnostics.entries()) {
        const localized = chinese.diagnostics[index];
        assert.match(localized.Title, /[\u3400-\u9fff]/, diagnostic.Id);
        if (diagnostic.NoCodeFixReason) {
            assert.match(localized.NoCodeFixReason, /[\u3400-\u9fff]/, diagnostic.Id);
        }
        const { Title, NoCodeFixReason, ...data } = diagnostic;
        const { Title: localizedTitle, NoCodeFixReason: localizedReason, ...localizedData } = localized;
        assert.deepEqual(localizedData, data, diagnostic.Id);
    }
});

test("localized status reports preserve commit metadata including timestamp offsets", () => {
    const report = JSON.parse(read("docs", "StyleCop.Analyzers.Status.json"));
    for (const [locale, culture] of [["ru-ru", "ru-RU"], ["cs-cz", "cs-CZ"], ["zh-cn", "zh-CN"]]) {
        const localized = JSON.parse(read("_site", locale, "status", `StyleCop.Analyzers.Status.${culture}.json`));
        assert.deepEqual(localized.git, report.git, `${locale}: preserve the original timestamp strings and commit metadata`);
    }
});

test("status localization preserves alternate timestamp strings on supported PowerShell editions", () => {
    const reportPath = path.join(root, "docs", "StyleCop.Analyzers.Status.json");
    const localizedPaths = ["ru-RU", "cs-CZ", "zh-CN"].map(culture =>
        path.join(root, "docs", `StyleCop.Analyzers.Status.${culture}.json`));
    const originals = new Map([reportPath, ...localizedPaths].map(file =>
        [file, fs.existsSync(file) ? fs.readFileSync(file) : null]));
    const report = JSON.parse(originals.get(reportPath).toString("utf8").replace(/^\uFEFF/, ""));
    const shells = process.platform === "win32" ? ["powershell.exe", "pwsh"] : ["pwsh"];
    try {
        for (const [author, committer] of [
            ["2026-10-13T13:22:42-06:00", "2026-10-14T01:52:42+05:30"],
            ["2026-10-13T19:22:42Z", "2026-10-13T19:22:42.1234567+00:00"],
        ]) {
            report.git.Author.When = author;
            report.git.Committer.When = committer;
            fs.writeFileSync(reportPath, JSON.stringify(report), "utf8");
            for (const shell of shells) {
                // Exercise the real localization script without rebuilding the site for each fixture.
                const result = spawnSync(shell, ["-NoProfile", "-NonInteractive", "-ExecutionPolicy", "Bypass",
                    "-Command", "function dotnet { $global:LASTEXITCODE = 0 }; & .\\build\\documentation.ps1"],
                    { cwd: root, encoding: "utf8" });
                assert.equal(result.status, 0, `${shell}: ${result.error || result.stderr || result.stdout}`);
                for (const file of localizedPaths) {
                    const localized = JSON.parse(fs.readFileSync(file, "utf8"));
                    assert.deepEqual(localized.git, report.git, `${shell}: ${file}: preserve all commit metadata`);
                }
            }
        }
    } finally {
        for (const [file, content] of originals) {
            if (content === null) {
                fs.rmSync(file, { force: true });
            } else {
                fs.writeFileSync(file, content);
            }
        }
    }
});

test("the Czech status page embeds its localized fragment and requests its localized report", () => {
    const page = read("documentation", "cs-cz", "RuleStatus.md");
    const fragment = read("docs", "status.cs-cz.md");
    const html = read("_site", "cs-cz", "RuleStatus.html");
    assert.match(page, /\.\.\/\.\.\/docs\/status\.cs-cz\.md/);
    assert.match(fragment, /url: "status\/StyleCop\.Analyzers\.Status\.cs-CZ\.json"/);
    assert.match(fragment, /Sestava stavu pravidel je neplatná nebo prázdná/);
    assert.doesNotMatch(fragment, /<!DOCTYPE|<\/?(?:html|head|body|header|footer|iframe)\b/i);
    assert.doesNotMatch(fragment, /\r?\n\s*\r?\n/);
    assert.match(html, /Načítá se stav pravidel/);
    assert.match(html, /StyleCop\.Analyzers\.Status\.cs-CZ\.json/);
});

test("Russian configuration preserves the English section bookmarks used by rule pages", () => {
    const english = read("_site", "Configuration.html");
    const russian = read("_site", "ru-ru", "Configuration.html");
    for (const heading of english.matchAll(/<h[1-6]\b[^>]*\bid="([^"]+)"/g)) {
        assert.ok(russian.includes(`id="${heading[1]}"`), `Missing configuration bookmark: ${heading[1]}`);
    }
});

for (const [locale, culture, labels, messages] of [
    ["ru-ru", "ru-RU", ["Включено по умолчанию", "Правила документирования"],
        [/Не удалось загрузить отчёт/, /недопустим или пуст/, /Не удалось загрузить скрипты/]],
    ["zh-cn", "zh-CN", ["默认启用", "文档规则"],
        [/[\u3400-\u9fff]/, /[\u3400-\u9fff]/, /[\u3400-\u9fff]/]],
]) {
test(`${locale} status embeds an unframed localized fragment and requests a localized report`, () => {
    const page = read("documentation", locale, "RuleStatus.md");
    const fragment = read("docs", `status.${locale}.md`);
    assert.ok(page.includes(`../../docs/status.${locale}.md`));
    assert.doesNotMatch(fragment, /<!DOCTYPE|<\/?(?:html|head|body|header|footer|iframe)\b/i);
    assert.doesNotMatch(fragment, /\r?\n\s*\r?\n/);
    const script = fragment.match(/<script type="text\/javascript">([\s\S]*?)<\/script>/)[1];
    let request;
    let fail;
    let succeed;
    let helpers;
    const errors = [];
    const elements = new Map();
    const $ = selector => {
        if (!elements.has(selector)) {
            elements.set(selector, {
                content: "",
                hidden: false,
                removeClass() { return this; },
                addClass() { return this; },
                text(value) { this.content = value; return this; },
                html(value) { if (value === undefined) { return this.content; } this.content = value; return this; },
                show() { this.hidden = false; return this; },
                hide() { this.hidden = true; return this; },
            });
        }

        return elements.get(selector);
    };
    $.views = { helpers(value) { helpers = value; } };
    $.templates = () => ({ render: data => JSON.stringify(data) });
    $.ajax = options => {
        request = options;
        return { done(callback) { succeed = callback; return this; }, fail(callback) { fail = callback; return this; } };
    };
    vm.runInNewContext(script, { window: { jQuery: $ }, $, console: { error: error => errors.push(error) } });
    assert.equal(request.url, `status/StyleCop.Analyzers.Status.${culture}.json`);
    assert.equal(request.timeout, 15000);
    assert.equal(helpers.label("EnabledByDefault"), labels[0]);
    assert.equal(helpers.label("DocumentationRules"), labels[1]);
    fail({}, "error", "Not Found");
    assert.match(elements.get("#statusMessage").content, messages[0]);
    assert.equal(errors.length, 1);
    for (const data of [null, {}, { diagnostics: [], git: {} }, { diagnostics: [{}] }]) {
        succeed(data);
        assert.match(elements.get("#statusMessage").content, messages[1]);
        assert.equal(elements.get("#statusMessage").hidden, false);
    }

    const data = { diagnostics: [{ Id: "SA1000", Title: "Пробелы вокруг ключевых слов" }], git: { Sha: "1234" } };
    succeed(data);
    assert.match(elements.get("#renderedDiagnostics").content, /Пробелы вокруг ключевых слов/);
    assert.match(elements.get("#renderedCommitInfo").content, /1234/);
    assert.equal(elements.get("#statusMessage").hidden, true);
    $.templates = () => ({ render() { throw new Error("Invalid template"); } });
    succeed(data);
    assert.match(elements.get("#statusMessage").content, messages[1]);
    assert.equal(elements.get("#statusMessage").hidden, false);
    const dependencyMessage = {};
    vm.runInNewContext(script, { window: {}, document: { getElementById: () => dependencyMessage } });
    assert.match(dependencyMessage.textContent, messages[2]);
});
}
