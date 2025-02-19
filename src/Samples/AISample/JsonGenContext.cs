// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Richasy.AgentKernel;
using System.Text.Json.Serialization;

namespace AISample;

[JsonSourceGenerationOptions(WriteIndented = true, IncludeFields = true)]
[JsonSerializable(typeof(ChatClientConfiguration))]
[JsonSerializable(typeof(AudioClientConfiguration))]
[JsonSerializable(typeof(DrawClientConfiguration))]
[JsonSerializable(typeof(TranslateClientConfiguration))]
internal sealed partial class JsonGenContext : JsonSerializerContext
{
}
