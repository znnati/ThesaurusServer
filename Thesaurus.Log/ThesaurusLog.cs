using System;
using log4net;

namespace Thesaurus.Log
{
	public class ThesaurusLog : IThesaurusLog
	{
		private readonly ILog log;

		public ThesaurusLog(string name)
		{
			log = LogManager.GetLogger(name);
		}

		public void Write(string message)
		{
			log.Info(message);
		}

		public void Write(string message, Exception exception)
		{
			log.Info(message, exception);
		}
	}
}
