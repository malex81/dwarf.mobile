using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Dwarf.Framework.SystemExtension;

public static partial class StringExtensions
{
	/// <summary>
	/// Analog of string.IsNullOrEmpty to invoke as extension method.
	/// </summary>
	/// <param name="str">String to check.</param>
	/// <returns>True if string is null or empty.</returns>
	public static bool IsNullOrEmpty(this string str) => string.IsNullOrEmpty(str);

	#region Uppercase/Lowercase

	/// <summary>
	/// Lowercases first letter of specified string.
	/// </summary>
	/// <param name="str">String to lowercase.</param>
	/// <returns>String.</returns>
	public static string LowercaseFirst(this string str)
	{
		if (string.IsNullOrEmpty(str))
			return string.Empty;
		char[] chars = str.ToCharArray();
		chars[0] = char.ToLower(chars[0]);
		return new string(chars);
	}

	/// <summary>
	/// Lowercases last letter of specified string.
	/// </summary>
	/// <param name="str">String to lowercase.</param>
	/// <returns>String.</returns>
	public static string LowercaseLast(this string str)
	{
		if (string.IsNullOrEmpty(str))
			return string.Empty;
		char[] chars = str.ToCharArray();
		chars[^1] = char.ToLower(chars[^1]);
		return new string(chars);
	}

	/// <summary>
	/// Capitalizes first letter of specified string.
	/// </summary>
	/// <param name="str">String to capitalize.</param>
	/// <returns>String.</returns>
	public static string UppercaseFirst(this string str)
	{
		if (string.IsNullOrEmpty(str))
			return string.Empty;
		char[] chars = str.ToCharArray();
		chars[0] = char.ToUpper(chars[0]);
		return new string(chars);
	}

	/// <summary>
	/// Capitalizes last letter of specified string.
	/// </summary>
	/// <param name="str">String to capitalize.</param>
	/// <returns>String.</returns>
	public static string UppercaseLast(this string str)
	{
		if (string.IsNullOrEmpty(str))
			return string.Empty;
		char[] chars = str.ToCharArray();
		chars[^1] = char.ToUpper(chars[^1]);
		return new string(chars);
	}

	public static string ToProperName(this string source)
	{
		StringBuilder sb = new(source.Trim());
		bool isToken = false;
		for (int i = 0; i < sb.Length; ++i)
		{
			char c = sb[i];
			if (char.IsLetter(c) || c == '\'')
			{
				sb[i] = !isToken ? Char.ToUpper(c) : Char.ToLower(c);
				isToken = true;
			}
			else
				isToken = false;
		}
		return sb.ToString();
	}

	#endregion

	/// <summary>
	/// Убирает "пустые" символы вначале и в конце строки. Если, при этом, получилась пустая строка,
	/// возвращает null. <seealso cref="string.Trim()"/>
	/// </summary>
	/// <param name="src">исходная строка</param>
	/// <returns>"Подрезанная" строка</returns>
	public static string? FullTrim(this string src)
	{
		var res = src == null ? "" : src.Trim();
		return res.Length > 0 ? res : null;
	}

	#region Добавлене подстрок

	/// <summary>
	/// собирает строку вида "<paramref name="src"/> + <paramref name="separator"/> + <paramref name="word"/>"
	/// При этом, парметры проверяются на пустую строку
	/// </summary>
	/// <param name="src">начальная строка</param>
	/// <param name="word">строка, которая будет добавлена к начальной</param>
	/// <param name="separator">разделитель между строками <paramref name="src"/> и <paramref name="word"/></param>
	/// <returns></returns>
	public static string AddWord(this string src, string word, string separator)
	{
		return string.IsNullOrEmpty(word) ? src
			: string.IsNullOrEmpty(src) ? word
			: string.Format("{0}{2}{1}", src, word, separator);
	}
	/// <summary>
	/// 
	/// </summary>
	/// <param name="src"></param>
	/// <param name="format"></param>
	/// <param name="separator"></param>
	/// <param name="words"></param>
	/// <returns></returns>
	public static string AddWord(this string src, string format, string separator, params object[] words)
	{
		string res = src;
		if (words.Length > 0 && Array.FindIndex(words, w => w == null || w is string _w && string.IsNullOrEmpty(_w)) == -1)
		{
			string word = string.Format(format, words);
			if (string.IsNullOrEmpty(src))
				res = word;
			else
				res += separator + word;
		}
		return res;
	}

	public static StringBuilder AddWord(this StringBuilder src, string word, string separator)
	{
		ArgumentNullException.ThrowIfNull(src);
		return src.Append(string.IsNullOrEmpty(word) ? ""
							: src.Length == 0 ? word
							: string.Format("{1}{0}", word, separator));
	}
	/// <summary>
	/// Similar to AppendLine method, but takes variable number of parameters.
	/// </summary>
	/// <param name="sb">String builder.</param>
	/// <param name="strings">Strings to append.</param>
	public static StringBuilder AppendLines(this StringBuilder sb, IEnumerable<string> strings)
	{
		foreach (var str in strings)
			sb.AppendLine(str);
		return sb;
	}

	#endregion

	#region Поиск и сравнение строк

	static bool CompareWithPart(string source, string part)
	{
		if (source.Length != part.Length) return false;
		for (int i = 0; i < source.Length; i++)
			if (source[i] != part[i] && part[i] != '?')
				return false;
		return true;
	}

	static bool ConsumePart(ref string source, string part, bool first, bool last)
	{
		if (part == string.Empty)
		{
			if (last) source = "";
			return !(first && last);
		}
		if (last)
		{
			var subStr = first ? source : source[Math.Max(0, source.Length - part.Length)..];
			source = "";
			return CompareWithPart(subStr, part);
		}
		if (first)
		{
			var len = Math.Min(part.Length, source.Length);
			var subStr = source[..len];
			source = source[len..];
			return CompareWithPart(subStr, part);
		}
		for (int i = 0; i <= source.Length - part.Length; i++)
			if (CompareWithPart(source.Substring(i, part.Length), part))
			{
				source = source[(i + part.Length)..];
				return true;
			}
		return false;
	}

	public static bool CompareWildcard(this string source, string mask)
	{
		ArgumentNullException.ThrowIfNull(source);
		ArgumentNullException.ThrowIfNull(mask);
		var parts = mask.Split('*');
		for (int i = 0; i < parts.Length; i++)
			if (!ConsumePart(ref source, parts[i], i == 0, i == parts.Length - 1))
				return false;
		return true;
	}

	public static bool CompareWildcard(this string source, string mask, bool ignoreCase)
	{
		ArgumentNullException.ThrowIfNull(source);
		ArgumentNullException.ThrowIfNull(mask);
		return ignoreCase
			? source.ToLower().CompareWildcard(mask.ToLower())
			: source.CompareWildcard(mask);
	}

	#endregion

	#region Compute hash

	public static string MD5HashUTF8(this string input)
	{
		using (var md5 = MD5.Create())
		{
			byte[] data = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
			return Encoding.UTF8.GetString(data);
		}
	}

	public static string MD5HashBase64(this string source)
	{
		using (var md5 = MD5.Create())
		{
			byte[] hash = md5.ComputeHash(Encoding.UTF8.GetBytes(source));
			return Convert.ToBase64String(hash);
		}
	}

	public static Guid MD5HashGuid(this string source)
	{
		using (var md5 = MD5.Create())
		{
			byte[] hash = md5.ComputeHash(Encoding.UTF8.GetBytes(source));
			return new Guid(hash);
		}
	}

	#endregion

	public static string ReplaceCaseIgnoring(this string input, string pattern, string replacement)
	{
		return Regex.Replace(input, pattern, replacement, RegexOptions.IgnoreCase);
	}

	public static string ReplaceBraces(this string format, Func<string, string> convert)
	{
		//Regex rx = new Regex( @"\{[\w_]+[\w_0-9]*\}" ); // \w эквивалентно [A-Za-z0-9_], а не [A-Za-z_]
		var rx = BracesRegex();
		return rx.Replace(format, m =>
		{
			var str = m.Value;
			var prm = str[1..^1];
			return convert(prm);
		});
	}

	[GeneratedRegex(@"\{[_a-zA-Z][_a-zA-Z0-9]*\}")]
	private static partial Regex BracesRegex();
}
