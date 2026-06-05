# Agent guide — Randomizer

Working agreement for **all** coding agents and human contributors working in
this repository. These rules are not optional. The full house spec lives in
the `Hawkynt/project-template` repo (`STANDARD.md`); this file is the
per-repo distillation.

## What this is

A **hybrid**: the `RandomNumberGenerators` NuGet library (PRNG/CSRNG/QRNG
implementations), the `Randomizer` app, and a 5700-line educational deep-dive
README that doubles as the documentation. Solution `Randomizer.slnx`; tests
in `RandomNumberGenerators.Tests`.

## Commits

- **Group changes semantically/logically** — one algorithm/concern per
  commit.
- **Every subject line starts with a prefix**: `+` added · `-` removed ·
  `*` changed · `#` bug fixed · `!` critical todo.
- Never start a subject with "fix"/"bugfix"/"changed"/"modified".
- **No AI traces anywhere**: no `Co-Authored-By` AI lines, no "Generated
  with" footers, no agent mentions in messages, comments, or authorship.

## The loop (always, in this order)

1. **Before committing**: `dotnet build Randomizer.slnx -c Release` and
   `dotnet test RandomNumberGenerators.Tests -c Release --filter
   "TestCategory!=Performance"` until green (the Performance category runs
   advisory in CI). New algorithms come with distribution/statistical-quality
   tests and a README section matching the existing article style.
2. **Commit** (rules above) and **push**.
3. **Wait for CI**; on `main` a green CI triggers the nightly (prerelease +
   GFS prune, same-day replace). Fix and loop until everything is green.

Stable releases are **manual** (`gh workflow run release.yml`) — never cut
one unless explicitly asked.

## Code conventions

- Latest C# features; algorithm implementations stay allocation-free in the
  hot path — never make a generator slower while refactoring.
- Every algorithm cites its source/paper in the README article and keeps the
  reference implementation's semantics (seed handling, period, output
  function) unless explicitly documented otherwise.
- The README *is* the documentation: code samples in multiple languages are
  intentional — keep that property when editing.

## README & repo conventions

- The README is an article-style deep dive — its chapter headers stay plain;
  the standard `## ❤️ Support` and `## 📜 License` sections close the file.
- License is LGPL-3.0-or-later; the `## ❤️ Support` section and
  `.github/FUNDING.yml` stay intact.
