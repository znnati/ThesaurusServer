using Thesaurus.Log;

namespace ThesaurusApi
{
	public class ThesaurusLogSentry	: ThesaurusLog, IThesaurusLog
	{
		public ThesaurusLogSentry() : base("console")	 // Send to console for now
		{
		}
	}
}
