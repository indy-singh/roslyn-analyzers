﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Threading.Tasks;
using MetaCompilation.Analyzers.UnitTests;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Xunit;

namespace Roslyn.Diagnostics.Analyzers.UnitTests
{
    public class NumberCommentsRefactoringTests : CodeRefactoringVerifier
    {
        protected override CodeRefactoringProvider GetCodeRefactoringProvider()
            => new NumberCommentslRefactoring();

        [Fact]
        public async Task TestAsync()
        {
            const string source = @"
public class C
{
    string s = @""
[||]class D { } //
"";
}";
            const string fixedSource = @"
public class C
{
    string s = @""
class D { } // 1
"";
}";
            await VerifyRefactoringAsync(source, fixedSource);
        }

        [Fact]
        public async Task CSharp_VerifyFix_WithTriviaAsync()
        {
            const string source = @"
public class C
{
    string s =
[||]/*before*/ @""
class D { } //
"" /*after*/ ;
}";
            const string fixedSource = @"
public class C
{
    string s =
/*before*/ @""
class D { } // 1
"" /*after*/ ;
}";
            await VerifyRefactoringAsync(source, fixedSource);
        }

        [Fact]
        public async Task CSharp_VerifyFix_NonNumberCommentsLeftAloneAsync()
        {
            const string source = @"
public class C
{
    string s = @""
[||]//
class D //
{//
} // test
"";
}";
            const string fixedSource = @"
public class C
{
    string s = @""
//
class D // 1
{//
} // test
"";
}";
            await VerifyRefactoringAsync(source, fixedSource);
        }

        [Fact]
        public async Task CSharp_VerifyFix_MultipleCommasAsync()
        {
            const string source = @"
public class C
{
    string s = @""
[||]class D //1
{ //,
} //
"";
}";
            const string fixedSource = @"
public class C
{
    string s = @""
class D // 1
{ // 2, 3
} // 4
"";
}";
            await VerifyRefactoringAsync(source, fixedSource);
        }

        [Fact]
        public async Task CSharp_VerifyFix_LastLineAsync()
        {
            const string source = @"
public class C
{
    string s = @""[||]class D { } //"";
}";
            const string fixedSource = @"
public class C
{
    string s = @""class D { } // 1"";
}";
            await VerifyRefactoringAsync(source, fixedSource);
        }

        [Fact]
        public async Task CountOverTen()
        {
            const string source = @"
public class C
{
    string s = @""
[||]class D { } // ,,,,,,,,,,,,
"";
}";
            const string fixedSource = @"
public class C
{
    string s = @""
class D { } // 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13
"";
}";
            await VerifyRefactoringAsync(source, fixedSource);
        }

        [Fact]
        public async Task EmptyNumberIsImproper()
        {
            const string source = @"
public class C
{
    string s = @""
[||]class D // 1
{ // 2, 3
} //
"";
}";

            const string fixedSource = @"
public class C
{
    string s = @""
class D // 1
{ // 2, 3
} // 4
"";
}";

            await VerifyRefactoringAsync(source, fixedSource);
        }

        [Fact]
        public async Task EmptyNumberBeforeCommaIsImproper()
        {
            const string source = @"
public class C
{
    string s = @""
[||]class C // 1
{ // , 3
}
"";
}";

            const string fixedSource = @"
public class C
{
    string s = @""
class C // 1
{ // 2, 3
}
"";
}";

            await VerifyRefactoringAsync(source, fixedSource);
        }

        [Fact]
        public async Task EmptyCommentOnEmptyLineIsProper()
        {
            const string source = @"
public class C
{
    string s = @""
// much stuff
//
// more stuff
[||]class C // 1
{ // 2, 3
} //
"";
}";

            const string fixedSource = @"
public class C
{
    string s = @""
// much stuff
//
// more stuff
class C // 1
{ // 2, 3
} // 4
"";
}";

            await VerifyRefactoringAsync(source, fixedSource);
        }

        [Fact]
        public async Task LastLineIsAnalyzed()
        {
            const string source = @"
public class C
{
    string s = @""[||]class D { } //"";
}";

            const string fixedSource = @"
public class C
{
    string s = @""class D { } // 1"";
}";

            await VerifyRefactoringAsync(source, fixedSource);
        }
    }
}
