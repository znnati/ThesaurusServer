using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Thesaurus
{
	/// <summary>
	/// The db for the Thesaurus words.
	/// </summary>
	[Serializable]
	public class ThesaurusDb
	{
		internal string name;

		//Table forThesaurus words.
		internal IDictionary<string, List<string>> WordsDb { get; private set; }

		internal ThesaurusDb(string dbName)
		{
			name = dbName;
			WordsDb = new Dictionary<string, List<string>>();
		}

		/// <summary>
		/// Load db tables into new instance.
		/// </summary>
		/// <param name="dbName"></param>
		/// <param name="dbSerialized"></param>
		internal ThesaurusDb(string dbName, string dbSerialized)
		{																	
			name = dbName;
			WordsDb = JsonConvert.DeserializeObject<IDictionary<string, List<string>>>(dbSerialized)
								?? new Dictionary<string, List<string>>();
		}


		/// <summary>
		/// Get all words and their synonyms in the dictionary.
		/// </summary>
		/// <returns></returns>
		internal IDictionary<string, List<string>> GetAllWordsAndSynonyms()
		{
			return GetAllWordsAndSynonyms(WordsDb.Count, 0);
		}

		internal string Serialize()
		{
			return JsonConvert.SerializeObject(WordsDb);
		}

		/// <summary>
		/// Get <see cref="take"/> words and their synonyms in the dictionary and skip <see cref="skip"/>.
		/// </summary>
		/// <returns></returns>
		internal IDictionary<string, List<string>> GetAllWordsAndSynonyms(int take, int skip)
		{
			if (take < 0)
			{
				take = WordsDb.Keys.Count;
			}
									
			return WordsDb.Skip(skip).Take(take).ToDictionary(p => p.Key, p => p.Value);

		}

		/// <summary>
		/// Find synonyms for the given <see cref="word"/>
		/// </summary>
		/// <param name="word"></param>
		/// <returns>List of synonyms</returns>
		internal IList<string> Find(string word)
		{
			if (WordsDb.TryGetValue(word, out List<string> list))
				return list;

			WordsDb.TryGetValue(word.ToLower(), out list);
			return list;
		}

		/// <summary>
		/// Insert the given synonyms in the words db
		/// </summary>
		/// <param name="word"></param>
		/// <param name="synonyms"></param>
		/// <returns></returns>
		internal bool InsertOrUpdate(string word, IList<string> synonyms)
		{
			if(WordsDb.TryAdd(word.ToLower(), synonyms.ToList()))
				return true;

			foreach (string synonym in synonyms.Where(s => !WordsDb[word].Contains(s)))
			{
				WordsDb[word].Add(synonym.ToLower());
			}

			return true;
		}		
	}
}
