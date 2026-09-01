#region Enbrea.Bcp47 - Copyright (c) STÜBER SYSTEMS GmbH
/*
 *    Enbrea.Bcp47
 *
 *    Copyright (c) STÜBER SYSTEMS GmbH
 *
 *    Licensed under the MIT License.
 *
 */
#endregion

using System;
using Xunit;

namespace Enbrea.Bcp47.XUnit;

/// <summary>
/// Unit tests for <see cref="LanguageTagBuilder"/>.
/// </summary>
public class LanguageTagBuilderTests
{
    [Fact]
    public void Build_NormalizesComponents()
    {
        var tag = new LanguageTagBuilder()
            .Language("DE")
            .Script("latn")
            .Region("de")
            .Build();

        Assert.Equal("de-Latn-DE", tag.ToString());
    }

    [Fact]
    public void Build_WithDuplicateExtensionSingleton_ThrowsFormatException()
    {
        var builder = new LanguageTagBuilder()
            .Language("en")
            .Extension('a', "aaa")
            .Extension('A', "bbb");

        Assert.Throws<FormatException>(() => builder.Build());
    }

    [Fact]
    public void Build_WithDuplicateVariant_ThrowsFormatException()
    {
        var builder = new LanguageTagBuilder()
            .Language("sl")
            .Variant("rozaj")
            .Variant("ROZAJ");

        Assert.Throws<FormatException>(() => builder.Build());
    }

    [Fact]
    public void Build_WithExtension_ReturnsLanguageTag()
    {
        var tag = new LanguageTagBuilder()
            .Language("en")
            .Extension('u', "ca", "gregory")
            .Build();

        Assert.Equal("en-u-ca-gregory", tag.ToString());

        _ = Assert.Single(tag.Extensions);
        Assert.Equal('u', tag.Extensions[0].Singleton);
        Assert.Equal(["ca", "gregory"], tag.Extensions[0].Subtags);
    }

    [Fact]
    public void Build_WithExtlang_ReturnsLanguageTag()
    {
        var tag = new LanguageTagBuilder()
            .Language("zh")
            .Extlang("cmn")
            .Script("Hans")
            .Region("CN")
            .Build();

        Assert.Equal("zh-cmn-Hans-CN", tag.ToString());
        Assert.Equal(["cmn"], tag.Extlangs);
    }

    [Fact]
    public void Build_WithInvalidLanguage_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new LanguageTagBuilder().Language("123"));
    }

    [Fact]
    public void Build_WithInvalidVariant_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new LanguageTagBuilder().Language("de").Variant("!"));
    }

    [Fact]
    public void Build_WithLanguageOnly_ReturnsLanguageTag()
    {
        var tag = new LanguageTagBuilder()
            .Language("de")
            .Build();

        Assert.Equal("de", tag.ToString());
        Assert.Equal("de", tag.Language);
    }

    [Fact]
    public void Build_WithLanguageScriptAndRegion_ReturnsLanguageTag()
    {
        var tag = new LanguageTagBuilder()
            .Language("zh")
            .Script("Hans")
            .Region("CN")
            .Build();

        Assert.Equal("zh-Hans-CN", tag.ToString());
        Assert.Equal("zh", tag.Language);
        Assert.Equal("Hans", tag.Script);
        Assert.Equal("CN", tag.Region);
    }
    
    [Fact]
    public void Build_WithMultipleExtensions_NormalizesExtensionOrder()
    {
        var tag = new LanguageTagBuilder()
            .Language("en")
            .Extension('b', "bbb")
            .Extension('a', "aaa")
            .Build();

        Assert.Equal("en-a-aaa-b-bbb", tag.ToString());
    }

    [Fact]
    public void Build_WithMultipleVariants_PreservesOrder()
    {
        var tag = new LanguageTagBuilder()
            .Language("sl")
            .Variant("rozaj")
            .Variant("biske")
            .Build();

        Assert.Equal("sl-rozaj-biske", tag.ToString());
        Assert.Equal(["rozaj", "biske"], tag.Variants);
    }

    [Fact]
    public void Build_WithoutLanguageOrPrivateUse_ThrowsInvalidOperationException()
    {
        var builder = new LanguageTagBuilder();

        Assert.Throws<InvalidOperationException>(() => builder.Build());
    }

    [Fact]
    public void Build_WithPrivateUse_ReturnsLanguageTag()
    {
        var tag = new LanguageTagBuilder()
            .Language("de")
            .Region("DE")
            .PrivateUse("company", "internal")
            .Build();

        Assert.Equal("de-DE-x-company-internal", tag.ToString());
        Assert.Equal(["company", "internal"], tag.PrivateUse);
        Assert.False(tag.IsPrivateUseOnly);
    }

    [Fact]
    public void Build_WithPrivateUseOnly_ReturnsPrivateUseLanguageTag()
    {
        var tag = new LanguageTagBuilder()
            .PrivateUse("company", "internal")
            .Build();

        Assert.Equal("x-company-internal", tag.ToString());
        Assert.True(tag.IsPrivateUseOnly);
        Assert.Null(tag.Language);
        Assert.Equal(["company", "internal"], tag.PrivateUse);
    }

    [Fact]
    public void Build_WithScriptButWithoutLanguage_ThrowsInvalidOperationException()
    {
        var builder = new LanguageTagBuilder()
            .Script("Latn")
            .PrivateUse("foo");

        Assert.Throws<InvalidOperationException>(() => builder.Build());
    }

    [Fact]
    public void Build_WithVariant_ReturnsLanguageTag()
    {
        var tag = new LanguageTagBuilder()
            .Language("de")
            .Region("DE")
            .Variant("1901")
            .Build();

        Assert.Equal("de-DE-1901", tag.ToString());
        Assert.Equal(["1901"], tag.Variants);
    }
    
    [Fact]
    public void Extension_WithoutSubtags_ThrowsArgumentException()
    {
        var builder = new LanguageTagBuilder();

        Assert.Throws<ArgumentException>(
            () => builder.Extension('u'));
    }

    [Fact]
    public void Extension_WithPrivateUseSingleton_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new LanguageTagBuilder().Language("en").Extension('x', "foo"));
    }

    [Fact]
    public void Extlang_WithInvalidSyntax_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new LanguageTagBuilder().Language("zh").Extlang("Hans"));
    }

    [Fact]
    public void FluentMethods_ReturnSameBuilderInstance()
    {
        var builder = new LanguageTagBuilder();

        Assert.Same(builder, builder.Language("de"));
        Assert.Same(builder, builder.Script("Latn"));
        Assert.Same(builder, builder.Region("DE"));
        Assert.Same(builder, builder.Variant("1901"));
        Assert.Same(builder, builder.Extension('u', "ca", "gregory"));
        Assert.Same(builder, builder.PrivateUse("test"));
    }

    [Fact]
    public void From_AllowsChangingRegion()
    {
        var original = Bcp47Parser.Parse("de-DE");

        var modified = LanguageTagBuilder
            .From(original)
            .Region("AT")
            .Build();

        Assert.Equal("de-AT", modified.ToString());
    }

    [Fact]
    public void From_CopiesExistingLanguageTag()
    {
        var original = Bcp47Parser.Parse("zh-Hans-CN-u-ca-gregory-x-test");

        var copy = LanguageTagBuilder
            .From(original)
            .Build();

        Assert.Equal(original, copy);
        Assert.Equal(original.ToString(), copy.ToString());
    }

    [Fact]
    public void From_GrandfatheredTag_ThrowsArgumentException()
    {
        var original = Bcp47Parser.Parse("i-klingon");

        Assert.Throws<ArgumentException>(() => LanguageTagBuilder.From(original));
    }

    [Fact]
    public void From_PrivateUseOnlyTag_CopiesPrivateUse()
    {
        var original = Bcp47Parser.Parse("x-company-internal");

        var copy = LanguageTagBuilder
            .From(original)
            .Build();

        Assert.Equal("x-company-internal", copy.ToString());
        Assert.True(copy.IsPrivateUseOnly);
    }

    [Fact]
    public void Language_WithSingleLetter_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new LanguageTagBuilder().Language("i"));
    }

    [Fact]
    public void PrivateUse_WithoutSubtags_ThrowsArgumentException()
    {
        var builder = new LanguageTagBuilder();

        Assert.Throws<ArgumentException>(() => builder.PrivateUse());
    }

    [Fact]
    public void Region_WithVariantSyntax_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new LanguageTagBuilder().Language("de").Region("1901"));
    }

    [Fact]
    public void Script_WithRegionSyntax_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new LanguageTagBuilder().Language("de").Script("US"));
    }

    [Fact]
    public void Variant_WithScriptSyntax_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new LanguageTagBuilder().Language("de").Variant("Latn"));
    }
}