using Brigadier.NET;
using Brigadier.NET.Exceptions;
using SCKRM.Text;

namespace SCKRM.Command
{
	public class BuiltInExceptions : IBuiltInExceptionProvider
	{
		public Dynamic2CommandExceptionType DoubleTooLow()
		{
			return new Dynamic2CommandExceptionType((found, min) =>
            {
				string message = CommandLanguage.SearchLanguage("double_too_small").Replace("%found%", found.ToString()).Replace("%min%", min.ToString());
				return new LiteralMessage(message);
            });
		}

		public Dynamic2CommandExceptionType DoubleTooHigh()
		{
			return new Dynamic2CommandExceptionType((found, max) =>
			{
				string message = CommandLanguage.SearchLanguage("double_too_big").Replace("%found%", found.ToString()).Replace("%max%", max.ToString());
				return new LiteralMessage(message);
			});
		}

		public Dynamic2CommandExceptionType FloatTooLow()
		{
			return new Dynamic2CommandExceptionType((found, min) =>
			{
				string message = CommandLanguage.SearchLanguage("float_too_small").Replace("%found%", found.ToString()).Replace("%min%", min.ToString());
				return new LiteralMessage(message);
			});
		}

		public Dynamic2CommandExceptionType FloatTooHigh()
		{
			return new Dynamic2CommandExceptionType((found, max) =>
			{
				string message = CommandLanguage.SearchLanguage("float_too_big").Replace("%found%", found.ToString()).Replace("%max%", max.ToString());
				return new LiteralMessage(message);
			});
		}

		public Dynamic2CommandExceptionType IntegerTooLow()
		{
			return new Dynamic2CommandExceptionType((found, min) =>
			{
				string message = CommandLanguage.SearchLanguage("integer_too_small").Replace("%found%", found.ToString()).Replace("%min%", min.ToString());
				return new LiteralMessage(message);
			});
		}

		public Dynamic2CommandExceptionType IntegerTooHigh()
		{
			return new Dynamic2CommandExceptionType((found, max) =>
			{
				string message = CommandLanguage.SearchLanguage("integer_too_big").Replace("%found%", found.ToString()).Replace("%max%", max.ToString());
				return new LiteralMessage(message);
			});
		}

		public Dynamic2CommandExceptionType LongTooLow()
		{
			return new Dynamic2CommandExceptionType((found, min) =>
			{
				string message = CommandLanguage.SearchLanguage("long_too_small").Replace("%found%", found.ToString()).Replace("%min%", min.ToString());
				return new LiteralMessage(message);
			});
		}

		public Dynamic2CommandExceptionType LongTooHigh()
		{
			return new Dynamic2CommandExceptionType((found, max) =>
			{
				string message = CommandLanguage.SearchLanguage("long_too_big").Replace("%found%", found.ToString()).Replace("%max%", max.ToString());
				return new LiteralMessage(message);
			});
		}

		public DynamicCommandExceptionType LiteralIncorrect()
		{
			return new DynamicCommandExceptionType((expected) =>
			{
				string message = CommandLanguage.SearchLanguage("literrl_incorrect").Replace("%expected%", expected.ToString());
				return new LiteralMessage(message);
			});
		}

		public SimpleCommandExceptionType ReaderExpectedStartOfQuote()
		{
			string message = CommandLanguage.SearchLanguage("reader_expected_start_of_quote");
			return new SimpleCommandExceptionType(new LiteralMessage(message));
		}

		public SimpleCommandExceptionType ReaderExpectedEndOfQuote()
		{
			string message = CommandLanguage.SearchLanguage("reader_expected_end_of_quote");
			return new SimpleCommandExceptionType(new LiteralMessage(message));
		}

		public DynamicCommandExceptionType ReaderInvalidEscape()
		{
			return new DynamicCommandExceptionType((character) =>
			{
				string message = CommandLanguage.SearchLanguage("reader_invalid_escape").Replace("%character%", character.ToString());
				return new LiteralMessage(message);
			});
		}

		public DynamicCommandExceptionType ReaderInvalidBool()
		{
			return new DynamicCommandExceptionType((value) =>
			{
				string message = CommandLanguage.SearchLanguage("reader_invalid_bool").Replace("%value%", value.ToString());
				return new LiteralMessage(message);
			});
		}

		public DynamicCommandExceptionType ReaderInvalidInt()
		{
			return new DynamicCommandExceptionType((value) =>
			{
				string message = CommandLanguage.SearchLanguage("reader_invalid_int").Replace("%value%", value.ToString());
				return new LiteralMessage(message);
			});
		}

		public SimpleCommandExceptionType ReaderExpectedInt()
		{
			string message = CommandLanguage.SearchLanguage("reader_expected_int");
			return new SimpleCommandExceptionType(new LiteralMessage(message));
		}

		public DynamicCommandExceptionType ReaderInvalidLong()
		{
			return new DynamicCommandExceptionType((value) =>
			{
				string message = CommandLanguage.SearchLanguage("reader_invalid_long").Replace("%value%", value.ToString());
				return new LiteralMessage(message);
			});
		}

		public SimpleCommandExceptionType ReaderExpectedLong()
		{
			string message = CommandLanguage.SearchLanguage("reader_expected_long");
			return new SimpleCommandExceptionType(new LiteralMessage(message));
		}

		public DynamicCommandExceptionType ReaderInvalidDouble()
		{
			return new DynamicCommandExceptionType((value) =>
			{
				string message = CommandLanguage.SearchLanguage("reader_invalid_double").Replace("%value%", value.ToString());
				return new LiteralMessage(message);
			});
		}

		public SimpleCommandExceptionType ReaderExpectedDouble()
		{
			string message = CommandLanguage.SearchLanguage("reader_expected_double");
			return new SimpleCommandExceptionType(new LiteralMessage(message));
		}

		public DynamicCommandExceptionType ReaderInvalidFloat()
		{
			return new DynamicCommandExceptionType((value) =>
			{
				string message = CommandLanguage.SearchLanguage("reader_invalid_float").Replace("%value%", value.ToString());
				return new LiteralMessage(message);
			});
		}

		public SimpleCommandExceptionType ReaderExpectedFloat()
		{
			string message = CommandLanguage.SearchLanguage("reader_expected_float");
			return new SimpleCommandExceptionType(new LiteralMessage(message));
		}

		public SimpleCommandExceptionType ReaderExpectedBool()
		{
			string message = CommandLanguage.SearchLanguage("reader_expected_bool");
			return new SimpleCommandExceptionType(new LiteralMessage(message));
		}

		public DynamicCommandExceptionType ReaderExpectedSymbol()
		{
			return new DynamicCommandExceptionType((symbol) =>
			{
				string message = CommandLanguage.SearchLanguage("reader_expected_symbol").Replace("%symbol%", symbol.ToString());
				return new LiteralMessage(message);
			});
		}

		public SimpleCommandExceptionType DispatcherUnknownCommand()
		{
			string message = CommandLanguage.SearchLanguage("dispatcher_unknown_command");
			return new SimpleCommandExceptionType(new LiteralMessage(message));
		}

		public SimpleCommandExceptionType DispatcherUnknownArgument()
		{
			string message = CommandLanguage.SearchLanguage("dispatcher_unknown_argument");
			return new SimpleCommandExceptionType(new LiteralMessage(message));
		}

		public SimpleCommandExceptionType DispatcherExpectedArgumentSeparator()
		{
			string message = CommandLanguage.SearchLanguage("dispatcher_expected_argument_separator");
			return new SimpleCommandExceptionType(new LiteralMessage(message));
		}

		public DynamicCommandExceptionType DispatcherParseException()
		{
			return new DynamicCommandExceptionType((message) =>
			{
				string message2 = CommandLanguage.SearchLanguage("dispatcher_parse_exception").Replace("%message%", message.ToString());
				return new LiteralMessage(message2);
			});
		}
	}

	public static class BuiltInExceptionsExpansion
	{
#pragma warning disable IDE0060 // 사용하지 않는 매개 변수를 제거하세요.
		public static Dynamic2CommandExceptionType ColonTooMany(this IBuiltInExceptionProvider e)
		{
			return new Dynamic2CommandExceptionType((found, max) =>
			{
				string message = CommandLanguage.SearchLanguage("colon_too_many").Replace("%found%", found.ToString()).Replace("%max%", max.ToString());
				return new LiteralMessage(message);
			});
		}

		public static SimpleCommandExceptionType InvalidPosSwizzle(this IBuiltInExceptionProvider e)
		{
			string message = CommandLanguage.SearchLanguage("reader_invalid_pos_swizzle");
			return new SimpleCommandExceptionType(new LiteralMessage(message));
		}

		public static SimpleCommandExceptionType WorldLocalPosMixed(this IBuiltInExceptionProvider e)
		{
			string message = CommandLanguage.SearchLanguage("world_local_pos_mixed");
			return new SimpleCommandExceptionType(new LiteralMessage(message));
		}
#pragma warning restore IDE0060 // 사용하지 않는 매개 변수를 제거하세요.

		public static CommandSyntaxException GetCustomException(this CommandSyntaxException exception)
		{
			exception.CustomMessage = GetCustomExceptionMessage(exception);
			return exception;
		}

		static readonly FastString exceptionFastString = new FastString();
		public static string GetCustomExceptionMessage(this CommandSyntaxException exception)
		{
			string here = CommandLanguage.SearchLanguage("context.here");
			string parseError = CommandLanguage.SearchLanguage("context.parse_error");
			string message = exception.RawMessage().String;

			parseError = parseError.Replace("%message%", message);
			parseError = parseError.Replace("%pos%", exception.Cursor.ToString());

			if (exception.Input == null || exception.Cursor < 0)
				return message;

			exceptionFastString.Clear();
			int num = exception.Input.Length.Min(exception.Cursor);
			if (num > CommandSyntaxException.ContextAmount)
				exceptionFastString.Append("...");

			int num2 = 0.Max(num - CommandSyntaxException.ContextAmount);
			exceptionFastString.Append(exception.Input.Substring(num2, num - num2));
			exceptionFastString.Append(here);

			return parseError.Replace("%text%", exceptionFastString.ToString());
		}
	}
}
