﻿// Copyright (c) Microsoft. All rights reserved.

using System.Diagnostics.CodeAnalysis;

namespace Microsoft.SemanticKernel.Data;

/// <summary>
/// Options when creating a <see cref="VolatileVectorRecordStore{TRecord}"/>.
/// </summary>
[Experimental("SKEXP0001")]
public sealed class VolatileVectorRecordStoreOptions
{
    /// <summary>
    /// Gets or sets the default collection name to use.
    /// If not provided here, the collection name will need to be provided for each operation or the operation will throw.
    /// </summary>
    public string? DefaultCollectionName { get; init; } = null;

    /// <summary>
    /// Gets or sets an optional record definition that defines the schema of the record type.
    /// </summary>
    /// <remarks>
    /// If not provided, the schema will be inferred from the record model class using reflection.
    /// In this case, the record model properties must be annotated with the appropriate attributes to indicate their usage.
    /// See <see cref="VectorStoreRecordKeyAttribute"/>, <see cref="VectorStoreRecordDataAttribute"/> and <see cref="VectorStoreRecordVectorAttribute"/>.
    /// </remarks>
    public VectorStoreRecordDefinition? VectorStoreRecordDefinition { get; init; } = null;
}
