# Changelog

All notable changes to this project will be documented in this file.
The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added

- public static methods for drawing Box, Sphere, WireSphere, Line, Arrow.
- DebugDraw.WireQuad() and DebugDraw.Quad() methods.
- DebugDraw.WireCapsule() method.

### Obselete

- [obselete] Fixed Arrow mesh rotations in world space by adding parameter for Arrow method(s) to use a provided up axis. This prevents the arrow from rotating about its forward axis.
