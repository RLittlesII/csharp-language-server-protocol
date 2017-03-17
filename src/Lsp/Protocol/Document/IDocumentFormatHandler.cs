﻿using JsonRPC;
using Lsp.Models;
// ReSharper disable CheckNamespace

namespace Lsp.Protocol
{
    [Registration(typeof(TextDocumentRegistrationOptions))]
    [Method("textDocument/formatting")]
    public interface IDocumentFormatHandler : IRequestHandler<DocumentFormattingParams, TextEditContainer> { }
}