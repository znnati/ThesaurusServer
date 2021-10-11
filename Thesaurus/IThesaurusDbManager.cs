using System.Collections.Generic;
using System.Threading.Tasks;

namespace Thesaurus
{
	public interface IThesaurusDbManager
	{

		/// <summary>
		/// Get all words and their synonyms in the dictionary.
		/// </summary>
		/// <returns></returns>
		public IDictionary<string, List<string>> GetAllWordsAndSynonyms();


		/// <summary>
		/// Get <see cref="take"/> words and their synonyms in the dictionary and skip <see cref="skip"/>.
		/// </summary>
		/// <returns></returns>
		public IDictionary<string, List<string>> GetAllWordsAndSynonyms(int take, int skip);

		/// <summary>
		/// Find synonyms for the given <see cref="word"/>
		/// </summary>
		/// <param name="word"></param>
		/// <returns>List of synonyms</returns>
		public IList<string> Find(string word);

		/// <summary>
		/// Insert the given synonyms in the words db
		/// </summary>
		/// <param name="word"></param>
		/// <param name="synonyms"></param>
		/// <returns></returns>
		public bool InsertOrUpdate(string word, IList<string> synonyms);


		/// <summary>
		/// Save the current db	in the context
		/// </summary>
		/// <returns></returns>
		public Task<bool> SaveAsync();

		/// <summary>
		/// Load the <see cref="dbName"/> db in the context. (use GetContext() to get access to the db afterwards)
		/// </summary>
		/// <returns></returns>
		public Task<bool> LoadAsync(string dbName);


		/// <summary>
		/// Create a database name <see cref="dbName"/>
		/// </summary>
		/// <param name="dbName"></param>
		/// <returns>Status</returns>
		public Task<bool> CreateAsync(string dbName);
	}
}