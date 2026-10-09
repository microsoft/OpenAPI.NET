# Regex handling

- For fixed patterns on modern targets, use source-generated regexes with explicit match timeouts (`GeneratedRegex` under `NET8_0_OR_GREATER`).
- Use conditional compilation to provide a regular `Regex` with the same pattern and an explicit match timeout for older targets. Do not duplicate regex validation with a manually maintained character scanner.
- Older-runtime regex matching may still time out under load because timeouts use wall-clock time. If consumers encounter this limitation, recommend upgrading to a modern runtime that uses the source-generated implementation.
- Keep shared patterns in constants and reference those constants in validation diagnostics and tests.
