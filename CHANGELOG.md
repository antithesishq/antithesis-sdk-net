# Changelog

## 3.1.x - 2026-11-10

### Breaking
- The package now targets .NET 10 (the current LTS); .NET 6 through 9 are no longer supported.

### Bug Fixes
- Serialization failures report contextual `antithesis_error` records and preserve assertions and events without the failed details. Guidance that cannot be serialized is skipped, since omitting its operands would produce an invalid record.
- Lifecycle and rich assertions no longer attach or mutate caller-owned details, allowing details to be reused.
- Increased efficiency of many assertion methods.
- `Catalog` now declares the guidance point a rich assertion carries, alongside its assertion. Previously it wrote only the
assertion, so a rich assertion's guidance was reported at runtime having never been declared; every other Antithesis SDK
declares both.

### New
- Details containing non-finite numbers are emitted with those numbers as the strings `"NaN"`, `"Infinity"` or `"-Infinity"`.

## 3.0.1 - 2026-08-28

### Breaking
- The `System.Random` subclass formerly named `Antithesis.SDK.Random` is now named `Antithesis.SDK.AntithesisRandom`.
This allows us to more closely align with other Antithesis SDKs that include a `Random` class for providing simple
Antithesis sourced entropy (see New section below).

### Bug Fixes
- Fixed the bug truncating strings containing non-ASCII characters when writing to `FFISink`.

### New
- Added `Assert.Raw`, a low-level assertion entry point for third-party frameworks that supply their own catalog metadata.
- Added `Antithesis.SDK.Random` with `GetRandom()` to source entropy directly from the Antithesis platform and
`RandomChoice<T>(IReadOnlyList<T>)` to randomly pick an element of a list.

## 2.0.4 - 2026-04-22

### Breaking
- In order to improve performance, "numeric guidance" Assert methods no longer write when the difference between its two
operands does not change (i.e. they only write the first occurrence of a given difference):
AlwaysGreaterThan(OrEqualTo), AlwaysLessThan(OrEqualTo), SometimesGreaterThan(OrEqualTo), SometimesLessThan(OrEqualTo). 
This is only breaking if you rely on ANTITHESIS_SDK_LOCAL_OUTPUT when testing locally.

### Bug Fixes
- None

### New
- None

## 1.0.31 - 2025-06-10

First official release.
