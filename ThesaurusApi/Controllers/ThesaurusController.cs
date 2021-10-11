using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Thesaurus;
using Thesaurus.Log;

namespace ThesaurusApi.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class ThesaurusController : ControllerBase
	{
		private readonly IThesaurusLog logger;
		private readonly ThesaurusDbFactory dbFactory;

		public ThesaurusController(IThesaurusLog logger, IThesaurusDbManager dbFactory)
		{
			this.logger = logger;
			this.dbFactory = (ThesaurusDbFactory) dbFactory;
		}

		[EnableCors("MyPolicy")]
		[HttpGet("getAll")]																													
		public async Task<string> GetAll()
		{																			 
			string result = await  GetInterval(-1, 0);

			HttpContext.Response.StatusCode = 200;
			//await AddResponseToContextAsync(HttpContext, result, 200);
			return result;
		}

		[EnableCors("MyPolicy")]
		[HttpGet("getInterval")]
		public async Task<string> GetInterval(int take, int skip)
		{
			using (dbFactory)
			{
				if (!await dbFactory.LoadAsync("words"))
				{
					var message = "Failed to load db words.";
					logger.Write(message);
					HttpContext?.Response.WriteAsync(message);
					return "";
				}

				IDictionary<string, List<string>> dict = dbFactory.GetAllWordsAndSynonyms(take, skip);
				IEnumerable<ThesaurusWord> thesaurusWord = dict.Select(d =>
					new ThesaurusWord()
					{
						Word = d.Key,
						Synonyms = d.Value.ToArray()
					});

				return JsonConvert.SerializeObject(thesaurusWord);
			}
		}

		[EnableCors("MyPolicy")]
		[HttpGet("get")]
		public async Task<string> Get(string word)
		{
			using (dbFactory)
			{
				if (await dbFactory.LoadAsync("words"))
				{
					IList<string> result = dbFactory.Find(word);
					return JsonConvert.SerializeObject(result);
				}

				var message = "Failed to load db words.";
				logger.Write(message);
				HttpContext?.Response.WriteAsync(message);
				return null;

			}
		}

		[EnableCors("MyPolicy")]
		[HttpPost("post")]
		public async Task<bool> Post(string word, [FromBody] string[] synonyms)
		{
			using (dbFactory)
			{
				if (!await dbFactory.LoadAsync("words"))
				{
					logger.Write("Failed to load db words.");
					return false;
				}

				if (dbFactory.InsertOrUpdate(word, synonyms))
				{
					await dbFactory.SaveAsync();
					return true;
				}

				var message = $"Failed to save synonyms {string.Concat('-', synonyms)} for word {word}.";

				HttpContext?.Response.WriteAsync(message);
				logger.Write(message);
				return false;
			}
		}
	}
}
