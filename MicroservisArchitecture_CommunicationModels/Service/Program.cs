

HttpClient httpClient = new HttpClient();

HttpResponseMessage response = await httpClient.GetAsync("https://localhost:7104/api/Values");
if (response.IsSuccessStatusCode)
{
    Console.WriteLine( await response.Content.ReadAsStringAsync());
}