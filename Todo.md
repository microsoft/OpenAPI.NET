# ToDo List

## Approach

- [DONE] Never throw an exception from the parser, just add to the errors collection, and try and keep going
- ..or provide a client parameter  (NeverThrow, ThrowIfErrors, ThrowOnFirst)
- [DONE] Load components first, or JIT load referenced items
- Started. Add validation to model classes, throw exception as normal.  Allow parser to catch exception and add to ParseErrors collection

## Organization

- [DONE] Move parse field dictionaries out of model classes  (Still not convinced about this)
- [DONE] Move errors collection to parsing context object 

## Missing features

- parse V2 OpenAPI (Need high level understanding of how this is going to work)
- [DONE] need tests to show $ref is working.

## Nice to have

- Add extension parsers