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
    const body = "sa1201 SA123 SA12345 XSA1201 SA1201x SA1201_suffix";
    assert.equal(linkDiagnostics(body), body);
    assert.equal(linkDiagnostics(""), "");
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
