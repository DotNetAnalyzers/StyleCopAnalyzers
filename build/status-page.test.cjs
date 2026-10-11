const assert = require("node:assert/strict");
const fs = require("node:fs");
const path = require("node:path");
const test = require("node:test");
const vm = require("node:vm");

const html = fs.readFileSync(path.join(__dirname, "..", "docs", "status.md"), "utf8");
const script = html.match(/<script type="text\/javascript">([\s\S]*?)<\/script>/)[1];

test("embeds the status fragment in DocFX without a second page frame", () => {
    const page = fs.readFileSync(path.join(__dirname, "..", "documentation", "RuleStatus.md"), "utf8");
    assert.match(page, /^---\r?\nlayout: landing\r?\ntitle: Rule implementation status\r?\n---/);
    assert.doesNotMatch(page, /^# /m);
    assert.doesNotMatch(html, /This page reports the current status/);
    assert.match(page, /\[!INCLUDE \[Rule status\]\(\.\.\/docs\/status\.md\)\]/);
    assert.doesNotMatch(html, /<!DOCTYPE|<\/?(?:html|head|body|header|footer|iframe)\b/i);
    assert.doesNotMatch(html, /bootstrap(?:-theme)?\.min\.css|font-awesome|modernizr/i);
    assert.doesNotMatch(html, /\r?\n\s*\r?\n/, "Blank lines end Markdown HTML blocks and can turn indented HTML into code");
    assert.match(html, /table-success/);
    assert.match(html, /scope="col"/);
});

test("keeps the old status URL as a redirect to the DocFX page", () => {
    const redirect = fs.readFileSync(path.join(__dirname, "..", "docs", "index.html"), "utf8");
    assert.match(redirect, /http-equiv="refresh" content="0; url=\.\.\/RuleStatus\.html"/);
    assert.match(redirect, /href="\.\.\/RuleStatus\.html"/);
});

test("links rules to documentation pages in the same site", () => {
    assert.match(html, /href="{{>Id}}\.html"/);
});

test("uses DocFX icons with accessible labels for code fix and Fix All status", () => {
    for (const icon of ["check-circle-fill", "x-circle-fill", "exclamation-circle-fill", "hourglass-split"]) {
        assert.ok(html.includes(`class="bi bi-${icon}" aria-hidden="true"`));
    }
    for (const label of ["Implemented", "Not yet implemented", "Batch fixer", "Custom implementation", "None"]) {
        assert.ok(html.includes(`<span class="visually-hidden">${label}</span>`));
        assert.ok(html.includes(`title="${label}"`));
    }
    assert.match(html, /Not applicable: {{>NoCodeFixReason}}/);
});

function loadPage(render = data => JSON.stringify(data)) {
    const elements = new Map();
    let request;
    let success;
    let failure;
    const errors = [];
    const $ = selector => {
        if (!elements.has(selector)) {
            elements.set(selector, {
                content: "",
                hidden: false,
                html(value) {
                    if (value === undefined) {
                        return this.content;
                    }

                    this.content = value;
                    return this;
                },
                text(value) { this.content = value; return this; },
                hide() { this.hidden = true; return this; },
                show() { this.hidden = false; return this; },
                removeClass() { return this; },
                addClass() { return this; },
            });
        }

        return elements.get(selector);
    };
    const deferred = {
        done(callback) { success = callback; return this; },
        fail(callback) { failure = callback; return this; },
    };
    $.getJSON = (url, callback) => {
        request = { url };
        success = callback;
        return deferred;
    };
    $.ajax = options => { request = options; return deferred; };
    $.templates = () => ({ render });
    const context = { $, jQuery: $, console: { error: error => errors.push(error) } };
    context.window = context;
    vm.runInNewContext(script, context);
    return {
        request,
        elements,
        errors,
        succeed: data => success(data),
        fail: () => failure({ status: 404 }, "error", "Not Found"),
    };
}

test("loads the report from the status resource directory, with a bounded request", () => {
    const page = loadPage();
    assert.equal(page.request.url, "status/StyleCop.Analyzers.Status.json");
    assert.equal(page.request.timeout, 15000);
});

test("renders rules and commit information", () => {
    const page = loadPage();
    const data = {
        diagnostics: [{ Id: "SA1000", Title: "Keywords should be spaced correctly" }],
        git: { Sha: "1234", Message: "Example commit" },
    };
    page.succeed(data);
    assert.match(page.elements.get("#renderedDiagnostics").content, /SA1000/);
    assert.match(page.elements.get("#renderedCommitInfo").content, /1234/);
    assert.equal(page.elements.get("#statusMessage").hidden, true);
});

test("shows a visible error when the report cannot be loaded", () => {
    const page = loadPage();
    page.fail();
    assert.match(page.elements.get("#statusMessage").content, /could not be loaded/i);
    assert.equal(page.elements.get("#statusMessage").hidden, false);
    assert.equal(page.errors.length, 1);
});

test("rejects empty or malformed reports instead of showing an empty table", () => {
    for (const data of [null, {}, { diagnostics: [], git: {} }, { diagnostics: [{}] }]) {
        const page = loadPage();
        page.succeed(data);
        assert.match(page.elements.get("#statusMessage").content, /invalid or empty/i);
        assert.equal(page.elements.get("#statusMessage").hidden, false);
        assert.equal(page.errors.length, 1);
    }
});

test("reports unavailable rendering dependencies", () => {
    for (const window of [{}, { jQuery: {} }]) {
        const statusMessage = { textContent: "", className: "" };
        vm.runInNewContext(script, {
            window,
            document: { getElementById: () => statusMessage },
        });
        assert.match(statusMessage.textContent, /scripts could not be loaded/i);
    }
});

test("does not partially render a report when a template fails", () => {
    const page = loadPage(data => {
        if (data.Sha) {
            throw new Error("Malformed report");
        }

        return JSON.stringify(data);
    });
    page.succeed({
        diagnostics: [{ Id: "SA1000" }],
        git: { Sha: "1234" },
    });
    assert.equal(page.elements.has("#renderedDiagnostics"), false);
    assert.equal(page.elements.has("#renderedCommitInfo"), false);
    assert.match(page.elements.get("#statusMessage").content, /invalid or empty/i);
    assert.equal(page.errors.length, 1);
});
