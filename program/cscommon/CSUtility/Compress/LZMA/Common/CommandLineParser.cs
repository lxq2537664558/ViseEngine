// CommandLineParser.cs

using System;
using System.Collections;

namespace SevenZip.CommandLineParser
{
    /// <summary>
    /// 转换类型枚举
    /// </summary>
	internal enum SwitchType
	{
		Simple,
		PostMinus,
		LimitedPostString,
		UnLimitedPostString,
		PostChar
	}
    /// <summary>
    /// 转换框类
    /// </summary>
    internal class SwitchForm
	{
        /// <summary>
        /// 框架ID
        /// </summary>
		public string IDString;
        /// <summary>
        /// 转换类型
        /// </summary>
		public SwitchType Type;
        /// <summary>
        /// 是否叠加
        /// </summary>
		public bool Multi;
        /// <summary>
        /// 最小长度
        /// </summary>
		public int MinLen;
        /// <summary>
        /// 最大长度
        /// </summary>
		public int MaxLen;
        /// <summary>
        /// 发送的字符
        /// </summary>
		public string PostCharSet;
        /// <summary>
        /// 带参构造函数
        /// </summary>
        /// <param name="idString">对象ID</param>
        /// <param name="type">转换类型</param>
        /// <param name="multi">是否叠加</param>
        /// <param name="minLen">最小长度</param>
        /// <param name="maxLen">最大长度</param>
        /// <param name="postCharSet">发送的字符</param>
		public SwitchForm(string idString, SwitchType type, bool multi,
			int minLen, int maxLen, string postCharSet)
		{
			IDString = idString;
			Type = type;
			Multi = multi;
			MinLen = minLen;
			MaxLen = maxLen;
			PostCharSet = postCharSet;
		}
        /// <summary>
        /// 带参构造函数
        /// </summary>
        /// <param name="idString">ID字符</param>
        /// <param name="type">切换类型</param>
        /// <param name="multi">是否叠加</param>
        /// <param name="minLen">最小长度</param>
		public SwitchForm(string idString, SwitchType type, bool multi, int minLen):
			this(idString, type, multi, minLen, 0, "")
		{
		}
        /// <summary>
        /// 带参构造函数
        /// </summary>
        /// <param name="idString">对象ID</param>
        /// <param name="type">切换类型</param>
        /// <param name="multi">是否叠加</param>
		public SwitchForm(string idString, SwitchType type, bool multi):
			this(idString, type, multi, 0)
		{
		}
	}
    /// <summary>
    /// 转换结果类
    /// </summary>
    internal class SwitchResult
	{
        /// <summary>
        /// 是否有结果
        /// </summary>
		public bool ThereIs;
        /// <summary>
        /// 是否区分正负
        /// </summary>
		public bool WithMinus;
        /// <summary>
        /// 发送的字符
        /// </summary>
		public ArrayList PostStrings = new ArrayList();
        /// <summary>
        /// 字符索引
        /// </summary>
		public int PostCharIndex;
        /// <summary>
        /// 构造函数
        /// </summary>
		public SwitchResult()
		{
			ThereIs = false;
		}
	}
    /// <summary>
    /// 解析类
    /// </summary>
    internal class Parser
	{
        /// <summary>
        /// 空的转换字符
        /// </summary>
		public ArrayList NonSwitchStrings = new ArrayList();
		SwitchResult[] _switches;
        /// <summary>
        /// 带参构造函数
        /// </summary>
        /// <param name="numSwitches">转换索引</param>
		public Parser(int numSwitches)
		{
			_switches = new SwitchResult[numSwitches];
			for (int i = 0; i < numSwitches; i++)
				_switches[i] = new SwitchResult();
		}
        /// <summary>
        /// 解析字符
        /// </summary>
        /// <param name="srcString">需要解析的源字符</param>
        /// <param name="switchForms">转换框架</param>
        /// <returns>解析成功返回true，否则返回false</returns>
		bool ParseString(string srcString, SwitchForm[] switchForms)
		{
			int len = srcString.Length;
			if (len == 0)
				return false;
			int pos = 0;
			if (!IsItSwitchChar(srcString[pos]))
				return false;
			while (pos < len)
			{
				if (IsItSwitchChar(srcString[pos]))
					pos++;
				const int kNoLen = -1;
				int matchedSwitchIndex = 0;
				int maxLen = kNoLen;
				for (int switchIndex = 0; switchIndex < _switches.Length; switchIndex++)
				{
					int switchLen = switchForms[switchIndex].IDString.Length;
					if (switchLen <= maxLen || pos + switchLen > len)
						continue;
					if (String.Compare(switchForms[switchIndex].IDString, 0,
							srcString, pos, switchLen, true) == 0)
					{
						matchedSwitchIndex = switchIndex;
						maxLen = switchLen;
					}
				}
				if (maxLen == kNoLen)
					throw new Exception("maxLen == kNoLen");
				SwitchResult matchedSwitch = _switches[matchedSwitchIndex];
				SwitchForm switchForm = switchForms[matchedSwitchIndex];
				if ((!switchForm.Multi) && matchedSwitch.ThereIs)
					throw new Exception("switch must be single");
				matchedSwitch.ThereIs = true;
				pos += maxLen;
				int tailSize = len - pos;
				SwitchType type = switchForm.Type;
				switch (type)
				{
					case SwitchType.PostMinus:
						{
							if (tailSize == 0)
								matchedSwitch.WithMinus = false;
							else
							{
								matchedSwitch.WithMinus = (srcString[pos] == kSwitchMinus);
								if (matchedSwitch.WithMinus)
									pos++;
							}
							break;
						}
					case SwitchType.PostChar:
						{
							if (tailSize < switchForm.MinLen)
								throw new Exception("switch is not full");
							string charSet = switchForm.PostCharSet;
							const int kEmptyCharValue = -1;
							if (tailSize == 0)
								matchedSwitch.PostCharIndex = kEmptyCharValue;
							else
							{
								int index = charSet.IndexOf(srcString[pos]);
								if (index < 0)
									matchedSwitch.PostCharIndex = kEmptyCharValue;
								else
								{
									matchedSwitch.PostCharIndex = index;
									pos++;
								}
							}
							break;
						}
					case SwitchType.LimitedPostString:
					case SwitchType.UnLimitedPostString:
						{
							int minLen = switchForm.MinLen;
							if (tailSize < minLen)
								throw new Exception("switch is not full");
							if (type == SwitchType.UnLimitedPostString)
							{
								matchedSwitch.PostStrings.Add(srcString.Substring(pos));
								return true;
							}
							String stringSwitch = srcString.Substring(pos, minLen);
							pos += minLen;
							for (int i = minLen; i < switchForm.MaxLen && pos < len; i++, pos++)
							{
								char c = srcString[pos];
								if (IsItSwitchChar(c))
									break;
								stringSwitch += c;
							}
							matchedSwitch.PostStrings.Add(stringSwitch);
							break;
						}
				}
			}
			return true;

		}
        /// <summary>
        /// 
        /// </summary>
        /// <param name="switchForms"></param>
        /// <param name="commandStrings"></param>
		public void ParseStrings(SwitchForm[] switchForms, string[] commandStrings)
		{
			int numCommandStrings = commandStrings.Length;
			bool stopSwitch = false;
			for (int i = 0; i < numCommandStrings; i++)
			{
				string s = commandStrings[i];
				if (stopSwitch)
					NonSwitchStrings.Add(s);
				else
					if (s == kStopSwitchParsing)
					stopSwitch = true;
				else
					if (!ParseString(s, switchForms))
					NonSwitchStrings.Add(s);
			}
		}
        /// <summary>
        /// 操作符重载
        /// </summary>
        /// <param name="index">索引值</param>
        /// <returns>返回转换结果</returns>
		public SwitchResult this[int index] { get { return _switches[index]; } }
        /// <summary>
        /// 解析命令
        /// </summary>
        /// <param name="commandForms">命令控件</param>
        /// <param name="commandString">命令字符</param>
        /// <param name="postString">输出的字符</param>
        /// <returns>返回索引值</returns>
		public static int ParseCommand(CommandForm[] commandForms, string commandString,
			out string postString)
		{
			for (int i = 0; i < commandForms.Length; i++)
			{
				string id = commandForms[i].IDString;
				if (commandForms[i].PostStringMode)
				{
					if (commandString.IndexOf(id) == 0)
					{
						postString = commandString.Substring(id.Length);
						return i;
					}
				}
				else
					if (commandString == id)
				{
					postString = "";
					return i;
				}
			}
			postString = "";
			return -1;
		}

		static bool ParseSubCharsCommand(int numForms, CommandSubCharsSet[] forms,
			string commandString, ArrayList indices)
		{
			indices.Clear();
			int numUsedChars = 0;
			for (int i = 0; i < numForms; i++)
			{
				CommandSubCharsSet charsSet = forms[i];
				int currentIndex = -1;
				int len = charsSet.Chars.Length;
				for (int j = 0; j < len; j++)
				{
					char c = charsSet.Chars[j];
					int newIndex = commandString.IndexOf(c);
					if (newIndex >= 0)
					{
						if (currentIndex >= 0)
							return false;
						if (commandString.IndexOf(c, newIndex + 1) >= 0)
							return false;
						currentIndex = j;
						numUsedChars++;
					}
				}
				if (currentIndex == -1 && !charsSet.EmptyAllowed)
					return false;
				indices.Add(currentIndex);
			}
			return (numUsedChars == commandString.Length);
		}
		const char kSwitchID1 = '-';
		const char kSwitchID2 = '/';

		const char kSwitchMinus = '-';
		const string kStopSwitchParsing = "--";

		static bool IsItSwitchChar(char c)
		{
			return (c == kSwitchID1 || c == kSwitchID2);
		}
	}
    /// <summary>
    /// 命令控件
    /// </summary>
    internal class CommandForm
	{
        /// <summary>
        /// 对象ID
        /// </summary>
		public string IDString = "";
        /// <summary>
        /// 是否输出字符
        /// </summary>
		public bool PostStringMode = false;
        /// <summary>
        /// 带参构造函数
        /// </summary>
        /// <param name="idString">对象ID</param>
        /// <param name="postStringMode">是否输出字符</param>
		public CommandForm(string idString, bool postStringMode)
		{
			IDString = idString;
			PostStringMode = postStringMode;
		}
	}
    /// <summary>
    /// 命令行减字符设置类
    /// </summary>
	class CommandSubCharsSet
	{
        /// <summary>
        /// 字符串
        /// </summary>
		public string Chars = "";
        /// <summary>
        /// 是否接受空字符
        /// </summary>
		public bool EmptyAllowed = false;
	}
}
