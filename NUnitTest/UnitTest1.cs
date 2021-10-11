using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Thesaurus;

namespace NUnitTest
{
	public class Tests
	{
		private ThesaurusDbFactory dbFactory { get; set; }

		[SetUp]
		public async Task Setup()
		{
			dbFactory = new ThesaurusDbFactory(new ThesaurusLogFile());
			await dbFactory.CreateAsync("test");
		}


		// As a user I want to be able to add a word with
		// some synonyms to the thesaurus, so that I can 
		// extend the content of the thesaurus
		[TestCase("Person","Human,Boy,Man,Girl,Woman")]
		public async Task AddWordWithSynonyms(string word, string synonyms)
		{
			using (dbFactory)
			{
				if (await dbFactory.LoadAsync("test"))
				{
					bool result = dbFactory.InsertOrUpdate(word, synonyms.Split(','));
					Assert.That(result);

					int nbrEntries = dbFactory.GetAllWordsAndSynonyms().Count;
					Assert.AreEqual(nbrEntries, 1);
				}
			}
		}

		// As a user I want to be able to get synonyms
		// for a word from the thesaurus, so that I can use 
		// this knowledge to impress my friends
		[TestCase("Good")]
		public async Task GetAWordsSynonyms(string word)
		{
			using (dbFactory)
			{
				if (!await dbFactory.LoadAsync("test"))
				{
					Assert.Fail("Failed to load db test");
					return;
				}

				var synonymsMock = new List<string> {"ok", "well", "nice", "tight"};
				if (dbFactory.InsertOrUpdate(word, synonymsMock))
				{
					IList<string> synonyms = dbFactory.Find(word);

					Assert.AreEqual(synonyms.Count, synonymsMock.Count);

					Assert.AreEqual(synonyms, synonymsMock);
				}
			}																							 
		}

		// As a user I want to be able to list all words
		// in the thesaurus, so that I know which words the 
		// thesaurus knows about.
		[TestCase(100, 10)]
		public async Task ListAllWords(int totalWords, int nbrSynonymsPerWord)
		{
			using (dbFactory)
			{
				if (!await dbFactory.LoadAsync("test"))
				{
					Assert.Fail("Failed to load db test");
					return;
				}

				for (var i = 0; i < totalWords; i++)
				{
					var synonymsMock = new List<string>();
					for (var j = 0; j < nbrSynonymsPerWord; j++)
					{
						synonymsMock.Add($"Synonym{j}");
					}

					dbFactory.InsertOrUpdate($"Word{i}", synonymsMock);
				}

				IDictionary<string, List<string>> result = dbFactory.GetAllWordsAndSynonyms();
				Assert.AreEqual(result.Keys.Count, totalWords);
				Assert.AreEqual(result.Values.Sum(r => r.Count), totalWords * nbrSynonymsPerWord);
			}
		}

		[TearDown]
		public void TearDown()
		{
			dbFactory.Dispose();
		}
	}
}