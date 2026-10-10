const assert = require("node:assert/strict");
const fs = require("node:fs");
const path = require("node:path");
const test = require("node:test");
const vm = require("node:vm");

const root = path.join(__dirname, "..");
const read = (...parts) => fs.readFileSync(path.join(root, ...parts), "utf8");
const pages = fs.readdirSync(path.join(root, "documentation")).filter(file => file.endsWith(".md")).sort();

test("every English page has a Russian translation with the same code examples", () => {
    const translatedPages = fs.readdirSync(path.join(root, "documentation", "ru-ru"))
        .filter(file => file.endsWith(".md")).sort();
    assert.deepEqual(translatedPages, pages);
    for (const file of pages) {
        const english = read("documentation", file);
        const russian = read("documentation", "ru-ru", file);
        assert.match(russian, /[А-Яа-яЁё]/, file);
        const codeBlocks = text => Array.from(text.matchAll(/^```[^\r\n]*\r?\n([\s\S]*?)^```/gm),
            match => match[1].replaceAll("\r\n", "\n"));
        assert.deepEqual(codeBlocks(russian), codeBlocks(english), `${file}: code examples must remain unchanged`);
    }
});

test("the two sidebars cover the same pages", () => {
    const hrefs = text => Array.from(text.matchAll(/href:\s*([^\s}]+)/g), match => match[1]);
    assert.deepEqual(hrefs(read("documentation", "ru-ru", "toc.yml")), hrefs(read("documentation", "toc.yml")));
});

test("Russian resources have exactly the neutral keys and preserve format placeholders", () => {
    const directory = path.join(root, "StyleCop.Analyzers", "StyleCop.Analyzers");
    const resources = text => {
        text = text.replace(/<!--[\s\S]*?-->/g, "");
        const entries = Array.from(text.matchAll(/<data\s+name="([^"]+)"[^>]*>([\s\S]*?)<\/data>/g),
            match => [match[1], match[2].match(/<value(?:\s[^>]*)?>([\s\S]*?)<\/value>/)?.[1] || ""]);
        assert.equal(new Set(entries.map(([name]) => name)).size, entries.length, "Duplicate resource keys");
        return new Map(entries);
    };
    for (const folder of fs.readdirSync(directory, { withFileTypes: true }).filter(entry => entry.isDirectory())) {
        for (const file of fs.readdirSync(path.join(directory, folder.name)).filter(file => /^[^.]+\.resx$/.test(file))) {
            const neutral = resources(read("StyleCop.Analyzers", "StyleCop.Analyzers", folder.name, file));
            const translated = resources(read("StyleCop.Analyzers", "StyleCop.Analyzers", folder.name,
                file.replace(".resx", ".ru-RU.resx")));
            assert.deepEqual(Array.from(translated.keys()).sort(), Array.from(neutral.keys()).sort(), file);
            for (const [name, value] of neutral) {
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

function loadHeader(lang, url, relativeRoot, extraLocales = []) {
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
    vm.runInNewContext(read("build", "docfx", "common", "public", "main.js")
        .replace("const currentLocale =", `locales.push(...${JSON.stringify(extraLocales)});\nconst currentLocale =`)
        .replace("export default", "var options ="), context);
    context.options.start();
    const picker = navpanel.children[0];
    return { options: context.options, picker, toggle: picker.children[0], links: picker.children[1].children.map(item => item.children[0]) };
}

test("language links preserve the page and the GitHub Pages project prefix", () => {
    for (const site of ["http://localhost:8080/", "https://dotnetanalyzers.github.io/StyleCopAnalyzers/"]) {
        for (const file of ["index.html", "SA1000.html", "RuleStatus.html"]) {
            for (const lang of ["en", "ru"]) {
                const prefix = lang === "ru" ? "ru-ru/" : "";
                const { links, options, toggle } = loadHeader(lang, `${site}${prefix}${file}#heading`, "");
                assert.equal(links[0].href, `${site}${file}`);
                assert.equal(links[1].href, `${site}ru-ru/${file}`);
                const current = links.find(link => link.lang === lang);
                assert.equal(current.attributes["aria-current"], "true");
                assert.equal(current.hreflang, lang);
                assert.equal(toggle.textContent, current.textContent);
                assert.equal(toggle.tag, "button");
                assert.equal(toggle.attributes["data-bs-toggle"], "dropdown");
                assert.equal(toggle.attributes["aria-expanded"], "false");
                assert.deepEqual(Array.from(options.lunrLanguages), lang === "ru" ? ["en", "ru"] : ["en"]);
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
    assert.equal(links.length, 5);
    assert.equal(toggle.textContent, "Français");
    assert.equal(toggle.attributes["aria-label"], "Langue: Français");
    assert.deepEqual(Array.from(options.lunrLanguages), ["en", "fr"]);
    for (const [index, prefix] of ["", "ru-ru/", "fr-fr/", "de-de/", "ja-jp/"].entries()) {
        assert.equal(links[index].href, `https://dotnetanalyzers.github.io/StyleCopAnalyzers/${prefix}KnownChanges.html`);
    }
    assert.throws(() => loadHeader("unknown", "http://localhost:8080/index.html", ""), /No documentation locale configured/);
});

test("both builds produce every page with separate search indexes and correct page languages", () => {
    for (const [prefix, lang] of [["", "en"], ["ru-ru", "ru"]]) {
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
    }

    const russian = read("_site", "ru-ru", "SA1000.html");
    assert.match(russian, /placeholder="Поиск"/);
    if (read("_site", "SA1000.html").includes("Edit this page")) {
        assert.match(russian, /Изменить эту страницу/);
    }
    assert.match(russian, /name="loc:inThisArticle" content="В этой статье"/);
});

test("Russian configuration preserves the English section bookmarks used by rule pages", () => {
    const english = read("_site", "Configuration.html");
    const russian = read("_site", "ru-ru", "Configuration.html");
    for (const heading of english.matchAll(/<h[1-6]\b[^>]*\bid="([^"]+)"/g)) {
        assert.ok(russian.includes(`id="${heading[1]}"`), `Missing configuration bookmark: ${heading[1]}`);
    }
});

test("Russian status embeds an unframed localized fragment and requests a localized report", () => {
    const page = read("documentation", "ru-ru", "RuleStatus.md");
    const fragment = read("docs", "status.ru-ru.md");
    assert.match(page, /\.\.\/\.\.\/docs\/status\.ru-ru\.md/);
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
    assert.equal(request.url, "status/StyleCop.Analyzers.Status.ru-RU.json");
    assert.equal(request.timeout, 15000);
    assert.equal(helpers.label("EnabledByDefault"), "Включено по умолчанию");
    assert.equal(helpers.label("DocumentationRules"), "Правила документирования");
    fail({}, "error", "Not Found");
    assert.match(elements.get("#statusMessage").content, /Не удалось загрузить отчёт/);
    assert.equal(errors.length, 1);
    for (const data of [null, {}, { diagnostics: [], git: {} }, { diagnostics: [{}] }]) {
        succeed(data);
        assert.match(elements.get("#statusMessage").content, /недопустим или пуст/);
        assert.equal(elements.get("#statusMessage").hidden, false);
    }

    const data = { diagnostics: [{ Id: "SA1000", Title: "Пробелы вокруг ключевых слов" }], git: { Sha: "1234" } };
    succeed(data);
    assert.match(elements.get("#renderedDiagnostics").content, /Пробелы вокруг ключевых слов/);
    assert.match(elements.get("#renderedCommitInfo").content, /1234/);
    assert.equal(elements.get("#statusMessage").hidden, true);
    $.templates = () => ({ render() { throw new Error("Invalid template"); } });
    succeed(data);
    assert.match(elements.get("#statusMessage").content, /недопустим или пуст/);
    assert.equal(elements.get("#statusMessage").hidden, false);
    const dependencyMessage = {};
    vm.runInNewContext(script, { window: {}, document: { getElementById: () => dependencyMessage } });
    assert.match(dependencyMessage.textContent, /Не удалось загрузить скрипты/);
});
