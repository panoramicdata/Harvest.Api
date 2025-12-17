// The MIT License(MIT)
//
// Copyright(c) 2016 Steven Atkinson
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
//
// Original license: https://github.com/mrstebo/SnakeCase.JsonNet/blob/master/LICENSE

using System.Text.Json;
using System.Text.RegularExpressions;

namespace Harvest.ContractResolvers;

public partial class SnakeCaseNamingPolicy : JsonNamingPolicy
{
	public static SnakeCaseNamingPolicy Instance { get; } = new();

	public override string ConvertName(string name) => GetSnakeCase(name);

	private static string GetSnakeCase(string input)
	{
		if (string.IsNullOrEmpty(input))
		{
			return input;
		}

		var buffer = input;

		buffer = UpperCaseSequenceRegex().Replace(buffer, "$1_$2");
		buffer = LowerOrDigitToUpperRegex().Replace(buffer, "$1_$2");
		buffer = HyphenRegex().Replace(buffer, "_");
		buffer = WhitespaceRegex().Replace(buffer, "_");
		buffer = MultipleUnderscoreRegex().Replace(buffer, "_");
		return buffer.ToLowerInvariant();
	}

	[GeneratedRegex("([A-Z]+)([A-Z][a-z])")]
	private static partial Regex UpperCaseSequenceRegex();

	[GeneratedRegex(@"([a-z\d])([A-Z])")]
	private static partial Regex LowerOrDigitToUpperRegex();

	[GeneratedRegex("-")]
	private static partial Regex HyphenRegex();

	[GeneratedRegex(@"\s")]
	private static partial Regex WhitespaceRegex();

	[GeneratedRegex("__+")]
	private static partial Regex MultipleUnderscoreRegex();
}