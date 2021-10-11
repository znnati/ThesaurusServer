using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Thesaurus.Log;

namespace Thesaurus
{
	public class ThesaurusDbFactory	 : IThesaurusDbManager, IDisposable
	{
		private readonly IThesaurusLog log;	
		private ThesaurusDb thesaurusDb { get; set; }

		public ThesaurusDbFactory(IThesaurusLog log)
		{
			this.log = log;
		}
			
		public async Task<bool> CreateAsync(string dbName)
		{
			try
			{												
				thesaurusDb = new ThesaurusDb(dbName);

				await SaveAsync();

				thesaurusDb = null;

				return true;
			}
			catch (Exception e)
			{
				log.Write($"Db create failed for {dbName}.", e);
				return false;
			}
		}
						
		public async Task<bool> LoadAsync(string dbName)
		{
			try
			{	 
				await using var stream = new FileStream($"{dbName}.json", FileMode.OpenOrCreate);
				using var streamReader = new StreamReader(stream, Encoding.UTF8);
				string wordsSerialized = await streamReader.ReadToEndAsync();

				thesaurusDb = new ThesaurusDb(dbName, wordsSerialized);
				stream.Close();

				if (thesaurusDb != null) 
					return true;

				thesaurusDb = new ThesaurusDb(dbName);
				return true;
			}
			catch (Exception e)
			{
				log.Write("Db load failed.", e);
				return false;
			}
		}

		public IDictionary<string, List<string>> GetAllWordsAndSynonyms()
		{
			return thesaurusDb.GetAllWordsAndSynonyms();
		}

		public IDictionary<string, List<string>> GetAllWordsAndSynonyms(int take, int skip)
		{
			return thesaurusDb.GetAllWordsAndSynonyms(take, skip);
		}

		public IList<string> Find(string word)
		{
			return thesaurusDb.Find(word);
		}

		public bool InsertOrUpdate(string word, IList<string> synonyms)
		{
			return thesaurusDb.InsertOrUpdate(word, synonyms);
		}										

		public async Task<bool> SaveAsync()
		{
			if (thesaurusDb == null)
			{
				log.Write("Cannot save to an unloaded db.");
				return false;
			}

			try
			{
				// ReSharper disable once MethodHasAsyncOverload
				string wordsSerialized = thesaurusDb.Serialize();
				await using var streamWriter = new StreamWriter($"{thesaurusDb.name}.json", false);
				await streamWriter.WriteAsync(wordsSerialized);

				return true;
			}
			catch (Exception e)
			{
				log.Write("Db save failed.", e);
				return false;
			}
		}	

		public void Dispose()
		{											
			thesaurusDb = null;
		}
	}
}
