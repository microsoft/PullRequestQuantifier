﻿// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Not applicable")]
[assembly: SuppressMessage("Style", "IDE0057:Use range operator", Justification = "Not applicable")]
[assembly: SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "FP", Scope = "member", Target = "~M:PullRequestQuantifier.Local.Client.CommandLine.#ctor(System.String[])")]
