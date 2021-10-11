using System;

namespace Thesaurus.Log
{
	public interface IThesaurusLog
	{
		void Write(string message);

		void Write(string message, Exception exception);
	}
}