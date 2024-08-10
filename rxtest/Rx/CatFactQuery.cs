

using rxtest.Models;

namespace rxtest.Rx
{
    public class CatFactQuery
    {

        public async Task<CatFact> Execute()
        {
            using (HttpClient client = new HttpClient())
            {
                CatFactResponse? response = await client.GetFromJsonAsync<CatFactResponse>("https://catfact.ninja/fact");


                return response == null
                    ? throw new Exception()
                    : new CatFact()
                {
                    Content = response.Fact
                };
            }


        }

        private class CatFactResponse
        {
            public string Fact { get; set; } = string.Empty;
        }

    }

    
}
