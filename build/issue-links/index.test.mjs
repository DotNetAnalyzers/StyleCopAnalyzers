import assert from "node:assert/strict";
import test from "node:test";
import { linkDiagnostics, updateIssue } from "./index.mjs";

const url = id => `https://dotnetanalyzers.github.io/StyleCopAnalyzers/${id}.html`;
const link = id => `[${id}](${url(id)})`;

test("links every standalone diagnostic without changing surrounding formatting", () => {
    assert.equal(
        linkDiagnostics("# SA1201\r\n\r\nSA1202, SA1201; (SA9999)! 😀"),
        `# ${link("SA1201")}\r\n\r\n${link("SA1202")}, ${link("SA1201")}; (${link("SA9999")})! 😀`);
});

test("does not link partial IDs or unrelated text", () => {
    const body = "SA123 SA12345 XSA1201 SA1201xy SA1201_suffix SX123 SX12345 XSX1309S SX1309ss sx1309s_suffix";
    assert.equal(linkDiagnostics(body), body);
    assert.equal(linkDiagnostics(""), "");
});

test("links both prefixes and optional letter suffixes regardless of case", () => {
    const body = "SA1201 SX1309 SA1201A SX1309S sa1201 sX1309 Sa1201a sx1309s";
    const expected = [
        link("SA1201"), link("SX1309"), link("SA1201A"), link("SX1309S"),
        `[sa1201](${url("SA1201")})`, `[sX1309](${url("SX1309")})`,
        `[Sa1201a](${url("SA1201A")})`, `[sx1309s](${url("SX1309S")})`,
    ].join(" ");
    assert.equal(linkDiagnostics(body), expected);
    assert.equal(linkDiagnostics(expected), expected);
});

test("accepts only alphabetic suffixes and keeps punctuation outside links", () => {
    assert.equal(
        linkDiagnostics("(sx1309s), SA1201@ SA1201^ SA1201`"),
        `([sx1309s](${url("SX1309S")})), ${link("SA1201")}@ ${link("SA1201")}^ ${link("SA1201")}\``);
});

test("links mixed-case inline-code IDs with suffixes using uppercase URLs", () => {
    assert.equal(
        linkDiagnostics("`sx1309s` and ``sA1201a``"),
        `[\`sx1309s\`](${url("SX1309S")}) and [\`\`sA1201a\`\`](${url("SA1201A")})`);
    const body = "`sx1309ss` and `xsx1309s`";
    assert.equal(linkDiagnostics(body), body);
});

test("preserves inline, reference, HTML, and automatic links and images", () => {
    const body = [
        "[SA1201 and **SA1202**](https://example.com/a_(b) \"SA1203\")",
        "[SA1201][rule] [SA1202][] [SA1203]",
        "[rule]: https://example.com/SA1201 \"SA1202\"",
        "[SA1202]: https://example.com",
        "[SA1203]: https://example.com",
        "<https://example.com/SA1201> https://example.com/SA1202.html",
        "www.example.com/SA1201 SA1202@example.com",
        "<a href=\"https://example.com/SA1201\">SA1202</a>",
        "![SA1201](https://example.com/SA1202.png)",
        "[sx1309s](https://example.com) [`Sa1201a`](https://example.com)",
        "HTTPS://example.com/sx1309s WWW.example.com/Sa1201a",
        "<a href=\"https://example.com\">sx1309s</a>",
    ].join("\n\n");
    assert.equal(linkDiagnostics(body), body);
});

test("links inline-code IDs without changing code samples", () => {
    const body = "`SA1201` and ``SA1202``\n\n```cs\n// SA1203\n```\n\n    // SA1204\n\n`var SA1205 = 0;`";
    assert.equal(
        linkDiagnostics(body),
        `[\`SA1201\`](${url("SA1201")}) and [\`\`SA1202\`\`](${url("SA1202")})\n\n\`\`\`cs\n// SA1203\n\`\`\`\n\n    // SA1204\n\n\`var SA1205 = 0;\``);
});

test("links unlinked text alongside existing links and is idempotent", () => {
    const body = `[SA1201](https://example.com) and SA1202 and [SA1203]`;
    const expected = `[SA1201](https://example.com) and ${link("SA1202")} and [${link("SA1203")}]`;
    assert.equal(linkDiagnostics(body), expected);
    assert.equal(linkDiagnostics(expected), expected);
});

test("updates only the body using the latest issue description", async () => {
    const context = { repo: { owner: "owner", repo: "repo" }, issue: { number: 42 } };
    const issue = { owner: "owner", repo: "repo", issue_number: 42 };
    const updates = [];
    const github = { rest: { issues: {
        async get(parameters) {
            assert.deepEqual(parameters, issue);
            return { data: { body: "SA1201" } };
        },
        async update(parameters) {
            updates.push(parameters);
        },
    } } };
    await updateIssue(github, context);
    assert.deepEqual(updates, [{ ...issue, body: link("SA1201") }]);
});

test("does not update null, empty, unrelated, or already linked descriptions", async () => {
    for (const body of [null, "", "No diagnostic here", link("SA1201")]) {
        const github = { rest: { issues: {
            async get() {
                return { data: { body } };
            },
            async update() {
                assert.fail("Unchanged descriptions must not be updated");
            },
        } } };
        await updateIssue(github, { repo: { owner: "owner", repo: "repo" }, issue: { number: 42 } });
    }
});
