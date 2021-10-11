using System;
using Newtonsoft.Json;

namespace ThesaurusApi.Controllers
{
	[Serializable]
	public class ThesaurusWord
	{
		[JsonProperty(PropertyName = "word")]
		public string Word { get; set; }

		[JsonProperty(PropertyName = "synonyms")]
		public string[] Synonyms { get; set; }
	}
}