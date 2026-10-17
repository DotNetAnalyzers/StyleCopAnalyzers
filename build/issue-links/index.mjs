import { fromMarkdown } from "mdast-util-from-markdown";

const documentation = "https://dotnetanalyzers.github.io/StyleCopAnalyzers/";
const diagnosticId = /^S[AX]\d{4}[a-z]?$/i;

export function linkDiagnostics(body) {
    const replacements = [];
    let anchorDepth = 0;

    function visit(node) {
        if (["link", "linkReference", "image", "imageReference", "definition", "code"].includes(node.type)) {
            return;
        }

        if (node.type === "html") {
            for (const match of node.value.matchAll(/<\/?a\b[^>]*>/gi)) {
                anchorDepth = match[0].startsWith("</") ? Math.max(0, anchorDepth - 1) : anchorDepth + 1;
            }
        } else if (!anchorDepth && node.type === "inlineCode" && diagnosticId.test(node.value)) {
            const start = node.position.start.offset;
            const end = node.position.end.offset;
            replacements.push({ start, end, text: `[${body.slice(start, end)}](${documentation}${node.value.toUpperCase()}.html)` });
        } else if (!anchorDepth && node.type === "text") {
            const offset = node.position.start.offset;
            const source = body.slice(offset, node.position.end.offset);
            // GitHub also autolinks bare URLs, which CommonMark treats as text.
            for (const match of source.matchAll(/(?:https?:\/\/|www\.)[^\s<>]+|[\w.+-]+@[\w.-]+\.[a-zA-Z]{2,}|\bS[AX]\d{4}[a-z]?\b/gi)) {
                if (!diagnosticId.test(match[0])) {
                    continue;
                }

                const start = offset + match.index;
                replacements.push({
                    start,
                    end: start + match[0].length,
                    text: `[${match[0]}](${documentation}${match[0].toUpperCase()}.html)`,
                });
            }
        }

        for (const child of node.children ?? []) {
            visit(child);
        }
    }

    visit(fromMarkdown(body));
    // Edit backwards so all source offsets remain valid, preserving the original formatting.
    for (const { start, end, text } of replacements.reverse()) {
        body = body.slice(0, start) + text + body.slice(end);
    }

    return body;
}

export async function updateIssue(github, context) {
    const issue = { ...context.repo, issue_number: context.issue.number };
    const { data } = await github.rest.issues.get(issue);
    const body = linkDiagnostics(data.body ?? "");
    if (body !== (data.body ?? "")) {
        await github.rest.issues.update({ ...issue, body });
    }
}
