# CI/CD Pipeline — Randomizer

Event-driven pipeline (no cron). Workflows live here; their helper scripts live
in `scripts/`.

| File | Trigger | Purpose |
|------|---------|---------|
| `ci.yml` | push + PR on `main` + `workflow_call` | Test the library on ubuntu + windows; build the whole solution on windows |
| `release.yml` | **manual dispatch** | Build the app, pack + **push** the NuGet package, then cut the dated `vyyyyMMdd` Release |
| `nightly.yml` | successful CI on `main` + manual | Publish `nightly-yyyyMMdd` prerelease (no NuGet push) and prune old ones |
| `_build.yml` | `workflow_call` (internal) | Publish the Windows app zip and pack the NuGet package; optionally push |
| `scripts/version.pl` | invoked by workflows | Stamp each project's own `<Version>` + its folder's commit count (`--stamp`) |
| `scripts/update-changelog.mjs` | invoked by workflows | Bucketise commits into release notes by `+ - * # !` prefix |
| `scripts/prune-nightlies.mjs` | invoked by workflows | GFS retention: 7 daily + 4 weekly + 3 monthly |

## Notes

- **Versioning — files drive, never tags.** The app (`Randomizer`) and the
  library (`RandomNumberGenerators`) each carry their own `<Version>`;
  `version.pl --stamp` appends each one's folder commit count, so they version
  independently. There is no single repo version, so the repo-level Release/tag
  is the date marker `vyyyyMMdd`.
- **`secrets.NUGET_TOKEN`** must be set for `release.yml` to push to nuget.org.
- **Behaviour change vs the old `Build.yml`:** publishing moved from a weekly
  cron to a manual dispatch; nightlies and changelog notes are automatic.
