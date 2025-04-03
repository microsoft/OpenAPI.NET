# Changelog

## [2.0.0-preview.16](https://github.com/microsoft/OpenAPI.NET/compare/v2.0.0-preview.15...v2.0.0-preview.16) (2025-03-20)


### Bug Fixes

* always serialize security schemes in components ([3aac661](https://github.com/microsoft/OpenAPI.NET/commit/3aac661ca2e8050136c423f2835fcdd3a9096482))
* always serialize security schemes in components ([a765acf](https://github.com/microsoft/OpenAPI.NET/commit/a765acf380135694bbd4d1336bd4beddef6ef808))

## [2.0.0-preview.15](https://github.com/microsoft/OpenAPI.NET/compare/v2.0.0-preview.14...v2.0.0-preview.15) (2025-03-18)


### Bug Fixes

* Include hidi in solution ([7f4bec8](https://github.com/microsoft/OpenAPI.NET/commit/7f4bec8304771b498e8b0e33c706869ff79fd155))
* remove duplicate unused property ([f62e039](https://github.com/microsoft/OpenAPI.NET/commit/f62e039a2efde04d0c3988b359ca09ab3349a40b))
* revert change to exclude hidi in solution ([c3afe4e](https://github.com/microsoft/OpenAPI.NET/commit/c3afe4e8af2526e957940503a31079ed5f027c0a))

## [2.0.0-preview.14](https://github.com/microsoft/OpenAPI.NET/compare/v2.0.0-preview.13...v2.0.0-preview.14) (2025-03-18)


### Bug Fixes

* exclude hidi from release due to package source mapping conflict ([72daa54](https://github.com/microsoft/OpenAPI.NET/commit/72daa544f2bfe8d51ed69d7ba82d31cbc36580f2))

## [2.0.0-preview.13](https://github.com/microsoft/OpenAPI.NET/compare/v2.0.0-preview.12...v2.0.0-preview.13) (2025-03-14)


### Features

* adds OpenApiDocument.SerializeAs() so simplify serialization scenarios ([371a574](https://github.com/microsoft/OpenAPI.NET/commit/371a57405b013bdc257a51cc831f7487d1749823))
* enable null reference type support ([#2146](https://github.com/microsoft/OpenAPI.NET/issues/2146)) ([96574ec](https://github.com/microsoft/OpenAPI.NET/commit/96574ecc46dca647a708b6673c7e5309824eda2f))
* enables references as components ([eeffba9](https://github.com/microsoft/OpenAPI.NET/commit/eeffba9d50a53a3be1630d01edd8d0b57a966dee))
* use http method object instead of enum ([8baff28](https://github.com/microsoft/OpenAPI.NET/commit/8baff287aa9450ad3bd467816de321e30157bcb3))


### Bug Fixes

* a bug where references would not serialize summary or descriptions in 3.1 ([ca7ccdd](https://github.com/microsoft/OpenAPI.NET/commit/ca7ccdd933b57c2775d0295e22e541c2904b5fb7))
* handling for reference IDs with http prefix ([3385a0e](https://github.com/microsoft/OpenAPI.NET/commit/3385a0e0088c44fb926affcb20f166a02391427c))

## [2.0.0-preview.12](https://github.com/microsoft/OpenAPI.NET/compare/v2.0.0-preview.11...v2.0.0-preview.12) (2025-03-07)


### Bug Fixes

* fixes serialization of openApidocs with operation tags with settings to inline references ([8eecae6](https://github.com/microsoft/OpenAPI.NET/commit/8eecae6183f594c5508fc2c74d395c6030ce8727))
* fixes serialization of openApidocs with operation tags with settings to inline references. ([f67fe64](https://github.com/microsoft/OpenAPI.NET/commit/f67fe64e669a3f8518d89b007150c3e1bdb69fd1))

## [2.0.0-preview.11](https://github.com/microsoft/OpenAPI.NET/compare/v2.0.0-preview10...v2.0.0-preview.11) (2025-03-03)


### Bug Fixes

* rename `OpenApiDocument.SecurityRequirements` as `Security` ([d8553d6](https://github.com/microsoft/OpenAPI.NET/commit/d8553d6e007c1fa38bb982c9eb757678e789111b))


### Miscellaneous Chores

* release 2.0.0-preview.11 ([f425b8e](https://github.com/microsoft/OpenAPI.NET/commit/f425b8ed48ce5e488f85ad3060e6c43734274250))

## [2.0.0-preview10](https://github.com/microsoft/OpenAPI.NET/compare/v2.0.0-preview9...v2.0.0-preview10) (2025-02-27)


### Features

* deduplicates tags at the document level ([93c468e](https://github.com/microsoft/OpenAPI.NET/commit/93c468ebd9ee30b0cb32a583821d8abe3d017b18))
* tags references are now deduplicated as well ([763c0c1](https://github.com/microsoft/OpenAPI.NET/commit/763c0c1c5856a0ed56128b0ab8ce4b3a29ed193a))


### Bug Fixes

* add logic for serializing date time objects ([23395c5](https://github.com/microsoft/OpenAPI.NET/commit/23395c5776a781f64a7dc7bfd2867ca83eaa0bb7))
* adds missing cancellation parameter to async method ([243a111](https://github.com/microsoft/OpenAPI.NET/commit/243a111c19f2939b0a5d27c21db302f8349049eb))
* avoid creating new http clients to load additional documents of the workspace ([0f23798](https://github.com/microsoft/OpenAPI.NET/commit/0f23798f61ac964f9e71ef7402213392ebe91151))
* deduplicates exclusive min/max properties in the object model ([08414a1](https://github.com/microsoft/OpenAPI.NET/commit/08414a16db5e0a627c953f107aa34501c18996bb))
* deduplicates exclusive min/max properties in the object model ([0d5b471](https://github.com/microsoft/OpenAPI.NET/commit/0d5b4716d8cf0215257680d6cbaddaa84438eac5))
* moves the http client for the reader to settings so it can be passed by client application ([9b910f3](https://github.com/microsoft/OpenAPI.NET/commit/9b910f3928ebcb24560ff004a58e5d397ed3d836))
* OpenAPIDocument JsonSchemaDialect property is now a URI ([45977b5](https://github.com/microsoft/OpenAPI.NET/commit/45977b50188a0065fde02a3ac44a1fe718a85b30))
* openapischema schema property is now a Uri ([452a6b9](https://github.com/microsoft/OpenAPI.NET/commit/452a6b9730a2fa310aee64d0b9d2a0c7ea6d131f))
* primitive parsing for strings as DateTimes is too greedy ([4ee1d8b](https://github.com/microsoft/OpenAPI.NET/commit/4ee1d8bf44b5fcdf0fd22deca1d36ee4faf421d1))
* removes static readers registry ([fe7a2fd](https://github.com/microsoft/OpenAPI.NET/commit/fe7a2fd654e93ce99dd0ebd628042f816c787104))
* use a single http client in hidi ([9386fae](https://github.com/microsoft/OpenAPI.NET/commit/9386faec70655279ec3a031fd2afcd9cab09af40))

## [2.0.0-preview9](https://github.com/microsoft/OpenAPI.NET/compare/v2.0.0-preview8...v2.0.0-preview9) (2025-02-21)


### Features

* add support for dependentRequired ([75d7a66](https://github.com/microsoft/OpenAPI.NET/commit/75d7a662fc873566e50191127e4082b4ecf5ca7a))


### Bug Fixes

* an issue where deprecation extension parsing would fail ([5db8757](https://github.com/microsoft/OpenAPI.NET/commit/5db8757df642dbe651552ce4a7c740e94474eafc))
* an issue where deprecation extension parsing would fail ([b59864c](https://github.com/microsoft/OpenAPI.NET/commit/b59864c2387c9410e71b0caa8d439e7f122ddc24))
* refactor ToIdentifier() to normalize flaggable enums ([#2156](https://github.com/microsoft/OpenAPI.NET/issues/2156)) ([b80e934](https://github.com/microsoft/OpenAPI.NET/commit/b80e9342018cf136cc54b900bb95832a6867e982))

## [2.0.0-preview8](https://github.com/microsoft/OpenAPI.NET/compare/v2.0.0-preview7...v2.0.0-preview8) (2025-02-17)


### Bug Fixes

* a bug where external reference loading for local files would not work on linux ([df99a00](https://github.com/microsoft/OpenAPI.NET/commit/df99a00010001b35b34f9b74bbf12437f90b6b18))
* add meaningful exception message during validation ([4a6547d](https://github.com/microsoft/OpenAPI.NET/commit/4a6547d08c86194e1783ce7f52e8248ca15556e0))
* add meaningful exception message during validation ([74d20ed](https://github.com/microsoft/OpenAPI.NET/commit/74d20edebb6c5ee6150e8e03a42a40ed8c01d1da))
* adds a cancellation token argument to external document loading ([a5ffab1](https://github.com/microsoft/OpenAPI.NET/commit/a5ffab1e77c19987fe468b9297e19e8f4c48f47a))
* parsing failure on nodes set to null ([20aacc1](https://github.com/microsoft/OpenAPI.NET/commit/20aacc1a21510dbfe8cb21fb6ec2fc8b7720f2aa))
* parsing failure on nodes set to null ([4245de9](https://github.com/microsoft/OpenAPI.NET/commit/4245de9ff333a1d34e125ba72c543b6e78db6980))

## [2.0.0-preview7](https://github.com/microsoft/OpenAPI.NET/compare/v2.0.0-preview6...v2.0.0-preview7) (2025-02-06)


### Bug Fixes

* do not write null for types on parameters in v2 ([f889937](https://github.com/microsoft/OpenAPI.NET/commit/f8899379d34054e919c670ab2f8dca876bc418d5))
* do not write null for types on parameters in v2 ([4aef7b7](https://github.com/microsoft/OpenAPI.NET/commit/4aef7b7aa4d6f19be86f7bdaf1c37edb7d174317))

## [2.0.0-preview6](https://github.com/microsoft/openapi.net/compare/2.0.0-preview5...v2.0.0-preview6) (2025-02-05)


### Features

* adds a net8 target to benefit from all the conditional compilation ([a201aa2](https://github.com/microsoft/openapi.net/commit/a201aa237c39ab6748db0bfebd7d8c7be7ce4530))
* adds components registration method for schemas ([10e548a](https://github.com/microsoft/openapi.net/commit/10e548ac943d6e87b132a2fcd3784c21d320346d))
* adds deconstructor to read result ([79336f6](https://github.com/microsoft/openapi.net/commit/79336f6b1432c33f612cfc5c8ac9c79abdc04659))
* adds deconstructor to read result ([d8c1593](https://github.com/microsoft/openapi.net/commit/d8c159331c230154236faafc315d008c61f3eb7b))
* adds to identifier mapping to non nullable enum ([bd9622e](https://github.com/microsoft/openapi.net/commit/bd9622e239d5a5b2b4629d2f371f674775193af5))
* bumps target OAS version to 3.1.1 ([9e8d8a4](https://github.com/microsoft/openapi.net/commit/9e8d8a4f46a6ae79d8bb53e18ff6e9d159388893))
* configure AOT for trimming compatibility ([b4f9c3e](https://github.com/microsoft/openapi.net/commit/b4f9c3edc62e67b588e0acc04ea843f1a3bf0a76))
* makes document optional ([754f763](https://github.com/microsoft/openapi.net/commit/754f763c2b148c04f0ba11b9c8e948557cc91b14))
* makes the reference interface covariant ([7405f3c](https://github.com/microsoft/openapi.net/commit/7405f3c0c2d48b9b124a28796c3a7e9bce909aa7))
* splits described and summarized interfaces ([2a10cd9](https://github.com/microsoft/openapi.net/commit/2a10cd95d254001c397a7cd28568e468800e6644))


### Bug Fixes

* 3.0 serialization when type is set to null ([920a51a](https://github.com/microsoft/openapi.net/commit/920a51a9170eb76921fd0e6529461e7681ac4c19))
* a bug where 3.0 downcast of type null would not work ([6b636d5](https://github.com/microsoft/openapi.net/commit/6b636d53a7842e3eaf5fa70a91bc27c72be14e47))
* a bug where 3.0 downcast of type null would not work ([ac05342](https://github.com/microsoft/openapi.net/commit/ac05342befbe51944cf3a1c966d564077e8e28ea))
* a flaky behaviour for format property serialization ([3ea1fa9](https://github.com/microsoft/openapi.net/commit/3ea1fa981ec4d94909358e1c70c0904a2e3c4269))
* a flaky behaviour for format property serialization ([52981d4](https://github.com/microsoft/openapi.net/commit/52981d4cebf4831f94dd59968231740e7891c5a3))
* additional 3.1.0 constants after merge and v2 release ([9e8d8a4](https://github.com/microsoft/openapi.net/commit/9e8d8a4f46a6ae79d8bb53e18ff6e9d159388893))
* adds generic shallow copy method to avoid inadvertent conversions of references to schemas ([e4c14a4](https://github.com/microsoft/openapi.net/commit/e4c14a451d9d50bcae0cf9b74162033cb2954a72))
* adds missing culture argument to date serialization ([45329e4](https://github.com/microsoft/openapi.net/commit/45329e4e5b3606964e85bbfdece4b5f239865353))
* adds missing null prop operator to proxy properties ([8361069](https://github.com/microsoft/openapi.net/commit/83610696a0c026071308d7247fab914f4db72190))
* adds missing null propagation operators for callback and header references ([0cb4ccb](https://github.com/microsoft/openapi.net/commit/0cb4ccb925ab54e15351cbf2b0f4ae58c6b866c8))
* adds support for all component types ([8a73b54](https://github.com/microsoft/openapi.net/commit/8a73b540e88bd18aeef27e86e41dbec3e7e1d2cd))
* aligns callback parameter name with interface ([68b25cc](https://github.com/microsoft/openapi.net/commit/68b25cc5cc9ffd809a45ce532200ce3262f39ad2))
* aligns missing properties for override ([e3325b9](https://github.com/microsoft/openapi.net/commit/e3325b9d4cedee6a9734899906e938888f023506))
* aligns parameter name with interface definition for example ([d7e1f91](https://github.com/microsoft/openapi.net/commit/d7e1f919ee61e7cd4596f216890e16c7719e99c9))
* aligns reference copy constructors ([ce93aa7](https://github.com/microsoft/openapi.net/commit/ce93aa7a23280b1fb60b9bc4e5ca4a070414fb0c))
* aligns to null propagation operator ([8d57b81](https://github.com/microsoft/openapi.net/commit/8d57b81d7122c9ffad4f1c348879cc6df971617c))
* allow registration of component references ([14750dc](https://github.com/microsoft/openapi.net/commit/14750dcabe29805479c3fed10152dee1ac4111af))
* an empty security requirement should not result in an object during serialization ([1c6fd8e](https://github.com/microsoft/openapi.net/commit/1c6fd8e8ff38d0259af7fbd9903f361ecfb19225))
* build passes ([ea68427](https://github.com/microsoft/openapi.net/commit/ea68427110e5f789019b46885ea45f8f6b975c53))
* callback reference proxy implementation ([028d60b](https://github.com/microsoft/openapi.net/commit/028d60bd4003f54f6f130e56400cd7533951e1f2))
* callback reference proxy implementation ([2cbb0fa](https://github.com/microsoft/openapi.net/commit/2cbb0fa352ecaf09014f26ab5fddc8ca89e63c2c))
* components schema copy ([88daad5](https://github.com/microsoft/openapi.net/commit/88daad5d31fee2f4826be9717ece808495d9b4d8))
* conditional version for extension causes invalid json ([4030c1f](https://github.com/microsoft/openapi.net/commit/4030c1fda6a04006326e76c8a2ca7ffd98e2d1d6))
* conditional version for extension causes invalid json ([0ce92cc](https://github.com/microsoft/openapi.net/commit/0ce92cc948869e0d5eb46d388559405c9b412b06))
* date time and date time offset shifting zones ([a6a44a7](https://github.com/microsoft/openapi.net/commit/a6a44a7e3d271a2cc88fda02aabec944402a32a9))
* default settings in case of null value ([ab2ddf0](https://github.com/microsoft/openapi.net/commit/ab2ddf0f264ccaf6efbf127c23be00adec51be1f))
* do not allow null argument for example copy constructor ([aa80b19](https://github.com/microsoft/openapi.net/commit/aa80b1968d9ec6ad26f8b45578040026883d5890))
* do not copy host document as it negatively impact performance ([1043e4e](https://github.com/microsoft/openapi.net/commit/1043e4e3d2fbe4bf84aae453c5d76ce5672f64b6))
* do not copy host document as it negatively impact performance ([a46e857](https://github.com/microsoft/openapi.net/commit/a46e8578519c85b9455e41ccbeebeb8740252ae3))
* do not emit a type array in 3.1 when unnecessary ([3b3d0e6](https://github.com/microsoft/openapi.net/commit/3b3d0e6da51f7958285b8aa5be5d1eb73ec69acd))
* draft security scheme reference proxy design pattern ([1bd2624](https://github.com/microsoft/openapi.net/commit/1bd2624dcb6751c9f31ecec422d5ec9852370397))
* empty security requirements are actually valid. to negate the document ones ([42bd396](https://github.com/microsoft/openapi.net/commit/42bd3960d799af3d522ba122ce713a7d418c98ba))
* enum description number values ([ff7b4a9](https://github.com/microsoft/openapi.net/commit/ff7b4a99351661b7dd26e24bd4daa9e61d39ff27))
* enum description number values ([e29e24c](https://github.com/microsoft/openapi.net/commit/e29e24c58f51af4f6a39aeb65041dbd7f7ab888f))
* enum parsing when encountering unknown values should not default to first member ([d4e155b](https://github.com/microsoft/openapi.net/commit/d4e155b6d66887f28cbb13fb22ba0d70eabc4139))
* enum parsing when encountering unknown values should not default to first member ([9d07ebb](https://github.com/microsoft/openapi.net/commit/9d07ebb4b70bdd641748270a831df031d35b7ec4))
* extensions collection initialization ([4f28b65](https://github.com/microsoft/openapi.net/commit/4f28b657310b90c82a5b5af9f91ef6e097847a97))
* extraneous null prop removal ([1006879](https://github.com/microsoft/openapi.net/commit/10068797577e14ed4ebf6909face0aef7e3f7d56))
* failing unit test after merge ([a4ac872](https://github.com/microsoft/openapi.net/commit/a4ac872c3336fad2f6a4e05a7602ea76f6db9b49))
* failing unit tests for security scheme references in security requirements ([d2e4111](https://github.com/microsoft/openapi.net/commit/d2e4111198a435547bacfe62a5626e4d78114f8d))
* fixes inlining override when they should not happen ([704943c](https://github.com/microsoft/openapi.net/commit/704943c28f87257e896d1a79eb6962d60f719bec))
* fixes invalid OAI document for unit tests ([837f000](https://github.com/microsoft/openapi.net/commit/837f00081a1e52200b90adeac6e2aff32c92d296))
* inconsistant API surface usage ([47ad76b](https://github.com/microsoft/openapi.net/commit/47ad76b4318d1a468478d4c1c82092bd5aa0eb6a))
* last reference to copy constructor ([d87375d](https://github.com/microsoft/openapi.net/commit/d87375dc8d463fb348938acb1ed048b5a5dde166))
* makes reference fields immutable ([fda05d4](https://github.com/microsoft/openapi.net/commit/fda05d465ef84f2c4c755aca2252e2672ad40107))
* makes reference of holder immutable ([a182f44](https://github.com/microsoft/openapi.net/commit/a182f44bfb74ccbbb5b4bbf842693de48d60dac1))
* makes target field read only ([89881fd](https://github.com/microsoft/openapi.net/commit/89881fd5fa28148969eba75fad07ac26d4fb4e3d))
* missing defensive programming in copy constructors ([227d99d](https://github.com/microsoft/openapi.net/commit/227d99d23557fab82fcb7eb7d6e8fa34b486719d))
* missing doc comment for annotations ([41759a1](https://github.com/microsoft/openapi.net/commit/41759a1cb587d38392f730dfce74e974c76189c6))
* missing null prop operator on parameter reference ([019eb99](https://github.com/microsoft/openapi.net/commit/019eb99fc26f323f7a5bc79609954d237b1c0bfc))
* missing property rename ([2443fa0](https://github.com/microsoft/openapi.net/commit/2443fa0d3da5ec4ef09d9e6cae2491a117fac77b))
* multiple performance fixes for type serialization ([bd9622e](https://github.com/microsoft/openapi.net/commit/bd9622e239d5a5b2b4629d2f371f674775193af5))
* multiple performance fixes for type serialization feat: adds to identifier mapping to non nullable enum ([5fef51c](https://github.com/microsoft/openapi.net/commit/5fef51c4cb3685eceabfdbb21abb1e88a87571a9))
* multiple unit test failures ([2f171a3](https://github.com/microsoft/openapi.net/commit/2f171a3476ea0f3227ecdcb724f1d1af5406ec0e))
* null flag comparison ([081e251](https://github.com/microsoft/openapi.net/commit/081e2511b9df964ad74f7cb0e48761977e50cc45))
* null propagation for most failed reference lookup ([7994691](https://github.com/microsoft/openapi.net/commit/7994691db279c23e3ac120a54b5d96cc7f88ae3f))
* null reference check ([a5023d6](https://github.com/microsoft/openapi.net/commit/a5023d659b7adaedbe18853c297e78ac12e22823))
* Open API header proxy design pattern implementation ([77e0ad1](https://github.com/microsoft/openapi.net/commit/77e0ad10ca213c449523e9ff1802da6a6bd800e2))
* open API link reference proxy design pattern implementation ([6a96462](https://github.com/microsoft/openapi.net/commit/6a9646278377d0f81289c34ad25b2102b488dd9e))
* open API link reference proxy design pattern implementation ([376e54d](https://github.com/microsoft/openapi.net/commit/376e54de6d419c4e6673111538140bedafd7896e))
* open api response reference should not clone objects ([4243873](https://github.com/microsoft/openapi.net/commit/42438730a57acada699a017da41838d0d54e141d))
* open api schema reference proxy design pattern implementation ([e57d049](https://github.com/microsoft/openapi.net/commit/e57d04972c360f1367583eb9e60f750f8882f0b7))
* open api schema reference proxy design pattern implementation ([aebefb7](https://github.com/microsoft/openapi.net/commit/aebefb76094e71718b1d60691e9ac12a95a25283))
* parameter reference proxy design pattern implementation ([ed6ffa1](https://github.com/microsoft/openapi.net/commit/ed6ffa1d4a59857cc57f6286d383ff3a9661a00e))
* parameter reference proxy design pattern implementation ([eeb79a4](https://github.com/microsoft/openapi.net/commit/eeb79a4a7700d6fb56fbcbbe6913d224fa126167))
* passes missing host document references to all layers ([d7c4621](https://github.com/microsoft/openapi.net/commit/d7c462163272a26705f9b30f2a6c407c74acfc0f))
* passes missing host document references to all layers ([ff1406c](https://github.com/microsoft/openapi.net/commit/ff1406c60082727851d22003211faaa4e876d2e8))
* path item reference implementation ([56f291b](https://github.com/microsoft/openapi.net/commit/56f291b325682e72f1f347097be2fb9786c628b1))
* path item reference implementation ([c725267](https://github.com/microsoft/openapi.net/commit/c7252677814eec7a9a8ff6f0f3a51f5e113f5f19))
* potential NRT ([9db6e2d](https://github.com/microsoft/openapi.net/commit/9db6e2d3ce9043ff6b702060eda75290aa37b401))
* potential NRT for net8 build ([f517deb](https://github.com/microsoft/openapi.net/commit/f517deb6c7f68947a4a25da5b76ed1ee94d307e2))
* proxy design pattern implementation for OpenAPiExample ([cc28ff2](https://github.com/microsoft/openapi.net/commit/cc28ff27446dae0fc0e9f9f44dafd6df6e8fc243))
* proxy design pattern implementation for request body ([425335e](https://github.com/microsoft/openapi.net/commit/425335eb46d4a48104046af62265ba0ca6a1ec7b))
* references callback writer ([88ad997](https://github.com/microsoft/openapi.net/commit/88ad99759d8735824c7a70321ed7efc164633f06))
* removes all obsolete APIs ([e861c08](https://github.com/microsoft/openapi.net/commit/e861c08442fe7b2f1b0e4079d4a007e525a75ca9))
* removes extraneuous null prop op in copy constructor ([227d99d](https://github.com/microsoft/openapi.net/commit/227d99d23557fab82fcb7eb7d6e8fa34b486719d))
* removes nullable property that shouldn't be part of dom ([4d9c17b](https://github.com/microsoft/openapi.net/commit/4d9c17b7287b27b9058828765722f55eb378e40a))
* removes redundant assignment ([8d70195](https://github.com/microsoft/openapi.net/commit/8d701955f24801b495dfd4b3b7a2351b499355b2))
* removes unnecessary null prop in copy constructor ([aa993b1](https://github.com/microsoft/openapi.net/commit/aa993b10ff72fb18f7dc3f49d87586662188a381))
* removes unused parameters ([de9d979](https://github.com/microsoft/openapi.net/commit/de9d979ec3f53fb7a86c43309f7e87d20d89d22a))
* removes unused parameters ([9cd7aae](https://github.com/microsoft/openapi.net/commit/9cd7aaea76316b3944e8a549db9aad3a3155b51b))
* removes useless condition for null check ([4a50c77](https://github.com/microsoft/openapi.net/commit/4a50c77a90f0e9810b4912cbb694883921c508cd))
* removes useless virtual definitions in components ([af3038a](https://github.com/microsoft/openapi.net/commit/af3038a0fcee46c4806382fe061e4f2e7059fdbe))
* removes virtual modifier in MediaType ([4dfc9b8](https://github.com/microsoft/openapi.net/commit/4dfc9b8c533d454cefa3d576adb4d3d422747d16))
* request body references are converted to v2 properly ([b84ea19](https://github.com/microsoft/openapi.net/commit/b84ea194a16e03a1f2b6f56af892eb6288d5627a))
* response reference proxy design pattern implementation ([8103c20](https://github.com/microsoft/openapi.net/commit/8103c20f669eb9c127aec3baaaafda3987381d9b))
* response reference proxy design pattern implementation ([5b4003b](https://github.com/microsoft/openapi.net/commit/5b4003bd04d59fd460280fa06e6151b0203680cb))
* restores default constructor for ISerializable implementation ([778184f](https://github.com/microsoft/openapi.net/commit/778184ff608cd4172de689684272b2d7a8627339))
* returns reference instead of null ([45e40fa](https://github.com/microsoft/openapi.net/commit/45e40fa675570fc382d4a72684a009b567d45118))
* sets hidi version to a preview ([975b1bf](https://github.com/microsoft/openapi.net/commit/975b1bfa563bc36ad8031de934af158cb807bca8))
* sets hidi version to a preview ([8999336](https://github.com/microsoft/openapi.net/commit/899933636f15991add45e367befd1c30c93bcf2c))
* shallow copy for callback ([4ea87ef](https://github.com/microsoft/openapi.net/commit/4ea87efad0edde89f2e29c0c495a33a4467ba939))
* shallow copy for example ([9bc3044](https://github.com/microsoft/openapi.net/commit/9bc30443ab95fd05b0b328c13b7e36e911628dda))
* shallow copy for parameter link path item and request body ([9af6f30](https://github.com/microsoft/openapi.net/commit/9af6f30719c7e0718798df98096f1679f74c20e7))
* side effects in tag references ([717deb0](https://github.com/microsoft/openapi.net/commit/717deb08d2198519a69a3ccf701f46dac229e608))
* side effects in tag references ([878593b](https://github.com/microsoft/openapi.net/commit/878593b7a7e6ff1f2adc6965608c46e5f8ce8f38))
* single copy and maintain for references ([30ee6ed](https://github.com/microsoft/openapi.net/commit/30ee6ed9ac8e6a6a7d1931bed9da22e0116ec9af))
* specifies encoding for net fx ([95dafe6](https://github.com/microsoft/openapi.net/commit/95dafe60a103293acba6c6aa16c4c780e7b576c9))
* specifies encoding for net fx ([cd13481](https://github.com/microsoft/openapi.net/commit/cd13481f4e3a883186d10b3af63cbd928a17c8ba))
* support non-standard MIME type in response header ([50ddca2](https://github.com/microsoft/openapi.net/commit/50ddca2f72a9b4eef21c45fdd1c978e8f6f32eb3))
* switches header to shallow copy ([2a42c36](https://github.com/microsoft/openapi.net/commit/2a42c36eb7d83c0b83f8263b7989f84c5ddf911d))
* tag reference proxy design pattern implementation ([46e08d4](https://github.com/microsoft/openapi.net/commit/46e08d4b53e756db5d717337488c9bb787ec8ee7))
* tag, response, and security scheme shallow copy ([7ac149c](https://github.com/microsoft/openapi.net/commit/7ac149c70d69c357aa0dc0d8e29e975a886226f9))
* updates public api file ([b727581](https://github.com/microsoft/openapi.net/commit/b727581d6fd1d814b5c1887400cb48f06dd96362))
* updates public API surface with net8 target ([1a1e013](https://github.com/microsoft/openapi.net/commit/1a1e0135e977440be91e64d14e3d2b094238facd))
* uses backing fields instead of schema copy ([6f4e7a2](https://github.com/microsoft/openapi.net/commit/6f4e7a245376cb816367ff86c497cbba023b6faf))
* uses the json node clone API to avoid unecessary allocs ([818414d](https://github.com/microsoft/openapi.net/commit/818414d73a351447a403e8555c140b180de5d375))
* v2 references for properties do not work as expected ([aa90edf](https://github.com/microsoft/openapi.net/commit/aa90edf1b624e1bd2381700f2d8b659507bf0119))
* v2 references for properties do not work as expected ([ec9c01b](https://github.com/microsoft/openapi.net/commit/ec9c01b9b873d02ab2682ceb5fb9ac509a931781))
* v2 request body content null propagation ([6d064c4](https://github.com/microsoft/openapi.net/commit/6d064c4b967f7f9262a85438d27cf7bf2ccc412c))
* v2 request body content null propagation ([8b4833c](https://github.com/microsoft/openapi.net/commit/8b4833cce98cb8d8c782ceed8d5d122357b71065))
* visibility of serialize internal methods ([dc8a757](https://github.com/microsoft/openapi.net/commit/dc8a7572ec436c1ed35f5a6208c6aa868702dc0f))


### Performance Improvements

* avoid round trip serialization ([a6a44a7](https://github.com/microsoft/openapi.net/commit/a6a44a7e3d271a2cc88fda02aabec944402a32a9))

## Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).
