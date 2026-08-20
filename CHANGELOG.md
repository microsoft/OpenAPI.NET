# Changelog

## [1.6.31](https://github.com/microsoft/OpenAPI.NET/compare/v1.6.30...v1.6.31) (2026-08-20)


### Bug Fixes

* circular ref guard ([#3042](https://github.com/microsoft/OpenAPI.NET/issues/3042)) ([02369d0](https://github.com/microsoft/OpenAPI.NET/commit/02369d09cc8044020e415874a85e89c5dff17f67))
* harden yaml parsing (v1 port) ([#3040](https://github.com/microsoft/OpenAPI.NET/issues/3040)) ([838c847](https://github.com/microsoft/OpenAPI.NET/commit/838c8470efdd46d1cd050db8650cbc9347067f6b))

## [1.6.30](https://github.com/microsoft/OpenAPI.NET/compare/v1.6.29...v1.6.30) (2026-08-13)


### Bug Fixes

* **readers:** bound YAML anchor/alias expansion to prevent OOM (billion laughs) ([1f9aea9](https://github.com/microsoft/OpenAPI.NET/commit/1f9aea9bdcbf4066b59ce33b3bd8f97ed9cb4766))

## [1.6.29](https://github.com/microsoft/OpenAPI.NET/compare/v1.6.28...v1.6.29) (2026-04-14)


### Bug Fixes

* **hidi:** update Microsoft.OpenApi.OData to 1.7.6 ([11ba851](https://github.com/microsoft/OpenAPI.NET/commit/11ba851eb7d0d8be034903a0163dcb84955fd3ce))
* **hidi:** update Microsoft.OpenApi.OData to 1.7.6 ([f596327](https://github.com/microsoft/OpenAPI.NET/commit/f5963276f95d1808177e5ada9025192d2ce5540a)), closes [#2813](https://github.com/microsoft/OpenAPI.NET/issues/2813)

## [1.6.28](https://github.com/microsoft/openapi.net/compare/v1.6.27...v1.6.28) (2025-10-06)


### Bug Fixes

* Improve server creation and URL handling logic to maintain port ([a15c50e](https://github.com/microsoft/openapi.net/commit/a15c50ef2d13483bb64a5d5767d58da2d89aeee7))
* missing deserialization for header content property in 3.0 ([7c65a49](https://github.com/microsoft/openapi.net/commit/7c65a49117a869f772cdd91f0cf9c2eea02dc7c5))
* missing deserialization for header content property in 3.0 ([e256d29](https://github.com/microsoft/openapi.net/commit/e256d29fa8b47f267e63749c36f61283851d8bbc))

## [1.6.27](https://github.com/microsoft/openapi.net/compare/v1.6.26...v1.6.27) (2025-10-02)


### Bug Fixes

* typo in encoding allow reserved deserialization ([6eeca52](https://github.com/microsoft/openapi.net/commit/6eeca5283bcbf6ee9d4065cd5581a36d96ad930a))
* typo in encoding allow reserved deserialization ([3fadb7d](https://github.com/microsoft/openapi.net/commit/3fadb7d9d0c4c4348a4c2eb8ffcf50efd79f7226))

## [1.6.26](https://github.com/microsoft/openapi.net/compare/v1.6.25...v1.6.26) (2025-09-19)


### Bug Fixes

* OpenApiEncoding explode default value when using form style ([09b5a28](https://github.com/microsoft/openapi.net/commit/09b5a28da90963ade2051d2581ad89ef02f1514d))

## [1.6.25](https://github.com/microsoft/openapi.net/compare/v1.6.24...v1.6.25) (2025-08-22)


### Bug Fixes

* missing examples when one example is with an empty array. ([4697af4](https://github.com/microsoft/openapi.net/commit/4697af47cb974386867e8b13118eff2e928abb4e))

## [1.6.24](https://github.com/microsoft/OpenAPI.NET/compare/1.6.23...v1.6.24) (2025-04-01)


### Bug Fixes

* a flaky behaviour for format property serialization ([e072790](https://github.com/microsoft/OpenAPI.NET/commit/e07279095fcf99aeb4ea3c102516f14c501f250b))
* a flaky behaviour for format property serialization ([4d06f86](https://github.com/microsoft/OpenAPI.NET/commit/4d06f864148656e6554b2342831461bd13d407ae))
* null reference on alternate keys during hidi transform ([17247ed](https://github.com/microsoft/OpenAPI.NET/commit/17247edd07d070823cb9ea6c962997b52d71ff2b))
* null reference on alternate keys during hidi transform ([55f0f54](https://github.com/microsoft/OpenAPI.NET/commit/55f0f54a97af23e4463f1ab4ac3b80c8f3e010c9))

## Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).
