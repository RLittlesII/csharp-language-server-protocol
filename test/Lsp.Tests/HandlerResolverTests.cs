using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using OmniSharp.Extensions.LanguageServer.Protocol;
using NSubstitute;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Xunit;
using HandlerCollection = OmniSharp.Extensions.LanguageServer.Server.HandlerCollection;

namespace Lsp.Tests
{
    public class HandlerResolverTests
    {
        [Theory]
        [InlineData(typeof(IInitializeHandler), "initialize", 1)]
        [InlineData(typeof(IInitializedHandler), "initialized", 1)]
        [InlineData(typeof(IShutdownHandler), "shutdown", 1)]
        [InlineData(typeof(IExitHandler), "exit", 1)]
        public void Should_Contain_AllDefinedMethods(Type requestHandler, string key, int count)
        {
            var handler = new HandlerCollection();
            var sub = (IJsonRpcHandler)Substitute.For(new Type[] { requestHandler }, new object[0]);

            handler.Add(sub);
            handler._handlers.Should().Contain(x => x.Method == key);
            handler._handlers.Count.Should().Be(count);
        }

        [Fact]
        public void Should_Contain_AllConcreteDefinedMethods()
        {
            var handler = new HandlerCollection();

            handler.Add(
                Substitute.For<IExitHandler>(),
                Substitute.For<IInitializeHandler>(),
                Substitute.For<IInitializedHandler>(),
                Substitute.For<IShutdownHandler>()
            );

            handler._handlers.Should().Contain(x => x.Method == "exit");
            handler._handlers.Should().Contain(x => x.Method == "shutdown");
            handler._handlers.Count.Should().Be(4);
        }

        [Theory]
        [InlineData(DocumentNames.DidOpen, 4)]
        [InlineData(DocumentNames.DidChange, 4)]
        [InlineData(DocumentNames.DidClose, 4)]
        [InlineData(DocumentNames.DidSave, 4)]
        public void Should_Contain_AllDefinedTextDocumentSyncMethods(string key, int count)
        {
            var handler = new HandlerCollection();
            var sub = (IJsonRpcHandler)TextDocumentSyncHandlerExtensions.With(DocumentSelector.ForPattern("**/*.something"));

            handler.Add(sub);
            handler._handlers.Should().Contain(x => x.Method == key);
            handler._handlers.Count.Should().Be(count);
        }

        [Theory]
        [InlineData(GeneralNames.Exit, 4)]
        [InlineData(GeneralNames.Shutdown, 4)]
        [InlineData(GeneralNames.Initialized, 4)]
        [InlineData(GeneralNames.Initialize, 4)]
        public void Should_Contain_AllDefinedLanguageServerMethods(string key, int count)
        {
            var handler = new HandlerCollection();
            handler.Add(
                Substitute.For<IInitializeHandler>(),
                Substitute.For<IInitializedHandler>(),
                Substitute.For<IExitHandler>(),
                Substitute.For<IShutdownHandler>()
            );
            handler._handlers.Should().Contain(x => x.Method == key);
            handler._handlers.Count.Should().Be(count);
        }

        [Theory]
        [InlineData(GeneralNames.Exit, 4)]
        [InlineData(GeneralNames.Shutdown, 4)]
        [InlineData(GeneralNames.Initialized, 4)]
        [InlineData(GeneralNames.Initialize, 4)]
        public void Should_Contain_AllDefinedLanguageServerMethods_GivenDuplicates(string key, int count)
        {
            var handler = new HandlerCollection();
            handler.Add(
                Substitute.For<IInitializeHandler>(),
                Substitute.For<IInitializedHandler>(),
                Substitute.For<IExitHandler>(),
                Substitute.For<IShutdownHandler>(),
                Substitute.For<IInitializeHandler>(),
                Substitute.For<IInitializedHandler>(),
                Substitute.For<IExitHandler>(),
                Substitute.For<IShutdownHandler>(),
                Substitute.For<IInitializeHandler>(),
                Substitute.For<IInitializedHandler>(),
                Substitute.For<IExitHandler>(),
                Substitute.For<IShutdownHandler>()
            );
            handler._handlers.Should().Contain(x => x.Method == key);
            handler._handlers.Count.Should().Be(count);
        }

        [Theory]
        [InlineData(DocumentNames.DidOpen, 8)]
        [InlineData(DocumentNames.DidChange, 8)]
        [InlineData(DocumentNames.DidClose, 8)]
        [InlineData(DocumentNames.DidSave, 8)]
        public void Should_Contain_AllDefinedMethods_ForDifferentKeys(string key, int count)
        {
            var handler = new HandlerCollection();
            var sub = TextDocumentSyncHandlerExtensions.With(DocumentSelector.ForPattern("**/*.cs"));

            var sub2 = TextDocumentSyncHandlerExtensions.With(DocumentSelector.ForPattern("**/*.cake"));

            handler.Add(sub);
            handler.Add(sub2);
            handler._handlers.Should().Contain(x => x.Method == key);
            handler._handlers.Count.Should().Be(count);
        }

        [Theory]
        [InlineData(typeof(IInitializeHandler), typeof(IInitializedHandler), "initialize", "initialized", 2)]
        public void Should_Contain_AllDefinedMethods_OnLanguageServer(Type requestHandler, Type type2, string key, string key2, int count)
        {
            var handler = new HandlerCollection();
            var sub = (IJsonRpcHandler)Substitute.For(new Type[] { requestHandler, type2 }, new object[0]);
            if (sub is IRegistration<TextDocumentRegistrationOptions> reg)
                reg.GetRegistrationOptions()
                    .Returns(new TextDocumentRegistrationOptions() {
                        DocumentSelector = new DocumentSelector()
                    });
            handler.Add(sub);
            handler._handlers.Should().Contain(x => x.Method == key);
            handler._handlers.Should().Contain(x => x.Method == key2);
            handler._handlers.Count.Should().Be(count);
        }

        [Theory]
        [InlineData(typeof(IInitializeHandler), typeof(IInitializedHandler), "initialize", "initialized", 2)]
        public void Should_Contain_AllDefinedMethods_OnLanguageServer_WithDifferentKeys(Type requestHandler, Type type2, string key, string key2, int count)
        {
            var handler = new HandlerCollection();
            var sub = (IJsonRpcHandler)Substitute.For(new Type[] { requestHandler, type2 }, new object[0]);
            if (sub is IRegistration<TextDocumentRegistrationOptions> reg)
                reg.GetRegistrationOptions()
                    .Returns(new TextDocumentRegistrationOptions() {
                        DocumentSelector = new DocumentSelector()
                    });
            var sub2 = (IJsonRpcHandler)Substitute.For(new Type[] { requestHandler, type2 }, new object[0]);
            if (sub2 is IRegistration<TextDocumentRegistrationOptions> reg2)
                reg2.GetRegistrationOptions()
                    .Returns(new TextDocumentRegistrationOptions() {
                        DocumentSelector = new DocumentSelector()
                    });
            handler.Add(sub);
            handler.Add(sub2);
            handler._handlers.Should().Contain(x => x.Method == key);
            handler._handlers.Should().Contain(x => x.Method == key2);
            handler._handlers.Count.Should().Be(count);
        }

        [Theory]
        [InlineData("somemethod", typeof(IRequestHandler<object, object>))]
        public void Should_AllowSpecificHandlers_ToBeAdded(string method, Type handlerType)
        {
            var handler = new HandlerCollection();
            var sub = (IJsonRpcHandler)Substitute.For(new Type[] { handlerType }, new object[0]);
            var sub2= (IJsonRpcHandler)Substitute.For(new Type[] { handlerType }, new object[0]);
            handler.Add(method, sub);
            handler.Add(method, sub2);
            handler._handlers.Should().Contain(x => x.Method == method);
            handler._handlers.Should().Contain(x => x.Method == method);
            handler._handlers.Count.Should().Be(1);
        }
    }
}
