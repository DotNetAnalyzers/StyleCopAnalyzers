# Agent instructions

StyleCop.Analyzers is a set of Roslyn analyzers and code fixes. The shipped assemblies compile against
Microsoft.CodeAnalysis 1.2.1 and reach newer compiler APIs through light-up. Tests run on every compiler from
Roslyn 1.3.2 (C# 6) up to the newest one, in `StyleCop.Analyzers.Test` and `StyleCop.Analyzers.Test.CSharp7`
through `StyleCop.Analyzers.Test.CSharp15`. Each test project inherits all the tests of the previous one.

Procedures live in [Agent Skills](https://agentskills.io) under [`.agents/skills`](.agents/skills). Read the
matching skill before you start:

| Skill | Use it when |
| --- | --- |
| [fixing-a-bug](.agents/skills/fixing-a-bug/SKILL.md) | fixing any reported bug or audit finding (reproduce it in a failing test first) |
| [testing-and-coverage](.agents/skills/testing-and-coverage/SKILL.md) | writing or running tests, reading local results against CI, coverage |
| [pull-request-workflow](.agents/skills/pull-request-workflow/SKILL.md) | branches, drafts, CI checks, contributors' commits, reviews |
| [analyzer-performance](.agents/skills/analyzer-performance/SKILL.md) | any analyzer change (maintainers don't accept perf risk) |
| [lightup-and-roslyn-versions](.agents/skills/lightup-and-roslyn-versions/SKILL.md) | new syntax in product code, `Syntax.xml`, generated files |
| [csharp-language-version-audit](.agents/skills/csharp-language-version-audit/SKILL.md) | auditing rules against a new C# version, and the test project for that version |
| [issue-triage](.agents/skills/issue-triage/SKILL.md) | reviewing, de-duplicating or closing issues |
| [maintaining-translations](.agents/skills/maintaining-translations/SKILL.md) | adding or reviewing localized resources, missing translations, terminology and English fallback |

Ground rules:

- Build with the SDK in `global.json` and keep warnings at zero. See [CONTRIBUTING.md](CONTRIBUTING.md).
- Never skip or weaken tests. Never hand-edit `StyleCop.Analyzers/StyleCop.Analyzers/Lightup/.generated`.
- Open PRs as drafts, and mark them ready only when every CI check is green. Humans merge.
- Don't close, comment on, or label issues or other people's PRs unless the person you work for asked you to.
