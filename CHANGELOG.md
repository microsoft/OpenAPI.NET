# Changelog

## [2.3.0](https://github.com/microsoft/OpenAPI.NET/compare/v2.2.0...v2.3.0) (2025-08-29)


### Features

* adds the detected format as part of the diagnostic ([#2482](https://github.com/microsoft/OpenAPI.NET/issues/2482)) ([59d7c81](https://github.com/microsoft/OpenAPI.NET/commit/59d7c81fae8cbc320a9005c529806e94cc4e9444))
* adds the detected format to the diagnostics ([59d7c81](https://github.com/microsoft/OpenAPI.NET/commit/59d7c81fae8cbc320a9005c529806e94cc4e9444))

## [2.2.0](https://github.com/microsoft/OpenAPI.NET/compare/v2.1.0...v2.2.0) (2025-08-25)


### Features

* add Validation Rule for path operations to not have a request body ([d101fc3](https://github.com/microsoft/OpenAPI.NET/commit/d101fc30cfc701f2d6c52a51b9e39fa7eae96194))


### Bug Fixes

* missing examples when one example is with an empty array. ([cb1c496](https://github.com/microsoft/OpenAPI.NET/commit/cb1c4967f37f11dad6ad42784e6c3cf8570081f9))

## [2.1.0](https://github.com/microsoft/OpenAPI.NET/compare/v2.0.1...v2.1.0) (2025-08-20)


### Features

* adds a default validation rule for unresolved references ([90b3966](https://github.com/microsoft/OpenAPI.NET/commit/90b3966e5e071e26570050733482660f41f944b0))


### Bug Fixes

* Improve OpenApiWalker performance ([a007c03](https://github.com/microsoft/OpenAPI.NET/commit/a007c039eb2a9190d4adeafd865f6d42df4221aa))
* Validate schema property is not null ([3326022](https://github.com/microsoft/OpenAPI.NET/commit/3326022b016fd3fc795f45926a55895b02557a09))

## [2.0.1](https://github.com/microsoft/openapi.net/compare/v2.0.0...v2.0.1) (2025-08-18)


### Bug Fixes

* add missing disposable for stream ([9318c00](https://github.com/microsoft/openapi.net/commit/9318c00c31466bf67d2e9701659a4cda3d82d1dd))
* add missing disposable for stream ([0c1ccbd](https://github.com/microsoft/openapi.net/commit/0c1ccbdc1ba4c53662ff6c7132366ae435d8fedb))

## [2.0.0](https://github.com/microsoft/OpenAPI.NET/compare/v2.0.0-preview.31...v2.0.0) (2025-07-10)


### Features

* General availability of version 2 with support for OpenAPI 3.1!!! 🎉🎉🎉 ([a02d74c](https://github.com/microsoft/OpenAPI.NET/commit/a02d74c2467d591a7c4d8fa89b3b18e2452be0a5))
* General availability of version 2 with support for OpenAPI 3.1!!! 🎉🎉🎉 ([97824e5](https://github.com/microsoft/OpenAPI.NET/commit/97824e5a9dc36b0c2ac56bbceef3439b9e8dfcf3))


### Bug Fixes

* inconsistent visibility of properties in current keys class ([cb9772f](https://github.com/microsoft/OpenAPI.NET/commit/cb9772fd4eb2a6fa36b65f0388f471eb66653aa2))
* removes extraneous default value constant ([bf8d0b6](https://github.com/microsoft/OpenAPI.NET/commit/bf8d0b63adc16013fdb22f38715f167ce6e21c98))
* removes extraneous default value constant ([b6eb46e](https://github.com/microsoft/OpenAPI.NET/commit/b6eb46ebb46ccd7314e00b5698fa7f6b9b6ae0da))
* removes loop methods from parsing context as its available in loop detector instead ([de3531b](https://github.com/microsoft/OpenAPI.NET/commit/de3531bc944bcb2bbc55e278e28f1a05d6d0a29f))
* removes loop methods from parsing context as its available in loop detector instead ([18a8cbe](https://github.com/microsoft/OpenAPI.NET/commit/18a8cbeeb00ee39e5bbdaabdde097ee9f8a75668))
* removes public mermaid types that were not usuable ([ef7ae33](https://github.com/microsoft/OpenAPI.NET/commit/ef7ae338dc6122a9ffc58c7661b161b319d307d9))
* removes public mermaid types that were not usuable ([a26096c](https://github.com/microsoft/OpenAPI.NET/commit/a26096c8cf9c3e53a547235a446296eaff3f413d))
* removes redundant marker interface ([5a055f9](https://github.com/microsoft/OpenAPI.NET/commit/5a055f9f616028116f1d035ee744820f37a85464))
* removes redundant marker interface ([74a9f08](https://github.com/microsoft/OpenAPI.NET/commit/74a9f083674134482b1349bce1643d9f4636ed82))
* switches to a getter for API consistency ([7975082](https://github.com/microsoft/OpenAPI.NET/commit/797508257501c8b4a336310c84030d0e128caf14))
* switches to a getter for API consistency ([0091f1c](https://github.com/microsoft/OpenAPI.NET/commit/0091f1cbf5f430fe9f54cfb5e2b66279663de632))
* unconsistent visibility of properties in current keys class ([d0c20ab](https://github.com/microsoft/OpenAPI.NET/commit/d0c20ab2209b3e7ed57cb89dbd4107d915975302))
* visibility of extension methods ([1e9112a](https://github.com/microsoft/OpenAPI.NET/commit/1e9112a4cfc01a54f265858c81b0effb8a897976))
* visibility of extension methods ([50c8e34](https://github.com/microsoft/OpenAPI.NET/commit/50c8e3459d9849d0f9acfc1e7efc99668828e8a4))

## [2.0.0-preview.31](https://github.com/microsoft/OpenAPI.NET/compare/v2.0.0-preview.30...v2.0.0-preview.31) (2025-07-02)


### Bug Fixes

* bumps openapi.net.odata to fix two critical bugs in hidi ([00c3018](https://github.com/microsoft/OpenAPI.NET/commit/00c30181214b35f4817be0cbd3827d765efd04d9))
* bumps openapi.net.odata to fix two critical bugs in hidi ([cf41355](https://github.com/microsoft/OpenAPI.NET/commit/cf41355aac3326c1cb9337078de989179f1b3b67))

## [2.0.0-preview.30](https://github.com/microsoft/OpenAPI.NET/compare/v2.0.0-preview.29...v2.0.0-preview.30) (2025-07-02)


### Bug Fixes

* migration of hidi to the latest version of system.commandline ([a5f8721](https://github.com/microsoft/OpenAPI.NET/commit/a5f8721a18e93ee881cfee371ba6d23ce0c55ae4))
* throw on circular reference ([e14258d](https://github.com/microsoft/OpenAPI.NET/commit/e14258dd2a8639702c8e0bc81a643874d207facb))
* throw on circular reference ([caea292](https://github.com/microsoft/OpenAPI.NET/commit/caea292b39d2c754bd2e2d214280add53a0a47ea))
* upgrades openapi.odata to avoid hidi failing to load ([0bea5ed](https://github.com/microsoft/OpenAPI.NET/commit/0bea5ed3cbb10230bf026288e118ce0e5025e55a))
* upgrades openapi.odata to avoid hidi failing to load ([6735397](https://github.com/microsoft/OpenAPI.NET/commit/67353976b8a17dcb6760223d06c097469fa6f794))
* validation to accept lowercase status code ranges (4xx, 5xx) in OpenAPI responses ([09f661f](https://github.com/microsoft/OpenAPI.NET/commit/09f661f0ff0511d5937fad49ae8a6182b1ea1aff))

## [2.0.0-preview.29](https://github.com/microsoft/OpenAPI.NET/compare/v2.0.0-preview.28...v2.0.0-preview.29) (2025-06-18)


### Bug Fixes

* avoid stack overflow on cyclical references ([06cc025](https://github.com/microsoft/OpenAPI.NET/commit/06cc025dca43d24955bcd205facefa4347d3f0c7))

## [2.0.0-preview.28](https://github.com/microsoft/OpenAPI.NET/compare/v2.0.0-preview.27...v2.0.0-preview.28) (2025-06-16)


### Bug Fixes

* sub-schema references are invalid ([92db49b](https://github.com/microsoft/OpenAPI.NET/commit/92db49b98ade782de3dec229936c100a1339b491))
* sub-schema references are invalid ([75d1d2b](https://github.com/microsoft/OpenAPI.NET/commit/75d1d2b62a6fab3727047be5c0e10b9987ad9f37))

## [2.0.0-preview.27](https://github.com/microsoft/OpenAPI.NET/compare/v2.0.0-preview.26...v2.0.0-preview.27) (2025-06-13)


### Bug Fixes

* implementation drift between the different version services ([1ed02e3](https://github.com/microsoft/OpenAPI.NET/commit/1ed02e3cbe7a6b484ee7db224e056ae2f0244a30))
* implementation drift between the different version services ([2514526](https://github.com/microsoft/OpenAPI.NET/commit/25145266a70a961e6899368951d95ce41fe55f5b))
* relative uri in json schema references would not parse appropriately or provide feedback to the user ([940945d](https://github.com/microsoft/OpenAPI.NET/commit/940945df29bea338a22553a3ddfb7e61853cb4e7))
* relative uri in json schema references would not parse appropriately or provide feedback to the user ([e0bceaa](https://github.com/microsoft/OpenAPI.NET/commit/e0bceaaa39e8e475652b390324254149e867f39f))
* warn instead of error out when $schema is present in the document ([388d6f7](https://github.com/microsoft/OpenAPI.NET/commit/388d6f79b4b45d1c30bddbff66dd79eef2d4e4b7))
* warn instead of error out when $schema is present in the document ([7c7b053](https://github.com/microsoft/OpenAPI.NET/commit/7c7b053b543ebdedf05b0357175fea3dc9d345db))

## [2.0.0-preview.26](https://github.com/microsoft/OpenAPI.NET/compare/v2.0.0-preview.25...v2.0.0-preview.26) (2025-06-12)


### Bug Fixes

* a bug where relative relative and sub-component JSON references would not resolve properly ([b7bc6be](https://github.com/microsoft/OpenAPI.NET/commit/b7bc6be4a05f1001130efb9af0b677dbec3055bc))
* a bug where relative relative and sub-component JSON references would not resolve properly ([c53165c](https://github.com/microsoft/OpenAPI.NET/commit/c53165c72fae25ba0864a9f3c52d42e4ee05184d))
* recursive relative reference resolution ([873acd4](https://github.com/microsoft/OpenAPI.NET/commit/873acd45eb6adecdc4219a08b028ddf35a568f01))
* recursive relative reference resolution ([f296505](https://github.com/microsoft/OpenAPI.NET/commit/f296505b7123f35b2e45eedc9118c59b8f925cbd))
* upgrades sharp yaml to avodi signing issues on netfx ([5dc7e81](https://github.com/microsoft/OpenAPI.NET/commit/5dc7e81fa7fad856edd96640ea7ee0e23e25a7f2))
* upgrades sharp yaml to avodi signing issues on netfx ([4db5c1a](https://github.com/microsoft/OpenAPI.NET/commit/4db5c1ae39d4bc6c5138fda5d9d14904f069fa3e))

## [2.0.0-preview.25](https://github.com/microsoft/OpenAPI.NET/compare/v2.0.0-preview.24...v2.0.0-preview.25) (2025-06-10)


### Bug Fixes

* ci configuration ([e7f76f7](https://github.com/microsoft/OpenAPI.NET/commit/e7f76f753649d5747a720b55babe2c59bbedfd49))
* ci configuration ([a8fd917](https://github.com/microsoft/OpenAPI.NET/commit/a8fd91728bc57102d36bfdb6442e3c273b2818e6))

## [2.0.0-preview.24](https://github.com/microsoft/OpenAPI.NET/compare/v2.0.0-preview.23...v2.0.0-preview.24) (2025-06-09)


### Bug Fixes

* callback reference annotations parsing ([86892b3](https://github.com/microsoft/OpenAPI.NET/commit/86892b361c7dcbe445ddfe10fd57fd9ab2d8a5a9))
* example reference annotation parsing ([8bf012b](https://github.com/microsoft/OpenAPI.NET/commit/8bf012b7d576f5efd6d0953859b98494958c2c4e))
* link reference annotations parsing ([2a62c5a](https://github.com/microsoft/OpenAPI.NET/commit/2a62c5a2874d935aba09c1995d022988ac793609))
* loading of header reference description ([9248560](https://github.com/microsoft/OpenAPI.NET/commit/9248560d0bac2c3e2d635b653a26d4bfa14e51f4))
* makes reference serialization object generic ([f0802e5](https://github.com/microsoft/OpenAPI.NET/commit/f0802e5cb1974b6b2d4dc7100b2e3f808ab3538b))
* parameter reference annoation parsing ([b1578f3](https://github.com/microsoft/OpenAPI.NET/commit/b1578f3fdc8bdbdeab3063ae3d3e6a554800f6d6))
* path item reference annoations parsing ([d31ed4c](https://github.com/microsoft/OpenAPI.NET/commit/d31ed4c9e0a095eb247de450b3122f0b1dd675d6))
* removes description field from references that do not support it ([03659f7](https://github.com/microsoft/OpenAPI.NET/commit/03659f7d055e6b339e15ac4434ae4037abb3a546))
* request body reference annotations parsing ([d9a78dc](https://github.com/microsoft/OpenAPI.NET/commit/d9a78dc5e5433d0f1b628569f4124d4de575cba1))
* response reference annotations parsing ([e455f52](https://github.com/microsoft/OpenAPI.NET/commit/e455f52f30fdb4937c27132b9596f89f17997663))
* security scheme reference annoations parsing ([ccc3733](https://github.com/microsoft/OpenAPI.NET/commit/ccc3733a2700d087aac289bb31a6107e9e6d743f))

## [2.0.0-preview.23](https://github.com/microsoft/OpenAPI.NET/compare/v2.0.0-preview.22...v2.0.0-preview.23) (2025-06-03)


### Features

* upgrades OData lib in Hidi to preview15 ([540240a](https://github.com/microsoft/OpenAPI.NET/commit/540240aba9f96b598459cd49b67cd02adc82713d))
* upgrades OData lib to preview15 ([b300265](https://github.com/microsoft/OpenAPI.NET/commit/b3002652805ff2a36e55531178d4fe579b196c56))

## [2.0.0-preview.22](https://github.com/microsoft/OpenAPI.NET/compare/v2.0.0-preview.21...v2.0.0-preview.22) (2025-06-02)


### Features

* Add writer settings to enable collection sorting using a comparer ([d7eaf47](https://github.com/microsoft/OpenAPI.NET/commit/d7eaf4707c26351cff4e4ec798b64967d9dd932f))


### Bug Fixes

* rename class; add logic for sorting IEnumerable collections ([58cb4ac](https://github.com/microsoft/OpenAPI.NET/commit/58cb4ac718a840a8dfb5c8821a3c0a484c29befd))

## [2.0.0-preview.21](https://github.com/microsoft/OpenAPI.NET/compare/v2.0.0-preview.20...v2.0.0-preview.21) (2025-05-21)


### Bug Fixes

* do not throw when operation tag is missing matching global tag ([fe133f2](https://github.com/microsoft/OpenAPI.NET/commit/fe133f2604e9b65cc1c7011aab7c62f44e649d19))
* do not throw when operation tag is missing matching global tag ([2c5aa40](https://github.com/microsoft/OpenAPI.NET/commit/2c5aa40cbb8bf1e96c1ccce6273579e70b69ade2))

## [2.0.0-preview.20](https://github.com/microsoft/OpenAPI.NET/compare/v2.0.0-preview.19...v2.0.0-preview.20) (2025-05-20)


### Bug Fixes

* nullable and type ordering should be maintain to ease up migration work ([6c82aa6](https://github.com/microsoft/OpenAPI.NET/commit/6c82aa6b2d6e4f39af3c594edd8590a0cf749530))
* nullable should not be inserted as an attempt to normalize the document ([6c82aa6](https://github.com/microsoft/OpenAPI.NET/commit/6c82aa6b2d6e4f39af3c594edd8590a0cf749530))
* refactor to avoid adding duplicate entries ([41fd508](https://github.com/microsoft/OpenAPI.NET/commit/41fd508074f1b70415026df3aa878cd5f5e7b1ee))
* refactor to avoid adding duplicate entries ([#2359](https://github.com/microsoft/OpenAPI.NET/issues/2359)) ([9791eb6](https://github.com/microsoft/OpenAPI.NET/commit/9791eb684a0f040feeb8c58701fd4f3577e73e2c))
* tree node has the wrong structure because of trailing slashes ([2ffb273](https://github.com/microsoft/OpenAPI.NET/commit/2ffb2735aa3718370d6094186142f9cf50b194fa))
* tree node has the wrong structure because of trailing slashes ([4439340](https://github.com/microsoft/OpenAPI.NET/commit/443934060e1e446de726addde69a2de955b95a7b))
* wrong link to json schema spec in schema doc comments ([d9b0c90](https://github.com/microsoft/OpenAPI.NET/commit/d9b0c906f7173b81fea15001d588edcbc3eed8f1))
* wrong link to json schema spec in schema doc comments ([9a73ec6](https://github.com/microsoft/OpenAPI.NET/commit/9a73ec6e5486d84b6a30a5fa0ac5961b381fc3d3))

## [2.0.0-preview.19](https://github.com/microsoft/OpenAPI.NET/compare/v2.0.0-preview.18...v2.0.0-preview.19) (2025-05-16)


### Bug Fixes

* base url should be read from the settings when available ([f5d6e81](https://github.com/microsoft/OpenAPI.NET/commit/f5d6e81c21fd18b6de0fb19b535ad6dbc187790d))
* base url should be read from the settings when available ([b17b7d8](https://github.com/microsoft/OpenAPI.NET/commit/b17b7d8d25fdf9a767411481287aacee31434aaa))
* discriminator mapping references don't get a document when created from DOM ([767d3fb](https://github.com/microsoft/OpenAPI.NET/commit/767d3fb163b273b275cd67d710c573c59e4e642b))
* discriminator mapping references don't get a document when created from DOM ([fdfe002](https://github.com/microsoft/OpenAPI.NET/commit/fdfe002d551fc3feaaeb5af24042826f13bdf412))

## [2.0.0-preview.18](https://github.com/microsoft/OpenAPI.NET/compare/v2.0.0-preview.17...v2.0.0-preview.18) (2025-05-13)


### Features

* upgrades openapi.net.odata and apimanifest to the latest version ([80844a6](https://github.com/microsoft/OpenAPI.NET/commit/80844a60ae50ba0a4d54d7dd2e45ce8360206bf5))
* upgrades openapi.net.odata and apimanifest to the latest version ([938a2e0](https://github.com/microsoft/OpenAPI.NET/commit/938a2e07b40b082e01ed1cdf3244767cbdca4061))


### Bug Fixes

* avoid calling virtual members in constructor ([5835057](https://github.com/microsoft/OpenAPI.NET/commit/5835057a7e905e371f859e727ddaf65ec08c6db0))
* Fix typo in error message ([#2345](https://github.com/microsoft/OpenAPI.NET/issues/2345)) ([3f8b2b9](https://github.com/microsoft/OpenAPI.NET/commit/3f8b2b99c07bd3c58825728c2dd2ffed91d88fbe))
* handle deserializing and writing empty security requirements [#1426](https://github.com/microsoft/OpenAPI.NET/issues/1426) ([#2323](https://github.com/microsoft/OpenAPI.NET/issues/2323)) ([962e0e4](https://github.com/microsoft/OpenAPI.NET/commit/962e0e436f96b1b68613013d75307dc1f92ce15c))
* normalized override implementation for parameter types serialization in v2 ([5930916](https://github.com/microsoft/OpenAPI.NET/commit/593091621926defcbc2727a922613e34557d882a))

## [2.0.0-preview.17](https://github.com/microsoft/OpenAPI.NET/compare/v2.0.0-preview.16...v2.0.0-preview.17) (2025-04-16)


### Features

* discriminator mappings now use schema references ([b4877f6](https://github.com/microsoft/OpenAPI.NET/commit/b4877f674ad1a240a367390d40d122eebccc0b20))
* openapiformat enum cleanup ([#2326](https://github.com/microsoft/OpenAPI.NET/issues/2326)) ([19ffd13](https://github.com/microsoft/OpenAPI.NET/commit/19ffd136a7d2137f3de0896148d9a39f469ac711))
* Remove default collection initialization for perf reasons ([#2284](https://github.com/microsoft/OpenAPI.NET/issues/2284)) ([3604382](https://github.com/microsoft/OpenAPI.NET/commit/36043829d29340a47fc93c6477a38ea93e59ef57))


### Bug Fixes

* Empty tag causes error generating Kiota client [#2283](https://github.com/microsoft/OpenAPI.NET/issues/2283) ([#2286](https://github.com/microsoft/OpenAPI.NET/issues/2286)) ([521d636](https://github.com/microsoft/OpenAPI.NET/commit/521d636e2c437c25e1758e9f6a22793d74adf2d7))
* hidi fails to parse yaml files when fixing references ([a5c4d61](https://github.com/microsoft/OpenAPI.NET/commit/a5c4d6109c433b949cdb1665c00ad778b82b28b0))
* hidi fails to parse yaml files when fixing references ([c5b69fe](https://github.com/microsoft/OpenAPI.NET/commit/c5b69fed9c413a6399c36e0f543e1019faac77e6))
* Improve handling of OpenAPI tag references ([#2325](https://github.com/microsoft/OpenAPI.NET/issues/2325)) ([bf9954a](https://github.com/microsoft/OpenAPI.NET/commit/bf9954a257691231fac6f56667bde208a98a5b42))
* read (Exclusive)Maximum and (Exclusive)Minimum values as strings and write their raw values during serialization ([#2309](https://github.com/microsoft/OpenAPI.NET/issues/2309)) ([ac66756](https://github.com/microsoft/OpenAPI.NET/commit/ac667560a951bef2824851c208c55ba070e96163))
* relative references in subdirectory documents are not loading [#1674](https://github.com/microsoft/OpenAPI.NET/issues/1674) ([#2243](https://github.com/microsoft/OpenAPI.NET/issues/2243)) ([4bcbd51](https://github.com/microsoft/OpenAPI.NET/commit/4bcbd51caff689a73e90efbc08f683383741e004))
* renames annotations schema property to metadata to match [#2241](https://github.com/microsoft/OpenAPI.NET/issues/2241) ([28e4a75](https://github.com/microsoft/OpenAPI.NET/commit/28e4a7590fb3525e30970112191d72eaf048ad6b))
* renames annotations schema property to metadata to match [#2241](https://github.com/microsoft/OpenAPI.NET/issues/2241) ([33fc7cb](https://github.com/microsoft/OpenAPI.NET/commit/33fc7cbcda71efea47070ab7a6ebf9db8787a7f8))
* set format to binary for file uploads ([#2305](https://github.com/microsoft/OpenAPI.NET/issues/2305)) ([47f10d3](https://github.com/microsoft/OpenAPI.NET/commit/47f10d323e78b9e6caa757c0d2efa378a19fc28c))

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
