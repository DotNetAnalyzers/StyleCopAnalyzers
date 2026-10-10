---
name: maintaining-translations
description: Keep localized .resx resources current without sacrificing translation quality. Use when adding resource strings, finding missing translations, translating code-fix text or diagnostics, or reviewing localized resources and their fallback behavior.
---

# Maintaining translations

Prefer a correct English fallback to an unreliable translation. Missing localized keys are supported; complete
key coverage is not a reason to ship mixed-language text or change the meaning of a diagnostic.

## 1. Find the actual gaps

- Discover all `.resx` files, then group each neutral resource file with its culture-specific siblings.
  Do not create translations for resource families or cultures that the repository does not already support
  unless requested.
- Parse XML and compare `<data name="...">` keys, not lines or file sizes. Check for duplicate keys before
  constructing a dictionary: a dictionary silently hides duplicates.
- Report missing keys separately from intentionally empty values and existing translations. Preserve existing
  translations unless correcting a demonstrated problem.
- Compare against the neutral English resource, not another partially translated locale. Do not assume that
  every locale has the same missing keys.

## 2. Learn terminology from context

Use the corresponding localized Microsoft Learn pages as terminology and style references. Compare the same
article in English and the target language, and read complete sentences rather than translating isolated words.
Useful starting points:

- [C# XML documentation examples](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/xmldoc/examples)
- [Recommended XML documentation tags](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/xmldoc/recommended-tags)

Change the URL locale to `cs-cz`, `de-de`, `fr-fr`, `nl-nl`, `pl-pl`, `pt-br`, or `ru-ru` as appropriate.
Microsoft Learn often uses `es-es` for Spanish documentation; use its technical terminology as a reference,
but adapt wording to the repository's `es-MX` locale. Confirm the fetched page's locale: a URL alone does not
prove the content was translated.

Some Learn pages are machine-translated (`ms.translationtype: MT`), and code samples may remain in English.
Treat these pages as evidence for terminology, not proof that every sentence is natural or technically correct.
Use existing repository translations for consistency, while checking their meaning against the English source.
Do not send repository strings to external translation services.

## 3. Translate conservatively

- Translate complete phrases manually. Never generate translations by substituting words or fragments in the
  English sentence: that produces mixed languages, incorrect grammar, and even corrupted technical names.
- Start with short, well-understood code-fix actions. Longer diagnostic descriptions need sentence-level review.
  If a translation cannot be made confidently, omit that newly added key and retain English fallback. Do not
  populate it with English solely to make key counts match.
- Preserve the meaning, negation, severity, and distinctions between constructors, destructors, parameters,
  type parameters, accessors, summaries, and return values.
- Keep placeholders such as `{0}` and `{1}`, XML tag/attribute names such as `<summary>`, `<returns>`, and
  `name`, and language identifiers such as `void` unchanged. XML-escape literal tags inside `<value>`.
  Natural word order may move placeholders; validate their multiset, not their order.
- Treat `en-GB` as a localization too: use British spellings such as `Finalise` and `unrecognised`, consistent
  with existing `Initialises` and `Finalises`. Check compounds in languages that require them, for example
  Dutch `Constructordocumentatie` and `Destructordocumentatie`.
- For generated documentation fragments, inspect how callers concatenate the text around `<see>` elements.
  Leading/trailing spaces and punctuation are part of the value. Preserve `xml:space="preserve"` and validate
  the assembled sentence. Omit uncertain fragments rather than generating ungrammatical documentation.
- Follow the existing three-line `<data>` format, whitespace-preservation attributes, encoding/BOM, and line
  endings. Do not rewrite unrelated entries or generated designer files.

## 4. Understand per-key fallback

The generated strongly typed resource accessors use `ResourceManager.GetString`. If a key is missing in the
requested culture, lookup continues through parent cultures and ultimately the neutral English resources.
Thus a localized `.resx` does not need every English key. An existing empty string is a value, not a missing
key, and does not trigger fallback. Direct `ResourceSet` access does not perform this per-key fallback.

See [ResourceManager.GetString](https://learn.microsoft.com/en-us/dotnet/api/system.resources.resourcemanager.getstring)
and [resource fallback](https://learn.microsoft.com/en-us/dotnet/core/extensions/package-and-deploy-resources#the-resource-fallback-process).
If the lookup path changes, inspect it rather than assuming fallback still applies.

## 5. Validate content, not just coverage

Before committing:

1. Parse every changed `.resx`; reject duplicate keys and keys absent from the neutral source.
2. Verify that pre-existing values remain unchanged unless intentionally corrected.
3. Compare placeholder multisets and literal XML tag/attribute names for every added or changed translation.
4. Check boundary whitespace and `xml:space="preserve"` on concatenated fragments, including the assembled
   text around generated XML elements.
5. Read each added translation as a complete sentence. Look for leftover English prose, wrong negation,
   translated XML identifiers, locale-specific spelling, and malformed compounds. Automated XML and key
   checks cannot establish translation quality.
6. Build the analyzer project with the SDK selected by `global.json`, using `--no-incremental` to run resource
   generation. Follow `lightup-and-roslyn-versions` for generated-file checks; never hand-edit `.generated`.
7. Run `git diff --check` and inspect the final diff for unrelated churn.

For behavior changes to generated documentation or a reported localization bug, follow `fixing-a-bug` and
`testing-and-coverage` for a targeted regression test. State clearly which translations were added, which
strings still fall back to English, and whether native-speaker review occurred. Never describe XML parsing,
a successful build, or matching key counts as proof of translation quality.
